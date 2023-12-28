// MIT License
// 
// Copyright (c) 2017 Wooga
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
// SOFTWARE.
// 

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.SceneManagement;

// ReSharper disable RedundantTypeArgumentsOfMethod

namespace Injection
{
	public class Utils
    {
		// See: https://stackoverflow.com/questions/2742276/how-do-i-check-if-a-type-is-a-subtype-or-the-type-of-an-object
		public static bool IsSameOrSubclass(Type potentialBase, Type potentialDescendant)
		{
			return potentialDescendant.IsSubclassOf(potentialBase)
				   || potentialDescendant == potentialBase;
		}

		public static void InjectAllSceneComponents(Scene scene, Injector injector)
		{
			List<MonoBehaviour> monoBehaviours = new List<MonoBehaviour>();
			GameObject[] rootGameObjects = scene.GetRootGameObjects();
			foreach (GameObject g in rootGameObjects)
			{
				if (g.scene != scene)
				{
					// Don't inject objects from another scene.
					// For example objects marked as dont destroy on load
					continue;
				}
				GetInjectableMonoBehavioursUnderGameObject(g, monoBehaviours, true);
			}

			foreach (MonoBehaviour component in monoBehaviours)
			{
				injector.Inject(component);
			}
		}

		public static void InjectAllComponentsInGameObjectExcept(Injector injector, GameObject gameObject, MonoBehaviour excludedComponent)
		{
			List<MonoBehaviour> monoBehaviours = new();

			GetInjectableMonoBehavioursUnderGameObject(gameObject, monoBehaviours, false);

			foreach (MonoBehaviour component in monoBehaviours)
			{
				if (component == excludedComponent)
                {
					continue;
                }
				injector.Inject(component);
			}
		}

		static void GetInjectableMonoBehavioursUnderGameObject(GameObject gameObject, List<MonoBehaviour> injectableComponents, bool ignoreGoContext)
		{
			if (gameObject == null)
			{
				return;
			}

			MonoBehaviour[] monoBehaviours = gameObject.GetComponents<MonoBehaviour>();

			if (ignoreGoContext)
            {
				for (int i = 0; i < monoBehaviours.Length; i++)
				{
					MonoBehaviour monoBehaviour = monoBehaviours[i];

					// Can be null for broken component references
					if (monoBehaviour != null && IsSameOrSubclass(monoBehaviour.GetType(), typeof(GameObjectContext)))
					{
						// Need to make sure we don't inject on any MonoBehaviour's that are below a GameObjectContext
						// Since that is the responsibility of the GameObjectContext
						// BUT we do want to inject on the GameObjectContext itself
						injectableComponents.Add(monoBehaviour);
						return;
					}
				}
            }

			// Recurse first so it adds components bottom up though it shouldn't really matter much
			// because it should always inject in the dependency order
			for (int i = 0; i < gameObject.transform.childCount; i++)
			{
				Transform child = gameObject.transform.GetChild(i);

				if (child != null)
				{
					GetInjectableMonoBehavioursUnderGameObject(child.gameObject, injectableComponents, ignoreGoContext);
				}
			}

			for (int i = 0; i < monoBehaviours.Length; i++)
			{
				MonoBehaviour monoBehaviour = monoBehaviours[i];

				// Can be null for broken component references
				if (monoBehaviour != null
					/*&& IsInjectableMonoBehaviourType(monoBehaviour.GetType())*/)
				{
					injectableComponents.Add(monoBehaviour);
				}
			}
		}
	}

	public interface IInjectable
	{
	}

	[System.AttributeUsage( System.AttributeTargets.Field )]
	[MeansImplicitUse]
	public sealed class Inject : System.Attribute
	{
	}

	public class InjectorException : System.Exception
	{
		public InjectorException( string message ) : base( message )
		{
		}
	}

	[Serializable]
	public class Injector : IInjectable
	{
#region DEBUG
		private static int _lastInjectorId = 0;
		private string m_name;
#endregion

		private readonly Injector m_parentInjector;
		private readonly Dictionary<System.Type, object> m_objects = new Dictionary<System.Type, object>();

		public Injector(  string name, Injector parent = null )
		{
			m_parentInjector = parent;
			m_name = name + $"({++_lastInjectorId})";
			Bind<Injector>( this );
		}
		

		public void Bind<T>( T obj )
		{
			var type = typeof(T);

			//Debug.Log( "INJECTOR binding " + type.Name + " to " + obj.ToString() );
			m_objects[ type ] = obj;

			var interfaces = type.GetInterfaces();
			foreach (var iface in interfaces)
            {
				if (iface.Name == "IInjectable")
                {
					continue;
                }
				//Debug.Log("INJECTOR binding " + iface.Name + " to " + obj.ToString());

				m_objects[iface] = obj;
            }
		}
		public void Bind(Type type, object obj)
		{
			Debug.Assert(obj as IInjectable != null);

			//Debug.Log("INJECTOR binding " + type.Name + " to " + obj.ToString());
			m_objects[type] = obj;

			var interfaces = type.GetInterfaces();
			foreach (var iface in interfaces)
			{
				if (iface.Name == "IInjectable")
				{
					continue;
				}
				//Debug.Log("INJECTOR binding " + iface.Name + " to " + obj.ToString());

				m_objects[iface] = obj;
			}
		}

		public void PostBindings()
		{
			foreach (var objKvp in m_objects)
			{
				//Debug.Log( "Injecting bound " + objKvp.Key.Name );
				Inject( objKvp.Value );
			}
		}

		public void InjectAllComponents(GameObject gameObject)
		{
			List<MonoBehaviour> monoBehaviours = new List<MonoBehaviour>();

			GetInjectableMonoBehavioursUnderGameObject(gameObject, monoBehaviours);
			foreach (var component in monoBehaviours)
			{
				//Debug.Log($"{component.name} : {component.GetType()}");

				Inject(component);
			}
		}

		static void GetInjectableMonoBehavioursUnderGameObject(GameObject gameObject, List<MonoBehaviour> injectableComponents)
		{
			if (gameObject == null)
			{
				return;
			}

			var monoBehaviours = gameObject.GetComponents<MonoBehaviour>();

            for (int i = 0; i < monoBehaviours.Length; i++)
            {
                var monoBehaviour = monoBehaviours[i];

				// Can be null for broken component references
				if (monoBehaviour != null && Utils.IsSameOrSubclass(monoBehaviour.GetType(), typeof(GameObjectContext)))
				{
					// Need to make sure we don't inject on any MonoBehaviour's that are below a GameObjectContext
					// Since that is the responsibility of the GameObjectContext
					// BUT we do want to inject on the GameObjectContext itself
					injectableComponents.Add(monoBehaviour);
					return;
				}
			}

            // Recurse first so it adds components bottom up though it shouldn't really matter much
            // because it should always inject in the dependency order
            for (int i = 0; i < gameObject.transform.childCount; i++)
			{
				var child = gameObject.transform.GetChild(i);

				if (child != null)
				{
					GetInjectableMonoBehavioursUnderGameObject(child.gameObject, injectableComponents);
				}
			}

			for (int i = 0; i < monoBehaviours.Length; i++)
			{
				var monoBehaviour = monoBehaviours[i];

				// Can be null for broken component references
				if (monoBehaviour != null
					/*&& IsInjectableMonoBehaviourType(monoBehaviour.GetType())*/)
				{
					injectableComponents.Add(monoBehaviour);
				}
			}
		}

		public void Inject(object obj)
        {
			if (obj == null)
			{
				return;
			}

			var fields = Reflector.Reflect(obj.GetType());
			InnerInject(obj, fields);
		}

		public void InjectShallow(object obj)
		{
			//Debug.Log( "INJECTOR injecting " + obj.GetType().Name );

			if (obj == null)
            {
				return;
            }

			var fields = Reflector.ReflectShallow(obj.GetType());
			InnerInject(obj, fields);
		}

		void InnerInject(object obj, FieldInfo[] fields)
        {
			var fieldsLength = fields.Length;
			for (var i = 0; i < fieldsLength; i++)
			{
				var field = fields[i];
				// Debug.Log($"===      [{m_name}] Injecting game object's ({obj} - [{obj.GetType().Name}]) field {field.Name} of type {field.FieldType}");
				try
                {
					var value = Get( field.FieldType );
					// if (value is Injector inj)
					// {
					// 	Debug.Log($"===      [{m_name}] Injecting game object's ({obj} - [{obj.GetType().Name}]) field {field.Name} of type {field.FieldType} Inj:({inj.m_name})");
					// }
					field.SetValue( obj, value );
                }
				catch (Exception e)
                {
					throw new Exception($"Error Injecting object's ({obj}) field {field.Name} of type {field.FieldType}: {e.Message}");
                }
				//Debug.Log($"Injected [" + obj.GetType().Name + "] setting [" + field.Name + "] to " + value.ToString() );
			}
		}

		private object Get( System.Type type )
		{
			object obj = null;

			if (!m_objects.TryGetValue( type, out obj ))
			{
				if (m_parentInjector != null)
				{
					return m_parentInjector.Get( type );
				}

				throw new InjectorException( "Could not get " + type.FullName + " from injector " + m_name );
				//Debug.LogError("Could not get " + type.FullName + " from injector");
			}

			return obj;
		}

		public T GetInstance<T>()
		{
			return (T)Get( typeof(T) );
		}

		private static class Reflector
		{
			private static readonly System.Type _injectAttributeType = typeof(Inject);
			private static readonly Dictionary<System.Type, FieldInfo[]> cachedFieldInfos = new Dictionary<System.Type, FieldInfo[]>();
			private static readonly Dictionary<System.Type, FieldInfo[]> cachedShallowFieldInfos = new Dictionary<System.Type, FieldInfo[]>();
			private static readonly List<FieldInfo> _reusableList = new List<FieldInfo>( 1024 );			

			public static FieldInfo[] Reflect(System.Type type)
			{
				FieldInfo[] cachedResult;
				if (cachedFieldInfos.TryGetValue(type, out cachedResult))
				{
					return cachedResult;
				}
				
				FieldInfo[] fieldsInfo = InnerReflect(type);
				cachedFieldInfos[type] = fieldsInfo;
				return fieldsInfo;
			}

			public static FieldInfo[] ReflectShallow(System.Type type)
			{
				FieldInfo[] cachedResult;
				if (cachedShallowFieldInfos.TryGetValue(type, out cachedResult))
				{
					return cachedResult;
				}

				List<FieldInfo> fieldsInfo = InnerReflect(type).ToList();
				fieldsInfo.RemoveAll(f => f.DeclaringType != type );

				cachedShallowFieldInfos[type] = fieldsInfo.ToArray();
				return cachedShallowFieldInfos[type];
			}

			static FieldInfo[] InnerReflect(System.Type type)
			{
				Assert.AreEqual(0, _reusableList.Count, "Reusable list in Reflector was not empty!");

				BindingFlags bindingFlags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.FlattenHierarchy;

				var fields = type.GetFields(bindingFlags);

				for (var fieldIndex = 0; fieldIndex < fields.Length; fieldIndex++)
				{
					var field = fields[fieldIndex];
					var hasInjectAttribute = field.IsDefined(_injectAttributeType, inherit: false);
					if (hasInjectAttribute)
					{
						_reusableList.Add(field);
					}
				}
				var resultAsArray = _reusableList.ToArray();
				_reusableList.Clear();				
				return resultAsArray;
			}
		}
	}
}

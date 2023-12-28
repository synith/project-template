// From Warped Imagination's Video: https://youtu.be/yqneLnM8syk

using System.IO;
using UnityEditor;
using UnityEditor.Overlays;
using UnityEditor.SceneManagement;
using UnityEditor.Toolbars;
using UnityEngine;
using UnityEngine.SceneManagement;
using static System.String;

namespace Slayground.Common.Editor.Tools
{
    /// <summary>
    /// SceneSelectionOverlay
    /// </summary>
    [Overlay(typeof(SceneView), "Scene Selection")]
    public class SceneSelectionOverlay : ToolbarOverlay
    {
        private SceneSelectionOverlay() : base(SceneDropdownToggle.ID) { }

        [EditorToolbarElement(ID, typeof(SceneView))]
        private class SceneDropdownToggle : EditorToolbarDropdownToggle, IAccessContainerWindow
        {
            public EditorWindow containerWindow { get; set; }

            public const string ID = "SceneSelectionOverlay/SceneDropdownToggle";

            private SceneDropdownToggle()
            {
                text = "Scenes";
                tooltip = "Select a scene to load";
                icon = EditorGUIUtility.IconContent("d_SceneAsset Icon").image as Texture2D;

                dropdownClicked += ShowSceneMenu;
            }

            private static void ShowSceneMenu()
            {
                GenericMenu menu = new();

                Scene currentScene = SceneManager.GetActiveScene();

                // ignore these names
                string[] ignoredScenes = { "Basic", "Standard", "BuildTestScene" };

                // Use this for exclusively selecting the scenes that are in the build.
                // EditorBuildSettingsScene[] buildScenes = EditorBuildSettings.scenes;

                string[] sceneGuids = AssetDatabase.FindAssets("t:scene", null);

                foreach (string t in sceneGuids)
                {
                    string path = AssetDatabase.GUIDToAssetPath(t);

                    string name = Path.GetFileNameWithoutExtension(path);

                    bool isIgnored = false;
                    foreach (string ignoredScene in ignoredScenes)
                    {
                        if (name == ignoredScene)
                            isIgnored = true;
                    }
                    // don't list ignored scenes
                    if (isIgnored) continue;

                    menu.AddItem(new GUIContent(name), CompareOrdinal(currentScene.name, name) == 0, () => OpenScene(currentScene, path));
                }

                menu.ShowAsContext();
            }

            private static void OpenScene(Scene currentScene, string path)
            {
                if (EditorApplication.isPlaying)
                {
                    Debug.LogWarning("SceneSelectionOverlay can't switch scenes while in play mode.");
                    return;
                }

                if (currentScene.isDirty)
                {
                    if (EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
                    {
                        EditorSceneManager.OpenScene(path);
                    }
                }
                else
                {
                    EditorSceneManager.OpenScene(path);
                }
            }
        }
    }
}

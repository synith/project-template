using System.IO;
using UnityEditor;
using UnityEngine;

namespace Slayground.ProjectTemplate.Common.Editor
{
    public class PackageSetupWindow : EditorWindow
    {
        [MenuItem("Project Template/Setup Template Scenes")]
        public static void ShowWindow()
        {
            GetWindow<PackageSetupWindow>("Setup Template Scenes");
        }

        void OnGUI()
        {
            GUILayout.Label("Setup Template Scenes", EditorStyles.boldLabel);

            if (GUILayout.Button("Create Scenes"))
            {
                SetupScenes();
            }
        }

        private static void SetupScenes()
        {
            string packagePath = "Packages/ProjectTemplate/Scenes";
            string targetPath = "Assets/Scenes";

            if (!Directory.Exists(targetPath))
            {
                Directory.CreateDirectory(targetPath);
            }

            foreach (string file in Directory.GetFiles(packagePath, "*.unity"))
            {
                string fileName = Path.GetFileName(file);
                string destFile = Path.Combine(targetPath, fileName);

                if (!File.Exists(destFile))
                {
                    File.Copy(file, destFile);
                    AssetDatabase.ImportAsset(destFile);
                }
                else
                {
                    Debug.LogWarning($"Scene '{fileName}' already exists in the target directory.");
                }
            }

            AssetDatabase.Refresh();
        }
    }
}
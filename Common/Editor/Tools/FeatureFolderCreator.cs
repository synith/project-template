using System.IO;
using UnityEditor;
using UnityEngine;

namespace Slayground.Common.Editor.Tools
{
    public class FeatureFolderCreator : EditorWindow
    {
        private string _featureFolderName = "NewFeature";
        private bool _createArtFolder;
        private bool _createAudioFolder;
        private bool _createUIFolder;
        private bool _createPrefabsFolder;
        private bool _createScriptsFolder = true;

        [MenuItem("Assets/Create/Feature Folder", false, 20)]
        private static void CreateFeatureFolderMenu()
        {
            // Open the popup window
            FeatureFolderCreator window = GetWindow<FeatureFolderCreator>(true, "New Feature Folder", true);
            window.minSize = new Vector2(300, 150);   
            window.maxSize = new Vector2(300, 150);
        }

        private void OnGUI()
        {
            // Allow user to name the Feature Folder
            _featureFolderName = EditorGUILayout.TextField("Feature Folder Name", _featureFolderName);

            // Checkboxes for subfolders
            _createArtFolder = EditorGUILayout.Toggle("Art", _createArtFolder);
            _createAudioFolder = EditorGUILayout.Toggle("Audio", _createAudioFolder);
            _createUIFolder = EditorGUILayout.Toggle("UI", _createUIFolder);
            _createPrefabsFolder = EditorGUILayout.Toggle("Prefabs", _createPrefabsFolder);
            _createScriptsFolder = EditorGUILayout.Toggle("Scripts", _createScriptsFolder);

            if (!GUILayout.Button("Create")) return;
            if (IsValidFolderName(_featureFolderName))
            {
                CreateFeatureFolder();
                Close();
            }
            else
            {
                EditorUtility.DisplayDialog("Invalid Folder Name", "The folder name contains invalid characters.",
                    "OK");
            }
        }

        private static bool IsValidFolderName(string folderName)
        {
            char[] invalidChars = Path.GetInvalidFileNameChars();
            return !string.IsNullOrEmpty(folderName) && folderName.IndexOfAny(invalidChars) < 0;
        }

        private void CreateFeatureFolder()
        {
            string parentPath = AssetDatabase.GetAssetPath(Selection.activeObject);
            if (string.IsNullOrEmpty(parentPath))
            {
                parentPath = "Assets"; // Default to the Assets folder if nothing is selected
            }
            else if (!Directory.Exists(parentPath))
            {
                parentPath = Path.GetDirectoryName(parentPath); // Use the directory of the selected file
            }

            // Create the main feature folder
            string featureFolderPath = AssetDatabase.GenerateUniqueAssetPath($"{parentPath}/{_featureFolderName}");
            string uniqueFolderName = Path.GetFileName(featureFolderPath);
            AssetDatabase.CreateFolder(parentPath, uniqueFolderName);

            // Create subfolders based on the checkboxes
            CreateSubFolder(featureFolderPath, "Art", _createArtFolder);
            CreateSubFolder(featureFolderPath, "Audio", _createAudioFolder);
            CreateSubFolder(featureFolderPath, "UI", _createUIFolder);
            CreateSubFolder(featureFolderPath, "Prefabs", _createPrefabsFolder);
            CreateSubFolder(featureFolderPath, "Scripts", _createScriptsFolder);

            AssetDatabase.Refresh();
        }

        private static void CreateSubFolder(string featureFolderPath, string subFolderName, bool shouldCreate)
        {
            if (shouldCreate)
            {
                AssetDatabase.CreateFolder(featureFolderPath, subFolderName);
            }
        }
    }
}
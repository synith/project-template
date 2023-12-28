using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace Slayground.Common.Editor.Tools
{
    public class CreateFolders : EditorWindow
    {
        private const string DefaultProjectName = "PROJECT_NAME";
        private string _projectName = DefaultProjectName;
        private bool _useProjectFolder = true;

        [MenuItem("Assets/Create Default Folders")]
        private static void Init()
        {
            CreateFolders window = GetWindow<CreateFolders>(true, "Create Default Folders", true);
            window._projectName = EditorPrefs.GetString("CreateFolders_LastProjectName", DefaultProjectName);
            window._useProjectFolder = EditorPrefs.GetBool("CreateFolders_UseProjectFolder", true);
            window.Show();
        }

        private void OnGUI()
        {
            EditorGUILayout.LabelField("Configure Folder Structure");

            // Checkbox for using project-specific folder
            _useProjectFolder = EditorGUILayout.Toggle("Use Project Folder:", _useProjectFolder);

            if (_useProjectFolder)
            {
                _projectName = EditorGUILayout.TextField("Project Name:", _projectName);
                // Project name validation
                if (string.IsNullOrWhiteSpace(_projectName))
                {
                    EditorGUILayout.HelpBox("Project Name cannot be empty.", MessageType.Warning);
                    return;
                }
            }

            if (GUILayout.Button("Generate!"))
            {
                EditorPrefs.SetString("CreateFolders_LastProjectName", _projectName);
                EditorPrefs.SetBool("CreateFolders_UseProjectFolder", _useProjectFolder);
                CreateAllFolders();
                Close();
            }
        }

        private void CreateAllFolders()
        {
            string basePath = _useProjectFolder ? Path.Combine(Application.dataPath, _projectName) : Application.dataPath;
            var folderStructure = GetFolderStructure(basePath);

            foreach (var pair in folderStructure)
            {
                CreateFolderIfNotExists(pair.Key);

                if (pair.Value != null)
                {
                    foreach (var subFolder in pair.Value)
                    {
                        CreateFolderIfNotExists(Path.Combine(pair.Key, subFolder));
                    }
                }
            }

            AssetDatabase.Refresh();
        }

        private static Dictionary<string, List<string>> GetFolderStructure(string rootPath)
        {
            return new Dictionary<string, List<string>>
            {
                { Path.Combine(rootPath, "Common/Art"), new List<string> { "Animations", "Materials", "Models", "Sprites", "Textures" } },
                { Path.Combine(rootPath, "Common/Audio"), new List<string> { "Music", "SFX" } },
                { Path.Combine(rootPath, "Common"), new List<string> { "Data", "Editor", "Prefabs", "Resources", "Scripts", "UI" } },
                { Path.Combine(rootPath, "Features"), null },
                { Path.Combine(rootPath, "Plugins"), null },
                { Path.Combine(rootPath, "Scenes"), null },
                { Path.Combine(rootPath, "Settings"), null }
            };
        }

        private static void CreateFolderIfNotExists(string path)
        {
            if (Directory.Exists(path)) return;
            
            try
            {
                Directory.CreateDirectory(path);
                Debug.Log($"Created folder: {path}");
            }
            catch (IOException e)
            {
                Debug.LogError($"Failed to create folder: {path}. Error: {e.Message}");
            }
        }
    }
}

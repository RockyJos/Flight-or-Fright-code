#if UNITY_EDITOR


using System;
using UnityEditor;
using UnityEngine;
using System.IO;
using System.Linq;
using UnityEditor.SceneManagement;
using UnityEditor.Callbacks;
using Object = UnityEngine.Object;

[InitializeOnLoad]
public class FolderSetupUtility
{
    private const string PendingPrefabKey = "PendingPrefabPath";

    static FolderSetupUtility()
    {
        EditorApplication.delayCall += UserFileCheck;
    }

    private static void UserFileCheck()
    {
        string resourcesPath = Path.Combine(Application.dataPath, "Resources");
        string corePath = Path.Combine(Application.dataPath, "Core");

        if (!Directory.Exists(resourcesPath) || !Directory.Exists(corePath))
        {
            EditorUtility.DisplayDialog(
                "Project Core or Resources Missing",
                "Project core or resources not found. Please re-download the starter project from Moodle.",
                "OK"
            );
            return;
        }

        string[] subfolders = Directory.GetDirectories(resourcesPath);
        bool found = false;
        foreach (string folder in subfolders)
        {
            string folderName = Path.GetFileName(folder);
            if (folderName == "Example") continue;

            string[] asmdefs = Directory.GetFiles(folder, "*.asmdef", SearchOption.AllDirectories);
            if (asmdefs.Length > 0)
            {
                found = true;
                break;
            }
        }

        if (!found)
        {
            CollectUserID("To use this project template, please enter your student ID number:");
        }
    }

    private static void CollectUserID(string message)
    {
        TextInputDialog.Show(
            "Game A Week - Game Pack: Setup Wizard",
            message,
            id =>
            {
                CreateUserFolder(id);
            }
        );
    }

    private static void CreateUserFolder(string id)
    {
        string userFolderPath = Path.Combine(Application.dataPath, "Resources", id);

        if (!Directory.Exists(userFolderPath))
        {
            Directory.CreateDirectory(userFolderPath);

            string asmdefPath = Path.Combine(userFolderPath, id + ".asmdef");
            string asmdefContent =
                "{\n" +
                "  \"name\": \"" + id + "\",\n" +
                "  \"references\": [\n" +
                "    \"Core\",\n" +
                "    \"Unity.TextMeshPro\",\n" +
                "    \"UnityEngine.UI\",\n" +
                "    \"Unity.InputSystem\"\n" +
                "  ],\n" +
                "  \"autoReferenced\": true,\n" +
                "  \"allowUnsafeCode\": false\n" +
                "}";
            File.WriteAllText(asmdefPath, asmdefContent);

            string assetsFolderPath = Path.Combine(userFolderPath, "Assets");
            Directory.CreateDirectory(assetsFolderPath);

            string gamemanagerTemplate = "Assets/Core/TemplateScripts/GameManager.cs";
            string gamemanagerDest = "Assets/Resources/" + id + "/Assets/GameManager.cs";

            if (AssetDatabase.LoadAssetAtPath<Object>(gamemanagerTemplate) != null)
            {
                AssetDatabase.CopyAsset(gamemanagerTemplate, gamemanagerDest);
            }

            string sourcePrefab = "Assets/Core/Template.prefab";
            string destPrefab = "Assets/Resources/" + id + "/Assets/" + id + "_Game_1.prefab";
            
            if (AssetDatabase.LoadAssetAtPath<Object>(sourcePrefab) != null)
            {
                AssetDatabase.CopyAsset(sourcePrefab, destPrefab);
                
                EditorPrefs.SetString(PendingPrefabKey, destPrefab);
            }

            AssetDatabase.Refresh();
        }
    }

    [DidReloadScripts]
    private static void OnScriptsReloaded()
    {
        if (!EditorPrefs.HasKey(PendingPrefabKey)) return;

        string prefabPath = EditorPrefs.GetString(PendingPrefabKey);
        EditorPrefs.DeleteKey(PendingPrefabKey);

        GameObject prefabRoot = PrefabUtility.LoadPrefabContents(prefabPath);

        if (prefabRoot != null)
        {
            string[] pathParts = prefabPath.Split('/');
            string studentId = pathParts[2];

            var studentAssembly = AppDomain.CurrentDomain.GetAssemblies()
                .FirstOrDefault(a => a.GetName().Name == studentId);

            if (studentAssembly != null)
            {
                Type managerType = studentAssembly.GetType("GameManager");

                if (managerType != null)
                {
                    if (prefabRoot.GetComponent(managerType) == null)
                    {
                        prefabRoot.AddComponent(managerType);
                    }

                }
                else
                {
                    Debug.LogError($"Could not find GameManager class in assembly {studentId}");
                }
            }

            Game gameScript = prefabRoot.GetComponent<Game>();
            if (gameScript != null)
            {
                gameScript.ResetGameInfo();
            }

            PrefabUtility.SaveAsPrefabAsset(prefabRoot, prefabPath);
            PrefabUtility.UnloadPrefabContents(prefabRoot);

            AssetDatabase.Refresh();

            Selection.activeObject = AssetDatabase.LoadAssetAtPath<Object>(prefabPath);
            PrefabStageUtility.OpenPrefab(prefabPath);

            Debug.Log("Successfully attached GameManager and finalised prefab.");
        }

        
    }
}
#endif
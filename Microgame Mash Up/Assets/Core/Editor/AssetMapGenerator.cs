#if UNITY_EDITOR

using UnityEngine;
using UnityEditor;
using System.IO;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Text;

[InitializeOnLoad]
public class AssetMapGenerator
{
    private const string MapPath = "Assets/Core/AssetMap.json";
    private const string ExclusionPattern = @"^Assets/Resources/\d{8}($|/)";

    static AssetMapGenerator()
    {
        EditorApplication.delayCall += ValidateAssets;
        EditorApplication.projectChanged += ValidateAssets;
        
        EditorApplication.playModeStateChanged += (state) =>
        {
            if (state == PlayModeStateChange.EnteredEditMode)
            {
                ValidateAssets();
            }
        };
    }

    //[MenuItem("Tools/Generate Asset Map")]
    public static void GenerateMap()
    {
        List<string> paths = GetFilteredPaths();
        AssetListData data = new AssetListData { paths = paths };
        string json = JsonUtility.ToJson(data, true);
        
        string directory = Path.GetDirectoryName(MapPath);
        if (!Directory.Exists(directory)) 
        {
            Directory.CreateDirectory(directory);
        }
        
        File.WriteAllText(MapPath, json);
        AssetDatabase.Refresh();
        Debug.Log($"[AssetMap] Map generated with {paths.Count} entries.");
    }

    private static void ValidateAssets()
    {
        if (!File.Exists(MapPath)) return;

        string json = File.ReadAllText(MapPath);
        AssetListData savedData = JsonUtility.FromJson<AssetListData>(json);
        
        if (savedData == null || savedData.paths == null) return;

        HashSet<string> savedPaths = new HashSet<string>(savedData.paths);
        List<string> currentPaths = GetFilteredPaths();
        List<string> unrecognizedAssets = new List<string>();

        foreach (string path in currentPaths)
        {
            if (!savedPaths.Contains(path))
            {
                unrecognizedAssets.Add(path);
            }
        }

        if (unrecognizedAssets.Count > 0)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("<color=#f01a1a><b>Unrecognised Asset(s)</b></color>:");
            
            foreach (string asset in unrecognizedAssets)
            {
                sb.AppendLine("- <color=yellow>"+asset+"</color>");
            }

            sb.Append("\nPlease ensure all custom assets are placed inside your user folder in Assets/Resources.\n");
            Debug.LogError(sb.ToString());
        }
    }

    private static List<string> GetFilteredPaths()
    {
        string[] allPaths = AssetDatabase.GetAllAssetPaths();
        List<string> filtered = new List<string>();

        foreach (string path in allPaths)
        {
            if (!path.StartsWith("Assets/")) continue;
            if (path == MapPath || path.EndsWith(".meta")) continue;
            if (Regex.IsMatch(path, ExclusionPattern)) continue;

            filtered.Add(path);
        }

        return filtered;
    }

    [System.Serializable]
    private class AssetListData
    {
        public List<string> paths;
    }
}
#endif
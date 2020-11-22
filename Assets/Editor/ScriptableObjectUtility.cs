// Copied with minimal modificationfrom:
// http://wiki.unity3d.com/index.php?title=CreateScriptableObjectAsset
// For creating scriptable objects store as .asset files.
// This is also helpful:
// http://www.jacobpennock.com/Blog/unity-pro-tip-use-custom-made-assets-as-configuration-files/
// 
// Unity Learn has a tutorial: https://learn.unity.com/tutorial/introduction-to-scriptable-objects#

using UnityEngine;
using UnityEditor;
using System.IO;

public static class ScriptableObjectUtility
{
    /// <summary>
    //	This makes it easy to create, name and place unique new ScriptableObject asset files.
    /// </summary>
    public static void CreateAsset<T>() where T : ScriptableObject
    {
        T asset = ScriptableObject.CreateInstance<T>();

        string path = AssetDatabase.GetAssetPath(Selection.activeObject);
        if (path == "")
        {
            path = "Assets/Resources/";
        }
        else if (Path.GetExtension(path) != "")
        {
            path = path.Replace(Path.GetFileName(AssetDatabase.GetAssetPath(Selection.activeObject)), "");
        }

        string assetPathAndName = AssetDatabase.GenerateUniqueAssetPath(path + typeof(T).ToString() + ".asset");

        AssetDatabase.CreateAsset(asset, assetPathAndName);

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        EditorUtility.FocusProjectWindow();
        Selection.activeObject = asset;
    }
}
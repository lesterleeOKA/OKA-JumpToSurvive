using System;
using UnityEditor;
using UnityEngine;

public class CreateAssetBundles
{
    [MenuItem("Assets/Create Assets Bundles")]
    private static void BuildAllAssetBundles()
    {
        string assetBundleDirectoryPath = Application.dataPath + "/AssetsBundles";

        Debug.Log("created to " + assetBundleDirectoryPath);
        try
        {
            //BuildPipeline.BuildAssetBundles(assetBundleDirectoryPath, BuildAssetBundleOptions.None, EditorUserBuildSettings.activeBuildTarget);
            BuildPipeline.BuildAssetBundles(assetBundleDirectoryPath, BuildAssetBundleOptions.ChunkBasedCompression, BuildTarget.WebGL);
        }
        catch (Exception e)
        {
            Debug.LogWarning(e);
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

//Generate AssetBundles and Store them in AssetBundles Folder
public class AssetBundleCreator : MonoBehaviour
{
    [MenuItem("Assets/Build AssetBundles")]
    static void BuildAllAssetBundles()
    {
        
           // BuildPipeline.BuildAssetBundles("Assets/AssetBundles", BuildAssetBundleOptions.None, BuildTarget.StandaloneWindows);

            //For Android Asset Bundles
            BuildPipeline.BuildAssetBundles("Assets/AssetBundles", BuildAssetBundleOptions.None, BuildTarget.Android);

    }
}

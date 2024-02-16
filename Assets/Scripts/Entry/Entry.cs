using System;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using YooAsset;

public static class Entry {
    private static void Start() {
        Debug.Log("Entry Start!");
        YooAssets.LoadSceneAsync("Entry");
    }
}
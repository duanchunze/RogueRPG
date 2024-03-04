using UnityEngine;
using YooAsset;

public static class Entry {
    private static void Start() {
        Debug.Log("Entry Start!");
        YooAssets.LoadSceneAsync("Entry");
    }
}
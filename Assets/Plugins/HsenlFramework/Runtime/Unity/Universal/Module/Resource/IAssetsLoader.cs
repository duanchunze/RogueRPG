namespace Hsenl {
    public interface IAssetsLoader {
        UnityEngine.Object GetAssetSync(string bundleName, string assetName);
        ETTask<UnityEngine.Object> GetAssetAsync(string bundleName, string assetName);
    }
}
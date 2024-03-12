using System;
using Hsenl;
using YooAsset.Editor;

public class AddressByLocalPath_BundlesLastOneFolder : IAddressRule {
    private const string _KeyFolderName = "Bundles";
    
    public string GetAssetAddress(AddressRuleData data) {
        var relativePath = data.AssetPath.Substring(0, data.AssetPath.IndexOf(_KeyFolderName, StringComparison.Ordinal) + _KeyFolderName.Length);
        var path = data.AssetPath.Replace(relativePath, "");

        path = path.Substring(0, path.LastIndexOf('.'));
        
        if (path.StartsWith("/"))
            path = path.Substring(1);
        
        var index = path.IndexOf("/", StringComparison.Ordinal);
        if (index != -1) {
            path = path.Substring(index + 1);
        }
        
        path = path.Replace("/", "_");
        return path;
    }
}
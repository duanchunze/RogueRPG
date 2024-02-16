using YooAsset.Editor;

public class AssetFilterRule : IFilterRule {
    public bool IsCollectAsset(FilterRuleData data) {
        return data.AssetPath.EndsWith(".asset");
    }
}
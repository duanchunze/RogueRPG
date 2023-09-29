namespace Hsenl {
    public static class Constant {
        public const string UIBundleName = "ui.unity3d";
        public const string InputBundleName = "input.unity3d";
        public const string ConfigBundleName = "config.unity3d";
        public const string AbilityBundleName = "ability.unity3d";
        public const string StatusBundleName = "status.unity3d";
        public const string AppearanceBundleName = "appearance.unity3d";
        public const string AudioBundleName = "audio.unity3d";
        public const string FxBundleName = "fx.unity3d";
        public const string ColliderBundleName = "collider.unity3d";
        public const string BoltBundleName = "bolt.unity3d";
        public const string WarningBoardBundleName = "warningboard.unity3d";
        public const string PickableModelBundleName = "pickable.unity3d";
        public const string AnimControlBundleName = "animcontrol.unity3d";
        public const string AnimClipBundleName = "animclip.unity3d";

        public const int BodyLayer = 6;
        public const int BodyTriggerLayer = 7;
        public const int PickableLayer = 8;
        public const int PickerLayer = 9;
        
        public const int BodyLayerIncludeMask = 1 << 6;
        public const int BodyLayerExcludeMask = ~BodyLayerIncludeMask;
        public const int BodyTriggerLayerIncludeMask = 1 << 7;
        public const int BodyTriggerLayerExcludeMask = ~BodyTriggerLayerIncludeMask;
        public const int PickableLayerIncludeMask = 1 << 8;
        public const int PickableLayerExcludeMask = ~PickableLayerIncludeMask;
        public const int PickerLayerIncludeMask = 1 << 9;
        public const int PickerLayerExcludeMask = ~PickerLayerIncludeMask;
    }
}
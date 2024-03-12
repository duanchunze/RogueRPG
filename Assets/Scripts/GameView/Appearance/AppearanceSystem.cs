using System;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace Hsenl.View {
    public static class AppearanceSystem {
        private static AppearanceMiddleArgStream _middleArgStream = new();

        public static AppearanceMiddleArgStream MiddleArgStream => _middleArgStream;

        // 加载人物模型
        public static GameObject LoadModelActor(string modelName, AppearanceMiddleArgStream middleArgStream = null) {
            return Load<GameObject>(modelName, AppearanceConst.Model.Actor.Tag, AppearanceConst.Model.Actor.Root, middleArgStream);
        }

        // 加载Bolt模型
        public static GameObject LoadModelBolt(string modelName, AppearanceMiddleArgStream middleArgStream = null) {
            return Load<GameObject>(modelName, AppearanceConst.Model.Bolt.Tag, AppearanceConst.Model.Bolt.Root, middleArgStream);
        }

        // 加载动画片段
        public static AnimationClip LoadAnimationClip(string modelName, string clipName, AppearanceMiddleArgStream middleArgStream = null) {
            var assetFinder = OptimalAssetFinder.Create(clipName, AppearanceConst.Anim.Clip.Tag);
            assetFinder.Append(AppearanceConst.Anim.Clip.Root);
            assetFinder.Append(modelName);
            AppeadMiddleArgs(assetFinder, middleArgStream);

            return Resource.GetOptimalAsset(assetFinder) as AnimationClip;
        }

        // 加载动画控制器
        public static RuntimeAnimatorController LoadRuntimeAnimatorController(string controlName, AppearanceMiddleArgStream middleArgStream = null) {
            var assetFinder = OptimalAssetFinder.Create(controlName, AppearanceConst.Anim.Controller.Tag);
            assetFinder.Append(AppearanceConst.Anim.Controller.Root);
            AppeadMiddleArgs(assetFinder, middleArgStream);

            return Resource.GetOptimalAsset(assetFinder) as RuntimeAnimatorController;
        }

        // 加载警示器
        public static GameObject LoadWarningBoard(string warnBoardName, AppearanceMiddleArgStream middleArgStream = null) {
            return Load<GameObject>(warnBoardName, AppearanceConst.WarningBoard.Tag, AppearanceConst.WarningBoard.Root, middleArgStream);
        }

        // 加载音效
        public static AudioClip LoadSoundClip(string audioClipName, AppearanceMiddleArgStream middleArgStream = null) {
            return Load<AudioClip>(audioClipName, AppearanceConst.Audio.Sound.Tag, AppearanceConst.Audio.Sound.Root, middleArgStream);
        }

        // 加载特效
        public static GameObject LoadFx(string fxName, AppearanceMiddleArgStream middleArgStream = null) {
            return Load<GameObject>(fxName, AppearanceConst.Fx.Tag, AppearanceConst.Fx.Root, middleArgStream);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static T Load<T>(string assetName, string tag, string root, AppearanceMiddleArgStream middleArgStream) where T : class {
            var assetFinder = OptimalAssetFinder.Create(assetName, tag);
            assetFinder.Append(root);
            AppeadMiddleArgs(assetFinder, middleArgStream);

            return Resource.GetOptimalAsset(assetFinder) as T;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void AppeadMiddleArgs(OptimalAssetFinder assetFinder, AppearanceMiddleArgStream middleArgStream = null) {
            if (middleArgStream != null) {
                for (int i = 0, len = middleArgStream.Position; i < len; i++) {
                    if (string.IsNullOrEmpty(middleArgStream[i]))
                        continue;

                    assetFinder.Append(middleArgStream[i]);
                }
            }
        }
    }
}
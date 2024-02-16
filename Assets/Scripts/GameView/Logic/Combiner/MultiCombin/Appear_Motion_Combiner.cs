using System;
using System.Text;
using UnityEngine;
using YooAsset;

namespace Hsenl.MultiCombiner {
    public class Appear_Motion_Combiner : MultiCombiner<Appearance, Motion> {
        private StringBuilder _stringBuilder = new();

        protected override void OnCombin(Appearance arg1, Motion arg2) {
            arg1.onModelLoaded += this.EnqueueAction<Action<GameObject>>(model => {
                arg2.animation = model.GetComponentInChildren<Animation>();
                if (arg2.animation != null) {
                    arg2.useLegacy = true;
                    var clipHandles = YooAssets.LoadAllAssetsSync<AnimationClip>($"{arg1.modelName}_idle");
                    foreach (var o in clipHandles.AllAssetObjects) {
                        if (o is not AnimationClip clip)
                            continue;

                        arg2.animation.AddClip((AnimationClip)clip, clip.name);
                    }

                    return;
                }

                arg2.animator = model.GetComponentInChildren<Animator>();
                if (arg2.animator != null) {
                    arg2.useLegacy = false;
                    var animControl = YooAssets.LoadAssetSync($"{arg1.modelName}_{arg1.modelName}")?.AssetObject;
                    arg2.animator.runtimeAnimatorController = animControl as RuntimeAnimatorController;
                    // kulou_zs_kulou_zs
                    // kulou_zs _kulou_zs
                }
            });
        }

        protected override void OnDecombin(Appearance arg1, Motion arg2) {
            arg1.onModelLoaded -= this.DequeueAction<Action<GameObject>>();
        }
    }
}
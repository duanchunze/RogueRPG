using System;
using System.Text;
using UnityEditor.Animations;
using UnityEngine;

namespace Hsenl.MultiCombiner {
    public class AppearMotionCombiner : MultiCombiner<Appearance, Motion> {
        private StringBuilder _stringBuilder = new();
        
        protected override void OnCombin(Appearance arg1, Motion arg2) {
            arg1.onModelLoaded += this.EnqueueAction<Action<GameObject>>(model => {
                arg2.animation = model.GetComponentInChildren<Animation>();
                if (arg2.animation != null) {
                    arg2.useLegacy = true;
                    this._stringBuilder.Clear();
                    this._stringBuilder.Append("animclip/");
                    this._stringBuilder.Append(arg1.assetName);
                    this._stringBuilder.Append(".unity3d");
                    var clips = ResourcesHelper.GetAssets(this._stringBuilder.ToString());
                    foreach (var clip in clips) {
                        arg2.animation.AddClip((AnimationClip)clip, clip.name);
                    }

                    return;
                }

                arg2.animator = model.GetComponentInChildren<Animator>();
                // anima control 和模型同名
                var animControl = ResourcesHelper.GetAsset<AnimatorController>(Constant.AnimControlBundleName, arg1.assetName);
                arg2.animator.runtimeAnimatorController = animControl;
            });
        }

        protected override void OnDecombin(Appearance arg1, Motion arg2) {
            arg1.onModelLoaded -= this.DequeueAction<Action<GameObject>>();
        }
    }
}
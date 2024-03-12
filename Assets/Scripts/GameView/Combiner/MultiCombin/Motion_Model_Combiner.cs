using System;
using UnityEngine;

namespace Hsenl.View.MultiCombiner {
    public class Motion_Model_Combiner : MultiCombiner<Motion, Model> {
        protected override void OnCombin(Motion arg1, Model arg2) {
            LoadAnim(arg2.ModelObj);
            arg2.OnModelChanged += this.EnqueueAction<Action<GameObject>>(LoadAnim);
            arg1.getAnimClipInvoke += this.EnqueueAction<Func<string, AnimationClip>>(LoadAnimClip);

            return;

            void LoadAnim(GameObject model) {
                // 如果模型发生改变了, 则动画需要全部清空
                arg1.Clear();
                
                if (model == null)
                    return;

                arg1.animation = model.GetComponentInChildren<Animation>();
                arg1.animator = model.GetComponentInChildren<Animator>();
                if (arg1.animator == null) {
                    // 这里只处理animator, 对于animation, 因为animation是分散的, 所以在实际使用的时候, 动态加载具体的clip, 如果后续有性能问题, 再针对优化
                    return;
                }

                var runtimeAnimatorController = AppearanceSystem.LoadRuntimeAnimatorController(model.name);
                arg1.animator.runtimeAnimatorController = runtimeAnimatorController;
            }

            AnimationClip LoadAnimClip(string clipName) {
                if (arg2.ModelObj == null)
                    return null;

                var clip = AppearanceSystem.LoadAnimationClip(arg2.ModelObj.name, clipName);

                return clip;
            }
        }

        protected override void OnDecombin(Motion arg1, Model arg2) {
            arg2.OnModelChanged -= this.DequeueAction<Action<GameObject>>();
            arg1.getAnimClipInvoke -= this.DequeueAction<Func<string, AnimationClip>>();
        }
    }
}
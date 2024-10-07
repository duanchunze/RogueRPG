using System;
using Hsenl.status;
#if UNITY_EDITOR
using Sirenix.OdinInspector;
#endif

namespace Hsenl {
    [Serializable]
    public class Status : Bodied {
        public int configId;
        public StatusConfig Config => Tables.Instance.TbStatusConfig.GetById(this.configId);

        public Action beginInvoke; // 开始
        public Action finishInvoke; // 结束
        public Func<bool> isEnterInvoke; // 是否在进入的状态
        public Action onBegin;
        public Action<float> onUpdate;
        public Action<StatusFinishDetails> onFinish;

        public bool IsEnter => this.isEnterInvoke.Invoke();

        public Bodied inflictor; // 施加的那个人

        public void Begin() {
            try {
                this.beginInvoke?.Invoke();
            }
            catch (Exception e) {
                Log.Error(e);
            }
        }

        public void Finish() {
            try {
                this.finishInvoke?.Invoke();
            }
            catch (Exception e) {
                Log.Error(e);
            }
        }

        public void OnBegin() {
            try {
                this.onBegin?.Invoke();
            }
            catch (Exception e) {
                Log.Error(e);
            }
        }

        public void OnUpdate(float deltaTime) {
            try {
                this.onUpdate?.Invoke(deltaTime);
            }
            catch (Exception e) {
                Log.Error(e);
            }
        }

        public void OnFinish(StatusFinishDetails finishDetails) {
            try {
                this.onFinish?.Invoke(finishDetails);
            }
            catch (Exception e) {
                Log.Error(e);
            }
        }
    }
}
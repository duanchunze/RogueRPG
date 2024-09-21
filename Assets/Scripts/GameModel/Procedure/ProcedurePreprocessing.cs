using UnityEngine;

namespace Hsenl {
    public class ProcedurePreprocessing : AProcedureState {
        protected override async void OnEnter(IFsm fsm, IFsmState prev) {
            // 注册控制码映射
            InputMaptable.RegisterControlCode(InputCode.WASD, ControlCode.MoveOfDirection);
            InputMaptable.RegisterControlCode(InputCode.Alpha1, ControlCode.Ability1);
            InputMaptable.RegisterControlCode(InputCode.Alpha2, ControlCode.Ability2);
            InputMaptable.RegisterControlCode(InputCode.Alpha3, ControlCode.Ability3);
            InputMaptable.RegisterControlCode(InputCode.Alpha4, ControlCode.Ability4);
            InputMaptable.RegisterControlCode(InputCode.Alpha5, ControlCode.Ability5);
            InputMaptable.RegisterControlCode(InputCode.Alpha6, ControlCode.Ability6);

            // 初始化数值系统
            Numerator.InitNumerator(NumericLayer.Max - 1);

            // 加载初始场景
            SceneManager.GetOrLoadDontDestroyScene();
            await SceneManager.LoadSceneWithUnity("GameMain", LoadSceneMode.Single);

            fsm.ChangeState<ProcedureMainInterface_Combat>();
        }

        protected override void OnLeave(IFsm fsm, IFsmState next) { }
    }
}
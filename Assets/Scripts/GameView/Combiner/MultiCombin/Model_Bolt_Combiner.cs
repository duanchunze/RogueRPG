namespace Hsenl.View.MultiCombiner {
    public class Model_Bolt_Combiner : MultiCombiner<Model, Bolt> {
        protected override void OnCombin(Model arg1, Bolt arg2) {
            LoadModel(arg2.Config.ModelName);

            return;

            void LoadModel(string modelName) {
                var prefab = AppearanceSystem.LoadModelBolt(modelName);
                if (prefab == null)
                    return;

                var obj = UnityEngine.Object.Instantiate(prefab);
                obj.name = prefab.name;
                arg1.SetModel(obj);
            }
        }

        protected override void OnDecombin(Model arg1, Bolt arg2) {
            arg1.SetModel(null);
        }
    }
}
namespace Hsenl.View.MultiCombiner {
    public class Model_Actor_Combiner : MultiCombiner<Model, Actor> {
        protected override void OnCombin(Model arg1, Actor arg2) {
            LoadModel(arg2.Config.ModelName);
            return;
            void LoadModel(string modelName) {
                var modelPrefab = AppearanceSystem.LoadModelActor(modelName);
                if (modelPrefab == null)
                    return;

                var modelObj = UnityEngine.Object.Instantiate(modelPrefab);
                modelObj.name = modelPrefab.name;
                arg1.SetModel(modelObj);
            }
        }

        protected override void OnDecombin(Model arg1, Actor arg2) {
            arg1.SetModel(null);
        }
    }
}
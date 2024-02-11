namespace Hsenl {
    public static class PhysicExtension {
        public static void SetUsage(this Rigidbody self, GameColliderPurpose colliderPurpose) {
            if ((colliderPurpose & GameColliderPurpose.Receptor) == GameColliderPurpose.Receptor) {
                self.SetLayer(Constant.ReceptorLayer);
                self.SetIncludeLayers(1 << Constant.DetectionLayer);
                self.SetExcludeLayers(~(1 << Constant.DetectionLayer));
            }

            if ((colliderPurpose & GameColliderPurpose.Detection) == GameColliderPurpose.Detection) {
                self.SetLayer(Constant.DetectionLayer);
                self.SetIncludeLayers(1 << Constant.ReceptorLayer);
                self.SetExcludeLayers(~(1 << Constant.ReceptorLayer));
            }

            if ((colliderPurpose & GameColliderPurpose.Pickable) == GameColliderPurpose.Pickable) {
                self.SetLayer(Constant.PickableLayer);
                self.SetIncludeLayers(1 << Constant.PickerLayer);
                self.SetExcludeLayers(~(1 << Constant.PickerLayer));
            }

            if ((colliderPurpose & GameColliderPurpose.Picker) == GameColliderPurpose.Picker) {
                self.SetLayer(Constant.PickerLayer);
                self.SetIncludeLayers(1 << Constant.PickableLayer);
                self.SetExcludeLayers(~(1 << Constant.PickableLayer));
            }
        }

        public static void SetUsage(this Collider self, GameColliderPurpose colliderPurpose) {
            if ((colliderPurpose & GameColliderPurpose.Receptor) == GameColliderPurpose.Receptor) {
                self.SetLayer(Constant.ReceptorLayer);
                self.SetIncludeLayers(1 << Constant.DetectionLayer);
                self.SetExcludeLayers(~(1 << Constant.DetectionLayer));
            }

            if ((colliderPurpose & GameColliderPurpose.Detection) == GameColliderPurpose.Detection) {
                self.SetLayer(Constant.DetectionLayer);
                self.SetIncludeLayers(1 << Constant.ReceptorLayer);
                self.SetExcludeLayers(~(1 << Constant.ReceptorLayer));
            }

            if ((colliderPurpose & GameColliderPurpose.Pickable) == GameColliderPurpose.Pickable) {
                self.SetLayer(Constant.PickableLayer);
                self.SetIncludeLayers(1 << Constant.PickerLayer);
                self.SetExcludeLayers(~(1 << Constant.PickerLayer));
            }

            if ((colliderPurpose & GameColliderPurpose.Picker) == GameColliderPurpose.Picker) {
                self.SetLayer(Constant.PickerLayer);
                self.SetIncludeLayers(1 << Constant.PickableLayer);
                self.SetExcludeLayers(~(1 << Constant.PickableLayer));
            }
        }
        
        public static void SetUsage(this CollisionEventListener self, GameColliderPurpose colliderPurpose) {
            if ((colliderPurpose & GameColliderPurpose.Receptor) == GameColliderPurpose.Receptor) {
                self.SetLayer(Constant.ReceptorLayer);
                self.SetIncludeLayers(1 << Constant.DetectionLayer);
                self.SetExcludeLayers(~(1 << Constant.DetectionLayer));
            }

            if ((colliderPurpose & GameColliderPurpose.Detection) == GameColliderPurpose.Detection) {
                self.SetLayer(Constant.DetectionLayer);
                self.SetIncludeLayers(1 << Constant.ReceptorLayer);
                self.SetExcludeLayers(~(1 << Constant.ReceptorLayer));
            }

            if ((colliderPurpose & GameColliderPurpose.Pickable) == GameColliderPurpose.Pickable) {
                self.SetLayer(Constant.PickableLayer);
                self.SetIncludeLayers(1 << Constant.PickerLayer);
                self.SetExcludeLayers(~(1 << Constant.PickerLayer));
            }

            if ((colliderPurpose & GameColliderPurpose.Picker) == GameColliderPurpose.Picker) {
                self.SetLayer(Constant.PickerLayer);
                self.SetIncludeLayers(1 << Constant.PickableLayer);
                self.SetExcludeLayers(~(1 << Constant.PickableLayer));
            }
        }
    }
}
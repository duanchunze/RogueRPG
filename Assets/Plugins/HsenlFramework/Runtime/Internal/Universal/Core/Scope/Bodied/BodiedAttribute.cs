using System;

namespace Hsenl {
    [AttributeUsage(AttributeTargets.Class)]
    public class BodiedAttribute : BaseAttribute {
        public BodiedStatus defaultStatus;

        public BodiedAttribute(BodiedStatus defaultStatus = BodiedStatus.Dependent) {
            this.defaultStatus = defaultStatus;
        }
    }
}
using System;

namespace Hsenl {
    [AttributeUsage(AttributeTargets.Class, Inherited = true, AllowMultiple = false)]
    public class ComponentOptionsAttribute : BaseAttribute {
        public ComponentMode ComponentMode { get; set; }
    }
}
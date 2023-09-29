namespace Hsenl {
    public static partial class Extension {
        public static bool IsInvalid(this Component self) {
            if (self == null)
                return true;

            return self.IsDisposed;
        }

        public static bool IsValid(this Component self) {
            if (self == null)
                return false;

            return !self.IsDisposed;
        }
        
        public static bool IsInvalid(this Entity self) {
            if (self == null)
                return true;

            return self.IsDisposed;
        }

        public static bool IsValid(this Entity self) {
            if (self == null)
                return false;

            return !self.IsDisposed;
        }
    }
}
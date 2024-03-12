using System;

namespace Hsenl.View {
    public class AppearanceMiddleArgStream {
        private string[] _middleArgs = Array.Empty<string>();
        private int _position;
        private int _capacity;

        public int Position {
            get => this._position;
            set {
                if (value > this._middleArgs.Length - 1)
                    throw new ArgumentOutOfRangeException(nameof(value));
                
                if (value < 0)
                    throw new ArgumentOutOfRangeException(nameof(value));

                this._position = value;
            }
        }

        public int Capacity {
            get => this._capacity;
            set {
                if (value < 0)
                    throw new Exception($"capacity can not less be 0 '{value}'");

                if (this._capacity == value)
                    return;

                string[] strs = new string[value];
                Buffer.BlockCopy(this._middleArgs, 0, strs, 0, this._middleArgs.Length);
                this._middleArgs = strs;
                this._capacity = value;
            }
        }

        public string this[int index] {
            get => this._middleArgs[index];
            set => this._middleArgs[index] = value;
        }

        public void Appead(string arg) {
            this.EnsureCapacity(this.Position + 1);
            this._middleArgs[this.Position++] = arg;
        }

        public void Reset() {
            this.Position = 0;
        }

        private void EnsureCapacity(int value) {
            if (value > this._middleArgs.Length) {
                this.Capacity = value;
            }
        }

        public int CalcHashCode() {
            if (this._middleArgs == null)
                return 0;

            if (this._middleArgs.Length == 0)
                return 0;

            var hashcode = this._middleArgs[0].GetHashCode();
            for (int i = 1; i < this.Position; i++) {
                hashcode = HashCode.Combine(hashcode, this._middleArgs[i].GetHashCode());
            }

            return hashcode;
        }
    }
}
using System;

namespace Hsenl.Network {
    public abstract class EncryptionPlug : IPlug, IAfterMessageWrited, IAfterMessageReaded {
        protected abstract void Encrypt(ref Memory<byte> data);
        protected abstract void Decrypt(ref Memory<byte> data);

        void IAfterMessageWrited.Handle(ref Memory<byte> data) {
            try {
                this.Encrypt(ref data);
            }
            catch (Exception e) {
                Log.Error(e);
            }
        }

        void IAfterMessageReaded.Handle(ref Memory<byte> data) {
            try {
                this.Decrypt(ref data);
            }
            catch (Exception e) {
                Log.Error(e);
            }
        }

        public abstract void Init(IPluggable pluggable);

        public abstract void Dispose();
    }
}
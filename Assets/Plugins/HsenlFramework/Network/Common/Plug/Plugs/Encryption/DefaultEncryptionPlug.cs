using System;
using System.Security.Cryptography;
using System.Text;

namespace Hsenl.Network {
    public class DefaultEncryptionPlug : EncryptionPlug {
        private static readonly byte[] _key = Encoding.UTF8.GetBytes("fsdfsdfsdfsdfs");

        protected override void Encrypt(ref Memory<byte> data) {
            EncryptionHelper.GetQuickXorBytes(data.Span, _key);
        }

        protected override void Decrypt(ref Memory<byte> data) {
            EncryptionHelper.GetQuickXorBytes(data.Span, _key);
        }

        public override void Init(IPluggable pluggable) { }

        public override void Dispose() { }
    }
}
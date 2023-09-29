using System;
using System.IO;

namespace Hsenl {
    /// <summary>
    /// 校验相关的实用函数。
    /// </summary>
    public static class VerifierHelper {
        /// <summary>
        /// CRC32 算法。
        /// </summary>
        private sealed class Crc32 {
            private const int TableLength = 256;
            private const uint DefaultPolynomial = 0xedb88320;
            private const uint DefaultSeed = 0xffffffff;

            private readonly uint m_Seed;
            private readonly uint[] m_Table;
            private uint m_Hash;

            public Crc32() : this(DefaultPolynomial, DefaultSeed) { }

            public Crc32(uint polynomial, uint seed) {
                m_Seed = seed;
                m_Table = InitializeTable(polynomial);
                m_Hash = seed;
            }

            public void Initialize() {
                m_Hash = m_Seed;
            }

            public void HashCore(byte[] bytes, int offset, int length) {
                m_Hash = CalculateHash(m_Table, m_Hash, bytes, offset, length);
            }

            public uint HashFinal() {
                return ~m_Hash;
            }

            private static uint CalculateHash(uint[] table, uint value, byte[] bytes, int offset, int length) {
                var last = offset + length;
                for (var i = offset; i < last; i++) {
                    unchecked {
                        value = value >> 8 ^ table[bytes[i] ^ value & 0xff];
                    }
                }

                return value;
            }

            private static uint[] InitializeTable(uint polynomial) {
                var table = new uint[TableLength];
                for (var i = 0; i < TableLength; i++) {
                    var entry = (uint)i;
                    for (var j = 0; j < 8; j++) {
                        if ((entry & 1) == 1) {
                            entry = entry >> 1 ^ polynomial;
                        }
                        else {
                            entry >>= 1;
                        }
                    }

                    table[i] = entry;
                }

                return table;
            }
        }

        private const int CachedBytesLength = 0x1000;
        private static readonly byte[] s_CachedBytes = new byte[CachedBytesLength];
        private static readonly Crc32 s_Algorithm = new Crc32();

        /// <summary>
        /// 计算二进制流的 CRC32。
        /// </summary>
        /// <param name="bytes">指定的二进制流。</param>
        /// <returns>计算后的 CRC32。</returns>
        public static int GetCrc32(byte[] bytes) {
            if (bytes == null) {
                throw new Exception("Bytes is invalid.");
            }

            return GetCrc32(bytes, 0, bytes.Length);
        }

        /// <summary>
        /// 计算二进制流的 CRC32。
        /// </summary>
        /// <param name="bytes">指定的二进制流。</param>
        /// <param name="offset">二进制流的偏移。</param>
        /// <param name="length">二进制流的长度。</param>
        /// <returns>计算后的 CRC32。</returns>
        public static int GetCrc32(byte[] bytes, int offset, int length) {
            if (bytes == null) {
                throw new Exception("Bytes is invalid.");
            }

            if (offset < 0 || length < 0 || offset + length > bytes.Length) {
                throw new Exception("Offset or length is invalid.");
            }

            s_Algorithm.HashCore(bytes, offset, length);
            var result = (int)s_Algorithm.HashFinal();
            s_Algorithm.Initialize();
            return result;
        }

        /// <summary>
        /// 计算二进制流的 CRC32。
        /// </summary>
        /// <param name="stream">指定的二进制流。</param>
        /// <returns>计算后的 CRC32。</returns>
        public static int GetCrc32(Stream stream) {
            if (stream == null) {
                throw new Exception("Stream is invalid.");
            }

            while (true) {
                var bytesRead = stream.Read(s_CachedBytes, 0, CachedBytesLength);
                if (bytesRead > 0) {
                    s_Algorithm.HashCore(s_CachedBytes, 0, bytesRead);
                }
                else {
                    break;
                }
            }

            var result = (int)s_Algorithm.HashFinal();
            s_Algorithm.Initialize();
            Array.Clear(s_CachedBytes, 0, CachedBytesLength);
            return result;
        }

        /// <summary>
        /// 获取 CRC32 数值的二进制数组。
        /// </summary>
        /// <param name="crc32">CRC32 数值。</param>
        /// <returns>CRC32 数值的二进制数组。</returns>
        public static byte[] GetCrc32Bytes(int crc32) {
            return new byte[] {
                (byte)(crc32 >> 24 & 0xff), (byte)(crc32 >> 16 & 0xff), (byte)(crc32 >> 8 & 0xff),
                (byte)(crc32 & 0xff)
            };
        }

        /// <summary>
        /// 获取 CRC32 数值的二进制数组。
        /// </summary>
        /// <param name="crc32">CRC32 数值。</param>
        /// <param name="bytes">要存放结果的数组。</param>
        public static void GetCrc32Bytes(int crc32, byte[] bytes) {
            GetCrc32Bytes(crc32, bytes, 0);
        }

        /// <summary>
        /// 获取 CRC32 数值的二进制数组。
        /// </summary>
        /// <param name="crc32">CRC32 数值。</param>
        /// <param name="bytes">要存放结果的数组。</param>
        /// <param name="offset">CRC32 数值的二进制数组在结果数组内的起始位置。</param>
        public static void GetCrc32Bytes(int crc32, byte[] bytes, int offset) {
            if (bytes == null) {
                throw new Exception("Result is invalid.");
            }

            if (offset < 0 || offset + 4 > bytes.Length) {
                throw new Exception("Offset or length is invalid.");
            }

            bytes[offset] = (byte)(crc32 >> 24 & 0xff);
            bytes[offset + 1] = (byte)(crc32 >> 16 & 0xff);
            bytes[offset + 2] = (byte)(crc32 >> 8 & 0xff);
            bytes[offset + 3] = (byte)(crc32 & 0xff);
        }

        internal static int GetCrc32(Stream stream, byte[] code, int length) {
            if (stream == null) {
                throw new Exception("Stream is invalid.");
            }

            if (code == null) {
                throw new Exception("Code is invalid.");
            }

            var codeLength = code.Length;
            if (codeLength <= 0) {
                throw new Exception("Code length is invalid.");
            }

            var bytesLength = (int)stream.Length;
            if (length < 0 || length > bytesLength) {
                length = bytesLength;
            }

            var codeIndex = 0;
            while (true) {
                var bytesRead = stream.Read(s_CachedBytes, 0, CachedBytesLength);
                if (bytesRead > 0) {
                    if (length > 0) {
                        for (var i = 0; i < bytesRead && i < length; i++) {
                            s_CachedBytes[i] ^= code[codeIndex++];
                            codeIndex %= codeLength;
                        }

                        length -= bytesRead;
                    }

                    s_Algorithm.HashCore(s_CachedBytes, 0, bytesRead);
                }
                else {
                    break;
                }
            }

            var result = (int)s_Algorithm.HashFinal();
            s_Algorithm.Initialize();
            Array.Clear(s_CachedBytes, 0, CachedBytesLength);
            return result;
        }
    }
}
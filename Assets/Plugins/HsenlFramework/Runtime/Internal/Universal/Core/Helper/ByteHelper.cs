﻿using System;
using System.Text;

namespace Hsenl {
    public static class ByteHelper {
        public static string ToHex(this byte b) {
            return b.ToString("X2");
        }

        public static string ToHex(this byte[] bytes) {
            var stringBuilder = new StringBuilder();
            foreach (var b in bytes) {
                stringBuilder.Append(b.ToString("X2"));
            }

            return stringBuilder.ToString();
        }

        public static string ToHex(this byte[] bytes, string format) {
            var stringBuilder = new StringBuilder();
            foreach (var b in bytes) {
                stringBuilder.Append(b.ToString(format));
            }

            return stringBuilder.ToString();
        }

        public static string ToHex(this byte[] bytes, int offset, int count) {
            var stringBuilder = new StringBuilder();
            for (var i = offset; i < offset + count; ++i) {
                stringBuilder.Append(bytes[i].ToString("X2"));
            }

            return stringBuilder.ToString();
        }

        public static string ToStr(this byte[] bytes) {
            return Encoding.Default.GetString(bytes);
        }

        public static string ToStr(this byte[] bytes, int index, int count) {
            return Encoding.Default.GetString(bytes, index, count);
        }

        public static string Utf8ToStr(this byte[] bytes) {
            return Encoding.UTF8.GetString(bytes);
        }

        public static string Utf8ToStr(this byte[] bytes, int index, int count) {
            return Encoding.UTF8.GetString(bytes, index, count);
        }

        public static void WriteTo(this byte[] bytes, int offset, uint num) {
            bytes[offset] = (byte)(num & 0xff);
            bytes[offset + 1] = (byte)((num & 0xff00) >> 8);
            bytes[offset + 2] = (byte)((num & 0xff0000) >> 16);
            bytes[offset + 3] = (byte)((num & 0xff000000) >> 24);
        }

        public static void WriteTo(this byte[] bytes, int offset, int num) {
            bytes[offset] = (byte)(num & 0xff);
            bytes[offset + 1] = (byte)((num & 0xff00) >> 8);
            bytes[offset + 2] = (byte)((num & 0xff0000) >> 16);
            bytes[offset + 3] = (byte)((num & 0xff000000) >> 24);
        }

        public static void WriteTo(this Span<byte> span, int num, int offset = 0) {
            span[offset] = (byte)(num & 0xff);
            span[offset + 1] = (byte)((num & 0xff00) >> 8);
            span[offset + 2] = (byte)((num & 0xff0000) >> 16);
            span[offset + 3] = (byte)((num & 0xff000000) >> 24);
        }
        
        public static void WriteTo(this Span<byte> span, ushort num, int offset = 0) {
            span[offset] = (byte)(num & 0xff);
            span[offset + 1] = (byte)((num & 0xff00) >> 8);
        }

        public static void WriteTo(this byte[] bytes, int offset, byte num) {
            bytes[offset] = num;
        }

        public static void WriteTo(this byte[] bytes, int offset, short num) {
            bytes[offset] = (byte)(num & 0xff);
            bytes[offset + 1] = (byte)((num & 0xff00) >> 8);
        }

        public static void WriteTo(this byte[] bytes, int offset, ushort num) {
            bytes[offset] = (byte)(num & 0xff);
            bytes[offset + 1] = (byte)((num & 0xff00) >> 8);
        }

        public static unsafe void WriteTo(this byte[] bytes, int offset, long num) {
            var bPoint = (byte*)&num;
            for (var i = 0; i < sizeof(long); ++i) {
                bytes[offset + i] = bPoint[i];
            }
        }
    }
}
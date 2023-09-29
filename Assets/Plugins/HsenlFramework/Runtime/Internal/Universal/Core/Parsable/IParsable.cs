#nullable enable
using System;
using System.Diagnostics.CodeAnalysis;

namespace Hsenl {
    public interface IParsable<T> where T : IParsable<T> {
        T Parse(string s, IFormatProvider? provider);
        bool TryParse([NotNullWhen(true)] string? s, IFormatProvider? provider, [MaybeNullWhen(false)] out T result);
    }
}
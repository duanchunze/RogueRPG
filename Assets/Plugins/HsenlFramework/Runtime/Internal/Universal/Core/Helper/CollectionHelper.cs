using System.Collections.Generic;

namespace Hsenl {
    public static class CollectionHelper {
        public static TValue GetValueOrDefault<TKey, TValue>(this IReadOnlyDictionary<TKey, TValue> self, TKey key, TValue defaultValue = default) {
            if (self.TryGetValue(key, out var value))
                return value;

            return defaultValue;
        }
    }
}
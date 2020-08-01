using Newtonsoft.Json.Schema;
using Newtonsoft.Json.Schema.Generation;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;

namespace sisbase.Utils {
    public static class SchemaUtils {
        private static ConcurrentDictionary<Type, JSchema> cache { get; } = new ConcurrentDictionary<Type, JSchema>();
        private static JSchemaGenerator gen = new JSchemaGenerator();
        public static JSchema For<T>() {
            if (cache.ContainsKey(typeof(T))){
                return cache[typeof(T)];
            }
            cache.TryAdd(typeof(T), gen.Generate(typeof(T)));
            return cache[typeof(T)];
        }
    }
}

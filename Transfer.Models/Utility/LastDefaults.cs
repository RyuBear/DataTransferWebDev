using System.Collections.Generic;

namespace Transfer.Models{
    public static class LastDefaults {
        public static class DataPager {
            private static readonly Dictionary<string, string> KeyValue = new Dictionary<string, string>();
            public static string GetValue(string key) {
                return KeyValue.ContainsKey(key) ? KeyValue[key] : string.Empty;
            }
            public static void SetValue(string key, string value) {
                if (KeyValue.ContainsKey(key)) KeyValue[key] = value;
                else KeyValue.Add(key, value);
            }
        }
    }
}
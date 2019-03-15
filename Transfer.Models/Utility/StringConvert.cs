using System;
using System.Collections.Generic;
using System.Linq;

namespace Transfer.Models{
    public static class HtmlConvert {
        public static string NewLine2Br(this string input) {
            if (input == null) return null;
            return input.Replace("\n", "<br />");
        }
        public static string NewLine2Para(this string input) {
            if (string.IsNullOrEmpty(input)) return null;
            var _paras = input.Split('\n');
            return string.Format("<p>{0}</p>", string.Join("</p><p>", _paras));
        }
    }
    public static class SqlTypeConvert {
        public static object IsNull(this object obj, object target) {
            return obj ?? target;
        }
        public static object ToSqlType(this object obj) {
            return obj.IsNull(DBNull.Value);
        }
    }

    public static class StringConvert {
        public static string ToFixLength(this string str, int length) {
            if (str == null) return new string(' ', length);
            if (str.Length <= length) return string.Format("{0,-"+length+"}", str);
            return str.Substring(0, length);
        }
        public static string ToLength(this string str, int length) {
            if (str == null || str.Length <= length) return str;
            return str.Substring(0, length);
        }
        public static bool IsEmpty(this string str) {
            return str == null || str.Trim().Length == 0;
        }
        public static bool? ToBool(this string str) {
            if (str == null) return null;
            bool b;
            return bool.TryParse(str, out b) ? (bool?)b : null;
        }
        public static int? ToInt(this string str) {
            if (str == null) return null;
            int n;
            return int.TryParse(str, out n) ? (int?)n : null;
        }
        public static decimal? ToDecimal(this string str) {
            if (str == null) return null;
            decimal d;
            return decimal.TryParse(str, out d) ? (decimal?)d : null;
        }
        public static DateTime? ToDateTime(this string str) {
            if (str == null) return null;
            DateTime d;
            return DateTime.TryParse(str, out d) ? (DateTime?)d : null;
        }
        public static TimeSpan? ToTimeSpan(this string str) {
            if (str == null) return null;
            TimeSpan t;
            return TimeSpan.TryParse(str, out t) ? (TimeSpan?)t : null;
        }
        public static string ToString(object obj) {
            if (obj == null) return null;
            return obj.ToString();
        }
        public static string ToString(this TimeSpan? obj) {
            return obj != null ? string.Format("{0:hh\\:mm}", obj.Value) : null;
        }
        public static string TimeSpan_To_Varchar4(this TimeSpan? _span)
        {
            return _span == null ? null : string.Format("{0:00}{1:00}", _span.Value.Hours, _span.Value.Minutes);
        }
        public static TimeSpan? Varchar4_To_TimeSpan(this string _char4) {
            if (_char4.IsEmpty() || _char4.Length < 4) return null;
            var _hh = _char4.Substring(0, 2).ToInt();
            int? _mm = _char4.Substring(2, 2).ToInt();
            if (_hh == null || 0 > _hh.Value || _hh.Value > 23 
                || _mm == null || 0 > _hh.Value || _hh.Value > 59)
                return null;
            
            return new TimeSpan(_hh.Value, _mm.Value, 0);
        }
        public static TimeSpan? Varchar5_To_TimeSpan(this string _char5) {
            if (_char5.IsEmpty() || _char5.Length < 5) return null;
            var _hh = _char5.Substring(0, 2).ToInt();
            int? _mm = _char5.Substring(3, 2).ToInt();
            if (_hh == null || 0 > _hh.Value || _hh.Value > 23 
                || _mm == null || 0 > _hh.Value || _hh.Value > 59)
                return null;
            
            return new TimeSpan(_hh.Value, _mm.Value, 0);
        }
        public static string TimeSpan4_To_TimeSpan5(this string _char4) {
            var _span = _char4.Varchar4_To_TimeSpan();
            if (_span == null) return null;

            return string.Format("{0:hh\\:mm}", _span.Value);
        }
        public static string TimeSpan5_To_TimeSpan4(this string _char5) {
            var _span = _char5.Varchar5_To_TimeSpan();
            if (_span == null) return null;

            return string.Format("{0:hhmm}", _span.Value);
        }
        public static bool NullableBool_To_Bool(this bool? _b) {
            return _b ?? false;
        }
        public static int? ToInt(this decimal? _d) {
            if (_d == null) return null;
            return _d.Value.ToInt();
        }
        public static int ToInt(this decimal _d) {
            return Decimal.ToInt32(_d);
        }
        public static string ToMoney(this decimal? _d) {
            if (_d == null) return null;
            return _d.Value.ToMoney();
        }
        public static string ToMoney(this decimal _d) {
            return string.Format("{0:n0}", _d);//#,0
        }
        public static string ToMoney(this int _i) {
            return string.Format("{0:n0}", _i);
        }
        public static string ToMoney(this int? _i) {
            if (_i == null) return null;
            return string.Format("{0:n0}", _i.Value);
        }
        public static List<int> ToIntegerList(this string str, char separator = ',') {
            return str.Split(separator)
                .Select(s => s.ToInt())
                .Where(s => s != null)
                .Select(s=>s.Value)
                .ToList();
        }
    }
}
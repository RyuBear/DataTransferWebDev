using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Text.RegularExpressions;

/// <summary>
/// DataCheck 的摘要描述
/// </summary>
namespace Transfer.Models {
    public static class DataCheck {
        public enum CheckType { DateTime, Email, IdentityNumber, Telephone, MobilePhone, TraceCode }
        public static bool Is(string input, CheckType type) {
            if (string.IsNullOrEmpty(input)) return false;
            switch (type) {
                case CheckType.DateTime: return CheckDateTime(input);
                case CheckType.Email: return IsEmail(input);
                case CheckType.IdentityNumber: return IsIdentityNumber(input);
                case CheckType.Telephone: return IsTelephone(input);
                case CheckType.MobilePhone: return IsMobilePhone(input);
                case CheckType.TraceCode: return IsTraceCode(input);
            }
            return true;
        }
        static DateTime _dt;
        static bool CheckDateTime(string dt) { return DateTime.TryParse(dt, out _dt); }

        static Regex emailRegex = new Regex(
@"^(?("")("".+?""@)|(([0-9a-zA-Z]((\.(?!\.))|[-!#\$%&'\*\+/=\?\^`\{\}\|~\w])*)(?<=[0-9a-zA-Z])@))" +
@"(?(\[)(\[(\d{1,3}\.){3}\d{1,3}\])|(([0-9a-zA-Z][-\w]*[0-9a-zA-Z]\.)+[a-zA-Z]{2,6}))$", RegexOptions.Compiled);
        static bool IsEmail(string input) {
            return emailRegex.IsMatch(input);
        }

        static string dummy = null;
        static int[] areacode = new int[] { 10, 11, 12, 13, 14, 15, 16, 17, 34, 18, 19, 20, 21, 22, 35, 23, 24, 25, 26, 27, 28, 29, 32, 30, 31, 33 };
        static int[] multiple = new int[] { 1, 8, 7, 6, 5, 4, 3, 2, 1, 1 };
        static bool IsIdentityNumber(string input, out string error) {
            error = null;
            if (input == null || input.Length != 10) {
                error = "長度必須是10。";
                return false;
            }

            char[] id = input.ToCharArray();
            id[0] = char.ToUpper(id[0]);

            if (!char.IsUpper(id[0])) {
                error = "首碼必須是英文字母。";
                return false;
            }

            for (int i = 1; i < id.Length; i++)
                if (!char.IsDigit(id[i])) {
                    error = "除了首碼，其餘均為數字。";
                    return false;
                }

            if ("12".IndexOf(id[1]) < 0) {
                error = "第二碼必須是1或2。";//1:男,2:女
                return false;
            }

            int sum = 0;
            int first = areacode[id[0] - 65];//'A' == 65
            sum += (first / 10) + (first % 10 * 9);

            for (int i = 1; i < id.Length; i++)
                sum += (id[i] - 48) * multiple[i];//'0' == 48
            if (sum % 10 != 0) {
                error = "身份證字號檢查碼(第十碼)有誤。";
                return false;
            }
            
            return error == null;
        }
        static bool IsIdentityNumber(string input) { return IsIdentityNumber(input, out dummy); }
        static readonly Regex telephoneRegex = new Regex(@"^\d{2,3}-\d{7,8}(#\d{1,6})?$");
        static bool IsTelephone(string tel) {
            return telephoneRegex.IsMatch(tel);
        }
        static readonly Regex mobilephoneRegex = new Regex(@"^\d{4}-\d{6}$");
        static bool IsMobilePhone(string tel) {
            return mobilephoneRegex.IsMatch(tel);
        }
        static readonly Regex tracecodeRegex = new Regex(@"^\d{5}-?\d{5}-?\d{5}$");
        static bool IsTraceCode(string tel) {
            return tracecodeRegex.IsMatch(tel);
        }
    }
}
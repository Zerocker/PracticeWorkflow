using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace Quaer.Model
{
    /// <summary>
    /// Class for the phone number 
    /// </summary>
    public class Number
    {
        /// <summary>
        /// First digit of the phone number
        /// </summary>
        public byte Country;

        /// <summary>
        /// Next 3 digits of the phone number
        /// </summary>
        public short Def;
        
        /// <summary>
        /// Last 7 digits of the phone number
        /// </summary>
        public int Subscriber;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="phone">The phone number as 64-bit integer</param>
        public Number(long phone)
        {
            var digits = GetDigits(phone).ToArray();
            ParseDigits(ref digits);
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="phone">The phone number as string</param>
        public Number(string phone)
        {
            if (IsNumber(phone))
            {
                var digits = GetDigits(phone).ToArray();
                ParseDigits(ref digits);
            }
            else
                throw new Exception("Invalid phone number format!");
        }

        /// <summary>
        /// Checks if the string is a phone number or not
        /// </summary>
        /// <param name="phone">String to be verified</param>
        public static bool IsNumber(string phone)
        {
            Regex valid = new Regex("^[+]*[(]{0,1}[0-9]{1,4}[)]{0,1}[-\\s\\./0-9]*$");
            return valid.IsMatch(phone);
        }

        /// <summary>
        /// Returns a string that represents the current object
        /// </summary>
        public override string ToString()
        {
            // Subscriber field may contain head zeros
            return $"{Country}{Def}{Subscriber:D7}";
        }

        /// <summary>
        /// Generates Regex pattern for the current object
        /// </summary>
        /// <returns>Regex pattern string</returns>
        public string GenerateRegex()
        {
            string begin = $"\\+?[{Country}]*";
            string def = $"[\\s.\\(-]?" + Def;

            var digits = GetDigits(Subscriber).ToList();

            // Subscriber field may contain head zeros
            if (digits.Count != 7) digits.Insert(0, 0);     

            string part1 = "[\\s.\\)-]?" + digits[0] + digits[1] + digits[2];
            string part2 = "[\\s.-]?" + digits[3] + digits[4];
            string part3 = "[\\s.-]?" + digits[5] + digits[6];

            return begin + def + part1 + part2 + part3;
        }

        /// <summary>
        /// Parses array of phone number digits into object fields
        /// </summary>
        /// <param name="digits">Array of integer digits</param>
        private void ParseDigits(ref int[] digits)
        {
            string combined = string.Join("", digits);

            if (!int.TryParse(combined.Substring(combined.Length - 7), out Subscriber))
                throw new Exception("Invalid [subscriber] part of phone number!");

            combined = combined.Substring(0, combined.Length - 7);
            if (!short.TryParse(combined.Substring(1, 3), out Def))
                throw new Exception("Invalid [def] part of phone number!");

            combined = combined.Substring(0, combined.Length - 3);
            if (!byte.TryParse(combined, out Country))
                throw new Exception("Invalid [country] part of phone number!");
        }

        /// <summary>
        /// Gets all numbers from the phone number
        /// </summary>
        /// <param name="source">The phone number as 64-bit integer</param>
        /// <returns>Enumerable of integer digits</returns>
        private IEnumerable<int> GetDigits(long source)
        {
            long factor = 0;
            long tenner = Convert.ToInt64(Math.Pow(10, source.ToString().Length));
            do
            {
                source -= tenner * factor;
                tenner /= 10;
                factor = source / tenner;

                yield return Convert.ToInt32(factor);
            } while (tenner > 1);
        }

        /// <summary>
        /// Gets all numbers from the phone number
        /// </summary>
        /// <param name="source">The phone number as stringr</param>
        /// <returns>Enumerable of integer digits</returns>
        public static IEnumerable<int> GetDigits(string source)
        {
            var chars = source.Where(char.IsDigit);
            return chars.Select(c => c - '0');
        }
    }
}

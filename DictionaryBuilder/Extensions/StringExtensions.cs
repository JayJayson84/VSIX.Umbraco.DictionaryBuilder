using System;
using System.Security.Cryptography;
using System.Text;

namespace DictionaryBuilder.Extensions
{
    internal static class String
    {

        /// <summary>
        /// Encodes a string to asterisk (*) characters of equal length to the original.
        /// </summary>
        /// <param name="value">The UTF-16 string to convert.</param>
        /// <returns>A string of encoded asterisk characters (*).</returns>
        public static string ToMaskedString(this string value)
        {
            if (string.IsNullOrWhiteSpace(value)) return string.Empty;

            return new string('*', value.Length);
        }

        /// <summary>
        /// Converts a sequence of UTF-16 code units to its equivalent string representation that is encoded with base-64 digits.
        /// </summary>
        /// <param name="utf16String">A sequence of UTF-16 code units.</param>
        /// <returns>An encoded base-64 string. Otherwise <see langword="null"/>.</returns>
        public static string ToBase64String(this string utf16String)
        {
            if (utf16String == null)
            {
                return null;
            }
            else
            {
                return Convert.ToBase64String(Encoding.UTF8.GetBytes(utf16String));
            }
        }

        /// <summary>
        /// Converts the specified string, which encodes binary data as base-64 digits, to an equivalent 8-bit unsigned integer array.
        /// </summary>
        /// <param name="base64String">An encoded base-64 string.</param>
        /// <returns>An array of 8-bit unsigned integers that is equivalent to <paramref name="base64String"/>. Otherwise <see langword="null"/>.</returns>
        public static byte[] FromBase64String(this string base64String)
        {
            if (base64String == null)
            {
                return new byte[0];
            }
            else
            {
                return Convert.FromBase64String(base64String);
            }
        }

        /// <summary>
        /// Returns a computed SHA256 hash value for the specified UTF-16 string.
        /// </summary>
        /// <param name="utf16String">A sequence of UTF-16 code units.</param>
        /// <param name="length">The number of bytes to allocate to the array, based on the computed block size.</param>
        /// <returns>An array of computed SHA256 hash bytes. Otherwise <see langword="null"/>.</returns>
        public static byte[] ToSHA256Hash(this string utf16String, int length = 0)
        {
            using (var sha256 = new SHA256Managed())
            {
                var hash = sha256.ComputeHash(Encoding.UTF8.GetBytes(utf16String));

                if (length == 0) return hash;

                var hashArray = new byte[length];
                Array.Copy(hash, hashArray, length);

                return hashArray;
            };
        }

    }
}

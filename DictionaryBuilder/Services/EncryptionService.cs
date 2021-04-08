using DictionaryBuilder.Extensions;
using DictionaryBuilder.Models;
using System;
using System.IO;
using System.Security.Cryptography;

namespace DictionaryBuilder.Services
{
    internal sealed class EncryptionService
    {
        /// <summary>
        /// Represents an encryption class which implements the Advanced Encryption Standard (AES).
        /// </summary>
        internal sealed class Aes
        {

            #region " Constructors "

            /// <summary>
            /// Represents methods to encrypt and decrypt strings using the Advanced Encryption Standard (AES).
            /// </summary>
            public Aes() { }

            #endregion " Constructors "

            #region " Instance Methods "

            /// <summary>
            /// Encrypts a string using the Advanced Encryption Standard (AES) algorithm.
            /// </summary>
            /// <param name="input">The UTF-16 string to encrypt.</param>
            /// <param name="key">An optional key to use for the symmetric algorithm.</param>
            /// <returns>A string encrypted using the AES algorithm.</returns>
            public string Encrypt(string input, string key = null)
            {
                return EncryptString(input, key);
            }

            /// <summary>
            /// Decrypts a string using the Advanced Encryption Standard (AES) algorithm.
            /// </summary>
            /// <param name="input">The UTF-16 string to decrypt.</param>
            /// <param name="key">An optional key to use for the symmetric algorithm.</param>
            /// <returns>A string decrypted using the AES algorithm.</returns>
            public string Decrypt(string input, string key = null)
            {
                return DecryptString(input, key);
            }

            #endregion " Instance Methods "

            #region " Static Methods "

            /// <summary>
            /// Encrypts a string using the Advanced Encryption Standard (AES) algorithm.
            /// </summary>
            /// <param name="input">The UTF-16 string to encrypt.</param>
            /// <param name="key">An optional key to use for the symmetric algorithm.</param>
            /// <returns>A string encrypted using the AES algorithm.</returns>
            public static string EncryptString(string input, string key = null)
            {
                if (string.IsNullOrWhiteSpace(input)) return string.Empty;
                if (string.IsNullOrWhiteSpace(key)) key = EncryptionKeys.CommonKey;

                byte[] encrypted;

                try
                {
                    using (var aesAlg = System.Security.Cryptography.Aes.Create())
                    {
                        aesAlg.Key = key.ToSHA256Hash(aesAlg.KeySize / 8);
                        aesAlg.IV = DateTime.MinValue.ToOADate().ToString().ToSHA256Hash(aesAlg.BlockSize / 8);

                        var encryptor = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV);

                        using (var msEncrypt = new MemoryStream())
                        {
                            using (var csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                            {
                                using (var swEncrypt = new StreamWriter(csEncrypt))
                                {
                                    swEncrypt.Write(input);
                                };

                                encrypted = msEncrypt.ToArray();
                            };
                        };
                    };

                    return encrypted?.ToBase64String();
                }
                catch
                {
                    return null;
                }
            }

            /// <summary>
            /// Decrypts a string using the Advanced Encryption Standard (AES) algorithm.
            /// </summary>
            /// <param name="input">The UTF-16 string to decrypt.</param>
            /// <param name="key">An optional key to use for the symmetric algorithm.</param>
            /// <returns>A string decrypted using the AES algorithm.</returns>
            public static string DecryptString(string input, string key = null)
            {
                if (string.IsNullOrWhiteSpace(input)) return string.Empty;
                if (string.IsNullOrWhiteSpace(key)) key = EncryptionKeys.CommonKey;

                var cipher = input.FromBase64String();

                try
                {
                    using (var aesAlg = System.Security.Cryptography.Aes.Create())
                    {
                        aesAlg.Key = key.ToSHA256Hash(aesAlg.KeySize / 8);
                        aesAlg.IV = DateTime.MinValue.ToOADate().ToString().ToSHA256Hash(aesAlg.BlockSize / 8);

                        var decryptor = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV);

                        using (var msDecrypt = new MemoryStream(cipher))
                        {
                            using (var csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
                            {
                                using (var srDecrypt = new StreamReader(csDecrypt))
                                {
                                    return srDecrypt.ReadToEnd();
                                };
                            };
                        };
                    };
                }
                catch
                {
                    return null;
                }
            }

            #endregion " Static Methods "

        }
    }
}

using DictionaryBuilder.Extensions;
using DictionaryBuilder.Services;
using System.Text.RegularExpressions;
using System.Xml.Serialization;

namespace DictionaryBuilder.Models
{
    [XmlRoot(elementName: "SqlConnection")]
    public sealed class SqlCredentials
    {
        public const int DEFAULT_PORT = 1433;
        public const string DEFAULT_PORT_STRING = "1433";

        public SqlCredentials()
        {
            Constraints = new SqlConstraints();
        }

        public string Hostname { get; set; }
        public int Port { get; set; } = DEFAULT_PORT;
        public string Database { get; set; }
        public string Username { get; set; }
        [XmlElement(elementName: "Password")] public string EncryptedPassword { get; set; }
        [XmlIgnore] public string PasswordMask { get; set; }
        [XmlIgnore] public SqlConstraints Constraints { get; }
        [XmlIgnore] public static SqlCredentials Empty => new SqlCredentials();

        /// <summary>
        /// Gets the connection string for this <see cref="SqlCredentials"/> instance.
        /// </summary>
        /// <param name="encryptionMethod">The type of key to use when decrypting the password.</param>
        /// <returns>A formatted SQL connection string.</returns>
        public string GetConnectionString(EncryptionMethod encryptionMethod)
        {
            return $"Server={Hostname},{Port}; Database={Database}; User Id={Username}; Password={GetPassword(encryptionMethod)};";
        }

        /// <summary>
        /// Sets the encrypted password and mask.
        /// </summary>
        /// <param name="password">The plaintext password.</param>
        /// <param name="encryptionMethod">The type of key to use when encrypting the password.</param>
        public void SetPassword(string password, EncryptionMethod encryptionMethod)
        {
            if (Regex.IsMatch(password ?? string.Empty, "^[*]*$")) return;

            var encryptionKey = EncryptionKeys.GetKey(encryptionMethod);

            PasswordMask = password.ToMaskedString();
            EncryptedPassword = EncryptionService.Aes.EncryptString(password, encryptionKey);
        }

        /// <summary>
        /// Gets the decrypted password credential.
        /// </summary>
        /// <param name="encryptionMethod">The type of key to use when decrypting the password.</param>
        /// <returns>A decrypted password string.</returns>
        public string GetPassword(EncryptionMethod encryptionMethod)
        {
            var encryptionKey = EncryptionKeys.GetKey(encryptionMethod);

            return EncryptionService.Aes.DecryptString(EncryptedPassword, encryptionKey);
        }
    }
}

using DictionaryBuilder.Extensions;
using DictionaryBuilder.Services;
using System;
using System.ComponentModel;
using System.ComponentModel.ExtendedAttributes;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml;
using System.Xml.Serialization;

namespace DictionaryBuilder
{
    /// <summary>
    /// Provides user options and helper methods to save and retrieve to a local project file.
    /// </summary>
    public sealed class VisualStudioOptions
    {

        #region " Constants "

        public const int DEFAULT_PORT = 1433;
        public const string DEFAULT_NAMESPACE = "{project}.{path}";
        public const string DEFAULT_CULTURE_MODEL_PATH = "/Models/UmbracoCulture.cs";
        public const string DEFAULT_CULTURE_MODEL_NAMESPACE = "Umbraco.Core.Models.Language";
        public const string DEFAULT_DICTIONARY_MODEL_PATH = "/Models/Dictionary.cs";
        public const string DEFAULT_DICTIONARY_MODEL_NAMESPACE = "Umbraco.Core.Models.Dictionary";
        public const string DEFAULT_DICTIONARYKEY_MODEL_PATH = "/Models/DictionaryKey.cs";
        public const string DEFAULT_DICTIONARYKEY_MODEL_NAMESPACE = "Umbraco.Core.Models.Dictionary";
        public const string DEFAULT_SERVICE_PATH = "/Services/Implement/DictionaryService.cs";
        public const string DEFAULT_SERVICE_NAMESPACE = "Umbraco.Core.Services.Implement";
        public const string DEFAULT_ISERVICE_PATH = "/Services/IDictionaryService.cs";
        public const string DEFAULT_ISERVICE_NAMESPACE = "Umbraco.Core.Services";
        public const string DEFAULT_SERVICE_EXTENSION_PATH = "/Extensions/DictionaryExtensions.cs";
        public const string DEFAULT_SERVICE_EXTENSION_NAMESPACE = "Umbraco.Core.Services";

        #endregion

        #region " Instance Fields "

        public static readonly VisualStudioOptions Instance = new VisualStudioOptions();

        private string PasswordMask;

        #endregion

        #region " Properties "

        [SortableCategory("SQL Connection", 1, 5)]
        [DisplayName("Hostname")]
        [Description("The host domain/IP address of the SQL server.")]
        public string Hostname { get; set; }

        [SortableCategory("SQL Connection", 1, 5)]
        [DisplayName("Port")]
        [Description("The connection port of the SQL server. Default: 1433")]
        [DefaultValue(DEFAULT_PORT)]
        public int Port { get; set; } = DEFAULT_PORT;

        [SortableCategory("SQL Connection", 1, 5)]
        [DisplayName("Database")]
        [Description("The name of the SQL database.")]
        public string Database { get; set; }

        [SortableCategory("SQL Credentials", 2, 5)]
        [DisplayName("Username")]
        [Description("The username credential to connect to the SQL server.")]
        public string Username { get; set; }

        [SortableCategory("SQL Credentials", 2, 5)]
        [DisplayName("Password")]
        [PasswordPropertyText(true)]
        [Description("The password credential to connect to the SQL server.")]
        [XmlIgnore]
        public string Password
        {
            get
            {
                return PasswordMask;
            }
            set
            {
                SetPassword(value);
            }
        }

        [Browsable(false)]
        [XmlElement(elementName: "Password")]
        public string EncryptedPassword { get; set; }

        [SortableCategory("Model Options", 3, 5)]
        [DisplayName("Culture namespace")]
        [Description("A namespace for the language culture models. Enter a static string and/or use valid macros: {project}, {path}. Default value: " + DEFAULT_CULTURE_MODEL_NAMESPACE)]
        [DefaultValue(DEFAULT_CULTURE_MODEL_NAMESPACE)]
        public string CultureModelNamespace { get; set; } = DEFAULT_CULTURE_MODEL_NAMESPACE;

        [SortableCategory("Model Options", 3, 5)]
        [DisplayName("Culture path")]
        [Description("A file path, relative to the project, where the language culture models will be exported. Default value: " + DEFAULT_CULTURE_MODEL_PATH)]
        [DefaultValue(DEFAULT_CULTURE_MODEL_PATH)]
        public string CultureModelPath { get; set; } = DEFAULT_CULTURE_MODEL_PATH;

        [SortableCategory("Model Options", 3, 5)]
        [DisplayName("Dictionary namespace")]
        [Description("A namespace for the dictionary models. Enter a static string and/or use valid macros: {project}, {path}. Default value: " + DEFAULT_DICTIONARY_MODEL_NAMESPACE)]
        [DefaultValue(DEFAULT_DICTIONARY_MODEL_NAMESPACE)]
        public string DictionaryModelNamespace { get; set; } = DEFAULT_DICTIONARY_MODEL_NAMESPACE;

        [SortableCategory("Model Options", 3, 5)]
        [DisplayName("Dictionary path")]
        [Description("A file path, relative to the project, where the dictionary models will be exported. Default value: " + DEFAULT_DICTIONARY_MODEL_PATH)]
        [DefaultValue(DEFAULT_DICTIONARY_MODEL_PATH)]
        public string DictionaryModelPath { get; set; } = DEFAULT_DICTIONARY_MODEL_PATH;

        [SortableCategory("Model Options", 3, 5)]
        [DisplayName("DictionaryKey namespace")]
        [Description("A namespace for the dictionarykey models. Enter a static string and/or use valid macros: {project}, {path}. Default value: " + DEFAULT_DICTIONARYKEY_MODEL_NAMESPACE)]
        [DefaultValue(DEFAULT_DICTIONARYKEY_MODEL_NAMESPACE)]
        public string DictionaryKeyModelNamespace { get; set; } = DEFAULT_DICTIONARYKEY_MODEL_NAMESPACE;

        [SortableCategory("Model Options", 3, 5)]
        [DisplayName("DictionaryKey path")]
        [Description("A file path, relative to the project, where the dictionarykey models will be exported. Default value: " + DEFAULT_DICTIONARYKEY_MODEL_PATH)]
        [DefaultValue(DEFAULT_DICTIONARYKEY_MODEL_PATH)]
        public string DictionaryKeyModelPath { get; set; } = DEFAULT_DICTIONARYKEY_MODEL_PATH;

        [SortableCategory("Service Options", 4, 5)]
        [DisplayName("Service namespace")]
        [Description("A namespace for the dictionary service methods. Enter a static string and/or use valid macros: {project}, {path}. Default value: " + DEFAULT_SERVICE_NAMESPACE)]
        [DefaultValue(DEFAULT_SERVICE_NAMESPACE)]
        public string ServiceNamespace { get; set; } = DEFAULT_SERVICE_NAMESPACE;

        [SortableCategory("Service Options", 4, 5)]
        [DisplayName("Service path")]
        [Description("A file path, relative to the project, where the dictionary service methods will be exported. Default value: " + DEFAULT_SERVICE_PATH)]
        [DefaultValue(DEFAULT_SERVICE_PATH)]
        public string ServicePath { get; set; } = DEFAULT_SERVICE_PATH;

        [SortableCategory("Service Options", 4, 5)]
        [DisplayName("Interface namespace")]
        [Description("A namespace for the dictionary service interface. Enter a static string and/or use valid macros: {project}, {path}. Default value: " + DEFAULT_ISERVICE_NAMESPACE)]
        [DefaultValue(DEFAULT_ISERVICE_NAMESPACE)]
        public string IServiceNamespace { get; set; } = DEFAULT_ISERVICE_NAMESPACE;

        [SortableCategory("Service Options", 4, 5)]
        [DisplayName("Interface path")]
        [Description("A file path, relative to the project, where the dictionary service interface will be exported. Default value: " + DEFAULT_ISERVICE_PATH)]
        [DefaultValue(DEFAULT_ISERVICE_PATH)]
        public string IServicePath { get; set; } = DEFAULT_ISERVICE_PATH;

        [SortableCategory("Extension Options", 5, 5)]
        [DisplayName("Extension namespace")]
        [Description("A namespace for the dictionary service extension methods. Enter a static string and/or use valid macros: {project}, {path}. Default value: " + DEFAULT_SERVICE_EXTENSION_NAMESPACE)]
        [DefaultValue(DEFAULT_SERVICE_EXTENSION_NAMESPACE)]
        public string ServiceExtensionNamespace { get; set; } = DEFAULT_SERVICE_EXTENSION_NAMESPACE;

        [SortableCategory("Extension Options", 5, 5)]
        [DisplayName("Extension path")]
        [Description("A file path, relative to the project, where the dictionary service extension methods will be exported. Default value: " + DEFAULT_SERVICE_EXTENSION_PATH)]
        [DefaultValue(DEFAULT_SERVICE_EXTENSION_PATH)]
        public string ServiceExtensionPath { get; set; } = DEFAULT_SERVICE_EXTENSION_PATH;

        #endregion

        #region " Public Methods "

        /// <summary>
        /// Saves user preferences to an XML formatted file in the solution directory.
        /// </summary>
        public void Save()
        {
            var xS = new XmlSerializer(GetType());
            var nS = new XmlSerializerNamespaces(new[] { new XmlQualifiedName(string.Empty, string.Empty) });

            var XMLSettings = new XmlWriterSettings()
            {
                Indent = true,
                IndentChars = "\t",
                OmitXmlDeclaration = false,
                Encoding = Encoding.UTF8
            };

            try
            {
                using (var sW = new StreamWriter(VisualStudioHelper.Paths.OptionsFilePath))
                {
                    using (var xW = XmlWriter.Create(sW, XMLSettings))
                    {
                        xS.Serialize(xW, this, nS);
                    }
                }
            }
            catch (Exception) { }
        }

        /// <summary>
        /// Loads user preferences from the XML formatted options file in the solution directory.
        /// </summary>
        /// <returns>A reference to the loaded <see cref="VisualStudioOptions"/>.</returns>
        public VisualStudioOptions Load()
        {
            var options = DeSerialize();

            Hostname = options.Hostname;
            Port = options.Port;
            Database = options.Database;
            Username = options.Username;
            EncryptedPassword = options.EncryptedPassword;
            PasswordMask = options.GetPassword().ToMaskedString();
            CultureModelNamespace = options.CultureModelNamespace;
            CultureModelPath = options.CultureModelPath;
            DictionaryModelNamespace = options.DictionaryModelNamespace;
            DictionaryModelPath = options.DictionaryModelPath;
            DictionaryKeyModelNamespace = options.DictionaryKeyModelNamespace;
            DictionaryKeyModelPath = options.DictionaryKeyModelPath;
            ServiceNamespace = options.ServiceNamespace;
            ServicePath = options.ServicePath;
            IServiceNamespace = options.IServiceNamespace;
            IServicePath = options.IServicePath;
            ServiceExtensionNamespace = options.ServiceExtensionNamespace;
            ServiceExtensionPath = options.ServiceExtensionPath;

            return this;
        }

        /// <summary>
        /// Clears the existing user preferences and reloads the saved options.
        /// </summary>
        /// <returns>A reference to the loaded <see cref="VisualStudioOptions"/>.</returns>
        public VisualStudioOptions Reload()
        {
            return Clear().Load();
        }

        /// <summary>
        /// Clears user defined options and restores default values.
        /// </summary>
        /// <returns>A reference to the cleared <see cref="VisualStudioOptions"/>.</returns>
        public VisualStudioOptions Clear()
        {
            Hostname = Database = Username = Password = null;
            Port = DEFAULT_PORT;

            CultureModelNamespace = DEFAULT_CULTURE_MODEL_NAMESPACE;
            CultureModelPath = DEFAULT_CULTURE_MODEL_PATH;
            DictionaryModelNamespace = DEFAULT_DICTIONARY_MODEL_NAMESPACE;
            DictionaryModelPath = DEFAULT_DICTIONARY_MODEL_PATH;
            DictionaryKeyModelNamespace = DEFAULT_DICTIONARYKEY_MODEL_NAMESPACE;
            DictionaryKeyModelPath = DEFAULT_DICTIONARYKEY_MODEL_PATH;
            ServiceNamespace = DEFAULT_SERVICE_NAMESPACE;
            ServicePath = DEFAULT_SERVICE_PATH;
            IServiceNamespace = DEFAULT_ISERVICE_NAMESPACE;
            IServicePath = DEFAULT_ISERVICE_PATH;
            ServiceExtensionNamespace = DEFAULT_SERVICE_EXTENSION_NAMESPACE;
            ServiceExtensionPath = DEFAULT_SERVICE_EXTENSION_PATH;

            return this;
        }

        /// <summary>
        /// Gets the decrypted SQL password credential.
        /// </summary>
        /// <returns>A password string.</returns>
        public string GetPassword()
        {
            return EncryptionService.Aes.DecryptString(EncryptedPassword);
        }

        #endregion

        #region " Private Methods "

        private void SetPassword(string password)
        {
            if (string.IsNullOrWhiteSpace(password) || !Regex.IsMatch(password, "^[*]*$"))
            {
                PasswordMask = password.ToMaskedString();
                EncryptedPassword = EncryptionService.Aes.EncryptString(password);
            }
        }

        #endregion

        #region " Static Methods "

        /// <summary>
        /// Deserializes user preferences from the XML formatted options file in the solution directory.
        /// </summary>
        /// <returns>A reference to the deserialized <see cref="VisualStudioOptions"/>.</returns>
        public static VisualStudioOptions DeSerialize()
        {
            if (!VisualStudioHelper.HasSolution) return Instance;

            var optionsFilePath = VisualStudioHelper.Paths.OptionsFilePath;

            if (!File.Exists(optionsFilePath)) return Instance;

            try
            {
                using (var sR = new StreamReader(optionsFilePath))
                {
                    return new XmlSerializer(typeof(VisualStudioOptions)).Deserialize(sR) as VisualStudioOptions;
                }
            }
            catch { return Instance; }
        }

        #endregion

    }
}

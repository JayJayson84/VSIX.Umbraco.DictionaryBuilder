using DictionaryBuilder.Extensions;
using DictionaryBuilder.Models;
using System;
using System.ComponentModel;
using System.ComponentModel.ExtendedAttributes;
using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Serialization;

namespace DictionaryBuilder
{
    /// <summary>
    /// Provides user options and helper methods to save and retrieve to a local project file.
    /// </summary>
    [XmlRoot(elementName: "DictionaryBuilderOptions")]
    public sealed class VisualStudioOptions
    {

        #region " Constants "

        public const SqlConnectionType DEFAULT_DICTIONARY_SOURCE = SqlConnectionType.Development;
        public const EncryptionMethod DEFAULT_ENCRYPTION_METHOD = EncryptionMethod.Common;
        public const string DEFAULT_NAMESPACE = "{project}.{path}";
        public const string DEFAULT_CULTURE_MODEL_PATH = "/Models/Dictionary/UmbracoCulture.cs";
        public const string DEFAULT_CULTURE_MODEL_NAMESPACE = "Umbraco.Core.Models.Language";
        public const string DEFAULT_DICTIONARY_MODEL_PATH = "/Models/Dictionary/Dictionary.cs";
        public const string DEFAULT_DICTIONARY_MODEL_NAMESPACE = "Umbraco.Core.Models.Dictionary";
        public const string DEFAULT_DICTIONARYKEY_MODEL_PATH = "/Models/Dictionary/DictionaryKey.cs";
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

        #endregion

        #region " Constructors " 

        public VisualStudioOptions()
        {
            SqlConnections = new SqlConnections();
        }

        #endregion

        #region " Properties "

        [SortableCategory("Dictionary Options", 1, 8)]
        [DisplayName("Dictionary Source")]
        [Description("Choose the source SQL connection to use when rebuilding the dictionary classes.")]
        [DefaultValue(DEFAULT_DICTIONARY_SOURCE)]
        [TypeConverter(typeof(EnumConverter))]
        public SqlConnectionType DictionarySource { get; set; } = DEFAULT_DICTIONARY_SOURCE;

        [SortableCategory("Encryption Options", 2, 8)]
        [DisplayName("Encryption Method")]
        [Description("Choose the type of encryption key to use when saving SQL passwords. Use Common for shared repositories or System for use with a single device. Re-enter your password(s) after changing this value.")]
        [DefaultValue(DEFAULT_ENCRYPTION_METHOD)]
        [TypeConverter(typeof(EnumConverter))]
        public EncryptionMethod EncryptionMethod { get; set; } = DEFAULT_ENCRYPTION_METHOD;

        [SortableCategory("SQL Connection (Development)", 3, 8)]
        [DisplayName("Hostname")]
        [Description("The host domain/IP address of the development SQL server.")]
        [XmlIgnore]
        public string SqlDevelopmentHostname
        {
            get => SqlConnections.Development.Hostname;
            set => SqlConnections.Development.Hostname = value;
        }

        [SortableCategory("SQL Connection (Development)", 3, 8)]
        [DisplayName("Port")]
        [Description("The connection port of the development SQL server. Default: " + SqlCredentials.DEFAULT_PORT_STRING)]
        [DefaultValue(SqlCredentials.DEFAULT_PORT)]
        [XmlIgnore]
        public int SqlDevelopmentPort
        {
            get => SqlConnections.Development.Port;
            set => SqlConnections.Development.Port = value;
        }

        [SortableCategory("SQL Connection (Development)", 3, 8)]
        [DisplayName("Database")]
        [Description("The name of the development SQL database.")]
        [XmlIgnore]
        public string SqlDevelopmentDatabase
        {
            get => SqlConnections.Development.Database;
            set => SqlConnections.Development.Database = value;
        }

        [SortableCategory("SQL Connection (Development)", 3, 8)]
        [DisplayName("Username")]
        [Description("The username credential to connect to the development SQL server.")]
        [XmlIgnore]
        public string SqlDevelopmentUsername
        {
            get => SqlConnections.Development.Username;
            set => SqlConnections.Development.Username = value;
        }

        [SortableCategory("SQL Connection (Development)", 3, 8)]
        [DisplayName("Password")]
        [PasswordPropertyText(true)]
        [Description("The password credential to connect to the development SQL server.")]
        [XmlIgnore]
        public string SqlDevelopmentPassword
        {
            get => SqlConnections.Development.PasswordMask;
            set => SqlConnections.Development.SetPassword(value, EncryptionMethod);
        }

        [SortableCategory("SQL Connection (Staging)", 4, 8)]
        [DisplayName("Hostname")]
        [Description("The host domain/IP address of the staging SQL server.")]
        [XmlIgnore]
        public string SqlStagingHostname
        {
            get => SqlConnections.Staging.Hostname;
            set => SqlConnections.Staging.Hostname = value;
        }

        [SortableCategory("SQL Connection (Staging)", 4, 8)]
        [DisplayName("Port")]
        [Description("The connection port of the staging SQL server. Default: " + SqlCredentials.DEFAULT_PORT_STRING)]
        [DefaultValue(SqlCredentials.DEFAULT_PORT)]
        [XmlIgnore]
        public int SqlStagingPort
        {
            get => SqlConnections.Staging.Port;
            set => SqlConnections.Staging.Port = value;
        }

        [SortableCategory("SQL Connection (Staging)", 4, 8)]
        [DisplayName("Database")]
        [Description("The name of the staging SQL database.")]
        [XmlIgnore]
        public string SqlStagingDatabase
        {
            get => SqlConnections.Staging.Database;
            set => SqlConnections.Staging.Database = value;
        }

        [SortableCategory("SQL Connection (Staging)", 4, 8)]
        [DisplayName("Username")]
        [Description("The username credential to connect to the staging SQL server.")]
        [XmlIgnore]
        public string SqlStagingUsername
        {
            get => SqlConnections.Staging.Username;
            set => SqlConnections.Staging.Username = value;
        }

        [SortableCategory("SQL Connection (Staging)", 4, 8)]
        [DisplayName("Password")]
        [PasswordPropertyText(true)]
        [Description("The password credential to connect to the staging SQL server.")]
        [XmlIgnore]
        public string SqlStagingPassword
        {
            get => SqlConnections.Staging.PasswordMask;
            set => SqlConnections.Staging.SetPassword(value, EncryptionMethod);
        }

        [SortableCategory("SQL Connection (Live)", 5, 8)]
        [DisplayName("Hostname")]
        [Description("The host domain/IP address of the live SQL server.")]
        [XmlIgnore]
        public string SqlLiveHostname
        {
            get => SqlConnections.Live.Hostname;
            set => SqlConnections.Live.Hostname = value;
        }

        [SortableCategory("SQL Connection (Live)", 5, 8)]
        [DisplayName("Port")]
        [Description("The connection port of the live SQL server. Default: " + SqlCredentials.DEFAULT_PORT_STRING)]
        [DefaultValue(SqlCredentials.DEFAULT_PORT)]
        [XmlIgnore]
        public int SqlLivePort
        {
            get => SqlConnections.Live.Port;
            set => SqlConnections.Live.Port = value;
        }

        [SortableCategory("SQL Connection (Live)", 5, 8)]
        [DisplayName("Database")]
        [Description("The name of the live SQL database.")]
        [XmlIgnore]
        public string SqlLiveDatabase
        {
            get => SqlConnections.Live.Database;
            set => SqlConnections.Live.Database = value;
        }

        [SortableCategory("SQL Connection (Live)", 5, 8)]
        [DisplayName("Username")]
        [Description("The username credential to connect to the live SQL server.")]
        [XmlIgnore]
        public string SqlLiveUsername
        {
            get => SqlConnections.Live.Username;
            set => SqlConnections.Live.Username = value;
        }

        [SortableCategory("SQL Connection (Live)", 5, 8)]
        [DisplayName("Password")]
        [PasswordPropertyText(true)]
        [Description("The password credential to connect to the live SQL server.")]
        [XmlIgnore]
        public string SqlLivePassword
        {
            get => SqlConnections.Live.PasswordMask;
            set => SqlConnections.Live.SetPassword(value, EncryptionMethod);
        }

        [Browsable(false)]
        public SqlConnections SqlConnections { get; set; }

        [SortableCategory("Model Options", 6, 8)]
        [DisplayName("Culture namespace")]
        [Description("A namespace for the language culture models. Enter a static string and/or use valid macros: {project}, {path}. Default value: " + DEFAULT_CULTURE_MODEL_NAMESPACE)]
        [DefaultValue(DEFAULT_CULTURE_MODEL_NAMESPACE)]
        public string CultureModelNamespace { get; set; } = DEFAULT_CULTURE_MODEL_NAMESPACE;

        [SortableCategory("Model Options", 6, 8)]
        [DisplayName("Culture path")]
        [Description("A file path, relative to the project, where the language culture models will be exported. Default value: " + DEFAULT_CULTURE_MODEL_PATH)]
        [DefaultValue(DEFAULT_CULTURE_MODEL_PATH)]
        public string CultureModelPath { get; set; } = DEFAULT_CULTURE_MODEL_PATH;

        [SortableCategory("Model Options", 6, 8)]
        [DisplayName("Dictionary namespace")]
        [Description("A namespace for the dictionary models. Enter a static string and/or use valid macros: {project}, {path}. Default value: " + DEFAULT_DICTIONARY_MODEL_NAMESPACE)]
        [DefaultValue(DEFAULT_DICTIONARY_MODEL_NAMESPACE)]
        public string DictionaryModelNamespace { get; set; } = DEFAULT_DICTIONARY_MODEL_NAMESPACE;

        [SortableCategory("Model Options", 6, 8)]
        [DisplayName("Dictionary path")]
        [Description("A file path, relative to the project, where the dictionary models will be exported. Default value: " + DEFAULT_DICTIONARY_MODEL_PATH)]
        [DefaultValue(DEFAULT_DICTIONARY_MODEL_PATH)]
        public string DictionaryModelPath { get; set; } = DEFAULT_DICTIONARY_MODEL_PATH;

        [SortableCategory("Model Options", 6, 8)]
        [DisplayName("DictionaryKey namespace")]
        [Description("A namespace for the dictionarykey models. Enter a static string and/or use valid macros: {project}, {path}. Default value: " + DEFAULT_DICTIONARYKEY_MODEL_NAMESPACE)]
        [DefaultValue(DEFAULT_DICTIONARYKEY_MODEL_NAMESPACE)]
        public string DictionaryKeyModelNamespace { get; set; } = DEFAULT_DICTIONARYKEY_MODEL_NAMESPACE;

        [SortableCategory("Model Options", 6, 8)]
        [DisplayName("DictionaryKey path")]
        [Description("A file path, relative to the project, where the dictionarykey models will be exported. Default value: " + DEFAULT_DICTIONARYKEY_MODEL_PATH)]
        [DefaultValue(DEFAULT_DICTIONARYKEY_MODEL_PATH)]
        public string DictionaryKeyModelPath { get; set; } = DEFAULT_DICTIONARYKEY_MODEL_PATH;

        [SortableCategory("Service Options", 7, 8)]
        [DisplayName("Service namespace")]
        [Description("A namespace for the dictionary service methods. Enter a static string and/or use valid macros: {project}, {path}. Default value: " + DEFAULT_SERVICE_NAMESPACE)]
        [DefaultValue(DEFAULT_SERVICE_NAMESPACE)]
        public string ServiceNamespace { get; set; } = DEFAULT_SERVICE_NAMESPACE;

        [SortableCategory("Service Options", 7, 8)]
        [DisplayName("Service path")]
        [Description("A file path, relative to the project, where the dictionary service methods will be exported. Default value: " + DEFAULT_SERVICE_PATH)]
        [DefaultValue(DEFAULT_SERVICE_PATH)]
        public string ServicePath { get; set; } = DEFAULT_SERVICE_PATH;

        [SortableCategory("Service Options", 7, 8)]
        [DisplayName("Interface namespace")]
        [Description("A namespace for the dictionary service interface. Enter a static string and/or use valid macros: {project}, {path}. Default value: " + DEFAULT_ISERVICE_NAMESPACE)]
        [DefaultValue(DEFAULT_ISERVICE_NAMESPACE)]
        public string IServiceNamespace { get; set; } = DEFAULT_ISERVICE_NAMESPACE;

        [SortableCategory("Service Options", 7, 8)]
        [DisplayName("Interface path")]
        [Description("A file path, relative to the project, where the dictionary service interface will be exported. Default value: " + DEFAULT_ISERVICE_PATH)]
        [DefaultValue(DEFAULT_ISERVICE_PATH)]
        public string IServicePath { get; set; } = DEFAULT_ISERVICE_PATH;

        [SortableCategory("Extension Options", 8, 8)]
        [DisplayName("Extension namespace")]
        [Description("A namespace for the dictionary service extension methods. Enter a static string and/or use valid macros: {project}, {path}. Default value: " + DEFAULT_SERVICE_EXTENSION_NAMESPACE)]
        [DefaultValue(DEFAULT_SERVICE_EXTENSION_NAMESPACE)]
        public string ServiceExtensionNamespace { get; set; } = DEFAULT_SERVICE_EXTENSION_NAMESPACE;

        [SortableCategory("Extension Options", 8, 8)]
        [DisplayName("Extension path")]
        [Description("A file path, relative to the project, where the dictionary service extension methods will be exported. Default value: " + DEFAULT_SERVICE_EXTENSION_PATH)]
        [DefaultValue(DEFAULT_SERVICE_EXTENSION_PATH)]
        public string ServiceExtensionPath { get; set; } = DEFAULT_SERVICE_EXTENSION_PATH;

        #endregion

        #region " Public Methods "

        /// <summary>
        /// Gets the <see cref="SqlCredentials"/> for the connection type selected in the user preferences.
        /// </summary>
        /// <returns>An <see cref="SqlCredentials"/> class.</returns>
        public SqlCredentials GetSqlCredentials()
        {
            switch (DictionarySource)
            {
                case SqlConnectionType.Development:
                    return SqlConnections.Development;
                case SqlConnectionType.Staging:
                    return SqlConnections.Staging;
                case SqlConnectionType.Live:
                    return SqlConnections.Live;
                default:
                    return null;
            }
        }

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

            DictionarySource = options.DictionarySource;
            EncryptionMethod = options.EncryptionMethod;
            SqlConnections.Development = options.SqlConnections.Development;
            SqlConnections.Development.PasswordMask = options.SqlConnections.Development.GetPassword(EncryptionMethod).ToMaskedString();
            SqlConnections.Staging = options.SqlConnections.Staging;
            SqlConnections.Staging.PasswordMask = options.SqlConnections.Staging.GetPassword(EncryptionMethod).ToMaskedString();
            SqlConnections.Live = options.SqlConnections.Live;
            SqlConnections.Live.PasswordMask = options.SqlConnections.Live.GetPassword(EncryptionMethod).ToMaskedString();
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
            SqlConnections = new SqlConnections();

            DictionarySource = DEFAULT_DICTIONARY_SOURCE;
            EncryptionMethod = DEFAULT_ENCRYPTION_METHOD;
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

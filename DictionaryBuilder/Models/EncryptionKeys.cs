using System.Linq;
using System.Net.NetworkInformation;

namespace DictionaryBuilder.Models
{
    internal sealed class EncryptionKeys
    {
        /// <summary>
        /// Returns a system specific key.
        /// </summary>
        public static string SystemKey
        {
            get
            {
                var networkInterface = NetworkInterface.GetAllNetworkInterfaces().FirstOrDefault();
                if (!(networkInterface is null))
                {
                    return networkInterface.GetPhysicalAddress().ToString();
                }
                else
                {
                    return CommonKey;
                }
            }
        }

        /// <summary>
        /// Returns an extension specific key.
        /// </summary>
        public static string CommonKey
        {
            get
            {
                return DictionaryBuilderPackage.PackageGuidString;
            }
        }
    }
}

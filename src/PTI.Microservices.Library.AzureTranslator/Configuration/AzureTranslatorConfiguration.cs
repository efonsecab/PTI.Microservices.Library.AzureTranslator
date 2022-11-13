using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PTI.Microservices.Library.Configuration
{
    /// <summary>
    /// Configuration for Azure Translator 
    /// </summary>
    public class AzureTranslatorConfiguration
    {
        /// <summary>
        /// Azure Translator Endpoint
        /// </summary>
        public string Endpoint { get; set; }
        /// <summary>
        /// Azure Translator Key
        /// </summary>
        public string Key { get; set; }
        public string ResourceName { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PTI.Microservices.Library.AzureTranslator.Models.TranslateDocuments
{

    public class TranslateDocumentsRequest
    {
        public Input[] inputs { get; set; }
    }

    public class Input
    {
        public string storageType { get; set; }
        public Source source { get; set; }
        public Target[] targets { get; set; }
    }

    public class Source
    {
        public string sourceUrl { get; set; }
    }

    public class Target
    {
        public string targetUrl { get; set; }
        public string language { get; set; }
    }

}

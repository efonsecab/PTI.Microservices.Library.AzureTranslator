using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PTI.Microservices.Library.AzureTranslator.Models.TranslateDocuments
{

    public class TranslateDocumentsStatusResponse
    {
        public string id { get; set; }
        public DateTime createdDateTimeUtc { get; set; }
        public DateTime lastActionDateTimeUtc { get; set; }
        public string status { get; set; }
        public Summary summary { get; set; }
        public Error error { get; set; }
    }

    public class Error
    {
        public string code { get; set; }
        public string message { get; set; }
        public string target { get; set; }
        public InnerError innerError { get; set; }
    }

    public class InnerError
    {
        public string code { get; set; }
        public string message { get; set; }
        public string target { get; set; }
    }

    public class Summary
    {
        public int total { get; set; }
        public int failed { get; set; }
        public int success { get; set; }
        public int inProgress { get; set; }
        public int notYetStarted { get; set; }
        public int cancelled { get; set; }
        public int totalCharacterCharged { get; set; }
    }

}

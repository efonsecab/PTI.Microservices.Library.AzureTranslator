using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PTI.Microservices.Library.Models.AzureTranslator.Translate
{
    public class TranslateResponse
    {
        public List<TranslateResponseLanguageInforomation> Property1 { get; set; }
    }

    public class TranslateResponseLanguageInforomation
    {
        public Detectedlanguage detectedLanguage { get; set; }
        public List<Translation> translations { get; set; }
    }

    public class Detectedlanguage
    {
        public string language { get; set; }
        public float score { get; set; }
    }

    public class Translation
    {
        public string text { get; set; }
        public string to { get; set; }
    }
}

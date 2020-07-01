using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LanguageService
{
    public class Translation
    {
        public string TranslatedText { get; set; }
        public string Language { get; set; }
    }

    public class ToTranslate
    {
        public string ToLanguage { get; set; }
        public string FromLanguage { get; set; }
        public string TextToTranslate { get; set; }
    }

    public class UpdateTranslation
    {
        public string Language { get; set; }
        public string SelectedWord { get; set; }
        public string TranslatedWord { get; set; }
    }
}
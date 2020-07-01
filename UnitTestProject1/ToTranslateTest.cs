using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnitTestProject1
{
    public class ToTranslateTest
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

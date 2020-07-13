using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PDFGenerator.Models.Magnuson
{
    public class FeatureData
    {
        public string Code { get; set; }
        public string Label { get; set; }
        public string Thumbnail { get; set; }
        public List<Option> Options { get; set; }
        public List<SingleSelection> MultipleSelection { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PDFGenerator.Models.Magnuson
{
    public class Option
    {
        public string Code { get; set; }
        public string Label { get; set; }
        public string Thumbnail { get; set; }
        public bool Selected { get; set; }
        public string PartNumber { get; set; }
    }
}

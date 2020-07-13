using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http;
using PDFGenerator.Models.Magnuson;
using System.IO;
using System.Net;
using Newtonsoft.Json.Linq;
using Microsoft.Extensions.Primitives;
using Microsoft.AspNetCore.Cors;
//using NReco.PdfGenerator;
using Syncfusion.HtmlConverter;
using Syncfusion.Pdf;
using Syncfusion.Pdf.Graphics;
using Syncfusion.Drawing;
using Microsoft.AspNetCore.Hosting;
using Syncfusion.Pdf.HtmlToPdf;

namespace PDFGenerator.Controllers
{
    
    [ApiController]
    public class MagnusonController : ControllerBase

    {
        private IHostingEnvironment _env;
        public MagnusonController(IHostingEnvironment env)
        {
            _env = env;
        }

        // POST: api/Magnuson/image  
        [Produces("application/json")]
        [Route("api/magnuson/image")]
        [HttpPost]

        public IActionResult UploadImage(ImagePDF postedFile)
        {
            var webRoot = _env.WebRootPath;

            if (postedFile.data.Length > 0) {
                byte[] bytes = Convert.FromBase64String(postedFile.data.Replace("data:image/png;base64,", ""));
                string filename = Guid.NewGuid() + ".png";
                var filePath = Path.Combine(webRoot, @"wwwroot\Magnuson\Images\" + filename);
                System.IO.File.WriteAllBytes(filePath, bytes);

                return Ok(filename);
                }
                else
            {
                return Ok("Not OK");
            }
            
            

        }

        // POST: api/Magnuson/image     
        [Route("api/magnuson/pdf")]
        [HttpPost]
        public IActionResult CreatePDF(FeaturesList features)
        {
            var webRoot = _env.WebRootPath;

            var loadPath = Path.Combine(webRoot, @"Pages\MGOUmea.cshtml");
            //var loadFile = Path.Combine(loadPath, @"MGOUmea.cshtml");
            var html = System.IO.File.ReadAllText(loadPath);
            var savePath = @"wwwroot\Magnuson\pdf";
            var fileName = Guid.NewGuid() + ".pdf";
            var saveFile = Path.Combine(savePath, fileName);
            if (features.FeaturesData != null) {
                string partNumber = "Umea-";
                string productImage = features.ImageName;
                FeatureData body = features.FeaturesData.Find(x => x.Label == "Body");
                FeatureData bodyColor = features.FeaturesData.Find(x => x.Label == "Body Color");
                FeatureData top = features.FeaturesData.Find(x => x.Label == "Opening");
                FeatureData topColor = features.FeaturesData.Find(x => x.Label == "Top Color");
                FeatureData hood = features.FeaturesData.Find(x => x.Label == "Hood");
                FeatureData hoodColor = features.FeaturesData.Find(x => x.Label == "Hood Color");
                FeatureData label = features.FeaturesData.Find(x => x.Label == "Label");
                FeatureData labelColor = features.FeaturesData.Find(x => x.Label == "Label Color");
                FeatureData sidepanel = features.FeaturesData.Find(x => x.Label == "Cutouts");
                FeatureData sidepanelColor = features.FeaturesData.Find(x => x.Label == "Side Panel Color");
                Option bodyOption = null;
                Option bodyColorOption = null;                
                Option hoodOption = null;
                Option hoodColorOption = null;                
                Option sidepanelOption = null;
                Option sidepanelColorOption = null;

                if (productImage != null)
                {                    
                    html = html.Replace("{productImage}", productImage);
                }

                if (body != null)
                {
                    bodyOption = body.Options.Find(x => x.Selected == true);
                    html = html.Replace("{bodySize}", bodyOption.Label);
                    html = html.Replace("{bodySizeThumbnail}", bodyOption.Thumbnail);
                    partNumber += bodyOption.PartNumber;
                }

                if (bodyColor != null)
                {
                    bodyColorOption = bodyColor.Options.Find(x => x.Selected == true);
                    html = html.Replace("{bodyColor}", bodyColorOption.Label);
                    html = html.Replace("{bodyColorThumbnail}", bodyColorOption.Thumbnail);

                }

                if (top != null)
                {                    
                    html = html.Replace("{topLeft}", top.MultipleSelection[0].Label);
                    html = html.Replace("{topLeftThumbnail}", top.MultipleSelection[0].Thumbnail);                    

                    if (bodyOption.Label == "Double")
                    {

                        html = html.Replace("{topRight}", top.MultipleSelection[1].Label);
                        html = html.Replace("{topRightThumbnail}", top.MultipleSelection[1].Thumbnail);
                    }
                    else
                    {
                        html = html.Replace("{topCenter}", top.MultipleSelection[1].Label);
                        html = html.Replace("{topCenterThumbnail}", top.MultipleSelection[1].Thumbnail);

                        html = html.Replace("{topRight}", topColor.MultipleSelection[2].Label);
                        html = html.Replace("{topRightThumbnail}", top.MultipleSelection[2].Thumbnail);
                    }
                }

                if (topColor != null)
                {                    
                    html = html.Replace("{topLeftColor}", topColor.MultipleSelection[0].Label);
                    html = html.Replace("{topLeftColorThumbnail}", topColor.MultipleSelection[0].Thumbnail);

                    if (bodyOption.Label == "Double")
                    {                     

                        html = html.Replace("{topRightColor}", topColor.MultipleSelection[1].Label);
                        html = html.Replace("{topRightColorThumbnail}", topColor.MultipleSelection[1].Thumbnail);
                    }
                    else
                    {
                        html = html.Replace("{topCenterColor}", topColor.MultipleSelection[1].Label);
                        html = html.Replace("{topCenterColorThumbnail}", topColor.MultipleSelection[1].Thumbnail);

                        html = html.Replace("{topRightColor}", topColor.MultipleSelection[2].Label);
                        html = html.Replace("{topRightColorThumbnail}", topColor.MultipleSelection[2].Thumbnail);
                    }
                }

                if (hood != null)
                {
                    hoodOption = hood.Options.Find(x => x.Selected == true);
                    html = html.Replace("{hoodSize}", hoodOption.Label);
                    html = html.Replace("{hoodSizeThumbnail}", hoodOption.Thumbnail);
                    partNumber += hoodOption.PartNumber;

                    if(hoodOption.Label == "None")
                        html = html.Replace("{noHood}", "none");
                    else
                        html = html.Replace("{noHood}", "inline-block");
                }

                if (hoodColor != null)
                {
                    hoodColorOption = hoodColor.Options.Find(x => x.Selected == true);
                    html = html.Replace("{hoodColor}", hoodColorOption.Label);
                    html = html.Replace("{hoodColorThumbnail}", hoodColorOption.Thumbnail);

                }

                if (label != null)
                {
                    html = html.Replace("{labelLeft}", label.MultipleSelection[0].Label);
                    html = html.Replace("{labelLeftThumbnail}", label.MultipleSelection[0].Thumbnail);

                    if (bodyOption.Label == "Double")
                    {
                        html = html.Replace("{labelRight}", label.MultipleSelection[1].Label);
                        html = html.Replace("{labelRightThumbnail}", label.MultipleSelection[1].Thumbnail);
                    }
                    else
                    {
                        html = html.Replace("{labelCenter}", label.MultipleSelection[1].Label);
                        html = html.Replace("{labelCenterThumbnail}", label.MultipleSelection[1].Thumbnail);

                        html = html.Replace("{labelRight}", label.MultipleSelection[2].Label);
                        html = html.Replace("{labelRightThumbnail}", label.MultipleSelection[2].Thumbnail);
                    }
                }

                if (labelColor != null)
                {
                    html = html.Replace("{labelLeftColor}", labelColor.MultipleSelection[0].Label);
                    html = html.Replace("{labelLeftColorThumbnail}", labelColor.MultipleSelection[0].Thumbnail);

                    if (bodyOption.Label == "Double")
                    {

                        html = html.Replace("{labelRightColor}", labelColor.MultipleSelection[1].Label);
                        html = html.Replace("{labelRightColorThumbnail}", labelColor.MultipleSelection[1].Thumbnail);
                    }
                    else
                    {
                        html = html.Replace("{labelCenterColor}", labelColor.MultipleSelection[1].Label);
                        html = html.Replace("{labelCenterColorThumbnail}", labelColor.MultipleSelection[1].Thumbnail);

                        html = html.Replace("{labelRightColor}", labelColor.MultipleSelection[2].Label);
                        html = html.Replace("{labelRightColorThumbnail}", labelColor.MultipleSelection[2].Thumbnail);
                    }

                }

                if (hood != null)
                {
                    hoodOption = hood.Options.Find(x => x.Selected == true);
                    html = html.Replace("{hood}", hoodOption.Label);
                    html = html.Replace("{hoodThumbnail}", hoodOption.Thumbnail);
                }

                if (hoodColor != null)
                {
                    hoodColorOption = hoodColor.Options.Find(x => x.Selected == true);
                    html = html.Replace("{hoodColor}", hoodColorOption.Label);
                    html = html.Replace("{hoodColorThumbnail}", hoodColorOption.Thumbnail);

                }

                if (sidepanel != null)
                {
                    sidepanelOption = sidepanel.Options.Find(x => x.Selected == true);
                    html = html.Replace("{sidepanel}", sidepanelOption.Label);
                    html = html.Replace("{sidepanelThumbnail}", sidepanelOption.Thumbnail);
                }

                if (sidepanelColor != null)
                {
                    sidepanelColorOption = sidepanelColor.Options.Find(x => x.Selected == true);
                    html = html.Replace("{sidepanelColor}", sidepanelColorOption.Label);
                    html = html.Replace("{sidepanelColorThumbnail}", sidepanelColorOption.Thumbnail);

                }

                //Hide or show the multiple waste receptacle options
                switch (bodyOption.Label)
                {
                    case "Single":
                        html = html.Replace("{left}", "inherit");
                        html = html.Replace("{center}", "none");
                        html = html.Replace("{right}", "none");
                        break;
                    case "Double":
                        html = html.Replace("{left}", "inherit");
                        html = html.Replace("{center}", "none");
                        html = html.Replace("{right}", "inherit");
                        break;
                    case "Triple":
                        html = html.Replace("{left}", "inherit");
                        html = html.Replace("{center}", "inherit");
                        html = html.Replace("{right}", "inherit");
                        break;
                }
                //Show Partnumber
                html = html.Replace("{sku}", partNumber);
            }

            //Initialize HTML to PDF converter 
            HtmlToPdfConverter htmlConverter = new HtmlToPdfConverter();
            WebKitConverterSettings settings = new WebKitConverterSettings();
            
            //Set PDF page orientation 
            settings.Orientation = PdfPageOrientation.Landscape;
            //Set WebKit viewport size
            settings.WebKitViewPort = new Size(2030, 500);
            settings.PdfPageSize = new SizeF(new SizeF(1377, 750));
            //Set WebKit path
            settings.WebKitPath = Path.Combine(webRoot, @"QtBinariesWindows");
            //Assign WebKit settings to HTML converter
            htmlConverter.ConverterSettings = settings;

            //Convert URL to PDF
            PdfDocument document = htmlConverter.Convert(html, webRoot);
            
            MemoryStream ms = new MemoryStream();
            //Save the PDF document.
            document.Save(ms);
            //Close the PDF document.
            document.Close(true);

            
            System.IO.File.WriteAllBytes(saveFile, ms.ToArray());

            return Ok(fileName);

        }
    }
}

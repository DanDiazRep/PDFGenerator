using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;

namespace PDFGenerator.Controllers
{
    public class MGOUmeaController : Controller
    {     
    
    public IActionResult Index()
        {
            
            return View();
        }
    }
}
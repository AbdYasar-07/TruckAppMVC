using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TruckAppMVC.Controllers
{
    public class CommonController : Controller
    {
        [Route("/help")]
        public IActionResult Help()
        {
            return View();
        }

        [Route("/On-This-Page")]
        public IActionResult OnThisPage()
        {
            return View();
        }

    }
}

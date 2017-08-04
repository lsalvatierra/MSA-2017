using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ESANMSA.Areas.Seguridad.Controllers
{
    public class PrincipalController : Controller
    {
        // GET: Seguridad/Principal
        public ActionResult Index()
        {
            return View();
        }
    }
}
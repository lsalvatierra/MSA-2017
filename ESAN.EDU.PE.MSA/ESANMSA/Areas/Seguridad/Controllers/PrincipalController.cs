using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ESANMSA.Utilitarios;
namespace ESANMSA.Areas.Seguridad.Controllers
{
    public class PrincipalController : Controller
    {
        // GET: Seguridad/Principal
        [NoCache]
        public ActionResult Index()
        {
            if (Session["Usuario"] != null)
            {
                return View();
            }else
            {
                return RedirectToAction("Login", "Acceso", new { area = "Seguridad" });
            }           
        }
    }
}
using ESAN.Componentes.CoreEvaluacion.Logic.Facade.EvaluacionMSA;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ESANMSA.Areas.Administrador.Controllers
{
    public class LinkController : Controller
    {
        // GET: Administrador/Link
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult ListadoPromocion()
        {
            ViewBag.ListadoPromocion = DAPromocion.Listado(-1);
            return PartialView();
        }
    }
}
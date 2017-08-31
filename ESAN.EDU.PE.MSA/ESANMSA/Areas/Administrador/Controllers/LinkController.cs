using ESAN.Componentes.CoreEvaluacion.Logic.Facade.EvaluacionMSA;
using ESAN.Componentes.CoreEvaluacion.Models.General.EvaluacionMSA;
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

        public ActionResult ListadoParticipante(int EvaluacionPromocionID)
        {
            ViewBag.ListadoPromocionCiclo = DAPromocionCiclo.Listado(EvaluacionPromocionID);
            return PartialView();
        }
        [HttpPost]
        public JsonResult ListadoMedicion(int EvaluacionPromocionID, int EvaluacionCicloID)
        {
            List<EvaluacionPromocionMedicion> listado = DAPromocionMedicion.Listado(EvaluacionPromocionID, EvaluacionCicloID);
            return Json(new { listamedicion = listado }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult ListadoParticipanteDetalle(int EvaluacionPromocionID, int EvaluacionMedicionID, int EvaluacionCicloID)
        {
            ViewBag.ListadoPromocionParticipante = DAPromocion.ListadoParticipanteLink(EvaluacionPromocionID, EvaluacionMedicionID, EvaluacionCicloID);
            ViewBag.EvaluacionMedicionID = EvaluacionMedicionID;
            ViewBag.ListadoEvaluadores = DAPromocion.ListadoPersonasEvaluaron(EvaluacionPromocionID, EvaluacionMedicionID);
            return PartialView();
        }


    }
}
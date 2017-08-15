using ESAN.Componentes.CoreEvaluacion.Logic.Facade.EvaluacionMSA;
using ESAN.Componentes.CoreEvaluacion.Models.General.EvaluacionMSA;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ESANMSA.Areas.Administrador.Controllers
{
    public class PromocionController : Controller
    {
        // GET: Administrador/Promocion
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult FormPromocion()
        {
            ViewBag.ListadoEvaluacion = DAEvaluacion.Listado();
            return PartialView();
        }

        public JsonResult GrabarPromocion(EvaluacionPromocion promo)
        {
            bool exito = DAPromocion.Registrar(promo);
            string mensaje = exito ? "La promoción se registró satisfactoriamente" : "La operación no se pudo completar. Intente nuevamente.";
            return Json(mensaje, JsonRequestBehavior.AllowGet);
        }

        [System.Web.Mvc.HttpGet]
        public JsonResult ObtenerDetalleEvaluacion(int EvaluacionID)
        {
            List<EvaluacionCiclo> lista = DACiclo.Listado(EvaluacionID);
            return Json("ssss", JsonRequestBehavior.AllowGet);
        }

        public ActionResult ListadoPromocion()
        {
            ViewBag.ListadoPromocion = DAPromocion.Listado(-1);
            return PartialView();
        }

        public ActionResult ListadoMedicion(int EvaluacionPromocionID)
        {
            ViewBag.ListadoPromocion = DAPromocion.Listado(EvaluacionPromocionID);
            ViewBag.ListadoCiclo = DAPromocionCiclo.Listado(EvaluacionPromocionID);
            ViewBag.ListadoMedicion = DAPromocionMedicion.Listado(-1, -1);
            return PartialView();
        }

        public JsonResult GrabarFechaMedicion(string tipo, DateTime? fecha, int PromocionID, int CicloID, int MedicionID)
        {
            bool exito = DAPromocionMedicion.Actualizar(tipo, fecha, PromocionID, CicloID, MedicionID);
            string mensaje = exito ? "Se actualizó satisfactoriamente" : "La operación no se pudo completar. Intente nuevamente.";
            return Json(mensaje, JsonRequestBehavior.AllowGet);
        }
    }
}
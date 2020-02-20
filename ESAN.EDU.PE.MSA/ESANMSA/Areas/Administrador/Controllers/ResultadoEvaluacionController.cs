using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ESAN.Componentes.CoreEvaluacion.Logic.Facade.EvaluacionMSA;
using ESAN.Componentes.CoreEvaluacion.Models.General.EvaluacionMSA;
using Newtonsoft.Json;
using System.Text;
using ESANMSA.Utilitarios;

namespace ESANMSA.Areas.Administrador.Controllers
{
    public class ResultadoEvaluacionController : Controller
    {
        // GET: Administrador/ResultadoEvaluacion

        [NoCache]
        public ActionResult frmPromociones()
        {
            if (Session["Usuario"] != null)
            {
                ViewBag.lstPromocion = DAPromocion.ListaPromocionEvaluacionActivas();
                return View();
            }
            else
            {
                return RedirectToAction("Login", "Acceso", new { area = "Seguridad" });
            }
            
        }
        
        [HttpPost]
        public ActionResult ParticipantesPromocion(int p_idPromocion) {
            ViewBag.lstCiclo = DAPromocionCiclo.Listado(p_idPromocion);
            ViewBag.idPromocion = p_idPromocion;
            return PartialView("_PartialParticipantes");
        }

        [HttpPost]
        public ActionResult ListaParticipantesPromocionCiclo(int p_idPromocion, int p_idCiclo) {
            ViewBag.lstParticipantes = DAParticipante.ListaParticipantes(p_idPromocion, p_idCiclo);
            ViewBag.idCiclo = p_idCiclo;
            return PartialView("_ListaParticipantes");
        }

        [HttpGet]
        public ActionResult ResultadoParticipante(int idEvaluacion, int idPromocion, int idParticipante,int idCiclo) 
        {
            string ExisteInfoReporte = "0";
            List<PromedioEvaluacionxCicloNivel_Result> lstResultA = DAParticipante.ObtenerResultadoFinalxParticipante(idEvaluacion, 1, idPromocion, idCiclo, idParticipante).ToList();
            List<PromedioEvaluacionxCicloNivel_Result> lstResultB = DAParticipante.ObtenerResultadoFinalxParticipante(idEvaluacion, 2, idPromocion, idCiclo, idParticipante).ToList();
            List<PromedioEvaluacionxCicloNivel_Result> lstResultC = DAParticipante.ObtenerResultadoFinalxParticipante(idEvaluacion, 3, idPromocion, idCiclo, idParticipante).ToList();
            if (lstResultA.Count > 0 && lstResultB.Count > 0 && lstResultC.Count > 0) {
                ViewBag.UniversoA = lstResultA.FirstOrDefault().CantParticipantes.ToString();
                ViewBag.UniversoB = lstResultB.FirstOrDefault().CantParticipantes.ToString();
                ViewBag.UniversoC = lstResultC.FirstOrDefault().CantParticipantes.ToString();
                ViewBag.lstResultA = lstResultA;
                ViewBag.lstResultB = lstResultB;
                ViewBag.lstResultC = lstResultC;
                ExisteInfoReporte = "1";
            }
            ViewBag.ExisteInfoReporte = ExisteInfoReporte;
            ViewBag.objParticipante = DAParticipante.ObtenerParticipantexID(idParticipante);
            ViewBag.objEvaluacion = DAEvaluacion.ObtenerEvaluacion(idPromocion);
            return new Rotativa.ViewAsPdf("ResultadoParticipante")
            {
                FileName = "TestViewAsPdf.pdf",
                PageSize = Rotativa.Options.Size.A4,
                PageMargins = new Rotativa.Options.Margins(27, 25, 25, 25)
            };
        }

        public ActionResult ExportarPDF()
        {
            return new Rotativa.ViewAsPdf("ExportarPDF")
            {

                FileName = "TestViewAsPdf.pdf",
                PageSize = Rotativa.Options.Size.A4,
                PageMargins = new Rotativa.Options.Margins(27, 25, 25, 25)
            };
        }
    }
}
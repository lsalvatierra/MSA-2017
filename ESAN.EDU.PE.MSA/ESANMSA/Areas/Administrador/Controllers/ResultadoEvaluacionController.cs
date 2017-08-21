using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ESAN.Componentes.CoreEvaluacion.Logic.Facade.EvaluacionMSA;
using ESAN.Componentes.CoreEvaluacion.Models.General.EvaluacionMSA;
using Newtonsoft.Json;

namespace ESANMSA.Areas.Administrador.Controllers
{
    public class ResultadoEvaluacionController : Controller
    {
        // GET: Administrador/ResultadoEvaluacion
        public ActionResult frmPromociones()
        {
            ViewBag.lstPromocion = DAPromocion.ListaPromocionEvaluacionActivas();
            return View();
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
            return PartialView("_ListaParticipantes");
        }

        [HttpGet]
        public ActionResult ResultadoParticipante(int idMedicion, int idPromocion, int idParticipante) 
        {
            ViewBag.lstResultAA = DAParticipante.ObtenerResultadoxParticipante(idPromocion, idMedicion, idParticipante).Where(q => q.IdNivelA == 1 && q.IdNivelB == 7).ToList();
            ViewBag.lstResultAB = DAParticipante.ObtenerResultadoxParticipante(idPromocion, idMedicion, idParticipante).Where(q => q.IdNivelA == 1 && q.IdNivelB == 8).ToList();
            ViewBag.lstResultAC = DAParticipante.ObtenerResultadoxParticipante(idPromocion, idMedicion, idParticipante).Where(q => q.IdNivelA == 1 && q.IdNivelB == 9).ToList();

            ViewBag.lstResultB = DAParticipante.ObtenerResultadoxParticipante(idPromocion, idMedicion, idParticipante).Where(q => q.IdNivelA == 2).ToList();

            ViewBag.lstResultCA = DAParticipante.ObtenerResultadoxParticipante(idPromocion, idMedicion, idParticipante).Where(q => q.IdNivelA == 3 && q.IdNivelB == 15).ToList();
            ViewBag.lstResultCB = DAParticipante.ObtenerResultadoxParticipante(idPromocion, idMedicion, idParticipante).Where(q => q.IdNivelA == 3 && q.IdNivelB == 16).ToList();
            Response.HeaderEncoding = System.Text.Encoding.Default;
            return new Rotativa.ViewAsPdf("ResultadoParticipante", null)
            {
                
                FileName = "TestViewAsPdf.pdf",
                PageSize = Rotativa.Options.Size.A4,
                PageMargins = new Rotativa.Options.Margins(27, 25, 25, 25)
            };
            //return View();
        }
    }
}
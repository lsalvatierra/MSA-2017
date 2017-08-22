using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ESAN.Componentes.CoreEvaluacion.Logic.Facade.EvaluacionMSA;
using ESAN.Componentes.CoreEvaluacion.Models.General.EvaluacionMSA;
using System.Globalization;

namespace ESANMSA.Areas.Alumno.Controllers
{
    public class RegistroController : Controller
    {
        // GET: Alumno/Registro
        public ActionResult Formulario(int idPromocion, int idMedicion,int idEvaluado, bool Externo)
        {
            EvaluacionPromocionMedicion objPromMed = DAParticipante.ObtenerEvaluacionPromocionMedicion(idPromocion, idMedicion);
            DateTime fechaActual = DateTime.ParseExact(DateTime.Now.ToShortDateString(), "dd/MM/yyyy",CultureInfo.InvariantCulture);
            DateTime fechaIniMed = DateTime.ParseExact(objPromMed.EvaluacionPromMedicionFecIni.Value.ToShortDateString(), "dd/MM/yyyy", CultureInfo.InvariantCulture);
            DateTime fechaFinMed = DateTime.ParseExact(objPromMed.EvaluacionPromMedicionFecFin.Value.ToShortDateString(), "dd/MM/yyyy", CultureInfo.InvariantCulture);
            ViewBag.IdPromocion = idPromocion;
            ViewBag.IdMedicion = idMedicion;
            ViewBag.EsExterno = Externo;
            ViewBag.IdEvaluado = idEvaluado;
            if (Externo)
            {
                Participante objParticipante = DAParticipante.ObtenerParticipantexID(idEvaluado);
                if (objParticipante == null)
                {
                    return RedirectToAction("FormularioError", "Registro", new { area = "Alumno", p_tipoError = 3 });
                }
                ViewBag.lstTipoRelacion = DATipoRelacionParticipante.ListaTipoRelacion();
            }
            if (objPromMed != null)
            {
                if (fechaActual > fechaIniMed && fechaActual < fechaFinMed) {

                    return View();
                }
                return RedirectToAction("FormularioError", "Registro", new { area = "Alumno", p_tipoError = 1 });
            }
            else {
                return RedirectToAction("FormularioError", "Registro", new { area = "Alumno", p_tipoError = 2 });
            }
        }

        public ActionResult FormularioError(int p_tipoError) {
            string mensajeError = "";
            switch (p_tipoError) {
                case 1: mensajeError = "No se encuentra en el rango de fechas para la evaluación."; break;
                case 2: mensajeError = "No existe evaluación."; break;
                case 3: mensajeError = "No existe el participante a evalular."; break;
                default: mensajeError = "Error desconocido."; break;
            }
            ViewBag.mensajeError = mensajeError;
            return View();

        }

        [HttpPost]
        public JsonResult VerificarUsuario(int p_idTipoDocumento, string p_nroDocumento, int p_idPromocion, int p_idMedicion)
        {
            int rptaExiste =-1;
            Participante objParticipante = DAParticipante.ObtenerParticipante(p_idTipoDocumento, p_nroDocumento);
            if(objParticipante != null)
            {
                int rptaParticipante = DAParticipante.ObtenerParticipantePromocionYRespuestas(Convert.ToInt32(objParticipante.ParticipanteID), p_idPromocion, p_idMedicion);
                if (rptaParticipante == 1)
                {
                    rptaExiste = 1;
                }
                else {
                    rptaExiste = 0;
                }
                Session["Alumno"] = objParticipante;
            }
            return Json(new { Existe = rptaExiste });
        }

        [HttpPost]
        public JsonResult RegistrarUsuario(int p_idTipoDocumento, string p_nroDocumento, string p_apePaterno, string p_apeMaterno, string p_nombres,
            int p_idPromocion, int p_idMedicion, bool p_Externo)
        {
            Participante objParticipante = new Participante();
            objParticipante.TipoDocumentoID = p_idTipoDocumento;
            objParticipante.ParticipanteNroDoc = p_nroDocumento;
            objParticipante.ParticipanteApePaterno = p_apePaterno;
            objParticipante.ParticipanteApeMaterno = p_apeMaterno;
            objParticipante.ParticipanteNombres = p_nombres;
            objParticipante.ParticipanteNombreCompleto = p_apePaterno + " " + p_apeMaterno + " " + p_nombres;
            objParticipante.ParticipanteEstado = true;
            objParticipante.EvaluacionPromocionParticipante.Add(new EvaluacionPromocionParticipante {
                EvaluacionMedicionID = p_idMedicion,
                EvaluacionPromocionID = p_idPromocion,
                EsExterno = p_Externo
            });
            int rptaReg = DAParticipante.RegistrarParticipante(objParticipante);
            bool rpta = false; 
            if (rptaReg > 0)
            {
                rpta = true;
                objParticipante.ParticipanteID = rptaReg;
                Session["Alumno"] = objParticipante;
            }
            return Json(new { rpta = rpta });
        }


        [HttpPost]
        public JsonResult RegistrarUsuarioExterno(int p_idPromocion, int p_idMedicion, bool p_Externo, int p_idTipoRelacion)
        {
            Participante objParticipante = new Participante();
            objParticipante.TipoRelacionId = p_idTipoRelacion;
            objParticipante.ParticipanteEstado = true;
            objParticipante.EvaluacionPromocionParticipante.Add(new EvaluacionPromocionParticipante
            {
                EvaluacionMedicionID = p_idMedicion,
                EvaluacionPromocionID = p_idPromocion,
                EsExterno = p_Externo
            });
            int rptaReg = DAParticipante.RegistrarParticipante(objParticipante);
            bool rpta = false;
            if (rptaReg > 0)
            {
                rpta = true;
                objParticipante.ParticipanteID = rptaReg;
                Session["Alumno"] = objParticipante;
            }
            return Json(new { rpta = rpta });
        }
    }
}
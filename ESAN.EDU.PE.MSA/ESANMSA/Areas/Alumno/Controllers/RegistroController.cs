using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ESAN.Componentes.CoreEvaluacion.Logic.Facade.EvaluacionMSA;
using ESAN.Componentes.CoreEvaluacion.Models.General.EvaluacionMSA;
using System.Globalization;
using ESANMSA.Utilitarios;
using System.Web.Configuration;
using System.Configuration;

namespace ESANMSA.Areas.Alumno.Controllers
{
    public class RegistroController : Controller
    {
        // GET: Alumno/Registro
        [NoCache]
        public ActionResult Formulario(int idPromocion, int idMedicion,int idEvaluado, bool Externo)
        {
            EvaluacionPromocionMedicion objPromMed = DAParticipante.ObtenerEvaluacionPromocionMedicion(idPromocion, idMedicion);
           
            if (objPromMed.EvaluacionPromMedicionFecIni != null && objPromMed.EvaluacionPromMedicionFecFin != null)
            {
                DateTime fechaActual = ValidarFechaCorrecta(DateTime.Now.ToShortDateString()); //DateTime.ParseExact(DateTime.Now.ToShortDateString(), "dd/MM/yyyy",CultureInfo.InvariantCulture);
                DateTime fechaIniMed = ValidarFechaCorrecta(objPromMed.EvaluacionPromMedicionFecIni.Value.ToShortDateString()); //DateTime.ParseExact(objPromMed.EvaluacionPromMedicionFecIni.Value.ToShortDateString(), "dd/MM/yyyy", CultureInfo.InvariantCulture);
                DateTime fechaFinMed = ValidarFechaCorrecta(objPromMed.EvaluacionPromMedicionFecFin.Value.ToShortDateString()); //DateTime.ParseExact(objPromMed.EvaluacionPromMedicionFecFin.Value.ToShortDateString(), "dd/MM/yyyy", CultureInfo.InvariantCulture);
                ViewBag.IdPromocion = idPromocion;
                ViewBag.IdMedicion = idMedicion;
                ViewBag.EsExterno = Externo;
                ViewBag.IdEvaluado = idEvaluado;
                ViewBag.IdTipoDocumento = Convert.ToInt32(ConfigurationManager.AppSettings["IdTipoDocumentoDefault"].ToString());//Variable del Web.config
                if (Externo)
                {
                    Participante objParticipante = DAParticipante.ObtenerParticipantexID(idEvaluado);
                    if (objParticipante == null)
                    {
                        return RedirectToAction("FormularioError", "Registro", new { area = "Alumno", p_tipoError = 3 });
                    }
                    ViewBag.IdTipoRelacionOtros = Convert.ToInt32(ConfigurationManager.AppSettings["IdTipoRelacionOtro"].ToString());//Variable del Web.config
                    ViewBag.lstTipoRelacion = DATipoRelacionParticipante.ListaTipoRelacion();
                }
                if (objPromMed != null)
                {
                    if (fechaActual >= fechaIniMed && fechaActual <= fechaFinMed)
                        return View();
                    else 
                        return RedirectToAction("FormularioError", "Registro", new { area = "Alumno", p_tipoError = 1 });
                }
                else
                    return RedirectToAction("FormularioError", "Registro", new { area = "Alumno", p_tipoError = 2 });
            }
            else {
                return RedirectToAction("FormularioError", "Registro", new { area = "Alumno", p_tipoError = 5 });
            }
            

        }

        public ActionResult FormularioError(int p_tipoError) {
            string mensajeError = "";
            switch (p_tipoError) {
                case 1: mensajeError = "No se encuentra en el rango de fechas para la evaluación."; break;
                case 2: mensajeError = "No existe evaluación."; break;
                case 3: mensajeError = "No existe el participante a evalular."; break;
                case 4: mensajeError = "Ud. ya está encuentra realizando la evaluación desde otra ventana del navegador."; break;
                case 5: mensajeError = "No existe fecha programada para la evaluación."; break;
                default: mensajeError = "Error desconocido."; break;
            }
            ViewBag.mensajeError = mensajeError;
            return View();

        }

        [HttpPost]
        public JsonResult VerificarUsuario(int p_idTipoDocumento, string p_nroDocumento, int p_idPromocion, int p_idMedicion)
        {
            int rptaExiste =-1;
            if (DAPromocionMedicionCicloParticipante.ExisteParticipantePromocionCicloMedicion(p_idPromocion, p_idMedicion, p_nroDocumento)) {
                Participante objParticipante = DAParticipante.ObtenerParticipante(p_idTipoDocumento, p_nroDocumento);
                if (objParticipante != null)
                {
                    int rptaParticipante = DAParticipante.ObtenerParticipantePromocionYRespuestas(Convert.ToInt32(objParticipante.ParticipanteID), p_idPromocion, p_idMedicion);
                    if (rptaParticipante == 1)
                    {
                        rptaExiste = 1;
                    }
                    else
                    {
                        Participante objNuevoParticipante = new Participante
                        {
                            ParticipanteID = objParticipante.ParticipanteID,
                            ParticipanteNombreCompleto = objParticipante.ParticipanteNombreCompleto
                        };
                        objNuevoParticipante.EvaluacionPromocionParticipante.Add(new EvaluacionPromocionParticipante
                        {
                            ParticipanteID = objParticipante.ParticipanteID,
                            EvaluacionMedicionID = p_idMedicion,
                            EvaluacionPromocionID = p_idPromocion,
                            EsExterno = false // Siempre falso por que sólo se verifica a los internos.
                        });
                        rptaExiste = 0;
                        Session["Alumno"] = objNuevoParticipante;
                    }
                }
            }else
                rptaExiste = -2;
           
            return Json(new { Existe = rptaExiste });
        }

        [HttpPost]
        public JsonResult RegistrarUsuario(int p_idTipoDocumento, string p_nroDocumento, string p_apePaterno, string p_apeMaterno, string p_nombres,
            int p_idPromocion, int p_idMedicion)
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
                EsExterno = false
            });
            Session["Alumno"] = objParticipante;
            return Json(new { rpta = true });
        }


        [HttpPost]
        public JsonResult RegistrarUsuarioExterno(int p_idPromocion, int p_idMedicion, int p_idTipoRelacion, string p_TipoRelacion)
        {
            Participante objParticipante = new Participante();
            if (p_idTipoRelacion == Convert.ToInt32(ConfigurationManager.AppSettings["IdTipoRelacionOtro"].ToString())) // Variable de Web.config
            {
                objParticipante.TipoRelacionParticipante = new TipoRelacionParticipante { DescripcionTipoRelacion = p_TipoRelacion, EstadoTipoRelacion = "0" };
            }
            else {
                objParticipante.TipoRelacionId = p_idTipoRelacion;
            }
            
            objParticipante.ParticipanteEstado = true;
            objParticipante.EvaluacionPromocionParticipante.Add(new EvaluacionPromocionParticipante
            {
                EvaluacionMedicionID = p_idMedicion,
                EvaluacionPromocionID = p_idPromocion,
                EsExterno = true
            });
            Session["Alumno"] = objParticipante;
            return Json(new { rpta = true });
        }

        /// <summary>
        /// Formatear fecha en formato dd/mm/aaaa
        /// </summary>
        /// <param name="p_fecha"></param>
        /// <returns></returns>
        private DateTime ValidarFechaCorrecta(string p_fecha) {
            string[] partesfecha = p_fecha.Split('/');
            string fechaCompleta = "";
            foreach (string partefecha in partesfecha) {
                if (partefecha.Length == 1)
                    fechaCompleta += "0" + partefecha + "/";
                else
                    fechaCompleta += partefecha + "/";
            }
            string fechaCompletaFinal = fechaCompleta.Substring(0, fechaCompleta.Length - 1);
            DateTime fechaCorrecta =  DateTime.ParseExact(fechaCompletaFinal, "dd/MM/yyyy", CultureInfo.InvariantCulture);
            return fechaCorrecta;
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ESAN.Componentes.CoreEvaluacion.Logic.Facade.EvaluacionMSA;
using ESAN.Componentes.CoreEvaluacion.Models.General.EvaluacionMSA;

namespace ESANMSA.Areas.Alumno.Controllers
{
    public class RegistroController : Controller
    {
        // GET: Alumno/Registro
        public ActionResult Formulario(int IdPromocion, int IdMedicion, bool Externa)
        {
            ViewBag.IdPromocion = IdPromocion;
            ViewBag.IdMedicion = IdMedicion;
            ViewBag.EsExterno = Externa;
            return View();
        }

        [HttpPost]
        public JsonResult VerificarUsuario(int p_idTipoDocumento, string p_nroDocumento)
        {
            bool rptaExiste = false;
            Participante objParticipante = DAParticipante.ObtenerParticipante(p_idTipoDocumento, p_nroDocumento);
            if(objParticipante != null)
            {
                rptaExiste = true;
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
            objParticipante.EsExterno = p_Externo;
            objParticipante.ParticipanteEstado = true;
            int rptaReg = DAParticipante.RegistrarParticipante(objParticipante, p_idPromocion);
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
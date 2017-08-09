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
        public ActionResult Index()
        {
            ViewBag.lstTipoDoc = DATipoDocumento.ListaTipoDocumentos();
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
    }
}
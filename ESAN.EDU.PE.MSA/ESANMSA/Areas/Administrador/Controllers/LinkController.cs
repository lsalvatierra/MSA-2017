using ESAN.Componentes.CoreEvaluacion.Email;
using ESAN.Componentes.CoreEvaluacion.Logic.Facade.EvaluacionMSA;
using ESAN.Componentes.CoreEvaluacion.Models.General.EvaluacionMSA;
using ESANMSA.Utilitarios;
using System;
using System.Collections.Generic;
using System.Configuration;
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
        [HttpPost]
        public JsonResult EnviarMail(string destinatario)
        {
            //var alumno = (Alumno)Session["Alumno"];
            //var alumnoCompleto = FC.Actor.ObtenerActor(alumno.IdActor);
            //List<string> listaCorreos = new List<string>();
            //listaCorreos.Add(alumnoCompleto.Usuario + "@ue.edu.pe");
            //if (!string.IsNullOrEmpty(alumnoCompleto.EMail))
            //{
            //    listaCorreos.Add(alumnoCompleto.EMail);
            //}

            //if (!string.IsNullOrEmpty(alumnoCompleto.EMailAdicional))
            //{
            //    listaCorreos.Add(alumnoCompleto.EMailAdicional);
            //}

            //if (!string.IsNullOrEmpty(alumnoCompleto.EMailOpcional))
            //{
            //    listaCorreos.Add(alumnoCompleto.EMailOpcional);
            //}
            //listaCorreos.Add("lsalvatierra@esan.edu.pe");
            ViewBag.Email = "elcorreo@esan.edu.pe";
            using (EmailProvider provider = EmailFactory.GetEmailProvider(
                                            EmailFactory.Providers.Default,
                                            ConfigurationManager.AppSettings["EnvioMailCompromisoAlumno"]))
            {
                //foreach (string email in listaCorreos)
                //{
                //    provider.AgregarDireccion(TipoDirecciones.To, email);
                //}
                provider.AgregarDireccion(TipoDirecciones.To, destinatario);
                provider.Enviar(
                    HttpUtility.HtmlDecode(
                        General.RenderPartialViewToString(this,
                            "~/Areas/Administrador/Views/Link/Email.cshtml"
                             , ViewBag))
                    , true
                    , System.Net.Mail.MailPriority.Normal);
            }

            return Json(new { listamedicion = "" }, JsonRequestBehavior.AllowGet);
        }


    }
}
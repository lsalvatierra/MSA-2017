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
            if (Session["Usuario"] != null)
            {
                return View();
            }
            else
            {
                return RedirectToAction("Login", "Acceso", new { area = "Seguridad" });
            }
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
        public JsonResult EnviarMail(int EvaluacionPromocionID, int EvaluacionMedicionID, bool EsEvaluado)
        {
            //Se obtiene el listado de alumnos para el envío de correo
            List<PromocionMedicionCicloParticipante> listadoParticipantes = DAPromocionMedicionCicloParticipante.Listado(EvaluacionPromocionID, EvaluacionMedicionID);

            string evaluado = "&idEvaluado=0&Externo=False";
            string link = string.Empty;
            string rutaCorreo = EsEvaluado ? "~/Areas/Administrador/Views/Link/EmailExterno.cshtml" : "~/Areas/Administrador/Views/Link/Email.cshtml";
            int participanteID = 0;
            string msjeExito = "Se enviaron los correos a cada participante";

            try
            {
                foreach (PromocionMedicionCicloParticipante participante in listadoParticipantes)
                {
                    using (EmailProvider provider = EmailFactory.GetEmailProvider(
                                                EmailFactory.Providers.Default,
                                                ConfigurationManager.AppSettings["EnvioMailCompromisoAlumno"]))
                    {
                        if (EsEvaluado)
                        {
                            participanteID = (int)DAParticipante.ObtenerParticipante(Convert.ToInt32(ConfigurationManager.AppSettings["IdTipoDocumentoDefault"].ToString()), participante.ParticipanteNroDoc).ParticipanteID;
                            evaluado = "&idEvaluado=" + participanteID + "&Externo=true";
                        }

                        link = "http://msa.esan.edu.pe/Alumno/Registro/Formulario?idPromocion=" + EvaluacionPromocionID.ToString() +
                                      "&idMedicion=" + EvaluacionMedicionID.ToString() + evaluado;
                        ViewBag.LinkEval = link;
                        ViewBag.LinkVideo = participante.DireccionVideo;

                        provider.AgregarDireccion(TipoDirecciones.To, ConfigurationManager.AppSettings["EsPrueba"] == "1" ? ConfigurationManager.AppSettings["CorreoPrueba"] : participante.ParticipanteNroDoc + ConfigurationManager.AppSettings["DominioCorreoEnvio"]);

                        provider.Enviar(
                            HttpUtility.HtmlDecode(
                                General.RenderPartialViewToString(this,
                                    rutaCorreo
                                     , ViewBag))
                            , true
                            , System.Net.Mail.MailPriority.Normal);
                    }
                }
            }
            catch 
            {
                msjeExito = "A ocurrido un error al enviar el correo.";
            }


           

            return Json(new { exito = msjeExito}, JsonRequestBehavior.AllowGet);
        }


    }
}
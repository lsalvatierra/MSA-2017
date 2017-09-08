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
            //String[] correos = new string[4];
            //correos[0] = "lchang@esan.edu.pe";
            //correos[1] = "1302177@esan.edu.pe";
            //correos[2] = "luis.chang@outlook.com";
            //correos[3] = "lchang86@gmail.com";

            List<PromocionMedicionCicloParticipante> listadoParticipantes = DAPromocionMedicionCicloParticipante.Listado(EvaluacionPromocionID, EvaluacionMedicionID);

            //ViewBag.Email = "elcorreo@esan.edu.pe";
            string evaluado = string.Empty;
            string link = string.Empty;
            string rutaCorreo = EsEvaluado ? "~/Areas/Administrador/Views/Link/EmailExterno.cshtml" : "~/Areas/Administrador/Views/Link/Email.cshtml";
            foreach (PromocionMedicionCicloParticipante participante in listadoParticipantes)
            {
                using (EmailProvider provider = EmailFactory.GetEmailProvider(
                                            EmailFactory.Providers.Default,
                                            ConfigurationManager.AppSettings["EnvioMailCompromisoAlumno"]))
                {
                    int participanteID = (int)DAParticipante.ObtenerParticipante(Convert.ToInt32(ConfigurationManager.AppSettings["IdTipoDocumentoDefault"].ToString()), participante.ParticipanteNroDoc).ParticipanteID;
                    evaluado = EsEvaluado ? "&idEvaluado=" + participanteID + "&Externo=true" : "&idEvaluado=0&Externo=False";
                    link = "http://msa.esan.edu.pe/Alumno/Registro/Formulario?idPromocion=" + EvaluacionPromocionID.ToString() +
                                  "&idMedicion=" + EvaluacionMedicionID.ToString() + evaluado;
                    ViewBag.LinkEval = link;
                    ViewBag.LinkVideo = participante.DireccionVideo;

                    provider.AgregarDireccion(TipoDirecciones.To, ConfigurationManager.AppSettings["EsPrueba"] == "1" ? "1302177@esan.edu.pe" : participante.ParticipanteNroDoc + "@esan.edu.pe");

                    provider.Enviar(
                        HttpUtility.HtmlDecode(
                            General.RenderPartialViewToString(this,
                                rutaCorreo
                                 , ViewBag))
                        , true
                        , System.Net.Mail.MailPriority.Normal);
                }
            }

            return Json(new { listamedicion = "" }, JsonRequestBehavior.AllowGet);
        }


    }
}
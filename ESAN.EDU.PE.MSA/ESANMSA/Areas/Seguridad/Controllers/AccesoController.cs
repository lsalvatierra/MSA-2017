using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using ESANMSA.Utilitarios;
using ESAN.Componentes.CoreEvaluacion.Models.General.EvaluacionMSA;
using ESAN.Componentes.CoreEvaluacion.Logic.Facade.EvaluacionMSA;

namespace ESANMSA.Areas.Seguridad.Controllers
{
    public class AccesoController : Controller
    {

        public static string msgTrans;

        // GET: Seguridad/Acceso

        [NoCache]
        public ActionResult Login()
        {
            if (Session["Usuario"] != null)
            {
                Session.Remove("Usuario");
                Session.Abandon();
                FormsAuthentication.SignOut();
            }
            
            ViewBag.Mensaje = msgTrans;
            
            return View();
        }



        [HttpPost]
        public ActionResult AccederSistema(FormCollection form)
        {
            Session.Remove("Usuario");
            string username = form["txtUsuario"].Trim();
            string password = form["txtPassword"].Trim();
            EvaluacionUsuario objUsuario = DAEvaluacionUsuario.ObtenerUsuario(username,password);
            if (objUsuario != null)
            {
                FormsAuthentication.SetAuthCookie(username, false);
                Session["Usuario"] = objUsuario;
                return RedirectToAction("Index", "Principal", new { area = "Seguridad" });
            }
            else
            {
                msgTrans = "Datos Incorrectos!";
                return RedirectToAction("Login", "Acceso", new { area = "Seguridad" });
            }
        }



        public ActionResult Logout()
        {
            msgTrans = null;
            Session.Remove("Usuario");
            Session.Abandon();
            FormsAuthentication.SignOut();
            return RedirectToAction("Login", "Acceso", new { area = "Seguridad" });
        }


    }
}
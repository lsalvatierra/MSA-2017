using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Configuration;
using System.Web.Mvc;
using System.Text;
using System.IO;
using System.Web.UI;

namespace ESANMSA.Utilitarios
{
    public class General
    {
        static public int ERROR = 1;
        static public int EXITO = 1;
        static public int NOSESION = 1;
        static public int SINACCESO = 1;

        static public string TEMPLATE = ConfigurationManager.AppSettings["Template"];

        //static public string RenderPartialToString(string controlName, object viewData)
        //{
        //    ViewPage viewPage = new ViewPage() { ViewContext = new ViewContext() };

        //    viewPage.ViewData = new ViewDataDictionary(viewData);
        //    viewPage.Controls.Add(viewPage.LoadControl(controlName));

        //    StringBuilder sb = new StringBuilder();
        //    using (StringWriter sw = new StringWriter(sb))
        //    {
        //        using (HtmlTextWriter tw = new HtmlTextWriter(sw))
        //        {
        //            viewPage.RenderControl(tw);
        //        }
        //    }
        //    return sb.ToString();
        //}
        static public string RenderPartialToString(string controlName, object viewData)
        {
            ViewPage viewPage = new ViewPage() { ViewContext = new ViewContext() };

            viewPage.ViewData = new ViewDataDictionary(viewData);
            viewPage.Controls.Add(viewPage.LoadControl(controlName));

            StringBuilder sb = new StringBuilder();
            using (StringWriter sw = new StringWriter(sb))
            {
                using (HtmlTextWriter tw = new HtmlTextWriter(sw))
                {
                    viewPage.RenderControl(tw);
                }
            }
            return sb.ToString();
        }

        static public string RenderPartialViewToString(ControllerBase control, string viewName, object model)
        {
            if (string.IsNullOrEmpty(viewName))
                viewName = control.ControllerContext.RouteData.GetRequiredString("action");

            control.ViewData.Model = model;

            using (StringWriter sw = new StringWriter())
            {
                ViewEngineResult viewResult = ViewEngines.Engines.FindPartialView(control.ControllerContext, viewName);
                ViewContext viewContext = new ViewContext(control.ControllerContext, viewResult.View, control.ViewData, control.TempData, sw);
                viewResult.View.Render(viewContext, sw);

                return sw.GetStringBuilder().ToString();
            }

        }

        static public void GuardarAcceso(string server,string mensaje) {
            try
            {
                string path = "e://Logs//";
                    


                    TextWriter tw = new StreamWriter(path + "accesos.txt", true);

                    // write a line of text to the file
                    tw.WriteLine(mensaje + ";" + server);

                    // close the stream
                    tw.Close();
                
            }
            catch { }        
        }
    }

    public class MensajeStatus
    {
        public int status;
        public string mensaje;

        static public MensajeStatus ERROR = new MensajeStatus(0, "Error");
        static public MensajeStatus EXITO = new MensajeStatus(1, "Exito");
        static public MensajeStatus ACCESONEGADO = new MensajeStatus(2, "Acceso negado");
        static public MensajeStatus SESSIONCADUCADO = new MensajeStatus(3, "La sesión caducó");

        public MensajeStatus(int _status, string _mensaje)
        {
            status = _status;
            mensaje = _mensaje;
        }
    }

    
}
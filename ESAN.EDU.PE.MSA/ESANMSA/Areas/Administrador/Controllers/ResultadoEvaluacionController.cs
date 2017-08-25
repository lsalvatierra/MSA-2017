using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ESAN.Componentes.CoreEvaluacion.Logic.Facade.EvaluacionMSA;
using ESAN.Componentes.CoreEvaluacion.Models.General.EvaluacionMSA;
using Newtonsoft.Json;
using System.Text;


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
            ViewBag.idCiclo = p_idCiclo;
            return PartialView("_ListaParticipantes");
        }

        [HttpGet]
        public ActionResult ResultadoParticipante(int idEvaluacion, int idPromocion, int idParticipante,int idCiclo) 
        {
            ViewBag.lstResultA = DAParticipante.ObtenerResultadoFinalxParticipante(idEvaluacion, 1, idPromocion, idCiclo, idParticipante).ToList();
            ViewBag.lstResultB = DAParticipante.ObtenerResultadoFinalxParticipante(idEvaluacion, 2, idPromocion, idCiclo, idParticipante).ToList();
            ViewBag.lstResultC = DAParticipante.ObtenerResultadoFinalxParticipante(idEvaluacion, 3, idPromocion, idCiclo, idParticipante).ToList();
            return new Rotativa.ViewAsPdf("ResultadoParticipante")
            {
                FileName = "TestViewAsPdf.pdf",
                PageSize = Rotativa.Options.Size.A4,
                PageMargins = new Rotativa.Options.Margins(27, 25, 25, 25)
            };
        }

        //public ActionResult ExportarPDF()
        //{
        //    // Render the view html to a string.
        //    string htmlText = RenderPartialViewToString(this, "ExportarPDF", null);

        //    // Let the html be rendered into a PDF document through iTextSharp.
        //    byte[] buffer = StandardPdfRenderer.Render(htmlText, "");

        //    // Return the PDF as a binary stream to the client.
        //    return new BinaryContentResult(buffer, "application/pdf");

        //    //return View();
        //}

        public ActionResult ExportarPDF()
        {
            return new Rotativa.ViewAsPdf("ExportarPDF")
            {

                FileName = "TestViewAsPdf.pdf",
                PageSize = Rotativa.Options.Size.A4,
                PageMargins = new Rotativa.Options.Margins(27, 25, 25, 25)
            };
        }

        //public static string RenderPartialViewToString(Controller controller, string viewName, object model)
        //{
        //    if (string.IsNullOrEmpty(viewName))
        //        viewName = controller.ControllerContext.RouteData.GetRequiredString("action");

        //    controller.ViewData.Model = model;

        //    using (var sw = new StringWriter())
        //    {
        //        var viewResult = ViewEngines.Engines.FindPartialView(controller.ControllerContext, viewName);
        //        var viewContext = new ViewContext(controller.ControllerContext, viewResult.View, controller.ViewData, controller.TempData, sw);
        //        viewResult.View.Render(viewContext, sw);

        //        return sw.GetStringBuilder().ToString();
        //    }
        }

    }
    //public class StandardPdfRenderer
    //{
    //    private const int HorizontalMargin = 40;
    //    private const int VerticalMargin = 40;

    //    public static byte[] Render(string htmlText, string pageTitle)
    //    {
    //        byte[] renderedBuffer;

    //        using (var outputMemoryStream = new MemoryStream())
    //        {
    //            using (var pdfDocument = new Document(PageSize.A4, HorizontalMargin, HorizontalMargin, VerticalMargin, VerticalMargin))
    //            {
    //                PdfWriter pdfWriter = PdfWriter.GetInstance(pdfDocument, outputMemoryStream);
    //                pdfWriter.CloseStream = false;
    //                pdfWriter.PageEvent = new PrintHeaderFooter { Title = pageTitle };
    //                pdfDocument.Open();
    //                using (var htmlViewReader = new StringReader(htmlText))
    //                {
    //                    using (var htmlWorker = new HTMLWorker(pdfDocument))
    //                    {
    //                        htmlWorker.Parse(htmlViewReader);
    //                    }
    //                }
    //            }

    //            renderedBuffer = new byte[outputMemoryStream.Position];
    //            outputMemoryStream.Position = 0;
    //            outputMemoryStream.Read(renderedBuffer, 0, renderedBuffer.Length);
    //        }

    //        return renderedBuffer;
    //    }
    //}
//}

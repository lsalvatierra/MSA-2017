using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ESAN.Componentes.CoreEvaluacion.Logic.Facade.EvaluacionMSA;
using ESAN.Componentes.CoreEvaluacion.Models.General.EvaluacionMSA;
using ESANMSA.Utilitarios;
using System.Configuration;

namespace ESANMSA.Areas.Alumno.Controllers
{
    public class EvaluacionController : Controller
    {

        // GET: Alumno/Evaluacion

        [NoCache]
        public ActionResult frmEvaluacion(int idMedicion, int idPromocion, int idEvaluado, bool Externo)
        {
            if (Session["Alumno"] != null)
            {
                InicializarVariablesSesiones();
                Evaluacion objEvaluacion = DAEvaluacion.ObtenerEvaluacion(idPromocion);
                ViewBag.NombreEvaluacion = objEvaluacion.EvaluacionDescripcion;
                ViewBag.IdMedicion = idMedicion;
                ViewBag.IdPromocion = idPromocion;
                ViewBag.EsExterno = Externo;
                ViewBag.IdEvaluado = idEvaluado;
                Participante objParticipante = null;
                List<EvaluacionNivel> lstEvaluacion;
                int cantidadPreguntasEvaluacion = 0;
                List<EvaluacionRespuesta> lstEvaluacionRespuesta = new List<EvaluacionRespuesta>();
                if (Externo)
                {
                    objParticipante = DAParticipante.ObtenerParticipantexID(idEvaluado);
                    if (objParticipante != null)
                        ViewBag.NombresParticipanteEvaluar = objParticipante.ParticipanteNombreCompleto;
                    else
                        return RedirectToAction("FormularioError", "Registro", new { area = "Alumno", p_tipoError = 3 });
                }
                else 
                    ViewBag.NombresParticipanteEvaluar = ((Participante)Session["Alumno"]).ParticipanteNombreCompleto;
                //Obtenemos toda la evaluación y se guarda a una variable global.
                if (((Participante)Session["Alumno"]).TipoRelacionId == Convert.ToInt32(ConfigurationManager.AppSettings["IdTipoRelacionComClase"].ToString()) || ((Participante)Session["Alumno"]).TipoDocumentoID == Convert.ToInt32(ConfigurationManager.AppSettings["IdTipoDocumentoDefault"].ToString()))
                {
                    lstEvaluacion = DAEvaluacionNivel.ListaNivelesxEvaluacion(idPromocion, 0);
                    cantidadPreguntasEvaluacion = DAEvaluacionPregunta.CantidadPreguntasxEvalucion(idPromocion);
                }
                else {
                    long[] lstNiveleExcluir = { Convert.ToInt32(ConfigurationManager.AppSettings["IdNivelePadreExcluir"].ToString()) };
                    string[] lstNiveles = ConfigurationManager.AppSettings["IdNivelesPreguntasExcluir"].ToString().Split('-');//{ 10,11,12,13,14 };
                    long?[] lstNivelePreguntaExcluir = new long? [lstNiveles.Length];
                    for (int x = 0; x < lstNiveles.Length; x++) {
                        lstNivelePreguntaExcluir[x] = Convert.ToInt32(lstNiveles[x].ToString());
                    }
                    lstEvaluacion = DAEvaluacionNivel.ListaNivelesxEvaluacionParaOtros(idPromocion, 0, lstNiveleExcluir);
                    cantidadPreguntasEvaluacion = DAEvaluacionPregunta.CantidadPreguntasxEvalucionOtros(idPromocion, lstNivelePreguntaExcluir);
                }                
                //De la evaluación se extrae los primeros objetos para esta primera pagina
                if (lstEvaluacion.FirstOrDefault() != null)
                {
                    //Cargamos en sesión la Evaluación para utilizarla en el controlador.
                    Session["LstEvaluacion"] = lstEvaluacion;
                    Session["cantidadPreguntasEvaluacion"] = cantidadPreguntasEvaluacion;
                    //Session["cantidadMostradas"] = 0;
                    //Session["cantidadIniPreg"] = 0;
                    //Session["cantidadResulta"] = 0;
                    Session["lstEvaluacionRespuesta"] = lstEvaluacionRespuesta;
                    ViewBag.NivelA = lstEvaluacion.FirstOrDefault().EvaluacionNivelDescripcion;
                    ViewBag.IDNivelA = lstEvaluacion.FirstOrDefault().EvaluacionNivelID;
                    ViewBag.IDNivelOrdenA = lstEvaluacion.FirstOrDefault().EvaluacionNivelOrden;
                    ViewBag.objIntroduccion = lstEvaluacion.FirstOrDefault().EvaluacionNivelIntro.FirstOrDefault();
                    if (lstEvaluacion.FirstOrDefault().EvaluacionNivel1.Count() > 0)
                    {
                        ViewBag.IDNivelOrdenB = lstEvaluacion.FirstOrDefault().EvaluacionNivel1.FirstOrDefault().EvaluacionNivelOrden;
                        ViewBag.IDNivelOrdenC = 0;
                    }
                    else
                    {
                        //Se coloca -1 cuando ya no existe más subniveles.
                        ViewBag.IDNivelOrdenB = -1;
                        ViewBag.IDNivelOrdenC = -1;
                    }
                }
                return View();
            }
            else {
                return RedirectToAction("Formulario","Registro",new { area="Alumno", idPromocion = idPromocion, idMedicion = idMedicion, idEvaluado = idEvaluado, Externo = Externo });
            }           
        }


        [HttpPost]
        public ActionResult alternativasNivel(int idNivel)
        {
            ViewBag.AlternativasNivel = DAEvaluacionNivelIntro.ObtenerEvaluacionNiveleIntro(idNivel).ListaAlternativas;
            return PartialView("_PartialAlternativas");
        }

        [HttpPost]
        public ActionResult agregarRespuestas(int idMedicion, int idPromocion, int idEvaluado, bool Externo, int idAlternativa, int idPregunta)
        {
            if (Session["Alumno"] != null)
            {
                if (((List<EvaluacionRespuesta>)Session["lstEvaluacionRespuesta"]).Count > 0)
                {
                    EvaluacionRespuesta objEvalRpta = ObtenerRespuesta(idPromocion, idMedicion, idPregunta, idEvaluado, Externo);
                    if (Externo)
                    {
                        if (objEvalRpta == null)
                            AgregarListaDeRespuestas(idPromocion, idMedicion, idAlternativa, idEvaluado, idPregunta);
                        else
                        {
                            ((List<EvaluacionRespuesta>)Session["lstEvaluacionRespuesta"]).Remove(objEvalRpta);
                            AgregarListaDeRespuestas(idPromocion, idMedicion, idAlternativa, idEvaluado, idPregunta);
                        }
                    }
                    else
                    {
                        if (objEvalRpta == null)
                            AgregarListaDeRespuestas(idPromocion, idMedicion, idAlternativa, 0, idPregunta);
                        else
                        {
                            ((List<EvaluacionRespuesta>)Session["lstEvaluacionRespuesta"]).Remove(objEvalRpta);
                            AgregarListaDeRespuestas(idPromocion, idMedicion, idAlternativa, 0, idPregunta);
                        }
                    }
                }
                else
                {
                    if (Externo)
                        AgregarListaDeRespuestas(idPromocion, idMedicion, idAlternativa, idEvaluado, idPregunta);
                    else
                        AgregarListaDeRespuestas(idPromocion, idMedicion, idAlternativa, 0, idPregunta);
                }
            }
            //else {
            //    lstEvaluacionRespuesta.Clear();
            //    cantidadMostradas = 0;
            //    cantidadIniPreg = 0;
            //    cantidadResulta = 0;
            //    Session.Abandon();
            //    Session.Remove("Alumno");
            //    return RedirectToAction("Formulario", "Registro", new { area = "Alumno", idPromocion = idPromocion, idMedicion = idMedicion, idEvaluado = idEvaluado, Externo = Externo });

            //}
            Session["cantidadResulta"] = ((List<EvaluacionRespuesta>)Session["lstEvaluacionRespuesta"]).Count;
            double avance = ((int)Session["cantidadResulta"] * 100) / (int)Session["cantidadPreguntasEvaluacion"];
            ViewBag.avanceEvaluacion = avance;
            return PartialView("_PartialPorcentajeAvance");
        }

        [HttpPost]
        public ActionResult siguientePaginaEvaluacion(int idNivelA, int idNivelOrdenA, int idNivelOrdenB, int idNivelOrdenC, bool Externo) {

            bool TieneIntroduccion = false;
            List<EvaluacionPregunta> lstPreguntas = new List<EvaluacionPregunta>();
            //De la evaluación se extrae los siguientes objetos para esta  página de la evaluación.
            EvaluacionNivel objNivelA = ((List<EvaluacionNivel>)Session["LstEvaluacion"]).Where(q => q.EvaluacionNivelID == idNivelA).FirstOrDefault();
            EvaluacionNivel objNivelB = null, objNivelC = null;            
            if (objNivelA != null)
            {
                if (idNivelOrdenC != -1 && idNivelOrdenB != -1)
                {
                    objNivelB = objNivelA.EvaluacionNivel1.Where(q => q.EvaluacionNivelPadreID == idNivelA && q.EvaluacionNivelOrden == idNivelOrdenB).FirstOrDefault();
                    if (objNivelB != null)
                    {
                        //Se busca el siguiente nivel C agregando en 1 al número de orden.
                        objNivelC = objNivelB.EvaluacionNivel1.Where(q => q.EvaluacionNivelPadreID == objNivelB.EvaluacionNivelID && q.EvaluacionNivelOrden == idNivelOrdenC + 1).FirstOrDefault();
                        if (objNivelC != null)
                        {
                            //Si existe se extrae las preguntas para ese nivel.
                            lstPreguntas = DAEvaluacionPregunta.ListaPreguntasxNivelEvaluacion(Convert.ToInt32(objNivelC.EvaluacionNivelID));
                        }
                        else
                        {
                            //Si no existe, se actualiza el Nivel B agregando 1 al número de orden.
                            objNivelB = objNivelA.EvaluacionNivel1.Where(q => q.EvaluacionNivelPadreID == idNivelA && q.EvaluacionNivelOrden == idNivelOrdenB + 1).FirstOrDefault();
                            if (objNivelB != null)
                            {
                                //Si existe el Nivel B, se extrae el primer elemento del Nivel C.
                                objNivelC = objNivelB.EvaluacionNivel1.Where(q => q.EvaluacionNivelPadreID == objNivelB.EvaluacionNivelID && q.EvaluacionNivelOrden == 1).FirstOrDefault();
                                if (objNivelC != null)
                                {
                                    //Si existe se extrae las preguntas para ese nivel.
                                    lstPreguntas = DAEvaluacionPregunta.ListaPreguntasxNivelEvaluacion(Convert.ToInt32(objNivelC.EvaluacionNivelID));
                                }
                                else
                                {
                                    objNivelC = null;
                                    //Si no existe se extrae las preguntas del Nivel anterior (Nivel B) .
                                    lstPreguntas = DAEvaluacionPregunta.ListaPreguntasxNivelEvaluacion(Convert.ToInt32(objNivelB.EvaluacionNivelID));
                                }
                            }
                            else {
                                //Si no existe, se actualiza el Nivel A agregando 1 al número de orden.
                                objNivelA = ((List<EvaluacionNivel>)Session["LstEvaluacion"]).Where(q => q.EvaluacionNivelOrden == idNivelOrdenA + 1).FirstOrDefault();
                                if (objNivelA != null)
                                {
                                    TieneIntroduccion = true;
                                    //Si existe el Nivel A, se extrae el primer elemento del Nivel B.
                                    objNivelB = objNivelA.EvaluacionNivel1.Where(q => q.EvaluacionNivelPadreID == objNivelA.EvaluacionNivelID && q.EvaluacionNivelOrden == 1).FirstOrDefault();
                                    if (objNivelB != null)
                                    {
                                        //Si existe el Nivel B, se extrae el primer elemento del Nivel C.
                                        objNivelC = objNivelB.EvaluacionNivel1.Where(q => q.EvaluacionNivelPadreID == objNivelB.EvaluacionNivelID && q.EvaluacionNivelOrden == 1).FirstOrDefault();
                                        if (objNivelC == null)
                                        {
                                            objNivelB = null;
                                            //Si existe se extrae las preguntas para ese nivel.
                                        }
                                    }
                                }
                                else {
                                    // NUEVA VALIDACIÓN PARA OTROS EXTERNOS
                                    //Si no existe, se actualiza el Nivel A agregando 2 al número de orden.
                                    objNivelA = ((List<EvaluacionNivel>)Session["LstEvaluacion"]).Where(q => q.EvaluacionNivelOrden == idNivelOrdenA + 2).FirstOrDefault();
                                    TieneIntroduccion = true;
                                    //Si existe el Nivel A, se extrae el primer elemento del Nivel B.
                                    objNivelB = objNivelA.EvaluacionNivel1.Where(q => q.EvaluacionNivelPadreID == objNivelA.EvaluacionNivelID && q.EvaluacionNivelOrden == 1).FirstOrDefault();
                                    if (objNivelB != null)
                                    {
                                        //Si existe el Nivel B, se extrae el primer elemento del Nivel C.
                                        objNivelC = objNivelB.EvaluacionNivel1.Where(q => q.EvaluacionNivelPadreID == objNivelB.EvaluacionNivelID && q.EvaluacionNivelOrden == 1).FirstOrDefault();
                                        if (objNivelC == null)
                                        {
                                            objNivelB = null;
                                            //Si existe se extrae las preguntas para ese nivel.
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
                else if (idNivelOrdenB != -1) {
                    objNivelB = objNivelA.EvaluacionNivel1.Where(q => q.EvaluacionNivelPadreID == idNivelA && q.EvaluacionNivelOrden == idNivelOrdenB + 1).FirstOrDefault();
                    if (objNivelB != null)
                    {
                        //Si existe el Nivel B, se extrae el primer elemento del Nivel C.
                        objNivelC = objNivelB.EvaluacionNivel1.Where(q => q.EvaluacionNivelPadreID == objNivelB.EvaluacionNivelID && q.EvaluacionNivelOrden == 1).FirstOrDefault();
                        if (objNivelC != null)
                        {
                            //Si existe se extrae las preguntas para ese nivel.
                            lstPreguntas = DAEvaluacionPregunta.ListaPreguntasxNivelEvaluacion(Convert.ToInt32(objNivelC.EvaluacionNivelID));
                        }
                        else
                        {
                            objNivelC = null;
                            //Si no existe se extrae las preguntas del Nivel anterior (Nivel B) .
                            lstPreguntas = DAEvaluacionPregunta.ListaPreguntasxNivelEvaluacion(Convert.ToInt32(objNivelB.EvaluacionNivelID));
                        }
                    }
                    else {
                        //Si no existe, se actualiza el Nivel A agregando 1 al número de orden.
                        objNivelA = ((List<EvaluacionNivel>)Session["LstEvaluacion"]).Where(q => q.EvaluacionNivelOrden == idNivelOrdenA + 1).FirstOrDefault();
                        if (objNivelA != null)
                        {
                            TieneIntroduccion = true;
                            //Si existe el Nivel A, se extrae el primer elemento del Nivel B.
                            objNivelB = objNivelA.EvaluacionNivel1.Where(q => q.EvaluacionNivelPadreID == objNivelA.EvaluacionNivelID && q.EvaluacionNivelOrden == 1).FirstOrDefault();
                            if (objNivelB != null)
                            {                               
                                //Si existe el Nivel B, se extrae el primer elemento del Nivel C.
                                objNivelC = objNivelB.EvaluacionNivel1.Where(q => q.EvaluacionNivelPadreID == objNivelB.EvaluacionNivelID && q.EvaluacionNivelOrden == 1).FirstOrDefault();
                                if (objNivelC == null)
                                {
                                    objNivelB = null;
                                    //Si existe se extrae las preguntas para ese nivel.
                                }
                            }
                        }
                        else
                        {
                            // NUEVA VALIDACIÓN PARA OTROS EXTERNOS
                            //Si no existe, se actualiza el Nivel A agregando 2 al número de orden.
                            objNivelA = ((List<EvaluacionNivel>)Session["LstEvaluacion"]).Where(q => q.EvaluacionNivelOrden == idNivelOrdenA + 2).FirstOrDefault();
                            TieneIntroduccion = true;
                            //Si existe el Nivel A, se extrae el primer elemento del Nivel B.
                            objNivelB = objNivelA.EvaluacionNivel1.Where(q => q.EvaluacionNivelPadreID == objNivelA.EvaluacionNivelID && q.EvaluacionNivelOrden == 1).FirstOrDefault();
                            if (objNivelB != null)
                            {
                                //Si existe el Nivel B, se extrae el primer elemento del Nivel C.
                                objNivelC = objNivelB.EvaluacionNivel1.Where(q => q.EvaluacionNivelPadreID == objNivelB.EvaluacionNivelID && q.EvaluacionNivelOrden == 1).FirstOrDefault();
                                if (objNivelC == null)
                                {
                                    objNivelB = null;
                                    //Si existe se extrae las preguntas para ese nivel.
                                }
                            }
                        }
                    }
                }
            }
            ViewBag.IDNivelA = objNivelA.EvaluacionNivelID;
            ViewBag.IDNivelOrdenA = objNivelA.EvaluacionNivelOrden;
            ViewBag.NivelA = objNivelA.EvaluacionNivelDescripcion;
            ViewBag.EsExterno = Externo;
            if (TieneIntroduccion) {
                ViewBag.objIntroduccion = objNivelA.EvaluacionNivelIntro.FirstOrDefault();
            }
            if (objNivelB != null)
            {
                ViewBag.NivelB = objNivelB.EvaluacionNivelDescripcion;
                ViewBag.DefNivelB = Externo ? objNivelB.EvaluacionNivelDefinicionPar : objNivelB.EvaluacionNivelDefinicion;
                ViewBag.IDNivelOrdenB = objNivelB.EvaluacionNivelOrden;
            }
            else {
                if (TieneIntroduccion) {
                    ViewBag.IDNivelOrdenB = 0;
                    ViewBag.DefNivelB = "";
                    ViewBag.NivelB = "";
                }
                else
                {
                    ViewBag.IDNivelOrdenB = -1;
                    ViewBag.DefNivelB = "";
                    ViewBag.NivelB = "";
                }
            }
            if (TieneIntroduccion)
            {
                if (objNivelC != null)
                {
                    ViewBag.IDNivelOrdenC = 0;
                    ViewBag.DefNivelC = "";
                    ViewBag.NivelC = "";
                }
                else
                {
                    ViewBag.IDNivelOrdenC = -1;
                    ViewBag.DefNivelC = "";
                    ViewBag.NivelC = "";
                }
            }
            else {
                if (objNivelC != null)
                {
                    ViewBag.IDNivelOrdenC = objNivelC.EvaluacionNivelOrden;
                    ViewBag.DefNivelC = Externo ? objNivelC.EvaluacionNivelDefinicionPar : objNivelC.EvaluacionNivelDefinicion;
                    ViewBag.NivelC = objNivelC.EvaluacionNivelDescripcion;
                }
                else {
                    ViewBag.IDNivelOrdenC = -1;
                    ViewBag.DefNivelC = "";
                    ViewBag.NivelC = "";
                }
            }
            Session["cantidadMostradas"] = (int)Session["cantidadMostradas"] + lstPreguntas.Count;
            if ((int)Session["cantidadPreguntasEvaluacion"] == (int)Session["cantidadMostradas"])
            {
                ViewBag.FinalizarEvaluacion = true;
            }
            else {
                ViewBag.FinalizarEvaluacion = false;
            }
            if ((int)Session["cantidadIniPreg"] == 0)
            {
                Session["cantidadIniPreg"] = lstPreguntas.Count;
            }
            ViewBag.AnteriorPagEvaluacion = true;
            ViewBag.LstRespuestas = ((List<EvaluacionRespuesta>)Session["lstEvaluacionRespuesta"]);
            ViewBag.LstPreguntas = lstPreguntas;
            return PartialView("_PartialSiguientePaginaEval");
        }


        [HttpPost]
        public ActionResult anteriorPaginaEvaluacion(int idNivelA, int idNivelOrdenA, int idNivelOrdenB, int idNivelOrdenC, int cntPregForm, bool Externo)
        {
            bool TieneIntroduccion = false;
            List<EvaluacionPregunta> lstPreguntas = new List<EvaluacionPregunta>();
            //De la evaluación se extrae los siguientes objetos para esta  página de la evaluación.
            EvaluacionNivel objNivelA = ((List<EvaluacionNivel>)Session["LstEvaluacion"]).Where(q => q.EvaluacionNivelID == idNivelA).FirstOrDefault();
            EvaluacionNivel objNivelB = null, objNivelC = null;
            if (objNivelA != null)
            {
                if (idNivelOrdenC != -1 && idNivelOrdenB != -1)
                {
                    objNivelB = objNivelA.EvaluacionNivel1.Where(q => q.EvaluacionNivelPadreID == idNivelA && q.EvaluacionNivelOrden == idNivelOrdenB).FirstOrDefault();
                    if (objNivelB != null)
                    {
                        //Se busca el siguiente nivel C quitando en 1 al número de orden.
                        objNivelC = objNivelB.EvaluacionNivel1.Where(q => q.EvaluacionNivelPadreID == objNivelB.EvaluacionNivelID && q.EvaluacionNivelOrden == idNivelOrdenC - 1).FirstOrDefault();
                        if (objNivelC != null)
                        {
                            //Si existe se extrae las preguntas para ese nivel.
                            lstPreguntas = DAEvaluacionPregunta.ListaPreguntasxNivelEvaluacion(Convert.ToInt32(objNivelC.EvaluacionNivelID));
                        }
                        else
                        {
                            //Si no existe, se actualiza el Nivel B quitando 1 al número de orden.
                            objNivelB = objNivelA.EvaluacionNivel1.Where(q => q.EvaluacionNivelPadreID == idNivelA && q.EvaluacionNivelOrden == idNivelOrdenB - 1).FirstOrDefault();
                            if (objNivelB != null)
                            {
                                //Si existe el Nivel B, se extrae el primer elemento del Nivel C.
                                objNivelC = objNivelB.EvaluacionNivel1.Where(q => q.EvaluacionNivelPadreID == objNivelB.EvaluacionNivelID && q.EvaluacionNivelOrden == objNivelB.EvaluacionNivel1.Max(x=>x.EvaluacionNivelOrden)).FirstOrDefault();
                                if (objNivelC != null)
                                {
                                    //Si existe se extrae las preguntas para ese nivel.
                                    lstPreguntas = DAEvaluacionPregunta.ListaPreguntasxNivelEvaluacion(Convert.ToInt32(objNivelC.EvaluacionNivelID));
                                }
                                else
                                {
                                    //Si no existe se extrae las preguntas del Nivel anterior (Nivel B) .
                                    lstPreguntas = DAEvaluacionPregunta.ListaPreguntasxNivelEvaluacion(Convert.ToInt32(objNivelB.EvaluacionNivelID));
                                }
                            }
                            else
                            {
                                //Si no existe, se actualiza el Nivel A quitando 1 al número de orden.
                                objNivelA = ((List<EvaluacionNivel>)Session["LstEvaluacion"]).Where(q => q.EvaluacionNivelOrden == idNivelOrdenA - 1).FirstOrDefault();                                
                                if (objNivelA != null)
                                {
                                    //Si cntPregForm = 0 Indica que el formulario es introducción                            
                                    if (cntPregForm == 0)
                                    {                                                                               
                                        //Si existe el Nivel A, se extrae el ultimo elemento del Nivel B.
                                        objNivelB = objNivelA.EvaluacionNivel1.Where(q => q.EvaluacionNivelPadreID == objNivelA.EvaluacionNivelID).OrderByDescending(x => x.EvaluacionNivelOrden).FirstOrDefault();
                                        if (objNivelB != null)
                                        {
                                            //Si existe el Nivel B, se extrae el ultimo elemento del Nivel C.
                                            objNivelC = objNivelB.EvaluacionNivel1.Where(q => q.EvaluacionNivelPadreID == objNivelB.EvaluacionNivelID).OrderByDescending(x => x.EvaluacionNivelOrden).FirstOrDefault();
                                            if (objNivelC != null)
                                            {
                                                //Si existe se extrae las preguntas para ese nivel.
                                                lstPreguntas = DAEvaluacionPregunta.ListaPreguntasxNivelEvaluacion(Convert.ToInt32(objNivelC.EvaluacionNivelID));
                                            }
                                            else
                                            {   
                                                //Si no existe se extrae las preguntas del Nivel anterior (Nivel B) .
                                                lstPreguntas = DAEvaluacionPregunta.ListaPreguntasxNivelEvaluacion(Convert.ToInt32(objNivelB.EvaluacionNivelID));
                                            }
                                        }
                                        else
                                        {
                                            objNivelB = null;
                                            lstPreguntas = DAEvaluacionPregunta.ListaPreguntasxNivelEvaluacion(Convert.ToInt32(objNivelA.EvaluacionNivelID));
                                        }
                                    }
                                    else {
                                        //Si cntPregForm > 0 Indica que tiene preguntas y se va obtener la INTRO.      
                                        objNivelA = ((List<EvaluacionNivel>)Session["LstEvaluacion"]).Where(q => q.EvaluacionNivelOrden == idNivelOrdenA).FirstOrDefault();
                                        TieneIntroduccion = true;
                                        //Si existe el Nivel A, se extrae el primer elemento del Nivel B.
                                        objNivelB = objNivelA.EvaluacionNivel1.Where(q => q.EvaluacionNivelPadreID == objNivelA.EvaluacionNivelID).OrderBy(x => x.EvaluacionNivelOrden).FirstOrDefault();
                                        if (objNivelB != null)
                                        {
                                            //Si existe el Nivel B, se extrae el primer elemento del Nivel C.
                                            objNivelC = objNivelB.EvaluacionNivel1.Where(q => q.EvaluacionNivelPadreID == objNivelB.EvaluacionNivelID).OrderBy(x => x.EvaluacionNivelOrden).FirstOrDefault();
                                            if (objNivelC == null)
                                            {
                                                objNivelB = null;                          
                                            }
                                        }
                                    }                                   
                                } else {
                                    //  NUEVA VALIDACIÓN PARA OTROS EXTERNOS
                                    //Si no existe, se actualiza el Nivel A quitando 2 al número de orden.
                                    objNivelA = ((List<EvaluacionNivel>)Session["LstEvaluacion"]).Where(q => q.EvaluacionNivelOrden == idNivelOrdenA - 2).FirstOrDefault();
                                    if (objNivelA != null)
                                    {
                                        //Si cntPregForm = 0 Indica que el formulario es introducción                            
                                        if (cntPregForm == 0)
                                        {
                                            //Si existe el Nivel A, se extrae el ultimo elemento del Nivel B.
                                            objNivelB = objNivelA.EvaluacionNivel1.Where(q => q.EvaluacionNivelPadreID == objNivelA.EvaluacionNivelID).OrderByDescending(x => x.EvaluacionNivelOrden).FirstOrDefault();
                                            if (objNivelB != null)
                                            {
                                                //Si existe el Nivel B, se extrae el ultimo elemento del Nivel C.
                                                objNivelC = objNivelB.EvaluacionNivel1.Where(q => q.EvaluacionNivelPadreID == objNivelB.EvaluacionNivelID).OrderByDescending(x => x.EvaluacionNivelOrden).FirstOrDefault();
                                                if (objNivelC != null)
                                                {
                                                    //Si existe se extrae las preguntas para ese nivel.
                                                    lstPreguntas = DAEvaluacionPregunta.ListaPreguntasxNivelEvaluacion(Convert.ToInt32(objNivelC.EvaluacionNivelID));
                                                }
                                                else
                                                {
                                                    //Si no existe se extrae las preguntas del Nivel anterior (Nivel B) .
                                                    lstPreguntas = DAEvaluacionPregunta.ListaPreguntasxNivelEvaluacion(Convert.ToInt32(objNivelB.EvaluacionNivelID));
                                                }
                                            }
                                            else
                                            {
                                                objNivelB = null;
                                                lstPreguntas = DAEvaluacionPregunta.ListaPreguntasxNivelEvaluacion(Convert.ToInt32(objNivelA.EvaluacionNivelID));
                                            }
                                        }
                                        else
                                        {
                                            //Si cntPregForm > 0 Indica que tiene preguntas y se va obtener la INTRO.      
                                            objNivelA = ((List<EvaluacionNivel>)Session["LstEvaluacion"]).Where(q => q.EvaluacionNivelOrden == idNivelOrdenA).FirstOrDefault();
                                            TieneIntroduccion = true;
                                            //Si existe el Nivel A, se extrae el primer elemento del Nivel B.
                                            objNivelB = objNivelA.EvaluacionNivel1.Where(q => q.EvaluacionNivelPadreID == objNivelA.EvaluacionNivelID).OrderBy(x => x.EvaluacionNivelOrden).FirstOrDefault();
                                            if (objNivelB != null)
                                            {
                                                //Si existe el Nivel B, se extrae el primer elemento del Nivel C.
                                                objNivelC = objNivelB.EvaluacionNivel1.Where(q => q.EvaluacionNivelPadreID == objNivelB.EvaluacionNivelID).OrderBy(x => x.EvaluacionNivelOrden).FirstOrDefault();
                                                if (objNivelC == null)
                                                {
                                                    objNivelB = null;
                                                }
                                            }
                                        }
                                    }
                                    else {
                                        //Si no existe se recupera el nivel actual.
                                        objNivelA = ((List<EvaluacionNivel>)Session["LstEvaluacion"]).Where(q => q.EvaluacionNivelID == idNivelA).FirstOrDefault();
                                        if (objNivelA != null)
                                        {
                                            TieneIntroduccion = true;
                                            //Si existe el Nivel A, se extrae el primer elemento del Nivel B.
                                            objNivelB = objNivelA.EvaluacionNivel1.Where(q => q.EvaluacionNivelPadreID == objNivelA.EvaluacionNivelID).OrderBy(x => x.EvaluacionNivelOrden).FirstOrDefault();
                                            if (objNivelB != null)
                                            {
                                                //Si existe el Nivel B, se extrae el primer elemento del Nivel C.
                                                objNivelC = objNivelB.EvaluacionNivel1.Where(q => q.EvaluacionNivelPadreID == objNivelB.EvaluacionNivelID).OrderBy(x => x.EvaluacionNivelOrden).FirstOrDefault();
                                                if (objNivelC == null)
                                                {
                                                    objNivelB = null;
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
                else if (idNivelOrdenB != -1)
                {
                    objNivelB = objNivelA.EvaluacionNivel1.Where(q => q.EvaluacionNivelPadreID == idNivelA && q.EvaluacionNivelOrden == idNivelOrdenB - 1).FirstOrDefault();
                    if (objNivelB != null)
                    {
                        //Si existe el Nivel B, se extrae el primer elemento del Nivel C.
                        objNivelC = objNivelB.EvaluacionNivel1.Where(q => q.EvaluacionNivelPadreID == objNivelB.EvaluacionNivelID && q.EvaluacionNivelOrden == objNivelB.EvaluacionNivel1.Max(x => x.EvaluacionNivelOrden)).FirstOrDefault();
                        if (objNivelC != null)
                        {
                            //Si existe se extrae las preguntas para ese nivel.
                            lstPreguntas = DAEvaluacionPregunta.ListaPreguntasxNivelEvaluacion(Convert.ToInt32(objNivelC.EvaluacionNivelID));
                        }
                        else
                        {
                            objNivelC = null;
                            //Si no existe se extrae las preguntas del Nivel anterior (Nivel B) .
                            lstPreguntas = DAEvaluacionPregunta.ListaPreguntasxNivelEvaluacion(Convert.ToInt32(objNivelB.EvaluacionNivelID));
                        }
                    }
                    else
                    {
                        //Si no existe, se actualiza el Nivel A quitando 1 al número de orden.
                        objNivelA = ((List<EvaluacionNivel>)Session["LstEvaluacion"]).Where(q => q.EvaluacionNivelOrden == idNivelOrdenA - 1).FirstOrDefault();                        
                        if (objNivelA != null)
                        {
                            if (cntPregForm == 0)
                            {
                                //Si existe el Nivel A, se extrae el primer elemento del Nivel B.
                                objNivelB = objNivelA.EvaluacionNivel1.Where(q => q.EvaluacionNivelPadreID == objNivelA.EvaluacionNivelID).OrderByDescending(x => x.EvaluacionNivelOrden).FirstOrDefault();
                                if (objNivelB != null)
                                {
                                    //Si existe el Nivel B, se extrae el primer elemento del Nivel C.
                                    objNivelC = objNivelB.EvaluacionNivel1.Where(q => q.EvaluacionNivelPadreID == objNivelB.EvaluacionNivelID).OrderByDescending(x => x.EvaluacionNivelOrden).FirstOrDefault();
                                    if (objNivelC != null)
                                    {
                                        //Si existe se extrae las preguntas para ese nivel.
                                        lstPreguntas = DAEvaluacionPregunta.ListaPreguntasxNivelEvaluacion(Convert.ToInt32(objNivelC.EvaluacionNivelID));
                                    }
                                    else
                                    {
                                        //Si no existe se extrae las preguntas del Nivel anterior (Nivel B) .
                                        objNivelC = null;
                                        lstPreguntas = DAEvaluacionPregunta.ListaPreguntasxNivelEvaluacion(Convert.ToInt32(objNivelB.EvaluacionNivelID));
                                    }
                                }
                                else
                                {
                                    objNivelB = null;
                                    lstPreguntas = DAEvaluacionPregunta.ListaPreguntasxNivelEvaluacion(Convert.ToInt32(objNivelA.EvaluacionNivelID));
                                }
                            }
                            else {
                                objNivelA = ((List<EvaluacionNivel>)Session["LstEvaluacion"]).Where(q => q.EvaluacionNivelOrden == idNivelOrdenA).FirstOrDefault();
                                TieneIntroduccion = true;
                                //Si existe el Nivel A, se extrae el primer elemento del Nivel B.
                                objNivelB = objNivelA.EvaluacionNivel1.Where(q => q.EvaluacionNivelPadreID == objNivelA.EvaluacionNivelID).OrderByDescending(x => x.EvaluacionNivelOrden).FirstOrDefault();
                                if (objNivelB != null)
                                {
                                    //Si existe el Nivel B, se extrae el primer elemento del Nivel C.
                                    objNivelC = objNivelB.EvaluacionNivel1.Where(q => q.EvaluacionNivelPadreID == objNivelB.EvaluacionNivelID).OrderByDescending(x => x.EvaluacionNivelOrden).FirstOrDefault();
                                    if (objNivelC == null)
                                    {
                                        objNivelB = null;
                                        //Si existe se extrae las preguntas para ese nivel.                                                
                                    }
                                }
                            }
                        } else {
                            //Si no existe, se actualiza el Nivel A quitando 1 al número de orden.
                            objNivelA = ((List<EvaluacionNivel>)Session["LstEvaluacion"]).Where(q => q.EvaluacionNivelOrden == idNivelOrdenA - 2).FirstOrDefault();
                            if (objNivelA != null)
                            {
                                if (cntPregForm == 0)
                                {
                                    //Si existe el Nivel A, se extrae el primer elemento del Nivel B.
                                    objNivelB = objNivelA.EvaluacionNivel1.Where(q => q.EvaluacionNivelPadreID == objNivelA.EvaluacionNivelID).OrderByDescending(x => x.EvaluacionNivelOrden).FirstOrDefault();
                                    if (objNivelB != null)
                                    {
                                        //Si existe el Nivel B, se extrae el primer elemento del Nivel C.
                                        objNivelC = objNivelB.EvaluacionNivel1.Where(q => q.EvaluacionNivelPadreID == objNivelB.EvaluacionNivelID).OrderByDescending(x => x.EvaluacionNivelOrden).FirstOrDefault();
                                        if (objNivelC != null)
                                        {
                                            //Si existe se extrae las preguntas para ese nivel.
                                            lstPreguntas = DAEvaluacionPregunta.ListaPreguntasxNivelEvaluacion(Convert.ToInt32(objNivelC.EvaluacionNivelID));
                                        }
                                        else
                                        {
                                            //Si no existe se extrae las preguntas del Nivel anterior (Nivel B) .
                                            objNivelC = null;
                                            lstPreguntas = DAEvaluacionPregunta.ListaPreguntasxNivelEvaluacion(Convert.ToInt32(objNivelB.EvaluacionNivelID));
                                        }
                                    }
                                    else
                                    {
                                        objNivelB = null;
                                        lstPreguntas = DAEvaluacionPregunta.ListaPreguntasxNivelEvaluacion(Convert.ToInt32(objNivelA.EvaluacionNivelID));
                                    }
                                }
                                else
                                {
                                    objNivelA = ((List<EvaluacionNivel>)Session["LstEvaluacion"]).Where(q => q.EvaluacionNivelOrden == idNivelOrdenA).FirstOrDefault();
                                    TieneIntroduccion = true;
                                    //Si existe el Nivel A, se extrae el primer elemento del Nivel B.
                                    objNivelB = objNivelA.EvaluacionNivel1.Where(q => q.EvaluacionNivelPadreID == objNivelA.EvaluacionNivelID).OrderByDescending(x => x.EvaluacionNivelOrden).FirstOrDefault();
                                    if (objNivelB != null)
                                    {
                                        //Si existe el Nivel B, se extrae el primer elemento del Nivel C.
                                        objNivelC = objNivelB.EvaluacionNivel1.Where(q => q.EvaluacionNivelPadreID == objNivelB.EvaluacionNivelID).OrderByDescending(x => x.EvaluacionNivelOrden).FirstOrDefault();
                                        if (objNivelC == null)
                                        {
                                            objNivelB = null;
                                            //Si existe se extrae las preguntas para ese nivel.                                                
                                        }
                                    }
                                }
                            }
                            else {
                                objNivelA = ((List<EvaluacionNivel>)Session["LstEvaluacion"]).Where(q => q.EvaluacionNivelID == idNivelA).FirstOrDefault();
                                if (objNivelA != null)
                                {
                                    TieneIntroduccion = true;
                                    //Si existe el Nivel A, se extrae el primer elemento del Nivel B.
                                    objNivelB = objNivelA.EvaluacionNivel1.Where(q => q.EvaluacionNivelPadreID == objNivelA.EvaluacionNivelID).OrderBy(x => x.EvaluacionNivelOrden).FirstOrDefault();
                                    if (objNivelB != null)
                                    {
                                        //Si existe el Nivel B, se extrae el primer elemento del Nivel C.
                                        objNivelC = objNivelB.EvaluacionNivel1.Where(q => q.EvaluacionNivelPadreID == objNivelB.EvaluacionNivelID).OrderBy(x => x.EvaluacionNivelOrden).FirstOrDefault();
                                        if (objNivelC == null)
                                        {
                                            objNivelB = null;
                                            //Si existe se extrae las preguntas para ese nivel.                                                
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
            ViewBag.IDNivelA = objNivelA.EvaluacionNivelID;
            ViewBag.IDNivelOrdenA = objNivelA.EvaluacionNivelOrden;
            ViewBag.NivelA = objNivelA.EvaluacionNivelDescripcion;
            ViewBag.EsExterno = Externo;
            if (TieneIntroduccion){
                ViewBag.objIntroduccion = objNivelA.EvaluacionNivelIntro.FirstOrDefault();
            }
            if (objNivelB != null)
            {
                ViewBag.NivelB = objNivelB.EvaluacionNivelDescripcion;
                ViewBag.DefNivelB = Externo ? objNivelB.EvaluacionNivelDefinicionPar : objNivelB.EvaluacionNivelDefinicion;
                ViewBag.IDNivelOrdenB = objNivelB.EvaluacionNivelOrden;
            }
            else
            {
                if (TieneIntroduccion)
                {
                    ViewBag.IDNivelOrdenB = 0;
                    ViewBag.DefNivelB = "";
                    ViewBag.NivelB = "";
                }
                else
                {
                    ViewBag.IDNivelOrdenB = -1;
                    ViewBag.DefNivelB = "";
                    ViewBag.NivelB = "";
                }
                
            }
            if (TieneIntroduccion)
            {
                if (objNivelC != null)
                {
                    ViewBag.IDNivelOrdenC = 0;
                    ViewBag.DefNivelC = "";
                    ViewBag.NivelC = "";
                }
                else
                {
                    ViewBag.IDNivelOrdenC = -1;
                    ViewBag.DefNivelC = "";
                    ViewBag.NivelC = "";
                }

            }
            else {
                if (objNivelC != null)
                {
                    ViewBag.IDNivelOrdenC = objNivelC.EvaluacionNivelOrden;
                    ViewBag.DefNivelC = Externo ? objNivelC.EvaluacionNivelDefinicionPar : objNivelC.EvaluacionNivelDefinicion;
                    ViewBag.NivelC = objNivelC.EvaluacionNivelDescripcion;
                }
                else
                {
                    ViewBag.IDNivelOrdenC = -1;
                    ViewBag.DefNivelC = "";
                    ViewBag.NivelC = "";
                }
            }
            Session["cantidadMostradas"] = (int)Session["cantidadMostradas"] - cntPregForm;
            ViewBag.FinalizarEvaluacion = false;
            if ((int)Session["cantidadMostradas"] == 0)
            {
                ViewBag.AnteriorPagEvaluacion = false;                
            }
            else
            {
                ViewBag.AnteriorPagEvaluacion = true;
            }
            ViewBag.LstRespuestas = ((List<EvaluacionRespuesta>)Session["lstEvaluacionRespuesta"]);
            ViewBag.LstPreguntas = lstPreguntas;
            return PartialView("_PartialSiguientePaginaEval");
        }


        [HttpPost]
        public ActionResult finalizarEvaluacion()
        {
            ViewBag.rptaRegEvaluacion = DAEvaluacionRespuesta.RegistrarRespuestaEvaluacion(((List<EvaluacionRespuesta>)Session["lstEvaluacionRespuesta"]), (Participante)Session["Alumno"]);
            Session.Remove("Alumno");
            Session.Remove("LstEvaluacion");
            Session.Remove("cantidadPreguntasEvaluacion");
            Session.Remove("cantidadMostradas");
            Session.Remove("cantidadIniPreg");
            Session.Remove("cantidadResulta");
            Session.Remove("lstEvaluacionRespuesta");
            Session.Abandon();
            return PartialView("_PartialFinEvaluacion");
        }

        /// <summary>
        /// Obtiene la respuesta de lista en Memoria.
        /// </summary>
        /// <param name="p_idPromocion">Id Promoción</param>
        /// <param name="p_idMedicion">Id Medición.</param>
        /// <param name="p_idParticipante">Id Participante.</param>
        /// <param name="p_idPregunta">Id Pregunta.</param>
        /// <param name="p_idParticipanteEval">Id Participante a Evaluar.</param>
        /// <returns></returns>
        private EvaluacionRespuesta ObtenerRespuesta(int p_idPromocion, int p_idMedicion, int p_idPregunta, int p_idParticipanteEval, bool p_Externo)
        {
            EvaluacionRespuesta objEvaluacionRpta = null;
            if (p_Externo)
                objEvaluacionRpta = ((List<EvaluacionRespuesta>)Session["lstEvaluacionRespuesta"]).Where(q => q.EvaluacionPromocionID == p_idPromocion && q.EvaluacionMedicionID == p_idMedicion && q.EvaluacionAlternativa.EvaluacionPreguntaID == p_idPregunta && q.ParticipanteEvaluadoID == p_idParticipanteEval).FirstOrDefault();
            else
                objEvaluacionRpta = ((List<EvaluacionRespuesta>)Session["lstEvaluacionRespuesta"]).Where(q => q.EvaluacionPromocionID == p_idPromocion && q.EvaluacionMedicionID == p_idMedicion && q.EvaluacionAlternativa.EvaluacionPreguntaID == p_idPregunta).FirstOrDefault();
            return objEvaluacionRpta;
        }

        /// <summary>
        /// Agrega a la lista la respuesta del alumno. 
        /// </summary>
        /// <param name="p_idPromocion"></param>
        /// <param name="p_idMedicion"></param>
        /// <param name="p_idAlternativa"></param>
        /// <param name="p_idParticipante"></param>
        /// <param name="p_idParticipanteEval"></param>
        private void AgregarListaDeRespuestas(int p_idPromocion, int p_idMedicion, int p_idAlternativa, int p_idParticipanteEval, int p_idPregunta)
        {  
            if (p_idParticipanteEval == 0)
                ((List<EvaluacionRespuesta>)Session["lstEvaluacionRespuesta"]).Add(new EvaluacionRespuesta { EvaluacionPromocionID = p_idPromocion, EvaluacionMedicionID = p_idMedicion, EvaluacionAlternativaID = p_idAlternativa, EvaluacionAlternativa = new EvaluacionAlternativa { EvaluacionPreguntaID = p_idPregunta } });
            else
                ((List<EvaluacionRespuesta>)Session["lstEvaluacionRespuesta"]).Add(new EvaluacionRespuesta { EvaluacionPromocionID = p_idPromocion, EvaluacionMedicionID = p_idMedicion, EvaluacionAlternativaID = p_idAlternativa, ParticipanteEvaluadoID = p_idParticipanteEval, EvaluacionAlternativa = new EvaluacionAlternativa { EvaluacionPreguntaID = p_idPregunta } });
        }

        private void InicializarVariablesSesiones() {
            Session["LstEvaluacion"] = null;
            Session["cantidadPreguntasEvaluacion"] = null;
            Session["cantidadMostradas"] = 0;
            Session["cantidadIniPreg"] = 0;
            Session["cantidadResulta"] = 0;
            Session["lstEvaluacionRespuesta"] = null;
        }


    }
}
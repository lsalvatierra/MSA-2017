using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ESAN.Componentes.CoreEvaluacion.Logic.Facade.EvaluacionMSA;
using ESAN.Componentes.CoreEvaluacion.Models.General.EvaluacionMSA;

namespace ESANMSA.Areas.Alumno.Controllers
{
    public class EvaluacionController : Controller
    {
        private Evaluacion objEvaluacion;
        private static List<EvaluacionNivel> lstEvaluacion;
        private static List<EvaluacionRespuesta> lstEvaluacionRespuesta = new List<EvaluacionRespuesta>();
        private static int cantidadPreguntasEvaluacion = 79;
        private static int cantidadMostradas = 0;
        private static int cantidadIniPreg = 0;
        private static int cantidadResulta = 0;

        // GET: Alumno/Evaluacion
        public ActionResult frmEvaluacion(int idMedicion, int idPromocion, int idEvaluado, bool Externo)
        {
            if (Session["Alumno"] != null)
            {
                lstEvaluacionRespuesta.Clear();
                cantidadMostradas = 0;
                cantidadIniPreg = 0;
                cantidadResulta = 0;
                objEvaluacion = DAEvaluacion.ObtenerEvaluacion(idPromocion);
                List<EvaluacionPregunta> lstPreguntas = new List<EvaluacionPregunta>();
                ViewBag.NombreEvaluacion = objEvaluacion.EvaluacionDescripcion;
                ViewBag.IdMedicion = idMedicion;
                ViewBag.IdPromocion = idPromocion;
                ViewBag.EsExterno = Externo;
                ViewBag.IdEvaluado = idEvaluado;
                if (Externo)
                {
                    Participante objParticipante = DAParticipante.ObtenerParticipantexID(idEvaluado);
                    if (objParticipante != null)
                    {
                        ViewBag.NombresParticipanteEvaluar = objParticipante.ParticipanteNombreCompleto;
                    }
                    else {
                        return RedirectToAction("FormularioError", "Registro", new { area = "Alumno", p_tipoError = 3 });
                    }
                    
                }
                //Obtenemos toda la evaluación y se guarda a una variable global.
                lstEvaluacion = DAEvaluacionNivel.ListaNivelesxEvaluacion(idPromocion, 0);
                //De la evaluación se extrae los primeros objetos para esta primera pagina
                if (lstEvaluacion.FirstOrDefault() != null)
                {
                    ViewBag.NivelA = lstEvaluacion.FirstOrDefault().EvaluacionNivelDescripcion;
                    ViewBag.IDNivelA = lstEvaluacion.FirstOrDefault().EvaluacionNivelID;
                    ViewBag.IDNivelOrdenA = lstEvaluacion.FirstOrDefault().EvaluacionNivelOrden;
                    if (lstEvaluacion.FirstOrDefault().EvaluacionNivel1.Count() > 0)
                    {
                        ViewBag.NivelB = lstEvaluacion.FirstOrDefault().EvaluacionNivel1.FirstOrDefault().EvaluacionNivelDescripcion;

                        ViewBag.IDNivelOrdenB = lstEvaluacion.FirstOrDefault().EvaluacionNivel1.FirstOrDefault().EvaluacionNivelOrden;

                        if (lstEvaluacion.FirstOrDefault().EvaluacionNivel1.FirstOrDefault().EvaluacionNivel1.Count > 0)
                        {
                            ViewBag.NivelC = lstEvaluacion.FirstOrDefault().EvaluacionNivel1.FirstOrDefault().EvaluacionNivel1.FirstOrDefault().EvaluacionNivelDescripcion;
                            ViewBag.IDNivelOrdenC = lstEvaluacion.FirstOrDefault().EvaluacionNivel1.FirstOrDefault().EvaluacionNivel1.FirstOrDefault().EvaluacionNivelOrden;
                            lstPreguntas = DAEvaluacionPregunta.ListaPreguntasxNivelEvaluacion(Convert.ToInt32(lstEvaluacion.FirstOrDefault().EvaluacionNivel1.FirstOrDefault().EvaluacionNivel1.FirstOrDefault().EvaluacionNivelID));
                            ViewBag.LstPreguntas = lstPreguntas;
                        }
                        else
                        {
                            //Se coloca 0 cuando ya no existe más subniveles.
                            ViewBag.NivelC = "";
                            ViewBag.IDNivelOrdenC = 0;
                            lstPreguntas = DAEvaluacionPregunta.ListaPreguntasxNivelEvaluacion(Convert.ToInt32(lstEvaluacion.FirstOrDefault().EvaluacionNivel1.FirstOrDefault().EvaluacionNivelID));

                            ViewBag.LstPreguntas = lstPreguntas;

                        }
                    }
                    else
                    {
                        //Se coloca 0 cuando ya no existe más subniveles.
                        ViewBag.NivelB = "";
                        ViewBag.IDNivelOrdenB = 0;
                        lstPreguntas = DAEvaluacionPregunta.ListaPreguntasxNivelEvaluacion(Convert.ToInt32(lstEvaluacion.FirstOrDefault().EvaluacionNivelID));
                        ViewBag.LstPreguntas = lstPreguntas;
                    }
                }
                cantidadIniPreg = lstPreguntas.Count;
                cantidadMostradas = cantidadMostradas + lstPreguntas.Count;
                return View();
            }
            else {
                return RedirectToAction("Formulario","Registro",new { area="Alumno", idPromocion = idPromocion, idMedicion = idMedicion, idEvaluado = idEvaluado, Externo = Externo });

            }
            
        }


        [HttpPost]
        public ActionResult agregarRespuestas(int idMedicion, int idPromocion, int idEvaluado, bool Externo, int idAlternativa, int idPregunta)
        {

            if (Session["Alumno"] != null)
            {
                int idParticipante = Convert.ToInt32(((Participante)Session["Alumno"]).ParticipanteID);
                if (lstEvaluacionRespuesta.Count > 0)
                {                                     
                    EvaluacionRespuesta objEvalRpta = ObtenerRespuesta(idPromocion, idMedicion, idParticipante, idPregunta, idEvaluado, Externo);
                    if (Externo)
                    {
                        if (objEvalRpta == null)
                        {
                            AgregarListaDeRespuestas(idPromocion, idMedicion, idAlternativa, idParticipante, idEvaluado, idPregunta);
                        }
                        else
                        {
                            lstEvaluacionRespuesta.Remove(objEvalRpta);
                            AgregarListaDeRespuestas(idPromocion, idMedicion, idAlternativa, idParticipante, idEvaluado, idPregunta);
                        }
                    }
                    else
                    {
                        if (objEvalRpta == null)
                        {
                            AgregarListaDeRespuestas(idPromocion, idMedicion, idAlternativa, idParticipante, 0, idPregunta);
                        }
                        else
                        {
                            lstEvaluacionRespuesta.Remove(objEvalRpta);
                            AgregarListaDeRespuestas(idPromocion, idMedicion, idAlternativa, idParticipante, 0, idPregunta);
                        }
                    }
                }
                else
                {
                    if (Externo)
                    {
                        AgregarListaDeRespuestas(idPromocion, idMedicion, idAlternativa, idParticipante, idEvaluado, idPregunta);
                    }
                    else
                    {
                        AgregarListaDeRespuestas(idPromocion, idMedicion, idAlternativa, idParticipante, 0, idPregunta);
                    }
                }
            }
            cantidadResulta = lstEvaluacionRespuesta.Count;
            double avance = (cantidadResulta * 100) / cantidadPreguntasEvaluacion;
            ViewBag.avanceEvaluacion = avance;
            return PartialView("_PartialPorcentajeAvance");
        }

        [HttpPost]
        public ActionResult siguientePaginaEvaluacion(int idNivelA, int idNivelOrdenA, int idNivelOrdenB, int idNivelOrdenC) {
            List<EvaluacionPregunta> lstPreguntas = new List<EvaluacionPregunta>();
            //De la evaluación se extrae los siguientes objetos para esta  página de la evaluación.
            EvaluacionNivel objNivelA = lstEvaluacion.Where(q => q.EvaluacionNivelID == idNivelA).FirstOrDefault();
            EvaluacionNivel objNivelB = null, objNivelC = null;            
            if (objNivelA != null)
            {
                if (idNivelOrdenC != 0 && idNivelOrdenB != 0)
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
                                objNivelA = lstEvaluacion.Where(q => q.EvaluacionNivelOrden == idNivelOrdenA + 1).FirstOrDefault();
                                if (objNivelA != null)
                                {
                                    //Si existe el Nivel A, se extrae el primer elemento del Nivel B.
                                    objNivelB = objNivelA.EvaluacionNivel1.Where(q => q.EvaluacionNivelPadreID == objNivelA.EvaluacionNivelID && q.EvaluacionNivelOrden == 1).FirstOrDefault();
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
                                    } else {
                                        objNivelB = null;
                                        lstPreguntas = DAEvaluacionPregunta.ListaPreguntasxNivelEvaluacion(Convert.ToInt32(objNivelA.EvaluacionNivelID));
                                    }
                                }
                            }

                        }
                    }
                }
                else if (idNivelOrdenB != 0) {
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
                        objNivelA = lstEvaluacion.Where(q => q.EvaluacionNivelOrden == idNivelOrdenA + 1).FirstOrDefault();
                        if (objNivelA != null)
                        {
                            //Si existe el Nivel A, se extrae el primer elemento del Nivel B.
                            objNivelB = objNivelA.EvaluacionNivel1.Where(q => q.EvaluacionNivelPadreID == objNivelA.EvaluacionNivelID && q.EvaluacionNivelOrden == 1).FirstOrDefault();
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
                    }
                }
            }
            ViewBag.IDNivelA = objNivelA.EvaluacionNivelID;
            ViewBag.IDNivelOrdenA = objNivelA.EvaluacionNivelOrden;
            ViewBag.NivelA = objNivelA.EvaluacionNivelDescripcion;
            

            if (objNivelB != null)
            {
                ViewBag.NivelB = objNivelB.EvaluacionNivelDescripcion;
                ViewBag.IDNivelOrdenB = objNivelB.EvaluacionNivelOrden;
            }
            else {
                ViewBag.IDNivelOrdenB = 0;
                ViewBag.NivelB = "";
            }
            if (objNivelC != null)
            {
                ViewBag.IDNivelOrdenC = objNivelC.EvaluacionNivelOrden;
                ViewBag.NivelC = objNivelC.EvaluacionNivelDescripcion;
            }else {
                ViewBag.IDNivelOrdenC = 0;
                ViewBag.NivelC = "";
            }
            cantidadMostradas = cantidadMostradas + lstPreguntas.Count;
            if (cantidadPreguntasEvaluacion == cantidadMostradas)
            {
                ViewBag.FinalizarEvaluacion = true;
            }
            else {
                ViewBag.FinalizarEvaluacion = false;
            }
            ViewBag.AnteriorPagEvaluacion = true;
            ViewBag.LstRespuestas = lstEvaluacionRespuesta;
            ViewBag.LstPreguntas = lstPreguntas;
            return PartialView("_PartialSiguientePaginaEval");
        }


        [HttpPost]
        public ActionResult anteriorPaginaEvaluacion(int idNivelA, int idNivelOrdenA, int idNivelOrdenB, int idNivelOrdenC, int cntPregForm)
        {
            List<EvaluacionPregunta> lstPreguntas = new List<EvaluacionPregunta>();
            //De la evaluación se extrae los siguientes objetos para esta  página de la evaluación.
            EvaluacionNivel objNivelA = lstEvaluacion.Where(q => q.EvaluacionNivelID == idNivelA).FirstOrDefault();
            EvaluacionNivel objNivelB = null, objNivelC = null;
            if (objNivelA != null)
            {
                if (idNivelOrdenC != 0 && idNivelOrdenB != 0)
                {
                    objNivelB = objNivelA.EvaluacionNivel1.Where(q => q.EvaluacionNivelPadreID == idNivelA && q.EvaluacionNivelOrden == idNivelOrdenB).FirstOrDefault();
                    if (objNivelB != null)
                    {
                        //Se busca el siguiente nivel C agregando en 1 al número de orden.
                        objNivelC = objNivelB.EvaluacionNivel1.Where(q => q.EvaluacionNivelPadreID == objNivelB.EvaluacionNivelID && q.EvaluacionNivelOrden == idNivelOrdenC - 1).FirstOrDefault();
                        if (objNivelC != null)
                        {
                            //Si existe se extrae las preguntas para ese nivel.
                            lstPreguntas = DAEvaluacionPregunta.ListaPreguntasxNivelEvaluacion(Convert.ToInt32(objNivelC.EvaluacionNivelID));
                        }
                        else
                        {
                            //Si no existe, se actualiza el Nivel B agregando 1 al número de orden.
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
                                    objNivelC = null;
                                    //Si no existe se extrae las preguntas del Nivel anterior (Nivel B) .
                                    lstPreguntas = DAEvaluacionPregunta.ListaPreguntasxNivelEvaluacion(Convert.ToInt32(objNivelB.EvaluacionNivelID));
                                }
                            }
                            else
                            {
                                //Si no existe, se actualiza el Nivel A agregando 1 al número de orden.
                                objNivelA = lstEvaluacion.Where(q => q.EvaluacionNivelOrden == idNivelOrdenA - 1).FirstOrDefault();
                                if (objNivelA != null)
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
                                            objNivelC = null;
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
                            }

                        }
                    }
                }
                else if (idNivelOrdenB != 0)
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
                        //Si no existe, se actualiza el Nivel A agregando 1 al número de orden.
                        objNivelA = lstEvaluacion.Where(q => q.EvaluacionNivelOrden == idNivelOrdenA - 1).FirstOrDefault();
                        if (objNivelA != null)
                        {
                            //Si existe el Nivel A, se extrae el primer elemento del Nivel B.
                            objNivelB = objNivelA.EvaluacionNivel1.Where(q => q.EvaluacionNivelPadreID == objNivelA.EvaluacionNivelID).OrderByDescending(x=>x.EvaluacionNivelOrden).FirstOrDefault();
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
                    }
                }
            }
            ViewBag.IDNivelA = objNivelA.EvaluacionNivelID;
            ViewBag.IDNivelOrdenA = objNivelA.EvaluacionNivelOrden;
            ViewBag.NivelA = objNivelA.EvaluacionNivelDescripcion;
            

            if (objNivelB != null)
            {
                ViewBag.NivelB = objNivelB.EvaluacionNivelDescripcion;
                ViewBag.IDNivelOrdenB = objNivelB.EvaluacionNivelOrden;
            }
            else
            {
                ViewBag.IDNivelOrdenB = 0;
                ViewBag.NivelB = "";
            }
            if (objNivelC != null)
            {
                ViewBag.IDNivelOrdenC = objNivelC.EvaluacionNivelOrden;
                ViewBag.NivelC = objNivelC.EvaluacionNivelDescripcion;
            }
            else
            {
                ViewBag.IDNivelOrdenC = 0;
                ViewBag.NivelC = "";
            }
            cantidadMostradas = cantidadMostradas - cntPregForm;
            ViewBag.FinalizarEvaluacion = false;
            if (cantidadIniPreg == cantidadMostradas)
            {
                ViewBag.AnteriorPagEvaluacion = false;                
            }
            else
            {
                ViewBag.AnteriorPagEvaluacion = true;
            }
            ViewBag.LstRespuestas = lstEvaluacionRespuesta;
            ViewBag.LstPreguntas = lstPreguntas;
            return PartialView("_PartialSiguientePaginaEval");
        }


        [HttpPost]
        public ActionResult finalizarEvaluacion()
        {
            Session.Abandon();
            Session.Remove("Alumno");
            ViewBag.rptaRegEvaluacion = DAEvaluacionRespuesta.RegistrarRespuestaEvaluacion(lstEvaluacionRespuesta);
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
        private EvaluacionRespuesta ObtenerRespuesta(int p_idPromocion, int p_idMedicion, int p_idParticipante, int p_idPregunta, int p_idParticipanteEval,bool p_Externo) {
            EvaluacionRespuesta objEvaluacionRpta = null;
            if (p_Externo)
                objEvaluacionRpta = lstEvaluacionRespuesta.Where(q => q.EvaluacionPromocionID == p_idPromocion && q.EvaluacionMedicionID == p_idMedicion && q.ParticipanteID == p_idParticipante && q.EvaluacionAlternativa.EvaluacionPreguntaID == p_idPregunta && q.ParticipanteEvaluadoID == p_idParticipanteEval).FirstOrDefault();
            else
                objEvaluacionRpta = lstEvaluacionRespuesta.Where(q => q.EvaluacionPromocionID == p_idPromocion && q.EvaluacionMedicionID == p_idMedicion && q.ParticipanteID == p_idParticipante && q.EvaluacionAlternativa.EvaluacionPreguntaID == p_idPregunta).FirstOrDefault();
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
        private void AgregarListaDeRespuestas(int p_idPromocion, int p_idMedicion, int p_idAlternativa, int p_idParticipante, int p_idParticipanteEval, int p_idPregunta)
        {  
            if (p_idParticipanteEval == 0)
                lstEvaluacionRespuesta.Add(new EvaluacionRespuesta { EvaluacionPromocionID = p_idPromocion, EvaluacionMedicionID = p_idMedicion, ParticipanteID = p_idParticipante, EvaluacionAlternativaID = p_idAlternativa, EvaluacionAlternativa = new EvaluacionAlternativa { EvaluacionPreguntaID = p_idPregunta } });
            else
                lstEvaluacionRespuesta.Add(new EvaluacionRespuesta { EvaluacionPromocionID = p_idPromocion, EvaluacionMedicionID = p_idMedicion, ParticipanteID = p_idParticipante, EvaluacionAlternativaID = p_idAlternativa, ParticipanteEvaluadoID = p_idParticipanteEval, EvaluacionAlternativa = new EvaluacionAlternativa { EvaluacionPreguntaID = p_idPregunta } });
        }


    }
}
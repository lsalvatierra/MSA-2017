using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ESAN.Componentes.CoreEvaluacion.Models.General.EvaluacionMSA;
using System.Data.Entity;
namespace ESAN.Componentes.CoreEvaluacion.Logic.Facade.EvaluacionMSA
{
    public class DAParticipante
    {

        /// <summary>
        /// Lista de tipos de documentos de identidad
        /// </summary>
        /// <param name="p_idTipoDocumento">Tipo de documento.</param>
        /// <param name="p_nroDocumento">Nro de documento</param>
        /// <returns>Objeto Participante</returns>
        public static Participante ObtenerParticipante(int p_idTipoDocumento, string p_nroDocumento)
        {
            Participante objParticipante = null;
            using (var data = new BDEvaluacionEntities())
            {
                return objParticipante = data.Participante.Where(q => q.TipoDocumentoID == p_idTipoDocumento && q.ParticipanteNroDoc == p_nroDocumento).FirstOrDefault();
            }
        }

        /// <summary>
        /// Obtener participante por ID
        /// </summary>
        /// <param name="p_idParticipante">Id participante</param>
        /// <returns>Objeto Participante.</returns>
        public static Participante ObtenerParticipantexID(int p_idParticipante)
        {
            using (var data = new BDEvaluacionEntities())
            {
                return data.Participante.Where(q => q.ParticipanteID == p_idParticipante).FirstOrDefault();
            }
        }

        /// <summary>
        /// Registrar Participante
        /// </summary>
        /// <param name="p_objParticipante">Objeto Participante</param>
        /// <returns>True o false</returns>
        public static int RegistrarParticipante(Participante p_objParticipante)
        {
            int rpta = 0;
            try
            {
                using (var data = new BDEvaluacionEntities())
                {
                    data.Participante.Add(p_objParticipante);
                    data.SaveChanges();
                    rpta = Convert.ToInt32(p_objParticipante.ParticipanteID);
                }
            }
            catch {
                rpta = 0;
            }
            return rpta;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="p_idParticipante"></param>
        /// <param name="p_idPromocion"></param>
        /// <param name="p_idMedicion"></param>
        /// <returns></returns>
        public static int ObtenerParticipantePromocionYRespuestas(int p_idParticipante, int p_idPromocion, int p_idMedicion)
        {
            int rpta = 0;
            using (var data = new BDEvaluacionEntities())
            {
                if (data.EvaluacionPromocionParticipante.Where(q => q.EvaluacionMedicionID == p_idMedicion && q.EvaluacionPromocionID == p_idPromocion && q.ParticipanteID == p_idParticipante).FirstOrDefault() != null) {
                    rpta = 0;
                    if (data.EvaluacionRespuesta.Where(q => q.EvaluacionMedicionID == p_idMedicion && q.EvaluacionPromocionID == p_idPromocion && q.ParticipanteID == p_idParticipante).ToList().Count > 0) {
                        rpta = 1;
                    }
                }
            }
            return rpta;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="p_idPromocion"></param>
        /// <param name="p_idMedicion"></param>
        /// <returns></returns>
        public static EvaluacionPromocionMedicion ObtenerEvaluacionPromocionMedicion(int p_idPromocion, int p_idMedicion) {
            
            using (var data = new BDEvaluacionEntities())
            {
                return data.EvaluacionPromocionMedicion.Where(q => q.EvaluacionMedicionID == p_idMedicion && q.EvaluacionPromocionID == p_idPromocion).FirstOrDefault();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="p_idPromocion"></param>
        /// <param name="p_idCiclo"></param>
        /// <returns></returns>
        static public List<EvaluacionPromocionParticipante> ListaParticipantes(int p_idPromocion, int p_idCiclo)
        {
            List<EvaluacionPromocionParticipante> lista = new List<EvaluacionPromocionParticipante>();
            using (var data = new BDEvaluacionEntities())
            {
                int idMedicion = data.EvaluacionPromocionMedicion.Where(q => q.EvaluacionPromocionID == p_idPromocion && q.EvaluacionCicloID == p_idCiclo).FirstOrDefault().EvaluacionMedicionID;
                lista = data.EvaluacionPromocionParticipante.Include(x => x.EvaluacionPromocion).Include(x => x.Participante).Include(x => x.Participante.EvaluacionRespuesta).Where(x => x.EvaluacionPromocionID == p_idPromocion && x.EvaluacionMedicionID == idMedicion && x.EsExterno == false).ToList();
            }
            return lista;
        }

        /// <summary>
        /// Obtiene el promedio de su resultado del alumno.
        /// </summary>
        /// <param name="p_idPromocion">Id Promoción.</param>
        /// <param name="p_idMedicion">Id Medición.</param>
        /// <param name="p_idParticipante">Id Participante.</param>
        /// <returns></returns>
        static public List<sp_PromedioEvaluacion_Result> ObtenerResultadoxParticipante(int p_idPromocion, int p_idMedicion, int p_idParticipante)
        {
           
            using (var data = new BDEvaluacionEntities())
            {
                return data.sp_PromedioEvaluacion(p_idParticipante, p_idPromocion, p_idMedicion).ToList();
            }
        }


        /// <summary>
        /// Obtiene el promedio y desviación standar de la evaluación del participante.
        /// </summary>
        /// <param name="p_idEvaluacion">Id evaluación.</param>
        /// <param name="p_idNivelPadre">Id nivel padre.</param>
        /// <param name="p_idPromocion">Id Promoción.</param>
        /// <param name="p_idCiclo">Id ciclo.</param>
        /// <param name="p_idParticipante">Id Participante.</param>
        /// <returns></returns>
        static public List<PromedioEvaluacionxCicloNivel_Result> ObtenerResultadoFinalxParticipante(int p_idEvaluacion, int p_idNivelPadre, int p_idPromocion, int p_idCiclo, int p_idParticipante)
        {
            using (var data = new BDEvaluacionEntities())
            {
                //data.PromedioEvaluacionxCicloNivel(p_idEvaluacion, p_idNivelPadre, p_idPromocion, p_idCiclo, p_idParticipante).ToList();
                return data.PromedioEvaluacionxCicloNivel(p_idEvaluacion, p_idNivelPadre, p_idPromocion, p_idCiclo, p_idParticipante).ToList();
            }
        }
    }
}

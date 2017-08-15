using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ESAN.Componentes.CoreEvaluacion.Models.General.EvaluacionMSA;


namespace ESAN.Componentes.CoreEvaluacion.Logic.Facade.EvaluacionMSA
{
    public class DAEvaluacionAlternativa
    {
        /// <summary>
        /// Obtener R
        /// </summary>
        /// <param name="p_idPromocion"></param>
        /// <param name="p_idMedicion"></param>
        /// <param name="p_idParticipante"></param>
        /// <param name="p_idPregunta"></param>
        /// <param name="p_idParticipanteEval"></param>
        /// <returns></returns>
        public static EvaluacionRespuesta ObtenerRespuestadeEvaluacion(int p_idPromocion, int p_idMedicion, int p_idParticipante, int p_idPregunta, int p_idParticipanteEval)
        {
            EvaluacionRespuesta objEvaluacionRpta = null;
            using (var data = new BDEvaluacionEntities())
            {
                if (p_idParticipanteEval == 0)
                {
                    objEvaluacionRpta = data.EvaluacionRespuesta.Where(q => q.EvaluacionPromocionID == p_idPromocion && q.EvaluacionMedicionID == p_idMedicion && q.ParticipanteID == p_idParticipante && q.EvaluacionAlternativa.EvaluacionPreguntaID == p_idPregunta).FirstOrDefault();
                }
                else {
                    objEvaluacionRpta = data.EvaluacionRespuesta.Where(q => q.EvaluacionPromocionID == p_idPromocion && q.EvaluacionMedicionID == p_idMedicion && q.ParticipanteID == p_idParticipante && q.EvaluacionAlternativa.EvaluacionPreguntaID == p_idPregunta && q.ParticipanteEvaluadoID == p_idParticipanteEval).FirstOrDefault();
                }
                
            }
            return objEvaluacionRpta;
        }
    }
}

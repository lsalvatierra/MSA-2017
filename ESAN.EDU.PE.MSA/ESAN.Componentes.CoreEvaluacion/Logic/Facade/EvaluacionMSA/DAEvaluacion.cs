using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ESAN.Componentes.CoreEvaluacion.Models.General.EvaluacionMSA;


namespace ESAN.Componentes.CoreEvaluacion.Logic.Facade.EvaluacionMSA
{
    public class DAEvaluacion
    {
        /// <summary>
        /// Obtener la evaluación por promoción.
        /// </summary>
        /// <param name="p_idPromocion">Id Promocion</param>
        /// <returns>Objeto Evaluación.</returns>
        public static Evaluacion ObtenerEvaluacion(int p_idPromocion)
        {
            Evaluacion objParticipante = null;
            using (var data = new BDEvaluacionEntities())
            {
                return objParticipante = data.EvaluacionPromocion.Where(q => q.EvaluacionPromocionID == p_idPromocion).FirstOrDefault().Evaluacion;
            }
        }
    }
}

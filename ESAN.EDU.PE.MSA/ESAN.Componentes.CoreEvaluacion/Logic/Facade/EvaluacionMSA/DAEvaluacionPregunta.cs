using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ESAN.Componentes.CoreEvaluacion.Models.General.EvaluacionMSA;
using System.Data.Entity;

namespace ESAN.Componentes.CoreEvaluacion.Logic.Facade.EvaluacionMSA
{
    public class DAEvaluacionPregunta
    {
        /// <summary>
        /// Preguntas a evalular por nivel. 
        /// </summary>
        /// <param name="p_idNivelPadre">Id nivel padre.</param>
        /// <returns>Lista de Preguntas.</returns>
        public static List<EvaluacionPregunta> ListaPreguntasxNivelEvaluacion(int p_idNivelPadre)
        {
            List<EvaluacionPregunta> lstEvaluacionPreg = null;
            using (var data = new BDEvaluacionEntities())
            {
                lstEvaluacionPreg = data.EvaluacionPregunta.Include(a => a.EvaluacionAlternativa).Where(q => q.EvaluacionNivelID == p_idNivelPadre).ToList();
            }
            return lstEvaluacionPreg;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ESAN.Componentes.CoreEvaluacion.Models.General.EvaluacionMSA;
using System.Data.Entity;


namespace ESAN.Componentes.CoreEvaluacion.Logic.Facade.EvaluacionMSA
{
    public class DAEvaluacionNivelIntro
    {

        /// <summary>
        /// Lista de Niveles de evaluación .
        /// </summary>
        /// <param name="p_idNivel">Id Nivel.</param>
        /// <returns>Objeto niveles de evaluaciones.</returns>
        public static EvaluacionNivelIntro ObtenerEvaluacionNiveleIntro(int p_idNivel)
        {
            EvaluacionNivelIntro objNivelIntro = null;
            using (var data = new BDEvaluacionEntities())
            {
                objNivelIntro = data.EvaluacionNivelIntro.Where(q => q.EvaluacionNivelID == p_idNivel).FirstOrDefault();

            }
            return objNivelIntro;
        }
    }
}

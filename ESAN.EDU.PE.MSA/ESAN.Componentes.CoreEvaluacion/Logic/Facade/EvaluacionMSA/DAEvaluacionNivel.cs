using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ESAN.Componentes.CoreEvaluacion.Models.General.EvaluacionMSA;
using System.Data.Entity;


namespace ESAN.Componentes.CoreEvaluacion.Logic.Facade.EvaluacionMSA
{
    public class DAEvaluacionNivel
    {
        /// <summary>
        /// Lista de Niveles de evaluación .
        /// </summary>
        /// <param name="p_idPromocion">Id Promoción.</param>
        /// <param name="p_idNivelPadre">Id Nivel padre.</param>
        /// <returns>Lista de niveles de evaluaciones.</returns>
        public static List<EvaluacionNivel> ListaNivelesxEvaluacion(int p_idPromocion, int p_idNivelPadre)
        {
            List<EvaluacionNivel> lstEvaluacionNivel = null;
            using (var data = new BDEvaluacionEntities())
            {
                var promocion = data.EvaluacionPromocion.Find(p_idPromocion);

                lstEvaluacionNivel = data.EvaluacionNivel.Include(a => a.EvaluacionNivel1.Select(b => b.EvaluacionNivel1.Select(c => c.EvaluacionNivel1))).Where(q => q.EvaluacionID == promocion.EvaluacionID && q.EvaluacionNivelPadreID == p_idNivelPadre && q.EvaluacionNivelEstado == true).OrderBy(q => q.EvaluacionNivelOrden).ToList();
               
                data.Configuration.LazyLoadingEnabled = false;
            }
            return lstEvaluacionNivel;
        }

    }
}

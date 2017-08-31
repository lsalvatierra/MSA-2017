using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using ESAN.Componentes.CoreEvaluacion.Models.General.EvaluacionMSA;

namespace ESAN.Componentes.CoreEvaluacion.Logic.Facade.EvaluacionMSA
{
    public class DATipoRelacionParticipante
    {
        /// <summary>
        /// Lista los  tipo de relación del alumno.
        /// </summary>
        /// <returns>Lista de objetos TipoRelacionParticipante</returns>
        public static List<TipoRelacionParticipante> ListaTipoRelacion()
        {
            using (var data = new BDEvaluacionEntities())
            {
                return data.TipoRelacionParticipante.Where(q => q.EstadoTipoRelacion == "1").ToList();
            }
        }

    }
}

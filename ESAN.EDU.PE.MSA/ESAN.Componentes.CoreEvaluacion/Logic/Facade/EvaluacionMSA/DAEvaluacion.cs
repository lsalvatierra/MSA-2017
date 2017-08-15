using ESAN.Componentes.CoreEvaluacion.Models.General.EvaluacionMSA;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity;

namespace ESAN.Componentes.CoreEvaluacion.Logic.Facade.EvaluacionMSA
{
    public class DAEvaluacion
    {
        /// <summary>
        /// Listado de todas las evaluaciones activas
        /// </summary>
        /// <returns>List<Evaluacion></returns>
        static public List<Evaluacion> Listado()
        {
            List<Evaluacion> lista = new List<Evaluacion>();
            using (var data = new BDEvaluacionEntities())
            {
                lista = data.Evaluacion.Include(b=>b.EvaluacionCiclo).ToList();
            }
            return lista;
        }

    }
}

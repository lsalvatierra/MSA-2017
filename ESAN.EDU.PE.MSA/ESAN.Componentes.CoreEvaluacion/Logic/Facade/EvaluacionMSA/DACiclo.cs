using ESAN.Componentes.CoreEvaluacion.Models.General.EvaluacionMSA;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity;

namespace ESAN.Componentes.CoreEvaluacion.Logic.Facade.EvaluacionMSA
{
    public class DACiclo
    {
        static public List<EvaluacionCiclo> Listado(int EvaluacionID)
        {
            List<EvaluacionCiclo> lista = new List<EvaluacionCiclo>();
            using (var data = new BDEvaluacionEntities())
            {
                lista = EvaluacionID == -1 ? 
                        data.EvaluacionCiclo.Include(b => b.EvaluacionMedicion).ToList() : 
                        data.EvaluacionCiclo.Where(x => x.EvaluacionID == EvaluacionID).Include(b => b.EvaluacionMedicion).ToList();
            }
            return lista;
        }
    }
}

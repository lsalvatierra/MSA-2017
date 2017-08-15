using ESAN.Componentes.CoreEvaluacion.Models.General.EvaluacionMSA;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity;

namespace ESAN.Componentes.CoreEvaluacion.Logic.Facade.EvaluacionMSA
{
    public class DAPromocionCiclo
    {
        static public List<EvaluacionPromocionCiclo> Listado(int PromocionID)
        {
            List<EvaluacionPromocionCiclo> lista = new List<EvaluacionPromocionCiclo>();
            using (var data = new BDEvaluacionEntities())
            {
                lista = PromocionID == -1 ?
                        data.EvaluacionPromocionCiclo.ToList() :
                        data.EvaluacionPromocionCiclo.Where(x => x.EvaluacionPromocionID == PromocionID).ToList();
            }
            return lista;
        }
    }
}

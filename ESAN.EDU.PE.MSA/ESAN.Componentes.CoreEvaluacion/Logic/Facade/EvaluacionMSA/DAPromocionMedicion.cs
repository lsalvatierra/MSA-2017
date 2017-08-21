using ESAN.Componentes.CoreEvaluacion.Models.General.EvaluacionMSA;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ESAN.Componentes.CoreEvaluacion.Logic.Facade.EvaluacionMSA
{
    public class DAPromocionMedicion
    {
        static public List<EvaluacionPromocionMedicion> Listado(int PromocionID, int CicloID)
        {
            List<EvaluacionPromocionMedicion> lista = new List<EvaluacionPromocionMedicion>();
            using (var data = new BDEvaluacionEntities())
            {
                lista = CicloID == -1 && PromocionID == -1 ?
                        data.EvaluacionPromocionMedicion.ToList() :
                        data.EvaluacionPromocionMedicion.Where(x => x.EvaluacionCicloID == CicloID && x.EvaluacionPromocionID == PromocionID).ToList();
                data.Configuration.LazyLoadingEnabled = false;
            }
            return lista;
        }


        static public bool Actualizar(string tipo, DateTime? fecha, int PromocionID, int CicloID, int MedicionID)
        {
            bool exito = true;
            EvaluacionPromocionMedicion medicionActual = new EvaluacionPromocionMedicion();
            try
            {
                using (var data = new BDEvaluacionEntities())
                {
                    medicionActual = data.EvaluacionPromocionMedicion.Where(x => x.EvaluacionPromocionID == PromocionID && x.EvaluacionCicloID == CicloID && x.EvaluacionMedicionID == MedicionID).FirstOrDefault();
                    if (tipo.Equals("I"))
                        medicionActual.EvaluacionPromMedicionFecIni = fecha;
                    else
                        medicionActual.EvaluacionPromMedicionFecFin = fecha;
                    data.SaveChanges();
                }
            }
            catch
            {
                exito = false;
            }
            return exito;
        }

        //Prueba
        //asdasdasd
    }
}

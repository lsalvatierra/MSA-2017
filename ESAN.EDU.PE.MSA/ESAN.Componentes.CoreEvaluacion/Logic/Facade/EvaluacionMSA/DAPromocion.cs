using ESAN.Componentes.CoreEvaluacion.Models.General.EvaluacionMSA;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ESAN.Componentes.CoreEvaluacion.Logic.Facade.EvaluacionMSA
{
    public class DAPromocion
    {
        /// <summary>
        /// Listado de todas las promociones activas creadas
        /// </summary>
        /// <returns>List<EvaluacionPromocion></returns>
        static public List<EvaluacionPromocion> Listado()
        {
            List<EvaluacionPromocion> lista = new List<EvaluacionPromocion>();
            using (var data = new BDEvaluacionEntities())
            {
                lista = data.EvaluacionPromocion.ToList();
            }
            return lista;
        }
        /// <summary>
        /// Registro de la promoción
        /// </summary>
        /// <param name="promocion">Objeto EvaluacionPromocion</param>
        /// <returns>Booleano indica exito o fracaso del registro</returns>
        static public bool Registrar(EvaluacionPromocion promocion)
        {
            bool exito = true;
            try
            {
                using (var data = new BDEvaluacionEntities())
                {
                    data.EvaluacionPromocion.Add(promocion);
                    data.SaveChanges();
                }
            }
            catch
            {
                exito = false;
            }
            return exito;
        }
        /// <summary>
        /// Actualización de la promoción
        /// </summary>
        /// <param name="promocion">Objeto EvaluacionPromocion<</param>
        /// <returns>Booleano indica exito o fracaso de la actualización</returns>
        static public bool Actualizar(EvaluacionPromocion promocion)
        {
            bool exito = true;
            EvaluacionPromocion promocionActual = new EvaluacionPromocion();
            try
            {
                using (var data = new BDEvaluacionEntities())
                {
                    promocionActual = data.EvaluacionPromocion.Where(x => x.EvaluacionPromocionID == promocion.EvaluacionPromocionID).FirstOrDefault();
                    promocionActual.EvaluacionPromocionDescripcion = promocion.EvaluacionPromocionDescripcion;
                    promocionActual.EvaluacionPromocionCodigo = promocion.EvaluacionPromocionCodigo;
                    promocionActual.EvaluacionPromocionFecIni = promocion.EvaluacionPromocionFecIni;
                    promocionActual.EvaluacionPromocionFecFin = promocion.EvaluacionPromocionFecFin;
                    promocionActual.EvaluacionPromocionID = promocion.EvaluacionPromocionID;
                    promocionActual.EvaluacionID = promocion.EvaluacionID;
                    data.SaveChanges();
                }
            }
            catch
            {
                exito = false;
            }
            return exito;
        }
        /// <summary>
        /// Eliminar una promoción
        /// </summary>
        /// <param name="EvaluacionPromocionID">ID</param>
        /// <returns>Booleano indica exito o fracaso de la eliminación</returns>
        static public bool Eliminar(int EvaluacionPromocionID)
        {
            bool exito = true;
            EvaluacionPromocion promocion = new EvaluacionPromocion();
            try
            {
                using (var data = new BDEvaluacionEntities())
                {
                    promocion= data.EvaluacionPromocion.Where(x => x.EvaluacionPromocionID == EvaluacionPromocionID).FirstOrDefault();
                    //promocion.EvaluacionPromocionEstado = false;
                    data.SaveChanges();
                }
            }
            catch
            {
                exito = false;
            }

            return exito;
        }

    }
}

using ESAN.Componentes.CoreEvaluacion.Models.General.EvaluacionMSA;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity;

namespace ESAN.Componentes.CoreEvaluacion.Logic.Facade.EvaluacionMSA
{
    public class DAPromocion
    {
        /// <summary>
        /// Listado de todas las promociones activas creadas
        /// </summary>
        /// <returns>List<EvaluacionPromocion></returns>
        static public List<EvaluacionPromocion> Listado(int EvaluacionPromocionID)
        {
            List<EvaluacionPromocion> lista = new List<EvaluacionPromocion>();
            using (var data = new BDEvaluacionEntities())
            {
                lista = EvaluacionPromocionID == -1 ?
                        data.EvaluacionPromocion.Where(x => x.EvaluacionPromocionEstado == true)
                        .Include(b => b.EvaluacionPromocionCiclo).Include(c => c.EvaluacionPromocionMedicion).Include(d => d.EvaluacionPromocionParticipante).ToList() :
                        data.EvaluacionPromocion.Where(x => x.EvaluacionPromocionEstado == true && x.EvaluacionPromocionID == EvaluacionPromocionID)
                        .Include(b => b.EvaluacionPromocionCiclo).Include(c => c.EvaluacionPromocionMedicion).Include(d => d.EvaluacionPromocionParticipante).ToList();

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
                    using (var dbContextTransaction = data.Database.BeginTransaction())
                    {
                        try
                        {
                            //Se registra la promoción
                            promocion.EvaluacionPromocionEstado = true;
                            data.EvaluacionPromocion.Add(promocion);
                            data.SaveChanges();
                            //Obtener el ciclo de la evaluación
                            List<EvaluacionCiclo> listaCiclo = data.EvaluacionCiclo.Where(x => x.EvaluacionID == promocion.EvaluacionID).ToList();
                            //Se registra el ciclo de la promoción
                            List<EvaluacionPromocionCiclo> listaPromocionCiclo = new List<EvaluacionPromocionCiclo>();
                            EvaluacionPromocionCiclo promocionCiclo;
                            List<EvaluacionPromocionMedicion> listaPromocionMedicion = new List<EvaluacionPromocionMedicion>();
                            List<EvaluacionMedicion> listaMedicion = new List<EvaluacionMedicion>();
                            EvaluacionPromocionMedicion promocionMedicion;
                            foreach (EvaluacionCiclo item in listaCiclo)
                            {
                                promocionCiclo = new EvaluacionPromocionCiclo();
                                promocionCiclo.EvaluacionCicloID = item.EvaluacionCicloID;
                                promocionCiclo.EvaluacionPromocionID = promocion.EvaluacionPromocionID;
                                promocionCiclo.EvaluacionCicloDescripcion = item.EvaluacionCicloDescripcion;
                                listaPromocionCiclo.Add(promocionCiclo);

                                listaMedicion = data.EvaluacionMedicion.Where(a => a.EvaluacionCicloID == item.EvaluacionCicloID).ToList();
                                foreach (EvaluacionMedicion item2 in listaMedicion)
                                {
                                    promocionMedicion = new EvaluacionPromocionMedicion();
                                    promocionMedicion.EvaluacionMedicionID = item2.EvaluacionMedicionID;
                                    promocionMedicion.EvaluacionCicloID = item.EvaluacionCicloID;
                                    promocionMedicion.EvaluacionMedicionDescripcion = item2.EvaluacionMedicionDescripcion;
                                    promocionMedicion.EvaluacionPromocionID = promocion.EvaluacionPromocionID;
                                    listaPromocionMedicion.Add(promocionMedicion);
                                }
                            }
                            data.EvaluacionPromocionCiclo.AddRange(listaPromocionCiclo);
                            data.EvaluacionPromocionMedicion.AddRange(listaPromocionMedicion);
                            //Confirmar datos
                            data.SaveChanges();
                            dbContextTransaction.Commit();
                        }
                        catch (Exception ex)
                        {
                            dbContextTransaction.Rollback();
                            exito = false;
                        }

                    }
                }
            }
            catch (Exception ex)
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
                    promocion = data.EvaluacionPromocion.Where(x => x.EvaluacionPromocionID == EvaluacionPromocionID).FirstOrDefault();
                    promocion.EvaluacionPromocionEstado = false;
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
        /// 
        /// </summary>
        /// <param name="EvaluacionPromocionID"></param>
        /// <returns></returns>
        static public List<EvaluacionPromocionParticipante> ListadoParticipante(int EvaluacionPromocionID, int EvaluacionMedicionID)
        {
            List<EvaluacionPromocionParticipante> lista = new List<EvaluacionPromocionParticipante>();
            using (var data = new BDEvaluacionEntities())
            {
                lista = EvaluacionPromocionID == -1 && EvaluacionMedicionID == -1 ?
                        data.EvaluacionPromocionParticipante.Include(x=>x.Participante).ToList() :
                        data.EvaluacionPromocionParticipante.Include(x => x.Participante).Where(x => x.EvaluacionPromocionID == EvaluacionPromocionID && x.EvaluacionMedicionID == EvaluacionMedicionID).ToList();

            }
            return lista;
        }
    }
}

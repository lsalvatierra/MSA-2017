using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ESAN.Componentes.CoreEvaluacion.Models.General.EvaluacionMSA;
using System.Data.Common;
using System.Data;

namespace ESAN.Componentes.CoreEvaluacion.Logic.Facade.EvaluacionMSA
{
    public class DAEvaluacionRespuesta
    {

        /// <summary>
        /// 
        /// </summary>
        /// <param name="p_lstEvaluacionRpta"></param>
        /// <returns></returns>
        public static bool RegistrarRespuestaEvaluacion(List<EvaluacionRespuesta> p_lstEvaluacionRpta)
        {
            bool rpta = false;
            using (var data = new BDEvaluacionEntities())
            {
                data.Database.Connection.Open();
                using (DbTransaction transacction = data.Database.Connection.BeginTransaction(IsolationLevel.ReadCommitted)) {
                    data.Database.UseTransaction(transacction);
                    try {
                        foreach (var objEvalRpta in p_lstEvaluacionRpta)
                        {
                            data.EvaluacionRespuesta.Add(new EvaluacionRespuesta {
                                EvaluacionPromocionID = objEvalRpta.EvaluacionPromocionID,
                                ParticipanteID = objEvalRpta.ParticipanteID,
                                EvaluacionAlternativaID = objEvalRpta.EvaluacionAlternativaID,
                                EvaluacionMedicionID = objEvalRpta.EvaluacionMedicionID,
                                ParticipanteEvaluadoID = objEvalRpta.ParticipanteEvaluadoID
                            });
                        }
                        data.SaveChanges();
                        transacction.Commit();
                        rpta = true;
                    } catch (Exception ex) {
                        transacction.Rollback();
                        rpta = false;
                    }

                }
            }
            return rpta;
        }
    }
}

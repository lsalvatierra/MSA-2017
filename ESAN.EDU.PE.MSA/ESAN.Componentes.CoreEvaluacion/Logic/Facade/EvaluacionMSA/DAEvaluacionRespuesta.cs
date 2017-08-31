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
        public static bool RegistrarRespuestaEvaluacion(List<EvaluacionRespuesta> p_lstEvaluacionRpta, Participante p_objParticipante)
        {
            bool rpta = false;
            using (var data = new BDEvaluacionEntities())
            {
                data.Database.Connection.Open();
                using (DbTransaction transacction = data.Database.Connection.BeginTransaction(IsolationLevel.ReadCommitted)) {
                    data.Database.UseTransaction(transacction);
                    try {
                        if (p_objParticipante.ParticipanteID > 0)
                        {
                            data.EvaluacionPromocionParticipante.Add(p_objParticipante.EvaluacionPromocionParticipante.FirstOrDefault());
                            foreach (var objEvalRpta in p_lstEvaluacionRpta)
                            {
                                data.EvaluacionRespuesta.Add(new EvaluacionRespuesta
                                {
                                    EvaluacionPromocionID = objEvalRpta.EvaluacionPromocionID,
                                    ParticipanteID = p_objParticipante.ParticipanteID,
                                    EvaluacionAlternativaID = objEvalRpta.EvaluacionAlternativaID,
                                    EvaluacionMedicionID = objEvalRpta.EvaluacionMedicionID,
                                    ParticipanteEvaluadoID = objEvalRpta.ParticipanteEvaluadoID
                                });
                            }
                        }
                        else
                        {
                            foreach (var objEvalRpta in p_lstEvaluacionRpta)
                            {
                                data.EvaluacionRespuesta.Add(new EvaluacionRespuesta
                                {
                                    EvaluacionPromocionID = objEvalRpta.EvaluacionPromocionID,
                                    Participante = p_objParticipante,
                                    EvaluacionAlternativaID = objEvalRpta.EvaluacionAlternativaID,
                                    EvaluacionMedicionID = objEvalRpta.EvaluacionMedicionID,
                                    ParticipanteEvaluadoID = objEvalRpta.ParticipanteEvaluadoID
                                });
                            }
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

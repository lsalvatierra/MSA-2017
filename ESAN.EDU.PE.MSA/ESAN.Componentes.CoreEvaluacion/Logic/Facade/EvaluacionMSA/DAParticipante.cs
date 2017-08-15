using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ESAN.Componentes.CoreEvaluacion.Models.General.EvaluacionMSA;

namespace ESAN.Componentes.CoreEvaluacion.Logic.Facade.EvaluacionMSA
{
    public class DAParticipante
    {

        /// <summary>
        /// Lista de tipos de documentos de identidad
        /// </summary>
        /// <param name="p_idTipoDocumento">Tipo de documento.</param>
        /// <param name="p_nroDocumento">Nro de documento</param>
        /// <returns>Objeto Participante</returns>
        public static Participante ObtenerParticipante(int p_idTipoDocumento, string p_nroDocumento)
        {
            Participante objParticipante = null;
            using (var data = new BDEvaluacionEntities())
            {
                return objParticipante = data.Participante.Where(q => q.TipoDocumentoID == p_idTipoDocumento && q.ParticipanteNroDoc == p_nroDocumento).FirstOrDefault();
            }
        }

        /// <summary>
        /// Obtener participante por ID
        /// </summary>
        /// <param name="p_idParticipante">Id participante</param>
        /// <returns>Objeto Participante.</returns>
        public static Participante ObtenerParticipantexID(int p_idParticipante)
        {
            Participante objParticipante = null;
            using (var data = new BDEvaluacionEntities())
            {
                return objParticipante = data.Participante.Where(q => q.TipoDocumentoID == p_idParticipante).FirstOrDefault();
            }
        }

        /// <summary>
        /// Registrar Participante
        /// </summary>
        /// <param name="p_objParticipante">Objeto Participante</param>
        /// <returns>True o false</returns>
        public static int RegistrarParticipante(Participante p_objParticipante, int p_idPromocion)
        {
            int rpta = 0;
            try
            {
                using (var data = new BDEvaluacionEntities())
                {
                    var promocion = data.EvaluacionPromocion.Find(p_idPromocion);
                    //promocion.Participante.Add(p_objParticipante);
                    rpta = data.SaveChanges();
                }
            }
            catch {
                rpta = 0;
            }
            return rpta;
        }
    }
}

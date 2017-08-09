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
        /// Registrar Participante
        /// </summary>
        /// <param name="p_objParticipante">Objeto Participante</param>
        /// <returns>True o false</returns>
        public static bool RegistrarParticipante(Participante p_objParticipante)
        {
            bool rpta = false;
            try
            {
                using (var data = new BDEvaluacionEntities())
                {
                    data.Participante.Add(p_objParticipante);
                    rpta = data.SaveChanges() > 0 ? true : false;
                }
            }
            catch {
                rpta = false;
            }
            return rpta;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ESAN.Componentes.CoreEvaluacion.Models.General.EvaluacionMSA;


namespace ESAN.Componentes.CoreEvaluacion.Logic.Facade.EvaluacionMSA
{
    public class DAPromocionMedicionCicloParticipante
    {
        /// <summary>
        /// Verifica si existe el participante en la promoción.
        /// </summary>
        /// <param name="p_idPromocion">Id promoción</param>
        /// <param name="p_idCiclo">Id ciclo.</param>
        /// <param name="p_idMedicion">Id Medición.</param>
        /// <param name="p_nroDocumento">Nro de documento.</param>
        /// <returns></returns>
        public static bool ExisteParticipantePromocionCicloMedicion(int p_idPromocion, int p_idMedicion, string p_nroDocumento)
        {
            using (var data = new BDEvaluacionEntities())
            {
                long idciclo = data.EvaluacionPromocionMedicion.Where(q => q.EvaluacionPromocionID == p_idPromocion && q.EvaluacionMedicionID == p_idMedicion).FirstOrDefault().EvaluacionCicloID;
                return data.PromocionMedicionCicloParticipante.Where(q => q.EvaluacionPromocionID == p_idPromocion && q.EvaluacionCicloID == idciclo && q.EvaluacionMedicionID == p_idMedicion && q.ParticipanteNroDoc == p_nroDocumento).Count() > 0 ? true : false;
            }
        }

        /// <summary>
        /// Listado de participantes de la promoción por medición
        /// </summary>
        /// <param name="p_idPromocion">Id promoción</param>
        /// <param name="p_idMedicion">Id Medición.</param>
        /// <returns>List<PromocionMedicionCicloParticipante></returns>
        public static List<PromocionMedicionCicloParticipante> Listado(int p_idPromocion, int p_idMedicion)
        {
            using (var data = new BDEvaluacionEntities())
            {
                return data.PromocionMedicionCicloParticipante.Where(x => x.EvaluacionPromocionID == p_idPromocion && x.EvaluacionMedicionID == p_idMedicion && x.Estado == true).ToList();
            }

        }




    }


}

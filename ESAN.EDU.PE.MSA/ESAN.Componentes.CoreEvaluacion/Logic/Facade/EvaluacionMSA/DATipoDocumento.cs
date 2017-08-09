using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ESAN.Componentes.CoreEvaluacion.Models.General.EvaluacionMSA;

namespace ESAN.Componentes.CoreEvaluacion.Logic.Facade.EvaluacionMSA
{
    public class DATipoDocumento
    {
        /// <summary>
        /// Lista de tipos de documentos de identidad
        /// </summary>
        /// <returns></returns>
        public static List<TipoDocumento> ListaTipoDocumentos()
        {
            List<TipoDocumento> lista = new List<TipoDocumento>();
            using (var data = new BDEvaluacionEntities())
            {
                return data.TipoDocumento.Where(q => q.TipoDocumentoEstado == true).ToList();
            }
        }

    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ESAN.Componentes.CoreEvaluacion.Models.General.EvaluacionMSA;


namespace ESAN.Componentes.CoreEvaluacion.Logic.Facade.EvaluacionMSA
{
    public class DAEvaluacionUsuario
    {


        /// <summary>
        /// Obtener al usuario del sistema.
        /// </summary>
        /// <param name="p_usuario">Tipo de documento.</param>
        /// <param name="p_password">Nro de documento</param>
        /// <returns>Objeto Usuario</returns>
        public static EvaluacionUsuario ObtenerUsuario(string p_usuario, string p_password)
        {
            using (var data = new BDEvaluacionEntities())
            {
                return data.EvaluacionUsuario.Where(q => q.CodUsuario == p_usuario && q.PassUsuario == p_password).FirstOrDefault();
            }
        }
    }
}

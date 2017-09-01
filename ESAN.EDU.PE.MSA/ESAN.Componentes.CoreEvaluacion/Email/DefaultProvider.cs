using System;
using System.Collections.Generic;
using System.Linq;
using System.Configuration;

using ESAN.Componentes.CoreEvaluacion.Email.Configuration;

namespace ESAN.Componentes.CoreEvaluacion.Email
{
    public class DefaultProvider : EmailProvider
    {
        private readonly DefaultConfigData _configuration;

        public DefaultProvider(string providerName)
        {

            object seccion = ConfigurationManager.GetSection("customEmailConfig/" + providerName);
            if (string.IsNullOrEmpty(providerName) || seccion == null)
            {
                _configuration = new DefaultConfigData("noreply@esan.edu.pe", "", "Sin asunto", false, "");
            }
            else
            {
                _configuration = seccion as DefaultConfigData;
            }

            Configuracion = _configuration;
            Subject = _configuration.Asunto;
            AgregarDireccion(TipoDirecciones.From, _configuration.FromAddress);
            AgregarDireccion(TipoDirecciones.To, _configuration.ToAddresss);

            if (!string.IsNullOrEmpty(_configuration.CcAddresss))
            {
                AgregarDireccion(TipoDirecciones.CC, _configuration.CcAddresss);
            }

            if (!string.IsNullOrEmpty(_configuration.BccAddresss))
            {
                AgregarDireccion(TipoDirecciones.BCC, _configuration.BccAddresss);
            }

            if (!string.IsNullOrEmpty(_configuration.CorreoPrueba))
            {
                AgregarDireccionPrueba(_configuration.CorreoPrueba);
            }
        }
    }
}

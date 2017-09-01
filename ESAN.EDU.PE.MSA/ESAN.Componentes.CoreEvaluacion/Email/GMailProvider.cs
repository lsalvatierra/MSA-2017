using System;
using System.Collections.Generic;
using System.Linq;
using System.Configuration;

using ESAN.Componentes.CoreEvaluacion.Email.Configuration;
using System.Net.Mail;
using System.Net;

namespace ESAN.Componentes.CoreEvaluacion.Email
{
    public class GMailProvider : EmailProvider
    {
        private readonly GMailConfigData _configuration;

        public GMailProvider(string providerName)
        {
            object seccion = ConfigurationManager.GetSection("customEmailConfig/" + providerName);
           
            if (string.IsNullOrEmpty(providerName) || seccion == null)
            {
                _configuration = new GMailConfigData("noreplay@esan.edu.pe", "", "Sin asunto");
            }
            else
            {
                _configuration = seccion as GMailConfigData;
            }

            _clientSMTP = new SmtpClient();
            _clientSMTP.Credentials = new NetworkCredential(_configuration.Usuario, _configuration.Password);
            _clientSMTP.Port = 587;
            _clientSMTP.Host = "smtp.gmail.com";
            _clientSMTP.EnableSsl = true;

            Subject = _configuration.Asunto;
            AgregarDireccion(TipoDirecciones.From, _configuration.FromAddress);
            AgregarDireccion(TipoDirecciones.To, _configuration.ToAddresss);
        }
    }
}

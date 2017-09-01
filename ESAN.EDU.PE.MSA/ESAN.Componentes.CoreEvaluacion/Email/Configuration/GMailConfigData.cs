using System;
using System.Collections.Generic;
using System.Linq;
using System.Configuration;
using System.ComponentModel;

namespace ESAN.Componentes.CoreEvaluacion.Email.Configuration
{
    public class GMailConfigData : DefaultConfigData
    {

        private const string usuarioProperty = "usuario";
        private const string passwordProperty = "password";

        public GMailConfigData()
        {
        }

        public GMailConfigData(string from, string to, string asunto)
        {
            FromAddress = from;
            ToAddresss = to;
            Asunto = asunto;
        }

        #region Propiedades de retorno de AppSettings

        [ConfigurationProperty(usuarioProperty, IsRequired = true)]
        public string Usuario
        {
            get
            {
                return (string)this[usuarioProperty];
            }
            set
            {
                this[usuarioProperty] = value;
            }
        }

        [ConfigurationProperty(passwordProperty, IsRequired = false)]
        public string Password
        {
            get
            {
                return (string)this[passwordProperty];
            }
            set
            {
                this[passwordProperty] = value;
            }
        }

#endregion

    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Configuration;
using System.ComponentModel;

namespace ESAN.Componentes.CoreEvaluacion.Email.Configuration
{
    public class DefaultConfigData : ConfigurationSection
    {
        public const string typeProperty = "type";

        [ConfigurationProperty(typeProperty, IsRequired = true)]
        [Browsable(true)]
        public string TypeName
        {
            get
            {
                return (string)this[typeProperty];
            }
            set
            {
                this[typeProperty] = value;
            }
        }

        private const string fromProperty = "fromAddress";
        private const string toProperty = "toAddress";
        private const string ccProperty = "ccAddress";
        private const string bccProperty = "bccAddress";
        private const string subjectProperty = "subject";
        private const string enviarCorreo = "enviarCorreo";
        private const string correoPrueba = "correoPrueba";

        public DefaultConfigData()
        {
        }

        public DefaultConfigData(string from, string to, string asunto
            , bool _enviarCorreo
            , string _correoPrueba
            )
        {
            FromAddress = from;
            ToAddresss = to;
            Asunto = asunto;
            EnviarCorreo = _enviarCorreo;
            CorreoPrueba = _correoPrueba;
        }

        #region Propiedades de retorno de AppSettings

        [ConfigurationProperty(fromProperty, IsRequired = true)]
        public string FromAddress
        {
            get
            {
                return (string)this[fromProperty];
            }
            set
            {
                this[fromProperty] = value;
            }
        }

        [ConfigurationProperty(toProperty, IsRequired = false)]
        public string ToAddresss
        {
            get
            {
                return (string)this[toProperty];
            }
            set
            {
                this[toProperty] = value;
            }
        }

        [ConfigurationProperty(ccProperty, IsRequired = false)]
        public string CcAddresss
        {
            get
            {
                return (string)this[ccProperty];
            }
            set
            {
                this[ccProperty] = value;
            }
        }

        [ConfigurationProperty(bccProperty, IsRequired = false)]
        public string BccAddresss
        {
            get
            {
                return (string)this[bccProperty];
            }
            set
            {
                this[bccProperty] = value;
            }
        }

        [ConfigurationProperty(correoPrueba, IsRequired = false)]
        public string CorreoPrueba
        {
            get
            {
                return (string)this[correoPrueba];
            }
            set
            {
                this[correoPrueba] = value;
            }
        }

        [ConfigurationProperty(enviarCorreo, IsRequired = false)]
        public bool EnviarCorreo
        {
            get
            {
                return (bool)this[enviarCorreo];
            }
            set
            {
                this[enviarCorreo] = (bool)value;
            }
        }

        [ConfigurationProperty(subjectProperty, IsRequired = false)]
        public string Asunto
        {
            get
            {
                return (string)this[subjectProperty];
            }
            set
            {
                this[subjectProperty] = value;
            }
        }

        #endregion
    }
}

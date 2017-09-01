using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;

using System.Text.RegularExpressions;
using System.Text;
using ESAN.Componentes.CoreEvaluacion.Email.Configuration;

namespace ESAN.Componentes.CoreEvaluacion.Email
{
    public abstract class EmailProvider : IDisposable
    {
        protected SmtpClient _clientSMTP = new SmtpClient();
        private MailMessage _correo = new MailMessage();
        private Regex _validador = new Regex("(?<user>[^@]+)@(?<host>.+)");

        #region Propiedades de la clase
        public String Subject
        {
            set
            {
                _correo.Subject = value;
            }
            get
            {
                return _correo.Subject;
            }
        }

        public MailAddress FromAddress
        {
            get
            {
                return _correo.From;
            }
        }

        public MailAddressCollection BCCAddresses
        {
            get
            {
                return _correo.Bcc;
            }
        }

        public MailAddressCollection CCAddresses
        {
            get
            {
                return _correo.CC;
            }
        }

        public MailAddressCollection CorreoPrueba
        {
            get
            {
                return _correo.CC;
            }
        }

        public MailAddressCollection ToAddresses
        {
            get
            {
                return _correo.To;
            }
        }

        public DefaultConfigData Configuracion;

        #endregion

        public void AgregarDireccionPrueba(string direccion)
        {
            _correo.Bcc.Add(direccion);
        }


        public void AgregarDireccion(TipoDirecciones tipo, string direccion)
        {
            //De solo lectura
            if (_validador.IsMatch(direccion))
            {

                if (!Configuracion.EnviarCorreo)
                {
                    direccion = "prueba-esandata-" + direccion;
                }


                switch (tipo)
                {
                    case TipoDirecciones.From:
                        _correo.From = new MailAddress(direccion);
                        break;
                    case TipoDirecciones.To:
                        _correo.To.Add(direccion);
                        break;
                    case TipoDirecciones.CC:
                        _correo.CC.Add(direccion);
                        break;
                    case TipoDirecciones.BCC:
                        _correo.Bcc.Add(direccion);
                        break;
                }
            }
        }

        public void AgregarAdjunto(string nombreArchivo)
        {
            _correo.Attachments.Add(new Attachment(nombreArchivo));
        }
        public virtual bool Enviar(string mensaje, bool esHTML, MailPriority prioridad)
        {
            bool exito = true;

            _correo.Priority = prioridad;
            _correo.Body = mensaje;
            _correo.IsBodyHtml = esHTML;
            _correo.BodyEncoding = Encoding.UTF8;

            try
            {
                _clientSMTP.Send(_correo);
            }
            catch { exito = false; }

            return exito;
        }

        public virtual bool Enviar(string mensaje, string asunto, bool esHTML, MailPriority prioridad)
        {
            bool exito = true;

            _correo.Priority = prioridad;
            _correo.Body = mensaje;
            _correo.IsBodyHtml = esHTML;
            _correo.Subject = asunto;
            _correo.BodyEncoding = Encoding.UTF8;

            try
            {
                _clientSMTP.Send(_correo);
            }
            catch { exito = false; }

            return exito;
        }
        #region Miembros de IDisposable

        void IDisposable.Dispose()
        {
            _correo.Dispose();
            _correo = null;

            _validador = null;
        }

        #endregion
    }
}

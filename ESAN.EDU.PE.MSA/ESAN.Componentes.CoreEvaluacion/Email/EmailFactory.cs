using System;
using System.Collections.Generic;
using System.Linq;


namespace ESAN.Componentes.CoreEvaluacion.Email
{

    public static class EmailFactory
    {
        public enum Providers
        {
            Default,
            GMail
        }

        public static EmailProvider GetEmailProvider()
        {
            return GetEmailProvider( Providers.Default, "Desarrollo");

        }

        /// <summary>
        /// Retorna la clase SAPContextBase con la configuración correspondiente
        /// </summary>
        /// <param name="esProduccion">Valor que indica si utilizará los parámetros de conexión al entorno de producción o desarrollo-pruebas</param>
        /// <returns>Un objeto con las funciones SAP</returns>
        public static EmailProvider GetEmailProvider(Providers providerType, string providerName)
        {
            EmailProvider provider = null;
            try
            {
                switch (providerType)
                {
                    case Providers.Default:
                        provider = new DefaultProvider(providerName);
                        break;
                    case Providers.GMail:
                        provider = new GMailProvider(providerName);
                        break;
                }

            }
            catch (Exception ex)
            {
                throw new Exception("Error al intentar instanciar el provider.", ex);
            }
            return provider;

        }
    }
}

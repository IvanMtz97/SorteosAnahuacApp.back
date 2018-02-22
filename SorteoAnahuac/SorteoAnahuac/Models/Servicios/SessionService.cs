using System;
using System.Configuration;
using System.IO;


namespace SorteoAnahuac.Models
{
    public static class SessionService
    {

        /// <summary>
        /// Función que genera un token para un usuario
        /// </summary>
        /// <param name="usuario">Usuario para el cual se quiere generar un token</param>
        /// <returns>Token con los datos de sesión del usuario</returns>
        public static string GeneraToken(string usuario)
        {
            string jsonSesion = string.Empty;
            using (StringWriter output = new StringWriter())
            {
                Jil.JSON.Serialize<Colaborador>(
                   new Colaborador()
                   {
                       correo = usuario,
                       expira = DateTime.Now.AddMinutes(int.Parse(ConfigurationManager.AppSettings["Seguridad.Expiracion"]) + 10)
                   },
                    output
                );
                jsonSesion = output.ToString();
            }

            return new Code.TokenUtil().Encripta(jsonSesion);
        }

        /// <summary>
        /// Función que descifra un token y obtiene los datos de sesión almacenados en el
        /// </summary>
        /// <param name="token">Token a descifrar</param>
        /// <returns>Objeto con valores de sesión. Null si no se pudo descifrar</returns>
        public static Colaborador DescifraToken(string token)
        {
            Colaborador objSesion = null;

            try
            {
                string jsonSesion = new Code.TokenUtil().Desencripta(token);
                objSesion = Jil.JSON.Deserialize<Colaborador>(jsonSesion);
            }
            catch
            {
                objSesion = null;
            };

            return objSesion;
        }

        /// <summary>
        /// Función que valida un token de usuario
        /// </summary>
        /// <param name="token">Valor del token a validar</param>
        /// <returns>Booleano con True si el token es valido para la fecha actual. False cuando el token no es válido o ha expirado.</returns>
        public static bool ValidaToken(string token)
        {
            bool valido = false;

            try
            {
                Colaborador objSesion = DescifraToken(token);

                if (objSesion != null && objSesion.expira.CompareTo(DateTime.Now) > 0)
                {
                    valido = true;
                };

            }
            catch
            {
                valido = false;
            };

            return valido;

        }
    }
}
using System.Configuration;
using System.Linq;

namespace SorteoAnahuac.Models
{
    /// <summary>
    /// Clase que permite autenticar y manejar sesiones de usuario
    /// </summary>
    public static class UsuarioService
    {
        /// <summary>
        /// Función que valida credeciales para regresar un colaborador activo
        /// </summary>
        /// <param name="correo"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        public static Usuario Auntenticar(string correo, string password)
        {
            Usuario persona = Obtiene(correo);

            //if (persona != null)
            //{
            //    System.Net.WebClient cliente = new System.Net.WebClient();
            //    cliente.Credentials = new System.Net.NetworkCredential(correo, password);
            //    try
            //    {
            //        string usuario = cliente.DownloadString("https://outlook.office365.com/api/v1.0/me/");
            //    }
            //    catch
            //    {
            //        persona = null;
            //    }
            //}

            return persona;

        }

        /// <summary>
        /// Función que revisa que el correo sea el de un colaborador válido, y trae sus datos.
        /// </summary>
        /// <param name="correo">Cuenta de correo a validar</param>
        /// <returns>Objeto Colaborador con los datos personales obtenidos por el correo electrónico</returns>
        public static Usuario Obtiene(string correo)
        {
            Usuario persona = null;

            /* Solo se permite el acceso a personas que tengan fuentas de correo terminadas en "@anahuac.mx" */

            //if (correo.ToLowerInvariant().EndsWith("@anahuac.mx"))
            //{
            //    using (Models.Sorteo2016Entities db = new Sorteo2016Entities())
            //    {
            //        /* Buscamos al colaborador para revisar que este registrado como un becario en el sorteo activo actual */
            //        /* Para saber que es becario, se busca que este registrado en un nicho con clave "0001" */
            //        USUARIOS_CORREOS dbCorreo = db.USUARIOS_CORREOS.Where(c => c.CORREO == correo).FirstOrDefault();

            //        if (dbCorreo != null) { 

            //            USUARIO dbPersona = db.USUARIOS.Where(c => c.USUARIO1 == dbCorreo.USUARIO).FirstOrDefault();
            //            if (dbPersona != null)
            //            {
            //                persona = new Usuario()
            //                {
            //                    clave = dbPersona.PK1
            //                };
            //            };
            //        };
            //    };

            //    if (persona != null)
            //    {
            //        persona = ObtienePorClave(persona.clave);
            //    }
            //}

            return persona;
        }



        /// <summary>
        /// Función que revisa trae un colaborador por su clave.
        /// </summary>
        /// <param name="clave">Clave del colaborador que se desea revisar</param>
        /// <returns>Objeto Colaborador con los datos personales </returns>
        public static Usuario ObtienePorClave(long clave)
        {
            Usuario persona = null;

            /* Solo se permite el acceso a personas que tengan fuentas de correo terminadas en "@anahuac.mx" */

            //using (Models.Sorteo2016Entities db = new Sorteo2016Entities())
            //{
            //    /* Buscamos al colaborador para revisar que este registrado como un becario en el sorteo activo actual */
            //    /* Para saber que es becario, se busca que este registrado en un nicho con clave "0001" */
            //    long permiso = long.Parse(ConfigurationManager.AppSettings["Seguridad.Permiso"]);
            //    USUARIO dbPersona = db.USUARIOS.Where(c => c.PK1 == clave && c.ROLES.Where(r => r.ROL.PERMISOS.Where(p => p.PERMISO.PK1 == permiso).Count() > 0).Count() > 0).FirstOrDefault();
            //    USUARIOS_CORREOS dbCorreo = db.USUARIOS_CORREOS.Where(c => c.USUARIO == dbPersona.USUARIO1 && c.CORREO.ToLower().EndsWith("@anahuac.mx")).FirstOrDefault();
            //    if (dbPersona != null && dbCorreo != null)
            //    {
            //        persona = new Usuario()
            //        {
            //            clave = dbPersona.PK1,
            //            identificador = dbPersona.USUARIO1,
            //            nombre = dbPersona.NOMBRE,
            //            apellido_paterno = dbPersona.APATERNO,
            //            apellido_materno = dbPersona.AMATERNO,
            //            correo = dbCorreo.CORREO.ToLower()
            //        };
            //    };
            //};

            return persona;
        }
    }
}
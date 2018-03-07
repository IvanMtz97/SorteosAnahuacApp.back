using ConnectDB;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SorteoAnahuac.Models
{
    /// <summary>
    /// Servicio que permite manipular información de comprador de boletos.
    /// </summary>
    public static class CompradorService
    {
        /// <summary>
        /// Función que revisa si hay compradores asignados para un colaborador.
        /// </summary>
        /// <param name="id">Identificador a validar</param>
        /// <param name="texto">Texto a validar</param>
        /// <returns>Objeto Comprador con los datos personales obtenidos de la busqueda</returns>
        public static List<Comprador> Obtiene(int id, string texto)
        {
            //Comprador persona = null;

            /* Solo se permite el acceso a personas que tengan fuentas de correo terminadas en "@anahuac.mx" */
            database db = new database();

            List<Comprador> compradores = new List<Comprador>(5);

            /* Buscamos al colaborador para revisar que este registrado como un colaborador en el sorteo activo actual */
            //            ResultSet dbComprador = db.getTable(String.Format(@"
            //select c.NOMBRE, c.APELLIDOS, c.TELEFONO_F, c.TELEFONO_M, c.CORREO, c.CALLE, c.NUMERO, c.COLONIA, c.ESTADO, c.MUNDEL, c.CP
            //from COMPRADORES c
            //inner join COMPRADORES_BOLETOS cmb on c.PK1 = cmb.PK_COMPRADOR
            //inner join COLABORADORES_BOLETOS clb on clb.PK_BOLETO = cmb.PK_BOLETO
            //where clb.PK_COLABORADOR = {0}
            //and (c.nombre like '%'+ '{1}' +'%' OR  c.APELLIDOS like '%'+ '{1}' +'%')", id, texto));

            ResultSet dbComprador = db.getTable(String.Format(@"SELECT * FROM COMPRADORES WHERE USUARIO = '{0}' AND NOMBRE LIKE '%{1}%' OR APELLIDOS LIKE '%{1}%';", id, texto));

            /* Por cada talonario, agregamos el folio a la lista*/
            while (dbComprador.Next())
            {
                compradores.Add(new Comprador()
                {
                    nombre = dbComprador.Get("NOMBRE"),
                    apellidos = dbComprador.Get("APELLIDOS"),
                    correo = dbComprador.Get("CORREO"),
                    direccion = new Direccion()
                    {
                        calle = dbComprador.Get("CALLE"),
                        numero = dbComprador.Get("NUMERO"),
                        colonia = dbComprador.Get("COLONIA"),
                        estado = dbComprador.Get("ESTADO"),
                        municipio = dbComprador.Get("MUNDEL"),
                        telefono = dbComprador.Get("TELEFONO_F"),
                        codigo_postal = dbComprador.Get("CP")
                    }
                });
            }

            /* Convertimos la lista a arreglo antes de ciclar sobre los talonarios y agregar losboletos*/
            //persona.compradores = compradores.ToArray();

            db.Close();

            return compradores;
        }
    }
}
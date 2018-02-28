using ConnectDB;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SorteoAnahuac.Models
{
    /// <summary>
    /// Clase con los métodos para obtener y generar información relacionada con los sorteos.
    /// </summary>
    public static class SorteoService
    {
        /// <summary>
        /// Función que obtiene los datos de un sorteo por su clave interna
        /// </summary>
        /// <param name="clave">Clave de identificación interna del sorteo</param>
        /// <returns>Estructura con los datos del Sorteo</returns>
        public static Sorteo Obtener(long clave)
        {
            Sorteo sorteo = null;

            /* Abrimos conexión a base de datos */
            database db = new database();

            /* Buscamos el sorteo */
            ResultSet dbSorteo = db.getTable(String.Format(@"
SELECT TOP 1 sorteo.CLAVE, sorteo.SORTEO, sorteo.DESCRIPCION, sorteo.CUENTA_DEPOSITO, sorteo.FECHA_I, sorteo.FECHA_T, sorteo.URL_1, sorteo.URL_2, sorteo.URL_3, sorteo.URL_4, n.LIMITE_VENTA, n.LIMITE_DEPOSITO
FROM SORTEOS sorteo
LEFT JOIN NICHOS n
ON n.PK_SORTEO = sorteo.PK1
WHERE sorteo.PK1 = {0}
ORDER BY n.LIMITE_VENTA DESC", clave));//db.SORTEOS.Where(s => s.PK1 == clave).FirstOrDefault();
            /* En caso de existir ese sorteo, procedemos a traer sus datos */
            if (dbSorteo.Next())
            {
                /* Llenamos los datos que estan en la tabla de SORTEOS */
                sorteo = new Sorteo()
                {
                    clave = clave,
                    identificador = dbSorteo.Get("CLAVE"),
                    nombre = dbSorteo.Get("SORTEO"),
                    descripcion = dbSorteo.Get("DESCRIPCION"),
                    cuenta_bancaria = dbSorteo.Get("CUENTA_DEPOSITO"),
                    fecha_inico = dbSorteo.GetDateTime("FECHA_I"),
                    fecha_fin = dbSorteo.GetDateTime("FECHA_T"),
                    url_conoce = dbSorteo.Get("URL_1"),
                    url_tips = dbSorteo.Get("URL_2"),
                    url_aviso = dbSorteo.Get("URL_3"),
                    url_terminos = dbSorteo.Get("URL_4")
                };

                sorteo.limite_venta = dbSorteo.GetDateTime("LIMITE_VENTA");
                sorteo.limite_abono = dbSorteo.GetDateTime("LIMITE_DEPOSITO");
            }

            // Si encontramos un sorteo válido, obtenemos su listado de ganadores
            if (sorteo != null)
            {
                List<Ganador> ganadores = new List<Ganador>();

                /* Traemos a los ganadores */
                ResultSet dbGanadores = db.getTable(String.Format(@"
SELECT boletos.FOLIO, premios.NUM_PREMIO
FROM GANADORES gana
INNER JOIN PREMIOS
ON gana.PK_PREMIO = PREMIOS.PK1
INNER JOIN COMPRADORES compra
ON compra.PK1 = gana.PK_COMPRADOR
INNER JOIN BOLETOS
ON boletos.PK1 = compra.PK_BOLETO
WHERE gana.PK_SORTEO = {0}
AND premios.CLAVE_BENEFICIARIO = 1
ORDER BY PREMIOS.CLASIFICACION, PREMIOS.NUM_PREMIO", clave));
                while (dbGanadores.Next())
                {
                    ganadores.Add(new Ganador()
                    {
                        folio = dbGanadores.Get("FOLIO"),
                        lugar = dbGanadores.GetInt("NUM_PREMIO")
                    });
                };
                sorteo.ganadores = ganadores.ToArray();
            }

            db.Close();

            return sorteo;
        }
        
        /// <summary>
        /// Función que obtiene los datos del sorteo activo
        /// </summary>
        /// <returns>Estructura con los datos del Sorteo Activo</returns>
        public static Sorteo ObtenerActivo()
        {
            Sorteo sorteo = null;

            /* Abrimos conexión a base de datos */
            database db = new database();

            /* Buscamos el sorteo */
            ResultSet dbSorteo = db.getTable(String.Format(@"
SELECT TOP 1 sorteo.PK1, sorteo.CLAVE, sorteo.SORTEO, sorteo.DESCRIPCION, sorteo.CUENTA, sorteo.FECHA_I, sorteo.FECHA_T, sorteo.URL_1, sorteo.URL_2, sorteo.URL_3, sorteo.URL_4, n.LIMITE_VENTA, n.LIMITE_DEPOSITO
FROM SORTEOS sorteo
LEFT JOIN SECTORES sc
ON sc.PK_SORTEO = sorteo.PK1
LEFT JOIN NICHOS n
ON n.PK_SECTOR = sc.PK1
WHERE sorteo.ACTIVO = 1
ORDER BY n.LIMITE_VENTA DESC"));//db.SORTEOS.Where(s => s.PK1 == clave).FirstOrDefault();
            /* En caso de existir ese sorteo, procedemos a traer sus datos */
            if (dbSorteo.Next())
            {
                /* Llenamos los datos que estan en la tabla de SORTEOS */
                sorteo = new Sorteo()
                {
                    clave = dbSorteo.GetLong("PK1"),
                    identificador = dbSorteo.Get("CLAVE"),
                    nombre = dbSorteo.Get("SORTEO"),
                    descripcion = dbSorteo.Get("DESCRIPCION"),
                    cuenta_bancaria = dbSorteo.Get("CUENTA"),
                    fecha_inico = dbSorteo.GetDateTime("FECHA_I"),
                    fecha_fin = dbSorteo.GetDateTime("FECHA_T"),
                    url_conoce = dbSorteo.Get("URL_1"),
                    url_tips = dbSorteo.Get("URL_2"),
                    url_aviso = dbSorteo.Get("URL_3"),
                    url_terminos = dbSorteo.Get("URL_4")
                };

                sorteo.limite_venta = dbSorteo.GetDateTime("LIMITE_VENTA");
                sorteo.limite_abono = dbSorteo.GetDateTime("LIMITE_DEPOSITO");
            }

            // Si encontramos un sorteo válido, obtenemos su listado de ganadores
//            if (sorteo != null)
//            {
//                List<Ganador> ganadores = new List<Ganador>();

//                /* Traemos a los ganadores */
//                ResultSet dbGanadores = db.getTable(String.Format(@"
//SELECT boletos.FOLIO, premios.NUM_PREMIO
//FROM GANADORES gana
//INNER JOIN PREMIOS
//ON gana.PK_PREMIO = PREMIOS.PK1
//INNER JOIN COMPRADORES compra
//ON compra.PK1 = gana.PK_COMPRADOR
//INNER JOIN BOLETOS
//ON boletos.PK1 = compra.PK_BOLETO
//WHERE gana.PK_SORTEO = {0}
//AND premios.CLAVE_BENEFICIARIO = 1
//ORDER BY PREMIOS.CLASIFICACION, PREMIOS.NUM_PREMIO", sorteo.clave));
//                while (dbGanadores.Next())
//                {
//                    ganadores.Add(new Ganador()
//                    {
//                        folio = dbGanadores.Get("FOLIO"),
//                        lugar = dbGanadores.GetInt("NUM_PREMIO")
//                    });
//                };
//                sorteo.ganadores = ganadores.ToArray();
//            }

            db.Close();

            return sorteo;
        }

        /// <summary>
        /// Función que regresa la lista de estados y municipios disponibles para este Sorteo
        /// </summary>
        /// <returns></returns>
        public static Ubicaciones.Estado[] Estados()
        {
            /* Inicializamos un diccionario de estados */
            Dictionary<string, HashSet<string>> diccEstados = new Dictionary<string, HashSet<string>>();

            /* Abrimos conexión a base de datos para buscar los datos */
            database db = new database();

            ResultSet dbEntidades = db.getTable(String.Format(@"
SELECT DISTINCT d_estado, D_mnpio
FROM SEPOMEX
ORDER BY 1, 2;"));

            /* Generamos la estructura de datos de los estados y municpios */
            while (dbEntidades.Next())
            {
                string estado = dbEntidades.Get("d_estado");
                string municipio = dbEntidades.Get("D_mnpio");
                if (!diccEstados.ContainsKey(estado))
                {
                    diccEstados[estado] = new HashSet<string>();
                }
                if (!diccEstados[estado].Contains(municipio))
                {
                    diccEstados[estado].Add(municipio);
                }
            }


            Ubicaciones.Estado[] estados = new Ubicaciones.Estado[diccEstados.Keys.Count];
            int indiceE = -1;

            /* Convertimos el diccionario a un arreglo */
            foreach (string estado in diccEstados.Keys)
            {
                estados[++indiceE] =new Ubicaciones.Estado()
                {
                    nombre = estado,
                    municipios = diccEstados[estado].Select(m => new Ubicaciones.Municipio()
                    {
                        nombre = m
                    }).ToArray()
                };
            };

            /* Cerramos la conexión de base de datos */
            db.Close();

            /* Regresamos el listado de estados */
            return estados;

        }
    }
}
﻿using ConnectDB;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SorteoAnahuac.Models
{
    /// <summary>
    /// Servicio que permite manipular información de colaboradores del sorteo.
    /// </summary>
    public static class ColaboradorService
    {
        /// <summary>
        /// Función que valida credeciales para regresar un colaborador activo
        /// </summary>
        /// <param name="correo">Correo del usuario que desea autenticarse</param>
        /// <param name="password">Contraseña del usuario a autenticar</param>
        /// <param name="ip_address">Dirección IP del usuario que se esta autenticando</param>
        /// <returns>Verdadero cuando el usuario y contraseña con correctos</returns>
        public static bool Auntenticar(string correo, string password, string ip_address)
        {
            /* Verificamos si la persona existe en la base de datos como colaborador */
            long clave = -1;
            if (correo.ToLowerInvariant().EndsWith("@anahuac.mx"))
            {
                database db = new database();
                /* Buscamos al colaborador para revisar que este registrado como un becario en el sorteo activo actual */
                /* Para saber que es becario, se busca que este registrado en un nicho con clave "0001" */
                ResultSet dbPersona = db.getTable(String.Format(@"
SELECT TOP 1 colab.PK1
FROM COLABORADORES as colab INNER JOIN COLABORADORES_CORREOS as CORREOS
ON colab.PK1 = correos.PK_COLABORADOR
INNER JOIN SORTEOS sorteo
ON colab.PK_SORTEO = sorteo.pk1
WHERE sorteo.ACTIVO = 1
AND correos.correo = '{0}'", correo));
                
                /* Buscamos a la persona */
                if (dbPersona.Next()) { 

                    clave = dbPersona.GetLong("PK1");

                    if (clave > -1)
                    {
                        System.Net.ServicePointManager.DefaultConnectionLimit = 5000;
                        System.Net.WebClient cliente = new System.Net.WebClient();
                        cliente.Credentials = new System.Net.NetworkCredential(correo, password);
                        try
                        {
                            string usuario = cliente.DownloadString("https://outlook.office365.com/api/v1.0/me/");
                        }
                        catch
                        {
                            clave = -1;
                        }
                    }

                    /* Si realizamos un login exitoso, registramos el acceso en la bitacora */
                    if (clave > -1)
                    {
                        Task.Run(() => {
                            db.execute(string.Format(@"
INSERT INTO [COLABORADORES_BITACORA_ACCESO] (
[PK_COLABORADOR],
[IP],
[FECHA_R])
VALUES(
{0},
'{1}',
GETDATE())", clave, ip_address));
                        });
                    }
                }
                
                db.Close();
            }
            
            

            return (clave > -1);

        }

        /// <summary>
        /// Función que revisa que el correo sea el de un colaborador válido, y trae sus datos.
        /// </summary>
        /// <param name="correo">Cuenta de correo a validar</param>
        /// <returns>Objeto Colaborador con los datos personales obtenidos por el correo electrónico</returns>
        public static Colaborador Obtiene(string correo)
        {
            Colaborador persona = null;

            /* Solo se permite el acceso a personas que tengan fuentas de correo terminadas en "@anahuac.mx" */
            database db = new database();

            /* Buscamos al colaborador para revisar que este registrado como un colaborador en el sorteo activo actual */
            ResultSet dbPersona = db.getTable(String.Format(@"
SELECT TOP 1 colab.PK1, colab.CLAVE, colab.NOMBRE, colab.APATERNO, colab.AMATERNO, correos.CORREO, colab.REFBANCARIA, colab.PK_SORTEO
FROM COLABORADORES as colab INNER JOIN COLABORADORES_CORREOS as correos
ON colab.PK1 = correos.PK_COLABORADOR
INNER JOIN SORTEOS sorteo
ON colab.PK_SORTEO = sorteo.pk1
WHERE correos.CORREO = '{0}'
AND correos.CORREO LIKE '%@anahuac.mx'
AND sorteo.ACTIVO = 1", correo));

            long sorteo_colab = -1;

            // Si existe la persona con correo anahuac, entonces extraemos sus datos
            if (dbPersona.Next())
            {
                persona = new Colaborador()
                {
                    clave = dbPersona.GetLong("PK1"),
                    identificador = dbPersona.Get("CLAVE"),
                    nombre = dbPersona.Get("NOMBRE"),
                    apellido_paterno = dbPersona.Get("APATERNO"),
                    apellido_materno = dbPersona.Get("AMATERNO"),
                    correo = dbPersona.Get("CORREO").ToLower(),
                    referencia_bancaria = dbPersona.Get("REFBANCARIA")
                };
                sorteo_colab = dbPersona.GetLong("PK_SORTEO");
            }

            /* Si la persona existe, obtenemos los datos del estado de cuenta */
            if (persona != null)
            {
                ResultSet dbEdoCuenta = db.getTable(String.Format(@"
SELECT TOP 1 IMPORTE, ABONO, SALDO
FROM vESTADO_CUENTA_COLABORADOR edo
WHERE edo.PK_COLABORADOR = {0}
AND edo.PK_SORTEO = {1}", persona.clave, sorteo_colab));

                /* Si encontramos datos, entonces extraemos la información del estado de cuenta */
                if (dbEdoCuenta.Next())
                {
                    persona.monto_total = dbEdoCuenta.GetDecimal("IMPORTE");
                    persona.monto_abonado = dbEdoCuenta.GetDecimal("ABONO");
                    persona.monto_deudor = dbEdoCuenta.GetDecimal("SALDO");
                }
            };

            db.Close();

            return persona;
        }

        /// <summary>
        /// Función que revisa que el correo sea el de un colaborador válido, y trae sus datos incluyendo sus Talonarios.
        /// </summary>
        /// <param name="correo">Cuenta de correo a validar</param>
        /// <returns>Objeto Colaborador con los datos personales obtenidos por el correo electrónico</returns>
        public static Colaborador ObtieneConTalonarios(string correo)
        {
            Colaborador persona = null;

            /* Solo se permite el acceso a personas que tengan fuentas de correo terminadas en "@anahuac.mx" */
            database db = new database();

            /* Buscamos al colaborador para revisar que este registrado como un colaborador en el sorteo activo actual */
            ResultSet dbPersona = db.getTable(String.Format(@"
SELECT TOP 1 colab.PK1, colab.CLAVE, colab.NOMBRE, colab.APATERNO, colab.AMATERNO, colab.REFBANCARIA, colab.PK_SORTEO
FROM COLABORADORES as colab INNER JOIN COLABORADORES_CORREOS as correos
ON colab.PK1 = correos.PK_COLABORADOR
INNER JOIN SORTEOS sorteo
ON colab.PK_SORTEO = sorteo.pk1
WHERE correos.CORREO = '{0}'
AND correos.CORREO LIKE '%@anahuac.mx'
AND sorteo.ACTIVO = 1", correo));

            long sorteo_colab = -1;
            bool existePersona = false;

            // Si existe la persona con correo anahuac, entonces extraemos sus datos
            if (dbPersona.Next())
            {
                // Obtenemos los datos del nombre de la persona
                string nombre = dbPersona.Get("NOMBRE"),
                    apellido_paterno = dbPersona.Get("APATERNO"),
                    apellido_materno = dbPersona.Get("AMATERNO");
                // Generamos el nombre completo
                string nombre_completo = string.Format("{0} {1} {2}", nombre, apellido_paterno, apellido_materno).Trim();

                persona = new Colaborador(false, nombre_completo)
                {
                    clave = dbPersona.GetLong("PK1"),
                    identificador = dbPersona.Get("CLAVE"),
                    nombre = nombre,
                    apellido_paterno = apellido_paterno,
                    apellido_materno = apellido_materno,
                    correo = correo.ToLower(),
                    referencia_bancaria = dbPersona.Get("REFBANCARIA")
                };
                sorteo_colab = dbPersona.GetLong("PK_SORTEO");
                existePersona = true;
            }

            /* Si la persona existe, obtenemos los datos del estado de cuenta */
            if (existePersona)
            {
                ResultSet dbEdoCuenta = db.getTable(String.Format(@"
SELECT TOP 1 IMPORTE, ABONO, SALDO
FROM vESTADO_CUENTA_COLABORADOR edo
WHERE edo.PK_COLABORADOR = {0}
AND edo.PK_SORTEO = {1}", persona.clave, sorteo_colab));

                /* Si encontramos datos, entonces extraemos la información del estado de cuenta */
                if (dbEdoCuenta.Next())
                {
                    persona.monto_total = dbEdoCuenta.GetDecimal("IMPORTE");
                    persona.monto_abonado = dbEdoCuenta.GetDecimal("ABONO");
                    persona.monto_deudor = dbEdoCuenta.GetDecimal("SALDO");
                }

                #region Talonarios y Boletos
                List<Talonario> talonarios = new List<Talonario>(5);

                /* Traemos los folios de los talonarios digitales asignados a un colaborador en el sorteo activo */
                ResultSet dbTalonarios = db.getTable(String.Format(@"
SELECT tal.FOLIO, tal.PK1
FROM TALONARIOS tal
INNER JOIN SORTEOS_COLABORADORES_TALONARIOS ctal
ON tal.PK1 = ctal.PK_TALONARIO
WHERE tal.DIGITAL = 1
AND tal.ASIGNADO = 1
AND ctal.PK_SORTEO = {1}
AND ctal.PK_COLABORADOR = {0}
ORDER BY tal.FOLIO", persona.clave, sorteo_colab));

                /* Por cada talonario, agregamos el folio a la lista*/
                while (dbTalonarios.Next())
                {
                    talonarios.Add(new Talonario() {
                        clave = dbTalonarios.GetLong("PK1"),
                        folio = dbTalonarios.Get("FOLIO")
                    });
                }

                /* Convertimos la lista a arreglo antes de ciclar sobre los talonarios y agregar losboletos*/
                persona.talonarios = talonarios.ToArray();

                /* Por cada folio obtenido, traemos los datos del talonario*/
                foreach (Talonario talonario in persona.talonarios)
                {
                    /* Traemos los datos del talonario */
                    ResultSet dbBoleto = db.getTable(String.Format(@"
SELECT
	boletos.PK1,
	boletos.FOLIO,
	boletos.FOLIODIGITAL
FROM SORTEOS_COLABORADORES_BOLETOS rel_boletos
INNER JOIN boletos
ON boletos.PK1 = rel_boletos.PK_BOLETO
WHERE rel_boletos.PK_SORTEO = {2}
AND rel_boletos.PK_COLABORADOR = {0}
AND rel_boletos.PK_TALONARIO = {1}", persona.clave, talonario.clave, sorteo_colab));


                    List<Boleto> boletos = new List<Boleto>(20);
                    while (dbBoleto.Next())
                    {
                        Boleto boleto = new Boleto(false)
                        {
                            clave = dbBoleto.GetLong("PK1"),
                            folio = dbBoleto.Get("FOLIO"),
                            folio_digital = dbBoleto.Get("FOLIODIGITAL")
                        };
                        if (boleto.folio_digital == "0")
                        {
                            boleto.folio_digital = null;
                        }
                        boleto.vendido = !String.IsNullOrEmpty(boleto.folio_digital);
                        boletos.Add(boleto);
                    }

                    talonario.boletos = boletos.ToArray();
                };

                #endregion
            };

            db.Close();

            persona.version = "1.0.2";

            return persona;
        }

        /// <summary>
        /// Función que revisa trae un colaborador por su clave.
        /// </summary>
        /// <param name="clave">Clave del colaborador que se desea revisar</param>
        /// <returns>Objeto Colaborador con los datos personales </returns>
        public static Colaborador ObtienePorClave(long clave)
        {
            Colaborador persona = null;

            /* Solo se permite el acceso a personas que tengan fuentas de correo terminadas en "@anahuac.mx" */
            database db = new database();

            /* Buscamos al colaborador para revisar que este registrado como un colaborador en el sorteo activo actual */
            ResultSet dbPersona = db.getTable(String.Format(@"
SELECT TOP 1 colab.CLAVE, colab.NOMBRE, colab.APATERNO, colab.AMATERNO, correos.CORREO, colab.REFBANCARIA, colab.PK_SORTEO
FROM COLABORADORES as colab INNER JOIN COLABORADORES_CORREOS as correos
ON colab.PK1 = correos.PK_COLABORADOR
WHERE colab.PK1 = {0}
AND correos.CORREO LIKE '%@anahuac.mx'", clave));

            long sorteo_colab = -1;

            // Si existe la persona con correo anahuac, entonces extraemos sus datos
            if (dbPersona.Next())
            {
                persona = new Colaborador()
                {
                    clave = clave,
                    identificador = dbPersona.Get("CLAVE"),
                    nombre = dbPersona.Get("NOMBRE"),
                    apellido_paterno = dbPersona.Get("APATERNO"),
                    apellido_materno = dbPersona.Get("AMATERNO"),
                    correo = dbPersona.Get("CORREO").ToLower(),
                    referencia_bancaria = dbPersona.Get("REFBANCARIA")
                };
                sorteo_colab = dbPersona.GetLong("PK_SORTEO");
            }

            /* Si la persona existe, obtenemos los datos del estado de cuenta */
            if (persona != null)
            {
                ResultSet dbEdoCuenta = db.getTable(String.Format(@"
SELECT TOP 1 IMPORTE, ABONO, SALDO
FROM vESTADO_CUENTA_COLABORADOR edo
WHERE edo.PK_COLABORADOR = {0}
AND edo.PK_SORTEO = {1}", clave, sorteo_colab));

                /* Si encontramos datos, entonces extraemos la información del estado de cuenta */
                if (dbEdoCuenta.Next())
                {
                    persona.monto_total = dbEdoCuenta.GetDecimal("IMPORTE");
                    persona.monto_abonado = dbEdoCuenta.GetDecimal("ABONO");
                    persona.monto_deudor = dbEdoCuenta.GetDecimal("SALDO");
                }
            };

            return persona;
        }
    }
}
using ConnectDB;
using Newtonsoft.Json;
using QRCoder;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Drawing;
using System.IO;
using System.Linq;

namespace SorteoAnahuac.Models
{
    /// <summary>
    /// Servicio que permite la obtención de datos y manipulación de boletos y talonarios.
    /// </summary>
    public static class TalonarioService
    {
        /// <summary>
        /// Función que obitene un talonaro en base a su folio
        /// </summary>
        /// <param name="folio">Folio del talonario</param>
        /// <param name="clave_persona">PK de la persona dueña del talonario</param>
        /// <returns>Talonario con sus boletos</returns>
        public static Talonario Obtiene(string folio, long clave_persona)
        {
            Talonario talonario = null;

            /* Traemos los datos del sorteo activo */
            Sorteo sorteo_activo = SorteoService.ObtenerActivo();

            /* Abrimos conexión a la base de datos */
            database db = new database();

            /* Traemos los folios de los talonarios digitales asignados a un colaborador en el sorteo activo */
            ResultSet dbTalonario = db.getTable(String.Format(@"
SELECT TOP 1 tal.PK1
FROM TALONARIOS tal
INNER JOIN SORTEOS_COLABORADORES_TALONARIOS ctal
ON tal.PK1 = ctal.PK_TALONARIO
WHERE tal.DIGITAL = 1
AND ctal.PK_SORTEO = {0}
AND ctal.PK_COLABORADOR = {1}
AND tal.FOLIO = '{2}'
ORDER BY tal.FOLIO", sorteo_activo.clave, clave_persona, folio));
            
            /* Si el talonario existe, traemos sus datos */
            if (dbTalonario.Next())
            {
                /* Traemos los datos del talonario */
                talonario = new Talonario()
                {
                    clave = dbTalonario.GetLong("PK1"),
                    folio = folio
                };
            }

            /* Si el talonario existe, traemos sus boletos */
            if (talonario != null)
            {
                ResultSet dbBoleto = db.getTable(String.Format(@"
SELECT
	boletos.PK1,
	boletos.FOLIO,
	boletos.FOLIODIGITAL
FROM SORTEOS_COLABORADORES_BOLETOS rel_boletos
INNER JOIN boletos
ON boletos.PK1 = rel_boletos.PK_BOLETO
WHERE rel_boletos.PK_SORTEO = {0}
AND rel_boletos.PK_COLABORADOR = {1}
AND rel_boletos.PK_TALONARIO = {2}", sorteo_activo.clave, clave_persona, talonario.clave));


                List<Boleto> boletos = new List<Boleto>();
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
            }

            db.Close();

            return talonario;
        }

        /// <summary>
        /// Función que obtiene los talonarios de un usuario
        /// </summary>
        /// <param name="usuario"></param>
        /// <returns></returns>
        public static Talonario[] ObtienePorUsuario(string usuario)
        {
            List<string> folios = new List<string>();
            List<Talonario> talonarios = new List<Talonario>();

            /* Traemos los datos del usuario del cual se quiere obtener los talonarios */
            Colaborador persona = ColaboradorService.Obtiene(usuario);

            /* Traemos los datos del sorteo activo */
            Sorteo sorteo_activo = SorteoService.ObtenerActivo();

            /* Si la persona existe y tenemos un sorteo activo, abrimos la conexión a la base de datos para obtener sus talonarios*/
            if (persona != null && sorteo_activo != null)
            {
                database db = new database();

                /* Traemos los folios de los talonarios digitales asignados a un colaborador en el sorteo activo */
                ResultSet dbTalonarios = db.getTable(String.Format(@"
SELECT tal.FOLIO
FROM TALONARIOS tal
INNER JOIN SORTEOS_COLABORADORES_TALONARIOS ctal
ON tal.PK1 = ctal.PK_TALONARIO
WHERE tal.DIGITAL = 1
AND ctal.PK_SORTEO = {0}
AND ctal.PK_COLABORADOR = {1}
ORDER BY tal.FOLIO", sorteo_activo.clave, persona.clave));

                /* Por cada talonario, agregamos el folio a la lista*/
                while (dbTalonarios.Next())
                {
                    folios.Add(dbTalonarios.Get("FOLIO"));
                }

                db.Close();

                /* Por cada folio obtenido, traemos los datos del talonario*/
                foreach (string folio in folios)
                {
                    /* Traemos los datos del talonario */
                    talonarios.Add(Obtiene(folio, persona.clave));
                };

            }

            return talonarios.ToArray();
        }

        /// <summary>
        /// Función que permite marcar como aceptado un talonario por su folio
        /// </summary>
        /// <param name="folio">Folio público del talonario a aceptar</param>
        /// <param name="clave_persona">Clave de la persona dueña del talonario</param>
        public static void AceptarTalonario(string folio, long clave_persona)
        {
            /* Abrimos la conexión de base de datos */
            database db = new database();

            /* Actualizamos los registros de colaborador_talonario del colaborador, donde el talonario sea del sorteo activo, del folio ingresado y sea digital */
            db.execute(string.Format(@"
UPDATE [SORTEOS_COLABORADORES_TALONARIOS] 
SET FECHA_M = GETDATE(),
USUARIO = {0}
WHERE PK_SORTEO IN (SELECT PK1 FROM SORTEOS WHERE ACTIVO = 1)
AND PK_COLABORADOR = {0}
AND PK_TALONARIO IN (SELECT PK1 FROM TALONARIOS WHERE FOLIO = {1} AND DIGITAL = 1)", clave_persona, folio));

            /* Cerramos la conexión de base de datos */
            db.Close();
        }

        /// <summary>
        /// Función que permite marcar un boleto como vendido
        /// </summary>
        /// <param name="boleto">Boleto que va a ser marcado como vendido</param>
        /// <param name="clave_persona">Clave de la persona dueña del talonario</param>
        /// <returns>1 si el boleto es vendido. 2 si el boleto ya había sido vendido. 0 si el boleto no existe.</returns>
        public static int VenderBoleto(Boleto boleto, long clave_persona)
        {
            /* Traemos los datos del sorteo activo */
            //Sorteo sorteo_activo = SorteoService.ObtenerActivo();

            bool vendido = false;
            int resultado = 0;
            bool boletoExiste = false;
            long clave_boleto = -1;
            string clave_talonario = string.Empty, folio_talonario = string.Empty, folio_boleto = string.Empty;
            bool esDigital = false, tieneComprador = false;
            
            /* Abrimos la conexión de base de datos */
            database db = new database();

            /* Buscamos el boleto por su folio, validando que el boleto pertenezca al colaborador que solicita la venta y que no tenga folio digital */
            ResultSet dbBoleto = db.getTable(string.Format(@"
SELECT TOP 1 bol.PK1,
bol.PK_TALONARIO,
bol.TALONARIO,
bol.FOLIO,
ISNULL(bol.FOLIODIGITAL,-1) digital,
ISNULL(com.PK1,-1) comprador
FROM BOLETOS bol
LEFT JOIN COMPRADORES com
ON com.PK_BOLETO = bol.PK1
WHERE bol.FOLIO = '{0}'
AND EXISTS (
	SELECT 'X'
	FROM SORTEOS_COLABORADORES_BOLETOS scb
	INNER JOIN SORTEOS
	ON scb.PK_SORTEO = SORTEOS.PK1
	WHERE SORTEOS.ACTIVO = 1
	AND scb.PK_COLABORADOR = {1}
	AND scb.PK_BOLETO = bol.PK1
)", boleto.folio, clave_persona));

            /* Validamos que el boleto exista y traemos sus datos*/
            if (dbBoleto.Next())
            {
                boletoExiste = true;
                clave_boleto = dbBoleto.GetLong("PK1");
                clave_talonario = dbBoleto.Get("PK_TALONARIO");
                folio_talonario = dbBoleto.Get("TALONARIO");
                folio_boleto = dbBoleto.Get("FOLIO");
                esDigital = dbBoleto.Get("digital") != "-1";
                tieneComprador = dbBoleto.Get("comprador") != "-1";
            }

            /* Si el boleto existe, procedemos a hacer las validaciones para su venta */
            if (boletoExiste)
            {
                if (!esDigital)
                {

                    /* Actualizamos el folio digital del boleto para marcarlo como vendido*/
                    db.execute(string.Format(@"
UPDATE boletos
SET FOLIODIGITAL = (SELECT COUNT('x') + 1 FROM boletos WHERE NOT foliodigital IS NULL)
WHERE PK1 = {0}", clave_boleto));

                    /* Si el boleto no tiene comprador, le agregamos los datos del comprador actual */
                    if (!tieneComprador)
                    {
                        /* Traemos la clave del sorteo activo */
                        string clave_sorteo = "NULL";
                        ResultSet dbSorteo = db.getTable("SELECT TOP 1 PK1 FROM SORTEOS WHERE ACTIVO = 1");
                        if (dbSorteo.Next())
                        {
                            clave_sorteo = dbSorteo.Get("PK1");
                        }

                        /* Traemos el nicho y sector */
                        string clave_nicho = "NULL", clave_sector = "NULL";
                        ResultSet dbColaborador = db.getTable(string.Format(@"
SELECT TOP 1 ISNULL(CAST(PK_NICHO as NVARCHAR(14)),'NULL') as PK_NICHO, ISNULL(CAST(PK_SECTOR as NVARCHAR(14)),'NULL') as PK_SECTOR
FROM SORTEOS_ASIGNACION_COLABORADORES
WHERE PK_COLABORADOR = {0}
AND PK_SORTEO = {1}", clave_persona, clave_sorteo));
                        if (dbColaborador.Next())
                        {
                            clave_nicho = dbColaborador.Get("PK_NICHO");
                            clave_sector = dbColaborador.Get("PK_SECTOR");
                        }

                        /* Insertamos al comprador */
                        db.execute(string.Format(@"
INSERT INTO [COMPRADORES]
           ([PK_SORTEO],[PK_TALONARIO],[TALONARIO],[PK_BOLETO],[BOLETO]
           ,[PK_SECTOR],[PK_NICHO],[PK_COLABORADOR],[NOMBRE],[APELLIDOS],[TELEFONOF]
           ,[TELEFONOM],[CORREO],[CALLE],[NUMERO],[COLONIA],[ESTADO],[MUNDEL]
           ,[USUARIO],[FECHA_R],[CP])
     VALUES
           ({0},{1},'{2}',{3},'{4}',{5},{6},{7},'{8}','{9}','{10}','{11}','{12}','{13}','{14}','{15}','{16}','{17}','{18}',GETDATE(),'{19}')",
           clave_sorteo, clave_talonario, folio_talonario, clave_boleto, folio_boleto, clave_sector, clave_nicho, clave_persona,
           boleto.comprador.nombre, boleto.comprador.apellidos, boleto.comprador.direccion.telefono, boleto.comprador.celular,
           boleto.comprador.correo, boleto.comprador.direccion.calle, boleto.comprador.direccion.numero, boleto.comprador.direccion.colonia,
           boleto.comprador.direccion.estado, boleto.comprador.direccion.municipio, clave_persona.ToString(), boleto.comprador.direccion.codigo_postal));

                    }
                    boleto.clave = clave_boleto;
                    vendido = true;
                    resultado = 1;

                } else
                {
                    /* En caso de que si exista con folio digital, cambiamos la salida para reflejar esta condición */
                    resultado = 2;
                }

            }

            /* Cerramos la base de datos */
            db.Close();

            if (vendido)
            {
                EnviarBoleto(boleto.clave, clave_persona);
            }

            return resultado;
        }

        /// <summary>
        /// Función que permite obtener los datos de un boleto
        /// </summary>
        /// <param name="clave">Clave del boleto que se desea consutar</param>
        /// <param name="clave_persona">Clave de la persona dueña del talonario</param>
        public static Boleto ObtieneBoleto(long clave, long clave_persona)
        {
            Boleto boleto = null;

            /* Traemos los datos del sorteo activo */
            Sorteo sorteo_activo = SorteoService.ObtenerActivo();

            /* Abrimos la conexión de base de datos */
            database db = new database();

            /* Buscamos el boleto por su clave */
            ResultSet dbBoleto = db.getTable(String.Format(@"
SELECT TOP 1 bol.PK1, bol.FOLIO, CAST(bol.FOLIODIGITAL as NVARCHAR(16)) FOLIODIGITAL, tal.FOLIO AS TAL_FOLIO, scb.PK_COLABORADOR, scb.PK_SORTEO, ISNULL(comp.PK1,-1) tiene_comprador, comp.NOMBRE, comp.APELLIDOS, comp.TELEFONOM, comp.CORREO, comp.CALLE, comp.NUMERO, comp.COLONIA, comp.ESTADO, comp.MUNDEL, comp.TELEFONOF
FROM boletos bol
INNER JOIN TALONARIOS tal
ON tal.PK1 = bol.PK_TALONARIO
LEFT JOIN SORTEOS_COLABORADORES_BOLETOS scb
ON scb.PK_BOLETO = bol.PK1
LEFT JOIN COMPRADORES comp
ON comp.PK_BOLETO = bol.PK1
WHERE bol.PK1 = {0}", clave));

            /* Si el boleto existe, procedemos a extraer sus datos */
            if (dbBoleto.Next())
            {
                boleto = new Boleto()
                {
                    clave = dbBoleto.GetLong("PK1"),
                    folio = dbBoleto.Get("FOLIO"),
                    folio_digital = dbBoleto.Get("FOLIODIGITAL"),
                    vendido = true,
                    folio_talonario = dbBoleto.Get("TAL_FOLIO"),
                    clave_colaborador = dbBoleto.GetLong("PK_COLABORADOR"),
                    clave_sorteo = dbBoleto.GetLong("PK_SORTEO")
                };

                /* Si el folio digital es un string vacío, significa que debe ser NULL y no ha sido vendido */
                if (string.IsNullOrEmpty(boleto.folio_digital))
                {
                    boleto.folio_digital = null;
                    boleto.vendido = false;
                }

                /* Revisamos si tiene un comprador */
                if (dbBoleto.GetInt("tiene_comprador") > -1)
                {
                    /* Si tiene comprador agregamos los datos al objeto de salida */
                    boleto.comprador = new Comprador()
                    {
                        nombre = dbBoleto.Get("NOMBRE"),
                        apellidos = dbBoleto.Get("APELLIDOS"),
                        celular = dbBoleto.Get("TELEFONOM"),
                        correo = dbBoleto.Get("CORREO"),
                        direccion = new Direccion()
                        {
                            calle = dbBoleto.Get("CALLE"),
                            numero = dbBoleto.Get("NUMERO"),
                            colonia = dbBoleto.Get("COLONIA"),
                            estado = dbBoleto.Get("ESTADO"),
                            municipio = dbBoleto.Get("MUNDEL"),
                            telefono = dbBoleto.Get("TELEFONOF")
                        }
                    };
                }

            }


            /* Cerramos la conexión a base de datos */
            db.Close();

            return boleto;
        }

        /// <summary>
        /// Función que genera un código QR en formato de MemoryStream
        /// </summary>
        /// <param name="text">Texto a convertir en QR</param>
        /// <returns>Memorystream con los bytes del QR en formato Jpeg</returns>
        public static MemoryStream GenerateQRCode(string text)
        {
            QRCodeGenerator qrGenerator = new QRCodeGenerator();
            QRCodeData qrCodeData = qrGenerator.CreateQrCode(text, QRCodeGenerator.ECCLevel.Q);
            QRCode qrCode = new QRCode(qrCodeData);
            Bitmap TempBMP = qrCode.GetGraphic(3);

            MemoryStream ms = new MemoryStream();
            TempBMP.Save(ms, System.Drawing.Imaging.ImageFormat.Jpeg);
            ms.Position = 0;
            return ms;
        }

        /// <summary>
        /// Función que permite enviar un boleto por correo
        /// </summary>
        /// <param name="clave">Clave del boleto al que se le va enviar el correo</param>
        /// <param name="clave_persona">Clave de la persona dueña del talonario</param>
        public static void EnviarBoleto(long clave, long clave_persona)
        {
            /* Traemos los datos del sorteo activo */
            Sorteo sorteo_activo = SorteoService.ObtenerActivo();

            Boleto boleto = ObtieneBoleto(clave, clave_persona);

            Colaborador colaborador = ColaboradorService.ObtienePorClave(clave_persona);

            /* Si el talonario existe, traemos sus datos */
            if (boleto != null && boleto.comprador != null)
            {
                string urlBoleto = string.Format("{0}/boleto/{1}", ConfigurationManager.AppSettings["App.Url.Base"], boleto.token);
                MemoryStream qrImage = GenerateQRCode(urlBoleto);

                String nombreArchivo = String.Format("boleto-{0}", boleto.folio);
                System.Net.Mail.Attachment qrAttachment = new System.Net.Mail.Attachment(qrImage, nombreArchivo, System.Net.Mime.MediaTypeNames.Image.Jpeg)
                {
                    ContentId = String.Format("{0}@anahuac.mx",nombreArchivo),
                    ContentType = new System.Net.Mime.ContentType(System.Net.Mime.MediaTypeNames.Image.Jpeg),
                    Name = String.Format("{0}.jpg", nombreArchivo)
                };
                qrAttachment.ContentDisposition.Inline = true;

                System.Drawing.Image frenteBoleto = Bitmap.FromFile(System.Web.Hosting.HostingEnvironment.MapPath("~/Content/Imagenes/Boleto_final_Sorteo_2017.png"));
                Graphics g = Graphics.FromImage(frenteBoleto);
                g.DrawString(boleto.folio, System.Drawing.SystemFonts.DefaultFont, System.Drawing.Brushes.Black, new RectangleF(832, 13, 85, 29));

                MemoryStream frenteImage = new MemoryStream();
                frenteBoleto.Save(frenteImage, System.Drawing.Imaging.ImageFormat.Jpeg);
                frenteImage.Position = 0;

                System.Net.Mail.Attachment boletoAttach = new System.Net.Mail.Attachment(frenteImage, String.Format("Boleto_final_Sorteo_2017_{0}.jpg", boleto.folio), System.Net.Mime.MediaTypeNames.Image.Jpeg)
                {
                    ContentId = "boleto-frente@anahuac.mx"
                };
                boletoAttach.ContentDisposition.Inline = true;

                System.Net.Mail.Attachment reversoAttach = new System.Net.Mail.Attachment(System.Web.Hosting.HostingEnvironment.MapPath("~/Content/Imagenes/Boleto_final_Sorteo.png"))
                {
                    ContentId = "boleto-reverso@anahuac.mx"
                };
                reversoAttach.ContentDisposition.Inline = true;

                System.Net.Mail.Attachment logoAttach = new System.Net.Mail.Attachment(System.Web.Hosting.HostingEnvironment.MapPath("~/Content/Imagenes/AHC_Logo-Correo.png"))
                {
                    ContentId = "sorteo-logo@anahuac.mx"
                };
                logoAttach.ContentDisposition.Inline = true;

                

                Code.CorreoUtil.Enviar(
                    new string[] { boleto.comprador.correo },
                    String.Format("Sorteo Anáhuac: Boleto {0}", boleto.folio),
                    String.Format(@"<html>
<head>
</head>
<body>
    <style type=""text/css"">
        #boleto-logo img {{
            width: 120px;
        }}

        #contenido-correo {{
            font-family: Tahoma, Verdana, Arial, sans-serif;
            text-align: center;
        }}

        #contenido-correo h3 {{
            color: #ea7200;
            font-style: italic;
            font-size: 1.1em;
        }}

        #contenido-correo p.boleto-descripcion {{
            color: #666;
        }}

        .boletos-conteo {{
            color: #ea7200;
        }}
    
        #boleto-detalle-contenido {{
            margin-top: 1em;
            text-align: center;
        }}

        .boleto-contacto, .boleto-url {{
            color: #666;
            font-size: 0.8em;
        }}

        .boleto-contacto span {{
            color: #000;
            font-size: 1.3em;
            font-weight: bold;
        }}

        #qrURL-boleto, #reverso-boleto {{
            margin-bottom: 1.2em;
        }}

        #frente-boleto img, #reverso-boleto img {{
            width: 75%;
        }}

    </style>
    <div id=""contenido-correo"">
    <p>
        <div id=""boleto-logo""><img src=""cid:{16}"" /></div>
    </p>
    <h2 class=""talonario-boletos-header con-flecha-llamada"">Boleto <span class=""boletos-conteo"">{0}</span></h2>
    <h3>¡Felicidades!</h3>
    <p class=""boleto-descripcion"">Has comprado el <b>boleto {0}</b> del Sorteo Anáhuac.<br/>A continuación te presentamos los datos de tu compra:</p>
    <div id=""boleto-detalle-contenido"">
    <p class=""boleto-contacto"">
        Dueño<br/>
        <span>{1} {2}</span>
    </p>
    <p class=""boleto-contacto"">
        Domicilio:<br/>
        <span>{6}</span> <span>{7}</span><br/>
        <span>{8}</span>, <span>{9}</span>
    </p>
    <p class=""boleto-contacto"">
        Teléfono fijo:<br/>
        <span>{5}</span>
    </p>
    <p class=""boleto-contacto"">
        Teléfono móvil:<br/>
        <span>{4}</span>
    </p>
    <p class=""boleto-contacto"">
        Correo electrónico:<br/>
        <span>{3}</span>
    </p>
    <p class=""boleto-contacto"">
        Cuenta bancaria:<br/>
        <span>{12}</span>
    </p>
    <p class=""boleto-contacto"">
        Referencia bancaria:<br/>
        <span>{13}</span>
    </p>
    <p>
        <div id=""qrURL-boleto""><img src=""cid:{11}"" /></div>
    </p>
    <p class=""boleto-contacto"">
        {17}
    </p>
    <p>
        <div id=""frente-boleto""><img src=""cid:{14}"" /></div>
        <div id=""reverso-boleto""><img src=""cid:{15}"" /></div>
    </p>
    <p class=""boleto-contacto"">
    Para más información, puedes revisar nuestro sitio web en<br/><a href=""http://www.sorteoanahuac.mx/"" target=""_blank"">http://www.sorteoanahuac.mx/</a>.
    </p>
    </div>
    </div>
</body>
<html>",
            boleto.folio,
            boleto.comprador.nombre,
            boleto.comprador.apellidos,
            boleto.comprador.correo,
            boleto.comprador.celular,
            boleto.comprador.direccion.telefono,
            boleto.comprador.direccion.calle,
            boleto.comprador.direccion.numero,
            boleto.comprador.direccion.municipio,
            boleto.comprador.direccion.estado,
            boleto.token,
            qrAttachment.ContentId,
            sorteo_activo.cuenta_bancaria,
            colaborador.referencia_bancaria,
            boletoAttach.ContentId,
            reversoAttach.ContentId,
            logoAttach.ContentId,
            urlBoleto),
                    new System.Net.Mail.Attachment[] { qrAttachment, boletoAttach, reversoAttach, logoAttach });
                return;
            }
        }

        /// <summary>
        /// Función que permite enviar un correo de solicitud de talonario al encargado de sorteos
        /// </summary>
        /// <param name="usuario"></param>
        public static void SolicitarTalonario(string usuario)
        {
            Colaborador persona = ColaboradorService.Obtiene(usuario);
            if (persona != null)
            {
                Code.CorreoUtil.Enviar(
                    new string[] { ConfigurationManager.AppSettings["Correo.SolicitudTalonario"] },
                    "Solicitud de Talonario",
                    String.Format("<html><head></head><body><style>body {{ font-family: Arial, Helvetica, Verdana, Tahoma, Sans-Serif; }}</style><p>Solicitud de Talonario:</p><ul><li>Id de Estudiante: <b>{1}</b></li><li>Nombre: <b>{0}</b></li><li>Talonarios: <b>1</b></li></ul></body></html>", persona.nombre_completo, persona.identificador)
                    );
            }
        }

        /// <summary>
        /// Función que permite verificar si el token de un boleto es válido
        /// </summary>
        /// <param name="token">Valor del token del boleto a verificar</param>
        /// <returns>Verdadero cuando el token corresponde a un boleto válido</returns>
        public static Boleto VerificarBoleto(string token)
        {
            Boleto valido = null;

            try
            {
                Code.TokenUtil util = new Code.TokenUtil();

                string valorToken = util.Desencripta(token);

                BoletoToken objetoToken = JsonConvert.DeserializeObject<BoletoToken>(valorToken);

                Boleto boleto = ObtieneBoleto(long.Parse(objetoToken.clave), -1);

                if (boleto != null)
                {
                    valido = boleto;
                }
            } catch
            {

            }

            return valido;

        }

        class BoletoToken
        {
            public string clave { get; set; }
            public string folio { get; set; }
        }
    }
}
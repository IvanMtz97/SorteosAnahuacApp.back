using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Cors;

namespace SorteoAnahuac.Controllers
{
    /// <summary>
    /// Controlador con métodos para obtener ubicaciones geográficas
    /// </summary>
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    public class UbicacionesController : ApiController
    {
        /// <summary>
        /// Acción que regresa el listado de estados y municipios disponibles en Sorteos Anáhuac
        /// </summary>
        /// <returns></returns>
        // GET api/<controller>
        [HttpGet]
        public async Task<Models.Ubicaciones.Estado[]> Get()
        {
            return await Task.Run(() =>
            { 
                Models.Ubicaciones.Estado[] estados = null;

                /* Leemos el archivo estados.json con el contenido de los estados */
                string contenido;
                using (StreamReader streamReader = new StreamReader(System.Web.Hosting.HostingEnvironment.MapPath("~/estados.json"), Encoding.UTF8))
                {
                    contenido = streamReader.ReadToEnd();
                }

                /* Si el archivo de json tiene contenido, lo deserializamos */
                if (!String.IsNullOrEmpty(contenido))
                {
                    estados = Jil.JSON.Deserialize<Models.Ubicaciones.Estado[]>(contenido);
                } else
                {
                    /* Cuando no existe un archivo, se calcula al vuelo */
                    estados = Models.SorteoService.Estados();
                }

                return estados;
            });
        }

        /// <summary>
        /// Acción que genera el archivo de listado de ubicaciones geográficas para ser usado por el servicio de consulta
        /// </summary>
        /// <returns>Booleando indicando que se generó el archivo</returns>
        [HttpPost]
        public async Task<bool> Post()
        {
            return await Task.Run(() =>
            {
                bool generado = false;

                Models.Ubicaciones.Estado[] arreglo = Models.SorteoService.Estados();

                /* Serializamos las ubicaciones */
                string jsonEstados = string.Empty;
                using (StringWriter output = new StringWriter())
                {
                    Jil.JSON.Serialize<Models.Ubicaciones.Estado[]>(arreglo, output);
                    jsonEstados = output.ToString();
                }

                /* Guardamos las ubicaciones en un archivo */
                using (StreamWriter _escritor = new StreamWriter(System.Web.Hosting.HostingEnvironment.MapPath("~/estados.json"), false, Encoding.UTF8))
                {
                    _escritor.WriteLine(jsonEstados);
                }

                generado = true;

                return generado;
            });
        }

    }
}
using Azure;
using Azure.Data.Tables;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using Microsoft.Win32;
using Newtonsoft.Json;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Formacion.Azure.Functions.FunctionApp3
{
    public class Function3
    {
        private readonly ILogger<Function3> _logger;

        public Function3(ILogger<Function3> logger)
        {
            _logger = logger;
        }

        [Function("Function3")]
        [TableOutput("operaciones", Connection = "AzureWebJobsStorage")]   // esto es con bindings
        public ITableEntity Run([HttpTrigger(AuthorizationLevel.Function, "get", "post")] HttpRequest req)
        {
            try
            {
                _logger.LogInformation("Function3 C# HTTP trigger -> iniciando el proceso de la petici�n.");

                string nombre = req.Query["nombre"];

                string requestBody = new StreamReader(req.Body).ReadToEndAsync().Result;
                dynamic data = JsonConvert.DeserializeObject(requestBody);
                nombre = nombre ?? data?.nombre;

                string mensaje = string.IsNullOrEmpty(nombre)
                    ? "Esta funci�n activada por HTTP se ejecut� correctamente. Pase un nombre en la cadena de consulta o en el cuerpo de la solicitud para obtener una respuesta personalizada."
                    : $"Hola, {nombre}. Esta funci�n activada por HTTP se ejecut� con �xito.";

                _logger.LogInformation("Function3 C# HTTP trigger -> proceso finalizado.");

                // a diferencia del function2, a partir de aqu� se simplifica bastante el c�digo

                req.HttpContext.Response.Headers.ContentType = "text/plain";
                req.HttpContext.Response.StatusCode = 200;
                req.HttpContext.Response.WriteAsync(mensaje);

                    var registro = new Register()
                    {
                        PartitionKey = "https",
                        RowKey = Guid.NewGuid().ToString(),
                        Nombre = nombre,
                        Mensaje = mensaje
                    };

                return registro;

            }
            catch (Exception e)
            {
                req.HttpContext.Response.Headers.ContentType = "text/plain; charset=UTF-8";
                req.HttpContext.Response.StatusCode = 400;
                req.HttpContext.Response.WriteAsync(e.Message);

                var registro = new Register()
                {
                    PartitionKey = "https",
                    RowKey = Guid.NewGuid().ToString(),
                    Nombre = "",
                    Mensaje = e.Message
                };
                return registro;
            }
        }
    }

    public class Register : ITableEntity
    {
        public string Nombre { get; set; }
        public string Mensaje { get; set; }


        public string PartitionKey { get; set; }
        public string RowKey { get; set; }
        public DateTimeOffset? Timestamp { get; set; }
        public ETag ETag { get; set; }

    }
}

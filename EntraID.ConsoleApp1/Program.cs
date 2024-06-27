using Microsoft.Graph;
using Microsoft.Graph.Auth;
using Microsoft.Identity.Client;
using Newtonsoft.Json;

namespace EntraID.ConsoleApp1
{
    internal class Program
    {
        static IConfidentialClientApplication app;

        static void Main(string[] args)
        {
            Console.Clear();

            ///////////////////////////////////////////
            // Generar el objeto aplicación
            ///////////////////////////////////////////
            app = ConfidentialClientApplicationBuilder
                .Create()  // id de aplicación cliente
                .WithClientSecret()   // clave generada en secretos
                .WithAuthority("https://login.microsoftonline.com/b553b4ad-a812-4b1d-8023-93468b1c84a0")   //tenantid (id de directorio inquilino). Tenant es el directorio activo.
                .Build();

            ///////////////////////////////////////////
            // Generar el Token
            ///////////////////////////////////////////
            string[] scopes = new string[] { "https://graph.microsoft.com/.default" };         // urls a las que daría acceso, dentro del array
                                                                                               // al poner .default daría acceso a todo lo que hay detrás de la /
                                                                                               // se puede hacer añadiendo "enlace1", "enlace2"...
            AuthenticationResult token = app.AcquireTokenForClient(scopes).ExecuteAsync().Result;

            Console.WriteLine(token.AccessToken);
            Console.ReadKey();

            ///////////////////////////////////////////////
            // Listado de usuarios mediante cliente HTTP
            ///////////////////////////////////////////////

            Console.Clear();

            var http = new HttpClient();
            string url = "https://graph.microsoft.com/v1.0/users";

            http.DefaultRequestHeaders.Authorization =
                new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token.AccessToken);  // Bearer es un valor del JSON, es fijo

            var response = http.GetAsync(url).Result;
            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                string dataJSON = response.Content.ReadAsStringAsync().Result;
                OData data = JsonConvert.DeserializeObject<OData>(dataJSON);  // deserializamos el json recibido
                List<User> usuarios = JsonConvert.DeserializeObject<List<User>>(data.Value.ToString());
                foreach (var usuario in usuarios) Console.WriteLine($"HTTP -> {usuario.DisplayName} - {usuario.UserPrincipalName}");
                Console.WriteLine(Environment.NewLine);
            }
            else Console.WriteLine($"Error {response.StatusCode}");

            ///////////////////////////////////////////////
            // Listado de usuarios mediante objetos .NET
            ///////////////////////////////////////////////
            
            ClientCredentialProvider authProvider = new ClientCredentialProvider(app);
            GraphServiceClient graphClient = new GraphServiceClient(authProvider);

            var result = graphClient.Users.Request().GetAsync().Result;

            foreach (var user in result) Console.WriteLine($".NET -> {user.DisplayName} - {user.UserPrincipalName}");
            Console.WriteLine(Environment.NewLine);



        }
    }

    public class OData  // clase para deserializar

    {

        [JsonProperty("odata.metadata")]
        public string Metadata { get; set; }
        public Object Value { get; set; }

    }
}

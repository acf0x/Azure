using Microsoft.Azure.Cosmos;

namespace Azure.CosmoDB.ConsoleApp1
{
    internal class Program
    {
        static readonly string endPointCosmosDB = "https://demodbacf.documents.azure.com:443/";
        static readonly string keyCosmosDB = "dSyMWSKNGB24uyG3ltPdQkwPuScmcRlAS3V2dQP3OxttUx6j1sT2f3W5bRsyRHyzI1ZZRhRDyp6rACDbXSJr8g==";

        static CosmosClient clientCosmosDB;

        static void Main(string[] args)
        {
            clientCosmosDB = new CosmosClient(endPointCosmosDB, keyCosmosDB);

            GetDatabases();
        }


        static void GetDatabases()
        {
            var resultIterator = clientCosmosDB.GetDatabaseQueryIterator<DatabaseProperties>();

            while (resultIterator.HasMoreResults)
            {
                var allProperties = resultIterator.ReadNextAsync().Result;
                foreach (var property in allProperties)
                {
                    Console.WriteLine($"Base de datos: {property.Id}");
                    GetContainers(property.Id);
                }
            }
        }

        static void GetContainers(string databaseName)
        {
            Database clientDatabase = clientCosmosDB.GetDatabase(databaseName);

            var resultIterator = clientDatabase.GetContainerQueryIterator<ContainerProperties>();

            while (resultIterator.HasMoreResults)
            {
                var allProperties = resultIterator.ReadNextAsync().Result;
                foreach (var property in allProperties)
                {
                    Console.WriteLine($" -> {property.Id}");
                }
            }
        }

    }

    public class Producto
    {
        public string id { get; set; }
        public string referencia { get; set; }
        public string categoria { get; set; }
        public string descripcion { get; set; }
        public string cantidad { get; set; }
        public string precio { get; set; }
    }


    public record Product(string id, string referencia, string categoria, string descripcion, int cantidad, double precio);
}

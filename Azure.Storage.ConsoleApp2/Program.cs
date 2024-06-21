using Azure.Storage.Blobs;

namespace Azure.Storage.ConsoleApp2
{
    internal class Program
    {
        static string connectionString = "DefaultEndpointsProtocol=https;AccountName=demostorageacf;AccountKey=/4jCzw5CpzsNul9shtuYuk+9DRQPyAB4AdygkKMlvX7QgT1kV3xpAs6TuFfjTh5sycHU57xgSBF9+AStFxb+dg==;EndpointSuffix=core.windows.net";
        static BlobServiceClient client;
        static void Main(string[] args)
        {
            // Configurar el cliente
            client = new BlobServiceClient(connectionString);

            // Listado de contenedores
            var contenedores = client.GetBlobContainers();
            foreach (var container in contenedores)
            {
                Console.WriteLine($"Nombre del contenedor: {container.Name}");

                // Listado de Blobs
                var containerClient = client.GetBlobContainerClient(container.Name);
                var blobs = containerClient.GetBlobs();

                foreach (var blob in blobs)
                {
                    Console.WriteLine($"  > {blob.Name} ({blob.Properties.ContentType})");

                    var blobClient = containerClient.GetBlobClient(blob.Name);
                    blobClient.DownloadTo(@$"C:\EOIFormación\azure-csharp-main\Azure.Storage.ConsoleApp2\{blob.Name}");
                }
                Console.WriteLine($"");
            }
        }
    }
}


// 1 - client = BlobServiceClient(connectionString) // containers = client.GetBlobContainers // foreach container in containers
// 2 - containerClient = client.GetBlobContainerClient // blobs = containerClient.GetBlobs // foreach blob in blobs
// 3 - blobClient = containerClient.GetBlobClient(blob.Name) -
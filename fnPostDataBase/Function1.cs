using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace fnPostDataBase
{
    public class Function1
    {
        private readonly ILogger<Function1> _logger;

        public Function1(ILogger<Function1> logger)
        {
            _logger = logger;
        }

        [Function("movie")]
        [CosmosDBOutput("%CosmoDBDatabaseName%", "movies", Connection = "CosmoDBConnection", CreateIfNotExists = true   )]
        public async Task<object?> Run([HttpTrigger(AuthorizationLevel.Function, "get", "post")] HttpRequest req)
        {
            _logger.LogInformation("C# HTTP trigger function processed a request.");

            try
            {
                MovieRequest movie = null;

                using (StreamReader streamReader = new StreamReader(req.Body))
                {
                    string requestBody = await streamReader.ReadToEndAsync();
                    movie = JsonConvert.DeserializeObject<MovieRequest>(requestBody);
                }

                return JsonConvert.SerializeObject(movie);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error");
                return new BadRequestObjectResult("Error");
            }
        }
    }
}

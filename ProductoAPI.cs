using System.Data;
using System.Net;
using AFProductoAPI.Models;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Azure.Functions.Worker.Extensions.Sql;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;


namespace AFProductoAPI
{

    public class OutputType
    {
        [SqlOutput("dbo.Producto", "SqlConnectionString")]
        public ProductoDAO? producto { get; set; }

        public HttpResponseData? HttpResponse { get; set; }
    }

    public class ProductoAPI
    {
        private readonly ILogger _logger;

        public ProductoAPI(ILoggerFactory loggerFactory)
        {
            _logger = loggerFactory.CreateLogger<ProductoAPI>();
        }

        [Function("GetProducto")]
        public async Task<HttpResponseData> Get(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "productoapi")] 
            HttpRequestData req,
            [SqlInput("SELECT *  FROM [dbo].[Producto]", "SqlConnectionString")]
            IEnumerable<ProductoDAO> productos)
        {
            _logger.LogInformation("C# HTTP trigger function processed a request GET.");
            var response = req.CreateResponse(HttpStatusCode.OK);
            await response.WriteAsJsonAsync(productos);
            return response;
        }

        [Function("GetProductoById")]
        public async Task<HttpResponseData> GetById(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route ="productoapi/{IdProducto:int}")] 
            HttpRequestData req,
            [SqlInput("SELECT *  FROM [dbo].[Producto] where IdProducto=@IdProducto", 
            "SqlConnectionString",
            parameters: "@IdProducto={IdProducto}")]
            IEnumerable<ProductoDAO> productos)
        {
            _logger.LogInformation("C# HTTP trigger function processed a request GET BY ID.");
            var response = req.CreateResponse();
            if (productos.Count() > 0)
            {
                response.StatusCode = HttpStatusCode.OK;
                await response.WriteAsJsonAsync(productos.First());
            }
            else
            {
                response.StatusCode = HttpStatusCode.BadRequest;
            }
            return response;
        }

        [Function("PostProducto")]
        public async Task<OutputType> Post(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "productoapi")] 
            HttpRequestData req)
        {
            _logger.LogInformation("C# HTTP trigger function processed a request POST.");           
            var response = req.CreateResponse();
            OutputType output = new();
            try
            {
                ProductoDAO? producto = await req.ReadFromJsonAsync<ProductoDAO>();

                if (producto != null)
                {
                    response.StatusCode = HttpStatusCode.OK;
                    await response.WriteAsJsonAsync(producto);
                    output.producto = producto;
                }
                else
                {
                    response.StatusCode = HttpStatusCode.BadRequest;        
                }
            }catch  (Exception ex)
            {
                response.StatusCode = HttpStatusCode.BadRequest;             
            }
            output.HttpResponse = response;

            return output;
        }

        [Function("PutProducto")]
        public async Task<OutputType> Put(
            [HttpTrigger(AuthorizationLevel.Anonymous, "put", Route = "productoapi/{IdProducto:int}")] 
            HttpRequestData req,
            [SqlInput("SELECT *  FROM [dbo].[Producto] where IdProducto=@IdProducto",
            "SqlConnectionString",
            parameters: "@IdProducto={IdProducto}")]
            IEnumerable<ProductoDAO> productos)
        {
            _logger.LogInformation("C# HTTP trigger function processed a request PUT.");
            var response = req.CreateResponse();
            OutputType output = new();

            try
            {              
                ProductoDAO? productoEditado = await req.ReadFromJsonAsync<ProductoDAO>();
                if (productos.Count() > 0 && productoEditado!=null)
                {
                    ProductoDAO? productoGuardado = productos.First();
                    productoGuardado.Nombre = productoEditado.Nombre != null ? productoEditado.Nombre : productoGuardado.Nombre;
                    productoGuardado.Cantidad = productoEditado.Cantidad != 0 ? productoEditado.Cantidad : productoGuardado.Cantidad;
                    productoGuardado.Descripcion = productoEditado.Descripcion != null ? productoEditado.Descripcion : productoGuardado.Descripcion;
                    productoGuardado.UrlImage = productoEditado.UrlImage != null ? productoEditado.UrlImage : productoGuardado.UrlImage;
                    response.StatusCode = HttpStatusCode.OK;
                    await response.WriteAsJsonAsync(productoGuardado);
                    output.producto = productoGuardado;                 
                }
                else
                {
                    response.StatusCode = HttpStatusCode.BadRequest;
                }
            }
            catch (Exception ex)
            {
                response.StatusCode = HttpStatusCode.BadRequest;
            }

            output.HttpResponse = response;
            return output;    
        }

        [Function("DeleteProducto")]
        public async Task<HttpResponseData> Delete(
            [HttpTrigger(AuthorizationLevel.Anonymous, "delete",Route = "productoapi/{IdProducto:int}")] 
            HttpRequestData req,
             [SqlInput("DELETE FROM [dbo].[Producto] where IdProducto=@IdProducto",
            "SqlConnectionString",
            parameters: "@IdProducto={IdProducto}")]
            IEnumerable<ProductoDAO> productos)
        {
            _logger.LogInformation("C# HTTP trigger function processed a request DELETE.");
            var response = req.CreateResponse(HttpStatusCode.OK);
            await response.WriteAsJsonAsync(productos);
            return response;
        }
    }
}

using System.Net;
using AFProductoAPI.Models;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;


namespace AFProductoAPI
{
    public class ProductoAPI
    {
        private readonly ILogger _logger;
        public static List<Producto> ListaProductos = new List<Producto>
        {
            new Producto
            {
                IdProducto =0,
                Nombre="Producto1",
                Descripcion="Descripcion 1",
                Cantidad=3,
                UrlImage =null
            }
        };

        public ProductoAPI(ILoggerFactory loggerFactory)
        {
            _logger = loggerFactory.CreateLogger<ProductoAPI>();
        }

        [Function("GetProducto")]
        public async Task<HttpResponseData> Get([HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "productoapi")] HttpRequestData req)
        {
            _logger.LogInformation("C# HTTP trigger function processed a request GET.");
            var response = req.CreateResponse(HttpStatusCode.OK);
            await response.WriteAsJsonAsync(ListaProductos);
            return response;
        }

        [Function("GetProductoById")]
        public async Task<HttpResponseData> GetById(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route ="productoapi/{IdProducto}")] HttpRequestData req,
            string IdProducto)
        {
            _logger.LogInformation("C# HTTP trigger function processed a request GET BY ID.");
            var response = req.CreateResponse();
            try
            {
                Producto? producto = ListaProductos.FirstOrDefault(x => x.IdProducto == Int32.Parse(IdProducto));

                if (producto != null)
                {
                    response.StatusCode = HttpStatusCode.OK;
                    await response.WriteAsJsonAsync(producto);
                }
                else
                {
                    response.StatusCode = HttpStatusCode.BadRequest;
                }
            }catch (Exception ex)
            {
                response.StatusCode = HttpStatusCode.BadRequest;
            }
            return response;
        }

        [Function("PostProducto")]
        public async Task<HttpResponseData> Post([HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "productoapi")] HttpRequestData req)
        {
            _logger.LogInformation("C# HTTP trigger function processed a request POST.");

            string requestData = await new StreamReader(req.Body).ReadToEndAsync();
            var response = req.CreateResponse();
            try
            {
                Producto? producto = JsonConvert.DeserializeObject<Producto>(requestData);
               
                if (producto != null)
                {
                    ListaProductos.Add(producto);
                    response.StatusCode = HttpStatusCode.OK;
                    await response.WriteAsJsonAsync(producto);
                }
                else
                {
                    response.StatusCode = HttpStatusCode.BadRequest;
                }
            }catch  (Exception ex)
            {
                response.StatusCode = HttpStatusCode.BadRequest;
            }
           

            return response;
        }

        [Function("PutProducto")]
        public async Task<HttpResponseData> Put([HttpTrigger(AuthorizationLevel.Anonymous, "put", Route = "productoapi/{IdProducto}")] HttpRequestData req,
            string IdProducto)
        {
            _logger.LogInformation("C# HTTP trigger function processed a request PUT.");

            var response = req.CreateResponse();
            string requestData = await new StreamReader(req.Body).ReadToEndAsync();
            try
            {
                Producto? productoEditado = JsonConvert.DeserializeObject<Producto>(requestData);
                Producto? productoGuardado = ListaProductos.FirstOrDefault(x => x.IdProducto == Int32.Parse(IdProducto));

                if (productoEditado != null && productoGuardado != null)
                {
                    productoGuardado.Nombre = productoEditado.Nombre != null ? productoEditado.Nombre : productoGuardado.Nombre;
                    productoGuardado.Cantidad = productoEditado.Cantidad != 0 ? productoEditado.Cantidad : productoGuardado.Cantidad;
                    productoGuardado.Descripcion = productoEditado.Descripcion != null ? productoEditado.Descripcion : productoGuardado.Descripcion;
                    productoGuardado.UrlImage = productoEditado.UrlImage != null ? productoEditado.UrlImage : productoGuardado.UrlImage;
                    response.StatusCode = HttpStatusCode.OK;
                    await response.WriteAsJsonAsync(productoGuardado);
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


            return response;    
        }

        [Function("DeleteProducto")]
        public async Task<HttpResponseData> Delete([HttpTrigger(AuthorizationLevel.Anonymous, "delete",Route = "productoapi/{IdProducto}")] HttpRequestData req,
            string IdProducto)
        {
            _logger.LogInformation("C# HTTP trigger function processed a request DELETE.");

            var response = req.CreateResponse();
            try
            {
                Producto? producto = ListaProductos.FirstOrDefault(x => x.IdProducto == Int32.Parse(IdProducto));

                if (producto != null)
                {
                    response.StatusCode = HttpStatusCode.NoContent;
                    ListaProductos.Remove(producto);
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
            return response;
        }
    }
}

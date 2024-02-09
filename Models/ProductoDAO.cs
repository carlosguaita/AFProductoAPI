using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AFProductoAPI.Models
{
    public class ProductoDAO
    {
        [Key]
        public int IdProducto { get; set; }
        [Required]
        public string Nombre { get; set; }
        public string Descripcion { get; set; }
        [Required]
        public int Cantidad { get; set; }
        public string UrlImage { get; set; }
    }
}

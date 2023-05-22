using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JCTobon.Clases
{
    public class ProductoVenta
    {
        public int ID { get; set; }
        public string Tipo { get; set; }
        public string Nombre { get; set; }
        public string Talla { get; set; }
        public int PrecioVenta { get; set; }
        public int CantidadPiezas { get; set; }
        public int Total { get; set; }
        public string Folio { get; set; }
        public DateTime Fecha { get; set; }

        public ProductoVenta(int id, string tipo, string nombre, string talla, int precioVenta, int cantidadPiezas, int total, string folio, DateTime fecha)
        {
            ID = id;
            Tipo = tipo;
            Nombre = nombre;
            Talla = talla;
            PrecioVenta = precioVenta;
            CantidadPiezas = cantidadPiezas;
            Total = total;
            Folio = folio;
            Fecha = fecha;
        }
    }

}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XmasAppCodePro.Models
{
    public class Prodotto
    {
        public int Id { get; set; }
        public string Barcode { get; set; }
        public string Nome { get; set; }
        public string Descrizione { get; set; }
        public string ImageUrl { get; set; }
        public decimal Prezzo { get; set; } = 0;
        public decimal PesoLordo { get; set; } = 0;
        public int Quantita { get; set; } = 0;
        public int CategoriaId { get; set; }

    }
}

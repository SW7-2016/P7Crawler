using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace analyzer.Products.Retailers
{
    class Retailer
    {
        public string name = "";
        public string url = "";
        public decimal price = 0;

        public Retailer(int id, int productId)
        {
            Id = id;
            ProductId = productId;
        }

        public int ProductId { get; }
        public int Id { get; }
    }
}

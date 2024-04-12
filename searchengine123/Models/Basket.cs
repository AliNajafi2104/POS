using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace searchengine123
{
   
        public class Basket
        {
            public List<Product> lister = new List<Product>();
            public double pris_ { get; set; }
            public void add(Product x)
            {
                lister.Add(x);
            }
        }
    
}

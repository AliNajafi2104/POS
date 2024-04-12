using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace searchengine123
{
    public class Product
    {

        public int Antal { get; set; }
        public string Vare { get; set; }
        public string Stregkode { get; set; }
        public double Pris { get; set; }
        public string Kategori { get; set; }
        public string Ingen_stregkodemærkning {  get; set; }
        
        public Product()
        { Antal = 1; }


    }
}


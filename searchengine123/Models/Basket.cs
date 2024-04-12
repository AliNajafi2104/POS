using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace searchengine123
{
   
        public class Basket
        {
        
        public Dictionary<String,double> keyValuePairs { get; set; }
        public double Total { get; set; }
        public DateTime time { get; set; }
        public Basket()
        {
            // Set the time property to the current time
            time = DateTime.Now;
        }

    }
    
}

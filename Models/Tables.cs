using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace aspprojet.Models
{
    public class Tables
    {
        public IEnumerable<client> clients { get; set; }
        public IEnumerable<proprietaire> proprietaires { get; set; }
   
    }
}
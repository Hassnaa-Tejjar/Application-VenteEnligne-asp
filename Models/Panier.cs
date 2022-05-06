using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace aspprojet.Models
{
    public class Panier
    {
        
        public article art
        {
            get;
            set;
        }
        
        public int Quantity
        {
            get;
            set;
        }
        public client clt
        {
            get;
            set;
        }

    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace aspprojet.Models
{
    public class Favoris
    {
        public article produit
        {
            get;
            set;
        }
        public client client
        {
            get;
            set;
        }
    }
}
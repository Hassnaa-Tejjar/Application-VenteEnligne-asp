using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace aspprojet.Models
{
    public class FavorisProp
    {
        public article produit
        {
            get;
            set;
        }
        public proprietaire prop
        {
            get;
            set;
        }
    }
}
﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace aspprojet.Models
{
    public class historique
    {
        public proprietaire proprietaire
        {
            get;
            set;
        }
        public String action
        {
            get;
            set;
        }
        public DateTime date
        {
            get;
            set;
        }
    }
}
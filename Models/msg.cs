using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace aspprojet.Models
{
    public class msg
    {
        public IEnumerable<message> clients { get; set; }
        public IEnumerable<messageprop> proprietaires { get; set; }
    }
}
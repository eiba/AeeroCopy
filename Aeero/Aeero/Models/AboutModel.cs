using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Aeero.Models
{
    public class AboutModel
    {
        public Contact Contact { get; set; }
        public FacebookModel Facebook { get; set; }
        public TwitterModel Twitter { get; set; }
    }
}
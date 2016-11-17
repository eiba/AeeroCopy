using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Aeero.Models
{
    public class PRpartial
    {
        public Contact Contact { get; set; }
        public IEnumerable<Contact> IndexContact { get; set; }
        public Hours Hours { get; set; }
        public IEnumerable<Hours> IndexHours { get; set; }
        public FacebookModel Facebook { get; set; }
        public TwitterModel Twitter { get; set; }
    }
}
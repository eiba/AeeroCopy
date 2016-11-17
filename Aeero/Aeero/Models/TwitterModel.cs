using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Aeero.Models
{
    public class TwitterModel
    {
        [Key]
        public virtual int Id { get; set; }

        public string Url { get; set; }
        [DisplayName("Brukernavn")]
        public string UserName { get; set; }
        public bool isActive { get; set; }
    }
}
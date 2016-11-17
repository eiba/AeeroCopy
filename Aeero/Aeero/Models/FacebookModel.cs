using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Aeero.Models
{
    public class FacebookModel
    {
        [Key]
        public virtual int Id { get; set; }

        [Required]
        public string Url { get; set; }
        public bool isActive { get; set; }
    }
}
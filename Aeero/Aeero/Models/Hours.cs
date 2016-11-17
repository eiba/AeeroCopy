using System.Web;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Web.Script.Serialization;

namespace Aeero.Models
{
    [Table("Hours")]
    public class Hours
    {
        [Key]
        [Required]
        public virtual int HoursId { get; set; }

        [Required]
        [DisplayName("Dag")]
        public virtual string Day { get; set; }

        [DisplayName("Åpningstid")]
        [DisplayFormat(DataFormatString = "{0:hh\\:mm}", ApplyFormatInEditMode = true)]
        public virtual System.TimeSpan? OpeningHours { get; set; }

        [DisplayName("Stengetid")]
        [DisplayFormat(DataFormatString = "{0:hh\\:mm}", ApplyFormatInEditMode = true)]
        public virtual System.TimeSpan? ClosingHours { get; set; }

        [DisplayName("Levering Start")]
        [DisplayFormat(DataFormatString = "{0:hh\\:mm}", ApplyFormatInEditMode = true)]
        public virtual System.TimeSpan? DeliveryStart { get; set; }

        [DisplayName("Levering Slutt")]
        [DisplayFormat(DataFormatString = "{0:hh\\:mm}", ApplyFormatInEditMode = true)]
        public virtual System.TimeSpan? DeliveryEnd { get; set; }
    }
}
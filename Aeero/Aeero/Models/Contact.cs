using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Aeero.Models
{
    [Table("Contact")]
    public class Contact
    {
        [Key]
        [Required]
        public virtual int ContactId { get; set; }

        [Required]
        [MaxLength(20)]
        [DisplayName("Telefon")]
        public virtual string Phone { get; set; }

        [Required]
        [DisplayName("E-mail")]
        [EmailAddress]
        public virtual string Email { get; set; }

        [Required]
        [DisplayName("Adresse")]
        public virtual string Address { get; set; }

        [Required]
        [DisplayName("Postnummer")]
        public virtual int ZipCode { get; set; }

        [Required]
        [DisplayName("Sted")]
        public virtual string City { get; set; }

        [Required]
        [DisplayName("Fastpris levering")]
        public virtual int FixedPriceDelivery { get; set; }
        
        [Required]
        [DisplayName("Stripe (offentlig nøkkel)")]
        public virtual string StripePublicKey { get; set; }

        [Required]
        [DisplayName("Stripe (privat nøkkel)")]
        public virtual string StripePrivateKey { get; set; }

        [Required]
        [DisplayName("Twilio (konto-SID)")]
        public virtual string TwilioAccountSid { get; set; }

        [Required]
        [DisplayName("Twilio (autentiseringsnøkkel)")]
        public virtual string TwilioAuthToken { get; set; }

        
    }
}
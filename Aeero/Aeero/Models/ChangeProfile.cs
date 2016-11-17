using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Aeero.Models
{
    [Table("ChangeProfile")]
    public class ChangeProfile
    {
        [Required]
        [Display(Name = "Fornavn")]
        public string firstName { get; set; }

        [Required]
        [Display(Name = "Etternavn")]
        public string lastName { get; set; }

        [Required]
        [Display(Name = "Adresse")]
        public string Adress { get; set; }

        [Required]
        [Display(Name = "Poststed")]
        public string City { get; set; }

        [Required]
        [Display(Name = "Postnummer")]
        public string zipCode { get; set; }

        [Required]
        [StringLength(20, ErrorMessage = "Telefonummer må være mellom 8 og 20 karakterer", MinimumLength = 8)]
        [RegularExpression(@"^[0-9]+$", ErrorMessage = "Telefonummer kan kun vare karakterer mellom 0 og 9 samt +")]
        [Display(Name = "Tlf")]
        public string Phone { get; set; }

        [Required]
        [EmailAddress(ErrorMessage = "Ugyldig email adresse")]
        [Display(Name = "Email")]
        public string Email { get; set; }

        [Required]
        [Display(Name = "Brukertype")]
        public string UserRole { get; set; }

        [Display(Name = "Brukernavn")]
        [DataType("EmailAddress")]
        public string UserName { get; set; }

        [Required]
        [Display(Name = "Rollenummer")]
        public int roleNr { get; set; }

        [Required]
        [Display(Name = "Id")]
        public string Id { get; set; }

        [StringLength(100, ErrorMessage = "{0} Må være minst {2} karakterer langt.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Passord")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Bekreft passord")]
        [Compare("Password", ErrorMessage = "Passordene må stemme overens!")]
        public string ConfirmPassword { get; set; }

        public string vueIdd { get; set; }
        public string modalIdd { get; set; }
        public string check { get; set; }
        public string search { get; set; }
    }
}
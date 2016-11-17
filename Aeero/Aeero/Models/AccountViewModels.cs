using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.ModelBinding;

namespace Aeero.Models
{
    public class ExternalLoginConfirmationViewModel
    {
        [Required]
        [Display(Name = "Email")]
        public string Email { get; set; }
    }

    public class ExternalLoginListViewModel
    {
        public string ReturnUrl { get; set; }
    }

    public class SendCodeViewModel
    {
        public string SelectedProvider { get; set; }
        public ICollection<System.Web.Mvc.SelectListItem> Providers { get; set; }
        public string ReturnUrl { get; set; }
        public bool RememberMe { get; set; }
    }

    public class VerifyCodeViewModel
    {
        [Required]
        public string Provider { get; set; }

        [Required]
        [Display(Name = "Code")]
        public string Code { get; set; }
        public string ReturnUrl { get; set; }

        [Display(Name = "Remember this browser?")]
        public bool RememberBrowser { get; set; }

        public bool RememberMe { get; set; }
    }

    public class ForgotViewModel
    {
        [Required]
        [Display(Name = "Email")]
        public string Email { get; set; }
    }

    public class SearchModel
    {
        public string search { get; set; }
    }

    public class SearchUserViewModel
    {
        public SearchModel SearchModel { get; set; }
        public IEnumerable<ApplicationUser> ApplicationUsers { get; set; }
        public RegisterViewModel RegisterViewModel { get; set; }
    }

    public class LoginViewModel
    {
        [Required]
        [Display(Name = "Epost")]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Passord")]
        public string Password { get; set; }

        [Display(Name = "Husk meg")]
        public bool RememberMe { get; set; }
    }

    public class RegisterViewModel
    {
        [Required]
        [Display(Name = "Fornavn")]
        public string firstName { get; set; }

        [Required]
        [Display(Name = "Etternavn")]
        public string lastName { get; set; }

        [Required]
        [Display(Name = "Adresse")]
        public string Address { get; set; }

        [Required]
        [Display(Name = "Poststed")]
        public string City { get; set; }

        [Required]
        [Display(Name = "Postnummer")]
        public string  zipCode { get; set; }

        [Required]
        [StringLength(12,ErrorMessage = "Telefonummer må være mellom 8 og 12 karakterer",MinimumLength =8)]
        [RegularExpression(@"^[0-9]+$",ErrorMessage ="Telefonummer kan kun vare karakterer mellom 0 og 9")]
        [Display(Name = "Tlf")]
        public string Phone { get; set; }

        [Required]
        [EmailAddress(ErrorMessage = "Ugyldig email adresse")]
        [Display(Name = "Email")]
        public string Email { get; set; }

        [Required]
        [Display(Name = "Brukertype")]
        public string UserRole { get; set; }

        [Display(Name = "Rollenummer")]
        public int roleNr { get; set; }

        [Display(Name = "Utestengt")]
        public bool IsEnabeled { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "{0} Må være minst {2} karakterer langt.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Passord")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Bekreft passord")]
        [Compare("Password", ErrorMessage = "Passordene må stemme overens!")]
        public string ConfirmPassword { get; set; }

        public string param { get; set; }
        public string check { get; set; }
    }

    public class ResetPasswordViewModel
    {
        [Required]
        [EmailAddress]
        [Display(Name = "Email")]
        public string Email { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Passord")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Bekreft passord")]
        [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }

        public string Code { get; set; }
    }

    public class ForgotPasswordViewModel
    {
        [Required]
        [EmailAddress]
        [Display(Name = "Email")]
        public string Email { get; set; }
    }
}

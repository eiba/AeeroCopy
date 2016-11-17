using System.Data.Entity;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;

namespace Aeero.Models
{
    // You can add profile data for the user by adding more properties to your ApplicationUser class, please visit http://go.microsoft.com/fwlink/?LinkID=317594 to learn more.
    public class ApplicationUser : IdentityUser
    {
        public string firstName { get; set; }
        public string lastName { get; set; }
        public string Adress { get; set; }
        public string City { get; set; }
        public string zipCode { get; set; }
        public string Phone { get; set; }
        public string UserRole { get; set; }
        public int roleNr { get; set; }
        public bool IsEnabeled { get; set; }
        public bool show { get; set; }

        public async Task<ClaimsIdentity> GenerateUserIdentityAsync(UserManager<ApplicationUser> manager)
        {
            // Note the authenticationType must match the one defined in CookieAuthenticationOptions.AuthenticationType
            var userIdentity = await manager.CreateIdentityAsync(this, DefaultAuthenticationTypes.ApplicationCookie);
            // Add custom user claims here
            return userIdentity;
        }

    }

    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext()
            : base("DefaultConnection", false)
        {
        }

        public virtual DbSet<Pizza> Pizza { get; set; }
        public virtual DbSet<Ingredient> Ingredient { get; set; }
        public virtual DbSet<Contact> Contact { get; set; }
        public virtual DbSet<Hours> Hours { get; set; }
        public virtual DbSet<IngredientType> IngredientType { get; set; }
        public virtual DbSet<PizzaIngredient> PizzaIngredient { get; set; }
        public DbSet<File> Files { get; set; }
        public virtual DbSet<Order> Order { get; set; }
        public virtual DbSet<PhoneCode> PhoneCode { get; set; }
        public virtual DbSet<FacebookModel> Facebook { get; set; }
        public virtual DbSet<TwitterModel> Twitter { get; set; }

        public static ApplicationDbContext Create()
        {
            return new ApplicationDbContext();
        }
    }
}

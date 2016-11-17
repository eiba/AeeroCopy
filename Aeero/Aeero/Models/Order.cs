using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using RestSharp.Deserializers;

namespace Aeero.Models
{
    [Table("Order")]
    public class Order
    {
        public Order()
        {
            State = "new";
            ReceiptKey = Guid.NewGuid().ToString();
            LastModified = DateTime.UtcNow;
            OrderTime = DateTime.UtcNow;
        }

        [Key]
        [Required]
        public int Id { get; set; }

        public string share { get; set; }
        public TwitterModel Twitter { get; set; }

        // --- Same as in OrderViewModel
        [Required]
        [MinLength(2)]
        [Display(Name = "Kundens navn")]
        public string CustomerName { get; set; }

        [Required]
        [RegularExpression("^[0-9]{8}")]
        [Display(Name = "Telefon")]
        public string Phone { get; set; }

        [Required]
        [Display(Name = "Hentes / leveres")]
        public DateTime DeliveryTime { get; set; }

        [Required]
        [Display(Name = "Skal leveres")]
        public bool Deliver { get; set; }

        [Display(Name = "Adresse")]
        public string Address { get; set; }

        [Display(Name = "Postnummer")]
        public string ZipCode { get; set; }

        [Display(Name = "Poststed")]
        public string City { get; set; }

        [Required]
        [Range(0, 100)]
        [Display(Name = "Rabatt %")]
        public int Discount { get; set; }

        // Price before discount
        [Required]
        [Range(0.0, 99999.0)]
        [Display(Name = "Pris")]
        public double Price { get; set; }

        [Display(Name = "Er betalt")]
        public bool IsPaid { get; set; }

        [Required]
        public string ReceiptKey { get; set; }

        // ---

        [Required]
        [Display(Name ="Bestillingstidspunkt")]
        public DateTime OrderTime { get; set; }
        
        /// <summary>
        /// Never set this manually! "new" is default, and use the NextState method to return next state. 
        /// </summary>
        [Required]
        [MaxLength(20)]
        [DefaultValue("new")]
        [Display(Name = "Stadie")]
        public string State { get; set; }

        [DisplayName("Sist Endret")]
        public DateTime? LastModified { get; set; }

        /// <summary>
        /// list of ordered pizzas
        /// </summary>
        [Required]
        [Display(Name ="Ordrelinjer")]
        public virtual List<OrderLine> Lines { get; set; }

        public virtual bool Canceled { get; set; }

        /// <summary>
        /// Returns the next state of an order. this is corresponding to the next workstation.
        /// </summary>
        /// <returns>The next state of an order</returns>
        public string NextState()
        {
            switch (State)
            {
                case "new":
                    return "confirmed";
                case "confirmed":
                    return "preparation";
                case "preparation":
                    return "oven";
                case "oven":
                    return Deliver ? "delivery" : "ready";
                case "delivery":
                    return "done";
                case "ready":
                    return "done";
                case "done": // done -> done to prevent invalid state error. 
                    return "done";
                default:
                    throw new Exception("Invalid state");
            }
        }

        public string StateLabel()
        {
            switch (State)
            {
                case "new":
                    return "Ny ordre";
                case "confirmed":
                    return "Bekreftet";
                case "preparation":
                    return "Baking";
                case "oven":
                    return "Steking";
                case "delivery":
                    return "Levering";
                case "ready":
                    return "Klar til henting";
                case "done":
                    return "Ferdig";
                default:
                    return "Ukjent status";
            }
        }

        public string NextStateLabel()
        {
            switch (State)
            {
                case "new":
                    return "Bekreftet";
                case "confirmed":
                    return "Baking";
                case "preparation":
                    return "Steking";
                case "oven":
                    return Deliver ? "Levering" : "Klar til henting";
                case "delivery":
                    return "Ferdig";
                case "ready":
                    return "Ferdig";
                case "done": 
                    return "Ferdig";
                default:
                    return "Ukjent status";
            }
        }
        /// <summary>
        /// Curently setts returns the value to the previous state. 
        /// </summary>
        /// <returns>The value to be reset to.</returns>
        public string PrevState()
        {
            switch (State)
            {
                case "new": // new returns new to prevent invalid state from ocuring.
                    return "new";
                case "confirmed":
                    return "new";
                case "preparation":
                    return "confirmed";
                case "oven":
                    return "preparation";
                case "delivery":
                    return "oven";
                case "ready":
                    return "oven";
                case "done":
                    return Deliver ? "delivery" : "ready";
                default:
                    throw new Exception("Invalid state"); 
            }
        }

        public string PrevStateLabel()
        {
            switch (State)
            {
                case "new": // new returns new to prevent invalid state from ocuring.
                    return "Ny ordre";
                case "confirmed":
                    return "Ny ordre";
                case "preparation":
                    return "Bekreftet";
                case "oven":
                    return "Baking";
                case "delivery":
                    return "Steking";
                case "ready":
                    return "Steking";
                case "done":
                    return Deliver ? "Levering" : "Klar til henting";
                default:
                    return "Ukjent status";
            }

        }
    }

    public class OrderLine
    {
        [Key]
        [Required]
        public virtual int Id { get; set; }

        [Required]
        public virtual int OrderId { get; set; }
        public virtual Order Order { get; set; }
        public virtual string PizzaState { get; set; } // used to se if a specifi pizza has changed position

        [Required]
        public virtual bool Changed { get; set; } // Set to notify employees that this has been changed from the original pizza template.

        [Required]
        public virtual string Description { get; set; } // Usually set to pizza name.
        
        [Required]
        public virtual PizzaSize Size { get; set; }

        [Required]
        public virtual int Count { get; set; }
        
        /// <summary>
        /// List over pizza ingredients to this pizza
        /// </summary>
        public virtual List<OrderLineIngredient> Ingredients { get; set; }

        public string SizeLabel()
        {
            switch (Size)
            {
                case PizzaSize.Large:
                    return "Stor";
                case PizzaSize.Medium:
                    return "Medium";
                case PizzaSize.Small:
                    return "Liten";
                default:
                    return "Ukjent";
            }
        }
    }

    public class OrderLineIngredient
    {
        [Key]
        [Required]
        public virtual int Id { get; set; }

        /// <summary>
        /// amount of an ingredient
        /// </summary>
        [Required]
        public virtual int Count { get; set; }

        
        [Required]
        public virtual int IngredientId { get; set; }

        /// <summary>
        /// the ingredient
        /// </summary>
        public virtual Ingredient Ingredient { get; set; }
    }

    [Table("PhoneCode")]
    public class PhoneCode
    {
        [Key]
        [Required]
        public virtual int Id { get; set; }

        [Required]
        public virtual string Number { get; set; }

        [Required]
        public virtual string Code { get; set; }

        [Required]
        public virtual DateTime Created { get; set; }

        public static string GeneratedCode()
        {
            const string chars = "ABCDEFGHJKLMNOPQRSTUVWXYZ023456789";
            var random = new Random();
            return new string(Enumerable.Repeat(chars, 5)
              .Select(s => s[random.Next(s.Length)]).ToArray());
        }
    }
}
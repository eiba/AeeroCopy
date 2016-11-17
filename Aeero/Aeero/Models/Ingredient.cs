using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Web.Script.Serialization;

namespace Aeero.Models
{
    [Table("Ingredient")]
    public class Ingredient
    {
        [Key]
        [Required]
        public virtual int IngredientId { get; set; }

        [Required]
        [DisplayName("Ingrediensnavn")]
        public virtual string Name { get; set; }

        [Required]
        [DisplayName("Vegetar")]
        public virtual bool Vegetarian { get; set; }

        [Required]
        [DisplayName("Pris (liten)")]
        public virtual double PriceSmall { get; set; }

        [Required]
        [DisplayName("Pris (medium)")]
        public virtual double PriceMedium { get; set; }

        [Required]
        [DisplayName("Pris (stor)")]
        public virtual double PriceLarge { get; set; }

        [DisplayName("Aktiv")]
        public virtual bool isActive { get; set; }

        public int TypeId { get; set; }
        
        [ForeignKey("TypeId")]
        public virtual IngredientType Type { get; set; }

        public double GetPriceForSize(PizzaSize size)
        {
            switch (size)
            {
                case PizzaSize.Small:
                    return PriceSmall;
                case PizzaSize.Medium:
                    return PriceMedium;
                case PizzaSize.Large:
                    return PriceLarge;
                default:
                    return 0;
            }
        }
    }
}
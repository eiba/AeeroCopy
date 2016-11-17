using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Aeero.Models
{
    public enum PizzaSize : int
    {
        Small,
        Medium,
        Large
    }

    [Table("Pizza")]
    public class Pizza
    {
        [Key]
        [Required]
        public virtual int PizzaId { get; set; }

        [Required]
        [DisplayName("Navn")]
        public virtual string Name { get; set; }

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

        public virtual List<PizzaIngredient> Ingredients { get; set; }
        public virtual ICollection<File> Files { get; set; }

        // Helper method to easily convert a pizza template to an order line, without any changes.
        public virtual OrderLine ToOrderLine(PizzaSize size, int count = 1)
        {
            OrderLine line = new OrderLine
            {
                Size = size,
                Count = count,
                Description = Name,
                Ingredients = new List<OrderLineIngredient>()
            };

            foreach (var ingredient in Ingredients)
            {
                line.Ingredients.Add(new OrderLineIngredient
                {
                    Count = ingredient.Count,
                    Ingredient = ingredient.Ingredient
                });
            }

            return line;
        }

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

    [Table("PizzaIngredient")]
    public class PizzaIngredient
    {
        [Key]
        [Required]
        public virtual int PizzaIngredientId { get; set; }

        [Required]
        [DisplayName("Pizza")]
        public virtual int PizzaId { get; set; }

        [Required]
        [DisplayName("Ingrediens")]
        public virtual int IngredientId { get; set; }

        [Required]
        [DisplayName("Antall")]
        public virtual int Count { get; set; }

        public virtual Ingredient Ingredient { get; set; }
        public virtual Pizza Pizza { get; set; }
    }
}
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using Microsoft.Owin.Security.Provider;

namespace Aeero.Models
{
    public class OrderViewModel
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        public string PaymentToken { get; set; }

        // --- Same in Order model
        [Required]
        [MinLength(2)]
        public string CustomerName { get; set; }

        [Required]
        [RegularExpression("^[0-9]{8}")]
        public string Phone { get; set; }

        [Required]
        public DateTime DeliveryTime { get; set; }

        [Required]
        public bool Deliver { get; set; }

        public string Address { get; set; }
        public string ZipCode { get; set; }
        public string City { get; set; }

        // Not currently saved to order, but used for order confirmation after payment.
        public string Email { get; set; }

        // Only used when SMS confirmation of order is used.
        public string SMSCode { get; set; }

        [Required]
        [Range(0, 100)]
        public int Discount { get; set; }
        // ---

        [Required]
        public List<OrderLineViewModel> Lines { get; set; }

        public double CalculatePrice()
        {
            var price = 0.0;
            if(Deliver) price += db.Contact.First().FixedPriceDelivery;
            price += Lines.Sum(o => o.Pizza.CalculatePrice() * o.Count);

            return price;
        }

        public Order ToOrder()
        {
            var order = new Order
            {
                CustomerName = CustomerName,
                Phone = Phone,
                DeliveryTime = DeliveryTime,

                State = "new",

                Deliver = Deliver,
                Address = Address,
                ZipCode = ZipCode,
                City = City,

                Discount = Discount,
                Price = CalculatePrice(), // Price before discount is applied.

                Lines = new List<OrderLine>()

            };

            foreach (var orderLine in Lines)
            {
                var orderLineIngredients = new List<OrderLineIngredient>();
                foreach (var lineIngredient in orderLine.Pizza.Ingredients)
                {
                    orderLineIngredients.Add(new OrderLineIngredient
                    {
                        Count = lineIngredient.Count,
                        IngredientId = lineIngredient.Id,
                    });
                }
                order.Lines.Add(new OrderLine
                {
                    Description = orderLine.Pizza.Name,
                    Changed = orderLine.Pizza.Changed,
                    Size = orderLine.Pizza.Size,
                    Ingredients = orderLineIngredients,
                    Count = orderLine.Count,
                    Order = order
                });
            }

            return order;
        }

        // Returns a list of validation error messages for this order.
        public List<string> Validate()
        {
            var errors = new List<string>();
            var ingredientTypes = db.IngredientType.ToList();

            foreach(var line in Lines) { 
                var idCounts = new Dictionary<int, int>();
                foreach (var ingredient in line.Pizza.Ingredients)
                {
                    idCounts[ingredient.Id] = ingredient.Count;
                }

                foreach (var type in ingredientTypes)
                {
                    var max = type.TypeMax;
                    foreach(var ingredient in type.Ingredients)
                    {
                        if (idCounts.Keys.Contains(ingredient.IngredientId)) max -= idCounts[ingredient.IngredientId];
                    }

                    if (max < 0) errors.Add("For mange ingredienser av typen " + type.TypeName + ".");
                }
            }
            return errors;
        }
    }

    public class OrderLineViewModel
    {
        [Required]
        public OrderPizzaViewModel Pizza { get; set; }

        [Range(1, 99999)]
        public int Count { get; set; }
    }

    public class OrderPizzaViewModel
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // Used only in customer panel to get an idea of what pizza we had, if many
        // modifications were made by customer. This is the pizza template ID.
        public int Id { get; set; }

        [MinLength(2)]
        public string Name { get; set; }

        public bool Changed { get; set; }

        [Required]
        public PizzaSize Size { get; set; }

        [Required]
        public List<OrderIngredientViewModel> Ingredients { get; set; }

        public double CalculatePrice()
        {
            var template = db.Pizza.Find(Id);
            var price = template.GetPriceForSize(Size);

            // Remove prices for all ingredients in templates. We then re-add costs for all actual ingredients on this instance.
            template.Ingredients.ForEach(i => price -= i.Ingredient.GetPriceForSize(Size));

            // Re-add actual ingredients
            Ingredients.ForEach(i => price += db.Ingredient.Find(i.Id).GetPriceForSize(Size) * i.Count);

            return price;
        }
    }

    public class OrderIngredientViewModel
    {
        public int Id { get; set; }

        [Range(1, 99999)]
        public int Count { get; set; }
    }
}
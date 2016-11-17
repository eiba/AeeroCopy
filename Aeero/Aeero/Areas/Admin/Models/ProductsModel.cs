using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Aeero.Models;

namespace Aeero.Areas.Admin.Models
{
    public class ProductsModel
    {
        public IngredientModelExtended Ingredients { get; set; }
        public IngredientTypeModelExtended IngredientTypes { get; set; }
        public PizzaModelExtended Pizzas { get; set; }
    }
}
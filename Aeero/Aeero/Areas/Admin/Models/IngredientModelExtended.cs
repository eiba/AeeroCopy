﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Aeero.Models;

namespace Aeero.Areas.Admin.Models
{
    public class IngredientModelExtended
    {
        public IEnumerable<Ingredient> Ingredients { get; set; }
        public bool show { get; set; }
    }
}
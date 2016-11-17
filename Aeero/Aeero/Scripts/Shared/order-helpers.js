var SIZE_SMALL = 0;
var SIZE_MEDIUM = 1;
var SIZE_LARGE = 2;

var zipCodes = [];
$.getJSON('/Content/zipcodes.json', function (data) {
    zipCodes = data;
});

var getIngredientFromPizza = function (pizza, ingredientId) {
    for (var i = 0; i < pizza.Ingredients.length; i++) {
        var ingredient = pizza.Ingredients[i];
        if (ingredient.Id === ingredientId) return ingredient;
    }
}

var getIngredientType = function(ingredient) {
    return ingredientTypes.filter(function(type) {
        return type.Ingredients.map(function(ing) {
            return ing.Id;
        }).indexOf(ingredient.Id) !== -1;
    })[0];
}

// Compares ingredients from both the template and the pizza instance to see if it has changed. Does not set the Changed flag on the pizza instance!
var hasPizzaChanged = function (pizza) {
    var pizzaTemplate = pizzas.filter(function (p) {
        return p.Id === pizza.Id;
    })[0];

    return JSON.stringify(pizzaTemplate.Ingredients).split('').sort().join('') !== JSON.stringify(pizza.Ingredients).split('').sort().join('');
}

// Takes a pizza template, and returns a pizza instance ready object (set size, changed and price attribute, remove small, medium and large pricing)
var pizzaToPizzaInstance = function (pizza) {
    var copy = JSON.parse(JSON.stringify(pizza));
    copy.Size = SIZE_LARGE;
    copy.Price = copy.PriceLarge;
    copy.Changed = false;

    delete copy.PriceSmall;
    delete copy.PriceMedium;
    delete copy.PriceLarge;

    return copy;
}

// Takes in a pizza instance, and returns its price based on original price and changes made. Does not change the price on the instance!
var calculatePizzaPrice = function (pizza) {
    var pizzaTemplate = pizzas.filter(function (p) {
        return p.Id === pizza.Id;
    })[0];

    var price = 0;
    if (pizza.Size === SIZE_SMALL) price = pizzaTemplate.PriceSmall;
    else if (pizza.Size === SIZE_MEDIUM) price = pizzaTemplate.PriceMedium;
    else if (pizza.Size === SIZE_LARGE) price = pizzaTemplate.PriceLarge;

    if (!pizza.Changed) {
        return price;
    }

    var extraCounts = {};
    pizza.Ingredients.forEach(function (i) {
        extraCounts[i.Id] = i.Count;
    });

    pizzaTemplate.Ingredients.forEach(function (i) {
        if (extraCounts[i.Id]) extraCounts[i.Id] -= i.Count;
        else extraCounts[i.Id] = -i.Count;
    });

    ingredientTypes.forEach(function (type) {
        type.Ingredients.forEach(function (i) {
            if (!extraCounts[i.Id]) return;

            var iPrice = 0;
            if (pizza.Size === SIZE_SMALL) iPrice = i.PriceSmall;
            if (pizza.Size === SIZE_MEDIUM) iPrice = i.PriceMedium;
            if (pizza.Size === SIZE_LARGE) iPrice = i.PriceLarge;

            price += iPrice * extraCounts[i.Id];
        });
    });

    return price;
}

var ingredientCount = function(pizza, ingredientId) {
    if (!pizza) return 0; // Happens when no pizza is being edited, so avoid error.

    var ingredient = getIngredientFromPizza(pizza, ingredientId);
    if (ingredient) return ingredient.Count;
    return 0;
}

// Removes all ingredients in active "TypeUnique" ingredient type, and re-adds the newly selected one.
// Not too pretty, but workaround as we can't map directly between our JSON model and Vue.js.
var updateUniqueIngredients = function(pizza, uniqueIngredients) {
    for (var i = 0; i < ingredientTypes.length; i++) {
        var type = ingredientTypes[i];
        var selectedIngredientId = uniqueIngredients[type.Id];
        if (!selectedIngredientId) continue;
                    
        for (var j = 0; j < type.Ingredients.length; j++) {
            removeIngredientFromPizza(pizza, type.Ingredients[j].Id);
            if (type.Ingredients[j].Id === selectedIngredientId) {
                this.updateIngredientCount(pizza, type.Ingredients[j], 1, false);
            }
        }
    }
                
    pizza.Changed = hasPizzaChanged(pizza);
    pizza.Price = calculatePizzaPrice(pizza);
}

// Used in edit modal
var updateIngredientCount = function(pizza, ingredient, numChange, calculatePrice) {
    var ingredientInstance = getIngredientFromPizza(pizza, ingredient.Id);
    if (ingredientInstance) {
        ingredientInstance.Count += numChange;
        if (ingredientInstance.Count <= 0) removeIngredientFromPizza(pizza, ingredient.Id);
    } else if(numChange > 0) {
        pizza.Ingredients.push({
            Id: ingredient.Id,
            Name: ingredient.Name,
            Count: numChange,
            PriceSmall: ingredient.PriceSmall,
            PriceMedium: ingredient.PriceMedium,
            PriceLarge: ingredient.PriceLarge
        });
    }
                
    if (calculatePrice === true) {
        pizza.Changed = hasPizzaChanged(pizza);
        pizza.Price = calculatePizzaPrice(pizza);
    }
}

// Called if count of an ingredient instance is 0, used to remove that instance from the pizza order.
var removeIngredientFromPizza = function (pizza, ingredientId) {
    for (var i = 0; i < pizza.Ingredients.length; i++) {
        var ingredient = pizza.Ingredients[i];
        if (ingredient.Id !== ingredientId) continue;

        pizza.Ingredients.splice(i, 1);
        return true;
    }
}

// Counts number of ingredients and checks that not too many are chosen.
var validatePizza = function (pizza) {
    var idCounts = {};
    pizza.Ingredients.forEach(function (ingredient) {
        idCounts[ingredient.Id] = ingredient.Count;
    });

    var errors = [];
    ingredientTypes.forEach(function (type) {
        var max = type.Max;
        type.Ingredients.forEach(function (ingredient) {
            if (idCounts[ingredient.Id]) max -= idCounts[ingredient.Id]
        });

        if (max < 0) errors.push('For mange ingredienser av typen ' + type.Name + '.');
    });

    return errors;
}

var calculateOrderPrice = function(order) {
    var sum = 0;
    for (var i = 0; i < order.Lines.length; i++) {
        var line = order.Lines[i];
        sum += line.Count * line.Pizza.Price;
    }

    if (order.Discount) return sum * (1 - order.Discount / 100);
    else return sum;
}

// Returns a list of invalid fields.
var validateOrder = function(order) {
    var errorFields = [];
    if (order.CustomerName.length < 3) errorFields.push('Kundenavn');
    if (order.Phone.length !== 8) errorFields.push('Telefon');
    if (order.Deliver) {
        if (order.Address.length < 3) errorFields.push('Adresse');
        if (order.ZipCode.length !== 4) errorFields.push('Postnummer');
        if (order.City.length === 0) errorFields.push('Poststed');
    }
    if (order.Lines.length === 0) errorFields.push('Ingen ordrelinjer');

    return errorFields;
}

Vue.directive('datepicker', {
    params: ['sidebyside'],
    bind: function () {
        var vm = this.vm;
        var key = this.expression;
        $(this.el).datetimepicker({
            locale: 'nb',
            minDate: new Date(),
            sideBySide: this.params.sidebyside,
            inline: true
        }).on('dp.change', function (date) {
            vm.$set(key, date.date.toJSON());
        });
    },
    update: function (val) {
        $(this.el).datetimepicker('setDate', val);
    }
});

Vue.filter('commaSeparatedKey', function(arr, key) {
    var parts = arr.map(function(obj) {
        return obj[key];
    });

    return parts.join(', ');
});
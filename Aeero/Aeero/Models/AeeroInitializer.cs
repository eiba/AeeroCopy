using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace Aeero.Models
{
    public class AeeroDbInitializer : CreateDatabaseIfNotExists<ApplicationDbContext>
    {
        // Add some test data, which is always smart to do.
        protected override void Seed(ApplicationDbContext db)
        {
            // These two managers handle storage in the given db context for us
            var roleManager = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(db));
            var userManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(db));

            // First create the admin role
            roleManager.Create(new IdentityRole("Admin")); // rights to view admin and ansatt
            roleManager.Create(new IdentityRole("Ansatt")); // ansatt sites (area/touch mostly)
            roleManager.Create(new IdentityRole("Manager"));// rights of admin

            // Then add one manager user
            var managerUser = new ApplicationUser
            {
                UserName = "manager@uia.no",
                Email = "manager@uia.no",
                firstName = "Mr.",
                lastName = "Manager",
                Adress = "Jon Lilletunsvei 11",
                City = "Grimstad",
                zipCode = "4849",
                Phone = "98569434",
                UserRole = "Manager",
                roleNr = 3,
                IsEnabeled = true
            };
            userManager.Create(managerUser, "Password1.");
            userManager.AddToRole(managerUser.Id, "Admin");

            // Then add one admin user
            var adminUser = new ApplicationUser {
                UserName = "admin@uia.no",
                Email = "admin@uia.no",
                firstName ="Bernt",
                lastName ="Berntsen",
                Adress = "Jon Lilletunsvei 9",
                City ="Grimstad",
                zipCode ="4849",
                Phone ="94569434",
                UserRole = "Admin",
                roleNr = 2,
                IsEnabeled = true};
            userManager.Create(adminUser, "Password1.");
            userManager.AddToRole(adminUser.Id, "Admin");

            // Add one regular user
            var employeeUser = new ApplicationUser {
                UserName = "employee@uia.no",
                Email = "employee@uia.no",
                firstName = "Bjørge",
                lastName = "Bjørgsen",
                Adress = "Jon Lilletunsvei 10",
                City = "Grimstad",
                zipCode = "4849",
                Phone = "96569434",
                UserRole= "Ansatt",
                roleNr = 1,
                IsEnabeled = true};
            userManager.Create(employeeUser, "Password1.");
            userManager.AddToRole(employeeUser.Id, "Ansatt");


            // Pizza templates and ingredients
            
            var baseType = db.IngredientType.Add(new IngredientType { TypeMax = 1, TypeName = "Bunn", TypeUnique = true, isActive = true });
            var normalBase = db.Ingredient.Add(new Ingredient
            {
                Name = "Vanlig",
                PriceSmall = 60,
                PriceMedium = 80,
                PriceLarge = 100,
                Type = baseType,
                isActive=true
            });
            var wholeBase = db.Ingredient.Add(new Ingredient
            {
                Name = "Grov",
                PriceSmall = 70,
                PriceMedium = 90,
                PriceLarge = 110,
                Type = baseType,
                isActive = true
            });
            
            var cheeseType = db.IngredientType.Add(new IngredientType { TypeMax = 3, TypeName = "Ost", TypeUnique = false, isActive = true });
            var mozarellaCheese = db.Ingredient.Add(new Ingredient
            {
                Name = "Mozarella",
                PriceSmall = 15,
                PriceMedium = 20,
                PriceLarge = 25,
                Type = cheeseType,
                isActive = true
            });
            var cheddarCheese = db.Ingredient.Add(new Ingredient
            {
                Name = "Cheddar",
                PriceSmall = 12,
                PriceMedium = 18,
                PriceLarge = 26,
                Type = cheeseType,
                isActive = true
            });
            var goudaCheese = db.Ingredient.Add(new Ingredient
            {
                Name = "Gouda",
                PriceSmall = 11,
                PriceMedium = 17,
                PriceLarge = 25,
                Type = cheeseType,
                isActive = true
            });
            var jarlsbergCheese = db.Ingredient.Add(new Ingredient
            {
                Name = "Jarlsberg",
                PriceSmall = 16,
                PriceMedium = 22,
                PriceLarge = 28,
                Type = cheeseType,
                isActive = true
            });

            var sauceType = db.IngredientType.Add(new IngredientType { TypeMax = 3, TypeName = "Saus", TypeUnique = true, isActive = true });
            var tomatoSauce = db.Ingredient.Add(new Ingredient
            {
                Name = "Vanlig tomatsaus",
                PriceSmall = 15,
                PriceMedium = 20,
                PriceLarge = 25,
                Type = sauceType,
                isActive = true
            });
            var organicTomatoSauce = db.Ingredient.Add(new Ingredient
            {
                Name = "Økologisk tomatsaus",
                PriceSmall = 20,
                PriceMedium = 25,
                PriceLarge = 30,
                Type = sauceType,
                isActive = true

            });

            var chilliSauce = db.Ingredient.Add(new Ingredient
            {
                Name = "Chilli saus",
                PriceSmall = 20,
                PriceMedium = 25,
                PriceLarge = 30,
                Type = sauceType,
                isActive = true

            });

            var meatType = db.IngredientType.Add(new IngredientType { TypeMax = 5, TypeName = "Kjøtt", TypeUnique = false, isActive = true });
            var hamMeat = db.Ingredient.Add(new Ingredient
            {
                Name = "Skinke",
                PriceSmall = 15,
                PriceMedium = 20,
                PriceLarge = 25,
                Type = meatType,
                isActive = true
            });
            var beaconMeat = db.Ingredient.Add(new Ingredient
            {
                Name = "Bacon",
                PriceSmall = 12,
                PriceMedium = 20,
                PriceLarge = 25,
                Type = meatType,
                isActive = true
            });
            var bitsMeat = db.Ingredient.Add(new Ingredient
            {
                Name = "Kjøttdeig",
                PriceSmall = 12,
                PriceMedium = 16,
                PriceLarge = 22,
                Type = meatType,
                isActive = true
            });
            var beefMeat = db.Ingredient.Add(new Ingredient
            {
                Name = "Biff",
                PriceSmall = 15,
                PriceMedium = 23,
                PriceLarge = 29,
                Type = meatType,
                isActive = true
            });
            var peperoniMeat = db.Ingredient.Add(new Ingredient
            {
                Name = "Pepperoni",
                PriceSmall = 13,
                PriceMedium = 20,
                PriceLarge = 26,
                Type = meatType,
                isActive = true
            });

            var vegieType = db.IngredientType.Add(new IngredientType { TypeMax = 5, TypeName = "Grønnsaker", TypeUnique = false, isActive = true });
            var pineVegie = db.Ingredient.Add(new Ingredient
            {
                Name = "Ananas",
                PriceSmall = 13,
                PriceMedium = 18,
                PriceLarge = 23,
                Type = vegieType,
                isActive = true
            });
            var paprikaVegie = db.Ingredient.Add(new Ingredient
            {
                Name = "Paprika",
                PriceSmall = 10,
                PriceMedium = 14,
                PriceLarge = 19,
                Type = vegieType,
                isActive = true
            });
            var garlicVegie = db.Ingredient.Add(new Ingredient
            {
                Name = "Hvitløk",
                PriceSmall = 16,
                PriceMedium = 22,
                PriceLarge = 26,
                Type = vegieType,
                isActive = true
            });
            var onionVegie = db.Ingredient.Add(new Ingredient
            {
                Name = "Løk",
                PriceSmall = 14,
                PriceMedium = 18,
                PriceLarge = 24,
                Type = vegieType,
                isActive = true
            });
            var spinachVegie = db.Ingredient.Add(new Ingredient
            {
                Name = "Spinat",
                PriceSmall = 11,
                PriceMedium = 17,
                PriceLarge = 24,
                Type = vegieType,
                isActive = true
            });
            var shroomVegie = db.Ingredient.Add(new Ingredient
            {
                Name = "Sjampinjong",
                PriceSmall = 13,
                PriceMedium = 19,
                PriceLarge = 24,
                Type = vegieType,
                isActive = true
            });

            // Extras
            var drinksType = db.IngredientType.Add(new IngredientType { TypeMax = 5, TypeName = "Drikke", TypeUnique = false, TypeExtras = true, isActive = true });
            var cokeDrink = db.Ingredient.Add(new Ingredient
            {
                Name = "Coca Cola (1,5l)",
                PriceSmall = 25,
                PriceMedium = 25,
                PriceLarge = 25,
                Type = drinksType,
                isActive = true,
            });
            var spriteDrink = db.Ingredient.Add(new Ingredient
            {
                Name = "Sprite (1,5l)",
                PriceSmall = 25,
                PriceMedium = 25,
                PriceLarge = 25,
                Type = drinksType,
                isActive = true,
            });
            var fantaDrink = db.Ingredient.Add(new Ingredient
            {
                Name = "Fanta (1,5l)",
                PriceSmall = 25,
                PriceMedium = 25,
                PriceLarge = 25,
                Type = drinksType,
                isActive = true,
            });
            var farrisDrink = db.Ingredient.Add(new Ingredient
            {
                Name = "Farris (1,5l)",
                PriceSmall = 25,
                PriceMedium = 25,
                PriceLarge = 25,
                Type = drinksType,
                isActive = true,
            });

            var dressingType = db.IngredientType.Add(new IngredientType { TypeMax = 3, TypeName = "Dressing", TypeUnique = false, TypeExtras = true, isActive = true });
            var sourCreamDressing = db.Ingredient.Add(new Ingredient
            {
                Name = "Rømmedressing",
                PriceSmall = 15,
                PriceMedium = 15,
                PriceLarge = 15,
                Type = dressingType,
                isActive = true,
            });
            var garlicDressing = db.Ingredient.Add(new Ingredient
            {
                Name = "Hvitløksdressing",
                PriceSmall = 20,
                PriceMedium = 20,
                PriceLarge = 20,
                Type = dressingType,
                isActive = true,
            });
            var chilidDressing = db.Ingredient.Add(new Ingredient
            {
                Name = "Chilidressing",
                PriceSmall = 25,
                PriceMedium = 25,
                PriceLarge = 25,
                Type = dressingType,
                isActive = true,
            });

            // Pizzas
            var boringPizza = db.Pizza.Add(new Pizza
            {
                Name = "Kjedelig pizza",
                PriceSmall = 79,
                PriceMedium = 99,
                PriceLarge = 139,
                isActive = true,
            });
            db.PizzaIngredient.Add(new PizzaIngredient { Pizza=boringPizza, Ingredient = wholeBase, Count = 1 });
            db.PizzaIngredient.Add(new PizzaIngredient { Pizza=boringPizza, Ingredient = mozarellaCheese, Count = 1 });
            db.PizzaIngredient.Add(new PizzaIngredient { Pizza = boringPizza, Ingredient = tomatoSauce, Count = 1 });

            var meatPizza = db.Pizza.Add(new Pizza
            {
                Name = "Meat-Lover",
                PriceSmall = 99,
                PriceMedium = 139,
                PriceLarge = 179,
                isActive = true,
            });

            db.PizzaIngredient.Add(new PizzaIngredient { Pizza = meatPizza, Ingredient = normalBase, Count = 1 });
            db.PizzaIngredient.Add(new PizzaIngredient { Pizza = meatPizza, Ingredient = jarlsbergCheese, Count = 1 });
            db.PizzaIngredient.Add(new PizzaIngredient { Pizza = meatPizza, Ingredient = tomatoSauce, Count = 1 });
            db.PizzaIngredient.Add(new PizzaIngredient { Pizza = meatPizza, Ingredient = beaconMeat, Count = 1 });
            db.PizzaIngredient.Add(new PizzaIngredient { Pizza = meatPizza, Ingredient = hamMeat, Count = 1 });
            db.PizzaIngredient.Add(new PizzaIngredient { Pizza = meatPizza, Ingredient = bitsMeat, Count = 1 });

            var vegiePizza = db.Pizza.Add(new Pizza
            {
                Name = "Planteeter",
                PriceSmall = 89,
                PriceMedium = 129,
                PriceLarge = 159,
                isActive = true,
            });

            db.PizzaIngredient.Add(new PizzaIngredient { Pizza = vegiePizza, Ingredient = wholeBase, Count = 1 });
            db.PizzaIngredient.Add(new PizzaIngredient { Pizza = vegiePizza, Ingredient = mozarellaCheese, Count = 1 });
            db.PizzaIngredient.Add(new PizzaIngredient { Pizza = vegiePizza, Ingredient = tomatoSauce, Count = 1 });
            db.PizzaIngredient.Add(new PizzaIngredient { Pizza = vegiePizza, Ingredient = pineVegie, Count = 1 });
            db.PizzaIngredient.Add(new PizzaIngredient { Pizza = vegiePizza, Ingredient = paprikaVegie, Count = 1 });
            db.PizzaIngredient.Add(new PizzaIngredient { Pizza = vegiePizza, Ingredient = spinachVegie, Count = 1 });
            db.PizzaIngredient.Add(new PizzaIngredient { Pizza = vegiePizza, Ingredient = shroomVegie, Count = 1 });

            var spicyPizza = db.Pizza.Add(new Pizza
            {
                Name = "Den Hotte",
                PriceSmall = 99,
                PriceMedium = 139,
                PriceLarge = 169,
                isActive = true,
            });

            db.PizzaIngredient.Add(new PizzaIngredient { Pizza = spicyPizza, Ingredient = wholeBase, Count = 1 });
            db.PizzaIngredient.Add(new PizzaIngredient { Pizza = spicyPizza, Ingredient = mozarellaCheese, Count = 1 });
            db.PizzaIngredient.Add(new PizzaIngredient { Pizza = spicyPizza, Ingredient = chilliSauce, Count = 1 });
            db.PizzaIngredient.Add(new PizzaIngredient { Pizza = spicyPizza, Ingredient = peperoniMeat, Count = 1 });
            db.PizzaIngredient.Add(new PizzaIngredient { Pizza = spicyPizza, Ingredient = paprikaVegie, Count = 1 });
            db.PizzaIngredient.Add(new PizzaIngredient { Pizza = spicyPizza, Ingredient = onionVegie, Count = 1 });
            db.PizzaIngredient.Add(new PizzaIngredient { Pizza = spicyPizza, Ingredient = garlicVegie, Count = 1 });

            var mixPizza = db.Pizza.Add(new Pizza
            {
                Name = "Mixen",
                PriceSmall = 99,
                PriceMedium = 119,
                PriceLarge = 159,
                isActive = true,
            });

            db.PizzaIngredient.Add(new PizzaIngredient { Pizza = mixPizza, Ingredient = wholeBase, Count = 1 });
            db.PizzaIngredient.Add(new PizzaIngredient { Pizza = mixPizza, Ingredient = cheddarCheese, Count = 1 });
            db.PizzaIngredient.Add(new PizzaIngredient { Pizza = mixPizza, Ingredient = chilliSauce, Count = 1 });
            db.PizzaIngredient.Add(new PizzaIngredient { Pizza = mixPizza, Ingredient = hamMeat, Count = 1 });
            db.PizzaIngredient.Add(new PizzaIngredient { Pizza = mixPizza, Ingredient = shroomVegie, Count = 1 });
            db.PizzaIngredient.Add(new PizzaIngredient { Pizza = mixPizza, Ingredient = beefMeat, Count = 1 });

            // Test orders
            db.Order.Add(new Order
            {
                CustomerName = "Test Testesen",
                Phone = "87654321",

                Deliver = true,
                Address = "Veien 2",
                City = "GRIMSTAD",
                ZipCode = "4879",
                
                DeliveryTime = DateTime.UtcNow.AddHours(1),
                Discount = 0,
                Price = 3 * boringPizza.PriceMedium, // Approximate pricing for test orders.

                State = "new",

                Lines = new List<OrderLine>
                {
                    boringPizza.ToOrderLine(PizzaSize.Medium, 3),
                },
            });
            db.Order.Add(new Order
            {
                CustomerName = "Henrik Henriksen",
                Phone = "34343434",

                Deliver = true,
                Address = "Neptunveien ",
                City = "ARENDAL",
                ZipCode = "4843",

                DeliveryTime = DateTime.UtcNow.AddDays(3),
                Discount = 0,
                Price = 3 * boringPizza.PriceMedium, // Approximate pricing for test orders.

                State = "preparation",

                Lines = new List<OrderLine>
                {
                    boringPizza.ToOrderLine(PizzaSize.Medium, 3),
                },
            });

            db.Order.Add(new Order
            {
                CustomerName = "Bekka Bekreftelsen",
                Phone = "22334455",

                Deliver = true,
                Address = "Frivoldveien 21",
                City = "GRIMSTAD",
                ZipCode = "4879",

                DeliveryTime = DateTime.UtcNow.AddDays(1),
                Discount = 0,
                IsPaid = true,
                Price = 2 * vegiePizza.PriceLarge, // Approximate pricing for test orders.

                State = "confirmed",

                Lines = new List<OrderLine>
                {
                    vegiePizza.ToOrderLine(PizzaSize.Large, 2),
                },
            });

            db.Order.Add(new Order
            {
                CustomerName = "Per Preppe",
                Phone = "12345678",

                Deliver = false,

                DeliveryTime = DateTime.UtcNow.AddHours(2),
                Discount = 25,
                IsPaid = true,
                Price = meatPizza.PriceLarge + 2 * boringPizza.PriceSmall, // Approximate pricing for test orders.

                State = "preparation",

                Lines = new List<OrderLine>
                {
                    meatPizza.ToOrderLine(PizzaSize.Large),
                    boringPizza.ToOrderLine(PizzaSize.Small, 2)
                },
            });

            db.Order.Add(new Order
            {
                CustomerName = "Baard Bake",
                Phone = "33454321",

                Deliver = false,

                DeliveryTime = DateTime.UtcNow.AddHours(1),
                Discount = 0,
                IsPaid = true,
                Price = 4 * vegiePizza.PriceLarge + 2 * mixPizza.PriceSmall, // Approximate pricing for test orders.

                State = "preparation",

                Lines = new List<OrderLine>
                {
                    vegiePizza.ToOrderLine(PizzaSize.Large, 4),
                    mixPizza.ToOrderLine(PizzaSize.Small, 2)
                },
            });

            db.Order.Add(new Order
            {
                CustomerName = "Ove Ovne",
                Phone = "33445566",

                Deliver = false,

                DeliveryTime = DateTime.UtcNow.AddHours(4),
                Discount = 0,
                IsPaid = true,
                Price = mixPizza.PriceMedium + 2 * boringPizza.PriceLarge, // Approximate pricing for test orders.

                State = "oven",

                Lines = new List<OrderLine>
                {
                    mixPizza.ToOrderLine(PizzaSize.Medium),
                    boringPizza.ToOrderLine(PizzaSize.Large, 2)
                },
            });

            db.Order.Add(new Order
            {
                CustomerName = "Stein Steike",
                Phone = "44553311",

                Deliver = false,

                DeliveryTime = DateTime.UtcNow.AddHours(2),
                Discount = 0,
                Price = meatPizza.PriceLarge + 3 * boringPizza.PriceSmall, // Approximate pricing for test orders.

                State = "oven",
                IsPaid = true,

                Lines = new List<OrderLine>
                {
                    meatPizza.ToOrderLine(PizzaSize.Large),
                    boringPizza.ToOrderLine(PizzaSize.Small, 2)
                },
            });

            db.Order.Add(new Order
            {
                CustomerName = "Lemmy Leversen",
                Phone = "33445566",

                Deliver = true,
                Address = "Storgaten 3",
                City = "GRIMSTAD",
                ZipCode = "4879",

                DeliveryTime = DateTime.UtcNow.AddHours(4),
                Discount = 0,
                Price = mixPizza.PriceMedium + 4 * meatPizza.PriceLarge, // Approximate pricing for test orders.

                State = "delivery",
                IsPaid = true,

                Lines = new List<OrderLine>
                {
                    mixPizza.ToOrderLine(PizzaSize.Medium),
                    meatPizza.ToOrderLine(PizzaSize.Large, 4)
                },
            });

            db.Order.Add(new Order
            {
                CustomerName = "Hans Hente",
                Phone = "99887766",

                Deliver = false,

                DeliveryTime = DateTime.UtcNow.AddHours(4),
                Discount = 0,
                Price = vegiePizza.PriceMedium, // Approximate pricing for test orders.

                State = "ready",
                IsPaid = true,

                Lines = new List<OrderLine>
                {
                    vegiePizza.ToOrderLine(PizzaSize.Medium)
                },
            });

            // Contact info

            var defaultContact = db.Contact.Add(new Contact
            {
                Phone = "22334455", 
                Email = "admin@uia.no",
                Address = "Storgaten 5",
                ZipCode = 4876,
                City = "Grimstad",
                FixedPriceDelivery = 49,

                StripePrivateKey = "sk_test_OHfoC78OCPGx96GoDYOIcV3z",
                StripePublicKey = "pk_test_k3vqOZV1iq43SPUrRvYZVDqR",
                TwilioAccountSid = "AC322d2bb833ee5c705850689cf5527179",
                TwilioAuthToken = "15dfdd875e6cfc90cc3c6ef88b18b0d1"
            });

            var monday = db.Hours.Add(new Hours
            {
                Day = "Mandag",
                OpeningHours = new TimeSpan(12, 00, 00),
                ClosingHours = new TimeSpan(22, 30, 00),
                DeliveryStart = new TimeSpan(14, 30, 00),
                DeliveryEnd = new TimeSpan(22, 00, 00)
            });
            var tuesday = db.Hours.Add(new Hours
            {
                Day = "Tirsdag",
                OpeningHours = new TimeSpan(12, 00, 00),
                ClosingHours = new TimeSpan(22, 30, 00),
                DeliveryStart = new TimeSpan(14, 30, 00),
                DeliveryEnd = new TimeSpan(22, 00, 00)
            });
            var wednesday = db.Hours.Add(new Hours
            {
                Day = "Onsdag",
                OpeningHours = new TimeSpan(12, 00, 00),
                ClosingHours = new TimeSpan(22, 30, 00),
                DeliveryStart = new TimeSpan(14, 30, 00),
                DeliveryEnd = new TimeSpan(22, 00, 00)
            });
            var thursday = db.Hours.Add(new Hours
            {
                Day = "Torsdag",
                OpeningHours = new TimeSpan(12, 00, 00),
                ClosingHours = new TimeSpan(22, 30, 00),
                DeliveryStart = new TimeSpan(14, 30, 00),
                DeliveryEnd = new TimeSpan(22, 00, 00)
            });
            var friday = db.Hours.Add(new Hours
            {
                Day = "Fredag",
                OpeningHours = new TimeSpan(12, 00, 00),
                ClosingHours = new TimeSpan(22, 30, 00),
                DeliveryStart = new TimeSpan(14, 30, 00),
                DeliveryEnd = new TimeSpan(22, 00, 00)
            });
            var saturday = db.Hours.Add(new Hours
            {
                Day = "Lørdag",
                OpeningHours = new TimeSpan(12, 00, 00),
                ClosingHours = new TimeSpan(22, 30, 00),
                DeliveryStart = new TimeSpan(14, 30, 00),
                DeliveryEnd = new TimeSpan(22, 00, 00)
            });
            var sunday = db.Hours.Add(new Hours
            {
                Day = "Søndag",
                OpeningHours = new TimeSpan(00, 00, 00),
                ClosingHours = new TimeSpan(00, 00, 00),
                DeliveryStart = new TimeSpan(00, 00, 00),
                DeliveryEnd = new TimeSpan(00, 00, 00)
            });

            db.SaveChanges();
        }
    }
}
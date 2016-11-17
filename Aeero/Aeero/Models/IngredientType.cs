using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Aeero.Models
{
    [Table("IngredientType")]
    public class IngredientType
    {
        [Key]
        [Required]
        public virtual int TypeId { get; set; }

        [Required]
        [DisplayName("Ingredienstype")]
        public virtual string TypeName { get; set; }

        [Required]
        [DisplayName("Maks antall")]
        public virtual int TypeMax { get; set; }

        [Required]
        [DisplayName("Unik")]
        public virtual bool TypeUnique { get; set; }

        [Required]
        [DisplayName("Tilbehør")]
        public virtual bool TypeExtras { get; set; }

        [DisplayName("Aktiv")]
        public virtual bool isActive { get; set; }

        public virtual List<Ingredient> Ingredients { get; set; }
    }
}
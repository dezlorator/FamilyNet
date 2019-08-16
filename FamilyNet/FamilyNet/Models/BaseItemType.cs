using FamilyNet.Models.Interfaces;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace FamilyNet.Models
{
    public class BaseItemType : IEntity
    {
        public int ID { get; set; }

        [Required]
        [Display(Name = "Название")]
        public string Name { get; set; }

        [Display(Name = "Родитель")]
        public virtual BaseItemType Parent { get; set; }

        [BindNever]
        public bool IsDeleted { get; set; } = false;

        [Display(Name = "Потребности")]
        public virtual ICollection<TypeBaseItem> TypeBaseItem { get; set; }
    }
}
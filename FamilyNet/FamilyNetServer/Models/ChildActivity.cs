using FamilyNetServer.Models.Interfaces;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace FamilyNetServer.Models
{
    public class ChildActivity : IEntity
    {
        public int ID { get; set; }
        [Display(Name = "Название")]
        public string Name { get; set; }
        [Display(Name = "Описание")]
        public string Description { get; set; }
        public virtual ICollection<Award> Awards { get; set; }
        public virtual Orphan Child { get; set; }
        [BindNever]
        public bool IsDeleted { get; set; } = false;
    }
}

using FamilyNetServer.Models.Interfaces;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace FamilyNetServer.Models
{
    public class FullName
    {
        [Display(Name = "Имя")]
        public string Name { get; set; }

        [Display(Name = "Фамилия")]
        public string Surname { get; set; }

        [Display(Name = "Отчество")]
        public string Patronymic { get; set; }

        public virtual void CopyState(FullName sender)
        {
            Name = sender.Name;
            Surname = sender.Surname;
            Patronymic = sender.Patronymic;
        }
    }
}

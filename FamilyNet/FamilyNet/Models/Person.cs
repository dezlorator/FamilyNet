using FamilyNet.Models.Interfaces;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace FamilyNet.Models
{
    public class Person : IEntity
    {
        public int ID { get; set; }
        //[Required(ErrorMessage = "Please enter Full Name")]
        public virtual FullName FullName { get; set; }
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        [Display(Name = "День рождения")]
        public virtual DateTime Birthday { get; set; }
        [Display(Name = "Рейтинг")]
        public float Rating { get; set; }
        [Display(Name = "Автар")]
        public string Avatar { get; set; }
        public int EmailID { get; set; }


        public virtual void CopyState(Person sender)
        {
            FullName.Name = sender.FullName.Name;
            FullName.Surname = sender.FullName.Surname;
            FullName.Patronymic = sender.FullName.Patronymic;

            Birthday = sender.Birthday;

        }
    }

}

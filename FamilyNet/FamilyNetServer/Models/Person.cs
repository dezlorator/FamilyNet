using FamilyNetServer.Models.Interfaces;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System;
using System.ComponentModel.DataAnnotations;
namespace FamilyNetServer.Models
{
    public class Person : IEntity, IAvatar
    {
        public int ID { get; set; }

        //TODO: AlPa -> customer Attr AllowSorting
        [Display(Name = "Полное имя")]
        public virtual FullName FullName { get; set; }

        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        [Display(Name = "День рождения")]
        public virtual DateTime Birthday { get; set; }

        [Display(Name = "Рейтинг")]
        public float Rating { get; set; }

        [Display(Name = "Аватар")]
        public string Avatar { get; set; }

        public int EmailID { get; set; }


        [BindNever]
        public bool IsDeleted { get; set; } = false;

        public virtual void CopyState(Person sender)
        {
            FullName.CopyState(sender.FullName);
            Birthday = sender.Birthday;
            Rating = sender.Rating;
            if(sender.Avatar != string.Empty && sender.Avatar != null)
            {
                Avatar = sender.Avatar;
            }
        }
    }

}

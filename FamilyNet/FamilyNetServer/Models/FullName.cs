using System.ComponentModel.DataAnnotations;


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

        public override string ToString()
        {
            return Name + " " + Patronymic + " " + Surname;
        }
    }
}

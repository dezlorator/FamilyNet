using System.ComponentModel.DataAnnotations;

namespace FamilyNet.Models.AddressCatalog {
    public interface ICatalogAddress {
        [Required(ErrorMessage = "Пожалуйста введите страну")]
        [Display(Name = "Cтрана")]
        string Country { get; set; }

        [Required(ErrorMessage = "Пожалуйста введите название области")]
        [Display(Name = "Область")]
        string Region { get; set; }

        [Required(ErrorMessage = "Пожалуйста введите название города")]
        [Display(Name = "Город")]
        string City { get; set; }

        [Required(ErrorMessage = "Пожалуйста введите название улицы")]
        [Display(Name = "Улица")]
        string Street { get; set; }

        [Required(ErrorMessage = "Пожалуйста введите номер дома")]
        [Display(Name = "Дом")]
        string House { get; set; }
    }
}



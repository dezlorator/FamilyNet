using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;
using FamilyNet.Models;

namespace FamilyNet.Models.ViewModels
{
    public class SortOrphanagesViewModel : Orphanage
    {
        public SortStateOrphanages NameSort { get; set; }

        public SortStateOrphanages AdressSort { get; set; }

        public SortStateOrphanages RatingSort { get; set; }

        public SortStateOrphanages Current { get; set; }

        public bool IsDescending { get; set; }

        public SortOrphanagesViewModel(SortStateOrphanages sortItem)
        {
            NameSort = SortStateOrphanages.NameAsc;
            AdressSort = SortStateOrphanages.AddressAsc;
            RatingSort = SortStateOrphanages.RatingAsc;
            IsDescending = true;

            if (sortItem == SortStateOrphanages.AddressDesc || sortItem == SortStateOrphanages.NameDesc
                || sortItem == SortStateOrphanages.RatingDesc)
            {
                IsDescending = false;
            }

            switch (sortItem)
            {
                case SortStateOrphanages.NameDesc:
                    Current = NameSort = SortStateOrphanages.NameAsc;
                    break;
                case SortStateOrphanages.AddressAsc:
                    Current = AdressSort = SortStateOrphanages.AddressDesc;
                    break;
                case SortStateOrphanages.AddressDesc:
                    Current = AdressSort = SortStateOrphanages.AddressAsc;
                    break;
                case SortStateOrphanages.RatingAsc:
                    Current = RatingSort = SortStateOrphanages.RatingDesc;
                    break;
                case SortStateOrphanages.RatingDesc:
                    Current = RatingSort = SortStateOrphanages.RatingAsc;
                    break;
                default:
                    Current = NameSort = SortStateOrphanages.NameDesc;
                    break;
            }
        }
    }
}

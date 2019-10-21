using System;
using DataTransferObjects;

namespace FamilyNetServer.Validators
{
    public class CategoryValidator : ICategoryValidator
    {
        public bool IsValid(CategoryDTO categoryDTO)
        {
            return categoryDTO.Name != String.Empty && categoryDTO.Name.Length > 3;
        }
    }
}

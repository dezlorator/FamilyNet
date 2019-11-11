using System;
using DataTransferObjects;

namespace FamilyNetServer.Validators
{
    public class CategoryValidator : IValidator<CategoryDTO>
    {
        public bool IsValid(CategoryDTO categoryDTO)
        {
            return categoryDTO.Name != String.Empty && categoryDTO.Name.Length > 3;
        }
    }
}

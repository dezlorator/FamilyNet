using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FamilyNetServer.DTO;

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

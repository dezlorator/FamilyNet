using FamilyNetServer.DTO;
using System;

namespace FamilyNetServer.Validators
{
    public interface ICategoryValidator
    {
        bool IsValid(CategoryDTO categoryDTO);
    }
}

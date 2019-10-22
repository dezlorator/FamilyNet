using DataTransferObjects;
using System;

namespace FamilyNetServer.Validators
{
    public interface ICategoryValidator
    {
        bool IsValid(CategoryDTO categoryDTO);
    }
}

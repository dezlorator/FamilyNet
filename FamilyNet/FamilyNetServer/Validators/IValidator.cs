using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FamilyNetServer.Validators
{
    public interface IValidator<T> where T: class
    {
        bool IsValid(T obfjDTO);
    }
}

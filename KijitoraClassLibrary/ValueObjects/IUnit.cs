using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KijitoraClassLibrary.ValueObjects
{
    public interface IUnit<T>
    {
        T Convert(T valueObject);
    }
}

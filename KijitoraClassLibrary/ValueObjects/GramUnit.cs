using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KijitoraClassLibrary.ValueObjects
{
    public class GramUnit : IUnit<Weight>
    {
        public Weight Convert(Weight weight)
        {
            if (weight == null)
            {
                throw new ArgumentNullException();
            }
            double value = weight.Value;
            switch (weight.Unit)
            {
                case KilogramUnit _:
                    value *= 1000;
                    break;
                case MilligramUnit _:
                    value /= 1000;
                    break;
            }
            return new Weight(value, new GramUnit());
        }

        public override string ToString()
        {
            return "g";
        }
    }
}

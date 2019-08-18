using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KijitoraClassLibrary.ValueObjects
{
    public class MilligramUnit : IUnit<Weight>
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
                    value *= 1000000;
                    break;
                case GramUnit _:
                    value *= 1000;
                    break;
            }
            return new Weight(value, new MilligramUnit());
        }

        public override string ToString()
        {
            return "mg";
        }
    }
}

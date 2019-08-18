using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KijitoraClassLibrary.ValueObjects
{
    public class KilogramUnit : IUnit<Weight>
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
                case GramUnit _:
                    value /= 1000;
                    break;
                case MilligramUnit _:
                    value /= 1000000;
                    break;
            }
            return new Weight(value, new KilogramUnit());
        }

        public override string ToString()
        {
            return "kg";
        }
    }
}

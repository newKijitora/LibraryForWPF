using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KijitoraClassLibrary.ValueObjects
{
    public class CentimeterUnit : IUnit<Length>
    {
        public Length Convert(Length length)
        {
            if (length == null)
            {
                throw new ArgumentNullException();
            }
            double value = length.Value;
            switch (length.Unit)
            {
                case MeterUnit _:
                    value *= 100;
                    break;
                case MillimeterUnit _:
                    value /= 10;
                    break;
            }
            return new Length(value, new CentimeterUnit());
        }

        public override string ToString()
        {
            return "cm";
        }
    }
}

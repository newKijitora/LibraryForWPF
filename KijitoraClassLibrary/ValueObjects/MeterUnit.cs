using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KijitoraClassLibrary.ValueObjects
{
    public class MeterUnit : IUnit<Length>
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
                case CentimeterUnit _:
                    value /= 100;
                    break;
                case MillimeterUnit _:
                    value /= 1000;
                    break;
            }
            return new Length(value, new MeterUnit());
        }

        public override string ToString()
        {
            return "m";
        }
    }
}

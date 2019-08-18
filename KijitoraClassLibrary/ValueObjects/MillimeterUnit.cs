using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KijitoraClassLibrary.ValueObjects
{
    public class MillimeterUnit : IUnit<Length>
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
                    value *= 1000;
                    break;
                case CentimeterUnit _:
                    value *= 10;
                    break;
            }
            return new Length(value, new MillimeterUnit());
        }

        public override string ToString()
        {
            return "mm";
        }
    }
}

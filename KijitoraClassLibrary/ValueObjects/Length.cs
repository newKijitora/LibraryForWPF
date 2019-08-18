using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KijitoraClassLibrary.ValueObjects
{
    public class Length
    {
        public double Value { get; }
        public IUnit<Length> Unit { get; }

        public Length(double value, IUnit<Length> unit)
        {
            if (unit == null)
            {
                throw new ArgumentNullException();
            }
            Value = value;
            Unit = unit;
        }

        public override bool Equals(object obj)
        {
            if (obj is Length otherLength)
            {
                double canCompareValue = otherLength.Convert(this.Unit).Value;
                return Equals(Value, canCompareValue);
            }
            return false;
        }

        public override int GetHashCode()
        {
            return Value.GetHashCode();
        }

        public Length Convert(IUnit<Length> unit)
        {
            if (unit == null)
            {
                throw new ArgumentNullException();
            }
            return unit.Convert(this);
        }

        public override string ToString()
        {
            var lengthWithUnit = new StringBuilder(Value.ToString()).Append(Unit).ToString();
            return lengthWithUnit;
        }
    }
}

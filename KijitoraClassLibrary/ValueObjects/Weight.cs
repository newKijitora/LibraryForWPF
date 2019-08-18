using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KijitoraClassLibrary.ValueObjects
{
    public class Weight
    {
        public double Value { get; }
        public IUnit<Weight> Unit { get; }

        public Weight(double value, IUnit<Weight> unit)
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
            if (obj is Weight otherWeight)
            {
                double canCompareValue = otherWeight.Convert(this.Unit).Value;
                return Equals(Value, canCompareValue);
            }
            return false;
        }

        public override int GetHashCode()
        {
            return Value.GetHashCode();
        }

        public Weight Convert(IUnit<Weight> unit)
        {
            if (unit == null)
            {
                throw new ArgumentNullException();
            }
            return unit.Convert(this);
        }

        public override string ToString()
        {
            var weightWithUnit = new StringBuilder(Value.ToString()).Append(Unit).ToString();
            return weightWithUnit;
        }
    }
}

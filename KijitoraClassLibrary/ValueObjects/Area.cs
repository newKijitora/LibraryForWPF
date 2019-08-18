using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KijitoraClassLibrary.ValueObjects
{
    public class Area
    {
        public Length Width { get; set; }
        public Length Height { get; set; }

        public Area(double width, double height, IUnit<Length> widthUnit, IUnit<Length> heightUnit)
        {
            Width = new Length(width, widthUnit);
            Height = new Length(height, heightUnit);
        }

        public Area(double width, double height, AreaUnit areaUnit)
        {
            Width = new Length(width, areaUnit.BaseUnit);
            Height = new Length(height, areaUnit.BaseUnit);
        }

        public Area Convert(IUnit<Length> widthUnit, IUnit<Length> heightUnit)
        {
            var width = widthUnit.Convert(Width);
            var height = heightUnit.Convert(Height);
            return new Area(width.Value, height.Value, widthUnit, heightUnit);
        }

        public Area Convert(AreaUnit areaUnit)
        {
            var width = areaUnit.BaseUnit.Convert(Width);
            var height = areaUnit.BaseUnit.Convert(Height);
            return new Area(width.Value, height.Value, areaUnit);
        }

        public double Calculate(AreaUnit areaUnit)
        {
            var area = areaUnit.Convert(this);
            return area.Width.Value * area.Height.Value;
        }
    }
}

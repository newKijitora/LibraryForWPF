using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KijitoraClassLibrary.ValueObjects
{
    /// <summary>
    /// 体積を表します。
    /// </summary>
    public class Volume
    {
        public Length Width { get; set; }
        public Length Height { get; set; }
        public Length Depth { get; set; }

        public Volume(double width, double height, double depth, IUnit<Length> widthUnit, IUnit<Length> heightUnit, IUnit<Length> depthUnit)
        {
            Width = new Length(width, widthUnit);
            Height = new Length(height, heightUnit);
            Depth = new Length(depth, depthUnit);
        }

        public Volume(double width, double height, double depth, VolumeUnit volumeUnit)
        {
            Width = new Length(width, volumeUnit.BaseUnit);
            Height = new Length(height, volumeUnit.BaseUnit);
            Depth = new Length(depth, volumeUnit.BaseUnit);
        }

        public Volume Convert(IUnit<Length> widthUnit, IUnit<Length> heightUnit, IUnit<Length> depthUnit)
        {
            var width = widthUnit.Convert(Width);
            var height = heightUnit.Convert(Height);
            var depth = depthUnit.Convert(Depth);
            return new Volume(width.Value, height.Value, depth.Value, widthUnit, heightUnit, depthUnit);
        }

        public Volume Convert(VolumeUnit volumeUnit)
        {
            var width = volumeUnit.BaseUnit.Convert(Width);
            var height = volumeUnit.BaseUnit.Convert(Height);
            var depth = volumeUnit.BaseUnit.Convert(Depth);
            return new Volume(width.Value, height.Value, depth.Value, volumeUnit);
        }

        public double Calculate(VolumeUnit volumeUnit)
        {
            var volume = volumeUnit.Convert(this);
            return volume.Width.Value * volume.Height.Value * volume.Depth.Value;
        }
    }
}

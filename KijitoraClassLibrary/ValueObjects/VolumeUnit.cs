using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KijitoraClassLibrary.ValueObjects
{
    /// <summary>
    /// 体積の単位を表します。
    /// </summary>
    public class VolumeUnit
    {
        public IUnit<Length> BaseUnit { get; set; }

        public VolumeUnit(IUnit<Length> baseUnit)
        {
            BaseUnit = baseUnit;
        }

        public Volume Convert(Volume volume)
        {
            var width = BaseUnit.Convert(volume.Width);
            var height = BaseUnit.Convert(volume.Height);
            var depth = BaseUnit.Convert(volume.Depth);
            return new Volume(width.Value, height.Value, depth.Value, this);
        }
    }
}

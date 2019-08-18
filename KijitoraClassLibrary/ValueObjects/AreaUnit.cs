using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KijitoraClassLibrary.ValueObjects
{
    /// <summary>
    /// 面積の単位を表します。
    /// </summary>
    public class AreaUnit
    {
        public IUnit<Length> BaseUnit { get; set; }

        public AreaUnit(IUnit<Length> baseUnit)
        {
            BaseUnit = baseUnit;
        }

        public Area Convert(Area area)
        {
            var width = BaseUnit.Convert(area.Width);
            var height = BaseUnit.Convert(area.Height);
            return new Area(width.Value, height.Value, this);
        }
    }
}

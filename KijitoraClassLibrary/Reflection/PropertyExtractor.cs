using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace KijitoraClassLibrary.Reflection
{
    /// <summary>
    /// オブジェクトの指定された属性を持つプロパティ値の列挙をサポートします。
    /// </summary>
    public static class PropertyExtractor
    {
        /// <summary>
        /// オブジェクトの指定された属性を持つプロパティ値を列挙します。
        /// </summary>
        public static IEnumerable<PropertyInfo> ExtractProperties(this object obj, Type attributeType)
        {
            var errorMessage = "メソッドに渡される引数は属性でなければいけません。";

            if (attributeType.BaseType != typeof(Attribute))
            {
                throw new ArgumentException(errorMessage);
            }

            PropertyInfo[] propertyInfos = obj.GetType().GetProperties();

            foreach (var propertyInfo in propertyInfos)
            {
                var hasAttribute = propertyInfo.GetCustomAttributes(attributeType, false).Length > 0;
                if (hasAttribute)
                {
                    yield return propertyInfo;
                }
            }
        }

        /// <summary>
        /// オブジェクトの指定された名前を持つプロパティ値を列挙します。
        /// </summary>
        public static IEnumerable<PropertyInfo> ExtractProperties(this object obj, params string[] names)
        {
            var errorMessage = "配列が空です。";

            if (names.Length == 0)
            {
                throw new ArgumentException(errorMessage);
            }

            PropertyInfo[] propInfos = obj.GetType().GetProperties();

            foreach (var name in names)
            {
                foreach (var propInfo in propInfos)
                {
                    if (Equals(propInfo.Name, name))
                    {
                        yield return propInfo;
                        break;
                    }
                }
            }
        }
    }
}

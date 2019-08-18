using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KijitoraClassLibrary.ForWpf
{
    public static class ExtensionMethods
    {
        public static void AddRange<T>(this ObservableCollection<T> collection, IEnumerable<T> enumerable)
        {
            foreach (T item in enumerable)
            {
                collection.Add(item);
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AvevaUtility
{
    public class ItemArrayComparer : IEqualityComparer<object>
    {
        public new bool Equals(object x, object y)
        {
            if (string.Compare(x.ToString().ToLower(), y.ToString().ToLower()) != 0) return false;
            
            return true;
        }

        public int GetHashCode([DisallowNull] object obj)
        {
           return obj.GetHashCode();
        }
    }
}

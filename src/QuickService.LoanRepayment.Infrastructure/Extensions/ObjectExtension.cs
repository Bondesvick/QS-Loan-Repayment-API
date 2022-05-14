using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace QuickService.LoanRepayment.Infrastructure.Extensions
{
    public static class ObjectExtension
    {
        public static void InjectFrom<T1, T2>(this T1 destination, T2 source)
        {
            if (destination is null)
                throw new ArgumentNullException(nameof(destination));

            if (source is null)
                throw new ArgumentNullException(nameof(source));

            var sourceProperties = source.GetType().GetProperties();
            var destinationProperties = destination.GetType().GetProperties();

            // hash up sourceProperties to get O(n) time O(n) extra space to sore reference.
            Dictionary<string, PropertyInfo> namePropMap = new Dictionary<string, PropertyInfo>();

            foreach (PropertyInfo propInfo in sourceProperties)
                namePropMap[propInfo.Name] = propInfo;

            //O(n) worst case time.
            foreach (PropertyInfo destPropInfo in destinationProperties)
                if (namePropMap.TryGetValue(destPropInfo.Name, out PropertyInfo sourcePropInfo))
                    if (sourcePropInfo.PropertyType == destPropInfo.PropertyType)
                        destPropInfo.SetValue(destination, sourcePropInfo.GetValue(source));
        }

    }
}

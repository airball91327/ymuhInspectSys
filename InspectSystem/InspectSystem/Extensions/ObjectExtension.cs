using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using System.Web;

namespace InspectSystem.Extensions
{
    public static class ObjectExtension
    {
        public static T GetAttributeFrom<T>(this object instance, string propertyName) where T : Attribute
        {
            var attributeType = typeof(T);
            var property = instance.GetType().GetProperty(propertyName);
            if (property == null) return null;
            return (T)property.GetCustomAttributes(attributeType, false).FirstOrDefault();
        }

        /// <summary>
        /// Compare to models, and get the modified fields and values.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj1">Origin Object.</param>
        /// <param name="obj2">Modified Object.</param>
        /// <returns></returns>
        public static IEnumerable<string> EnumeratePropertyDifferences<T>(this T obj1, T obj2)
        {
            PropertyInfo[] properties = typeof(T).GetProperties();
            List<string> changes = new List<string>();

            foreach (PropertyInfo pi in properties)
            {
                object value1 = typeof(T).GetProperty(pi.Name).GetValue(obj1, null);
                object value2 = typeof(T).GetProperty(pi.Name).GetValue(obj2, null);
                DisplayAttribute displayObj = obj1.GetAttributeFrom<DisplayAttribute>(pi.Name);
                string displayName = pi.Name;
                if (displayObj != null)
                {
                    displayName = displayObj.Name;
                }

                if (value1 != value2 && (value1 == null || !value1.Equals(value2)))
                {
                    //changes.Add(string.Format("Property {0} changed from {1} to {2}", pi.Name, value1, value2));
                    changes.Add(string.Format("欄位 {0} 變更，從 {1} 改為 {2}", displayName, value1, value2));
                }
            }
            return changes;
        }
    }
}
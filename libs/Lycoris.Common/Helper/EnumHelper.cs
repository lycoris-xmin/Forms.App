﻿using Lycoris.Common.Extensions;
using System.ComponentModel;
using System.Reflection;

namespace Lycoris.Common.Helper
{
    /// <summary>
    /// 
    /// </summary>
    public class EnumHelper
    {
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static int[]? GetEnumValues<T>() where T : struct
        {
            var fields = typeof(T).GetFields(BindingFlags.Public | BindingFlags.Static);
            var values = new int[fields.Length];

            for (int i = 0; i < fields.Length; i++)
            {
                values[i] = (int)(fields[i].GetValue(null))!;
            }

            return values;
        }

        /// <summary>
        /// 核验枚举值是否存在
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="enumValue"></param>
        /// <returns></returns>
        public static bool CheckEnumValueExists<T>(int enumValue)
        {
            var fields = typeof(T).GetFields(BindingFlags.Public | BindingFlags.Static);

            var enumValueList = new List<int>();
            foreach (var item in fields)
            {
                var value = item.GetValue(null);
                enumValueList.Add((int)value!);
            }

            return enumValueList.Any(x => x == enumValue);
        }

        /// <summary>
        /// 根据描述信息获取对应的枚举值
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="enumLabel"></param>
        /// <returns></returns>
        public static int? GetEnumValue<T>(string enumLabel) where T : struct
        {

            var fields = typeof(T).GetFields(BindingFlags.Public | BindingFlags.Static);

            var enumValueList = new List<int>();

            foreach (var item in fields)
            {
                var descriptionAttribute = item.GetCustomAttribute<DescriptionAttribute>();

                if (descriptionAttribute != null && descriptionAttribute.Description == enumLabel)
                {
                    var enumValue = (T)item.GetValue(null)!;
                    return Convert.ToInt32(enumValue);
                }
            }

            return null;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="selector"></param>
        /// <returns></returns>
        public static List<TResult> GetEnumsDescription<T, TResult>(Func<int, DescriptionAttribute?, TResult> selector)
            where T : struct
            where TResult : class => GetEnumsDescription(typeof(T), selector);

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="enumType"></param>
        /// <param name="selector"></param>
        /// <returns></returns>
        public static List<T> GetEnumsDescription<T>(Type enumType, Func<int, DescriptionAttribute?, T> selector) where T : class
        {
            var fields = enumType.GetFields(BindingFlags.Public | BindingFlags.Static);
            if (!fields.HasValue())
                return new List<T>();

            var list = new List<T>();

            foreach (var item in fields)
            {
                var value = (int)item.GetValue(null)!;
                var attr = item.GetCustomAttribute<DescriptionAttribute>();

                list.Add(selector.Invoke(value, attr));
            }

            return list;
        }
    }
}

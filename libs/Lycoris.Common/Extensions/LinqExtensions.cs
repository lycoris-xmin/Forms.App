using Lycoris.Common.Extensions;
using Lycoris.Common.Extensions.Models;
using System.Linq.Expressions;

namespace Lycoris.Common.Extensions
{
    /// <summary>
    /// linq部分常用扩展
    /// </summary>
    public static class LinqExtensions
    {
        /// <summary>
        /// Where扩展
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <param name="condition">判断条件</param>
        /// <param name="predicate">条件成立则执行该Where条件</param>
        /// <returns></returns>
        public static IEnumerable<T> WhereIf<T>(this IEnumerable<T> source, bool condition, Func<T, bool> predicate) => condition ? source.Where(predicate) : source;

        /// <summary>
        /// Where扩展
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <param name="condition">判断条件</param>
        /// <param name="predicate">条件成立则执行该Where条件</param>
        /// <param name="elsePredicate">条件不成立则执行该Where条件</param>
        /// <returns></returns>
        public static IEnumerable<T> WhereIf<T>(this IEnumerable<T> source, bool condition, Func<T, bool> predicate, Func<T, bool> elsePredicate) => condition ? source.Where(predicate) : source.Where(elsePredicate);

        /// <summary>
        /// Where扩展
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <param name="condition">判断条件</param>
        /// <param name="predicate">条件成立则执行该Where条件</param>
        /// <returns></returns>
        public static IEnumerable<T> WhereIf<T>(this IEnumerable<T> source, bool condition, Func<T, int, bool> predicate) => condition ? source.Where(predicate) : source;

        /// <summary>
        /// Where扩展
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <param name="condition">判断条件</param>
        /// <param name="predicate">条件成立则执行该Where条件</param>
        /// <param name="elsePredicate">条件不成立则执行该Where条件</param>
        /// <returns></returns>
        public static IEnumerable<T> WhereIf<T>(this IEnumerable<T> source, bool condition, Func<T, int, bool> predicate, Func<T, int, bool> elsePredicate) => condition ? source.Where(predicate) : source.Where(elsePredicate);

        /// <summary>
        /// Where扩展
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <param name="condition">判断条件</param>
        /// <param name="predicate">条件成立则执行该Where条件</param>
        /// <returns></returns>
        public static IQueryable<T> WhereIf<T>(this IQueryable<T> source, bool condition, Expression<Func<T, bool>> predicate) => condition ? source.Where(predicate) : source;

        /// <summary>
        /// Where扩展
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>       
        /// <param name="condition">判断条件</param>
        /// <param name="predicate">条件成立则执行该Where条件</param>
        /// <param name="elsePredicate">条件不成立则执行该Where条件</param>
        /// <returns></returns>
        public static IQueryable<T> WhereIf<T>(this IQueryable<T> source, bool condition, Expression<Func<T, bool>> predicate, Expression<Func<T, bool>> elsePredicate) => condition ? source.Where(predicate) : source.Where(elsePredicate);

        /// <summary>
        /// Where扩展
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <param name="condition">判断条件</param>
        /// <param name="predicate">条件成立则执行该Where条件</param>
        /// <returns></returns>
        public static IQueryable<T> WhereIf<T>(this IQueryable<T> source, bool condition, Expression<Func<T, int, bool>> predicate) => condition ? source.Where(predicate) : source;

        /// <summary>
        /// Where扩展
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <param name="condition">判断条件</param>
        /// <param name="predicate">条件成立则执行该Where条件</param>
        /// <param name="elsePredicate">条件不成立则执行该Where条件</param>
        /// <returns></returns>
        public static IQueryable<T> WhereIf<T>(this IQueryable<T> source, bool condition, Expression<Func<T, int, bool>> predicate, Expression<Func<T, int, bool>> elsePredicate) => condition ? source.Where(predicate) : source.Where(elsePredicate);

        /// <summary>
        /// 根据某个属性进行去重
        /// </summary>
        /// <typeparam name="TSource"></typeparam>
        /// <typeparam name="TKey"></typeparam>
        /// <param name="source"></param>
        /// <param name="keySelector">针对去重的键</param>
        /// <returns></returns>
        [Obsolete("请确认当前版本的是否有系统的DistinctBy方法，优先使用DistinctBy方法")]
        public static IEnumerable<TSource> Distinct<TSource, TKey>(this IEnumerable<TSource> source, Func<TSource, TKey> keySelector)
        {
            var keys = new HashSet<TKey>();
            foreach (TSource element in source)
                if (keys.Add(keySelector(element)))
                    yield return element;
        }

        /// <summary>
        /// 根据属性名进行去重
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj"></param>
        /// <param name="propertyName">针对去重的属性名</param>
        /// <returns></returns>
        [Obsolete("请确认当前版本的是否有系统的DistinctBy方法，优先使用DistinctBy方法")]
        public static IQueryable<T> Distinct<T>(this IQueryable<T> obj, string propertyName) => obj.Distinct(new PropertyComparer<T>(propertyName));

        /// <summary>
        /// 分页
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="query"></param>
        /// <param name="pageIndex">页码(页码起始位置从1开始)</param>
        /// <param name="pageSize">每页最大条数</param>
        /// <returns></returns>
        public static IEnumerable<T> PageBy<T>(this IEnumerable<T> query, int pageIndex, int pageSize)
        {
            if (pageIndex < 1)
                throw new ArgumentOutOfRangeException(nameof(pageIndex));

            if (pageSize < 0)
                throw new ArgumentOutOfRangeException(nameof(pageSize));

            return query.Skip((pageIndex - 1) * pageSize).Take(pageSize);
        }

        /// <summary>
        /// 分页
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="query"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public static IQueryable<T> PageBy<T>(this IQueryable<T> query, int pageIndex, int pageSize)
        {
            if (pageIndex < 1)
                throw new ArgumentOutOfRangeException(nameof(pageIndex));

            if (pageSize < 0)
                throw new ArgumentOutOfRangeException(nameof(pageSize));

            return query.Skip((pageIndex - 1) * pageSize).Take(pageSize);
        }

        /// <summary>
        /// 合计
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="query"></param>
        /// <param name="expression"></param>
        /// <returns></returns>
        public static IQueryable<TResult>? PageSum<T, TResult>(this IEnumerable<T> query, Expression<Func<IGrouping<int, T>, TResult>> expression)
            => query.AsQueryable().PageSum(expression) ?? default;

        /// <summary>
        /// 合计
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="query"></param>
        /// <param name="expression"></param>
        /// <returns></returns>
        public static IQueryable<TResult>? PageSum<T, TResult>(this IQueryable<T> query, Expression<Func<IGrouping<int, T>, TResult>> expression)
            => query.GroupBy(x => 1).Select(expression);

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="data"></param>
        /// <param name="condition"></param>
        /// <param name="action"></param>
        /// <returns></returns>
        public static T UpdatePorpertyIf<T>(this T data, bool condition, Action<T> action)
        {
            if (condition)
                action(data);

            return data;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="data"></param>
        /// <param name="condition"></param>
        /// <param name="action"></param>
        /// <returns></returns>
        public static T UpdatePorpertyIf<T>(this T data, Func<T, bool> condition, Action<T> action)
        {
            if (condition.Invoke(data))
                action(data);

            return data;
        }

        /// <summary>
        /// 扩展 Between 操作符
        /// 使用 var query = repository.Between(person => person.Age, 18, 21);
        /// </summary>
        /// <typeparam name="TSource"></typeparam>
        /// <typeparam name="TKey"></typeparam>
        /// <param name="source"></param>
        /// <param name="keySelector"></param>
        /// <param name="low"></param>
        /// <param name="high"></param>
        /// <returns></returns>
        public static IQueryable<TSource> Between<TSource, TKey>(this IQueryable<TSource> source, Expression<Func<TSource, TKey>> keySelector, TKey low, TKey high) where TKey : IComparable<TKey>
        {
            Expression key = Expression.Invoke(keySelector, keySelector.Parameters.ToArray());
            Expression lowerBound = Expression.GreaterThanOrEqual(key, Expression.Constant(low));
            Expression upperBound = Expression.LessThanOrEqual(key, Expression.Constant(high));
            Expression and = Expression.AndAlso(lowerBound, upperBound);
            Expression<Func<TSource, bool>> lambda = Expression.Lambda<Func<TSource, bool>>(and, keySelector.Parameters);
            return source.Where(lambda.Compile()).AsQueryable();
        }

        /// <summary>
        /// 条件排序（OrderBy）
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <param name="fieldName"></param>
        /// <param name="direction"></param>
        /// <returns></returns>
        public static IOrderedQueryable<T> OrderBy<T>(this IQueryable<T> source, string fieldName, string direction)
        {
            return ApplyOrder(source, fieldName, direction, false);
        }

        /// <summary>
        /// 条件排序（OrderBy）
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <param name="condition"></param>
        /// <param name="fieldName"></param>
        /// <param name="direction"></param>
        /// <returns></returns>
        public static IOrderedQueryable<T> OrderByIf<T>(this IQueryable<T> source, bool condition, string fieldName, string direction)
        {
            if (!condition)
                return source.OrderBy(x => 0);

            return source.OrderBy(fieldName, direction);
        }

        /// <summary>
        /// 条件排序
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="TKey"></typeparam>
        /// <param name="source"></param>
        /// <param name="condition"></param>
        /// <param name="fieldName"></param>
        /// <param name="direction"></param>
        /// <param name="falsekeySelector"></param>
        /// <returns></returns>
        public static IOrderedQueryable<T> OrderThenIf<T, TKey>(this IQueryable<T> source, bool condition, string fieldName, string direction, Expression<Func<T, TKey>> falsekeySelector)
        {
            if (!condition)
                return source.OrderBy(falsekeySelector);

            return source.OrderBy(fieldName, direction).ThenBy(falsekeySelector);
        }

        /// <summary>
        /// 条件排序
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="TKey"></typeparam>
        /// <param name="source"></param>
        /// <param name="condition"></param>
        /// <param name="fieldName"></param>
        /// <param name="direction"></param>
        /// <param name="falsekeySelector"></param>
        /// <returns></returns>
        public static IOrderedQueryable<T> OrderThenDescIf<T, TKey>(this IQueryable<T> source, bool condition, string fieldName, string direction, Expression<Func<T, TKey>> falsekeySelector)
        {
            if (!condition)
                return source.OrderBy(falsekeySelector);

            return source.OrderBy(fieldName, direction).ThenByDescending(falsekeySelector);
        }

        /// <summary>
        /// 条件排序（OrderBy）
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="TKey"></typeparam>
        /// <param name="source"></param>
        /// <param name="condition"></param>
        /// <param name="keySelector"></param>
        /// <param name="falsekeySelector"></param>
        /// <returns></returns>
        public static IOrderedQueryable<T> OrderByIf<T, TKey>(this IQueryable<T> source, bool condition, Expression<Func<T, TKey>> keySelector, Expression<Func<T, TKey>> falsekeySelector)
        {
            if (!condition)
                return source.OrderBy(keySelector);

            return condition ? source.OrderBy(keySelector) : source.OrderBy(falsekeySelector);
        }

        /// <summary>
        /// 条件排序（OrderBy）
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="TKey"></typeparam>
        /// <param name="source"></param>
        /// <param name="condition"></param>
        /// <param name="fieldName"></param>
        /// <param name="direction"></param>
        /// <param name="keySelector"></param>
        /// <returns></returns>
        public static IOrderedQueryable<T> OrderByIf<T, TKey>(this IQueryable<T> source, bool condition, string fieldName, string direction, Expression<Func<T, TKey>> keySelector)
        {
            if (!condition)
                return source.OrderBy(keySelector);

            return source.OrderBy(fieldName, direction);
        }

        /// <summary>
        /// 条件排序（OrderByDescending）
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="TKey"></typeparam>
        /// <param name="source"></param>
        /// <param name="condition"></param>
        /// <param name="keySelector"></param>
        /// <returns></returns>
        public static IOrderedQueryable<T> OrderByDescIf<T, TKey>(this IQueryable<T> source, bool condition, Expression<Func<T, TKey>> keySelector)
        {
            if (!condition)
                return source.OrderBy(x => 0);

            return source.OrderByDescending(keySelector);
        }

        /// <summary>
        /// 条件排序（OrderByDescending）
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="TKey"></typeparam>
        /// <param name="source"></param>
        /// <param name="condition"></param>
        /// <param name="keySelector"></param>
        /// <param name="falsekeySelector"></param>
        /// <returns></returns>
        public static IOrderedQueryable<T> OrderByDescIf<T, TKey>(this IQueryable<T> source, bool condition, Expression<Func<T, TKey>> keySelector, Expression<Func<T, TKey>> falsekeySelector)
        {
            if (!condition)
                return source.OrderBy(keySelector);

            return condition ? source.OrderByDescending(keySelector) : source.OrderByDescending(falsekeySelector);
        }

        /// <summary>
        /// 条件次排序（ThenBy）
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <param name="fieldName"></param>
        /// <param name="direction"></param>
        /// <returns></returns>
        public static IOrderedQueryable<T> ThenBy<T>(this IOrderedQueryable<T> source, string fieldName, string direction)
        {
            return ApplyOrder(source, fieldName, direction, true);
        }

        /// <summary>
        /// 条件次排序（ThenBy）
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <param name="condition"></param>
        /// <param name="fieldName"></param>
        /// <param name="direction"></param>
        /// <returns></returns>
        public static IOrderedQueryable<T> ThenByIf<T>(this IOrderedQueryable<T> source, bool condition, string fieldName, string direction)
        {
            if (!condition)
                return source;

            return source.ThenBy(fieldName, direction);
        }

        /// <summary>
        /// 条件次排序（ThenBy）
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="TKey"></typeparam>
        /// <param name="source"></param>
        /// <param name="condition"></param>
        /// <param name="keySelector"></param>
        /// <returns></returns>
        public static IOrderedQueryable<T> ThenByIf<T, TKey>(this IOrderedQueryable<T> source, bool condition, Expression<Func<T, TKey>> keySelector)
        {
            if (!condition)
                return source;

            return source.ThenBy(keySelector);
        }

        /// <summary>
        /// 条件次排序（ThenByDescending）
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="TKey"></typeparam>
        /// <param name="source"></param>
        /// <param name="condition"></param>
        /// <param name="keySelector"></param>
        /// <returns></returns>
        public static IOrderedQueryable<T> ThenByDescIf<T, TKey>(this IOrderedQueryable<T> source, bool condition, Expression<Func<T, TKey>> keySelector)
        {
            if (!condition)
                return source;

            return source.ThenByDescending(keySelector);
        }

        /// <summary>
        /// 排序
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <param name="orderFields"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        /// <exception cref="InvalidOperationException"></exception>
        public static IOrderedQueryable<T> SortBy<T>(this IQueryable<T> source, Dictionary<string, string> orderFields)
        {
            if (orderFields == null || orderFields.Count == 0)
                throw new ArgumentException("排序字段不能为空");

            IOrderedQueryable<T>? orderedQuery = null;

            foreach (var (field, direction) in orderFields)
            {
                if (orderedQuery == null)
                    orderedQuery = source.OrderBy(field, direction);
                else
                    orderedQuery = orderedQuery.ThenBy(field, direction);
            }

            return orderedQuery ?? throw new InvalidOperationException("ApplySort 无法构建排序查询");
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <param name="fieldName"></param>
        /// <param name="direction"></param>
        /// <param name="isThenBy"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        private static IOrderedQueryable<T> ApplyOrder<T>(IQueryable<T> source, string fieldName, string direction, bool isThenBy)
        {
            var property = typeof(T).GetProperties().FirstOrDefault(p => p.Name.Equals(fieldName, StringComparison.OrdinalIgnoreCase))
                ?? throw new ArgumentException($"Invalid sort field: {fieldName}");

            var parameter = Expression.Parameter(typeof(T), "x");
            var selector = Expression.Lambda(Expression.Property(parameter, property), parameter);

            var methodName = direction.Equals("desc", StringComparison.CurrentCultureIgnoreCase)
                ? (isThenBy ? "ThenByDescending" : "OrderByDescending")
                : (isThenBy ? "ThenBy" : "OrderBy");

            var resultExp = Expression.Call(
                typeof(Queryable),
                methodName,
                new[] { typeof(T), property.PropertyType },
                source.Expression,
                Expression.Quote(selector));

            return (IOrderedQueryable<T>)source.Provider.CreateQuery<T>(resultExp);
        }
    }
}

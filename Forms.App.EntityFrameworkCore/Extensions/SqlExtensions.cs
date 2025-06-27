using Forms.App.EntityFrameworkCore.Data.Tables.Shared;
using Lycoris.Common.Extensions;
using Microsoft.EntityFrameworkCore.Infrastructure;
using System.Linq.Expressions;

namespace Forms.App.EntityFrameworkCore.Extensions
{
    public static class SqlExtensions
    {
        public static bool HasModify<T, TSelector>(this List<Expression<Func<T, object>>>? fieIds, Expression<Func<T, TSelector>> selector) where T : ITableBaseEntity
        {
            if (!fieIds.HasValue())
                return false;

            var name = GetPropertyName(selector);

            foreach (var item in fieIds!)
            {
                foreach (var member in item.GetMemberAccessList())
                {
                    if (member.Name.Equals(name))
                        return true;
                }
            }

            return false;
        }


        private static string GetPropertyName<T, TSelector>(Expression<Func<T, TSelector>> selector)
        {
            if (selector.Body is MemberExpression memberExpression)
            {
                return memberExpression.Member.Name;
            }
            else if (selector.Body is UnaryExpression unaryExpression && unaryExpression.Operand is MemberExpression unaryMember)
            {
                return unaryMember.Member.Name;
            }

            throw new ArgumentException("Selector must be a property access expression.", nameof(selector));
        }
    }
}

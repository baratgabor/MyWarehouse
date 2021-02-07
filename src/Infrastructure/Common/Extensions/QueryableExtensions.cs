using StringToExpression.LanguageDefinitions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace MyWarehouse.Infrastructure
{
    internal static class QueryableExtensions
    {
        /// <summary>
        /// Applies filtering to a query by parsing the provided OData-standard filter string and translating it into expresions.
        /// </summary>
        /// <exception cref="FormatException">Thrown when the filter string is incorrectly formatted.</exception>
        public static IQueryable<T> ApplyFilter<T>(this IQueryable<T> query, string oDataFilterString)
        {
            try
            {
                return ApplyFilterInternal(query, oDataFilterString);
            }
            catch(Exception e)
            {
                throw new FormatException($"The specified filter string '{oDataFilterString}' is invalid.", e);
            }
        }

        /// <summary>
        /// Applies sorting to a query by parsing the provided OData-standard OrderBy string and translating it into expressions.
        /// General expected string format is 'propertyName1, properyName2 asc, propertyName3 desc'. Specifying 'asc'/'desc' is optional. Nested property access is supported with '/' (e.g. Customer/Name).
        /// </summary>
        /// <exception cref="FormatException">Thrown when the orderBy string is incorrectly formatted.</exception>
        public static IQueryable<T> ApplyOrder<T>(this IQueryable<T> query, string oDataOrderByString, int maximumNumberOfOrdering = 5)
        {
            try
            {
                return ApplyOrderInternal(query, oDataOrderByString, maximumNumberOfOrdering);
            }
            catch(Exception e)
            {
                throw new FormatException($"The specified orderBy string '{oDataOrderByString}' is invalid.", e);
            }
        }

        /// <summary>
        /// Applies paging to a query expression, where index 1 is the first page.
        /// </summary>
        public static IQueryable<T> ApplyPaging<T>(this IQueryable<T> query, int pageSize, int pageIndex)
            => query
                .Skip((pageIndex - 1) * pageSize)
                .Take(pageSize);

        private static IQueryable<T> ApplyFilterInternal<T>(IQueryable<T> query, string oDataFilterString)
        {
            if (string.IsNullOrWhiteSpace(oDataFilterString))
            {
                return query;
            }

            var filterExpression = new ODataFilterLanguage().Parse<T>(oDataFilterString);
            return query.Where(filterExpression);
        }

        private static IQueryable<T> ApplyOrderInternal<T>(IQueryable<T> query, string oDataOrderByString, int maximumNumberOfOrdering)
        {
            if (string.IsNullOrWhiteSpace(oDataOrderByString))
            {
                return query;
            }

            bool firstOrdering = true;
            foreach (var (propertyName, order) in GetOrderEntries(oDataOrderByString, maximumNumberOfOrdering))
            {
                query = ApplyOrdering(query, propertyName, order, firstOrdering);
                firstOrdering = false;
            }

            //TODO: Look into the necessity (or lack thereof) of filtering out multiple orderBy on the same property.
            return query;

            static IEnumerable<(string propertyPath, SortOrder order)> GetOrderEntries(string orderByString, int maxOrders)
            {
                return orderByString
                    .Split(',', count: maxOrders, StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
                    .Select(orderStr =>
                    {
                        var divider = orderStr.IndexOf(' ');
                        if (divider < 0) return (propertyPath: orderStr, order: SortOrder.Asc);
                        else return (
                            propertyPath: orderStr[0..divider],
                            order: Enum.Parse<SortOrder>(orderStr[divider..].Trim(), ignoreCase: true)
                        );
                    });
            }

            static IQueryable<T> ApplyOrdering(IQueryable<T> query, string propertyPath, SortOrder order, bool firstOrdering)
            {
                var param = Expression.Parameter(typeof(T), "p");
                var member = (MemberExpression)propertyPath.Split('/').Aggregate((Expression)param, Expression.Property); //Expression.Property(param, propertyPath);
                var exp = Expression.Lambda(member, param);
                string methodName = order switch
                {
                    SortOrder.Asc => firstOrdering ? "OrderBy" : "ThenBy",
                    SortOrder.Desc => firstOrdering ? "OrderByDescending" : "ThenByDescending"
                };
                Type[] types = new Type[] { query.ElementType, exp.Body.Type };
                var orderByExpression = Expression.Call(typeof(Queryable), methodName, types, query.Expression, exp);
                return query.Provider.CreateQuery<T>(orderByExpression);
            }
        }

        private enum SortOrder
        {
            Asc,
            Desc
        }
    }
}

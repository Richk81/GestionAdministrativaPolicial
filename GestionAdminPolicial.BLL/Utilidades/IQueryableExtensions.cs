using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace GestionAdminPolicial.BLL.Utilidades
{
    public static class IQueryableExtensions
    {
        // Método de extensión para ordenar dinámicamente por una propiedad (en listar de reportes necesito esto)
        // para el ordenamiento dinámico en DataTables y buscar por diferentes columnas de otras entidades relacionadas
        public static IQueryable<T> OrderByDynamic<T>(this IQueryable<T> query, string propertyName, bool ascending)
        {
            if (string.IsNullOrEmpty(propertyName))
                return query;

            var parameter = Expression.Parameter(typeof(T), "x");
            var selector = Expression.PropertyOrField(parameter, propertyName);
            var lambda = Expression.Lambda(selector, parameter);

            string method = ascending ? "OrderBy" : "OrderByDescending";

            var result = typeof(Queryable).GetMethods()
                .Single(m => m.Name == method && m.GetParameters().Length == 2)
                .MakeGenericMethod(typeof(T), selector.Type)
                .Invoke(null, new object[] { query, lambda });

            return (IQueryable<T>)result!;
        }
    }
}

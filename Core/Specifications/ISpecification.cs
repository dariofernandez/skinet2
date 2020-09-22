using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace Core.Specifications
{
    public interface ISpecification<T>
    {
        // the Criteria will be like Type 1
        Expression<Func<T, bool>> Criteria { get; }

        List<Expression<Func<T, object>>> Includes { get; }

        // takes generic and returns object
        Expression<Func<T, object>> OrderBy { get; }

        Expression<Func<T, object>> OrderByDescending { get; }

        // for pagination 63.
        int Take { get; }
        int Skip { get; }
        bool IsPagingEnabled { get; }


    }
}

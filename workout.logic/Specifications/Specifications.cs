using System;
using workout.abstractions.Entities;
using workout.abstractions.Specifications;

namespace workout.logic.Specifications;

public class SpecificationsQuery
{

    public static IQueryable<Workout> GetQuery(IQueryable<Workout> query, Specs specs)
    {
        if (specs.Where != null)
        {
            query.Where(specs.Where);
        }
        if (specs.OrderBy != null)
        {
            query.OrderBy(specs.OrderBy);
        }
        return query;
    }

}

using System;
using System.Linq.Expressions;
using workout.abstractions.Entities;

namespace workout.abstractions.Specifications;

public class Specs
{

    public Expression<Func<Workout, bool>> Where { get; set; }

    public Expression<Func<Workout, object>> OrderBy { get; set; }


    public Specs(SpecsParams sp, string UserId)
    {
        Where = x => x.UserId == UserId && x.Ejercicio.Contains(sp.WorkoutName);
        AddOrderBy(x => x.CreatedAt > sp.Date);
    }


    public void AddOrderBy(Expression<Func<Workout, object>> expression)
    {
        this.OrderBy = expression;
    }

    // public void AddCriteria()


}
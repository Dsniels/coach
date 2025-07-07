using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using workout.abstractions.Entities;

namespace workout.logic.Context;

public class WorkoutDbContext : IdentityDbContext<User>
{
    public WorkoutDbContext(DbContextOptions<WorkoutDbContext> opts) : base(opts) { }
    public required DbSet<Workout> Workouts { get; set; }


    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
    }

}

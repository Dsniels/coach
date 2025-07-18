using Microsoft.EntityFrameworkCore;
using workout.abstractions.Entities;
using workout.abstractions.Interfaces;
using workout.abstractions.Specifications;
using workout.logic.Context;
using workout.logic.Specifications;

namespace workout.logic.Repositories;

public class WorkoutRepository : IWorkoutRepository
{
    private readonly WorkoutDbContext _context;
    private readonly DbSet<Workout> _db;
    public WorkoutRepository(WorkoutDbContext context)
    {
        _context = context;
        _db = context.Set<Workout>();

    }
    public async Task CreateWorkoutAsync(Workout workout)
    {
        await _db.AddAsync(workout);
    }

    public async Task<int> DeleteWorkoutAsync(Workout workout)
    {
        _db.Remove(workout);
        return await this.SaveChangesAsync();

    }
    public async Task<IReadOnlyCollection<Workout>> GetWorkoutsAsync(string UserId)
    {
        return await _db.Where(w => w.UserId == UserId).ToListAsync();
    }
    private IQueryable<Workout> ApplyFilters(Specs spec)
    {
        return SpecificationsQuery.GetQuery(_db, spec);
    }

    public async  Task<Workout> GetWorkoutsBySpec(Specs spec)
    {
        return await ApplyFilters(spec).FirstOrDefaultAsync()!;

    }

    public async Task<int> SaveChangesAsync()
    {
        return await _context.SaveChangesAsync();
    }
}

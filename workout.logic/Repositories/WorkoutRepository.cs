using Microsoft.EntityFrameworkCore;
using workout.abstractions.Entities;
using workout.abstractions.Interfaces;
using workout.logic.Context;

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
    public async Task<int> CreateWorkoutAsync(Workout workout)
    {
        await _db.AddAsync(workout);
        return await this.SaveChangesAsync();
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

    public async Task<int> SaveChangesAsync()
    {
        return await _context.SaveChangesAsync();
    }
}

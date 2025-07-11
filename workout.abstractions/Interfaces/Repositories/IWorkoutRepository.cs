using workout.abstractions.Entities;

namespace workout.abstractions.Interfaces;

public interface IWorkoutRepository
{
    Task<int> CreateWorkoutAsync(Workout workout);
    Task<int> DeleteWorkoutAsync(Workout workout);
    Task<IReadOnlyCollection<Workout>> GetWorkoutsAsync(string UserId);
    Task<int> SaveChangesAsync();
}

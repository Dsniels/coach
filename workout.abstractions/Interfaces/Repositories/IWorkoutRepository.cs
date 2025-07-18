
using workout.abstractions.Entities;
using workout.abstractions.Specifications;

namespace workout.abstractions.Interfaces;

public interface IWorkoutRepository
{
    Task<Workout> GetWorkoutsBySpec(Specs spec);
    Task CreateWorkoutAsync(Workout workout);
    Task<int> DeleteWorkoutAsync(Workout workout);
    Task<IReadOnlyCollection<Workout>> GetWorkoutsAsync(string UserId);
    Task<int> SaveChangesAsync();
}
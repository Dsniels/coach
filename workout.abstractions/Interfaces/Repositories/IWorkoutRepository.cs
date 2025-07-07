using workout.abstractions.Entities;

namespace workout.abstractions.Interfaces;

public interface IWorkoutRepository
{
    Task<int> CreateWorkout(Workout workout);
    Task<int> DeleteWorkout(int id);
    Task<IReadOnlyCollection<Workout>> GetWorkouts(string UserId);
}

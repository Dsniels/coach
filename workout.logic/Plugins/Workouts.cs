using Microsoft.AspNetCore.Http;
using workout.abstractions.Entities;
using workout.abstractions.Interfaces;
using workout.abstractions.Specifications;

namespace workout.logic.Options;

public class WorkoutsPlugin
{

    private readonly IWorkoutRepository _workoutRepository;
    public WorkoutsPlugin(IHttpContextAccessor context, IWorkoutRepository workoutRepository)
    {
        _workoutRepository = workoutRepository;
    }

    public async Task<int> CreateWorkouts(IList<WorkoutDto> dtos)
    {
        foreach (var dto in dtos)
        {
            Workout workout = new()
            {
                Ejercicio = dto.Ejercicio,
                Sets = dto.Sets,
                Repeticiones = dto.Repeticiones
            };
            await _workoutRepository.CreateWorkoutAsync(workout);
        }
        return await _workoutRepository.SaveChangesAsync();
    }

    public async Task<Workout> GetWorkOutByDate(SpecsParams @params)
    {
        var spec = new Specs(@params, "testss");
        var workout = await _workoutRepository.GetWorkoutsBySpec(spec);
        return workout;
    }

}

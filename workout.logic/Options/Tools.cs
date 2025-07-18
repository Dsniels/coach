using System;
using Microsoft.AspNetCore.Http;
using workout.abstractions.Entities;
using workout.abstractions.Interfaces;
using workout.abstractions.Specifications;

namespace workout.logic.Options;

public class Tools
{
    private readonly HttpContext _context;
    private readonly IWorkoutRepository _workoutRepository;
    public Tools(HttpContextAccessor context, IWorkoutRepository workoutRepository)
    {
        _context = context.HttpContext;
        _workoutRepository = workoutRepository;
    }

    public Func<IList<WorkoutDto>, Task<int>> CreateWorkout()
    {
        return async (dtos) =>
            {
                try
                {
                    foreach (var dto in dtos)
                    {
                        var workout = new Workout
                        {
                            Ejercicio = dto.Ejercicio,
                            Sets = dto.Sets,
                            Repeticiones = dto.Repeticiones,
                        };
                        workout.UserId = "testss";
                        await this._workoutRepository.CreateWorkoutAsync(workout);
                    }
                    return await _workoutRepository.SaveChangesAsync();
                }
                catch (System.Exception e)
                {
                    return 0;
                }

            };
    }

    public Func<SpecsParams, Task<Workout>> GetWorkOutByDate()
    {
        return async (specifications) =>
        {
            var spec = new Specs(specifications, "testss");
            var workout = await _workoutRepository.GetWorkoutsBySpec(spec);
            return workout;
        };
    }

}

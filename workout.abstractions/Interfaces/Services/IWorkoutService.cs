using System;

namespace workout.abstractions.Interfaces.Services;

public interface IWorkoutService
{
    Task GetWorkoutByIdAsync();
    Task GetWorkoutBySpecAsync();
    

}

using System;

namespace workout.abstractions.Entities;

public class Workout
{
    public int Id { get; set; }
    public string? UserId { get; set; }
    public string Ejercicio { get; set; }
    public int Repeticiones { get; set; }
    public int Sets { get; set; }
    public DateTime CreatedAt { get; init; } = DateTime.UtcNow;
}
public record WorkoutDto(string Ejercicio, int Repeticiones, int Sets);

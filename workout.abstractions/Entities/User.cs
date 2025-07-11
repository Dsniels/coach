using Microsoft.AspNetCore.Identity;

namespace workout.abstractions.Entities;

public class User : IdentityUser
{
    public string Nombre { get; set; }

}

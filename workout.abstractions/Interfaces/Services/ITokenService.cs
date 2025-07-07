using workout.abstractions.Entities;

namespace workout.abstractions.Interfaces.Services;

public interface ITokenService
{
    string CreateToken(User user);

}

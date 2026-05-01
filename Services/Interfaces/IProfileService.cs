using HabitApi.Models.DTO;

namespace HabitApi.Services.Interfaces;

/// <summary>
/// Service for reading and updating the current user's profile settings.
/// </summary>
public interface IProfileService
{
    Task<UserProfileDto?> GetProfileAsync(Guid userId, CancellationToken cancellationToken);
    Task<UserProfileDto?> UpdateProfileAsync(Guid userId, UpdateUserProfileDto request, CancellationToken cancellationToken);
}

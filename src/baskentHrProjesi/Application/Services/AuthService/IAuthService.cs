using Domain.Entities;
using NArchitecture.Core.Security.JWT;

namespace Application.Services.AuthService;

public interface IAuthService
{
    Task<AccessToken> CreateAccessToken(User user, Employee? employee = null);
    Task<RefreshToken> CreateRefreshToken(User user, string ipAddress);
    Task<RefreshToken?> GetRefreshTokenByToken(string refreshToken);
    Task<RefreshToken> AddRefreshToken(RefreshToken refreshToken);
    Task DeleteOldRefreshTokens(Guid userId);
    Task RevokeDescendantRefreshTokens(RefreshToken refreshToken, string ipAddress, string reason);
    Task RevokeRefreshToken(RefreshToken token, string ipAddress, string? reason = null, string? replacedByToken = null);
    Task<RefreshToken> RotateRefreshToken(User user, RefreshToken refreshToken, string ipAddress);
}

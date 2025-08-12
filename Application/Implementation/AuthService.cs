using Application.DTOs;
using Application.Interfaces;
using DAL.Interfaces;

namespace Application.Implementation
{
    public class AuthService : IAuthService
    {
        private readonly IIdentityService _identity;
        public AuthService(IIdentityService identity) => _identity = identity;
        public Task RegisterAsync(RegisterDto dto) => _identity.RegisterAsync(dto.Email, dto.Password);
        public Task<string> LoginAsync(LoginDto dto) => _identity.LoginAsync(dto.Email, dto.Password);
    }

}

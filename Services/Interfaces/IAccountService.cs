using Contoso_BookStore_AccountService.DTO.Requests;
using Contoso_BookStore_AccountService.DTO.Responses;

namespace Contoso_BookStore_AccountService.Services.Interfaces
{
    public interface IAccountService
    {
        Task<GenericResponse<string>> Register(RegisterRequest request);
        Task<GenericResponse<LoginResponse>> Login(LoginRequest request);
    }
}
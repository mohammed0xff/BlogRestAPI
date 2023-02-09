using Models.ApiModels;
using System.Threading.Tasks;


namespace Services
{
    public interface IAuthService
    {
        Task<List<string>> RegisterAsync(RegistrationModelRequest model);
        Task<LoginModelResponse> LoginAsync(LoginModelRequest model);
        Task<LoginModelResponse> VerifyAndGenerateToken(TokenRequest tokenRequest);
        Task RevokeTokenAsync(string userId);
    }
}
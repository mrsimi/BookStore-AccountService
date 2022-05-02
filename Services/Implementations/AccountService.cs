using Contoso_BookStore_AccountService.Data;
using Contoso_BookStore_AccountService.DTO.Requests;
using Contoso_BookStore_AccountService.DTO.Responses;
using Contoso_BookStore_AccountService.Models;
using Contoso_BookStore_AccountService.Services.Interfaces;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using Microsoft.EntityFrameworkCore;
using System.Net;
using System.Security.Cryptography;


namespace Contoso_BookStore_AccountService.Services.Implementations
{
    public class AccountService : IAccountService
    {

        private readonly ContosoBookStoreDbContext  _dbContext;

        public AccountService(ContosoBookStoreDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<GenericResponse<LoginResponse>> Login(LoginRequest request)
        {
            try
            {
                var userAccount = await GetUser(request.Email.ToLower().Trim());
                if(userAccount == null) return new GenericResponse<LoginResponse>{Data = null, ResponseMessage = "User not found", StatusCode = (int)HttpStatusCode.NotFound};

                if(IsPasswordCorrect(request.Password, userAccount.PasswordSalt, userAccount.PasswordHash)) 
                    return new GenericResponse<LoginResponse>
                    {
                        Data = new LoginResponse{ Email = userAccount.Email, Token = "Welcome"}, 
                        StatusCode = (int)HttpStatusCode.OK, 
                        ResponseMessage = "Login successful"
                    };

                
                else
                    return new GenericResponse<LoginResponse>
                    {
                        Data = null, 
                        StatusCode = (int)HttpStatusCode.Conflict, 
                        ResponseMessage = "Invalid Credentials"
                    };
            }
            catch (System.Exception)
            {
                return new GenericResponse<LoginResponse>
                {
                    Data = null, 
                    StatusCode = (int)HttpStatusCode.Created, 
                    ResponseMessage = "Internal Server Error"
                };
            }  
        }

        public async Task<GenericResponse<string>> Register(RegisterRequest request)
        {
            try
            {
                var userAccount = await GetUser(request.Email.ToLower().Trim());
                if(userAccount != null)
                {
                    return new GenericResponse<string>
                    {
                        Data = null,
                        StatusCode = (int)HttpStatusCode.Conflict,
                        ResponseMessage = "User already exist"
                    };
                }

                var passwordSaltHashDict = ComputePasswordSaltAndHash(request.Password);

                var newUserAccount = new UserAccount
                {
                    Email = request.Email.ToLower().Trim(), 
                    PasswordSalt = passwordSaltHashDict.Keys.FirstOrDefault(), 
                    PasswordHash = passwordSaltHashDict.Values.FirstOrDefault(), 
                    DateAdded = DateTime.UtcNow
                };

                await _dbContext.UserAccounts.AddAsync(newUserAccount);
                await _dbContext.SaveChangesAsync();


                return new GenericResponse<string>
                {
                    Data = null, 
                    StatusCode = (int)HttpStatusCode.Created, 
                    ResponseMessage = "User created successfully"
                };
            }
            catch (System.Exception ex)
            {
                return new GenericResponse<string>
                {
                    Data = $"Exception description: {ex.Message.ToString()}", 
                    StatusCode = (int)HttpStatusCode.Created, 
                    ResponseMessage = "Internal Server Error"
                };
            }
        }

        private Dictionary<string, string> ComputePasswordSaltAndHash(string password)
        {
            Dictionary<string, string> result = new Dictionary<string, string>();
            byte[] salt = new byte[128 / 8];
            
            var rng = RandomNumberGenerator.Create();
            rng.GetNonZeroBytes(salt);

            string hashed = Convert.ToBase64String(KeyDerivation.Pbkdf2(
            password: password,
            salt: salt,
            prf: KeyDerivationPrf.HMACSHA256,
            iterationCount: 100000,
            numBytesRequested: 256 / 8));


            result.Add(Convert.ToBase64String(salt), hashed);

            return result;
        }

        private bool IsPasswordCorrect(string userPassword, string savedPasswordSalt, string savedPasswordHash)
        {
            string computedHash = Convert.ToBase64String(KeyDerivation.Pbkdf2(
            password: userPassword,
            salt: Convert.FromBase64String(savedPasswordSalt),
            prf: KeyDerivationPrf.HMACSHA256,
            iterationCount: 100000,
            numBytesRequested: 256 / 8));

            if (computedHash != savedPasswordHash)
            {
                return false;
            }

            return true;
                  
        }

        private async Task<UserAccount> GetUser(string Email)
        {
            return await _dbContext.UserAccounts.FirstOrDefaultAsync(m => m.Email == Email);
        }
    }
}
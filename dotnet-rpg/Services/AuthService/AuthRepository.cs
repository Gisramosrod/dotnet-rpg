using dotnet_rpg.Data;
using dotnet_rpg.Utilities;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Runtime.InteropServices;
using System.Security.Claims;

namespace dotnet_rpg.Services.AuthService {
    public class AuthRepository : IAuthRepository {
        private readonly DataContext _context;
        private readonly IConfiguration _configuration;

        public AuthRepository(DataContext context, IConfiguration configuration) {
            _context = context;
            _configuration = configuration;
        }

        public async Task<ServiceResponse<int>> Register(User user, string password) {

            var response = new ServiceResponse<int>();

            if (await UserExist(user.UserName)) {
                response.Success = false;
                response.Message = "User already exists.";
                return response;
            }

            CreatePasswordHash(password, out byte[] passwordHash, out byte[] passwordSalt);

            user.PasswordHash = passwordHash;
            user.PasswordSalt = passwordSalt;

            _context.Add(user);
            await _context.SaveChangesAsync();

            response.Data = user.Id;
            return response;
        }

        public async Task<ServiceResponse<string>> Login(string username, string password) {
            var response = new ServiceResponse<string>();
            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.UserName.ToLower() == username.ToLower());

            if (user == null) {
                response.Success = false;
                response.Message = "User not found.";
                return response;
            }

            if (!VerifyPasswordHash(password, user.PasswordHash, user.PasswordSalt)) {
                response.Success = false;
                response.Message = "Wrong password.";
                return response;
            }

            response.Data = CreateToken(user);
            return response;
        }

        public async Task<bool> UserExist(string username) {
            var userExist = await _context.Users.AnyAsync(u => u.UserName.ToLower() == username.ToLower());
            if (userExist) return true;
            return false;
        }

        private void CreatePasswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt) {
            using (var hmac = new System.Security.Cryptography.HMACSHA512()) {
                passwordSalt = hmac.Key;
                passwordHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
            }
        }

        private bool VerifyPasswordHash(string password, byte[] passwordHash, byte[] passwordSalt) {
            using (var hmac = new System.Security.Cryptography.HMACSHA512(passwordSalt)) {
                var computeHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
                return computeHash.SequenceEqual(passwordHash);
            }
        }

        private string CreateToken(User user) {

            //Create claims
            List<Claim> claims = new List<Claim>() {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.UserName)
            };

            //Get Security Key
            SymmetricSecurityKey key =
                new SymmetricSecurityKey(System.Text.Encoding.UTF8
                .GetBytes(_configuration.GetSection("AppSettings:Token").Value));

            //Create credentials with key 
            SigningCredentials creds = 
                new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);

            //Create token descriptor with claims, an expiration date and with the credentials
            SecurityTokenDescriptor tokenDescriptor = new SecurityTokenDescriptor() {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.Now.AddDays(1),
                SigningCredentials = creds
            };

            //Create token handler
            JwtSecurityTokenHandler tokenHandler = new JwtSecurityTokenHandler();

            //Generate token with the token handler from the token descriptor
            SecurityToken token = tokenHandler.CreateToken(tokenDescriptor);

            //Return token handler with the token generated
            return tokenHandler.WriteToken(token);
        }
    }
}

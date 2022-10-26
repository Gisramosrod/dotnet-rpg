﻿using dotnet_rpg.Utilities;

namespace dotnet_rpg.Services.AuthService {
    public interface IAuthRepository {
        Task<ServiceResponse<int>> Register (User user, string password);
        Task<ServiceResponse<string>> Login (string username, string password);
        Task<bool> UserExist(string username);
    }
}

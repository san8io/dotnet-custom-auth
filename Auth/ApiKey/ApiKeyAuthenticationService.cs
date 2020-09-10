using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Threading.Tasks;

namespace custom_auth.Auth
{
    public interface IApikeyAuthenticationService
    {
        Task<ApiKeyDB> IsValidApiKey(ApiKey apiKey);
    }
    public class ApiKeyAuthenticationService : IApikeyAuthenticationService
    {
        private readonly ApiKeyRepository _repo;
        private readonly PasswordHasher<HashDummy> _hasher;
        private readonly ILogger _logger;
        private const int API_KEY_VERSION = 0;

        public ApiKeyAuthenticationService(ApiKeyRepository repo, PasswordHasher<HashDummy> hasher, ILogger<ApiKeyAuthenticationService> logger)
        {
            _repo = repo;
            _hasher = hasher;
            _logger = logger;
        }

        public async Task<ApiKeyDB> IsValidApiKey(ApiKey apiKey)
        {
            if (DateTime.UtcNow > new DateTime(apiKey.ExpirationDate) || apiKey.Version != API_KEY_VERSION)
            {
                return null;
            }
            ApiKeyDB keyDBInfo = await _repo.GetApiKeyInfoAsync(apiKey);
            PasswordVerificationResult res = _hasher.VerifyHashedPassword(null, keyDBInfo.Hash, apiKey.Secret);
            if (res == PasswordVerificationResult.Failed)
            {
                _logger.LogError($"Secret hash verification failed. Object: {JsonConvert.SerializeObject(keyDBInfo)}");
                return null;
            }
            if (res == PasswordVerificationResult.SuccessRehashNeeded)
            {
                string hash = _hasher.HashPassword(null, apiKey.Secret);
                await _repo.UpdateApiKeyHashAsync(apiKey.Id, hash);
            }
            if (keyDBInfo.State == ApiKeyState.Inactive)
            {
                _logger.LogError($"Api key validation failed. Object: {JsonConvert.SerializeObject(keyDBInfo)}");
                return null;
            }
            return keyDBInfo;
        }
    }

    public class HashDummy
    {

    }
}

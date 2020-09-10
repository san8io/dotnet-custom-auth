using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace custom_auth.Auth
{
    public interface IApiKeyAuthorization
    {
        Task<(Resource, ActionResult)> Authorize(string resourceRef, ClaimsPrincipal user, ActionType[] actionTypes);
    }
    public class ApiKeyAuthorization : IApiKeyAuthorization
    {
        private readonly ILogger _logger;
        private readonly IResourceRepository _resRepo;
        const string ACTIONS_CLAIM_PATTERN = @"^app\.claims\.(?<rid>\d+)$";

        public ApiKeyAuthorization(ILogger<ApiKeyAuthorization> logger, IResourceRepository resRepo)
        {
            _logger = logger;
            _resRepo = resRepo;
        }
        public async Task<(Resource, ActionResult)> Authorize(string resourceRef, ClaimsPrincipal user, ActionType[] actionTypes)
        {
            Resource resource = await _resRepo.GetResourceByResourceRefAsync(resourceRef);
            if (resource == null)
            {
                return (null, new NotFoundResult());
            }
            var authRes = AuthorizeAsync(resource, user.Claims, actionTypes);
            if (authRes != AuthorizationResult.Success)
            {
                _logger.LogWarning($"Unauthorized Authorization Request for apikey {user.Identity.Name} on resource {resourceRef}");
                return (null, new ObjectResult("Not allowed")
                {
                    StatusCode = 403
                });
            }
            return (resource, null);
        }

        private AuthorizationResult AuthorizeAsync(Resource resource, IEnumerable<Claim> claims, ActionType[] resourceActionTypesAllowed)
        {
            var userClaims = claims
                                .Select(c => new { claim = c, match = Regex.Match(c.Type, ACTIONS_CLAIM_PATTERN) })
                                .Where(c => c.match.Success)
                                .Select(c => new { resource = int.Parse(c.match.Groups["rid"].Value), value = c.claim.Value }).ToArray();
            var userActions = userClaims
                                .SelectMany(c => c.value.Split(',')
                                .Select(rt => new Action() { ResourceId = c.resource, ActionType = Enum.Parse<ActionType>(rt) }))
                                .ToArray();

            if (userActions.Where(ur => ur.ResourceId == resource.Id).Join(resourceActionTypesAllowed, ur => ur.ActionType, rr => rr, (ur, rr) => ur).Count() > 0)
            {
                return AuthorizationResult.Success;
            }
            return AuthorizationResult.Failure;
        }
    }

    public enum ResourceType
    {
        Application,
        WeatherModule
    }

    public enum ActionType
    {
        Read,
        Write,
        Delete
    }

    public enum AuthorizationResult
    {
        Success, Failure
    }

    public class Resource
    {
        public int Id { get; set; }
        public ResourceType ResourceType { get; set; }

    }

    public class Action
    {
        public int ResourceId { get; set; }
        public ActionType ActionType { get; set; }
    }
}


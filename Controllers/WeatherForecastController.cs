using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using custom_auth.Auth;

namespace custom_auth.Controllers
{
    [ApiController]
    [Route("{resourceRef}/[controller]")]
    public class WeatherForecastController : ControllerBase
    {
        private readonly IApiKeyAuthorization _auth;
        private static readonly string[] Summaries = new[]
        {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };

        private readonly ILogger<WeatherForecastController> _logger;

        public WeatherForecastController(IApiKeyAuthorization auth, ILogger<WeatherForecastController> logger)
        {
            _auth = auth;
            _logger = logger;
        }

        [HttpGet]
        public async Task<IEnumerable<WeatherForecast>> Get(string resourceRef)
        {
            var (res, ar) = await _auth.Authorize(resourceRef, this.User, new ActionType[] { ActionType.Read });

            var rng = new Random();
            return Enumerable.Range(1, 5).Select(index => new WeatherForecast
            {
                Date = DateTime.Now.AddDays(index),
                TemperatureC = rng.Next(-20, 55),
                Summary = Summaries[rng.Next(Summaries.Length)]
            })
            .ToArray();
        }
    }
}

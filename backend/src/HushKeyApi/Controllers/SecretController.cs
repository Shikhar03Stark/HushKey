using HushKeyApi.Data.Secret;
using HushKeyApi.Models;
using HushKeyCore.Data.Secret;
using HushKeyCore.HushKeyLogger;
using HushKeyService.Service;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;
using System.Security;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace HushKeyApi.Controllers
{
    [Route("api/secrets")]
    [ApiController]
    public class SecretController : ControllerBase
    {
        private readonly IHushKeyLogger _logger;
        private readonly SecretService _secretService;

        public SecretController(IHushKeyLogger logger, SecretService secretService)
        {
            _logger = logger;
            _secretService = secretService;
        }

        // GET api/secrets/symmetric/<secretId>
        [HttpGet("public/{secretId}")]
        public async Task<IActionResult> GetSymmetricEncryptedText(string secretId)
        {
            try
            {
                var secret = await _secretService.GetSymmetricEncryptedSecretFromId(secretId);
                if (secret == null)
                {
                    _logger.Warning($"Secret with ID {secretId} not found.");
                    return NotFound(new { Message = "Secret not found/expired/burned." });
                }
                return Ok(secret);
            }
            catch (Exception ex)
            {
                _logger.Error($"Error retrieving secret with ID {secretId}.", ex);
                throw;
            }
        }

        // POST api/secrets/symmetric
        [HttpPost("public")]
        public async Task<IActionResult> CreateSymmetricEncryptedSecret([FromBody] SecretRequest secretRequest)
        {
            try
            {
                var secretText = secretRequest.SecretText;
                if (string.IsNullOrWhiteSpace(secretText))
                {
                    _logger.Warning("Secret text is empty or null.");
                    return BadRequest(new { Message = "Secret text cannot be empty." });
                }
                var secret = await _secretService.GenerateSymmetricEncryptSecretAsync(secretText, secretRequest.TTL ?? 86400, secretRequest.BurnAfterRead ?? false);
                var response = new SecretResponse()
                {
                    UILink = GetShareableLink(ExtractHostFromRequest(HttpContext), secret, true),
                    ServiceShareableLink = GetShareableLink(ExtractHostFromRequest(HttpContext), secret),
                    ExpiresAt = secret.ExpiresAt,
                };
                _logger.Info($"Symmetric encrypted secret created with ID {secret.EncryptedSecretId}.");
                return CreatedAtAction(nameof(GetSymmetricEncryptedText), new { secretId = secret.EncryptedSecretId }, response);
            }
            catch (Exception ex)
            {
                _logger.Error("Error creating symmetric encrypted secret.", ex);
                return StatusCode(500, new { Message = "Internal server error." });
            }
        }

        private Uri GetShareableLink(string host, SecretModel secretModel, bool isUI = false)
        {
            var uriBuilder = new UriBuilder();
            uriBuilder.Scheme = "https";
            uriBuilder.Host = HostHasPort(host) ? host.Split(":")[0] : host;
            if (HostHasPort(host))
            {
                uriBuilder.Port = int.Parse(host.Split(":")[1]);
            }
            if (isUI)
            {
                uriBuilder.Path = $"/secrets/public/{secretModel.EncryptedSecretId}";
            }
            else
            {
                uriBuilder.Path = $"/api/secrets/public/{secretModel.EncryptedSecretId}";
            }
            uriBuilder.Fragment = secretModel.Key;
            return uriBuilder.Uri;
        }

        private bool HostHasPort(string host)
        {
            return host.Contains(":");
        }

        private string ExtractHostFromRequest(HttpContext httpContext)
        {
            return httpContext.Request.Host.ToString();
        }
    }
}

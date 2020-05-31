using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;

namespace AWSAugment.Cognito.JwtBearer
{
    public class AwsCognitoTokenValidator : ISecurityTokenValidator
    {
        private readonly JwtSecurityTokenHandler _jwtSecurityTokenHandler = new JwtSecurityTokenHandler();
        
        public string RequiredClientId { get; set; }
        public string RequiredScope { get; set; }
        public string Authority { get; set; }
        
        public bool CanReadToken(string securityToken)
        {
            return _jwtSecurityTokenHandler.CanReadToken(securityToken);
        }

        public ClaimsPrincipal ValidateToken
        (
            string securityToken, 
            TokenValidationParameters validationParameters,
            out SecurityToken validatedToken
        )
        {
            var principles =
                _jwtSecurityTokenHandler.ValidateToken(securityToken, validationParameters, out var jwtValidatedToken);

            // Verify client id and scope.
            if (RequiredClientId != null &&
                !principles.HasClaim(_ => 
                    _.Type == "client_id" && 
                    _.Value == RequiredClientId &&
                    (Authority == null || _.Issuer == Authority)) || 
                RequiredScope != null && 
                !principles.HasClaim(_ => 
                    _.Type == "scope" &&
                    _.Value.Split(',').Contains(RequiredScope)))
            {
                principles = new ClaimsPrincipal();
                validatedToken = null;
            }

            // Transform cognito groups to roles.
            var roleClaims = principles.Claims
                .Where(t => t.Type == "cognito:groups")
                .Select(claim => new Claim(ClaimTypes.Role, claim.Value));
            principles.AddIdentity(new ClaimsIdentity(roleClaims));
            
            validatedToken = jwtValidatedToken;
            
            return principles;
        }

        public bool CanValidateToken => _jwtSecurityTokenHandler.CanValidateToken;

        public int MaximumTokenSizeInBytes { get; set; } = TokenValidationParameters.DefaultMaximumTokenSizeInBytes;
    }
}
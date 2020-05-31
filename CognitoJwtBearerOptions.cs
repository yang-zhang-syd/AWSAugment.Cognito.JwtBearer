using System.Runtime.CompilerServices;
using Microsoft.AspNetCore.Authentication.JwtBearer;

namespace AWSAugment.Cognito.JwtBearer
{
    public class CognitoJwtBearerOptions 
    {
        public CognitoJwtBearerOptions(JwtBearerOptions jwtBearerOptions)
        {
            JwtBearerOptions = jwtBearerOptions;
        }
        
        public JwtBearerOptions JwtBearerOptions { get; set; }
        public string ClientId { get; set; }
        public string IdpUrl { get; set; }
    }
}
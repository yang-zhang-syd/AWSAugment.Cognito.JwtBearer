using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;

namespace AWSAugment.Cognito.JwtBearer
{
    public static class JwtBearerExtensions
    {
        public static AuthenticationBuilder AddCognitoJwtBearer
        (
            this AuthenticationBuilder builder,
            Action<CognitoJwtBearerOptions> configureCognitoOptions
        )
        {
            Action<JwtBearerOptions> action = options =>
            {
                var cognitoJwtBearerOptions = new CognitoJwtBearerOptions(options);
                configureCognitoOptions(cognitoJwtBearerOptions);
                
                // AWS cognito jwt token does not have aud included so that we cannot validate audience.
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateAudience = false
                };
                    
                var existingJwtSecurityTokenHandler = options.SecurityTokenValidators
                    .FirstOrDefault(_ => _.GetType() == typeof(JwtSecurityTokenHandler));
                if (existingJwtSecurityTokenHandler != null)
                {
                    options.SecurityTokenValidators.Remove(existingJwtSecurityTokenHandler);
                }
                
                var awsTokenValidator = new AwsCognitoTokenValidator
                {
                    RequiredClientId = cognitoJwtBearerOptions.ClientId,
                    Authority = cognitoJwtBearerOptions.IdpUrl
                };
                    
                options.SecurityTokenValidators.Add(awsTokenValidator);
                options.Authority = cognitoJwtBearerOptions.IdpUrl;
            };
            
            builder.AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, action);

            return builder;
        }
    }
}
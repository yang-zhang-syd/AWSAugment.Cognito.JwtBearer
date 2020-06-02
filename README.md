# AWSAugment.Cognito.JwtBearer

DotNet Core Jwt Bearer extension for AWS Cognito

## Background

JWT token from AWS Cognito signin doesnot include standard `aud` property in the payload. So you cannot verify the audience using your app's client id. This package will validate `client_id` included in the bearer token payload instead.

AWS Cognito User Pools support user groups. However, the user group is in the `cognito:groups` property of the payload. It cannot be directly used as `Roles` in asp.net apps. This package will transform the groups to user roles instead. 

## Example

Following is an example how to use the package. Replace [APP_CLIENT_ID] with your app's client id, [REGION] with your aws region and [USER_POOL_ID] with your user pool id. 

After this, remember to configure the JwtBearerOptions as you would in AddJWTBearer method in the standard `Microsoft.AspNetCore.Authentication.JwtBearer` package.

```C#
public void ConfigureServices(IServiceCollection services)
{
    ......
    
    services
    .AddAuthentication(options => {
        options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    })
    .AddCognitoJwtBearer
    (
        options =>
        {
            options.ClientId = "[APP_CLIENT_ID]";
            options.IdpUrl = "https://cognito-idp.[REGION].amazonaws.com/[USER_POOL_ID]";

            // Configure JwtBearerOptions.
            options.JwtBearerOptions.RequireHttpsMetadata = false;
        }
    );

    ......
}
```

In the asp.net controllers, you could then use the `Authorize` decrator to authorize the requests. In the following example, `Admin` is an user group in AWS Cognito.

```C#
[Route("api/[controller]")]
[Authorize(Roles = "Admin")]
public class ValuesController : ControllerBase
{
    // GET api/values
    [HttpGet]
    public IEnumerable<string> Get()
    {
        return new string[] { "value1", "value2" };
    }
}
```

// See https://aka.ms/new-console-template for more information
using Microsoft.Identity.Client;
using Microsoft.Identity.Client.Broker;
using Microsoft.Identity.Client.Extensions.Msal;
using SvR.AuthenticationDemo.Console;
using System.Net.Http.Headers;

Console.WriteLine("Hello, ESPC23!");

// Create public client application, with native broker support (popup window interacting with Windows credential manager)
var publicClient = PublicClientApplicationBuilder
    .Create("92eb5b3d-51d8-4285-be83-b4726dcf6cec")
    .WithAuthority(AzureCloudInstance.AzurePublic, "svrooij.io")
    .WithDefaultRedirectUri()
    .WithParentActivityOrWindow(BrokerHandle.GetConsoleOrTerminalWindow)
    .WithBroker(new BrokerOptions(BrokerOptions.OperatingSystems.Windows) { Title = "ESPC23 - Authentication demo" })
    .Build();

// Create scopes to request (this is the full identifier of the scope defined on the api)
// {appUri}/{scopeName}
var scopes = new[] { "api://c47ece9b-5c83-49ea-82fd-71df69074581/user_impersonation" };

// Register token cache
var storageProperties = new StorageCreationPropertiesBuilder(".accounts", Path.Combine(Path.GetTempPath(), "auth-demo-espc")).Build();
var cacheHelper = await MsalCacheHelper.CreateAsync(storageProperties);
cacheHelper.RegisterCache(publicClient.UserTokenCache);


Console.WriteLine("Get token with MSAL.NET");
AuthenticationResult? authResult = null;
try
{
    // Check if we already have a previously logged in user
    var accounts = await publicClient.GetAccountsAsync();

    if (accounts is not null && accounts.Any())
    {
        // Try to get a token silently, if this fails, we need to do an interactive login.
        authResult = await publicClient.AcquireTokenSilent(scopes, accounts.FirstOrDefault())
            .ExecuteAsync();
    }
    else
    {
        // No user logged in, do an interactive login.
        authResult = await publicClient.AcquireTokenInteractive(scopes)
            .ExecuteAsync();
    }
}
catch (MsalUiRequiredException)
{
    // User needs to consent, or is not logged in.
    // Lets do an interactive login.
    authResult = await publicClient.AcquireTokenInteractive(scopes)
        .ExecuteAsync();
}

catch (Exception ex)
{
    Console.WriteLine($"Error: {ex.Message}");

}

if (authResult is null)
{
    Console.WriteLine("No authentication result found");
    Console.WriteLine("Press any key to continue");
    Console.ReadLine();
    return;
}

Console.WriteLine($"User: {authResult?.Account?.Username}");
Console.WriteLine($"Tenant: {authResult?.TenantId}");
Console.WriteLine($"Expires: {authResult?.ExpiresOn}");
Console.WriteLine($"AccessToken: {authResult?.AccessToken}");

Console.WriteLine();
Console.WriteLine("Calling API");
Console.WriteLine("Press any key to continue");
Console.ReadLine();

var httpClient = new HttpClient();
httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", authResult!.AccessToken);

var claimsResponse = await httpClient.GetAsync("https://auth-demo.svrooij.io/api/User/Details");
Console.WriteLine("/api/User/Details result:");
if (claimsResponse.IsSuccessStatusCode)
{
    var content = await claimsResponse.Content.ReadAsStringAsync();

    Console.WriteLine(content);
}
else
{
    Console.WriteLine($"Error: {claimsResponse.StatusCode}");
}

Console.WriteLine();
var hiResponse = await httpClient.GetAsync("https://auth-demo.svrooij.io/api/User/hi");
Console.WriteLine("/api/User/hi result:");

if (hiResponse.IsSuccessStatusCode)
{
    var content = await hiResponse.Content.ReadAsStringAsync();

    Console.WriteLine(content);
}
else
{
    Console.WriteLine($"Error: {hiResponse.StatusCode}");
}

Console.WriteLine("Press any key to continue");
Console.ReadLine();
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;

namespace SvR.AuthenticationDemo.Authentication.BasicAuth
{
    internal static class BasicAuthExtensions
    {

        /// <summary>
        /// Enables Basic authentication using the default scheme.
        /// <para>
        /// Basic Auth authentication performs authentication by extracting and validating a Base64 encoded username and password from the <c>Authorization</c> request header.
        /// </para>
        /// </summary>
        /// <param name="builder">The <see cref="AuthenticationBuilder"/>.</param>
        /// <param name="username">The allowed username</param>
        /// <param name="password">The allowed password</param>
        /// <returns>A reference to <paramref name="builder"/> after the operation has completed.</returns>
        /// <remarks>
        /// This Basic Auth handler is just for demo purposes, it is **NOT SECURE** and should **NOT BE USED IN PRODUCTION**!
        /// You can only add one Basic Auth handler, the settings are not programmed to allow multiple handlers.
        /// </remarks>
        public static AuthenticationBuilder AddBasicAuth(this AuthenticationBuilder builder, string username, string password)
        {
            return builder.AddBasicAuth(BasicAuthDefaults.DefaultScheme, BasicAuthDefaults.DefaultScheme, options =>
            {
                options.Username = username;
                options.Password = password;
            });
        }


        /// <summary>
        /// Enables Basic authentication using the specified scheme.
        /// <para>
        /// Basic Auth authentication performs authentication by extracting and validating a Base64 encoded username and password from the <c>Authorization</c> request header.
        /// </para>
        /// </summary>
        /// <param name="builder">The <see cref="AuthenticationBuilder"/>.</param>
        /// <param name="authenticationScheme">The authentication scheme.</param>
        /// <param name="displayName">The display name for the authentication handler.</param>
        /// <param name="configureOptions">A delegate that allows configuring <see cref="JwtBearerOptions"/>.</param>
        /// <returns>A reference to <paramref name="builder"/> after the operation has completed.</returns>
        /// <remarks>
        /// This Basic Auth handler is just for demo purposes, it is not secure and should not be used in production!
        /// You can only add one Basic Auth handler, the settings are not programmed to allow multiple handlers.
        /// </remarks>
        public static AuthenticationBuilder AddBasicAuth(this AuthenticationBuilder builder, string authenticationScheme, string? displayName, Action<StaticBasicAuthOptions> configureOptions)
        {
            builder.Services.Configure(configureOptions);
            return builder.AddScheme<StaticBasicAuthOptions, StaticBasicAuthHandler>(authenticationScheme, displayName, configureOptions);
        }
    }
}

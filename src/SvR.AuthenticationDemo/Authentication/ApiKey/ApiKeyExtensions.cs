using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;

namespace SvR.AuthenticationDemo.Authentication.ApiKey
{
    internal static class ApiKeyExtensions
    {

        /// <summary>
        /// Enables API Key authentication using the default scheme.
        /// <para>
        /// API Key authentication performs authentication by extracting and validating an API key from the <c>X-API-Key</c> request header.
        /// </para>
        /// </summary>
        /// <param name="builder">The <see cref="AuthenticationBuilder"/>.</param>
        /// <param name="keys">The allowed keys</param>
        /// <returns>A reference to <paramref name="builder"/> after the operation has completed.</returns>
        /// <remarks>
        /// This Api Key handler is just for demo purposes, it is **NOT SECURE** and should **NOT BE USED IN PRODUCTION**!
        /// You can only add one Api Key handler, the settings are not programmed to allow multiple handlers.
        /// </remarks>
        public static AuthenticationBuilder AddApiKey(this AuthenticationBuilder builder, string[] keys)
        {
            return builder.AddApiKey(ApiKeyDefaults.DefaultScheme, ApiKeyDefaults.DefaultScheme, options =>
            {
                options.ApiKeys = keys;
            });
        }


        /// <summary>
        /// Enables API Key authentication using the specified scheme.
        /// <para>
        /// API Key authentication performs authentication by extracting and validating an API key from the <c>X-API-Key</c> request header.
        /// </para>
        /// </summary>
        /// <param name="builder">The <see cref="AuthenticationBuilder"/>.</param>
        /// <param name="authenticationScheme">The authentication scheme.</param>
        /// <param name="displayName">The display name for the authentication handler.</param>
        /// <param name="configureOptions">A delegate that allows configuring <see cref="ApiKeyOptions"/>.</param>
        /// <returns>A reference to <paramref name="builder"/> after the operation has completed.</returns>
        /// <remarks>
        /// This Api Key handler is just for demo purposes, it is not secure and should not be used in production!
        /// You can only add one Api Key handler, the settings are not programmed to allow multiple handlers.
        /// </remarks>
        public static AuthenticationBuilder AddApiKey(this AuthenticationBuilder builder, string authenticationScheme, string? displayName, Action<ApiKeyOptions> configureOptions)
        {
            builder.Services.Configure(configureOptions);
            return builder.AddScheme<ApiKeyOptions, ApiKeyHandler>(authenticationScheme, displayName, configureOptions);
        }
    }
}

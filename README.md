# Authentication demo by [@svrooij](https://github.com/svrooij)

This project is to show how easy it is to enable JWT authentication on your api, with tokens from Entra ID.

The swagger ui for this demo api if available [here](https://auth-demo.svrooij.io/swagger/index.html).

## Codetour available

[![CodeTour badge][badge_codetour]][link_codetour]

This project uses [CodeTour][link_codetour] in [Visual Studio Code](https://code.visualstudio.com/) to describe how stuff works. If you want a detailed explanation on how JWT authentication works, I suggest to install this extension and follow the tour.

## Additional information

I blog a lot on security stuff, check it out [svrooij.io](https://svrooij.io)

### Access your api with a managed identity

Now that you have your api protected with a managed identity, you can easily access it with a managed identity.

1. Grant access to your api using [this post](https://svrooij.io/2023/06/19/assign-additional-permissions-to-service-principal/).
2. Add code to get the token.

```csharp
using Azure.Identity;
...

var credentials = new ManagedIdentityCredential();
// Replace the api://.../.default with `{appIDUri}/.default` (so your Application ID URI, with /.default suffix)
var tokenResult = await credentials.GetTokenAsync(new Azure.Core.TokenRequestContext(new[] { "api://0a2dc1ae-040c-4228-9edf-f9e074127323/.default" }));
// the access token is in tokenResult.Token
```

### Sample scripts

I've created some sample http requests you can use to call the api from VSCode as well.

## Don't use Client Secrets

Even though this demo api would allow you to use a secret for client credentials, I'm strongly advising against that. Please use a managed identity and if that is not an option, please use a certificate securely stored in the KeyVault (and accessed with a managed identity).

In [protection against certificate extraction](https://svrooij.io/2022/05/27/certificate-extraction-client-credentials/) I'll explain you all about the risks in someone extracting a secret or certificate.

## During development

During development, if you follewed the tour, you have three options.

1. Automatically get a token using the built-in Token client in the swagger api.
2. Use my [Azure KeyVault Proxy](https://svrooij.io/2022/03/03/keyvault-proxy/) to get a token with your developer credentials, while the certificate is stored in the Key Vault without a way to extract it.
3. Build a small console app that just creates a token for you using the msal client library, and set that in a variable to use during testing.

[badge_codetour]: https://img.shields.io/badge/VSCode-CodeTour-orange?style=for-the-badge&logo=visualstudiocode
[link_codetour]: https://marketplace.visualstudio.com/items?itemName=vsls-contrib.codetour

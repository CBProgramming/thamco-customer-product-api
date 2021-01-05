﻿using IdentityModel.Client;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace HttpManager
{
    public class UnmockablesWrapper : IUnmockablesWrapper
    {
        public async Task<string> GetAccessToken(TokenResponse tokenResponse)
        {
            return tokenResponse != null ?
                tokenResponse.AccessToken :
                "";
        }

        public Task<DiscoveryDocumentResponse> GetDiscoveryDocumentAsync(HttpClient client, string url)
        {
            return string.IsNullOrEmpty(url) ?
                null :
                HttpClientDiscoveryExtensions.GetDiscoveryDocumentAsync(client, url);
        }

        public async Task<string> GetTokenEndPoint(DiscoveryDocumentResponse disco)
        {
            return disco != null ?
                disco.TokenEndpoint :
                "";
        }

        public Task<TokenResponse> RequestClientCredentialsTokenAsync(HttpClient client,
            ClientCredentialsTokenRequest request)
        {
            return request == null ?
                null :
                HttpClientTokenRequestExtensions.RequestClientCredentialsTokenAsync(client, request);
        }
    }
}

// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using IdentityModel.Client;
using Newtonsoft.Json.Linq;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace Client
{
    public class Program
    {
        public static void Main(string[] args)
        {
            MainAsync().GetAwaiter().GetResult();
            while(true) { }
        }

        private static async Task MainAsync()
        {
            // discover endpoints from metadata
            // IdentityModel includes a client library to use with the discovery endpoint.
            // This way you only need to know the base-address of IdentityServer 
            // - the actual endpoints addresses can be read from the metadata:
            var disco = await DiscoveryClient.GetAsync("http://localhost:5000");
            if (disco.IsError)
            {
                Console.WriteLine(disco.Error);
                return;
            }

            // request token
            TokenClient tokenClient = new TokenClient(disco.TokenEndpoint, "client", "secret");
            var tokenResponse = await tokenClient.RequestClientCredentialsAsync("api1");


            if (tokenResponse.IsError)
            {
                Console.WriteLine(tokenResponse.Error);
                return;
            }

            Console.WriteLine(tokenResponse.Json);
            Console.WriteLine("\n\n");

            // call api
            HttpClient client = new HttpClient();

            // To send the access token to the API you typically use the HTTP Authorization header. This is done using the SetBearerToken extension method:
            client.SetBearerToken(tokenResponse.AccessToken);

            var response = await client.GetAsync("http://localhost:5001/identity");
            if (!response.IsSuccessStatusCode)
            {
                Console.WriteLine(response.StatusCode);
            }
            else
            {
                string content = await response.Content.ReadAsStringAsync();
                Console.WriteLine(JArray.Parse(content));
            }


            //requesting token using password grant
            // The token will now contain "sub" claim. The presence or absence of sub claim lets the Api distiguish between calls on behalf of clients and calls on behalf of users
            var tokenClientPasswordGrant = new TokenClient(disco.TokenEndpoint, "ro.client", "secret");
            var tokenResponsePasswordGrant = await tokenClientPasswordGrant.RequestResourceOwnerPasswordAsync("alice", "password", "api1");

            if (tokenResponsePasswordGrant.IsError)
            {
                Console.WriteLine(tokenResponsePasswordGrant.Error);
                return;
            }

            Console.WriteLine(tokenResponsePasswordGrant.Json);
            Console.WriteLine("\n\n");

            // call api

            // To send the access token to the API you typically use the HTTP Authorization header. This is done using the SetBearerToken extension method:
            client.SetBearerToken(tokenResponsePasswordGrant.AccessToken);
            var responsePasswordGrant = await client.GetAsync("http://localhost:5001/identity");
            if (!responsePasswordGrant.IsSuccessStatusCode)
            {
                Console.WriteLine(responsePasswordGrant.StatusCode);
            }
            else
            {
                string content = await responsePasswordGrant.Content.ReadAsStringAsync();
                Console.WriteLine(JArray.Parse(content));
            }
        }
    }
}
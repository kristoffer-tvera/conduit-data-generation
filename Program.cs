using ConduitData.Models;
using Microsoft.Extensions.Configuration;
using RestSharp;
using RestSharp.Authenticators;
using RestSharp.Serializers.SystemTextJson;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;


namespace ConduitData
{
    class Program
    {
        static void Main(string[] args)
        {
            var authToken = GetAuthToken(GetConfig());

            var client = new RestClient("https://eu.api.blizzard.com");
            client.AddDefaultHeader("Authorization", $"Bearer {authToken.AccessToken}");
            client.UseSystemTextJson();

            var conduitIndexResponse = GetConduitIndexResponse(client);
            var conduits = conduitIndexResponse.Conduits.Select(conduit => GetConduitResponse(client, conduit));

            var idiot = conduits.First();

            var journalExpansionResponse = GetJournalExpansionResponse(client);
            var journalEncounterRaidResponse = journalExpansionResponse.Raids.Select(raid => GetJournalInstanceResponse(client, raid.Key));
            var journalEncounterDungeonResponse = journalExpansionResponse.Dungeons.Select(dungeon => GetJournalInstanceResponse(client, dungeon.Key));


            var encounters = new List<Encounter>();
            
            foreach(var raid in journalEncounterRaidResponse)
            {
                encounters.AddRange(raid.Encounters);
            }

            foreach (var dungeon in journalEncounterDungeonResponse)
            {
                encounters.AddRange(dungeon.Encounters);
            }

            //TODO: Worldbosses.

            //var encounterResponses = encounters.Select(encounter => GetJournalEncounterResponse(client, encounter.Key));

            Debugger.Break(); 
        }

        private static JournalInstanceResponse GetJournalInstanceResponse(IRestClient restClient, Key key)
        {
            var uri = new Uri(key.Href);

            var request = new RestRequest(uri.PathAndQuery, Method.GET);
            var response = restClient.Execute<JournalInstanceResponse>(request);

            if (!response.IsSuccessful)
            {
                throw response.ErrorException;
            }

            return response.Data;
        }

        private static JournalEncounterResponse GetJournalEncounterResponse(IRestClient restClient, Key key)
        {
            var uri = new Uri(key.Href);
            var request = new RestRequest(uri.PathAndQuery, Method.GET);
            var response = restClient.Execute<JournalEncounterResponse>(request);

            if (!response.IsSuccessful)
            {
                throw response.ErrorException;
            }

            return response.Data;
        }

        private static JournalExpansionResponse GetJournalExpansionResponse(IRestClient restClient)
        {
            var request = new RestRequest("/data/wow/journal-expansion/499", Method.GET);
            request.AddParameter("namespace", "static-eu");
            var response = restClient.Execute<JournalExpansionResponse>(request);

            if (!response.IsSuccessful)
            {
                throw response.ErrorException;
            }

            return response.Data;
        }

        private static ConduitResponse GetConduitResponse(IRestClient restClient, ConduitData.Models.Conduit conduit)
        {
            var uri = new Uri(conduit.Key.Href);
            var request = new RestRequest(uri.PathAndQuery, Method.GET);
            var response = restClient.Execute<ConduitResponse>(request);

            if (!response.IsSuccessful)
            {
                throw response.ErrorException;
            }

            return response.Data;
        }

        private static ConduitIndexResponse GetConduitIndexResponse(IRestClient restClient)
        {
            var request = new RestRequest("/data/wow/covenant/conduit/index", Method.GET);
            request.AddParameter("namespace", "static-eu");
            var response = restClient.Execute<ConduitIndexResponse>(request);

            if (!response.IsSuccessful)
            {
                throw response.ErrorException;
            }

            return response.Data;
        }

        private static AuthToken GetAuthToken(IConfigurationRoot config)
        {
            var client = new RestClient("https://eu.battle.net");
            client.Authenticator = new HttpBasicAuthenticator(config["Blizzard:ClientId"], config["Blizzard:ClientSecret"]);

            var request = new RestRequest("/oauth/token", Method.POST);
            request.AddParameter("grant_type", "client_credentials");
            var response = client.Execute<AuthToken>(request);

            if (!response.IsSuccessful)
            {
                throw response.ErrorException;
            }

            return response.Data;
        }

        private static IConfigurationRoot GetConfig()
        {
            var builder = new ConfigurationBuilder()
                .AddJsonFile($"appSettings.json", true, true);

            return builder.Build();
        }
    }
}

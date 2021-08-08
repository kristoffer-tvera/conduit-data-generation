using ConduitData.Models;
using Microsoft.Extensions.Configuration;
using RestSharp;
using RestSharp.Authenticators;
using RestSharp.Serializers.SystemTextJson;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConduitData
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var authToken = await GetAuthToken(GetConfig());

            var client = new RestClient("https://eu.api.blizzard.com");
            client.AddDefaultHeader("Authorization", $"Bearer {authToken.AccessToken}");
            client.UseSystemTextJson();

            var conduitIndexResponse = await GetConduitIndexResponse(client);

            var conduits = new List<ConduitResponse>();
            foreach(var conduit in conduitIndexResponse.Conduits)
            {
                conduits.Add(await GetConduitResponse(client, conduit));
            }

            var journalExpansionResponse = await GetJournalExpansionResponse(client);
            var journalExpansionWorldBossesInstanceResponse = await GetJournalInstanceResponseForWorldBosses(client);

            var journalEncounterDungeonResponse = new List<JournalInstanceResponse>();
            foreach(var dungeon in journalExpansionResponse.Dungeons)
            {
                journalEncounterDungeonResponse.Add(await GetJournalInstanceResponse(client, dungeon.Key));
            }

            var journalEncounterRaidResponse = new List<JournalInstanceResponse>();
            foreach (var raid in journalExpansionResponse.Raids)
            {
                journalEncounterRaidResponse.Add(await GetJournalInstanceResponse(client, raid.Key));
            }


            var pepe = GetJournalEncounterResponse(client, journalExpansionWorldBossesInstanceResponse.Encounters.First().Key);

            //var encounters = new List<Encounter>();

            //foreach(var raid in journalEncounterRaidResponse)
            //{
            //    encounters.AddRange(raid.Encounters);
            //}

            //foreach (var dungeon in journalEncounterDungeonResponse)
            //{
            //    encounters.AddRange(dungeon.Encounters);
            //}

            //encounters.AddRange(journalExpansionWorldBossesInstanceResponse.Encounters);

            var conduitWithDropLocations = new List<ConduitViewModel>();
            foreach (var dungeon in journalEncounterDungeonResponse)
            {
                foreach(var encounterIdentifier in dungeon.Encounters)
                {
                    var encounter = await GetJournalEncounterResponse(client, encounterIdentifier.Key);
                    foreach(var itemContainer in encounter.Items)
                    {
                        var conduit = conduits.FirstOrDefault(c => c.Name.EnGB == itemContainer.Item.Name.EnGB);
                        if (conduit == null) continue;
                        conduitWithDropLocations.Add(new ConduitViewModel
                        {
                            Name = conduit.Name.EnGB,
                            Ilvl145 = $"Dungeon ({encounter.Name.EnGB}), Normal",
                            Ilvl158 = $"Dungeon ({encounter.Name.EnGB}), Heroic",
                            Ilvl171 = $"Dungeon ({encounter.Name.EnGB}), Mythic",
                        });

                    }

                }
            }

            var dropLocations = new List<string>();
            foreach (var conduit in conduitWithDropLocations)
            {
                dropLocations.Add($"[\"{conduit.Name}\"] = {{[\"145\"] = \"{conduit.Ilvl145}\", [\"158\"] = \"{conduit.Ilvl158}\", [\"171\"] = \"{conduit.Ilvl171}\", [\"184\"] = \"{conduit.Ilvl184}\", [\"200\"] = \"{conduit.Ilvl200}\", [\"213\"] = \"{conduit.Ilvl213}\", [\"226\"] = \"{conduit.Ilvl226}\", [\"239\"] = \"{conduit.Ilvl239}\", [\"252\"] = \"{conduit.Ilvl252}\"}}");
            }


            var sb = new StringBuilder();
            sb.Append("local name,addon=...;\r\n");
            sb.Append("addon.CONDUIT_DB = ");
            var allDropLocations = string.Join(",\r\n", dropLocations);
            sb.Append(allDropLocations);

            await System.IO.File.WriteAllTextAsync("ConduitDB.lua", sb.ToString());

            Debugger.Break(); 
        }

        private static async Task<JournalInstanceResponse> GetJournalInstanceResponse(IRestClient restClient, Key key)
        {
            var uri = new Uri(key.Href);

            var request = new RestRequest(uri.PathAndQuery, Method.GET);
            var response = await restClient.ExecuteAsync<JournalInstanceResponse>(request);
            
            if (!response.IsSuccessful && response.ErrorException != null) throw response.ErrorException;

            return response.Data;
        }


        private static async Task<JournalInstanceResponse> GetJournalInstanceResponseForWorldBosses(IRestClient restClient)
        {
            // 1192 is the Shadowlands world bosses instance id
            var request = new RestRequest("data/wow/journal-instance/1192");
            request.AddParameter("namespace", "static-eu");
            var response = await restClient.ExecuteAsync<JournalInstanceResponse>(request);

            if (!response.IsSuccessful && response.ErrorException != null) throw response.ErrorException;

            return response.Data;
        }


        private static async Task<JournalEncounterResponse> GetJournalEncounterResponse(IRestClient restClient, Key key)
        {
            var uri = new Uri(key.Href);
            var request = new RestRequest(uri.PathAndQuery, Method.GET);
            var response = await restClient.ExecuteAsync<JournalEncounterResponse>(request);

            if (!response.IsSuccessful && response.ErrorException != null) throw response.ErrorException;
            
            return response.Data;
        }

        private static async Task<JournalExpansionResponse> GetJournalExpansionResponse(IRestClient restClient)
        {
            var request = new RestRequest("/data/wow/journal-expansion/499", Method.GET);
            request.AddParameter("namespace", "static-eu");
            var response = await restClient.ExecuteAsync<JournalExpansionResponse>(request);
            
            if (!response.IsSuccessful && response.ErrorException != null) throw response.ErrorException;

            return response.Data;
        }

        private static async Task<ConduitResponse> GetConduitResponse(IRestClient restClient, ConduitData.Models.Conduit conduit)
        {
            var uri = new Uri(conduit.Key.Href);
            var request = new RestRequest(uri.PathAndQuery, Method.GET);
            var response = await restClient.ExecuteAsync<ConduitResponse>(request);
            
            if (!response.IsSuccessful && response.ErrorException != null) throw response.ErrorException;

            return response.Data;
        }

        private static async Task<ConduitIndexResponse> GetConduitIndexResponse(IRestClient restClient)
        {
            var request = new RestRequest("/data/wow/covenant/conduit/index", Method.GET);
            request.AddParameter("namespace", "static-eu");
            var response = await restClient.ExecuteAsync<ConduitIndexResponse>(request);
            
            if (!response.IsSuccessful && response.ErrorException != null) throw response.ErrorException;

            return response.Data;
        }

        private static async Task<AuthToken> GetAuthToken(IConfigurationRoot config)
        {
            var client = new RestClient("https://eu.battle.net");
            client.Authenticator = new HttpBasicAuthenticator(config["Blizzard:ClientId"], config["Blizzard:ClientSecret"]);

            var request = new RestRequest("/oauth/token", Method.POST);
            request.AddParameter("grant_type", "client_credentials");
            var response = await client.ExecuteAsync<AuthToken>(request);
            
            if (!response.IsSuccessful && response.ErrorException != null) throw response.ErrorException;

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

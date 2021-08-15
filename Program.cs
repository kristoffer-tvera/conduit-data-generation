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
        private static int CASTLE_NATHRIA_ID = 1190;
        private static int SANCTUM_OF_DOMINATION_ID = 1193;
        private static int WORLDBOSS_ID = 1192;
        private static int SHADOWLANDS_ID = 499;
        private static int TAZAVESH_ID = 1194;
        private static string OUTPUT_FILENAME = "ConduitDB.lua";
        private static int[] PVP_VENDOR = { 187506, 182621, 182667, 180935, 182461, 182128, 183470, 181461, 180842, 183478, 182368, 181712, 182480, 182140, 181462, 181837, 181373, 183514, 181944, 183480, 182142, 181848, 181836, 183506, 182748, 182187, 182137, 182325, 182686, 183485, 183197, 182344, 182743, 181498, 182449, 182769, 182109, 182681, 183501, 182624, 182465, 181980, 183184, 181700, 181844, 183491, 183507, 181511, 181737, 182598 };
        static async Task Main(string[] args)
        {
            Console.WriteLine("Starting");

            var authToken = await GetAuthToken(GetConfig());

            var client = new RestClient("https://eu.api.blizzard.com");
            client.AddDefaultHeader("Authorization", $"Bearer {authToken.AccessToken}");
            client.UseSystemTextJson();

            Console.WriteLine("Token aquired, fetching conduits next");

            var conduitIndexResponse = await GetConduitIndexResponse(client);

            var conduits = new List<ConduitResponse>();
            foreach (var conduit in conduitIndexResponse.Conduits)
            {
                conduits.Add(await GetConduitResponse(client, conduit));
            }

            Console.WriteLine("Conduits fetched");

            var journalExpansionResponse = await GetJournalExpansionResponse(client);
            var journalExpansionWorldBossesInstanceResponse = await GetJournalInstanceResponseForWorldBosses(client);

            Console.WriteLine("All dungeons / raids fetched");

            var journalEncounterDungeonResponse = new List<JournalInstanceResponse>();
            foreach (var dungeon in journalExpansionResponse.Dungeons)
            {
                journalEncounterDungeonResponse.Add(await GetJournalInstanceResponse(client, dungeon.Key));
            }

            var journalEncounterRaidResponse = new List<JournalInstanceResponse>();
            foreach (var raid in journalExpansionResponse.Raids)
            {
                journalEncounterRaidResponse.Add(await GetJournalInstanceResponse(client, raid.Key));
            }

            Console.WriteLine("All encounters from dungeons / raids fetched");

            var conduitWithDropLocations = new List<ConduitViewModel>();
            foreach (var dungeon in journalEncounterDungeonResponse)
            {
                foreach (var encounterIdentifier in dungeon.Encounters)
                {
                    var encounter = await GetJournalEncounterResponse(client, encounterIdentifier.Key);
                    foreach (var itemContainer in encounter.Items)
                    {
                        var conduit = conduits.FirstOrDefault(c => c.Name.EnGB == itemContainer.Item.Name.EnGB);
                        if (conduit == null) continue;

                        if (dungeon.Id == TAZAVESH_ID)
                        {
                            conduitWithDropLocations.Add(new ConduitViewModel
                            {
                                Name = conduit.Name.EnGB,
                                Ilvl226 = $"Dungeon ({dungeon.Name.EnGB})",
                            });
                        }
                        else
                        {
                            conduitWithDropLocations.Add(new ConduitViewModel
                            {
                                Name = conduit.Name.EnGB,
                                Ilvl158 = $"Dungeon ({dungeon.Name.EnGB})",
                                Ilvl171 = $"Dungeon ({dungeon.Name.EnGB})",
                                Ilvl184 = $"Dungeon ({dungeon.Name.EnGB})",
                            });
                        }
                    }
                }
            }

            Console.WriteLine("All data for dungeons complete");

            foreach (var raid in journalEncounterRaidResponse)
            {
                foreach (var encounterIdentifier in raid.Encounters)
                {
                    var encounter = await GetJournalEncounterResponse(client, encounterIdentifier.Key);
                    foreach (var itemContainer in encounter.Items)
                    {
                        var conduit = conduits.FirstOrDefault(c => c.Name.EnGB == itemContainer.Item.Name.EnGB);
                        if (conduit == null) continue;

                        if (raid.Id == CASTLE_NATHRIA_ID)
                        {
                            conduitWithDropLocations.Add(new ConduitViewModel
                            {
                                Name = conduit.Name.EnGB,
                                Ilvl184 = $"Raid-boss ({encounter.Name.EnGB}), LFR",
                                Ilvl200 = $"Raid-boss ({encounter.Name.EnGB}), Normal",
                                Ilvl213 = $"Raid-boss ({encounter.Name.EnGB}), Heroic",
                                Ilvl226 = $"Raid-boss ({encounter.Name.EnGB}), Mythic",
                            });
                        }

                        if (raid.Id == SANCTUM_OF_DOMINATION_ID)
                        {
                            conduitWithDropLocations.Add(new ConduitViewModel
                            {
                                Name = conduit.Name.EnGB,
                                Ilvl213 = $"Raid-boss ({encounter.Name.EnGB}), LFR",
                                Ilvl226 = $"Raid-boss ({encounter.Name.EnGB}), Normal",
                                Ilvl239 = $"Raid-boss ({encounter.Name.EnGB}), Heroic",
                                Ilvl252 = $"Raid-boss ({encounter.Name.EnGB}), Mythic",
                            });
                        }
                    }
                }
            }

            Console.WriteLine("All data for raids complete");

            foreach (var worldbossIdentifier in journalExpansionWorldBossesInstanceResponse.Encounters)
            {
                var encounter = await GetJournalEncounterResponse(client, worldbossIdentifier.Key);
                foreach (var itemContainer in encounter.Items)
                {
                    var conduit = conduits.FirstOrDefault(c => c.Name.EnGB == itemContainer.Item.Name.EnGB);
                    if (conduit == null) continue;

                    conduitWithDropLocations.Add(new ConduitViewModel
                    {
                        Name = conduit.Name.EnGB,
                        Ilvl158 = $"World-quests",
                        Ilvl171 = $"World-quests",
                        Ilvl184 = $"World-quests",
                        Ilvl200 = $"World-boss ({encounter.Name.EnGB})",
                    });
                }
            }

            Console.WriteLine("All data for worldbosses complete");

            foreach(var itemId in PVP_VENDOR)
            {
                var item = await GetItemResponse(client, itemId);
                var conduit = conduits.FirstOrDefault(c => c.Name.EnGB == item.Name.EnGB);
                if (conduit == null) continue;

                conduitWithDropLocations.Add(new ConduitViewModel
                {
                    Name = conduit.Name.EnGB,
                    Ilvl213 = $"PvP Vendor (unranked)",
                    Ilvl226 = $"PvP Vendor (1600+)",
                    Ilvl239 = $"PvP Vendor (1800+)",
                    Ilvl252 = $"PvP Vendor (2100+)"
                });
            }

            Console.WriteLine("All data for PvP vendor complete");

            conduitWithDropLocations = conduitWithDropLocations.OrderBy(c => c.Name).ToList();
            var usedArrayKeys = new List<string>();
            var dropLocations = new List<string>();
            foreach (var conduit in conduitWithDropLocations)
            {
                var name = conduit.Name;
                while (usedArrayKeys.Contains(name))
                {
                    name = $"_{name}";
                }

                dropLocations.Add($"[\"{name}\"] = {{[\"145\"] = \"{conduit.Ilvl145}\", [\"158\"] = \"{conduit.Ilvl158}\", [\"171\"] = \"{conduit.Ilvl171}\", [\"184\"] = \"{conduit.Ilvl184}\", [\"200\"] = \"{conduit.Ilvl200}\", [\"213\"] = \"{conduit.Ilvl213}\", [\"226\"] = \"{conduit.Ilvl226}\", [\"239\"] = \"{conduit.Ilvl239}\", [\"252\"] = \"{conduit.Ilvl252}\"}}");
                usedArrayKeys.Add(name);
            }


            var sb = new StringBuilder();
            sb.Append("local name,addon=...;\r\n");
            sb.Append("addon.CONDUIT_DB = {");
            var allDropLocations = string.Join(",\r\n", dropLocations);
            sb.Append(allDropLocations);
            sb.Append("};");

            await System.IO.File.WriteAllTextAsync(OUTPUT_FILENAME, sb.ToString());

            Console.WriteLine($"All data dumped to {OUTPUT_FILENAME}");

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
            var request = new RestRequest($"data/wow/journal-instance/{WORLDBOSS_ID}");
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
            var request = new RestRequest($"/data/wow/journal-expansion/{SHADOWLANDS_ID}", Method.GET);
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

        private static async Task<ItemResponse> GetItemResponse(IRestClient restClient, int itemId)
        {
            var request = new RestRequest($"/data/wow/item/{itemId}", Method.GET);
            request.AddParameter("namespace", "static-eu");
            var response = await restClient.ExecuteAsync<ItemResponse>(request);

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

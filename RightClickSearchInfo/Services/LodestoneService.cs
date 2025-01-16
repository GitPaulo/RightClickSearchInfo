using System;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using NetStone;
using NetStone.Model.Parseables.Search.Character;
using NetStone.Search.Character;
using RightClickSearchInfo.Util;

namespace RightClickSearchInfo.Services;

public class LodestoneService
{
    private readonly LodestoneClient client = LodestoneClient.GetClientAsync().GetAwaiter().GetResult();

    public void OpenCharacterLodestone(string fullName, uint worldId)
    {
        var world = WorldUtils.WorldIdToName(worldId);
        if (world == "Unknown")
        {
            Shared.Chat.Print("Failed to retrieve world name. Opening lodestone without world information.");
        }

        var encodedWorld = Uri.EscapeDataString(world);
        var encodedFullName = Uri.EscapeDataString(fullName);

        Dalamud.Utility.Util.OpenLink(
            $"https://eu.finalfantasyxiv.com/lodestone/character/?q={encodedFullName}&worldname={encodedWorld}");
        Shared.SoundEngine.PlaySound(Shared.SoundNotificationPath);
    }

    public string? GetLodestoneId(string characterName, string serverName)
    {
        try
        {
            // Perform the Lodestone search synchronously
            var searchResults = SearchCharacterOnLodestone(characterName, serverName);

            if (searchResults == null || !searchResults.Results.Any())
            {
                Shared.Log.Warning($"No Lodestone character search results found for {characterName} on {serverName}.");
                return null;
            }

            var lodestoneId = searchResults.Results.FirstOrDefault()?.Id;
            if (lodestoneId == null)
            {
                Shared.Log.Warning($"No Lodestone ID found for {characterName} on {serverName}.");
            }

            return lodestoneId;
        }
        catch (HttpRequestException httpEx)
        {
            Shared.Log.Error(
                $"Network error while fetching Lodestone ID for {characterName} on {serverName}: {httpEx.Message}");
            return null;
        }
        catch (Exception ex)
        {
            Shared.Log.Error(
                $"Unexpected error while fetching Lodestone ID for {characterName} on {serverName}: {ex.Message}");
            return null;
        }
    }

    private CharacterSearchPage? SearchCharacterOnLodestone(string characterName, string serverName)
    {
        var searchQuery = new CharacterSearchQuery
        {
            World = serverName,
            CharacterName = characterName
        };

        try
        {
            var stopwatch = Stopwatch.StartNew();
            var result = client.SearchCharacter(searchQuery).GetAwaiter().GetResult(); // Blocking call
            stopwatch.Stop();

            return result;
        }
        catch (HttpRequestException httpEx)
        {
            Shared.Log.Error($"Network error during character search: {httpEx.Message}");
            return null;
        }
    }
}

using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using System.Threading.Tasks;

namespace HandsomeBot.Models;

public class NextMoveModel() // Class to make next move decision
{
    public NextMoveModel(GameModel game, AllOptionsModel options, Dictionary<string, int> nameToNo) : this()
    {
        theGame = game;
        allOptions = options;
        noToName = nameToNo.ToDictionary(x => x.Value, x => x.Key);
        Moves = [new(noToName), new(noToName)];
    }
    private GameModel theGame = new();
    private AllOptionsModel allOptions = new();
    private Dictionary<int, string> noToName = [];
    public List<MoveModel> Moves { get; set; } = [];
    public async Task UpdateNextMove()
    {
        ParseTurn();
        UpdateSpeeds();
        List<CalcRespModel> damages = await CalcDamages();
        ParseCalc(damages);
        damages = await CalcDamages();
        ChooseNextMove(damages);
    }
    private void ParseTurn()
    {
        if (theGame.Turns.Count < 2) return;
        foreach (EventModel eventModel in theGame.Turns[^2].EventList)
        {
            switch (eventModel.EventType)
            {
                case "Move":
                case "Move Reveal":
                case "Z-Move":
                    ParseMove();
                    break;
                case "Item Activation":
                case "Item Reveal":
                case "Item Change":
                    ParseItem();
                    break;
                case "Ability Activation":
                case "Ability Reveal":
                case "Ability Change":
                    ParseAbility();
                    break;
                case "Forme Reveal":
                case "Forme Change":
                case "Dynamax":
                case "Gigantamax":
                case "Mega Evolution":
                case "Transformation":
                case "Illusioin Reveal":
                    ParseForme();
                    break;
                case "Field Effect Change":
                case "Field Effect Ended":
                    ParseField();
                    break;
                case "Stat Level Change":
                case "Stat Levels Reset":
                    ParseStat();
                    break;
                case "Type Change":
                case "Terastallize":
                    ParseType();
                    break;
                case "Status Change":
                case "Status Activation":
                case "Status Ended":
                    ParseStatus();
                    break;
                case "Switch":
                    ParseSwitch();
                    break;
                case "HP Loss":
                case "Recoil Damage":
                    ParseDamage();
                    break;
                case "KO":
                    ParseKO();
                    break;
            }
        }
    }

    private void ParseMove()
    {
        
    }

    private void ParseAbility()
    {
        
    }

    private void ParseItem()
    {
        
    }

    private void ParseForme()
    {
        
    }

    private void ParseField()
    {
        
    }   

    private void ParseStat()
    {
        
    } 

    private void ParseStatus()
    {
        
    }

    private void ParseType()
    {
        
    }

    private void ParseSwitch()
    {
        
    }

    private void ParseDamage()
    {
        
    }

    private void ParseKO()
    {
        
    }

    private void UpdateSpeeds()
    {
        
    }

    public async Task<List<CalcRespModel>> CalcDamages() // Calculates damage portion of weightings
    {
        ObservableCollection<PokemonModel> botPokemon = []; // Collection of bot's pokemon in server compatable format
        ObservableCollection<PokemonModel> oppPokemon = []; // Collection of opponent's pokemon in server compatable format
        for (int i = 0; i < 2; i++) // Add all mons to collections
        {
            botPokemon.Add(new PokemonModel(theGame.Gen, theGame.BotTeam[theGame.Turns[^1].BotStartMons[i]])); // This needs updating to account for switches in future
            oppPokemon.Add(new PokemonModel(theGame.Gen, theGame.OppTeam[theGame.Turns[^1].OppStartMons[i]]));
        }
        CalcCallModel callData = new() // Collect all data together for calc
        {
            Gen = theGame.Gen,
            BotMons = botPokemon,
            OppMons = oppPokemon,
            Field = new(
                theGame.GameType,
                theGame.CurrentArena
                )
        };
        string callString = JsonSerializer.Serialize(callData); // Serialise call data into string
        HttpClient client = new();
        List<CalcRespModel>? response = await client.GetFromJsonAsync<List<CalcRespModel>>($"http://{theGame.ServerUrl}/calc?{callString}"); // Send data to server and await response
        if (response == null) return []; // Return empty results on null response
        return response;
    }

    private void ParseCalc(List<CalcRespModel> damages)
    {
        
    }

    private void ChooseNextMove(List<CalcRespModel> damages)
    {

    }
}
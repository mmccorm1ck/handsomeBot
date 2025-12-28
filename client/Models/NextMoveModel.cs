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
                    ParseMove(eventModel);
                    break;
                case "Item Activation":
                case "Item Reveal":
                case "Item Change":
                case "Z-Move":
                    ParseItem(eventModel);
                    break;
                case "Ability Activation":
                case "Ability Reveal":
                case "Ability Change":
                    ParseAbility(eventModel);
                    break;
                case "Forme Reveal":
                case "Forme Change":
                case "Dynamax":
                case "Gigantamax":
                case "Mega Evolution":
                    ParseForme(eventModel);
                    break;
                case "Illusion Reveal":
                    ParseZoro(eventModel);
                    break;
                case "Transformation":
                    ParseTransform(eventModel);
                    break;
                case "Field Effect Change":
                case "Field Effect Ended":
                    ParseField(eventModel);
                    break;
                case "Stat Level Change":
                case "Stat Levels Reset":
                    ParseStat(eventModel);
                    break;
                case "Type Change":
                case "Terastallize":
                    ParseType(eventModel);
                    break;
                case "Status Change":
                case "Status Activation":
                case "Status Ended":
                    ParseStatus(eventModel);
                    break;
                case "Switch":
                    ParseSwitch(eventModel);
                    break;
                case "HP Loss":
                case "Recoil Damage":
                    ParseDamage(eventModel);
                    break;
                case "KO":
                    ParseKO(eventModel);
                    break;
            }
        }
    }

    private void ParseMove(EventModel eventModel)
    {
        if (allOptions.AllMoves[eventModel.MoveName].isZ != null)
        {
            ParseItem(eventModel);
            return;
        }
        if (allOptions.AllMoves[eventModel.MoveName].isMax != null)
        {
            ParseForme(eventModel);
            return;
        }
        if (eventModel.UserMon > 5)
        {
            SaveMove(eventModel);
        }
        if (eventModel.MoveName == "Transform")
        {
            ParseTransform(eventModel);
        }
    }

    private void SaveMove(EventModel eventModel)
    {
        if (theGame.OppTeam[eventModel.UserMon - 6].Move1 == "None")
        {
            theGame.OppTeam[eventModel.UserMon - 6].Move1 = eventModel.MoveName;
            return;
        }
        if (theGame.OppTeam[eventModel.UserMon - 6].Move1 == eventModel.MoveName)
        {
            return;
        }
        if (theGame.OppTeam[eventModel.UserMon - 6].Move2 == "None")
        {
            theGame.OppTeam[eventModel.UserMon - 6].Move2 = eventModel.MoveName;
            return;
        }
        if (theGame.OppTeam[eventModel.UserMon - 6].Move2 == eventModel.MoveName)
        {
            return;
        }
        if (theGame.OppTeam[eventModel.UserMon - 6].Move3 == "None")
        {
            theGame.OppTeam[eventModel.UserMon - 6].Move3 = eventModel.MoveName;
            return;
        }
        if (theGame.OppTeam[eventModel.UserMon - 6].Move3 == eventModel.MoveName)
        {
            return;
        }
        if (theGame.OppTeam[eventModel.UserMon - 6].Move4 == "None")
        {
            theGame.OppTeam[eventModel.UserMon - 6].Move4 = eventModel.MoveName;
        }
    }

    private void ParseAbility(EventModel eventModel)
    {
        if (eventModel.AbilityName == "Illusion")
        {
            ParseZoro(eventModel);
            return;
        }
        if (eventModel.UserMon > 5)
        {
            theGame.OppTeam[eventModel.UserMon - 6].Ability = eventModel.AbilityName;
        }
        else if (eventModel.EventType == "Ability Change")
        {
            theGame.BotTeam[eventModel.UserMon].Ability = eventModel.AbilityName;
        }
        if (eventModel.AbilityName == "Imposter")
        {
            ParseTransform(eventModel);
        }
    }

    private void ParseItem(EventModel eventModel)
    {
        if (eventModel.UserMon > 5)
        {
            if (eventModel.EventType == "Z-Move")
            {
                theGame.OppTeam[eventModel.UserMon = 6].Item = "Normalium Z"; // Placeholder before determining correct crystal
            }
            else
            {
                theGame.OppTeam[eventModel.UserMon - 6].Item = eventModel.ItemName;
            }
        }
        if (eventModel.EventType == "Z-Move" ||
            (eventModel.EventType == "Item Activation" && allOptions.SingleUseItems.Contains(eventModel.ItemName)))
        {
            if (eventModel.UserMon > 5)
            {
                theGame.OppTeam[eventModel.UserMon - 6].ItemRemoved = true;
            }
            else
            {
                theGame.BotTeam[eventModel.UserMon].ItemRemoved = true;
            }
        }
    }

    private void ParseForme(EventModel eventModel)
    {
        
    }

    private void ParseField(EventModel eventModel)
    {
        
    }   

    private void ParseStat(EventModel eventModel)
    {
        
    } 

    private void ParseStatus(EventModel eventModel)
    {
        
    }

    private void ParseType(EventModel eventModel)
    {
        
    }

    private void ParseSwitch(EventModel eventModel)
    {
        
    }

    private void ParseDamage(EventModel eventModel)
    {
        
    }

    private void ParseKO(EventModel eventModel)
    {
        
    }

    private void ParseZoro(EventModel eventModel)
    {
        
    }

    private void ParseTransform(EventModel eventModel)
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
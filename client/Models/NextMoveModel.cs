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
                case "Return to Base Forme":
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
        TeamModel tempMon;
        if (eventModel.UserMon > 5)
        {
            tempMon = theGame.OppTeam[eventModel.UserMon - 6];
        }
        else
        {
            tempMon = theGame.BotTeam[eventModel.UserMon];
        }

        if (!allOptions.AllFormes[tempMon.Name].Contains(eventModel.FormeName))
        {
            return;
        }
        if (eventModel.EventType == "Return to Base Forme" && tempMon.BaseForme != "")
        {
            tempMon.Name = tempMon.BaseForme;
            return;
        }
        if (eventModel.EventType == "Forme Reveal")
        {
            tempMon.Name = eventModel.FormeName;
            tempMon.BaseForme = eventModel.FormeName;
            return;
        }
        tempMon.BaseForme = tempMon.Name;
        tempMon.Name = eventModel.FormeName;

        if (eventModel.EventType == "Dynamax")
        {
            tempMon.TurnDynamaxed = theGame.Turns[^2].TurnNo;
        }
        if (eventModel.EventType == "Gigantamax")
        {
            tempMon.TurnDynamaxed = theGame.Turns[^2].TurnNo;
            tempMon.GMax = true;
        }
        if (eventModel.EventType == "Mega Evolution" && eventModel.UserMon > 5)
        {
            tempMon.Item = "Abomasite"; // Place holder for correct mega stone
        }
    }

    private void ParseField(EventModel eventModel)
    {
        if (_bothSideFieldEffects.Contains(eventModel.FieldChange))
        {
            if (eventModel.EventType == "Field Effect Ended")
            {
                theGame.CurrentArena.RemoveEffect(eventModel.FieldChange);
                return;
            }
            theGame.CurrentArena.AddEffect(eventModel.FieldChange);
            return;
        }
        if (_userSideFieldEffects.Contains(eventModel.FieldChange))
        {
            if (eventModel.EventType == "Field Effect Ended")
            {
                theGame.CurrentArena.BotSide.RemoveEffect(eventModel.FieldChange);
                return;
            }
            theGame.CurrentArena.BotSide.AddEffect(eventModel.FieldChange);
            return;
        }
        if (eventModel.EventType == "Field Effect Ended")
        {
            theGame.CurrentArena.OppSide.RemoveEffect(eventModel.FieldChange);
            return;
        }
        theGame.CurrentArena.OppSide.AddEffect(eventModel.FieldChange);
        return;
    }

    private void ParseStat(EventModel eventModel)
    {
        TeamModel tempMon;
        if (eventModel.UserMon > 5)
        {
            tempMon = theGame.OppTeam[eventModel.UserMon - 6];
        }
        else
        {
            tempMon = theGame.BotTeam[eventModel.UserMon];
        }

        if (eventModel.EventType == "Stat Levels Reset")
        {
            tempMon.StatChanges = new();
            return;
        }
        if (eventModel.StatAdjustment == "Returned to Normal")
        {
            tempMon.StatChanges.SetStat(eventModel.StatChange, 0);
            return;
        }
        tempMon.StatChanges.IncrementStat(eventModel.StatChange,
            _statAdjustmentDictionary[eventModel.StatAdjustment]);
    }

    private void ParseStatus(EventModel eventModel)
    {
        TeamModel tempMon;
        if (eventModel.UserMon > 5)
        {
            tempMon = theGame.OppTeam[eventModel.UserMon - 6];
        }
        else
        {
            tempMon = theGame.BotTeam[eventModel.UserMon];
        }

        if (_nonVolStatuses.Contains(eventModel.StatusChange))
        {
            if (eventModel.EventType == "Status Ended")
            {
                tempMon.NonVolStatus = "";
                return;
            }
            if (tempMon.NonVolStatus != "")
            {
                return;
            }
            tempMon.NonVolStatus = eventModel.StatusChange;
            return;
        }
        if (tempMon.VolStatus.Contains(eventModel.StatusChange))
        {
            if (eventModel.EventType == "Status Ended")
            {
                tempMon.VolStatus.Remove(eventModel.StatusChange);
            }
            return;
        }
        tempMon.VolStatus.Add(eventModel.StatusChange);
    }

    private void ParseType(EventModel eventModel)
    {
        TeamModel tempMon;
        if (eventModel.UserMon > 5)
        {
            tempMon = theGame.OppTeam[eventModel.UserMon - 6];
        }
        else
        {
            tempMon = theGame.BotTeam[eventModel.UserMon];
        }

        if (eventModel.EventType == "Terastallize")
        {
            tempMon.TeraActive = true;
            if (eventModel.UserMon > 5 && tempMon.Tera == "None")
            {
                tempMon.Tera = eventModel.TypeChange;
            }
            return;
        }
        tempMon.TypeChange = eventModel.TypeChange;
    }

    private void ParseSwitch(EventModel eventModel)
    {
        if (eventModel.UserMon > 5 && eventModel.TargetMons[0].MonNo > 5)
        {
            theGame.Turns[^2].OppEndMons[theGame.Turns[^2].OppEndMons.IndexOf(eventModel.UserMon - 6)] =
                eventModel.TargetMons[0].MonNo - 6;
            if (!theGame.MonsSeen.Contains(eventModel.TargetMons[0].MonNo - 6))
            {
                theGame.MonsSeen.Add(eventModel.TargetMons[0].MonNo - 6);
            }
            return;
        }
        if (eventModel.UserMon < 6 && eventModel.TargetMons[0].MonNo < 6)
        {
            theGame.Turns[^2].BotEndMons[theGame.Turns[^2].BotEndMons.IndexOf(eventModel.UserMon)] =
                eventModel.TargetMons[0].MonNo;
        }
    }

    private void ParseDamage(EventModel eventModel)
    {
        if (eventModel.UserMon < 6)
        {
            theGame.BotTeam[eventModel.UserMon].RemainingHP = eventModel.RemainingHP;
        }
        else
        {
            theGame.OppTeam[eventModel.UserMon - 6].RemainingHP = eventModel.RemainingHP;
        }
    }

    private void ParseKO(EventModel eventModel)
    {
        if (eventModel.UserMon < 6)
        {
            theGame.BotTeam[eventModel.UserMon].RemainingHP = 0;
            theGame.BotTeam[eventModel.UserMon].Position = "KO";
        }
        else
        {
            theGame.OppTeam[eventModel.UserMon - 6].RemainingHP = 0;
            theGame.OppTeam[eventModel.UserMon - 6].Position = "KO";
        }
    }

    private void ParseZoro(EventModel eventModel)
    {
        if (eventModel.UserMon < 6)
        {
            return;
        }
        if (!theGame.ZoroPresent)
        {
            return;
        }

        if (eventModel.EventType != "Illusion Reveal" &&
            !(eventModel.EventType.Contains("Ability") && eventModel.AbilityName == "Illusion"))
        {
            return;
        }
        int zoroNum;
        for (zoroNum = 0; zoroNum < 6; zoroNum++)
        {
            if (!theGame.OppTeam[zoroNum].Name.Contains("Zoroark") && !theGame.OppTeam[zoroNum].Name.Contains("Zorua"))
            {
                continue;
            }
            theGame.Turns[^2].OppEndMons[theGame.Turns[^2].OppEndMons.IndexOf(eventModel.UserMon - 6)] = zoroNum;
            if (!theGame.MonsSeen.Contains(zoroNum))
            {
                theGame.MonsSeen.Add(zoroNum);
            }
            break;
        }
        List<EventModel> eventList = theGame.Turns[^2].EventList;
        for (int i = eventModel.EventNo; i <= 0; i--)
        {
            foreach (TargetModel target in eventList[i].TargetMons)
            {
                if (target.MonNo == eventModel.UserMon)
                {
                    target.MonName = theGame.OppTeam[zoroNum].Name;
                    if (eventList[i].EventType == "Switch")
                    {
                        return;
                    }
                }
            }
            if (eventList[i].UserMon == eventModel.UserMon)
            {
                eventList[i].UserMon = zoroNum;
            }
        }
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
    private readonly Dictionary<string, int> _statAdjustmentDictionary = new()
    {
        {"Rose", 1},
        {"Rose Sharply", 2},
        {"Rose Drastically (+3)", 3},
        {"Rose Drastically (+4)", 4},
        {"Rose Drastically (+5)", 5},
        {"Rose Drastically (+6)", 6},
        {"Won't go any Higher", 0},
        {"Fell", -1},
        {"Harshly Fell", -2},
        {"Severely Fell (-3)", -3},
        {"Severely Fell (-4)", -4},
        {"Severely Fell (-5)", -5},
        {"Severely Fell (-6)", -6},
        {"Won't go any Lower", 0}
    };
    private readonly List<string> _nonVolStatuses = [
        "Burn",
        "Freeze",
        "Paralysis",
        "Poison",
        "Badly Poisoned",
        "Sleep",
        "Frostbite"
    ];
    private readonly List<string> _bothSideFieldEffects = [
        "Rain",
        "Harsh Sunlight",
        "Snow",
        "Hail",
        "Sandstorm",
        "Extremely Harsh Sunlight",
        "Heavy Rain",
        "Strong Winds",
        "Electric Terrain",
        "Psychic Terrain",
        "Grassy Terrain",
        "Misty Terrain",
        "Magic Room",
        "Trick Room",
        "Wonder Room",
        "Gravity",
        "Mud Sport",
        "Water Sport",
    ];
    private readonly List<string> _userSideFieldEffects = [
        "Tailwind",
        "Rainbow",
        "Reflect",
        "Light Screen",
        "Aurora Veil"
    ];
}
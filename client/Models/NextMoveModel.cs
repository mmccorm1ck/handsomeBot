using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using System.Threading.Tasks;
using System.Web;

namespace HandsomeBot.Models;

public class NextMoveModel() // Class to make next move decision
{
    public NextMoveModel(GameModel game, AllOptionsModel options, Dictionary<string, int> nameToNo) : this()
    {
        theGame = game;
        allOptions = options;
        _nameToNo = nameToNo;
        noToName = nameToNo.ToDictionary(x => x.Value, x => x.Key);
        Moves = [new(noToName), new(noToName)];
    }
    private GameModel theGame = new();
    private AllOptionsModel allOptions = new();
    private Dictionary<string, int> _nameToNo = [];
    private Dictionary<int, string> noToName = [];
    public List<MoveModel> Moves { get; set; } = [];
    public async Task UpdateNextMove()
    {
        ParseTurn();
        List<CalcRespModel> damages = await CalcDamages();
        ParseCalc(damages);
        List<int> turnOrder = UpdateSpeeds();
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
                case "HP Restored":
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
        if (theGame.OppTeam[eventModel.UserMon - 6].Moves.Count < 4)
        {
            theGame.OppTeam[eventModel.UserMon - 6].Moves.Add(eventModel.MoveName);
        }
            /*return;
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
        }*/
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
            if (eventModel.EventType == "Ability Activation")
            {
                theGame.OppTeam[eventModel.UserMon - 6].AbilityActive = true;
            }
        }
        else if (eventModel.EventType == "Ability Change")
        {
            theGame.BotTeam[eventModel.UserMon].Ability = eventModel.AbilityName;
        }
        else if (eventModel.EventType == "Ability Activation")
        {
            theGame.BotTeam[eventModel.UserMon].AbilityActive = true;
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
            (eventModel.EventType == "Item Activation" && (allOptions.SingleUseItems.Contains(eventModel.ItemName) ||
            eventModel.ItemName.Contains(" Berry") || eventModel.ItemName.Contains(" Gem"))))
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
        TeamModel tempMon = eventModel.UserMon > 5 ?
            theGame.OppTeam[eventModel.UserMon - 6] : theGame.BotTeam[eventModel.UserMon];

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
            if (!_monData.Keys.Contains(tempMon.Name))
            {
                return;
            }
            if (_monData[tempMon.Name].abilities["0"] == null)
            {
                return;
            }
            tempMon.MegaAbility = _monData[tempMon.Name].abilities["0"];
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
        TeamModel tempMon = eventModel.UserMon > 5 ?
            theGame.OppTeam[eventModel.UserMon - 6] : theGame.BotTeam[eventModel.UserMon];

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
            _statAdjustmentDictionary[eventModel.StatAdjustment], "Stage");
    }

    private void ParseStatus(EventModel eventModel)
    {
        TeamModel tempMon = eventModel.UserMon > 5 ?
            theGame.OppTeam[eventModel.UserMon - 6] : theGame.BotTeam[eventModel.UserMon];

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
        TeamModel tempMon = eventModel.UserMon > 5 ?
            theGame.OppTeam[eventModel.UserMon - 6] : theGame.BotTeam[eventModel.UserMon];

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
            theGame.OppTeam[eventModel.UserMon - 6].Transform = null;
            theGame.OppTeam[eventModel.UserMon - 6].AbilityActive = false;
            return;
        }
        if (eventModel.UserMon < 6 && eventModel.TargetMons[0].MonNo < 6)
        {
            theGame.Turns[^2].BotEndMons[theGame.Turns[^2].BotEndMons.IndexOf(eventModel.UserMon)] =
                eventModel.TargetMons[0].MonNo;
            theGame.BotTeam[eventModel.UserMon].Transform = null;
            theGame.BotTeam[eventModel.UserMon].AbilityActive = false;
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
        TeamModel tempMon = eventModel.UserMon > 5 ?
            theGame.OppTeam[eventModel.UserMon - 6] : theGame.BotTeam[eventModel.UserMon];
        TeamModel targetMon = eventModel.TargetMons[0].MonNo > 5 ?
            theGame.OppTeam[eventModel.TargetMons[0].MonNo - 6] : theGame.BotTeam[eventModel.TargetMons[0].MonNo];

        tempMon.Transform = targetMon.CloneForTransform();
        tempMon.Transform.Level = tempMon.Level;
        tempMon.Transform.Item = tempMon.Item;
        tempMon.Transform.ItemRemoved = tempMon.ItemRemoved;
        tempMon.Transform.Tera = tempMon.Tera;
        tempMon.Transform.TeraActive = tempMon.TeraActive;
        tempMon.Transform.RemainingHP = tempMon.RemainingHP;
        tempMon.Transform.VolStatus = tempMon.VolStatus;
        tempMon.Transform.NonVolStatus = tempMon.NonVolStatus;
    }

    private void ParseNature(TeamModel teamModel)
    {
        if (teamModel.NatureBoost == null)
        {
            teamModel.Nature = "Hardy";
            return;
        }
        if (teamModel.NatureDrop == null)
        {
            if (teamModel.NatureBoost == "Atk")
            {
                teamModel.NatureDrop = "SpA";
            }
            else if (teamModel.NatureBoost == "SpA")
            {
                teamModel.NatureDrop = "Atk";
            }
            else
            {
                return;
            }
        }
        foreach (string nature in _natures.Keys)
        {
            if (_natures[nature].ContainsKey(teamModel.NatureBoost) &&
                _natures[nature].ContainsKey(teamModel.NatureDrop))
            {
                if (_natures[nature][teamModel.NatureBoost] == 1.1)
                {
                    teamModel.Nature = nature;
                    return;
                }
            }
        }
    }

    private List<int> UpdateSpeeds()
    {
        int turnPos = 100;
        Dictionary<int, List<int>> eventOrders = [];
        foreach (EventModel eventModel in theGame.Turns[^2].EventList)
        {
            int priority = DecidePriority(eventModel);
            if (priority > turnPos)
            {
                continue;
            }
            turnPos = priority;
            if (!eventOrders[priority].Contains(eventModel.UserMon))
            {
                eventOrders[priority].Add(eventModel.UserMon);
            }
        }
        Dictionary<int, int> speeds = [];
        foreach (int monNo in noToName.Keys)
        {
            speeds.Add(monNo, CalcStat("Spe", monNo));
        }
        List<int> speedOrder = [];
        foreach (int monNo in speeds.Keys)
        {
            if (speedOrder.Count == 0)
            {
                speedOrder.Add(monNo);
                continue;
            }
            for (int i = 0; i < speedOrder.Count; i++)
            {
                if (speeds[monNo] <= speeds[speedOrder[i]])
                {
                    continue;
                }
                speedOrder.Insert(i, monNo);
            }
            speedOrder.Add(monNo);
        }
        foreach (List<int> order in eventOrders.Values)
        {
            if (order.Count == 1)
            {
                continue;
            }
            for (int i = order.Count - 2; i >= 0; i--)
            {
                if (speedOrder.IndexOf(order[i]) > speedOrder.IndexOf(order[i + 1]))
                {
                    speedOrder.Remove(order[i]);
                    speedOrder.Insert(speedOrder.IndexOf(order[i + 1]), order[i]);
                }
            }
        }
        for (int i = speedOrder.Count - 1; i >= 0; i--)
        {
            if (speedOrder[i] < 6)
            {
                continue;
            }
            int monNo = speedOrder[i];
            TeamModel mon = theGame.OppTeam[monNo - 6];
            int max = i > 0 ? speeds[speedOrder[i - 1]] : 10000;
            int min = i < speedOrder.Count - 1 ? speeds[speedOrder[i + 1]] : 0;
            max = max < min ? min : max;
            string? prevBoost = null;
            string? prevDrop = null;
            while (true)
            {
                if (speeds[monNo] < min)
                {
                    if (mon.EV.Spe <= 248)
                    {
                        mon.EV.Spe += 4;
                    }
                    else if (mon.IV.Spe < 31)
                    {
                        mon.IV.Spe++;
                    }
                    else if (mon.NatureBoost != "Spe")
                    {
                        prevBoost = mon.NatureBoost;
                        if (mon.NatureDrop == "Spe")
                        {
                            mon.NatureDrop = null;
                        }
                        else
                        {
                            mon.NatureBoost = "Spe";
                        }
                    }
                    else
                    {
                        // Parse items/abilities etc
                        break; // break to prevent infinite loops
                    }
                    speeds[monNo] = CalcStat("Spe", monNo);
                    continue;
                }
                if (speeds[monNo] > max)
                {
                    if (mon.EV.Spe >= 4)
                    {
                        mon.EV.Spe -= 4;
                    }
                    else if (mon.IV.Spe > 0)
                    {
                        mon.IV.Spe--;
                    }
                    else if (mon.NatureDrop != "Spe")
                    {
                        prevDrop = mon.NatureDrop;
                        if (mon.NatureBoost == "Spe")
                        {
                            mon.NatureBoost = null;
                        }
                        else
                        {
                            mon.NatureDrop = "Spe";
                        }
                    }
                    else
                    {
                        // Parse items/abilities etc
                        break;// break to prevent infinite loops
                    }
                    speeds[monNo] = CalcStat("Spe", monNo);
                    continue;
                }
                break;
            }
            if (mon.NatureBoost == null && prevBoost != null && prevBoost != mon.NatureDrop)
            {
                mon.NatureBoost = prevBoost;
            }
            if (mon.NatureDrop == null && prevDrop != null && prevDrop != mon.NatureBoost)
            {
                mon.NatureDrop = prevDrop;
            }
            ParseNature(mon);
        }
        return speedOrder;
    }

    private int DecidePriority(EventModel eventModel)
    {
        if (!_priorities.Keys.Contains(eventModel.EventType))
        {
            return 1000;
        }
        int priority = _priorities[eventModel.EventType];
        if (eventModel.EventType != "Move")
        {
            return priority;
        }
        MoveInfoModel moveInfo = allOptions.AllMoves[eventModel.MoveName];
        TeamModel userMon = eventModel.UserMon > 5 ?
            theGame.OppTeam[eventModel.UserMon - 6] : theGame.BotTeam[eventModel.UserMon];
        if (userMon.Transform != null)
        {
            userMon = userMon.Transform;
        }
        if (moveInfo.priotity != null)
        {
            priority += (int)moveInfo.priotity;
        }
        if (moveInfo.category == "Status" && userMon.Ability == "Prankster")
        {
            priority++;
        }
        if (moveInfo.type == "Flying" && userMon.Ability == "Gale Wings" && userMon.RemainingHP == 100)
        {
            priority++;
        }
        if (_healingMoves.Contains(eventModel.MoveName) && userMon.Ability == "Triage")
        {
            priority += 3;
        }
        return priority;
    }

    public async Task LoadMonData()
    {
        HttpClient client = new();
        List<string> monsForCall = [];
        foreach (TeamModel mon in theGame.BotTeam)
        {
            foreach (string name in allOptions.AllFormes[mon.Name])
            {
                if (!monsForCall.Contains(name))
                {
                    monsForCall.Add(name);
                }
            }
        }
        foreach (TeamModel mon in theGame.OppTeam)
        {
            foreach (string name in allOptions.AllFormes[mon.Name])
            {
                if (!monsForCall.Contains(name))
                {
                    monsForCall.Add(name);
                }
            }
        }
        string url = "http://" + theGame.ServerUrl + "/mons?{%22Gen%22:" + theGame.Gen.ToString() +
            "%2c%22Filter%22:" + HttpUtility.UrlEncode(JsonSerializer.Serialize(monsForCall)) + "}";
        Dictionary<string, MonData>? response = await client.GetFromJsonAsync<Dictionary<string, MonData>>(url);
        if (response == null) return;
        _monData = response;
        UpdateDefaultAbilities();
    }

    public void UpdateDefaultAbilities()
    {
        foreach (TeamModel mon in theGame.OppTeam)
        {
            if (!_monData.Keys.Contains(mon.Name))
            {
                continue;
            }
            if (_monData[mon.Name].abilities["0"] == null)
            {
                continue;
            }
            mon.AbilityDefault = _monData[mon.Name].abilities["0"];
        }
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
        Dictionary<int, Dictionary<int, List<float>>> expectedDamages = [];
        foreach (CalcRespModel damage in damages)
        {
            int monNo;
            int targetMon;
            if (damage.BotUser)
            {
                monNo = _nameToNo[damage.UserMon];
                targetMon = _nameToNo["Opponent's" + damage.TargetMon];
            }
            else
            {
                monNo = _nameToNo["Opponent's " + damage.UserMon];
                targetMon = _nameToNo[damage.TargetMon];
            }
            if (expectedDamages.TryGetValue(monNo, out Dictionary<int, List<float>>? value))
            {
                value.Add(targetMon, [damage.MinDamage, damage.MaxDamage]);
            }
            else
            {
                expectedDamages.Add(
                    monNo, new(){{targetMon, [damage.MinDamage, damage.MaxDamage]}}
                );
            }
        }
        if (theGame.Turns.Count < 2) return;
        foreach (EventModel eventModel in theGame.Turns[^2].EventList)
        {
            if (eventModel.EventType != "Move")
            {
                continue;
            }
            if (!allOptions.AllMoves.ContainsKey(eventModel.MoveName))
            {
                continue;
            }
            if (allOptions.AllMoves[eventModel.MoveName].category == "Status")
            {
                continue;
            }
            foreach (TargetModel target in eventModel.TargetMons)
            {
                if (target.MoveResult == "Immune" || target.MoveResult == "Miss" ||
                    target.MoveResult == "Failed" || target.MoveResult == "")
                {
                    continue;
                }
                float maxExpected = expectedDamages[eventModel.UserMon][target.MonNo][1];
                float minExpected = (float)Math.Floor(expectedDamages[eventModel.UserMon][target.MonNo][0]);
                if (target.Damage >= minExpected && target.Damage <= maxExpected)
                {
                    continue;
                }
            }
        }
    }

    private void ChooseNextMove(List<CalcRespModel> damages)
    {

    }

    private int CalcStat(string stat, int monNo)
    {
        TeamModel tempMon = monNo > 5 ? theGame.OppTeam[monNo - 6] : theGame.BotTeam[monNo];
        if (tempMon.Transform != null)
        {
            tempMon = tempMon.Transform;
        }
        if (!_monData.ContainsKey(tempMon.Name))
        {
            return -1;
        }
        int? baseStat = stat switch
        {
            "Atk" => _monData[tempMon.Name].bs.at,
            "Def" => _monData[tempMon.Name].bs.df,
            "SpA" => _monData[tempMon.Name].bs.sa,
            "SpD" => _monData[tempMon.Name].bs.sd,
            "Spe" => _monData[tempMon.Name].bs.sp,
            _ => -1
        };
        int iv = stat switch
        {
            "Atk" => tempMon.IV.Atk,
            "Def" => tempMon.IV.Def,
            "SpA" => tempMon.IV.SpA,
            "SpD" => tempMon.IV.SpD,
            "Spe" => tempMon.IV.Spe,
            _ => -1
        };
        int ev = stat switch
        {
            "Atk" => tempMon.EV.Atk,
            "Def" => tempMon.EV.Def,
            "SpA" => tempMon.EV.SpA,
            "SpD" => tempMon.EV.SpD,
            "Spe" => tempMon.EV.Spe,
            _ => -1
        };
        int statChange = stat switch
        {
            "Atk" => tempMon.StatChanges.Atk,
            "Def" => tempMon.StatChanges.Def,
            "SpA" => tempMon.StatChanges.SpA,
            "SpD" => tempMon.StatChanges.SpD,
            "Spe" => tempMon.StatChanges.Spe,
            _ => 0
        };
        double statChangeMult = CalcStatMult(statChange);
        double natureMod = 1;
        if (_natures[tempMon.Nature].ContainsKey(stat))
        {
            natureMod = _natures[tempMon.Nature][stat];
        }
        else if (tempMon.NatureBoost == stat) // If exact nature hasn't been determined yet
        {
            natureMod = 1.1;
        }
        else if (tempMon.NatureDrop == stat)
        {
            natureMod = 0.9;
        }
        if (baseStat == -1 || iv == -1 || ev == -1 || baseStat == null)
        {
            return -1;
        }
        int calcedStat = (int)Math.Floor(Math.Floor((Math.Floor((double)(
            (2 * baseStat + iv + Math.Floor(ev / 4.0)) * tempMon.Level / 100.0)) + 5) * natureMod) * statChangeMult);
        if (stat != "Spe") // Damage-based stat modifiers are handled by calc code
        {
            return calcedStat;
        }

        if (tempMon.Item == "Choice Scarf")
        {
            calcedStat = (int)Math.Floor(calcedStat * 1.5);
        }
        else if (tempMon.Item == "Quick Powder" && tempMon.Name == "Ditto" && tempMon.Transform == null)
        {
            calcedStat *= 2;
        }
        if ((tempMon.Ability == "Protosynthesis (Speed)" || tempMon.Ability == "Quark Drive (Speed)") && tempMon.AbilityActive)
        {
            calcedStat = (int)Math.Floor(calcedStat * 1.5);
        }
        else if (tempMon.Ability == "Chlorophyll" && theGame.CurrentArena.Weather.Contains("Harsh Sunlight"))
        {
            calcedStat *= 2;
        }
        else if (tempMon.Ability == "Swift Swim" && theGame.CurrentArena.Weather.Contains("Rain"))
        {
            calcedStat *= 2;
        }
        else if (tempMon.Ability == "Sand Rush" && theGame.CurrentArena.Weather == "Sandstorm")
        {
            calcedStat *= 2;
        }
        else if (tempMon.Ability == "Slush Rush" &&
            (theGame.CurrentArena.Weather == "Snow" || theGame.CurrentArena.Weather == "Hail"))
        {
            calcedStat *= 2;
        }
        else if (tempMon.Ability == "Quick Feet" && (tempMon.NonVolStatus != "" || tempMon.VolStatus.Count > 0))
        {
            calcedStat = (int)Math.Floor(calcedStat * 1.5);
        }
        else if (tempMon.Ability == "Unburden" && tempMon.ItemRemoved)
        {
            calcedStat *= 2;
        }
        else if (tempMon.Ability == "Surge Surfer" && theGame.CurrentArena.Terrain == "Electric Terrain")
        {
            calcedStat *= 2;
        }
        if (tempMon.NonVolStatus == "Paralysis" && tempMon.Ability != "Quick Feet")
        {
            calcedStat = (int)Math.Floor(calcedStat / 2.0);
        }
        if ((monNo < 6 && theGame.CurrentArena.BotSide.Tailwind) ||
            (monNo > 5 && theGame.CurrentArena.OppSide.Tailwind))
        {
            calcedStat *= 2;
        }
        return calcedStat;
    }

    private static double CalcStatMult(int statChange)
    {
        if (statChange >= 0)
        {
            return 1.0 + 0.5 * statChange;
        }
        return 2.0 / (2 - statChange);
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
    private readonly Dictionary<string, int> _priorities = new()
    {
        {"Switch", 20},
        {"Ability Activation", 15},
        {"Mega Evolution", 10},
        {"Dynamax", 10},
        {"Gigantamax", 10},
        {"Terastallize", 10},
        {"Move", 0}
    };
    private readonly List<string> _healingMoves = [
        "Absorb",
        "Bitter Blade",
        "Drain Punch",
        "Draining Kiss",
        "Dream Eater",
        "Floral Healing",
        "Giga Drain",
        "Heal Order",
        "Heal Pulse",
        "Healing Wish",
        "Horn Leech",
        "Leech Life",
        "Lunar Blessing",
        "Lunar Dance",
        "Macha Gotcha",
        "Mega Drain",
        "Milk Drink",
        "Moonlight",
        "Morning Sun",
        "Oblivion Wing",
        "Parabolic Charge",
        "Purify",
        "Recover",
        "Rest",
        "Revival Blessing",
        "Roost",
        "Shore Up",
        "Slack Off",
        "Soft-Boiled",
        "Strength Sap",
        "Swallow",
        "Synthesis",
        "Wish"
    ];
    private readonly Dictionary<string, Dictionary<string, double>> _natures = new()
    {
        {"Hardy",   new()},
        {"Docile",  new()},
        {"Serious", new()},
        {"Bashful", new()},
        {"Quirky",  new()},
        {"Lonely",  new() {{"Atk", 1.1},{"Def", 0.9}}},
        {"Brave",   new() {{"Atk", 1.1},{"Spe", 0.9}}},
        {"Adamant", new() {{"Atk", 1.1},{"SpA", 0.9}}},
        {"Naughty", new() {{"Atk", 1.1},{"SpD", 0.9}}},
        {"Bold",    new() {{"Def", 1.1},{"Atk", 0.9}}},
        {"Relaxed", new() {{"Def", 1.1},{"Spe", 0.9}}},
        {"Impish",  new() {{"Def", 1.1},{"Spa", 0.9}}},
        {"Lax",     new() {{"Def", 1.1},{"SpD", 0.9}}},
        {"Timid",   new() {{"Spe", 1.1},{"Atk", 0.9}}},
        {"Hasty",   new() {{"Spe", 1.1},{"Def", 0.9}}},
        {"Jolly",   new() {{"Spe", 1.1},{"SpA", 0.9}}},
        {"Naive",   new() {{"Spe", 1.1},{"SpD", 0.9}}},
        {"Modest",  new() {{"SpA", 1.1},{"Atk", 0.9}}},
        {"Mild",    new() {{"SpA", 1.1},{"Def", 0.9}}},
        {"Quiet",   new() {{"SpA", 1.1},{"Spe", 0.9}}},
        {"Rash",    new() {{"SpA", 1.1},{"SpD", 0.9}}},
        {"Calm",    new() {{"SpD", 1.1},{"Atk", 0.9}}},
        {"Gentle",  new() {{"SpD", 1.1},{"Def", 0.9}}},
        {"Sassy",   new() {{"SpD", 1.1},{"Spe", 0.9}}},
        {"Careful", new() {{"SpD", 1.1},{"SpA", 0.9}}}
    };
    private Dictionary<string, MonData> _monData = [];
    public class MonData()
    {
        public Dictionary<string, string> abilities { get; set; } = [];
        public List<string> types { get; set; } = [];
        public BaseStats bs { get; set; } = new();
        public class BaseStats()
        {
            public int hp { get; set; }
            public int at { get; set; }
            public int df { get; set; }
            public int? sa { get; set; }
            public int? sd { get; set; }
            public int sp { get; set; }
            public int? sl { get; set; }
        }
    }
}
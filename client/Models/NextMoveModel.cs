using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using System.Threading.Tasks;
using System.Web;
using DynamicData;

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
    private Dictionary<int, Dictionary<int, Dictionary<int, List<float>>>>[] _calcedDamages = [];
    private List<int> _turnOrder = [];
    public List<MoveModel> Moves { get; set; } = [];
    public async Task<List<int>> UpdateTurnInfo()
    {
        foreach (MoveModel move in Moves)
        {
            move.MoveType = "Calculating...";
            move.TargetNo = -1;
            move.Mega = false;
            move.Dynamax = false;
            move.ZMove = false;
            move.Tera = false;
        }
        bool trickRoomActive = theGame.CurrentArena.TrickRoom;
        ParseTurn();
        List<CalcRespModel> damages = await CalcDamages(false);
        ParseCalc(damages);
        _turnOrder = UpdateSpeeds(trickRoomActive);
        damages = await CalcDamages(true);
        _calcedDamages = FormatCalcs(damages);
        return ChooseKOSwitch(_calcedDamages[0]);
    }
    private void ParseTurn()
    {
        if (theGame.Turns.Count < 2) return;
        foreach (EventModel eventModel in theGame.Turns[^2].EventList)
        {
            if (eventModel.UserMon == -1)
            {
                continue;
            }
            TeamModel user = eventModel.UserMon < 6 ? theGame.BotTeam[eventModel.UserMon] : theGame.OppTeam[eventModel.UserMon - 6];
            eventModel.UserStartingHP = user.RemainingHP;
            foreach (TargetModel target in eventModel.TargetMons)
            {
                if (target.MonNo == -1)
                {
                    continue;
                }
                TeamModel targetMon = target.MonNo < 6 ? theGame.BotTeam[target.MonNo] : theGame.OppTeam[target.MonNo - 6];
                target.StartingHP = targetMon.RemainingHP;
                if (target.Damage != null)
                {
                    targetMon.RemainingHP -= (int)target.Damage;
                }                
            }
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
                case "Move Disabled":
                    ParseDisable(eventModel);
                    break;
                case "Disable Ended":
                    ParseDisableEnd(eventModel);
                    break;
                case "PP Depleated":
                    ParsePP(eventModel);
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
        TeamModel userMon;
        if (eventModel.UserMon > 5)
        {
            SaveMove(eventModel);
            userMon = theGame.OppTeam[eventModel.UserMon - 6];
        }
        else
        {
            userMon = theGame.BotTeam[eventModel.UserMon];
        }
        if (eventModel.EventType != "Move")
        {
            return;
        }
        if (userMon.Item.Contains("Choice ") && !userMon.ItemRemoved)
        {
            userMon.Choiced = userMon.Moves.FindIndex(x => x == eventModel.MoveName);
        }
        if (eventModel.MoveName == "Transform")
        {
            ParseTransform(eventModel);
        }
    }

    private void SaveMove(EventModel eventModel)
    {
        if (eventModel.MoveName == "Struggle")
        {
            return;
        }
        for (int i = 0; i < 4; i++)
        {
            if (theGame.OppTeam[eventModel.UserMon - 6].Moves[i] == "")
            {
                theGame.OppTeam[eventModel.UserMon - 6].Moves[i] = eventModel.MoveName;
                return;
            }
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
                theGame.GimmickUsed[1] = true;
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
                if (eventModel.EventType == "Z-Move")
                {
                    theGame.GimmickUsed[0] = true;
                }
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

        if (eventModel.EventType.Contains("amax"))
        {
            tempMon.TurnDynamaxed = theGame.Turns[^2].TurnNo;
            if (eventModel.UserMon < 6)
            {
                theGame.GimmickUsed[0] = true;
            }
            else
            {
                theGame.GimmickUsed[1] = true;
                if (eventModel.EventType == "Gigantamax")
                {
                    tempMon.GMax = true;
                }
            }
        }
        if (eventModel.EventType == "Mega Evolution")
        {
            if (eventModel.UserMon < 6)
            {
                theGame.GimmickUsed[0] = true;
                return;
            }
            tempMon.Item = "Abomasite"; // Place holder for correct mega stone
            theGame.GimmickUsed[1] = true;
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
            if (eventModel.UserMon < 6)
            {
                theGame.GimmickUsed[0] = true;
                return;
            }
            theGame.GimmickUsed[1] = true;
            if (tempMon.Tera == "None")
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
            theGame.Turns[^1].OppStartMons[theGame.Turns[^1].OppStartMons.IndexOf(eventModel.UserMon - 6)] =
                eventModel.TargetMons[0].MonNo - 6;
            theGame.OppTeam[eventModel.UserMon - 6].Position = "Reserve";
            theGame.OppTeam[eventModel.TargetMons[0].MonNo - 6].Position = "Active";
            if (!theGame.MonsSeen.Contains(eventModel.TargetMons[0].MonNo - 6))
            {
                theGame.MonsSeen.Add(eventModel.TargetMons[0].MonNo - 6);
            }
            theGame.OppTeam[eventModel.UserMon - 6].Transform = null;
            theGame.OppTeam[eventModel.UserMon - 6].AbilityActive = false;
            theGame.OppTeam[eventModel.UserMon - 6].VolStatus = [];
            theGame.OppTeam[eventModel.UserMon - 6].DisabledMoves = [];
            return;
        }
        if (eventModel.UserMon < 6 && eventModel.TargetMons[0].MonNo < 6)
        {
            theGame.Turns[^1].BotStartMons[theGame.Turns[^2].BotStartMons.IndexOf(eventModel.UserMon)] =
                eventModel.TargetMons[0].MonNo;
            theGame.BotTeam[eventModel.UserMon].Position = "Reserve";
            theGame.BotTeam[eventModel.TargetMons[0].MonNo].Position = "Active";
            theGame.BotTeam[eventModel.UserMon].Transform = null;
            theGame.BotTeam[eventModel.UserMon].AbilityActive = false;
            theGame.BotTeam[eventModel.UserMon].VolStatus = [];
            theGame.BotTeam[eventModel.UserMon].DisabledMoves = [];
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
            theGame.Turns[^1].BotStartMons.Replace(eventModel.UserMon, -1);
        }
        else
        {
            theGame.OppTeam[eventModel.UserMon - 6].RemainingHP = 0;
            theGame.OppTeam[eventModel.UserMon - 6].Position = "KO";
            theGame.Turns[^1].OppStartMons.Replace(eventModel.UserMon - 6, -1);
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
            theGame.Turns[^1].OppStartMons[theGame.Turns[^1].OppStartMons.IndexOf(eventModel.UserMon - 6)] = zoroNum;
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

    private void ParseDisable(EventModel eventModel)
    {
        TeamModel mon;
        if (eventModel.UserMon < 6)
        {
            mon = theGame.BotTeam[eventModel.UserMon];
        }
        else
        {
            mon = theGame.OppTeam[eventModel.UserMon - 6];
        }
        int moveNo = mon.Moves.FindIndex(x => x == eventModel.MoveName);
        if (moveNo != -1 && !mon.DisabledMoves.Contains(moveNo))
        {
            mon.DisabledMoves.Add([moveNo]);
        }
    }

    private void ParseDisableEnd(EventModel eventModel)
    {
        TeamModel mon;
        if (eventModel.UserMon < 6)
        {
            mon = theGame.BotTeam[eventModel.UserMon];
        }
        else
        {
            mon = theGame.OppTeam[eventModel.UserMon - 6];
        }
        int moveNo = mon.Moves.FindIndex(x => x == eventModel.MoveName);
        mon.DisabledMoves.Remove([moveNo]);
    }

    private void ParsePP(EventModel eventModel)
    {
        TeamModel mon;
        if (eventModel.UserMon < 6)
        {
            mon = theGame.BotTeam[eventModel.UserMon];
        }
        else
        {
            mon = theGame.OppTeam[eventModel.UserMon - 6];
        }
        int moveNo = mon.Moves.FindIndex(x => x == eventModel.MoveName);
        if (moveNo != -1 && !mon.OutOfPP.Contains(moveNo))
        {
            mon.OutOfPP.Add([moveNo]);
        }
    }

    private List<int> UpdateSpeeds(bool trickRoomActive)
    {
        int turnPos = 100;
        Dictionary<int, List<int>> eventOrders = [];
        foreach (EventModel eventModel in theGame.Turns[^2].EventList)
        {
            int priority = DecidePriority(eventModel);
            if (priority >= 10 && turnPos < 10)
            {
                continue;
            }
            turnPos = priority;
            if (!eventOrders.TryGetValue(priority, out List<int>? value))
            {
                value = [];
                eventOrders.Add(priority, value);
            }
            if (!value.Contains(eventModel.UserMon))
            {
                value.Add(eventModel.UserMon);
            }
        }
        if (trickRoomActive)
        {
            foreach (int priority in eventOrders.Keys)
            {
                eventOrders[priority].Reverse();
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
                break;
            }
            if (!speedOrder.Contains(monNo))
            {
                speedOrder.Add(monNo);
            }
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
                    else if (mon.PossibleAbility == null)
                    {
                        bool priorityPos = PriorityPossible(eventOrders, monNo, trickRoomActive);
                        EventModel moveEvent = theGame.Turns[^2].EventList.Find(x => x.EventType == "Move" && x.UserMon == monNo) ?? new EventModel{MoveName = "Struggle"};
                        if (priorityPos && allOptions.AllMoves[moveEvent.MoveName].category == null && _invisibleAbilities["Prankster"].Contains(mon.Name))
                        {
                            mon.PossibleAbility = "Prankster";
                            break;
                        }
                        else if (priorityPos && _healingMoves.Contains(moveEvent.MoveName) && _invisibleAbilities["Triage"].Contains(mon.Name))
                        {
                            mon.PossibleAbility = "Triage";
                            break;
                        }
                        else if (priorityPos && allOptions.AllMoves[moveEvent.MoveName].type == "Flying" && _invisibleAbilities["Gale Wings"].Contains(mon.Name) && moveEvent.UserStartingHP == 100)
                        {
                            mon.PossibleAbility = "Gale Wings";
                            break;
                        }
                        else if (theGame.CurrentArena.Weather.Contains("Harsh Sunlight") && _invisibleAbilities["Chlorophyll"].Contains(mon.Name))
                        {
                            mon.PossibleAbility = "Chlorophyll";
                        }
                        else if (theGame.CurrentArena.Weather.Contains("Rain") && _invisibleAbilities["Swift Swim"].Contains(mon.Name))
                        {
                            mon.PossibleAbility = "Swift Swim";
                        }
                        else if (theGame.CurrentArena.Weather == "Sandstorm" && _invisibleAbilities["Sand Rush"].Contains(mon.Name))
                        {
                            mon.PossibleAbility = "Sand Rush";
                        }
                        else if ((theGame.CurrentArena.Weather == "Snow" || theGame.CurrentArena.Weather == "Hail") && _invisibleAbilities["Slush Rush"].Contains(mon.Name))
                        {
                            mon.PossibleAbility = "Slush Rush";
                        }
                        else if ((mon.NonVolStatus != "" || mon.VolStatus.Count > 0) && _invisibleAbilities["Quick Feet"].Contains(mon.Name))
                        {
                            mon.PossibleAbility = "Quick Feet";
                        }
                        else if (mon.ItemRemoved && _invisibleAbilities["Unburden"].Contains(mon.Name))
                        {
                            mon.PossibleAbility = "Unburden";
                        }
                        else if (theGame.CurrentArena.Terrain == "Electric Terrain" && _invisibleAbilities["Surge Surfer"].Contains(mon.Name))
                        {
                            mon.PossibleAbility = "Surge Surfer";
                        }
                    }
                    // Items go here
                    else
                    {
                        break;
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
                    else if (PriorityPossible(eventOrders, monNo, !trickRoomActive) && mon.PossibleAbility == null)
                    {
                        EventModel moveEvent = theGame.Turns[^2].EventList.Find(x => x.EventType == "Move" && x.UserMon == monNo) ?? new EventModel{MoveName = "Struggle"};
                        if (allOptions.AllMoves[moveEvent.MoveName].category == null && _invisibleAbilities["Mycelium Might"].Contains(mon.Name))
                        {
                            mon.PossibleAbility = "Mycelium Might";
                            break;
                        }
                        if (_invisibleAbilities["Stall"].Contains(mon.Name))
                        {
                            mon.PossibleAbility = "Stall";
                            break;
                        }
                    }
                    else
                    {
                        // Parse items here
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

public static bool PriorityPossible(Dictionary<int, List<int>> order, int monNo, bool trickRoomActive)
    {
        foreach (int key in order.Keys)
        {
            if (key >= 10)
            {
                continue;
            }
            if (order[key].Count > 0)
            {
                if (trickRoomActive && order[key][^1] == monNo)
                {
                    return true;
                }
                if (!trickRoomActive && order[key][0] == monNo)
                {
                    return true;
                }
            }
        }
        return false;
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

    public async Task<List<CalcRespModel>> CalcDamages(bool includeGimmick) // Calculates damage portion of weightings
    {
        ObservableCollection<PokemonModel> botPokemon = []; // Collection of bot's pokemon in server compatable format
        ObservableCollection<PokemonModel> oppPokemon = []; // Collection of opponent's pokemon in server compatable format
        foreach (int i in theGame.MonsBrought) // Add all brought mons to collections
        {
            if (theGame.BotTeam[i].Position == "KO")
            {
                continue;
            }
            botPokemon.Add(new PokemonModel(theGame.Gen, theGame.BotTeam[i]));
        }
        for (int i = 0; i < 6; i++)
        {
            if (theGame.OppTeam[i].Position == "KO" || theGame.OppTeam[i].Position == "Not Brought")
            {
                continue;
            }
            oppPokemon.Add(new PokemonModel(theGame.Gen, theGame.OppTeam[i]));
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
        if (includeGimmick && !theGame.GimmickUsed[0])
        {
            callData.IncludeGimmick = theGame.Gimmicks.GetGimmick();
        }
        string callString = JsonSerializer.Serialize(callData); // Serialise call data into string
        HttpClient client = new();
        List<CalcRespModel>? response = await client.GetFromJsonAsync<List<CalcRespModel>>($"http://{theGame.ServerUrl}/calc?{callString}"); // Send data to server and await response
        if (response == null) return []; // Return empty results on null response
        return response;
    }

    private void ParseCalc(List<CalcRespModel> damages)
    {
        Dictionary<int, Dictionary<int, Dictionary<int, List<float>>>> expectedDamages = FormatCalcs(damages)[0];
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
            string atkCategory = allOptions.AllMoves[eventModel.MoveName].category == "Physical" ? "Atk" : "SpA";
            string defCategory = atkCategory == "Atk" ? "Def" :
                _SpaDefMoves.Contains(eventModel.MoveName) ? "Def" : "SpD";
            TeamModel userMon = eventModel.UserMon > 5 ?
                theGame.OppTeam[eventModel.UserMon - 6] : theGame.BotTeam[eventModel.UserMon];
            foreach (TargetModel target in eventModel.TargetMons)
            {
                if ((eventModel.UserMon < 6 && target.MonNo < 6) || (eventModel.UserMon > 5 && target.MonNo > 5))
                {
                    continue;
                }
                if (target.MoveResult == "Immune" || target.MoveResult == "Miss" ||
                    target.MoveResult == "Failed" || target.MoveResult == "")
                {
                    continue;
                }
                if (target.Damage == null)
                {
                    continue;
                }
                if (_categoryChangingMoves.Contains(eventModel.MoveName))
                {
                    continue; // will implement this properly down the line
                }
                float maxExpected =
                    expectedDamages[eventModel.UserMon][target.MonNo][userMon.Moves.IndexOf(eventModel.MoveName)][1];
                float minExpected = (float)Math.Floor(
                    expectedDamages[eventModel.UserMon][target.MonNo][userMon.Moves.IndexOf(eventModel.MoveName)][0]);
                if (target.Damage >= minExpected && target.Damage <= maxExpected)
                {
                    continue;
                }
                TeamModel targetMon = target.MonNo > 5 ?
                    theGame.OppTeam[target.MonNo - 6] : userMon;
                float mult = (float)(target.Damage < minExpected ?
                    minExpected / target.Damage : maxExpected / target.Damage);
                string statName;
                int monNoToChange;
                if (eventModel.UserMon < 6)
                {
                    statName = defCategory;
                    mult = 1 / mult;
                    monNoToChange = target.MonNo;
                }
                else
                {
                    statName = atkCategory;
                    monNoToChange = eventModel.UserMon;
                }
                int targetStat = (int)Math.Floor(CalcStat(statName, monNoToChange) * mult);
                if (mult > 1)
                {
                    targetStat++;
                }
                string? prevBoost = null;
                string? prevDrop = null;

                while (true)
                {
                    int calcedStat = CalcStat(statName, monNoToChange);
                    if (calcedStat > targetStat)
                    {
                        if (mult > 1)
                        {
                            break;
                        }
                        if (targetMon.EV.GetStatFromName(statName) <= 248)
                        {
                            targetMon.EV.IncrementStat(statName, 4, "EV");
                            continue;
                        }
                        if (targetMon.IV.GetStatFromName(statName) <= 30)
                        {
                            targetMon.IV.IncrementStat(statName, 1, "IV");
                            continue;
                        }
                        if (!statName.Contains('A')) // If not an attacking stat
                        {
                            if (targetMon.EV.HP <= 248)
                            {
                                targetMon.EV.HP += 4;
                                continue;
                            }
                            if (targetMon.IV.HP < 31)
                            {
                                targetMon.IV.HP++;
                                continue;
                            }
                        }
                        if (targetMon.NatureBoost != statName)
                        {
                            prevBoost = targetMon.NatureBoost;
                            if (targetMon.NatureDrop == statName)
                            {
                                targetMon.NatureDrop = null;
                            }
                            else
                            {
                                targetMon.NatureBoost = statName;
                            }
                            continue;
                        }
                        // Account for abilities/items here
                        break;
                    }
                    if (calcedStat < targetStat)
                    {
                        if (mult < 1)
                        {
                            break;
                        }
                        if (targetMon.EV.GetStatFromName(statName) >= 4)
                        {
                            targetMon.EV.IncrementStat(statName, -4, "EV");
                            continue;
                        }
                        if (targetMon.IV.GetStatFromName(statName) > 0)
                        {
                            targetMon.IV.IncrementStat(statName, -1, "IV");
                            continue;
                        }
                        if (!statName.Contains('A')) // If not an attacking stat
                        {
                            if (targetMon.EV.HP >= 4)
                            {
                                targetMon.EV.HP -= 4;
                                continue;
                            }
                            if (targetMon.IV.HP > 0)
                            {
                                targetMon.IV.HP--;
                                continue;
                            }
                        }
                        if (targetMon.NatureDrop != statName)
                        {
                            prevDrop = targetMon.NatureDrop;
                            if (targetMon.NatureBoost == statName)
                            {
                                targetMon.NatureBoost = null;
                            }
                            else
                            {
                                targetMon.NatureDrop = statName;
                            }
                            continue;
                        }
                        // Account for abilities/items here
                        break;
                    }
                    break;
                }
                if (targetMon.NatureBoost == null && prevBoost != null && prevBoost != targetMon.NatureDrop)
                {
                    targetMon.NatureBoost = prevBoost;
                }
                if (targetMon.NatureDrop == null && prevDrop != null && prevDrop != targetMon.NatureBoost)
                {
                    targetMon.NatureDrop = prevDrop;
                }
                ParseNature(targetMon);
            }
        }
    }

    private Dictionary<int, Dictionary<int, Dictionary<int, List<float>>>>[] FormatCalcs(List<CalcRespModel> damages)
    {
        Dictionary<int, Dictionary<int, Dictionary<int, List<float>>>>[] expectedDamages = [[], []];
        foreach (CalcRespModel damage in damages)
        {
            int monNo;
            int targetMon;
            if (damage.BotUser)
            {
                monNo = _nameToNo[damage.UserMon];
                targetMon = _nameToNo["Opponent's " + damage.TargetMon];
            }
            else
            {
                monNo = _nameToNo["Opponent's " + damage.UserMon];
                targetMon = _nameToNo[damage.TargetMon];
            }

            Dictionary<int, Dictionary<int, Dictionary<int, List<float>>>> parsedDamages;

            if (damage.GimmickUsed)
            {
                parsedDamages = expectedDamages[1];
            }
            else
            {
                parsedDamages = expectedDamages[0];
            }

            if (parsedDamages.TryAdd(
                monNo, new() { { targetMon, new() { { damage.MoveNo, [damage.MinDamage, damage.MaxDamage] } } } }))
            {
                continue;
            }
            if (parsedDamages[monNo].TryAdd(
                targetMon, new() { { damage.MoveNo, [damage.MinDamage, damage.MaxDamage] } }))
            {
                continue;
            }
            parsedDamages[monNo][targetMon].Add(damage.MoveNo, [damage.MinDamage, damage.MaxDamage]);
        }
        return expectedDamages;
    }

    private bool CouldBeChoiced(int monNo)
    {
        if (monNo > 5)
        {
            return false;
        }
        string? moveName = null;
        for (int turnNo = theGame.Turns.Count; turnNo >= 0; turnNo--)
        {
            if (!theGame.Turns[turnNo].OppStartMons.Contains(monNo))
            {
                moveName = null;
                continue;
            }
            foreach (EventModel ev in theGame.Turns[turnNo].EventList)
            {
                if (ev.UserMon != monNo + 6 || ev.EventType != "Move")
                {
                    continue;
                }
                if (allOptions.AllMoves[ev.MoveName].isMax != null || allOptions.AllMoves[ev.MoveName].isZ != null || ev.MoveName == "Struggle")
                {
                    continue;
                }
                if (moveName == null)
                {
                    moveName = ev.MoveName;
                }
                else if (moveName != ev.MoveName)
                {
                    return false;
                }
            }
        }
        return true;
    }

    private List<int> ChooseKOSwitch(Dictionary<int, Dictionary<int, Dictionary<int, List<float>>>> expectedDamages)
    {
        List<int> switchIn = [];
        if (!theGame.BotTeam.Any(x => x.Position == "Reserve"))
        {
            return switchIn;
        }
        for (int i = 0; i < 2; i++)
        {
            if (theGame.Turns[^1].BotStartMons[i] != -1)
            {
                continue;
            }
            int switchMon = ChooseSwitch(expectedDamages, 10000);
            if (switchMon == -1)
            {
                continue;
            }
            theGame.Turns[^1].BotStartMons[i] = switchMon;
            theGame.BotTeam[switchMon].Position = "Active";
            switchIn.Add(switchMon);
        }
        return switchIn;
    }

    private int ChooseSwitch(Dictionary<int, Dictionary<int, Dictionary<int, List<float>>>> expectedDamages, float damageToBeat)
    {
        int switchMon = -1;
        foreach (TeamModel tempMon in theGame.BotTeam)
        {
            if (tempMon.Position != "Reserve")
            {
                continue;
            }
            int monNo = _nameToNo[tempMon.Name];
            float maxDamage = 0;
            foreach (int targetmon in theGame.Turns[^1].OppStartMons)
            {
                if (targetmon == -1)
                {
                    continue;
                }
                float maxMonDamage = 0;
                if (!expectedDamages.TryGetValue(targetmon, out Dictionary<int, Dictionary<int, List<float>>>? value) || !value.ContainsKey(monNo))
                {
                    continue;
                }
                foreach (List<float> move in expectedDamages[targetmon][monNo].Values)
                {
                    maxMonDamage = maxMonDamage < move[0] ? move[0] : maxMonDamage;
                }
                maxDamage += maxMonDamage;
            }
            if (maxDamage < damageToBeat)
            {
                damageToBeat = maxDamage;
                switchMon = monNo;
            }
        }
        return switchMon;
    }

    private bool ImmuneToFakeOut(int target, TeamModel targetMon, TeamModel user) // Probably more things to consider in here
    {
        return ImmuneToMove("Fake Out", targetMon, user, target) || (targetMon.Tera == "Ghost" && !theGame.GimmickUsed[target / 6]) ||
            (targetMon.Item == "Covert Cloak" && !targetMon.ItemRemoved) || targetMon.VolStatus.Contains("Substitute") ||
            (targetMon.TurnDynamaxed >= theGame.Turns.Count - 3 && targetMon.TurnDynamaxed != -1);
    }

    private bool ImmuneToPriority(int target, TeamModel targetMon)
    {
        if (target < 6)
        {
            int partnerMon = theGame.Turns[^1].BotStartMons.Find(x => x != target);
            if (partnerMon != -1)
            {
                if (theGame.BotTeam[partnerMon].Ability == "Queenly Magesty" || theGame.BotTeam[partnerMon].Ability == "Dazzling" || theGame.BotTeam[partnerMon].Ability == "Armor Tail")
                {
                    return true;
                }
            }
        }
        else
        {
            int partnerMon = theGame.Turns[^1].OppStartMons.Find(x => x != target - 6);
            if (partnerMon != -1)
            {
                if (theGame.BotTeam[partnerMon].Ability == "Queenly Magesty" || theGame.BotTeam[partnerMon].Ability == "Dazzling" || theGame.BotTeam[partnerMon].Ability == "Armor Tail")
                {
                    return true;
                }
            }
        }
        return targetMon.Ability == "Queenly Majesty" || targetMon.Ability == "Dazzling" || targetMon.Ability == "Armor Tail" ||
            (theGame.CurrentArena.Terrain == "Psychic Terrain" && Grounded(targetMon));
    }

    private bool HasType(TeamModel mon, string type)
    {
        if (!mon.TeraActive && _monData[mon.Name].types.Contains(type) && mon.TypeChange == "")
        {
            return true;
        }
        if (!mon.TeraActive && mon.TypeChange == type)
        {
            return true;
        }
        if (mon.Tera == type && mon.TeraActive)
        {
            return true;
        }
        return false;
    }

    private bool ImmuneToMove(string moveName, TeamModel target, TeamModel user, int targetNo)
    {
        MoveInfoModel move = allOptions.AllMoves[moveName];
        string? moveType = move.type;
        if (user.VolStatus.Contains("Electrify"))
        {
            moveType = "Electric";
        }
        if (move.isSound != null && (user.VolStatus.Contains("Throat Chop") || target.Ability == "Soundproof"))
        {
            return true;
        }
        if (move.isBullet != null && target.Ability == "Bulletproof")
        {
            return true;
        }
        if (move.isWind != null && target.Ability == "Wind Rider")
        {
            return true;
        }
        switch (user.Ability)
        {
            case "Normalize":
                moveType = "Normal";
                break;
            case "Aerilate":
                moveType = moveType == "Normal" ? "Flying" : moveType;
                break;
            case "Dragonize":
                moveType = moveType == "Normal" ? "Dragon" : moveType;
                break;
            case "Galvanize":
                moveType = moveType == "Normal" ? "Electric" : moveType;
                break;
            case "Pixilate":
                moveType = moveType == "Normal" ? "Fairy" : moveType;
                break;
            case "Refrigerate":
                moveType = moveType == "Normal" ? "Ice" : moveType;
                break;
            case "Liquid Voice":
                moveType = move.isSound != null ? "Water" : moveType;
                break;
        }
        switch (moveType)
        {
            case "Dragon":
                if (move.category != null && HasType(target, "Fairy"))
                {
                    return true;
                }
                break;
            case "Electric":
                if (((move.category != null || moveName == "Thunder Wave") && HasType(target, "Ground")) || target.Ability == "Volt Absorb" || (target.Ability == "Lightning Rod" && theGame.Gen > 4) || target.Ability == "Motor Drive")
                {
                    return true;
                }
                break;
            case "Fighting":
                if (move.category != null && HasType(target, "Ghost") && !(user.Ability == "Scrappy" || user.Ability == "Mind's Eye") && !target.VolStatus.Contains("Identified"))
                {
                    return true;
                }
                break;
            case "Fire":
                if (target.Ability == "Flash Fire" || (theGame.CurrentArena.Weather == "Heavy Rain" && move.category != null))
                {
                    return true;
                }
                break;
            case "Ghost":
                if (move.category != null && HasType(target, "Normal"))
                {
                    return true;
                }
                break;
            case "Grass":
                if (target.Ability == "Sap Sipper")
                {
                    return true;
                }
                break;
            case "Ground":
                if (move.category != null && !Grounded(target))
                {
                    return true;
                }
                break;
            case "Normal": // Need to acount for ion deluge and plasma fists
                if (HasType(target, "Ghost"))
                {
                    return true;
                }
                break;
            case "Poison":
                if (HasType(target, "Steel"))
                {
                    return true;
                }
                break;
            case "Psychic":
                if (HasType(target, "Dark") && !target.VolStatus.Contains("Identified"))
                {
                    return true;
                }
                break;
            case "Water":
                if (target.Ability == "Dry Skin" || target.Ability == "Storm Drain" || (move.category != null && theGame.CurrentArena.Weather == "Extremely Harsh Sunlight"))
                {
                    return true;
                }
                break;
        }
        if (_powderMoves.Contains(moveName) && (HasType(target, "Grass") || target.Ability == "Overcoat" || (target.Item == "Safety Goggles" && !target.ItemRemoved)))
        {
            return true;
        }
        if (move.category == null && (target.Ability == "Good as Gold" || target.Ability == "Magic Bounce"))
        {
            return true;
        }
        if (ImmuneToPriority(targetNo, target) && (move.priotity > 0 || (user.Ability == "Prankster" && move.category == null) ||
            (user.Ability == "Gale Wings" && move.type == "Flying" && user.RemainingHP == 100) || (user.Ability == "Triage" && _healingMoves.Contains(moveName))))
        {
            return true;
        }
        if (HasType(target, "Dark") && user.Ability == "Prankster" && move.category == null)
        {
            return true;
        }
        return false;
    }

    private bool ImmuneToStatus(TeamModel target, string status, MoveInfoModel move)
    {
        if (target.Ability == "Purifying Salt" || target.Ability == "Comatose" || (target.Ability == "Leaf Guard" && theGame.CurrentArena.Weather.Contains("Harsh Sunlight")) || target.Ability == "Synchronize" ||
            target.VolStatus.Contains("Substitute") || (theGame.CurrentArena.Terrain == "Misty Terrain" && Grounded(target)) || theGame.CurrentArena.OppSide.Safeguard || (move.category != null && target.Ability == "Shield Dust"))
        {
            return true;
        }
        switch (status)
        {
            case "Sleep":
                if ((theGame.CurrentArena.Terrain == "Electric Terrain" && Grounded(target)) ||
                    target.Ability == "Insomnia" || target.Ability == "Vital Spirit" || target.Ability == "Early Bird" ||
                    theGame.Turns[^1].OppStartMons.Any(x => x > 5 ? theGame.OppTeam[x - 6].Ability == "Sweet Veil" ||
                    (theGame.OppTeam[x - 6].VolStatus.Contains("Making an Uproar") && (target.Ability != "Soundproof" || theGame.Gen > 4)) : false))
                {
                    return true;
                }
                break;
            case "Burn":
                if (target.Ability == "Water Veil" || target.Ability == "Water Bubble" || target.Ability == "Flare Boost" ||
                    target.Ability == "Thermal Exchange" || target.Ability == "Guts" || HasType(target, "Fire"))
                {
                    return true;
                }
                break;
            case "Para":
                if (target.Ability == "Limber" || target.Ability == "Quick Feet" || target.Ability == "Guts" || HasType(target, "Electric"))
                {
                    return true;
                }
                break;
        }
        return false;
    }

    private bool ImmuneToLowerStat(TeamModel target, string stat)
    {
        if (target.Ability == "Clear Body" || target.Ability == "Full Metal Body" || target.Ability == "White Smoke" ||
            target.Ability == "Mirror Armor" || target.Ability == "Contrary" || target.Ability == "Defiant" || target.Ability == "Competitive" ||
            (theGame.Turns[^1].OppStartMons.Any(x => x > 5 ? theGame.OppTeam[x - 6].Ability == "Flower Veil" : false) && HasType(target, "Grass")))
        {
            return true;
        }
        switch (stat)
        {
            case "Atk":
                if (target.Ability == "Hyper Cutter")
                {
                    return true;
                }
                break;
            case "Def":
                if (target.Ability == "Big Pecks")
                {
                    return true;
                }
                break;
        }
        return false;
    }

    private bool ProtectedLastTurn(int user)
    {
        return theGame.Turns[^2].EventList.Find(x => x.UserMon == user && _protectionMoves.Contains(x.MoveName)) != null;
    }

    private bool Grounded(TeamModel mon)
    {
        return !(_monData[mon.Name].types.Contains("Flying") || mon.Ability == "Levitate" || mon.VolStatus.Contains("Magnetic Levitation") || mon.VolStatus.Contains("Telekinesis") || (mon.Item == "Air Balloon" && !mon.ItemRemoved)) ||
            (mon.Item == "Iron Ball" && !mon.ItemRemoved) || mon.VolStatus.Contains("Grounded") || theGame.CurrentArena.Gravity;
    }

    private static bool GimmickImmune(int target, int user, int moveNo, Dictionary<int, Dictionary<int, Dictionary<int, List<float>>>> gimmickDamages)
    {
        if (gimmickDamages.ContainsKey(user) && gimmickDamages[user].ContainsKey(target) && gimmickDamages[user][target].ContainsKey(moveNo))
        {
            if (gimmickDamages[user][target][moveNo][1] == 0)
            {
                return true;
            }
        }
        return false;
    }

    private bool GimmickSave(int target, int user, int moveNo, Dictionary<int, Dictionary<int, Dictionary<int, List<float>>>> gimmickDamages)
    {
        if (gimmickDamages.ContainsKey(user) && gimmickDamages[user].ContainsKey(target) && gimmickDamages[user][target].ContainsKey(moveNo))
        {
            if (gimmickDamages[user][target][moveNo][1] < theGame.BotTeam[target].RemainingHP)
            {
                return true;
            }
        }
        return false;
    }

    private bool CanSwitch(TeamModel mon)
    {
        if (theGame.Turns[^1].OppStartMons.Any(x => x > 5 &&
            ((theGame.OppTeam[x - 6].Ability == "Shadow Tag" && !HasType(mon, "Ghost")) || (theGame.OppTeam[x - 6].Ability == "Arena Trap" && Grounded(mon)) || (theGame.OppTeam[x - 6].Ability == "Magnet Pull" && HasType(mon, "Steel")))))
        {
            return false;
        }
        if (mon.VolStatus.Contains("Can't Escape"))
        {
            return false;
        }
        return true;
    }

    public void ChooseNextMove()
    {
        Dictionary<int, Dictionary<int, Dictionary<int, List<float>>>> expectedDamages = _calcedDamages[0];
        Dictionary<int, Dictionary<int, Dictionary<int, List<float>>>> gimmickDamages = _calcedDamages[1];

        if (theGame.CurrentArena.TrickRoom)
        {
            _turnOrder.Reverse();
        }

        List<BestDamages> bestDamages = [];
        for (int i = 0; i < 2; i++)
        {
            int mon = theGame.Turns[^1].BotStartMons[i];
            for (int j = 0; j < 2; j++)
            {
                int opp = theGame.Turns[^1].OppStartMons[j] + 6;
                if (opp == 5)
                {
                    opp = -1;
                }
                bestDamages.Add(new(mon, opp));
            }
        }
        foreach (BestDamages matchup in bestDamages)
        {
            if (matchup.MonNo == -1 || matchup.Target == -1)
            {
                continue;
            }
            foreach (int move in expectedDamages[matchup.MonNo][matchup.Target].Keys)
            {
                if (theGame.BotTeam[matchup.MonNo].DisabledMoves.Contains(move) || theGame.BotTeam[matchup.MonNo].OutOfPP.Contains(move))
                {
                    continue;
                }
                if (theGame.BotTeam[matchup.MonNo].Moves[move] == "Fake Out")
                {
                    continue;
                }
                if (matchup.OKOChance && matchup.OutspeedTarget)
                {
                    continue;
                }
                if (ImmuneToMove(theGame.BotTeam[matchup.MonNo].Moves[move], theGame.OppTeam[matchup.Target - 6], theGame.BotTeam[matchup.MonNo], matchup.Target))
                {
                    continue;
                }
                if (expectedDamages[matchup.MonNo][matchup.Target][move][0] > matchup.MinDamage)
                {
                    matchup.MinDamage = expectedDamages[matchup.MonNo][matchup.Target][move][0];
                    matchup.OKOChance = expectedDamages[matchup.MonNo][matchup.Target][move][1] >= theGame.OppTeam[matchup.Target - 6].RemainingHP;
                    matchup.TKOGuaranteed = matchup.MinDamage >= 50;
                    matchup.MoveName = theGame.BotTeam[matchup.MonNo].Moves[move];
                    matchup.OutspeedTarget = allOptions.AllMoves[matchup.MoveName].priotity > 0;
                    if (gimmickDamages.ContainsKey(matchup.MonNo) && gimmickDamages[matchup.MonNo].ContainsKey(matchup.Target) && gimmickDamages[matchup.MonNo][matchup.Target].ContainsKey(move))
                    {
                        if (!matchup.OKOChance && gimmickDamages[matchup.MonNo][matchup.Target][move][1] >= theGame.OppTeam[matchup.Target - 6].RemainingHP)
                        {
                            matchup.GimmickSignificant = true;
                        }
                        else
                        {
                            matchup.GimmickSignificant = false;
                        }
                    }
                    else
                    {
                        matchup.GimmickSignificant = false;
                    }
                }
            }
            if (_turnOrder.IndexOf(matchup.MonNo) < _turnOrder.IndexOf(matchup.Target))
            {
                matchup.OutspeedTarget = true;
            }
        }
        bestDamages.Sort(delegate (BestDamages a, BestDamages b)
        {
            return (int)(b.MinDamage - a.MinDamage - (a.OutspeedTarget ? 100 : 0) + (b.OutspeedTarget ? 100 : 0));
        });

        List<bool> BotCanFakeOut = [false, false];
        List<bool> OppCanFakeOut = [false, false];
        for (int i = 0; i < 2; i++)
        {
            if (theGame.Turns[^1].BotStartMons[i] == -1)
            {
                continue;
            }
            if (theGame.BotTeam[theGame.Turns[^1].BotStartMons[i]].Moves.Contains("Fake Out"))
            {
                int move = theGame.BotTeam[theGame.Turns[^1].BotStartMons[i]].Moves.FindIndex(x => x == "Fake Out");
                if (theGame.BotTeam[theGame.Turns[^1].BotStartMons[i]].DisabledMoves.Contains(move) || theGame.BotTeam[theGame.Turns[^1].BotStartMons[i]].OutOfPP.Contains(move))
                {
                    continue;
                }
                if (theGame.Turns.Count <= 2)
                {
                    BotCanFakeOut[i] = true;
                    continue;
                }
                if (!theGame.Turns[^2].BotStartMons.Contains(theGame.Turns[^1].BotStartMons[i]))
                {
                    BotCanFakeOut[i] = true;
                }
            }
        }
        for (int i = 0; i < 2; i++)
        {
            if (theGame.Turns[^1].OppStartMons[i] == -1)
            {
                continue;
            }
            if (theGame.OppTeam[theGame.Turns[^1].OppStartMons[i]].Moves.Contains("Fake Out"))
            {
                int move = theGame.OppTeam[theGame.Turns[^1].OppStartMons[i]].Moves.FindIndex(x => x == "Fake Out");
                if (theGame.OppTeam[theGame.Turns[^1].OppStartMons[i]].DisabledMoves.Contains(move))
                    if (theGame.Turns.Count <= 2)
                    {
                        OppCanFakeOut[i] = true;
                        continue;
                    }
                if (!theGame.Turns[^2].OppStartMons.Contains(theGame.Turns[^1].OppStartMons[i]))
                {
                    OppCanFakeOut[i] = true;
                }
            }
        }

        if (BotCanFakeOut.Contains(true))
        {
            if (!BotCanFakeOut.Contains(false))
            {
                BotCanFakeOut[theGame.Turns[^1].BotStartMons.IndexOf(bestDamages[0].MonNo)] = false;
            }
            int fakeOutUser = theGame.Turns[^1].BotStartMons[BotCanFakeOut.IndexOf(true)];
            MoveModel fakeOutMove = Moves[BotCanFakeOut.IndexOf(true)];
            foreach (int target in _turnOrder)
            {
                if (target < 6)
                {
                    continue;
                }
                TeamModel targetMon = theGame.OppTeam[target - 6];
                if (ImmuneToFakeOut(target, targetMon, theGame.BotTeam[fakeOutUser]))
                {
                    continue;
                }
                fakeOutMove.MoveType = "Fake Out";
                fakeOutMove.UserNo = fakeOutUser;
                fakeOutMove.TargetNo = target;
            }
        }

        List<BestDamages> bestDamagesOpp = [];
        for (int i = 0; i < 2; i++)
        {
            int mon = theGame.Turns[^1].OppStartMons[i] + 6;
            if (mon == 5)
            {
                mon = -1;
            }
            for (int j = 0; j < 2; j++)
            {
                int opp = theGame.Turns[^1].BotStartMons[j];
                bestDamagesOpp.Add(new(mon, opp));
            }
        }
        foreach (BestDamages matchup in bestDamagesOpp)
        {
            if (matchup.MonNo == -1 || matchup.Target == -1 || !expectedDamages.ContainsKey(matchup.MonNo))
            {
                continue;
            }
            foreach (int move in expectedDamages[matchup.MonNo][matchup.Target].Keys)
            {
                if (theGame.OppTeam[matchup.MonNo].DisabledMoves.Contains(move))
                {
                    continue;
                }
                if (theGame.OppTeam[matchup.MonNo - 6].Moves[move] == "Fake Out" || theGame.OppTeam[matchup.MonNo - 6].Moves[move] == "")
                {
                    continue;
                }
                if (matchup.OKOChance && matchup.OutspeedTarget)
                {
                    continue;
                }
                if (ImmuneToMove(theGame.OppTeam[matchup.MonNo - 6].Moves[move], theGame.BotTeam[matchup.Target], theGame.OppTeam[matchup.MonNo], matchup.Target))
                {
                    continue;
                }
                if (expectedDamages[matchup.MonNo][matchup.Target][move][0] > matchup.MinDamage)
                {
                    matchup.MinDamage = expectedDamages[matchup.MonNo][matchup.Target][move][0];
                    matchup.OKOChance = expectedDamages[matchup.MonNo][matchup.Target][move][1] >= theGame.BotTeam[matchup.Target].RemainingHP;
                    matchup.TKOGuaranteed = matchup.MinDamage >= 50;
                    matchup.MoveName = theGame.OppTeam[matchup.MonNo].Moves[move];
                    matchup.OutspeedTarget = allOptions.AllMoves[matchup.MoveName].priotity > 0;
                    if ((matchup.OKOChance && GimmickSave(matchup.Target, matchup.MonNo, move, gimmickDamages)) ||
                        (matchup.TKOGuaranteed && GimmickImmune(matchup.Target, matchup.MonNo, move, gimmickDamages)))
                    {
                        matchup.GimmickSignificant = true;
                    }
                    else
                    {
                        matchup.GimmickSignificant = false;
                    }
                }
            }
            if (_turnOrder.IndexOf(matchup.MonNo) < _turnOrder.IndexOf(matchup.Target))
            {
                matchup.OutspeedTarget = true;
            }
        }

        Dictionary<int, bool> providingOffensvePressure = [];
        foreach (BestDamages matchup in bestDamages)
        {
            if (!providingOffensvePressure.TryAdd(matchup.MonNo, matchup.TKOGuaranteed))
            {
                providingOffensvePressure[matchup.MonNo] = providingOffensvePressure[matchup.MonNo] || matchup.TKOGuaranteed || matchup.GimmickSignificant;
            }
        }

        Dictionary<int, bool> underOffensvePressure = [];
        foreach (BestDamages matchup in bestDamagesOpp)
        {
            if (!underOffensvePressure.TryAdd(matchup.Target, matchup.TKOGuaranteed))
            {
                underOffensvePressure[matchup.Target] = underOffensvePressure[matchup.Target] || matchup.TKOGuaranteed;
            }
        }

        bool protectionBreakingMove = theGame.Turns[^1].OppStartMons.Any(x => x != -1 && theGame.OppTeam[x].Moves.Any(y => _protectionBreakingMoves.Contains(y) && !theGame.OppTeam[x].DisabledMoves.Contains(theGame.OppTeam[x].Moves.IndexOf(y))));
        bool usedGimmick = theGame.GimmickUsed[0];

        // Also need to calculate support pressures

        foreach (int monNo in _turnOrder)
        {
            if (!theGame.Turns[^1].BotStartMons.Contains(monNo))
            {
                continue;
            }
            MoveModel move = Moves[theGame.Turns[^1].BotStartMons.IndexOf(monNo)];
            if (move.MoveType != "Calculating...")
            {
                continue;
            }
            TeamModel user = theGame.BotTeam[monNo];
            int allyMon = theGame.Turns[^1].BotStartMons.Find(x => x != monNo);
            bool canProtect = user.Moves.Any(x => _protectionMoves.Contains(x) && !user.DisabledMoves.Contains(user.Moves.IndexOf(x)) && !user.OutOfPP.Contains(user.Moves.IndexOf(x))) &&
                !ProtectedLastTurn(monNo) && !protectionBreakingMove &&
                !theGame.Turns[^1].OppStartMons.Any(x => x > 5 && theGame.OppTeam[x - 6].Ability == "Unseen Fist" && theGame.OppTeam[x - 6].Item != "Punching Glove");
            bool threatOfFakeOut = OppCanFakeOut.Contains(true) && !ImmuneToFakeOut(monNo, user, theGame.OppTeam[theGame.Turns[^1].OppStartMons[OppCanFakeOut.IndexOf(true)]]);
            bool underPressure = underOffensvePressure[monNo] && !Moves.Any(x => x.MoveType != "Calculating...");
            if (allyMon >= 0)
            {
                underPressure = underPressure && !providingOffensvePressure[allyMon];
            }
            if (canProtect && (threatOfFakeOut || underPressure))
            {
                move.UserNo = monNo;
                move.TargetNo = monNo;
                string? moveName = user.Moves.Find(_protectionMoves.Contains);
                moveName ??= "Protect";
                move.MoveType = moveName;
                continue;
            }
            if (!usedGimmick)
            {
                if (threatOfFakeOut)
                {
                    int fakeOutUser = theGame.Turns[^1].OppStartMons[OppCanFakeOut.IndexOf(true)];
                    BestDamages? bestDamage = bestDamagesOpp.Find(x => x.MonNo == fakeOutUser && x.Target == monNo);
                    if (bestDamage == null)
                    {
                        break;
                    }
                    bool gimmickNotWorse = gimmickDamages[fakeOutUser][monNo][theGame.OppTeam[fakeOutUser].Moves.IndexOf(bestDamage.MoveName)][0] <= bestDamage.MinDamage;
                    int nonFakeOutUserIndex = theGame.Turns[^1].OppStartMons.FindIndex(x => x != fakeOutUser);
                    if (nonFakeOutUserIndex != -1)
                    {
                        int nonFakeOutUser = theGame.Turns[^1].OppStartMons[nonFakeOutUserIndex];
                        bestDamage = bestDamagesOpp.Find(x => x.MonNo == nonFakeOutUser && x.Target == monNo);
                        if (bestDamage != null)
                        {
                            gimmickNotWorse = gimmickNotWorse && gimmickDamages[nonFakeOutUser][monNo][theGame.OppTeam[nonFakeOutUser].Moves.IndexOf(bestDamage.MoveName)][0] <= bestDamage.MinDamage;
                        }
                    }
                    if (GimmickImmune(monNo, fakeOutUser, theGame.OppTeam[fakeOutUser].Moves.IndexOf("Fake Out"), gimmickDamages) && gimmickNotWorse)
                    {
                        move.UseGimmick(theGame.Gimmicks.GetGimmick());
                        usedGimmick = true;
                    }
                }
                else if (underPressure)
                {
                    bool gimmickNotWorse = true;
                    bool gimmickSignificant = false;
                    foreach (int opp in theGame.Turns[^1].OppStartMons)
                    {
                        if (opp == -1)
                        {
                            continue;
                        }
                        BestDamages? bestDamage = bestDamagesOpp.Find(x => x.MonNo == opp && x.Target == monNo);
                        if (bestDamage != null)
                        {
                            if (bestDamage.GimmickSignificant)
                            {
                                gimmickSignificant = true;
                            }
                            if (gimmickDamages[opp][monNo][theGame.OppTeam[opp].Moves.IndexOf(bestDamage.MoveName)][0] > bestDamage.MinDamage)
                            {
                                gimmickNotWorse = false;
                            }
                        }
                    }
                    if (gimmickNotWorse && gimmickSignificant)
                    {
                        move.UseGimmick(theGame.Gimmicks.GetGimmick());
                        usedGimmick = true;
                    }
                }
            }

            if (underOffensvePressure[monNo] && !move.UsingGimmick())
            {
                float damageToBeat = 0;
                foreach (BestDamages matchup in bestDamagesOpp)
                {
                    if (matchup.Target == monNo && matchup.OutspeedTarget)
                    {
                        damageToBeat += matchup.MinDamage;
                    }
                }
                int switchMon = ChooseSwitch(expectedDamages, damageToBeat);
                if (switchMon != -1 && CanSwitch(user))
                {
                    move.UserNo = monNo;
                    move.TargetNo = switchMon;
                    move.MoveType = "Switch";
                    theGame.BotTeam[switchMon].Position = "Switching";
                    continue;
                }
            }

            if (providingOffensvePressure[monNo])
            {
                Dictionary<int, Dictionary<int, Dictionary<int, List<float>>>> damagesToUse;
                if (move.UsingGimmick())
                {
                    damagesToUse = gimmickDamages;
                }
                else
                {
                    damagesToUse = expectedDamages;
                }
                for (int i = 0; i < 4; i++)
                {
                    if (move.TargetNo != -1)
                    {
                        break;
                    }
                    if (user.DisabledMoves.Contains(i) || user.OutOfPP.Contains(i))
                    {
                        continue;
                    }
                    string moveName = user.Moves[i];
                    MoveInfoModel moveInfo = allOptions.AllMoves[moveName];
                    if (moveInfo.target == "AllAdjacentFoes" || moveInfo.target == "AllAdjacent" || moveInfo.priotity > 0)
                    {
                        int target = -1;
                        foreach (int key in damagesToUse[monNo].Keys)
                        {
                            if (!theGame.Turns[^1].OppStartMons.Contains(key))
                            {
                                continue;
                            }
                            if (ImmuneToMove(moveName, theGame.OppTeam[key], user, key))
                            {
                                continue;
                            }
                            if (damagesToUse[monNo][key][i][0] >= theGame.OppTeam[key - 6].RemainingHP)
                            {
                                target = key;
                            }
                        }
                        if (target != -1)
                        {
                            move.UserNo = monNo;
                            move.TargetNo = target;
                            move.MoveType = user.Moves[i];
                            if (!usedGimmick && (theGame.Gimmicks.GetGimmick() == "Tera" || theGame.Gimmicks.GetGimmick() == "Mega"))
                            {
                                if (gimmickDamages.ContainsKey(move.UserNo) && gimmickDamages[move.UserNo].ContainsKey(move.TargetNo) && gimmickDamages[move.UserNo][move.TargetNo].ContainsKey(i))
                                {
                                    if (expectedDamages[move.UserNo][move.TargetNo][i][1] < theGame.OppTeam[move.TargetNo - 6].RemainingHP && gimmickDamages[move.UserNo][move.TargetNo][i][0] >= theGame.OppTeam[move.TargetNo - 6].RemainingHP)
                                    {
                                        move.UseGimmick(theGame.Gimmicks.GetGimmick());
                                        usedGimmick = true;
                                    }
                                    else
                                    {
                                        move.UseGimmick(null);
                                    }
                                }
                            }
                        }
                    }
                }
                if (move.TargetNo != -1)
                {
                    continue;
                }
                foreach (BestDamages matchup in bestDamagesOpp)
                {
                    if (matchup.Target != monNo)
                    {
                        continue;
                    }
                    if (_turnOrder.IndexOf(monNo) < _turnOrder.IndexOf(matchup.MonNo) && matchup.OKOChance &&
                        user.Moves.Any(x => allOptions.AllMoves[x].priotity > 0 && x != "Fake Out"))
                    {
                        int moveNo = -1;
                        float highestDamage = 0;
                        for (int i = 0; i < 4; i++)
                        {
                            if (user.DisabledMoves.Contains(i) || user.OutOfPP.Contains(i))
                            {
                                continue;
                            }
                            string moveName = user.Moves[i];
                            if (allOptions.AllMoves[moveName].priotity > 0 && moveName != "Fake Out" && !ImmuneToMove(moveName, theGame.OppTeam[matchup.MonNo], user, matchup.MonNo))
                            {
                                if (damagesToUse[monNo][matchup.MonNo][i][0] > highestDamage)
                                {
                                    highestDamage = damagesToUse[monNo][matchup.MonNo][i][0];
                                    moveNo = i;
                                }
                            }
                        }
                        if (moveNo != -1)
                        {
                            move.UserNo = monNo;
                            move.TargetNo = matchup.MonNo;
                            move.MoveType = user.Moves[moveNo];
                            if (!usedGimmick && (theGame.Gimmicks.GetGimmick() == "Tera" || theGame.Gimmicks.GetGimmick() == "Mega"))
                            {
                                if (gimmickDamages.ContainsKey(move.UserNo) && gimmickDamages[move.UserNo].ContainsKey(move.TargetNo) && gimmickDamages[move.UserNo][move.TargetNo].ContainsKey(moveNo))
                                {
                                    if (highestDamage < theGame.OppTeam[move.TargetNo - 6].RemainingHP && gimmickDamages[move.UserNo][move.TargetNo][moveNo][0] >= theGame.OppTeam[move.TargetNo - 6].RemainingHP)
                                    {
                                        move.UseGimmick(theGame.Gimmicks.GetGimmick());
                                        usedGimmick = true;
                                    }
                                    else
                                    {
                                        move.UseGimmick(null);
                                    }
                                }
                            }
                        }
                    }
                }
                if (move.TargetNo != -1)
                {
                    continue;
                }

                BestDamages? currBest = new(monNo, -1);
                foreach (BestDamages matchup in bestDamages)
                {
                    if (matchup.MonNo != monNo)
                    {
                        continue;
                    }
                    if ((matchup.OKOChance && !currBest.OKOChance) || (matchup.TKOGuaranteed && matchup.MinDamage > currBest.MinDamage) ||
                        (matchup.GimmickSignificant && matchup.OutspeedTarget && (!usedGimmick || move.UsingGimmick())))
                    {
                        currBest = matchup;
                    }
                }
                if (currBest.Target != -1)
                {
                    move.UserNo = monNo;
                    move.TargetNo = currBest.Target;
                    move.MoveType = currBest.MoveName;
                    if (currBest.GimmickSignificant && !usedGimmick)
                    {
                        move.UseGimmick(theGame.Gimmicks.GetGimmick());
                        usedGimmick = true;
                    }
                    continue;
                }
            }

            if (move.Dynamax)
            {
                continue;
            }

            if (user.Moves.Contains("Tailwind") && !theGame.CurrentArena.BotSide.Tailwind && !theGame.CurrentArena.TrickRoom && !Moves.Any(x => x.MoveType == "Tailwind" || x.MoveType == "Trick Room") &&
                !user.DisabledMoves.Contains(user.Moves.IndexOf("Tailwind")) && !user.OutOfPP.Contains(user.Moves.IndexOf("Tailwind")))
            {
                move.UserNo = monNo;
                move.TargetNo = monNo;
                move.MoveType = "Tailwind";
                continue;
            }
            if (user.Moves.Contains("Trick Room") && !theGame.CurrentArena.BotSide.Tailwind && !theGame.CurrentArena.TrickRoom && !bestDamages.Any(x => x.OutspeedTarget) && !Moves.Any(x => x.MoveType == "Tailwind" || x.MoveType == "Trick Room") &&
                !user.DisabledMoves.Contains(user.Moves.IndexOf("Trick Room")) && !user.OutOfPP.Contains(user.Moves.IndexOf("Trick Room")))
            {
                move.UserNo = monNo;
                move.TargetNo = monNo;
                move.MoveType = "Trick Room";
                continue;
            }

            if (user.Moves.Any(_screenMoves.Contains))
            {
                List<string> screenMoves = user.Moves.FindAll(_screenMoves.Contains);
                foreach (string moveName in screenMoves)
                {
                    if (user.DisabledMoves.Contains(user.Moves.IndexOf(moveName)) || user.OutOfPP.Contains(user.Moves.IndexOf(moveName)))
                    {
                        continue;
                    }
                    if (moveName == "Aurora Veil" && !(theGame.CurrentArena.Weather == "Snow" || theGame.CurrentArena.Weather == "Hail"))
                    {
                        continue;
                    }
                    if (_physicalScreenMoves.Contains(moveName) && (theGame.CurrentArena.BotSide.Reflect || theGame.CurrentArena.BotSide.AuroraVeil || Moves.Any(x => _physicalScreenMoves.Contains(x.MoveType))))
                    {
                        continue;
                    }
                    if (_specialScreenMoves.Contains(moveName) && (theGame.CurrentArena.BotSide.LightScreen || theGame.CurrentArena.BotSide.AuroraVeil || Moves.Any(x => _specialScreenMoves.Contains(x.MoveType))))
                    {
                        continue;
                    }
                    move.UserNo = monNo;
                    move.TargetNo = monNo;
                    move.MoveType = moveName;
                    break;
                }
                if (move.TargetNo != -1)
                {
                    continue;
                }
            }

            if (user.Moves.Any(_statusCausingMoves.ContainsKey))
            {
                foreach (string moveName in user.Moves)
                {
                    if (!_statusCausingMoves.ContainsKey(moveName))
                    {
                        continue;
                    }
                    if (_statusCausingMoves[moveName] != "Sleep")
                    {
                        continue;
                    }
                    if (user.DisabledMoves.Contains(user.Moves.IndexOf(moveName)) || user.OutOfPP.Contains(user.Moves.IndexOf(moveName)))
                    {
                        continue;
                    }
                    for (int i = 0; i < 2; i++)
                    {
                        int targetNo = theGame.Turns[^1].OppStartMons[i];
                        TeamModel target = theGame.OppTeam[targetNo - 6];
                        if (Moves.Any(x => x.TargetNo == targetNo && _statusCausingMoves.ContainsKey(x.MoveType)))
                        {
                            continue;
                        }
                        if (target.NonVolStatus != "")
                        {
                            continue;
                        }
                        if (ImmuneToStatus(target, "Sleep", allOptions.AllMoves[moveName]) || ImmuneToMove(moveName, target, user, targetNo))
                        {
                            continue;
                        }
                        move.UserNo = monNo;
                        move.TargetNo = targetNo;
                        move.MoveType = moveName;
                        break;
                    }
                    if (move.TargetNo != -1)
                    {
                        break;
                    }
                }
                if (move.TargetNo != -1)
                {
                    continue;
                }
                foreach (string moveName in user.Moves)
                {
                    if (!_statusCausingMoves.ContainsKey(moveName))
                    {
                        continue;
                    }
                    if (_statusCausingMoves[moveName] != "Burn")
                    {
                        continue;
                    }
                    if (user.DisabledMoves.Contains(user.Moves.IndexOf(moveName)) || user.OutOfPP.Contains(user.Moves.IndexOf(moveName)))
                    {
                        continue;
                    }
                    for (int i = 0; i < 2; i++)
                    {
                        int targetNo = theGame.Turns[^1].OppStartMons[i];
                        TeamModel target = theGame.OppTeam[targetNo - 6];
                        if (Moves.Any(x => x.TargetNo == targetNo && _statusCausingMoves.ContainsKey(x.MoveType)))
                        {
                            continue;
                        }
                        if (target.NonVolStatus != "")
                        {
                            continue;
                        }
                        if (CalcStat("Atk", targetNo) < CalcStat("SpA", targetNo))
                        {
                            continue;
                        }
                        if (ImmuneToStatus(target, "Burn", allOptions.AllMoves[moveName]) || ImmuneToMove(moveName, target, user, targetNo))
                        {
                            continue;
                        }
                        move.UserNo = monNo;
                        move.TargetNo = targetNo;
                        move.MoveType = moveName;
                        break;
                    }
                    if (move.TargetNo != -1)
                    {
                        break;
                    }
                }
                if (move.TargetNo != -1)
                {
                    continue;
                }
                foreach (string moveName in user.Moves)
                {
                    if (!_statusCausingMoves.ContainsKey(moveName))
                    {
                        continue;
                    }
                    if (_statusCausingMoves[moveName] != "Para")
                    {
                        continue;
                    }
                    if (user.DisabledMoves.Contains(user.Moves.IndexOf(moveName)) || user.OutOfPP.Contains(user.Moves.IndexOf(moveName)))
                    {
                        continue;
                    }
                    for (int i = 0; i < 2; i++)
                    {
                        int targetNo = theGame.Turns[^1].OppStartMons[i];
                        TeamModel target = theGame.OppTeam[targetNo - 6];
                        if (Moves.Any(x => x.TargetNo == targetNo && _statusCausingMoves.ContainsKey(x.MoveType)))
                        {
                            continue;
                        }
                        if (target.NonVolStatus != "")
                        {
                            continue;
                        }
                        int targetSpe = CalcStat("Spe", targetNo);
                        if (targetSpe < CalcStat("Spe", monNo))
                        {
                            continue;
                        }
                        if (allyMon != -1)
                        {
                            if (targetSpe < CalcStat("Spe", allyMon))
                            {
                                continue;
                            }
                        }
                        if (ImmuneToStatus(target, "Para", allOptions.AllMoves[moveName]) || ImmuneToMove(moveName, target, user, targetNo))
                        {
                            continue;
                        }
                        move.UserNo = monNo;
                        move.TargetNo = targetNo;
                        move.MoveType = moveName;
                        break;
                    }
                    if (move.TargetNo != -1)
                    {
                        break;
                    }
                }
                if (move.TargetNo != -1)
                {
                    continue;
                }
            }

            if (user.Moves.Any(_statLoweringMoves.ContainsKey))
            {
                foreach (string moveName in user.Moves)
                {
                    if (!_statLoweringMoves.ContainsKey(moveName))
                    {
                        continue;
                    }
                    if (!_statLoweringMoves[moveName].Contains("Spe"))
                    {
                        continue;
                    }
                    if (user.DisabledMoves.Contains(user.Moves.IndexOf(moveName)) || user.OutOfPP.Contains(user.Moves.IndexOf(moveName)))
                    {
                        continue;
                    }
                    for (int i = 0; i < 2; i++)
                    {
                        int targetNo = theGame.Turns[^1].OppStartMons[i];
                        TeamModel target = theGame.OppTeam[targetNo - 6];
                        if (ImmuneToLowerStat(target, "Spe") || ImmuneToMove(moveName, target, user, targetNo))
                        {
                            continue;
                        }
                        int targetSpe = CalcStat("Spe", targetNo);
                        if (targetSpe < CalcStat("Spe", monNo))
                        {
                            continue;
                        }
                        if (allyMon != -1)
                        {
                            if (targetSpe < CalcStat("Spe", allyMon))
                            {
                                continue;
                            }
                        }
                        move.UserNo = monNo;
                        move.TargetNo = targetNo;
                        move.MoveType = moveName;
                        break;
                    }
                    if (move.TargetNo != -1)
                    {
                        break;
                    }
                }
                if (move.TargetNo != -1)
                {
                    continue;
                }
                foreach (string moveName in user.Moves)
                {
                    if (!_statLoweringMoves.ContainsKey(moveName))
                    {
                        continue;
                    }
                    if (!_statLoweringMoves[moveName].Contains("Atk") && !_statLoweringMoves[moveName].Contains("SpA"))
                    {
                        continue;
                    }
                    if (user.DisabledMoves.Contains(user.Moves.IndexOf(moveName)) || user.OutOfPP.Contains(user.Moves.IndexOf(moveName)))
                    {
                        continue;
                    }
                    for (int i = 0; i < 2; i++)
                    {
                        int targetNo = theGame.Turns[^1].OppStartMons[i];
                        TeamModel target = theGame.OppTeam[targetNo - 6];
                        if (CalcStat("Atk", targetNo) >= CalcStat("SpA", targetNo))
                        {
                            if (ImmuneToLowerStat(target, "Atk") || ImmuneToMove(moveName, target, user, targetNo))
                            {
                                continue;
                            }
                            if (!_statLoweringMoves[moveName].Contains("Atk"))
                            {
                                continue;
                            }
                        }
                        else
                        {
                            if (ImmuneToLowerStat(target, "SpA") || ImmuneToMove(moveName, target, user, targetNo))
                            {
                                continue;
                            }
                            if (!_statLoweringMoves[moveName].Contains("SpA"))
                            {
                                continue;
                            }
                        }
                        move.UserNo = monNo;
                        move.TargetNo = targetNo;
                        move.MoveType = moveName;
                        break;
                    }
                    if (move.TargetNo != -1)
                    {
                        break;
                    }
                }
                if (move.TargetNo != -1)
                {
                    continue;
                }
                foreach (string moveName in user.Moves)
                {
                    if (!_statLoweringMoves.ContainsKey(moveName))
                    {
                        continue;
                    }
                    if (!_statLoweringMoves[moveName].Contains("Def") && !_statLoweringMoves[moveName].Contains("SpD"))
                    {
                        continue;
                    }
                    if (allyMon == -1)
                    {
                        break;
                    }
                    if (user.DisabledMoves.Contains(user.Moves.IndexOf(moveName)) || user.OutOfPP.Contains(user.Moves.IndexOf(moveName)))
                    {
                        continue;
                    }
                    for (int i = 0; i < 2; i++)
                    {
                        int targetNo = theGame.Turns[^1].OppStartMons[i];
                        TeamModel target = theGame.OppTeam[targetNo - 6];
                        if (CalcStat("Atk", allyMon) >= CalcStat("SpA", allyMon))
                        {
                            if (ImmuneToLowerStat(target, "Def") || ImmuneToMove(moveName, target, user, targetNo))
                            {
                                continue;
                            }
                            if (!_statLoweringMoves[moveName].Contains("Def"))
                            {
                                continue;
                            }
                        }
                        else
                        {
                            if (ImmuneToLowerStat(target, "SpD") || ImmuneToMove(moveName, target, user, targetNo))
                            {
                                continue;
                            }
                            if (!_statLoweringMoves[moveName].Contains("SpD"))
                            {
                                continue;
                            }
                        }
                        move.UserNo = monNo;
                        move.TargetNo = targetNo;
                        move.MoveType = moveName;
                        break;
                    }
                    if (move.TargetNo != -1)
                    {
                        break;
                    }
                }
                if (move.TargetNo != -1)
                {
                    continue;
                }
            }

            if (user.Moves.Any(_statRaisingMovesSelf.ContainsKey))
            {
                foreach (string moveName in user.Moves)
                {
                    if (!_statRaisingMovesSelf.ContainsKey(moveName))
                    {
                        continue;
                    }
                    if (!_statRaisingMovesSelf[moveName].Contains("Spe"))
                    {
                        continue;
                    }
                    if (user.Ability == "Contrary")
                    {
                        break;
                    }
                    if (user.DisabledMoves.Contains(user.Moves.IndexOf(moveName)) || user.OutOfPP.Contains(user.Moves.IndexOf(moveName)))
                    {
                        continue;
                    }
                    int monSpeed = CalcStat("Spe", monNo);
                    bool outsped = false;
                    foreach (int target in theGame.Turns[^1].OppStartMons)
                    {
                        if (target == -1)
                        {
                            continue;
                        }
                        if (CalcStat("Spe", target) > monSpeed)
                        {
                            outsped = true;
                        }
                    }
                    if (!outsped)
                    {
                        break;
                    }
                    move.UserNo = monNo;
                    move.TargetNo = monNo;
                    move.MoveType = moveName;
                    break;
                }
                if (move.TargetNo != -1)
                {
                    continue;
                }
                string statToBoost = "Atk";
                if (CalcStat("Atk", monNo) < CalcStat("SpA", monNo))
                {
                    statToBoost = "SpA";
                }
                foreach (string moveName in user.Moves)
                {
                    if (!_statRaisingMovesSelf.ContainsKey(moveName))
                    {
                        continue;
                    }
                    if (!_statRaisingMovesSelf[moveName].Contains(statToBoost))
                    {
                        continue;
                    }
                    if (user.Ability == "Contrary")
                    {
                        break;
                    }
                    if (user.DisabledMoves.Contains(user.Moves.IndexOf(moveName)) || user.OutOfPP.Contains(user.Moves.IndexOf(moveName)))
                    {
                        continue;
                    }
                    move.UserNo = monNo;
                    move.TargetNo = monNo;
                    move.MoveType = moveName;
                    break;
                }
                if (move.TargetNo != -1)
                {
                    continue;
                }
                bestDamagesOpp.Sort(delegate (BestDamages a, BestDamages b)
                    {
                        return (int)(b.MinDamage - a.MinDamage);
                    });
                BestDamages? biggestThreat = bestDamages.Find(x => x.Target == monNo);
                statToBoost = "Def";
                if (biggestThreat != null && allOptions.AllMoves[biggestThreat.MoveName].category == "Special")
                {
                    statToBoost = "SpD";
                }
                foreach (string moveName in user.Moves)
                {
                    if (!_statRaisingMovesSelf.ContainsKey(moveName))
                    {
                        continue;
                    }
                    if (!_statRaisingMovesSelf[moveName].Contains(statToBoost))
                    {
                        continue;
                    }
                    if (user.Ability == "Contrary")
                    {
                        break;
                    }
                    if (user.DisabledMoves.Contains(user.Moves.IndexOf(moveName)) || user.OutOfPP.Contains(user.Moves.IndexOf(moveName)))
                    {
                        continue;
                    }
                    move.UserNo = monNo;
                    move.TargetNo = monNo;
                    move.MoveType = moveName;
                    break;
                }
                if (move.TargetNo != -1)
                {
                    continue;
                }
            }


            if (user.Moves.Any(_statRaisingMovesAlly.ContainsKey) && allyMon != -1)
            {
                TeamModel ally = theGame.BotTeam[allyMon];
                foreach (string moveName in user.Moves)
                {
                    if (!_statRaisingMovesAlly.ContainsKey(moveName))
                    {
                        continue;
                    }
                    if (!_statRaisingMovesAlly[moveName].Contains("Spe"))
                    {
                        continue;
                    }
                    if (ally.Ability == "Contrary")
                    {
                        break;
                    }
                    if (user.DisabledMoves.Contains(user.Moves.IndexOf(moveName)) || user.OutOfPP.Contains(user.Moves.IndexOf(moveName)))
                    {
                        continue;
                    }
                    int monSpeed = CalcStat("Spe", allyMon);
                    bool outsped = false;
                    foreach (int target in theGame.Turns[^1].OppStartMons)
                    {
                        if (target == -1)
                        {
                            continue;
                        }
                        if (CalcStat("Spe", target) > monSpeed)
                        {
                            outsped = true;
                        }
                    }
                    if (!outsped)
                    {
                        break;
                    }
                    move.UserNo = monNo;
                    move.TargetNo = allyMon;
                    move.MoveType = moveName;
                    break;
                }
                if (move.TargetNo != -1)
                {
                    continue;
                }
                string statToBoost = "Atk";
                if (CalcStat("Atk", allyMon) < CalcStat("SpA", allyMon))
                {
                    statToBoost = "SpA";
                }
                foreach (string moveName in user.Moves)
                {
                    if (!_statRaisingMovesAlly.ContainsKey(moveName))
                    {
                        continue;
                    }
                    if (!_statRaisingMovesAlly[moveName].Contains(statToBoost))
                    {
                        continue;
                    }
                    if (ally.Ability == "Contrary")
                    {
                        break;
                    }
                    if (user.DisabledMoves.Contains(user.Moves.IndexOf(moveName)) || user.OutOfPP.Contains(user.Moves.IndexOf(moveName)))
                    {
                        continue;
                    }
                    move.UserNo = monNo;
                    move.TargetNo = allyMon;
                    move.MoveType = moveName;
                    break;
                }
                if (move.TargetNo != -1)
                {
                    continue;
                }
                bestDamagesOpp.Sort(delegate (BestDamages a, BestDamages b)
                    {
                        return (int)(b.MinDamage - a.MinDamage);
                    });
                BestDamages? biggestThreat = bestDamages.Find(x => x.Target == allyMon);
                statToBoost = "Def";
                if (biggestThreat != null && allOptions.AllMoves[biggestThreat.MoveName].category == "Special")
                {
                    statToBoost = "SpD";
                }
                foreach (string moveName in user.Moves)
                {
                    if (!_statRaisingMovesAlly.ContainsKey(moveName))
                    {
                        continue;
                    }
                    if (!_statRaisingMovesAlly[moveName].Contains(statToBoost))
                    {
                        continue;
                    }
                    if (ally.Ability == "Contrary")
                    {
                        break;
                    }
                    if (user.DisabledMoves.Contains(user.Moves.IndexOf(moveName)) || user.OutOfPP.Contains(user.Moves.IndexOf(moveName)))
                    {
                        continue;
                    }
                    move.UserNo = monNo;
                    move.TargetNo = monNo;
                    move.MoveType = moveName;
                    break;
                }
                if (move.TargetNo != -1)
                {
                    continue;
                }
            }
        }

        for (int i = 0; i < 2; i++)
        {
            int monNo = theGame.Turns[^1].BotStartMons[i];
            if (monNo == -1)
            {
                continue;
            }
            MoveModel move = Moves[i];
            if (move.TargetNo != -1)
            {
                continue;
            }
            BestDamages? bestOption = bestDamages.Find(x => x.MonNo == monNo);
            if (bestOption != null)
            {
                move.UserNo = monNo;
                move.TargetNo = bestOption.Target;
                move.MoveType = bestOption.MoveName;
                if (bestOption.GimmickSignificant && !usedGimmick)
                {
                    move.UseGimmick(theGame.Gimmicks.GetGimmick());
                    usedGimmick = true;
                }
            }
        }

        foreach (TeamModel mon in theGame.BotTeam)
        {
            if (mon.Position == "Switching")
            {
                mon.Position = "Reserve";
            }
        }
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
        if (tempMon.Ability == "Slow Start" && tempMon.AbilityActive == true)
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
        "Aurora Veil",
        "Safeguard"
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
    private readonly List<string> _powderMoves = [
        "Cotton Spore",
        "Magic Powder",
        "Poison Powder",
        "Powder",
        "Rage Powder",
        "Sleep Powder",
        "Spore",
        "Stun Spore"
    ];
    private readonly List<string> _categoryChangingMoves = [
        "Photon Geyser",
        "Shell Side Arm",
        "Tera Blast",
        "Tera Starstorm"
    ];
    private readonly List<string> _SpaDefMoves = [
        "Psyshock",
        "Psystrike",
        "Secret Sword"
    ];
    private readonly List<string> _protectionMoves = [
        "Baneful Bunker",
        "Burning Bulwark",
        "Detect",
        "King's Shield",
        "Max Guard",
        "Obstruct",
        "Protect",
        "Silk Trap",
        "Spiky Shield"
    ];
    private readonly List<string> _protectionBreakingMoves = [
        "Feint",
        "Hyperspace Fury",
        "Hyperspace Hole",
        "Phantom Force",
        "Shadow Force"
    ];
    private readonly List<string> _screenMoves = [
        "Aurora Veil",
        "Baddy Bad",
        "Glitzy Glow",
        "Light Screen",
        "Reflect"
    ];
    private readonly List<string> _physicalScreenMoves = [
        "Aurora Veil",
        "Baddy Bad",
        "Reflect"
    ];
    private readonly List<string> _specialScreenMoves = [
        "Aurora Veil",
        "Glitzy Glow",
        "Light Screen"
    ];
    private readonly Dictionary<string, string> _statusCausingMoves = new()
    {
        {"Buzzy Buzz", "Para"},
        {"Dark Void", "Sleep"},
        {"Glare", "Para"},
        {"Grass Whistle", "Sleep"},
        {"Hypnosis", "Sleep"},
        {"Lovely Kiss", "Sleep"},
        {"Nuzzle", "Para"},
        {"Sing", "Sleep"},
        {"Sleep Powder", "Sleep"},
        {"Spore", "Sleep"},
        {"Stun Spore", "Para"},
        {"Thunder Wave", "Para"},
        {"Will-O-Wisp", "Burn"},
        {"Yawn", "Sleep"}
    };
    private readonly Dictionary<string, List<string>> _statLoweringMoves = new()
    {
        {"Acid Spray", ["SpD"]},
        {"Apple Acid", ["SpD"]},
        {"Baby-Doll Eyes", ["Atk"]},
        {"Breaking Swipe", ["Atk"]},
        {"Bulldoze", ["Spe"]},
        {"Captivate", ["SpA"]},
        {"Charm", ["Atk"]},
        {"Chilling Water", ["Atk"]},
        {"Confide", ["SpA"]},
        {"Cotton Spore", ["Spe"]},
        {"Drum Beating", ["Spe"]},
        {"Eerie Impulse", ["SpA"]},
        {"Electroweb", ["Spe"]},
        {"Fake Tears", ["SpD"]},
        {"Feather Dance", ["Atk"]},
        {"Fire Lash", ["Def"]},
        {"Glaciate", ["Spe"]},
        {"Grav Apple", ["Def"]},
        {"Growl", ["Atk"]},
        {"Icy Wind", ["Spe"]},
        {"Leer", ["Def"]},
        {"Low Sweep", ["Spe"]},
        {"Lumina Crash", ["SpD"]},
        {"Lunge", ["Atk"]},
        {"Metal Sound", ["SpD"]},
        {"Mud Shot", ["Spe"]},
        {"Mystical Fire", ["SpA"]},
        {"Noble Roar", ["Atk", "SpA"]},
        {"Octolock", ["Def", "SpD"]},
        {"Parting Shot", ["Atk", "SpA"]},
        {"Play Nice", ["Atk"]},
        {"Pounce", ["Spe"]},
        {"Rock Tomb", ["Spe"]},
        {"Scary Face", ["Spe"]},
        {"Screech", ["Def"]},
        {"Skitter Smack", ["SpA"]},
        {"Snarl", ["SpA"]},
        {"Spicy Extract", ["Def"]},
        {"Spirit Break", ["SpA"]},
        {"Strength Sap", ["Atk"]},
        {"String Shot", ["Spe"]},
        {"Struggle Bug", ["SpA"]},
        {"Tar Shot", ["Spe"]},
        {"Tail Whip", ["Def"]},
        {"Tearful Look", ["Atk", "SpA"]},
        {"Thunderous Kick", ["Def"]},
        {"Tickle", ["Atk", "Def"]},
        {"Toxic Thread", ["Spe"]},
        {"Triple Arrows", ["Def", "SpD"]},
        {"Trop Kick", ["Atk"]},
        {"Venom Drench", ["Atk", "SpA", "Spe"]}
    };
    private readonly Dictionary<string, List<string>> _statRaisingMovesSelf = new()
    {
        {"Acid Armor", ["Def"]},
        {"Agility", ["Spe"]},
        {"Amnesia", ["SpD"]},
        {"Aqua Step", ["Spe"]},
        {"Aura Wheel", ["Spe"]},
        {"Autotomize", ["Spe"]},
        {"Barrier", ["Def"]},
        {"Belly Drum", ["Atk"]},
        {"Bulk Up", ["Atk", "Def"]},
        {"Calm Mind", ["SpA"]},
        {"Charge", ["SpD"]},
        {"Clangorous Soul", ["Atk", "Def", "SpA", "SpD", "Spe"]},
        {"Coil", ["Atk", "Def"]},
        {"Cosmic Power", ["Def", "SpD"]},
        {"Cotton Guard", ["Def"]},
        {"Curse", ["Atk", "Def"]},
        {"Defend Order", ["Def", "SpD"]},
        {"Defence Curl", ["Def"]},
        {"Dragon Dance", ["Atk", "Spe"]},
        {"Esper Wing", ["Spe"]},
        {"Fillet Away", ["Atk", "SpA", "Spe"]},
        {"Flame Charge", ["Spe"]},
        {"Flower Shield", ["Def", "SpD"]},
        {"Gear Up", ["Atk", "SpA"]},
        {"Geomancy", ["SpA", "SpD", "Spe"]},
        {"Growth", ["Atk", "SpA"]},
        {"Harden", ["Def"]},
        {"Hone Claws", ["Atk"]},
        {"Howl", ["Atk"]},
        {"Iron Defence", ["Def"]},
        {"Magnetic Flux", ["Def", "SpD"]},
        {"Meditate", ["Atk"]},
        {"Meteor Beam", ["SpA"]},
        {"Nasty Plot", ["SpA"]},
        {"No Retreat", ["Atk", "Def", "SpA", "SpD", "Spe"]},
        {"Order Up", ["Atk", "Def", "Spe"]},
        {"Power-Up Punch", ["Atk"]},
        {"Psyshield Bash", ["Def"]},
        {"Rapid Spin", ["Spe"]},
        {"Rock Polish", ["Spe"]},
        {"Rototiller", ["Atk", "SpA"]},
        {"Scale Shot", ["Spe"]},
        {"Sharpen", ["Atk"]},
        {"Shell Smash", ["Atk", "SpA", "Spe"]},
        {"Shelter", ["Def"]},
        {"Shift Gear", ["Atk", "Spe"]},
        {"Skull Bash", ["Def"]},
        {"Stockpile", ["Def", "SpD"]},
        {"Stuff Cheeks", ["Def"]},
        {"Swords Dance", ["Atk"]},
        {"Tail Glow", ["SpA"]},
        {"Tidy Up", ["Atk", "Spe"]},
        {"Torch Song", ["SpA"]},
        {"Trailblaze", ["Spe"]},
        {"Quiver Dance", ["SpA", "SpD", "Spe"]},
        {"Victory Dance", ["Atk", "Def", "Spe"]},
        {"Withdraw", ["Def"]},
        {"Work Up", ["Atk", "SpA"]}
    };
    private readonly Dictionary<string, List<string>> _statRaisingMovesAlly = new()
    {
        {"Aromatic Mist", ["SpD"]},
        {"Coaching", ["Atk", "Def"]},
        {"Decorate", ["Atk", "SpA"]},
        {"Flatter", ["SpA"]},
        {"Flower Shield", ["Def", "SpD"]},
        {"Gear Up", ["Atk", "SpA"]},
        {"Magnetic Flux", ["Def", "SpD"]},
        {"Rototiller", ["Atk", "SpA"]},
        {"Spicy Extract", ["Atk"]},
        {"Swagger", ["Atk"]}
    };
    private readonly Dictionary<string, List<string>> _invisibleAbilities = new()
    {
        {"Adaptability", [
            "Eevee",
            "Corphish",
            "Crawdaunt",
            "Feebas",
            "Basculin",
            "Basculin-Blue-Striped",
            "Basculin-White-Striped",
            "Skrelp",
            "Dragalge",
            "Yungoos",
            "Gumshoos",
            "Basculegion",
            "Basculegion-F"
        ]},
        {"Analytic", [
            "Magnemite",
            "Magneton",
            "Staryu",
            "Starmie",
            "Porygon",
            "Porygon2",
            "Magnezone",
            "Porygon-Z",
            "Patrat",
            "Watchog",
            "Elgyem",
            "Beheeyem"
        ]},
        {"Battery", []},
        {"Battle Armor", [
            "Cubone",
            "Marowak",
            "Kabuto",
            "Kabutops"
        ]},
        {"Blaze", [
            "Pansear",
            "Simisear"
        ]},
        {"Chlorophyll", [
            "Sewaddle",
            "Swadloon",
            "Leavanny",
            "Maractus",
            "Bulbasaur",
            "Ivysaur",
            "Venusaur",
            "Leafeon",
            "Cottonee",
            "Whimsicott"
            ]},
        {"Contrary", [
            "Shuckle",
            "Spinda",
            "Snivy",
            "Servine",
            "Serperior",
            "Fomantis",
            "Lurantis",
            "Enamorus"
        ]},
        {"Dragon\'s Maw", []},
        {"Dry Skin", [
            "Paras",
            "Parasect",
            "Jynx",
            "Croagunk",
            "Toxicroak"
        ]},
        {"Filter", [
            "Mr. Mime",
            "Mime Jr.",
            "Revavroom"
        ]},
        {"Fire Mane", []},
        {"Flare Boost", [
            "Drifloon",
            "Drifblim"
        ]},
        {"Fluffy", [
            "Greavard",
            "Houndstone"
        ]},
        {"Friend Guard", [
            "Clefairy",
            "Jigglypuff",
            "Cleffa",
            "Igglybuff",
            "Happiny",
            "Scatterbug",
            "Spewpa",
            "Vivillon"
        ]},
        {"Fur Coat", []},
        {"Gale Wings", [
            "Fletchling",
            "Fletchinder",
            "Talonflame"
        ]},
        {"Gorilla Tactics", []},
        {"Grass Pelt", [
            "Skiddo",
            "Gogoat"
        ]},
        {"Guts", [
            "Rattata",
            "Raticate",
            "Heracross",
            "Makuhita",
            "Hariyama",
            "Obstagoon",
            "Flareon",
            "Shinx",
            "Luxio",
            "Luxray",
            "Squawkabilly",
            "Squawkabilly-Blue"
        ]},
        {"Heatproof", [
            "Bronzor",
            "Bronzong",
            "Rolycoly",
            "Poltchageist",
            "Sinistcha"
        ]},
        {"Heavy Metal", [
            "Aron",
            "Lairon",
            "Aggron",
            "Bronzor",
            "Bronzong",
            "Cufant",
            "Copperajah",
            "Duraludon"
        ]},
        {"Huge Power", [
            "Marill",
            "Azumarill",
            "Azurill",
            "Bunnelby",
            "Diggersby"
        ]},
        {"Hustle", [
            "Rattata-Alola",
            "Raticate-Alola",
            "Delibird",
            "Lilligant-Hisui",
            "Durant",
            "Dracozolt",
            "Squawkabilly",
            "Rattata",
            "Raticate",
            "Nidoran-F",
            "Nidorina",
            "Nidoran-M",
            "Nidorino",
            "Combee",
            "Rufflet",
            "Flapple"
        ]},
        {"Ice Scales", [
            "Snom",
            "Frosmoth"
        ]},
        {"Infiltrator", [
            "Zubat",
            "Golbat",
            "Crobat",
            "Hoppip",
            "Skiploom",
            "Jumpluff",
            "Ninjask",
            "Seviper",
            "Spiritomb",
            "Cottonee",
            "Whimsicott",
            "Litwick",
            "Lampent",
            "Chandelure",
            "Espurr",
            "Meowstic",
            "Meowstic-F",
            "Inkay",
            "Malamar",
            "Noibat",
            "Noivern",
            "Dreepy",
            "Drakloak",
            "Dragapult",
            "Bramblin",
            "Brambleghast"
        ]},
        {"Inner Focus", [
            "Abra",
            "Kadabra",
            "Alakazam",
            "Farfetch\'d",
            "Drowzee",
            "Hypno",
            "Hitmonchan",
            "Kangaskhan",
            "Umbreon",
            "Raikou",
            "Entei",
            "Suicune",
            "Riolu",
            "Lucario",
            "Throh",
            "Sawk",
            "Darumaka",
            "Darumaka-Galar",
            "Pawniard",
            "Bisharp",
            "Mudbray",
            "Mudsdale",
            "Annihilape"
        ]},
        {"Iron Fist", [
            "Hitmonchan",
            "Ledian",
            "Chimchar",
            "Monferno",
            "Infernape",
            "Timburr",
            "Gurdurr",
            "Conkeldurr",
            "Crabrawler",
            "Crabominable",
            "Pawmi",
            "Pawmo",
            "Pawmot"
        ]},
        {"Light Metal", [
            "Scizor",
            "Beldum",
            "Metang",
            "Metagross",
            "Registeel"
        ]},
        {"Long Reach", [
            "Rowlet",
            "Dartrix",
            "Decidueye"
        ]},
        {"Magic Guard", [
            "Clefairy",
            "Clefable",
            "Abra",
            "Kadabra",
            "Alakazam",
            "Cleffa",
            "Sigilyph",
            "Solosis",
            "Duosion",
            "Reuniclus"
        ]},
        {"Magma Armor", []},
        {"Marvel Scale", [
            "Dratini",
            "Dragonair"
        ]},
        {"Mega Launcher", []},
        {"Mega Sol", []},
        {"Merciless", []},
        {"Mind\'s Eye", []},
        {"Minus", [
            "Electrike",
            "Manectric",
            "Klink",
            "Klang",
            "Klinklang",
            "Toxtricity-Low-Key"
        ]},
        {"Multiscale", [
            "Dragonite",
            "Lugia"
        ]},
        {"Mycelium Might", []},
        {"Neuroforce", []},
        {"Oblivious", [
            "Lickitung",
            "Wailmer",
            "Wailord",
            "Feebas",
            "Spheal",
            "Sealeo",
            "Walrein",
            "Lickilicky",
            "Salandit",
            "Salazzle",
            "Bounsweet",
            "Steenee",
            "Dondozo"
        ]},
        {"Overgrow", [
            "Pansage",
            "Simisage"
        ]},
        {"Plus", [
            "Mareep",
            "Flaaffy",
            "Ampharos",
            "Dedenne",
            "Toxtricity"
        ]},
        {"Power Spot", []},
        {"Prankster", [
            "Murkrow",
            "Sableye",
            "Volbeat",
            "Illumise",
            "Riolu",
            "Purrloin",
            "Liepard",
            "Meowstic",
            "Shroodle",
            "Grafaiai"
        ]},
        {"Prism Armor", []},
        {"Propeller Tail", [
            "Arrokuda",
            "Barraskewda" 
        ]},
        {"Punk Rock", []},
        {"Pure Power", []},
        {"Purifying Salt", []},
        {"Quick Feet", [
            "Teddiursa",
            "Ursaring",
            "Poochyena",
            "Mightyena",
            "Jolteon",
            "Zigzagoon",
            "Zigzagoon-Galar",
            "Linoon",
            "Linoon-Galar",
            "Shroomish"
        ]},
        {"Reckless", [
            "Hitmonlee",
            "Rhyhorn",
            "Rhydon",
            "Starly",
            "Staravia",
            "Staraptor",
            "Rhyperior",
            "Emboar",
            "Mienfoo",
            "Mienshao"
        ]},
        {"Rivalry", [
            "Nidoran-F",
            "Nidorina",
            "Nidoqueen",
            "Nidoran-M",
            "Nidorino",
            "Nidoking",
            "Beautifly",
            "Pidove",
            "Tranquill",
            "Unfezant"
        ]},
        {"Rock Head", [
            "Growlithe-Hisui",
            "Arcanine-Hisui",
            "Marowak-Alola",
            "Rhyhorn",
            "Rhydon",
            "Sudowoodo",
            "Aron",
            "Lairon",
            "Aggron",
            "Relicanth",
            "Bonsly",
            "Tyrantrum"
        ]},
        {"Rocky Payload", [
            "Bombirdier"
        ]},
        {"Sand Force", [
            "Diglett",
            "Diglett-Alola",
            "Dugtrio",
            "Dugtrio-Alola",
            "Nosepass",
            "Shellos",
            "Gastrodon",
            "Hippopotas",
            "Hippowdon",
            "Probopass",
            "Roggenrola",
            "Boldore",
            "Gigalith",
            "Drilbur",
            "Exadrill"
        ]},
        {"Sand Rush", [
            "Herdier",
            "Stoutland",
            "Lycanroc-Midday",
            "Sandshrew",
            "Sandslash",
            "Dracozolt",
            "Dracovish"
        ]},
        {"Scrappy", [
            "Farfetch\'d-Galar",
            "Kangaskhan",
            "Miltank",
            "Taillow",
            "Swellow",
            "Loudred",
            "Exploud",
            "Herdier",
            "Stoutland",
            "Pancham",
            "Pangoro",
            "Decidueye-Hisui",
            "Sirfetch\'d"
        ]},
        {"Shadow Shield", []},
        {"Sharpness", [
            "Gallade",
            "Samurott-Hisui",
            "Kleavor",
            "Veluza"
        ]},
        {"Sheer Force", [
            "Timburr",
            "Gurdurr",
            "Conkeldurr",
            "Druddigon",
            "Rufflet",
            "Braviary",
            "Braviary-Hisui",
            "Kleavor",
            "Nidoqueen",
            "Nidoking",
            "Krabby",
            "Kingler",
            "Tauros",
            "Totodile",
            "Croconaw",
            "Feraligatr",
            "Steelix",
            "Makuhita",
            "Hariyama",
            "Mawile",
            "Trapinch",
            "Bagon",
            "Cranidos",
            "Rampardos",
            "Landorus",
            "Toucannon",
            "Squawkabilly-Yellow",
            "Squawkabilly-White",
            "Cetoddle",
            "Cetitan"
        ]},
        {"Shell Armor", [
            "Krabby",
            "Kingler",
            "Lapras",
            "Omanyte",
            "Omastar",
            "Torkoal",
            "Corphish",
            "Crawdaunt",
            "Turtwig",
            "Grotle",
            "Torterra",
            "Oshawott",
            "Dewott",
            "Samurott",
            "Dwebble",
            "Crustle",
            "Escavalier",
            "Shelmet",
            "Sliggoo-Hisui",
            "Goodra-Hisui",
            "Chewtle",
            "Drednaw",
            "Klawf"
        ]},
        {"Shield Dust", [
            "Cutiefly",
            "Ribombee" 
        ]},
        {"Skill Link", [
            "Shellder",
            "Cloyster",
            "Aipom",
            "Ambipom",
            "Minccino",
            "Cinccino",
            "Pikipek",
            "Trumbeak",
            "Toucannon"
        ]},
        {"Slush Rush", [
            "Cubchu",
            "Beartic",
            "Cetitan",
            "Sandshrew-Alola",
            "Sandslash-Alola",
            "Arctozolt",
            "Arctovish"
        ]},
        {"Sniper", [
            "Beedrill",
            "Spearow",
            "Fearow",
            "Horsea",
            "Seadra",
            "Spinarak",
            "Ariados",
            "Remoraid",
            "Octillery",
            "Kingdra",
            "Skorupi",
            "Drapion",
            "Sobble",
            "Drizzile",
            "Inteleon"
        ]},
        {"Solid Rock", [
            "Camerupt",
            "Rhyperior"
        ]},
        {"Stakeout", [
            "Nickit",
            "Thievul",
            "Tarountula",
            "Spidops",
            "Maschiff",
            "Mabosstiff"
        ]},
        {"Stall", [
            "Sableye"
        ]},
        {"Stalwart", [
            "Duraludon",
            "Archaludon" 
        ]},
        {"Steelworker", []},
        {"Steely Spirit", [
           "Perrserker"
        ]},
        {"Stench", [
            "Gloom",
            "Koffing",
            "Weezing"
        ]},
        {"Strong Jaw", [
            "Yungoos",
            "Gumshoos",
            "Bruxish",
            "Dracovish"
        ]},
        {"Surge Surfer", []},
        {"Swarm", [
            "Volbeat",
            "Venipede",
            "Whirlipede",
            "Scolipede",
            "Joltik",
            "Galvantula",
            "Larvesta",
            "Volcarona"
        ]},
        {"Swift Swim", [
            "Quilfish",
            "Quilfish-Hisui",
            "Basculegion",
            "Basculegion-F",
            "Overquil",
            "Psyduck",
            "Golduck",
            "Poliwag",
            "Poliwhirl",
            "Poliwrath",
            "Anorith",
            "Armaldo",
            "Tirtouga",
            "Carracosta",
            "Beartic",
            "Chewtle",
            "Dreadnaw"
        ]},
        {"Technician", [
            "Meowth",
            "Meowth-Alola",
            "Persian",
            "Persian-Alola",
            "Mr. Mime",
            "Scyther",
            "Scizor",
            "Smeargle",
            "Hitmontop",
            "Breloom",
            "Kricketune",
            "Roserade",
            "Mime Jr.",
            "Minccino",
            "Cinccino",
            "Toxtricity",
            "Toxtricity-Low-Key",
            "Clobbopus",
            "Grapploct",
            "Maushold",
            "Fezandipiti"
        ]},
        {"Thick Fat", [
            "Rattata-Alola",
            "Raticate-Alola",
            "Snorlax",
            "Swinub",
            "Piloswine",
            "Munchlax",
            "Mamoswine",
            "Tepig",
            "Pignite",
            "Appletun",
            "Lechonk",
            "Oinkologne",
            "Oinkologne-F"
        ]},
        {"Tinted Lens", [
            "Butterfree",
            "Venonat",
            "Venomoth",
            "Hoothoot",
            "Noctowl",
            "Illumise",
            "Mothim",
            "Yanmega",
            "Sigilyph",
            "Braviary-Hisui",
            "Nymble",
            "Lokix"
        ]},
        {"Torrent", [
            "Panpour",
            "Simipour"
        ]},
        {"Tough Claws", [
            "Meowth-Galar",
            "Binacle",
            "Barbaracle",
            "Perrserker"
        ]},
        {"Toxic Boost", [
            "Zangoose"
        ]},
        {"Transistor", []},
        {"Triage", [
            "Comfey"
        ]},
        {"Unaware", [
            "Clefable",
            "Wooper",
            "Wooper-Paldea",
            "Quagsire",
            "Bidoof",
            "Bibarel",
            "Pyukumuku",
            "Fuecoco",
            "Crocalor",
            "Skeledirge",
            "Clodsire"
        ]},
        {"Unburden", [
            "Hitmonlee",
            "Treecko",
            "Grovyle",
            "Sceptile",
            "Drifloon",
            "Drifblim",
            "Purrloin",
            "Liepard",
            "Accelgor",
            "Swirlix",
            "Slurpuff",
            "Nickit",
            "Thievul",
            "Sneasler"
        ]},
        {"Unseen Fist", []},
        {"Water Bubble", []}
    };
    private readonly Dictionary<string, TypeChangeAbility> _typeChangeAbilities = new()
    {
          {"Aerilate",  new("Normal", "Flying", [])},
          {"Dragonize", new("Normal", "Dragon", [])},
          {"Galvanize", new("Normal", "Electric", [
              "Geodude-Alola",
              "Graveler-Alola",
              "Golem-Alola"
          ])},
          {"Liquid Voice", new(null, "Water", [
              "Popplio",
              "Brionne",
              "Primarina"
          ])},
          {"Normalize", new(null, "Normal", [
              "Skitty",
              "Delcatty"
          ])},
          {"Pixilate", new("Normal", "Fairy", [
              "Sylveon"
          ])},
          {"Refrigerate", new("Normal", "Ice", [])},
    };
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
    public class BestDamages(int user, int target)
    {
        public int MonNo { get; set; } = user;
        public int Target { get; set; } = target;
        public string MoveName { get; set; } = "";
        public float MinDamage { get; set; } = 0;
        public bool OKOChance { get; set; } = false;
        public bool TKOGuaranteed { get; set; } = false;
        public bool OutspeedTarget { get; set; } = false;
        public bool GimmickSignificant { get; set; } = false;
    }
    private class TypeChangeAbility(string? source, string result, List<string> users)
    {
        public string? source = source;
        public string result = result;
        public List<string> users = users;
    }
}
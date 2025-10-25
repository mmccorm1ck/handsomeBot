using System.Collections.Generic;
using System.Linq;
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
        // Put calc code here
    }
}
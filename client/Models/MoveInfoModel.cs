namespace HandsomeBot.Models;

public class MoveInfoModel() // Class holding info about a move
{
    public string? category;
    public string? type;
    public int? priotity;
    public bool? isZ;
    public bool? isMax;
    public string? target;
    public bool? makesContact;
    public int[]? multihit;
    public bool? multiaccuracy;
    public bool? secondaries;
    public bool? willCrit;
    public int[]? recoil;
    public int[]? drain;
    public bool? isPunch;
    public bool? isBite;
    public bool? isBullet;
    public bool? isSound;
    public bool? isPulse;
    public bool? isSlicing;
    public bool? isWind;
}
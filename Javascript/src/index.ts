import {calculate, Generations, GenerationNum, Pokemon, Move, Field, SPECIES} from "@Smogon/calc";
import * as I from "@smogon/calc/src/data/interface"

function ReturnAllMons(): void
{
    const genNum = parseInt(process.argv[3]);
    const temp = SPECIES[genNum];
    console.log("£start");
    for (const specie in temp)
    {
        console.log(specie);
    }
    console.log("£stop");
}

function CalcDamage(): void
{
    const genNum = parseInt(process.argv[3]);
    const gen = Generations.get(genNum as GenerationNum);
    const result = calculate(
        gen,
        ParsePokemon(gen, 4),
        ParsePokemon(gen, 5),
        ParseMove(gen),
        ParseField()
    );
    console.log("£start");
    console.log(result);
    console.log("£stop");
}

function ParsePokemon(gen: I.Generation, argNum: number): Pokemon
{
    console.log(gen);
    const temp = process.argv[argNum] as string;
    const fields = temp.split('%');
    let mon: Pokemon = {
        gen: gen as I.Generation,
        name: fields[0] as I.SpeciesName,
        species: fields[0] as I.Specie,
        gender: fields[1] as I.GenderName,
        item: fields[2] as I.ItemName,
        level: parseInt(fields[3]),
        ability: fields[4] as I.AbilityName,
        nature: fields[5] as I.NatureName
    };
    return mon;
}

function ParseMove(gen: I.Generation): Move
{
    let move: Move;
    move.gen = gen;
    move.name = process.argv[7] as I.MoveName;
    return move;
}

function ParseField(): Field
{
    let field: Field;
    const temp = process.argv[8] as string;
    const fields = temp.split('%');
    return field;
}

const funcToUse = process.argv[2];

if (funcToUse === 'l')
{
    ReturnAllMons();
}
else if (funcToUse === 'c')
{
    CalcDamage();
}
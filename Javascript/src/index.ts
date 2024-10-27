import {calculate, Generations, GenerationNum, Pokemon, Move, Field, SPECIES} from "@Smogon/calc";

function ReturnAllMons(): void
{
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
    const result = calculate(
        gen,
        ParsePokemon(4),
        ParsePokemon(5),
        ParseMove(),
        ParseField()
    );
    console.log("£start");
    console.log(result);
    console.log("£stop");
}

function ParsePokemon(argNum: Number): Pokemon
{
    let mon: Pokemon;
    return mon;
}

function ParseMove(): Move
{
    let move: Move;
    return move;
}

function ParseField(): Field
{
    let field: Field;
    return field;
}

const funcToUse = process.argv[2];
const genNum = parseInt(process.argv[3]);
const gen = Generations.get(genNum as GenerationNum);

if (funcToUse === 'l')
{
    ReturnAllMons();
}
else if (funcToUse === 'c')
{
    CalcDamage();
}
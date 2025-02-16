import {calculate, Generations, GenerationNum, Pokemon, Move, Field, SPECIES} from "@Smogon/calc";

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
        ParsePokemon(4),
        ParsePokemon(5),
        ParseMove(),
        ParseField()
    );
    console.log("£start");
    console.log(result);
    console.log("£stop");
}

function ParsePokemon(argNum: number): Pokemon
{
    const temp = process.argv[argNum] as string;
    const mon: Pokemon = JSON.parse(temp);
    return mon;
}

function ParseMove(): Move
{
    const temp = process.argv[7] as string;
    const move: Move = JSON.parse(temp);
    return move;
}

function ParseField(): Field
{
    const temp = process.argv[8] as string;
    const field: Field = JSON.parse(temp);
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
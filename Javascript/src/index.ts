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
        ParseMove()
        //ParseField()
    );
    console.log("£start");
    console.log(result.damage);
    console.log("£stop");
}

function ParsePokemon(argNum: number): Pokemon
{
    const temp: string = process.argv[argNum].replace(/#/g,'\"').replace(/_/g,' ');
    const mon: Pokemon = JSON.parse(temp);
    return mon;
}

function ParseMove(): Move
{
    const temp: string = process.argv[6].replace(/#/g,'\"').replace(/_/g,' ');
    console.log(temp);
    const move: Move = JSON.parse(temp);
    return move;
}

function ParseField(): Field
{
    const temp: string = process.argv[7].replace(/#/g,'\"').replace(/_/g,' ');
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
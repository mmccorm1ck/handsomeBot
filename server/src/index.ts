import {calculate, Generations, GenerationNum, Pokemon, Move, Field, SPECIES, ITEMS, ABILITIES} from "@Smogon/calc";

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

function ReturnAllItems(): void
{
    const genNum = parseInt(process.argv[3]);
    const temp = ITEMS[genNum];
    console.log("£start");
    for (const item in temp)
    {
        console.log(temp[item]);
    }
    console.log("£stop");
}

function ReturnAllAbilities(): void
{
    const genNum = parseInt(process.argv[3]);
    const temp = ABILITIES[genNum];
    console.log("£start");
    for (const ability in temp)
    {
        console.log(temp[ability]);
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
    //console.log(result.damage)
    console.log(result.moveDesc());
    //console.log(result.kochance(false));
    console.log("£stop");
}

function ParsePokemon(argNum: number): Pokemon
{
    const temp: string = process.argv[argNum].replace(/#/g,'\"').replace(/_/g,' ');
    const tempJson = JSON.parse(temp);
    const mon = new Pokemon(tempJson.gen, tempJson.name, tempJson.options);
    return mon;
}

function ParseMove(): Move
{
    const temp: string = process.argv[6].replace(/#/g,'\"').replace(/_/g,' ');
    const tempJson = JSON.parse(temp);
    const move = new Move(tempJson.gen, tempJson.name, tempJson.options);
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
else if (funcToUse === 'i')
{
    ReturnAllItems();
}
else if (funcToUse === 'a')
{
    ReturnAllAbilities();
}
else if (funcToUse === 'c')
{
    try {CalcDamage();}
    catch {
        console.log("£start");
        console.log("0 - 0%");
        console.log("£stop");
    }
}
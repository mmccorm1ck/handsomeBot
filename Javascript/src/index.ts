import {SPECIES} from "@Smogon/calc";

function ReturnAllMons(): void
{
    const gen = process.argv[3];
    const temp = SPECIES[gen];
    console.log("£start");
    for (const specie in temp)
    {
        console.log(specie);
    }
    console.log("£stop");
}

function CalcDamage(): void
{
    // calc damage percentages
    console.log("£start");
    // print damage percentages
    console.log("£stop");
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
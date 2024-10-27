import {SPECIES} from "@Smogon/calc";

function Test(): void
{
    console.log("£start");
    const temp = SPECIES[9];
    for (const specie in temp)
    {
        console.log(specie);
    }
    console.log("£stop");
}

Test();
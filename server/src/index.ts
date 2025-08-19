import {calculate, Generations, GenerationNum, Pokemon, Move, Field, SPECIES, ITEMS, ABILITIES} from "@Smogon/calc";
import 'dotenv/config';
import { createServer } from "http";

const port = process.env.port || 3000;

const server = createServer(async (req, res) => {
    console.log("New connection");
    const calcResult = handleRequest(req.url);
    const statusCode = calcResult.valid ? 200 : 404;
    const response: string = JSON.stringify(calcResult.result);
    res.writeHead(statusCode, {"Content-Type": "text/plain"});
    res.write(response);
    //console.log(response + " sent");
    res.end();
});

const handleRequest = (url:string) => {
    let valid = false;
    let result = {};
    const params = url.split('?');
    if (url === '/' || !params[1]) return { valid, result };
    console.log(params[1]);
    let inputJson: inputObject;
    try {
        inputJson = JSON.parse(decodeURI(params[1])) as inputObject;
    }
    catch
    {
        return {valid, result};
    }
    const genNum = inputJson.Gen || 0;
    if (params[0] === '/calc' && inputJson.BotMons && inputJson.OppMons) {
        result = calcDamages(inputJson);
        if (!result) valid = false;
        else valid = true;
    }
    else if (params[0] === '/mons') {
        result = SPECIES[genNum];
        valid = true;
    }
    else if (params[0] === '/items') {
        result = ITEMS[genNum];
        valid = true;
    }
    else if (params[0] === '/abilities') {
        result = ABILITIES[genNum];
        valid = true;
    }
    return { valid, result };
}

function calcDamages(input: inputObject): object[] {
    const gen = Generations.get(input.Gen as GenerationNum);
    const field: Field = input.Field || new Field;
    let results: object[] = [];
    for (const userMon of input.BotMons) {
        for (const oppMon of input.OppMons) {
            let tempResult: resultObject = {
                user: userMon.name,
                opponent: oppMon.name,
                damages: []
            };
            let options = { ability: userMon.ability.toString(), item: userMon.item.toString(), species: userMon.species.name };
            for (const moveName of userMon.moves) {
                const move: Move = new Move(gen, moveName, options);
                tempResult.damages.push({
                    attacker: "user",
                    damage: calculate(
                        gen,
                        userMon,
                        oppMon,
                        move,
                        field
                    )
                });
            }
            options = { ability: oppMon.ability.toString(), item: oppMon.item.toString(), species: oppMon.species.name };
            for (const moveName of oppMon.moves) {
                const move: Move = new Move(gen, moveName, options);
                tempResult.damages.push({
                    attacker: "opponent",
                    damage: calculate(
                        gen,
                        oppMon,
                        userMon,
                        move,
                        field
                    )
                });
            }
            results.push(tempResult);
        }
    }
    return results;
}

type resultObject = 
{
    user:string,
    opponent:string,
    damages:object[]
}

type inputObject = 
{
    Gen: number,
    BotMons?: Pokemon[],
    OppMons?: Pokemon[],
    Field?: Field
}

server.listen(port, () => {
    console.log("Server running on port " + port);
});
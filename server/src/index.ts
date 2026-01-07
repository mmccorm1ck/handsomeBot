import {calculate, Generations, GenerationNum, Pokemon, Move, Field, State, SPECIES, ITEMS, ABILITIES, MOVES} from "@Smogon/calc";
import { SpeciesData } from "@Smogon/calc/dist/data/species";
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
    res.end();
});

const handleRequest = (url:string) => {
    let valid = false;
    let result = {};
    const params = url.split('?');
    if (url === '/' || !params[1]) return { valid, result };
    let inputJson: inputObject;
    try {
        inputJson = JSON.parse(decodeURIComponent(params[1])) as inputObject;
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
        if (!inputJson.Filter) {
            result = SPECIES[genNum];
            valid = true;
        } else {
            inputJson.Filter.forEach(element => {
                const data: SpeciesData = SPECIES[genNum][element];
                if (data !== undefined) {
                    Object.defineProperty(result, element, {value: data, enumerable: true});    
                }
            });
            valid = true;
        }
    }
    else if (params[0] === '/items') {
        result = ITEMS[genNum];
        valid = true;
    }
    else if (params[0] === '/abilities') {
        result = ABILITIES[genNum];
        valid = true;
    }
    else if (params[0] === '/moves') {
        result = MOVES[genNum];
        valid = true;
    }
    return { valid, result };
}

function calcDamages(input: inputObject): object[] {
    const gen = Generations.get(input.Gen as GenerationNum);
    const field: Field = new Field(input.Field);
    let switchedField = Object.assign({}, field);
    switchedField.attackerSide = field.defenderSide;
    switchedField.defenderSide = field.attackerSide;
    let results: object[] = [];
    for (const rawUserMon of input.BotMons) {
        const userMon: Pokemon = new Pokemon(rawUserMon.gen as GenerationNum, rawUserMon.name, rawUserMon.options);
        for (const rawOppMon of input.OppMons) {
            const oppMon: Pokemon = new Pokemon(rawOppMon.gen as GenerationNum, rawOppMon.name, rawOppMon.options);
            let options = { ability: userMon.ability, item: userMon.item, species: userMon.species.name };
            for (let i = 0; i < userMon.moves.length; i++) {
                const moveName = userMon.moves[i];
                if (moveName === "None") continue;
                const move: Move = new Move(gen, moveName, options);
                try {
                    results.push({
                        BotUser: true,
                        UserMon: userMon.name,
                        TargetMon: oppMon.name,
                        MoveNo: i,
                        Damage: calculate(
                            gen,
                            userMon,
                            oppMon,
                            move,
                            field
                        ).desc()
                    });
            }
            catch {continue;}
            }
            options = { ability: oppMon.ability, item: oppMon.item, species: oppMon.species.name };
            for (let i = 0; i < oppMon.moves.length; i++) {
                const moveName = oppMon.moves[i];
                if (moveName === "None") continue;
                const move: Move = new Move(gen, moveName, options);
                try {
                    results.push({
                        BotUser: false,
                        UserMon: oppMon.name,
                        TargetMon: userMon.name,
                        MoveNo: i,
                        damage: calculate(
                            gen,
                            oppMon,
                            userMon,
                            move,
                            switchedField
                        ).desc()
                    });
                }
                catch {continue;}
            }
        }
    }
    return results;
}

type inputObject = 
{
    Gen: number,
    BotMons?: inputPokemon[],
    OppMons?: inputPokemon[],
    Field?: Partial<State.Field>,
    Filter?: string[]
}

type inputPokemon = 
{
    gen: number,
    name: string,
    options: object
}

server.listen(port, () => {
    console.log("Server running on port " + port);
});
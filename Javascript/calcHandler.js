var InputFunc = {
    CalcMove: function () {
        /*const gen = Generations.get(5);
        const result = calculate(
        gen,
        new Pokemon(gen, 'Gengar', {
            item: 'Choice Specs',
            nature: 'Timid',
            evs: {spa: 252},
            boosts: {spa: 1},
        }),
        new Pokemon(gen, 'Chansey', {
            item: 'Eviolite',
            nature: 'Calm',
            evs: {hp: 252, spd: 252},
        }),
        new Move(gen, 'Focus Blast')
        );*/
        //import("calc.ts");
        import("calc.ts");
        const result = Calc();
        OutputFunc.ReturnData(result);        
    }
}

var OutputFunc = {
    ReturnData: function (data) {
        console.log(data);
    }
}
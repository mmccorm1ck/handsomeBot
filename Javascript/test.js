var InputFunc = {
    Replyer: function (input) {
        let output;
        if (input == "Beep")
        {
            output = "Boop";
        }
        else
        {
            output = "Oops"
        }
        OutputFunc.Reply(output)
    }
}

var OutputFunc = {
    Reply: function (input) {
        console.log(input);
    }
}
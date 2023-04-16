using Newtonsoft.Json.Linq;
using System.CommandLine;
using System.CommandLine.Parsing;

var vOption = new Option<List<Tuple<string, JToken>>>(
    "-v",
    description: "The variables.",
    parseArgument: result =>
    {
        List<Tuple<string, JToken>> res = new();
        foreach (var item in result.Tokens)
        {
            var fragments = item!.Value.Split('=');

            try
            {
                // variable key
                var variableKey = fragments[0].Trim();

                // variable value, a json format value, such as json object, array, number, etc.
                var variableValue = JToken.Parse(fragments[1].Trim());

                res.Add(new Tuple<string, JToken>(variableKey, variableValue));
            }
            catch (Exception ex)
            {
                throw new FormatException(
                    $"Invalid value specified for {result.Argument.Name}. '{item!.Value} is not a valid Tuple<string, JToken>",
                    ex);
            }
        }
       
        return res;
    });

vOption.IsRequired = true;

var rootCommand = new RootCommand("Parse variables")
{
    vOption
};

rootCommand.SetHandler((opList) =>
{
    foreach (var item in opList)
    {
        Console.WriteLine($"variable key:{item.Item1}");
        Console.WriteLine($"variable value:{item.Item2}");
        Console.WriteLine("====");
    }
}, vOption);

await rootCommand.InvokeAsync(new string[] 
{ 
    "-v", "customHeaders=['accept-encoding: deflate', 'a: test']", 
    "-v", "test={'a' : 'b', 'c' : 123}", 
    "-v", "k=123", 
    "-v", "k='abc'" 
});
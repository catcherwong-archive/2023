using McMaster.Extensions.CommandLineUtils;
using McMaster.Extensions.CommandLineUtils.Abstractions;
using Newtonsoft.Json.Linq;
using System.Globalization;

var app = new CommandLineApplication();
app.ValueParsers.Add(new VariableParser());
app.HelpOption();
var v = app.Option<Tuple<string, JToken>>("-v", "The variables ", CommandOptionType.MultipleValue);

app.OnExecute(() =>
{
    foreach (var item in v.ParsedValues)
    {
        Console.WriteLine($"variable key:{item.Item1}");
        Console.WriteLine($"variable value:{item.Item2}");
        Console.WriteLine("====");
    }
});

return app.Execute(new string[] 
{ 
    "-v", "customHeaders=['accept-encoding:deflate', 'a: test']", 
    "-v", "test={'a':'b', 'c': 123}", 
    "-v", "k=123", 
    "-v", "k='abc'" 
});

public class VariableParser : IValueParser
{
    public Type TargetType { get; } = typeof(Tuple<string, JToken>);

    public object? Parse(string? argName, string? value, CultureInfo culture)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return default(Tuple<string, JToken>);
        }

        var fragments = value!.Split('=');

        try
        {
            // variable key
            var variableKey = fragments[0].Trim();

            // variable value, a json format value, such as json object, array, number, etc.
            var variableValue = JToken.Parse(fragments[1].Trim());

            return new Tuple<string, JToken>(variableKey, variableValue);
        }
        catch (Exception ex)
        {
            throw new FormatException(
                $"Invalid value specified for {argName}. '{value} is not a valid Tuple<string, JToken>",
                ex);
        }
    }
}
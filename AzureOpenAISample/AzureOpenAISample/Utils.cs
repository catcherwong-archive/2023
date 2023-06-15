namespace AzureOpenAISample
{
    internal class Utils
    {
        public static string GetEnvironmentVariable(string name)
        {
            string? value = Environment.GetEnvironmentVariable(name);
            if (string.IsNullOrEmpty(value))
            {
                Console.WriteLine("empty");
                return "";
            }

            return value;
        }
    }
}

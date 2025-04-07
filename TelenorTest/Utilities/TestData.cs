using System.IO;
using Newtonsoft.Json.Linq;

namespace TelenorTest.Utilities
{
    public static class TestData
    {
        private static JObject _testData;

        static TestData()
        {
            var jsonPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "TestData.json");
           

            if (!File.Exists(jsonPath))
            {
                Console.WriteLine("ERROR: TestData.json file not found.");
                return;
            }

            var json = File.ReadAllText(jsonPath);
            

            try
            {
                _testData = JObject.Parse(json);
            }
            catch (Exception ex)
            {
                Console.WriteLine("JSON PARSE ERROR: " + ex.Message);
            }
        }

        public static string BaseUrl => _testData["BaseUrl"]?.ToString();
        public static string DefaultAddress => _testData["DefaultAddress"]?.ToString();


    }
}

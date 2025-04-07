using Newtonsoft.Json;

namespace TelenorTest.Utilities
{
    public class TestDataReader
    {
        private static dynamic _testData;

        static TestDataReader()
        {
            var json = File.ReadAllText("TestData.json");
            _testData = JsonConvert.DeserializeObject<dynamic>(json);
        }

        public static string Get(string key) => _testData[key];
    }
}

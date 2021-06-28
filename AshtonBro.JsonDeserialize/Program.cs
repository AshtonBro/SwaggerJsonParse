using System.IO;

namespace AshtonBro.JsonDeserialize
{
    class Program
    {
        static void Main(string[] args)
        {
            var pathJsonFile = @"D:\Web-developer\3. С#\VS_source\AshtonBro.JsonDeserialize\AshtonBro.JsonDeserialize\Data\swagger.json";

            var savePath = @"D:\Web-developer\3. С#\VS_source\AshtonBro.JsonDeserialize\AshtonBro.JsonDeserialize\Data\";

            var swaggerParse = new SwaggerJsonParse();

            var jsonRef = swaggerParse.JsonParseRef(pathJsonFile); 

            var jsonOnlyMethod = swaggerParse.JsonParseMethod(pathJsonFile, "/API-M/SendFlight");

            var jsonDeleteMethod = swaggerParse.JsonParseRemove(pathJsonFile, "/API-M/SendFlight");

            File.WriteAllText($"{savePath}jsonRef.json", jsonRef);
            File.WriteAllText($"{savePath}jsonDeleteMethod.json", jsonDeleteMethod);
            File.WriteAllText($"{savePath}jsonOnlyMethod.json", jsonOnlyMethod);
        }
    }
}

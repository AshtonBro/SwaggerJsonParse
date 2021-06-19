using System.IO;

namespace AshtonBro.JsonDeserialize
{
    class Program
    {
        static void Main(string[] args)
        {
            var pathJsonFile = "#";

            var savePath = @"#";

            var swaggerParse = new SwaggerJsonParse();

            var jsonRef = swaggerParse.JsonParseRef(pathJsonFile);

            var jsonOnlyMethod = swaggerParse.JsonParseMethod(pathJsonFile, "/API-A/Login");

            var jsonDeleteMethod = swaggerParse.JsonParseRemove(pathJsonFile, "/API-A/Login");

            File.WriteAllText($"{savePath}jsonRef.json", jsonRef);
            File.WriteAllText($"{savePath}jsonDeleteMethod.json", jsonDeleteMethod);
            File.WriteAllText($"{savePath}jsonOnlyMethod.json", jsonOnlyMethod);
        }
    }
}

using System.IO;

namespace AshtonBro.JsonDeserialize
{
    class Program
    {
        static void Main(string[] args)
        {
            var pathJsonFile = "#";

            var savePath = @"#";

            var swaggerParce = new SwaggerJsonParse();

            var jsonRef = swaggerParce.JsonParseRef(pathJsonFile);

            var jsonOnlyMethod = swaggerParce.JsonParseMethod(pathJsonFile, "/API-A/Login");

            var jsonDeleteMethod = swaggerParce.JsonParseRemove(pathJsonFile, "/API-A/Login");


            //File.WriteAllText($"{savePath}jsonRef.json", jsonRef);
            File.WriteAllText($"{savePath}jsonDeleteMethod.json", jsonDeleteMethod);
            File.WriteAllText($"{savePath}jsonOnlyMethod.json", jsonOnlyMethod);
        }
    }
}


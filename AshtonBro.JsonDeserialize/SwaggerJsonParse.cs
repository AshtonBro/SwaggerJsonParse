using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Globalization;
using System.IO;
using System.Linq;

namespace AshtonBro.JsonDeserialize
{
    public class SwaggerJsonParse
    {
        const string refPropertyName = "$ref";

        private int refsCount { get; set; } = 1;

        /// <summary>
        /// Returns modified string json where all refs are filled
        /// </summary>
        /// <param name="pathJson">Json file path</param>
        /// <returns></returns>
        public string JsonParseRef(string pathJson)
        {
            if (pathJson is null || pathJson.Count() == 0) return null;

            var data = File.ReadAllText(pathJson);

            var root = JsonConvert.DeserializeObject<JToken>(data, new JsonSerializerSettings { DateParseHandling = DateParseHandling.None });

            if (root is not JContainer container) return null;

            while (refsCount != 0)
            {
                var refs = container.Descendants().OfType<JObject>().Where(f => GetRefObjectValue(f) != null).ToList();

                refsCount = refs.Count();

                JsonConvert.SerializeObject(refs);

                foreach (var refObj in refs)
                {
                    var path = (string)GetRefObjectValue(refObj);

                    var original = ReplaceRef(root, path);

                    if (original != null) refObj.Replace(original);

                }
            }

            return root.ToString();
        }

        /// <summary>
        /// Returns a modified json string in which the specified method has been removed
        /// </summary>
        /// <param name="pathJson">Json file path</param>
        /// <param name="methodName">The method to be removed from the json file</param>
        /// <returns></returns>
        public string JsonParseRemove(string pathJson, string methodName)
        {
            if (pathJson is null || pathJson.Count() == 0) return null;

            var data = File.ReadAllText(pathJson);

            var root = JsonConvert.DeserializeObject<JToken>(data, new JsonSerializerSettings { DateParseHandling = DateParseHandling.None });

            if (!string.IsNullOrEmpty(methodName))
            {
                if (root is JObject rootObj)
                {
                    var paths = (JObject)rootObj["paths"];

                    if (paths is not null)
                    {
                        if (paths.Property(methodName) is not null)
                        {
                            var refs = paths.Property(methodName).Descendants().OfType<JObject>().Where(f => GetRefObjectValue(f) != null).ToList();

                            paths.Property(methodName).Remove();

                            foreach (var refObj in refs)
                            {
                                var path = (string)GetRefObjectValue(refObj);

                                DeleteRef(rootObj, path);
                            }

                            return rootObj.ToString();
                        }

                        return null;
                    }

                    return null;
                }

                return null;
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// Returns the modified json string, the specified method is saved, and all the rest will be removed
        /// </summary>
        /// <param name="pathJson">Json file path</param>
        /// <param name="methodName">The method to be saved in the json file</param>
        /// <returns></returns>
        public string JsonParseMethod(string pathJson, string methodName)
        {
            if (pathJson is null || pathJson.Count() == 0) return null;

            var data = File.ReadAllText(pathJson);

            var root = JsonConvert.DeserializeObject<JToken>(data, new JsonSerializerSettings { DateParseHandling = DateParseHandling.None });

            if (root is not JContainer container) return null;

            var refs = container.Descendants().OfType<JObject>().Where(f => GetRefObjectValue(f) != null).ToList();

            if (!string.IsNullOrEmpty(methodName))
            {
                var jMethods = container.Descendants()
                    .Where(t => t.Type == JTokenType.Property && ((JProperty)t).Name == methodName);

                var jMethod = jMethods.FirstOrDefault();

                if (root is JObject rootObj)
                {
                    var paths = (JObject)rootObj["paths"];

                    if (paths is not null)
                    {
                        paths.RemoveAll();

                        paths.Add(((JProperty)jMethod).Name, ((JProperty)jMethod).Value);

                        var refsMethod = paths.Property(methodName).Descendants().OfType<JObject>().Where(f => GetRefObjectValue(f) != null).ToList().FirstOrDefault();

                        foreach (var refObj in refs)
                        {
                            if (!JObject.DeepEquals(refsMethod, refObj))
                            {
                                var path = (string)GetRefObjectValue(refObj);

                                DeleteRef(rootObj, path);
                            }
                        }

                        return rootObj.ToString();
                    }
                    
                    return null;
                }

                return null;
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// Return $ref link value
        /// </summary>
        /// <param name="jsonObj">Json Object</param>
        /// <returns></returns>
        static JToken GetRefObjectValue(JObject jsonObj)
        {
            if (jsonObj.Count == 1)
            {
                var refValue = jsonObj[refPropertyName];

                if (refValue != null && refValue.Type == JTokenType.String)
                {
                    return refValue;
                }
            }
            return null;
        }

        /// <summary>
        /// Returns a JToken with the data inserted by the ref reference
        /// </summary>
        /// <param name="token">JToken Json file</param>
        /// <param name="path">The path taken from ref</param>
        /// <returns></returns>
        static JToken ReplaceRef(JToken token, string path)
        {
            var components = path[2..].Split('/');

            foreach (var component in components)
            {
                if (token is JObject obj)
                {
                    token = obj[component];
                }
                else if (token is JArray array)
                {
                    token = token[int.Parse(component, NumberFormatInfo.InvariantInfo)];
                }
                else
                {
                    throw new JsonException("Unexpected token type.");
                }
            }
            return token;
        }

        /// <summary>
        /// Delete component from ref path in json
        /// </summary>
        /// <param name="token">JToken Json file</param>
        /// <param name="path">The path which ref remove</param>
        static void DeleteRef(JToken token, string path)
        {
            var components = path[2..].Split('/');

            foreach (var component in components)
            {
                if (token is JObject obj)
                {
                    token = obj[component];

                    if (component.Contains("schemas"))
                    {
                        var objToket = (JObject)token;

                        if(objToket.Property(components.Last()) == null)
                        {
                            break;
                        }

                        objToket.Property(components.Last()).Remove();
                    }
                }
                else
                {
                    throw new JsonException("Unexpected token type.");
                }
            }
        }
    }
}

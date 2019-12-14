using Newtonsoft.Json;

namespace VanillaTransformer.Core.PostTransformations
{
    public class ReFormatJSONTransformation : IPostTransformation
    {
        public string Name => "ReFormatJSON";
        public string Execute(string configContent)
        {
            object parsedJson = JsonConvert.DeserializeObject(configContent);
            return JsonConvert.SerializeObject(parsedJson, Formatting.Indented);
        }
    }
}
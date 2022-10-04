using System.Text.Json;

namespace Scrapper.Common
{
    public class JsonUtilities
    {
        public static bool IsValid(string source)
        {
            if (source == null)
            {
                return false;
            }

            try
            {
                JsonDocument.Parse(source);
                return true;
            }
            catch (JsonException)
            {
                return false;
            }
        }
    }
}
using System.Text.Json;

namespace Discovery.Common
{
    public static class JsonUtilities
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
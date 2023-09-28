using Dalamud.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NOTED.Models;
using System;
using System.IO;
using System.IO.Compression;
using System.Text;

namespace NOTED.Helpers
{
    internal static class ImportExportHelper
    {
        public static string CompressAndBase64Encode(string jsonString)
        {
            using MemoryStream output = new();

            using (DeflateStream gzip = new(output, CompressionLevel.Optimal))
            {
                using StreamWriter writer = new(gzip, Encoding.UTF8);
                writer.Write(jsonString);
            }

            return Convert.ToBase64String(output.ToArray());
        }

        public static string Base64DecodeAndDecompress(string base64String)
        {
            var base64EncodedBytes = Convert.FromBase64String(base64String);

            using MemoryStream inputStream = new(base64EncodedBytes);
            using DeflateStream gzip = new(inputStream, CompressionMode.Decompress);
            using StreamReader reader = new(gzip, Encoding.UTF8);
            var decodedString = reader.ReadToEnd();

            return decodedString;
        }

        public static string GenerateExportString(Duty duty, Note note)
        {
            JsonSerializerSettings settings = new JsonSerializerSettings
            {
                TypeNameAssemblyFormatHandling = TypeNameAssemblyFormatHandling.Simple,
                TypeNameHandling = TypeNameHandling.Objects
            };

            return duty.ID.ToString() + "|" + duty.Name + "|" + CompressAndBase64Encode(JsonConvert.SerializeObject(note, Formatting.Indented, settings));
        }

        public static (uint, string?, Note?) ImportNote(string importString)
        {
            string[] importStrings = importString.Trim().Split(new string[] { "|" }, StringSplitOptions.RemoveEmptyEntries);
            if (importStrings.Length < 3) { return (0, null, null); }

            uint id = 0;
            string? dutyName = null;
            Note? note = null;

            try
            {
                id = uint.Parse(importStrings[0]);
                dutyName = importStrings[1];

                string jsonString = Base64DecodeAndDecompress(importStrings[2]);

                string? typeString = (string?)JObject.Parse(jsonString)["$type"];
                if (typeString == null) { return (0, null, null); }

                Type? type = Type.GetType(typeString);
                if (type == null || type != typeof(Note)) { return (0, null, null); }

                note = JsonConvert.DeserializeObject<Note>(jsonString);
                if (note == null) { return (0, null, null); }

                return (id, dutyName, note);
            }
            catch (Exception e)
            {
                Plugin.Logger.Error(e.Message);
            }

            return (0, null, null);
        }
    }
}

using System.Text.Json.Serialization;

namespace dotnet_rpg.Models {

    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum RpgClass {
        S = 1,
        A = 2,
        B = 3,
        C = 4,
    }
}

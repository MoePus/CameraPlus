using Newtonsoft.Json;

namespace CameraPlus
{
    [JsonObject("StartPos")]
    public class StartPos
    {
        [JsonProperty("x")]
        public string x { get; set; }
        [JsonProperty("y")]
        public string y { get; set; }
        [JsonProperty("z")]
        public string z { get; set; }
        [JsonProperty("FOV")]
        public string FOV { get; set; }
    }

    [JsonObject("StartRot")]
    public class StartRot
    {
        [JsonProperty("x")]
        public string x { get; set; }
        [JsonProperty("y")]
        public string y { get; set; }
        [JsonProperty("z")]
        public string z { get; set; }
    }

    [JsonObject("EndPos")]
    public class EndPos
    {
        [JsonProperty("x")]
        public string x { get; set; }
        [JsonProperty("y")]
        public string y { get; set; }
        [JsonProperty("z")]
        public string z { get; set; }
        [JsonProperty("FOV")]
        public string FOV { get; set; }
    }

    [JsonObject("EndRot")]
    public class EndRot
    {
        [JsonProperty("x")]
        public string x { get; set; }
        [JsonProperty("y")]
        public string y { get; set; }
        [JsonProperty("z")]
        public string z { get; set; }
    }

    [JsonObject("Movements")]
    public class JSONMovement
    {
        [JsonProperty("StartPos")]
        public StartPos startPos { get; set; }
        [JsonProperty("StartRot")]
        public StartRot startRot { get; set; }
        [JsonProperty("EndPos")]
        public EndPos endPos { get; set; }
        [JsonProperty("EndRot")]
        public EndRot endRot { get; set; }

        [JsonProperty("Duration")]
        public string Duration { get; set; }
        [JsonProperty("Delay")]
        public string Delay { get; set; }
        [JsonProperty("EaseTransition")]
        public string EaseTransition { get; set; }
    }

    [JsonObject("MovementScriptJson")]
    public class MovementScriptJson
    {
        [JsonProperty("ActiveInPauseMenu")]
        public string ActiveInPauseMenu { get; set; }
        [JsonProperty("Movements")]
        public JSONMovement[] Jsonmovement { get; set; }
    }
}

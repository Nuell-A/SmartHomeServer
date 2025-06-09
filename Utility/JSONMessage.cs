/*
This class is our "JSON" object (e.g. class that is serialized to JSON), four our communication protocol.
*/

class JSONMessage
{
    public string source { get; set; } = "";
    public string type { get; set; } = "";
    public string device_ip { get; set; } = "";
    public string device_mac { get; set; } = "";
    public Dictionary<string, string> state { get; set; } = new();
    public string timestamp { get; set; } = "";
    public string status { get; set; } = "";
    public string error { get; set; } = "";
}

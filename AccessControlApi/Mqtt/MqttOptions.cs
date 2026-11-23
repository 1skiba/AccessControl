namespace AccessControlApi.Mqtt;

public class MqttOptions
{
    public string Host { get; set; } = "localhost";
    public int Port { get; set; } = 1883;
    public string ClientId { get; set; } = "access-control-api";
    public string? Username { get; set; }
    public string? Password { get; set; }
    public string ScanTopic { get; set; } = "+/rfid";
    public string? ResponseTopicSuffix { get; set; } = "response"; 
}
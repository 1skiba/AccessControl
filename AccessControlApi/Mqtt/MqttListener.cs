using System.Text.Json;
using AccessControlApi.Entities.Enums;
using AccessControlApi.Services;
using Microsoft.Extensions.Options;

namespace AccessControlApi.Mqtt;
using MQTTnet;

public class MqttListener : BackgroundService
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly MqttOptions _opt;
    private readonly ILogger<MqttListener> _logger;
    private IMqttClient? _client;

    public MqttListener(IServiceScopeFactory scopeFactory, IOptions<MqttOptions> options, ILogger<MqttListener> logger)
    {
        _scopeFactory = scopeFactory;
        _opt = options.Value;
        _logger = logger;
    }
    
    protected override async Task ExecuteAsync(CancellationToken ct)
    {
        _client = new MqttClientFactory().CreateMqttClient();
        _client.ApplicationMessageReceivedAsync += HandleMessageAsync;
        
        var builder = new MqttClientOptionsBuilder()
            .WithTcpServer(_opt.Host, _opt.Port)
            .WithClientId(_opt.ClientId)
            .WithCleanSession(false);
        
        if (!string.IsNullOrEmpty(_opt.Username))
            builder.WithCredentials(_opt.Username, _opt.Password);
        
        var options = builder.Build();
        
        while (!ct.IsCancellationRequested)
        {
            try
            {
                if (!_client.IsConnected)
                {
                    await _client.ConnectAsync(options, ct);
                    await _client.SubscribeAsync(_opt.ScanTopic, cancellationToken: ct);
                    _logger.LogInformation("MQTT connected, subscribed to {Topic}", _opt.ScanTopic);
                }
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "MQTT connection failed, retrying in 5s");
            }

            try { await Task.Delay(TimeSpan.FromSeconds(5), ct); }
            catch (OperationCanceledException) { break; }
        }
    }

    private async Task HandleMessageAsync(MqttApplicationMessageReceivedEventArgs e)
    {
        try
        {
            // location/rfid
            var parts = e.ApplicationMessage.Topic.Split('/');
            if (parts.Length != 2) return;
            var location = parts[0];

            var uid = NormalizeUid(e.ApplicationMessage.ConvertPayloadToString());
            if (string.IsNullOrWhiteSpace(uid)) return;

            using var scope = _scopeFactory.CreateScope();
            var service = scope.ServiceProvider.GetRequiredService<AccessService>();
            var decision = await service.ProcessScanAsync(location, uid);

            if (decision is null)
            {
                _logger.LogWarning("Scan from unknown reader location {Location}", location);
                return;
            }

            _logger.LogInformation("{Location}: {Uid} -> {Result} {Reason}",
                location, uid, decision.Result, decision.Reason);

            if (!string.IsNullOrEmpty(_opt.ResponseTopicSuffix))
            {
                var response = new MqttApplicationMessageBuilder()
                    .WithTopic($"{location}/{_opt.ResponseTopicSuffix}")
                    .WithPayload(JsonSerializer.Serialize(
                        new { granted = decision.Result == AccessResult.Granted }))
                    .Build();
                await _client!.PublishAsync(response);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to process MQTT message");
        }
    }
    
    // "04:a1:b2:c3" -> "04A1B2C3"
    private static string NormalizeUid(string payload) =>
        payload.Trim().Replace(":", "").Replace(" ", "").ToUpperInvariant();
}
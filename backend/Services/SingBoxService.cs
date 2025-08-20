using System.Diagnostics;

namespace Backend.Services;

public interface ISingBoxService
{
    bool IsRunning { get; }
    Task StartAsync();
    Task StopAsync();
    Task RestartAsync();
}

public class SingBoxService : ISingBoxService
{
    private Process? _process;
    private readonly string _binaryPath;
    private readonly string _configPath;

    public SingBoxService(IConfiguration config)
    {
        _binaryPath = config.GetValue<string>("SingBox:BinaryPath") ?? "sing-box";
        _configPath = config.GetValue<string>("SingBox:ConfigPath") ?? "singbox.json";
    }

    public bool IsRunning => _process != null && !_process.HasExited;

    public Task StartAsync()
    {
        if (IsRunning) return Task.CompletedTask;
        _process = Process.Start(new ProcessStartInfo
        {
            FileName = _binaryPath,
            Arguments = $"run -c {_configPath}",
            UseShellExecute = false,
        });
        return Task.CompletedTask;
    }

    public Task StopAsync()
    {
        if (IsRunning)
        {
            _process!.Kill(true);
            _process!.WaitForExit();
            _process = null;
        }
        return Task.CompletedTask;
    }

    public async Task RestartAsync()
    {
        await StopAsync();
        await StartAsync();
    }
}

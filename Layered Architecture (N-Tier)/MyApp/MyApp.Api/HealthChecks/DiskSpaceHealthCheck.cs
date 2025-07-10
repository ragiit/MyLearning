using Microsoft.Extensions.Diagnostics.HealthChecks;

public class DiskSpaceHealthCheck : IHealthCheck
{
    public Task<HealthCheckResult> CheckHealthAsync(
        HealthCheckContext context,
        CancellationToken cancellationToken = default)
    {
        // Contoh logika dummy
        var drive = new DriveInfo(Path.GetPathRoot(Environment.CurrentDirectory)!);

        if (drive.AvailableFreeSpace < 1_000_000_000) // kurang dari 1GB
        {
            return Task.FromResult(
                HealthCheckResult.Unhealthy($"Low disk space: {drive.AvailableFreeSpace} bytes remaining."));
        }

        return Task.FromResult(
            HealthCheckResult.Healthy($"Available disk space: {drive.AvailableFreeSpace} bytes."));
    }
}
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace SimpleApi
{
    public class MyCustomHealthCheck : IHealthCheck
    {
        Task<HealthCheckResult> IHealthCheck.CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken)
        {
            // Implement your health check logic here
            // Example: Check if a database connection is available
            try
            {
                // Your database connection logic
                return Task.FromResult(HealthCheckResult.Healthy());
            }
            catch (Exception ex)
            {
                return Task.FromResult(new HealthCheckResult(context.Registration.FailureStatus, exception: ex));
            }
        }
    }
}

using System;
using System.Threading.Tasks;

using Microsoft.Extensions.Logging;

using Polly;

namespace Healthcare.Engineering.Services;

using Healthcare.Engineering.DataObject.Settings;
using Interfaces;

public class EmailService : IEmailService
{
    private readonly ChaosGenerator _chaosGenerator;
    private readonly ILogger<EmailService> _logger;
    private readonly RetryPolicy _retryPolicy;

    public EmailService(ChaosGenerator chaosGenerator, ILogger<EmailService> logger, RetryPolicy retryPolicy)
    {
        _chaosGenerator = chaosGenerator;
        _logger = logger;
        _retryPolicy = retryPolicy;
    }

    public async Task Send(string email, string message)
    {
        _logger.LogInformation("Sending e-mail message '{message}' to '{email}'.", message, email);

        var retryPolicy = Policy.Handle<Exception>().WaitAndRetryAsync(_retryPolicy.Retries,
            attempt =>
            {
                _logger.LogError("Request to e-mail service failed. Retry '{attempt}' of '{Retries}'.", attempt,
                    _retryPolicy.Retries);

                return TimeSpan.FromSeconds(_retryPolicy.PauseBetweenRetries);
            });

        try
        {
            await retryPolicy.ExecuteAsync(async () =>
            {
                _logger.LogInformation("Rolling the dice!");
                _chaosGenerator.RollTheDice();

                _logger.LogInformation("Waiting for 10 seconds!");
                await Task.Delay(10000);
            });
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Email send task failed.");
        }
    }
}
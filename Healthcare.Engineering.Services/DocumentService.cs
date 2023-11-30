using System;
using System.Threading.Tasks;

using Microsoft.Extensions.Logging;

using Polly;

namespace Healthcare.Engineering.Services;

using Healthcare.Engineering.DataObject.Settings;
using Interfaces;

public class DocumentService : IDocumentService
{
    private readonly ChaosGenerator _chaosGenerator;
    private readonly ILogger<EmailService> _logger;
    private readonly RetryPolicy _retryPolicy;

    public DocumentService(ChaosGenerator chaosGenerator, ILogger<EmailService> logger, RetryPolicy retryPolicy)
    {
        _chaosGenerator = chaosGenerator;
        _logger = logger;
        _retryPolicy = retryPolicy;
    }

    public async Task SyncDocumentsFromExternalSource(string email)
    {
        _logger.LogInformation("Synchronizing documents for the e-mail '{email}'.", email);

        var retryPolicy = Policy.Handle<Exception>().WaitAndRetryAsync(_retryPolicy.Retries,
            attempt =>
            {
                _logger.LogError(
                    "Request to document synchronization service failed. Retry '{attempt}' of '{Retries}'.", attempt,
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
            _logger.LogError(e, "Document synchronization task failed.");
        }
    }
}

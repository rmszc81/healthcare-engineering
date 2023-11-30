namespace Healthcare.Engineering.DataObject.Settings;

public class RetryPolicy
{
    public int PauseBetweenRetries { get; init; }

    public int Retries { get; init; }
}
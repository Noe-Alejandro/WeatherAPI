namespace WeatherAPI.Application.Interfaces.Utils
{
    public interface IParallelRequestProcessor
    {
        Task<IEnumerable<TResult>> ProcessInParallel<TInput, TResult>(
            IEnumerable<TInput> items,
            Func<TInput, Task<TResult>> processFunc,
            int maxDegreeOfParallelism = 5);
    }
}

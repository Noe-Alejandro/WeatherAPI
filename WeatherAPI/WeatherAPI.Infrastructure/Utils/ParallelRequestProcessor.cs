using WeatherAPI.Application.Interfaces.Utils;

namespace WeatherAPI.Infrastructure.Utils
{
    public class ParallelRequestProcessor : IParallelRequestProcessor
    {
        public async Task<IEnumerable<TResult>> ProcessInParallel<TInput, TResult>(
            IEnumerable<TInput> items,
            Func<TInput, Task<TResult>> processFunc,
            int maxDegreeOfParallelism = 5)
        {
            var tasks = new List<Task<TResult>>();
            var semaphore = new SemaphoreSlim(maxDegreeOfParallelism);

            foreach (var item in items)
            {
                tasks.Add(Task.Run(async () =>
                {
                    await semaphore.WaitAsync();
                    try
                    {
                        return await processFunc(item);
                    }
                    finally
                    {
                        semaphore.Release();
                    }
                }));
            }

            var results = await Task.WhenAll(tasks);
            return results;
        }
    }
}

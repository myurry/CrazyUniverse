using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CrazyUniverse.Core.Models;

namespace CrazyUniverse.Core.Services
{
    public class SimulationReport
    {
        public string Mode { get; set; } = string.Empty;
        public List<AnimalSimulationResult> Results { get; set; } = new();
        public TimeSpan TotalDuration { get; set; }
    }

    public class SimulationService
    {
        private CancellationTokenSource? _cts;

        /// <summary>
        /// Runs computations sequentially: waits for one animal to finish before starting the next.
        /// </summary>
        public async Task<SimulationReport> RunSequentialAsync(
            IEnumerable<Animal> selectedAnimals,
            int workloadSize,
            CancellationToken externalToken)
        {
            _cts = CancellationTokenSource.CreateLinkedTokenSource(externalToken);
            var reportResults = new List<AnimalSimulationResult>();
            var totalStopwatch = Stopwatch.StartNew();

            foreach (var animal in selectedAnimals)
            {
                try
                {
                    // Check for error demonstration trigger
                    if (animal.Name.Contains("Error", StringComparison.OrdinalIgnoreCase))
                    {
                        throw new InvalidOperationException($"Simulated hardware failure for {animal.Name}!");
                    }

                    // Wrap in Task.Run to keep the UI thread responsive during CPU computation
                    var result = await Task.Run(() => animal.PerformHeavyAction(workloadSize, _cts.Token), _cts.Token);
                    reportResults.Add(result);
                }
                catch (OperationCanceledException)
                {
                    break;
                }
                catch (Exception ex)
                {
                    // Capture error into the report and continue processing remaining animals
                    reportResults.Add(new AnimalSimulationResult
                    {
                        ObjectName = animal.Name,
                        FinalState = animal.EnergyLevel.ToString(),
                        ResultMessage = "Failed",
                        Duration = TimeSpan.Zero,
                        IsSuccessful = false,
                        ErrorMessage = ex.Message
                    });
                }
            }

            totalStopwatch.Stop();

            return new SimulationReport
            {
                Mode = "Sequential",
                Results = reportResults,
                TotalDuration = totalStopwatch.Elapsed
            };
        }

        /// <summary>
        /// Runs computations in parallel: launches tasks with Task.Run and awaits all with Task.WhenAll.
        /// </summary>
        public async Task<SimulationReport> RunParallelAsync(
            IEnumerable<Animal> selectedAnimals,
            int workloadSize,
            CancellationToken externalToken)
        {
            _cts = CancellationTokenSource.CreateLinkedTokenSource(externalToken);
            var totalStopwatch = Stopwatch.StartNew();

            // Wrap each individual animal execution in a safe task wrapper to isolate errors
            var tasks = selectedAnimals.Select(async animal =>
            {
                try
                {
                    // Check for error demonstration trigger
                    if (animal.Name.Contains("Error", StringComparison.OrdinalIgnoreCase))
                    {
                        throw new InvalidOperationException($"Simulated hardware failure for {animal.Name}!");
                    }

                    return await Task.Run(() => animal.PerformHeavyAction(workloadSize, _cts.Token), _cts.Token);
                }
                catch (Exception ex) when (ex is not OperationCanceledException)
                {
                    // Return a failed simulation result object instead of crashing the parallel batch
                    return new AnimalSimulationResult
                    {
                        ObjectName = animal.Name,
                        FinalState = animal.EnergyLevel.ToString(),
                        ResultMessage = "Failed",
                        Duration = TimeSpan.Zero,
                        IsSuccessful = false,
                        ErrorMessage = ex.Message
                    };
                }
            }).ToArray();

            List<AnimalSimulationResult> reportResults = new();

            try
            {
                var results = await Task.WhenAll(tasks);
                // Filter out nulls if any task was cancelled or returned null
                reportResults.AddRange(results.Where(r => r != null));
            }
            catch (OperationCanceledException)
            {
                // Handled if cancellation token triggered globally
            }

            totalStopwatch.Stop();

            return new SimulationReport
            {
                Mode = "Parallel",
                Results = reportResults,
                TotalDuration = totalStopwatch.Elapsed
            };
        }

        /// <summary>
        /// Triggers cancellation across active computations.
        /// </summary>
        public void Cancel()
        {
            _cts?.Cancel();
        }
    }
}
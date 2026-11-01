Continue your Vol.2 project in the same repository. Choose at least three objects from your world.
For each object, perform a resource-intensive CPU computation: for example, a comparison of travel route variants, a development forecast, or a repetitive simulation.
Use a snapshot of the object's data. The user should be able to select the objects and the workload size.

Implement two modes: sequential and parallel. In sequential mode, wait for one object to finish before starting the next.
In parallel mode, run the object computations using Task.Run and wait for them with Task.WhenAll.
Use the same input in both modes. Compare the results and measure the total duration using a Stopwatch. The UI remains responsive during execution.

"Cancel" sends a shared CancellationToken to all computations. Every CPU cycle checks it.
Handle success, error, and cancellation for each object. An error in one object goes into the report, and the other jobs continue.
In the report: object name, final state, result or error message, duration, and total execution time.
Submit a Vol.3 PR and a brief instruction guide. Demonstrate both modes, an error, and cancellation.
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Threading;

namespace CrazyUniverse.Core.Models
{
    public class AnimalSimulationResult
    {
        public string ObjectName { get; set; } = string.Empty;
        public string FinalState { get; set; } = string.Empty;
        public string ResultMessage { get; set; } = string.Empty;
        public TimeSpan Duration { get; set; }
        public bool IsSuccessful { get; set; }
        public string? ErrorMessage { get; set; }
    }

    public abstract class Animal : INotifyPropertyChanged, IDisposable
    {
        private string _name = string.Empty;
        public string Name
        {
            get => _name;
            private set
            {
                if (_name != value)
                {
                    _name = value;
                    OnPropertyChanged(nameof(Name));
                }
            }
        }

        private IAnimalRepository? _currentRepository;
        private int _energyLevel;
        public int EnergyLevel
        {
            get => _energyLevel;
            protected set
            {
                int clamped = Math.Max(0, Math.Min(100, value));
                if (_energyLevel != clamped)
                {
                    _energyLevel = clamped;
                    OnPropertyChanged(nameof(EnergyLevel));
                }
            }
        }

        public string TypeName => GetType().Name;
        protected Action<string>? _logger;

        private bool _isSelectedForSimulation;
        public bool IsSelectedForSimulation
        {
            get => _isSelectedForSimulation;
            set
            {
                if (_isSelectedForSimulation != value)
                {
                    _isSelectedForSimulation = value;
                    OnPropertyChanged(nameof(IsSelectedForSimulation));
                }
            }
        }

        protected Animal(string name, int energyLevel)
        {
            Name = name;
            EnergyLevel = energyLevel;
        }

        public void ModifyEnergy(int amount)
        {
            EnergyLevel += amount;
        }

        public void BindToRepository(IAnimalRepository repository, Action<string> logger)
        {
            UnbindRepository();
            _currentRepository = repository;
            _logger = logger;

            _currentRepository.ItemAdded += OnRepositoryItemAdded;
            _currentRepository.ItemRemoved += OnRepositoryItemRemoved;
            _currentRepository.CrazyActionExecuted += OnRepositoryCrazyActionExecuted;
        }

        public void UnbindRepository()
        {
            if (_currentRepository != null)
            {
                _currentRepository.ItemAdded -= OnRepositoryItemAdded;
                _currentRepository.ItemRemoved -= OnRepositoryItemRemoved;
                _currentRepository.CrazyActionExecuted -= OnRepositoryCrazyActionExecuted;
                _currentRepository = null;
            }
        }

        private void OnRepositoryItemAdded(object? sender, Animal newcomer)
        {
            if (newcomer == this) return;
            _logger?.Invoke($"[{Name}] greets newcomer [{newcomer.Name}]!");
            ModifyEnergy(5);
        }

        private void OnRepositoryItemRemoved(object? sender, Animal departing)
        {
            if (departing == this) return;
            _logger?.Invoke($"[{Name}] says goodbye to departing [{departing.Name}].");
        }

        private void OnRepositoryCrazyActionExecuted(object? sender, Animal actor)
        {
            if (actor == this) return;
            _logger?.Invoke($"[{Name}] reacts to [{actor.Name}]'s CrazyAction!");
            ModifyEnergy(-10);
        }

        public abstract void PerformSpecialAction(IAnimalRepository repository, Action<string> logger);

        public abstract void MakeSound(Action<string> logger);

        /// <summary>
        /// Performs a resource-intensive CPU computation (simulation/forecast) using a data snapshot.
        /// Checks the CancellationToken on every cycle and returns a detailed execution result.
        /// </summary>
        public virtual AnimalSimulationResult PerformHeavyAction(int workloadSize, CancellationToken cancellationToken)
        {
            // 1. Capture a thread-safe data snapshot
            string snapshotName = Name;
            int snapshotEnergy = EnergyLevel;
            string snapshotType = TypeName;

            var stopwatch = Stopwatch.StartNew();
            double simulationScore = 0;

            try
            {
                // 2. Resource-intensive CPU loop
                for (int i = 0; i < workloadSize; i++)
                {
                    // Check cancellation on every CPU cycle
                    cancellationToken.ThrowIfCancellationRequested();

                    // Heavy math / repetitive simulation calculation
                    simulationScore += Math.Sin(i + snapshotEnergy) * Math.Cos(i * 0.1);
                }

                stopwatch.Stop();

                return new AnimalSimulationResult
                {
                    ObjectName = snapshotName,
                    FinalState = $"{snapshotType}: {snapshotName} (Energy: {snapshotEnergy}%)",
                    ResultMessage = $"Computation completed successfully. Score: {simulationScore:F4}",
                    Duration = stopwatch.Elapsed,
                    IsSuccessful = true
                };
            }
            catch (OperationCanceledException)
            {
                stopwatch.Stop();
                throw; // Rethrow cancellation so task runner handles it explicitly
            }
            catch (Exception ex)
            {
                stopwatch.Stop();

                // Return failed result object so other jobs can continue (as required by Vol.3 report specs)
                return new AnimalSimulationResult
                {
                    ObjectName = snapshotName,
                    FinalState = $"{snapshotType}: {snapshotName} (Energy: {snapshotEnergy}%)",
                    ResultMessage = "Computation failed.",
                    Duration = stopwatch.Elapsed,
                    IsSuccessful = false,
                    ErrorMessage = ex.Message
                };
            }
        }

        public void Dispose()
        {
            UnbindRepository();
            GC.SuppressFinalize(this);
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged(string propertyName) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
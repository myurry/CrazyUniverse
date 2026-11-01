using CrazyUniverse.Core;
using CrazyUniverse.Core.Models;
using CrazyUniverse.Core.Services;
using CrazyUniverse.WpfApp.Commands;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;

namespace CrazyUniverse.WpfApp.ViewModels
{
    public class MainViewModel : NotifyPropertyChangedBase
    {
        // Repositories for different animal types
        private readonly Repository<Horse> _horseRepository = new();
        private readonly Repository<Cow> _cowRepository = new();
        private readonly Repository<Monkey> _monkeyRepository = new();
        private readonly Repository<Bird> _birdRepository = new();

        // Simulation Service & State
        private readonly SimulationService _simulationService = new();
        private CancellationTokenSource? _cts;
        private int _selectedWorkloadSize = 300000000; // Default to Heavy (>5s)

        private IAnimalRepository _currentRepository;
        private Animal? _selectedAnimal;
        private string _logText = string.Empty;
        private string _searchTerm = string.Empty;
        private string _selectedAnimalType = "Horse";
        private string _newAnimalName = string.Empty;
        private int _minEnergyFilter = 0;
        private bool _isSortedDescending;
        private string _newAnimalType = "Horse";

        public string NewAnimalType
        {
            get => _newAnimalType;
            set
            {
                if (_newAnimalType != value)
                {
                    _newAnimalType = value;
                    OnPropertyChanged(nameof(NewAnimalType));
                }
            }
        }

        public int SelectedWorkloadSize
        {
            get => _selectedWorkloadSize;
            set
            {
                if (_selectedWorkloadSize != value)
                {
                    _selectedWorkloadSize = value;
                    OnPropertyChanged(nameof(SelectedWorkloadSize));
                }
            }
        }

        public MainViewModel()
        {
            // Default to Horse repository
            _currentRepository = _horseRepository;

            // Initialize Existing Commands
            AddCommand = new RelayCommand(_ => ExecuteAdd(), _ => !string.IsNullOrWhiteSpace(NewAnimalName));
            RemoveCommand = new RelayCommand(_ => ExecuteRemove(), _ => SelectedAnimal != null);
            CrazyActionCommand = new RelayCommand(async _ => await ExecuteCrazyActionAsync(), _ => SelectedAnimal != null);
            MakeSoundCommand = new RelayCommand(_ => ExecuteMakeSound(), _ => SelectedAnimal != null);
            TriggerCrazyActionCommand = new RelayCommand(_ => { });

            // Initialize Vol.3 Simulation Commands
            RunSequentialCommand = new RelayCommand(async _ => await ExecuteRunSequentialAsync());
            RunParallelCommand = new RelayCommand(async _ => await ExecuteRunParallelAsync());
            CancelSimulationCommand = new RelayCommand(_ => ExecuteCancelSimulation());

            // Seed sample data to test UI and LINQ immediately
            SeedData();
            UpdateDisplayedAnimals();
        }

        // --- Properties for UI Binding ---

        public ObservableCollection<Animal> DisplayedAnimals { get; } = new();

        public Animal? SelectedAnimal
        {
            get => _selectedAnimal;
            set
            {
                if (_selectedAnimal != value)
                {
                    _selectedAnimal = value;
                    OnPropertyChanged(nameof(SelectedAnimal));
                }
            }
        }

        public string LogText
        {
            get => _logText;
            set
            {
                if (_logText != value)
                {
                    _logText = value;
                    OnPropertyChanged(nameof(LogText));
                }
            }
        }

        public string SearchTerm
        {
            get => _searchTerm;
            set
            {
                if (_searchTerm != value)
                {
                    _searchTerm = value;
                    OnPropertyChanged(nameof(SearchTerm));
                    UpdateDisplayedAnimals();
                }
            }
        }

        public int MinEnergyFilter
        {
            get => _minEnergyFilter;
            set
            {
                if (_minEnergyFilter != value)
                {
                    _minEnergyFilter = value;
                    OnPropertyChanged(nameof(MinEnergyFilter));
                    UpdateDisplayedAnimals();
                }
            }
        }

        public bool IsSortedDescending
        {
            get => _isSortedDescending;
            set
            {
                if (_isSortedDescending != value)
                {
                    _isSortedDescending = value;
                    OnPropertyChanged(nameof(IsSortedDescending));
                    UpdateDisplayedAnimals();
                }
            }
        }

        public string NewAnimalName
        {
            get => _newAnimalName;
            set
            {
                if (_newAnimalName != value)
                {
                    _newAnimalName = value;
                    OnPropertyChanged(nameof(NewAnimalName));
                }
            }
        }

        public string SelectedAnimalType
        {
            get => _selectedAnimalType;
            set
            {
                if (_selectedAnimalType != value)
                {
                    _selectedAnimalType = value;
                    OnPropertyChanged(nameof(SelectedAnimalType));

                    _currentRepository = GetRepositoryForType(value);
                    UpdateDisplayedAnimals();
                }
            }
        }

        public IEnumerable<object> GroupSummary => SanctuaryQueryService.GetGroupSummary(_currentRepository.GetAllAnimals());

        // --- Commands ---

        public ICommand AddCommand { get; }
        public ICommand RemoveCommand { get; }
        public ICommand CrazyActionCommand { get; }
        public ICommand MakeSoundCommand { get; }
        public ICommand TriggerCrazyActionCommand { get; }

        // Vol.3 Simulation Commands
        public ICommand RunSequentialCommand { get; }
        public ICommand RunParallelCommand { get; }
        public ICommand CancelSimulationCommand { get; }

        // --- Helper methods ---

        private IAnimalRepository GetRepositoryForType(string animalType) => animalType switch
        {
            "Horse" => _horseRepository,
            "Cow" => _cowRepository,
            "Monkey" => _monkeyRepository,
            "Bird" => _birdRepository,
            _ => _horseRepository
        };

        // --- Action Handlers ---

        private void ExecuteAdd()
        {
            if (string.IsNullOrWhiteSpace(NewAnimalName)) return;

            Animal newAnimal = NewAnimalType switch
            {
                "Horse" => new Horse(NewAnimalName, 80),
                "Cow" => new Cow(NewAnimalName, 70),
                "Monkey" => new Monkey(NewAnimalName, 75),
                "Bird" => new Bird(NewAnimalName, 60),
                _ => new Horse(NewAnimalName, 50)
            };

            var targetRepo = GetRepositoryForType(NewAnimalType);

            newAnimal.BindToRepository(targetRepo, LogMessage);
            targetRepo.AddAnimal(newAnimal);

            NewAnimalName = string.Empty;
            UpdateDisplayedAnimals();
        }

        private void ExecuteRemove()
        {
            if (SelectedAnimal == null) return;

            _currentRepository.RemoveAnimal(SelectedAnimal);
            SelectedAnimal.UnbindRepository();
            SelectedAnimal = null;
            UpdateDisplayedAnimals();
        }

        public void RemoveSelectedAnimal()
        {
            ExecuteRemove();
        }

        private async System.Threading.Tasks.Task ExecuteCrazyActionAsync()
        {
            if (SelectedAnimal == null) return;

            LogMessage($"--- Action triggered by {SelectedAnimal.Name} ---");

            SelectedAnimal.PerformSpecialAction(_currentRepository, LogMessage);
            _currentRepository.TriggerCrazyActionForAnimal(SelectedAnimal);

            UpdateDisplayedAnimals();
            await System.Threading.Tasks.Task.CompletedTask;
        }

        private void ExecuteMakeSound()
        {
            if (SelectedAnimal == null) return;
            SelectedAnimal.MakeSound(LogMessage);
        }

        // --- Vol.3 Simulation Handlers ---

        private async Task ExecuteRunSequentialAsync()
        {
            var selectedAnimals = _currentRepository.GetAllAnimals().Where(a => a.IsSelectedForSimulation).ToList();
            if (selectedAnimals.Count < 3)
            {
                LogMessage("Simulation Notice: Please check at least 3 animals in the list to run the simulation.");
                return;
            }

            _cts = new CancellationTokenSource();
            LogMessage($"=== Starting Sequential Simulation ({selectedAnimals.Count} animals, Workload: {SelectedWorkloadSize:N0}) ===");

            try
            {
                var report = await _simulationService.RunSequentialAsync(selectedAnimals, SelectedWorkloadSize, _cts.Token);
                FormatAndLogReport(report);
            }
            catch (Exception ex)
            {
                LogMessage($"Sequential Simulation Error: {ex.Message}");
            }
        }

        private async Task ExecuteRunParallelAsync()
        {
            var selectedAnimals = _currentRepository.GetAllAnimals().Where(a => a.IsSelectedForSimulation).ToList();
            if (selectedAnimals.Count < 3)
            {
                LogMessage("Simulation Notice: Please check at least 3 animals in the list to run the simulation.");
                return;
            }

            _cts = new CancellationTokenSource();
            LogMessage($"=== Starting Parallel Simulation ({selectedAnimals.Count} animals, Workload: {SelectedWorkloadSize:N0}) ===");

            try
            {
                var report = await _simulationService.RunParallelAsync(selectedAnimals, SelectedWorkloadSize, _cts.Token);
                FormatAndLogReport(report);
            }
            catch (Exception ex)
            {
                LogMessage($"Parallel Simulation Error: {ex.Message}");
            }
        }

        private void ExecuteCancelSimulation()
        {
            _simulationService.Cancel();
            _cts?.Cancel();
            LogMessage("--- Simulation cancellation requested ---");
        }

        private void FormatAndLogReport(SimulationReport report)
        {
            var sb = new StringBuilder();
            sb.AppendLine($"[Report - {report.Mode} Mode]");
            sb.AppendLine($"Total Execution Time: {report.TotalDuration.TotalMilliseconds:N2} ms");
            foreach (var res in report.Results)
            {
                string outcome = res.IsSuccessful ? res.ResultMessage : (res.ErrorMessage ?? "Unknown Error");
                sb.AppendLine($"  • {res.ObjectName} | State: {res.FinalState} | Result: {outcome} | Duration: {res.Duration.TotalMilliseconds:N2} ms");
            }
            LogMessage(sb.ToString().TrimEnd());
        }

        // --- LINQ Integration & View Refresh ---

        private void UpdateDisplayedAnimals()
        {
            DisplayedAnimals.Clear();

            var all = _currentRepository.GetAllAnimals();
            var filtered = SanctuaryQueryService.FilterByNameOrEnergy(all, SearchTerm, MinEnergyFilter);
            var sorted = SanctuaryQueryService.SortAnimals(filtered, IsSortedDescending);

            foreach (var item in sorted)
            {
                DisplayedAnimals.Add(item);
            }

            OnPropertyChanged(nameof(GroupSummary));
        }

        private void LogMessage(string message)
        {
            LogText = $"{DateTime.Now:HH:mm:ss} {message}\n" + LogText;
        }

        private void SeedData()
        {
            // Horses (5)
            var horses = new[]
            {
                new Horse("Spirit", 90),
                new Horse("Thunder", 85),
                new Horse("Blaze", 75),
                new Horse("Shadow", 80),
                new Horse("Storm", 95)
            };
            foreach (var h in horses)
            {
                h.BindToRepository(_horseRepository, LogMessage);
                _horseRepository.AddAnimal(h);
            }

            // Cows (5)
            var cows = new[]
            {
                new Cow("Bessie", 70),
                new Cow("Daisy", 65),
                new Cow("Clarabelle", 80),
                new Cow("Buttercup", 75),
                new Cow("Moo", 60)
            };
            foreach (var c in cows)
            {
                c.BindToRepository(_cowRepository, LogMessage);
                _cowRepository.AddAnimal(c);
            }

            // Monkeys (5)
            var monkeys = new[]
            {
                new Monkey("George", 85),
                new Monkey("Abu", 90),
                new Monkey("Coco", 75),
                new Monkey("Banana", 80),
                new Monkey("Jojo", 88)
            };
            foreach (var m in monkeys)
            {
                m.BindToRepository(_monkeyRepository, LogMessage);
                _monkeyRepository.AddAnimal(m);
            }

            // Birds (5)
            var birds = new[]
            {
                new Bird("Sky", 60),
                new Bird("Feather", 70),
                new Bird("Tweety", 65),
                new Bird("Blue", 80),
                new Bird("Chirp", 75)
            };
            foreach (var b in birds)
            {
                b.BindToRepository(_birdRepository, LogMessage);
                _birdRepository.AddAnimal(b);
            }
        }
    }

    public abstract class NotifyPropertyChangedBase : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged(string propertyName) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
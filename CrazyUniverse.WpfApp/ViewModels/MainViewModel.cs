using CrazyUniverse.Core;
using CrazyUniverse.Core.Models;
using CrazyUniverse.Core.Services;
using CrazyUniverse.WpfApp.Commands;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
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

        public MainViewModel()
        {
            // Default to Horse repository
            _currentRepository = _horseRepository;

            // Initialize Commands
            AddCommand = new RelayCommand(_ => ExecuteAdd(), _ => !string.IsNullOrWhiteSpace(NewAnimalName));
            RemoveCommand = new RelayCommand(_ => ExecuteRemove(), _ => SelectedAnimal != null);
            CrazyActionCommand = new RelayCommand(async _ => await ExecuteCrazyActionAsync(), _ => SelectedAnimal != null);

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
        public ICommand TriggerCrazyActionCommand { get; } = new RelayCommand(_ => { });

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

            // Используем NewAnimalType для определения типа создаваемого объекта
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
            LogText = $"{DateTime.Now:HH:mm:ss} - {message}\n" + LogText;
        }

        private void SeedData()
        {
            var h1 = new Horse("Spirit", 90);
            var h2 = new Horse("Thunder", 85);
            h1.BindToRepository(_horseRepository, LogMessage);
            h2.BindToRepository(_horseRepository, LogMessage);
            _horseRepository.AddAnimal(h1);
            _horseRepository.AddAnimal(h2);
        }
    }

    public abstract class NotifyPropertyChangedBase : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged(string propertyName) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
using System;
using System.ComponentModel;

namespace CrazyUniverse.Core.Models
{
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
        private Action<string>? _logger;

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

        // Polymorphic special action replaces manager if/else statements
        public abstract void PerformSpecialAction(IAnimalRepository repository, Action<string> logger);

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
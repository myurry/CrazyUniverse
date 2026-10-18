using System;
using System.ComponentModel;

namespace CrazyUniverse.Core.Models
{
    public abstract class Animal : INotifyPropertyChanged, IDisposable
    {
        private string _name;
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

        private int _energyLevel;
        public int EnergyLevel
        {
            get => _energyLevel;
            set
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

        private Repository<Animal>? _currentRepository;
        private Action<string>? _logger;

        protected Animal(string name, int energyLevel)
        {
            Name = name;
            EnergyLevel = energyLevel;
        }

        // Attach to repository events when added
        public void BindToRepository(Repository<Animal> repository, Action<string> logger)
        {
            UnbindRepository(); // Clean up previous bindings if any

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
            // Do not react to self
            if (newcomer == this) return;

            _logger?.Invoke($"[{Name}] greets newcomer [{newcomer.Name}]!");
            // State change reaction
            EnergyLevel = Math.Min(100, EnergyLevel + 5);
        }

        private void OnRepositoryItemRemoved(object? sender, Animal departing)
        {
            // Do not react to self
            if (departing == this) return;

            _logger?.Invoke($"[{Name}] says goodbye to departing [{departing.Name}].");
        }

        private void OnRepositoryCrazyActionExecuted(object? sender, Animal actor)
        {
            // Do not react to self
            if (actor == this) return;

            _logger?.Invoke($"[{Name}] reacts to [{actor.Name}]'s CrazyAction!");
            // State change reaction
            EnergyLevel = Math.Max(0, EnergyLevel - 10);
        }

        public abstract void PerformSpecialAction(Repository<Animal> repository, Action<string> logger);

        public void Dispose()
        {
            Unbinding();
            GC.SuppressFinalize(this);
        }

        protected virtual void Unbinding() => UnbindRepository();

        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged(string propertyName) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
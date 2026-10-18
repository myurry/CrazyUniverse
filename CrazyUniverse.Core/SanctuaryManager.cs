using System;
using System.Threading.Tasks;
using CrazyUniverse.Core.Models;

namespace CrazyUniverse.Core
{
    public class SanctuaryManager
    {
        private readonly IAnimalRepository _repository;
        private readonly Action<string> _logger;

        public SanctuaryManager(IAnimalRepository repository, Action<string> logger)
        {
            _repository = repository;
            _logger = logger;
        }

        public async Task ExecuteCrazyActionAsync(Animal selectedAnimal)
        {
            if (selectedAnimal == null) return;

            // Clean polymorphic call — no type checking needed!
            if (_repository is IAnimalRepository concreteRepo)
            {
                selectedAnimal.PerformSpecialAction(concreteRepo, _logger);
                concreteRepo.TriggerCrazyActionForAnimal(selectedAnimal);
            }

            await Task.CompletedTask;
        }
    }
}
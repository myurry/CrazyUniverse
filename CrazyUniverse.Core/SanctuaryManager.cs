using System;
using System.Threading.Tasks;
using CrazyUniverse.Core.Models;

namespace CrazyUniverse.Core
{
    public class SanctuaryManager
    {
        private readonly IRepository<Animal> _repository;
        private readonly Action<string> _logger;

        public SanctuaryManager(IRepository<Animal> repository, Action<string> logger)
        {
            _repository = repository;
            _logger = logger;
        }

        public async Task ExecuteCrazyActionAsync(Animal selectedAnimal)
        {
            if (selectedAnimal == null) return;

            // Polymorphic call — no type checking (if/else) needed here!
            selectedAnimal.PerformSpecialAction(_repository, _logger);

            await Task.CompletedTask;
        }
    }
}
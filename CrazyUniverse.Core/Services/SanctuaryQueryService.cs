using System;
using System.Collections.Generic;
using System.Linq;
using CrazyUniverse.Core.Models;

namespace CrazyUniverse.Core.Services
{
    public static class SanctuaryQueryService
    {
        /// <summary>
        /// 1. Search / Filtering (Where)
        /// Filters characters by name match and minimum energy level.
        /// </summary>
        public static IEnumerable<Animal> FilterAnimals(IEnumerable<Animal> animals, string searchTerm, int minEnergy)
        {
            if (animals == null) return Enumerable.Empty<Animal>();

            return animals.Where(a =>
                (string.IsNullOrWhiteSpace(searchTerm) || a.Name.Contains(searchTerm, StringComparison.OrdinalIgnoreCase)) &&
                a.EnergyLevel >= minEnergy);
        }

        /// <summary>
        /// 2. Sorting (OrderBy / OrderByDescending)
        /// Sorts characters by energy level and secondary sort by name alphabetically.
        /// </summary>
        public static IEnumerable<Animal> SortAnimals(IEnumerable<Animal> animals, bool descending)
        {
            if (animals == null) return Enumerable.Empty<Animal>();

            return descending
                ? animals.OrderByDescending(a => a.EnergyLevel).ThenBy(a => a.Name)
                : animals.OrderBy(a => a.EnergyLevel).ThenBy(a => a.Name);
        }

        /// <summary>
        /// 3. Grouping / Summary (GroupBy)
        /// Groups characters by their specific type and calculates aggregate metrics.
        /// </summary>
        public static IEnumerable<CharacterGroupSummary> GetGroupSummary(IEnumerable<Animal> animals)
        {
            if (animals == null) return Enumerable.Empty<CharacterGroupSummary>();

            return animals
                .GroupBy(a => a.TypeName)
                .Select(group => new CharacterGroupSummary
                {
                    CharacterType = group.Key,
                    Count = group.Count(),
                    AverageEnergy = group.Any() ? group.Average(a => a.EnergyLevel) : 0
                })
                .ToList();
        }
    }

    /// <summary>
    /// Clean data model holding aggregated group statistics for the UI summary view.
    /// </summary>
    public class CharacterGroupSummary
    {
        public string CharacterType { get; set; } = string.Empty;
        public int Count { get; set; }
        public double AverageEnergy { get; set; }
    }
}
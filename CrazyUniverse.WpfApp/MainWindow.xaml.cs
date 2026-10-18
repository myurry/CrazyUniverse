using System.Windows;
using CrazyUniverse.Core.Models;
using CrazyUniverse.WpfApp.ViewModels;

namespace CrazyUniverse.WpfApp
{
    public partial class MainWindow : Window
    {
        private MainViewModel ViewModel => DataContext as MainViewModel;

        public MainWindow()
        {
            InitializeComponent();
        }

        private async void OnCrazyActionClick(object sender, RoutedEventArgs e)
        {
            if (ViewModel?.SelectedAnimal == null) return;

            // Remember currently selected item to prevent selection loss
            var previouslySelected = ViewModel.SelectedAnimal;

            await ViewModel.TriggerCrazyActionCommand.ExecuteAsync(null);

            // Restore selection cleanly without code-behind string hacks (`Contains(...)`)
            if (ViewModel.Animals.Contains(previouslySelected))
            {
                ViewModel.SelectedAnimal = previouslySelected;
            }
        }

        private void OnRemoveClick(object sender, RoutedEventArgs e)
        {
            if (ViewModel?.SelectedAnimal != null)
            {
                ViewModel.RemoveSelectedAnimal();
            }
        }
    }
}
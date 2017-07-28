using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using SharpMusicLibraryUpdater.App.ViewModels;
using SharpMusicLibraryUpdater.App.Services;
using iTunesSearch.Library;
using System.Windows.Interactivity;
using System.IO;

namespace SharpMusicLibraryUpdater.App
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private static readonly string settingsFilename = "settings.bin";
        private static readonly string settingsFileFullPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, settingsFilename);

        public MainWindow()
        {
            InitializeComponent();
            this.DataContext = new ArtistViewModel(new MusicLibraryReader(), new iTunesSearchManager(),
                new SettingsSerializer(settingsFileFullPath)
                );
        }

        private void CommandBindingCloseWindow_CanExecute(object sender, CanExecuteRoutedEventArgs e) => e.CanExecute = true;

        private void CommandBindingCloseWindow_Executed(object sender, ExecutedRoutedEventArgs e) => SystemCommands.CloseWindow(this);
    }
}

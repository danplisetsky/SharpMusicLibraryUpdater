using System;
using System.Collections.ObjectModel;
using iTunesSearch.Library;
using System.Threading.Tasks;
using System.Linq;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using SharpMusicLibraryUpdater.App.Models;
using SharpMusicLibraryUpdater.App.Services;
using SharpMusicLibraryUpdater.App.Commands;
using Microsoft.WindowsAPICodePack.Dialogs;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.IO;
using System.Collections.Concurrent;
using System.Windows.Data;
using System.Windows.Controls;
using NodaTime;
using System.Windows.Media;

namespace SharpMusicLibraryUpdater.App.ViewModels
{
    public class ArtistViewModel : INotifyPropertyChanged
    {
        private readonly MusicLibraryReader musicLibraryReader;
        private readonly iTunesSearchManager searchManager;
        private string currentDirectory; // TODO: probably remove it, go back to only having it in the local OpenMusicFolder scope

        private ObservableCollection<Artist> Artists = new ObservableCollection<Artist>();
        public ListCollectionView ModelCollectionView { get; private set; }

        private DataGrid dataGridAlbums;

        private bool _isNotBusy = true;
        public bool IsNotBusy
        {
            get => _isNotBusy;
            set
            {
                if (_isNotBusy != value)
                {
                    _isNotBusy = value;
                    OnPropertyChanged();
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged([CallerMemberName] string prop = "") => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));

        public ICommand OpenMusicLibraryCommand { get; }
        public ICommand GetAlbumsCommand { get; }
        public ICommand ShowAlbumsCommand { get; }

        public ArtistViewModel(MusicLibraryReader musicLibraryReader, iTunesSearchManager iTunesSearchManager, DataGrid dataGrid_Albums)
        {
            this.searchManager = iTunesSearchManager;
            this.musicLibraryReader = musicLibraryReader;
            this.OpenMusicLibraryCommand = new DelegateCommand(this.OpenMusicLibrary, this.CanOpenMusicLibrary);
            this.GetAlbumsCommand = new DelegateCommand(this.GetAlbumsFromITunes, this.CanGetAlbums);
            this.ShowAlbumsCommand = new DelegateCommand(this.ShowAlbums, this.CanGetAlbums);

            this.ModelCollectionView = new ListCollectionView(Artists);

            this.dataGridAlbums = dataGrid_Albums;
        }

        private void ShowAlbums(object param) => dataGridAlbums.DataContext = param as Artist;

        private async void GetAlbumsFromITunes(object param)
        {
            this.IsNotBusy = false;
            await Task.Run(() => Parallel.ForEach(Artists, async artist =>
            {
                artist.NewAlbums = await GetNewAlbums(artist);
                artist.Color = artist.NewAlbums.Any() ? Brushes.Green : Brushes.Red;
            }));

            this.IsNotBusy = true;

            string ReadyToCompare(string input) => input.Replace("&", "and").Replace(" ", "").ToUpperInvariant();

            async Task<List<NewAlbum>> GetNewAlbums(Artist artist)
            {
                var iTunesAlbumsSearchResult = (await searchManager.GetAlbumsByArtistIdAsync(artist.ArtistId)).Albums
                    .Where(al => !(String.IsNullOrWhiteSpace(al.CollectionName) || String.IsNullOrEmpty(al.CollectionName)));
                var localAlbumsReadyToCompare = artist.LocalAlbums.Select(al => ReadyToCompare(al.AlbumName)).ToList();
                // TODO: Implement checks for singles, then add distinct back
                var iTunesAlbumsReadyToCompare = iTunesAlbumsSearchResult
                    .Select(al => new { Id = al.CollectionId, Name = ReadyToCompare(GetFormattedAlbumName(al.CollectionName, isLocalFolder: false)) })
                    .ToDictionary(x => x.Id, x => x.Name);
                var newAlbums = iTunesAlbumsReadyToCompare.Select(kvp => kvp.Value)
                    .Except(localAlbumsReadyToCompare).ToList();

                var newAlbumsIds = iTunesAlbumsReadyToCompare
                      .Where(kvp => newAlbums.Contains(kvp.Value))
                      .Select(kvp => kvp.Key);

                return iTunesAlbumsSearchResult.Where(al => newAlbumsIds.Contains(al.CollectionId))
                    .Select(al => new NewAlbum(al.CollectionName, LocalDate.FromDateTime(DateTime.Parse(al.ReleaseDate))))
                    .OrderBy(na => na.ReleaseDate).ToList(); 
            }
        }

        bool CanGetAlbums(object param) => this.Artists.Any();

        private string SelectFolder()
        {
            var dialog = new CommonOpenFileDialog("Open Music Library Folder...")
            {
                IsFolderPicker = true,
                Multiselect = false,
                ShowPlacesList = true,
                InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile)
            };

            var result = dialog.ShowDialog();
            if (result == CommonFileDialogResult.Ok)
            {
                return dialog.FileName;
            }
            return String.Empty;
        }

        private bool CanOpenMusicLibrary(object param) => true;

        private async void OpenMusicLibrary(object param)
        {
            currentDirectory = this.SelectFolder();
            if (String.IsNullOrEmpty(currentDirectory))
            {
                return;
            }

            this.IsNotBusy = false;

            this.Artists.Clear();
            var artistsFullLocalPath = musicLibraryReader.ReadMusicLibrary(currentDirectory);
            foreach (string artistFolder in artistsFullLocalPath)
            {
                string artistName = new DirectoryInfo(artistFolder).Name.Trim();
                var iTunesArtistInfo = (await searchManager.GetSongArtistsAsync(artistName)).Artists.FirstOrDefault();
                if (iTunesArtistInfo != null)
                {
                    this.Artists.Add(new Artist { Name = iTunesArtistInfo.ArtistName, ArtistId = iTunesArtistInfo.ArtistId, LocalAlbums = GetLocalAlbums(artistFolder) });
                }
            }

            this.IsNotBusy = true;
        }

        private List<Album> GetLocalAlbums(string artistFolder)
        {
            var albums = new List<Album>();
            var dirs = new Stack<string>();
            dirs.Push(artistFolder);

            while (dirs.Any())
            {
                string currentDir = dirs.Pop();

                if (currentDir != artistFolder)
                {
                    string formattedAlbumName = GetFormattedAlbumName(new DirectoryInfo(currentDir).Name, isLocalFolder: true);
                    if (!String.IsNullOrEmpty(formattedAlbumName))
                    {
                        albums.Add(new Album(formattedAlbumName));
                    }
                }

                string[] subDirs;
                subDirs = Directory.GetDirectories(currentDir);

                foreach (string str in subDirs)
                {
                    dirs.Push(str);
                }
            }

            return albums;
        }

        private string GetFormattedAlbumName(string fullAlbumName, bool isLocalFolder)
        {
            string result = String.Empty;
            if (String.IsNullOrEmpty(fullAlbumName))
            {
                return result;
            }
            var year = GetAlbumYear(fullAlbumName);
            if (isLocalFolder && !year.Success)
            {
                return result;
            }
            result = year.Success ? FormatAlbumName(fullAlbumName, year) : FormatAlbumName(fullAlbumName);

            return result;

            Match GetAlbumYear(string inputFolder)
            {
                string regexPattern = @"\d{4}";
                return Regex.Match(inputFolder, regexPattern);
            }

            string FormatAlbumName(string _fullAlbumName, Match _year = default(Match))
            {
                string nameSansYear = _year == default(Match) ? _fullAlbumName : _fullAlbumName.Remove(_year.Index, _year.Length);
                string regexPattern = @"^[\W]+";
                var match = Regex.Match(nameSansYear, regexPattern);
                string nameSansLeadingNonAlphanumericCharacters = nameSansYear.Remove(match.Index, match.Length);
                string finalName = new string(nameSansLeadingNonAlphanumericCharacters.TakeWhile(c => char.IsLetterOrDigit(c) || char.IsWhiteSpace(c) || c == '&')
                    .ToArray()).Trim();
                return finalName;
            }
        }

    }
}

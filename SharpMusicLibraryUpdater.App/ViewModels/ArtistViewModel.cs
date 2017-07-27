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
using System.Diagnostics;

namespace SharpMusicLibraryUpdater.App.ViewModels
{
    public class ArtistViewModel : INotifyPropertyChanged
    {
        #region Fields

        private readonly MusicLibraryReader musicLibraryReader;
        private readonly iTunesSearchManager searchManager;
        private readonly SettingsSerializer settingsSerializer;
        private readonly Settings settings;

        #endregion Fields

        public ObservableCollection<Artist> Artists { get; set; } = new ObservableCollection<Artist>();

        private Artist _currentlySelectedArtist;

        public Artist CurrentlySelectedArtist
        {
            get => _currentlySelectedArtist;
            set
            {
                if (_currentlySelectedArtist != value)
                {
                    _currentlySelectedArtist = value;
                    OnPropertyChanged();
                }
            }
        }

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
        private void OnPropertyChanged([CallerMemberName] string prop = "") =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));

        public ICommand OpenMusicLibraryCommand { get; }
        public ICommand GetAlbumsCommand { get; }
        public ICommand ShowAlbumsCommand { get; }
        public ICommand OnClosingCommand { get; }

        public ArtistViewModel(MusicLibraryReader musicLibraryReader, iTunesSearchManager iTunesSearchManager, SettingsSerializer settingsSerializer)
        {
            this.musicLibraryReader = musicLibraryReader;
            this.searchManager = iTunesSearchManager;
            this.settingsSerializer = settingsSerializer;
            this.settings = settingsSerializer.LoadSettings();

            this.OpenMusicLibraryCommand = new DelegateCommand(this.OpenMusicLibrary, this.CommandCanAlwaysExecute);
            this.GetAlbumsCommand = new DelegateCommand(this.GetAlbums, this.CanGetAlbums);
            this.ShowAlbumsCommand = new DelegateCommand(this.ShowAlbums, this.CanGetAlbums);
            this.OnClosingCommand = new DelegateCommand(this.OnClosing, this.CommandCanAlwaysExecute);

            this.ReadSettings();
        }

        private void ReadSettings()
        {
            if (!String.IsNullOrEmpty(settings.MusicLibraryFolder))
            {
                this.ReadMusicLibraryAsync(isLoadingFromSettings: true).Wait();
            }
        }

        private void OnClosing(object param)
        {
            settings.Artists = this.Artists.ToList();
            settingsSerializer.SaveSettings(settings);
        }

        private void ShowAlbums(object param) => this.CurrentlySelectedArtist = param as Artist;

        private async void GetAlbums(object param)
        {
            this.IsNotBusy = false;

            await Task.Run(() => Parallel.ForEach(this.Artists.Where(ar => !ar.IsIgnored), async artist =>
            {
                artist.LocalAlbums = GetLocalAlbums(artist.LocalPath);
                var newAlbumsFromiTunes = await GetAlbumsFromiTunesAsync(artist);
                artist.NewAlbums = AddOnlyNewAlbums(newAlbumsFromiTunes, artist.NewAlbums);
            }));

            this.IsNotBusy = true;

            List<NewAlbum> AddOnlyNewAlbums(List<NewAlbum> newAlbumsFromiTunes, List<NewAlbum> newAlbums)
            {
                if (newAlbums == null)
                    return newAlbumsFromiTunes;

                var result = new List<NewAlbum>();
                var albumsIds = newAlbums.Select(al => al.AlbumId);
                foreach (var album in newAlbumsFromiTunes)
                {
                    if (!albumsIds.Contains(album.AlbumId))
                        result.Add(album);
                }
                return result.Union(newAlbums).ToList();
            }
        }

        private async Task<List<NewAlbum>> GetAlbumsFromiTunesAsync(Artist artist)
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
                .Select(al => new NewAlbum(al.CollectionName, al.CollectionId, LocalDate.FromDateTime(DateTime.Parse(al.ReleaseDate))))
                .OrderBy(na => na.ReleaseDate).ToList();


            string ReadyToCompare(string input) => input.Replace("&", "and").Replace(" ", "").ToUpperInvariant();
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

        private bool CommandCanAlwaysExecute(object param) => true;

        private async void OpenMusicLibrary(object param)
        {
            settings.MusicLibraryFolder = this.SelectFolder();
            if (String.IsNullOrEmpty(settings.MusicLibraryFolder))
            {
                return;
            }
            await ReadMusicLibraryAsync(isLoadingFromSettings: false);
        }

        private async Task ReadMusicLibraryAsync(bool isLoadingFromSettings)
        {
            this.IsNotBusy = false;

            this.Artists.Clear();
            var artistsFullLocalPath = musicLibraryReader.ReadMusicLibrary(settings.MusicLibraryFolder);
            foreach (string artistFolder in artistsFullLocalPath)
            {
                string artistName = new DirectoryInfo(artistFolder).Name.Trim();
                if (isLoadingFromSettings)
                {
                    var artist = settings.Artists.Find(ar => ar.Name == artistName);
                    if (artist != null)
                    {
                        this.Artists.Add(new Artist
                        {
                            Name = artist.Name,
                            ITunesName = artist.ITunesName,
                            ArtistId = artist.ArtistId,
                            IsIgnored = artist.IsIgnored,
                            LocalPath = artist.LocalPath,
                            NewAlbums = artist.NewAlbums
                        });
                    }
                    else
                    {
                        await AddArtist(artistName, artistFolder);
                    }
                }
                else
                {
                    await AddArtist(artistName, artistFolder);
                }
            }

            this.GetAlbumsCommand.CanExecute(null);
            this.IsNotBusy = true;

            async Task AddArtist(string artistName, string artistFolder)
            {
                var iTunesArtistInfo = (await searchManager.GetSongArtistsAsync(artistName)).Artists.FirstOrDefault();
                if (iTunesArtistInfo != null)
                {
                    this.Artists.Add(new Artist
                    {
                        Name = artistName,
                        ITunesName = iTunesArtistInfo.ArtistName,
                        ArtistId = iTunesArtistInfo.ArtistId,
                        LocalPath = artistFolder
                    });
                }
            }
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
                string finalName = new string(nameSansLeadingNonAlphanumericCharacters.TakeWhile(c => IsAcceptableChar(c))
                    .ToArray()).Trim();
                return finalName;

                bool IsAcceptableChar(char c) => char.IsLetterOrDigit(c) || char.IsWhiteSpace(c) || c == '&' || c == ',';
            }
        }

    }
}

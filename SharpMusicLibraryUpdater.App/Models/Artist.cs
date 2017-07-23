using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace SharpMusicLibraryUpdater.App.Models
{
    public class Artist : INotifyPropertyChanged
    {
        private long _artistId;

        public long ArtistId
        {
            get => _artistId;
            set
            {
                if (_artistId != value)
                {
                    _artistId = value;
                    OnPropertyChanged();
                }
            }
        }

        private string _name;

        public string Name
        {
            get => _name;
            set
            {
                if (_name != value)
                {
                    _name = value;
                    OnPropertyChanged();
                }
            }
        }

        private bool _isIgnored;

        public bool IsIgnored
        {
            get => _isIgnored;
            set
            {
                if (_isIgnored != value)
                {
                    _isIgnored = value;
                    OnPropertyChanged();
                }
            }
        }

        private bool _checkForSingles;

        public bool CheckForSingles
        {
            get => _checkForSingles;
            set
            {
                if (_checkForSingles != value)
                {
                    _checkForSingles = value;
                    OnPropertyChanged();
                }
            }
        }

        private List<Album> _localAlbums;

        public List<Album> LocalAlbums
        {
            get => _localAlbums;
            set
            {
                if (_localAlbums != value)
                {
                    _localAlbums = value;
                    OnPropertyChanged();
                }
            }
        }

        private List<NewAlbum> _newAlbums;

        public List<NewAlbum> NewAlbums
        {
            get => _newAlbums;
            set
            {
                if (_newAlbums != value)
                {
                    _newAlbums = value;
                    OnPropertyChanged();
                }
            }
        }



        private void OnPropertyChanged([CallerMemberName] string prop = "") => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));

        public event PropertyChangedEventHandler PropertyChanged;
    }
}

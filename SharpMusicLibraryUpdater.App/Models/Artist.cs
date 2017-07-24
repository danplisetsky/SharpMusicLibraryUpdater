using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;
using System.Windows.Media;

namespace SharpMusicLibraryUpdater.App.Models
{
    [DataContract]
    public class Artist : INotifyPropertyChanged
    {
        private long _artistId;

        [DataMember]
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

        [DataMember]
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

        [DataMember]
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

        [DataMember]
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

        [IgnoreDataMember]
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

        [IgnoreDataMember]
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

        private Brush _color;

        [IgnoreDataMember]
        public Brush Color
        {
            get => _color;
            set
            {
                if (_color != value)
                {
                    _color = value;
                    OnPropertyChanged();
                }
            }
        }

        private void OnPropertyChanged([CallerMemberName] string prop = "") => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));

        public event PropertyChangedEventHandler PropertyChanged;
    }
}

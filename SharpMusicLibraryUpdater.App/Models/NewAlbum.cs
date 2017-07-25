using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using NodaTime;

namespace SharpMusicLibraryUpdater.App.Models
{
    public class NewAlbum : Album,  INotifyPropertyChanged
    {
        public long AlbumId { get; }
        public LocalDate ReleaseDate { get; }

        private bool _markAsSeen;
        public bool MarkAsSeen
        {
            get => _markAsSeen;
            set
            {
                if (_markAsSeen != value)
                {
                    _markAsSeen = value;
                    OnPropertyChanged();
                }
            }
        }

        public NewAlbum(string name, long albumId, LocalDate releaseDate, bool markAsSeen = false) : base(name)
        {
            this.AlbumId = albumId;
            this.ReleaseDate = releaseDate;
            this.MarkAsSeen = markAsSeen;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged([CallerMemberName] string prop = "") => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
    }
}

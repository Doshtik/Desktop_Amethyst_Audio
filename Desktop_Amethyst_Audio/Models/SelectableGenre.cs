using System.ComponentModel;
using System.Runtime.CompilerServices;
using Desktop_Amethyst_Audio.Models.DTO;

namespace Desktop_Amethyst_Audio.Models;

public class SelectableGenre 
{
    private bool _isSelected;
    public GenreInfoDto Genre { get; }

    public short Id => Genre.Id;
    public string Name => Genre.GenreName;

    public bool IsSelected
    {
        get => _isSelected;
        set
        {
            if (_isSelected != value)
            {
                _isSelected = value;
                OnPropertyChanged();
            }
        }
    }
    public SelectableGenre(GenreInfoDto genre)
    {
        Genre = genre;
    }
    
    public event PropertyChangedEventHandler PropertyChanged;
    protected void OnPropertyChanged([CallerMemberName] string propertyName = null) =>
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
}
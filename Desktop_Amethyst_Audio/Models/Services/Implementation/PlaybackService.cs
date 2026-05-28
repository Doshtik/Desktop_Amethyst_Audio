using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;
using CommunityToolkit.Mvvm.Messaging;
using Desktop_Amethyst_Audio.Messages.Action;
using Desktop_Amethyst_Audio.Models.DTO.Tracks;

namespace Desktop_Amethyst_Audio.Models.Services.Implementation;

public class PlaybackService
{
    
    public static ObservableCollection<TrackInfoDto> Queue { get; } = new();
    
    private static TrackInfoDto? _currentTrack;
    public static TrackInfoDto? CurrentTrack 
    { 
        get => _currentTrack;
        set
        {
            if (value == null || value == _currentTrack) 
                return;
            
            if (Queue.Contains(value))
            {
                int queueIndex = Queue.IndexOf(value);
                _playbackIndex = _playbackOrder.IndexOf(queueIndex);
                _currentTrack = value;
            }
            else
            {
                Queue.Add(value);
                _currentTrack = value;
                RebuildPlaybackOrder();
                int queueIndex = Queue.IndexOf(value);
                _playbackIndex = _playbackOrder.IndexOf(queueIndex);
            }
        }
    }

    public static bool IsShuffled { get; private set; } = false;

    private static List<int> _playbackOrder = new();
    
    private static int _playbackIndex = -1;

    public static void RemoveTrack(TrackInfoDto track)
    {
        if (track is null || !Queue.Contains(track))
            return;
        
        bool wasCurrent = Equals(CurrentTrack, track);
        
        Queue.Remove(track);
        RebuildPlaybackOrder();
        
        if (wasCurrent)
        {
            if (Queue.Count == 0)
            {
                // Был последний трек → полная остановка
                _currentTrack = null;
                _playbackIndex = -1;
            }
            else
            {
                // Берём следующий (или текущую позицию, если были не в конце)
                // Clamp индекса, чтобы не выйти за границы _playbackOrder
                if (_playbackIndex >= Queue.Count) 
                    _playbackIndex = Queue.Count - 1;
            
                ApplyCurrentTrackFromIndex();
            }
        }
    }

    public static void SetQueue(IEnumerable<TrackInfoDto> tracks)
    {
        Queue.Clear();
        foreach (var t in tracks) Queue.Add(t);
        
        RebuildPlaybackOrder();
        _playbackIndex = Queue.Count > 0 ? 0 : -1;
        _currentTrack = Queue.ElementAtOrDefault(0);
        WeakReferenceMessenger.Default.Send(new TrackChangedMessage(CurrentTrack));
    }

    public static void ToggleShuffle()
    {
        IsShuffled = !IsShuffled;
        RebuildPlaybackOrder();
        
        // Опционально: пытаемся сохранить текущий трек на том же месте
        if (CurrentTrack != null)
        {
            var idx = _playbackOrder.IndexOf(Queue.IndexOf(CurrentTrack));
            if (idx >= 0) _playbackIndex = idx;
        }
    }
    
    public static void ClearQueue()
    {
        Queue.Clear();
        _playbackOrder.Clear();
        _playbackIndex = -1;
    }

    private static void RebuildPlaybackOrder()
    {
        _playbackOrder = Enumerable.Range(0, Queue.Count).ToList();
        
        if (IsShuffled && _playbackOrder.Count > 1)
        {
            // Fisher-Yates (алгоритм Кнута)
            for (int i = _playbackOrder.Count - 1; i > 0; i--)
            {
                int j = Random.Shared.Next(i + 1);
                (_playbackOrder[i], _playbackOrder[j]) = (_playbackOrder[j], _playbackOrder[i]);
            }
        }
    }

    public static void NextTrack()
    {
        if (_playbackOrder.Count == 0) return;
        _playbackIndex = (_playbackIndex + 1) % _playbackOrder.Count;
        ApplyCurrentTrackFromIndex();
        WeakReferenceMessenger.Default.Send(new TrackChangedMessage(CurrentTrack));
    }

    public static void PreviousTrack()
    {
        if (_playbackOrder.Count == 0) return;
        _playbackIndex = _playbackIndex <= 0 ? _playbackOrder.Count - 1 : _playbackIndex - 1;
        ApplyCurrentTrackFromIndex();
        WeakReferenceMessenger.Default.Send(new TrackChangedMessage(CurrentTrack));
    }

    private static void ApplyCurrentTrackFromIndex()
    {
        int actualQueueIndex = _playbackOrder[_playbackIndex];
        CurrentTrack = Queue[actualQueueIndex];
    }
}
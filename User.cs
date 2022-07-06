using System.ComponentModel;

namespace Witcher3SaveToggle;

public class User : INotifyPropertyChanged {
    private string _codeName;
    private string _displayName;
    private bool _isActive;

    public string CodeName {
        get => _codeName;
        set {
            _codeName = value;
            OnPropertyChanged("CodeName");
        }
    }

    public string DisplayName {
        get => _displayName;
        set {
            _displayName = value;
            OnPropertyChanged("DisplayName");
        }
    }

    public bool IsActive {
        get => _isActive;
        set {
            _isActive = value;
            OnPropertyChanged("IsActive");
        }
    }

    public User() { }

    public User(string codeName, string displayName, bool isActive) {
        _codeName = codeName;
        _displayName = displayName;
        _isActive = isActive;
    }

    public event PropertyChangedEventHandler PropertyChanged;

    private void OnPropertyChanged(string name) {
        var handler = PropertyChanged;
        handler?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}
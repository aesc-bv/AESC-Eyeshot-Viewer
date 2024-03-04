using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace AESC_Eyeshot_Viewer.ViewModel
{
    internal class EyeshotTabViewModel : INotifyPropertyChanged
    {
        private const string _defaultInfoText = "Click on any line, curve, or object to see information regarding your selection";
        private string _selectedEntityLengthInformationText;
        private string _selectedEntityRadiusInformationText;

        public string SelectedEntityLengthInformationText
        {
            get => _selectedEntityLengthInformationText;
            set
            {
                _selectedEntityLengthInformationText = value == string.Empty ? "" : $"Lengte: {value}";
                NotifyPropertyChanged();
            }
        }

        public string SelectedEntityRadiusInformationText
        {
            get => _selectedEntityRadiusInformationText;
            set
            {
                _selectedEntityRadiusInformationText = value == string.Empty ? "" : $"Radius: {value}";
                NotifyPropertyChanged();
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public EyeshotTabViewModel(string selectedEntityInformationText = _defaultInfoText)
        {
            _selectedEntityLengthInformationText = selectedEntityInformationText;
            NotifyPropertyChanged(nameof(SelectedEntityLengthInformationText));
        }

        public EyeshotTabViewModel() { }

        protected virtual void NotifyPropertyChanged([CallerMemberName] string propertyName = "") =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}

using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace AESC_Eyeshot_Viewer.ViewModel
{
    internal class EyeshotTabViewModel : INotifyPropertyChanged
    {
        private string _selectedEntityLengthInformationText;
        private string _selectedEntityRadiusInformationText;

        public string SelectedEntityLengthInformationText
        {
            get => _selectedEntityLengthInformationText;
            set
            {
                _selectedEntityLengthInformationText = value == string.Empty ? "" : $"{Properties.Resources.LengthInformationLabel} {value}";
                NotifyPropertyChanged();
            }
        }

        public string SelectedEntityRadiusInformationText
        {
            get => _selectedEntityRadiusInformationText;
            set
            {
                _selectedEntityRadiusInformationText = value == string.Empty ? "" : $"{Properties.Resources.RadiusInformationLabel} {value}";
                NotifyPropertyChanged();
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public EyeshotTabViewModel(string selectedEntityInformationText)
        {
            _selectedEntityLengthInformationText = selectedEntityInformationText ?? Properties.Resources.SelectionDefaultInfoText;
            NotifyPropertyChanged(nameof(SelectedEntityLengthInformationText));
        }

        public EyeshotTabViewModel() 
        {
            _selectedEntityLengthInformationText = Properties.Resources.SelectionDefaultInfoText;
            NotifyPropertyChanged(nameof(SelectedEntityLengthInformationText));
        }

        protected virtual void NotifyPropertyChanged([CallerMemberName] string propertyName = "") =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace AESC_Eyeshot_Viewer.ViewModel
{
    internal class EyeshotTabViewModel : INotifyPropertyChanged
    {
        private const string _defaultInfoText = "This pane shows information about your selection";
        private string _selectedEntityInformationText;
        public string SelectedEntityInformationText
        {
            get
            {
                return _selectedEntityInformationText;
            }
            set
            {
                _selectedEntityInformationText = value;
                NotifyPropertyChanged();
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public EyeshotTabViewModel(string selectedEntityInformationText = _defaultInfoText) 
            => SelectedEntityInformationText = selectedEntityInformationText;

        public EyeshotTabViewModel() => SelectedEntityInformationText = _defaultInfoText;

        public void SetLengthInformationString(double length)
        {
            {
                if (SelectedEntityInformationText == string.Empty || SelectedEntityInformationText == _defaultInfoText)
                    SelectedEntityInformationText = $"Lengte: {length:F}";
                else
                    SelectedEntityInformationText += $", Lengte: {length:F}";
            }
        }

        public void SetRadiusInformationString(double radius)
        {
            if (SelectedEntityInformationText == string.Empty)
                SelectedEntityInformationText = $"Radius: {radius:F}";
            else
                SelectedEntityInformationText += $", Radius: {radius:F}";
        }

        public void ClearInformationString(string defaultString = _defaultInfoText) 
            => SelectedEntityInformationText = defaultString;

        protected virtual void NotifyPropertyChanged([CallerMemberName] string propertyName = "") =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}

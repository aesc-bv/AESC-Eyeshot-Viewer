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
        private const string _defaultInfoText = "Click on any line, curve, or object to see information regarding your selection";
        private string _selectedEntityInformationText;
        private string _selectedEntityLengthInformationText;
        private string _selectedEntityRadiusInformationText;
        private string _selectedEntityAText = string.Empty;
        private string _selectedEntityBText = string.Empty;
        public string SelectedEntityInformationText
        {
            get => _selectedEntityInformationText;
            set
            {
                _selectedEntityInformationText = value;
                NotifyPropertyChanged();
            }
        }

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

        public string SelectedEntityAText
        {
            get => _selectedEntityAText;
            set
            {
                _selectedEntityAText = value == string.Empty ? string.Empty : $"Entity A: {value}";
                NotifyPropertyChanged();
            }
        }

        public string SelectedEntityBText
        {
            get => _selectedEntityBText;
            set
            {
                _selectedEntityBText = value == string.Empty ? string.Empty : $"Entity B: {value}";
                NotifyPropertyChanged();
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public EyeshotTabViewModel(string selectedEntityInformationText = _defaultInfoText)
        {
            SelectedEntityInformationText = selectedEntityInformationText;
            _selectedEntityLengthInformationText = selectedEntityInformationText;
            NotifyPropertyChanged(nameof(SelectedEntityLengthInformationText));
        }

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

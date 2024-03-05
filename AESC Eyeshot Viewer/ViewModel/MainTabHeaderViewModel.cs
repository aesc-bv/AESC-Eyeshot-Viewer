using System.ComponentModel;
using System.Runtime.CompilerServices;


namespace AESC_Eyeshot_Viewer.ViewModel
{
    internal class MainTabHeaderViewModel : INotifyPropertyChanged
    {
        public string Header 
        {
            get => _header;
            set {
                _header = value;
                NotifyPropertyChanged();
            } 
        }
        public int Index
        {
            get => _index;
            set
            {
                _index = value;
                NotifyPropertyChanged();
            }
        }

        private string _header;
        private int _index;

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void NotifyPropertyChanged([CallerMemberName] string propertyName = "") =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace AESC_Eyeshot_Viewer.ViewModel
{
    internal class MainTabHeaderViewModel : INotifyPropertyChanged
    {
        public string Header 
        { 
            get { return _header; } 
            set {
                _header = value;
                NotifyPropertyChanged();
            } 
        }
        public int Index
        {
            get { return _index; }
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

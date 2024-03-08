using devDept.Eyeshot.Control;
using devDept.Eyeshot.Translators;
using devDept.Eyeshot;
using System;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Threading;
using System.Threading;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace AESC_Eyeshot_Viewer.ViewModel
{
    public class EyeshotDesignViewModel : INotifyPropertyChanged
    {
        private readonly string[] stepExtensions = new string[] { ".stp", ".step" };
        private readonly string[] dxfExtensions = new string[] { ".dxf" };
        private readonly string[] dwgExtensions = new string[] { ".dwg" };
        private bool _isMeasureModeActive = false;
        private Visibility _shouldShowGuide = Visibility.Collapsed;
        private string _currentActiveAction = "selecting";
        private string _userGuide = string.Empty;

        public string LoadedFilePath { get; set; } = string.Empty;
        public string LoadedFileName { get; set; } = string.Empty;

        public bool IsLoaded { get; set; } = false;
        public bool IsMeasureModeActive
        {
            get => _isMeasureModeActive;
            set
            {
                _isMeasureModeActive = value;
                NotifyPropertyChanged();

                if (_isMeasureModeActive)
                {
                    ShouldShowGuide = Visibility.Visible;
                    CurrentActiveFunction = Properties.Resources.ActiveModeMeasuring;
                    UserGuide = Properties.Resources.GuideMeasureStepOne;
                }
                else
                {
                    ShouldShowGuide = Visibility.Collapsed;
                    CurrentActiveFunction = Properties.Resources.ActiveModeSelecting;
                    UserGuide = string.Empty;
                }
            }
        }
        public bool IsMeasureVisible { get; set; } = false;

        public Visibility ShouldShowGuide 
        { 
            get => _shouldShowGuide; 
            set 
            {
                _shouldShowGuide = value;
                NotifyPropertyChanged();
            } 
        }

        public string CurrentActiveFunction
        {
            get => _currentActiveAction; 
            set
            {
                _currentActiveAction = value;
                NotifyPropertyChanged();
            }
        }

        public string UserGuide
        {
            get => _userGuide;
            set
            {
                _userGuide = value;
                NotifyPropertyChanged();
            }
        }

        public event IsLoadedEventHandler IsLoadedEvent;
        public event PropertyChangedEventHandler PropertyChanged;

        public delegate void IsLoadedEventHandler(object sender, EventArgs isLoadedEventArgs);


        public EntityList EntityList { get; set; } = new EntityList();

        public string ImportFile(string filePath, Design design)
        {
            if (File.Exists(LoadedFilePath))
            {
                ReadFileAsync fileReader;

                if (stepExtensions.Contains(Path.GetExtension(filePath).ToLower()))
                    fileReader = new ReadSTEP(filePath);
                else if (dxfExtensions.Contains(Path.GetExtension(filePath).ToLower()))
                    fileReader = new ReadDXF(filePath);
                else if (dwgExtensions.Contains(Path.GetExtension(filePath).ToLower()))
                    fileReader = new ReadDWG(filePath);
                else
                    throw new InvalidDataException($"Given file extension is not valid: { Path.GetExtension(filePath) }");

                if (fileReader is null) return string.Empty;

                design.Clear();

                Dispatcher.CurrentDispatcher.InvokeAsync(() =>
                {
                    var counter = 0;
                    while (design.IsBusy && counter < 50)
                    {
                        Thread.Sleep(100);
                        counter++;
                    }

                    try
                    {
                        design.StartWork(fileReader);
                    } catch 
                    {
                        MessageBox.Show("Could not load file, because other tasks were running in parallel. Please wait a bit and try again");
                    }
                    
                });

                LoadedFilePath = filePath;
                LoadedFileName = Path.GetFileNameWithoutExtension(filePath);

                return filePath;
            }

            return string.Empty;
        }

        public void InvokeIsLoadedEvent() => IsLoadedEvent?.Invoke(this, new EventArgs());

        protected virtual void NotifyPropertyChanged([CallerMemberName] string propertyName = "") =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}

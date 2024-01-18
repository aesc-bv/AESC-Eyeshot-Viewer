using AESC_Eyeshot_Viewer.Models;
using devDept.Eyeshot.Control;
using devDept.Eyeshot.Translators;
using devDept.Eyeshot;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using devDept;
using AESC_Eyeshot_Viewer.ViewModel;
using System.IO;
using AESC_Eyeshot_Viewer.View;
using System.ComponentModel;
using System.Runtime.Remoting.Contexts;
using System.CodeDom.Compiler;
using System.Threading;
using AESC_Eyeshot_Viewer.Interfaces;

namespace AESC_Eyeshot_Viewer
{

    public delegate void CloseLoadingWindow();
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow() => InitializeComponent();

        private volatile LoadingWindow loadingWindow;

        private void Design_DragEnter(object _, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
                e.Effects = DragDropEffects.Copy;
            else
                e.Effects = DragDropEffects.None;
        }

        private void Design_Drop(object _, DragEventArgs e)
        {
            if (e.Data.GetData(DataFormats.FileDrop) is string[] files && files.Length > 0)
            {
                var context = DataContext as MainWindowViewModel;

                var loadingWindowThread = new Thread(() =>
                {
                    loadingWindow = new LoadingWindow
                    {
                        LoadingText = "Tabbladen aan het laden...",
                    };

                    loadingWindow.Closed += (sender, args) =>
                    {
                        loadingWindow.Dispatcher.InvokeShutdown();
                        loadingWindow = null;
                    };

                    loadingWindow.Show();
                    System.Windows.Threading.Dispatcher.Run();
                });

                loadingWindowThread.SetApartmentState(ApartmentState.STA);
                loadingWindowThread.Start();

                foreach (var filePath in files.Where(path => context.IsExtensionAcceptable(Path.GetExtension(path))))
                {
                    var loadedFile = new EyeshotFile { Name = Path.GetFileNameWithoutExtension(filePath), Path = filePath };
                    context.Files.Add(loadedFile);

                    var fileType = context.GetFileTypeOf(filePath);

                    IEyeshotTabView newTabView;
                    if (fileType == FileType.File2D)
                        newTabView = new Eyeshot2DTabView();
                    else
                        newTabView = new EyeshotTabView();

                    newTabView.GetEyeshotView().EyeshotDesignLoadComplete += DesignView_EyeshotDesignLoadComplete;
                    var newTabContext = newTabView.GetEyeshotView().GetDataContext();
                    newTabContext.LoadedFilePath = loadedFile.Path;
                    newTabContext.LoadedFileName = loadedFile.Name;

                    var newTabHeader = new MainTabHeaderView(loadedFile.Name, MainTabControl.Items.Count);
                    newTabHeader.CloseTabEvent += NewTabHeader_CloseTabEvent;

                    var newTab = new TabItem
                    {
                        Content = newTabView,
                        Header = newTabHeader,
                        Name = $"Tab{(newTabHeader.DataContext as MainTabHeaderViewModel).Index}",
                    };

                    MainTabControl.Items.Add(newTab);
                    MainTabControl.SelectedIndex = 1;
                }                
            }
        }

        private void DesignView_EyeshotDesignLoadComplete(object _, EventArgs _e)
        {
            if (MainTabControl.Items.Count == (DataContext as MainWindowViewModel).Files.Count + 1
                && loadingWindow != null)
                loadingWindow.Dispatcher.BeginInvoke(loadingWindow.CloseLoadingWindow, System.Windows.Threading.DispatcherPriority.ApplicationIdle);
        }

        private void NewTabHeader_CloseTabEvent(object sender, CloseTabEventArgs closeTabEventArgs)
        {
            (DataContext as MainWindowViewModel).Files.RemoveAt(closeTabEventArgs.IndexToClose - 1);
            MainTabControl.Items.RemoveAt(closeTabEventArgs.IndexToClose);
        }
    }
}

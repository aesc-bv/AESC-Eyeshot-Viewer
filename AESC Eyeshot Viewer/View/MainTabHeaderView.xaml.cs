using AESC_Eyeshot_Viewer.ViewModel;
using System.Windows;
using System.Windows.Controls;

namespace AESC_Eyeshot_Viewer.View
{
    /// <summary>
    /// Interaction logic for MainTabHeaderView.xaml
    /// </summary>
    public partial class MainTabHeaderView : UserControl
    {
        public event CloseTabEventHandler CloseTabEvent;
        public delegate void CloseTabEventHandler(object sender, CloseTabEventArgs closeTabEventArgs);

        public MainTabHeaderView() => InitializeComponent();

        public MainTabHeaderView(string header, int index) : base()
        {
            InitializeComponent();
            (DataContext as MainTabHeaderViewModel).Header = header;
            (DataContext as MainTabHeaderViewModel).Index = index;
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e) 
            => CloseTabEvent?.Invoke(this, new CloseTabEventArgs((DataContext as MainTabHeaderViewModel).Index));
    }

    public class CloseTabEventArgs
    {
        public CloseTabEventArgs(int index) => IndexToClose = index;
        public int IndexToClose { get; }
    }
}

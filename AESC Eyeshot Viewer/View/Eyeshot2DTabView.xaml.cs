using AESC_Eyeshot_Viewer.Interfaces;
using AESC_Eyeshot_Viewer.ViewModel;
using System;
using System.Collections.Generic;
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
using System.Windows.Shapes;

namespace AESC_Eyeshot_Viewer.View
{
    /// <summary>
    /// Interaction logic for Eyeshot2DTabView.xaml
    /// </summary>
    public partial class Eyeshot2DTabView : UserControl, IEyeshotTabView
    {
        public Eyeshot2DTabView()
        {
            InitializeComponent();
        }

        public IEyeshotDesignView GetEyeshotView() => DraftView;
    }
}

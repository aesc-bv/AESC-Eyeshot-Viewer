using devDept.CustomControls;
using devDept.Eyeshot;
using devDept.Eyeshot.Control;
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
    /// Interaction logic for EyeshotTreeViewer.xaml
    /// </summary>
    public partial class EyeshotTreeViewer : UserControl
    {
        public Workspace Workspace { get; set; }
        public EyeshotTreeViewer()
        {
            
        }

        public void Initialize()
        {
            InitializeComponent();
            AssemblyTreeView.Workspace = Workspace;
            AssemblyTreeView.InitializeContextMenu();
            Workspace.WorkCompleted += Workspace_WorkCompleted;
        }

        private void Workspace_WorkCompleted(object sender, devDept.WorkCompletedEventArgs e)
        {
            try
            {
                if (e.WorkUnit is Regeneration)
                    AssemblyTreeView.RefreshTree();
            } catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.ToString());
            }
            
        }
    }
}

using AESC_Eyeshot_Viewer.Interfaces;
using AESC_Eyeshot_Viewer.ViewModel;
using devDept.Eyeshot.Entities;
using System.Linq;
using System.Windows.Controls;
using System.Windows.Input;

namespace AESC_Eyeshot_Viewer.View
{
    /// <summary>
    /// Interaction logic for EyeshotTabView.xaml
    /// </summary>
    public partial class EyeshotTabView : UserControl, IEyeshotTabView
    {
        public EyeshotTabView()
        {
            InitializeComponent();

            TreeView.Workspace = DesignView.Design;
            TreeView.Initialize();

            DesignView.EntityWasSelected += DesignView_EntityWasSelected;
        }

        public UserControl GetEyeshotView() => DesignView;

        private void DesignView_EntityWasSelected(object sender, EntityWasSelectedEventArgs e)
        {
            var context = DataContext as EyeshotTabViewModel;
            context.ClearInformationString();
            if (e.Entity is Curve curve && curve.IsLinear(0.5, out var line))
                context.SetLengthInformationString(line.Length);
            else if (e.Entity is Curve curvedEntity)
            {
                if (curvedEntity.ConvertToArcsAndLines().FirstOrDefault(arcOrLine => arcOrLine is Arc) is Arc firstArc)
                {
                    context.SetLengthInformationString(curvedEntity.Length());
                    context.SetRadiusInformationString(firstArc.Radius);
                }
                else
                    context.SetLengthInformationString(curvedEntity.Length());
            }
            else if (e.Entity is devDept.Eyeshot.Entities.Line lineEntity && lineEntity.IsLinear(0.5, out var lineEntityLine))
                context.SetLengthInformationString(lineEntityLine.Length);
        }

        IEyeshotDesignView IEyeshotTabView.GetEyeshotView()
        {
            throw new System.NotImplementedException();
        }

        private void UserControl_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if ((Keyboard.Modifiers & ModifierKeys.Control) == ModifierKeys.Control)
            {
                if (Keyboard.IsKeyDown(Key.H))
                {
                    TreeView.AssemblyTreeView.AssemblyTreeView.ChangeVisibilityOnClick(sender, e);
                    e.Handled = true;
                }
                else if (Keyboard.IsKeyDown(Key.I))
                {
                    if (TreeView.AssemblyTreeView.AssemblyTreeView.IsIsolateActive)
                        TreeView.AssemblyTreeView.AssemblyTreeView.UnisolateAllOnClick(sender, e);
                    else
                        TreeView.AssemblyTreeView.AssemblyTreeView.IsolateOnClick(sender, e);

                    e.Handled = true;
                }
            }
        }
    }
}

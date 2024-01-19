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
using AESC_Eyeshot_Viewer.Events;
using devDept.Eyeshot.Entities;

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

            DesignViewEvents.EntityWasSelected += DesignViewEvents_EntityWasSelected;
            LayerView.Workspace = DraftView.DraftDesign;
        }

        private void DesignViewEvents_EntityWasSelected(object sender, Events.EntityWasSelectedEventArgs e)
        {
            if (sender is EyeshotDraftView)
            {
                var context = DataContext as EyeshotTabViewModel;
                context.SelectedEntityRadiusInformationText = string.Empty;
                context.SelectedEntityLengthInformationText = string.Empty;

                if (e.Entity is devDept.Eyeshot.Entities.Line lineEntity)
                    context.SelectedEntityLengthInformationText = lineEntity.Length().ToString("F");
                    
                if (e.Entity is Arc arcEntity)
                {
                    context.SelectedEntityLengthInformationText = arcEntity.Length().ToString("F");
                    context.SelectedEntityRadiusInformationText = arcEntity.Radius.ToString("F");
                }
                    

                if (e.Entity is Circle circleEntity)
                {
                    context.SelectedEntityLengthInformationText = circleEntity.Length().ToString("F");
                    context.SelectedEntityRadiusInformationText = circleEntity.Radius.ToString("F");
                }
            }
        }

        public IEyeshotDesignView GetEyeshotView() => DraftView;

        private void TriggerLayerView_Click(object sender, RoutedEventArgs e)
        {
            LayerView.Visible = !LayerView.Visible;
            WindowsFormHost.Visibility = WindowsFormHost.Visibility == Visibility.Collapsed ? Visibility.Visible : Visibility.Collapsed;

            if(!LayerView.Visible)
            {
                Grid.SetColumn(DraftView, 0);
                Grid.SetColumnSpan(DraftView, 2);
            }
            else
            {
                Grid.SetColumn(DraftView, 1);
                Grid.SetColumnSpan(DraftView, 1);
                Dispatcher.InvokeAsync(() => DraftView.DraftDesign.ZoomFit(margin: 25), System.Windows.Threading.DispatcherPriority.ContextIdle);
            }
        }
    }
}

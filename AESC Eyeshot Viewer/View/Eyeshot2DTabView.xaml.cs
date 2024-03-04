using AESC_Eyeshot_Viewer.Interfaces;
using AESC_Eyeshot_Viewer.ViewModel;
using System.Windows;
using System.Windows.Controls;
using AESC_Eyeshot_Viewer.Events;
using devDept.Eyeshot.Entities;
using AESC_Eyeshot_Viewer.Models;

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

        private void DesignViewEvents_EntityWasSelected(object sender, EntityWasSelectedEventArgs e)
        {
            if (sender is EyeshotDraftView)
            {
                var context = DataContext as EyeshotTabViewModel;
                context.SelectedEntityLengthInformationText = string.Empty;
                context.SelectedEntityRadiusInformationText = string.Empty;
                
                if (e.Entity is Line lineEntity)
                    context.SelectedEntityLengthInformationText = lineEntity.Length().ToString("F") + MeasurementHelper.ToAbbreviation(e.Unit);
                    
                if (e.Entity is Arc arcEntity)
                {
                    context.SelectedEntityLengthInformationText = arcEntity.Length().ToString("F") + MeasurementHelper.ToAbbreviation(e.Unit);
                    context.SelectedEntityRadiusInformationText = arcEntity.Radius.ToString("F") + MeasurementHelper.ToAbbreviation(e.Unit);
                }
                    

                if (e.Entity is Circle circleEntity)
                {
                    context.SelectedEntityLengthInformationText = circleEntity.Length().ToString("F") + MeasurementHelper.ToAbbreviation(e.Unit);
                    context.SelectedEntityRadiusInformationText = circleEntity.Radius.ToString("F") + MeasurementHelper.ToAbbreviation(e.Unit);
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

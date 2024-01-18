using AESC_Eyeshot_Viewer.Interfaces;
using AESC_Eyeshot_Viewer.ViewModel;
using devDept.Eyeshot.Control;
using devDept.Eyeshot;
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
        }

        private void DesignViewEvents_EntityWasSelected(object sender, Events.EntityWasSelectedEventArgs e)
        {
            if (sender is EyeshotDraftView)
            {
                if (e.Entity is devDept.Eyeshot.Entities.Line lineEntity)
                    LengthStatusLabel.Text = lineEntity.Length().ToString();

                if (e.Entity is Arc arcEntity)
                    LengthStatusLabel.Text = arcEntity.Length().ToString();

                if (e.Entity is Circle circleEntity)
                    LengthStatusLabel.Text = circleEntity.Length().ToString();
            }
        }

        public IEyeshotDesignView GetEyeshotView() => DraftView;
    }
}

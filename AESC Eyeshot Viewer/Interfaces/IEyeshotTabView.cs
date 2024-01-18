using AESC_Eyeshot_Viewer.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace AESC_Eyeshot_Viewer.Interfaces
{
    internal interface IEyeshotTabView
    {
        IEyeshotDesignView GetEyeshotView();
    }
}

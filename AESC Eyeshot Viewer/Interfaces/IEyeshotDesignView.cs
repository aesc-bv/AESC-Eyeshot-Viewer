using AESC_Eyeshot_Viewer.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using static AESC_Eyeshot_Viewer.View.EyeshotDesignView;

namespace AESC_Eyeshot_Viewer.Interfaces
{
    public interface IEyeshotDesignView
    {
        event EyeshotDesignLoadCompleted EyeshotDesignLoadComplete;

        EyeshotDesignViewModel GetDataContext();
    }
}

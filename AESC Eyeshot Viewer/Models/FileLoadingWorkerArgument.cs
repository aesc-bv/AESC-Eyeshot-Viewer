using AESC_Eyeshot_Viewer.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace AESC_Eyeshot_Viewer.Models
{
    internal class FileLoadingWorkerArgument
    {
        public IEnumerable<string> Files { get; set; }
        public MainWindowViewModel Context { get; set; }
        public TabControl TabControl { get; set; }
    }
}

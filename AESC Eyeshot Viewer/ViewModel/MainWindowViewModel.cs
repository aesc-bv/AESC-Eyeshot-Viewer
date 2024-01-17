using AESC_Eyeshot_Viewer.Models;
using devDept.Eyeshot;
using devDept.Eyeshot.Control;
using devDept.Eyeshot.Translators;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Remoting.Contexts;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace AESC_Eyeshot_Viewer.ViewModel
{
    public class MainWindowViewModel
    {
        public List<EyeshotFile> Files { get; } = new List<EyeshotFile>();
        public int FilesLoaded = 0;
        private string[] AcceptableExtensions { get; } = new string[] { ".stp", ".step" };
        public bool IsExtensionAcceptable(string extension) => AcceptableExtensions.Contains(extension.ToLower());
    }
}

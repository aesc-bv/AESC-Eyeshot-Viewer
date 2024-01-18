using devDept.Eyeshot.Control;
using devDept.Eyeshot.Translators;
using devDept.Eyeshot;
using devDept;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Runtime.Remoting.Contexts;
using AESC_Eyeshot_Viewer.View;
using System.Runtime.Remoting.Channels;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Threading;
using System.Threading;

namespace AESC_Eyeshot_Viewer.ViewModel
{
    public class EyeshotDesignViewModel
    {
        private string[] AcceptableExtensions { get; } = new string[] { ".stp", ".step" };
        private readonly string[] stepExtensions = new string[] { ".stp", ".step" };
        private readonly string[] dxfExtensions = new string[] { ".dxf" };
        private readonly string[] dwgExtensions = new string[] { ".dwg" };
        public string LoadedFilePath { get; set; } = string.Empty;
        public string LoadedFileName { get; set; } = string.Empty;

        public bool IsLoaded { get; set; } = false;

        public event IsLoadedEventHandler IsLoadedEvent;

        public delegate void IsLoadedEventHandler(object sender, EventArgs isLoadedEventArgs);


        public EntityList EntityList { get; set; } = new EntityList();

        public string ImportFile(string filePath, Design design)
        {
            if (File.Exists(filePath))
            {
                ReadFileAsync fileReader;

                if (stepExtensions.Contains(Path.GetExtension(filePath).ToLower()))
                    fileReader = new ReadSTEP(filePath);
                else if (dxfExtensions.Contains(Path.GetExtension(filePath).ToLower()))
                    fileReader = new ReadDXF(filePath);
                else if (dwgExtensions.Contains(Path.GetExtension(filePath).ToLower()))
                    fileReader = new ReadDWG(filePath);
                else
                    throw new InvalidDataException($"Given file extension is not valid: { Path.GetExtension(filePath) }");

                if (fileReader is null) return string.Empty;

                design.Clear();
                design.StartWork(fileReader);

                LoadedFilePath = filePath;
                LoadedFileName = Path.GetFileNameWithoutExtension(filePath);

                return filePath;
            }

            return string.Empty;
        }

        public void InvokeIsLoadedEvent() => IsLoadedEvent?.Invoke(this, new EventArgs());

        public bool IsExtensionAcceptable(string extension) => AcceptableExtensions.Contains(extension.ToLower());
    }
}

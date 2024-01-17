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

namespace AESC_Eyeshot_Viewer.ViewModel
{
    public class EyeshotDesignViewModel
    {
        private string[] AcceptableExtensions { get; } = new string[] { ".stp", ".step" };
        public string LoadedFilePath { get; set; } = string.Empty;
        public string LoadedFileName { get; set; } = string.Empty;

        public bool IsLoaded { get; set; } = false;

        public event IsLoadedEventHandler IsLoadedEvent;

        public delegate void IsLoadedEventHandler(object sender, EventArgs isLoadedEventArgs);


        public EntityList EntityList { get; set; } = new EntityList();

        public string ImportFileSTP(string filePath, Design design)
        {
            if (File.Exists(filePath))
            {
                var stepReader = new ReadSTEP(filePath);
                if (stepReader is null) return string.Empty;

                design.Clear();
                design.StartWork(stepReader);

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

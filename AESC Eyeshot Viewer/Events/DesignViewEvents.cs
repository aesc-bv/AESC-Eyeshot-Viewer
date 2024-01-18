using devDept.Eyeshot.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AESC_Eyeshot_Viewer.Events
{
    public static class DesignViewEvents
    {
        public static event EntityWasSelectedEvent EntityWasSelected;
        public delegate void EntityWasSelectedEvent(object sender, EntityWasSelectedEventArgs e);

        public static void InvokeEntityWasSelectedEvent(object sender, EntityWasSelectedEventArgs args)
            => EntityWasSelected?.Invoke(sender, args);
    }
}

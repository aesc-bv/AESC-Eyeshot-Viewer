using devDept.Eyeshot.Entities;
using devDept.Geometry;

namespace AESC_Eyeshot_Viewer.Events
{
    public class EntityWasSelectedEventArgs
    {
        public Entity Entity { get; set; }
        public bool IsMeasuring { get; set; } = false;
        public linearUnitsType Unit { get; set; }
    }
}

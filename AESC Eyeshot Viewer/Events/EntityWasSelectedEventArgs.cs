using devDept.Eyeshot.Entities;

namespace AESC_Eyeshot_Viewer.Events
{
    public class EntityWasSelectedEventArgs
    {
        public Entity Entity { get; set; }
        public bool IsMeasuring { get; set; } = false;
    }
}

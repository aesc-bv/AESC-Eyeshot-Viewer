using devDept.Geometry;

namespace AESC_Eyeshot_Viewer.Models
{
    public static class MeasurementHelper
    {
        public static string ToAbbreviation(linearUnitsType unit)
        {
            switch(unit)
            {
                case linearUnitsType.Kilometers:
                    return "km";
                case linearUnitsType.Meters:
                    return "m";
                case linearUnitsType.Centimeters:
                    return "cm";
                case linearUnitsType.Millimeters:
                    return "mm";
                case linearUnitsType.Inches:
                    return "in";
                case linearUnitsType.Feet:
                    return "ft";
                case linearUnitsType.Miles:
                    return "mi";
                default:
                    return "unit";
            }
        }
    }
}

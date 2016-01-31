public class DistanceDisplay
{
    public static string GetDistanceString(float distance)
    {
        if (distance < 10f)
            return string.Format("{0:n2}m", distance);
        if (distance < 100f)
            return string.Format("{0:n1}m", distance);
        if (distance < 1000f)
            return string.Format("{0:n0}m", distance);
        if (distance < 10000f)
            return string.Format("{0:n2}km", distance/1000f);
        if (distance < 100000f)
            return string.Format("{0:n1}km", distance/1000f);
        if (distance < 1000000f)
            return string.Format("{0:n0}km", distance/1000f);
        if (distance < 10000000f)
            return string.Format("{0:n2}Mm", distance/1000000f);
        if (distance < 100000000f)
            return string.Format("{0:n1}Mm", distance/1000000f);
        return string.Format("{0:n0}Mm", distance/1000000f);
    }
}

using UnityEngine;

public class Formations
{
    public static Vector3 GetArrowOffset(int index, float stepSize)
    {
        var halfStep = stepSize/2f;
        var alternating = Mathf.Sign(-index%2);
        if (alternating < 0)
        {
            return Vector3.forward*-halfStep*(index - 1) + Vector3.right*halfStep*(index - 1)*alternating;
        }
        return Vector3.forward*-halfStep*index + Vector3.right*halfStep*index*alternating;
    }
}

using UnityEngine;

public class MotherhsipBayDoors : MonoBehaviour
{
    public Transform DoorLeft;
    public Transform DoorRight;
    public float CloseTime = 1f;

    [Header("Offsets")]
    public Vector3 LeftClosedOffset;
    public Vector3 RightClosedOffset;

    public Vector3 LeftOpenOffset;
    public Vector3 RightOpenOffset;

    private bool _isClosing;
    private float _closeCooldown;

    private void Update()
    {
        if (_isClosing)
        {
            if (_closeCooldown >= 0f)
            {
                _closeCooldown -= Time.deltaTime;
                var closeFraction = Mathf.Clamp01(_closeCooldown / CloseTime);
                if (_closeCooldown > 0f)
                {
                    DoorLeft.localPosition = Vector3.Lerp(LeftOpenOffset, LeftClosedOffset, 1f - closeFraction);
                    DoorRight.localPosition = Vector3.Lerp(RightOpenOffset, RightClosedOffset, 1f - closeFraction);
                }
                else
                {
                    _isClosing = false;
                    DoorLeft.localPosition = LeftClosedOffset;
                    DoorRight.localPosition = RightClosedOffset;
                    enabled = false;
                }
            }
        }
    }

    public void TriggerClose()
    {
        _closeCooldown = CloseTime;
        _isClosing = true;
        enabled = true;
    }
}

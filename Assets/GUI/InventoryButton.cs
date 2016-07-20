using UnityEngine;
using UnityEngine.EventSystems;
using System;

public class InventoryButton : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    public float DelayTime = 0.5f;
    public float HoldTime = 1f;

    public delegate void OnInventoryButtonHoldFinish();
    public OnInventoryButtonHoldFinish OnHoldFinish;

    public Action OnStartHoldAction;
    public Action<float> OnHoldAction;
    public Action OnHoldFinishAction;
    public Action OnReleaseAction;

    private bool _isDown;
    private float _holdCooldown;

    public void OnPointerDown(PointerEventData eventData)
    {
        if (!_isDown)
        {
            _isDown = true;
            _holdCooldown = DelayTime + HoldTime;
            if (OnStartHoldAction != null)
                OnStartHoldAction();
        }
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (_isDown)
        {
            _isDown = false;
            if (OnReleaseAction != null)
                OnReleaseAction();
        }
    }

    private void Update()
    {
        if (_isDown)
        {
            if (_holdCooldown >= 0f)
            {
                _holdCooldown -= Time.deltaTime;
                if (OnHoldAction != null)
                    OnHoldAction(GetHoldFraction());
                if (_holdCooldown < 0f)
                {
                    if (OnHoldFinish != null)
                        OnHoldFinish();
                    if (OnHoldFinishAction != null)
                        OnHoldFinishAction();
                }
            }
        }
    }

    public bool GetIsDown()
    {
        return _isDown;
    }

    public float GetHoldFraction()
    {
        if (_holdCooldown > HoldTime)
        {
            return 0f;
        }
        return Mathf.Clamp01(1f - _holdCooldown / HoldTime);
    }
}

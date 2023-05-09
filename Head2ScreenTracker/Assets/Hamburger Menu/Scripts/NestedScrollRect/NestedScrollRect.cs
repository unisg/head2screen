using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class NestedScrollRect : ScrollRect
{
    public DragRegisterVertical m_RegisterkarteStation;
    public ScrollRect m_ParentScrollRect;

    private bool m_SendToParent = false;

    public override void OnInitializePotentialDrag(PointerEventData eventData)
    {
        m_ParentScrollRect.OnInitializePotentialDrag(eventData);
        base.OnInitializePotentialDrag(eventData);
    }

    public override void OnBeginDrag(PointerEventData eventData)
    {
        if (!horizontal && Mathf.Abs(eventData.delta.x) > Mathf.Abs(eventData.delta.y))
        {
            m_SendToParent = true;
            m_ParentScrollRect.OnBeginDrag(eventData);
            m_RegisterkarteStation.OnBeginDrag(eventData);
            return;
        }

        if (!vertical && Mathf.Abs(eventData.delta.y) > Mathf.Abs(eventData.delta.x))
        {
            m_SendToParent = true;
            m_ParentScrollRect.OnBeginDrag(eventData);
            m_RegisterkarteStation.OnBeginDrag(eventData);
            return;
        }

        m_SendToParent = false;
        base.OnBeginDrag(eventData);
    }

    public override void OnDrag(PointerEventData eventData)
    {
        if (m_SendToParent)
            m_ParentScrollRect.OnDrag(eventData);
        else
            base.OnDrag(eventData);
    }

    public override void OnEndDrag(PointerEventData eventData)
    {
        if (m_SendToParent)
        {
            m_RegisterkarteStation.OnEndDrag(eventData);
            m_ParentScrollRect.OnEndDrag(eventData);
        }
        else
            base.OnEndDrag(eventData);
    }
}

using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class DragRegisterHorizontal : MonoBehaviour, IBeginDragHandler, IEndDragHandler
{
    public enum State
    {
        Open,
        Opening,
        Dragging,
        Closed,
        Closing,
    }

    private State m_CurrentState = State.Closed;

    [SerializeField] CanvasScaler m_CanvasScalar;

    [SerializeField] ScrollRect m_ScrollRect;

    [SerializeField] LayoutElement m_Menu_LayoutElement;
    [SerializeField] LayoutElement m_DragHandle_LayoutElement;
    [SerializeField] LayoutElement m_TransparentBG_LayoutElement;

    [SerializeField] Image m_DragHandleImage;
    [SerializeField] Image m_TransparentImage;
    [SerializeField] Color32 m_OverlayColor;

    [SerializeField] float m_DragHandleWidth = 200f;
    private float m_MenuSizeWidth = 400f;
    [SerializeField] float m_TransitionSpeed = 5f;
    [Range(0.0f, 1f)]
    [SerializeField] float m_OpenThreshold = .33f;

    [SerializeField] bool m_AccountForScreenChanges = true;

    private float m_CurrentLerpValue = 0.0f;

    [SerializeField] float m_FastSwipeTimeThreshold = 0.33f;
    private float m_DragStartXPos = 0.0f;
    private float m_FastSwipeTimer = 0.0f;

    private Vector2 m_LastScreenSize = Vector2.zero;

    private void Awake()
    {
        m_MenuSizeWidth = m_Menu_LayoutElement.preferredWidth;

        SetupPanelSizes();

        ChangeState(State.Closed);
    }

    private void SetupPanelSizes()
    {
        m_Menu_LayoutElement.preferredWidth = m_MenuSizeWidth;
        m_DragHandle_LayoutElement.preferredWidth = m_DragHandleWidth;

        // account for scaling
        float width = Screen.width / GetScale(Screen.width, Screen.height, m_CanvasScalar.referenceResolution, m_CanvasScalar.matchWidthOrHeight);
        m_TransparentBG_LayoutElement.preferredWidth = width - m_DragHandleWidth;
    }

    private void Start()
    {
        // Set scroll to closed position
        m_ScrollRect.horizontalNormalizedPosition = 0;
    }

    private void ChangeState(State newState)
    {
        m_TransparentImage.raycastTarget = false;
        switch (newState)
        {
            case State.Dragging:
                m_FastSwipeTimer = 0.0f;
                m_DragStartXPos = Input.mousePosition.x;
                break;
            case State.Open:
                m_TransparentImage.raycastTarget = true;
                break;
            case State.Closed:
                m_TransparentImage.raycastTarget = false;
                break;
        }

        m_CurrentState = newState;
    }

    private void Update()
    {
        switch (m_CurrentState)
        {
            case State.Dragging:
                StateUpdate_Dragging();
                break;
            case State.Opening:
                StateUpdate_Opening();
                break;
            case State.Closing:
                StateUpdate_Closing();
                break;
        }

        if (m_AccountForScreenChanges)
        {
            if (m_LastScreenSize != new Vector2(Screen.width, Screen.height))
            {
                m_LastScreenSize = new Vector2(Screen.width, Screen.height);

                SetupPanelSizes();
            }
        }

        m_DragHandleImage.color = Color.Lerp(Color.clear, m_OverlayColor, 1 - m_ScrollRect.horizontalNormalizedPosition);
        m_TransparentImage.color = Color.Lerp(Color.clear, m_OverlayColor, 1 - m_ScrollRect.horizontalNormalizedPosition);
    }

    public void ToggleMenu()
    {
        switch(m_CurrentState)
        {
            case State.Open:
            case State.Opening:
                ChangeState(State.Closing);
                break;
            case State.Closed:
            case State.Closing:
                ChangeState(State.Opening);
                break;
        }

        m_CurrentLerpValue = m_ScrollRect.horizontalNormalizedPosition;
    }

    private void StateUpdate_Dragging()
    {
        m_FastSwipeTimer += Time.deltaTime;
    }

    private void StateUpdate_Opening()
    {
        m_CurrentLerpValue += m_TransitionSpeed * Time.deltaTime;
        m_ScrollRect.horizontalNormalizedPosition = m_CurrentLerpValue;
        if (m_CurrentLerpValue <= 0)
            ChangeState(State.Open);
    }

    private void StateUpdate_Closing()
    {
        m_CurrentLerpValue -= m_TransitionSpeed * Time.deltaTime;
        m_ScrollRect.horizontalNormalizedPosition = m_CurrentLerpValue;

        if (m_CurrentLerpValue >= 1.0f)
            ChangeState(State.Closed);
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        ChangeState(State.Dragging);
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (m_FastSwipeTimer < m_FastSwipeTimeThreshold)
        {
            if (Input.mousePosition.x < m_DragStartXPos)
            {
                // fast swipe left
                ChangeState(State.Closing);
            }
            else
            {
                // fast swipe right
                ChangeState(State.Opening);
            }
        }
        else
        {
            // User just let go... so do threshold check
            if (m_ScrollRect.horizontalNormalizedPosition < m_OpenThreshold)
            {
                ChangeState(State.Opening);
            }
            else
            {
                ChangeState(State.Closing);
            }
        }

        m_CurrentLerpValue = m_ScrollRect.horizontalNormalizedPosition;
    }

    private float GetScale(int width, int height, Vector2 scalerReferenceResolution, float scalerMatchWidthOrHeight)
    {
        return Mathf.Pow(width / scalerReferenceResolution.x, 1f - scalerMatchWidthOrHeight) *
               Mathf.Pow(height / scalerReferenceResolution.y, scalerMatchWidthOrHeight);
    }
}

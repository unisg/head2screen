
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class DragRegisterVertical : MonoBehaviour, IBeginDragHandler, IEndDragHandler
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
    [SerializeField] bool startOpen = false;
    [SerializeField] bool deactivateOnClose = false;
   

    [SerializeField] CanvasScaler m_CanvasScalar;

    [SerializeField] ScrollRect m_ScrollRect;

    [SerializeField] LayoutElement m_Menu_LayoutElement;
    [SerializeField] LayoutElement m_DragHandle_LayoutElement;
    [SerializeField] LayoutElement m_TransparentBG_LayoutElement;

    [SerializeField] Image m_TransparentImage;

    [SerializeField] float m_DragHandleHeight = 200f;
    private float m_MenuSizeHeight = 400f;
    [SerializeField] float m_TransitionSpeed = 5f;
    [Range(0.0f, 1f)]
    //[SerializeField] float m_OpenThreshold = .33f;

    [SerializeField] bool m_AccountForScreenChanges = true;

    [Range(0.0f, 1f)]
    [SerializeField] float m_OpenDistance = .65f;

    private float m_CurrentLerpValue = 0.0f;

    [SerializeField] float m_FastSwipeTimeThreshold = 0.33f;
    private float m_DragStartYPos = 0.0f;
    private float m_FastSwipeTimer = 0.0f;

    private Vector2 m_LastScreenSize = Vector2.zero;

    private void Awake()
    {
        m_MenuSizeHeight = m_Menu_LayoutElement.preferredHeight;

        SetupPanelSizes();

        ChangeState(State.Closed);

    }

    private void SetupPanelSizes()
    {
        m_Menu_LayoutElement.preferredHeight = m_MenuSizeHeight;
        m_DragHandle_LayoutElement.preferredHeight = m_DragHandleHeight;

        // account for scaling
        float height = Screen.height / GetScale(Screen.width, Screen.height, m_CanvasScalar.referenceResolution, m_CanvasScalar.matchWidthOrHeight);
        m_TransparentBG_LayoutElement.preferredHeight = height - m_DragHandleHeight;
    }

    private void Start()
    {
        // Set scroll to closed position
        m_ScrollRect.verticalNormalizedPosition = 1;

    }

    private void OnEnable()
    {
        if (startOpen)
        {
            ChangeState(State.Opening);
        }
    }

    private void OnDisable()
    {
        
    }


   


    private void ChangeState(State newState)
    {
        m_TransparentImage.raycastTarget = false;
        switch (newState)
        {
            case State.Dragging:
                m_FastSwipeTimer = 0.0f;
                m_DragStartYPos = Input.mousePosition.y;
                break;
            case State.Open:
                m_TransparentImage.raycastTarget = true;
                break;
            case State.Closed:
                m_TransparentImage.raycastTarget = false;
                 if (deactivateOnClose)
                     this.gameObject.SetActive(false);

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

        m_CurrentLerpValue = m_ScrollRect.verticalNormalizedPosition;
    }

    private void StateUpdate_Dragging()
    {
        m_FastSwipeTimer += Time.deltaTime;
    }

    private void StateUpdate_Opening()
    {
        m_CurrentLerpValue -= m_TransitionSpeed * Time.deltaTime;
        m_ScrollRect.verticalNormalizedPosition = m_OpenDistance;
        if (m_CurrentLerpValue >= m_OpenDistance)
            ChangeState(State.Open);
    }

    private void StateUpdate_Closing()
    {
        m_CurrentLerpValue += m_TransitionSpeed * Time.deltaTime;
        m_ScrollRect.verticalNormalizedPosition = m_CurrentLerpValue;
        if (m_CurrentLerpValue <= 0.0f)
            ChangeState(State.Closed);
        if (deactivateOnClose)
            this.gameObject.SetActive(false);
    }

    private void DeactivateOnClose()
    {
        if (deactivateOnClose)
            this.gameObject.SetActive(false);
    }
    public void OnBeginDrag(PointerEventData eventData)
    {
        ChangeState(State.Dragging);
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (m_FastSwipeTimer < m_FastSwipeTimeThreshold)
        {
            if (Input.mousePosition.y < m_DragStartYPos)
            {
                // fast swipe left
                ChangeState(State.Closing);
            }
            else
            {
                // fast swipe right
                ChangeState(State.Opening);

              //  m_ScrollRect.verticalNormalizedPosition = 1;
            }
        }
        else
        {
            //// User just let go... so do threshold check
            //if (m_ScrollRect.verticalNormalizedPosition < m_OpenThreshold)
            //{
            //    ChangeState(State.Opening);
            //}
            //else
            //{
            //    ChangeState(State.Closing);
            //}
        }

        m_CurrentLerpValue = m_ScrollRect.verticalNormalizedPosition;
    }

    private float GetScale(int width, int height, Vector2 scalerReferenceResolution, float scalerMatchWidthOrHeight)
    {
        return Mathf.Pow(width / scalerReferenceResolution.x, scalerMatchWidthOrHeight) *
               Mathf.Pow(height / scalerReferenceResolution.y, 1f - scalerMatchWidthOrHeight);
    }
}

using UnityEditor;

/// <summary>
/// Used to simply add public property
/// </summary>
[CustomEditor(typeof(NestedScrollRect))]
public class NestedScrollRectEditor : Editor
{
    private NestedScrollRect m_SiblingScrollRect;
    private SerializedObject m_GetTarget;

    private void OnEnable()
    {
        m_SiblingScrollRect = (NestedScrollRect)target;
        m_GetTarget = new SerializedObject(m_SiblingScrollRect);
    }

    public override void OnInspectorGUI()
    {
        m_GetTarget.Update();

        base.OnInspectorGUI();
    }
}

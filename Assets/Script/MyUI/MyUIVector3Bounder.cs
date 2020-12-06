using UnityEngine;
using System.Collections;
using UnityEngine.Events;

public class MyUIVector3Bounder : MonoBehaviour
{
    [System.Serializable]
    public class ResultEvent : UnityEvent<Vector3> { }

    [SerializeField]
    private bool useXBounding;
    [SerializeField]
    private float minX;
    [SerializeField]
    private float maxX;
    [SerializeField]
    private bool useYBounding;
    [SerializeField]
    private float minY;
    [SerializeField]
    private float maxY;
    [SerializeField]
    private Vector3 aditionalOffset;
    [SerializeField]
    private ResultEvent resurtEvent;

    public Bounds bounds { get { return new Bounds(new Vector3((maxX + minX) * 0.5f, (maxY + minY) * 0.5f), new Vector3(maxX - minX, maxY - minY)); } }

    public void Evaluate(Vector3 vector)
    {
        vector += aditionalOffset;
        if(useXBounding)
            vector.x = Mathf.Clamp(vector.x, minX, maxX);
        if(useYBounding)
            vector.y = Mathf.Clamp(vector.y, minY, maxY);
        resurtEvent.Invoke(vector);
    }
}

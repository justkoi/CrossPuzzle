using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SizeUpDown : MonoBehaviour
{
    public float additionalScale = 0.1f;
    [Range(0.001f, 5)]
    public float speed = 1;

    private Vector3 newSize;
    private Vector3 _orgSize;
    float deltaValue = 0;
    public float baseSpeed = 5;
    private float mark = 1;
    void OnEnable()
    {
        _orgSize = transform.localScale;
        newSize = _orgSize;
        deltaValue = 0;
    }

    void OnDisable()
    {
        transform.localScale = _orgSize;
    }

    // Update is called once per frame
    void Update()
    {
        deltaValue += (baseSpeed * Time.deltaTime) * mark;

        if (deltaValue > 2 * additionalScale || deltaValue < 0)
        {
            mark *= -1;
        }

        var newScale = (deltaValue - additionalScale);


        newSize.x = _orgSize.x + newScale;
        newSize.y = _orgSize.y + newScale;
        newSize.z = 1.0f;
        transform.localScale = newSize;

    }
}

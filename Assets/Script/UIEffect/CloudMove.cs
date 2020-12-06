using UnityEngine;
using System.Collections;

namespace UI.Animation
{
    public class CloudMove : MonoBehaviour
    {
        public float moveRange = 1;
        [Range(0.001f, 5)]
        public float speed = 1;

        private Vector3 newPos;
        private Vector3 _orgPos;
        float deltaValue = 0;
        public float baseSpeed = 5;
        void OnEnable()
        {
            _orgPos = transform.localPosition;
            newPos = _orgPos;
            deltaValue = 0;
        }

        void OnDisable()
        {
            transform.SetLocalPositionXY(_orgPos.x, _orgPos.y);
        }

        void Update()
        {
            deltaValue += baseSpeed * Time.deltaTime;

            newPos.y = _orgPos.y + Mathf.Sin(deltaValue) * moveRange;

            transform.SetLocalPositionXY(newPos.x, newPos.y);

        }
    }
}



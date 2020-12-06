using UnityEngine;
using System.Collections;
using UnityEngine.Events;

namespace MyUI
{
    public class MyUIBounds : MonoBehaviour
    {
        [System.Serializable]
        public class BoundsSizeEvent : UnityEvent<Vector2> { }

        public enum BoundsType { Fixed, Flexible }
        public enum AlignMode { Left, Middle, Right }
        public enum AnchorType { TopLeft, TopCenter, TopRight, CenterLeft, Center, CenterRight, BottomLeft, BottomCenter, BottomRight }
        
        public BoundsType alignDirLengthType;
        public BoundsType perDirLengthType;
        public float alignDirLength;
        public float perDirLength;
        public AlignMode alignMode;
        public AnchorType anchorType;
        public bool useCustomTextMeshBound;

        public BoundsSizeEvent onBoundsSizeChanged;

        public float CurHeight { get; private set; }
        public float CurWidth { get; private set; }

        // OnBoundsSettingChanged -> 부모 레이아웃이 들음

        public void SetBounds(Vector3 topLeftPos, Direction alignLineDir, float alignDirLength, float layoutWidth)
        {
            Vector3 alingOffset;
            float newHeight = 0f;
            float newWidth = 0f;
            bool isChanged = false;

            if (alignLineDir == Direction.Right)
            {
                newHeight = perDirLengthType == BoundsType.Fixed ? perDirLength : layoutWidth;
                newWidth = alignDirLength;

                alingOffset = Vector3.down * ((layoutWidth - newHeight) * (2 - (int)alignMode) * 0.5f);
            }
            else
            {
                newHeight = alignDirLength;
                newWidth = perDirLengthType == BoundsType.Fixed ? perDirLength : layoutWidth;

                alingOffset = Vector3.right * ((layoutWidth - newWidth) * (int)alignMode * 0.5f);
            }

            if(newHeight != CurHeight)
            {
                CurHeight = newHeight;
                isChanged = true;
            }
            
            if(newWidth != CurWidth)
            {
                CurWidth = newWidth;
                isChanged = true;
            }

            Vector3 offset = Vector3.right * (((int)anchorType % 3) * CurWidth * 0.5f) + Vector3.down * (((int)anchorType / 3) * CurHeight * 0.5f) + alingOffset;
            transform.localPosition = topLeftPos + offset;

            if(isChanged)
                onBoundsSizeChanged.Invoke(new Vector2(CurWidth * 7.5f, CurHeight * 7.5f));
        }

#if UNITY_EDITOR

        private void OnDrawGizmosSelected()
        {
            var oldColor = Gizmos.color;
            var oldMatrix = Gizmos.matrix;

            Gizmos.matrix = transform.localToWorldMatrix;

            Vector3 xOffset = Vector3.right * (((int)anchorType % 3) - 1) * CurWidth * 0.5f;
            Vector3 yOffset = Vector3.down * (((int)anchorType / 3) - 1) * CurHeight * 0.5f;

            Matrix4x4 m = Matrix4x4.TRS((xOffset + yOffset) * -1, Quaternion.identity, Vector3.one);
            Gizmos.matrix = Gizmos.matrix * m;

            Gizmos.color = Color.blue;

            Vector3 right = Vector3.right * CurWidth * 0.5f;
            Vector3 up = Vector3.up * CurHeight * 0.5f;

            Gizmos.DrawLine(-right + up, right + up);
            Gizmos.DrawLine(-right - up, right - up);
            Gizmos.DrawLine(right + up, right - up);
            Gizmos.DrawLine(-right + up, -right - up);

            Gizmos.matrix = oldMatrix;
            Gizmos.color = oldColor;
        }
#endif

    }
}
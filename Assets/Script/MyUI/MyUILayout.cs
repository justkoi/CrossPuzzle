using UnityEngine;
using System.Collections;
using UnityEngine.Events;
using System.Collections.Generic;

namespace MyUI
{
    public enum Direction { Right, Down }
}

namespace MyUI
{
    public class MyUILayout : MonoBehaviour
    {
        [System.Serializable]
        public class LayoutSizeEvent : UnityEvent<Rect> { }

        public enum AlignLinePivot { Start, Middle, End }
        public enum AlignLineLengthType { Finite, Infinite }
        public enum AlignMode { Start, Middle, End }

        // 유니티 에디터를 이용해서 강제할 것
        // Infinite : Start -> Start
        // Infinite : Middle -> Middle
        // Infinite : End -> End
        // Finite : all

        // WrappingBounds 설정이 되어있으면
        // 조건 : MyUIBounds 가 같은 오브젝트에 있다.

        public Direction lineDir;
        public AlignLineLengthType lineLengthType;
        public AlignLinePivot linePivot;
        public AlignMode alignMode;
        public float lineLength;
        public float layoutWidth;
        public float space;

        // Direction Vector
        //private readonly Vector3[] dir = new Vector3[2] { Vector3.right, Vector3.down };
        private List<MyUIBounds> childBounds = new List<MyUIBounds>();

        // child 의 수가, alignDirWidth 가 변할 때 마다 변경되는 상태값
        private int flexibleChildCount = 0;
        private float fixedUsingSpace = 0f;
        private float flexibleMinUsingSpace = 0f;
        private float totalEmptySpace = 0f;
        private float minRequiredCapacity = 0f;

        public float CurContentsLength { get { return minRequiredCapacity; } }

        public LayoutSizeEvent onLayoutRefreshed = new LayoutSizeEvent();

        public void Refresh()
        {
            childBounds.Clear();

            foreach (Transform each in transform)
            {
                if (each.gameObject.activeSelf == false)
                    continue;

                var bounds = each.GetComponent<MyUIBounds>();

                if (bounds != null && bounds.enabled)
                    childBounds.Add(bounds);
            }

            CaculateLayoutData();
            ReAlignChildren();
        }

        public void CaculateLayoutData()
        {
            flexibleChildCount = 0;
            fixedUsingSpace = 0f;
            flexibleMinUsingSpace = 0f;
            totalEmptySpace = 0f;
            minRequiredCapacity = 0f;

            if (childBounds.Count > 0)
                totalEmptySpace = (childBounds.Count - 1) * space;

            for (int i = 0; i < childBounds.Count; i++)
            {
                var each = childBounds[i];

                if (each.alignDirLengthType == MyUIBounds.BoundsType.Flexible)
                {
                    flexibleChildCount++;
                    flexibleMinUsingSpace += each.alignDirLength;
                }
                else
                {
                    fixedUsingSpace += each.alignDirLength;
                }
            }

            minRequiredCapacity = fixedUsingSpace + totalEmptySpace + flexibleMinUsingSpace;
        }

        public void ReAlignChildren()
        {
            bool alignAsInfiniteLine = lineLengthType == AlignLineLengthType.Infinite;
            AlignMode alignModeToUse = alignMode;

            Vector3 dir = lineDir == Direction.Right ? Vector3.right : Vector3.down;
            Vector3 topLeftOffset = (lineDir == Direction.Right ? Vector3.up : Vector3.left) * (layoutWidth * 0.5f);

            if (lineLengthType == AlignLineLengthType.Finite && lineLength < minRequiredCapacity)
            {
                Debug.LogError("레이아웃의 길이를 초과했습니다.");
                alignAsInfiniteLine = true;
                alignModeToUse = (AlignMode)linePivot;
            }

            Vector3 bottomLeftPos;
            float height = 0f;
            float width = 0f;

            if (lineDir == Direction.Right)
                height = layoutWidth;
            else if (lineDir == Direction.Down)
                width = layoutWidth;

            if (alignAsInfiniteLine)
            {
                // 어떻게 정렬하냐에 따라 시작 위치를 달리합니다. Vector 연산을 줄여 연산을 살짝 빠르게 합니다.
                // 방향을 기준으로 LeftTop 을 사용.
                Vector3 pivot = topLeftOffset + dir * (minRequiredCapacity * (int)alignModeToUse * -0.5f);
                bottomLeftPos = pivot;
                bottomLeftPos.y *= -1;

                for (int i = 0; i < childBounds.Count; i++)
                {
                    var each = childBounds[i];
                    float alignSize = each.alignDirLength;

                    each.SetBounds(pivot, lineDir, alignSize, layoutWidth);
                    pivot += dir * (alignSize + space);
                }

                if (lineDir == Direction.Right)
                    width = minRequiredCapacity;
                else if (lineDir == Direction.Down)
                    height = minRequiredCapacity;
            }
            else
            {
                float spaceForFlexibles = lineLength - fixedUsingSpace - totalEmptySpace;
                float alignSizeOfFlexibles = flexibleChildCount > 0 ? spaceForFlexibles / flexibleChildCount : 0;
                float totalUsingSpace = flexibleChildCount > 0 ? lineLength : minRequiredCapacity;

                Vector3 pivot = topLeftOffset + dir * ((int)alignModeToUse * (-totalUsingSpace + lineLength) * 0.5f - (int)linePivot * lineLength * 0.5f);
                bottomLeftPos = pivot;
                bottomLeftPos.y *= -1;

                for (int i = 0; i < childBounds.Count; i++)
                {
                    var each = childBounds[i];
                    float alignSize;

                    if (each.alignDirLengthType == MyUIBounds.BoundsType.Flexible)
                        alignSize = alignSizeOfFlexibles;
                    else
                        alignSize = each.alignDirLength;

                    each.SetBounds(pivot, lineDir, alignSize, layoutWidth);
                    pivot += dir * (alignSize + space);
                }

                if (lineDir == Direction.Right)
                    width = totalUsingSpace;
                else if (lineDir == Direction.Down)
                    height = totalUsingSpace;
            }
            
            onLayoutRefreshed.Invoke(new Rect(transform.localToWorldMatrix.MultiplyPoint(bottomLeftPos), new Vector2(width, height)));
        }

#if UNITY_EDITOR
        private void OnDrawGizmosSelected()
        {
            var oldColor = Gizmos.color;
            var oldMatrix = Gizmos.matrix;

            if (lineDir == Direction.Right)
                Gizmos.matrix = transform.localToWorldMatrix;
            else
            {
                Matrix4x4 rot = Matrix4x4.TRS(Vector3.zero, Quaternion.Euler(0, 0, -90), Vector3.one);
                Gizmos.matrix = transform.localToWorldMatrix * rot;
            }

            Gizmos.color = Color.red;

            float width = lineLengthType == AlignLineLengthType.Finite ? lineLength : 30f;
            Vector3 right = Vector3.right * width;

            Vector3 up = Vector3.up * layoutWidth * 0.5f;

            Gizmos.DrawLine(up, -up);

            Matrix4x4 trans = Matrix4x4.TRS(Vector3.left * (int)linePivot * 0.5f * width, Quaternion.identity, Vector3.one);
            Gizmos.matrix = Gizmos.matrix * trans;

            if (lineLengthType == AlignLineLengthType.Finite)
            {
                Gizmos.DrawLine(up, up + right);
                Gizmos.DrawLine(-up, -up + right);

                Gizmos.DrawLine(up, -up);
                Gizmos.DrawLine(up + right, -up + right);
            }
            else
            {
                Gizmos.DrawLine(up, up + right);
                Gizmos.DrawLine(-up, -up + right);

                DrawArrow(up + right, Vector3.right);
                DrawArrow(-up + right, Vector3.right);
                DrawArrow(up, Vector3.left);
                DrawArrow(-up, Vector3.left);
            }

            Gizmos.color = Color.blue;

            float alignBoxWidth = width / 5f;
            Vector3 alignBoxCenterPos = Vector3.right * ((width - alignBoxWidth) * (int)alignMode * 0.5f + alignBoxWidth * 0.5f);

            Vector3 quaterHeightV = Vector3.up * layoutWidth * 0.25f;
            Vector3 leftCenter = alignBoxCenterPos + Vector3.left * alignBoxWidth * 0.5f;
            Vector3 rightCenter = alignBoxCenterPos + Vector3.right * alignBoxWidth * 0.5f;

            Gizmos.DrawLine(leftCenter + quaterHeightV, rightCenter + quaterHeightV);
            Gizmos.DrawLine(leftCenter - quaterHeightV, rightCenter - quaterHeightV);
            Gizmos.DrawLine(leftCenter + quaterHeightV, leftCenter - quaterHeightV);
            Gizmos.DrawLine(rightCenter - quaterHeightV, rightCenter + quaterHeightV);

            Gizmos.DrawLine(leftCenter - quaterHeightV, rightCenter);
            Gizmos.DrawLine(leftCenter + quaterHeightV, rightCenter);

            Gizmos.color = Color.green;

            Gizmos.DrawLine(up + right * 0.5f + Vector3.left * space * 0.5f, -up + right * 0.5f + Vector3.left * space * 0.5f);
            Gizmos.DrawLine(up + right * 0.5f + Vector3.right * space * 0.5f, -up + right * 0.5f + Vector3.right * space * 0.5f);

            Gizmos.matrix = oldMatrix;
            Gizmos.color = oldColor;
        }

        private void DrawArrow(Vector3 pos, Vector3 dir)
        {
            Gizmos.DrawLine(pos, pos - (dir + Vector3.up));
            Gizmos.DrawLine(pos, pos - (dir - Vector3.up));
        }
#endif
    }
}
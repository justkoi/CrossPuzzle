using UnityEngine;

namespace MyUI
{
    public class MyUILayoutEventListener : MonoBehaviour
    {
        [SerializeField]
        private Vector2 rectPadding;
        [SerializeField]
        MyUIBounds.AnchorType setPosAnchor;

        private static Rect GetRectWithPadding(Rect rect, Vector2 padding)
        {
            Vector2 pos = rect.position;
            pos.x -= padding.x;
            pos.y -= padding.y;
            float width = rect.width + padding.x * 2;
            float height = rect.height + padding.y * 2;
            return new Rect(pos, new Vector2(width, height));
        }

        public void SetSpriteDimmension(Rect rect)
        {
            rect = GetRectWithPadding(rect, rectPadding);
            tk2dSlicedSprite sprite = GetComponent<tk2dSlicedSprite>();

            if (sprite == null)
                return;

            float z = transform.position.z;
            transform.position = new Vector3(rect.center.x, rect.center.y, z);
            sprite.dimensions = new Vector2(rect.width * Constants.UnityMeterInPixels_UI, rect.height * Constants.UnityMeterInPixels_UI);
        }

        public void SetPos(Rect rect)
        {
            rect = GetRectWithPadding(rect, rectPadding);
            Vector2 pos = rect.position;
            Vector2 offset = Vector2.right * (((int)setPosAnchor % 3) * rect.width * 0.5f) + Vector2.up * ((2 - (int)setPosAnchor / 3) * rect.height * 0.5f);
            pos = pos + offset;
            float z = transform.position.z;
            transform.position = new Vector3(pos.x, pos.y, z);
        }

        public void SetSpriteDimmension(Vector2 size)
        {
            size += rectPadding * 2 * Constants.UnityMeterInPixels_UI;
            tk2dSlicedSprite sprite = GetComponent<tk2dSlicedSprite>();

            if (sprite == null)
                return;

            float z = transform.position.z;
            sprite.dimensions = new Vector2(size.x, size.y);
        }

        public void SetMyUIBounds(Rect rect)
        {
            rect = GetRectWithPadding(rect, rectPadding);

            MyUIBounds bounds = GetComponent<MyUIBounds>();

            if (bounds == null)
                return;

            MyUILayout layout = bounds.transform.parent != null ? bounds.transform.parent.GetComponent<MyUILayout>() : null;

            if (layout == null)
                return;
            
            if (layout.lineDir == Direction.Right)
            {
                bounds.alignDirLength = rect.width;
                bounds.perDirLength = rect.height;
            }
            else
            {
                bounds.alignDirLength = rect.height;
                bounds.perDirLength = rect.width;
            }

            layout.Refresh();
        }
    }
}
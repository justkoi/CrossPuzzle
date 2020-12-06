using UnityEngine;
using System.Collections;

public class MyUIAnchoredSprite : tk2dSprite {

    [SerializeField]
    private Anchor _anchor = Anchor.MiddleCenter;
    public Anchor anchor
    {
        get { return _anchor; }
        set
        {
            if(_anchor != value)
            {
                _anchor = value;
                UpdateVertices();
            }
        }
    }

    protected Matrix4x4 verticesMatrix
    {
        get
        {
            if (collectionInst == null || spriteId < 0 || spriteId >= collectionInst.spriteDefinitions.Length)
                return Matrix4x4.identity;

            var sprite = collectionInst.spriteDefinitions[spriteId];
            int xDir = (int)anchor % 3 - 1;
            int yDir = (int)anchor / 3 - 1;
            xDir *= -1;
            yDir *= -1;
            float width = (sprite.positions[1].x - sprite.positions[0].x) * scale.x;
            float height = (sprite.positions[2].y - sprite.positions[0].y) * scale.y;
            float dx = xDir * width * 0.5f;
            float dy = yDir * height * 0.5f;

            return Matrix4x4.TRS(new Vector3(dx, dy, 0), Quaternion.identity, Vector3.one);
        }
    }
}

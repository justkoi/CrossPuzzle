using UnityEngine;
using System.Collections;

public class MyUIProgressBar : MonoBehaviour
{
    public tk2dSlicedSprite slicedSpriteBar;
    public tk2dTiledSprite spriteBarPattern;
    [SerializeField]
    private float value = 0;
    [SerializeField]
    protected Vector2 emptyDimension;
    [SerializeField]
    protected Vector2 fullDimension;

    private bool initedBorder = false;
    private float baseBorderLeft = 0;
    private float baseBorderRight = 0;
    private float baseBorderTop = 0;
    private float baseBorderBottom = 0;

    public void Awake()
    {
        if (!initedBorder)
        {
            initedBorder = true;
            if (slicedSpriteBar != null)
            {
                baseBorderLeft = slicedSpriteBar.borderLeft;
                baseBorderBottom = slicedSpriteBar.borderBottom;
                baseBorderRight = slicedSpriteBar.borderRight;
                baseBorderTop = slicedSpriteBar.borderTop;
            }
        }
    }

    public void Start()
    {
        Value = value;
    }
    /// <summary>
    /// Percent complete, between 0-1
    /// </summary>
    public virtual float Value
    {
        get { return value; }
        set
        {
            this.value = Mathf.Clamp(value, 0f, 1f);
            float slicedLength = Mathf.Lerp(0, fullDimension.x, this.value);
            if (Mathf.Approximately(slicedLength, 0f))
            {
                slicedSpriteBar.dimensions = new Vector2(0, 0);
                slicedSpriteBar.SetBorder(0, 0, 0, 0);
            }
            else
            {
                slicedSpriteBar.dimensions = new Vector2(Mathf.Max(slicedLength, emptyDimension.x), fullDimension.y);
                slicedSpriteBar.SetBorder(baseBorderLeft, baseBorderBottom, baseBorderRight, baseBorderTop);
            }
            if (spriteBarPattern != null)
            {
                float tileX = Mathf.Clamp(slicedSpriteBar.dimensions.x - emptyDimension.x / 2, 0,
                    slicedSpriteBar.dimensions.x);
                spriteBarPattern.dimensions = new Vector2(tileX, spriteBarPattern.dimensions.y);
            }
#if UNITY_EDITOR
            if(!Application.isPlaying)
                slicedSpriteBar.ForceBuild();
#endif
        }
    }

    public void SaveNowDimension()
    {
        tk2dSpriteDefinition spriteDef = slicedSpriteBar.CurrentSprite;
        Vector3 extents = spriteDef.boundsData[1];
        emptyDimension = new Vector2((slicedSpriteBar.borderLeft + slicedSpriteBar.borderRight) * extents.x / spriteDef.texelSize.x, slicedSpriteBar.dimensions.y);
        fullDimension = slicedSpriteBar.dimensions;
    }

#if UNITY_EDITOR
    private void OnDrawGizmosSelected()
    {
        if (slicedSpriteBar == null)
            return;

        tk2dSpriteDefinition spriteDef = slicedSpriteBar.CurrentSprite;

        if (spriteDef == null)
            return;

        var oldMatrix = Gizmos.matrix;
        var oldColor = Gizmos.color;
        
        var tempEmpty = emptyDimension;
        tempEmpty.Scale(slicedSpriteBar.scale);
        var tempFull = fullDimension;
        tempFull.Scale(slicedSpriteBar.scale);
        DrawDimension(tempEmpty, Color.blue);
        DrawDimension(tempFull, Color.red);

        Gizmos.color = oldColor;
        Gizmos.matrix = oldMatrix;
    }

    private void DrawDimension(Vector2 dim, Color color)
    {
        tk2dSpriteDefinition spriteDef = slicedSpriteBar.CurrentSprite;

        float xSize = dim.x * spriteDef.texelSize.x;
        float ySize = dim.y * spriteDef.texelSize.y;

        int anchorValue = (int)slicedSpriteBar.anchor;
        float xOffset = (anchorValue % 3) * 0.5f * xSize * -1;
        float yOffset = (2 - anchorValue / 3) * 0.5f * ySize;

        Gizmos.matrix = Matrix4x4.TRS(new Vector3(xOffset, yOffset, 0f), Quaternion.identity, Vector3.one) * slicedSpriteBar.transform.localToWorldMatrix;

        Gizmos.color = color;
        Vector3 right = Vector3.right * xSize;
        Vector3 down = Vector3.down * ySize;

        Gizmos.DrawLine(Vector3.zero, right);
        Gizmos.DrawLine(right, right + down);
        Gizmos.DrawLine(right + down, down);
        Gizmos.DrawLine(down, Vector3.zero);
    }
#endif
}

using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;
using System.Text;
using Global.Constants;

/// <summary>
/// UI에서 공통적으로 사용하는 표준 버튼입니다.
/// Style 변환기능
/// Block 옵션기능
/// TODO : Dimention 및 Bolder 에 관한 DetailStyle 추가기능
/// TODO : Text 추가기능
/// TODO : Selection 추가기능
/// TODO : SlotPoolManager기능
/// Assets\Editor\CommonEditor 에디터를 사용하여 스타일 및 블록 여부를 조작할 수 있습니다.
/// </summary>

/// TODO : 버튼 스타일과 텍스트 스타일의 분리 필요.



/// <summary>
/// 커먼 버튼 스타일 정보 (추가할시 기존 번호를 변경하지 말아야합니다)
/// </summary>
public enum E_COMMON_BUTTON_STYLE
{
    COMMON_STAGE_EASY = 0,
    COMMON_STAGE_NORMAL = 1,
    COMMON_STAGE_HARD = 2,
    COMMON_NONE = 999999,
}
class E_COMMON_BUTTON_STYLEComparer : System.Collections.Generic.IEqualityComparer<E_COMMON_BUTTON_STYLE>
{
    bool System.Collections.Generic.IEqualityComparer<E_COMMON_BUTTON_STYLE>.Equals(E_COMMON_BUTTON_STYLE x, E_COMMON_BUTTON_STYLE y)
    {
        return (int)x == (int)y;
    }

    int System.Collections.Generic.IEqualityComparer<E_COMMON_BUTTON_STYLE>.GetHashCode(E_COMMON_BUTTON_STYLE obj)
    {
        return ((int)obj).GetHashCode();
    }
}


//!< 콜렉션 바꾸는 기능 아직 추가 안함
//!< 후에 필요하다면 추가 예정
/// <summary>
/// 커먼 버튼 스타일 정보
/// </summary>

[SerializeField]
public class CommonButtonStyle
{
    /// <summary>
    /// 스타일
    /// </summary>
    public E_COMMON_BUTTON_STYLE m_eStyle;

    /// <summary>
    /// 기본 스프라이트
    /// </summary>
    public string m_strBaseSpriteName;

    /// <summary>
    /// 다운 스프라이트
    /// </summary>
    public string m_strDownSpriteName;

    /// <summary>
    /// 블록 상태 스프라이트
    /// </summary>
    public string m_strBlockSpriteName;

    /// <summary>
    /// 해당 스타일의 local위치 변경값
    /// </summary>
    public Vector3 m_vSpritePos;

    /// <summary>
    /// 기본 칼라 값
    /// </summary>
    public Color m_cBaseColor;

    public bool m_hideTextOnBlock;


    /// <summary>
    /// 블록 옵션의 종류
    /// </summary>
    public E_COMMON_BUTTON_BLOCK_OPTION m_eBlockOption;

    static private Dictionary<E_COMMON_BUTTON_STYLE, CommonButtonStyle> m_CommonButtonStyleList = null;
    /*
    public E_COMMON_TEXTLAYOUT_STYLE m_eBaseTextStyle;
    public E_COMMON_TEXTLAYOUT_STYLE m_eBlockTextStyle;

    */
    public CommonButtonStyle(E_COMMON_BUTTON_STYLE eStyle, string strBaseSpriteName, string strDownSpriteName, string strBlockSpriteName, Vector3 vSpritePos, E_COMMON_BUTTON_BLOCK_OPTION eBlockOption, bool hideTextOnBlock)//, E_COMMON_TEXTLAYOUT_STYLE eBaseTextStyle, E_COMMON_TEXTLAYOUT_STYLE eBlockTextStyle)
    {
        m_eStyle = eStyle;
        m_strDownSpriteName = strDownSpriteName;
        m_strBaseSpriteName = strBaseSpriteName;
        m_strBlockSpriteName = strBlockSpriteName;
        m_vSpritePos = vSpritePos;
        m_eBlockOption = eBlockOption;
        m_hideTextOnBlock = hideTextOnBlock;//블록시 텍스트 숨김
        //m_eBaseTextStyle = eBaseTextStyle;
        //m_eBlockTextStyle = eBlockTextStyle;
    }


    /// <summary>
    /// 해당 Enum에 맞는 버튼 스타일을 가져옵니다.
    /// </summary>
    /// <param name="eStyle"></param>
    /// <returns></returns>
    static public CommonButtonStyle GetStyle(E_COMMON_BUTTON_STYLE eStyle)
    {

        if (m_CommonButtonStyleList == null)
            ReadStyle();

        CommonButtonStyle eTempStyle = null;

        if (m_CommonButtonStyleList.ContainsKey(eStyle))
            eTempStyle = m_CommonButtonStyleList[eStyle];
        else
        {
            Debugs.Log("해당 스타일은 존재하지 않는 스타일입니다. 0번 스타일로 대체됩니다");
            eTempStyle = m_CommonButtonStyleList[E_COMMON_BUTTON_STYLE.COMMON_STAGE_EASY]; ///해당 스타일이 없다면 0번째 스타일을 가져옵니다.
        }

        return eTempStyle;
    }
    /// <summary>
    /// 이곳에서 스타일을 정의 합니다. (후에 Xml파일이나 기타 파일DB에서 읽어올때 이곳에서 지정)
    /// </summary>
    static private void ReadStyle()
    {
        m_CommonButtonStyleList = new Dictionary<E_COMMON_BUTTON_STYLE, CommonButtonStyle>(new E_COMMON_BUTTON_STYLEComparer());
        m_CommonButtonStyleList.Add(E_COMMON_BUTTON_STYLE.COMMON_STAGE_EASY, new CommonButtonStyle(E_COMMON_BUTTON_STYLE.COMMON_STAGE_EASY, "Button_SelectMenu_1_1", "Button_SelectMenu_3_1", "Lock",Vector3.zero, E_COMMON_BUTTON_BLOCK_OPTION.SPRITE, true)); //E_COMMON_TEXTLAYOUT_STYLE.NONE, E_COMMON_TEXTLAYOUT_STYLE.NONE));
        m_CommonButtonStyleList.Add(E_COMMON_BUTTON_STYLE.COMMON_STAGE_NORMAL, new CommonButtonStyle(E_COMMON_BUTTON_STYLE.COMMON_STAGE_NORMAL, "Button_SelectMenu_1_2", "Button_SelectMenu_3_2", "Lock", Vector3.zero, E_COMMON_BUTTON_BLOCK_OPTION.SPRITE, true)); //E_COMMON_TEXTLAYOUT_STYLE.NONE, E_COMMON_TEXTLAYOUT_STYLE.NONE));
        m_CommonButtonStyleList.Add(E_COMMON_BUTTON_STYLE.COMMON_STAGE_HARD, new CommonButtonStyle(E_COMMON_BUTTON_STYLE.COMMON_STAGE_HARD, "Button_SelectMenu_1_3", "Button_SelectMenu_3_3", "Lock", Vector3.zero, E_COMMON_BUTTON_BLOCK_OPTION.SPRITE, true)); //E_COMMON_TEXTLAYOUT_STYLE.NONE, E_COMMON_TEXTLAYOUT_STYLE.NONE));

        /*
        m_CommonButtonStyleList.Add(E_COMMON_BUTTON_STYLE.COMMON_YES, new CommonButtonStyle(E_COMMON_BUTTON_STYLE.COMMON_YES, "c_btn_6", "c_btn_7", Vector3.zero, E_COMMON_BUTTON_BLOCK_OPTION.SPRITE, E_COMMON_TEXTLAYOUT_STYLE.NONE, E_COMMON_TEXTLAYOUT_STYLE.NONE));
        m_CommonButtonStyleList.Add(E_COMMON_BUTTON_STYLE.COMMON_NO, new CommonButtonStyle(E_COMMON_BUTTON_STYLE.COMMON_NO, "c_btn_7", "c_btn_7", Vector3.zero, E_COMMON_BUTTON_BLOCK_OPTION.SPRITE, E_COMMON_TEXTLAYOUT_STYLE.NONE, E_COMMON_TEXTLAYOUT_STYLE.NONE));
        m_CommonButtonStyleList.Add(E_COMMON_BUTTON_STYLE.COMMON_CLOSE, new CommonButtonStyle(E_COMMON_BUTTON_STYLE.COMMON_CLOSE, "c_icon_25", string.Empty, Vector3.zero, E_COMMON_BUTTON_BLOCK_OPTION.NONE, E_COMMON_TEXTLAYOUT_STYLE.NONE, E_COMMON_TEXTLAYOUT_STYLE.NONE));
        m_CommonButtonStyleList.Add(E_COMMON_BUTTON_STYLE.COMMON_TAP, new CommonButtonStyle(E_COMMON_BUTTON_STYLE.COMMON_NO, "Window_1_0_Tab", "Window_1_1_Tab", Vector3.zero, E_COMMON_BUTTON_BLOCK_OPTION.SPRITE, E_COMMON_TEXTLAYOUT_STYLE.NONE, E_COMMON_TEXTLAYOUT_STYLE.NONE));
        m_CommonButtonStyleList.Add(E_COMMON_BUTTON_STYLE.COMMON_TITLE, new CommonButtonStyle(E_COMMON_BUTTON_STYLE.COMMON_TITLE, "Window_2_0_Title", string.Empty, Vector3.zero, E_COMMON_BUTTON_BLOCK_OPTION.COLOR, E_COMMON_TEXTLAYOUT_STYLE.NONE, E_COMMON_TEXTLAYOUT_STYLE.NONE));
        m_CommonButtonStyleList.Add(E_COMMON_BUTTON_STYLE.COMMON_CLOSE_SMALL, new CommonButtonStyle(E_COMMON_BUTTON_STYLE.COMMON_CLOSE_SMALL, "inven_icon_15", string.Empty, Vector3.zero, E_COMMON_BUTTON_BLOCK_OPTION.COLOR, E_COMMON_TEXTLAYOUT_STYLE.NONE, E_COMMON_TEXTLAYOUT_STYLE.NONE));
        m_CommonButtonStyleList.Add(E_COMMON_BUTTON_STYLE.COMMON_ORANGE, new CommonButtonStyle(E_COMMON_BUTTON_STYLE.COMMON_ORANGE, "c_btn_5", "c_btn_7", Vector3.zero, E_COMMON_BUTTON_BLOCK_OPTION.SPRITE, E_COMMON_TEXTLAYOUT_STYLE.NONE, E_COMMON_TEXTLAYOUT_STYLE.NONE));
        m_CommonButtonStyleList.Add(E_COMMON_BUTTON_STYLE.COMMON_MINUS, new CommonButtonStyle(E_COMMON_BUTTON_STYLE.COMMON_MINUS, "c_btn_20", "c_btn_20", Vector3.zero, E_COMMON_BUTTON_BLOCK_OPTION.SPRITE, E_COMMON_TEXTLAYOUT_STYLE.NONE, E_COMMON_TEXTLAYOUT_STYLE.NONE));
        m_CommonButtonStyleList.Add(E_COMMON_BUTTON_STYLE.COMMON_PLUS, new CommonButtonStyle(E_COMMON_BUTTON_STYLE.COMMON_PLUS, "c_btn_19", "c_btn_19", Vector3.zero, E_COMMON_BUTTON_BLOCK_OPTION.SPRITE, E_COMMON_TEXTLAYOUT_STYLE.NONE, E_COMMON_TEXTLAYOUT_STYLE.NONE));
        m_CommonButtonStyleList.Add(E_COMMON_BUTTON_STYLE.COMMON_ORANGE_OUTLINE, new CommonButtonStyle(E_COMMON_BUTTON_STYLE.COMMON_ORANGE_OUTLINE, "c_btn_13", "c_btn_14", Vector3.zero, E_COMMON_BUTTON_BLOCK_OPTION.SPRITE, E_COMMON_TEXTLAYOUT_STYLE.NONE, E_COMMON_TEXTLAYOUT_STYLE.NONE));
        m_CommonButtonStyleList.Add(E_COMMON_BUTTON_STYLE.COMMON_YELLOW_OUTLINE, new CommonButtonStyle(E_COMMON_BUTTON_STYLE.COMMON_YELLOW_OUTLINE, "c_btn_12", "c_btn_14", Vector3.zero, E_COMMON_BUTTON_BLOCK_OPTION.SPRITE, E_COMMON_TEXTLAYOUT_STYLE.NONE, E_COMMON_TEXTLAYOUT_STYLE.NONE));
        m_CommonButtonStyleList.Add(E_COMMON_BUTTON_STYLE.COMMON_BLUE_OUTLINE, new CommonButtonStyle(E_COMMON_BUTTON_STYLE.COMMON_BLUE_OUTLINE, "c_window_80", "c_window_79", Vector3.zero, E_COMMON_BUTTON_BLOCK_OPTION.SPRITE, E_COMMON_TEXTLAYOUT_STYLE.COMMON_OUTLINE_BLUE, E_COMMON_TEXTLAYOUT_STYLE.COMMON_OUTLINE_BLACK));
        m_CommonButtonStyleList.Add(E_COMMON_BUTTON_STYLE.COMMON_SCROLL_THUMB, new CommonButtonStyle(E_COMMON_BUTTON_STYLE.COMMON_SCROLL_THUMB, "c_window_48", "c_window_48", Vector3.zero, E_COMMON_BUTTON_BLOCK_OPTION.SPRITE, E_COMMON_TEXTLAYOUT_STYLE.NONE, E_COMMON_TEXTLAYOUT_STYLE.NONE));
        m_CommonButtonStyleList.Add(E_COMMON_BUTTON_STYLE.COMMON_SOCIAL, new CommonButtonStyle(E_COMMON_BUTTON_STYLE.COMMON_SOCIAL, "c_window_104", "c_window_104", Vector3.zero, E_COMMON_BUTTON_BLOCK_OPTION.SPRITE, E_COMMON_TEXTLAYOUT_STYLE.NONE, E_COMMON_TEXTLAYOUT_STYLE.NONE));
        m_CommonButtonStyleList.Add(E_COMMON_BUTTON_STYLE.COMMON_RED, new CommonButtonStyle(E_COMMON_BUTTON_STYLE.COMMON_RED, "c_btn_2", "c_btn_7", Vector3.zero, E_COMMON_BUTTON_BLOCK_OPTION.SPRITE, E_COMMON_TEXTLAYOUT_STYLE.NONE, E_COMMON_TEXTLAYOUT_STYLE.NONE));
        m_CommonButtonStyleList.Add(E_COMMON_BUTTON_STYLE.COMMON_PARTY_DECLINE, new CommonButtonStyle(E_COMMON_BUTTON_STYLE.COMMON_PARTY_DECLINE, "scene_window_22", "scene_window_22", Vector3.zero, E_COMMON_BUTTON_BLOCK_OPTION.SPRITE, E_COMMON_TEXTLAYOUT_STYLE.NONE, E_COMMON_TEXTLAYOUT_STYLE.NONE));
        m_CommonButtonStyleList.Add(E_COMMON_BUTTON_STYLE.COMMON_PARTY_ACCEPT, new CommonButtonStyle(E_COMMON_BUTTON_STYLE.COMMON_PARTY_ACCEPT, "scene_window_23", "scene_window_23", Vector3.zero, E_COMMON_BUTTON_BLOCK_OPTION.SPRITE, E_COMMON_TEXTLAYOUT_STYLE.NONE, E_COMMON_TEXTLAYOUT_STYLE.NONE));
        m_CommonButtonStyleList.Add(E_COMMON_BUTTON_STYLE.COMMON_WORLDMAP_ARROW, new CommonButtonStyle(E_COMMON_BUTTON_STYLE.COMMON_WORLDMAP_ARROW, "WorldMap_Arrow", "WorldMap_Arrow", Vector3.zero, E_COMMON_BUTTON_BLOCK_OPTION.SPRITE, E_COMMON_TEXTLAYOUT_STYLE.NONE, E_COMMON_TEXTLAYOUT_STYLE.NONE));
        m_CommonButtonStyleList.Add(E_COMMON_BUTTON_STYLE.COMMON_RADIO_ON, new CommonButtonStyle(E_COMMON_BUTTON_STYLE.COMMON_RADIO_ON, "c_window_81", "c_window_81", Vector3.zero, E_COMMON_BUTTON_BLOCK_OPTION.NONE, E_COMMON_TEXTLAYOUT_STYLE.NONE, E_COMMON_TEXTLAYOUT_STYLE.NONE));
        m_CommonButtonStyleList.Add(E_COMMON_BUTTON_STYLE.COMMON_RADIO_OFF, new CommonButtonStyle(E_COMMON_BUTTON_STYLE.COMMON_RADIO_OFF, "c_window_82", "c_window_82", Vector3.zero, E_COMMON_BUTTON_BLOCK_OPTION.NONE, E_COMMON_TEXTLAYOUT_STYLE.NONE, E_COMMON_TEXTLAYOUT_STYLE.NONE));
        m_CommonButtonStyleList.Add(E_COMMON_BUTTON_STYLE.COMMON_BROWN_OUTLINE, new CommonButtonStyle(E_COMMON_BUTTON_STYLE.COMMON_BROWN_OUTLINE, "c_btn_12", "c_btn_14", Vector3.zero, E_COMMON_BUTTON_BLOCK_OPTION.SPRITE, E_COMMON_TEXTLAYOUT_STYLE.NONE, E_COMMON_TEXTLAYOUT_STYLE.NONE));
        m_CommonButtonStyleList.Add(E_COMMON_BUTTON_STYLE.COMMON_PINK_OUTLINE, new CommonButtonStyle(E_COMMON_BUTTON_STYLE.COMMON_PINK_OUTLINE, "c_window_4", "c_window_4", Vector3.zero, E_COMMON_BUTTON_BLOCK_OPTION.SPRITE, E_COMMON_TEXTLAYOUT_STYLE.NONE, E_COMMON_TEXTLAYOUT_STYLE.NONE));
        m_CommonButtonStyleList.Add(E_COMMON_BUTTON_STYLE.COMMON_RED_OUTLINE, new CommonButtonStyle(E_COMMON_BUTTON_STYLE.COMMON_RED_OUTLINE, "c_btn_2", "c_btn_2", Vector3.zero, E_COMMON_BUTTON_BLOCK_OPTION.SPRITE, E_COMMON_TEXTLAYOUT_STYLE.NONE, E_COMMON_TEXTLAYOUT_STYLE.NONE));
        m_CommonButtonStyleList.Add(E_COMMON_BUTTON_STYLE.COMMON_BROWN, new CommonButtonStyle(E_COMMON_BUTTON_STYLE.COMMON_BROWN, "c_btn_10", "c_btn_11", Vector3.zero, E_COMMON_BUTTON_BLOCK_OPTION.SPRITE, E_COMMON_TEXTLAYOUT_STYLE.NONE, E_COMMON_TEXTLAYOUT_STYLE.NONE));
        m_CommonButtonStyleList.Add(E_COMMON_BUTTON_STYLE.COMMON_PARTY_MEMBER, new CommonButtonStyle(E_COMMON_BUTTON_STYLE.COMMON_PARTY_MEMBER, "c_window_161", "c_window_151", Vector3.zero, E_COMMON_BUTTON_BLOCK_OPTION.COLOR, E_COMMON_TEXTLAYOUT_STYLE.NONE, E_COMMON_TEXTLAYOUT_STYLE.NONE));
        m_CommonButtonStyleList.Add(E_COMMON_BUTTON_STYLE.COMMON_RED_OUTLINE_2, new CommonButtonStyle(E_COMMON_BUTTON_STYLE.COMMON_RED_OUTLINE_2, "c_window_111", "c_btn_14", Vector3.zero, E_COMMON_BUTTON_BLOCK_OPTION.COLOR, E_COMMON_TEXTLAYOUT_STYLE.NONE, E_COMMON_TEXTLAYOUT_STYLE.NONE));
    */
    }

}

public enum E_COMMON_BUTTON_BLOCK_OPTION
{
    NONE = 0, // 아무런 옵션도 사용하지 않습니다.
    SPRITE = 1, // 스프라이트로 블록을 대체합니다.
    COLOR = 2, // 색으로 블록을 대체합니다. D_BLOCK_COLOR에서 어두워지는 정도를 설정합니다. (0.0f ~ 1.0f)
}



public enum E_COMMON_BUTTON_DATA_TYPE
{
    /// <summary>
    /// 콜리더만 있는 버튼
    /// </summary>
    COLLIDER,
    /// <summary>
    /// 스프라이트와 콜리더가 있는 버튼
    /// </summary>
    SPRITE,
    /// <summary>
    /// 스프라이트와 콜리더, 자체 텍스트가 전부 있는 버튼
    /// </summary>
    COMMON,
    /// <summary>
    /// 에디터용 히트박스 추가
    /// </summary>
    HITBOX,
    NONE,
}

public class CommonButtonCreateData
{
    /// <summary>
    /// 데이터 세팅중인가?
    /// </summary>
    public bool m_bDataSetting_Part;
    /// <summary>
    /// 데이터 세팅중인가?
    /// </summary>
    public bool m_bDataSetting_All;
    /// <summary>
    /// 픽셀기준
    /// </summary>
    public Vector2 m_fStartPos;
    /// <summary>
    /// 픽셀기준
    /// </summary>
    public Vector2 m_fEndPos;
    /// <summary>
    /// 유니티미터 기준
    /// </summary>
    public float m_fWidth;
    /// <summary>
    /// 유니티미터 기준
    /// </summary>
    public float m_fHeight;
    /// <summary>
    /// 유니티미터 기준
    /// </summary>
    public float m_fOffsetX;
    /// <summary>
    /// 유니티미터 기준
    /// </summary>
    public float m_fOffsetY;


    public float m_fCharSize;
    /// <summary>
    /// 이 텍스트의 라인 첫째줄이 0
    /// </summary>
    public int m_nCurrLine;

    public float m_fLineWidth;
    /// <summary>
    /// 한 줄의 높이
    /// </summary>
    public float m_fLineHeight;
    /// <summary>
    /// 총 줄수
    /// </summary>
    public int m_nMaxLine;

    public TextAnchor m_Anchor;

    /// <summary>
    /// 개행 종료 시 여백 삽입이 필요한가?
    /// </summary>
    public bool m_bNeedCustomEndSpace = false;

    private readonly float m_fCustomEndSpace = 7.0f;

    public StringBuilder m_strbData_All = new StringBuilder();
    public StringBuilder m_strbData_Part = new StringBuilder();
    /// <summary>
    /// 포함하고 있는 전체 글자 [이 글자에는 현재 여백이 제거된상태입니다. 수정예정, 잠시 미뤄놓았습니다.]
    /// </summary>
    public string m_strData_All = string.Empty;
    /// <summary>
    /// 포함하고 있는 부분 글자 (버튼이 두 줄 이상으로 나뉘어진 경우 현재 버튼) [이 글자에는 현재 여백이 제거된상태입니다. 수정예정, 잠시 미뤄놓았습니다.]
    /// </summary>
    public string m_strData_Part = string.Empty;

    public E_COMMON_BUTTON_DATA_TYPE m_eType;
    public void SetStartPos(E_COMMON_BUTTON_DATA_TYPE eType, float fX, float fY, int nCurrLine)
    {
        m_eType = eType;
        m_fStartPos = new Vector2(fX, fY);
        m_bDataSetting_Part = true;
        m_bDataSetting_All = true;
        m_nCurrLine = nCurrLine;
    }

    public void SetEndPos(float fX, float fY)
    {
        m_fEndPos = new Vector2(fX, fY);
        m_bDataSetting_Part = false;
    }

    public void SetPartDatas(char cTarget)
    {
        if (m_bDataSetting_Part)
        {
            m_strbData_Part.Append(cTarget);
            m_strbData_All.Append(cTarget);
        }
    }
    public void SetAllDatas(char cTarget)
    {
        if (m_bDataSetting_All)
        {
            m_strbData_All.Append(cTarget);
        }
    }

    public void EndDataSetting()
    {
        m_bDataSetting_All = false;
    }

    public void SetLineWidth(float fLineWidth)
    {
        m_fLineWidth = fLineWidth;
    }

    public void SetDatas(float fCharSize, TextAnchor Anchor, Vector3 vStartOffset, float m_fLineHeight)
    {
        m_Anchor = Anchor;
        Vector2 vOffset = new Vector2(0, 0);


        m_fCharSize = fCharSize;
        if(m_bNeedCustomEndSpace)
            ResizeCustomEndSpace();

        m_fWidth = (m_fEndPos.x - m_fStartPos.x);
        m_fHeight = m_fEndPos.y;


        switch (m_Anchor)
        {
            case TextAnchor.UpperLeft:
            case TextAnchor.MiddleLeft:
            case TextAnchor.LowerLeft:
                vOffset.x = 0;
                break;
            case TextAnchor.UpperCenter:
            case TextAnchor.MiddleCenter:
            case TextAnchor.LowerCenter:
                vOffset.x = -m_fLineWidth * 0.5f;
                break;
            case TextAnchor.UpperRight:
            case TextAnchor.MiddleRight:
            case TextAnchor.LowerRight:
                vOffset.x = -m_fLineWidth;
                break;
        }
        //switch (m_Anchor)
        //{
        //    case TextAnchor.UpperLeft:
        //    case TextAnchor.UpperRight:
        //    case TextAnchor.UpperCenter:
        //        vOffset.y = 0;
        //        break;
        //    case TextAnchor.MiddleLeft:
        //    case TextAnchor.MiddleRight:
        //    case TextAnchor.MiddleCenter:
        //        vOffset.y = (wrappedText.lineHeight * 0.5f + wrappedText.extraLineCount * wrappedText.lineHeight * 0.5f * LineSpacing) * Size;//- wrappedText.lineHeight * 0.8515f * 0.5f;
        //        break;
        //    case TextAnchor.LowerLeft:
        //    case TextAnchor.LowerRight:
        //    case TextAnchor.LowerCenter:
        //        vOffset.y = (wrappedText.lineHeight + wrappedText.extraLineCount * wrappedText.lineHeight * LineSpacing) * Size; //- wrappedText.lineHeight * 0.8515f;
        //        break;
        //    default:
        //        break;
        //}

        vOffset = vStartOffset;

        m_fOffsetX += +(m_fWidth / 2) + (m_fStartPos.x);
        m_fOffsetY += -(m_fHeight / 2);
        m_fOffsetY += -(m_fLineHeight * (m_nCurrLine - 1));

        m_fWidth *= fCharSize;
        m_fHeight *= fCharSize;

        m_fOffsetX *= fCharSize;
        m_fOffsetY *= fCharSize;

        m_fOffsetY += vOffset.y;
        m_fOffsetX += vOffset.x;

        BuildString();
    }

    private void BuildString()
    {
        m_strData_All = m_strbData_All.ToString();
        m_strData_Part = m_strbData_Part.ToString();
    }

    public void AddCustomEndSpace()
    {
        m_bNeedCustomEndSpace = true;
    }

    private void ResizeCustomEndSpace()
    {
        m_fEndPos = new Vector2(m_fEndPos.x + (m_fCustomEndSpace*2), m_fEndPos.y);
    }
}

public class CommonButtonGroupList
{
    public List<CommonButton> m_PartList = new List<CommonButton>();
    /// <summary>
    /// 이 키는 공백이 제외된 상태입니다.
    /// </summary>
    public string m_strKey;
}

[SerializeField]
public class CommonButton : MonoBehaviour//, ISlot
{

    public bool activeSelf
    {
        get
        {
            return this.gameObject.activeSelf;
        }
    }
    /// <summary>
    /// 블록 시 어두워지는 정도 (0.0f ~ 1.0f)
    /// </summary>
    private static float D_BLOCK_COLOR = 0.4f;

    /// <summary>
    /// 버튼의 스타일(Inspector창에서 수정가능) 버튼 초기화시 이 정보를 기반으로 초기화
    /// </summary>
    public E_COMMON_BUTTON_STYLE g_Style;

    /// <summary>
    /// 버튼의 스타일
    /// </summary>
    private CommonButtonStyle m_Style;

    /// <summary>
    /// 버튼 아이템 입니다.
    /// </summary>
    public tk2dUIItem m_btnButton;
    /// <summary>
    /// 버튼 스프라이트 입니다. TODO : => 복수의 스프라이트를 담을수 있게 수정
    /// </summary>
    public tk2dBaseSprite m_spriteButtonUp;
    public tk2dBaseSprite m_spriteButtonDown;
    /// <summary>
    /// 버튼 콜리더 입니다.
    /// </summary>
    public Collider m_collButton;
    /// <summary>
    /// BoxCollider
    /// </summary>
    public BoxCollider m_collBox
    {
        get
        {
            return (BoxCollider)m_collButton;
        }
    }
    /// <summary>
    /// 버튼에 글자가 있다면 언어테이블을 참조해 줘야합니다.
    /// TODO : 텍스트 List 추가 => 복수의 텍스트를 담을수있게 수정
    /// </summary>
    public tk2dTextMesh m_textUp;
    public tk2dTextMesh m_textDown;


    /// <summary>
    /// 텍스트 레이아웃 기능 추가
    /// </summary>
    //public CommonTextLayout g_TextLayout;

    /// <summary>
    /// 버튼 아이콘
    /// </summary>
    public tk2dSprite m_spriteIcon;
    /// <summary>
    /// TODO : 에디터상에서 아래의 LanguageId를 수정하고 결과를 즉시볼수있게 한 후에 Init단계에서 LanguageId값을 받아 버튼 텍스트 초기화해주기
    /// </summary>
    public int m_nLanguageId;

    /// <summary>
    /// 버튼 클릭시 이벤트
    /// </summary>
    private Action m_OnClickEvent;

    /// <summary>
    /// 버튼 클릭시 이벤트
    /// </summary>
    private Action<tk2dUIItem> m_OnClickEvent_Item;

    /// <summary>
    /// 현재 블록되어있는가?
    /// </summary>
    public bool m_bBlocked;

    public string soundName = "";

    public Action m_CustomEvent;

    /// <summary>
    /// 버튼을 초기화 합니다.
    /// </summary>
    /// <param name="OnClickEvent">버튼 클릭시 이벤트</param>
    public void Init(Action OnClickEvent = null)
    {
        m_btnButton.OnClick += PlaySound;
        m_btnButton.OnClick -= m_OnClickEvent;
        m_btnButton.OnClickUIItem -= m_OnClickEvent_Item;

        m_OnClickEvent = OnClickEvent;
        SetStyle(g_Style);
        SetBlock(false);
        //AddEvent(OnClickEvent);

        m_btnButton.OnClick += m_OnClickEvent;
        //m_CustomEvent += CustomEvent;
    }
    public void Init(Action<tk2dUIItem> OnClickEvent)
    {
        m_btnButton.OnClick += PlaySound;
        m_btnButton.OnClick -= m_OnClickEvent;
        m_btnButton.OnClickUIItem -= m_OnClickEvent_Item;

        m_OnClickEvent_Item = OnClickEvent;
        SetStyle(g_Style);
        SetBlock(false);
        //AddEvent(OnClickEvent);

        m_btnButton.OnClickUIItem += m_OnClickEvent_Item;
        //m_CustomEvent += CustomEvent;
    }

    public void InitWithObservable()
    {
        SetStyle(g_Style);
        SetBlock(false);
    }

    public void PlaySound()
    {
        if(string.IsNullOrEmpty(soundName))
            return;
        SoundManager.Instance.Play(new SoundPlayData(soundName, E_AUDIO_GROUP_TYPE.UI, E_AUDIO_CHANNEL_TYPE.UISE, E_AUDIO_CLIP_GROUP.UI, null, false));
    }

    public E_COMMON_BUTTON_DATA_TYPE m_eInitType = E_COMMON_BUTTON_DATA_TYPE.NONE;
    public CommonButtonCreateData m_CreateData;
    /// <summary>
    /// Data로 최초 초기화 (CustomTextMesh에서 [n] [/n] 태그로 버튼을 만들어 커스텀 텍스트 메쉬 하위에 붙일경우에 사용합니다.)
    /// </summary>
    /// <param name="Data"></param>
    public void InitByData(CommonButtonCreateData Data)
    {
        m_eInitType = Data.m_eType;
        m_CreateData = Data;
        
        switch (Data.m_eType)
        {
            case E_COMMON_BUTTON_DATA_TYPE.COLLIDER:
                {
                    this.transform.SetLocalPosition(0, 0, 0);
                    BoxCollider BoxCollider = m_collBox;
                    BoxCollider.size = new Vector3(Data.m_fWidth, Data.m_fHeight, 0);
                    BoxCollider.center = new Vector3(Data.m_fOffsetX, Data.m_fOffsetY, 0);
                }
                break;
            case E_COMMON_BUTTON_DATA_TYPE.SPRITE:
                {
                    this.transform.SetLocalPosition(0, 0, 0.1f);
                    BoxCollider BoxCollider = m_collBox;
                    BoxCollider.size = new Vector3(Data.m_fWidth * 0.95f, Data.m_fHeight * 1.5f, 0);
                    this.transform.SetLocalPositionXY(Data.m_fOffsetX, Data.m_fOffsetY);

                    tk2dSlicedSprite spriteBackGround = (tk2dSlicedSprite)m_spriteButtonUp;
                    float fWidth = Data.m_fWidth * Constants.UnityMeterInPixels_UI * 0.95f;
                    
                    spriteBackGround.dimensions = new Vector2(fWidth, Data.m_fHeight * Constants.UnityMeterInPixels_UI * 1.5f);
                    if (fWidth < 20.0f)
                    {
                        spriteBackGround.SetBorder(0, 0, 0, 0);
                    }
                    else
                    {
                        float fBorder = 0.4f;
                        spriteBackGround.SetBorder(fBorder, fBorder, fBorder, fBorder);
                    }
                }
                break;
            case E_COMMON_BUTTON_DATA_TYPE.HITBOX:
                {
                    this.transform.SetLocalPosition(0, 0, 0.1f);
                    BoxCollider BoxCollider = m_collBox;
                    BoxCollider.size = new Vector3(Data.m_fWidth * 0.95f, Data.m_fHeight * 1.5f, 0);
                    this.transform.SetLocalPositionXY(Data.m_fOffsetX, Data.m_fOffsetY);

                    tk2dSlicedSprite spriteBackGround = (tk2dSlicedSprite)m_spriteButtonUp;
                    float fWidth = Data.m_fWidth * Constants.UnityMeterInPixels_UI * 0.95f;

                    spriteBackGround.dimensions = new Vector2(fWidth, Data.m_fHeight * Constants.UnityMeterInPixels_UI * 1.5f);
                    if (fWidth < 20.0f)
                    {
                        spriteBackGround.SetBorder(0, 0, 0, 0);
                    }
                    else
                    {
                        float fBorder = 0.4f;
                        spriteBackGround.SetBorder(fBorder, fBorder, fBorder, fBorder);
                    }
                    spriteBackGround.color = new Color(1, 1, 1, 0.5f);
                    spriteBackGround.anchor = tk2dBaseSprite.Anchor.LowerCenter;
                    this.transform.localScale = new Vector3(0.01f, 0.01f, 1);
                }
                break;
        }
        /*
        tk2dUITweenItem tween = GetComponent<tk2dUITweenItem>();
        if (tween != null)
        {
            tween.SetupStartingSize();
        }
        */
    }

    //public void CustomEvent()
    //{

    //}

    //public void ClearEvent()
    //{
    //    m_btnButton.OnClick
    //}

    //public void RemoveEvent()
    //{

    //}
    //public void AddEvent(Action OnClickEvent)
    //{
    //    m_CustomEvent += OnClickEvent;

    //}

    /// <summary>
    /// 버튼 삭제시 작업.
    /// </summary>
    public void OnDestory()
    {
        m_btnButton.OnClick -= m_OnClickEvent;
        m_btnButton.OnClickUIItem -= m_OnClickEvent_Item;
    }

    /// <summary>
    /// 버튼 스타일을 전환합니다. 스프라이트설정, (위치설정, 스케일 설정 : 추가필요)
    /// </summary>
    public void SetStyle(E_COMMON_BUTTON_STYLE eStyle)
    {
        /// NONE이면 변화 없음
        if (eStyle == E_COMMON_BUTTON_STYLE.COMMON_NONE)
            return;

        g_Style = eStyle;

        m_Style = CommonButtonStyle.GetStyle(eStyle);
        if (m_spriteButtonUp != null)
        {
            m_spriteButtonUp.SetSprite(m_Style.m_strBaseSpriteName);
        }
        if (m_spriteButtonDown != null)
        {
            m_spriteButtonDown.SetSprite(m_Style.m_strDownSpriteName);
        }
    }
    public void SetStyle()
    {
        E_COMMON_BUTTON_STYLE eStyle = g_Style;
        /// NONE이면 변화 없음
        if (eStyle == E_COMMON_BUTTON_STYLE.COMMON_NONE)
            return;

        m_Style = CommonButtonStyle.GetStyle(eStyle);

        /// 블록 중일 경우 집행하지 않음
        if (m_bBlocked == false)
        {
            if (m_spriteButtonUp != null)
            {
                m_spriteButtonUp.SetSprite(m_Style.m_strBaseSpriteName);
            }
            if (m_spriteButtonDown != null)
            {
                m_spriteButtonDown.SetSprite(m_Style.m_strDownSpriteName);
            }
        }
    }

    public void SetActive(bool bActive)
    {
        this.gameObject.SetActive(bActive);
    }

    /// <summary>
    /// 에디터에서만 변경할 수 있는 Detail 스타일.
    /// TODO : 작성필요 (DetailStyle에 Bloder 및 Dimenstion 정보 포함)
    /// </summary>
    public void SetDetailStyle()
    {
        ///tk2dSlicedSprite s = (tk2dSlicedSprite)m_spriteButton;
    }

    /// <summary>
    /// 버튼의 블록상태를 해당 상태로 전환합니다. true일 경우 Block상태(회색) false일경우 일반상태
    /// </summary>
    /// <param name="bBlocked"></param>
    public void SetBlock(bool bBlocked)
    {
        if (m_bBlocked == bBlocked) /// 이미 같은 상태라면 예외처리
            return;

        if (m_Style != null && m_Style.m_eBlockOption == E_COMMON_BUTTON_BLOCK_OPTION.NONE) ///블록옵션을 사용하지 않으면 예외처리
            return;

        m_bBlocked = bBlocked;
        if (bBlocked == true)
        {
            if (g_Style != E_COMMON_BUTTON_STYLE.COMMON_NONE)
            {
                SetBlockDisplay(bBlocked);
                //SetTextStyle(m_Style.m_eBlockTextStyle);
            }
            m_collButton.enabled = !bBlocked; ///박스 콜리더 제거
        }
        else
        {
            if (g_Style != E_COMMON_BUTTON_STYLE.COMMON_NONE)
            {
                SetBlockDisplay(bBlocked);
                //SetTextStyle(m_Style.m_eBaseTextStyle);
            }
            m_collButton.enabled = !bBlocked; /// 박스 콜리더 활성화
        }
        if (m_textUp != null)
            m_textUp.gameObject.SetActive( !(bBlocked && m_Style.m_hideTextOnBlock) );
        if (m_textDown != null)
            m_textDown.gameObject.SetActive(!(bBlocked && m_Style.m_hideTextOnBlock));

    }
    public void SetBlock()
    {
        if (m_Style != null && m_Style.m_eBlockOption == E_COMMON_BUTTON_BLOCK_OPTION.NONE) ///블록옵션을 사용하지 않으면 예외처리
            return;
        
        if (m_bBlocked == true)
        {
            if (g_Style != E_COMMON_BUTTON_STYLE.COMMON_NONE)
            {
                SetBlockDisplay(m_bBlocked);
                //SetTextStyle(m_Style.m_eBlockTextStyle);
                SetBlockTextStyle(m_bBlocked);
            }
            m_collButton.enabled = !m_bBlocked; ///박스 콜리더 제거
        }
        else
        {
            if (g_Style != E_COMMON_BUTTON_STYLE.COMMON_NONE)
            {
                SetBlockDisplay(m_bBlocked);
                //SetTextStyle(m_Style.m_eBaseTextStyle);
                SetBlockTextStyle(m_bBlocked);
            }
            m_collButton.enabled = !m_bBlocked; /// 박스 콜리더 활성화
        }

    }

    
    /// 블록 여부를 나타내기위한 임시함수
    /// 
    public void SetBlockTextStyle(bool isBlocked)
    {
        if (isBlocked)
        {
            m_textUp.color = new Color(0.7f, 0.7f, 0.7f);
            m_textUp.color2 = new Color(0.7f, 0.7f, 0.7f);
        }
        else
        {
            m_textUp.color = new Color(1.0f, 1.0f, 1.0f);
            m_textUp.color2 = new Color(1.0f, 1.0f, 1.0f);
        }
    }

    /*
    public void SetTextStyle(E_COMMON_TEXTLAYOUT_STYLE eTextStyle)
    {
        if (g_TextLayout == null)
            return;

        g_TextLayout.SetStyle(eTextStyle);
    }
    */

    public void SetBlockDisplay(bool bBlocked)
    {
        if (bBlocked == true)
        {
            if (m_Style.m_eBlockOption == E_COMMON_BUTTON_BLOCK_OPTION.SPRITE)
            {
                if(m_spriteButtonUp != null)
                    m_spriteButtonUp.SetSprite(m_Style.m_strBlockSpriteName);
                if (m_spriteButtonDown != null)
                    m_spriteButtonDown.SetSprite(m_Style.m_strBlockSpriteName);
            }
            else if (m_Style.m_eBlockOption == E_COMMON_BUTTON_BLOCK_OPTION.COLOR)
            {
                if (m_spriteButtonUp != null)
                    m_spriteButtonUp.color = new Color(D_BLOCK_COLOR, D_BLOCK_COLOR, D_BLOCK_COLOR);
                if (m_spriteButtonDown != null)
                    m_spriteButtonDown.color = new Color(D_BLOCK_COLOR, D_BLOCK_COLOR, D_BLOCK_COLOR);
            }
        }
        else
        {
            if (m_Style.m_eBlockOption == E_COMMON_BUTTON_BLOCK_OPTION.SPRITE)
            {
                if (m_spriteButtonUp != null)
                    m_spriteButtonUp.SetSprite(m_Style.m_strBaseSpriteName);
                if (m_spriteButtonDown != null)
                    m_spriteButtonDown.SetSprite(m_Style.m_strDownSpriteName);
            }
            else if (m_Style.m_eBlockOption == E_COMMON_BUTTON_BLOCK_OPTION.COLOR)
            {
                if (m_spriteButtonUp != null)
                    m_spriteButtonUp.color = new Color(1.0f, 1.0f, 1.0f);
                if (m_spriteButtonDown != null)
                    m_spriteButtonDown.color = new Color(1.0f, 1.0f, 1.0f);
            }
        }
    }
    /// <summary>
    /// 버튼의 블록상태를 전환합니다.
    /// </summary>
    /// <param name="bBlocked"></param>
    public void ChangeBlock()
    {
        SetBlock(!m_bBlocked);
    }

    /// <summary>
    /// 텍스트를 설정합니다.
    /// </summary>
    /// <param name="strText"></param>
    /// <param name="nLanguageID"></param>
    public void SetText(string strText)
    {
        
       // if (m_textUp == null)
      //      return;

        /// 레이아웃 호환
       // if (g_TextLayout != null)
       // {
       //     g_TextLayout.SetText(strText);
       //     return;
       // }

        /// 구버전 호환
        if (m_textUp != null)
        {
            m_textUp.text = strText;
        }

        if(m_textDown != null)
        {
            m_textDown.text = strText;
        }
    }

    public void SetSprite(string spriteName)
    {
        if (m_spriteIcon != null)
            m_spriteIcon.SetSprite(spriteName);
    }

    public void SetText(int nLanguageID)
    {
        /// 레이아웃 호환
        /// 
        /*
        if (g_TextLayout != null)
        {
            g_TextLayout.SetText(nLanguageID);
            return;
        }
        */
    }

    public Vector2 GetAddSizeByBoxCollider()
    {
        Vector2 AddSize = (m_collBox).size;
        return AddSize;
    }

    /*

    #region SlotPoolManager Interface 
    /// <summary>
    /// 슬롯의 모든 정보를 초기 값으로 설정합니다. 해당 함수는 GetInstance 요쳥에 대한 반환전에 자동으로 호출됩니다.
    /// </summary>
    public void OnInitInPoolMng()
    {

    }

    /// <summary>
    /// 슬롯이 오브젝트 풀 매니저로 돌아갈때 이 함수가 자동으로 호출됩니다.
    /// </summary>
    public void OnDestroyInPoolMng()
    {

    }

    public tk2dUIItem[] GetPoolButtonList()
    {
        tk2dUIItem[] pButton = new tk2dUIItem[1];
        pButton[0] = m_btnButton;
        return pButton;
    }
    #endregion
    */

}
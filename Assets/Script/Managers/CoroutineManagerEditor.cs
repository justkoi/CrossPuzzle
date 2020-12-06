#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System.Collections;
using CoroutineManagerInfo;
using System.Collections.Generic;

[CustomEditor(typeof(CoroutineManager))]
public class CoroutineManagerEditor : Editor
{

    private Vector2 m_ScrollPos;
    public CoroutineManager m_This
    {
        get;
        set;
    }

    public void OnEnable()
    {
        m_This = (CoroutineManager)target;
    }

    public override void OnInspectorGUI()
    {
        GUILayout.Label(string.Format("사용중인 객체 수 : {0}", m_This.g_nObjectCount));
        GUILayout.Label(string.Format("사용중인 모든 코루틴 수 : {0}", m_This.g_nCoroutineCount));
        GUILayout.Label(string.Format("사용중인 전역 코루틴 수 : {0}", m_This.g_nGlobalCoroutineCount));

        GUIStyle AlignmentStyle = new GUIStyle("Button");
        AlignmentStyle.alignment = TextAnchor.MiddleLeft;

        m_ScrollPos = GUILayout.BeginScrollView(m_ScrollPos);

        for (int i = 0; i < m_This.g_ObjectList.Count; i++)
        {
            bool bOpen = m_This.g_ObjectList[i].m_bOpen;
            if (GUILayout.Button(string.Concat(bOpen ? "▼" : "△", " [", m_This.g_ObjectList[i].m_CoroutineList.Count, "]\t", m_This.g_ObjectList[i].m_Mono.gameObject.name), AlignmentStyle))
            {
                m_This.g_ObjectList[i].m_bOpen = !m_This.g_ObjectList[i].m_bOpen;
            }

            if (m_This.g_ObjectList[i].m_bOpen)
            {
                GUILayout.BeginVertical(EditorStyles.textArea);
                EditorGUILayout.ObjectField(m_This.g_ObjectList[i].m_Mono, typeof(MonoBehaviour), false);
                SerializedProperty g_ObjectList = serializedObject.FindProperty("g_ObjectList");
                SerializedProperty ObjectInfo = g_ObjectList.GetArrayElementAtIndex(i);
                for (int j = 0; j < m_This.g_ObjectList[i].m_CoroutineList.Count; j++)
                {
                    GUILayout.BeginVertical(EditorStyles.textArea);
                    string strName = m_This.g_ObjectList[i].m_CoroutineList[j].m_strIterName;
                    Updater CurrUpdater = m_This.g_ObjectList[i].m_CoroutineList[j];
                    GUILayout.BeginHorizontal();
                    GUILayout.Label(string.Format("{0}", strName));
                    if (GUILayout.Button((CurrUpdater.m_bOpenLog ? "▼" : "△") + "Log", AlignmentStyle))
                    {
                        CurrUpdater.m_bOpenLog = !CurrUpdater.m_bOpenLog;
                    }
                    if (GUILayout.Button((CurrUpdater.m_bOpenInfo ? "▼" : "△") + "Detail", AlignmentStyle))
                    {
                        CurrUpdater.m_bOpenInfo = !CurrUpdater.m_bOpenInfo;
                    }
                    GUILayout.EndHorizontal();
                    if (CurrUpdater.m_bOpenLog)
                    {
                        GUILayout.BeginVertical(EditorStyles.textArea);
                        for (int k = 0; k < CurrUpdater.m_strLog.Count; k++)
                        {
                            GUILayout.BeginHorizontal(EditorStyles.textArea);
                            GUILayout.Label(string.Concat("[",k.ToString(),"]"));
                            GUILayout.Label(CurrUpdater.m_strLog[k]);
                            GUILayout.EndHorizontal();
                        }
                        GUILayout.EndVertical();
                    }
                    if (CurrUpdater.m_bOpenInfo)
                    {
                        SerializedProperty CoroutineList = ObjectInfo.FindPropertyRelative("m_CoroutineList");
                        SerializedProperty Item = CoroutineList.GetArrayElementAtIndex(j);
                        EditorGUILayout.PropertyField(Item,true);
                    }
                    GUILayout.BeginHorizontal();
                    GUILayout.Label(string.Format("상태 : "));
                    EditorGUI.BeginChangeCheck();
                    E_PROCESS_TYPE eNextProcess = (E_PROCESS_TYPE)EditorGUILayout.EnumPopup(CurrUpdater.m_eCurrProcess, GUILayout.Width(150));
                    if (EditorGUI.EndChangeCheck())
                    {
                        if (eNextProcess == E_PROCESS_TYPE.WAIT)
                        {
                            CoroutineManager.m_fWaitSecond = 10.0f;
                        }
                        CurrUpdater.StartProcess(eNextProcess);
                    }
                    if (CurrUpdater.m_eCurrProcess == E_PROCESS_TYPE.WAIT)
                    {
                        GUILayout.Label("대기시간 ");
                        float fSecond = (CurrUpdater.m_fTargetTime - CurrUpdater.m_fTimer);
                        EditorGUI.BeginChangeCheck();
                        fSecond = EditorGUILayout.FloatField(fSecond, GUILayout.Width(100));
                        if (EditorGUI.EndChangeCheck())
                        {
                            CurrUpdater.m_fTargetTime = CurrUpdater.m_fTimer + fSecond;
                        }
                        GUILayout.Label("초");
                    }

                    GUILayout.FlexibleSpace();
                    GUILayout.EndHorizontal();

                    GUILayout.EndVertical();
                }
                GUILayout.EndVertical();
            }
        }

        GUILayout.EndScrollView();

        if (!Application.isPlaying)
        {
            if (GUILayout.Button(string.Format("{0} 에디터 실행", CoroutineManager.Instance.IsRunningOnEditor ? "(On)" : "(Off)")))
            {
                CoroutineManager.Instance.Init();
            }
        }

        if (GUILayout.Button("코루틴초기화"))
        {
            CoroutineManager.Instance.Clear();
        }

        base.OnInspectorGUI();
    }
}
#endif
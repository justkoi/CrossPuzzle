using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using CoroutineManagerInfo;
using System.Text;

namespace CoroutineManagerInfo
{
    public class CoroutineList
    {
        public CustomDictionary<IEnumerator, Updater> m_CoroutineList = new CustomDictionary<IEnumerator, Updater>();
    }

#if UNITY_EDITOR
    [System.Serializable]
    public class SimpleObjectInfo
    {
        public int m_nMonoKey;
        public MonoBehaviour m_Mono;
        public List<Updater> m_CoroutineList = new List<Updater>();
        public bool m_bOpen;
        public SimpleObjectInfo(int nMonoKey, MonoBehaviour Mono)
        {
            m_nMonoKey = nMonoKey;
            m_Mono = Mono;
            m_bOpen = false;
        }
    }
#endif

    public enum E_PROCESS_TYPE
    {
        /// <summary>
        /// 없음
        /// </summary>
        PASS,
        WAIT,
        END,
        PAUSE,
        WWW
    }
}

/// <summary>
/// Yield WaitFor Cache
///  (UniRX - YieldInstructionCache)
/// </summary>
internal static class YieldInstructionCache
{
    class FloatComparer : IEqualityComparer<float>
    {
        bool IEqualityComparer<float>.Equals(float x, float y)
        {
            return x == y;
        }
        int IEqualityComparer<float>.GetHashCode(float obj)
        {
            return obj.GetHashCode();
        }
    }

    public static readonly WaitForEndOfFrame WaitForEndOfFrame = new WaitForEndOfFrame();
    public static readonly WaitForFixedUpdate WaitForFixedUpdate = new WaitForFixedUpdate();

    private static readonly Dictionary<float, WaitForSeconds> timeInterval = new Dictionary<float, WaitForSeconds>(new FloatComparer());

    public static WaitForSeconds WaitForSeconds(float seconds)
    {
        WaitForSeconds wfs;
        if (!timeInterval.TryGetValue(seconds, out wfs))
        {
            timeInterval.Add(seconds, wfs = new WaitForSeconds(seconds));
//#if UNITY_EDITOR
//            Debugs.Log(string.Format("YieldInstructionCache.timeInterfval.Count:{0}", timeInterval.Count));
//#endif
        }
        return wfs;
    }
}

public class CoroutineManager : Singleton<CoroutineManager>
{
    static public E_PROCESS_TYPE m_eCurrProcess = E_PROCESS_TYPE.PASS;
    static public float m_fWaitSecond;
    static public WWW m_Www;
#if UNITY_EDITOR
    static public List<string> m_strMsg = new List<string>();
#endif

    static public List<Updater> m_UpdaterPoolList = new List<Updater>();
    static public List<Updater> m_ActiveUpdaterList = new List<Updater>();
    static public Action<Exception> OnErrorEvent;

    static public Updater GetUpdater(IEnumerator Routine)
    {
        if (m_UpdaterPoolList.Count <= 0)
        {
            for (int i = 0; i < 10; i++)
                m_UpdaterPoolList.Add(new Updater());
        }

        Updater TargetUpdater = m_UpdaterPoolList[0];
        m_UpdaterPoolList.Remove(TargetUpdater);
        m_ActiveUpdaterList.Add(TargetUpdater);
        TargetUpdater.SetupUpdater(Routine);
        return TargetUpdater;
    }

    static public void RemoveUpdater(Updater TargetUpdater)
    {
        m_ActiveUpdaterList.Remove(TargetUpdater);
        m_UpdaterPoolList.Add(TargetUpdater);
    }

    static public object Pass()
    {
        m_eCurrProcess = E_PROCESS_TYPE.PASS;
        return null;
    }
    static public object Wait(float fSecond)
    {
        m_fWaitSecond = fSecond;
        m_eCurrProcess = E_PROCESS_TYPE.WAIT;
        return null;
    }
    static public object Pause()
    {
        m_eCurrProcess = E_PROCESS_TYPE.PAUSE;
        return null;
    }
    static public object End()
    {
        m_eCurrProcess = E_PROCESS_TYPE.END;
        return null;
    }
    static public object WWW(WWW Www)
    {
        m_Www = Www;
        m_eCurrProcess = E_PROCESS_TYPE.WWW;
        return null;
    }
    static public void Log(params object[] message)
    {
#if UNITY_EDITOR
        if (Debug.isDebugBuild)
        {
            StringBuilder sb = new StringBuilder(message.Length);
            for (int i = 0; i < message.Length; i++)
            {
                sb.Append(message[i]);
            }
            m_strMsg.Add(sb.ToString());
        }
#endif
    }
    static public void LogUnity(params object[] message)
    {
#if UNITY_EDITOR
        if (Debug.isDebugBuild)
        {
            StringBuilder sb = new StringBuilder(message.Length);
            for (int i = 0; i < message.Length; i++)
            {
                sb.Append(message[i]);
            }
            m_strMsg.Add(sb.ToString());
            //Debugs.Log(sb.ToString());
        }
#endif
    }
#if UNITY_EDITOR
    static public void ClearLog()
    {
        m_strMsg.Clear();
    }
#endif

#if UNITY_EDITOR
    public bool g_bPrintLog = false;
    public int g_nObjectCount = 0;
    public int g_nCoroutineCount = 0;
    public int g_nGlobalCoroutineCount = 0;
    public List<SimpleObjectInfo> g_ObjectList = new List<SimpleObjectInfo>();
#endif
    /// <summary>
    /// Key Instance ObjectId , Value CoroutineList
    /// </summary>
    private CustomDictionary<int, CoroutineList> m_ObjectList = new CustomDictionary<int, CoroutineList>();

    /// <summary>
    /// 초기화
    /// </summary>
    void Start()
    {
        //Debug.Log("Component From PaperLine = " + typeof(Component).IsAssignableFrom(typeof(PaperLine)));
        //Debug.Log("Component From CommonSlot = " + typeof(Component).IsAssignableFrom(typeof(CommonSlot)));
        //Debug.Log("Component From Texture = " + typeof(Component).IsAssignableFrom(typeof(Texture)));
        //Debug.Log("Component From RenderTexture = " + typeof(Component).IsAssignableFrom(typeof(RenderTexture)));
        //Debug.Log("Component From TextAsset = " + typeof(Component).IsAssignableFrom(typeof(TextAsset)));
        //Debug.Log("Component From AudioClip = " + typeof(Component).IsAssignableFrom(typeof(AudioClip)));
        //Debug.Log("Component From GameObject = " + typeof(Component).IsAssignableFrom(typeof(GameObject)));

        //StartCoroutine(TestRoutine());
        //int a = 0;
        SetupUpdaterPool(100);
        //CoroutineManager.Instance.AddCoroutine(this, TestNullRoutine());
        //CoroutineManager.Instance.AddCoroutine(this, TestRoutine());
        //CoroutineManager.Instance.AddCoroutine(this, TestRoutine());
        //CoroutineManager.Instance.AddCoroutine(this, TestRoutine());
        //CoroutineManager.Instance.AddNextCoroutine(this, TestRoutine(3, 2));
        //CoroutineManager.Instance.AddNextCoroutine(this, TestRoutine(3, 3));
        //CoroutineManager.Instance.AddNextCoroutine(this, TestRoutine(3, 4));
        //CoroutineManager.Instance.AddCoroutine(this, TestRoutine());
    }


    public IEnumerator TestRoutine()
    {
        while (true)
        {
            CoroutineManager.LogUnity("로그확인");
            yield return CoroutineManager.Pass();
        }
    }
    //public IEnumerator TestNullRoutine()
    //{
    //    while (true)
    //    {
    //        Transform tr = null;
    //        tr.SetParent(this);
    //        yield return null;
    //    }
    //}

#if UNITY_EDITOR
    public bool IsRunningOnEditor { get; private set; }
    /// <summary>
    /// 에디터 초기화
    /// </summary>
    public void Init()
    {
        if (!Application.isPlaying)
        {
            UnityEditor.EditorApplication.update -= UpdateInEditor;
            UnityEditor.EditorApplication.update += UpdateInEditor;
            IsRunningOnEditor = true;
            //Debugs.Log("[CoroutineManager] 에디터 작동시작 완료!");
        }
    }


#endif

#if UNITY_EDITOR
    private void UpdateInEditor()
    {
        LateUpdate();
    }
#endif

    //public void OnGUI()
    //{
    //    if (GUILayout.Button("추가"))
    //    {
    //        for (int i = 0; i < 1000; i++)
    //        {
    //            CoroutineManager.Instance.AddCoroutine(this, TestRoutine());
    //        }
    //    }
    //}

    private void SetupUpdaterPool(int nCount)
    {
        for (int i = 0; i < nCount; i++)
            m_UpdaterPoolList.Add(new Updater());
    }

    //public void OnGUI()
    //{

    //}

    /// <summary>
    /// 각 객체별로 코루틴을 다음단계로 진행시킵니다. 끝까지 진행된 코루틴은 Updater내부에서 폐기 시킵니다.
    /// </summary>
    void LateUpdate()
    {
        //PartyManager.StartStopWatch("코루틴메인");
        for (int i = 0; i < m_ObjectList.Count; i++)
        {
            CustomDictionary<IEnumerator, Updater> UpdaterList = m_ObjectList.ElementAt(i).m_CoroutineList;
            for (int j = 0; j < UpdaterList.Count; j++)
            {
                Updater CurrCoroutine = UpdaterList.ElementAt(j);
                CurrCoroutine.MainRoutine();
            }
        }
        //PartyManager.EndStopWatch("코루틴메인");
    }

    /// <summary>
    /// 해당 코루틴을 일시정지합니다.
    /// 일시정지된 코루틴은 Resume명령이 있을때 까지 동작하지 않습니다.
    /// </summary>
    /// <param name="Key"></param>
    public void PauseCoroutine(MonoBehaviour Mono, IEnumerator Key)
    {
        if (Key == null)
            return;

        Updater TargetUpdater = GetCoroutine(Mono, Key);
        if (TargetUpdater == null)
            return;

        PauseCoroutine(Mono, TargetUpdater);
    }

    /// <summary>
    /// 해당 코루틴을 일시정지합니다.
    /// 일시정지된 코루틴은 Resume명령이 있을때 까지 동작하지 않습니다.
    /// </summary>
    public void PauseCoroutine(MonoBehaviour Mono, Updater Routine)
    {
        //IEnumerator Key = Routine.m_Iterator;
        int nMonoKey = Mono.GetInstanceID();

        CoroutineList RoutineList;

        if (!m_ObjectList.ContainsKey(nMonoKey))
        {
            return;
        }

        RoutineList = m_ObjectList[nMonoKey];

        if (!RoutineList.m_CoroutineList.ContainsKey(Routine.m_Iterator))
        {
            return;
        }

        RoutineList.m_CoroutineList[Routine.m_Iterator].Pause();

    }

    /// <summary>
    /// 해당 코루틴을 일시정지 해제 합니다.
    /// </summary>
    /// <param name="Routine"></param>
    public void ResumeCoroutine(MonoBehaviour Mono, IEnumerator Key)
    {
        if (Key == null)
            return;

        Updater TargetUpdater = GetCoroutine(Mono, Key);
        if (TargetUpdater == null)
            return;

        ResumeCoroutine(Mono, TargetUpdater);
    }

    /// <summary>
    /// 해당 코루틴을 일시정지 해제 합니다.
    /// </summary>
    public void ResumeCoroutine(MonoBehaviour Mono, Updater Routine)
    {
        //IEnumerator Key = Routine.m_Iterator;
        int nMonoKey = Mono.GetInstanceID();

        CoroutineList RoutineList;

        if (!m_ObjectList.ContainsKey(nMonoKey))
        {
            return;
        }

        RoutineList = m_ObjectList[nMonoKey];

        if (!RoutineList.m_CoroutineList.ContainsKey(Routine.m_Iterator))
        {
            return;
        }

        RoutineList.m_CoroutineList[Routine.m_Iterator].Resume();
    }

    /// <summary>
    /// 전역 코루틴을 추가합니다. (CoroutineManager가 소유한 코루틴으로 간주합니다.)
    /// 제거가 필요한 시점에 반드시 제거해 주어야 합니다.
    /// </summary>
    /// <param name="Routine">해당 객체</param>
    /// <returns>코루틴</returns>
    public IEnumerator AddGlobalCoroutine(IEnumerator Routine)
    {
        return AddCoroutine(this, Routine);
    }


    /// <summary>
    /// 코루틴을 추가합니다. (해당 객체가 소유한 코루틴으로 간주합니다.)
    /// yield return null 이외에 사용할 수 없습니다.
    /// 대신 그 윗줄에 코루틴 매니저에 다음에 처리해야할 명령을 알려줄 수 있습니다. CoroutineManager.Wait(); End(); Pause(); WWW(); 등등...
    /// 해당 객체를 파괴시에 반드시 제거해 주어야 합니다!(OnDestroy에서 RemoveAllCoroutines 호출)
    /// 필요에 따라 비활성화시에도 제거해 주어야 합니다.
    /// 제거하지 않은 코루틴은 누적될 수 있습니다!
    /// </summary>
    /// <param name="Mono">해당 객체</param>
    /// <param name="Routine">코루틴</param>
    /// <returns></returns>
    public IEnumerator AddCoroutine(MonoBehaviour Mono, IEnumerator Routine, float fDelay = 0.0f, bool bFirstUpdate = true)
    {
        if (Routine == null)
            return null;

        AddCoroutine(Mono, GetUpdater(Routine), fDelay, bFirstUpdate);
        return Routine;
    }

    public Updater GetCoroutine(MonoBehaviour Mono, IEnumerator Routine)
    {
        return GetCoroutine(Mono.GetInstanceID(), Routine);
    }

    public Updater GetCoroutine(int nMonoKey, IEnumerator Routine)
    {
        if (!m_ObjectList.ContainsKey(nMonoKey))
            return null;

        CoroutineList RoutineList;
        RoutineList = m_ObjectList[nMonoKey];

        if (!RoutineList.m_CoroutineList.ContainsKey(Routine))
            return null;

        return RoutineList.m_CoroutineList[Routine];
    }

    public IEnumerator m_LastRoutine;

    /// <summary>
    /// 마지막으로 추가한 코루틴에 자식 코루틴을 추가합니다. (전부 하나의 리스트에 밀어 넣는데 이것을 UpdaterList, NextList 두개로 쪼개어서 저장하고 UpdateList만 Update돌리도록 변경하자...)
    /// </summary>
    /// <param name="Mono"></param>
    /// <param name="NextRoutine"></param>
    /// <returns></returns>
    public IEnumerator AddNextCoroutine(MonoBehaviour Mono, IEnumerator NextRoutine)
    {
        return AddNextCoroutine(Mono, m_LastRoutine, NextRoutine);
    }

    public IEnumerator AddNextCoroutine(MonoBehaviour Mono, IEnumerator Routine, IEnumerator NextRoutine)
    {
        int nMonoKey = Mono.GetInstanceID();
        CoroutineList RoutineList;


        if (!m_ObjectList.ContainsKey(nMonoKey))
            return null;

        RoutineList = m_ObjectList[nMonoKey];

        if (!RoutineList.m_CoroutineList.ContainsKey(Routine))
            return null;

        RoutineList.m_CoroutineList[Routine].AddNextRoutine(Mono, NextRoutine);

        return Routine;
    }

    public IEnumerator AddNextCoroutine(MonoBehaviour Mono, IEnumerator Routine, IEnumerator[] NextRoutines)
    {
        int nMonoKey = Mono.GetInstanceID();
        CoroutineList RoutineList;


        if (!m_ObjectList.ContainsKey(nMonoKey))
            return null;

        RoutineList = m_ObjectList[nMonoKey];

        if (!RoutineList.m_CoroutineList.ContainsKey(Routine))
            return null;

        RoutineList.m_CoroutineList[Routine].AddNextRoutine(Mono, NextRoutines);

        return Routine;
    }

    public void AddCoroutine(MonoBehaviour Mono, Updater Routine, float fDelay, bool bFirstUpdate = true)
    {
        //IEnumerator Key = Routine.m_Iterator;
        int nMonoKey = Mono.GetInstanceID();
        CoroutineList RoutineList;

        ///해당 객체가 없다면
        if (!m_ObjectList.ContainsKey(nMonoKey))
        {
            CoroutineList NewList = new CoroutineList();
            m_ObjectList.Add(nMonoKey, NewList);
#if UNITY_EDITOR
            g_ObjectList.Add(new SimpleObjectInfo(nMonoKey, Mono));
            g_nObjectCount++;
#endif
        }

        RoutineList = m_ObjectList[nMonoKey];

        ///중복 방지
        if (RoutineList.m_CoroutineList.ContainsKey(Routine.m_Iterator))
        {
            //Debugs.Log("[실패] Add New Coroutine ", Routine.GetHashCode());
            return;
        }

        RoutineList.m_CoroutineList.Add(Routine.m_Iterator, Routine);
#if UNITY_EDITOR
        SimpleObjectInfo Info = g_ObjectList.Find(x => x.m_nMonoKey == nMonoKey);
        Info.m_CoroutineList.Add(Routine);
        g_nCoroutineCount++;
        //if (g_bPrintLog)
        //    Debugs.Log("[코루틴매니저-코루틴추가] 성공! 카운트[", RoutineList.m_CoroutineList.Count, "]", nMonoKey.ToString(), " ", Routine.m_Iterator.GetHashCode());
        if (RoutineList.m_CoroutineList.Count >= 5)
        {
            //if (g_bPrintLog)
                //Debugs.LogError("[코루틴매니저-코루틴추가] 이 객체가 활성화 중인 코루틴이 너무 많습니다! 카운트[전체 객체 수", m_ObjectList.Count, "][이 객체의 활성화 코루틴 수", RoutineList.m_CoroutineList.Count, "]");
        }
#endif
        m_LastRoutine = Routine.m_Iterator;
        InitRoutine(Routine, nMonoKey, fDelay, bFirstUpdate);
    }

    /// <summary>
    /// 전역 코루틴을 제거합니다.
    /// </summary>
    /// <param name="Key"></param>
    public void RemoveGlobalCoroutine(IEnumerator Key)
    {
        RemoveCoroutine(this, Key);
    }

    /// <summary>
    /// 해당 객체가 소유하고 있는 코루틴을 제거합니다.
    /// 코루틴을 제거하기 위해서는 IEnumerator가 일치해야합니다!
    /// 제거 시에 해당 객체가 소유한 코루틴이 0 이하면 해당 객체또한 객체 리스트에서 제거 됩니다.
    /// </summary>
    /// <param name="Mono"></param>
    /// <param name="Key"></param>
    public void RemoveCoroutine(MonoBehaviour Mono, IEnumerator Key)
    {
        if (Key == null)
            return;

        Updater TargetUpdater = GetCoroutine(Mono, Key);
        if (TargetUpdater == null)
            return;

        RemoveCoroutine(Mono, TargetUpdater);
    }

    public void RemoveCoroutine(MonoBehaviour Mono, Updater Routine)
    {
        RemoveCoroutine(Mono.GetInstanceID(), Routine);
    }

    public bool RemoveCoroutine(int nMonoKey, Updater Routine)
    {
        IEnumerator Key = Routine.m_Iterator;
        CoroutineList RoutineList;

        ///해당 객체가 없다면
        if (!m_ObjectList.ContainsKey(nMonoKey))
        {
            return false;
        }

        RoutineList = m_ObjectList[nMonoKey];

        if (!RoutineList.m_CoroutineList.ContainsKey(Key))
        {
            //Debugs.Log("[실패] Remove New Coroutine ", m_CoroutineList[Key].GetHashCode());
            return false;
        }

        //Debugs.Log("[성공] Remove New Coroutine ", m_CoroutineList[Key].GetHashCode());
        Updater Target = Routine;
        ExitRoutine(Target);
        RoutineList.m_CoroutineList.Remove(Key);

#if UNITY_EDITOR
        SimpleObjectInfo Info = g_ObjectList.Find(x => x.m_nMonoKey == nMonoKey);
        Info.m_CoroutineList.Remove(Routine);
        g_nCoroutineCount--;
#endif
        RemoveUpdater(Target);
        if (RoutineList.m_CoroutineList.Count <= 0)
        {
            m_ObjectList.Remove(nMonoKey);
#if UNITY_EDITOR
            g_ObjectList.RemoveAll(x => x.m_nMonoKey == nMonoKey);
            g_nObjectCount--;
#endif
        }
#if UNITY_EDITOR
       // if (g_bPrintLog)
            //Debugs.Log("[코루틴매니저-코루틴제거] 성공! 카운트[", RoutineList.m_CoroutineList.Count, "]", nMonoKey.ToString(), " ", Routine.m_Iterator.GetHashCode());
#endif
        return true;
    }

    /// <summary>
    /// 해당 객체가 소유하고 있는 모든 코루틴을 제거하고 해당 객체또한 객체 리스트에서 제거합니다.
    /// </summary>
    /// <param name="Mono"></param>
    public void RemoveAllCoroutines(MonoBehaviour Mono)
    {
        int nMonoKey = Mono.GetInstanceID();
        ///해당 객체가 없다면
        if (!m_ObjectList.ContainsKey(nMonoKey))
        {
            return;
        }
        CustomDictionary<IEnumerator, Updater> CoroutineList = m_ObjectList[nMonoKey].m_CoroutineList;
        for (int i = 0; i < CoroutineList.Count; i++)
        {
            if (RemoveCoroutine(nMonoKey, CoroutineList.ElementAt(i)))
            {
                i--;
            }
        }
    }

    /// <summary>
    /// Updater 초기화
    /// </summary>
    /// <param name="Routine"></param>
    /// <param name="nMonoKey"></param>
    private void InitRoutine(Updater Routine, int nMonoKey, float fDelay, bool bFirstUpdate)
    {
        if (fDelay > 0)
        {
            Routine.StartWait(fDelay);
            bFirstUpdate = false;
        }
        Routine.Init(bFirstUpdate);
        Routine.m_nMonoKey = nMonoKey;
    }

    /// <summary>
    /// Updater 클리어
    /// </summary>
    /// <param name="Routine"></param>
    private void ExitRoutine(Updater Routine)
    {
        Routine.Exit();
    }


    /// <summary>
    /// 대상 스프라이트에 Fade가 진행중이라면 중지시킵니다.
    /// </summary>
    /// <param name="TargetSprite"></param>
    public void RemoveFadeCoroutine(MonoBehaviour Mono, tk2dBaseSprite TargetSprite)
    {
        if (TargetSprite == null)
            return;

        int nSpriteKey = TargetSprite.GetInstanceID();
        if (m_FadeCoroutineList.ContainsKey(nSpriteKey))
        {
            RemoveCoroutine(Mono, m_FadeCoroutineList[nSpriteKey]);
            m_FadeCoroutineList.Remove(nSpriteKey);
        }
    }

    public Dictionary<int, IEnumerator> m_FadeCoroutineList = new Dictionary<int, IEnumerator>();

    public IEnumerator CreateFadeCoroutine(tk2dBaseSprite TargetSprite, float fStartAlpha, float fTargetAlpha, float fFadeTime)
    {
        if (TargetSprite == null)
            return null;

        int nSpriteKey = TargetSprite.GetInstanceID();
        TargetSprite.SetAlpha(fStartAlpha);
        return FadeBlind(TargetSprite, fTargetAlpha, fFadeTime);
    }


    /// <summary>
    /// 대상 스프라이트에 일정시간동안 목표 Alpha까지 Fade를 시전합니다.
    /// 이 코루틴은 대상 객체가 소유한 코루틴으로 간주됩니다. 
    /// 글로벌 객체가 소유한 코루틴으로 추가하려면 CoroutineManager의 Mono를 등록하십시오.
    /// 대상 객체의 대상 스프라트에가 이미 페이드 코루틴을 진행하고 있다면 그 코루틴을 제거하고 새로운 페이드를 시작합니다.
    /// </summary>
    /// <param name="Mono"></param>
    /// <param name="TargetSprite"></param>
    /// <param name="fTargetAlpha"></param>
    /// <param name="fFadeTime"></param>
    /// <param name="bFirstUpdate"></param>
    /// <returns></returns>
    public IEnumerator AddFadeCoroutine(MonoBehaviour Mono, tk2dBaseSprite TargetSprite, IEnumerator FadeRoutine, bool bFirstUpdate = true)
    {
        if (TargetSprite == null)
            return null;

        RemoveFadeCoroutine(Mono, TargetSprite);
        return AddCoroutine(Mono, FadeRoutine, 0, bFirstUpdate);
    }



    public IEnumerator FadeBlind(tk2dBaseSprite TargetSprite, float fTargetAlpha, float fFadeTime)
    {
        if (TargetSprite == null)
        {
            yield return CoroutineManager.End();
        }

        float fAlpha = TargetSprite.color.a;
        float fReverse = fAlpha <= fTargetAlpha ? 1.0f : -1.0f;

        while (true)
        {
            if (TargetSprite == null)
            {
                yield return CoroutineManager.End();
            }

            fAlpha += fReverse * Time.deltaTime / fFadeTime;
            TargetSprite.color = new Color(TargetSprite.color.r, TargetSprite.color.g, TargetSprite.color.b, fAlpha);

            if (fAlpha > 1.0f)
            {
                fAlpha = 0.9999f;
                TargetSprite.color = new Color(TargetSprite.color.r, TargetSprite.color.g, TargetSprite.color.b, fAlpha);
                break;
            }
            else if (fAlpha < 0.0f)
            {
                fAlpha = 0.0001f;
                TargetSprite.color = new Color(TargetSprite.color.r, TargetSprite.color.g, TargetSprite.color.b, fAlpha);
                break;
            }

            yield return null;
        }
    }

    public void Clear()
    {
        for (int i = 0; i < m_ObjectList.Count; i++)
        {
            CoroutineList List = m_ObjectList.ElementAt(i);
            for (int j = 0; j < List.m_CoroutineList.Count; j++)
            {
                List.m_CoroutineList.ElementAt(j).RemoveThis();
                j--;
            }
        }
        m_ObjectList.Clear();
        m_FadeCoroutineList.Clear();
        m_LastRoutine = null;

#if UNITY_EDITOR
        g_ObjectList.Clear();
        g_nObjectCount = 0;
        g_nCoroutineCount = 0;
        g_nGlobalCoroutineCount = 0;
#endif
       // Debugs.Log("[CoroutineManager] 코루틴 초기화 완료!");
    }
}

[Serializable]
public class Updater
{
    /// <summary>
    /// Wait 메서드 실행중 경과 시간
    /// </summary>
    public float m_fTimer = 0;

    /// <summary>
    /// Wait 메서드 실행중 목표 시간
    /// </summary>
    public float m_fTargetTime = 0;

    /// <summary>
    /// Wait 가 진행 중인지 여부
    /// </summary>
    public bool m_bWaiting = false;

    /// <summary>
    /// 반복자
    /// </summary>
    public IEnumerator m_Iterator;

#if UNITY_EDITOR

    public string m_strIterName;
    public bool m_bOpenInfo;
    public bool m_bOpenLog;
    public List<string> m_strLog = new List<string>();
    public int m_nReturnCount = 0;

#endif

    /// <summary>
    /// 반복자
    /// </summary>
    public List<IEnumerator> m_Next_IteratorList = new List<IEnumerator>();

    [NonSerialized]
    /// <summary>
    /// 다음 코루틴
    /// </summary>
    public List<Updater> m_Next_RoutineList = new List<Updater>();

    /// <summary>
    /// 해당 객체의 Instance ID로서 객체 리스트의 키 값이 됩니다.
    /// </summary>
    public int m_nMonoKey;

    /// <summary>
    /// 일시정지 여부
    /// </summary>
    public bool m_bPause = false;

    public bool m_bWaitWWW = false;

    public WWW m_WWW;

    public E_PROCESS_TYPE m_eCurrProcess;

    public int m_nUpdateCount = 0;

    public bool m_bFirstUpdate = true;

    public void AddNextRoutine(MonoBehaviour Mono, IEnumerator Routine)
    {
        m_Next_IteratorList.Add(Routine);
        CoroutineManager.Instance.AddCoroutine(Mono, Routine, 0, false);
        var nextRoutine = CoroutineManager.Instance.GetCoroutine(m_nMonoKey, Routine);
        m_Next_RoutineList.Add(nextRoutine);
        nextRoutine.Pause();
    }

    public void AddNextRoutine(MonoBehaviour Mono, IEnumerator[] Routine)
    {
        for (int i = 0; i < Routine.Length; i++)
        {
            AddNextRoutine(Mono, Routine[i]);
        }
    }

    public Updater()
    {

    }

    public Updater(IEnumerator Main)
    {
        SetupUpdater(Main);
    }

    public void SetupUpdater(IEnumerator Main)
    {
        m_Iterator = Main;
#if UNITY_EDITOR
        m_strIterName = m_Iterator.ToString();
#endif
        m_fTimer = 0.0f;
        m_fTargetTime = 0.0f;
        m_bWaiting = false;
        m_Next_IteratorList.Clear();
        m_Next_RoutineList.Clear();
        m_nMonoKey = 0;
        m_bPause = false;
        m_bWaitWWW = false;
        m_WWW = null;
        m_eCurrProcess = E_PROCESS_TYPE.PASS;
        m_nUpdateCount = 0;
        m_bFirstUpdate = true;
    }

    public virtual void Init(bool bFirstUpdate)
    {
        m_bFirstUpdate = bFirstUpdate;

        if (bFirstUpdate)
            Update();
    }
    public virtual void Update()
    {
        bool bResult = false;

        if (m_bPause)
            return;

        CoroutineManager.Pass();
#if UNITY_EDITOR
        CoroutineManager.ClearLog();
#endif

        //안전을 중요시한다면 D_SAFE를 Define 하십시오. (추천) 
        //성능을 중요시한다면 D_SAFE를 Define 하지 마십시오. (비활성화 할경우 하나의 코루틴이라도 예외가 발생되었을때 다른 코루틴에 대한 동작을 보장할 수 없습니다!)
#if COROUTINE_MANAGER_SAFE_MODE
        try
        {
            bResult = m_Iterator.MoveNext();
        }
        catch (Exception ex)
        {
            RemoveThis();
            Debug.LogError("[코루틴매니저-FatalError] 심각한 에러가 있어 에디터를 일시정지합니다. 다음 에러 로그를 확인하십시오.");
            Debug.LogError(string.Concat("[코루틴매니저-FatalError] 잘못된 코루틴 사용이 있어 이 코루틴(", m_Iterator.ToString(), ")은 제거되었습니다. 해당 코루틴을 확인하십시오. 자세한 내용은 아래의 에러를 확인하십시오.\n", ex.ToString()));
            string errorLog = string.Concat("ERROR : CoroutineManager : ", m_Iterator.ToString());
            CoroutineManager.OnErrorEvent.Execute(ex);
            return;
        }
#else
        bResult = m_Iterator.MoveNext();
#endif
        if (!bResult)
            RemoveThis();
        else
        {
            SetState();
        }

        if (m_nUpdateCount > 1000000)
            m_nUpdateCount = 0;
    }

    public void SetState()
    {
        m_eCurrProcess = CoroutineManager.m_eCurrProcess;
        StartProcess(m_eCurrProcess);

#if UNITY_EDITOR
        m_nReturnCount++;
        for (int i = 0; i < CoroutineManager.m_strMsg.Count; i++)
        {
            m_strLog.Add("[" + m_nReturnCount.ToString() + "]" + CoroutineManager.m_strMsg[i]);
        }
        if (m_strLog.Count > 50)
        {
            m_strLog.RemoveAt(0);
        }
#endif
    }

    public void MainRoutine()
    {
        if (m_bFirstUpdate && m_nUpdateCount++ == 1)
            return;

#if UNITY_EDITOR
        if (this.m_Iterator.Current == null)
        {
#endif
            ExecuteProcess(m_eCurrProcess);
#if UNITY_EDITOR
        }
        else
        {
           // Debugs.LogError("[CoroutineManager-잘못된형식] ", this.m_Iterator.Current.GetType(), " 이런 형식은 코루틴 매니저에서 사용되지 않습니다. 메뉴얼을 확인하십시오.\n", m_strIterName);
        }
#endif
    }

    public void StartProcess(E_PROCESS_TYPE eProcess)
    {
        this.m_eCurrProcess = eProcess;

        if (m_eCurrProcess != E_PROCESS_TYPE.PAUSE)
        {
            Resume();
        }

        switch (this.m_eCurrProcess)
        {
            case E_PROCESS_TYPE.WAIT:
                StartWait(CoroutineManager.m_fWaitSecond);
                break;
            case E_PROCESS_TYPE.END:
                RemoveThis();
                break;
            case E_PROCESS_TYPE.PAUSE:
                Pause();
                break;
            case E_PROCESS_TYPE.WWW:
                if (CoroutineManager.m_Www != null)
                {
                    m_WWW = CoroutineManager.m_Www;
                    m_bWaitWWW = true;
                }
                break;
        }
    }
    private void ExecuteProcess(E_PROCESS_TYPE eProcess)
    {
        this.m_eCurrProcess = eProcess;
        switch (this.m_eCurrProcess)
        {
            case E_PROCESS_TYPE.PASS:
                this.Update();
                break;
            case E_PROCESS_TYPE.WAIT:
                if (this.m_bWaiting)
                {
                    if (this.ProcessWait())
                    {
                        this.InitTimer();
                        this.Update();
                    }
                }
                break;

            case E_PROCESS_TYPE.END:
                RemoveThis();
                break;
            case E_PROCESS_TYPE.PAUSE:
                Pause();
                break;
            case E_PROCESS_TYPE.WWW:
                if (this.m_WWW.isDone)
                {
                    this.Update();
                    this.m_bWaitWWW = false;
                }
                break;
        }
    }

    public void Pause()
    {
        m_bPause = true;
    }

    public void Resume()
    {
        m_bPause = false;
    }

    public void RemoveThis()
    {
        CoroutineManager.Instance.RemoveCoroutine(m_nMonoKey, this);
    }


    public virtual void Exit()
    {
        for (int i = 0; i < m_Next_RoutineList.Count; i++)
        {
            m_Next_RoutineList[i].Resume();
        }
    }

    public bool ProcessWait()
    {
        m_fTimer = Time.realtimeSinceStartup;
        return m_fTimer >= m_fTargetTime;
    }

    public void InitTimer()
    {
        m_fTimer = 0.0f;
        m_bWaiting = false;
    }

    public void StartWait(float fTime)
    {
        m_eCurrProcess = E_PROCESS_TYPE.WAIT;
        m_fTargetTime = Time.realtimeSinceStartup + fTime;
        //m_fTimer = 0.0f;
        m_bWaiting = true;
    }

    public object Current
    {
        get
        {
            return m_Iterator.Current;
        }
    }

    public void Reset()
    {
        m_Iterator.Reset();
    }

}

#region Command
/// <summary>
/// 코루틴을 일정시간 중지 시킵니다.
/// </summary>
public class Wait
{
    public float m_fSecond;

    public Wait(float fSecond)
    {
        m_fSecond = fSecond;
    }
}

public class Pause
{
    public Pause()
    {

    }
}

/// <summary>
/// 코루틴을 즉시 종료시킵니다.
/// </summary>
public class End
{
    public End()
    {

    }
}

#endregion

//static public class CoroutineManagerUtil
//{
//    static public void AddCoroutine(Updater Routine)
//    {
//        CoroutineManager.Instance.AddCoroutine(Routine);
//    }

//    static public void RemoveCoroutine(Updater Routine)
//    {
//        CoroutineManager.Instance.RemoveCoroutine(Routine.m_Iterator);
//    }

//    static public void AddCoroutine(IEnumerator Iter)
//    {
//        CoroutineManager.Instance.AddCoroutine(new Updater(Iter));
//    }

//    static public void RemoveCoroutine(IEnumerator Iter)
//    {
//        CoroutineManager.Instance.RemoveCoroutine(Iter);
//    }
//}


//public class CustomUpdater : Updater
//{
//    public override void Init()
//    {
//        base.Init();
//        ///Write
//        ///
//    }

//    public override void Update()
//    {
//        ///Write
//        ///
//        base.Update();
//    }

//    public override void Exit()
//    {
//        base.Exit();
//        ///Write
//        ///
//    }

//}
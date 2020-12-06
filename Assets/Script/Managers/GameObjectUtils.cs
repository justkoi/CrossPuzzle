using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using Global.Constants;
using System;

public static class GameObjectUtils
{
    /// <summary>
    /// GameObject의 Active를 키고 끕니다.
    /// </summary>
    /// <param name="go"></param>
    /// <param name="isActive"></param>
    public static void SetActive(GameObject self, bool isActive)
    {
        if (!self) return;
        if (self.activeSelf == isActive) return;
        self.SetActive(isActive);
    }

    public static void Execute(this Action action)
    {
        if(action != null)
        {
            action.Invoke();
        }
    }

    public static void Execute<T>(this Action<T> action, T t)
    {
        if (action != null)
        {
            action.Invoke(t);
        }
    }


    public static void Execute<T1,T2>(this Action<T1,T2> action, T1 t1, T2 t2)
    {
        if (action != null)
        {
            action.Invoke(t1,t2);
        }
    }

    /// <summary>
    /// 지정된 컴퍼넌트를 돌려줍니다 지정된 구성 요소가 연결되어 있지 않으면 추가하고 나서 돌려줍니다
    /// 구성 요소를 검색 할 때이 확장 메소드를 사용하여 
    /// Unity 편집기에서 스크립트를 연결 잊었다해도 null 참조를 방지 할 수 있습니다
    /// var player = gameObject.SafeGetComponent<Player> ();
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="self"></param>
    /// <returns></returns>
    public static T SafeGetComponent<T>(this GameObject self) where T : Component
    {
        return self.GetComponent<T>() ?? self.AddComponent<T>();
    }

    public static T SafeGetComponent<T>(this Component self) where T : Component
    {
        return self.GetComponent<T>() ?? self.gameObject.AddComponent<T>();
    }
    /// <summary>
    /// 모든 자식 객체를 반환합니다
    /// 모든 자식 개체를 검색하여 일괄 적으로 처리하고 싶을 때 유용합니다
    /// foreach (var n in gameObject.GetChildren ()) { }
    /// </summary>
    /// <param name="self"></param>
    /// <param name="includeInactive"></param>
    /// <returns></returns>
    public static GameObject[] GetChildren(this GameObject self, bool includeInactive = false)
    {
        return self
            .GetComponentsInChildren<Transform>(includeInactive)
            .Where(c => c != self.transform)
            .Select(c => c.gameObject)
            .ToArray();
    }

    public static GameObject[] GetChildren(this Component self, bool includeInactive = false)
    {
        return self
            .GetComponentsInChildren<Transform>(includeInactive)
            .Where(c => c != self.transform)
            .Select(c => c.gameObject)
            .ToArray();
    }
    /// <summary>
    /// 손자 개체를 제외한 모든 자식 객체를 반환합니다
    /// 위의 GetChildren 손자 개체도 얻을 수 있기 때문에 
    /// 손자 개체는 검색 대상에 포함하지 않을 경우이 확장 메소드를 사용했습니다
    /// foreach (var n in gameObject.GetChildrenWithoutGrandchild ()) { }
    /// </summary>
    /// <param name="self"></param>
    /// <returns></returns>
    public static GameObject[] GetChildrenWithoutGrandchildren(this GameObject self)
    {
        var result = new List<GameObject>();
        foreach (Transform n in self.transform)
        {
            result.Add(n.gameObject);
        }
        return result.ToArray();
    }

    public static GameObject[] GetChildrenWithoutGrandchildren(this Component self)
    {
        var result = new List<GameObject>();
        foreach (Transform n in self.transform)
        {
            result.Add(n.gameObject);
        }
        return result.ToArray();
    }
    /// <summary>
    /// 자신을 제외한 모든 자식 개체에 연결되어있는 지정된 모든 구성 요소를 반환합니다
    /// 표준 GetComponentsInChildren 자신도 검색 대상으로 해 버리기 때문에 
    /// 자신은 검색 대상에 포함하지 않을 경우이 확장 메소드를 사용했습니다
    /// var uiWidgetList = gameObject.GetComponentsInChildrenWithoutSelf<UIWidget> ();
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="self"></param>
    /// <param name="includeInactive"></param>
    /// <returns></returns>
    public static T[] GetComponentsInChildrenWithoutSelf<T>(this GameObject self, bool includeInactive = false) where T : Component
    {
        return self
            .GetComponentsInChildren<T>(includeInactive)
            .Where(c => self != c.gameObject)
            .ToArray();
    }

    public static T[] GetComponentsInChildrenWithoutSelf<T>(this Component self, bool includeInactive = false) where T : Component
    {
        return self
            .GetComponentsInChildren<T>(includeInactive)
            .Where(c => self.gameObject != c.gameObject)
            .ToArray();
    }

    /// <summary>
    /// 지정된 컴퍼넌트를 삭제합니다
    /// 일반적으로 게임 오브젝트와 구성 요소를 제거 할 때 Object.Destroy 를 사용하지만 
    /// 구성 요소 제거를 직관적으로 설명하고 싶었 기 때문에이 확장 방법을 준비했습니다
    /// gameObject.RemoveComponent<Character>();
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="self"></param>
    public static void RemoveComponent<T>(this GameObject self) where T : Component
    {
        UnityEngine.Object.Destroy(self.GetComponent<T>());
    }

    public static void RemoveComponent<T>(this Component self) where T : Component
    {
        UnityEngine.Object.Destroy(self.GetComponent<T>());
    }
    /// <summary>
    /// 지정된 구성 요소를 즉시 삭제합니다
    /// 편집기 확장에서 즉시 구성 요소를 제거하고 싶을 때는 이쪽의 확장 메소드를 사용했습니다
    /// gameObject.RemoveComponentImmediate<Character> ();
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="self"></param>
    public static void RemoveComponentImmediate<T>(this GameObject self, bool allowDestroyingAssets = false) where T : Component
    {
        T selfcomponent = self.GetComponent<T>();
        if (selfcomponent == null)
            return;

        GameObject.DestroyImmediate(selfcomponent, allowDestroyingAssets);
    }

    public static void RemoveComponentImmediate<T>(this Component self) where T : Component
    {
        GameObject.DestroyImmediate(self.GetComponent<T>());
    }
    /// <summary>
    /// 지정된 구성 요소를 제거합니다
    /// iTween 과 같이 함수를 호출 할 때마다 구성 요소가 연결되는 플러그인을 사용하고있을 때 
    /// 일괄 적으로 그 구성 요소를 제거 할 수 때때로 있었기 때문에이 확장 메소드를 만들었습니다
    /// gameObject.RemoveComponents<iTween> ();
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="self"></param>
    public static void RemoveComponents<T>(this GameObject self) where T : Component
    {
        foreach (var n in self.GetComponents<T>())
        {
            GameObject.Destroy(n);
        }
    }

    public static void RemoveComponents<T>(this Component self) where T : Component
    {
        foreach (var n in self.GetComponents<T>())
        {
            GameObject.Destroy(n);
        }
    }
    /// <summary>
    /// 지정된 구성 요소가 연결되어있는 경우 true를 돌려줍니다
    /// 구성 요소가 연결되어 있는지 확인하고 싶지만 구성 요소 자체는 필요없는 경우에이 확장 메소드를 사용했습니다
    /// if (gameObject.HasComponent<UISprite> ()) { }
    /// 반환 값의 null 체크를 생략 할 수 있으므로 약간의 코드를 알기 쉽게 설명 할 수 있습니다
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="self"></param>
    /// <returns></returns>
    public static bool HasComponent<T>(this GameObject self) where T : Component
    {
        return self.GetComponent<T>() != null;
    }

    public static bool HasComponent<T>(this Component self) where T : Component
    {
        return self.GetComponent<T>() != null;
    }
    /// <summary>
    /// 지정된 이름으로 자식 개체를 검색합니다
    /// 일반적으로 게임 오브젝트 나 MonoBehaviour 를 상속 한 고유 구성 요소에서 
    /// 자식 개체를 검색하고 싶을 때는 transform 속성을 통해 Find 함수를 호출합니다
    /// var player = gameObject.transform.Find ( "Player");
    /// 이처럼 자식 개체를 검색 할 때마다 transform 속성을 통하지이 번거롭게 느꼈기 때문에
    /// 이 확장 메서드를 작성하고 다음과 같이 쓸 수있게했습니다
    /// var player = gameObject.Find("Player");
    /// </summary>
    /// <param name="self"></param>
    /// <param name="name"></param>
    /// <returns></returns>
    public static Transform Find(this GameObject self, string name)
    {
        return self.transform.Find(name);
    }

    public static Transform Find(this Component self, string name)
    {
        return self.transform.Find(name);
    }
    /// <summary>
    /// 지정된 이름의 자식 객체를 검색하고 GameObject 형으로 반환합니다
    /// transform.Find 는 Transform 형 검색 결과를 돌려줍니다 만 
    /// 자식 개체를 검색 한 후 transform.gameObject 에 액세스 할 많았어요
    /// var player = gameObject.transform.Find("Player").gameObject;
    /// 그래서 GameObject 형 검색 결과를 돌려주는이 확장 메소드를 만들었습니다
    /// 이 확장 메소드를 사용하여 다음과 같이 검색 처리를 작성할 수 있습니다
    /// var player = gameObject.FindGameObject("Player");
    /// </summary>
    /// <param name="self"></param>
    /// <param name="name"></param>
    /// <returns></returns>
    public static GameObject FindGameObject(this GameObject self, string name)
    {
        var result = self.transform.Find(name);
        return result != null ? result.gameObject : null;
    }

    public static GameObject FindGameObject(this Component self, string name)
    {
        var result = self.transform.Find(name);
        return result != null ? result.gameObject : null;
    }
    /// <summary>
    /// 지정된 이름의 자식 객체를 검색하고 그 자식 개체에서 지정된 구성 요소를 가져옵니다
    /// 자식 개체를 검색 한 후 GetComponent 를 호출 할 수도 많았어요
    /// var player = gameObject.transform.Find("Player").GetComponent<Player>();
    /// 이 확장 메소드를 사용하여 다음과 같이 약간 간결하게 처리를 작성할 수 있습니다
    /// var player = gameObject.FindComponent<Player>("Player");
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="self"></param>
    /// <param name="name"></param>
    /// <returns></returns>
    public static T FindComponent<T>(this GameObject self, string name) where T : Component
    {
        var t = self.transform.Find(name);
        if (t == null)
        {
            return null;
        }
        return t.GetComponent<T>();
    }

    public static T FindComponent<T>(this Component self, string name) where T : Component
    {
        var t = self.transform.Find(name);
        if (t == null)
        {
            return null;
        }
        return t.GetComponent<T>();
    }
    /// <summary>
    /// 지정된 이름의 자식 객체를 깊은 계층까지 검색하고 GameObject 형으로 반환합니다
    /// transform.Find 손자 개체를 검색하려면 다음과 같이 부모도 지정해야합니다
    /// var player = transform.Find("Character / Player / Hand");   // 가로질러 검색.
    /// 이 확장 메소드를 사용하여 친자 관계를 의식하지 않고 개체의 이름만으로 손자 객체를 검색 할 수 있습니다
    /// var player = transform.FindDeep("Player");
    /// 그러나 같은 이름의 오브젝트가 2 개 이상 존재하는 경우는 어느 한쪽 만 불러 않기 때문에주의가 필요합니다
    /// </summary>
    /// <param name="self"></param>
    /// <param name="name"></param>
    /// <param name="includeInactive"></param>
    /// <returns></returns>
    public static GameObject FindDeep(this GameObject self, string name, bool includeInactive = false)
    {
        var children = self.GetComponentsInChildren<Transform>(includeInactive);
        foreach (var transform in children)
        {
            if (transform.name == name)
            {
                return transform.gameObject;
            }
        }
        return null;
    }

    public static GameObject FindDeep(this Component self, string name, bool includeInactive = false)
    {
        var children = self.GetComponentsInChildren<Transform>(includeInactive);
        foreach (var transform in children)
        {
            if (transform.name == name)
            {
                return transform.gameObject;
            }
        }
        return null;
    }
    /// <summary>
    /// 부모 개체를 설정합니다
    /// Unity4.6에서 추가 된 SetParent 는 Transform 형의 인스턴스를 인수 데리고 
    /// 부모 개체를 설정하고 싶을 때는 매번 transform 속성을 전달해야했습니다
    /// player.SetParent(parent.transform);
    /// 그 번거 로움을 해소하기 위해 GameObject 형의 인스턴스도 Component 형의 인스턴스도 
    /// 받을 SetParent 함수를 확장 메서드로 실현했습니다
    /// player.SetParent (parent);
    /// </summary>
    /// <param name="self"></param>
    /// <param name="parent"></param>
    public static void SetParent(this GameObject self, Component parent)
    {
        self.transform.SetParent(parent.transform);
    }

    public static void SetParent(this GameObject self, GameObject parent)
    {
        self.transform.SetParent(parent.transform);
    }
    public static void SetParent(this Component self, Component parent)
    {
        self.transform.SetParent(parent.transform);
    }
    
    public static void SetParent(this Component self, GameObject parent)
    {
        self.transform.SetParent(parent.transform);
    }
    /// <summary>
    /// 자식 개체가 존재하는지 여부를 반환합니다
    /// 재귀 처리 등으로 자식 개체가 존재하는 경우에만 뭔가 처리를하고 싶은 경우에 
    /// 매번 transform.childCount 를 참조 자식 개체가 존재하는지 여부를 
    /// 판정하는 수고를 생략하기 위해서이 확장 메소드를 만들었습니다
    /// if (gameObject.HasChild ())
    /// </summary>
    /// <param name="self"></param>
    /// <returns></returns>
    public static bool HasChild(this GameObject self)
    {
        return 0 < self.transform.childCount;
    }

    public static bool HasChild(this Component self)
    {
        return 0 < self.transform.childCount;
    }
    /// <summary>
    /// 부모 개체가 존재하는지 여부를 반환합니다
    /// 이쪽도 위의 HasChild 처럼 
    /// 매번 transform.parent 을 참조 null 체크하지 않아도 위해 만들었습니다
    /// if (gameObject.HasParent ())
    /// </summary>
    /// <param name="self"></param>
    /// <returns></returns>
    public static bool HasParent(this GameObject self)
    {
        return self.transform.parent != null;
    }

    public static bool HasParent(this Component self)
    {
        return self.transform.parent != null;
    }
    /// <summary>
    /// 지정된 인덱스의 자식 객체를 반환합니다
    /// transform 속성을 참조하지 않아도 GetChild 함수를 호출 할 수 있도록하기 위해
    /// 이 확장 메소드를 만들었습니다
    /// 
    /// </summary>
    /// <param name="self"></param>
    /// <param name="index"></param>
    /// <returns></returns>
    public static GameObject GetChild(this GameObject self, int index)
    {
        var t = self.transform.GetChild(index);
        return t != null ? t.gameObject : null;
    }

    public static GameObject GetChild(this Component self, int index)
    {
        var t = self.transform.GetChild(index);
        return t != null ? t.gameObject : null;
    }
    /// <summary>
    /// 부모 객체를 돌려줍니다
    /// transform 속성을 참조하지 않아도 parent 속성을 참조 할 수 있도록하기 위해
    /// 이 확장 메소드를 만들었습니다
    /// var parent = gameObject.GetParent ();
    /// </summary>
    /// <param name="self"></param>
    /// <returns></returns>
    public static GameObject GetParent(this GameObject self)
    {
        var t = self.transform.parent;
        return t != null ? t.gameObject : null;
    }

    public static GameObject GetParent(this Component self)
    {
        var t = self.transform.parent;
        return t != null ? t.gameObject : null;
    }
    /// <summary>
    /// 루트가되는 오브젝트를 돌려줍니다
    /// transform 속성을 참조하지 않아도 root 속성을 참조 할 수 있도록하기 위해
    /// 이 확장 메소드를 만들었습니다
    /// var root = gameObject.GetRoot ();
    /// </summary>
    /// <param name="self"></param>
    /// <returns></returns>
    public static GameObject GetRoot(this GameObject self)
    {
        var root = self.transform.root;
        return root != null ? root.gameObject : null;
    }

    public static GameObject GetRoot(this Component self)
    {
        var root = self.transform.root;
        return root != null ? root.gameObject : null;
    }

    /// <summary>
    /// 모든 자식 개체를 보여준다.
    /// Main Camera 
    /// UIRoot 
    /// UIRoot / Base 
    /// UIRoot / Base / Button 
    /// UIRoot / Base / Button / Label
    /// </summary>
    /// <param name="self"></param>
    /// <returns></returns>
    public static string GetPath(this GameObject self)
    {
        var list = new List<GameObject>();
        for (var n = self.transform.parent; n != null; n = n.parent)
          {
            list.Add(n.gameObject);
        }
        list.Reverse();
        list.Add(self);
        var names = list.Select(c => c.name).ToArray();
        return string.Join("/", names);
    }


    /// <summary>
    /// 레이어 이름을 사용하여 레이어를 설정합니다
    /// gameObject.SetLayer ( "UI");
    /// 만약 게임 오브젝트의 레이어를 레이어 이름으로 지정하려는 경우 LayerMask.NameToLayer 를 호출 시간을 절약하기 위해
    /// 이 확장 메소드를 만들었습니다 이 확장 메소드를 사용하여 레이어 이름으로 레이어를 지정할 수 있습니다
    /// </summary>
    /// <param name="self"></param>
    /// <param name="layerName"></param>
    public static void SetLayer(this GameObject self, string layerName)
    {
        self.layer = LayerMask.NameToLayer(layerName);
    }

    public static void SetLayer(this Component self, string layerName)
    {
        self.gameObject.layer = LayerMask.NameToLayer(layerName);
    }

    /// <summary>
    /// 자신을 포함한 모든 자식 개체의 레이어를 설정합니다.
    /// gameObject.SetLayerRecursively (8); 
    /// gameObject.SetLayerRecursively ( "UI");
    /// </summary>
    /// <param name="self"></param>
    /// <param name="layerName"></param>
    public static void SetLayerRecursively(this GameObject self, int layer)
    {
        self.layer = layer;

        foreach (Transform n in self.transform)
        {
            SetLayerRecursively(n.gameObject, layer);
        }
    }

    public static void SetLayerRecursively(this Component self, int layer)
    {
        self.gameObject.layer = layer;

        foreach (Transform n in self.gameObject.transform)
        {
            SetLayerRecursively(n, layer);
        }
    }
    
    public static void SetLayerRecursively(this GameObject self, string layerName)
    {
        self.SetLayerRecursively(LayerMask.NameToLayer(layerName));
    }

    public static void SetLayerRecursively(this Component self, string layerName)
    {
        self.SetLayerRecursively(LayerMask.NameToLayer(layerName));
    }

    public static void SetLocalPosition(this Transform trTarget, float fX, float fY, float fZ)
    {
        trTarget.localPosition = new Vector3(fX, fY, fZ);
    }

    public static void SetLocalPositionX(this Transform trTarget, float fX)
    {
        trTarget.localPosition = new Vector3(fX, trTarget.localPosition.y, trTarget.localPosition.z);
    }

    public static void SetLocalPositionY(this Transform trTarget, float fY)
    {
        trTarget.localPosition = new Vector3(trTarget.localPosition.x, fY, trTarget.localPosition.z);
    }

    public static void SetLocalPositionZ(this Transform trTarget, float fZ)
    {
        trTarget.localPosition = new Vector3(trTarget.localPosition.x, trTarget.localPosition.y, fZ);
    }

    public static void SetLocalPositionXY(this Transform trTarget, float fX, float fY)
    {
        trTarget.localPosition = new Vector3(fX, fY, trTarget.localPosition.z);
    }


    public static void SetWorldPosition(this Transform trTarget, float fX, float fY, float fZ)
    {
        trTarget.position = new Vector3(fX, fY, fZ);
    }

    public static void SetWorldPositionX(this Transform trTarget, float fX)
    {
        trTarget.position = new Vector3(fX, trTarget.position.y, trTarget.position.z);
    }

    public static void SetWorldPositionY(this Transform trTarget, float fY)
    {
        trTarget.position = new Vector3(trTarget.position.x, fY, trTarget.position.z);
    }

    public static void SetWorldPositionXY(this Transform trTarget, float fX, float fY)
    {
        trTarget.position = new Vector3(fX, fY, trTarget.position.z);
    }
    public static void SetWorldPositionZ(this Transform trTarget, float fZ)
    {
        trTarget.position = new Vector3(trTarget.position.x, trTarget.position.y, fZ);
    }
    public static void SetSize(this BoxCollider Target, float fX, float fY, float fZ)
    {
        Target.size = new Vector3(fX, fY, fZ);
    }

    public static void Init(this RenderTexture TargetRenderTexture, tk2dSpriteFromTexture SpriteFromTexture,  Camera EffectCam)
    {
        if (TargetRenderTexture == null)
            TargetRenderTexture = RenderTexture.GetTemporary(512, 512, 24, RenderTextureFormat.ARGB32);
        
        if (EffectCam != null)
            EffectCam.targetTexture = TargetRenderTexture;

        if (SpriteFromTexture != null)
        {
            SpriteFromTexture.texture = TargetRenderTexture;
            SpriteFromTexture.Create(tk2dSpriteCollectionSize.PixelsPerMeter(Constants.UnityMeterInPixels_UI), TargetRenderTexture, tk2dBaseSprite.Anchor.MiddleCenter);
        }
    }

    public static void Release(this RenderTexture TargetRenderTexture, tk2dSpriteFromTexture SpriteFromTexture, Camera EffectCam)
    {
        if (EffectCam != null)
            EffectCam.targetTexture = null;

        if (SpriteFromTexture != null)
            SpriteFromTexture.texture = null;

        if (TargetRenderTexture != null)
        {
            TargetRenderTexture.Release();
            RenderTexture.ReleaseTemporary(TargetRenderTexture);
        }
    }

    /// <summary>
    /// 확장함수 추가하였습니다.
    /// </summary>
    /// <param name="strTarget"></param>
    /// <returns></returns>
    public static string PutComma(this int strTarget)
    {
        return string.Format("{0:n0}", strTarget);
    }
    

    public static string ToRGBHex(this Color c)
    {
        return string.Format("{0:X2}{1:X2}{2:X2}{3:X2}", ToByte(c.r), ToByte(c.g), ToByte(c.b), "FF");
    }

    private static byte ToByte(float f)
    {
        f = Mathf.Clamp01(f);
        return (byte)(f * 255);
    }
    
    public static void SetArrowIcon(this tk2dBaseSprite spriteTarget, bool bUp)
    {
        if (bUp)
        {
            spriteTarget.SetSprite(string.Concat("c_icon_2"));
            spriteTarget.transform.localRotation = Quaternion.Euler(new Vector3(0, 0, 0));
        }
        else
        {
            spriteTarget.SetSprite(string.Concat("c_icon_1"));
            spriteTarget.transform.localRotation = Quaternion.Euler(new Vector3(0, 0, 180));
        }
    }

    public static int CeilToInt(this float fTarget)
    {
        return (int)(fTarget + 0.0000000001f);
    }
    public static int ToPercentInt(this float fTarget)
    {
        return (int)(fTarget*100.0f).CeilToInt();
    }

    public static int ToPermilInt(this float fTarget)
    {
        return (int)(fTarget * 1000.0f).CeilToInt();
    }
    public static float ToDecimalFloat(this int nTarget)
    {
        return (float)(nTarget * 0.1f);
    }

    public static string AppendLine(this string msg)
    {
        return string.Concat(msg, "\n");
    }

    public static string AppendSpace(this string msg)
    {
        return string.Concat(msg, " ");
    }

    public static float ToPercentFloat(this int nTarget)
    {
        return (float)(nTarget * 0.01f);
    }

    public static float ToPermilFloat(this int nTarget)
    {
        return (float)(nTarget * 0.001f);
    }
    public static Color DecimalCode(this Color color, int R, int G, int B)
    {
        return new Color(R / 255.0f, G / 255.0f, B / 255.0f);
    }
    public static Color LightBlue(this Color color)
    {
        return new Color().DecimalCode(173, 216, 230);
    }

    public static Color LightRed(this Color color)
    {
        return new Color().DecimalCode(250, 128, 114);
    }
    public static Color LightGreen(this Color color)
    {
        return new Color().DecimalCode(144, 238, 144);
    }
    

    public static float ToUIMeter(this float UnityMeter)
    {
        return UnityMeter * Constants.UnityMeterInPixels_UI;
    }

    public static float ToUnityMeter(this float UIMeter)
    {
        return UIMeter / Constants.UnityMeterInPixels_UI;
    }


    public static void SafetyAdd<TKey, TValue>(this Dictionary<TKey, TValue> dictionary, TKey key, TValue value)
    {
        if (!dictionary.ContainsKey(key))
            dictionary.Add(key, value);
      //  else
       //     logger.Error("The key already exists");
    }

    public static TValue SafetyGet<TKey, TValue>(this Dictionary<TKey, TValue> dictionary, TKey key)
    {
        if (dictionary.ContainsKey(key))
            return dictionary[key];
        else
            return default(TValue);
    }
}

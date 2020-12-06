using System.Collections.Generic;
using UnityEngine;

public class UIManager : Singleton<UIManager>

{
    public Transform uiRoot;

    public List<GameObject> uiList = new List<GameObject>();
    public void Start()
    {
        
    }

    public void CreateUI(GameObject newUI, int layer = 0)
    {
        var ui = Instantiate(newUI, uiRoot);
        uiList.Add(ui);
        ui.transform.SetLocalPositionZ(layer * -100);
    }

    public void DestroyAllUIs()
    {
        foreach(var ui in uiList)
        {
            Destroy(ui);
        }
        uiList.Clear();
    }
}
#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System.Collections;

[CustomEditor(typeof(CommonButton))]
public class CommonButtonEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();


        if (GUILayout.Button("Repaint"))
        {
            RepaintButton();
        }

        if (GUILayout.Button("ChangeBlock"))
        {
            for (int i = 0; i < targets.Length; i++)
            {
                if (targets[i] is CommonButton)
                {
                    CommonButton btnTarget = (CommonButton)targets[i];
                    btnTarget.ChangeBlock();
                    btnTarget.SetStyle();
                    btnTarget.SetBlock();
                }
            }
        }

        if (GUI.changed)
        {
            RepaintButton();
        }
    }

    private void RepaintButton()
    {
        for (int i = 0; i < targets.Length; i++)
        {
            if (targets[i] is CommonButton)
            {
                CommonButton btnTarget = (CommonButton)targets[i];
                btnTarget.SetStyle();
                btnTarget.SetBlock();
            }
        }
    }
}
#endif
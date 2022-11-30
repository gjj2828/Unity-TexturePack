using UnityEditor;
using UnityEngine;

public class ChangeMatWnd : EditorWindow
{
    private GameObject m_Prefab;
    private Material m_ReplacedMat;
    private Material m_ReplaceMat;

    [MenuItem("Window/Change Material")]
    static void Init()
    {
        var wnd = GetWindow<ChangeMatWnd>();
        wnd.Show();
    }

    private void OnGUI()
    {
        EditorGUILayout.BeginVertical();
        m_Prefab = EditorGUILayout.ObjectField("Prefab", m_Prefab, typeof(GameObject), false) as GameObject;
        m_ReplacedMat =  EditorGUILayout.ObjectField("Replaced Material", m_ReplacedMat, typeof(Material), false) as Material;
        m_ReplaceMat =  EditorGUILayout.ObjectField("Replace Material", m_ReplaceMat, typeof(Material), false) as Material;
        if (GUILayout.Button("Replace"))
        {
            if (m_Prefab != null)
            {
                var renders = m_Prefab.GetComponentsInChildren<Renderer>();
                foreach (var render in renders)
                {
                    var mats = render.sharedMaterials;
                    if (mats == null) continue;
                    for(int i = 0; i < mats.Length; i++)
                    {
                        if(mats[i] == m_ReplacedMat)
                        {
                            mats[i] = m_ReplaceMat;
                        }
                    }
                    render.sharedMaterials = mats;
                }
                AssetDatabase.SaveAssetIfDirty(m_Prefab);
                Debug.Log("Change material complete.");
            }
            else
            {
                Debug.Log("Prefab is null.");
            }
        }
        EditorGUILayout.EndVertical();
    }
}

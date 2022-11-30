using UnityEditor;

public static class Util
{
    [MenuItem("Assets/Create/Texture Pack", priority = 0)]
    public static void CreateAsset()
    {
        ProjectWindowUtil.CreateAssetWithContent("new_texture_pack.dummy", string.Empty);
    }
}

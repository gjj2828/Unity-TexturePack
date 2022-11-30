using UnityEditor;

public static class Util
{
    [MenuItem("Assets/Create/Texture Pack", priority = 0)]
    public static void CreateAsset()
    {
        ProjectWindowUtil.CreateAssetWithContent($"new_texture_pack.{Define.TEXTURE_PACK_EXTENSION}", string.Empty);
    }
}

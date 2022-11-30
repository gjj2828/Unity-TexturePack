using UnityEditor;
using UnityEditor.AssetImporters;
using UnityEngine;
using UnityEngine.Experimental.Rendering;

[ScriptedImporter(0, Define.TEXTURE_PACK_EXTENSION)]
public class TexturePackImporter : ScriptedImporter
{
    public LazyLoadReference<Texture2D> metallic;
    public LazyLoadReference<Texture2D> ao;

    public override void OnImportAsset(AssetImportContext ctx)
    {
        var result = ScriptableObject.CreateInstance<TexturePack>();

        result.metallicAo = CombineMetallicAo(ctx);
        if(result.metallicAo)
        {
            ctx.AddObjectToAsset("metallicAo", result.metallicAo);
        }

        ctx.AddObjectToAsset("result", result);
        ctx.SetMainObject(result);
    }

    private Texture2D CombineMetallicAo(AssetImportContext ctx)
    {
        if (!AnalyzeMetallicAo(ctx, out Color32[] metallicPixels, out Color32[] aoPixels, out int width, out int height)) return null;

        var newTexture = new Texture2D(width, height, TextureFormat.RGBA32, true, false);
        var pixels = new Color32[width * height];
        for (int i = 0; i < pixels.Length; i++)
        {
            pixels[i] = new Color32(metallicPixels != null ? metallicPixels[i].r : (byte)255
                , aoPixels != null ? aoPixels[i].r : (byte)255, 0, 255);
        }
        newTexture.SetPixels32(pixels);
        newTexture.Apply(true, false);
        newTexture.Compress(true);
        newTexture.Apply(true, true);
        newTexture.name = "MetallicAo";

        return newTexture;
    }

    private bool AnalyzeMetallicAo(AssetImportContext ctx, out Color32[] metallicPixels, out Color32[] aoPixels, out int width, out int height)
    {
        metallicPixels = null;
        aoPixels = null;
        width = 0;
        height = 0;

        if (metallic.isSet)
        {
            width = metallic.asset.width;
            height = metallic.asset.height;
            metallicPixels = GetPixels32(metallic.asset);
        }

        if (ao.isSet)
        {
            width = Mathf.Max(width, ao.asset.width);
            height = Mathf.Max(height, ao.asset.height);
            aoPixels = GetPixels32(ao.asset);
        }

        if (metallicPixels == null && aoPixels == null) return false;

        if (metallicPixels != null && (metallic.asset.width != width || metallic.asset.height != height)) return false;
        if (aoPixels != null && (ao.asset.width != width || ao.asset.height != height)) return false;

        if(metallic.isSet)
        {
            ctx.DependsOnArtifact(AssetDatabase.GetAssetPath(metallic.asset));
        }
        if (ao.isSet)
        {
            ctx.DependsOnArtifact(AssetDatabase.GetAssetPath(ao.asset));
        }

        return true;
    }

    private Color32[] GetPixels32(Texture2D tex)
    {
        if (tex.isReadable) return tex.GetPixels32();

        var texReadable = new Texture2D(tex.width, tex.height, tex.format, tex.mipmapCount, GraphicsFormatUtility.IsSRGBFormat(tex.graphicsFormat));
        Graphics.CopyTexture(tex, texReadable);
        var pixels = texReadable.GetPixels32();
        DestroyImmediate(texReadable);
        return pixels;
    }
}

using UnityEngine;
using UnityEngine.Rendering;

namespace LeTai.Asset.TranslucentImage.UniversalRP
{
public static class Extensions
{
    public static void BlitCustom(
        this CommandBuffer     cmd,
        RenderTargetIdentifier source,
        RenderTargetIdentifier destination,
        Material               material,
        int                    passIndex
    )
    {
        if (
            SystemInfo.graphicsShaderLevel >= 30
#if !UNITY_2023_1_OR_NEWER
         && SystemInfo.graphicsDeviceType != GraphicsDeviceType.OpenGLES2
#endif
        )
            cmd.BlitProcedural(source, destination, material, passIndex);
        else
            cmd.BlitFullscreenTriangle(source, destination, material, passIndex);
    }

    public static void BlitProcedural(
        this CommandBuffer     cmd,
        RenderTargetIdentifier source,
        RenderTargetIdentifier destination,
        Material               material,
        int                    passIndex
    )
    {
        cmd.SetGlobalTexture(ShaderId.MAIN_TEX, source);
        cmd.SetRenderTarget(new RenderTargetIdentifier(destination, 0, CubemapFace.Unknown, -1),
                            RenderBufferLoadAction.DontCare,
                            RenderBufferStoreAction.Store,
                            RenderBufferLoadAction.DontCare,
                            RenderBufferStoreAction.DontCare);
        cmd.DrawProcedural(Matrix4x4.identity, material, passIndex, MeshTopology.Quads, 4, 1, null);
    }
}
}

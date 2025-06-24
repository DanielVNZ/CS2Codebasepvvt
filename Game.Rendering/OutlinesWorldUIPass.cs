using System;
using Game.Settings;
using UnityEngine;
using UnityEngine.Experimental.Rendering;
using UnityEngine.Profiling;
using UnityEngine.Rendering;
using UnityEngine.Rendering.HighDefinition;
using UnityEngine.Rendering.RendererUtils;

namespace Game.Rendering;

public class OutlinesWorldUIPass : CustomPass
{
	private static class ShaderID
	{
		public static readonly int _OutlineBuffer = Shader.PropertyToID("_OutlineBuffer");

		public static readonly int _Outlines_MaxDistance = Shader.PropertyToID("_Outlines_MaxDistance");

		public static readonly int _DRSScale = Shader.PropertyToID("_DRSScale");

		public static readonly int _DRSScaleSquared = Shader.PropertyToID("_DRSScaleSquared");
	}

	public LayerMask m_OutlineLayer = LayerMask.op_Implicit(0);

	public Material m_FullscreenOutline;

	public float m_MaxDistance = 16000f;

	private MaterialPropertyBlock m_OutlineProperties;

	private ShaderTagId[] m_ShaderTags;

	private RTHandle m_OutlineBuffer;

	private CustomSampler m_OutlinesSampler;

	public RTHandle outlineBuffer => m_OutlineBuffer;

	private void CheckResource()
	{
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Invalid comparison between Unknown and I4
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Invalid comparison between Unknown and I4
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Invalid comparison between I4 and Unknown
		AntiAliasingQualitySettings obj = SharedSettings.instance?.graphics?.GetQualitySetting<AntiAliasingQualitySettings>();
		MSAASamples val = (MSAASamples)((obj == null) ? 1 : ((int)obj.outlinesMSAA));
		if ((int)val < 1)
		{
			val = (MSAASamples)1;
		}
		if ((int)val > 8)
		{
			val = (MSAASamples)8;
		}
		if (m_OutlineBuffer == null || (Object)(object)m_OutlineBuffer.rt == (Object)null || m_OutlineBuffer.rt.antiAliasing != (int)val)
		{
			ReleaseResources();
			CreateResources(val);
		}
	}

	private void CreateResources(MSAASamples msaaSamples)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		m_OutlineBuffer = RTHandles.Alloc(Vector2.one, TextureXR.slices, (DepthBits)0, (GraphicsFormat)4, (FilterMode)0, (TextureWrapMode)0, TextureXR.dimension, false, false, true, false, 1, 0f, msaaSamples, false, false, (RenderTextureMemoryless)0, (VRTextureUsage)0, "Outline Buffer");
	}

	private void ReleaseResources()
	{
		if (m_OutlineBuffer != null)
		{
			m_OutlineBuffer.Release();
		}
	}

	protected override void Setup(ScriptableRenderContext renderContext, CommandBuffer cmd)
	{
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Expected O, but got Unknown
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		m_OutlinesSampler = CustomSampler.Create("Outlines pass", false);
		m_OutlineProperties = new MaterialPropertyBlock();
		m_ShaderTags = (ShaderTagId[])(object)new ShaderTagId[3]
		{
			new ShaderTagId("Forward"),
			new ShaderTagId("ForwardOnly"),
			new ShaderTagId("SRPDefaultUnlit")
		};
	}

	protected override void AggregateCullingParameters(ref ScriptableCullingParameters cullingParameters, HDCamera hdCamera)
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		((ScriptableCullingParameters)(ref cullingParameters)).cullingMask = ((ScriptableCullingParameters)(ref cullingParameters)).cullingMask | (uint)LayerMask.op_Implicit(m_OutlineLayer);
	}

	private static RendererListDesc CreateOpaqueRendererListDesc(CullingResults cull, Camera camera, ShaderTagId passName, PerObjectData rendererConfiguration = (PerObjectData)0, RenderQueueRange? renderQueueRange = null, RenderStateBlock? stateBlock = null, Material overrideMaterial = null, bool excludeObjectMotionVectors = false)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		RendererListDesc result = default(RendererListDesc);
		((RendererListDesc)(ref result))._002Ector(passName, cull, camera);
		result.rendererConfiguration = rendererConfiguration;
		result.renderQueueRange = (renderQueueRange.HasValue ? renderQueueRange.Value : HDRenderQueue.k_RenderQueue_AllOpaque);
		result.sortingCriteria = (SortingCriteria)59;
		result.stateBlock = stateBlock;
		result.overrideMaterial = overrideMaterial;
		result.excludeObjectMotionVectors = excludeObjectMotionVectors;
		return result;
	}

	private static RendererListDesc CreateTransparentRendererListDesc(CullingResults cull, Camera camera, ShaderTagId passName, PerObjectData rendererConfiguration = (PerObjectData)0, RenderQueueRange? renderQueueRange = null, RenderStateBlock? stateBlock = null, Material overrideMaterial = null, bool excludeObjectMotionVectors = false)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		RendererListDesc result = default(RendererListDesc);
		((RendererListDesc)(ref result))._002Ector(passName, cull, camera);
		result.rendererConfiguration = rendererConfiguration;
		result.renderQueueRange = (renderQueueRange.HasValue ? renderQueueRange.Value : HDRenderQueue.k_RenderQueue_AllTransparent);
		result.sortingCriteria = (SortingCriteria)87;
		result.stateBlock = stateBlock;
		result.overrideMaterial = overrideMaterial;
		result.excludeObjectMotionVectors = excludeObjectMotionVectors;
		return result;
	}

	private void DrawOutlineMeshes(CustomPassContext ctx)
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_007d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0092: Unknown result type (might be due to invalid IL or missing references)
		//IL_0093: Unknown result type (might be due to invalid IL or missing references)
		//IL_0098: Unknown result type (might be due to invalid IL or missing references)
		//IL_009e: Unknown result type (might be due to invalid IL or missing references)
		//IL_009f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b2: Unknown result type (might be due to invalid IL or missing references)
		RendererListDesc val = default(RendererListDesc);
		((RendererListDesc)(ref val))._002Ector(m_ShaderTags, ctx.cullingResults, ctx.hdCamera.camera);
		val.rendererConfiguration = (PerObjectData)13;
		val.renderQueueRange = RenderQueueRange.all;
		val.sortingCriteria = (SortingCriteria)4;
		val.excludeObjectMotionVectors = false;
		val.layerMask = LayerMask.op_Implicit(m_OutlineLayer);
		RendererListDesc val2 = val;
		ctx.cmd.EnableShaderKeyword("SHADERPASS_OUTLINES");
		ctx.cmd.SetGlobalFloat(ShaderID._Outlines_MaxDistance, m_MaxDistance);
		CoreUtils.SetRenderTarget(ctx.cmd, m_OutlineBuffer, (ClearFlag)1, 0, (CubemapFace)(-1), -1);
		ScriptableRenderContext renderContext = ctx.renderContext;
		CommandBuffer cmd = ctx.cmd;
		ScriptableRenderContext renderContext2 = ctx.renderContext;
		CoreUtils.DrawRendererList(renderContext, cmd, ((ScriptableRenderContext)(ref renderContext2)).CreateRendererList(val2));
		ctx.cmd.DisableShaderKeyword("SHADERPASS_OUTLINES");
	}

	private void DrawAfterDRSObjects(CustomPassContext ctx)
	{
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0070: Unknown result type (might be due to invalid IL or missing references)
		//IL_0075: Unknown result type (might be due to invalid IL or missing references)
		//IL_007f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		//IL_0085: Unknown result type (might be due to invalid IL or missing references)
		//IL_008b: Unknown result type (might be due to invalid IL or missing references)
		//IL_008c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0091: Unknown result type (might be due to invalid IL or missing references)
		//IL_0094: Unknown result type (might be due to invalid IL or missing references)
		//IL_0095: Unknown result type (might be due to invalid IL or missing references)
		//IL_009a: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c5: Unknown result type (might be due to invalid IL or missing references)
		float currentScale = DynamicResolutionHandler.instance.GetCurrentScale();
		ctx.cmd.SetGlobalFloat(ShaderID._DRSScale, currentScale);
		ctx.cmd.SetGlobalFloat(ShaderID._DRSScaleSquared, currentScale * currentScale);
		ScriptableRenderContext renderContext = ctx.renderContext;
		CommandBuffer cmd = ctx.cmd;
		ScriptableRenderContext renderContext2 = ctx.renderContext;
		CoreUtils.DrawRendererList(renderContext, cmd, ((ScriptableRenderContext)(ref renderContext2)).CreateRendererList(CreateOpaqueRendererListDesc(ctx.cameraCullingResults, ctx.hdCamera.camera, HDShaderPassNames.s_ForwardOnlyName, (PerObjectData)0, HDRenderQueue.k_RenderQueue_AfterDRSOpaque)));
		ScriptableRenderContext renderContext3 = ctx.renderContext;
		CommandBuffer cmd2 = ctx.cmd;
		renderContext2 = ctx.renderContext;
		CoreUtils.DrawRendererList(renderContext3, cmd2, ((ScriptableRenderContext)(ref renderContext2)).CreateRendererList(CreateTransparentRendererListDesc(ctx.cameraCullingResults, ctx.hdCamera.camera, HDShaderPassNames.s_ForwardOnlyName, (PerObjectData)0, HDRenderQueue.k_RenderQueue_AfterDRSTransparent)));
	}

	protected unsafe override void Execute(CustomPassContext ctx)
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Expected O, but got Unknown
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		CheckResource();
		ProfilingScope val = default(ProfilingScope);
		((ProfilingScope)(ref val))._002Ector(ctx.cmd, new ProfilingSampler("Outlines and World UI Pass"));
		try
		{
			DrawOutlineMeshes(ctx);
			CoreUtils.SetRenderTarget(ctx.cmd, ctx.cameraColorBuffer, (ClearFlag)0, 0, (CubemapFace)(-1), -1);
			DrawAfterDRSObjects(ctx);
			m_OutlineProperties.SetTexture(ShaderID._OutlineBuffer, RTHandle.op_Implicit(m_OutlineBuffer));
			CoreUtils.DrawFullScreen(ctx.cmd, m_FullscreenOutline, m_OutlineProperties, 0);
		}
		finally
		{
			((IDisposable)(*(ProfilingScope*)(&val))/*cast due to .constrained prefix*/).Dispose();
		}
	}

	protected override void Cleanup()
	{
		ReleaseResources();
	}
}

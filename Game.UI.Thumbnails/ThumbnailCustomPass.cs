using UnityEngine;
using UnityEngine.Experimental.Rendering;
using UnityEngine.Rendering;
using UnityEngine.Rendering.HighDefinition;
using UnityEngine.Rendering.RendererUtils;

namespace Game.UI.Thumbnails;

public class ThumbnailCustomPass : CustomPass
{
	public LayerMask m_ThumbnailLayer = LayerMask.op_Implicit(0);

	private ShaderTagId[] m_ShaderTags;

	private RTHandle m_ThumbnailBuffer;

	private RTHandle m_ThumbnailDepthBuffer;

	private bool m_CanRender;

	protected override void Setup(ScriptableRenderContext renderContext, CommandBuffer cmd)
	{
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		m_ShaderTags = (ShaderTagId[])(object)new ShaderTagId[3]
		{
			new ShaderTagId("Forward"),
			new ShaderTagId("ForwardOnly"),
			new ShaderTagId("SRPDefaultUnlit")
		};
	}

	public void AllocateRTHandles(int width, int height)
	{
		//IL_00b9: Unknown result type (might be due to invalid IL or missing references)
		if (m_ThumbnailBuffer != null && (Object)(object)m_ThumbnailBuffer.rt != (Object)null && (((Texture)m_ThumbnailBuffer.rt).width != width || ((Texture)m_ThumbnailBuffer.rt).height != height))
		{
			m_ThumbnailBuffer.Release();
			m_ThumbnailBuffer = null;
			if (m_ThumbnailDepthBuffer != null)
			{
				m_ThumbnailDepthBuffer.Release();
				m_ThumbnailDepthBuffer = null;
			}
		}
		QualitySettings.GetRenderPipelineAssetAt(QualitySettings.GetQualityLevel());
		if (m_ThumbnailBuffer == null)
		{
			m_ThumbnailBuffer = RTHandles.Alloc(width, height, 1, (DepthBits)0, (GraphicsFormat)8, (FilterMode)0, (TextureWrapMode)0, (TextureDimension)2, false, false, true, false, 1, 0f, (MSAASamples)1, false, true, (RenderTextureMemoryless)0, (VRTextureUsage)0, "Thumbnail Color Buffer");
		}
		if (m_ThumbnailDepthBuffer == null)
		{
			m_ThumbnailDepthBuffer = RTHandles.Alloc(width, height, 1, (DepthBits)16, (GraphicsFormat)29, (FilterMode)0, (TextureWrapMode)0, TextureXR.dimension, false, false, true, false, 1, 0f, (MSAASamples)1, false, false, (RenderTextureMemoryless)0, (VRTextureUsage)0, "Thumbnail Depth Buffer");
		}
		m_CanRender = true;
	}

	protected override void Execute(CustomPassContext ctx)
	{
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0077: Unknown result type (might be due to invalid IL or missing references)
		//IL_0082: Unknown result type (might be due to invalid IL or missing references)
		//IL_0083: Unknown result type (might be due to invalid IL or missing references)
		//IL_008f: Unknown result type (might be due to invalid IL or missing references)
		//IL_009f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ec: Unknown result type (might be due to invalid IL or missing references)
		if (m_CanRender)
		{
			RendererListDesc val = default(RendererListDesc);
			((RendererListDesc)(ref val))._002Ector(m_ShaderTags, ctx.cullingResults, ctx.hdCamera.camera);
			val.rendererConfiguration = (PerObjectData)13;
			val.renderQueueRange = RenderQueueRange.all;
			val.sortingCriteria = (SortingCriteria)4;
			val.excludeObjectMotionVectors = true;
			val.layerMask = LayerMask.op_Implicit(m_ThumbnailLayer);
			RenderStateBlock value = default(RenderStateBlock);
			((RenderStateBlock)(ref value))._002Ector((RenderStateMask)12);
			((RenderStateBlock)(ref value)).depthState = new DepthState(true, (CompareFunction)4);
			val.stateBlock = value;
			RendererListDesc val2 = val;
			int globalInt = Shader.GetGlobalInt("colossal_InfoviewOn");
			ctx.cmd.DisableShaderKeyword("INFOVIEW_ON");
			ctx.cmd.SetGlobalInt("colossal_InfoviewOn", 0);
			CoreUtils.SetRenderTarget(ctx.cmd, m_ThumbnailBuffer, m_ThumbnailDepthBuffer, (ClearFlag)7, 0, (CubemapFace)(-1), -1);
			ScriptableRenderContext renderContext = ctx.renderContext;
			CommandBuffer cmd = ctx.cmd;
			ScriptableRenderContext renderContext2 = ctx.renderContext;
			CoreUtils.DrawRendererList(renderContext, cmd, ((ScriptableRenderContext)(ref renderContext2)).CreateRendererList(val2));
			ctx.cmd.SetGlobalInt("colossal_InfoviewOn", globalInt);
		}
	}

	public RenderTexture GetBuffer()
	{
		return m_ThumbnailBuffer.rt;
	}

	public void Release()
	{
		if (m_ThumbnailBuffer != null)
		{
			m_ThumbnailBuffer.Release();
		}
		if (m_ThumbnailDepthBuffer != null)
		{
			m_ThumbnailDepthBuffer.Release();
		}
		m_CanRender = false;
	}

	protected override void Cleanup()
	{
		Release();
	}
}

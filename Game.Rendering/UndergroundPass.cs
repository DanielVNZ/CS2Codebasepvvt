using Unity.Entities;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.HighDefinition;
using UnityEngine.Rendering.RendererUtils;

namespace Game.Rendering;

public class UndergroundPass : CustomPass
{
	private enum ComputeFlags
	{
		FadeCameraColor = 1,
		FadeNearSurface = 2,
		EmphasizeCustomColor = 4
	}

	private UndergroundViewSystem m_UndergroundViewSystem;

	private ShaderTagId[] m_ShaderTags;

	private ComputeShader m_ComputeShader;

	private Material m_ContourMaterial;

	private int m_TunnelMask;

	private int m_MarkerMask;

	private int m_PipelineMask;

	private int m_SubPipelineMask;

	private int m_CameraColorBuffer;

	private int m_UndergroundColorBuffer;

	private int m_UndergroundDepthBuffer;

	private int m_UndergroundFlags;

	private int m_UndergroundPassKernel;

	private int m_ContourPassKernel;

	protected override void Setup(ScriptableRenderContext renderContext, CommandBuffer cmd)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Invalid comparison between Unknown and I4
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		//IL_0076: Unknown result type (might be due to invalid IL or missing references)
		//IL_007d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0082: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b1: Expected O, but got Unknown
		foreach (World item in World.All)
		{
			if (item.IsCreated && (item.Flags & 9) == 9)
			{
				m_UndergroundViewSystem = item.GetExistingSystemManaged<UndergroundViewSystem>();
				if (m_UndergroundViewSystem != null)
				{
					break;
				}
			}
		}
		m_ShaderTags = (ShaderTagId[])(object)new ShaderTagId[3]
		{
			HDShaderPassNames.s_ForwardName,
			HDShaderPassNames.s_ForwardOnlyName,
			HDShaderPassNames.s_SRPDefaultUnlitName
		};
		m_ComputeShader = Resources.Load<ComputeShader>("UndergroundPass");
		m_ContourMaterial = new Material(Resources.Load<Shader>("TerrainHeights"));
		m_TunnelMask = (1 << LayerMask.NameToLayer("Tunnel")) | (1 << LayerMask.NameToLayer("Moving"));
		m_MarkerMask = 1 << LayerMask.NameToLayer("Marker");
		m_PipelineMask = 1 << LayerMask.NameToLayer("Pipeline");
		m_SubPipelineMask = 1 << LayerMask.NameToLayer("SubPipeline");
		m_CameraColorBuffer = Shader.PropertyToID("_CameraColorBuffer");
		m_UndergroundColorBuffer = Shader.PropertyToID("_UndergroundColorBuffer");
		m_UndergroundDepthBuffer = Shader.PropertyToID("_UndergroundDepthBuffer");
		m_UndergroundFlags = Shader.PropertyToID("_UndergroundFlags");
		m_UndergroundPassKernel = m_ComputeShader.FindKernel("UndergroundPass");
		m_ContourPassKernel = m_ComputeShader.FindKernel("ContourPass");
	}

	protected override void AggregateCullingParameters(ref ScriptableCullingParameters cullingParameters, HDCamera hdCamera)
	{
		if (m_UndergroundViewSystem == null)
		{
			return;
		}
		if (m_UndergroundViewSystem.tunnelsOn)
		{
			((ScriptableCullingParameters)(ref cullingParameters)).cullingMask = ((ScriptableCullingParameters)(ref cullingParameters)).cullingMask | (uint)m_TunnelMask;
			if (m_UndergroundViewSystem.markersOn)
			{
				((ScriptableCullingParameters)(ref cullingParameters)).cullingMask = ((ScriptableCullingParameters)(ref cullingParameters)).cullingMask | (uint)m_MarkerMask;
			}
		}
		if (m_UndergroundViewSystem.pipelinesOn)
		{
			((ScriptableCullingParameters)(ref cullingParameters)).cullingMask = ((ScriptableCullingParameters)(ref cullingParameters)).cullingMask | (uint)m_PipelineMask;
		}
		if (m_UndergroundViewSystem.subPipelinesOn)
		{
			((ScriptableCullingParameters)(ref cullingParameters)).cullingMask = ((ScriptableCullingParameters)(ref cullingParameters)).cullingMask | (uint)m_SubPipelineMask;
		}
	}

	protected override void Execute(CustomPassContext ctx)
	{
		//IL_01dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01fa: Unknown result type (might be due to invalid IL or missing references)
		//IL_01fb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0200: Unknown result type (might be due to invalid IL or missing references)
		//IL_0214: Unknown result type (might be due to invalid IL or missing references)
		//IL_021b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0220: Unknown result type (might be due to invalid IL or missing references)
		//IL_0229: Unknown result type (might be due to invalid IL or missing references)
		//IL_0238: Unknown result type (might be due to invalid IL or missing references)
		//IL_0251: Unknown result type (might be due to invalid IL or missing references)
		//IL_0253: Unknown result type (might be due to invalid IL or missing references)
		//IL_0436: Unknown result type (might be due to invalid IL or missing references)
		//IL_0442: Unknown result type (might be due to invalid IL or missing references)
		//IL_0454: Unknown result type (might be due to invalid IL or missing references)
		//IL_0455: Unknown result type (might be due to invalid IL or missing references)
		//IL_045a: Unknown result type (might be due to invalid IL or missing references)
		//IL_046e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0475: Unknown result type (might be due to invalid IL or missing references)
		//IL_047a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0483: Unknown result type (might be due to invalid IL or missing references)
		//IL_0492: Unknown result type (might be due to invalid IL or missing references)
		//IL_029e: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_02be: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_02cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e4: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ef: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f0: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f5: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f9: Unknown result type (might be due to invalid IL or missing references)
		//IL_02fb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0305: Unknown result type (might be due to invalid IL or missing references)
		//IL_0315: Unknown result type (might be due to invalid IL or missing references)
		//IL_0325: Unknown result type (might be due to invalid IL or missing references)
		//IL_0332: Unknown result type (might be due to invalid IL or missing references)
		//IL_034c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0356: Unknown result type (might be due to invalid IL or missing references)
		//IL_036e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0379: Unknown result type (might be due to invalid IL or missing references)
		//IL_0383: Unknown result type (might be due to invalid IL or missing references)
		//IL_039b: Unknown result type (might be due to invalid IL or missing references)
		//IL_03a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_03b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_03ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_008f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0095: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f6: Unknown result type (might be due to invalid IL or missing references)
		//IL_0101: Unknown result type (might be due to invalid IL or missing references)
		//IL_0107: Unknown result type (might be due to invalid IL or missing references)
		//IL_0114: Unknown result type (might be due to invalid IL or missing references)
		//IL_012e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0138: Unknown result type (might be due to invalid IL or missing references)
		//IL_0150: Unknown result type (might be due to invalid IL or missing references)
		//IL_015b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0165: Unknown result type (might be due to invalid IL or missing references)
		//IL_017d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0188: Unknown result type (might be due to invalid IL or missing references)
		//IL_0192: Unknown result type (might be due to invalid IL or missing references)
		//IL_04c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_04c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_0507: Unknown result type (might be due to invalid IL or missing references)
		//IL_0517: Unknown result type (might be due to invalid IL or missing references)
		//IL_0527: Unknown result type (might be due to invalid IL or missing references)
		//IL_052d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0538: Unknown result type (might be due to invalid IL or missing references)
		//IL_054c: Unknown result type (might be due to invalid IL or missing references)
		//IL_054d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0552: Unknown result type (might be due to invalid IL or missing references)
		//IL_0558: Unknown result type (might be due to invalid IL or missing references)
		//IL_0559: Unknown result type (might be due to invalid IL or missing references)
		//IL_055e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0562: Unknown result type (might be due to invalid IL or missing references)
		//IL_0564: Unknown result type (might be due to invalid IL or missing references)
		//IL_056e: Unknown result type (might be due to invalid IL or missing references)
		//IL_057e: Unknown result type (might be due to invalid IL or missing references)
		//IL_058e: Unknown result type (might be due to invalid IL or missing references)
		//IL_059b: Unknown result type (might be due to invalid IL or missing references)
		//IL_05b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_05bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_05d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_05e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_05ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_0604: Unknown result type (might be due to invalid IL or missing references)
		//IL_060f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0619: Unknown result type (might be due to invalid IL or missing references)
		//IL_0631: Unknown result type (might be due to invalid IL or missing references)
		if (m_UndergroundViewSystem == null || (!m_UndergroundViewSystem.tunnelsOn && !m_UndergroundViewSystem.pipelinesOn && !m_UndergroundViewSystem.subPipelinesOn && !m_UndergroundViewSystem.contourLinesOn))
		{
			return;
		}
		ComputeFlags computeFlags = (m_UndergroundViewSystem.undergroundOn ? ((ComputeFlags)5) : ((ComputeFlags)0));
		if (m_UndergroundViewSystem.contourLinesOn)
		{
			TerrainSurface validSurface = TerrainSurface.GetValidSurface();
			CBTSubdivisionTerrainEngine cbt = validSurface.cbt;
			if (cbt != null && cbt.IsValid && (Object)(object)validSurface.material != (Object)null)
			{
				CoreUtils.SetRenderTarget(ctx.cmd, ctx.customColorBuffer.Value, ctx.customDepthBuffer.Value, (ClearFlag)7, 0, (CubemapFace)(-1), -1);
				TerrainRenderingParameters val = HDRenderPipeline.PrepareTerrainRenderingParameters(((Component)ctx.hdCamera.camera).transform.position, validSurface);
				m_ContourMaterial.CopyPropertiesFromMaterial(validSurface.material);
				val.terrainMaterial = m_ContourMaterial;
				HDRenderPipeline.RenderTerrainSurfaceCBT(ctx.cmd, 0, validSurface, ctx.hdCamera.camera, val);
				Texture val2 = RTHandle.op_Implicit(ctx.cameraColorBuffer);
				ctx.cmd.SetComputeTextureParam(m_ComputeShader, m_ContourPassKernel, m_CameraColorBuffer, RenderTargetIdentifier.op_Implicit(val2));
				ctx.cmd.SetComputeTextureParam(m_ComputeShader, m_ContourPassKernel, m_UndergroundColorBuffer, RTHandle.op_Implicit(ctx.customColorBuffer.Value));
				ctx.cmd.SetComputeTextureParam(m_ComputeShader, m_ContourPassKernel, m_UndergroundDepthBuffer, RTHandle.op_Implicit(ctx.customDepthBuffer.Value));
				ctx.cmd.DispatchCompute(m_ComputeShader, m_ContourPassKernel, val2.width + 15 >> 4, val2.height + 15 >> 4, 1);
			}
		}
		RendererListDesc val3 = default(RendererListDesc);
		ScriptableRenderContext renderContext2;
		if (m_UndergroundViewSystem.tunnelsOn)
		{
			RenderStateBlock value = default(RenderStateBlock);
			((RenderStateBlock)(ref value))._002Ector((RenderStateMask)4);
			((RenderStateBlock)(ref value)).depthState = DepthState.defaultValue;
			((RenderStateBlock)(ref value)).stencilState = StencilState.defaultValue;
			((RendererListDesc)(ref val3))._002Ector(m_ShaderTags, ctx.cullingResults, ctx.hdCamera.camera);
			val3.rendererConfiguration = (PerObjectData)13;
			val3.renderQueueRange = RenderQueueRange.all;
			val3.sortingCriteria = (SortingCriteria)59;
			val3.excludeObjectMotionVectors = false;
			val3.stateBlock = value;
			val3.layerMask = m_TunnelMask;
			RendererListDesc val4 = val3;
			if (m_UndergroundViewSystem.markersOn)
			{
				val4.layerMask |= m_MarkerMask;
			}
			if (m_UndergroundViewSystem.pipelinesOn && !m_UndergroundViewSystem.subPipelinesOn)
			{
				val4.layerMask |= m_PipelineMask;
			}
			ctx.cmd.EnableShaderKeyword("DECALS_OFF");
			ctx.cmd.DisableShaderKeyword("DECALS_4RT");
			CoreUtils.SetRenderTarget(ctx.cmd, ctx.customColorBuffer.Value, ctx.customDepthBuffer.Value, (ClearFlag)7, 0, (CubemapFace)(-1), -1);
			ScriptableRenderContext renderContext = ctx.renderContext;
			CommandBuffer cmd = ctx.cmd;
			renderContext2 = ctx.renderContext;
			CoreUtils.DrawRendererList(renderContext, cmd, ((ScriptableRenderContext)(ref renderContext2)).CreateRendererList(val4));
			ctx.cmd.EnableShaderKeyword("DECALS_4RT");
			ctx.cmd.DisableShaderKeyword("DECALS_OFF");
			Texture val5 = RTHandle.op_Implicit(ctx.cameraColorBuffer);
			ctx.cmd.SetComputeTextureParam(m_ComputeShader, m_UndergroundPassKernel, m_CameraColorBuffer, RenderTargetIdentifier.op_Implicit(val5));
			ctx.cmd.SetComputeTextureParam(m_ComputeShader, m_UndergroundPassKernel, m_UndergroundColorBuffer, RTHandle.op_Implicit(ctx.customColorBuffer.Value));
			ctx.cmd.SetComputeTextureParam(m_ComputeShader, m_UndergroundPassKernel, m_UndergroundDepthBuffer, RTHandle.op_Implicit(ctx.customDepthBuffer.Value));
			ctx.cmd.SetComputeIntParam(m_ComputeShader, m_UndergroundFlags, (int)(computeFlags | ComputeFlags.FadeNearSurface));
			ctx.cmd.DispatchCompute(m_ComputeShader, m_UndergroundPassKernel, val5.width + 15 >> 4, val5.height + 15 >> 4, 1);
			computeFlags &= (ComputeFlags)(-2);
		}
		if (m_UndergroundViewSystem.subPipelinesOn || (m_UndergroundViewSystem.pipelinesOn && !m_UndergroundViewSystem.tunnelsOn))
		{
			RenderStateBlock value2 = default(RenderStateBlock);
			((RenderStateBlock)(ref value2))._002Ector((RenderStateMask)4);
			((RenderStateBlock)(ref value2)).depthState = DepthState.defaultValue;
			((RenderStateBlock)(ref value2)).stencilState = StencilState.defaultValue;
			((RendererListDesc)(ref val3))._002Ector(m_ShaderTags, ctx.cullingResults, ctx.hdCamera.camera);
			val3.rendererConfiguration = (PerObjectData)13;
			val3.renderQueueRange = RenderQueueRange.all;
			val3.sortingCriteria = (SortingCriteria)59;
			val3.excludeObjectMotionVectors = false;
			val3.stateBlock = value2;
			val3.layerMask = (m_UndergroundViewSystem.pipelinesOn ? (m_PipelineMask | m_SubPipelineMask) : m_SubPipelineMask);
			RendererListDesc val6 = val3;
			if (m_UndergroundViewSystem.pipelinesOn)
			{
				val6.layerMask |= m_PipelineMask;
			}
			if (m_UndergroundViewSystem.subPipelinesOn)
			{
				val6.layerMask |= m_SubPipelineMask;
			}
			ctx.cmd.EnableShaderKeyword("DECALS_OFF");
			ctx.cmd.DisableShaderKeyword("DECALS_4RT");
			CoreUtils.SetRenderTarget(ctx.cmd, ctx.customColorBuffer.Value, ctx.customDepthBuffer.Value, (ClearFlag)7, 0, (CubemapFace)(-1), -1);
			ScriptableRenderContext renderContext3 = ctx.renderContext;
			CommandBuffer cmd2 = ctx.cmd;
			renderContext2 = ctx.renderContext;
			CoreUtils.DrawRendererList(renderContext3, cmd2, ((ScriptableRenderContext)(ref renderContext2)).CreateRendererList(val6));
			ctx.cmd.EnableShaderKeyword("DECALS_4RT");
			ctx.cmd.DisableShaderKeyword("DECALS_OFF");
			Texture val7 = RTHandle.op_Implicit(ctx.cameraColorBuffer);
			ctx.cmd.SetComputeTextureParam(m_ComputeShader, m_UndergroundPassKernel, m_CameraColorBuffer, RenderTargetIdentifier.op_Implicit(val7));
			ctx.cmd.SetComputeTextureParam(m_ComputeShader, m_UndergroundPassKernel, m_UndergroundColorBuffer, RTHandle.op_Implicit(ctx.customColorBuffer.Value));
			ctx.cmd.SetComputeTextureParam(m_ComputeShader, m_UndergroundPassKernel, m_UndergroundDepthBuffer, RTHandle.op_Implicit(ctx.customDepthBuffer.Value));
			ctx.cmd.SetComputeIntParam(m_ComputeShader, m_UndergroundFlags, (int)computeFlags);
			ctx.cmd.DispatchCompute(m_ComputeShader, m_UndergroundPassKernel, val7.width + 15 >> 4, val7.height + 15 >> 4, 1);
		}
	}

	protected override void Cleanup()
	{
	}
}

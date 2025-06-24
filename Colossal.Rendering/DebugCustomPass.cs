using System.Collections.Generic;
using Game.Rendering;
using Game.SceneFlow;
using Game.Settings;
using Game.Simulation;
using Game.UI.Debug;
using Unity.Entities;
using UnityEngine;
using UnityEngine.Experimental.Rendering;
using UnityEngine.Rendering;
using UnityEngine.Rendering.HighDefinition;

namespace Colossal.Rendering;

public class DebugCustomPass : CustomPass
{
	public enum TextureDebugMode
	{
		None,
		SelectionOutlines,
		HeightMap,
		HeightMapCascades,
		TerrainOverlay,
		SplatMap,
		WaterDepth,
		WaterVelocity,
		WaterPolution,
		SnowAccumulation,
		RainAccumulation,
		WaterRawVelocity,
		Wind,
		TerrainTesselation,
		WaterSurfaceSpectrum,
		WaterSurfaceDisplacement,
		WaterSurfaceGradient,
		WaterSurfaceJacobianSurface,
		WaterSurfaceJacobianDeep,
		WaterSurfaceCaustics
	}

	private const int kPadding = 10;

	public const TextureDebugMode kGlobalMapStart = TextureDebugMode.HeightMap;

	public const TextureDebugMode kGlobalMapEnd = TextureDebugMode.Wind;

	public const TextureDebugMode kWaterSimulationMapStart = TextureDebugMode.WaterSurfaceSpectrum;

	public const TextureDebugMode kWaterSimulationMapEnd = TextureDebugMode.WaterSurfaceCaustics;

	private Material m_DebugBlitMaterial;

	private MaterialPropertyBlock m_MaterialPropertyBlock;

	private ComputeBuffer m_TopViewRenderIndirectArgs;

	private Material m_TopViewMaterial;

	private RTHandle m_TopViewRenderTexture;

	public int activeInstance { get; set; }

	public int sliceIndex { get; set; }

	public float debugOverlayRatio { get; set; } = 1f / 3f;

	public TextureDebugMode textureDebugMode { get; set; }

	public float zoom { get; set; }

	public bool showExtra { get; set; }

	public float minValue { get; set; }

	public float maxValue { get; set; }

	public float GetDefaultMinValue()
	{
		return textureDebugMode switch
		{
			TextureDebugMode.WaterRawVelocity => 0.5f, 
			TextureDebugMode.WaterVelocity => 0.03f, 
			_ => GetMinValue(), 
		};
	}

	public float GetDefaultMaxValue()
	{
		if (textureDebugMode == TextureDebugMode.WaterVelocity)
		{
			return 0.5f;
		}
		return GetMaxValue();
	}

	public float GetMinValue()
	{
		_ = textureDebugMode;
		return 0f;
	}

	public float GetMaxValue()
	{
		if (textureDebugMode == TextureDebugMode.WaterDepth)
		{
			return 4096f;
		}
		return 1f;
	}

	public bool HasExtra()
	{
		if (textureDebugMode == TextureDebugMode.WaterVelocity)
		{
			return true;
		}
		return false;
	}

	protected override void Setup(ScriptableRenderContext renderContext, CommandBuffer cmd)
	{
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Expected O, but got Unknown
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Expected O, but got Unknown
		m_DebugBlitMaterial = CoreUtils.CreateEngineMaterial(Shader.Find("Hidden/BH/CustomPass/DebugBlitQuad"));
		m_MaterialPropertyBlock = new MaterialPropertyBlock();
		m_TopViewRenderIndirectArgs = new ComputeBuffer(8, 4, (ComputeBufferType)256);
		m_TopViewMaterial = CoreUtils.CreateEngineMaterial(HDRenderPipelineGlobalSettings.instance.renderPipelineResources.shaders.terrainCBTTopViewDebug);
	}

	protected override void Cleanup()
	{
		CoreUtils.Destroy((Object)(object)m_DebugBlitMaterial);
		RTHandles.Release(m_TopViewRenderTexture);
		m_TopViewRenderIndirectArgs.Dispose();
		CoreUtils.Destroy((Object)(object)m_TopViewMaterial);
	}

	private static T GetSystem<T>() where T : ComponentSystemBase
	{
		return World.DefaultGameObjectInjectionWorld.GetOrCreateSystemManaged<T>();
	}

	private static float RemToPxScale(HDCamera hdCamera)
	{
		InterfaceSettings interfaceSettings = GameManager.instance?.settings?.userInterface;
		if (interfaceSettings != null && interfaceSettings.interfaceScaling)
		{
			if ((double)((Rect)(ref hdCamera.finalViewport)).height > 0.5625 * (double)((Rect)(ref hdCamera.finalViewport)).width)
			{
				return ((Rect)(ref hdCamera.finalViewport)).width / 1920f;
			}
			return ((Rect)(ref hdCamera.finalViewport)).height / 1080f;
		}
		return 1f;
	}

	private static int GetRuntimeDebugPanelWidth(HDCamera hdCamera)
	{
		int num = (int)((float)(GetSystem<DebugUISystem>().visible ? 610 : 10) * RemToPxScale(hdCamera));
		return Mathf.Min(hdCamera.actualWidth, num);
	}

	private static int GetRuntimePadding(HDCamera hdCamera)
	{
		return (int)(10f * RemToPxScale(hdCamera));
	}

	private static T GetCustomPass<T>(string passName, CustomPassInjectionPoint injectionPoint) where T : CustomPass
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		List<CustomPassVolume> list = new List<CustomPassVolume>();
		CustomPassVolume.GetActivePassVolumes(injectionPoint, list);
		foreach (CustomPassVolume item in list)
		{
			foreach (CustomPass customPass in item.customPasses)
			{
				if (customPass.name == passName)
				{
					T val = (T)(object)((customPass is T) ? customPass : null);
					if (val != null)
					{
						return val;
					}
				}
			}
		}
		return default(T);
	}

	public bool SetupTexture(out Texture tex, out int sliceCount)
	{
		//IL_0302: Unknown result type (might be due to invalid IL or missing references)
		//IL_029c: Unknown result type (might be due to invalid IL or missing references)
		//IL_02cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_0335: Unknown result type (might be due to invalid IL or missing references)
		//IL_0368: Unknown result type (might be due to invalid IL or missing references)
		//IL_0260: Unknown result type (might be due to invalid IL or missing references)
		//IL_039b: Unknown result type (might be due to invalid IL or missing references)
		//IL_03ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_03fa: Unknown result type (might be due to invalid IL or missing references)
		//IL_043a: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_014b: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_021d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0451: Unknown result type (might be due to invalid IL or missing references)
		//IL_0457: Invalid comparison between Unknown and I4
		//IL_045b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0461: Invalid comparison between Unknown and I4
		Vector4 val = default(Vector4);
		((Vector4)(ref val))._002Ector(minValue, 1f / (maxValue - minValue));
		sliceCount = 0;
		tex = null;
		switch (textureDebugMode)
		{
		case TextureDebugMode.WaterSurfaceSpectrum:
			if (WaterSurface.instanceCount > 0)
			{
				tex = RTHandle.op_Implicit(WaterSurface.instancesAsArray[activeInstance].simulation.gpuBuffers.phillipsSpectrumBuffer);
				m_MaterialPropertyBlock.SetInt("_Slice", sliceIndex);
				m_MaterialPropertyBlock.SetVector("_ValidRange", new Vector4(minValue, maxValue));
			}
			break;
		case TextureDebugMode.WaterSurfaceDisplacement:
			if (WaterSurface.instanceCount > 0)
			{
				tex = RTHandle.op_Implicit(WaterSurface.instancesAsArray[activeInstance].simulation.gpuBuffers.displacementBuffer);
				m_MaterialPropertyBlock.SetInt("_Slice", sliceIndex);
				m_MaterialPropertyBlock.SetVector("_ValidRange", new Vector4(minValue, maxValue));
			}
			break;
		case TextureDebugMode.WaterSurfaceGradient:
		case TextureDebugMode.WaterSurfaceJacobianSurface:
		case TextureDebugMode.WaterSurfaceJacobianDeep:
			if (WaterSurface.instanceCount > 0)
			{
				tex = RTHandle.op_Implicit(WaterSurface.instancesAsArray[activeInstance].simulation.gpuBuffers.additionalDataBuffer);
				m_MaterialPropertyBlock.SetInt("_Slice", sliceIndex);
				m_MaterialPropertyBlock.SetVector("_ValidRange", new Vector4(minValue, maxValue));
			}
			break;
		case TextureDebugMode.WaterSurfaceCaustics:
			if (WaterSurface.instanceCount > 0)
			{
				tex = RTHandle.op_Implicit(WaterSurface.instancesAsArray[activeInstance].simulation.gpuBuffers.causticsBuffer);
				m_MaterialPropertyBlock.SetInt("_Slice", sliceIndex);
				m_MaterialPropertyBlock.SetVector("_ValidRange", new Vector4(minValue, maxValue));
			}
			break;
		case TextureDebugMode.WaterVelocity:
			tex = GetSystem<WaterRenderSystem>().waterTexture;
			m_MaterialPropertyBlock.SetInt("_Slice", 0);
			m_MaterialPropertyBlock.SetVector("_ValidRange", new Vector4(minValue, maxValue));
			break;
		case TextureDebugMode.HeightMapCascades:
			tex = GetSystem<TerrainSystem>().GetCascadeTexture();
			m_MaterialPropertyBlock.SetInt("_Slice", sliceIndex);
			m_MaterialPropertyBlock.SetVector("_ValidRange", val);
			break;
		case TextureDebugMode.TerrainOverlay:
			tex = GetSystem<TerrainRenderSystem>().overrideOverlaymap;
			m_MaterialPropertyBlock.SetInt("_Slice", 0);
			m_MaterialPropertyBlock.SetVector("_ValidRange", val);
			break;
		case TextureDebugMode.HeightMap:
			tex = GetSystem<TerrainSystem>().heightmap;
			m_MaterialPropertyBlock.SetInt("_Slice", 0);
			m_MaterialPropertyBlock.SetVector("_ValidRange", val);
			break;
		case TextureDebugMode.SplatMap:
			tex = GetSystem<TerrainMaterialSystem>().splatmap;
			m_MaterialPropertyBlock.SetInt("_Slice", 0);
			m_MaterialPropertyBlock.SetVector("_ValidRange", val);
			break;
		case TextureDebugMode.WaterDepth:
		case TextureDebugMode.WaterPolution:
		case TextureDebugMode.WaterRawVelocity:
			tex = GetSystem<WaterRenderSystem>().waterTexture;
			m_MaterialPropertyBlock.SetInt("_Slice", 0);
			m_MaterialPropertyBlock.SetVector("_ValidRange", val);
			break;
		case TextureDebugMode.SnowAccumulation:
		case TextureDebugMode.RainAccumulation:
			tex = (Texture)(object)GetSystem<SnowSystem>().SnowDepth;
			m_MaterialPropertyBlock.SetInt("_Slice", 0);
			m_MaterialPropertyBlock.SetVector("_ValidRange", val);
			break;
		case TextureDebugMode.Wind:
			tex = (Texture)(object)GetSystem<WindTextureSystem>().WindTexture;
			m_MaterialPropertyBlock.SetInt("_Slice", 0);
			m_MaterialPropertyBlock.SetVector("_ValidRange", val);
			break;
		case TextureDebugMode.TerrainTesselation:
			tex = GetDebugTesselationTexture();
			m_MaterialPropertyBlock.SetInt("_Slice", 0);
			m_MaterialPropertyBlock.SetVector("_ValidRange", val);
			break;
		case TextureDebugMode.SelectionOutlines:
		{
			OutlinesWorldUIPass customPass = GetCustomPass<OutlinesWorldUIPass>("Outlines Pass", (CustomPassInjectionPoint)3);
			if (customPass != null)
			{
				tex = RTHandle.op_Implicit(customPass.outlineBuffer);
				m_MaterialPropertyBlock.SetInt("_Slice", 0);
				m_MaterialPropertyBlock.SetVector("_ValidRange", val);
			}
			break;
		}
		default:
			tex = null;
			break;
		}
		if ((Object)(object)tex != (Object)null)
		{
			if ((int)tex.dimension == 5 || (int)tex.dimension == 3)
			{
				Texture obj = tex;
				Texture2DArray val2 = (Texture2DArray)(object)((obj is Texture2DArray) ? obj : null);
				if (val2 != null)
				{
					sliceCount = val2.depth - 1;
				}
				Texture obj2 = tex;
				Texture3D val3 = (Texture3D)(object)((obj2 is Texture3D) ? obj2 : null);
				if (val3 != null)
				{
					sliceCount = val3.depth - 1;
				}
				Texture obj3 = tex;
				RenderTexture val4 = (RenderTexture)(object)((obj3 is RenderTexture) ? obj3 : null);
				if (val4 != null)
				{
					sliceCount = val4.volumeDepth - 1;
				}
			}
			return true;
		}
		return false;
	}

	private Texture GetDebugTesselationTexture()
	{
		//IL_0086: Unknown result type (might be due to invalid IL or missing references)
		TerrainSurface validSurface = TerrainSurface.GetValidSurface();
		if ((Object)(object)validSurface != (Object)null)
		{
			ComputeBuffer cameraCbtBuffer = validSurface.GetCameraCbtBuffer(Camera.main);
			if (cameraCbtBuffer != null)
			{
				ComputeShader terrainCBTTopViewDispatchDebug = HDRenderPipelineGlobalSettings.instance.renderPipelineResources.shaders.terrainCBTTopViewDispatchDebug;
				terrainCBTTopViewDispatchDebug.SetBuffer(0, "u_CbtBuffer", cameraCbtBuffer);
				terrainCBTTopViewDispatchDebug.SetBuffer(0, "u_DrawCommand", m_TopViewRenderIndirectArgs);
				terrainCBTTopViewDispatchDebug.Dispatch(0, 1, 1, 1);
				Graphics.SetRenderTarget(RTHandle.op_Implicit(m_TopViewRenderTexture));
				GL.Clear(true, true, new Color(0.8f, 0.8f, 0.8f, 1f));
				m_TopViewMaterial.SetBuffer("u_CbtBuffer", cameraCbtBuffer);
				m_TopViewMaterial.SetPass(0);
				bool wireframe = GL.wireframe;
				GL.wireframe = true;
				Graphics.DrawProceduralIndirectNow((MeshTopology)0, m_TopViewRenderIndirectArgs, 0);
				GL.wireframe = wireframe;
				Graphics.SetRenderTarget((RenderTexture)null);
				return RTHandle.op_Implicit(m_TopViewRenderTexture);
			}
		}
		return null;
	}

	private void CheckResources(int size)
	{
		if (m_TopViewRenderTexture == null || ((Texture)m_TopViewRenderTexture.rt).width != size || ((Texture)m_TopViewRenderTexture.rt).height != size)
		{
			RTHandle topViewRenderTexture = m_TopViewRenderTexture;
			if (topViewRenderTexture != null)
			{
				topViewRenderTexture.Release();
			}
			m_TopViewRenderTexture = RTHandles.Alloc(size, size, 1, (DepthBits)0, (GraphicsFormat)4, (FilterMode)0, (TextureWrapMode)0, (TextureDimension)2, false, false, true, false, 1, 0f, (MSAASamples)8, false, false, (RenderTextureMemoryless)0, (VRTextureUsage)0, "CBTTopDownView");
		}
	}

	protected override void Execute(CustomPassContext ctx)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Invalid comparison between Unknown and I4
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cb: Unknown result type (might be due to invalid IL or missing references)
		if ((int)ctx.hdCamera.camera.cameraType == 1)
		{
			float num = debugOverlayRatio;
			int num2 = (int)((Rect)(ref ctx.hdCamera.finalViewport)).width;
			int num3 = (int)((Rect)(ref ctx.hdCamera.finalViewport)).height;
			int num4 = (int)((float)Mathf.Min(num2, num3) * num);
			Rect viewportSize = default(Rect);
			((Rect)(ref viewportSize))._002Ector((float)GetRuntimeDebugPanelWidth(ctx.hdCamera), (float)(num3 - num4 - GetRuntimePadding(ctx.hdCamera)), (float)num4, (float)num4);
			CheckResources(num4);
			m_MaterialPropertyBlock.Clear();
			m_MaterialPropertyBlock.SetFloat("_Zoom", zoom);
			m_MaterialPropertyBlock.SetInt("_ShowExtra", showExtra ? 1 : 0);
			bool applyExposure = false;
			if (SetupTexture(out var tex, out var _))
			{
				DisplayTexture(ctx.cmd, viewportSize, tex, m_DebugBlitMaterial, (int)textureDebugMode, m_MaterialPropertyBlock, applyExposure);
			}
		}
	}

	private static void DisplayTexture(CommandBuffer cmd, Rect viewportSize, Texture texture, Material debugMaterial, int mode, MaterialPropertyBlock mpb, bool applyExposure)
	{
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Invalid comparison between Unknown and I4
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		mpb.SetFloat(HDShaderIDs._ApplyExposure, applyExposure ? 1f : 0f);
		mpb.SetFloat(HDShaderIDs._Mipmap, 0f);
		mpb.SetTexture(HDShaderIDs._InputTexture, texture);
		mpb.SetInt("_Mode", mode);
		if ((int)texture.dimension == 5)
		{
			cmd.EnableShaderKeyword("TEXTURE_SOURCE_ARRAY");
		}
		else
		{
			cmd.DisableShaderKeyword("TEXTURE_SOURCE_ARRAY");
		}
		cmd.SetViewport(viewportSize);
		cmd.DrawProcedural(Matrix4x4.identity, debugMaterial, 0, (MeshTopology)0, 3, 1, mpb);
	}
}

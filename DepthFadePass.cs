using UnityEngine;
using UnityEngine.Experimental.Rendering;
using UnityEngine.Rendering;
using UnityEngine.Rendering.HighDefinition;

internal class DepthFadePass : CustomPass
{
	private static class ShaderID
	{
		public static readonly int _LinearDepthTarget = Shader.PropertyToID("_LinearDepthTarget");

		public static readonly int _LinearDepth = Shader.PropertyToID("_LinearDepth");

		public static readonly int _DepthHistory = Shader.PropertyToID("_DepthHistory");

		public static readonly int _DepthHistoryTarget = Shader.PropertyToID("_DepthHistoryTarget");

		public static readonly int _ShaderVariables = Shader.PropertyToID("_ShaderVariablesDepthFadePass");

		public static readonly int _GlobalDepthFadeTex = Shader.PropertyToID("_DepthFadeTex");
	}

	private struct ShaderVariablesDepthFadePass
	{
		public Vector4 _TextureSizes;

		public float _MotionAdaptation;

		public float _DepthAdaptationThreshold;
	}

	public float m_MotionAdaptation = 2f;

	public float m_DepthAdaptationThreshold = 0.7f;

	private RTHandle m_LinearDepthBuffer;

	private RTHandle[] m_HistoryBuffers;

	private int currentBuffer;

	private ComputeShader m_ComputeShader;

	private int m_LinearizePassKernel;

	private int m_HistoryPassKernel;

	protected override void Setup(ScriptableRenderContext renderContext, CommandBuffer cmd)
	{
		CreateResources();
		m_ComputeShader = Resources.Load<ComputeShader>("DepthFadePass");
		m_LinearizePassKernel = m_ComputeShader.FindKernel("LinearizePass");
		m_HistoryPassKernel = m_ComputeShader.FindKernel("HistoryPass");
	}

	protected override void Execute(CustomPassContext ctx)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		//IL_007b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00de: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e8: Unknown result type (might be due to invalid IL or missing references)
		//IL_0100: Unknown result type (might be due to invalid IL or missing references)
		//IL_010a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0122: Unknown result type (might be due to invalid IL or missing references)
		//IL_0170: Unknown result type (might be due to invalid IL or missing references)
		//IL_0175: Unknown result type (might be due to invalid IL or missing references)
		//IL_0198: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e0: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		if (!ctx.hdCamera.RequiresCameraJitter())
		{
			ReleaseResources();
			ctx.cmd.DisableShaderKeyword("DEPTH_FADE_FROM_TEXTURE");
			return;
		}
		CreateResources();
		int num = (((Texture)m_LinearDepthBuffer.rt).width + 7) / 8;
		int num2 = (((Texture)m_LinearDepthBuffer.rt).height + 7) / 8;
		ctx.cmd.SetComputeTextureParam(m_ComputeShader, m_LinearizePassKernel, ShaderID._LinearDepthTarget, RTHandle.op_Implicit(m_LinearDepthBuffer));
		ctx.cmd.DispatchCompute(m_ComputeShader, m_LinearizePassKernel, num, num2, 1);
		RTHandle val = m_HistoryBuffers[currentBuffer];
		currentBuffer = (currentBuffer + 1) % 2;
		RTHandle val2 = m_HistoryBuffers[currentBuffer];
		ctx.cmd.SetComputeTextureParam(m_ComputeShader, m_HistoryPassKernel, ShaderID._LinearDepth, RTHandle.op_Implicit(m_LinearDepthBuffer));
		ctx.cmd.SetComputeTextureParam(m_ComputeShader, m_HistoryPassKernel, ShaderID._DepthHistory, RTHandle.op_Implicit(val));
		ctx.cmd.SetComputeTextureParam(m_ComputeShader, m_HistoryPassKernel, ShaderID._DepthHistoryTarget, RTHandle.op_Implicit(val2));
		ShaderVariablesDepthFadePass shaderVariablesDepthFadePass = new ShaderVariablesDepthFadePass
		{
			_TextureSizes = new Vector4((float)((Texture)val.rt).width, (float)((Texture)val.rt).height, (float)((Texture)m_LinearDepthBuffer.rt).width, (float)((Texture)m_LinearDepthBuffer.rt).height),
			_MotionAdaptation = m_MotionAdaptation,
			_DepthAdaptationThreshold = m_DepthAdaptationThreshold
		};
		ConstantBuffer.Push<ShaderVariablesDepthFadePass>(ctx.cmd, ref shaderVariablesDepthFadePass, m_ComputeShader, ShaderID._ShaderVariables);
		ctx.cmd.DispatchCompute(m_ComputeShader, m_HistoryPassKernel, num, num2, 1);
		ctx.cmd.SetGlobalTexture(ShaderID._GlobalDepthFadeTex, RTHandle.op_Implicit(val2));
		ctx.cmd.EnableShaderKeyword("DEPTH_FADE_FROM_TEXTURE");
	}

	protected override void Cleanup()
	{
		ReleaseResources();
	}

	private void CreateResources()
	{
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		if (m_LinearDepthBuffer == null)
		{
			m_LinearDepthBuffer = RTHandles.Alloc(Vector2.one, TextureXR.slices, (DepthBits)0, (GraphicsFormat)49, (FilterMode)0, (TextureWrapMode)0, TextureXR.dimension, true, false, true, false, 1, 0f, (MSAASamples)1, false, true, (RenderTextureMemoryless)0, (VRTextureUsage)0, "DepthFade Intermediate Linear Depth");
		}
		if (m_HistoryBuffers == null)
		{
			m_HistoryBuffers = (RTHandle[])(object)new RTHandle[2];
			for (int i = 0; i < m_HistoryBuffers.Length; i++)
			{
				m_HistoryBuffers[i] = RTHandles.Alloc(Vector2.one, TextureXR.slices, (DepthBits)0, (GraphicsFormat)8, (FilterMode)0, (TextureWrapMode)0, TextureXR.dimension, true, false, true, false, 1, 0f, (MSAASamples)1, false, false, (RenderTextureMemoryless)0, (VRTextureUsage)0, "DepthFade History " + i);
			}
		}
	}

	private void ReleaseResources()
	{
		if (m_LinearDepthBuffer != null)
		{
			m_LinearDepthBuffer.Release();
			m_LinearDepthBuffer = null;
		}
		if (m_HistoryBuffers == null)
		{
			return;
		}
		for (int i = 0; i < m_HistoryBuffers.Length; i++)
		{
			RTHandle obj = m_HistoryBuffers[i];
			if (obj != null)
			{
				obj.Release();
			}
			m_HistoryBuffers[i] = null;
		}
		m_HistoryBuffers = null;
	}
}

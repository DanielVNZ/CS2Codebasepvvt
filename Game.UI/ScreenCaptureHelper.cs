using System;
using System.Threading.Tasks;
using Colossal;
using Colossal.IO.AssetDatabase;
using Colossal.Logging;
using Colossal.UI;
using Game.Rendering;
using Game.UI.Menu;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using UnityEngine;
using UnityEngine.Experimental.Rendering;
using UnityEngine.Rendering;
using UnityEngine.Rendering.HighDefinition;

namespace Game.UI;

public static class ScreenCaptureHelper
{
	public class AsyncRequest
	{
		private readonly TaskCompletionSource<bool> m_TaskCompletionSource;

		private NativeArray<byte> m_Data;

		public int width { get; }

		public int height { get; }

		public GraphicsFormat format { get; }

		public ref NativeArray<byte> result => ref m_Data;

		public AsyncRequest(Texture previewTexture)
		{
			//IL_0020: Unknown result type (might be due to invalid IL or missing references)
			//IL_0025: Unknown result type (might be due to invalid IL or missing references)
			//IL_0037: Unknown result type (might be due to invalid IL or missing references)
			//IL_0047: Unknown result type (might be due to invalid IL or missing references)
			//IL_004c: Unknown result type (might be due to invalid IL or missing references)
			//IL_009b: Unknown result type (might be due to invalid IL or missing references)
			width = previewTexture.width;
			height = previewTexture.height;
			format = previewTexture.graphicsFormat;
			int num = TextureUtils.ComputeMipchainSize(previewTexture.width, previewTexture.height, previewTexture.graphicsFormat, 1);
			m_Data = new NativeArray<byte>(num, (Allocator)4, (NativeArrayOptions)0);
			log.DebugFormat("Issued request {0}x{1} size: {2}", (object)width, (object)height, (object)num);
			m_TaskCompletionSource = new TaskCompletionSource<bool>();
			AsyncGPUReadback.RequestIntoNativeArray<byte>(ref m_Data, previewTexture, 0, (Action<AsyncGPUReadbackRequest>)OnCompleted);
		}

		private void OnCompleted(AsyncGPUReadbackRequest request)
		{
			if (width == ((AsyncGPUReadbackRequest)(ref request)).width && height == ((AsyncGPUReadbackRequest)(ref request)).height && m_Data.Length == ((AsyncGPUReadbackRequest)(ref request)).layerDataSize)
			{
				if (!((AsyncGPUReadbackRequest)(ref request)).done)
				{
					log.ErrorFormat("Waiting for request {0}x{1} size: {2}. This should never happen!", (object)((AsyncGPUReadbackRequest)(ref request)).width, (object)((AsyncGPUReadbackRequest)(ref request)).height, (object)((AsyncGPUReadbackRequest)(ref request)).layerDataSize);
					((AsyncGPUReadbackRequest)(ref request)).WaitForCompletion();
				}
				if (((AsyncGPUReadbackRequest)(ref request)).done && ((AsyncGPUReadbackRequest)(ref request)).hasError)
				{
					log.ErrorFormat("Request failed {0}x{1} size: {2}. Result will be incorrect.", (object)((AsyncGPUReadbackRequest)(ref request)).width, (object)((AsyncGPUReadbackRequest)(ref request)).height, (object)((AsyncGPUReadbackRequest)(ref request)).layerDataSize);
				}
				m_TaskCompletionSource.SetResult(result: true);
			}
			else
			{
				log.WarnFormat("Request failed {0}x{1} size: {2}. Completed successfully but not matching any request.", (object)((AsyncGPUReadbackRequest)(ref request)).width, (object)((AsyncGPUReadbackRequest)(ref request)).height, (object)((AsyncGPUReadbackRequest)(ref request)).layerDataSize);
				m_TaskCompletionSource.SetResult(result: false);
			}
		}

		public async Task Dispose()
		{
			log.DebugFormat("Manual release of request {0}x{1}. Probably due to an error.", (object)width, (object)height);
			await Complete();
			m_Data.Dispose();
		}

		public Task Complete()
		{
			return m_TaskCompletionSource.Task;
		}
	}

	private static ILog log = LogManager.GetLogger("SceneFlow");

	private const string kOutlinesPassName = "Outlines Pass";

	public static RenderTexture CreateRenderTarget(string name, int width, int height, GraphicsFormat format = (GraphicsFormat)8)
	{
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Expected O, but got Unknown
		RenderTexture val = new RenderTexture(width, height, 0, format, 0)
		{
			name = name,
			hideFlags = (HideFlags)61
		};
		val.Create();
		return val;
	}

	public static void CaptureScreenshot(Camera camera, RenderTexture destination, MenuHelpers.SaveGamePreviewSettings settings)
	{
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Expected O, but got Unknown
		//IL_00db: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e2: Expected O, but got Unknown
		if (!((Object)(object)destination == (Object)null) && !((Object)(object)camera == (Object)null))
		{
			JobHandle punctualLightsJobHandle = HDRPDotsInputs.punctualLightsJobHandle;
			((JobHandle)(ref punctualLightsJobHandle)).Complete();
			RenderPipelineAsset renderPipelineAssetAt = QualitySettings.GetRenderPipelineAssetAt(QualitySettings.GetQualityLevel());
			ColorBufferFormat colorBufferFormat = ((HDRenderPipelineAsset)((renderPipelineAssetAt is HDRenderPipelineAsset) ? renderPipelineAssetAt : null)).currentPlatformRenderPipelineSettings.colorBufferFormat;
			RenderTexture val = new RenderTexture(((Texture)destination).width, ((Texture)destination).height, 16, (GraphicsFormat)colorBufferFormat, 0);
			RenderingSystem existingSystemManaged = World.DefaultGameObjectInjectionWorld.GetExistingSystemManaged<RenderingSystem>();
			CustomPassCache.SetPassEnabled("Outlines Pass", false);
			UIManager.defaultUISystem.enabled = false;
			existingSystemManaged.hideOverlay = true;
			RenderTexture targetTexture = camera.targetTexture;
			camera.forceIntoRenderTexture = true;
			camera.targetTexture = val;
			for (int i = 0; i < 8; i++)
			{
				camera.Render();
			}
			camera.targetTexture = targetTexture;
			camera.forceIntoRenderTexture = false;
			existingSystemManaged.hideOverlay = false;
			UIManager.defaultUISystem.enabled = true;
			CustomPassCache.SetPassEnabled("Outlines Pass", true);
			Material val2 = new Material(Shader.Find("Hidden/ScreenCaptureCompose"));
			if (settings.stylized)
			{
				val2.EnableKeyword("STYLIZE");
			}
			else
			{
				val2.DisableKeyword("STYLIZE");
			}
			val2.SetFloat("_Radius", settings.stylizedRadius);
			TextureAsset overlayImage = settings.overlayImage;
			if ((AssetData)(object)overlayImage != (IAssetData)null)
			{
				val2.SetTexture("_Overlay", overlayImage.Load(0));
			}
			Graphics.Blit((Texture)(object)val, destination, val2, 0);
			if ((AssetData)(object)overlayImage != (IAssetData)null)
			{
				((AssetData)overlayImage).Unload(false);
			}
			Object.Destroy((Object)(object)val2);
			((Texture)destination).IncrementUpdateCount();
			Object.Destroy((Object)(object)val);
		}
	}
}

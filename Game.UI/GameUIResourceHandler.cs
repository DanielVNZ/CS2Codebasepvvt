using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using cohtml.Net;
using Colossal.PSI.Common;
using Colossal.UI;
using Game.Prefabs;
using Game.SceneFlow;
using Game.UI.Menu;
using Game.UI.Thumbnails;
using Unity.Entities;
using UnityEngine;
using UnityEngine.Experimental.Rendering;
using UnityEngine.SceneManagement;

namespace Game.UI;

public class GameUIResourceHandler : DefaultResourceHandler
{
	protected class GameResourceRequestData : ResourceRequestData
	{
		public bool IsThumbnailRequest => ((RequestData)this).UriBuilder.Scheme == "thumbnail";

		public bool IsScreenshotRequest => ((RequestData)this).UriBuilder.Scheme == "screencapture";

		public bool IsUserAvatarRequest => ((RequestData)this).UriBuilder.Scheme == "useravatar";

		public GameResourceRequestData(IResourceRequest request, IResourceResponse response)
			: base(request, response)
		{
		}
	}

	private const string kScreencaptureScheme = "screencapture";

	public const string kScreencaptureProtocol = "screencapture://";

	public const string kScreenshotOpString = "Screenshot";

	private const string kThumbnailScheme = "thumbnail";

	public const string kThumbnailProtocol = "thumbnail://";

	private const string kUserAvatarScheme = "useravatar";

	public const string kUserAvatarProtocol = "useravatar://";

	private Dictionary<string, Camera> m_HostCameraCache = new Dictionary<string, Camera>();

	public GameUIResourceHandler(MonoBehaviour coroutineHost)
	{
		((DefaultResourceHandler)this).coroutineHost = coroutineHost;
	}

	public override void OnResourceRequest(IResourceRequest request, IResourceResponse response)
	{
		try
		{
			DefaultResourceHandler.log.TraceFormat("OnResourceRequest {0}", (object)request.GetURL());
			GameResourceRequestData requestData = new GameResourceRequestData(request, response);
			((DefaultResourceHandler)this).coroutineHost.StartCoroutine(TryGetResourceRequestAsync(requestData));
		}
		catch (Exception ex)
		{
			response.Finish((Status)1);
			DefaultResourceHandler.log.Error(ex, (object)("URL: " + request.GetURL()));
		}
	}

	private Camera GetCameraFromHost(string host)
	{
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		if (m_HostCameraCache.TryGetValue(host, out var value))
		{
			if ((Object)(object)value != (Object)null && host == ((Component)value).tag.ToLowerInvariant())
			{
				return value;
			}
			m_HostCameraCache.Remove(host);
		}
		value = null;
		for (int i = 0; i < SceneManager.sceneCount; i++)
		{
			Scene sceneAt = SceneManager.GetSceneAt(i);
			List<GameObject> list = new List<GameObject>(((Scene)(ref sceneAt)).rootCount);
			((Scene)(ref sceneAt)).GetRootGameObjects(list);
			foreach (GameObject item in list)
			{
				Camera[] componentsInChildren = item.GetComponentsInChildren<Camera>(true);
				foreach (Camera val in componentsInChildren)
				{
					if (host == ((Component)val).tag.ToLowerInvariant())
					{
						value = val;
						break;
					}
				}
			}
		}
		if ((Object)(object)value != (Object)null)
		{
			m_HostCameraCache.Add(host, value);
		}
		return value;
	}

	private RenderTexture SetupCameraTarget(string name, Camera camera, int width, int height)
	{
		RenderTexture targetTexture = camera.targetTexture;
		if (((Object)targetTexture).name == string.Empty)
		{
			((Object)targetTexture).name = name;
		}
		camera.targetTexture = null;
		targetTexture.Release();
		((Texture)targetTexture).width = width;
		((Texture)targetTexture).height = height;
		targetTexture.Create();
		camera.targetTexture = targetTexture;
		return targetTexture;
	}

	private IEnumerator RequestScreenshot(GameResourceRequestData requestData)
	{
		((DefaultResourceHandler)this).AddPendingRequest((RequestData)(object)requestData);
		yield return (object)new WaitForEndOfFrame();
		if (((RequestData)requestData).Aborted)
		{
			yield break;
		}
		try
		{
			Camera cameraFromHost = GetCameraFromHost(((RequestData)requestData).UriBuilder.Host);
			if ((Object)(object)cameraFromHost != (Object)null)
			{
				UrlQuery val = new UrlQuery(((RequestData)requestData).UriBuilder.Query);
				int pixelWidth = default(int);
				if (!val.Read("width", ref pixelWidth))
				{
					pixelWidth = cameraFromHost.pixelWidth;
				}
				int pixelHeight = default(int);
				if (!val.Read("height", ref pixelHeight))
				{
					pixelHeight = cameraFromHost.pixelHeight;
				}
				string text = default(string);
				if (!val.Read("op", ref text))
				{
					text = null;
				}
				ResourceType val2 = default(ResourceType);
				if (!val.Read<ResourceType>("alloc", ref val2))
				{
					val2 = (ResourceType)0;
				}
				bool flag = false;
				bool flag2 = default(bool);
				if (val.Read("liveView", ref flag2))
				{
					flag = flag2;
				}
				MenuHelpers.SaveGamePreviewSettings saveGamePreviewSettings = new MenuHelpers.SaveGamePreviewSettings();
				saveGamePreviewSettings.FromUri(val);
				string text2 = Uri.UnescapeDataString(((RequestData)requestData).UriBuilder.Path.Substring(1));
				Texture val3;
				if (text == "Screenshot")
				{
					val3 = ((DefaultResourceHandler)this).userImagesManager.GetUserImageTarget(text2, pixelWidth, pixelHeight, default(Rect));
					if ((Object)(object)val3 == (Object)null)
					{
						val3 = (Texture)(object)ScreenCaptureHelper.CreateRenderTarget(text2, pixelWidth, pixelHeight, (GraphicsFormat)8);
					}
					RenderTexture val4 = (RenderTexture)(object)((val3 is RenderTexture) ? val3 : null);
					if (val4 != null)
					{
						ScreenCaptureHelper.CaptureScreenshot(cameraFromHost, val4, saveGamePreviewSettings);
					}
				}
				else
				{
					val3 = ((DefaultResourceHandler)this).userImagesManager.GetUserImageTarget(text2, pixelWidth, pixelHeight, default(Rect));
					if ((Object)(object)val3 == (Object)null)
					{
						val3 = (Texture)(object)SetupCameraTarget(text2, cameraFromHost, pixelWidth, pixelHeight);
					}
				}
				if ((Object)(object)val3 != (Object)null)
				{
					((ResourceRequestData)requestData).ReceiveUserImage(((DefaultResourceHandler)this).userImagesManager.GetUserImageData(val3, val2, flag, default(Rect), (AlphaPremultiplicationMode)0));
					((DefaultResourceHandler)this).RespondWithSuccess((RequestData)(object)requestData);
					((DefaultResourceHandler)this).RemovePendingRequest((RequestData)(object)requestData);
				}
				else
				{
					((RequestData)requestData).Error = "No available render target for '" + ((RequestData)requestData).UriBuilder.Host + "'";
					((DefaultResourceHandler)this).CheckForFailedRequest((RequestData)(object)requestData);
				}
			}
			else
			{
				((RequestData)requestData).Error = "No camera '" + ((RequestData)requestData).UriBuilder.Host + "'";
				((DefaultResourceHandler)this).CheckForFailedRequest((RequestData)(object)requestData);
			}
		}
		catch (Exception ex)
		{
			((RequestData)requestData).Error = ex.ToString();
			((DefaultResourceHandler)this).CheckForFailedRequest((RequestData)(object)requestData);
		}
	}

	private IEnumerator TryGetResourceRequestAsync(GameResourceRequestData requestData)
	{
		DefaultResourceHandler.log.Debug((object)$"Requesting resource with URL: {((RequestData)requestData).UriBuilder.Uri}");
		if (((RequestData)requestData).IsDataRequest)
		{
			((DefaultResourceHandler)this).RespondWithSuccess((RequestData)(object)requestData);
			yield return 0;
			((DefaultResourceHandler)this).RemovePendingRequest((RequestData)(object)requestData);
		}
		else if (requestData.IsScreenshotRequest)
		{
			yield return RequestScreenshot(requestData);
		}
		else if (requestData.IsThumbnailRequest)
		{
			yield return TryThumbnailRequestAsync(requestData);
		}
		else if (requestData.IsUserAvatarRequest)
		{
			yield return RequestUserAvatarAsync(requestData);
		}
		else
		{
			yield return ((DefaultResourceHandler)this).TryPreloadedResourceRequestAsync((ResourceRequestData)(object)requestData);
		}
	}

	private bool UpdateTexture(ref Texture target, string name, (int width, int height, byte[] data) p)
	{
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Expected O, but got Unknown
		if ((Object)(object)target == (Object)null)
		{
			Texture2D val = new Texture2D(p.width, p.height, (GraphicsFormat)8, (TextureCreationFlags)0)
			{
				name = name,
				hideFlags = (HideFlags)61
			};
			val.LoadRawTextureData(p.data);
			val.Apply();
			((Texture)val).IncrementUpdateCount();
			target = (Texture)(object)val;
			return true;
		}
		Texture obj = target;
		Texture2D val2 = (Texture2D)(object)((obj is Texture2D) ? obj : null);
		if (val2 != null)
		{
			val2.LoadRawTextureData(p.data);
			val2.Apply();
			((Texture)val2).IncrementUpdateCount();
			return true;
		}
		return false;
	}

	private IEnumerator RequestUserAvatarAsync(GameResourceRequestData requestData)
	{
		((DefaultResourceHandler)this).AddPendingRequest((RequestData)(object)requestData);
		AvatarSize val = default(AvatarSize);
		new UrlQuery(((RequestData)requestData).UriBuilder.Query).Read<AvatarSize>("size", ref val);
		string path = ((RequestData)requestData).UriBuilder.Path;
		string name = Uri.UnescapeDataString(path.Substring(1, path.Length - 1));
		Task<(int width, int height, byte[] data)> avatarTask = PlatformManager.instance.GetAvatar(val);
		yield return (object)new WaitUntil((Func<bool>)(() => avatarTask.IsCompleted));
		if (((RequestData)requestData).Aborted || avatarTask.IsFaulted)
		{
			yield break;
		}
		(int, int, byte[]) result = avatarTask.Result;
		if (result.Item3 == null)
		{
			((RequestData)requestData).Error = "Getting user avatar failed.";
			((DefaultResourceHandler)this).CheckForFailedRequest((RequestData)(object)requestData);
			yield break;
		}
		Texture target = ((DefaultResourceHandler)this).userImagesManager.GetUserImageTarget(name, result.Item1, result.Item2, default(Rect));
		if (UpdateTexture(ref target, name, result))
		{
			((ResourceRequestData)requestData).ReceiveUserImage(((DefaultResourceHandler)this).userImagesManager.GetUserImageData(target, (ResourceType)0, false, default(Rect), (AlphaPremultiplicationMode)0));
			((DefaultResourceHandler)this).RespondWithSuccess((RequestData)(object)requestData);
			((DefaultResourceHandler)this).RemovePendingRequest((RequestData)(object)requestData);
		}
		else
		{
			((RequestData)requestData).Error = "No available render target.";
			((DefaultResourceHandler)this).CheckForFailedRequest((RequestData)(object)requestData);
		}
	}

	private IEnumerator TryThumbnailRequestAsync(GameResourceRequestData requestData)
	{
		ThumbnailCache thumbnailCache = GameManager.instance?.thumbnailCache;
		if (thumbnailCache != null)
		{
			Camera cameraFromHost = GetCameraFromHost(((RequestData)requestData).UriBuilder.Host);
			ThumbnailCache.ThumbnailInfo info = null;
			try
			{
				bool flag = (Object)(object)cameraFromHost != (Object)null;
				UrlQuery val = new UrlQuery(((RequestData)requestData).UriBuilder.Query);
				int width = default(int);
				if (!val.Read("width", ref width))
				{
					width = (flag ? cameraFromHost.pixelWidth : 0);
				}
				int height = default(int);
				if (!val.Read("height", ref height))
				{
					height = (flag ? cameraFromHost.pixelHeight : 0);
				}
				string text = ((RequestData)requestData).UriBuilder.Path.Substring(1);
				string[] array = text.Split('/', StringSplitOptions.None);
				if (array.Length != 2)
				{
					throw new ArgumentException("Invalid url path {0}", text);
				}
				string type = Uri.UnescapeDataString(array[0]);
				string name = Uri.UnescapeDataString(array[1]);
				PrefabSystem orCreateSystemManaged = World.DefaultGameObjectInjectionWorld.GetOrCreateSystemManaged<PrefabSystem>();
				PrefabID id = new PrefabID(type, name);
				if (orCreateSystemManaged.TryGetPrefab(id, out var prefab))
				{
					info = thumbnailCache.GetThumbnail(prefab, width, height, cameraFromHost);
				}
			}
			catch (Exception ex)
			{
				((RequestData)requestData).Error = ex.ToString();
				((DefaultResourceHandler)this).CheckForFailedRequest((RequestData)(object)requestData);
				yield break;
			}
			((DefaultResourceHandler)this).AddPendingRequest((RequestData)(object)requestData);
			if (info != null)
			{
				while (info.status == ThumbnailCache.Status.Pending)
				{
					if (((RequestData)requestData).Aborted)
					{
						yield break;
					}
					yield return 0;
				}
			}
			if (((RequestData)requestData).Aborted)
			{
				yield break;
			}
			if (info == null || info.status == ThumbnailCache.Status.Unavailable)
			{
				((RequestData)requestData).Error = $"Thumbnail not found {((RequestData)requestData).UriBuilder.Uri}";
				((RequestData)requestData).IsHandled = true;
				((DefaultResourceHandler)this).CheckForFailedRequest((RequestData)(object)requestData);
				yield break;
			}
			try
			{
				Texture texture = (Texture)(object)info.atlasFrame.texture;
				Rect region = info.region;
				if ((Object)(object)texture != (Object)null)
				{
					((ResourceRequestData)requestData).ReceiveUserImage(((DefaultResourceHandler)this).userImagesManager.GetUserImageData(texture, (ResourceType)1, true, region, (AlphaPremultiplicationMode)1));
					((DefaultResourceHandler)this).RespondWithSuccess((RequestData)(object)requestData);
					((DefaultResourceHandler)this).RemovePendingRequest((RequestData)(object)requestData);
				}
				else
				{
					((RequestData)requestData).Error = $"Thumbnail not found {((RequestData)requestData).UriBuilder.Uri}";
					((DefaultResourceHandler)this).CheckForFailedRequest((RequestData)(object)requestData);
				}
			}
			catch (Exception ex2)
			{
				((RequestData)requestData).Error = ex2.ToString();
				((DefaultResourceHandler)this).CheckForFailedRequest((RequestData)(object)requestData);
			}
		}
		else
		{
			((RequestData)requestData).Error = "Thumbnails are not available at this time '" + ((RequestData)requestData).UriBuilder.Host + "'";
			((DefaultResourceHandler)this).CheckForFailedRequest((RequestData)(object)requestData);
		}
	}
}

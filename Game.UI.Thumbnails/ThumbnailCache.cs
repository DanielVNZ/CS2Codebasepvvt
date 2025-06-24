using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Colossal;
using Colossal.IO.AssetDatabase;
using UnityEngine;

namespace Game.UI.Thumbnails;

public class ThumbnailCache : IDisposable
{
	public enum Status
	{
		Ready,
		Pending,
		Unavailable,
		Refresh
	}

	public class ThumbnailInfo : IEquatable<ThumbnailInfo>
	{
		public object baseObjectRef;

		public Camera camera;

		public AtlasFrame atlasFrame;

		public Rect region;

		public Status status;

		public static bool operator ==(ThumbnailInfo left, ThumbnailInfo right)
		{
			return object.Equals(left, right);
		}

		public static bool operator !=(ThumbnailInfo left, ThumbnailInfo right)
		{
			return !object.Equals(left, right);
		}

		public override bool Equals(object obj)
		{
			if (obj is ThumbnailInfo other)
			{
				return Equals(other);
			}
			return false;
		}

		public bool Equals(ThumbnailInfo other)
		{
			//IL_0015: Unknown result type (might be due to invalid IL or missing references)
			//IL_001a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0033: Unknown result type (might be due to invalid IL or missing references)
			//IL_0038: Unknown result type (might be due to invalid IL or missing references)
			//IL_004c: Unknown result type (might be due to invalid IL or missing references)
			//IL_004d: Unknown result type (might be due to invalid IL or missing references)
			object obj = baseObjectRef;
			Camera val = camera;
			AtlasFrame val2 = atlasFrame;
			Rect val3 = region;
			object obj2 = other.baseObjectRef;
			Camera val4 = other.camera;
			AtlasFrame val5 = other.atlasFrame;
			Rect val6 = other.region;
			if (obj == obj2 && (Object)(object)val == (Object)(object)val4 && val2 == val5)
			{
				return val3 == val6;
			}
			return false;
		}

		public override int GetHashCode()
		{
			return (baseObjectRef, camera, atlasFrame, ((object)System.Runtime.CompilerServices.Unsafe.As<Rect, Rect>(ref region)/*cast due to .constrained prefix*/).GetHashCode()).GetHashCode();
		}
	}

	public readonly struct ThumbnailKey : IEquatable<ThumbnailKey>
	{
		public string name { get; }

		public int width { get; }

		public int height { get; }

		public ThumbnailKey(string name, int width, int height)
		{
			this.name = name;
			this.width = width;
			this.height = height;
		}

		public static bool operator ==(ThumbnailKey left, ThumbnailKey right)
		{
			return object.Equals(left, right);
		}

		public static bool operator !=(ThumbnailKey left, ThumbnailKey right)
		{
			return !object.Equals(left, right);
		}

		public override bool Equals(object obj)
		{
			if (obj is ThumbnailKey other)
			{
				return Equals(other);
			}
			return false;
		}

		public bool Equals(ThumbnailKey other)
		{
			string text = name;
			int num = width;
			int num2 = height;
			string text2 = other.name;
			int num3 = other.width;
			int num4 = other.height;
			if (text == text2 && num == num3)
			{
				return num2 == num4;
			}
			return false;
		}

		public override int GetHashCode()
		{
			return (name, width, height).GetHashCode();
		}

		public override string ToString()
		{
			return $"{name}_{width}x{height}";
		}
	}

	private Dictionary<ThumbnailKey, ThumbnailInfo> m_CacheData;

	public ThumbnailCache()
	{
		m_CacheData = new Dictionary<ThumbnailKey, ThumbnailInfo>();
	}

	private void OnAtlasAssetChanged(AssetChangedEventArgs args)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Invalid comparison between Unknown and I4
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Invalid comparison between Unknown and I4
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_006e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0078: Expected O, but got Unknown
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0064: Invalid comparison between Unknown and I4
		//IL_007b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0081: Invalid comparison between Unknown and I4
		//IL_008b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0095: Expected O, but got Unknown
		if ((int)((AssetChangedEventArgs)(ref args)).change == 2)
		{
			m_CacheData.Clear();
			{
				foreach (AtlasAsset asset in AssetDatabase.global.GetAssets<AtlasAsset>(default(SearchFilter<AtlasAsset>)))
				{
					LoadAtlas(asset);
				}
				return;
			}
		}
		if ((int)((AssetChangedEventArgs)(ref args)).change == 3 || (int)((AssetChangedEventArgs)(ref args)).change == 5)
		{
			LoadAtlas((AtlasAsset)((AssetChangedEventArgs)(ref args)).asset);
		}
		else if ((int)((AssetChangedEventArgs)(ref args)).change == 4)
		{
			UnloadAtlas((AtlasAsset)((AssetChangedEventArgs)(ref args)).asset);
		}
	}

	private void LoadAtlas(AtlasAsset asset)
	{
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		try
		{
			AtlasFrame atlasFrame = asset.Load();
			foreach (Entry item in asset)
			{
				Dictionary<ThumbnailKey, ThumbnailInfo> cacheData = m_CacheData;
				string name = item.name;
				Rect region = item.region;
				int width = (int)((Rect)(ref region)).width;
				region = item.region;
				cacheData.Add(new ThumbnailKey(name, width, (int)((Rect)(ref region)).height), new ThumbnailInfo
				{
					atlasFrame = atlasFrame,
					region = item.region,
					status = Status.Ready
				});
			}
		}
		catch (Exception ex)
		{
			Debug.LogException(ex);
		}
	}

	private void UnloadAtlas(AtlasAsset asset)
	{
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		try
		{
			try
			{
				asset.Load();
				foreach (Entry item in asset)
				{
					Dictionary<ThumbnailKey, ThumbnailInfo> cacheData = m_CacheData;
					string name = item.name;
					Rect region = item.region;
					int width = (int)((Rect)(ref region)).width;
					region = item.region;
					cacheData.Remove(new ThumbnailKey(name, width, (int)((Rect)(ref region)).height));
				}
				((AssetData)asset).Unload(false);
			}
			finally
			{
				((IDisposable)asset)?.Dispose();
			}
		}
		catch (Exception ex)
		{
			Debug.LogException(ex);
		}
	}

	public void Initialize()
	{
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		EventExtensions.Subscribe<AtlasAsset>(AssetDatabase.global.onAssetDatabaseChanged, (EventDelegate<AssetChangedEventArgs>)OnAtlasAssetChanged, AssetChangedEventArgs.Default);
	}

	public ThumbnailInfo GetCachedThumbnail(ThumbnailKey key)
	{
		if (m_CacheData.TryGetValue(key, out var value))
		{
			return value;
		}
		return null;
	}

	public void Refresh()
	{
		foreach (KeyValuePair<ThumbnailKey, ThumbnailInfo> cacheDatum in m_CacheData)
		{
			cacheDatum.Value.status = Status.Pending;
		}
	}

	public ThumbnailInfo GetThumbnail(object obj, int width, int height, Camera camera = null)
	{
		ThumbnailInfo value = null;
		Object val = (Object)((obj is Object) ? obj : null);
		if (val != (Object)null)
		{
			ThumbnailKey key = new ThumbnailKey(val.name, width, height);
			m_CacheData.TryGetValue(key, out value);
		}
		return value;
	}

	public void Update()
	{
	}

	public void Dispose()
	{
		foreach (KeyValuePair<ThumbnailKey, ThumbnailInfo> cacheDatum in m_CacheData)
		{
			cacheDatum.Value.baseObjectRef = null;
		}
	}
}

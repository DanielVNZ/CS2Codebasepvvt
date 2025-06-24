using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Colossal;
using Colossal.AssetPipeline.Diagnostic;
using Colossal.IO.AssetDatabase;
using Colossal.Logging;
using Colossal.PSI.Common;
using Colossal.PSI.PdxSdk;
using Game.AssetPipeline;
using Game.Assets;
using Game.PSI;
using Game.SceneFlow;
using Game.Simulation;
using Unity.Entities;

namespace Game.UI.Menu;

public class PdxAssetUploadHandle
{
	private ILog log = LogManager.GetLogger("AssetUpload");

	private PdxSdkPlatform m_Manager = PlatformManager.instance.GetPSI<PdxSdkPlatform>("PdxSdk");

	private List<AssetData> m_Screenshots = new List<AssetData>();

	private List<AssetData> m_Assets = new List<AssetData>();

	private List<AssetData> m_OriginalPreviews = new List<AssetData>();

	private Dictionary<AssetData, AssetData> m_WIPAssets = new Dictionary<AssetData, AssetData>();

	private List<AssetData> m_AdditionalAssets = new List<AssetData>();

	private HashSet<AssetData> m_CachedAssetDependencies = new HashSet<AssetData>();

	public Action onSocialProfileSynced;

	public AssetData mainAsset { get; private set; }

	public IReadOnlyList<AssetData> assets => m_Assets;

	public IReadOnlyList<AssetData> additionalAssets => m_AdditionalAssets;

	public HashSet<AssetData> cachedDependencies => m_CachedAssetDependencies;

	public bool hasPrefabAssets { get; private set; }

	public IEnumerable<AssetData> allAssets
	{
		get
		{
			foreach (AssetData asset in assets)
			{
				yield return asset;
			}
			foreach (AssetData additionalAsset in additionalAssets)
			{
				yield return additionalAsset;
			}
		}
	}

	public IReadOnlyList<AssetData> screenshots => m_Screenshots;

	public AssetData preview { get; private set; }

	public IReadOnlyList<AssetData> originalPreviews => m_OriginalPreviews;

	public ModInfo modInfo { get; set; }

	public bool updateExisting { get; set; }

	public int processVT { get; set; } = -1;

	public bool packThumbnailsAtlas { get; set; }

	public List<ModInfo> authorMods { get; private set; } = new List<ModInfo>();

	public ModTag[] availableTags { get; private set; } = Array.Empty<ModTag>();

	public DLCTag[] availableDLCs { get; private set; } = Array.Empty<DLCTag>();

	public HashSet<string> typeTags { get; private set; } = new HashSet<string>();

	public HashSet<string> tags { get; private set; } = new HashSet<string>();

	public List<string> additionalTags { get; private set; } = new List<string>();

	public int tagCount => tags.Count + additionalTags.Count;

	public bool binaryPackAssets { get; set; } = true;

	public SocialProfile socialProfile { get; private set; }

	public bool LoggedIn()
	{
		PdxSdkPlatform manager = m_Manager;
		if (manager == null)
		{
			return false;
		}
		return manager.cachedLoggedIn;
	}

	public PdxAssetUploadHandle()
	{
		Initialize();
	}

	public PdxAssetUploadHandle(AssetData mainAsset, params AssetData[] assets)
	{
		this.mainAsset = mainAsset;
		if (mainAsset != (IAssetData)null)
		{
			m_Assets.Add(mainAsset);
		}
		m_Assets.AddRange(assets);
		Initialize();
	}

	private void Initialize()
	{
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Expected O, but got Unknown
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		//IL_008d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0095: Unknown result type (might be due to invalid IL or missing references)
		//IL_00be: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c5: Unknown result type (might be due to invalid IL or missing references)
		m_Manager = PlatformManager.instance.GetPSI<PdxSdkPlatform>("PdxSdk");
		PlatformManager.instance.onPlatformRegistered += (PlatformRegisteredHandler)delegate(IPlatformServiceIntegration psi)
		{
			PdxSdkPlatform val2 = (PdxSdkPlatform)(object)((psi is PdxSdkPlatform) ? psi : null);
			if (val2 != null)
			{
				m_Manager = val2;
			}
		};
		InitializePreviews();
		ModInfo val = modInfo;
		((ModInfo)(ref val)).Clear();
		Version current = Version.current;
		object arg = ((Version)(ref current)).majorVersion;
		current = Version.current;
		val.m_RecommendedGameVersion = $"{arg}.{((Version)(ref current)).minorVersion}.*";
		AssetData obj = mainAsset;
		val.m_DisplayName = ((obj != null) ? obj.name : null);
		val.m_ExternalLinks.Add(new ExternalLinkData
		{
			m_Type = ExternalLinkInfo.kAcceptedTypes[0].m_Type,
			m_URL = string.Empty
		});
		modInfo = val;
		RebuildDependencyCache();
	}

	public async Task<ModOperationResult> BeginSubmit()
	{
		ModOperationResult val = ((!updateExisting) ? (await m_Manager.RegisterWIP(modInfo)) : (await m_Manager.RegisterExistingWIP(modInfo)));
		ModOperationResult val2 = val;
		modInfo = val2.m_ModInfo;
		if (!val2.m_Success)
		{
			return val2;
		}
		HashSet<ModDependency> hashSet = new HashSet<ModDependency>();
		var (flag, error) = CopyFiles(hashSet);
		if (!flag)
		{
			log.Error((object)error);
			await Cleanup();
			return new ModOperationResult
			{
				m_ModInfo = modInfo,
				m_Success = false,
				m_Error = new ModError
				{
					m_Details = error
				}
			};
		}
		ModInfo val3 = modInfo;
		val3.m_ModDependencies = hashSet.ToArray();
		val3.m_Tags = CollectTags();
		modInfo = val3;
		ModOperationResult updateResult = await m_Manager.UpdateWIP(modInfo);
		modInfo = updateResult.m_ModInfo;
		if (!updateResult.m_Success)
		{
			await Cleanup();
		}
		return updateResult;
	}

	public async Task<ModOperationResult> FinalizeSubmit()
	{
		ModOperationResult val = ((!updateExisting) ? (await m_Manager.PublishWIP(modInfo)) : (await m_Manager.UpdateExisting(modInfo)));
		ModOperationResult publishResult = val;
		modInfo = publishResult.m_ModInfo;
		await Cleanup();
		return publishResult;
	}

	public void ShowModsUIProfilePage()
	{
		m_Manager.onModsUIClosed += OnModsUIClosed;
		m_Manager.ShowModsUIProfilePage();
	}

	private void OnModsUIClosed()
	{
		m_Manager.onModsUIClosed -= OnModsUIClosed;
		RefreshSocialProfile();
	}

	private async void RefreshSocialProfile()
	{
		SocialProfile socialProfileResult = await m_Manager.GetSocialProfile();
		GameManager.instance.RunOnMainThread(delegate
		{
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			socialProfile = socialProfileResult;
			onSocialProfileSynced?.Invoke();
		});
	}

	public void ExcludeSourceTextures(IEnumerable<SurfaceAsset> surfaces, ILocalAssetDatabase database)
	{
		//IL_00ed: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f3: Unknown result type (might be due to invalid IL or missing references)
		Dictionary<TextureAsset, List<SurfaceAsset>> dictionary = new Dictionary<TextureAsset, List<SurfaceAsset>>();
		Dictionary<TextureAsset, List<SurfaceAsset>> dictionary2 = new Dictionary<TextureAsset, List<SurfaceAsset>>();
		foreach (SurfaceAsset surface in surfaces)
		{
			surface.LoadProperties(true);
			if (surface.isVTMaterial)
			{
				foreach (KeyValuePair<string, TextureAsset> texture in surface.textures)
				{
					if (surface.IsHandledByVirtualTexturing(texture))
					{
						AddReferenceTo(dictionary, texture.Value, surface);
					}
					else
					{
						AddReferenceTo(dictionary2, texture.Value, surface);
					}
				}
			}
			else
			{
				foreach (KeyValuePair<string, TextureAsset> texture2 in surface.textures)
				{
					AddReferenceTo(dictionary2, texture2.Value, surface);
				}
			}
			((AssetData)surface).Unload(false);
		}
		List<TextureAsset> list = ((IAssetDatabase)database).GetAssets<TextureAsset>(default(SearchFilter<TextureAsset>)).ToList();
		for (int i = 0; i < list.Count; i++)
		{
			TextureAsset val = list[i];
			if (dictionary.ContainsKey(val))
			{
				if (dictionary2.ContainsKey(val))
				{
					log.WarnFormat("Texture {0} is referenced {1} times by VT materials and {2} times by non VT materials. It will be duplicated on disk.", (object)val, (object)dictionary[val].Count, (object)dictionary2[val].Count);
					log.InfoFormat("Detail for {0}:\nvt: {1}\nnon vt: {2}", (object)val, (object)string.Join(", ", dictionary[val]), (object)string.Join(", ", dictionary2[val]));
				}
				else
				{
					log.InfoFormat($"Deleting {val}", Array.Empty<object>());
					((AssetData)val).Delete();
				}
			}
		}
		static void AddReferenceTo(Dictionary<TextureAsset, List<SurfaceAsset>> references, TextureAsset texture, SurfaceAsset surface)
		{
			if (!references.TryGetValue(texture, out var value))
			{
				value = new List<SurfaceAsset>();
				references.Add(texture, value);
			}
			value.Add(surface);
		}
	}

	private (bool, string) CopyFiles(HashSet<ModDependency> externalReferences)
	{
		//IL_007a: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00db: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_014d: Unknown result type (might be due to invalid IL or missing references)
		Directory.CreateDirectory(GetAbsoluteContentPath());
		if (allAssets.Any())
		{
			ILocalAssetDatabase transient = AssetDatabase.GetTransient(0L, (string)null);
			try
			{
				Dictionary<AssetData, AssetData> dictionary = new Dictionary<AssetData, AssetData>();
				foreach (AssetData allAsset in allAssets)
				{
					log.VerboseFormat("Copying {0} to {1}. Processed {2} references.", (object)allAsset, (object)transient, (object)dictionary.Count);
					AssetUploadUtils.CopyAsset(allAsset, transient, dictionary, externalReferences, allAsset == (IAssetData)(object)mainAsset, binaryPackAssets, modInfo.m_PublishedID);
				}
				if (processVT > -1)
				{
					SimulationSystem existingSystemManaged = World.DefaultGameObjectInjectionWorld.GetExistingSystemManaged<SimulationSystem>();
					float selectedSpeed = existingSystemManaged.selectedSpeed;
					int mipBias = processVT;
					Report val = new Report();
					ImportStep report = ((ReportBase)val).AddImportStep("Convert Selected VT");
					List<SurfaceAsset> list = ((IAssetDatabase)transient).GetAssets<SurfaceAsset>(default(SearchFilter<SurfaceAsset>)).ToList();
					AssetImportPipeline.ConvertSurfacesToVT(list, list, writeVTSettings: false, 512, 3, mipBias, force: false, report);
					AssetImportPipeline.BuildMidMipsCache(list, 512, 3, transient);
					ExcludeSourceTextures(list, transient);
					val.Log(log, (Severity)1000);
					existingSystemManaged.selectedSpeed = selectedSpeed;
				}
				if (packThumbnailsAtlas)
				{
					AssetUploadUtils.CreateThumbnailAtlas(dictionary, transient);
				}
				PackageAsset val2 = PackageAssetExtensions.AddAsset((ILocalAssetDatabase)(object)AssetDatabase<ParadoxMods>.instance, AssetDataPath.Create(GetContentPath(), modInfo.m_DisplayName, (EscapeStrategy)2), transient);
				((AssetData)val2).Save(false);
				m_WIPAssets.Add((AssetData)(object)val2, (AssetData)(object)val2);
			}
			finally
			{
				((IDisposable)transient)?.Dispose();
			}
		}
		Dictionary<AssetData, string> processed = new Dictionary<AssetData, string>();
		CopyPreview(processed);
		for (int i = 0; i < screenshots.Count; i++)
		{
			CopyScreenshot(screenshots[i], i, processed);
		}
		return (true, null);
	}

	public async Task Cleanup()
	{
		foreach (KeyValuePair<AssetData, AssetData> wIPAsset in m_WIPAssets)
		{
			AssetDatabase<ParadoxMods>.instance.DeleteAsset<AssetData>(wIPAsset.Value);
		}
		m_WIPAssets.Clear();
		modInfo = (await m_Manager.UnregisterWIP(modInfo)).m_ModInfo;
	}

	public async Task SyncPlatformData()
	{
		Task<List<ModInfo>> modsTask = m_Manager.ListAllModsByMe(typeTags.ToArray(), 20);
		Task<(ModTag[], DLCTag[])> tagsTask = m_Manager.GetTags();
		Task<SocialProfile> socialProfileTask = m_Manager.GetSocialProfile();
		await Task.WhenAll(modsTask, tagsTask, socialProfileTask);
		GameManager.instance.RunOnMainThread(delegate
		{
			//IL_008d: Unknown result type (might be due to invalid IL or missing references)
			authorMods = modsTask.Result;
			authorMods?.Sort((ModInfo a, ModInfo b) => string.Compare(a.m_DisplayName, b.m_DisplayName, StringComparison.OrdinalIgnoreCase));
			availableTags = tagsTask.Result.Item1;
			availableDLCs = tagsTask.Result.Item2;
			socialProfile = socialProfileTask.Result;
			HashSet<string> validTags = new HashSet<string>(availableTags.Select((ModTag tag) => tag.m_Id));
			(HashSet<string>, HashSet<string>) tuple = GetTags(mainAsset, validTags);
			(tags, _) = tuple;
			HashSet<string> hashSet = (typeTags = tuple.Item2);
		});
	}

	public async Task<ModInfo> GetExistingInfo()
	{
		return await m_Manager.GetDetails(modInfo);
	}

	public async Task<(bool, ModLocalData)> GetLocalData(int id)
	{
		var (success, localData) = await m_Manager.GetLocalData(id);
		if (success)
		{
			IDataSourceProvider dataSource = AssetDatabase<ParadoxMods>.instance.dataSource;
			ParadoxModsDataSource val = (ParadoxModsDataSource)(object)((dataSource is ParadoxModsDataSource) ? dataSource : null);
			if (val != null)
			{
				await val.PopulateMetadata(localData.m_AbsolutePath);
			}
		}
		return (success, localData);
	}

	private void CopyPreview(Dictionary<AssetData, string> processed)
	{
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		if (!(preview == (IAssetData)null))
		{
			string text = CopyMetadata(preview, "preview", processed);
			if (text != null)
			{
				ModInfo val = modInfo;
				val.m_ThumbnailFilename = text;
				modInfo = val;
			}
		}
	}

	private string CopyMetadata(AssetData asset, string name, Dictionary<AssetData, string> processed)
	{
		if (processed.TryGetValue(asset, out var value))
		{
			return value;
		}
		ILocalAssetDatabase transient = AssetDatabase.GetTransient(0L, (string)null);
		try
		{
			AssetData val = AssetUploadUtils.CopyPreviewImage(asset, transient, AssetDataPath.op_Implicit(name));
			value = GetFilename(val);
			using FileStream destination = LongFile.Create(GetAbsoluteMetadataPath() + "/" + value);
			using Stream stream = val.GetReadStream();
			stream.CopyTo(destination);
		}
		catch (Exception ex)
		{
			log.Error(ex);
			return null;
		}
		finally
		{
			((IDisposable)transient)?.Dispose();
		}
		processed[asset] = value;
		return value;
	}

	public void AddScreenshot(AssetData asset)
	{
		m_Screenshots.Add(asset);
	}

	public void RemoveScreenshot(AssetData asset)
	{
		m_Screenshots.Remove(asset);
	}

	public void ClearScreenshots()
	{
		m_Screenshots.Clear();
	}

	public void SetPreview(AssetData asset)
	{
		preview = asset;
	}

	private void CopyScreenshot(AssetData asset, int index, Dictionary<AssetData, string> processed)
	{
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		string text = CopyMetadata(asset, $"screenshot{index}", processed);
		if (text != null)
		{
			ModInfo val = modInfo;
			if (!val.m_ScreenshotFileNames.Contains(text))
			{
				val.m_ScreenshotFileNames.Add(text);
			}
			modInfo = val;
		}
	}

	public void SetPreviewsFromExisting(ModLocalData localData)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		//IL_0087: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
		if (localData.m_ThumbnailFilename != null)
		{
			string thumbnailPath = Path.GetFullPath(Path.Combine(localData.m_AbsolutePath, localData.m_ThumbnailFilename));
			ImageAsset val = default(ImageAsset);
			if (AssetDatabase<ParadoxMods>.instance.TryGetAsset<ImageAsset>(SearchFilter<ImageAsset>.ByCondition((Func<ImageAsset, bool>)((ImageAsset candidate) => FindByPath(candidate, thumbnailPath)), false), ref val))
			{
				SetPreview((AssetData)(object)val);
			}
		}
		if (localData.m_ScreenshotFilenames == null)
		{
			return;
		}
		ClearScreenshots();
		string[] screenshotFilenames = localData.m_ScreenshotFilenames;
		ImageAsset asset = default(ImageAsset);
		foreach (string path in screenshotFilenames)
		{
			string screenshotPath = Path.GetFullPath(Path.Combine(localData.m_AbsolutePath, path));
			if (AssetDatabase<ParadoxMods>.instance.TryGetAsset<ImageAsset>(SearchFilter<ImageAsset>.ByCondition((Func<ImageAsset, bool>)((ImageAsset candidate) => FindByPath(candidate, screenshotPath)), false), ref asset))
			{
				AddScreenshot((AssetData)(object)asset);
			}
		}
	}

	public void AddAdditionalAsset(AssetData asset)
	{
		m_AdditionalAssets.Add(asset);
		RebuildDependencyCache();
	}

	public void RemoveAdditionalAsset(AssetData asset)
	{
		m_AdditionalAssets.Remove(asset);
		RebuildDependencyCache();
	}

	private bool FindByPath(ImageAsset candidate, string imagePath)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		string fullPath = Path.GetFullPath(((AssetData)candidate).GetMeta().path);
		return imagePath.Equals(fullPath, StringComparison.OrdinalIgnoreCase);
	}

	private static (HashSet<string>, HashSet<string>) GetTags(AssetData asset, HashSet<string> validTags)
	{
		HashSet<string> item = new HashSet<string>();
		HashSet<string> item2 = new HashSet<string>();
		if (asset != (IAssetData)null)
		{
			ModTags.GetTags(asset, item2, item, validTags);
		}
		return (item2, item);
	}

	private void InitializePreviews()
	{
		if (AssetUploadUtils.TryGetPreview(mainAsset, out var result))
		{
			preview = result;
		}
		else
		{
			preview = (AssetData)(object)MenuHelpers.defaultPreview;
		}
		HashSet<AssetData> hashSet = new HashSet<AssetData>();
		foreach (AssetData asset in assets)
		{
			if (AssetUploadUtils.TryGetPreview(asset, out var result2))
			{
				hashSet.Add(result2);
			}
		}
		m_OriginalPreviews.AddRange(hashSet);
		m_Screenshots.AddRange(hashSet);
	}

	private void InitializeContentPrerequisite()
	{
		//IL_00cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e0: Unknown result type (might be due to invalid IL or missing references)
		HashSet<string> hashSet = new HashSet<string>();
		foreach (AssetData asset in assets)
		{
			if (asset is MapMetadata mapMetadata && ((Metadata<MapInfo>)mapMetadata).target.contentPrerequisites != null)
			{
				string[] contentPrerequisites = ((Metadata<MapInfo>)mapMetadata).target.contentPrerequisites;
				foreach (string item in contentPrerequisites)
				{
					hashSet.Add(item);
				}
			}
			if (asset is SaveGameMetadata saveGameMetadata && ((Metadata<SaveInfo>)saveGameMetadata).target.contentPrerequisites != null)
			{
				string[] contentPrerequisites = ((Metadata<SaveInfo>)saveGameMetadata).target.contentPrerequisites;
				foreach (string item2 in contentPrerequisites)
				{
					hashSet.Add(item2);
				}
			}
		}
		ModInfo val = modInfo;
		val.m_DLCDependencies = hashSet.ToArray();
		modInfo = val;
	}

	private string GetFilename(AssetData asset)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		SourceMeta meta = asset.GetMeta();
		return meta.fileName + meta.extension;
	}

	public void AddAdditionalTag(string tag)
	{
		additionalTags.Add(tag);
	}

	public void RemoveAdditionalTag(string tag)
	{
		additionalTags.Remove(tag);
	}

	private string[] CollectTags()
	{
		HashSet<string> hashSet = new HashSet<string>(tags);
		foreach (string additionalTag in additionalTags)
		{
			hashSet.Add(additionalTag);
		}
		return hashSet.ToArray();
	}

	private string GetMetadataPath()
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		return modInfo.m_RootPath + "/" + ModInfo.kMetadataDirectory;
	}

	private string GetContentPath()
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		return modInfo.m_RootPath + "/" + ModInfo.kContentDirectory;
	}

	public string GetAbsoluteContentPath()
	{
		return Path.Combine(m_Manager.modsRootPath, GetContentPath());
	}

	private string GetAbsoluteMetadataPath()
	{
		return Path.Combine(m_Manager.modsRootPath, GetMetadataPath());
	}

	private void RebuildDependencyCache()
	{
		m_CachedAssetDependencies.Clear();
		hasPrefabAssets = false;
		foreach (AssetData allAsset in allAssets)
		{
			PrefabAsset val = (PrefabAsset)(object)((allAsset is PrefabAsset) ? allAsset : null);
			if (val != null)
			{
				AssetUploadUtils.CollectPrefabAssetDependencies(val, m_CachedAssetDependencies, allAsset == (IAssetData)(object)mainAsset);
				hasPrefabAssets = true;
			}
			m_CachedAssetDependencies.Add(allAsset);
		}
		InitializeContentPrerequisite();
	}
}

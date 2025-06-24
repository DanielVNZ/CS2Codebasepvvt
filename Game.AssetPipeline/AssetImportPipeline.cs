using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Colossal;
using Colossal.Animations;
using Colossal.AssetPipeline;
using Colossal.AssetPipeline.Collectors;
using Colossal.AssetPipeline.Diagnostic;
using Colossal.AssetPipeline.Importers;
using Colossal.AssetPipeline.PostProcessors;
using Colossal.Collections.Generic;
using Colossal.IO.AssetDatabase;
using Colossal.IO.AssetDatabase.VirtualTexturing;
using Colossal.Json;
using Colossal.Logging;
using Colossal.PSI.Environment;
using Colossal.Reflection;
using Game.Prefabs;
using Game.Rendering;
using Game.Rendering.Debug;
using TMPro;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Mathematics;
using Unity.Profiling;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Rendering;
using UnityEngine.UI;

namespace Game.AssetPipeline;

public static class AssetImportPipeline
{
	private class ThemesConfig
	{
		public string[] themePrefixes;
	}

	private class Progress
	{
		public class ScopedThreadDescriptionObject : IDisposable
		{
			private Progress owner;

			public ScopedThreadDescriptionObject(Progress owner, string description)
			{
				this.owner = owner;
				owner.SetThreadDescription(description);
			}

			public void Dispose()
			{
				owner.SetThreadDescription(null);
			}
		}

		private string title;

		private string description;

		private string threadDescription;

		private float value;

		private Func<string, string, float, bool> progressCallback;

		private Thread mainThread;

		public bool shouldCancel { get; private set; }

		public Progress()
		{
			mainThread = Thread.CurrentThread;
		}

		public void SetHandler(Func<string, string, float, bool> progressCallback)
		{
			this.progressCallback = progressCallback;
		}

		public void Reset(Func<string, string, float, bool> progressCallback)
		{
			this.progressCallback = progressCallback;
			shouldCancel = false;
		}

		public void Set(string title, string description, float value)
		{
			this.title = title;
			this.description = description;
			Interlocked.Exchange(ref this.value, value);
			if (mainThread == Thread.CurrentThread)
			{
				Update();
			}
		}

		public ScopedThreadDescriptionObject ScopedThreadDescription(string description)
		{
			return new ScopedThreadDescriptionObject(this, description);
		}

		public void SetThreadDescription(string description)
		{
			threadDescription = description;
			if (mainThread == Thread.CurrentThread)
			{
				Update();
			}
		}

		public void Set(string title, string description)
		{
			this.title = title;
			this.description = description;
			if (mainThread == Thread.CurrentThread)
			{
				Update();
			}
		}

		public void Update()
		{
			string text = threadDescription ?? description;
			if (shouldCancel)
			{
				text += " (Canceled)";
			}
			if (progressCallback != null && progressCallback(title, text, value))
			{
				shouldCancel = true;
			}
		}
	}

	private static readonly ILog log;

	private const string kMainSettings = "settings.json";

	public static bool useParallelImport;

	public static ILocalAssetDatabase targetDatabase;

	private static readonly ProfilerMarker s_ImportPath;

	private static readonly ProfilerMarker s_ProfPostImport;

	private static readonly ProfilerMarker s_ProfImportModels;

	private static readonly ProfilerMarker s_ProfImportTextures;

	private static readonly ProfilerMarker s_ProfCreateGeomsSurfaces;

	private static readonly ProfilerMarker s_ProfImportAssetGroup;

	private static readonly ProfilerMarker s_ProfImportDidimo;

	private static readonly Progress s_Progress;

	private static MainThreadDispatcher s_MainThreadDispatcher;

	public static Action<string, Texture> OnDebugTexture;

	private static Material s_BackgroundMaterial;

	private const float kBoundSize = 0.1f;

	private const float kHalfBoundSize = 0.05f;

	public static Material backgroundMaterial
	{
		get
		{
			//IL_0017: Unknown result type (might be due to invalid IL or missing references)
			//IL_0021: Expected O, but got Unknown
			//IL_0026: Unknown result type (might be due to invalid IL or missing references)
			if ((Object)(object)s_BackgroundMaterial == (Object)null)
			{
				s_BackgroundMaterial = new Material(Shader.Find("HDRP/Unlit"));
				s_BackgroundMaterial.color = Color.white;
			}
			return s_BackgroundMaterial;
		}
	}

	static AssetImportPipeline()
	{
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0074: Unknown result type (might be due to invalid IL or missing references)
		//IL_0079: Unknown result type (might be due to invalid IL or missing references)
		log = LogManager.GetLogger("AssetPipeline");
		useParallelImport = true;
		s_ImportPath = new ProfilerMarker("AssetImportPipeline.ImportPath");
		s_ProfPostImport = new ProfilerMarker("AssetImportPipeline.PostImportMainThread");
		s_ProfImportModels = new ProfilerMarker("AssetImportPipeline.ImportModels");
		s_ProfImportTextures = new ProfilerMarker("AssetImportPipeline.ImportTextures");
		s_ProfCreateGeomsSurfaces = new ProfilerMarker("AssetImportPipeline.CreateGeometriesAndSurfaces");
		s_ProfImportAssetGroup = new ProfilerMarker("AssetImportPipeline.ImportAssetGroup");
		s_ProfImportDidimo = new ProfilerMarker("AssetImportPipeline.ImportDidimo");
		s_Progress = new Progress();
		ImporterCache.GetSupportedExtensions();
	}

	private static string GetNameWithoutGUID(string str)
	{
		return str.Substring(0, str.LastIndexOf("_"));
	}

	private static async Task ExecuteMainThreadQueue(Task importTask, Report report)
	{
		ImportStep val = ((ReportBase)report).AddImportStep("Process main thread task queue");
		try
		{
			while ((!importTask.IsCompleted || s_MainThreadDispatcher.hasPendingTasks) && !s_Progress.shouldCancel)
			{
				s_Progress.Update();
				if (s_MainThreadDispatcher.hasPendingTasks)
				{
					s_Progress.SetThreadDescription($"Executing {s_MainThreadDispatcher.pendingTasksCount} tasks");
					s_MainThreadDispatcher.ProcessTasks();
				}
				await Task.Yield();
			}
		}
		finally
		{
			((IDisposable)val)?.Dispose();
		}
		await importTask;
	}

	private static string MakeRelativePath(string path, string rootPath)
	{
		if (path.IndexOf(rootPath) == 0)
		{
			return path.Substring(rootPath.Length + 1);
		}
		throw new FormatException(path + " is not relative to " + rootPath);
	}

	public static void SetReportCallback(Func<string, string, float, bool> progressCallback)
	{
		s_Progress.Reset(progressCallback);
	}

	private static void AddSupportedThemes(string projectRootPath)
	{
		string text = projectRootPath + "/themes.json";
		if (!LongFile.Exists(text))
		{
			return;
		}
		Variant val = JSON.Load(LongFile.ReadAllText(text).Trim());
		if (val != null)
		{
			ThemesConfig themesConfig = JSON.MakeInto<ThemesConfig>(val);
			if (themesConfig.themePrefixes != null)
			{
				AssetUtils.AddSupportedThemes((IEnumerable<string>)themesConfig.themePrefixes);
				log.InfoFormat("Theme prefixes added: {0}", (object)string.Join(',', themesConfig.themePrefixes));
			}
		}
	}

	public unsafe static async Task ImportPath(string projectRootPath, IEnumerable<string> relativePaths, ImportMode importMode, bool convertToVT, Func<string, string, float, bool> progressCallback = null, IPrefabFactory prefabFactory = null)
	{
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		if (targetDatabase == null)
		{
			throw new Exception("targetDatabase must be set");
		}
		if (s_MainThreadDispatcher == null)
		{
			s_MainThreadDispatcher = new MainThreadDispatcher();
		}
		ProfilerMarker val = s_ImportPath;
		AutoScope prof = ((ProfilerMarker)(ref val)).Auto();
		try
		{
			Report report = new Report();
			int failures = 0;
			int total = 0;
			PerformanceCounter perf = PerformanceCounter.Start((Action<TimeSpan>)delegate(TimeSpan t)
			{
				log.Info((object)string.Format("Completed {0} asset groups import in {1:F3}s. Errors {2}. {3}", total, t.TotalSeconds, failures, s_Progress.shouldCancel ? "(Canceled)" : ""));
			});
			try
			{
				ImportStep val2 = ((ReportBase)report).AddImportStep("Cache importers & post processors");
				try
				{
					ImporterCache.CacheSupportedExtensions(val2);
					PostProcessorCache.CachePostProcessors(val2);
				}
				finally
				{
					((IDisposable)val2)?.Dispose();
				}
				SetReportCallback(progressCallback);
				AddSupportedThemes(projectRootPath);
				ImportStep val3 = ((ReportBase)report).AddImportStep("Collect source assets");
				SourceAssetCollector assetCollector;
				try
				{
					s_Progress.Set("Importing assets", "Collecting files...", 0f);
					assetCollector = new SourceAssetCollector(projectRootPath, relativePaths);
				}
				finally
				{
					((IDisposable)val3)?.Dispose();
				}
				ParallelOptions opts = new ParallelOptions
				{
					MaxDegreeOfParallelism = ((!useParallelImport) ? 1 : Environment.ProcessorCount)
				};
				HashSet<SurfaceAsset> VTMaterials = new HashSet<SurfaceAsset>();
				int parallelCount = 0;
				await ExecuteMainThreadQueue(Task.Run(() => Parallel.ForEach((IEnumerable<AssetGroup<Asset>>)(object)assetCollector, opts, delegate(AssetGroup<Asset> asset, ParallelLoopState state, long index)
				{
					//IL_000e: Unknown result type (might be due to invalid IL or missing references)
					//IL_0034: Unknown result type (might be due to invalid IL or missing references)
					//IL_008d: Unknown result type (might be due to invalid IL or missing references)
					//IL_00d9: Unknown result type (might be due to invalid IL or missing references)
					//IL_017d: Unknown result type (might be due to invalid IL or missing references)
					string relativeRootPath = MakeRelativePath(asset.rootPath, projectRootPath);
					Asset assetReport = report.AddAsset(asset.name);
					Interlocked.Increment(ref parallelCount);
					s_Progress.Set($"Importing {parallelCount} assets ({total + 1}/{((SourceAssetCollector)(ref assetCollector)).count})", "Importing textures and meshes for " + asset.name, (float)total / (float)((SourceAssetCollector)(ref assetCollector)).count);
					if (s_Progress.shouldCancel)
					{
						state.Stop();
					}
					if (ImportAssetGroup(projectRootPath, relativeRootPath, asset, out var _, out var postImportOperations, report, assetReport))
					{
						s_MainThreadDispatcher.Dispatch((Action)delegate
						{
							//IL_001c: Unknown result type (might be due to invalid IL or missing references)
							postImportOperations?.Invoke(relativeRootPath, importMode, report, VTMaterials, prefabFactory);
						});
					}
					else
					{
						Interlocked.Increment(ref failures);
					}
					Interlocked.Increment(ref total);
					Interlocked.Decrement(ref parallelCount);
					s_Progress.Set($"Importing {parallelCount} assets ({total + 1}/{((SourceAssetCollector)(ref assetCollector)).count})", "Completed textures and meshes for " + asset.name, (float)total / (float)((SourceAssetCollector)(ref assetCollector)).count);
				})), report);
				if (convertToVT)
				{
					ImportStep val4 = ((ReportBase)report).AddImportStep("Convert materials to VT");
					try
					{
						ProcessSurfacesForVT(VTMaterials, null, (importMode & 0x400) == 1024, val4);
					}
					finally
					{
						((IDisposable)val4)?.Dispose();
					}
				}
				s_Progress.Set("Completed", "", 1f);
				report.totalTime = perf.result;
			}
			finally
			{
				((IDisposable)perf)?.Dispose();
			}
			report.Log(log);
			s_MainThreadDispatcher = null;
		}
		finally
		{
			((IDisposable)(*(AutoScope*)(&prof))/*cast due to .constrained prefix*/).Dispose();
		}
	}

	private static Settings ImportSettings(AssetGroup<Asset> assetGroup, (ImportStep step, Asset asset) report)
	{
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0075: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d6: Unknown result type (might be due to invalid IL or missing references)
		ImportStep val = ((ReportBase)report.step).AddImportStep("Settings import");
		try
		{
			bool flag = false;
			Settings val2 = Settings.GetDefault(assetGroup.name);
			foreach (Asset item in assetGroup)
			{
				IAssetImporter importer = ImporterCache.GetImporter(Path.GetExtension(item.path), (Type)null);
				SettingsImporter val3 = (SettingsImporter)(object)((importer is SettingsImporter) ? importer : null);
				if (val3 != null && item.name == "settings.json")
				{
					flag = true;
					FileReport val4 = report.asset.AddFile(item);
					((JSONImporter<Settings>)(object)val3).Import(item.path, ref val2, val4);
				}
			}
			if (!flag)
			{
				SettingsImporter.Expand(ref val2, (ReportBase)(object)val);
				((ReportBase)val).AddMessage("Using default settings: " + Extensions.ToJSONString<Settings>(val2, (EncodeOptions)9), (Severity)4000);
			}
			return val2;
		}
		finally
		{
			((IDisposable)val)?.Dispose();
		}
	}

	private static string ResolveRelativePath(string projectRootPath, string target, string to)
	{
		if (target.StartsWith('/'))
		{
			return Path.GetFullPath(Path.Combine(projectRootPath, target.Substring(1)));
		}
		return Path.GetFullPath(Path.Combine(to, target));
	}

	private static AssetGroup<IAsset> CreateAssetGroupFromSettings(string projectRootPath, Settings settings, AssetGroup<Asset> assetGroup, Asset assetReport)
	{
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
		//IL_009c: Unknown result type (might be due to invalid IL or missing references)
		//IL_012f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0136: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ff: Unknown result type (might be due to invalid IL or missing references)
		HashSet<IAsset> hashSet = new HashSet<IAsset>(assetGroup.count);
		foreach (Asset item in assetGroup)
		{
			if (settings.ignoreSuffixes == null || !StringUtils.EndsWithAny(Path.GetFileNameWithoutExtension(item.name), settings.ignoreSuffixes))
			{
				IAsset val = IAsset.Create(settings, Path.GetFileNameWithoutExtension(item.name), item);
				if (val != null)
				{
					hashSet.Add(val);
				}
			}
		}
		Asset val2 = default(Asset);
		foreach (KeyValuePair<string, string> item2 in ((Settings)(ref settings)).UsedShaderAssets(assetGroup, assetReport))
		{
			string text = ResolveRelativePath(projectRootPath, item2.Value, assetGroup.rootPath);
			if (!LongFile.Exists(text))
			{
				string fileName = Path.GetFileName(text);
				text = EnvPath.kContentPath + "/Game/.ModdingToolchain/shared_assets_fallback/" + fileName;
				log.InfoFormat("Using fallback {0}", (object)fileName);
			}
			if (LongFile.Exists(text))
			{
				((Asset)(ref val2))._002Ector(text, projectRootPath);
				IAsset val3 = IAsset.Create(settings, Path.GetFileNameWithoutExtension(item2.Key), val2);
				if (val3 != null)
				{
					hashSet.Add(val3);
				}
			}
		}
		return new AssetGroup<IAsset>(assetGroup.rootPath, hashSet);
	}

	private unsafe static void ImportModels(Settings settings, string relativeRootPath, AssetGroup<IAsset> assetGroup, (ImportStep step, Asset asset) report)
	{
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		ProfilerMarker val = s_ProfImportModels;
		AutoScope val2 = ((ProfilerMarker)(ref val)).Auto();
		try
		{
			ImportStep modelsReport = ((ReportBase)report.step).AddImportStep("Import Models");
			try
			{
				ParallelOptions parallelOptions = new ParallelOptions
				{
					MaxDegreeOfParallelism = ((!useParallelImport) ? 1 : Environment.ProcessorCount)
				};
				int failures = 0;
				int total = 0;
				PerformanceCounter val3 = PerformanceCounter.Start((Action<TimeSpan>)delegate(TimeSpan t)
				{
					if (total == 0)
					{
						log.Info((object)$"No models processed. All models in this group were already loaded or none were found. {t.TotalSeconds:F3}");
					}
					else
					{
						log.Info((object)$"Completed {total} models import in {t.TotalSeconds:F3}s. Errors {failures}.");
					}
				});
				try
				{
					Parallel.ForEach(Extensions.FilterBy<ModelAsset>(assetGroup, (Func<ModelAsset, bool>)null), parallelOptions, delegate(ModelAsset asset, ParallelLoopState state, long index)
					{
						//IL_0100: Unknown result type (might be due to invalid IL or missing references)
						//IL_010c: Unknown result type (might be due to invalid IL or missing references)
						//IL_0141: Unknown result type (might be due to invalid IL or missing references)
						if (((ConcreteAsset<ModelImporter, ModelList>)(object)asset).instance != null)
						{
							return;
						}
						FileReport val4 = report.asset.AddFile((IAsset)(object)asset);
						using (s_Progress.ScopedThreadDescription("Importing " + ((ConcreteAsset<ModelImporter, ModelList>)(object)asset).fileName))
						{
							if (s_Progress.shouldCancel)
							{
								state.Stop();
							}
							try
							{
								ISettings importSettings = ((Settings)(ref settings)).GetImportSettings(((ConcreteAsset<ModelImporter, ModelList>)(object)asset).fileName, (IAssetImporter)(object)((ConcreteAsset<ModelImporter, ModelList>)(object)asset).importer, (ReportBase)(object)val4);
								ImportStep val5 = ((ReportBase)modelsReport).AddImportStep("Asset import");
								try
								{
									if (((ConcreteAsset<ModelImporter, ModelList>)(object)asset).importer.Import<ModelList>(importSettings, ((ConcreteAsset<ModelImporter, ModelList>)(object)asset).path, val4, ref ((ConcreteAsset<ModelImporter, ModelList>)(object)asset).instance))
									{
										((ConcreteAsset<ModelImporter, ModelList>)(object)asset).instance.sourceAsset = (IAsset)(object)asset;
										Interlocked.Increment(ref total);
										if (((ConcreteAsset<ModelImporter, ModelList>)(object)asset).instance.isValid)
										{
											ISettings val6 = default(ISettings);
											Context val7 = default(Context);
											foreach (IModelPostProcessor modelPostProcessor in PostProcessorCache.GetModelPostProcessors())
											{
												if (((Settings)(ref settings)).GetPostProcessSettings(((ConcreteAsset<ModelImporter, ModelList>)(object)asset).fileName, (ISettingable)(object)modelPostProcessor, (ReportBase)(object)val4, ref val6))
												{
													((Context)(ref val7))._002Ector(s_MainThreadDispatcher, relativeRootPath, OnDebugTexture, val6, settings);
													if (modelPostProcessor.ShouldExecute(val7, asset))
													{
														ImportStep val8 = ((ReportBase)modelsReport).AddImportStep("Execute " + PPUtils.GetPostProcessorString(((object)modelPostProcessor).GetType(), true) + " Post Processors");
														try
														{
															modelPostProcessor.Execute(val7, asset, val4);
														}
														finally
														{
															((IDisposable)val8)?.Dispose();
														}
													}
												}
											}
											return;
										}
									}
								}
								finally
								{
									((IDisposable)val5)?.Dispose();
								}
							}
							catch (Exception ex)
							{
								Interlocked.Increment(ref failures);
								log.Error(ex, (object)("Error importing " + ((ConcreteAsset<ModelImporter, ModelList>)(object)asset).name + ". Skipped..."));
								((ReportBase)val4).AddError($"Error: {ex}");
							}
						}
					});
				}
				finally
				{
					((IDisposable)val3)?.Dispose();
				}
			}
			finally
			{
				if (modelsReport != null)
				{
					((IDisposable)modelsReport).Dispose();
				}
			}
		}
		finally
		{
			((IDisposable)(*(AutoScope*)(&val2))/*cast due to .constrained prefix*/).Dispose();
		}
	}

	private unsafe static void ImportDidimo(Settings settings, AssetGroup<IAsset> assetGroup, (ImportStep step, Asset asset) report)
	{
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0079: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c2: Unknown result type (might be due to invalid IL or missing references)
		ProfilerMarker val = s_ProfImportDidimo;
		AutoScope val2 = ((ProfilerMarker)(ref val)).Auto();
		try
		{
			ImportStep importReport = ((ReportBase)report.step).AddImportStep("Import didimo");
			try
			{
				int failures = 0;
				int total = 0;
				ParallelOptions parallelOptions = new ParallelOptions
				{
					MaxDegreeOfParallelism = ((!useParallelImport) ? 1 : Environment.ProcessorCount)
				};
				PerformanceCounter val3 = PerformanceCounter.Start((Action<TimeSpan>)delegate(TimeSpan t)
				{
					log.Info((object)$"Completed {total} animations import in {t.TotalSeconds:F3}s. Errors {failures}.");
				});
				try
				{
					Parallel.ForEach(Extensions.FilterBy<AnimationAsset>(assetGroup, (Func<AnimationAsset, bool>)null), parallelOptions, delegate(AnimationAsset asset, ParallelLoopState state, long index)
					{
						FileReport val4 = report.asset.AddFile((IAsset)(object)asset);
						using (s_Progress.ScopedThreadDescription("Importing " + ((ConcreteAsset<Animation, AnimationData>)(object)asset).fileName))
						{
							if (s_Progress.shouldCancel)
							{
								state.Stop();
							}
							try
							{
								ISettings importSettings = ((Settings)(ref settings)).GetImportSettings(((ConcreteAsset<Animation, AnimationData>)(object)asset).fileName, (IAssetImporter)(object)((ConcreteAsset<Animation, AnimationData>)(object)asset).importer, (ReportBase)(object)val4);
								ImportStep val5 = ((ReportBase)importReport).AddImportStep("Animations import");
								try
								{
									if (((ModelImporter)((ConcreteAsset<Animation, AnimationData>)(object)asset).importer).Import<AnimationData>(importSettings, ((ConcreteAsset<Animation, AnimationData>)(object)asset).path, val4, ref ((ConcreteAsset<Animation, AnimationData>)(object)asset).instance))
									{
										((ConcreteAsset<Animation, AnimationData>)(object)asset).instance.sourceAsset = (IAsset)(object)asset;
										Interlocked.Increment(ref total);
										_ = ((ConcreteAsset<Animation, AnimationData>)(object)asset).instance.isValid;
									}
								}
								finally
								{
									((IDisposable)val5)?.Dispose();
								}
							}
							catch (Exception ex)
							{
								Interlocked.Increment(ref failures);
								log.Error(ex, (object)("Error importing " + ((ConcreteAsset<Animation, AnimationData>)(object)asset).name + ". Skipped..."));
								((ReportBase)val4).AddError($"Error: {ex}");
							}
						}
					});
				}
				finally
				{
					((IDisposable)val3)?.Dispose();
				}
				failures = 0;
				total = 0;
				val3 = PerformanceCounter.Start((Action<TimeSpan>)delegate(TimeSpan t)
				{
					log.Info((object)$"Completed {total} didimo asset import in {t.TotalSeconds:F3}s. Errors {failures}.");
				});
				try
				{
					Parallel.ForEach(Extensions.FilterBy<DidimoAsset>(assetGroup, (Func<DidimoAsset, bool>)null), parallelOptions, delegate(DidimoAsset asset, ParallelLoopState state, long index)
					{
						FileReport val4 = report.asset.AddFile((IAsset)(object)asset);
						using (s_Progress.ScopedThreadDescription("Importing " + ((ConcreteAsset<Didimo, DidimoData>)(object)asset).fileName))
						{
							if (s_Progress.shouldCancel)
							{
								state.Stop();
							}
							try
							{
								ISettings importSettings = ((Settings)(ref settings)).GetImportSettings(((ConcreteAsset<Didimo, DidimoData>)(object)asset).fileName, (IAssetImporter)(object)((ConcreteAsset<Didimo, DidimoData>)(object)asset).importer, (ReportBase)(object)val4);
								ImportStep val5 = ((ReportBase)importReport).AddImportStep("Asset import");
								try
								{
									if (((ModelImporter)((ConcreteAsset<Didimo, DidimoData>)(object)asset).importer).Import<DidimoData>(importSettings, ((ConcreteAsset<Didimo, DidimoData>)(object)asset).path, val4, ref ((ConcreteAsset<Didimo, DidimoData>)(object)asset).instance))
									{
										((ConcreteAsset<Didimo, DidimoData>)(object)asset).instance.sourceAsset = (IAsset)(object)asset;
										Interlocked.Increment(ref total);
										_ = ((ConcreteAsset<Didimo, DidimoData>)(object)asset).instance.isValid;
									}
								}
								finally
								{
									((IDisposable)val5)?.Dispose();
								}
							}
							catch (Exception ex)
							{
								Interlocked.Increment(ref failures);
								log.Error(ex, (object)("Error importing " + ((ConcreteAsset<Didimo, DidimoData>)(object)asset).name + ". Skipped..."));
								((ReportBase)val4).AddError($"Error: {ex}");
							}
						}
					});
				}
				finally
				{
					((IDisposable)val3)?.Dispose();
				}
			}
			finally
			{
				if (importReport != null)
				{
					((IDisposable)importReport).Dispose();
				}
			}
		}
		finally
		{
			((IDisposable)(*(AutoScope*)(&val2))/*cast due to .constrained prefix*/).Dispose();
		}
	}

	private static void CreateDidimoAssets(Settings settings, string relativeRootPath, AssetGroup<IAsset> assetGroup, out Action<string, ImportMode, Report, HashSet<SurfaceAsset>, IPrefabFactory> postImportOperations, (Report parent, ImportStep step, Asset asset) report)
	{
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00db: Unknown result type (might be due to invalid IL or missing references)
		//IL_010d: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f2: Unknown result type (might be due to invalid IL or missing references)
		//IL_0201: Unknown result type (might be due to invalid IL or missing references)
		//IL_0251: Unknown result type (might be due to invalid IL or missing references)
		//IL_0256: Unknown result type (might be due to invalid IL or missing references)
		//IL_0262: Unknown result type (might be due to invalid IL or missing references)
		//IL_0267: Unknown result type (might be due to invalid IL or missing references)
		//IL_026c: Unknown result type (might be due to invalid IL or missing references)
		//IL_028a: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f5: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f7: Unknown result type (might be due to invalid IL or missing references)
		//IL_02fe: Unknown result type (might be due to invalid IL or missing references)
		//IL_0325: Unknown result type (might be due to invalid IL or missing references)
		//IL_035a: Unknown result type (might be due to invalid IL or missing references)
		//IL_035f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0364: Unknown result type (might be due to invalid IL or missing references)
		//IL_0370: Unknown result type (might be due to invalid IL or missing references)
		//IL_0372: Unknown result type (might be due to invalid IL or missing references)
		//IL_037a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0387: Unknown result type (might be due to invalid IL or missing references)
		//IL_0966: Unknown result type (might be due to invalid IL or missing references)
		//IL_096b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0974: Unknown result type (might be due to invalid IL or missing references)
		//IL_03c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_0400: Unknown result type (might be due to invalid IL or missing references)
		//IL_0405: Unknown result type (might be due to invalid IL or missing references)
		//IL_0407: Unknown result type (might be due to invalid IL or missing references)
		//IL_0989: Unknown result type (might be due to invalid IL or missing references)
		//IL_098e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0997: Unknown result type (might be due to invalid IL or missing references)
		//IL_041a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0522: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a89: Unknown result type (might be due to invalid IL or missing references)
		//IL_0450: Unknown result type (might be due to invalid IL or missing references)
		//IL_0465: Unknown result type (might be due to invalid IL or missing references)
		//IL_0566: Unknown result type (might be due to invalid IL or missing references)
		//IL_056b: Unknown result type (might be due to invalid IL or missing references)
		//IL_056f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0577: Unknown result type (might be due to invalid IL or missing references)
		//IL_057e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0585: Unknown result type (might be due to invalid IL or missing references)
		//IL_058c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0593: Unknown result type (might be due to invalid IL or missing references)
		//IL_05a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_05af: Unknown result type (might be due to invalid IL or missing references)
		//IL_05bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_05c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_05cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_05cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_05d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_05db: Unknown result type (might be due to invalid IL or missing references)
		//IL_05e0: Unknown result type (might be due to invalid IL or missing references)
		//IL_05ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_05ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_05f1: Unknown result type (might be due to invalid IL or missing references)
		//IL_0aaf: Unknown result type (might be due to invalid IL or missing references)
		//IL_0604: Unknown result type (might be due to invalid IL or missing references)
		//IL_0667: Unknown result type (might be due to invalid IL or missing references)
		//IL_066c: Unknown result type (might be due to invalid IL or missing references)
		//IL_066e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0670: Unknown result type (might be due to invalid IL or missing references)
		//IL_0675: Unknown result type (might be due to invalid IL or missing references)
		//IL_067a: Unknown result type (might be due to invalid IL or missing references)
		//IL_067c: Unknown result type (might be due to invalid IL or missing references)
		//IL_06b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_06c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_063a: Unknown result type (might be due to invalid IL or missing references)
		//IL_064f: Unknown result type (might be due to invalid IL or missing references)
		//IL_09e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_09eb: Unknown result type (might be due to invalid IL or missing references)
		//IL_09f0: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a01: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a03: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a08: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a19: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a1b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a20: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a31: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a33: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a38: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a4d: Unknown result type (might be due to invalid IL or missing references)
		//IL_047f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0893: Unknown result type (might be due to invalid IL or missing references)
		//IL_0895: Unknown result type (might be due to invalid IL or missing references)
		//IL_0897: Unknown result type (might be due to invalid IL or missing references)
		//IL_08a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_08ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_0716: Unknown result type (might be due to invalid IL or missing references)
		//IL_0726: Unknown result type (might be due to invalid IL or missing references)
		//IL_0733: Unknown result type (might be due to invalid IL or missing references)
		//IL_074c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0754: Unknown result type (might be due to invalid IL or missing references)
		//IL_0768: Unknown result type (might be due to invalid IL or missing references)
		//IL_076a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0788: Unknown result type (might be due to invalid IL or missing references)
		//IL_078a: Unknown result type (might be due to invalid IL or missing references)
		//IL_04b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_0814: Unknown result type (might be due to invalid IL or missing references)
		//IL_07a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_083b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0880: Unknown result type (might be due to invalid IL or missing references)
		//IL_0791: Unknown result type (might be due to invalid IL or missing references)
		//IL_07b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_07b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_07be: Unknown result type (might be due to invalid IL or missing references)
		//IL_07c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_07c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_07d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_07d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_07df: Unknown result type (might be due to invalid IL or missing references)
		//IL_07ec: Unknown result type (might be due to invalid IL or missing references)
		postImportOperations = null;
		ImportStep geometryReport = ((ReportBase)report.parent).AddImportStep("Create Geometry and Surfaces");
		try
		{
			ParallelOptions opts = new ParallelOptions
			{
				MaxDegreeOfParallelism = ((!useParallelImport) ? 1 : Environment.ProcessorCount)
			};
			List<(string, IReadOnlyList<(Animation, BoneHierarchy, string, int, Hash128)>, int, int)> characterStyles = new List<(string, IReadOnlyList<(Animation, BoneHierarchy, string, int, Hash128)>, int, int)>();
			List<(List<LOD> lods, string bodyParts)> allRenderGroups = new List<(List<LOD>, string)>();
			List<(string name, IReadOnlyList<(int styleIndex, CharacterGroup.Meta meta, IReadOnlyList<int> meshes)> characters)> groups = new List<(string, IReadOnlyList<(int, CharacterGroup.Meta, IReadOnlyList<int>)>)>();
			string[] defaultTextures = new string[8] { "Identity512_MaskMap.png", "Identity1024_MaskMap.png", "Identity2048_MaskMap.png", "Identity4096_MaskMap.png", "White512_ControlMask.png", "White1024_ControlMask.png", "White2048_ControlMask.png", "White4096_ControlMask.png" };
			ImportTextures(settings, relativeRootPath, assetGroup, (TextureAsset x) => defaultTextures.Contains(((ConcreteAsset<TextureImporter, Texture>)(object)x).fileName), (step: report.step, asset: report.asset));
			foreach (DidimoAsset asset in Extensions.FilterBy<DidimoAsset>(assetGroup, (Func<DidimoAsset, bool>)null))
			{
				DidimoData didimo = ((ConcreteAsset<Didimo, DidimoData>)(object)asset).instance;
				if (didimo == null)
				{
					continue;
				}
				int parallelCount = 0;
				(List<LOD>, string)[] renderGroups = new(List<LOD>, string)[didimo.renderGroups.Count];
				Parallel.ForEach(didimo.renderGroups, opts, delegate(RenderGroup renderGroup, ParallelLoopState rgState, long rgIndex)
				{
					//IL_0022: Unknown result type (might be due to invalid IL or missing references)
					//IL_0035: Unknown result type (might be due to invalid IL or missing references)
					//IL_0047: Unknown result type (might be due to invalid IL or missing references)
					//IL_00cb: Unknown result type (might be due to invalid IL or missing references)
					//IL_00d1: Expected O, but got Unknown
					//IL_00da: Unknown result type (might be due to invalid IL or missing references)
					//IL_00e4: Expected O, but got Unknown
					//IL_00b9: Unknown result type (might be due to invalid IL or missing references)
					//IL_0318: Unknown result type (might be due to invalid IL or missing references)
					//IL_0244: Unknown result type (might be due to invalid IL or missing references)
					//IL_0250: Unknown result type (might be due to invalid IL or missing references)
					//IL_028a: Unknown result type (might be due to invalid IL or missing references)
					try
					{
						Model[] models = (Model[])(object)new Model[renderGroup.renderObjects.Length];
						Surface[] surfaces = (Surface[])(object)new Surface[renderGroup.renderObjects.Length];
						Parallel.ForEach(renderGroup.renderObjects, opts, delegate(RenderObject renderObject, ParallelLoopState roState, long roIndex)
						{
							//IL_005e: Unknown result type (might be due to invalid IL or missing references)
							//IL_005f: Unknown result type (might be due to invalid IL or missing references)
							//IL_0064: Unknown result type (might be due to invalid IL or missing references)
							//IL_0065: Unknown result type (might be due to invalid IL or missing references)
							//IL_006a: Unknown result type (might be due to invalid IL or missing references)
							//IL_006d: Unknown result type (might be due to invalid IL or missing references)
							//IL_0084: Unknown result type (might be due to invalid IL or missing references)
							//IL_008a: Unknown result type (might be due to invalid IL or missing references)
							//IL_0090: Unknown result type (might be due to invalid IL or missing references)
							//IL_0096: Expected O, but got Unknown
							//IL_009b: Unknown result type (might be due to invalid IL or missing references)
							//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
							//IL_00a7: Unknown result type (might be due to invalid IL or missing references)
							//IL_00ad: Expected O, but got Unknown
							//IL_00b2: Unknown result type (might be due to invalid IL or missing references)
							//IL_00b8: Unknown result type (might be due to invalid IL or missing references)
							//IL_00be: Unknown result type (might be due to invalid IL or missing references)
							//IL_00c4: Expected O, but got Unknown
							//IL_00c9: Unknown result type (might be due to invalid IL or missing references)
							//IL_00cf: Unknown result type (might be due to invalid IL or missing references)
							//IL_00d5: Unknown result type (might be due to invalid IL or missing references)
							//IL_00db: Expected O, but got Unknown
							//IL_00dc: Unknown result type (might be due to invalid IL or missing references)
							//IL_0130: Unknown result type (might be due to invalid IL or missing references)
							//IL_00fd: Unknown result type (might be due to invalid IL or missing references)
							//IL_0106: Unknown result type (might be due to invalid IL or missing references)
							//IL_010c: Unknown result type (might be due to invalid IL or missing references)
							//IL_0112: Expected O, but got Unknown
							//IL_011b: Unknown result type (might be due to invalid IL or missing references)
							//IL_0124: Unknown result type (might be due to invalid IL or missing references)
							//IL_012a: Unknown result type (might be due to invalid IL or missing references)
							//IL_0130: Expected O, but got Unknown
							//IL_01b0: Unknown result type (might be due to invalid IL or missing references)
							//IL_01b2: Unknown result type (might be due to invalid IL or missing references)
							//IL_01b7: Unknown result type (might be due to invalid IL or missing references)
							//IL_01cb: Unknown result type (might be due to invalid IL or missing references)
							//IL_01e6: Unknown result type (might be due to invalid IL or missing references)
							//IL_01ed: Expected O, but got Unknown
							//IL_0211: Unknown result type (might be due to invalid IL or missing references)
							//IL_0219: Unknown result type (might be due to invalid IL or missing references)
							//IL_0223: Unknown result type (might be due to invalid IL or missing references)
							//IL_0225: Unknown result type (might be due to invalid IL or missing references)
							//IL_022c: Unknown result type (might be due to invalid IL or missing references)
							//IL_0233: Expected O, but got Unknown
							//IL_023c: Unknown result type (might be due to invalid IL or missing references)
							//IL_023d: Unknown result type (might be due to invalid IL or missing references)
							//IL_0242: Unknown result type (might be due to invalid IL or missing references)
							//IL_0244: Unknown result type (might be due to invalid IL or missing references)
							//IL_0281: Unknown result type (might be due to invalid IL or missing references)
							//IL_0288: Unknown result type (might be due to invalid IL or missing references)
							//IL_0294: Unknown result type (might be due to invalid IL or missing references)
							//IL_029b: Expected O, but got Unknown
							//IL_02ce: Unknown result type (might be due to invalid IL or missing references)
							//IL_02d7: Unknown result type (might be due to invalid IL or missing references)
							//IL_02f1: Unknown result type (might be due to invalid IL or missing references)
							//IL_02fb: Unknown result type (might be due to invalid IL or missing references)
							//IL_0310: Unknown result type (might be due to invalid IL or missing references)
							//IL_0315: Unknown result type (might be due to invalid IL or missing references)
							//IL_031e: Unknown result type (might be due to invalid IL or missing references)
							//IL_035a: Unknown result type (might be due to invalid IL or missing references)
							//IL_032c: Unknown result type (might be due to invalid IL or missing references)
							//IL_034e: Unknown result type (might be due to invalid IL or missing references)
							//IL_0350: Unknown result type (might be due to invalid IL or missing references)
							//IL_03b6: Unknown result type (might be due to invalid IL or missing references)
							//IL_03bd: Unknown result type (might be due to invalid IL or missing references)
							//IL_0382: Unknown result type (might be due to invalid IL or missing references)
							//IL_0389: Unknown result type (might be due to invalid IL or missing references)
							//IL_0490: Unknown result type (might be due to invalid IL or missing references)
							//IL_04ba: Unknown result type (might be due to invalid IL or missing references)
							//IL_046f: Unknown result type (might be due to invalid IL or missing references)
							//IL_0476: Unknown result type (might be due to invalid IL or missing references)
							//IL_047b: Unknown result type (might be due to invalid IL or missing references)
							//IL_0407: Unknown result type (might be due to invalid IL or missing references)
							//IL_040e: Unknown result type (might be due to invalid IL or missing references)
							//IL_0434: Unknown result type (might be due to invalid IL or missing references)
							//IL_044f: Unknown result type (might be due to invalid IL or missing references)
							//IL_0451: Unknown result type (might be due to invalid IL or missing references)
							//IL_052e: Unknown result type (might be due to invalid IL or missing references)
							//IL_069f: Unknown result type (might be due to invalid IL or missing references)
							//IL_06e8: Unknown result type (might be due to invalid IL or missing references)
							//IL_06ef: Expected O, but got Unknown
							//IL_088e: Unknown result type (might be due to invalid IL or missing references)
							//IL_089a: Unknown result type (might be due to invalid IL or missing references)
							//IL_0956: Unknown result type (might be due to invalid IL or missing references)
							//IL_0957: Unknown result type (might be due to invalid IL or missing references)
							//IL_0575: Unknown result type (might be due to invalid IL or missing references)
							//IL_0562: Unknown result type (might be due to invalid IL or missing references)
							//IL_0966: Unknown result type (might be due to invalid IL or missing references)
							//IL_0967: Unknown result type (might be due to invalid IL or missing references)
							//IL_0723: Unknown result type (might be due to invalid IL or missing references)
							//IL_07a3: Unknown result type (might be due to invalid IL or missing references)
							//IL_05ed: Unknown result type (might be due to invalid IL or missing references)
							//IL_08e5: Unknown result type (might be due to invalid IL or missing references)
							//IL_0976: Unknown result type (might be due to invalid IL or missing references)
							//IL_0977: Unknown result type (might be due to invalid IL or missing references)
							//IL_07b8: Unknown result type (might be due to invalid IL or missing references)
							//IL_05ac: Unknown result type (might be due to invalid IL or missing references)
							//IL_09e3: Unknown result type (might be due to invalid IL or missing references)
							//IL_09e4: Unknown result type (might be due to invalid IL or missing references)
							//IL_09f0: Unknown result type (might be due to invalid IL or missing references)
							//IL_09f1: Unknown result type (might be due to invalid IL or missing references)
							//IL_09af: Unknown result type (might be due to invalid IL or missing references)
							//IL_09c7: Unknown result type (might be due to invalid IL or missing references)
							//IL_09c8: Unknown result type (might be due to invalid IL or missing references)
							//IL_09dd: Unknown result type (might be due to invalid IL or missing references)
							//IL_062a: Unknown result type (might be due to invalid IL or missing references)
							//IL_0a5f: Unknown result type (might be due to invalid IL or missing references)
							//IL_0a60: Unknown result type (might be due to invalid IL or missing references)
							//IL_0a77: Unknown result type (might be due to invalid IL or missing references)
							//IL_0a78: Unknown result type (might be due to invalid IL or missing references)
							//IL_0a8b: Unknown result type (might be due to invalid IL or missing references)
							//IL_0a23: Unknown result type (might be due to invalid IL or missing references)
							//IL_0a2c: Unknown result type (might be due to invalid IL or missing references)
							//IL_0a2d: Unknown result type (might be due to invalid IL or missing references)
							//IL_0a41: Unknown result type (might be due to invalid IL or missing references)
							//IL_0a42: Unknown result type (might be due to invalid IL or missing references)
							//IL_0a57: Unknown result type (might be due to invalid IL or missing references)
							//IL_05da: Unknown result type (might be due to invalid IL or missing references)
							//IL_0658: Unknown result type (might be due to invalid IL or missing references)
							s_Progress.Set($"Importing {parallelCount} render objects", "Importing textures and meshes for " + ((ConcreteAsset<Didimo, DidimoData>)(object)asset).name);
							Interlocked.Increment(ref parallelCount);
							Mesh mesh = renderObject.mesh;
							Matrix4x4 identity = Matrix4x4.identity;
							NativeArray<int> val14 = default(NativeArray<int>);
							val14._002Ector(mesh.indices, (Allocator)4);
							VertexData[] array3 = (VertexData[])(object)new VertexData[4]
							{
								new VertexData((VertexAttribute)0, (VertexAttributeFormat)0, 3, FromManagedArray<float3>(mesh.vertices), false),
								new VertexData((VertexAttribute)1, (VertexAttributeFormat)0, 3, FromManagedArray<float3>(mesh.normals), false),
								new VertexData((VertexAttribute)2, (VertexAttributeFormat)0, 4, FromManagedArray<float4>(mesh.tangents), false),
								new VertexData((VertexAttribute)4, (VertexAttributeFormat)0, 2, FromManagedArray<float2>(mesh.uv), false)
							};
							if (mesh.boneWeights.Length != 0)
							{
								int num11 = array3.Length;
								Array.Resize(ref array3, num11 + 2);
								array3[num11] = new VertexData((VertexAttribute)13, (VertexAttributeFormat)10, 4, FromManagedArray<IndexWeight4>(mesh.boneWeights, 0, 4, 4), false);
								array3[num11 + 1] = new VertexData((VertexAttribute)12, (VertexAttributeFormat)0, 4, FromManagedArray<IndexWeight4>(mesh.boneWeights, 4, 4, 4), false);
							}
							string text = Path.GetFileNameWithoutExtension(mesh.name);
							if (didimo.props.Any((Prop x) => x.renderGroupIndex == rgIndex))
							{
								text += "_Prop";
							}
							text = AdjustNamingConvention(text);
							SubMeshDescriptor val15 = Model.CreateSubMesh(text, ((Mesh)(ref mesh)).indexCount, ((Mesh)(ref mesh)).vertexCount, val14, array3);
							ModelAsset val16 = new ModelAsset(text, ((ConcreteAsset<Didimo, DidimoData>)(object)asset).collectedAsset, (ModelImporter)(object)((ConcreteAsset<Didimo, DidimoData>)(object)asset).importer, false);
							report.parent.AddAsset(text);
							Model val17 = new Model(text, identity, ((Mesh)(ref mesh)).vertexCount, val14, array3, (SubMeshDescriptor[])(object)new SubMeshDescriptor[1] { val15 }, -1, (BoneInfo[])null);
							val17.sourceAsset = (IAsset)(object)val16;
							Material material = renderObject.material;
							string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(material.name);
							fileNameWithoutExtension = AdjustNamingConvention(fileNameWithoutExtension);
							if (models.Length > 1)
							{
								fileNameWithoutExtension += $"#{roIndex}";
							}
							Surface val18 = new Surface(fileNameWithoutExtension, Shader.GetCharacterShader(material.name, material.shader));
							AssetData item3 = report.parent.AddAssetData(val18.name, typeof(Surface), false);
							material.textures.Add(new Texture
							{
								index = -1,
								key = "_ControlMask"
							});
							int num12 = 0;
							foreach (Texture texture in material.textures)
							{
								Texture textureData;
								if (texture.index == -1)
								{
									textureData = new Texture
									{
										path = new string[1] { fileNameWithoutExtension + "_ControlMask.png" }
									};
								}
								else if ((texture.key == "_MaskMap" && didimo.textures[texture.index].path[0] == "Green.png") || didimo.textures[texture.index].path[0] == "Male_Beard01_BaseColor.png")
								{
									((ReportBase)report.asset).AddMessage(didimo.textures[texture.index].path[0] + " was removed by import for " + fileNameWithoutExtension, (Severity)4000);
									textureData = new Texture
									{
										path = new string[1] { string.Empty }
									};
								}
								else
								{
									textureData = didimo.textures[texture.index];
								}
								ImportTextures(settings, relativeRootPath, assetGroup, (TextureAsset x) => textureData.path.Contains(((ConcreteAsset<TextureImporter, Texture>)(object)x).fileName), (step: report.step, asset: report.asset));
								if (textureData.path.Length == 1)
								{
									Texture val19 = ((ConcreteAsset<TextureImporter, Texture>)(object)Extensions.Find<TextureAsset>(assetGroup, (Func<TextureAsset, bool>)((TextureAsset x) => ((ConcreteAsset<TextureImporter, Texture>)(object)x).fileName == textureData.path[0])))?.instance;
									if (val19 != null)
									{
										num12 = val19.width;
										val18.AddProperty(texture.key, (ITexture)(object)val19);
									}
									else if (texture.key == "_MaskMap" && num12 != 0)
									{
										int resolution = num12;
										Texture val20 = ((ConcreteAsset<TextureImporter, Texture>)(object)Extensions.Find<TextureAsset>(assetGroup, (Func<TextureAsset, bool>)((TextureAsset x) => ((ConcreteAsset<TextureImporter, Texture>)(object)x).fileName == string.Format("Identity{0}{1}.png", resolution, "_MaskMap"))))?.instance;
										if (val20 != null)
										{
											val18.AddProperty(texture.key, (ITexture)(object)val20);
										}
									}
									else if (texture.key == "_ControlMask" && num12 != 0)
									{
										int resolution2 = num12;
										Texture val21 = ((ConcreteAsset<TextureImporter, Texture>)(object)Extensions.Find<TextureAsset>(assetGroup, (Func<TextureAsset, bool>)((TextureAsset x) => ((ConcreteAsset<TextureImporter, Texture>)(object)x).fileName == string.Format("White{0}{1}.png", resolution2, "_ControlMask"))))?.instance;
										if (val21 != null)
										{
											val18.AddProperty(texture.key, (ITexture)(object)val21);
										}
									}
								}
								else
								{
									int num13 = 0;
									int num14 = 0;
									for (int num15 = 0; num15 < textureData.path.Length; num15++)
									{
										int i3 = num15;
										int width = ((ConcreteAsset<TextureImporter, Texture>)(object)Extensions.Find<TextureAsset>(assetGroup, (Func<TextureAsset, bool>)((TextureAsset x) => ((ConcreteAsset<TextureImporter, Texture>)(object)x).fileName == textureData.path[i3]))).instance.width;
										if (width > num14)
										{
											num13 = num15;
											num14 = width;
										}
									}
									TextureArray val22 = new TextureArray();
									for (int num16 = num13; num16 < textureData.path.Length; num16++)
									{
										int i4 = num16;
										if (!val22.AddSlice(((ConcreteAsset<TextureImporter, Texture>)(object)Extensions.Find<TextureAsset>(assetGroup, (Func<TextureAsset, bool>)((TextureAsset x) => ((ConcreteAsset<TextureImporter, Texture>)(object)x).fileName == textureData.path[i4]))).instance))
										{
											log.WarnFormat("Texture {0} does not match the texture array resolution {1}x{2}. Skipped!", (object)textureData.path[i4], (object)val22.width, (object)val22.height);
										}
									}
									val18.AddProperty(texture.key, (ITexture)(object)val22);
									if (num13 > 0)
									{
										val18.AddProperty(texture.key + "_IndexOffset", (float)(-num13));
									}
								}
							}
							FileReport fileReport = report.parent.GetFileReport((IAsset)(object)val16);
							ISettings val23 = default(ISettings);
							Context val24 = default(Context);
							foreach (IModelSurfacePostProcessor modelSurfacePostProcessor in PostProcessorCache.GetModelSurfacePostProcessors())
							{
								string name = val17.name;
								if (((Settings)(ref settings)).GetPostProcessSettings(name, (ISettingable)(object)modelSurfacePostProcessor, (ReportBase)(object)fileReport, ref val23))
								{
									((Context)(ref val24))._002Ector(s_MainThreadDispatcher, relativeRootPath, OnDebugTexture, val23, settings);
									if (modelSurfacePostProcessor.ShouldExecute(val24, val16, 0, val18))
									{
										ImportStep val25 = ((ReportBase)geometryReport).AddImportStep("Execute " + PPUtils.GetPostProcessorString(((object)modelSurfacePostProcessor).GetType(), true) + " Post Processors");
										try
										{
											modelSurfacePostProcessor.Execute(val24, val16, 0, val18, (report.parent, report.asset, fileReport, item3));
										}
										finally
										{
											((IDisposable)val25)?.Dispose();
										}
									}
								}
							}
							if (renderObject.shapeBuffer.stride != 0 && renderObject.shapeBuffer.elements != null)
							{
								if (renderObject.shapeBuffer.stride != ((Mesh)(ref mesh)).vertexCount)
								{
									throw new ModelImportException($"Error importing {((ConcreteAsset<Didimo, DidimoData>)(object)asset).name}: mesh {mesh.name} vertex count ({((Mesh)(ref mesh)).vertexCount}) does not match shape stride ({renderObject.shapeBuffer.stride})");
								}
								if (renderObject.shapeBuffer.elements.Length % renderObject.shapeBuffer.stride != 0)
								{
									throw new ModelImportException($"Error importing {((ConcreteAsset<Didimo, DidimoData>)(object)asset).name}: mesh {mesh.name} shape buffer size ({renderObject.shapeBuffer.elements.Length}) is not multiple of stride ({renderObject.shapeBuffer.stride})");
								}
								NativeArray<Element> val26 = default(NativeArray<Element>);
								val26._002Ector(renderObject.shapeBuffer.elements, (Allocator)4);
								int num17 = val26.Length / renderObject.shapeBuffer.stride;
								val17.SetShapeData(val26.Reinterpret<byte>(24), num17);
							}
							surfaces[roIndex] = val18;
							models[roIndex] = val17;
							Interlocked.Decrement(ref parallelCount);
						});
						int firstShapeCount = models.First().shapeCount;
						if (models.Any((Model m) => m.shapeCount != firstShapeCount))
						{
							throw new ModelImportException($"Error importing {((ConcreteAsset<Didimo, DidimoData>)(object)asset).name}: not all meshes use the same shape count (first shape count {firstShapeCount})");
						}
						List<LOD> list5 = new List<LOD>();
						Geometry val8 = new Geometry(models);
						list5.Add(new LOD(val8, surfaces, 0));
						AssetData val9 = report.parent.AddAssetData(val8.name, typeof(Geometry), false);
						val9.AddFiles(models.Select((Model t) => t.sourceAsset));
						AssetData[] array = (AssetData[])(object)new AssetData[surfaces.Length];
						int num9 = 0;
						Surface[] array2 = surfaces;
						foreach (Surface val10 in array2)
						{
							array[num9] = report.parent.AddAssetData(val10.name, typeof(Surface), false);
							array[num9++].AddFiles(val10.textures.Values.Select((ITexture t) => t.sourceAsset));
						}
						ISettings val11 = default(ISettings);
						Context val12 = default(Context);
						foreach (IGeometryPostProcessor geometryPostProcessor in PostProcessorCache.GetGeometryPostProcessors())
						{
							try
							{
								if (((Settings)(ref settings)).GetPostProcessSettings(val8.name, (ISettingable)(object)geometryPostProcessor, (ReportBase)(object)report.asset, ref val11))
								{
									((Context)(ref val12))._002Ector(s_MainThreadDispatcher, relativeRootPath, OnDebugTexture, val11, settings);
									if (geometryPostProcessor.ShouldExecute(val12, list5))
									{
										ImportStep val13 = ((ReportBase)geometryReport).AddImportStep("Execute " + PPUtils.GetPostProcessorString(((object)geometryPostProcessor).GetType(), true) + " Post Processors");
										try
										{
											geometryPostProcessor.Execute(val12, list5, (report.parent, report.asset, val9, array));
										}
										finally
										{
											((IDisposable)val13)?.Dispose();
										}
									}
								}
							}
							catch (Exception ex)
							{
								log.Error(ex, (object)("Exception occured with " + val8.name));
								throw;
							}
						}
						renderGroups[rgIndex] = (list5, renderGroup.bodyParts);
					}
					catch (Exception ex2)
					{
						log.Error(ex2, (object)("Exception occured with " + ((ConcreteAsset<Didimo, DidimoData>)(object)asset).name));
					}
				});
				int count = allRenderGroups.Count;
				allRenderGroups.AddRange(renderGroups);
				HashSet<int> hashSet = new HashSet<int>();
				Dictionary<string, (int, string)> dictionary = new Dictionary<string, (int, string)>();
				for (int num = 0; num < didimo.props.Count; num++)
				{
					Prop val = didimo.props[num];
					hashSet.Add(val.renderGroupIndex);
					dictionary.Add(val.name, (num, null));
				}
				foreach (AnimationGroup animationGroup in didimo.animationGroups)
				{
					BoneHierarchy val2 = CastStruct<BoneHierarchy, BoneHierarchy>(animationGroup.boneHierarchy);
					List<(Animation, BoneHierarchy, string, int, Hash128)> list = new List<(Animation, BoneHierarchy, string, int, Hash128)>(animationGroup.paths.Count + 1);
					Animation val3 = new Animation
					{
						name = animationGroup.styleName + "_RestPose",
						type = (AnimationType)(-1),
						layer = (AnimationLayer)(-1),
						shapeIndices = new int[animationGroup.shapeCount],
						boneIndices = new int[animationGroup.boneCount],
						frameCount = 1
					};
					for (int num2 = 0; num2 < animationGroup.shapeCount; num2++)
					{
						val3.shapeIndices[num2] = num2;
					}
					for (int num3 = 0; num3 < animationGroup.boneCount; num3++)
					{
						val3.boneIndices[num3] = num3;
					}
					((Animation)(ref val3)).SetElements(Span<ElementRaw>.op_Implicit(MemoryMarshal.Cast<Element, ElementRaw>(Span<Element>.op_Implicit(animationGroup.restPose.elements))));
					list.Add((val3, val2, "RestPose", -1, HashUtils.GetHash(val3, relativeRootPath)));
					List<(string, int)> list2 = new List<(string, int)>();
					int i;
					int num4;
					for (i = 0; i < animationGroup.paths.Count; num4 = i + 1, i = num4)
					{
						Animation animation = (from a in Extensions.FilterBy<AnimationAsset>(assetGroup, (Func<AnimationAsset, bool>)((AnimationAsset a) => animationGroup.paths[i].Contains(((ConcreteAsset<Animation, AnimationData>)(object)a).fileName)))
							select ((ConcreteAsset<Animation, AnimationData>)(object)a).instance).First().m_Animation;
						if (string.IsNullOrEmpty(animation.targetName))
						{
							continue;
						}
						if (!dictionary.TryGetValue(animation.targetName, out var value))
						{
							throw new ModelImportException("Error importing " + ((ConcreteAsset<Didimo, DidimoData>)(object)asset).name + ": animation target model not found (" + animation.targetName + ")");
						}
						foreach (var item4 in list2)
						{
							if (!animation.name.StartsWith(item4.Item1))
							{
								continue;
							}
							goto IL_04c9;
						}
						list2.Add((animation.name, value.Item1));
						IL_04c9:;
					}
					int i2;
					for (i2 = 0; i2 < animationGroup.paths.Count; i2 = num4)
					{
						AnimationData val4 = (from a in Extensions.FilterBy<AnimationAsset>(assetGroup, (Func<AnimationAsset, bool>)((AnimationAsset a) => animationGroup.paths[i2].Contains(((ConcreteAsset<Animation, AnimationData>)(object)a).fileName)))
							select ((ConcreteAsset<Animation, AnimationData>)(object)a).instance).First();
						Animation animation2 = val4.m_Animation;
						Animation val5 = new Animation
						{
							type = (AnimationType)animation2.animationType,
							layer = (AnimationLayer)animation2.layerIndex,
							shapeIndices = animation2.shapeIndices,
							boneIndices = animation2.boneIndices,
							frameCount = animation2.frameCount,
							frameRate = animation2.fps
						};
						((Animation)(ref val5)).SetElements(Span<ElementRaw>.op_Implicit(MemoryMarshal.Cast<Element, ElementRaw>(Span<Element>.op_Implicit(animation2.elements))));
						BoneHierarchy item = val2;
						int num5 = -1;
						if (!string.IsNullOrEmpty(animation2.targetName))
						{
							if (!dictionary.TryGetValue(animation2.targetName, out var value2))
							{
								throw new ModelImportException("Error importing " + ((ConcreteAsset<Didimo, DidimoData>)(object)asset).name + ": animation target model not found (" + animation2.targetName + ")");
							}
							Prop val6 = didimo.props[value2.Item1];
							item = CastStruct<BoneHierarchy, BoneHierarchy>(val6.boneHierarchy);
							num5 = val6.renderGroupIndex;
							num5 += count;
							val5.name = animationGroup.styleName + "_" + animation2.name + "#" + Path.GetFileNameWithoutExtension(animation2.targetName);
							if (value2.Item2 != animationGroup.styleName)
							{
								value2.Item2 = animationGroup.styleName;
								dictionary[animation2.targetName] = value2;
								Animation val7 = new Animation
								{
									name = "RestPose#" + Path.GetFileNameWithoutExtension(animation2.targetName),
									type = (AnimationType)(-1),
									layer = (AnimationLayer)1,
									shapeIndices = new int[1],
									boneIndices = new int[val6.boneHierarchy.hierarchyParentIndices.Length],
									frameCount = 1
								};
								for (int num6 = 0; num6 < val7.boneIndices.Length; num6++)
								{
									val7.boneIndices[num6] = num6;
								}
								((Animation)(ref val7)).SetElements(Span<ElementRaw>.op_Implicit(MemoryMarshal.Cast<Element, ElementRaw>(Span<Element>.op_Implicit(val6.restPose.elements))));
								list.Add((val7, item, "RestPose", num5, HashUtils.GetHash(val7, relativeRootPath)));
							}
						}
						else
						{
							foreach (var item5 in list2)
							{
								if (animation2.name.StartsWith(item5.Item1))
								{
									num5 = didimo.props[item5.Item2].renderGroupIndex;
									num5 += count;
									break;
								}
							}
							val5.name = animationGroup.styleName + "_" + animation2.name;
						}
						list.Add((val5, item, animation2.name, num5, HashUtils.GetHash(val5, val4.sourceAsset.path)));
						num4 = i2 + 1;
					}
					characterStyles.Add((animationGroup.styleName, list, animationGroup.boneCount, animationGroup.shapeCount));
				}
				int num7 = 0;
				foreach (Group group in didimo.groups)
				{
					List<(int, CharacterGroup.Meta, IReadOnlyList<int>)> list3 = new List<(int, CharacterGroup.Meta, IReadOnlyList<int>)>();
					foreach (Character character2 in group.characters)
					{
						List<int> list4 = new List<int>();
						foreach (int renderObjectIndex in character2.renderObjectIndices)
						{
							if (!hashSet.Contains(renderObjectIndex))
							{
								list4.Add(count + renderObjectIndex);
							}
						}
						CharacterGroup.Meta item2 = new CharacterGroup.Meta
						{
							shapeWeights = CastStruct<IndexWeight8, CharacterGroup.IndexWeight8>(character2.meta.shapeWeights),
							textureWeights = CastStruct<IndexWeight8, CharacterGroup.IndexWeight8>(character2.meta.textureWeights),
							overlayWeights = CastStruct<IndexWeight8, CharacterGroup.IndexWeight8>(character2.meta.overlayWeights),
							maskWeights = CastStruct<IndexWeight8, CharacterGroup.IndexWeight8>(character2.meta.maskWeights)
						};
						list3.Add((character2.styleIndex, item2, list4));
					}
					groups.Add(((!string.IsNullOrEmpty(group.name)) ? group.name : $"Group#{num7++}", list3));
				}
				log.InfoFormat($"textureOverlays.Count: {didimo.textureOverlays.Count}", Array.Empty<object>());
				for (int num8 = 0; num8 < didimo.textureOverlays.Count; num8++)
				{
					log.InfoFormat($"textureOverlays[{num8}]: {didimo.textureOverlays[num8]}", Array.Empty<object>());
				}
			}
			postImportOperations = delegate(string sourcePath, ImportMode importMode, Report report2, HashSet<SurfaceAsset> VTMaterials, IPrefabFactory prefabFactory)
			{
				//IL_002a: Unknown result type (might be due to invalid IL or missing references)
				//IL_0036: Unknown result type (might be due to invalid IL or missing references)
				AssetDatabase.global.UnloadAllAssets();
				List<RenderPrefab> list5 = new List<RenderPrefab>();
				foreach (var item6 in allRenderGroups)
				{
					(RenderPrefab, Prefab) tuple = CreateRenderPrefab(settings, sourcePath, item6.lods, importMode, report2, VTMaterials, prefabFactory)[0];
					if (!string.IsNullOrEmpty(item6.bodyParts))
					{
						CharacterProperties characterProperties = tuple.Item1.AddOrGetComponent<CharacterProperties>();
						characterProperties.m_BodyParts = (CharacterProperties.BodyPart)0;
						string[] array = item6.bodyParts.Split(',', StringSplitOptions.None);
						foreach (string text in array)
						{
							if (Enum.TryParse<CharacterProperties.BodyPart>(text, out var result))
							{
								characterProperties.m_BodyParts |= result;
							}
							else
							{
								log.WarnFormat("Unknown bodypart type ({0}).", (object)text);
							}
						}
					}
					list5.Add(tuple.Item1);
				}
				foreach (var item7 in allRenderGroups)
				{
					DisposeLODs(item7.lods);
				}
				Dictionary<Hash128, AnimationAsset> overrideSafeGuard = new Dictionary<Hash128, AnimationAsset>();
				foreach (var item8 in groups)
				{
					CharacterGroup characterGroup = CreatePrefab<CharacterGroup>("Group", sourcePath, item8.name, 0, prefabFactory);
					characterGroup.m_Characters = new CharacterGroup.Character[item8.characters.Count];
					for (int k = 0; k < item8.characters.Count; k++)
					{
						(int, CharacterGroup.Meta, IReadOnlyList<int>) tuple2 = item8.characters[k];
						CharacterGroup.Character character = new CharacterGroup.Character();
						int count2 = tuple2.Item3.Count;
						character.m_MeshPrefabs = new RenderPrefab[count2];
						character.m_Style = CreateStylePrefab(characterStyles[tuple2.Item1], sourcePath, overrideSafeGuard, list5, prefabFactory);
						character.m_Meta = tuple2.Item2;
						for (int l = 0; l < count2; l++)
						{
							character.m_MeshPrefabs[l] = list5[tuple2.Item3[l]];
						}
						characterGroup.m_Characters[k] = character;
					}
				}
			};
		}
		finally
		{
			if (geometryReport != null)
			{
				((IDisposable)geometryReport).Dispose();
			}
		}
	}

	private static TTo CastStruct<TFrom, TTo>(TFrom s) where TFrom : struct where TTo : struct
	{
		TFrom val = s;
		return UnsafeUtility.As<TFrom, TTo>(ref val);
	}

	private unsafe static NativeArray<byte> FromManagedArray<T>(T[] array) where T : unmanaged
	{
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		int num = array.Length * UnsafeUtility.SizeOf<T>();
		NativeArray<byte> val = new NativeArray<byte>(num, (Allocator)4, (NativeArrayOptions)1);
		fixed (T* ptr = array)
		{
			void* ptr2 = ptr;
			UnsafeUtility.MemCpy(NativeArrayUnsafeUtility.GetUnsafePtr<byte>(val), ptr2, (long)num);
		}
		return val;
	}

	private static CharacterStyle CreateStylePrefab((string name, IReadOnlyList<(Animation data, BoneHierarchy boneHierarchy, string shortName, int targetRenderGroup, Hash128 hash)> animations, int boneCount, int shapeCount) style, string sourcePath, Dictionary<Hash128, AnimationAsset> overrideSafeGuard, List<RenderPrefab> prefabs, IPrefabFactory prefabFactory = null)
	{
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b4: Invalid comparison between Unknown and I4
		//IL_00b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c3: Invalid comparison between Unknown and I4
		//IL_00d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_011e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0136: Unknown result type (might be due to invalid IL or missing references)
		//IL_013b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0141: Invalid comparison between Unknown and I4
		//IL_015c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0161: Unknown result type (might be due to invalid IL or missing references)
		//IL_0164: Unknown result type (might be due to invalid IL or missing references)
		//IL_0169: Unknown result type (might be due to invalid IL or missing references)
		//IL_016e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0171: Unknown result type (might be due to invalid IL or missing references)
		//IL_0176: Unknown result type (might be due to invalid IL or missing references)
		//IL_017d: Expected O, but got Unknown
		//IL_0183: Unknown result type (might be due to invalid IL or missing references)
		//IL_0145: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01fc: Unknown result type (might be due to invalid IL or missing references)
		//IL_01aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_0255: Unknown result type (might be due to invalid IL or missing references)
		//IL_025a: Unknown result type (might be due to invalid IL or missing references)
		//IL_025f: Unknown result type (might be due to invalid IL or missing references)
		//IL_026e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0273: Unknown result type (might be due to invalid IL or missing references)
		//IL_0278: Unknown result type (might be due to invalid IL or missing references)
		//IL_0287: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_02de: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f6: Unknown result type (might be due to invalid IL or missing references)
		//IL_02fb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0301: Invalid comparison between Unknown and I4
		//IL_02e7: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f2: Invalid comparison between Unknown and I4
		//IL_0312: Unknown result type (might be due to invalid IL or missing references)
		//IL_0319: Unknown result type (might be due to invalid IL or missing references)
		//IL_0321: Unknown result type (might be due to invalid IL or missing references)
		CharacterStyle characterStyle = CreatePrefab<CharacterStyle>("Style", sourcePath, style.name, 0, prefabFactory);
		int num = 0;
		Dictionary<Hash128, CharacterStyle.AnimationInfo> dictionary = new Dictionary<Hash128, CharacterStyle.AnimationInfo>();
		if (characterStyle.m_Animations != null)
		{
			CharacterStyle.AnimationInfo[] animations = characterStyle.m_Animations;
			foreach (CharacterStyle.AnimationInfo animationInfo in animations)
			{
				dictionary[((AssetReference)animationInfo.animationAsset).guid] = animationInfo;
			}
		}
		characterStyle.m_ShapeCount = style.shapeCount;
		characterStyle.m_BoneCount = style.boneCount;
		characterStyle.m_Animations = new CharacterStyle.AnimationInfo[style.animations.Count];
		Dictionary<int, Animation> dictionary2 = new Dictionary<int, Animation>();
		foreach (var item in style.animations)
		{
			if ((int)item.data.type == -1)
			{
				(dictionary2[((int)item.data.layer == 1) ? item.targetRenderGroup : (-1)], _, _, _, _) = item;
			}
		}
		foreach (var item2 in style.animations)
		{
			using (s_Progress.ScopedThreadDescription("Processing animation " + item2.data.name))
			{
				if ((int)item2.data.layer != 1)
				{
					Assert.AreEqual(item2.boneHierarchy.hierarchyParentIndices.Length, style.boneCount);
				}
				AnimationClip val = new AnimationClip
				{
					m_BoneHierarchy = item2.boneHierarchy,
					m_Animation = item2.data
				};
				bool flag = false;
				if (!overrideSafeGuard.TryGetValue(item2.hash, out var value))
				{
					AnimationAsset val2 = AnimationAssetExtensions.AddAsset(AssetDatabase.game, AssetDataPath.Create("StreamingData~", $"{val.name}_{HashUtils.GetHash(val, sourcePath)}", (EscapeStrategy)2), val);
					try
					{
						((AssetData)val2).Save(false);
						value = val2;
						overrideSafeGuard.Add(item2.hash, val2);
						flag = true;
					}
					finally
					{
						((IDisposable)val2)?.Dispose();
					}
				}
				if (dictionary.TryGetValue(Identifier.op_Implicit(((AssetData)value).id), out var value2))
				{
					characterStyle.m_Animations[num] = value2;
				}
				else
				{
					characterStyle.m_Animations[num] = new CharacterStyle.AnimationInfo();
					characterStyle.m_Animations[num].animationAsset = AssetReference<AnimationAsset>.op_Implicit(value);
				}
				characterStyle.m_Animations[num].name = item2.shortName;
				characterStyle.m_Animations[num].type = item2.data.type;
				characterStyle.m_Animations[num].layer = item2.data.layer;
				characterStyle.m_Animations[num].frameCount = item2.data.frameCount;
				characterStyle.m_Animations[num].frameRate = item2.data.frameRate;
				if (item2.targetRenderGroup != -1)
				{
					characterStyle.m_Animations[num].target = prefabs[item2.targetRenderGroup];
				}
				if (flag)
				{
					if ((int)item2.data.layer == 0 || (int)item2.data.layer == 1)
					{
						int key2 = (((int)item2.data.layer == 1) ? item2.targetRenderGroup : (-1));
						characterStyle.CalculateRootMotion(item2.boneHierarchy, item2.data, dictionary2[key2], num);
					}
					else
					{
						characterStyle.m_Animations[num].rootMotionBone = -1;
						characterStyle.m_Animations[num].rootMotion = null;
					}
				}
				num++;
			}
		}
		return characterStyle;
	}

	private unsafe static NativeArray<byte> FromManagedArray<T>(T[] array, int offset, int elementSize, int countPerElement) where T : unmanaged
	{
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		int num = UnsafeUtility.SizeOf<T>() / countPerElement;
		int num2 = array.Length * countPerElement;
		NativeArray<byte> val = new NativeArray<byte>(num2 * elementSize, (Allocator)4, (NativeArrayOptions)1);
		fixed (T* ptr = array)
		{
			void* ptr2 = ptr;
			void* ptr3 = (byte*)ptr2 + offset;
			UnsafeUtility.MemCpyStride(NativeArrayUnsafeUtility.GetUnsafePtr<byte>(val), elementSize, ptr3, num, elementSize, num2);
		}
		return val;
	}

	private static void SetupEmissiveComponent(RenderPrefab meshPrefab, LOD lod)
	{
		List<EmissiveProperties.SingleLightMapping> list = new List<EmissiveProperties.SingleLightMapping>();
		int num = 0;
		Surface[] surfaces = lod.surfaces;
		foreach (Surface val in surfaces)
		{
			if (val.HasProperty("_EmissiveColorMap"))
			{
				list.Add(new EmissiveProperties.SingleLightMapping
				{
					purpose = (val.name.Contains("Neon") ? EmissiveProperties.Purpose.NeonSign : EmissiveProperties.Purpose.DecorativeLight),
					intensity = 5f,
					materialId = num++
				});
			}
		}
		if (list.Count > 0)
		{
			if (!meshPrefab.TryGet<EmissiveProperties>(out var component))
			{
				log.WarnFormat((Object)(object)meshPrefab, "Mesh prefab {1} was missing EmissiveProperties. {0} single lights found. Please set up correctly...", (object)list.Count, (object)((Object)meshPrefab).name);
				component = meshPrefab.AddComponent<EmissiveProperties>();
				component.m_SingleLights = list;
			}
			else if (component.m_SingleLights.Count != list.Count)
			{
				log.WarnFormat((Object)(object)meshPrefab, "Mesh prefab {2} has an EmissiveProperties but the asset contains a different lightCount than set. Expected: {0} Found: {1}. Please set up correctly...", (object)list.Count, (object)component.m_SingleLights.Count, (object)((Object)meshPrefab).name);
			}
		}
	}

	private static void ImportTextures(Settings settings, string relativeRootPath, AssetGroup<IAsset> assetGroup, (ImportStep step, Asset asset) report)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		ImportTextures(settings, relativeRootPath, assetGroup, null, report);
	}

	private unsafe static void ImportTextures(Settings settings, string relativeRootPath, AssetGroup<IAsset> assetGroup, Func<TextureAsset, bool> predicate, (ImportStep step, Asset asset) report)
	{
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b5: Unknown result type (might be due to invalid IL or missing references)
		ProfilerMarker val = s_ProfImportTextures;
		AutoScope val2 = ((ProfilerMarker)(ref val)).Auto();
		try
		{
			ImportStep texturesReport = ((ReportBase)report.step).AddImportStep("Import Textures");
			try
			{
				ParallelOptions parallelOptions = new ParallelOptions
				{
					MaxDegreeOfParallelism = ((!useParallelImport) ? 1 : Environment.ProcessorCount)
				};
				int failures = 0;
				int totalTif = 0;
				int totalPng = 0;
				int total = 0;
				int totalNpot = 0;
				int totalNon8Bpp = 0;
				long totalFileSize = 0L;
				long totalWidth = 0L;
				long totalHeight = 0L;
				PerformanceCounter val3 = PerformanceCounter.Start((Action<TimeSpan>)delegate(TimeSpan t)
				{
					double num = (double)totalFileSize * 1.0 / (double)total;
					double num2 = (double)totalWidth * 1.0 / (double)total;
					double num3 = (double)totalHeight * 1.0 / (double)total;
					if (total == 0)
					{
						log.Info((object)$"No textures processed. All textures in this group were already loaded. {t.TotalSeconds:F3}");
					}
					else
					{
						log.Info((object)$"Completed {total} textures import in {t.TotalSeconds:F3}s. Errors {failures}, png {totalPng}, tif {totalTif}, NPOT {totalNpot}; 16bpp {totalNon8Bpp}. Total size {FormatUtils.FormatBytes(totalFileSize)}, avg size {FormatUtils.FormatBytes((long)num)}, {num2:F0}x{num3:F0}");
					}
				});
				try
				{
					Parallel.ForEach(Extensions.FilterBy<TextureAsset>(assetGroup, predicate), parallelOptions, delegate(TextureAsset asset, ParallelLoopState state, long index)
					{
						//IL_0102: Unknown result type (might be due to invalid IL or missing references)
						//IL_010e: Unknown result type (might be due to invalid IL or missing references)
						//IL_0142: Unknown result type (might be due to invalid IL or missing references)
						//IL_0226: Unknown result type (might be due to invalid IL or missing references)
						//IL_022c: Invalid comparison between Unknown and I4
						//IL_0245: Unknown result type (might be due to invalid IL or missing references)
						//IL_024b: Invalid comparison between Unknown and I4
						if (((ConcreteAsset<TextureImporter, Texture>)(object)asset).instance != null)
						{
							return;
						}
						FileReport val4 = report.asset.AddFile((IAsset)(object)asset);
						using (s_Progress.ScopedThreadDescription("Importing " + ((ConcreteAsset<TextureImporter, Texture>)(object)asset).fileName))
						{
							if (s_Progress.shouldCancel)
							{
								state.Stop();
							}
							try
							{
								ISettings importSettings = ((Settings)(ref settings)).GetImportSettings(((ConcreteAsset<TextureImporter, Texture>)(object)asset).fileName, (IAssetImporter)(object)((ConcreteAsset<TextureImporter, Texture>)(object)asset).importer, (ReportBase)(object)val4);
								ImportStep val5 = ((ReportBase)texturesReport).AddImportStep("Asset import");
								try
								{
									((ConcreteAsset<TextureImporter, Texture>)(object)asset).instance = ((ConcreteAsset<TextureImporter, Texture>)(object)asset).importer.Import(importSettings, ((ConcreteAsset<TextureImporter, Texture>)(object)asset).path, val4);
								}
								finally
								{
									((IDisposable)val5)?.Dispose();
								}
								((ConcreteAsset<TextureImporter, Texture>)(object)asset).instance.sourceAsset = (IAsset)(object)asset;
								Interlocked.Increment(ref total);
								if (((ConcreteAsset<TextureImporter, Texture>)(object)asset).instance != null)
								{
									ISettings val6 = default(ISettings);
									Context val7 = default(Context);
									foreach (ITexturePostProcessor texturePostProcessor in PostProcessorCache.GetTexturePostProcessors())
									{
										if (((Settings)(ref settings)).GetPostProcessSettings(((ConcreteAsset<TextureImporter, Texture>)(object)asset).fileName, (ISettingable)(object)texturePostProcessor, (ReportBase)(object)val4, ref val6))
										{
											((Context)(ref val7))._002Ector(s_MainThreadDispatcher, relativeRootPath, OnDebugTexture, val6, settings);
											if (texturePostProcessor.ShouldExecute(val7, asset))
											{
												val5 = ((ReportBase)texturesReport).AddImportStep("Execute " + PPUtils.GetPostProcessorString(((object)texturePostProcessor).GetType(), true) + " Post Processors");
												try
												{
													texturePostProcessor.Execute(val7, asset, val4);
												}
												finally
												{
													((IDisposable)val5)?.Dispose();
												}
											}
										}
									}
									Interlocked.Add(ref totalFileSize, ((ConcreteAsset<TextureImporter, Texture>)(object)asset).instance.fileDataLength);
									Interlocked.Add(ref totalWidth, ((ConcreteAsset<TextureImporter, Texture>)(object)asset).instance.info.width);
									Interlocked.Add(ref totalHeight, ((ConcreteAsset<TextureImporter, Texture>)(object)asset).instance.info.height);
									if (!Mathf.IsPowerOfTwo(((ConcreteAsset<TextureImporter, Texture>)(object)asset).instance.info.width) || !Mathf.IsPowerOfTwo(((ConcreteAsset<TextureImporter, Texture>)(object)asset).instance.info.height))
									{
										Interlocked.Increment(ref totalNpot);
									}
									if (((ConcreteAsset<TextureImporter, Texture>)(object)asset).instance.info.bpp != 8)
									{
										Interlocked.Increment(ref totalNon8Bpp);
									}
									if ((int)((ConcreteAsset<TextureImporter, Texture>)(object)asset).instance.info.fileFormat == 1)
									{
										Interlocked.Increment(ref totalPng);
									}
									if ((int)((ConcreteAsset<TextureImporter, Texture>)(object)asset).instance.info.fileFormat == 2)
									{
										Interlocked.Increment(ref totalTif);
									}
								}
							}
							catch (Exception ex)
							{
								Interlocked.Increment(ref failures);
								log.Error(ex, (object)("Error importing " + ((ConcreteAsset<TextureImporter, Texture>)(object)asset).name + ". Skipped..."));
							}
						}
					});
				}
				finally
				{
					((IDisposable)val3)?.Dispose();
				}
			}
			finally
			{
				if (texturesReport != null)
				{
					((IDisposable)texturesReport).Dispose();
				}
			}
		}
		finally
		{
			((IDisposable)(*(AutoScope*)(&val2))/*cast due to .constrained prefix*/).Dispose();
		}
	}

	private unsafe static void CreateGeometriesAndSurfaces(Settings settings, string relativeRootPath, AssetGroup<IAsset> assetGroup, out Action<string, ImportMode, Report, HashSet<SurfaceAsset>, IPrefabFactory> postImportOperations, (Report parent, Asset asset) report)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_068a: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c3: Expected O, but got Unknown
		//IL_00e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_0714: Unknown result type (might be due to invalid IL or missing references)
		//IL_073d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0744: Expected O, but got Unknown
		//IL_0933: Unknown result type (might be due to invalid IL or missing references)
		//IL_093d: Expected O, but got Unknown
		//IL_0416: Unknown result type (might be due to invalid IL or missing references)
		//IL_041d: Expected O, but got Unknown
		//IL_017f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0186: Unknown result type (might be due to invalid IL or missing references)
		//IL_0490: Unknown result type (might be due to invalid IL or missing references)
		//IL_049a: Expected O, but got Unknown
		//IL_045f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0469: Expected O, but got Unknown
		//IL_02ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_030b: Unknown result type (might be due to invalid IL or missing references)
		//IL_087b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0887: Unknown result type (might be due to invalid IL or missing references)
		//IL_08ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_05be: Unknown result type (might be due to invalid IL or missing references)
		//IL_05ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_05fb: Unknown result type (might be due to invalid IL or missing references)
		ProfilerMarker val = s_ProfCreateGeomsSurfaces;
		AutoScope val2 = ((ProfilerMarker)(ref val)).Auto();
		try
		{
			ImportStep val3 = ((ReportBase)report.parent).AddImportStep("Create Geometry and Surfaces");
			try
			{
				Dictionary<ModelAsset, List<List<(Model, Surface)>>> dictionary = new Dictionary<ModelAsset, List<List<(Model, Surface)>>>();
				ISettings val6 = default(ISettings);
				Context val7 = default(Context);
				foreach (ModelAsset item2 in Extensions.FilterBy<ModelAsset>(assetGroup, (Func<ModelAsset, bool>)null))
				{
					int lod = ((Asset<ModelImporter, ModelList>)(object)item2).lod;
					if (!dictionary.TryGetValue(item2, out var value))
					{
						value = new List<List<(Model, Surface)>>();
						dictionary.Add(item2, value);
					}
					while (value.Count <= lod)
					{
						value.Add(new List<(Model, Surface)>());
					}
					List<(Model, Surface)> list = value[lod];
					for (int i = 0; i < ((ConcreteAsset<ModelImporter, ModelList>)(object)item2).instance.Count; i++)
					{
						Model val4 = ((ConcreteAsset<ModelImporter, ModelList>)(object)item2).instance[i];
						Surface val5 = new Surface(val4.name, (string)null);
						AssetData item = report.parent.AddAssetData(val5.name, typeof(Surface), false);
						foreach (TextureAsset item3 in Extensions.FilterBy<TextureAsset>(assetGroup, (Func<TextureAsset, bool>)null))
						{
							string bakingTextureProperty = Material.GetBakingTextureProperty(((Asset<TextureImporter, Texture>)(object)item3).suffix);
							if (bakingTextureProperty != null && !val5.HasBakingTexture(bakingTextureProperty) && ((Asset<TextureImporter, Texture>)(object)item3).material == ((Asset<ModelImporter, ModelList>)(object)item2).material)
							{
								val5.AddBakingTexture(bakingTextureProperty, ((ConcreteAsset<TextureImporter, Texture>)(object)item3).instance);
							}
							bakingTextureProperty = Material.GetShaderProperty(((Asset<TextureImporter, Texture>)(object)item3).suffix, report.asset);
							if (bakingTextureProperty != null && (!val5.HasProperty(bakingTextureProperty) || (((Asset<TextureImporter, Texture>)(object)item3).assetName == ((Asset<ModelImporter, ModelList>)(object)item2).assetName && ((Asset<TextureImporter, Texture>)(object)item3).module == ((Asset<ModelImporter, ModelList>)(object)item2).module)) && ((Asset<TextureImporter, Texture>)(object)item3).material == ((Asset<ModelImporter, ModelList>)(object)item2).material && ((Asset<TextureImporter, Texture>)(object)item3).lod == ((Asset<ModelImporter, ModelList>)(object)item2).lod)
							{
								val5.AddProperty(bakingTextureProperty, (ITexture)(object)((ConcreteAsset<TextureImporter, Texture>)(object)item3).instance);
								report.parent.AddAssetData(Path.GetFileNameWithoutExtension(((ConcreteAsset<TextureImporter, Texture>)(object)item3).fileName), typeof(Texture), false).AddFile((IAsset)(object)item3);
							}
						}
						if (i == 0 && ((ConcreteAsset<ModelImporter, ModelList>)(object)item2).name == settings.mainAsset)
						{
							list.Insert(0, (val4, val5));
						}
						else
						{
							list.Add((val4, val5));
						}
						FileReport fileReport = report.parent.GetFileReport((IAsset)(object)item2);
						foreach (IModelSurfacePostProcessor modelSurfacePostProcessor in PostProcessorCache.GetModelSurfacePostProcessors())
						{
							string text = ((ConcreteAsset<ModelImporter, ModelList>)(object)item2).name;
							if (((ConcreteAsset<ModelImporter, ModelList>)(object)item2).instance.Count > 1)
							{
								text += $"#{i}";
							}
							if (!((Settings)(ref settings)).GetPostProcessSettings(text, (ISettingable)(object)modelSurfacePostProcessor, (ReportBase)(object)fileReport, ref val6))
							{
								continue;
							}
							((Context)(ref val7))._002Ector(s_MainThreadDispatcher, relativeRootPath, OnDebugTexture, val6, settings);
							if (modelSurfacePostProcessor.ShouldExecute(val7, item2, i, val5))
							{
								ImportStep val8 = ((ReportBase)val3).AddImportStep("Execute " + PPUtils.GetPostProcessorString(((object)modelSurfacePostProcessor).GetType(), true) + " Post Processors");
								try
								{
									modelSurfacePostProcessor.Execute(val7, item2, i, val5, (report.parent, report.asset, fileReport, item));
								}
								finally
								{
									((IDisposable)val8)?.Dispose();
								}
							}
						}
					}
				}
				List<List<LOD>> assets = new List<List<LOD>>(dictionary.Count);
				if (dictionary.Count > 0)
				{
					ISettings val12 = default(ISettings);
					Context val13 = default(Context);
					foreach (List<List<(Model, Surface)>> value2 in dictionary.Values)
					{
						List<LOD> list2 = new List<LOD>();
						for (int j = 0; j < value2.Count; j++)
						{
							List<(Model, Surface)> list3 = value2[j];
							if (list3.Count == 0)
							{
								continue;
							}
							Geometry val9 = new Geometry(list3.Select<(Model, Surface), Model>(((Model model, Surface surface) t) => t.model).ToArray());
							Surface[] array = list3.Select<(Model, Surface), Surface>(((Model model, Surface surface) t) => t.surface).ToArray();
							if (list2.Count > j)
							{
								list2[j] = new LOD(val9, array, j);
								((ReportBase)report.asset).AddWarning($"LOD {j} already exist and was replaced by last imported.");
							}
							else
							{
								list2.Add(new LOD(val9, array, j));
							}
							AssetData val10 = report.parent.AddAssetData(val9.name, typeof(Geometry), false);
							val10.AddFiles(list3.Select<(Model, Surface), IAsset>(((Model model, Surface surface) t) => t.model.sourceAsset));
							AssetData[] array2 = (AssetData[])(object)new AssetData[array.Length];
							int num = 0;
							Surface[] array3 = array;
							foreach (Surface val11 in array3)
							{
								array2[num] = report.parent.AddAssetData(val11.name, typeof(Surface), false);
								array2[num++].AddFiles(val11.textures.Values.Select((ITexture t) => t.sourceAsset));
							}
							foreach (IGeometryPostProcessor geometryPostProcessor in PostProcessorCache.GetGeometryPostProcessors())
							{
								if (!((Settings)(ref settings)).GetPostProcessSettings(val9.name, (ISettingable)(object)geometryPostProcessor, (ReportBase)(object)report.asset, ref val12))
								{
									continue;
								}
								((Context)(ref val13))._002Ector(s_MainThreadDispatcher, relativeRootPath, OnDebugTexture, val12, settings);
								if (geometryPostProcessor.ShouldExecute(val13, list2))
								{
									ImportStep val8 = ((ReportBase)val3).AddImportStep("Execute " + PPUtils.GetPostProcessorString(((object)geometryPostProcessor).GetType(), true) + " Post Processors");
									try
									{
										geometryPostProcessor.Execute(val13, list2, (report.parent, report.asset, val10, array2));
									}
									finally
									{
										((IDisposable)val8)?.Dispose();
									}
								}
							}
						}
						assets.Add(list2);
					}
				}
				else
				{
					List<LOD> list4 = new List<LOD>();
					Dictionary<string, List<TextureAsset>> dictionary2 = (from t in Extensions.FilterBy<TextureAsset>(assetGroup, (Func<TextureAsset, bool>)null)
						group t by ((Asset<TextureImporter, Texture>)(object)t).material).ToDictionary((IGrouping<string, TextureAsset> g) => g.Key, (IGrouping<string, TextureAsset> g) => g.ToList());
					List<Surface> list5 = new List<Surface>();
					ISettings val16 = default(ISettings);
					Context val17 = default(Context);
					foreach (KeyValuePair<string, List<TextureAsset>> item4 in dictionary2)
					{
						string text2 = assetGroup.name;
						if (!string.IsNullOrEmpty(item4.Key))
						{
							text2 += item4.Key;
						}
						Surface val14 = new Surface(text2, (string)null);
						AssetData val15 = report.parent.AddAssetData(val14.name, typeof(Surface), false);
						foreach (TextureAsset item5 in item4.Value)
						{
							string bakingTextureProperty2 = Material.GetBakingTextureProperty(((Asset<TextureImporter, Texture>)(object)item5).suffix);
							if (bakingTextureProperty2 != null && !val14.HasBakingTexture(bakingTextureProperty2))
							{
								val14.AddBakingTexture(bakingTextureProperty2, ((ConcreteAsset<TextureImporter, Texture>)(object)item5).instance);
							}
							bakingTextureProperty2 = Material.GetShaderProperty(((Asset<TextureImporter, Texture>)(object)item5).suffix, report.asset);
							if (bakingTextureProperty2 != null && !val14.HasProperty(bakingTextureProperty2) && ((Asset<TextureImporter, Texture>)(object)item5).material == item4.Key)
							{
								val14.AddProperty(bakingTextureProperty2, (ITexture)(object)((ConcreteAsset<TextureImporter, Texture>)(object)item5).instance);
								report.parent.AddAssetData(Path.GetFileNameWithoutExtension(((ConcreteAsset<TextureImporter, Texture>)(object)item5).fileName), typeof(Texture), false).AddFile((IAsset)(object)item5);
							}
						}
						foreach (IModelSurfacePostProcessor modelSurfacePostProcessor2 in PostProcessorCache.GetModelSurfacePostProcessors())
						{
							if (!((Settings)(ref settings)).GetPostProcessSettings(text2, (ISettingable)(object)modelSurfacePostProcessor2, (ReportBase)(object)val15, ref val16))
							{
								continue;
							}
							((Context)(ref val17))._002Ector(s_MainThreadDispatcher, relativeRootPath, OnDebugTexture, val16, settings);
							if (modelSurfacePostProcessor2.ShouldExecute(val17, (ModelAsset)null, 0, val14))
							{
								ImportStep val8 = ((ReportBase)val3).AddImportStep("Execute " + PPUtils.GetPostProcessorString(((object)modelSurfacePostProcessor2).GetType(), true) + " Post Processors");
								try
								{
									modelSurfacePostProcessor2.Execute(val17, (ModelAsset)null, 0, val14, (ValueTuple<Report, Asset, FileReport, AssetData>)(report.parent, report.asset, null, val15));
								}
								finally
								{
									((IDisposable)val8)?.Dispose();
								}
							}
						}
						list5.Add(val14);
					}
					list4.Add(new LOD((Geometry)null, list5.ToArray(), 0));
					assets.Add(list4);
				}
				postImportOperations = delegate(string sourcePath, ImportMode importMode, Report report2, HashSet<SurfaceAsset> VTMaterials, IPrefabFactory prefabFactory)
				{
					//IL_0001: Unknown result type (might be due to invalid IL or missing references)
					//IL_000d: Unknown result type (might be due to invalid IL or missing references)
					CreateRenderPrefabs(settings, sourcePath, assets, importMode, report2, VTMaterials, prefabFactory);
					DisposeLODs(assets);
				};
			}
			finally
			{
				((IDisposable)val3)?.Dispose();
			}
		}
		finally
		{
			((IDisposable)(*(AutoScope*)(&val2))/*cast due to .constrained prefix*/).Dispose();
		}
	}

	private unsafe static bool ImportAssetGroup(string projectRootPath, string relativeRootPath, AssetGroup<Asset> assetGroup, out List<List<LOD>> lods, out Action<string, ImportMode, Report, HashSet<SurfaceAsset>, IPrefabFactory> postImportOperations, Report report, Asset assetReport)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e8: Invalid comparison between Unknown and I4
		//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_00eb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fa: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fc: Unknown result type (might be due to invalid IL or missing references)
		lods = null;
		postImportOperations = null;
		ProfilerMarker val = s_ProfImportAssetGroup;
		AutoScope val2 = ((ProfilerMarker)(ref val)).Auto();
		try
		{
			log.Info((object)("Start processing " + ((object)assetGroup/*cast due to .constrained prefix*/).ToString()));
			ImportStep val3 = ((ReportBase)report).AddImportStep("Import asset group");
			try
			{
				Settings val4 = ImportSettings(assetGroup, (step: val3, asset: assetReport));
				AssetGroup<IAsset> assetGroup2 = CreateAssetGroupFromSettings(projectRootPath, val4, assetGroup, assetReport);
				foreach (IAsset item in assetGroup2)
				{
					log.Verbose((object)$"   {item}");
				}
				if ((int)val4.pipeline == 0)
				{
					ImportTextures(val4, relativeRootPath, assetGroup2, (step: val3, asset: assetReport));
					ImportModels(val4, relativeRootPath, assetGroup2, (step: val3, asset: assetReport));
					CreateGeometriesAndSurfaces(val4, relativeRootPath, assetGroup2, out postImportOperations, (parent: report, asset: assetReport));
				}
				else if ((int)val4.pipeline == 1)
				{
					ImportDidimo(val4, assetGroup2, (step: val3, asset: assetReport));
					CreateDidimoAssets(val4, relativeRootPath, assetGroup2, out postImportOperations, (parent: report, step: val3, asset: assetReport));
				}
			}
			finally
			{
				((IDisposable)val3)?.Dispose();
			}
			return true;
		}
		catch (Exception ex)
		{
			log.ErrorFormat(ex, "Error processing {0}.. Skipped!", (object)((object)assetGroup/*cast due to .constrained prefix*/).ToString());
			lods = null;
			return false;
		}
		finally
		{
			((IDisposable)(*(AutoScope*)(&val2))/*cast due to .constrained prefix*/).Dispose();
		}
	}

	private static bool IsLODsValid(IReadOnlyList<List<LOD>> assets)
	{
		if (assets == null || assets.Count == 0)
		{
			return false;
		}
		foreach (List<LOD> asset in assets)
		{
			foreach (LOD item in asset)
			{
				if (item.geometry == null && (item.surfaces == null || item.surfaces.Length == 0))
				{
					return false;
				}
				if (item.geometry != null && !item.geometry.isValid)
				{
					return false;
				}
				if (item.surfaces != null && (item.surfaces.Length == 0 || item.surfaces.Any((Surface surface) => !surface.isValid)))
				{
					return false;
				}
			}
		}
		return true;
	}

	private static void DisposeLODs(IReadOnlyList<IReadOnlyList<LOD>> assets)
	{
		foreach (IReadOnlyList<LOD> asset in assets)
		{
			DisposeLODs(asset);
		}
	}

	private static void DisposeLODs(IReadOnlyList<LOD> assets)
	{
		foreach (LOD asset in assets)
		{
			asset.Dispose();
		}
	}

	private static IReadOnlyList<(RenderPrefab prefab, Prefab report)> CreateRenderPrefab(Settings settings, string sourcePath, IReadOnlyList<LOD> asset, ImportMode importMode, Report report, HashSet<SurfaceAsset> VTMaterials, IPrefabFactory prefabFactory = null)
	{
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		//IL_0151: Unknown result type (might be due to invalid IL or missing references)
		//IL_02aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_0280: Unknown result type (might be due to invalid IL or missing references)
		//IL_0281: Unknown result type (might be due to invalid IL or missing references)
		//IL_0098: Unknown result type (might be due to invalid IL or missing references)
		//IL_02bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b9: Invalid comparison between Unknown and I4
		//IL_0288: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ba: Unknown result type (might be due to invalid IL or missing references)
		List<(RenderPrefab, Prefab)> list = new List<(RenderPrefab, Prefab)>(asset.Count);
		try
		{
			foreach (LOD item in asset)
			{
				RenderPrefab renderPrefab = CreateRenderPrefab(sourcePath, item.name, item.level, prefabFactory);
				Prefab val = report.AddPrefab(((Object)renderPrefab).name);
				list.Add((renderPrefab, val));
				if (Extensions.Has(importMode, (ImportMode)1))
				{
					Geometry geometry = item.geometry;
					if (geometry != null)
					{
						GeometryAsset val2 = GeometryAssetExtensions.AddAsset(targetDatabase, AssetDataPath.Create("StreamingData~", $"{item.name}_{HashUtils.GetHash(item, sourcePath)}", (EscapeStrategy)2), geometry);
						try
						{
							((AssetData)val2).Save(false);
							report.AddInfoToAsset(item.name, typeof(Geometry), (IAddressableAsset)(object)val2);
							renderPrefab.geometryAsset = val2;
							renderPrefab.bounds = geometry.CalcBounds();
							renderPrefab.surfaceArea = geometry.CalcSurfaceArea();
							renderPrefab.indexCount = geometry.CalcTotalIndices();
							renderPrefab.vertexCount = geometry.CalcTotalVertices();
							renderPrefab.meshCount = geometry.models.Length;
						}
						finally
						{
							((IDisposable)val2)?.Dispose();
						}
					}
					else if (item.surfaces != null)
					{
						renderPrefab.meshCount = item.surfaces.Length;
					}
				}
				if (Extensions.Has(importMode, (ImportMode)2))
				{
					Surface[] surfaces = item.surfaces;
					if (surfaces != null)
					{
						SurfaceAsset[] array = (SurfaceAsset[])(object)new SurfaceAsset[surfaces.Length];
						for (int i = 0; i < surfaces.Length; i++)
						{
							Surface val3 = surfaces[i];
							try
							{
								EventExtensions.Subscribe<TextureAsset>(((IAssetDatabase)targetDatabase).onAssetDatabaseChanged, (EventDelegate<AssetChangedEventArgs>)OnTextureAdded);
								SurfaceAsset val4 = SurfaceAssetExtensions.AddAsset(targetDatabase, AssetDataPath.Create("StreamingData~", $"{val3.name}_{HashUtils.GetHash(val3, sourcePath)}", (EscapeStrategy)2), val3);
								try
								{
									val4.Save(0, false, true, false, (VirtualTexturingConfig)null, (Dictionary<TextureAsset, List<SurfaceAsset>>)null, (int?)null, (int?)null);
									VTMaterials.Add(val4);
									array[i] = val4;
									report.AddInfoToAsset(val3.name, typeof(Surface), (IAddressableAsset)(object)val4);
								}
								finally
								{
									((IDisposable)val4)?.Dispose();
								}
							}
							finally
							{
								((IAssetDatabase)targetDatabase).onAssetDatabaseChanged.Unsubscribe((EventDelegate<AssetChangedEventArgs>)OnTextureAdded);
							}
							if (val3.isImpostor)
							{
								renderPrefab.isImpostor = true;
							}
						}
						renderPrefab.surfaceAssets = array;
					}
				}
				if ((int)settings.pipeline == 0)
				{
					SetupComponents(settings, renderPrefab, item, val);
				}
			}
			Pipeline pipeline = settings.pipeline;
			if ((int)pipeline == 0 || (int)pipeline == 1)
			{
				SetupLODs(settings, list);
			}
		}
		catch (Exception ex)
		{
			log.Error(ex, (object)("Error occured with " + asset[0].name));
		}
		return list;
		void OnTextureAdded(AssetChangedEventArgs args)
		{
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_000d: Expected O, but got Unknown
			TextureAsset val5 = (TextureAsset)((AssetChangedEventArgs)(ref args)).asset;
			report.AddInfoToAsset(GetNameWithoutGUID(((AssetData)val5).name), typeof(Texture), (IAddressableAsset)(object)val5);
		}
	}

	private static RenderPrefab CreateRenderPrefab(string sourcePath, string name, int lodLevel, IPrefabFactory prefabFactory = null)
	{
		return CreatePrefab<RenderPrefab>("Mesh", sourcePath, name, lodLevel, prefabFactory);
	}

	private static T CreatePrefab<T>(string suffix, string sourcePath, string name, int lodLevel, IPrefabFactory prefabFactory = null) where T : PrefabBase
	{
		string name2 = name + ((!string.IsNullOrEmpty(suffix)) ? (" " + suffix) : string.Empty);
		T val = ((prefabFactory != null) ? prefabFactory.CreatePrefab<T>(sourcePath, name2, lodLevel) : null);
		if ((Object)(object)val == (Object)null)
		{
			val = ScriptableObject.CreateInstance<T>();
			((Object)val).name = name2;
		}
		return val;
	}

	private unsafe static void CreateRenderPrefabs(Settings settings, string sourcePath, IReadOnlyList<List<LOD>> assets, ImportMode importMode, Report report, HashSet<SurfaceAsset> VTMaterials, IPrefabFactory prefabFactory = null)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		ProfilerMarker val = s_ProfPostImport;
		AutoScope val2 = ((ProfilerMarker)(ref val)).Auto();
		try
		{
			if (!IsLODsValid(assets))
			{
				log.DebugFormat("Result for {0} is not valid and will not be serialized", (object)sourcePath);
				return;
			}
			string name = assets[0][0].name;
			try
			{
				ImportStep val3 = ((ReportBase)report).AddImportStep("Perform main thread tasks (Assetdatabase serialization + Prefabs upgrade)");
				try
				{
					foreach (List<LOD> asset in assets)
					{
						CreateRenderPrefab(settings, sourcePath, asset, importMode, report, VTMaterials, prefabFactory);
					}
				}
				finally
				{
					((IDisposable)val3)?.Dispose();
				}
			}
			catch (Exception ex)
			{
				log.Error(ex, (object)("Error post-importing " + name + "."));
				((ReportBase)report).AddError("Error post-importing " + name + ": " + ex.Message + ".");
			}
		}
		finally
		{
			((IDisposable)(*(AutoScope*)(&val2))/*cast due to .constrained prefix*/).Dispose();
		}
	}

	private static void SetupLODs(Settings settings, IReadOnlyList<(RenderPrefab prefab, Prefab report)> meshPrefabs)
	{
		if (meshPrefabs.Count <= 1)
		{
			return;
		}
		RenderPrefab item = meshPrefabs[0].prefab;
		ProceduralAnimationProperties component = item.GetComponent<ProceduralAnimationProperties>();
		ContentPrerequisite component2 = item.GetComponent<ContentPrerequisite>();
		LodProperties lodProperties = item.AddOrGetComponent<LodProperties>();
		meshPrefabs[0].report.AddComponent(((object)lodProperties).ToString());
		lodProperties.m_LodMeshes = new RenderPrefab[meshPrefabs.Count - 1];
		for (int i = 1; i < meshPrefabs.Count; i++)
		{
			if ((Object)(object)component != (Object)null)
			{
				ProceduralAnimationProperties proceduralAnimationProperties = meshPrefabs[i].prefab.AddComponentFrom(component);
				meshPrefabs[i].report.AddComponent(((object)proceduralAnimationProperties).ToString());
			}
			if ((Object)(object)component2 != (Object)null)
			{
				ContentPrerequisite contentPrerequisite = meshPrefabs[i].prefab.AddComponentFrom(component2);
				meshPrefabs[i].report.AddComponent(((object)contentPrerequisite).ToString());
			}
			lodProperties.m_LodMeshes[i - 1] = meshPrefabs[i].prefab;
		}
	}

	private static void SetupComponents(Settings settings, RenderPrefab meshPrefab, LOD lod, Prefab report)
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		if (lod.level == 0)
		{
			SetupEmissiveComponent(settings, meshPrefab, lod, report);
			if (settings.useProceduralAnimation)
			{
				SetupProceduralAnimationComponent(settings, meshPrefab, lod, report);
			}
		}
	}

	private static void SetupEmissiveComponent(Settings settings, RenderPrefab meshPrefab, LOD lod, Prefab report)
	{
		//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00de: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f7: Unknown result type (might be due to invalid IL or missing references)
		//IL_03b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_03b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_03d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_03d6: Unknown result type (might be due to invalid IL or missing references)
		List<EmissiveProperties.MultiLightMapping> multiLightProps = new List<EmissiveProperties.MultiLightMapping>();
		List<EmissiveProperties.SingleLightMapping> list = new List<EmissiveProperties.SingleLightMapping>();
		int num = 0;
		Surface[] surfaces = lod.surfaces;
		foreach (Surface val in surfaces)
		{
			if (val.emissiveLayers.Count == 0)
			{
				if (val.HasProperty("_EmissiveColorMap"))
				{
					list.Add(new EmissiveProperties.SingleLightMapping
					{
						purpose = (val.name.Contains("Neon") ? EmissiveProperties.Purpose.NeonSign : EmissiveProperties.Purpose.DecorativeLight),
						intensity = 5f,
						materialId = num++
					});
				}
				continue;
			}
			foreach (EmissiveLayer emissiveLayer in val.emissiveLayers)
			{
				multiLightProps.Add(new EmissiveProperties.MultiLightMapping
				{
					intensity = emissiveLayer.intensity,
					luminance = emissiveLayer.luminance,
					color = emissiveLayer.color,
					layerId = emissiveLayer.layerId,
					purpose = EmissiveProperties.Purpose.None,
					colorOff = Color.black,
					animationIndex = -1,
					responseTime = 0f
				});
			}
		}
		if (list.Count > 0)
		{
			if (!meshPrefab.TryGet<EmissiveProperties>(out var component))
			{
				component = meshPrefab.AddComponent<EmissiveProperties>();
				component.m_SingleLights = list;
				((ReportBase)report.AddComponent(((object)component).ToString())).AddMessage($"Missing EmissiveProperties. {list.Count} single lights found. Please set up correctly...", (Severity)4000);
				log.WarnFormat((Object)(object)meshPrefab, "Mesh prefab {1} was missing EmissiveProperties. {0} single lights found. Please set up correctly...", (object)list.Count, (object)((Object)meshPrefab).name);
			}
			else if (component.m_SingleLights.Count != list.Count)
			{
				((ReportBase)report.AddComponent(((object)component).ToString())).AddMessage($"EmissiveProperties already added but the asset contains a different lightCount than set. Expected: {list.Count} Found: {component.m_SingleLights.Count}. Please set up correctly...", (Severity)4000);
				log.WarnFormat((Object)(object)meshPrefab, "Mesh prefab {2} has an EmissiveProperties but the asset contains a different lightCount than set. Expected: {0} Found: {1}. Please set up correctly...", (object)list.Count, (object)component.m_SingleLights.Count, (object)((Object)meshPrefab).name);
			}
		}
		if (multiLightProps.Count <= 0)
		{
			return;
		}
		if (!meshPrefab.TryGet<EmissiveProperties>(out var component2))
		{
			component2 = meshPrefab.AddComponent<EmissiveProperties>();
			component2.m_MultiLights = multiLightProps;
			((ReportBase)report.AddComponent(((object)component2).ToString())).AddMessage($"Missing EmissiveProperties. {multiLightProps.Count} light layers found. Please set up correctly...", (Severity)4000);
			log.WarnFormat((Object)(object)meshPrefab, "Mesh prefab {1} was missing EmissiveProperties. {0} light layers found. Please set up correctly...", (object)multiLightProps.Count, (object)((Object)meshPrefab).name);
			return;
		}
		if (component2.m_MultiLights.Count != multiLightProps.Count)
		{
			((ReportBase)report.AddComponent(((object)component2).ToString())).AddWarning($"EmissiveProperties already added but the asset contains a different light layer count than set. Expected: {list.Count} Found: {component2.m_MultiLights.Count}. Please set up correctly...");
			log.WarnFormat((Object)(object)meshPrefab, "Mesh prefab {2} has an EmissiveProperties but the asset contains a different light layer count than set. Expected: {0} Found: {1}. Please set up correctly...", (object)list.Count, (object)component2.m_MultiLights.Count, (object)((Object)meshPrefab).name);
		}
		int i2;
		int i;
		for (i2 = 0; i2 < multiLightProps.Count; i2 = i)
		{
			EmissiveProperties.MultiLightMapping multiLightMapping = component2.m_MultiLights.Find((EmissiveProperties.MultiLightMapping x) => x.layerId == multiLightProps[i2].layerId);
			if (multiLightMapping != null)
			{
				multiLightProps[i2].purpose = multiLightMapping.purpose;
				multiLightProps[i2].color = multiLightMapping.color;
				multiLightProps[i2].colorOff = multiLightMapping.colorOff;
				multiLightProps[i2].animationIndex = multiLightMapping.animationIndex;
				multiLightProps[i2].responseTime = multiLightMapping.responseTime;
			}
			i = i2 + 1;
		}
		component2.m_MultiLights = multiLightProps;
	}

	private static bool GetSkinningInfo(Model model, out BoneInfo[] bones, Prefab report)
	{
		bones = model.bones;
		if (model.HasAttribute((VertexAttribute)13))
		{
			if (model.rootBoneIndex == -1)
			{
				((ReportBase)report).AddWarning(model.name + " is missing root bone");
				return false;
			}
			if (model.bones == null)
			{
				((ReportBase)report).AddWarning(model.name + " is missing bind poses");
				return false;
			}
			if (!model.HasAttribute((VertexAttribute)12) && model.GetAttributeData((VertexAttribute)13).dimension != 1)
			{
				((ReportBase)report).AddWarning(model.name + " has BlendIndices but no BlendWeight. Assuming rigid skinning..");
				return false;
			}
			return true;
		}
		if (model.HasAttribute((VertexAttribute)12))
		{
			((ReportBase)report).AddWarning(model.name + " has BlendWeight but is missing BlendIndices");
		}
		return false;
	}

	private static string GetUniqueString(string input, int currentIndex, ProceduralAnimationProperties.BoneInfo[] array)
	{
		int num = 0;
		for (int i = 0; i < currentIndex; i++)
		{
			if (array[i].name.StartsWith(input))
			{
				num++;
			}
		}
		if (num == 0)
		{
			return input;
		}
		return $"{input} {num}";
	}

	private static void SetupProceduralAnimationComponent(Settings settings, RenderPrefab meshPrefab, LOD lod, Prefab report)
	{
		//IL_00bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00db: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ed: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f2: Unknown result type (might be due to invalid IL or missing references)
		//IL_011b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0120: Unknown result type (might be due to invalid IL or missing references)
		if (lod.geometry == null || !GetSkinningInfo(lod.geometry.models[0], out var bones, report))
		{
			return;
		}
		if (!meshPrefab.TryGet<ProceduralAnimationProperties>(out var component))
		{
			log.WarnFormat((Object)(object)meshPrefab, "Mesh prefab {0} was missing ProceduralAnimationProperties. Please set up correctly...", (object)((Object)meshPrefab).name);
			component = meshPrefab.AddComponent<ProceduralAnimationProperties>();
			report.AddComponent(((object)component).ToString());
		}
		ProceduralAnimationProperties.BoneInfo[] bones2 = new ProceduralAnimationProperties.BoneInfo[bones.Length];
		int i = 0;
		while (i < bones.Length)
		{
			bones2[i] = new ProceduralAnimationProperties.BoneInfo
			{
				name = GetUniqueString(bones[i].name, i, bones2),
				position = bones[i].localPosition,
				rotation = bones[i].localRotation,
				scale = bones[i].localScale,
				parentId = bones[i].parentIndex,
				bindPose = bones[i].bindPose
			};
			if (component.m_Bones != null)
			{
				ProceduralAnimationProperties.BoneInfo boneInfo = Array.Find(component.m_Bones, (ProceduralAnimationProperties.BoneInfo x) => x.name == bones2[i].name);
				if (boneInfo != null)
				{
					bones2[i].m_Acceleration = boneInfo.m_Acceleration;
					bones2[i].m_Speed = boneInfo.m_Speed;
					bones2[i].m_ConnectionID = boneInfo.m_ConnectionID;
					bones2[i].m_SourceID = boneInfo.m_SourceID;
					bones2[i].m_Type = boneInfo.m_Type;
				}
			}
			int num = i + 1;
			i = num;
		}
		component.m_Bones = bones2;
	}

	private static Variant ToJsonSchema(object obj)
	{
		return ToJsonSchema(obj.GetType());
	}

	private static Variant ToJsonSchema(Type type, Variant previous = null)
	{
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Expected O, but got Unknown
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b8: Expected O, but got Unknown
		//IL_0074: Unknown result type (might be due to invalid IL or missing references)
		//IL_007a: Expected O, but got Unknown
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		//IL_008a: Expected O, but got Unknown
		Variant val = (Variant)(((object)previous) ?? ((object)new ProxyObject()));
		string text = ToJsonType(type);
		val["type"] = JSON.Load(text);
		ProxyObject properties = new ProxyObject();
		Extensions.ForEachField(type, (EncodeOptions)0, (MemberDelegate<FieldInfo>)delegate(FieldInfo fieldInfo, bool typeHint)
		{
			//IL_001a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0020: Expected O, but got Unknown
			ProxyObject val3 = (ProxyObject)JSON.Load("{ " + ToJsonSchema(fieldInfo) + " }");
			((Variant)properties)[fieldInfo.Name] = (Variant)(object)val3;
		});
		if (typeof(ISettings).IsAssignableFrom(type))
		{
			((Variant)properties)["@type"] = JSON.Load("{ \"$ref\": \"#/definitions/settingsType\" }");
			ProxyArray val2 = new ProxyArray();
			val2.Add((Variant)new ProxyString("@type"));
			val["required"] = (Variant)(object)val2;
		}
		val["properties"] = (Variant)(object)properties;
		val["additionalProperties"] = (Variant)new ProxyBoolean(false);
		if (((Variant)properties).Count <= 0 && !(text != "object"))
		{
			return null;
		}
		return val;
	}

	private static string ToJsonType(Type type, bool nullable = false)
	{
		if (type == typeof(string))
		{
			if (!nullable)
			{
				return "\"string\"";
			}
			return "[ \"string\", \"null\" ]";
		}
		if (type.IsEnum)
		{
			return "\"string\"";
		}
		if (type == typeof(uint))
		{
			return "\"nonNegativeInteger\"";
		}
		if (type == typeof(int))
		{
			return "\"integer\"";
		}
		if (type == typeof(float) || type == typeof(double))
		{
			return "\"number\"";
		}
		if (type == typeof(bool))
		{
			return "\"boolean\"";
		}
		if (type.IsArray || typeof(IList).IsAssignableFrom(type))
		{
			return "\"array\"";
		}
		return "\"object\"";
	}

	private static string GetSettings()
	{
		string[] array = (from x in (from settings in (from ext in ImporterCache.GetSupportedExtensions()
					select ((ISettingable)ImporterCache.GetImporter(ext.Key, (Type)null)).GetDefaultSettings()).Concat(from x in PostProcessorCache.GetTexturePostProcessors()
					select ((ISettingable)x).GetDefaultSettings()).Concat(from x in PostProcessorCache.GetModelPostProcessors()
					select ((ISettingable)x).GetDefaultSettings()).Concat(from x in PostProcessorCache.GetModelSurfacePostProcessors()
					select ((ISettingable)x).GetDefaultSettings())
					.Concat(from x in PostProcessorCache.GetGeometryPostProcessors()
						select ((ISettingable)x).GetDefaultSettings())
				where settings != null
				group settings by ((object)settings).GetType() into @group
				select @group.First()).Select(GetIfThen)
			where x != null
			select x).ToArray();
		string text = string.Empty;
		for (int num = 0; num < array.Length; num++)
		{
			if (num > 0)
			{
				text += ", \"else\": {";
			}
			text += array[num];
		}
		text += ", \"else\": { \"additionalProperties\": false";
		for (int num2 = 0; num2 < array.Length; num2++)
		{
			text += "}";
		}
		return text;
		static string GetIfThen(ISettings settings)
		{
			Variant val = ToJsonSchema(settings);
			if (val == null)
			{
				return null;
			}
			string text2 = "[ { \"const\": \"" + ((object)settings).GetType().FullName + "\" }, { \"const\": \"" + ReflectionUtils.TypeName(((object)settings).GetType(), false) + "\" } ]";
			return "\"if\": { \"properties\": { \"@type\": { \"oneOf\": " + text2 + " } } }, \"then\": " + Extensions.ToJSONString<Variant>(val, (EncodeOptions)9);
		}
	}

	private static Variant GetDefinitions()
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_021f: Unknown result type (might be due to invalid IL or missing references)
		//IL_024c: Expected O, but got Unknown
		ProxyObject val = new ProxyObject();
		IEnumerable<string> source = (from ext in ImporterCache.GetSupportedExtensions()
			select ((object)ImporterCache.GetImporter(ext.Key, (Type)null)).GetType()).Distinct().SelectMany((Type t) => new string[2]
		{
			t.FullName,
			ReflectionUtils.TypeName(t, false)
		});
		((Variant)val)["importers"] = JSON.Load("{ \"type\": \"string\", \"enum\": " + Extensions.ToJSONString<string[]>(source.ToArray(), (EncodeOptions)9) + " }");
		IEnumerable<string> source2 = (from x in (from x in (from x in (from x in (from ext in ImporterCache.GetSupportedExtensions()
							select ((object)((ISettingable)ImporterCache.GetImporter(ext.Key, (Type)null)).GetDefaultSettings())?.GetType()).Distinct().Concat(from x in PostProcessorCache.GetTexturePostProcessors()
							select ((object)((ISettingable)x).GetDefaultSettings())?.GetType())
						where x != null
						select x).Concat(from x in PostProcessorCache.GetModelPostProcessors()
						select ((object)((ISettingable)x).GetDefaultSettings())?.GetType())
					where x != null
					select x).Concat(from x in PostProcessorCache.GetModelSurfacePostProcessors()
					select ((object)((ISettingable)x).GetDefaultSettings())?.GetType())
				where x != null
				select x).Concat(from x in PostProcessorCache.GetGeometryPostProcessors()
				select ((object)((ISettingable)x).GetDefaultSettings())?.GetType())
			where x != null
			select x).SelectMany((Type t) => new string[2]
		{
			t.FullName,
			ReflectionUtils.TypeName(t, false)
		});
		((Variant)val)["settingsType"] = JSON.Load("{ \"type\": \"string\", \"enum\": " + Extensions.ToJSONString<string[]>(source2.ToArray(), (EncodeOptions)9) + " }");
		return (Variant)val;
	}

	private static string ToJsonSchema(FieldInfo fieldInfo)
	{
		string name = fieldInfo.Name;
		Type fieldType = fieldInfo.FieldType;
		bool nullable = name == "materialTemplate";
		string text = "\"type\": " + ToJsonType(fieldType, nullable);
		if (fieldType.IsEnum)
		{
			return text + ", \"enum\": " + Extensions.ToJSONString<string[]>(Enum.GetNames(fieldType), (EncodeOptions)9);
		}
		if (fieldType.IsArray)
		{
			return text + ", \"items\": " + Extensions.ToJSONString<Variant>(ToJsonSchema(fieldType.GetElementType()), (EncodeOptions)9);
		}
		if (typeof(IList).IsAssignableFrom(fieldType))
		{
			return text + ", \"items\": " + Extensions.ToJSONString<Variant>(ToJsonSchema(fieldType.GetGenericArguments()[0]), (EncodeOptions)9);
		}
		return name switch
		{
			"importerTypeHints" => text + ", \"patternProperties\": { \"^\\\\.[A-Za-z0-9]+$\": { \"$ref\": \"#/definitions/importers\" } }, \"additionalProperties\": false", 
			"sharedAssets" => text + ", \"patternProperties\": { \"^[A-Za-z0-9_*{}]+(\\\\.[A-Za-z0-9]+)?(/_[A-Za-z0-9]+)?$\": { \"type\": \"string\", \"format\": \"uri-reference\" } }, \"additionalProperties\": false", 
			"importSettings" => text + ", \"additionalProperties\": { " + GetSettings() + " }", 
			_ => text, 
		};
	}

	private static void AddSimplifiedProperties(Variant schema)
	{
		schema["^LOD(\\d+|\\*)?(_[A-Za-z0-9*]+)?$"] = ToJsonSchema(typeof(LODLevelSettings));
		schema["^Surface(_[A-Za-z0-9*]+)?$"] = ToJsonSchema(typeof(PostProcessSettings));
	}

	public static void GenerateJSONSchema()
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Expected O, but got Unknown
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Expected O, but got Unknown
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Expected O, but got Unknown
		//IL_005a: Expected O, but got Unknown
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_0071: Expected O, but got Unknown
		PostProcessorCache.CachePostProcessors((ImportStep)null);
		ImporterCache.CacheSupportedExtensions((ImportStep)null);
		Variant val = (Variant)new ProxyObject();
		val["$schema"] = (Variant)new ProxyString("http://json-schema.org/draft-07/schema#");
		val["definitions"] = GetDefinitions();
		ToJsonSchema(typeof(Settings), val);
		ProxyObject val2 = new ProxyObject();
		Variant schema = (Variant)val2;
		val["patternProperties"] = (Variant)val2;
		AddSimplifiedProperties(schema);
		val["additionalProperties"] = (Variant)new ProxyBoolean(false);
		string text = (GUIUtility.systemCopyBuffer = val.ToJSON());
		log.Info((object)text);
	}

	private static string AdjustNamingConvention(string input)
	{
		StringBuilder stringBuilder = new StringBuilder();
		bool flag = true;
		foreach (char c in input)
		{
			if (flag)
			{
				stringBuilder.Append(char.ToUpper(c));
				flag = false;
			}
			else
			{
				stringBuilder.Append(c);
			}
			if (c == '_')
			{
				flag = true;
			}
		}
		return stringBuilder.ToString();
	}

	private static bool IsArtRootPath(string rootPathName, string path, out string artProjectPath, out string artProjectRelativePath)
	{
		if (rootPathName == null)
		{
			throw new IOException("rootPath can not be null");
		}
		if (path == null)
		{
			throw new IOException("import path can not be null");
		}
		if (path == rootPathName)
		{
			throw new IOException("rootPath can not be the same as import path");
		}
		int num = path.IndexOf(rootPathName, StringComparison.Ordinal);
		bool flag = num != -1;
		artProjectRelativePath = (flag ? path.Substring(num + rootPathName.Length).Replace('\\', '/').TrimStart('/') : null);
		artProjectPath = (flag ? path.Substring(0, num + rootPathName.Length).Replace('\\', '/').TrimEnd('/') : null);
		return flag;
	}

	public static bool IsArtRootPath(string rootPathName, string[] paths, out string artProjectPath, out List<string> artProjectRelativePaths)
	{
		artProjectPath = null;
		artProjectRelativePaths = new List<string>(paths.Length);
		foreach (string text in paths)
		{
			if (!string.IsNullOrEmpty(text))
			{
				if (!IsArtRootPath(rootPathName, text, out var artProjectPath2, out var artProjectRelativePath))
				{
					return false;
				}
				if (artProjectPath != null && artProjectPath2 != artProjectPath)
				{
					throw new Exception("Root project path does not match. Previous: " + artProjectPath + " Current: " + artProjectPath2);
				}
				artProjectPath = artProjectPath2;
				artProjectRelativePaths.Add(artProjectRelativePath);
			}
		}
		return true;
	}

	public static IEnumerable<ISettingable> GetImportChainFor(Settings settings, Asset asset, ReportBase report)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_008f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e4: Unknown result type (might be due to invalid IL or missing references)
		List<ISettingable> list = new List<ISettingable>();
		IAssetImporter val = default(IAssetImporter);
		if (ImporterCache.GetImporter<IAssetImporter>(asset.path, ref val, settings.importerTypeHints))
		{
			list.Add((ISettingable)(object)val);
		}
		string fileName = default(string);
		try
		{
			string text = default(string);
			int num = default(int);
			int2 val2 = default(int2);
			Module val3 = default(Module);
			int num2 = default(int);
			string text2 = default(string);
			string text3 = default(string);
			AssetUtils.ParseName(Path.GetFileNameWithoutExtension(asset.name), ref text, ref fileName, ref num, ref val2, ref val3, ref num2, ref text2, ref text3);
		}
		catch (FormatException)
		{
			log.WarnFormat("Invalid filename: {0}", (object)Path.GetFileNameWithoutExtension(asset.name));
			fileName = Path.GetFileName(asset.name);
		}
		if (val is TextureImporter)
		{
			ISettings val4 = default(ISettings);
			foreach (ITexturePostProcessor texturePostProcessor in PostProcessorCache.GetTexturePostProcessors())
			{
				if (((Settings)(ref settings)).GetPostProcessSettings(asset.name, (ISettingable)(object)texturePostProcessor, report, ref val4))
				{
					list.Add((ISettingable)(object)texturePostProcessor);
				}
			}
		}
		if (val is ModelImporter)
		{
			ISettings val5 = default(ISettings);
			foreach (IModelPostProcessor modelPostProcessor in PostProcessorCache.GetModelPostProcessors())
			{
				if (((Settings)(ref settings)).GetPostProcessSettings(asset.name, (ISettingable)(object)modelPostProcessor, report, ref val5))
				{
					list.Add((ISettingable)(object)modelPostProcessor);
				}
			}
			ISettings val6 = default(ISettings);
			foreach (IModelSurfacePostProcessor modelSurfacePostProcessor in PostProcessorCache.GetModelSurfacePostProcessors())
			{
				if (((Settings)(ref settings)).GetPostProcessSettings(fileName, (ISettingable)(object)modelSurfacePostProcessor, report, ref val6))
				{
					list.Add((ISettingable)(object)modelSurfacePostProcessor);
				}
			}
			ISettings val7 = default(ISettings);
			foreach (IGeometryPostProcessor geometryPostProcessor in PostProcessorCache.GetGeometryPostProcessors())
			{
				if (((Settings)(ref settings)).GetPostProcessSettings(fileName, (ISettingable)(object)geometryPostProcessor, report, ref val7))
				{
					list.Add((ISettingable)(object)geometryPostProcessor);
				}
			}
		}
		return list;
	}

	public static IDictionary<AssetGroup<Asset>, Settings> CollectDataToImport(string projectRootPath, string[] assetPaths, Report report)
	{
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0074: Unknown result type (might be due to invalid IL or missing references)
		//IL_0076: Unknown result type (might be due to invalid IL or missing references)
		//IL_007f: Unknown result type (might be due to invalid IL or missing references)
		//IL_008b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_009b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e0: Unknown result type (might be due to invalid IL or missing references)
		//IL_0103: Unknown result type (might be due to invalid IL or missing references)
		//IL_0122: Unknown result type (might be due to invalid IL or missing references)
		//IL_0124: Unknown result type (might be due to invalid IL or missing references)
		OrderedDictionary<AssetGroup<Asset>, Settings> val = new OrderedDictionary<AssetGroup<Asset>, Settings>();
		if (IsArtRootPath(projectRootPath, assetPaths, out var artProjectPath, out var artProjectRelativePaths))
		{
			ImportStep val2 = ((ReportBase)report).AddImportStep("Collect asset group");
			try
			{
				SourceAssetCollector val3 = default(SourceAssetCollector);
				((SourceAssetCollector)(ref val3))._002Ector(artProjectPath, (IEnumerable<string>)artProjectRelativePaths);
				Asset val6 = default(Asset);
				foreach (AssetGroup<Asset> item in (SourceAssetCollector)(ref val3))
				{
					Asset val4 = report.AddAsset(item.name);
					Settings val5 = ImportSettings(item, (step: val2, asset: val4));
					foreach (Asset item2 in item)
					{
						if (val5.ignoreSuffixes != null && StringUtils.EndsWithAny(Path.GetFileNameWithoutExtension(item2.name), val5.ignoreSuffixes))
						{
							item.RemoveFile(item2);
						}
					}
					foreach (KeyValuePair<string, string> item3 in ((Settings)(ref val5)).UsedShaderAssets(item, val4))
					{
						string text = ResolveRelativePath(projectRootPath, item3.Value, item.rootPath);
						if (LongFile.Exists(text))
						{
							((Asset)(ref val6))._002Ector(text, projectRootPath);
							item.AddFile(val6);
						}
					}
					val.Add(item, val5);
				}
				return (IDictionary<AssetGroup<Asset>, Settings>)val;
			}
			finally
			{
				((IDisposable)val2)?.Dispose();
			}
		}
		throw new Exception("Invalid " + artProjectPath);
	}

	private static void CreateTitle(Transform parent, string text, Vector3 textPosOffset, Color bgColor, Color txtColor, int txtSize, float txtPadding)
	{
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0076: Unknown result type (might be due to invalid IL or missing references)
		//IL_007b: Unknown result type (might be due to invalid IL or missing references)
		//IL_007e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0087: Unknown result type (might be due to invalid IL or missing references)
		//IL_009a: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f3: Unknown result type (might be due to invalid IL or missing references)
		//IL_0102: Unknown result type (might be due to invalid IL or missing references)
		GameObject val = new GameObject("Title");
		val.transform.SetParent(parent, false);
		val.transform.rotation = Quaternion.Euler(90f, 0f, 0f);
		TextMeshPro obj = val.AddComponent<TextMeshPro>();
		((TMP_Text)obj).font = Resources.Load<TMP_FontAsset>("Fonts & Materials/LiberationSans SDF");
		((TMP_Text)obj).text = text;
		((TMP_Text)obj).fontSize = txtSize;
		((Graphic)obj).color = txtColor;
		((TMP_Text)obj).alignment = (TextAlignmentOptions)514;
		((TMP_Text)obj).enableWordWrapping = false;
		Vector2 preferredValues = ((TMP_Text)obj).GetPreferredValues();
		Vector3 val2 = default(Vector3);
		((Vector3)(ref val2))._002Ector(preferredValues.x + txtPadding, preferredValues.y + txtPadding, 0.1f);
		val.transform.localPosition = textPosOffset + new Vector3((0f - preferredValues.x) / 2f, 0f, 0f);
		CreateBackground(val.transform, "TextBg", Vector3.zero, new Vector3(val2.x / 10f, 1f, val2.y / 10f));
		backgroundMaterial.color = bgColor;
	}

	private static void CreateBounds(Transform parent)
	{
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_0079: Unknown result type (might be due to invalid IL or missing references)
		//IL_0089: Unknown result type (might be due to invalid IL or missing references)
		//IL_00af: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c3: Unknown result type (might be due to invalid IL or missing references)
		Transform obj = parent.Find("Title");
		Transform val = obj.Find("TextBg");
		CreateBackground(obj, "BoundsTop", Vector3.zero, new Vector3(1f, 1f, 0.1f));
		CreateBackground(obj, "BoundsBottom", Vector3.zero, new Vector3(1f, 1f, 0.1f));
		CreateBackground(obj, "BoundsRight", Vector3.zero, new Vector3(0.1f, 1f, 1f));
		CreateBackground(obj, "BoundsLeft", new Vector3((val.localScale.x * 0.5f + 0.05f) * 10f, 0f, 0f), new Vector3(0.1f, 1f, 1f));
	}

	private static void AdjustZBounds(Transform parent, float z)
	{
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		//IL_0069: Unknown result type (might be due to invalid IL or missing references)
		//IL_006e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0084: Unknown result type (might be due to invalid IL or missing references)
		//IL_008c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0091: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a8: Unknown result type (might be due to invalid IL or missing references)
		Transform obj = parent.Find("Title");
		Transform val = obj.Find("BoundsTop");
		Transform val2 = obj.Find("BoundsBottom");
		Transform val3 = obj.Find("BoundsRight");
		Transform obj2 = obj.Find("BoundsLeft");
		Vector3 localPosition = val.localPosition;
		localPosition.y = z;
		val.localPosition = localPosition;
		Vector3 localPosition2 = val2.localPosition;
		localPosition2.y = 0f - z;
		val2.localPosition = localPosition2;
		Vector3 localScale = obj2.localScale;
		localScale.z = z * 2f / 10f;
		obj2.localScale = localScale;
		Vector3 localScale2 = val3.localScale;
		localScale2.z = z * 2f / 10f;
		val3.localScale = localScale2;
	}

	private static void AdjustXBounds(Transform parent, float x)
	{
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		//IL_0077: Unknown result type (might be due to invalid IL or missing references)
		//IL_007c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0088: Unknown result type (might be due to invalid IL or missing references)
		//IL_0090: Unknown result type (might be due to invalid IL or missing references)
		//IL_0095: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00af: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00de: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ef: Unknown result type (might be due to invalid IL or missing references)
		Transform obj = parent.Find("Title");
		Transform val = obj.Find("BoundsTop");
		Transform val2 = obj.Find("BoundsBottom");
		Transform val3 = obj.Find("BoundsRight");
		Vector3 localPosition = ((Component)obj.Find("BoundsLeft")).transform.localPosition;
		Vector3 val4 = localPosition;
		val4.x += x;
		((Component)val3).transform.localPosition = val4;
		float x2 = (localPosition.x + val4.x) * 0.5f;
		Vector3 localPosition2 = val.localPosition;
		localPosition2.x = x2;
		val.localPosition = localPosition2;
		Vector3 localPosition3 = val2.localPosition;
		localPosition3.x = x2;
		val2.localPosition = localPosition3;
		float x3 = (val4.x - localPosition.x) / 10f + 0.1f;
		Vector3 localScale = val.localScale;
		localScale.x = x3;
		val.localScale = localScale;
		Vector3 localScale2 = val2.localScale;
		localScale2.x = x3;
		val2.localScale = localScale2;
	}

	private static void CreateBackground(Transform parent, string name, Vector3 position, Vector3 size)
	{
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		GameObject obj = GameObject.CreatePrimitive((PrimitiveType)4);
		((Object)obj).name = name;
		obj.transform.SetParent(parent, false);
		obj.transform.localRotation = Quaternion.Euler(-90f, 0f, 0f);
		obj.transform.localScale = size;
		obj.transform.localPosition = new Vector3(position.x, position.y, position.z + 0.05f);
		obj.GetComponent<Renderer>().sharedMaterial = backgroundMaterial;
	}

	public static void InstantiateRenderPrefabs<T>(IEnumerable<(T prefab, string sourcePath)> prefabs, bool smartInstantiate, bool ignoreLODs) where T : PrefabBase
	{
		//IL_045a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0461: Expected O, but got Unknown
		//IL_0471: Unknown result type (might be due to invalid IL or missing references)
		//IL_0476: Unknown result type (might be due to invalid IL or missing references)
		//IL_047b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0481: Unknown result type (might be due to invalid IL or missing references)
		//IL_04a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_04b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_04c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_011c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_02de: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f0: Unknown result type (might be due to invalid IL or missing references)
		//IL_0309: Unknown result type (might be due to invalid IL or missing references)
		//IL_030e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0313: Unknown result type (might be due to invalid IL or missing references)
		//IL_032c: Unknown result type (might be due to invalid IL or missing references)
		//IL_035a: Unknown result type (might be due to invalid IL or missing references)
		//IL_037b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0259: Unknown result type (might be due to invalid IL or missing references)
		//IL_01de: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e5: Expected O, but got Unknown
		//IL_01fd: Unknown result type (might be due to invalid IL or missing references)
		//IL_0212: Unknown result type (might be due to invalid IL or missing references)
		//IL_0217: Unknown result type (might be due to invalid IL or missing references)
		//IL_021c: Unknown result type (might be due to invalid IL or missing references)
		if (smartInstantiate)
		{
			List<(RenderPrefab, string)> list = (from tuple in prefabs
				where tuple.prefab is RenderPrefab && (!ignoreLODs || !((Object)tuple.prefab).name.Contains("_LOD"))
				select (tuple.prefab as RenderPrefab, GetParent(tuple.sourcePath)) into x
				orderby ((Object)x.Item1).name, x.Item2
				select x).ToList();
			Dictionary<string, int> dictionary = new Dictionary<string, int>();
			List<float> list2 = new List<float>();
			int num = 0;
			foreach (var item in list)
			{
				Bounds val = RenderingUtils.ToBounds(item.Item1.bounds);
				if (!dictionary.TryGetValue(item.Item2, out var value))
				{
					list2.Add(((Bounds)(ref val)).extents.z * 1.5f);
					dictionary.Add(item.Item2, num);
					num++;
				}
				else
				{
					list2[value] = Mathf.Max(list2[value], ((Bounds)(ref val)).extents.z * 1.5f);
				}
			}
			float num2 = 5f;
			float num3 = 0f;
			Dictionary<string, GameObject> dictionary2 = new Dictionary<string, GameObject>(dictionary.Count);
			List<float> list3 = Enumerable.Repeat(num2, dictionary.Count).ToList();
			{
				foreach (var item2 in list)
				{
					int index = dictionary[item2.Item2];
					if (!dictionary2.TryGetValue(item2.Item2, out var value2))
					{
						string fileName = Path.GetFileName(item2.Item2);
						value2 = GameObject.Find(fileName);
						if ((Object)(object)value2 == (Object)null)
						{
							value2 = new GameObject(fileName);
							CreateTitle(value2.transform, fileName, new Vector3(0f, 0.1f, 0f), Color32.op_Implicit(new Color32((byte)1, (byte)174, (byte)240, byte.MaxValue)), Color.white, 48, 0.1f);
							CreateBounds(value2.transform);
						}
						num3 += list2[index];
						value2.transform.position = new Vector3(0f, 0f, num3);
						num3 += list2[index] + 10f;
						AdjustZBounds(value2.transform, list2[index] + 5f);
						dictionary2.Add(item2.Item2, value2);
					}
					if ((Object)(object)GameObject.Find(((Object)value2).name + "/" + ((Object)item2.Item1).name) == (Object)null)
					{
						GameObject val2 = new GameObject(((Object)item2.Item1).name);
						val2.transform.parent = value2.transform;
						val2.AddComponent<RenderPrefabRenderer>().m_Prefab = item2.Item1;
						Bounds val3 = RenderingUtils.ToBounds(item2.Item1.bounds);
						list3[index] += ((Bounds)(ref val3)).extents.x * 1.5f;
						val2.transform.localPosition = new Vector3(list3[index], 0f, 0f);
						list3[index] += ((Bounds)(ref val3)).extents.x * 1.5f + num2;
						AdjustXBounds(value2.transform, list3[index]);
					}
				}
				return;
			}
		}
		int num4 = 0;
		float num5 = 0f;
		float num6 = 0f;
		float num7 = 0f;
		foreach (var prefab in prefabs)
		{
			if ((ignoreLODs && ((Object)prefab.prefab).name.Contains("_LOD")) || !(prefab.prefab is RenderPrefab renderPrefab))
			{
				continue;
			}
			GameObject val4 = GameObject.Find(((Object)renderPrefab).name);
			if ((Object)(object)val4 == (Object)null)
			{
				val4 = new GameObject(((Object)renderPrefab).name);
				val4.AddComponent<RenderPrefabRenderer>().m_Prefab = renderPrefab;
				Bounds val5 = RenderingUtils.ToBounds(renderPrefab.bounds);
				num6 += ((Bounds)(ref val5)).extents.x * 1.5f;
				val4.transform.position = new Vector3(num6, 0f, num7);
				num6 += ((Bounds)(ref val5)).extents.x * 1.5f;
				num5 = Mathf.Max(num5, ((Bounds)(ref val5)).extents.z * 3f);
				num4++;
				if (num4 % 10 == 0)
				{
					num7 += num5;
					num5 = 0f;
					num6 = 0f;
				}
			}
			else
			{
				RenderPrefabRenderer component = val4.GetComponent<RenderPrefabRenderer>();
				if ((Object)(object)component != (Object)null)
				{
					component.m_Prefab = renderPrefab;
				}
			}
		}
		static string GetParent(string path)
		{
			int num8 = path.LastIndexOf('/');
			if (num8 < 0)
			{
				return path;
			}
			return path.Substring(0, num8);
		}
	}

	public static Dictionary<TextureAsset, List<SurfaceAsset>> GetTextureReferenceCount(IEnumerable<SurfaceAsset> surfaces, out int surfaceCount)
	{
		Dictionary<TextureAsset, List<SurfaceAsset>> dictionary = new Dictionary<TextureAsset, List<SurfaceAsset>>();
		surfaceCount = 0;
		foreach (SurfaceAsset surface in surfaces)
		{
			SurfaceAsset val = surface;
			try
			{
				surface.LoadProperties(false);
				foreach (KeyValuePair<string, TextureAsset> texture in surface.textures)
				{
					if (!dictionary.TryGetValue(texture.Value, out var value))
					{
						value = new List<SurfaceAsset>();
						dictionary.Add(texture.Value, value);
					}
					value.Add(surface);
				}
				surfaceCount++;
			}
			finally
			{
				((IDisposable)val)?.Dispose();
			}
		}
		return dictionary;
	}

	private static void ReportTextureReferenceStats(Dictionary<TextureAsset, List<SurfaceAsset>> textureReferencesMap, ImportStep report)
	{
		int num = 0;
		int num2 = 0;
		int num3 = 0;
		foreach (TextureAsset key in textureReferencesMap.Keys)
		{
			if (textureReferencesMap[key].Count == 1)
			{
				num++;
			}
			else if (textureReferencesMap[key].Count == 2)
			{
				num2++;
			}
			else
			{
				num3++;
			}
		}
		((ReportBase)report).AddMessage($"Singles: {num}", (Severity)4000);
		((ReportBase)report).AddMessage($"Doubles: {num2}", (Severity)4000);
		((ReportBase)report).AddMessage($"Multiple: {num3}", (Severity)4000);
	}

	private static int TestTextureSizesUniformity(SurfaceAsset asset, int tileSize, MaterialDescription description)
	{
		int num = description.m_Stacks.Length;
		int[] array = new int[num];
		int[] array2 = new int[num];
		for (int i = 0; i < num; i++)
		{
			array[i] = -1;
		}
		foreach (KeyValuePair<string, TextureAsset> texture in asset.textures)
		{
			int stackConfigIndex = description.GetStackConfigIndex(texture.Key);
			if (stackConfigIndex != -1)
			{
				TextureAsset value = texture.Value;
				value.LoadData(0);
				if (value.width < tileSize)
				{
					return -1;
				}
				if (value.height < tileSize)
				{
					return -2;
				}
				if (array[stackConfigIndex] == -1)
				{
					array[stackConfigIndex] = value.width;
					array2[stackConfigIndex] = value.height;
				}
				else if (array[stackConfigIndex] != value.width || array2[stackConfigIndex] != value.height)
				{
					return -4;
				}
			}
		}
		return 0;
	}

	public static void ProcessSurfacesForVT(IEnumerable<SurfaceAsset> surfacesToConvert, IEnumerable<SurfaceAsset> surfaces, bool force, ImportStep report)
	{
		int midMipsCount = 3;
		int tileSize = 512;
		int mipBias = 20;
		ConvertSurfacesToVT(surfacesToConvert, surfaces, writeVTSettings: false, tileSize, midMipsCount, mipBias, force, report);
		BuildMidMipsCache(surfaces, tileSize, midMipsCount, AssetDatabase.game);
		HideVTSourceTextures(surfacesToConvert);
		ResaveCache(report);
	}

	public static void ConvertSurfacesToVT(IEnumerable<SurfaceAsset> surfacesToConvert, IEnumerable<SurfaceAsset> allSurfaces, bool force, ImportStep report)
	{
		int midMipsCount = 3;
		int tileSize = 512;
		int mipBias = 20;
		ConvertSurfacesToVT(surfacesToConvert, allSurfaces, writeVTSettings: false, tileSize, midMipsCount, mipBias, force, report);
	}

	public static void ConvertSurfacesToVT(IEnumerable<SurfaceAsset> surfacesToConvert, IEnumerable<SurfaceAsset> allSurfaces, bool writeVTSettings, int tileSize, int midMipsCount, int mipBias, bool force, ImportStep report)
	{
		//IL_02a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a9: Unknown result type (might be due to invalid IL or missing references)
		s_Progress.Set("VT post process - Converting surfaces", "Collecting references...", 0f);
		int surfaceCount;
		Dictionary<TextureAsset, List<SurfaceAsset>> textureReferenceCount = GetTextureReferenceCount(allSurfaces, out surfaceCount);
		ReportTextureReferenceStats(textureReferenceCount, report);
		MaterialLibrary materialLibrary = AssetDatabase.global.resources.materialLibrary;
		VirtualTexturingConfig val = Resources.Load<VirtualTexturingConfig>("VirtualTexturingConfig");
		int num = 0;
		foreach (SurfaceAsset item in surfacesToConvert)
		{
			s_Progress.Set("VT post process - Converting surfaces", "Processing " + ((AssetData)item).name, (float)num++ / (float)surfaceCount);
			try
			{
				bool flag = item.IsVTMaterialFromHeader();
				if (!force && flag)
				{
					continue;
				}
				item.LoadProperties(false);
				MaterialDescription materialDescription = materialLibrary.GetMaterialDescription(item.materialTemplateHash);
				if (materialDescription != null)
				{
					if (materialDescription.m_SupportsVT)
					{
						switch (TestTextureSizesUniformity(item, tileSize, materialDescription))
						{
						case 0:
						{
							if (!materialDescription.m_SupportsVT)
							{
								break;
							}
							int num2 = (materialDescription.hasMipBiasOverride ? materialDescription.m_MipBiasOverride : mipBias);
							if (item.Save(num2, true, true, true, val, textureReferenceCount, (int?)tileSize, (int?)midMipsCount))
							{
								log.InfoFormat("File {0} has been converted to VT", (object)item);
							}
							goto end_IL_0089;
						}
						case -1:
							log.WarnFormat("File {0} cannot use VT because at least one of its textures width is smaller than the tileSize {1}", (object)item, (object)tileSize);
							break;
						case -2:
							log.WarnFormat("File {0} cannot use VT because at least one of its textures height is smaller than the tileSize {1}", (object)item, (object)tileSize);
							break;
						case -3:
							log.WarnFormat("File {0} cannot use VT because at least one texture uses a wrap mode that is not Clamp", (object)item);
							break;
						case -4:
							log.WarnFormat("File {0} cannot use VT because its texture sizes is not uniform", (object)item);
							break;
						case -5:
							log.WarnFormat("File {0} cannot use VT because some of its textures are null", (object)item);
							break;
						}
					}
					else
					{
						log.WarnFormat("File {0} cannot use VT because its template {2} (Shader:{3}) from material hash {1} is not set to support VT", (object)item, (object)item.materialTemplateHash, (object)((Object)materialDescription.m_Material).name, (object)((Object)materialDescription.m_Material.shader).name);
					}
				}
				else
				{
					log.WarnFormat("File {0} cannot use VT because its material hash {1} is not mapped or not found", (object)item, (object)item.materialTemplateHash);
				}
				if (flag)
				{
					item.Save(0, true, false, false, (VirtualTexturingConfig)null, (Dictionary<TextureAsset, List<SurfaceAsset>>)null, (int?)null, (int?)null);
					log.InfoFormat("File {0} has been unconverted from VT", (object)item);
				}
				end_IL_0089:;
			}
			catch (Exception ex)
			{
				log.ErrorFormat(ex, "Error occured with {0}", (object)item);
				throw;
			}
			finally
			{
				((AssetData)item).Unload(false);
			}
		}
		if (writeVTSettings)
		{
			VTSettingsAsset val2 = AssetDatabase.game.AddAsset<VTSettingsAsset>(AssetDataPath.Create(EnvPath.kVTSubPath, "VT", (EscapeStrategy)2), default(Hash128));
			try
			{
				val2.Save(mipBias, tileSize, midMipsCount);
			}
			finally
			{
				((IDisposable)val2)?.Dispose();
			}
		}
	}

	private static void ResaveCache(ImportStep report)
	{
		s_Progress.Set("VT post process", "Resaving asset cache", 100f);
		((ReportBase)report).AddMessage(AssetDatabase.global.ResaveCache().Result, (Severity)4000);
	}

	public static void HideVTSourceTextures(IEnumerable<SurfaceAsset> surfaces)
	{
		//IL_010a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0110: Unknown result type (might be due to invalid IL or missing references)
		s_Progress.Set("VT post process - Hiding converted textures", "", 0f);
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
		List<TextureAsset> list = AssetDatabase.global.GetAssets<TextureAsset>(default(SearchFilter<TextureAsset>)).ToList();
		for (int i = 0; i < list.Count; i++)
		{
			TextureAsset val = list[i];
			s_Progress.Set("VT post process - Hiding", "Processing " + ((AssetData)val).name, (float)i / (float)dictionary.Count);
			if (dictionary.ContainsKey(val))
			{
				if (dictionary2.ContainsKey(val))
				{
					log.WarnFormat("Texture {0} is referenced {1} times by VT materials and {2} times by non VT materials. It will be duplicated on disk.", (object)val, (object)dictionary[val].Count, (object)dictionary2[val].Count);
					log.InfoFormat("Detail for {0}:\nvt: {1}\nnon vt: {2}", (object)val, (object)string.Join(", ", dictionary[val]), (object)string.Join(", ", dictionary2[val]));
				}
				else
				{
					log.InfoFormat($"Hiding {val}", Array.Empty<object>());
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

	public static void BuildMidMipsCache(IEnumerable<SurfaceAsset> surfaces, int tileSize, int midMipsCount, ILocalAssetDatabase database)
	{
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Expected O, but got Unknown
		//IL_01dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_010a: Unknown result type (might be due to invalid IL or missing references)
		//IL_010f: Unknown result type (might be due to invalid IL or missing references)
		//IL_021d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0222: Unknown result type (might be due to invalid IL or missing references)
		//IL_0155: Unknown result type (might be due to invalid IL or missing references)
		//IL_015a: Unknown result type (might be due to invalid IL or missing references)
		//IL_015c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0169: Unknown result type (might be due to invalid IL or missing references)
		s_Progress.Set("VT post process - Rebuilding mip cache", "", 0f);
		if (midMipsCount < 0)
		{
			throw new Exception("Nb mid mip levels can't be negative");
		}
		VirtualTexturingConfig val = Resources.Load<VirtualTexturingConfig>("VirtualTexturingConfig");
		int num = val.stackDatas.Length;
		MaterialLibrary materialLibrary = AssetDatabase.global.resources.materialLibrary;
		AtlasMaterialsGrouper val2 = new AtlasMaterialsGrouper(num, tileSize, midMipsCount);
		List<SurfaceAsset> list = surfaces.ToList();
		Dictionary<Hash128, NativeArray<byte>> dictionary = new Dictionary<Hash128, NativeArray<byte>>();
		Dictionary<Hash128, Hash128[]>[] array = new Dictionary<Hash128, Hash128[]>[2];
		for (int i = 0; i < array.Length; i++)
		{
			array[i] = new Dictionary<Hash128, Hash128[]>();
		}
		for (int j = 0; j < list.Count; j++)
		{
			SurfaceAsset val3 = list[j];
			try
			{
				s_Progress.Set("VT post process - Rebuilding mip cache", "Processing " + ((AssetData)val3).name, (float)j / (float)list.Count);
				val3.LoadProperties(true);
				if (!val3.isVTMaterial)
				{
					continue;
				}
				MaterialDescription materialDescription = materialLibrary.GetMaterialDescription(val3.materialTemplateHash);
				long[] array2 = new long[materialDescription.m_Stacks.Length];
				for (int k = 0; k < materialDescription.m_Stacks.Length; k++)
				{
					Hash128[] value = (Hash128[])(object)new Hash128[4];
					array[k][Identifier.op_Implicit(((AssetData)val3).id)] = value;
					val3.AddMidMipTexturesDataToDictionnary(k, midMipsCount, tileSize, materialDescription, dictionary);
				}
				int num2 = val3.ComputeVTLayersMask(materialDescription, array, array2);
				for (int l = 0; l < val3.stackCount; l++)
				{
					AtlassedSize unbiasedStackTextureSize = val3.GetUnbiasedStackTextureSize(l);
					if (unbiasedStackTextureSize.x >= 0)
					{
						val2.Add(l, unbiasedStackTextureSize, num2, val3, array2, materialDescription.m_MipBiasOverride);
					}
				}
			}
			finally
			{
				((IDisposable)val3)?.Dispose();
			}
		}
		val2.ResolveDuplicates(array, 2);
		val2.GroupEntries(val, dictionary, array);
		string assetName = AtlasMaterialsGrouper.GetAssetName(tileSize, midMipsCount);
		using (BinaryWriter binaryWriter = new BinaryWriter(((AssetData)database.AddAsset<MidMipCacheAsset>(AssetDataPath.Create("StreamingData~", assetName, (EscapeStrategy)2), default(Hash128))).GetWriteStream()))
		{
			val2.Write(binaryWriter);
		}
		foreach (NativeArray<byte> value2 in dictionary.Values)
		{
			value2.Dispose();
		}
		val2.Dispose();
	}

	public static async Task ApplyVTMipBias(IAssetDatabase database, int mipBias, int tileSize, int midMipCount, string folder)
	{
		if (s_MainThreadDispatcher == null)
		{
			s_MainThreadDispatcher = new MainThreadDispatcher();
		}
		if (mipBias < 0)
		{
			throw new Exception("Mip bias cannot be smaller than zero in that context!");
		}
		bool flag = true;
		string text = null;
		IAssetDatabase obj = database;
		ILocalAssetDatabase val = (ILocalAssetDatabase)(object)((obj is ILocalAssetDatabase) ? obj : null);
		if (val != null)
		{
			IDataSourceProvider dataSource = ((IDataSourceAccessor)val).dataSource;
			FileSystemDataSource val2 = (FileSystemDataSource)(object)((dataSource is FileSystemDataSource) ? dataSource : null);
			if (val2 != null)
			{
				text = val2.rootPath + "/StreamingData~";
				if ((object)database != AssetDatabase.game)
				{
					flag = false;
				}
			}
		}
		if (text == null)
		{
			throw new ArgumentException("Master VT file path is null.");
		}
		ILocalAssetDatabase vtMipXDatabase = AssetDatabase.GetTransient(0L, text + "/." + folder);
		VirtualTexturingConfig virtualTexturingConfig = Resources.Load<VirtualTexturingConfig>("VirtualTexturingConfig");
		ParallelOptions opts = new ParallelOptions
		{
			MaxDegreeOfParallelism = ((!useParallelImport) ? 1 : Environment.ProcessorCount)
		};
		int total = 0;
		Task importTask = Task.Run(delegate
		{
			//IL_0016: Unknown result type (might be due to invalid IL or missing references)
			//IL_001c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0034: Unknown result type (might be due to invalid IL or missing references)
			//IL_003a: Unknown result type (might be due to invalid IL or missing references)
			List<VTTextureAsset> texture2DPreProcessedAssets = database.GetAssets<VTTextureAsset>(default(SearchFilter<VTTextureAsset>)).ToList();
			List<SurfaceAsset> list = database.GetAssets<SurfaceAsset>(default(SearchFilter<SurfaceAsset>)).ToList();
			int assetsToProcess = texture2DPreProcessedAssets.Count + list.Count;
			for (int i = 0; i < list.Count; i++)
			{
				try
				{
					SurfaceAsset val4 = list[i];
					try
					{
						s_Progress.Set("VT post process - Apply Surface MipBias", "Applying Mip Bias to " + ((AssetData)val4).name, (float)total / (float)assetsToProcess);
						log.InfoFormat("Processing {0} ({1}/{2})", (object)val4, (object)(total + 1), (object)assetsToProcess);
						if (!s_Progress.shouldCancel)
						{
							val4.LoadProperties(false);
							if (val4.isVTMaterial && val4.hasVTSurfaceAsset)
							{
								val4.UpdateMipBias(vtMipXDatabase, mipBias, virtualTexturingConfig, tileSize, midMipCount);
							}
							goto IL_0127;
						}
					}
					finally
					{
						((IDisposable)val4)?.Dispose();
					}
					goto end_IL_0067;
					IL_0127:
					Interlocked.Increment(ref total);
					continue;
					end_IL_0067:;
				}
				catch (Exception ex)
				{
					log.ErrorFormat(ex, "Error with {0}", (object)list[i]);
					continue;
				}
				break;
			}
			Parallel.ForEach(texture2DPreProcessedAssets, opts, delegate(VTTextureAsset texture2DPreProcessedAsset, ParallelLoopState state, long index)
			{
				//IL_00fe: Unknown result type (might be due to invalid IL or missing references)
				//IL_0103: Unknown result type (might be due to invalid IL or missing references)
				try
				{
					s_Progress.Set("VT post process - Apply Texture MipBias", "Applying Mip Bias to " + ((AssetData)texture2DPreProcessedAsset).name, (float)total / (float)assetsToProcess);
					log.InfoFormat("Processing {0} ({1}/{2})", (object)texture2DPreProcessedAsset, (object)(total + 1), (object)assetsToProcess);
					if (s_Progress.shouldCancel)
					{
						state.Stop();
					}
					texture2DPreProcessedAsset.LoadHeader();
					TextureAsset textureAsset = texture2DPreProcessedAsset.textureAsset;
					try
					{
						textureAsset.LoadData(0);
						if (textureAsset.width < tileSize || textureAsset.height < tileSize)
						{
							log.ErrorFormat("That texture [{0}] dimension is too small to be supported by the VT system textureSize: {1}x{2} VT tileSize: {3}", (object)((AssetData)textureAsset).name, (object)textureAsset.width, (object)textureAsset.height, (object)tileSize);
						}
						VTTextureAsset val5 = vtMipXDatabase.AddAsset<VTTextureAsset>(AssetDataPath.op_Implicit(((AssetData)texture2DPreProcessedAsset).name), Identifier.op_Implicit(((AssetData)texture2DPreProcessedAsset).id));
						try
						{
							val5.Save(mipBias, textureAsset, tileSize, midMipCount, virtualTexturingConfig);
						}
						finally
						{
							((IDisposable)val5)?.Dispose();
						}
					}
					finally
					{
						((IDisposable)textureAsset)?.Dispose();
					}
					Interlocked.Increment(ref total);
				}
				catch (Exception ex2)
				{
					log.ErrorFormat(ex2, "Error with {0}", (object)texture2DPreProcessedAssets);
				}
			});
		});
		if (flag)
		{
			VTSettingsAsset val3 = vtMipXDatabase.AddAsset<VTSettingsAsset>(AssetDataPath.op_Implicit("VT"), default(Hash128));
			try
			{
				val3.Save(mipBias, tileSize, midMipCount);
			}
			finally
			{
				((IDisposable)val3)?.Dispose();
			}
		}
		Report report = new Report();
		await ExecuteMainThreadQueue(importTask, report);
	}
}

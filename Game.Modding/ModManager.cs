using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using System.Text;
using Colossal;
using Colossal.IO.AssetDatabase;
using Colossal.Logging;
using Colossal.Logging.Diagnostics;
using Colossal.PSI.Common;
using Colossal.Reflection;
using Colossal.UI;
using Game.PSI;
using Game.SceneFlow;
using Game.Serialization;
using Game.UI;
using Game.UI.Localization;
using Game.UI.Menu;
using Unity.Entities;

namespace Game.Modding;

public class ModManager : IEnumerable<ModManager.ModInfo>, IEnumerable, IDisposable
{
	public class ModInfo
	{
		public enum State
		{
			Unknown,
			Loaded,
			Disposed,
			IsNotModWarning,
			IsNotUniqueWarning,
			GeneralError,
			MissedDependenciesError,
			LoadAssemblyError,
			LoadAssemblyReferenceError
		}

		private readonly List<IMod> m_Instances = new List<IMod>();

		public IReadOnlyList<IMod> instances => m_Instances;

		public ExecutableAsset asset { get; private set; }

		public bool isValid
		{
			get
			{
				if (asset.isMod && asset.isEnabled)
				{
					return asset.isUnique;
				}
				return false;
			}
		}

		public bool isLoaded => asset.isLoaded;

		public bool isBursted => asset.isBursted;

		public string name => asset.fullName;

		public string assemblyFullName => asset.assembly.FullName;

		public State state { get; private set; }

		public string loadError { get; private set; }

		public ModInfo(ExecutableAsset asset)
		{
			this.asset = asset;
		}

		public void Preload(Assembly[] assemblies)
		{
		}

		public void Load(UpdateSystem updateSystem)
		{
			//IL_0180: Expected O, but got Unknown
			//IL_0198: Expected O, but got Unknown
			try
			{
				if (state != State.Unknown || !asset.isEnabled || !asset.isRequired)
				{
					return;
				}
				if (!asset.isMod && !asset.isReference)
				{
					state = State.IsNotModWarning;
					return;
				}
				if (asset.isMod && !asset.isUnique)
				{
					state = State.IsNotUniqueWarning;
					return;
				}
				if (asset.isMod && !asset.canBeLoaded)
				{
					state = State.MissedDependenciesError;
					loadError = string.Join("\n", from r in asset.references
						where (AssetData)(object)r.Value == (IAssetData)null
						select r.Key);
					return;
				}
				ExecutableAsset val = default(ExecutableAsset);
				asset.LoadAssembly((Action<Assembly>)AfterLoadAssembly, ref val);
				asset = val;
				foreach (Type item in ReflectionUtils.GetTypesDerivedFrom<IMod>(asset.assembly))
				{
					m_Instances.Add((IMod)FormatterServices.GetUninitializedObject(item));
				}
				OnLoad(updateSystem);
				state = State.Loaded;
			}
			catch (LoadExecutableException ex)
			{
				LoadExecutableException ex2 = ex;
				state = State.LoadAssemblyError;
				loadError = StackTraceHelper.ExtractStackTraceFromException((Exception)(object)ex2, (StringBuilder)null);
				throw;
			}
			catch (LoadExecutableReferenceException ex3)
			{
				LoadExecutableReferenceException ex4 = ex3;
				state = State.LoadAssemblyReferenceError;
				loadError = StackTraceHelper.ExtractStackTraceFromException((Exception)(object)ex4, (StringBuilder)null);
				throw;
			}
			catch (Exception ex5)
			{
				state = State.GeneralError;
				loadError = StackTraceHelper.ExtractStackTraceFromException(ex5, (StringBuilder)null);
				throw;
			}
		}

		private static void AfterLoadAssembly(Assembly assembly)
		{
			TypeManager.InitializeAdditionalTypes(assembly);
			World.DefaultGameObjectInjectionWorld.GetOrCreateSystemManaged<SerializerSystem>().SetDirty();
		}

		private void OnLoad(UpdateSystem updateSystem)
		{
			foreach (IMod instance in m_Instances)
			{
				instance.OnLoad(updateSystem);
			}
		}

		private void OnDispose()
		{
			foreach (IMod instance in m_Instances)
			{
				instance.OnDispose();
			}
			m_Instances.Clear();
		}

		public void Dispose()
		{
			OnDispose();
			state = State.Disposed;
		}
	}

	private const string kBurstSuffix = "_win_x86_64";

	private static ILog log = LogManager.GetLogger("Modding").SetShowsErrorsInUI(false);

	private readonly List<ModInfo> m_ModsInfos = new List<ModInfo>();

	private bool m_Disabled;

	private bool m_Initialized;

	private bool m_IsInProgress;

	public bool isInitialized => m_Initialized;

	public bool restartRequired { get; private set; }

	public IEnumerator<ModInfo> GetEnumerator()
	{
		return m_ModsInfos.GetEnumerator();
	}

	IEnumerator IEnumerable.GetEnumerator()
	{
		return GetEnumerator();
	}

	private ModManager()
	{
	}

	public static bool AreModsEnabled()
	{
		GameManager instance = GameManager.instance;
		if (instance == null)
		{
			return false;
		}
		return instance.modManager?.ListModsEnabled().Length > 0;
	}

	public static string[] GetModsEnabled()
	{
		return GameManager.instance?.modManager?.ListModsEnabled();
	}

	public string[] ListModsEnabled()
	{
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		return (from x in m_ModsInfos
			where x.isLoaded
			select x.name).Concat(from x in AssetDatabase.global.GetAssets<UIModuleAsset>(default(SearchFilter<UIModuleAsset>))
			select ((AssetData)x).name).ToArray();
	}

	public ModManager(bool disabled)
	{
		m_Disabled = disabled;
		if (!disabled)
		{
			ProgressState? progressState = (ProgressState)2;
			NotificationSystem.Push("ModLoadingStatus", null, null, "ModsLoading", "ModsLoadingWaiting", null, progressState);
		}
	}

	public void Initialize(UpdateSystem updateSystem)
	{
		//IL_00e0: Unknown result type (might be due to invalid IL or missing references)
		//IL_0102: Unknown result type (might be due to invalid IL or missing references)
		//IL_0152: Unknown result type (might be due to invalid IL or missing references)
		//IL_0157: Unknown result type (might be due to invalid IL or missing references)
		//IL_015c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0161: Unknown result type (might be due to invalid IL or missing references)
		//IL_0166: Unknown result type (might be due to invalid IL or missing references)
		//IL_016b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0174: Unknown result type (might be due to invalid IL or missing references)
		//IL_0176: Unknown result type (might be due to invalid IL or missing references)
		//IL_018e: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_0170: Unknown result type (might be due to invalid IL or missing references)
		if (m_Disabled || m_Initialized || m_IsInProgress)
		{
			return;
		}
		try
		{
			m_IsInProgress = true;
			LocalizedString? text = "Initializing mods";
			ProgressState? progressState = (ProgressState)2;
			NotificationSystem.Push("ModLoadingStatus", null, text, "ModsLoading", null, null, progressState);
			RegisterMods();
			InitializeMods(updateSystem);
			m_Initialized = true;
			int num = 0;
			foreach (ModInfo modsInfo in m_ModsInfos)
			{
				ModInfo modInfo = modsInfo;
				if (modInfo.state < ModInfo.State.IsNotModWarning)
				{
					continue;
				}
				num++;
				string id = ((object)modInfo.asset).GetHashCode().ToString();
				string text2 = (string.IsNullOrEmpty(modInfo.asset.mod.thumbnailPath) ? null : $"{modInfo.asset.mod.thumbnailPath}?width={NotificationUISystem.width})");
				ProgressState progressState2 = (ProgressState)(modInfo.state switch
				{
					ModInfo.State.IsNotModWarning => 5, 
					ModInfo.State.IsNotUniqueWarning => 5, 
					ModInfo.State.GeneralError => 4, 
					ModInfo.State.MissedDependenciesError => 4, 
					ModInfo.State.LoadAssemblyError => 4, 
					ModInfo.State.LoadAssemblyReferenceError => 4, 
					_ => 4, 
				});
				string identifier = id;
				LocalizedString? title = modInfo.asset.mod.displayName;
				string thumbnail = text2;
				progressState = progressState2;
				NotificationSystem.Push(identifier, title, null, null, "ModsLoadingFailed", thumbnail, progressState, null, delegate
				{
					//IL_0006: Unknown result type (might be due to invalid IL or missing references)
					//IL_000c: Invalid comparison between Unknown and I4
					//IL_0058: Unknown result type (might be due to invalid IL or missing references)
					string text3 = "Common.DIALOG_TITLE_MODDING[" + (((int)progressState2 == 5) ? "ModLoadingWarning" : "ModLoadingError") + "]";
					LocalizedString message = new LocalizedString($"Common.DIALOG_MESSAGE_MODDING[{modInfo.state}]", null, new Dictionary<string, ILocElement> { 
					{
						"MODNAME",
						LocalizedString.Value(modInfo.asset.mod.displayName)
					} });
					LocalizedString[] otherActions = (modInfo.asset.isLocal ? Array.Empty<LocalizedString>() : new LocalizedString[2]
					{
						LocalizedString.Id("Common.DIALOG_MESSAGE_MODDING[ModPage]"),
						LocalizedString.Id("Common.DIALOG_MESSAGE_MODDING[Disable]")
					});
					if (modInfo.loadError != null)
					{
						MessageDialog dialog = new MessageDialog(text3, message, LocalizedString.Value(modInfo.loadError.Replace("\\", "\\\\").Replace("*", "\\*")), copyButton: true, LocalizedString.Id("Common.OK"), otherActions);
						GameManager.instance.userInterface.appBindings.ShowMessageDialog(dialog, Callback);
					}
					else
					{
						MessageDialog dialog2 = new MessageDialog(text3, message, LocalizedString.Id("Common.OK"), otherActions);
						GameManager.instance.userInterface.appBindings.ShowMessageDialog(dialog2, Callback);
					}
				});
				void Callback(int msg)
				{
					//IL_0093: Unknown result type (might be due to invalid IL or missing references)
					//IL_00e6: Unknown result type (might be due to invalid IL or missing references)
					switch (msg)
					{
					case 0:
						NotificationSystem.Pop(id);
						break;
					case 2:
						NotificationSystem.Pop(id);
						modInfo.asset.mod.onClick();
						break;
					case 3:
						NotificationSystem.Pop(id);
						modInfo.asset.mod.onEnable(obj: false);
						break;
					case 1:
						break;
					}
				}
			}
			LocalizedString value = ((m_ModsInfos.Count == 0) ? LocalizedString.Id(NotificationUISystem.GetText("ModsLoadingDoneZero")) : new LocalizedString(NotificationUISystem.GetText("ModsLoadingDone"), null, new Dictionary<string, ILocElement>
			{
				{
					"LOADED",
					new LocalizedNumber<int>(m_ModsInfos.Count - num, "integer")
				},
				{
					"TOTAL",
					new LocalizedNumber<int>(m_ModsInfos.Count, "integer")
				}
			}));
			text = value;
			progressState = (ProgressState)3;
			NotificationSystem.Pop("ModLoadingStatus", 5f, null, text, "ModsLoading", null, null, progressState);
		}
		catch (Exception ex)
		{
			log.Error(ex);
			LocalizedString? text = LocalizedString.Id(NotificationUISystem.GetText("ModsLoadingAllFailed"));
			ProgressState? progressState = (ProgressState)4;
			NotificationSystem.Pop("ModLoadingStatus", 5f, null, text, "ModsLoading", null, null, progressState);
		}
		finally
		{
			m_IsInProgress = false;
		}
	}

	private void RegisterMods()
	{
		PerformanceCounter val = PerformanceCounter.Start((Action<TimeSpan>)delegate(TimeSpan t)
		{
			log.InfoFormat("Mods registered in {0}ms", (object)t.TotalMilliseconds);
		});
		try
		{
			m_ModsInfos.Clear();
			ExecutableAsset[] modAssets = ExecutableAsset.GetModAssets(typeof(IMod));
			foreach (ExecutableAsset val2 in modAssets)
			{
				try
				{
					m_ModsInfos.Add(new ModInfo(val2));
				}
				catch (Exception ex)
				{
					log.ErrorFormat(ex, "Error registering mod {0}", (object)val2.fullName);
				}
			}
		}
		finally
		{
			((IDisposable)val)?.Dispose();
		}
	}

	private void InitializeMods(UpdateSystem updateSystem)
	{
		PerformanceCounter val = PerformanceCounter.Start((Action<TimeSpan>)delegate(TimeSpan t)
		{
			log.InfoFormat($"Mods initialized in {0}ms", (object)t.TotalMilliseconds);
		});
		try
		{
			foreach (ModInfo modInfo in m_ModsInfos)
			{
				try
				{
					PerformanceCounter val2 = PerformanceCounter.Start((Action<TimeSpan>)delegate(TimeSpan t)
					{
						log.InfoFormat($"Loaded {{1}} in {0}ms", (object)t.TotalMilliseconds, (object)modInfo.name);
					});
					try
					{
						modInfo.Load(updateSystem);
					}
					finally
					{
						((IDisposable)val2)?.Dispose();
					}
				}
				catch (Exception ex)
				{
					modInfo.Dispose();
					log.ErrorFormat(ex, "Error initializing mod {0} ({1})", (object)modInfo.name, (object)modInfo.assemblyFullName);
				}
			}
		}
		finally
		{
			((IDisposable)val)?.Dispose();
		}
		InitializeUIModules();
	}

	private void InitializeUIModules()
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		UIModuleAsset[] array = AssetDatabase.global.GetAssets<UIModuleAsset>(default(SearchFilter<UIModuleAsset>)).ToArray();
		List<string> list = new List<string>();
		UIModuleAsset[] array2 = array;
		foreach (UIModuleAsset val in array2)
		{
			if (val.isEnabled)
			{
				UIManager.defaultUISystem.AddHostLocation("ui-mods", Path.GetDirectoryName(((AssetData)val).path), val.isLocal, 0);
				log.InfoFormat("Registered UI Module {0} from {1}", (object)val.moduleInfo, (object)val);
				list.Add(val.couiPath);
			}
		}
		GameManager.instance.userInterface.appBindings.AddActiveUIModLocation(list);
	}

	public void AddUIModule(UIModuleAsset uiModule)
	{
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		if (m_Initialized)
		{
			UIManager.defaultUISystem.AddHostLocation("ui-mods", Path.GetDirectoryName(((AssetData)uiModule).path), uiModule.isLocal, 0);
			GameManager.instance.userInterface.appBindings.AddActiveUIModLocation(new string[1] { uiModule.couiPath });
			log.InfoFormat("Registered UI Module {0} from {1}", (object)uiModule.moduleInfo, (object)uiModule);
		}
	}

	public void RemoveUIModule(UIModuleAsset uiModule)
	{
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		if (m_Initialized)
		{
			UIManager.defaultUISystem.RemoveHostLocation("ui-mods", Path.GetDirectoryName(((AssetData)uiModule).path));
			GameManager.instance.userInterface.appBindings.RemoveActiveUIModLocation(new string[1] { uiModule.couiPath });
			log.InfoFormat("Unregistered UI Module {0}", (object)uiModule.moduleInfo);
		}
	}

	public void Dispose()
	{
		PerformanceCounter val = PerformanceCounter.Start((Action<TimeSpan>)delegate(TimeSpan t)
		{
			log.InfoFormat($"Mods disposed in {0}ms", (object)t.TotalMilliseconds);
		});
		try
		{
			foreach (ModInfo modInfo in m_ModsInfos)
			{
				try
				{
					PerformanceCounter val2 = PerformanceCounter.Start((Action<TimeSpan>)delegate(TimeSpan t)
					{
						log.InfoFormat($"Disposed {{1}} in {0}ms", (object)t.TotalMilliseconds, (object)modInfo.name);
					});
					try
					{
						modInfo.Dispose();
					}
					finally
					{
						((IDisposable)val2)?.Dispose();
					}
				}
				catch (Exception ex)
				{
					log.ErrorFormat(ex, "Error disposing mod {0} ({1})", (object)modInfo.name, (object)modInfo.assemblyFullName);
				}
			}
			m_ModsInfos.Clear();
		}
		finally
		{
			((IDisposable)val)?.Dispose();
		}
	}

	public void RequireRestart()
	{
		if (!m_Initialized || restartRequired)
		{
			return;
		}
		restartRequired = true;
		log.Info((object)"Restart required");
		ProgressState? progressState = (ProgressState)5;
		NotificationSystem.Push("RestartRequired", null, null, "EnabledModsChanged", "EnabledModsChanged", null, progressState, null, delegate
		{
			ConfirmationDialog dialog = new ConfirmationDialog("Common.DIALOG_TITLE[Warning]", DialogMessage.GetId("EnabledModsChanged"), "Common.DIALOG_ACTION[Yes]", "Common.DIALOG_ACTION[No]");
			GameManager.instance.userInterface.appBindings.ShowConfirmationDialog(dialog, delegate(int msg)
			{
				if (msg == 0)
				{
					restartRequired = false;
					GameManager.QuitGame();
				}
			});
		});
	}

	public bool TryGetExecutableAsset(IMod mod, out ExecutableAsset asset)
	{
		foreach (ModInfo modsInfo in m_ModsInfos)
		{
			foreach (IMod instance in modsInfo.instances)
			{
				if (instance == mod)
				{
					asset = modsInfo.asset;
					return true;
				}
			}
		}
		asset = null;
		return false;
	}

	public bool TryGetExecutableAsset(Assembly assembly, out ExecutableAsset asset)
	{
		foreach (ModInfo modsInfo in m_ModsInfos)
		{
			if (modsInfo.asset.assembly == assembly)
			{
				asset = modsInfo.asset;
				return true;
			}
		}
		asset = null;
		return false;
	}
}

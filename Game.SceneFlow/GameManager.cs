using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ATL;
using cohtml.Net;
using Colossal;
using Colossal.AssetPipeline.Importers;
using Colossal.Core;
using Colossal.FileSystem;
using Colossal.IO.AssetDatabase;
using Colossal.IO.AssetDatabase.VirtualTexturing;
using Colossal.Json;
using Colossal.Localization;
using Colossal.Logging;
using Colossal.Logging.Backtrace;
using Colossal.PSI.Common;
using Colossal.PSI.Environment;
using Colossal.PSI.PdxSdk;
using Colossal.Reflection;
using Colossal.Serialization.Entities;
using Colossal.TestFramework;
using Colossal.UI;
using Colossal.UI.Fatal;
using Game.Assets;
using Game.Audio;
using Game.Common;
using Game.Debug;
using Game.Input;
using Game.Modding;
using Game.Prefabs;
using Game.PSI;
using Game.PSI.PdxSdk;
using Game.Rendering;
using Game.Serialization;
using Game.Settings;
using Game.Threading;
using Game.UI;
using Game.UI.Localization;
using Game.UI.Menu;
using Game.UI.Thumbnails;
using Mono.Options;
using PDX.SDK;
using PDX.SDK.Contracts.Service.Mods.Enums;
using Unity.Collections;
using Unity.Entities;
using UnityEngine;
using UnityEngine.Profiling;
using UnityEngine.Rendering.HighDefinition;

namespace Game.SceneFlow;

public class GameManager : MonoBehaviour, ICoroutineHost
{
	public class Configuration
	{
		public enum StdoutCaptureMode
		{
			None,
			Console,
			CaptureOnly,
			Redirect
		}

		public Hash128 startGame;

		public bool disablePDXSDK;

		public bool noThumbnails;

		public string profilerTarget;

		public bool saveAllSettings;

		public bool cleanupSettings = true;

		public bool developerMode;

		public bool uiDeveloperMode;

		public bool qaDeveloperMode;

		public bool duplicateLogToDefault;

		public bool disableUserSection;

		public bool disableModding;

		public bool disableCodeModding;

		public StdoutCaptureMode captureStdout;

		public string showHelp;

		public bool continuelastsave;

		public override string ToString()
		{
			return Extensions.ToJSONString<Configuration>(this, (EncodeOptions)9);
		}

		public void LogConfiguration()
		{
			log.InfoFormat("Configuration: {0}", (object)this);
		}
	}

	public enum State
	{
		Booting,
		Terminated,
		UIReady,
		WorldDisposed,
		WorldReady,
		Quitting,
		Loading
	}

	public delegate void EventCallback();

	public delegate void EventGamePreload(Purpose purpose, GameMode mode);

	public delegate void EventGameSaveLoad(string saveName, bool start);

	public class LocalTypeCache
	{
		private readonly Dictionary<(Type type, string name, BindingFlags bindingFlags), MethodInfo> m_MethodCache = new Dictionary<(Type, string, BindingFlags), MethodInfo>();

		private readonly Dictionary<(Type type, string name), PropertyInfo> m_PropertyCache = new Dictionary<(Type, string), PropertyInfo>();

		private readonly Dictionary<(Type type, string name), FieldInfo> m_FieldCache = new Dictionary<(Type, string), FieldInfo>();

		public MethodInfo GetMethod(Type type, string methodName, BindingFlags bindingFlags)
		{
			(Type, string, BindingFlags) key = (type, methodName, bindingFlags);
			if (!m_MethodCache.TryGetValue(key, out var value))
			{
				value = type.GetMethod(methodName, bindingFlags);
				m_MethodCache[key] = value;
			}
			return value;
		}

		public PropertyInfo GetProperty(Type type, string propertyName)
		{
			(Type, string) key = (type, propertyName);
			if (!m_PropertyCache.TryGetValue(key, out var value))
			{
				value = type.GetProperty(propertyName);
				m_PropertyCache[key] = value;
			}
			return value;
		}

		public FieldInfo GetField(Type type, string propertyName)
		{
			(Type, string) key = (type, propertyName);
			if (!m_FieldCache.TryGetValue(key, out var value))
			{
				value = type.GetField(propertyName);
				m_FieldCache[key] = value;
			}
			return value;
		}
	}

	[Serializable]
	[CompilerGenerated]
	private sealed class _003C_003Ec
	{
		public static readonly _003C_003Ec _003C_003E9 = new _003C_003Ec();

		public static Action<string> _003C_003E9__12_2;

		public static Func<SaveGameMetadata> _003C_003E9__50_0;

		public static Func<MapMetadata> _003C_003E9__50_1;

		public static Func<CinematicCameraAsset> _003C_003E9__50_2;

		public static Action<TimeSpan> _003C_003E9__51_0;

		public static Action<TimeSpan> _003C_003E9__54_0;

		public static Action<TimeSpan> _003C_003E9__64_0;

		public static Func<Task> _003C_003E9__64_1;

		public static AchievementUpdatedEventHandler _003C_003E9__106_0;

		public static Func<string> _003C_003E9__107_0;

		public static Func<string> _003C_003E9__107_1;

		public static Func<string> _003C_003E9__107_2;

		public static Action _003C_003E9__123_0;

		public static Action<TimeSpan> _003C_003E9__152_0;

		public static EventHandler<UnobservedTaskExceptionEventArgs> _003C_003E9__162_0;

		public static UnhandledExceptionEventHandler _003C_003E9__162_1;

		public static Func<Assembly, bool> _003C_003E9__169_0;

		public static Func<Type, bool> _003C_003E9__173_0;

		internal void _003CParseOptions_003Eb__12_2(string option)
		{
			LogManager.SetDefaultEffectiveness(Level.GetLevel(option));
		}

		internal SaveGameMetadata _003CSetupCustomAssetTypes_003Eb__50_0()
		{
			return new SaveGameMetadata();
		}

		internal MapMetadata _003CSetupCustomAssetTypes_003Eb__50_1()
		{
			return new MapMetadata();
		}

		internal CinematicCameraAsset _003CSetupCustomAssetTypes_003Eb__50_2()
		{
			return new CinematicCameraAsset();
		}

		internal void _003CAwake_003Eb__51_0(TimeSpan t)
		{
			ILog log = GameManager.log;
			if (log != null)
			{
				log.InfoFormat("GameManager created! ({0}ms)", (object)t.TotalMilliseconds);
			}
		}

		internal void _003CInitialize_003Eb__54_0(TimeSpan t)
		{
			ILog log = GameManager.log;
			if (log != null)
			{
				log.InfoFormat("GameManager initialized! ({0}ms)", (object)t.TotalMilliseconds);
			}
		}

		internal void _003CTerminateGame_003Eb__64_0(TimeSpan t)
		{
			ILog log = GameManager.log;
			if (log != null)
			{
				log.InfoFormat("GameManager destroyed ({0}ms)", (object)t.TotalMilliseconds);
			}
		}

		internal Task _003CTerminateGame_003Eb__64_1()
		{
			return AssetDatabase.global.SaveSettings();
		}

		internal void _003CTelemetryReady_003Eb__106_0(IAchievementsSupport p, AchievementId a)
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			Telemetry.AchievementUnlocked(a);
		}

		internal string _003CInitializePlatformManager_003Eb__107_0()
		{
			return "In Main-Menu";
		}

		internal string _003CInitializePlatformManager_003Eb__107_1()
		{
			return "In-Game";
		}

		internal string _003CInitializePlatformManager_003Eb__107_2()
		{
			return "In-Editor";
		}

		internal void _003CShowFallbackUI_003Eb__123_0()
		{
			try
			{
				Process.Start(new ProcessStartInfo
				{
					FileName = "https://pdxint.at/3Do979W",
					UseShellExecute = true
				});
			}
			catch
			{
				QuitGame();
			}
		}

		internal void _003CLoadAssetLibraryAsync_003Eb__152_0(TimeSpan t)
		{
			log.InfoFormat("LoadAssetLibraryAsync performed in {0}ms", (object)t.TotalMilliseconds);
		}

		internal void _003CTryCatchUnhandledExceptions_003Eb__162_0(object sender, UnobservedTaskExceptionEventArgs e)
		{
			e.SetObserved();
			log.Critical((Exception)e.Exception, (object)"Unobserved exception triggered");
		}

		internal void _003CTryCatchUnhandledExceptions_003Eb__162_1(object sender, UnhandledExceptionEventArgs e)
		{
			Exception ex = (Exception)e.ExceptionObject;
			log.Critical(ex, (object)"Unhandled domain exception triggered");
		}

		internal bool _003CListHarmonyPatches_003Eb__169_0(Assembly a)
		{
			return a.GetName().Name.Contains("Harmony");
		}

		internal bool _003CDetectModdingRuntimeName_003Eb__173_0(Type t)
		{
			if (t.Namespace != null)
			{
				return t.Namespace.StartsWith("BepInEx");
			}
			return false;
		}
	}

	private Configuration m_Configuration;

	[SerializeField]
	private string m_AdditionalCommandLineToggles;

	private static ILog log;

	private ModManager m_ModManager;

	private CancellationTokenSource m_Cts;

	private readonly CancellationTokenSource m_QuitRequested = new CancellationTokenSource();

	private readonly TaskCompletionSource<bool> m_WorldReadySource = new TaskCompletionSource<bool>();

	public GameObject[] m_SettingsDependantObjects;

	private int m_MainThreadId;

	private State m_State;

	private OverlayScreen m_InitialEngagementScreen = OverlayScreen.Engagement;

	private bool m_IsEngagementStarted;

	public const string kInMainMenuState = "#StatusInMainMenu";

	public const string kInGameState = "#StatusInGame";

	public const string kInEditorState = "#StatusInEditor";

	private bool m_StartUpTelemetryFired;

	[SerializeField]
	private string m_UILocation;

	private UIManager m_UIManager;

	private UIInputSystem m_UIInputSystem;

	private readonly ConcurrentDictionary<Guid, Func<bool>> m_Updaters = new ConcurrentDictionary<Guid, Func<bool>>();

	private const string kBootTask = "Boot";

	private LayerMask m_DefaultCullingMask;

	private LayerMask m_DefaultVolumeLayerMask;

	private ConsoleWindow m_Console;

	private static string s_ModdingRuntime;

	private World m_World;

	private UpdateSystem m_UpdateSystem;

	private LoadGameSystem m_DeserializationSystem;

	private SaveGameSystem m_SerializationSystem;

	private PrefabSystem m_PrefabSystem;

	public string[] cmdLine { get; private set; }

	public Configuration configuration
	{
		get
		{
			if (m_Configuration == null)
			{
				m_Configuration = new Configuration();
			}
			return m_Configuration;
		}
	}

	public static GameManager instance { get; private set; }

	public bool isMainThread => Thread.CurrentThread.ManagedThreadId == m_MainThreadId;

	public GameMode gameMode { get; private set; } = GameMode.Other;

	public bool isGameLoading
	{
		get
		{
			if (state != State.Booting)
			{
				return state == State.Loading;
			}
			return true;
		}
	}

	public SharedSettings settings { get; private set; }

	public ModManager modManager => m_ModManager;

	public CancellationToken terminationToken
	{
		get
		{
			if (m_Cts == null)
			{
				return CancellationToken.None;
			}
			return m_Cts.Token;
		}
	}

	public State state => m_State;

	public bool shouldUpdateManager => m_State >= State.UIReady;

	public bool shouldUpdateWorld => m_State >= State.WorldReady;

	public static UIInputSystem UIInputSystem => instance?.m_UIInputSystem;

	public LocalizationManager localizationManager { get; private set; }

	public UserInterface userInterface { get; private set; }

	public ThumbnailCache thumbnailCache { get; private set; }

	public event EventGameSaveLoad onGameSaveLoad;

	public event EventGamePreload onGamePreload;

	public event EventGamePreload onGameLoadingComplete;

	public event EventCallback onWorldReady;

	private void OnGUI()
	{
		if (shouldUpdateWorld && !m_Cts.IsCancellationRequested)
		{
			TerrainDebugSystem orCreateSystemManaged = m_World.GetOrCreateSystemManaged<TerrainDebugSystem>();
			if (((ComponentSystemBase)orCreateSystemManaged).Enabled)
			{
				orCreateSystemManaged.RenderDebugUI();
			}
		}
	}

	private static string[] MergeAdditionalCommandLineArguments(string[] cmdLineArgs, string additionalCmdLine)
	{
		HashSet<string> hashSet = (string.IsNullOrEmpty(additionalCmdLine) ? new HashSet<string>() : additionalCmdLine.Split(new char[1] { ' ' }, StringSplitOptions.RemoveEmptyEntries).ToHashSet());
		if (hashSet.Count > 0)
		{
			string[] array = new string[cmdLineArgs.Length + hashSet.Count];
			cmdLineArgs.CopyTo(array, 0);
			hashSet.CopyTo(array, cmdLineArgs.Length);
			return array;
		}
		return cmdLineArgs;
	}

	private Configuration.StdoutCaptureMode GetStdoutCaptureMode(string option)
	{
		return option switch
		{
			"console" => Configuration.StdoutCaptureMode.Console, 
			"capture" => Configuration.StdoutCaptureMode.CaptureOnly, 
			"redirect" => Configuration.StdoutCaptureMode.Redirect, 
			_ => Configuration.StdoutCaptureMode.None, 
		};
	}

	private void ParseOptions()
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_02af: Expected O, but got Unknown
		OptionSet val = new OptionSet().Add("cleanupSettings", "Cleanup unchanged settings", (Action<string>)delegate(string option)
		{
			configuration.cleanupSettings = option != null;
		}).Add("saveAllSettings", "Dump all settings regardless if they have changed", (Action<string>)delegate(string option)
		{
			configuration.saveAllSettings = option != null;
		}).Add("logsEffectiveness=", "Override effectiveness level of all logs", (Action<string>)delegate(string option)
		{
			LogManager.SetDefaultEffectiveness(Level.GetLevel(option));
		})
			.Add("duplicateLogToDefault", "Duplicate logs to default log handler", (Action<string>)delegate(string option)
			{
				configuration.duplicateLogToDefault = option != null;
			})
			.Add("developerMode", "Enable developer mode", (Action<string>)delegate(string option)
			{
				configuration.developerMode = option != null;
			})
			.Add("uiDeveloperMode", "Enable UI debugger and memory tracker", (Action<string>)delegate(string option)
			{
				configuration.uiDeveloperMode = option != null;
			})
			.Add("qaDeveloperMode", "Enable tests and automation", (Action<string>)delegate(string option)
			{
				configuration.qaDeveloperMode = option != null;
			})
			.Add("help", "Display usage", (Action<string>)delegate(string option)
			{
				configuration.showHelp = option;
			})
			.Add("disableThumbnails", "Disable thumbnails", (Action<string>)delegate(string option)
			{
				configuration.noThumbnails = option != null;
			})
			.Add("disablePdxSdk", "Disables PDX SDK integration", (Action<string>)delegate(string option)
			{
				configuration.disablePDXSDK = option != null;
			})
			.Add("disableModding", "Disable modding", (Action<string>)delegate(string option)
			{
				configuration.disableModding = option != null;
				configuration.disableCodeModding = configuration.disableModding;
			})
			.Add("disableCodeModding", "Disable code modding", (Action<string>)delegate(string option)
			{
				configuration.disableCodeModding = option != null;
			})
			.Add("disableUserSection", "Disable user section in main menu", (Action<string>)delegate(string option)
			{
				configuration.disableUserSection = option != null;
			})
			.Add("startGame=", "Auto start the game with the asset referenced", (Action<string>)delegate(string option)
			{
				//IL_0007: Unknown result type (might be due to invalid IL or missing references)
				//IL_000c: Unknown result type (might be due to invalid IL or missing references)
				configuration.startGame = Hash128.Parse(option);
			})
			.Add("profile=", "Enable profiling to the specific file", (Action<string>)delegate(string option)
			{
				configuration.profilerTarget = option;
			})
			.Add("captureStdout=", "Capture all logs on stdout. Options: \"console\",\"capture\"", (Action<string>)delegate(string option)
			{
				configuration.captureStdout = GetStdoutCaptureMode(option);
			})
			.Add("continuelastsave", "Auto start the game with the asset referenced", (Action<string>)delegate(string option)
			{
				configuration.continuelastsave = option != null;
			});
		try
		{
			string text = EnvPath.kUserDataPath + "/runOnce.txt";
			if (LongFile.Exists(text))
			{
				m_AdditionalCommandLineToggles = StringUtils.Concatenate(" ", m_AdditionalCommandLineToggles, File.ReadAllText(text));
				LongFile.Delete(text);
			}
			cmdLine = Environment.GetCommandLineArgs();
			cmdLine = MergeAdditionalCommandLineArguments(cmdLine, m_AdditionalCommandLineToggles);
			val.Parse((IEnumerable<string>)cmdLine);
			log.InfoFormat("Command line: {0}", (object)string.Join("\n", MaskArguments(cmdLine)));
			if (configuration.showHelp != null)
			{
				using (TextWriter textWriter = new StringWriter())
				{
					val.WriteOptionDescriptions(textWriter);
					configuration.showHelp = textWriter.ToString();
					return;
				}
			}
		}
		catch (OptionException ex)
		{
			Debug.LogException((Exception)ex);
		}
	}

	private static string[] MaskArguments(string[] cmdLine)
	{
		try
		{
			HashSet<string> hashSet = new HashSet<string> { "pdx-launcher-session-token", "paradox-account-userid", "accessToken", "hubSessionId", "licensingIpc" };
			string[] array = (string[])cmdLine.Clone();
			for (int i = 0; i < array.Length; i++)
			{
				string text = array[i].TrimStart('-');
				int num = text.IndexOf('=');
				if (num != -1)
				{
					string text2 = text.Substring(num + 1);
					text = text.Substring(0, num);
					array[i] = text + "=" + Utils.Sensitive(text2);
				}
				else if (hashSet.Contains(text) && i + 1 < array.Length)
				{
					array[i + 1] = Utils.Sensitive(array[i + 1]);
				}
			}
			return array;
		}
		catch (Exception ex)
		{
			log.Warn(ex, (object)"An error occured parsing the command line for logging");
			return Array.Empty<string>();
		}
	}

	public async void RegisterCancellationOnQuit(TaskCompletionSource<bool> tcs, bool stateOnCancel)
	{
		CancellationTokenRegistration cancellationTokenRegistration = m_QuitRequested.Token.Register(delegate
		{
			tcs.TrySetResult(stateOnCancel);
		});
		try
		{
			await tcs.Task;
		}
		finally
		{
			await ((IAsyncDisposable)cancellationTokenRegistration/*cast due to .constrained prefix*/).DisposeAsync();
		}
	}

	[RuntimeInitializeOnLoadMethod]
	private static void SetupCustomAssetTypes()
	{
		DefaultAssetFactory.instance.AddSupportedType<SaveGameMetadata>(".SaveGameMetadata", (Func<SaveGameMetadata>)(() => new SaveGameMetadata()), false, true);
		DefaultAssetFactory.instance.AddSupportedType<MapMetadata>(".MapMetadata", (Func<MapMetadata>)(() => new MapMetadata()), false, true);
		DefaultAssetFactory.instance.AddSupportedType<CinematicCameraAsset>(".CinematicCamera", (Func<CinematicCameraAsset>)(() => new CinematicCameraAsset()), false, true);
	}

	private async void Awake()
	{
		_ = 1;
		try
		{
			CoroutineHost.Register((ICoroutineHost)(object)this);
			if (!CheckValidity())
			{
				return;
			}
			PerformanceCounter val = PerformanceCounter.Start((Action<TimeSpan>)delegate(TimeSpan t)
			{
				ILog obj = log;
				if (obj != null)
				{
					obj.InfoFormat("GameManager created! ({0}ms)", (object)t.TotalMilliseconds);
				}
			});
			try
			{
				Task checkCapabilities = CheckCapabilities();
				DetectModdingRuntime();
				BacktraceHelper.SetDefaultAttributes(GetDefaultBacktraceAttributes());
				EnableMemoryLeaksDetection();
				Application.wantsToQuit += WantsToQuit;
				m_MainThreadId = Thread.CurrentThread.ManagedThreadId;
				m_State = State.Booting;
				Application.focusChanged += FocusChanged;
				SetNativeStackTrace();
				m_Cts = new CancellationTokenSource();
				CultureInfo.CurrentCulture = CultureInfo.InvariantCulture;
				LogManager.SetDefaultEffectiveness(Level.Info);
				BacktraceHelper.SetDefaultAttributes(GetDefaultBacktraceAttributes());
				log = LogManager.GetLogger("SceneFlow");
				TryCatchUnhandledExceptions();
				ParseOptions();
				InitConsole();
				if (!HandleConfiguration())
				{
					QuitGame();
					return;
				}
				DisableCameraRendering();
				await PreparePersistentStorage();
				HandleUserFolderVersion();
				await checkCapabilities;
				instance = this;
				Initialize();
			}
			finally
			{
				((IDisposable)val)?.Dispose();
			}
		}
		catch (Exception ex)
		{
			log.Fatal(ex);
			QuitGame();
		}
	}

	private Task CheckCapabilities()
	{
		return Capabilities.CacheCapabilities();
	}

	private void OnMainMenuReached(Purpose purpose, GameMode mode)
	{
		if (mode == GameMode.MainMenu)
		{
			AutomationClientSystem.instance.OnMainMenuReached();
		}
	}

	private async void Initialize()
	{
		_ = 13;
		try
		{
			PerformanceCounter val = PerformanceCounter.Start((Action<TimeSpan>)delegate(TimeSpan t)
			{
				ILog obj = log;
				if (obj != null)
				{
					obj.InfoFormat("GameManager initialized! ({0}ms)", (object)t.TotalMilliseconds);
				}
			});
			try
			{
				await TestScenarioSystem.Create(cmdLine);
				onGameLoadingComplete += OnMainMenuReached;
				ListHarmonyPatches();
				TaskManager taskManager = TaskManager.instance;
				InputManager.CreateInstance();
				AssetDatabase.global.SetSettingsConfiguration(configuration.saveAllSettings, configuration.cleanupSettings);
				await InitializePlatformManager();
				await taskManager.SharedTask("CacheAssets", (Func<Task>)(() => AssetDatabase.global.CacheAssets(true, m_Cts.Token)));
				Task caching = taskManager.SharedTask("CacheAssets", (Func<Task>)(() => AssetDatabase.global.CacheAssets(false, m_Cts.Token)));
				InitializeLocalization();
				settings = new SharedSettings(localizationManager);
				CreateWorld();
				InputManager.instance.SetDefaultControlScheme();
				await InitializeUI();
				TaskManager.instance.onNotifyProgress += new OnNotifyProgress(NotifyProgress);
				ReportBootProgress(0f);
				Task engagement = SetInitialEngagementScreenActive();
				Task loading = SetScreenActive<LoadingScreen>();
				await SetScreenActive<SplashScreenSequence>();
				Task assetLoading = LoadUnityPrefabs();
				log.Info((object)GetVersionsInfo());
				log.Info((object)GetSystemInfoString());
				configuration.LogConfiguration();
				await engagement;
				RegisterDeviceAndUserListeners();
				m_ModManager = new ModManager(configuration.disableCodeModding);
				await caching;
				await RegisterPdxSdk();
				ReportBootProgress(0.3f);
				settings.LoadUserSettings();
				ReportBootProgress(0.5f);
				CreateSystems();
				InitializeModManager();
				settings.Apply();
				EnableSettingsDependantObjects();
				await assetLoading;
				ReportBootProgress(0.8f);
				LoadPrefabs();
				InitializeThumbnails();
				m_State = State.WorldReady;
				ReportBootProgress(1f);
				await Task.Yield();
				this.onWorldReady?.Invoke();
				await Task.WhenAll(loading, PlatformManager.instance.WaitForAchievements());
				EnableCameraRendering();
				m_WorldReadySource.TrySetResult(result: true);
				log.Info((object)"Boot completed");
				bool flag = true;
				if (((Hash128)(ref configuration.startGame)).isValid)
				{
					flag = !(await AutoLoad(configuration.startGame));
				}
				else if (configuration.continuelastsave)
				{
					flag = !(await userInterface.appBindings.LauncherContinueGame());
				}
				if (flag)
				{
					await MainMenu();
				}
			}
			finally
			{
				((IDisposable)val)?.Dispose();
			}
		}
		catch (OperationCanceledException)
		{
			Debug.Log((object)"GameManager termination requested before initialization completed");
		}
		catch (Exception ex2)
		{
			log.Fatal(ex2);
			ShowFallbackUI(ex2);
		}
	}

	private void InitializeModManager(bool ignoreParadox = false)
	{
		if (m_UpdateSystem != null && (ignoreParadox || AssetDatabase<ParadoxMods>.instance.isCached))
		{
			m_ModManager.Initialize(m_UpdateSystem);
		}
	}

	private void EnableSettingsDependantObjects()
	{
		m_Cts.Token.ThrowIfCancellationRequested();
		GameObject[] settingsDependantObjects = m_SettingsDependantObjects;
		foreach (GameObject obj in settingsDependantObjects)
		{
			obj.SetActive(Object.op_Implicit((Object)(object)obj));
		}
	}

	public async Task<bool> WaitForReadyState()
	{
		_ = 1;
		try
		{
			CancellationTokenRegistration cancellationTokenRegistration = m_Cts.Token.Register(delegate
			{
				m_WorldReadySource.TrySetCanceled();
			});
			try
			{
				await m_WorldReadySource.Task.ConfigureAwait(continueOnCapturedContext: false);
			}
			finally
			{
				await ((IAsyncDisposable)cancellationTokenRegistration/*cast due to .constrained prefix*/).DisposeAsync();
			}
			return ((Task)m_WorldReadySource.Task).IsCompletedSuccessfully;
		}
		catch (OperationCanceledException)
		{
			return false;
		}
	}

	private void Update()
	{
		if (shouldUpdateManager && !m_Cts.IsCancellationRequested)
		{
			TestScenarioSystem.instance.Update();
			InputManager.instance.Update();
			m_UIInputSystem.DispatchInputEvents(InputManager.instance.activeControlScheme == InputManager.ControlScheme.KeyboardAndMouse);
			UpdateWorld();
			UpdateUI();
			PostUpdateWorld();
		}
		UpdateUpdaters();
		UpdatePlatforms();
	}

	private void LateUpdate()
	{
		if (!m_Cts.IsCancellationRequested)
		{
			LateUpdateWorld();
		}
	}

	public static void QuitGame()
	{
		Application.Quit();
	}

	public void FocusChanged(bool hasFocus)
	{
		InputManager.instance?.OnFocusChanged(hasFocus);
	}

	private bool WantsToQuit()
	{
		if (m_State != State.Quitting && m_State != State.Terminated)
		{
			TerminateGame();
			return false;
		}
		if (m_State == State.Quitting)
		{
			Debug.LogWarning((object)"TerminateGame is already in progress, please wait.");
			return false;
		}
		return true;
	}

	private void OnDestroy()
	{
		Application.wantsToQuit -= WantsToQuit;
	}

	private async Task TerminateGame()
	{
		if (m_Cts == null)
		{
			m_State = State.Terminated;
			QuitGame();
		}
		else
		{
			if (m_State == State.Quitting || m_State == State.Terminated)
			{
				return;
			}
			try
			{
				PerformanceCounter val = PerformanceCounter.Start((Action<TimeSpan>)delegate(TimeSpan t)
				{
					ILog obj = log;
					if (obj != null)
					{
						obj.InfoFormat("GameManager destroyed ({0}ms)", (object)t.TotalMilliseconds);
					}
				});
				try
				{
					State quittingState = m_State;
					m_State = State.Quitting;
					m_QuitRequested.Cancel();
					await TaskManager.instance.Complete("SaveLoadGame");
					if (quittingState >= State.WorldReady)
					{
						LauncherSettings.SaveSettings(settings);
						await TaskManager.instance.SharedTask("CacheAssets", (Func<Task>)(() => AssetDatabase.global.SaveSettings()));
					}
					TaskManager.instance.onNotifyProgress -= new OnNotifyProgress(NotifyProgress);
					m_Cts.Cancel();
					m_ModManager?.Dispose();
					DestroyWorld();
					DisposeThumbnails();
					bool flag = await DisposePlatforms().AwaitWithTimeout(TimeSpan.FromSeconds(10.0));
					ReleaseUI();
					InputManager.DestroyInstance();
					((MonoBehaviour)this).StopAllCoroutines();
					CoroutineHost.Register((ICoroutineHost)null);
					bool flag2 = flag;
					flag = flag2 & await TaskManager.instance.CompleteAndClear().AwaitWithTimeout(TimeSpan.FromSeconds(10.0));
					VolumeHelper.Dispose();
					AssetDatabase.global.Dispose();
					LogManager.ReleaseResources();
					Gizmos.ReleaseResources();
					LogManager.stdOutActive = false;
					ReleaseConsole();
					onGameLoadingComplete -= OnMainMenuReached;
					TestScenarioSystem.Destroy();
					instance = null;
					Application.focusChanged -= FocusChanged;
					if (flag)
					{
						Debug.Log((object)"Game terminated successfully");
					}
					else
					{
						Debug.Log((object)"Game terminated due to timeout");
					}
				}
				finally
				{
					((IDisposable)val)?.Dispose();
				}
			}
			catch (Exception ex)
			{
				instance = null;
				log.Error(ex);
			}
			finally
			{
				m_State = State.Terminated;
				QuitGame();
			}
		}
	}

	private Task SetInitialEngagementScreenActive()
	{
		m_IsEngagementStarted = true;
		if (!PlatformManager.instance.requiresEngagement)
		{
			return Task.CompletedTask;
		}
		if (AutomationClientSystem.instance.IsConnected)
		{
			return Task.CompletedTask;
		}
		return m_InitialEngagementScreen switch
		{
			OverlayScreen.UserLoggedOut => SetScreenActive<LoggedOutScreen>(), 
			OverlayScreen.ControllerPairingChanged => SetScreenActive<ControllerPairingScreen>(), 
			OverlayScreen.ControllerDisconnected => SetScreenActive<ControllerDisconnectedScreen>(), 
			_ => SetScreenActive<EngagementScreen>(), 
		};
	}

	private void RegisterDeviceAndUserListeners()
	{
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Expected O, but got Unknown
		InputManager.instance.EventActiveDeviceAssociationLost += HandleDeviceAssociationLost;
		InputManager.instance.EventActiveDeviceDisconnected += HandleDeviceDisconnected;
		InputManager.instance.EventDevicePaired += HandleDevicePaired;
		PlatformManager.instance.onUserUpdated += new OnUserUpdatedEventHandler(HandleUserUpdated);
	}

	private void HandleDeviceAssociationLost()
	{
		if (m_IsEngagementStarted)
		{
			SetScreenActive<ControllerPairingScreen>();
		}
		else if (m_InitialEngagementScreen > OverlayScreen.ControllerPairingChanged)
		{
			m_InitialEngagementScreen = OverlayScreen.ControllerPairingChanged;
		}
	}

	private void HandleDeviceDisconnected()
	{
		if (m_IsEngagementStarted)
		{
			SetScreenActive<ControllerDisconnectedScreen>();
		}
		else if (m_InitialEngagementScreen > OverlayScreen.ControllerDisconnected)
		{
			m_InitialEngagementScreen = OverlayScreen.ControllerDisconnected;
		}
	}

	private void HandleDevicePaired()
	{
		if (!m_IsEngagementStarted)
		{
			m_InitialEngagementScreen = OverlayScreen.Engagement;
		}
	}

	private void HandleUserUpdated(IPlatformServiceIntegration psi, UserChangedFlags flags)
	{
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		if (m_IsEngagementStarted)
		{
			if (((Enum)flags).HasFlag((Enum)(object)(UserChangedFlags)8) && !((Enum)flags).HasFlag((Enum)(object)(UserChangedFlags)64))
			{
				SetScreenActive<LoggedOutScreen>();
			}
		}
		else if (((Enum)flags).HasFlag((Enum)(object)(UserChangedFlags)8) && m_InitialEngagementScreen > OverlayScreen.UserLoggedOut)
		{
			m_InitialEngagementScreen = OverlayScreen.UserLoggedOut;
		}
		else if (((Enum)flags).HasFlag((Enum)(object)(UserChangedFlags)32))
		{
			m_InitialEngagementScreen = OverlayScreen.Engagement;
		}
	}

	private void CleanupMemory()
	{
		Resources.UnloadUnusedAssets();
		foreach (UISystem uISystem in UIManager.UISystems)
		{
			uISystem.ClearCachedUnusedImages();
		}
		GC.Collect();
	}

	private async Task<string[]> SaveSimulationData(Context context, Stream stream)
	{
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		CleanupMemory();
		m_SerializationSystem.stream = stream;
		m_SerializationSystem.context = context;
		await m_SerializationSystem.RunOnce();
		string[] array = ((IEnumerable<Entity>)(object)m_SerializationSystem.referencedContent).Select((Entity x) => m_PrefabSystem.GetPrefabName(x)).ToArray();
		return (array.Length != 0) ? array : null;
	}

	public Task<bool> Save(string saveName, SaveInfo meta, ILocalAssetDatabase database, Texture savePreview)
	{
		return Save(saveName, meta, database, new ScreenCaptureHelper.AsyncRequest(savePreview));
	}

	public unsafe async Task<bool> Save(string saveName, SaveInfo meta, ILocalAssetDatabase database, ScreenCaptureHelper.AsyncRequest previewRequest)
	{
		log.Info((object)("Save " + saveName + " to " + ((IAssetDatabase)database).name));
		this.onGameSaveLoad?.Invoke(saveName, start: true);
		ILocalAssetDatabase saveDatabase = AssetDatabase.GetTransient(0L, (string)null);
		try
		{
			meta.sessionGuid = Telemetry.GetCurrentSession();
			meta.lastModified = DateTime.Now;
			AssetDataPath saveNameDataPath = AssetDataPath.op_Implicit(saveName);
			SaveGameData saveGameData = saveDatabase.AddAsset<SaveGameData>(saveNameDataPath, default(Hash128));
			Context context = default(Context);
			((Context)(ref context))._002Ector((Purpose)0, Version.current, ((AssetData)saveGameData).id.guid);
			SaveInfo saveInfo = meta;
			saveInfo.contentPrerequisites = await SaveSimulationData(context, ((AssetData)saveGameData).GetWriteStream());
			meta.saveGameData = saveGameData;
			if (previewRequest != null)
			{
				await previewRequest.Complete();
				Texture val = Texture.CreateUncompressed1Mip(saveName, previewRequest.width, previewRequest.height, false, previewRequest.result);
				try
				{
					TextureAsset val2 = TextureAssetExtensions.AddAsset(saveDatabase, (ITexture)(object)val, false);
					try
					{
						meta.preview = val2;
						val2.Save(0, false);
					}
					finally
					{
						((IDisposable)val2)?.Dispose();
					}
				}
				finally
				{
					((IDisposable)val)?.Dispose();
				}
			}
			PackageAsset val3 = await Task.Run(delegate
			{
				//IL_000e: Unknown result type (might be due to invalid IL or missing references)
				//IL_0014: Unknown result type (might be due to invalid IL or missing references)
				//IL_0045: Unknown result type (might be due to invalid IL or missing references)
				//IL_004a: Unknown result type (might be due to invalid IL or missing references)
				SaveGameMetadata saveGameMetadata = saveDatabase.AddAsset<SaveGameMetadata>(saveNameDataPath, default(Hash128));
				((Metadata<SaveInfo>)saveGameMetadata).target = meta;
				((AssetData)saveGameMetadata).Save(false);
				AssetDataPath assetDataPath = SaveHelpers.GetAssetDataPath<SaveGameMetadata>(database, saveName);
				DisableNotificationsScoped val4 = AssetDatabase.global.DisableNotificationsScoped();
				try
				{
					PackageAsset val5 = default(PackageAsset);
					if (database.Exists<PackageAsset>(assetDataPath, ref val5))
					{
						((IAssetDatabase)database).DeleteAsset<PackageAsset>(val5);
					}
					PackageAsset obj = PackageAssetExtensions.AddAsset(database, assetDataPath, saveDatabase);
					((AssetData)obj).Save(false);
					settings.userState.lastSaveGameMetadata = saveGameMetadata;
					settings.userState.ApplyAndSave();
					Launcher.SaveLastSaveMetadata(meta);
					return obj;
				}
				finally
				{
					((IDisposable)(*(DisableNotificationsScoped*)(&val4))/*cast due to .constrained prefix*/).Dispose();
				}
			});
			this.onGameSaveLoad?.Invoke(saveName, start: false);
			log.InfoFormat("Saving completed {0}", (object)val3);
			return true;
		}
		finally
		{
			if (saveDatabase != null)
			{
				((IDisposable)saveDatabase).Dispose();
			}
		}
	}

	private Task LoadSimulationData(Context context, AsyncReadDescriptor dataDescriptor)
	{
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		this.onGamePreload?.Invoke(((Context)(ref context)).purpose, gameMode);
		CleanupMemory();
		m_DeserializationSystem.dataDescriptor = dataDescriptor;
		m_DeserializationSystem.context = context;
		return m_DeserializationSystem.RunOnce();
	}

	private async Task<bool> Load(GameMode mode, Purpose purpose, AsyncReadDescriptor descriptor, Hash128 instigatorGuid, Guid sessionGuid)
	{
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		log.InfoFormat("Loading mode {0} with purpose {1}", (object)mode, (object)purpose);
		if (descriptor == AsyncReadDescriptor.Invalid && (int)purpose != 1 && (int)purpose != 4 && (int)purpose != 6)
		{
			log.WarnFormat("Invalid descriptor provided with purpose {0}", (object)purpose);
			return false;
		}
		GameMode oldMode = gameMode;
		gameMode = mode;
		m_State = State.Loading;
		if (mode.IsGameOrEditor())
		{
			TaskManager taskManager = TaskManager.instance;
			taskManager.ScheduleGroup((Group)1, 1);
			taskManager.ScheduleGroup((Group)2, 1);
			taskManager.ScheduleGroup((Group)3, 1);
			TaskProgress progress = taskManager.progress;
			ProgressTracker val = new ProgressTracker("LoadTextures", (Group)1, false);
			((ProgressTracker)(ref val)).progress = 0f;
			progress.Report(val);
			TaskProgress progress2 = taskManager.progress;
			val = new ProgressTracker("LoadMeshes", (Group)2, false);
			((ProgressTracker)(ref val)).progress = 0f;
			progress2.Report(val);
			TaskProgress progress3 = taskManager.progress;
			val = new ProgressTracker("LoadSimulation", (Group)3, false);
			((ProgressTracker)(ref val)).progress = 0f;
			progress3.Report(val);
			TextureStreamingSystem tss = m_World.GetExistingSystemManaged<TextureStreamingSystem>();
			RegisterUpdater(delegate
			{
				//IL_003f: Unknown result type (might be due to invalid IL or missing references)
				float num = (tss.VTMaterialAssetsProgression + tss.VTMaterialDuplicatesProgression) * 0.5f;
				TaskProgress progress4 = taskManager.progress;
				ProgressTracker val2 = default(ProgressTracker);
				((ProgressTracker)(ref val2))._002Ector("LoadTextures", (Group)1, false);
				((ProgressTracker)(ref val2)).progress = num;
				progress4.Report(val2);
				return num >= 1f;
			});
		}
		Task loading = SetScreenActive<LoadingScreen>();
		await UnityTask.WaitForGPUFrame();
		if (mode != GameMode.MainMenu || oldMode != GameMode.Other)
		{
			Context context = default(Context);
			((Context)(ref context))._002Ector(purpose, Version.current, instigatorGuid);
			await LoadSimulationData(context, descriptor);
		}
		switch (gameMode)
		{
		case GameMode.Editor:
			Telemetry.OpenSession(sessionGuid);
			userInterface.appBindings.SetEditorActive();
			break;
		case GameMode.Game:
			Telemetry.OpenSession(sessionGuid);
			userInterface.appBindings.SetGameActive();
			break;
		case GameMode.MainMenu:
			Telemetry.CloseSession();
			userInterface.appBindings.SetMainMenuActive();
			Cursor.lockState = (CursorLockMode)0;
			break;
		}
		PlatformManager.instance.SetRichPresence(gameMode.ToRichPresence());
		await loading;
		CleanupMemory();
		this.onGameLoadingComplete?.Invoke(purpose, mode);
		m_State = State.WorldReady;
		HDCamera.GetOrCreate(Camera.main, 0).Reset();
		log.Info((object)"Loading completed");
		return true;
	}

	private Guid GetSessionGuid(Purpose purpose, Guid existingGuid)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Invalid comparison between Unknown and I4
		//IL_0004: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Invalid comparison between Unknown and I4
		if ((int)purpose == 4 || (int)purpose == 1)
		{
			return Guid.NewGuid();
		}
		return existingGuid;
	}

	public Task<bool> Load(GameMode mode, Purpose purpose, IAssetData asset = null)
	{
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_013b: Unknown result type (might be due to invalid IL or missing references)
		//IL_013d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0143: Unknown result type (might be due to invalid IL or missing references)
		//IL_0148: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_0173: Unknown result type (might be due to invalid IL or missing references)
		//IL_0175: Unknown result type (might be due to invalid IL or missing references)
		//IL_017b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0180: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e8: Unknown result type (might be due to invalid IL or missing references)
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0073: Unknown result type (might be due to invalid IL or missing references)
		//IL_0195: Unknown result type (might be due to invalid IL or missing references)
		//IL_0196: Unknown result type (might be due to invalid IL or missing references)
		//IL_019b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ed: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fb: Unknown result type (might be due to invalid IL or missing references)
		log.InfoFormat("Starting game from '{0}'", (object)asset);
		if (asset is MapMetadata mapMetadata)
		{
			MapMetadata mapMetadata2 = mapMetadata;
			try
			{
				AsyncReadDescriptor descriptor = AsyncReadDescriptor.Invalid;
				MapInfo target = ((Metadata<MapInfo>)mapMetadata).target;
				if ((AssetData)(object)target.mapData == (IAssetData)null)
				{
					log.WarnFormat("The mapData referenced by '{0}' (meta: '{1}') doesn't not exist'", (object)target.id, (object)asset);
				}
				else
				{
					descriptor = ((AssetData)target.mapData).GetAsyncReadDescriptor();
				}
				return Load(mode, purpose, descriptor, Identifier.op_Implicit(((AssetData)mapMetadata).id), GetSessionGuid(purpose, target.sessionGuid));
			}
			finally
			{
				((IDisposable)mapMetadata2)?.Dispose();
			}
		}
		if (asset is SaveGameMetadata saveGameMetadata)
		{
			SaveGameMetadata saveGameMetadata2 = saveGameMetadata;
			try
			{
				AsyncReadDescriptor descriptor2 = AsyncReadDescriptor.Invalid;
				SaveInfo target2 = ((Metadata<SaveInfo>)saveGameMetadata).target;
				if ((AssetData)(object)target2.saveGameData == (IAssetData)null)
				{
					log.WarnFormat("The saveGameData referenced by '{0}' (meta: '{1}') doesn't not exist'", (object)target2.id, (object)asset);
				}
				else
				{
					descriptor2 = ((AssetData)target2.saveGameData).GetAsyncReadDescriptor();
				}
				return Load(mode, purpose, descriptor2, Identifier.op_Implicit(((AssetData)saveGameMetadata).id), GetSessionGuid(purpose, target2.sessionGuid));
			}
			finally
			{
				((IDisposable)saveGameMetadata2)?.Dispose();
			}
		}
		MapData val = (MapData)(object)((asset is MapData) ? asset : null);
		if (val != null)
		{
			log.Warn((object)"Loading with MapData. Session guid will be lost, rather use metadata if available");
			return Load(mode, purpose, ((AssetData)val).GetAsyncReadDescriptor(), Identifier.op_Implicit(((AssetData)val).id), Guid.NewGuid());
		}
		SaveGameData val2 = (SaveGameData)(object)((asset is SaveGameData) ? asset : null);
		if (val2 != null)
		{
			log.Warn((object)"Loading with SaveGameData. Session guid will be lost, rather use metadata if available");
			return Load(mode, purpose, ((AssetData)val2).GetAsyncReadDescriptor(), Identifier.op_Implicit(((AssetData)val2).id), Guid.NewGuid());
		}
		if (asset == null)
		{
			return Load(mode, purpose, AsyncReadDescriptor.Invalid, Hash128.Empty, Guid.NewGuid());
		}
		log.WarnFormat("Couldn't start game from '{0}'", (object)asset);
		return Task.FromResult(result: false);
	}

	public Task<bool> Load(GameMode mode, Purpose purpose, Hash128 guid)
	{
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		IAssetData asset = default(IAssetData);
		if (AssetDatabase.global.TryGetAsset(guid, ref asset))
		{
			return Load(mode, purpose, asset);
		}
		log.WarnFormat("Couldn't load '{0}'. Asset doesn't exist!", (object)guid);
		return Task.FromResult(result: false);
	}

	private Task<bool> AutoLoad(IAssetData asset)
	{
		if (asset is MapData || asset is MapMetadata)
		{
			return Load(GameMode.Game, (Purpose)1, asset);
		}
		if (asset is SaveGameData || asset is SaveGameMetadata)
		{
			return Load(GameMode.Game, (Purpose)2, asset);
		}
		log.WarnFormat("Couldn't load '{0}'. Asset doesn't exist!", (object)asset);
		return Task.FromResult(result: false);
	}

	private Task<bool> AutoLoad(Hash128 guid)
	{
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		IAssetData asset = default(IAssetData);
		if (AssetDatabase.global.TryGetAsset(guid, ref asset))
		{
			return AutoLoad(asset);
		}
		log.WarnFormat("Couldn't load '{0}'. Asset doesn't exist!", (object)guid);
		return Task.FromResult(result: false);
	}

	public async Task<bool> MainMenu()
	{
		_ = 2;
		try
		{
			IAssetData asset = null;
			bool ret = await Load(GameMode.MainMenu, (Purpose)6, asset);
			if (ret)
			{
				await AudioManager.instance.ResetAudioOnMainThread();
				await AudioManager.instance.PlayMenuMusic("Main Menu Theme");
				log.Info((object)"MainMenu reached");
			}
			return ret;
		}
		catch (OperationCanceledException)
		{
		}
		catch (Exception ex2)
		{
			log.Error(ex2);
		}
		return false;
	}

	private Task PreparePersistentStorage()
	{
		EnvPath.RegisterSpecialPath<SaveGameMetadata>(SaveGameMetadata.kPersistentLocation);
		EnvPath.RegisterSpecialPath<SaveGameData>(SaveGameMetadata.kPersistentLocation);
		EnvPath.RegisterSpecialPath<MapMetadata>(MapMetadata.kPersistentLocation);
		EnvPath.RegisterSpecialPath<MapData>(MapMetadata.kPersistentLocation);
		EnvPath.RegisterSpecialPath<CinematicCameraAsset>(CinematicCameraAsset.kPersistentLocation);
		return EnvPath.WipeTempPath();
	}

	private async Task RegisterPdxSdk()
	{
		if (!configuration.disablePDXSDK)
		{
			PdxSdkConfiguration val = new PdxSdkConfiguration
			{
				language = localizationManager.activeLocaleId,
				gameNamespace = "cities_skylines_2"
			};
			Version current = Version.current;
			val.gameVersion = ((Version)(ref current)).fullVersion;
			val.environment = (ProductEnvironment)1;
			PdxSdkConfiguration pdxConfiguration = val;
			await PlatformManager.instance.RegisterPSI<PdxSdkPlatform>((Func<PdxSdkPlatform>)delegate
			{
				//IL_001a: Unknown result type (might be due to invalid IL or missing references)
				//IL_001f: Unknown result type (might be due to invalid IL or missing references)
				//IL_0024: Unknown result type (might be due to invalid IL or missing references)
				//IL_0040: Expected O, but got Unknown
				//IL_006e: Unknown result type (might be due to invalid IL or missing references)
				//IL_0078: Expected O, but got Unknown
				//IL_009c: Unknown result type (might be due to invalid IL or missing references)
				//IL_00a6: Expected O, but got Unknown
				//IL_00b3: Unknown result type (might be due to invalid IL or missing references)
				//IL_00bd: Expected O, but got Unknown
				//IL_00cf: Unknown result type (might be due to invalid IL or missing references)
				//IL_00d9: Expected O, but got Unknown
				//IL_00fd: Unknown result type (might be due to invalid IL or missing references)
				//IL_0107: Expected O, but got Unknown
				CancellationTokenSource cts = new CancellationTokenSource();
				string text = default(string);
				PdxSdkPlatform pdxSdkPlatform = new PdxSdkPlatform(pdxConfiguration)
				{
					translationHandler = (string localeId) => (!localizationManager.activeDictionary.TryGetValue(localeId, ref text)) ? localeId : text
				};
				localizationManager.onActiveDictionaryChanged += delegate
				{
					pdxSdkPlatform.ChangeLanguage(localizationManager.activeLocaleId);
				};
				pdxSdkPlatform.onLegalDocumentStatusChanged += (OnLegalDocumentStatusChangedEventHandler)delegate(LegalDocument doc, int remaining)
				{
					if (remaining == 0)
					{
						TelemetryReady();
						PlatformManager.instance.EnableSharing();
					}
				};
				pdxSdkPlatform.onNoLogin += async delegate
				{
					try
					{
						if (state != State.Quitting && !configuration.disableModding)
						{
							await RegisterDatabase();
						}
					}
					catch (OperationCanceledException)
					{
					}
					catch (Exception ex2)
					{
						InitializeModManager(ignoreParadox: true);
						PdxSdkPlatform.log.Error(ex2);
					}
					finally
					{
						cts = new CancellationTokenSource();
					}
				};
				pdxSdkPlatform.onLoggedIn += (OnLoggedInEventHandler)async delegate
				{
					_ = 1;
					try
					{
						if (state != State.Quitting)
						{
							Task task = Task.CompletedTask;
							if (!configuration.disableModding)
							{
								task = pdxSdkPlatform.SyncMods((SyncDirection)0);
							}
							if (!configuration.disableModding)
							{
								await task;
								await RegisterDatabase();
							}
						}
					}
					catch (OperationCanceledException)
					{
					}
					catch (Exception ex2)
					{
						InitializeModManager(ignoreParadox: true);
						PdxSdkPlatform.log.Error(ex2);
					}
					finally
					{
						cts = new CancellationTokenSource();
					}
				};
				pdxSdkPlatform.onLoggedOut += (OnLoggedOutEventHandler)async delegate
				{
					cts.Cancel();
					cts = new CancellationTokenSource();
					if (!configuration.disableModding && await AssetDatabase.global.UnregisterDatabase((IAssetDatabase)(object)AssetDatabase<ParadoxMods>.instance))
					{
						AssetDatabase<ParadoxMods>.instance.Dispose();
					}
				};
				pdxSdkPlatform.onContentUnlocked += (ContentUnlockedEventHandler)delegate(List<IDlc> dlcs)
				{
					if (dlcs != null)
					{
						foreach (IDlc dlc in dlcs)
						{
							if (!string.IsNullOrEmpty(dlc.internalName))
							{
								string internalName = dlc.internalName;
								string internalName2 = dlc.internalName;
								string internalName3 = dlc.internalName;
								ProgressState? progressState = (ProgressState)3;
								NotificationSystem.Pop(internalName, 4f, null, null, internalName2, internalName3, null, progressState);
							}
						}
					}
					LoadUnityPrefabs();
				};
				pdxSdkPlatform.onDataSyncConflict += delegate
				{
					ProgressState? progressState = (ProgressState)5;
					NotificationSystem.Push("PDXDataSyncConflict", null, null, "ActionRequired", "PDXDataSyncConflict", null, progressState, null, async delegate
					{
						TaskCompletionSource<int> tcs = new TaskCompletionSource<int>();
						userInterface.appBindings.ShowConfirmationDialog(new ParadoxCloudConflictResolutionDialog(), delegate(int msg)
						{
							tcs.SetResult(msg);
						});
						await tcs.Task;
						if (tcs.Task.Result != -1)
						{
							NotificationSystem.Pop("PDXDataSyncConflict");
							ProgressState? progressState2 = (ProgressState)2;
							NotificationSystem.Push("PDXDataSyncConflictResolving", null, null, "PDXDataSyncConflict", "PDXDataSyncConflictResolving", null, progressState2);
							if (await pdxSdkPlatform.SyncModConflict((SyncDirection)((tcs.Task.Result == 0) ? 1 : 2)))
							{
								progressState2 = (ProgressState)3;
								NotificationSystem.Pop("PDXDataSyncConflictResolving", 1f, null, null, "PDXDataSyncConflict", "PDXDataSyncConflictResolved", null, progressState2);
							}
							else
							{
								progressState2 = (ProgressState)4;
								NotificationSystem.Pop("PDXDataSyncConflictResolving", 1f, null, null, "PDXDataSyncConflict", "PDXDataSyncConflictFailed", null, progressState2);
							}
						}
					});
				};
				pdxSdkPlatform.onModSyncCompleted += (ModSyncEventHandler)delegate
				{
					if (!pdxSdkPlatform.HasLocalChanges())
					{
						NotificationSystem.Pop("PDXDataSyncConflict");
					}
				};
				return pdxSdkPlatform;
				async Task RegisterDatabase()
				{
					cts.Token.ThrowIfCancellationRequested();
					AssetDatabase<ParadoxMods> modsDatabase = AssetDatabase<ParadoxMods>.instance;
					IDataSourceProvider dataSource = modsDatabase.dataSource;
					ParadoxModsDataSource dataSource2 = (ParadoxModsDataSource)(object)((dataSource is ParadoxModsDataSource) ? dataSource : null);
					if (dataSource2 != null)
					{
						dataSource2.onAfterActivePlaysetOrModStatusChanged -= OnActivePlaysetChanged;
						modsDatabase.onAssetDatabaseChanged.Unsubscribe((EventDelegate<AssetChangedEventArgs>)OnAssetChanged);
						dataSource2.onEntryIsInActivePlaysetChanged -= OnEntryIsInActivePlaysetChanged;
						await AssetDatabase.global.RegisterDatabase((IAssetDatabase)(object)modsDatabase);
						modsDatabase.onAssetDatabaseChanged.Subscribe((EventDelegate<AssetChangedEventArgs>)OnAssetChanged);
						dataSource2.onEntryIsInActivePlaysetChanged += OnEntryIsInActivePlaysetChanged;
						dataSource2.onAfterActivePlaysetOrModStatusChanged += OnActivePlaysetChanged;
						await dataSource2.Populate();
					}
					InitializeModManager(!modsDatabase.isCached);
					void OnEntryIsInActivePlaysetChanged(Hash128 guid, bool isInActivePlayset)
					{
						//IL_0006: Unknown result type (might be due to invalid IL or missing references)
						IAssetData val2 = default(IAssetData);
						if (modsDatabase.TryGetAsset(guid, ref val2))
						{
							ExecutableAsset val3 = (ExecutableAsset)(object)((val2 is ExecutableAsset) ? val2 : null);
							if (val3 == null)
							{
								UIModuleAsset val4 = (UIModuleAsset)(object)((val2 is UIModuleAsset) ? val2 : null);
								if (val4 == null)
								{
									if (!(val2 is SurfaceAsset) && !(val2 is MidMipCacheAsset))
									{
										PrefabAsset val5 = (PrefabAsset)(object)((val2 is PrefabAsset) ? val2 : null);
										if (val5 != null)
										{
											try
											{
												log.DebugFormat("OnEntryIsInActivePlaysetChanged: {0} ({1})", (object)val2.name, (object)isInActivePlayset);
												PrefabBase prefabBase = val5.Load() as PrefabBase;
												if (isInActivePlayset)
												{
													if (m_PrefabSystem.AddPrefab(prefabBase))
													{
														log.DebugFormat("Loaded {0}", (object)((Object)prefabBase).name);
													}
												}
												else if (m_PrefabSystem.RemovePrefab(prefabBase))
												{
													log.DebugFormat("Removed {0}", (object)((Object)prefabBase).name);
												}
											}
											catch (Exception ex)
											{
												log.Error(ex);
											}
										}
									}
									else
									{
										TextureStreamingSystem orCreateSystemManaged = m_World.GetOrCreateSystemManaged<TextureStreamingSystem>();
										if (orCreateSystemManaged != null)
										{
											orCreateSystemManaged.MarkVTAssetsDirty();
										}
									}
								}
								else
								{
									val4.isInActivePlayset = isInActivePlayset;
								}
							}
							else
							{
								val3.isInActivePlayset = isInActivePlayset;
							}
						}
					}
				}
			}, m_Cts.Token).ConfigureAwait(continueOnCapturedContext: false);
		}
		else
		{
			PlatformManager.instance.EnableSharing();
			await Task.CompletedTask;
		}
		void OnActivePlaysetChanged()
		{
			TextureStreamingSystem orCreateSystemManaged = m_World.GetOrCreateSystemManaged<TextureStreamingSystem>();
			if (orCreateSystemManaged != null)
			{
				orCreateSystemManaged.RefreshVT((ILocalAssetDatabase)(object)AssetDatabase<ParadoxMods>.instance);
			}
		}
		void OnAssetChanged(AssetChangedEventArgs args)
		{
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			//IL_0008: Invalid comparison between Unknown and I4
			if ((int)((AssetChangedEventArgs)(ref args)).change == 3)
			{
				IAssetData asset = ((AssetChangedEventArgs)(ref args)).asset;
				UIModuleAsset val2 = (UIModuleAsset)(object)((asset is UIModuleAsset) ? asset : null);
				if (val2 == null)
				{
					ExecutableAsset val3 = (ExecutableAsset)(object)((asset is ExecutableAsset) ? asset : null);
					if (val3 != null)
					{
						val3.onActivePlaysetChanged += delegate(ExecutableAsset val4, bool isInActivePlayset)
						{
							if (val4.isILAssembly && val4.isLoaded != isInActivePlayset)
							{
								m_ModManager?.RequireRestart();
							}
						};
					}
				}
				else
				{
					val2.onActivePlaysetChanged += delegate(UIModuleAsset uiModule, bool isInActivePlayset)
					{
						if (isInActivePlayset)
						{
							m_ModManager?.AddUIModule(uiModule);
						}
						else
						{
							m_ModManager?.RemoveUIModule(uiModule);
						}
					};
				}
			}
		}
	}

	private void TelemetryReady()
	{
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Expected O, but got Unknown
		if (m_StartUpTelemetryFired)
		{
			return;
		}
		Telemetry.FireSessionStartEvents();
		PlatformManager obj = PlatformManager.instance;
		object obj2 = _003C_003Ec._003C_003E9__106_0;
		if (obj2 == null)
		{
			AchievementUpdatedEventHandler val = delegate(IAchievementsSupport p, AchievementId a)
			{
				//IL_0000: Unknown result type (might be due to invalid IL or missing references)
				Telemetry.AchievementUnlocked(a);
			};
			_003C_003Ec._003C_003E9__106_0 = val;
			obj2 = (object)val;
		}
		obj.onAchievementUpdated += (AchievementUpdatedEventHandler)obj2;
		m_StartUpTelemetryFired = true;
	}

	private async Task InitializePlatformManager()
	{
		PlatformManager.instance.RegisterRichPresenceKey("#StatusInMainMenu", (Func<string>)(() => "In Main-Menu"));
		PlatformManager.instance.RegisterRichPresenceKey("#StatusInGame", (Func<string>)(() => "In-Game"));
		PlatformManager.instance.RegisterRichPresenceKey("#StatusInEditor", (Func<string>)(() => "In-Editor"));
		await PlatformManager.instance.RegisterPSI<IPlatformServiceIntegration>(PlatformSupport.kCreateSteamPlatform, m_Cts.Token).ConfigureAwait(continueOnCapturedContext: false);
		await PlatformManager.instance.RegisterPSI<IPlatformServiceIntegration>(PlatformSupport.kCreateDiscordRichPresence, m_Cts.Token).ConfigureAwait(continueOnCapturedContext: false);
		if (!(await PlatformManager.instance.Initialize(m_Cts.Token)))
		{
			log.ErrorFormat("A platform service integration failed to initialize", Array.Empty<object>());
			QuitGame();
		}
		EnvPath.UpdateSpecialPathCache();
		await AssetDatabase.global.RegisterDatabase((IAssetDatabase)(object)AssetDatabase<SteamCloud>.instance);
		await ContentHelper.RegisterContent();
	}

	private void UpdatePlatforms()
	{
		PlatformManager.instance.Update();
	}

	private async Task DisposePlatforms()
	{
		Task task = PlatformManager.instance.Dispose(true, CancellationToken.None);
		while (!task.IsCompleted)
		{
			Update();
			await Task.Delay(500);
		}
		await task;
	}

	private void ShowFallbackUI(Exception ex)
	{
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Expected O, but got Unknown
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Expected O, but got Unknown
		//IL_00aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_00af: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bd: Expected O, but got Unknown
		//IL_00cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e6: Unknown result type (might be due to invalid IL or missing references)
		//IL_010e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0118: Expected O, but got Unknown
		if (m_UIManager == null)
		{
			m_UIManager = new UIManager(false);
		}
		ErrorPage val = new ErrorPage();
		val.AddAction("quit", (Action)QuitGame);
		val.AddAction("visit", (Action)delegate
		{
			try
			{
				Process.Start(new ProcessStartInfo
				{
					FileName = "https://pdxint.at/3Do979W",
					UseShellExecute = true
				});
			}
			catch
			{
				QuitGame();
			}
		});
		val.SetStopCode(ex);
		val.SetRoot(EnvPath.kContentPath + "/Game/UI/.fatal", EnvPath.kContentPath + "/Game/.fatal");
		val.SetFonts(EnvPath.kContentPath + "/Game/UI/Fonts", EnvPath.kContentPath + "/Game/Fonts.cok");
		Settings val2 = Settings.New;
		((Settings)(ref val2)).resourceHandler = (IResourceHandler)new FatalResourceHandler(val);
		val2.enableDebugger = false;
		UISystem val3 = m_UIManager.CreateUISystem(val2);
		Settings val4 = Settings.New;
		val4.liveReload = true;
		val3.CreateView("fatal://error", val4, ((Component)this).GetComponent<Camera>()).enabled = true;
		if (m_UIInputSystem != null)
		{
			m_UIInputSystem.Dispose();
		}
		m_UIInputSystem = new UIInputSystem(val3, true);
		if (!shouldUpdateManager)
		{
			RegisterUpdater(delegate
			{
				m_UIInputSystem.DispatchInputEvents(true);
				m_UIManager.Update();
				return false;
			});
		}
	}

	private async Task InitializeUI()
	{
		m_Cts.Token.ThrowIfCancellationRequested();
		ILog uiLog = UIManager.log;
		try
		{
			uiLog.Info((object)"Bootstrapping cohtmlUISystem");
			m_UIManager = new UIManager(configuration.uiDeveloperMode);
			Settings val = Settings.New;
			((Settings)(ref val)).localizationManager = (ILocalizationManager)(object)new UILocalizationManager(localizationManager);
			((Settings)(ref val)).resourceHandler = (IResourceHandler)(object)new GameUIResourceHandler((MonoBehaviour)(object)this);
			UISystem val2 = m_UIManager.CreateUISystem(val);
			foreach (UIHostAsset asset in AssetDatabase.global.GetAssets<UIHostAsset>(default(SearchFilter<UIHostAsset>)))
			{
				if (asset.scheme == "assetdb")
				{
					val2.AddDatabaseHostLocation(asset.hostname, asset.uiUri, asset.priority);
				}
				else
				{
					val2.AddHostLocation(asset.hostname, asset.uiPath, true, asset.priority);
				}
			}
			m_UIInputSystem = new UIInputSystem(val2, true);
			userInterface = new UserInterface(m_UILocation, localizationManager, val2);
			m_World.GetOrCreateSystem<NotificationUISystem>();
			m_World.GetOrCreateSystem<OptionsUISystem>();
			settings.RegisterInOptionsUI();
			m_State = State.UIReady;
			InputManager.instance.CheckConflicts();
			log.DebugFormat("Time to UI {0}s", (object)Time.realtimeSinceStartup);
			await userInterface.WaitForBindings();
		}
		catch (Exception ex)
		{
			uiLog.Error(ex);
		}
	}

	private void CreateUISystems()
	{
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		foreach (Type item in ReflectionUtils.GetAllTypesDerivedFrom<UISystemBase>(true))
		{
			if (!item.IsAbstract)
			{
				m_World.GetOrCreateSystem(item);
			}
		}
	}

	private void UpdateUI()
	{
		m_UIManager.Update();
		userInterface.Update();
	}

	private void ReleaseUI()
	{
		userInterface?.Dispose();
		UIInputSystem uIInputSystem = m_UIInputSystem;
		if (uIInputSystem != null)
		{
			uIInputSystem.Dispose();
		}
		UIManager uIManager = m_UIManager;
		if (uIManager != null)
		{
			uIManager.Dispose();
		}
	}

	public Task SetScreenActive<T>() where T : IScreenState, new()
	{
		return new T().Execute(this, m_Cts.Token);
	}

	private void UpdateUpdaters()
	{
		foreach (KeyValuePair<Guid, Func<bool>> updater in m_Updaters)
		{
			if (updater.Value())
			{
				UnregisterUpdater(updater.Key);
			}
		}
	}

	public Guid RegisterUpdater(Action action)
	{
		return RegisterUpdater(delegate
		{
			action();
			return true;
		});
	}

	public Guid RegisterUpdater(Func<bool> func)
	{
		if (func != null)
		{
			Guid guid = Guid.NewGuid();
			m_Updaters.TryAdd(guid, func);
			log.DebugFormat("Updater {0} registered with guid {1}", (object)func.Method.Name, (object)GuidUtils.ToLowerNoDashString(guid));
			return guid;
		}
		return Guid.Empty;
	}

	public bool UnregisterUpdater(Guid guid)
	{
		if (m_Updaters.TryRemove(guid, out var value))
		{
			log.DebugFormat("Updater {0} with {1} unregistered", (object)GuidUtils.ToLowerNoDashString(guid), (object)value.Method.Name);
			return true;
		}
		log.DebugFormat("Updater {0} was not found", Array.Empty<object>());
		return false;
	}

	public string[] GetAvailablePrerequisitesNames()
	{
		return m_PrefabSystem.GetAvailablePrerequisitesNames();
	}

	public bool ArePrerequisitesMet(string[] contentPrerequisites)
	{
		if (contentPrerequisites == null)
		{
			return true;
		}
		foreach (string name in contentPrerequisites)
		{
			if (!m_PrefabSystem.TryGetPrefab(new PrefabID("ContentPrefab", name), out var prefab) || !((ContentPrefab)prefab).IsAvailable())
			{
				return false;
			}
		}
		return true;
	}

	public bool ArePrerequisitesMet<T>(Metadata<T> meta) where T : IContentPrerequisite
	{
		string[] contentPrerequisites = meta.target.contentPrerequisites;
		return ArePrerequisitesMet(contentPrerequisites);
	}

	private void ReportBootProgress(float progress)
	{
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		TaskProgress progress2 = TaskManager.instance.progress;
		ProgressTracker val = default(ProgressTracker);
		((ProgressTracker)(ref val))._002Ector("Boot", (Group)3, false);
		((ProgressTracker)(ref val)).progress = progress;
		progress2.Report(val);
	}

	private void NotifyProgress(string identifier, int progress)
	{
		string titleId = identifier;
		string textId = identifier;
		ProgressState? progressState = (ProgressState)1;
		int? progress2 = progress;
		NotificationSystem.Push(identifier, null, null, titleId, textId, null, progressState, progress2);
		if (progress >= 100)
		{
			textId = identifier;
			titleId = identifier;
			progressState = (ProgressState)3;
			progress2 = progress;
			NotificationSystem.Pop(identifier, 2f, null, null, textId, titleId, null, progressState, progress2);
		}
	}

	private void EnableMemoryLeaksDetection()
	{
		NativeLeakDetection.Mode = (NativeLeakDetectionMode)1;
	}

	public void RunOnMainThread(Action action)
	{
		if (isMainThread)
		{
			action();
		}
		else
		{
			RegisterUpdater(action);
		}
	}

	private void InitializeThumbnails()
	{
		m_Cts.Token.ThrowIfCancellationRequested();
		thumbnailCache = new ThumbnailCache();
		thumbnailCache.Initialize();
	}

	private void DisposeThumbnails()
	{
		thumbnailCache?.Dispose();
	}

	private Task LoadUnityPrefabs()
	{
		return LoadAssetLibraryAsync();
	}

	private void LoadPrefabs()
	{
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		int count = 0;
		PerformanceCounter val = PerformanceCounter.Start((Action<TimeSpan>)delegate(TimeSpan t)
		{
			log.InfoFormat("Loaded {1} prefabs in {0}s", (object)t.TotalSeconds, (object)count);
		});
		try
		{
			foreach (PrefabAsset asset in AssetDatabase.global.GetAssets<PrefabAsset>(default(SearchFilter<PrefabAsset>)))
			{
				if (asset.Load() is PrefabBase prefab)
				{
					m_PrefabSystem.AddPrefab(prefab);
					count++;
				}
			}
		}
		finally
		{
			((IDisposable)val)?.Dispose();
		}
	}

	private async Task<AssetLibrary> LoadAssetLibraryAsync()
	{
		PerformanceCounter val = PerformanceCounter.Start((Action<TimeSpan>)delegate(TimeSpan t)
		{
			log.InfoFormat("LoadAssetLibraryAsync performed in {0}ms", (object)t.TotalMilliseconds);
		});
		try
		{
			ResourceRequest asyncLoad = Resources.LoadAsync<AssetLibrary>("GameAssetLibrary");
			while (!((AsyncOperation)asyncLoad).isDone)
			{
				m_Cts.Token.ThrowIfCancellationRequested();
				await Task.Yield();
			}
			((AssetLibrary)(object)asyncLoad.asset).Load(m_PrefabSystem, m_Cts.Token);
			return asyncLoad.asset as AssetLibrary;
		}
		finally
		{
			((IDisposable)val)?.Dispose();
		}
	}

	private void DisableCameraRendering()
	{
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		Camera main = Camera.main;
		if ((Object)(object)main != (Object)null)
		{
			m_DefaultCullingMask = LayerMask.op_Implicit(main.cullingMask);
			main.cullingMask = 0;
			HDAdditionalCameraData component = ((Component)main).GetComponent<HDAdditionalCameraData>();
			if ((Object)(object)component != (Object)null)
			{
				m_DefaultVolumeLayerMask = component.volumeLayerMask;
				component.volumeLayerMask = LayerMask.op_Implicit(0);
			}
		}
	}

	private void EnableCameraRendering()
	{
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		m_Cts.Token.ThrowIfCancellationRequested();
		Camera main = Camera.main;
		if ((Object)(object)main != (Object)null)
		{
			main.cullingMask = LayerMask.op_Implicit(m_DefaultCullingMask);
			HDAdditionalCameraData component = ((Component)main).GetComponent<HDAdditionalCameraData>();
			if ((Object)(object)component != (Object)null)
			{
				component.volumeLayerMask = m_DefaultVolumeLayerMask;
			}
		}
	}

	[DllImport("user32.dll")]
	private static extern bool SetWindowText(IntPtr hWnd, string lpString);

	[DllImport("user32.dll", CharSet = CharSet.Auto)]
	private static extern IntPtr FindWindow(string strClassName, string strWindowName);

	[DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
	private static extern int GetWindowText(IntPtr hWnd, StringBuilder lpString, int nMaxCount);

	[RuntimeInitializeOnLoadMethod]
	private static void SetWindowsTitle()
	{
		IntPtr intPtr = FindWindow(null, Application.productName);
		if (intPtr != IntPtr.Zero)
		{
			SetWindowText(intPtr, "Cities: Skylines II");
		}
	}

	private void InitConsole()
	{
		if (configuration.captureStdout != Configuration.StdoutCaptureMode.None)
		{
			if (configuration.captureStdout != Configuration.StdoutCaptureMode.Redirect)
			{
				m_Console = new ConsoleWindow(Application.productName, configuration.captureStdout == Configuration.StdoutCaptureMode.Console);
			}
			LogManager.stdOutActive = true;
			log.Info((object)"\u001b[1m\u001b[38;2;0;135;215mWelcome to Cities: Skylines II\u001b[0m");
			log.Info((object)"\u001b[1m\u001b[38;2;0;135;215mColossal Order Oy - 2023\u001b[0m");
			Thread.Sleep(1000);
		}
	}

	private void ReleaseConsole()
	{
		m_Console?.Dispose();
	}

	private void TryCatchUnhandledExceptions()
	{
		TaskScheduler.UnobservedTaskException += delegate(object sender, UnobservedTaskExceptionEventArgs e)
		{
			e.SetObserved();
			log.Critical((Exception)e.Exception, (object)"Unobserved exception triggered");
		};
		AppDomain.CurrentDomain.UnhandledException += delegate(object sender, UnhandledExceptionEventArgs e)
		{
			Exception ex = (Exception)e.ExceptionObject;
			log.Critical(ex, (object)"Unhandled domain exception triggered");
		};
	}

	private bool CheckValidity()
	{
		try
		{
			_ = ((Behaviour)instance).enabled;
		}
		catch (MissingReferenceException)
		{
			((Behaviour)this).enabled = false;
			Object.Destroy((Object)(object)((Component)this).gameObject);
			QuitGame();
			return false;
		}
		catch
		{
		}
		if ((Object)(object)instance != (Object)null && (Object)(object)instance != (Object)(object)this)
		{
			((Behaviour)this).enabled = false;
			Object.Destroy((Object)(object)((Component)this).gameObject);
			return false;
		}
		return true;
	}

	public static string GetVersionsInfo()
	{
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_00aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_00af: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00eb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0179: Unknown result type (might be due to invalid IL or missing references)
		//IL_017e: Unknown result type (might be due to invalid IL or missing references)
		StringBuilder stringBuilder = new StringBuilder();
		string text = null;
		text = "Mono";
		stringBuilder.AppendLine($"Date: {DateTime.UtcNow}");
		Version val = Version.current;
		stringBuilder.AppendLine($"Game version: {((Version)(ref val)).fullVersion} {PlatformExt.ToPlatform(Application.platform)} {PlatformManager.instance.principalPlatformName}");
		stringBuilder.AppendLine("Game configuration: " + (Debug.isDebugBuild ? "Development" : "Release") + " (" + text + ")");
		val = Version.current;
		stringBuilder.AppendLine("COre version: " + ((Version)(ref val)).fullVersion);
		val = Version.current;
		stringBuilder.AppendLine("Localization version: " + ((Version)(ref val)).fullVersion);
		val = Version.current;
		stringBuilder.AppendLine("UI version: " + ((Version)(ref val)).fullVersion);
		stringBuilder.AppendLine("Unity version: " + Application.unityVersion);
		stringBuilder.AppendLine($"Cohtml version: {Versioning.Build}");
		stringBuilder.AppendLine("ATL Version: " + Version.getVersion());
		PlatformManager.instance.LogVersion(stringBuilder);
		foreach (IDlc item in PlatformManager.instance.EnumerateLocalDLCs())
		{
			string text2 = StringUtils.Nicify(item.internalName);
			val = item.version;
			stringBuilder.AppendLine(text2 + ": " + ((Version)(ref val)).fullVersion);
		}
		if (Application.genuineCheckAvailable)
		{
			stringBuilder.AppendLine($"Genuine: {Application.genuine}");
		}
		return stringBuilder.ToString().TrimEnd();
	}

	public static string GetSystemInfoString()
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f7: Unknown result type (might be due to invalid IL or missing references)
		//IL_0213: Unknown result type (might be due to invalid IL or missing references)
		//IL_0218: Unknown result type (might be due to invalid IL or missing references)
		//IL_021c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0221: Unknown result type (might be due to invalid IL or missing references)
		//IL_0294: Unknown result type (might be due to invalid IL or missing references)
		//IL_0299: Unknown result type (might be due to invalid IL or missing references)
		StringBuilder stringBuilder = new StringBuilder();
		stringBuilder.AppendLine("Type: " + ((object)SystemInfo.deviceType/*cast due to .constrained prefix*/).ToString());
		stringBuilder.AppendLine("OS: " + SystemInfo.operatingSystem);
		stringBuilder.AppendLine("System memory: " + FormatUtils.FormatBytes((long)SystemInfo.systemMemorySize * 1024L * 1024));
		stringBuilder.AppendLine("Graphics device: " + SystemInfo.graphicsDeviceName + " (Version: " + SystemInfo.graphicsDeviceVersion + ")");
		stringBuilder.AppendLine("Graphics memory: " + FormatUtils.FormatBytes((long)SystemInfo.graphicsMemorySize * 1024L * 1024));
		stringBuilder.AppendLine("Max texture size: " + SystemInfo.maxTextureSize);
		stringBuilder.AppendLine("Shader level: " + SystemInfo.graphicsShaderLevel);
		stringBuilder.AppendLine("3D textures: " + SystemInfo.supports3DTextures);
		stringBuilder.AppendLine("Shadows: " + SystemInfo.supportsShadows);
		stringBuilder.AppendLine("Compute: " + SystemInfo.supportsComputeShaders);
		stringBuilder.AppendLine("CPU: " + SystemInfo.processorType);
		stringBuilder.AppendLine("Core count: " + SystemInfo.processorCount);
		stringBuilder.AppendLine("Platform: " + ((object)Application.platform/*cast due to .constrained prefix*/).ToString());
		string[] obj = new string[6] { "Screen resolution: ", null, null, null, null, null };
		Resolution currentResolution = Screen.currentResolution;
		obj[1] = ((Resolution)(ref currentResolution)).width.ToString();
		obj[2] = "x";
		currentResolution = Screen.currentResolution;
		obj[3] = ((Resolution)(ref currentResolution)).height.ToString();
		obj[4] = "x";
		currentResolution = Screen.currentResolution;
		RefreshRate refreshRateRatio = ((Resolution)(ref currentResolution)).refreshRateRatio;
		obj[5] = ((int)((RefreshRate)(ref refreshRateRatio)).value).ToString();
		stringBuilder.AppendLine(string.Concat(obj));
		stringBuilder.AppendLine("Window resolution: " + Screen.width + "x" + Screen.height);
		stringBuilder.AppendLine("DPI: " + Screen.dpi);
		stringBuilder.AppendLine("Rendering Threading Mode: " + ((object)SystemInfo.renderingThreadingMode/*cast due to .constrained prefix*/).ToString());
		stringBuilder.AppendLine("CLR: " + Environment.Version);
		stringBuilder.AppendLine("Modding runtime: " + s_ModdingRuntime);
		Type type = Type.GetType("Mono.Runtime");
		if (type != null)
		{
			MethodInfo method = type.GetMethod("GetDisplayName", BindingFlags.Static | BindingFlags.NonPublic);
			if (method != null)
			{
				stringBuilder.AppendLine("Scripting runtime: Mono " + method.Invoke(null, null));
			}
		}
		return stringBuilder.ToString().TrimEnd();
	}

	private static Dictionary<string, string> GetDefaultBacktraceAttributes()
	{
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		Dictionary<string, string> dictionary = new Dictionary<string, string>();
		Version current = Version.current;
		dictionary["game.version"] = ((Version)(ref current)).fullVersion;
		dictionary["cohtml.version"] = Versioning.Build.ToString();
		dictionary["pdxsdk.version"] = SDKVersion.Version;
		dictionary["atl.version"] = Version.getVersion();
		dictionary["game.moddingRuntime"] = s_ModdingRuntime;
		return dictionary;
	}

	private static void ListHarmonyPatches()
	{
		ILog logger = LogManager.GetLogger("Modding");
		logger.InfoFormat("Modding runtime: {0}", (object)s_ModdingRuntime);
		try
		{
			LocalTypeCache localTypeCache = new LocalTypeCache();
			Assembly assembly = AppDomain.CurrentDomain.GetAssemblies().FirstOrDefault((Assembly a) => a.GetName().Name.Contains("Harmony"));
			if (assembly == null)
			{
				return;
			}
			log.Info((object)"Harmony found.");
			Type type = assembly.GetType("Harmony.HarmonyInstance", throwOnError: false) ?? assembly.GetType("HarmonyLib.Harmony", throwOnError: false);
			if (type == null)
			{
				logger.Info((object)"HarmonyInstance/Harmony class not found.");
				return;
			}
			MethodInfo method = localTypeCache.GetMethod(type, "GetAllPatchedMethods", BindingFlags.Static | BindingFlags.Public);
			if (method == null)
			{
				logger.Info((object)"Method GetAllPatchedMethods not found.");
				return;
			}
			if (!(method.Invoke(null, null) is IEnumerable<MethodBase> enumerable))
			{
				logger.Info((object)"No patched methods found.");
				return;
			}
			MethodInfo method2 = localTypeCache.GetMethod(type, "GetPatchInfo", BindingFlags.Static | BindingFlags.Public);
			if (method2 == null)
			{
				logger.Info((object)"Method GetPatchInfo not found.");
				return;
			}
			Type type2 = assembly.GetType("HarmonyLib.Patches", throwOnError: false);
			if (type2 == null)
			{
				logger.Info((object)"Patches class not found.");
				return;
			}
			foreach (MethodBase item in enumerable)
			{
				logger.InfoFormat("Patched Method: {0}.{1}", (object)(item.DeclaringType?.FullName ?? "<Global Type>"), (object)item.Name);
				object patchInfo = method2.Invoke(null, new object[1] { item });
				PrintPatchDetails(logger, patchInfo, type2, localTypeCache);
			}
		}
		catch (Exception ex)
		{
			log.Warn(ex, (object)"ListHarmonyPatches failed");
		}
	}

	private static void PrintPatchDetails(ILog moddingLog, object patchInfo, Type patchInfoType, LocalTypeCache typeCache)
	{
		if (patchInfo != null)
		{
			FieldInfo field = typeCache.GetField(patchInfoType, "Prefixes");
			FieldInfo field2 = typeCache.GetField(patchInfoType, "Postfixes");
			FieldInfo field3 = typeCache.GetField(patchInfoType, "Transpilers");
			FieldInfo field4 = typeCache.GetField(patchInfoType, "Finalizers");
			IEnumerable<object> patches = field?.GetValue(patchInfo) as IEnumerable<object>;
			IEnumerable<object> patches2 = field2?.GetValue(patchInfo) as IEnumerable<object>;
			IEnumerable<object> patches3 = field3?.GetValue(patchInfo) as IEnumerable<object>;
			IEnumerable<object> patches4 = field4?.GetValue(patchInfo) as IEnumerable<object>;
			PrintIndividualPatches(moddingLog, "Prefixes", patches, typeCache);
			PrintIndividualPatches(moddingLog, "Postfixes", patches2, typeCache);
			PrintIndividualPatches(moddingLog, "Transpilers", patches3, typeCache);
			PrintIndividualPatches(moddingLog, "Finalizers", patches4, typeCache);
		}
	}

	private static void PrintIndividualPatches(ILog moddingLog, string patchType, IEnumerable<object> patches, LocalTypeCache typeCache)
	{
		if (patches == null || !patches.Any())
		{
			return;
		}
		moddingLog.InfoFormat(" {0}:", (object)patchType);
		Indent scoped = moddingLog.indent.scoped;
		try
		{
			foreach (object patch in patches)
			{
				MethodBase methodBase = typeCache.GetProperty(patch.GetType(), "PatchMethod").GetValue(patch, null) as MethodBase;
				if (methodBase != null)
				{
					string text = methodBase.DeclaringType?.FullName ?? "<Global Method>";
					moddingLog.InfoFormat("Patch Method: {0}.{1}", (object)text, (object)methodBase.Name);
				}
			}
		}
		finally
		{
			((IDisposable)scoped)?.Dispose();
		}
	}

	private static void DetectModdingRuntime()
	{
		s_ModdingRuntime = DetectModdingRuntimeName();
	}

	private static string DetectModdingRuntimeName()
	{
		try
		{
			Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();
			Assembly[] array = assemblies;
			foreach (Assembly assembly in array)
			{
				if (assembly.GetName().Name.Equals("BepInEx", StringComparison.OrdinalIgnoreCase))
				{
					return $"{assembly.GetName().Name} {assembly.GetName().Version}";
				}
			}
			array = assemblies;
			foreach (Assembly assembly2 in array)
			{
				if (assembly2.GetName().Name.Contains("BepInEx", StringComparison.OrdinalIgnoreCase))
				{
					return $"{assembly2.GetName().Name} {assembly2.GetName().Version}";
				}
				if (assembly2.GetTypes().Any((Type t) => t.Namespace != null && t.Namespace.StartsWith("BepInEx")))
				{
					return $"{assembly2.GetName().Name} {assembly2.GetName().Version}";
				}
			}
			return "Builtin";
		}
		catch
		{
			return "Unknown";
		}
	}

	private static void SetNativeStackTrace()
	{
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		Application.SetStackTraceLogType((LogType)1, (StackTraceLogType)2);
		Application.SetStackTraceLogType((LogType)0, (StackTraceLogType)2);
		Application.SetStackTraceLogType((LogType)4, (StackTraceLogType)2);
		Application.SetStackTraceLogType((LogType)3, (StackTraceLogType)2);
		Application.SetStackTraceLogType((LogType)2, (StackTraceLogType)2);
		Version current = Version.current;
		Debug.Log((object)("Game version: " + ((Version)(ref current)).fullVersion));
		Debug.Log((object)GetSystemInfoString());
	}

	private bool HandleConfiguration()
	{
		if (configuration.showHelp != null)
		{
			Console.WriteLine(configuration.showHelp);
			return false;
		}
		if (!string.IsNullOrEmpty(configuration.profilerTarget))
		{
			Profiler.logFile = configuration.profilerTarget;
			Profiler.enableBinaryLog = true;
			Profiler.enabled = true;
		}
		return true;
	}

	private void HandleUserFolderVersion()
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_007a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		try
		{
			Version current = Version.current;
			string text = EnvPath.kUserDataPath + "/version";
			if (LongFile.Exists(text))
			{
				((Version)(ref current))._002Ector(LongFile.ReadAllText(text));
			}
			Version current2;
			if (current < Version.current)
			{
				string[] obj = new string[5]
				{
					"Persistent folder version is outdated ",
					((Version)(ref current)).fullVersion,
					" (Game: ",
					null,
					null
				};
				current2 = Version.current;
				obj[3] = ((Version)(ref current2)).fullVersion;
				obj[4] = ")";
				Debug.Log((object)string.Concat(obj));
				if (current < new Version("1.0.6f2"))
				{
					Debug.Log((object)"User settings deleted due to outdated persistent folder version. Backups were created ending with ~");
					DeleteSettings(EnvPath.kUserDataPath);
					PlayerPrefs.DeleteAll();
					PlayerPrefs.Save();
				}
			}
			current2 = Version.current;
			LongFile.WriteAllText(text, ((Version)(ref current2)).fullVersion);
		}
		catch (Exception ex)
		{
			log.Error(ex);
		}
		static void DeleteSettings(string settingsPath)
		{
			foreach (FileInfo item in new DirectoryInfo(settingsPath).EnumerateFiles("*", SearchOption.AllDirectories))
			{
				if (item.Extension.ToLower() == ".coc")
				{
					string text2 = Path.ChangeExtension(item.FullName, ".coc~");
					LongFile.Delete(text2);
					item.MoveTo(text2);
				}
			}
		}
	}

	private void InitializeLocalization()
	{
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Expected O, but got Unknown
		m_Cts.Token.ThrowIfCancellationRequested();
		localizationManager = new LocalizationManager("en-US", (SystemLanguage)10, "English");
		localizationManager.LoadAvailableLocales();
	}

	private void CreateWorld()
	{
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Expected O, but got Unknown
		m_Cts.Token.ThrowIfCancellationRequested();
		log.Info((object)"Creating ECS world");
		CORuntimeApplication.Initialize();
		m_World = new World("Game", (WorldFlags)9);
		World.DefaultGameObjectInjectionWorld = m_World;
		m_PrefabSystem = m_World.GetOrCreateSystemManaged<PrefabSystem>();
		m_UpdateSystem = m_World.GetOrCreateSystemManaged<UpdateSystem>();
		m_DeserializationSystem = m_World.GetOrCreateSystemManaged<LoadGameSystem>();
		m_SerializationSystem = m_World.GetOrCreateSystemManaged<SaveGameSystem>();
	}

	private void CreateSystems()
	{
		m_Cts.Token.ThrowIfCancellationRequested();
		log.Info((object)"Creating ECS systems");
		SystemOrder.Initialize(m_UpdateSystem);
		userInterface.view.AudioSource = AudioManager.instance.UIHtmlAudioSource;
		Telemetry.gameplayData = new Telemetry.GameplayData(m_World);
	}

	private void UpdateWorld()
	{
		if (shouldUpdateWorld)
		{
			CORuntimeApplication.ResetUpdateAllocator(m_World);
			m_UpdateSystem.Update(SystemUpdatePhase.MainLoop);
		}
	}

	private void PostUpdateWorld()
	{
		if (shouldUpdateWorld)
		{
			m_UpdateSystem.Update(SystemUpdatePhase.Cleanup);
		}
	}

	private void LateUpdateWorld()
	{
		if (shouldUpdateWorld)
		{
			m_UpdateSystem.Update(SystemUpdatePhase.LateUpdate);
			m_UpdateSystem.Update(SystemUpdatePhase.DebugGizmos);
			CORuntimeApplication.Update();
		}
	}

	private void DestroyWorld()
	{
		Telemetry.gameplayData = null;
		World.DisposeAllWorlds();
		CORuntimeApplication.Shutdown();
		m_State = State.WorldDisposed;
	}

	public void TakeScreenshot()
	{
		((MonoBehaviour)this).StartCoroutine(CaptureScreenshot());
	}

	private IEnumerator CaptureScreenshot()
	{
		yield return (object)new WaitForEndOfFrame();
		ScreenUtility.CaptureScreenshot((string)null, 1);
	}

	Coroutine ICoroutineHost.StartCoroutine(IEnumerator routine)
	{
		return ((MonoBehaviour)this).StartCoroutine(routine);
	}
}

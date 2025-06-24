using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Colossal;
using Colossal.Annotations;
using Colossal.IO.AssetDatabase;
using Colossal.UI.Binding;
using Game.Assets;
using Game.Rendering.Utilities;
using Game.SceneFlow;
using Game.Settings;
using Game.UI.Debug;
using Game.UI.Menu;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Experimental.Rendering;
using UnityEngine.Rendering;
using UnityEngine.Rendering.HighDefinition;

namespace Game.UI;

public class AppBindings : CompositeBinding, IDisposable
{
	private struct FrameTiming : IJsonWritable
	{
		private FrameTimeSampleHistory m_History;

		private GeneralSettings m_Settings;

		private DebugUISystem m_DebugUISystem;

		public float fps;

		public float fullFameTime;

		public float cpuMainThreadTime;

		public float cpuRenderThreadTime;

		public float gpuTime;

		public void Update()
		{
			if (m_Settings == null)
			{
				m_Settings = SharedSettings.instance?.general;
			}
			if (m_Settings != null)
			{
				switch (m_Settings.fpsMode)
				{
				case GeneralSettings.FPSMode.Simple:
					fps = math.max(1f / Time.smoothDeltaTime, 0f);
					break;
				case GeneralSettings.FPSMode.Advanced:
					fps = math.max(1f / Time.smoothDeltaTime, 0f);
					fullFameTime = 1000f / fps;
					break;
				case GeneralSettings.FPSMode.Precise:
					if (m_History == null)
					{
						HDRenderPipeline currentPipeline = HDRenderPipeline.currentPipeline;
						m_History = ((currentPipeline == null) ? null : currentPipeline.debugDisplaySettings?.debugFrameTiming?.m_FrameHistory);
					}
					if (m_History != null)
					{
						fps = m_History.SampleAverage.FramesPerSecond;
						fullFameTime = m_History.SampleAverage.FullFrameTime;
						cpuMainThreadTime = m_History.SampleAverage.MainThreadCPUFrameTime;
						cpuRenderThreadTime = m_History.SampleAverage.RenderThreadCPUFrameTime;
						gpuTime = m_History.SampleAverage.GPUFrameTime;
						DebugManager.instance.externalDebugUIActive = true;
					}
					break;
				}
			}
			if (m_DebugUISystem == null)
			{
				World defaultGameObjectInjectionWorld = World.DefaultGameObjectInjectionWorld;
				m_DebugUISystem = ((defaultGameObjectInjectionWorld != null) ? defaultGameObjectInjectionWorld.GetOrCreateSystemManaged<DebugUISystem>() : null);
			}
			DebugManager instance = DebugManager.instance;
			instance.externalDebugUIActive |= m_DebugUISystem?.visible ?? false;
		}

		public void Dispose()
		{
			m_History = null;
			m_Settings = null;
			m_DebugUISystem = null;
		}

		public void Write(IJsonWriter writer)
		{
			writer.TypeBegin(GetType().FullName);
			writer.PropertyName("fps");
			writer.Write(fps);
			writer.PropertyName("fullFameTime");
			writer.Write(fullFameTime);
			writer.PropertyName("cpuMainThreadTime");
			writer.Write(cpuMainThreadTime);
			writer.PropertyName("cpuRenderThreadTime");
			writer.Write(cpuRenderThreadTime);
			writer.PropertyName("gpuTime");
			writer.Write(gpuTime);
			writer.TypeEnd();
		}
	}

	private const string kGroup = "app";

	public const string kBodyClassNames = "";

	private ValueBinding<string> m_BackgroundProcessMessageBinding;

	private EventBinding<ConfirmationDialogBase> m_ConfirmationDialogBinding;

	private ValueBinding<HashSet<string>> m_ActiveUIModsLocation;

	private GetterValueBinding<SaveInfo> m_CanContinueBinding;

	private ValueBinding<string[]> m_OwnedPrerequisites;

	private EventBinding m_CheckContinueGamePrerequisites;

	private Action<int> m_ConfirmationDialogCallback;

	private Action<int, bool> m_DismissibleConfirmationDialogCallback;

	private DebugUISystem m_DebugUISystem;

	private static FrameTiming m_FrameTiming;

	public bool ready { get; set; }

	public string activeUI { get; set; }

	private float GetFPS()
	{
		GeneralSettings generalSettings = SharedSettings.instance?.general;
		if (generalSettings != null && generalSettings.fpsMode == GeneralSettings.FPSMode.Precise)
		{
			HDRenderPipeline currentPipeline = HDRenderPipeline.currentPipeline;
			return ((currentPipeline == null) ? ((float?)null) : currentPipeline.debugDisplaySettings?.debugFrameTiming?.m_FrameHistory?.SampleAverage.FramesPerSecond) ?? (1f / Time.smoothDeltaTime);
		}
		return 1f / Time.smoothDeltaTime;
	}

	private float GetFullFrameTime()
	{
		HDRenderPipeline currentPipeline = HDRenderPipeline.currentPipeline;
		return ((currentPipeline == null) ? ((float?)null) : currentPipeline.debugDisplaySettings?.debugFrameTiming?.m_FrameHistory?.SampleAverage.FullFrameTime).GetValueOrDefault();
	}

	private float GetCPUMainThreadTime()
	{
		HDRenderPipeline currentPipeline = HDRenderPipeline.currentPipeline;
		return ((currentPipeline == null) ? ((float?)null) : currentPipeline.debugDisplaySettings?.debugFrameTiming?.m_FrameHistory?.SampleAverage.MainThreadCPUFrameTime).GetValueOrDefault();
	}

	private float GetCPURenderThreadTime()
	{
		HDRenderPipeline currentPipeline = HDRenderPipeline.currentPipeline;
		return ((currentPipeline == null) ? ((float?)null) : currentPipeline.debugDisplaySettings?.debugFrameTiming?.m_FrameHistory?.SampleAverage.RenderThreadCPUFrameTime).GetValueOrDefault();
	}

	private float GetGPUTime()
	{
		HDRenderPipeline currentPipeline = HDRenderPipeline.currentPipeline;
		return ((currentPipeline == null) ? ((float?)null) : currentPipeline.debugDisplaySettings?.debugFrameTiming?.m_FrameHistory?.SampleAverage.GPUFrameTime).GetValueOrDefault();
	}

	public void SetMainMenuActive()
	{
		activeUI = "Menu";
	}

	public void SetGameActive()
	{
		activeUI = "Game";
	}

	public void SetEditorActive()
	{
		activeUI = "Editor";
	}

	public void SetNoneActive()
	{
		activeUI = null;
	}

	public AppBindings()
	{
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Expected O, but got Unknown
		//IL_0168: Unknown result type (might be due to invalid IL or missing references)
		//IL_0172: Expected O, but got Unknown
		//IL_01be: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c8: Expected O, but got Unknown
		//IL_01df: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e9: Expected O, but got Unknown
		//IL_0200: Unknown result type (might be due to invalid IL or missing references)
		//IL_020a: Expected O, but got Unknown
		//IL_0221: Unknown result type (might be due to invalid IL or missing references)
		//IL_022b: Expected O, but got Unknown
		//IL_0392: Unknown result type (might be due to invalid IL or missing references)
		//IL_0397: Unknown result type (might be due to invalid IL or missing references)
		//IL_039a: Expected O, but got Unknown
		//IL_039f: Expected O, but got Unknown
		ErrorDialogManager.Initialize();
		((CompositeBinding)this).AddUpdateBinding((IUpdateBinding)(object)new GetterValueBinding<bool>("app", "ready", (Func<bool>)(() => ready), (IWriter<bool>)null, (EqualityComparer<bool>)null));
		((CompositeBinding)this).AddUpdateBinding((IUpdateBinding)(object)new GetterValueBinding<string>("app", "activeUI", (Func<string>)(() => activeUI), (IWriter<string>)(object)ValueWriters.Nullable<string>((IWriter<string>)new StringWriter()), (EqualityComparer<string>)null));
		((CompositeBinding)this).AddBinding((IBinding)(object)new ValueBinding<string>("app", "bodyClassNames", "", (IWriter<string>)null, (EqualityComparer<string>)null));
		((CompositeBinding)this).AddUpdateBinding((IUpdateBinding)(object)new GetterValueBinding<int>("app", "fpsMode", (Func<int>)(() => (int)(SharedSettings.instance?.general.fpsMode ?? GeneralSettings.FPSMode.Off)), (IWriter<int>)null, (EqualityComparer<int>)null));
		((CompositeBinding)this).AddUpdateBinding((IUpdateBinding)(object)new GetterValueBinding<FrameTiming>("app", "frameStats", (Func<FrameTiming>)(() => m_FrameTiming), (IWriter<FrameTiming>)(object)new ValueWriter<FrameTiming>(), (EqualityComparer<FrameTiming>)null));
		((CompositeBinding)this).AddUpdateBinding((IUpdateBinding)(object)new GetterValueBinding<string>("app", "activeLocale", (Func<string>)(() => GameManager.instance.localizationManager.activeDictionary.localeID), (IWriter<string>)null, (EqualityComparer<string>)null));
		((CompositeBinding)this).AddUpdateBinding((IUpdateBinding)(object)new GetterValueBinding<ErrorDialog>("app", "currentError", (Func<ErrorDialog>)(() => ErrorDialogManager.currentErrorDialog), (IWriter<ErrorDialog>)(object)ValueWriters.Nullable<ErrorDialog>((IWriter<ErrorDialog>)(object)new ValueWriter<ErrorDialog>()), (EqualityComparer<ErrorDialog>)null));
		((CompositeBinding)this).AddBinding((IBinding)(object)(m_BackgroundProcessMessageBinding = new ValueBinding<string>("app", "backgroundProcessMessage", (string)null, (IWriter<string>)(object)ValueWriters.Nullable<string>((IWriter<string>)new StringWriter()), (EqualityComparer<string>)null)));
		((CompositeBinding)this).AddBinding((IBinding)(object)new TriggerBinding<string>("app", "setClipboard", (Action<string>)SetClipboard, (IReader<string>)null));
		((CompositeBinding)this).AddBinding((IBinding)new TriggerBinding("app", "exitApplication", (Action)ExitApplication));
		((CompositeBinding)this).AddBinding((IBinding)new TriggerBinding("app", "saveBackupAndExitApplication", (Action)SaveBackupAndExitApplication));
		((CompositeBinding)this).AddBinding((IBinding)new TriggerBinding("app", "saveBackup", (Action)SaveBackup));
		((CompositeBinding)this).AddBinding((IBinding)new TriggerBinding("app", "dismissCurrentError", (Action)DismissCurrentError));
		((CompositeBinding)this).AddBinding((IBinding)(object)(m_ConfirmationDialogBinding = new EventBinding<ConfirmationDialogBase>("app", "confirmationDialog", (IWriter<ConfirmationDialogBase>)(object)new ValueWriter<ConfirmationDialogBase>())));
		((CompositeBinding)this).AddBinding((IBinding)(object)new TriggerBinding<int>("app", "confirmationDialogCallback", (Action<int>)OnConfirmationDialogCallback, (IReader<int>)null));
		((CompositeBinding)this).AddBinding((IBinding)(object)new TriggerBinding<int, bool>("app", "dismissibleConfirmationDialogCallback", (Action<int, bool>)OnDismissibleConfirmationDialogCallback, (IReader<int>)null, (IReader<bool>)null));
		((CompositeBinding)this).AddBinding((IBinding)(object)(m_ActiveUIModsLocation = new ValueBinding<HashSet<string>>("app", "activeUIModsLocation", new HashSet<string>(), (IWriter<HashSet<string>>)(object)new CollectionWriter<string>((IWriter<string>)null), (EqualityComparer<HashSet<string>>)null)));
		((CompositeBinding)this).AddBinding((IBinding)(object)new GetterValueBinding<int>("app", "platform", (Func<int>)(() => (int)PlatformExt.ToPlatform(Application.platform)), (IWriter<int>)null, (EqualityComparer<int>)null));
		((CompositeBinding)this).AddBinding((IBinding)(object)(m_CanContinueBinding = new GetterValueBinding<SaveInfo>("app", "canContinueGame", (Func<SaveInfo>)GetLastSaveInfo, (IWriter<SaveInfo>)(object)ValueWriters.Nullable<SaveInfo>((IWriter<SaveInfo>)(object)new ValueWriter<SaveInfo>()), (EqualityComparer<SaveInfo>)null)));
		((CompositeBinding)this).AddBinding((IBinding)(object)(m_OwnedPrerequisites = new ValueBinding<string[]>("app", "ownedPrerequisites", (string[])null, (IWriter<string[]>)(object)new NullableWriter<string[]>((IWriter<string[]>)(object)new ArrayWriter<string>((IWriter<string>)null, false)), (EqualityComparer<string[]>)null)));
		((CompositeBinding)this).AddBinding((IBinding)(object)new CallBinding<string[], bool>("app", "arePrerequisitesMet", (Func<string[], bool>)GameManager.instance.ArePrerequisitesMet, (IReader<string[]>)(object)new NullableReader<string[]>((IReader<string[]>)(object)new ArrayReader<string>((IReader<string>)null))));
		EventBinding val = new EventBinding("app", "checkContinueGamePrerequisites");
		EventBinding val2 = val;
		m_CheckContinueGamePrerequisites = val;
		((CompositeBinding)this).AddBinding((IBinding)(object)val2);
	}

	internal Task<bool> LauncherContinueGame()
	{
		m_CheckContinueGamePrerequisites.Trigger();
		return Task.FromResult(result: true);
	}

	public void UpdateCanContinueBinding()
	{
		m_CanContinueBinding.Update();
	}

	public void UpdateOwnedPrerequisiteBinding()
	{
		string[] availablePrerequisitesNames = GameManager.instance.GetAvailablePrerequisitesNames();
		m_OwnedPrerequisites.Update(availablePrerequisitesNames);
	}

	private SaveInfo GetLastSaveInfo()
	{
		SaveGameMetadata lastSaveGameMetadata = GameManager.instance.settings.userState.lastSaveGameMetadata;
		if ((AssetData)(object)lastSaveGameMetadata != (IAssetData)null && lastSaveGameMetadata.isValidSaveGame)
		{
			return ((Metadata<SaveInfo>)lastSaveGameMetadata).target;
		}
		return null;
	}

	public void UpdateActiveUIModsLocation(IList<string> locations)
	{
		HashSet<string> hashSet = new HashSet<string>(locations);
		m_ActiveUIModsLocation.Update(hashSet);
	}

	public void AddActiveUIModLocation(IList<string> locations)
	{
		int count = m_ActiveUIModsLocation.value.Count;
		foreach (string location in locations)
		{
			m_ActiveUIModsLocation.value.Add(location);
		}
		if (m_ActiveUIModsLocation.value.Count != count)
		{
			m_ActiveUIModsLocation.TriggerUpdate();
		}
	}

	public void RemoveActiveUIModLocation(IList<string> locations)
	{
		int count = m_ActiveUIModsLocation.value.Count;
		foreach (string location in locations)
		{
			m_ActiveUIModsLocation.value.Remove(location);
		}
		if (m_ActiveUIModsLocation.value.Count != count)
		{
			m_ActiveUIModsLocation.TriggerUpdate();
		}
	}

	public void Dispose()
	{
		ErrorDialogManager.Dispose();
		m_FrameTiming.Dispose();
	}

	public override bool Update()
	{
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		m_FrameTiming.Update();
		AdaptiveDynamicResolutionScale instance = AdaptiveDynamicResolutionScale.instance;
		DebugManager.instance.adaptiveDRSActive = instance.isEnabled && instance.isAdaptive;
		HDRenderPipeline currentPipeline = HDRenderPipeline.currentPipeline;
		DebugFrameTiming val = ((currentPipeline == null) ? null : currentPipeline.debugDisplaySettings?.debugFrameTiming);
		if (val != null)
		{
			FrameTimeSample sample = val.m_Sample;
			instance.UpdateDRS(sample.FullFrameTime, sample.MainThreadCPUFrameTime, sample.RenderThreadCPUFrameTime, sample.GPUFrameTime);
		}
		return ((CompositeBinding)this).Update();
	}

	private void ExitApplication()
	{
		GameManager.QuitGame();
	}

	private async void SaveBackupAndExitApplication()
	{
		await SaveBackupImpl();
		GameManager.QuitGame();
	}

	private async void SaveBackup()
	{
		await SaveBackupImpl();
	}

	private async Task SaveBackupImpl()
	{
		RenderTexture preview = ScreenCaptureHelper.CreateRenderTarget("PreviewSaveGame-Exit", 680, 383, (GraphicsFormat)8);
		ScreenCaptureHelper.CaptureScreenshot(Camera.main, preview, new MenuHelpers.SaveGamePreviewSettings());
		ScreenCaptureHelper.AsyncRequest request = new ScreenCaptureHelper.AsyncRequest((Texture)(object)preview);
		MenuUISystem existingSystemManaged = World.DefaultGameObjectInjectionWorld.GetExistingSystemManaged<MenuUISystem>();
		string saveName = "SaveRecovery" + DateTime.Now.ToString("dd-MMMM-HH-mm-ss");
		try
		{
			await GameManager.instance.Save(saveName, existingSystemManaged.GetSaveInfo(autoSave: false), AssetDatabase.user, request);
		}
		catch (Exception ex)
		{
			CompositeBinding.log.Error(ex);
		}
		finally
		{
			await request.Dispose();
			CoreUtils.Destroy((Object)(object)preview);
		}
	}

	private void SetClipboard(string text)
	{
		GUIUtility.systemCopyBuffer = text;
	}

	private void DismissCurrentError()
	{
		ErrorDialogManager.DismissCurrentErrorDialog();
	}

	public void ShowConfirmationDialog([NotNull] ConfirmationDialog dialog, [NotNull] Action<int> callback)
	{
		m_ConfirmationDialogCallback = callback;
		m_ConfirmationDialogBinding.Trigger((ConfirmationDialogBase)dialog);
	}

	public void ShowMessageDialog([NotNull] MessageDialog dialog, Action<int> callback)
	{
		m_ConfirmationDialogCallback = callback;
		m_ConfirmationDialogBinding.Trigger((ConfirmationDialogBase)dialog);
	}

	public void ShowConfirmationDialog([NotNull] DismissibleConfirmationDialog dialog, [NotNull] Action<int, bool> callback)
	{
		m_DismissibleConfirmationDialogCallback = callback;
		m_ConfirmationDialogBinding.Trigger((ConfirmationDialogBase)dialog);
	}

	private void OnConfirmationDialogCallback(int msg)
	{
		if (m_ConfirmationDialogCallback != null)
		{
			Action<int> confirmationDialogCallback = m_ConfirmationDialogCallback;
			m_ConfirmationDialogCallback = null;
			confirmationDialogCallback(msg);
		}
	}

	private void OnDismissibleConfirmationDialogCallback(int msg, bool dontShowAgain)
	{
		if (m_DismissibleConfirmationDialogCallback != null)
		{
			Action<int, bool> dismissibleConfirmationDialogCallback = m_DismissibleConfirmationDialogCallback;
			m_DismissibleConfirmationDialogCallback = null;
			dismissibleConfirmationDialogCallback(msg, dontShowAgain);
		}
	}
}

using System;
using System.Collections.Generic;
using Colossal.PSI.Common;
using Colossal.Serialization.Entities;
using Colossal.UI.Binding;
using Game.Input;
using Game.PSI;
using Game.SceneFlow;
using Game.Serialization;
using Game.Settings;
using Game.UI.Localization;
using UnityEngine;
using UnityEngine.Scripting;

namespace Game.UI.InGame;

public class GameScreenUISystem : UISystemBase, IPreDeserialize
{
	public enum GameScreen
	{
		Main = 0,
		FreeCamera = 1,
		PauseMenu = 10,
		SaveGame = 11,
		NewGame = 12,
		LoadGame = 13,
		Options = 14
	}

	private const string kSavingGameNotificationTitle = "SavingGame";

	private const string kGroup = "game";

	private ValueBinding<GameScreen> m_ActiveScreenBinding;

	private ValueBinding<bool> m_CanUseSaveSystem;

	public GameScreen activeScreen
	{
		get
		{
			return m_ActiveScreenBinding.value;
		}
		set
		{
			m_ActiveScreenBinding.Update(value);
		}
	}

	public bool isMenuActive => m_ActiveScreenBinding.value >= GameScreen.PauseMenu;

	[Preserve]
	protected override void OnCreate()
	{
		base.OnCreate();
		AddBinding((IBinding)(object)(m_ActiveScreenBinding = new ValueBinding<GameScreen>("game", "activeScreen", GameScreen.Main, (IWriter<GameScreen>)(object)new EnumWriter<GameScreen>(), (EqualityComparer<GameScreen>)null)));
		AddBinding((IBinding)(object)new TriggerBinding<GameScreen>("game", "setActiveScreen", (Action<GameScreen>)SetScreen, (IReader<GameScreen>)(object)new EnumReader<GameScreen>()));
		AddBinding((IBinding)(object)(m_CanUseSaveSystem = new ValueBinding<bool>("game", "canUseSaveSystem", true, (IWriter<bool>)null, (EqualityComparer<bool>)null)));
		GameManager.instance.onGameSaveLoad += SaveLoadInProgress;
	}

	[Preserve]
	protected override void OnDestroy()
	{
		GameManager.instance.onGameSaveLoad -= SaveLoadInProgress;
		base.OnDestroy();
	}

	private void SaveLoadInProgress(string name, bool start)
	{
		if (start)
		{
			string identifier = "SavingGame" + name;
			LocalizedString? text = LocalizedString.Value(name);
			ProgressState? progressState = (ProgressState)2;
			NotificationSystem.Push(identifier, null, text, "SavingGame", null, null, progressState);
		}
		else
		{
			string identifier2 = "SavingGame" + name;
			LocalizedString? text = LocalizedString.Value(name);
			ProgressState? progressState = (ProgressState)3;
			NotificationSystem.Pop(identifier2, 1f, null, text, "SavingGame", null, null, progressState);
		}
		m_CanUseSaveSystem.Update(!start);
	}

	[Preserve]
	protected override void OnUpdate()
	{
	}

	public void PreDeserialize(Context context)
	{
		SetScreen(GameScreen.Main);
	}

	public void SetScreen(GameScreen screen)
	{
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		InputManager.instance.hideCursor = screen == GameScreen.FreeCamera;
		InputManager instance = InputManager.instance;
		CursorLockMode cursorLockMode = (((uint)screen > 1u) ? ((CursorLockMode)0) : SharedSettings.instance.graphics.cursorMode.ToUnityCursorMode());
		instance.cursorLockMode = cursorLockMode;
		m_ActiveScreenBinding.Update(screen);
	}

	[Preserve]
	public GameScreenUISystem()
	{
	}
}

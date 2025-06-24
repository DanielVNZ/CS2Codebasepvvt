using System;
using Colossal.Entities;
using Colossal.Serialization.Entities;
using Game.SceneFlow;
using Game.Serialization;
using Unity.Entities;
using Unity.Jobs;
using UnityEngine;
using UnityEngine.Scripting;

namespace Game;

public abstract class GameSystemBase : COSystemBase
{
	private LoadGameSystem m_LoadGameSystem;

	[Preserve]
	protected override void OnCreate()
	{
		((COSystemBase)this).OnCreate();
		if (((ComponentSystemBase)this).World == World.DefaultGameObjectInjectionWorld)
		{
			m_LoadGameSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<LoadGameSystem>();
			LoadGameSystem loadGameSystem = m_LoadGameSystem;
			loadGameSystem.onOnSaveGameLoaded = (LoadGameSystem.EventGameLoaded)Delegate.Combine(loadGameSystem.onOnSaveGameLoaded, new LoadGameSystem.EventGameLoaded(GameLoaded));
		}
		GameManager.instance.onWorldReady += WorldReady;
		GameManager.instance.onGamePreload += GamePreload;
		GameManager.instance.onGameLoadingComplete += GameLoadingComplete;
		Application.focusChanged += FocusChanged;
	}

	private void FocusChanged(bool hasfocus)
	{
		try
		{
			OnFocusChanged(hasfocus);
		}
		catch (Exception ex)
		{
			COSystemBase.baseLog.Error(ex, (object)(((object)this).GetType().Name + ": Error on Focus change"));
		}
	}

	[Preserve]
	protected override void OnDestroy()
	{
		GameManager.instance.onWorldReady -= WorldReady;
		GameManager.instance.onGamePreload -= GamePreload;
		GameManager.instance.onGameLoadingComplete -= GameLoadingComplete;
		if (((ComponentSystemBase)this).World == World.DefaultGameObjectInjectionWorld && m_LoadGameSystem != null)
		{
			LoadGameSystem loadGameSystem = m_LoadGameSystem;
			loadGameSystem.onOnSaveGameLoaded = (LoadGameSystem.EventGameLoaded)Delegate.Remove(loadGameSystem.onOnSaveGameLoaded, new LoadGameSystem.EventGameLoaded(GameLoaded));
		}
		Application.focusChanged -= FocusChanged;
		((COSystemBase)this).OnDestroy();
	}

	private void GameLoadingComplete(Purpose purpose, GameMode mode)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		try
		{
			OnGameLoadingComplete(purpose, mode);
		}
		catch (Exception ex)
		{
			COSystemBase.baseLog.Error(ex, (object)(((object)this).GetType().Name + ": Error on state change, disabling system..."));
		}
	}

	private void GameLoaded(Context serializationContext)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		try
		{
			OnGameLoaded(serializationContext);
		}
		catch (Exception ex)
		{
			COSystemBase.baseLog.Error(ex, (object)(((object)this).GetType().Name + ": Error on game load, disabling system..."));
			((ComponentSystemBase)this).Enabled = false;
		}
	}

	private void GamePreload(Purpose purpose, GameMode mode)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		try
		{
			OnGamePreload(purpose, mode);
		}
		catch (Exception ex)
		{
			COSystemBase.baseLog.Error(ex, (object)(((object)this).GetType().Name + ": Error on game preload, disabling system..."));
			((ComponentSystemBase)this).Enabled = false;
		}
	}

	private void WorldReady()
	{
		try
		{
			OnWorldReady();
		}
		catch (Exception ex)
		{
			COSystemBase.baseLog.Error(ex, (object)(((object)this).GetType().Name + ": Error on game preload, disabling system..."));
			((ComponentSystemBase)this).Enabled = false;
		}
	}

	protected virtual void OnWorldReady()
	{
	}

	protected virtual void OnGamePreload(Purpose purpose, GameMode mode)
	{
	}

	protected virtual void OnGameLoaded(Context serializationContext)
	{
	}

	protected virtual void OnGameLoadingComplete(Purpose purpose, GameMode mode)
	{
	}

	protected virtual void OnFocusChanged(bool hasFocus)
	{
	}

	public virtual int GetUpdateInterval(SystemUpdatePhase phase)
	{
		return 1;
	}

	public virtual int GetUpdateOffset(SystemUpdatePhase phase)
	{
		return -1;
	}

	public void ResetDependency()
	{
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		((SystemBase)this).Dependency = default(JobHandle);
	}

	[Preserve]
	protected GameSystemBase()
	{
	}
}

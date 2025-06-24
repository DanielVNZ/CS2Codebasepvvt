using System;
using System.Collections.Generic;
using Colossal.PSI.Common;
using Colossal.Serialization.Entities;
using Colossal.UI.Binding;
using Game.City;
using Unity.Entities;
using UnityEngine.Scripting;

namespace Game.UI.InGame;

public class AchievementsUISystem : UISystemBase
{
	private enum AchievementTabStatus
	{
		Available,
		Hidden,
		ModsDisabled,
		OptionsDisabled
	}

	private const string kGroup = "achievements";

	private CityConfigurationSystem m_CityConfigurationSystem;

	private RawValueBinding m_AchievementsBinding;

	private GetterValueBinding<int> m_TabStatusBinding;

	[Preserve]
	protected override void OnCreate()
	{
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Expected O, but got Unknown
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Expected O, but got Unknown
		//IL_0051: Expected O, but got Unknown
		base.OnCreate();
		m_CityConfigurationSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<CityConfigurationSystem>();
		PlatformManager.instance.onAchievementUpdated += new AchievementUpdatedEventHandler(UpdateAchievements);
		RawValueBinding val = new RawValueBinding("achievements", "achievements", (Action<IJsonWriter>)BindAchievements);
		RawValueBinding binding = val;
		m_AchievementsBinding = val;
		AddBinding((IBinding)(object)binding);
		AddBinding((IBinding)(object)(m_TabStatusBinding = new GetterValueBinding<int>("achievements", "achievementTabStatus", (Func<int>)GetAchievementTabStatus, (IWriter<int>)null, (EqualityComparer<int>)null)));
	}

	private int GetAchievementTabStatus()
	{
		if (PlatformManager.instance.CountAchievements(false) == 0)
		{
			return 1;
		}
		if (m_CityConfigurationSystem.usedMods.Count > 0)
		{
			return 2;
		}
		if (!PlatformManager.instance.achievementsEnabled)
		{
			return 3;
		}
		return 0;
	}

	[Preserve]
	protected override void OnUpdate()
	{
	}

	protected override void OnGameLoadingComplete(Purpose purpose, GameMode mode)
	{
		m_TabStatusBinding.Update();
	}

	private void UpdateAchievements(IAchievementsSupport backend, AchievementId id)
	{
		m_AchievementsBinding.Update();
	}

	private void BindAchievements(IJsonWriter binder)
	{
		int num = PlatformManager.instance.CountAchievements(false);
		m_TabStatusBinding.Update();
		if (num > 0)
		{
			JsonWriterExtensions.ArrayBegin(binder, num);
			foreach (IAchievement item in PlatformManager.instance.EnumerateAchievements())
			{
				BindAchievement(binder, item);
			}
			binder.ArrayEnd();
		}
		else
		{
			binder.ArrayBegin(0u);
			binder.ArrayEnd();
		}
	}

	private void BindAchievement(IJsonWriter binder, IAchievement achievement)
	{
		//IL_00a8: Unknown result type (might be due to invalid IL or missing references)
		binder.TypeBegin("achievements.Achievement");
		binder.PropertyName("localeKey");
		binder.Write(achievement.internalName);
		bool flag = !achievement.achieved;
		binder.PropertyName("imagePath");
		binder.Write(GetImagePath(achievement, flag));
		binder.PropertyName("locked");
		binder.Write(flag);
		binder.PropertyName("isIncremental");
		binder.Write(achievement.isIncremental);
		binder.PropertyName("progress");
		binder.Write(achievement.progress);
		binder.PropertyName("maxProgress");
		binder.Write(achievement.maxProgress);
		binder.PropertyName("dlcImage");
		binder.Write(GetDlcImage(achievement.dlcId));
		binder.PropertyName("isDevelopment");
		binder.Write(achievement is DevelopmentAchievement);
		binder.TypeEnd();
	}

	private static string GetDlcImage(DlcId dlcId)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		if (!(dlcId != DlcId.BaseGame))
		{
			return null;
		}
		return "Media/DLC/" + PlatformManager.instance.GetDlcName(dlcId) + ".svg";
	}

	private static string GetImagePath(IAchievement achievement, bool locked)
	{
		return "Media/Game/Achievements/" + achievement.internalName + (locked ? "_locked" : "") + ".png";
	}

	[Preserve]
	public AchievementsUISystem()
	{
	}
}

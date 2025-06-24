using System;
using System.Collections.Generic;
using Colossal.Entities;
using Colossal.Mathematics;
using Colossal.UI.Binding;
using Game.Simulation;
using Unity.Entities;
using UnityEngine.Scripting;

namespace Game.UI.Editor;

public class EditorBottomBarUISystem : UISystemBase
{
	private static readonly string kGroup = "editorBottomBar";

	private ClimateSystem m_ClimateSystem;

	private PlanetarySystem m_PlanetarySystem;

	public override GameMode gameMode => GameMode.Editor;

	private float m_NormalizedTimeBindingValue => MathUtils.Snap(m_PlanetarySystem.normalizedTime, 0.01f);

	private float m_NormalizedDateBindingValue => MathUtils.Snap((float)m_ClimateSystem.currentDate, 0.01f);

	[Preserve]
	protected override void OnCreate()
	{
		//IL_00ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d4: Expected O, but got Unknown
		//IL_010d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0117: Expected O, but got Unknown
		//IL_0150: Unknown result type (might be due to invalid IL or missing references)
		//IL_015a: Expected O, but got Unknown
		base.OnCreate();
		m_ClimateSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<ClimateSystem>();
		m_PlanetarySystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<PlanetarySystem>();
		AddUpdateBinding((IUpdateBinding)(object)new GetterValueBinding<float>(kGroup, "timeOfDay", (Func<float>)(() => m_NormalizedTimeBindingValue), (IWriter<float>)null, (EqualityComparer<float>)null));
		AddUpdateBinding((IUpdateBinding)(object)new GetterValueBinding<float>(kGroup, "date", (Func<float>)(() => m_NormalizedDateBindingValue), (IWriter<float>)null, (EqualityComparer<float>)null));
		AddUpdateBinding((IUpdateBinding)(object)new GetterValueBinding<float>(kGroup, "cloudiness", (Func<float>)(() => m_ClimateSystem.cloudiness), (IWriter<float>)null, (EqualityComparer<float>)null));
		AddBinding((IBinding)(object)new TriggerBinding<float>(kGroup, "setTimeOfDay", (Action<float>)SetTimeOfDay, (IReader<float>)null));
		AddBinding((IBinding)new TriggerBinding(kGroup, "resetTimeOfDay", (Action)ResetTimeOfDay));
		AddBinding((IBinding)(object)new TriggerBinding<float>(kGroup, "setDate", (Action<float>)SetDate, (IReader<float>)null));
		AddBinding((IBinding)new TriggerBinding(kGroup, "resetDate", (Action)ResetDate));
		AddBinding((IBinding)(object)new TriggerBinding<float>(kGroup, "setCloudiness", (Action<float>)SetCloudiness, (IReader<float>)null));
		AddBinding((IBinding)new TriggerBinding(kGroup, "resetCloudiness", (Action)ResetCloudiness));
	}

	[Preserve]
	protected override void OnStopRunning()
	{
		((COSystemBase)this).OnStopRunning();
		m_PlanetarySystem.overrideTime = false;
		m_ClimateSystem.currentDate.overrideState = false;
		m_ClimateSystem.cloudiness.overrideState = false;
	}

	private void SetTimeOfDay(float time)
	{
		m_PlanetarySystem.overrideTime = true;
		m_PlanetarySystem.normalizedTime = time;
	}

	private void ResetTimeOfDay()
	{
		m_PlanetarySystem.overrideTime = false;
	}

	private void SetDate(float date)
	{
		m_ClimateSystem.currentDate.overrideValue = date;
	}

	private void ResetDate()
	{
		m_ClimateSystem.currentDate.overrideState = false;
	}

	private void SetCloudiness(float cloudiness)
	{
		m_ClimateSystem.cloudiness.overrideValue = cloudiness;
	}

	private void ResetCloudiness()
	{
		m_ClimateSystem.cloudiness.overrideState = false;
	}

	[Preserve]
	public EditorBottomBarUISystem()
	{
	}
}

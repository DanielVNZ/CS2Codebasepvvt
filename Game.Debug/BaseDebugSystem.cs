using System.Collections.Generic;
using Unity.Entities;
using Unity.Jobs;
using UnityEngine.Rendering;
using UnityEngine.Scripting;

namespace Game.Debug;

public abstract class BaseDebugSystem : GameSystemBase
{
	public class Option
	{
		public string name { get; private set; }

		public bool enabled { get; set; }

		public Option(string displayName, bool defaultEnabled)
		{
			name = displayName;
			enabled = defaultEnabled;
		}
	}

	public List<Option> options { get; private set; }

	[Preserve]
	protected override void OnCreate()
	{
		base.OnCreate();
		options = new List<Option>();
	}

	protected Option AddOption(string displayName, bool defaultEnabled)
	{
		Option option = new Option(displayName, defaultEnabled);
		options.Add(option);
		return option;
	}

	[Preserve]
	protected override void OnUpdate()
	{
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		((SystemBase)this).Dependency = OnUpdate(((SystemBase)this).Dependency);
	}

	[Preserve]
	protected virtual JobHandle OnUpdate(JobHandle inputDeps)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		return inputDeps;
	}

	public virtual void OnEnabled(Container container)
	{
	}

	public virtual void OnDisabled(Container container)
	{
	}

	[Preserve]
	protected BaseDebugSystem()
	{
	}
}

using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Colossal.UI.Binding;
using Game.Buildings;
using Game.Common;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine.Scripting;

namespace Game.UI.InGame;

[CompilerGenerated]
public class EfficiencySection : InfoSectionBase
{
	private struct EfficiencyFactor : IJsonWritable
	{
		private Game.Buildings.EfficiencyFactor factor;

		private int value;

		private int result;

		public EfficiencyFactor(Game.Buildings.EfficiencyFactor factor, int value, int result)
		{
			this.factor = factor;
			this.value = value;
			this.result = result;
		}

		public void Write(IJsonWriter writer)
		{
			writer.TypeBegin(typeof(EfficiencyFactor).FullName);
			writer.PropertyName("factor");
			writer.Write(Enum.GetName(typeof(Game.Buildings.EfficiencyFactor), factor));
			writer.PropertyName("value");
			writer.Write(value);
			writer.PropertyName("result");
			writer.Write(result);
			writer.TypeEnd();
		}
	}

	protected override string group => "EfficiencySection";

	private int efficiency { get; set; }

	private List<EfficiencyFactor> factors { get; set; }

	[Preserve]
	protected override void OnCreate()
	{
		base.OnCreate();
		factors = new List<EfficiencyFactor>(30);
	}

	protected override void Reset()
	{
		efficiency = 0;
		factors.Clear();
	}

	private bool Visible()
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_0073: Unknown result type (might be due to invalid IL or missing references)
		//IL_0074: Unknown result type (might be due to invalid IL or missing references)
		EntityManager entityManager = ((ComponentSystemBase)this).EntityManager;
		if (((EntityManager)(ref entityManager)).HasComponent<Building>(selectedEntity))
		{
			entityManager = ((ComponentSystemBase)this).EntityManager;
			if (((EntityManager)(ref entityManager)).HasComponent<Efficiency>(selectedEntity))
			{
				entityManager = ((ComponentSystemBase)this).EntityManager;
				if (!((EntityManager)(ref entityManager)).HasComponent<Abandoned>(selectedEntity))
				{
					entityManager = ((ComponentSystemBase)this).EntityManager;
					if (!((EntityManager)(ref entityManager)).HasComponent<Destroyed>(selectedEntity))
					{
						if (CompanyUIUtils.HasCompany(((ComponentSystemBase)this).EntityManager, selectedEntity, selectedPrefab, out var company))
						{
							return company != Entity.Null;
						}
						return true;
					}
				}
			}
		}
		return false;
	}

	[Preserve]
	protected override void OnUpdate()
	{
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		base.visible = Visible();
		if (base.visible)
		{
			EntityManager entityManager = ((ComponentSystemBase)this).EntityManager;
			DynamicBuffer<Efficiency> buffer = ((EntityManager)(ref entityManager)).GetBuffer<Efficiency>(selectedEntity, true);
			m_Dirty = (int)math.round(100f * BuildingUtils.GetEfficiency(buffer)) != efficiency;
		}
	}

	protected override void OnProcess()
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fd: Unknown result type (might be due to invalid IL or missing references)
		//IL_0102: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		EntityManager entityManager = ((ComponentSystemBase)this).EntityManager;
		DynamicBuffer<Efficiency> buffer = ((EntityManager)(ref entityManager)).GetBuffer<Efficiency>(selectedEntity, true);
		efficiency = (int)math.round(100f * BuildingUtils.GetEfficiency(buffer));
		NativeArray<Efficiency> val = buffer.ToNativeArray(AllocatorHandle.op_Implicit((Allocator)2));
		try
		{
			NativeSortExtension.Sort<Efficiency>(val);
			factors.Clear();
			if (val.Length == 0)
			{
				return;
			}
			Enumerator<Efficiency> enumerator;
			if (efficiency > 0)
			{
				float num = 100f;
				enumerator = val.GetEnumerator();
				try
				{
					while (enumerator.MoveNext())
					{
						Efficiency current = enumerator.Current;
						float num2 = math.max(0f, current.m_Efficiency);
						num *= num2;
						int num3 = math.max(-99, (int)math.round(100f * num2) - 100);
						int result = math.max(1, (int)math.round(num));
						if (num3 != 0)
						{
							factors.Add(new EfficiencyFactor(current.m_Factor, num3, result));
						}
					}
					return;
				}
				finally
				{
					((IDisposable)enumerator/*cast due to .constrained prefix*/).Dispose();
				}
			}
			enumerator = val.GetEnumerator();
			try
			{
				while (enumerator.MoveNext())
				{
					Efficiency current2 = enumerator.Current;
					if (math.max(0f, current2.m_Efficiency) == 0f)
					{
						factors.Add(new EfficiencyFactor(current2.m_Factor, -100, -100));
						if ((int)current2.m_Factor <= 3)
						{
							break;
						}
					}
				}
			}
			finally
			{
				((IDisposable)enumerator/*cast due to .constrained prefix*/).Dispose();
			}
		}
		finally
		{
			((IDisposable)val/*cast due to .constrained prefix*/).Dispose();
		}
	}

	public override void OnWriteProperties(IJsonWriter writer)
	{
		writer.PropertyName("efficiency");
		writer.Write(efficiency);
		writer.PropertyName("factors");
		JsonWriterExtensions.ArrayBegin(writer, factors.Count);
		for (int i = 0; i < factors.Count; i++)
		{
			JsonWriterExtensions.Write<EfficiencyFactor>(writer, factors[i]);
		}
		writer.ArrayEnd();
	}

	[Preserve]
	public EfficiencySection()
	{
	}
}

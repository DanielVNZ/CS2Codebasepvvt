using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Colossal.Entities;
using Colossal.UI.Binding;
using Game.Buildings;
using Game.Objects;
using Game.Prefabs;
using Game.Simulation;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using UnityEngine;
using UnityEngine.Scripting;

namespace Game.UI.InGame;

[CompilerGenerated]
public class AttractivenessSection : InfoSectionBase
{
	private readonly struct AttractivenessFactor : IJsonWritable, IComparable<AttractivenessFactor>
	{
		private int localeKey { get; }

		private float delta { get; }

		public AttractivenessFactor(int factor, float delta)
		{
			localeKey = factor;
			this.delta = delta;
		}

		public void Write(IJsonWriter writer)
		{
			writer.TypeBegin(typeof(AttractionSystem.AttractivenessFactor).FullName);
			writer.PropertyName("localeKey");
			writer.Write(Enum.GetName(typeof(AttractionSystem.AttractivenessFactor), (AttractionSystem.AttractivenessFactor)localeKey));
			writer.PropertyName("delta");
			writer.Write(delta);
			writer.TypeEnd();
		}

		public int CompareTo(AttractivenessFactor other)
		{
			return delta.CompareTo(other.delta);
		}
	}

	private TerrainAttractivenessSystem m_TerrainAttractivenessSystem;

	private TerrainSystem m_TerrainSystem;

	private EntityQuery m_SettingsQuery;

	protected override string group => "AttractivenessSection";

	private float baseAttractiveness { get; set; }

	private float attractiveness { get; set; }

	private List<AttractivenessFactor> factors { get; set; }

	protected override void Reset()
	{
		baseAttractiveness = 0f;
		factors.Clear();
	}

	[Preserve]
	protected override void OnCreate()
	{
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		base.OnCreate();
		factors = new List<AttractivenessFactor>(5);
		m_TerrainAttractivenessSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<TerrainAttractivenessSystem>();
		m_TerrainSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<TerrainSystem>();
		m_SettingsQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[1] { ComponentType.ReadOnly<AttractivenessParameterData>() });
	}

	private bool Visible()
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		EntityManager entityManager = ((ComponentSystemBase)this).EntityManager;
		if (!((EntityManager)(ref entityManager)).HasComponent<AttractionData>(selectedPrefab))
		{
			entityManager = ((ComponentSystemBase)this).EntityManager;
			return ((EntityManager)(ref entityManager)).HasComponent<AttractivenessProvider>(selectedEntity);
		}
		return true;
	}

	[Preserve]
	protected override void OnUpdate()
	{
		base.visible = Visible();
	}

	protected override void OnProcess()
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_0093: Unknown result type (might be due to invalid IL or missing references)
		//IL_0099: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_012e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0134: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		//IL_007d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0154: Unknown result type (might be due to invalid IL or missing references)
		//IL_0159: Unknown result type (might be due to invalid IL or missing references)
		//IL_015b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0166: Unknown result type (might be due to invalid IL or missing references)
		//IL_016b: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_0118: Unknown result type (might be due to invalid IL or missing references)
		NativeArray<int> val = default(NativeArray<int>);
		val._002Ector(5, (Allocator)3, (NativeArrayOptions)1);
		if (TryGetComponentWithUpgrades<AttractionData>(selectedEntity, selectedPrefab, out AttractionData data))
		{
			baseAttractiveness = data.m_Attractiveness;
		}
		attractiveness = baseAttractiveness;
		EntityManager entityManager = ((ComponentSystemBase)this).EntityManager;
		DynamicBuffer<Efficiency> buffer = default(DynamicBuffer<Efficiency>);
		if (!((EntityManager)(ref entityManager)).HasComponent<Signature>(selectedEntity) && EntitiesExtensions.TryGetBuffer<Efficiency>(((ComponentSystemBase)this).EntityManager, selectedEntity, true, ref buffer))
		{
			float efficiency = BuildingUtils.GetEfficiency(buffer);
			attractiveness *= efficiency;
			AttractionSystem.SetFactor(val, AttractionSystem.AttractivenessFactor.Efficiency, (efficiency - 1f) * 100f);
		}
		Game.Buildings.Park park = default(Game.Buildings.Park);
		if (EntitiesExtensions.TryGetComponent<Game.Buildings.Park>(((ComponentSystemBase)this).EntityManager, selectedEntity, ref park) && TryGetComponentWithUpgrades<ParkData>(selectedEntity, selectedPrefab, out ParkData data2))
		{
			float num = ((data2.m_MaintenancePool > 0) ? ((float)park.m_Maintenance / (float)data2.m_MaintenancePool) : 0f);
			float num2 = Mathf.Min(1f, 0.25f + 0.25f * (float)Mathf.FloorToInt(num / 0.3f));
			attractiveness *= num2;
			AttractionSystem.SetFactor(val, AttractionSystem.AttractivenessFactor.Maintenance, (num2 - 1f) * 100f);
		}
		Transform transform = default(Transform);
		if (EntitiesExtensions.TryGetComponent<Transform>(((ComponentSystemBase)this).EntityManager, selectedEntity, ref transform))
		{
			JobHandle dependencies;
			CellMapData<TerrainAttractiveness> data3 = m_TerrainAttractivenessSystem.GetData(readOnly: true, out dependencies);
			((SystemBase)this).Dependency = JobHandle.CombineDependencies(((SystemBase)this).Dependency, dependencies);
			JobHandle dependency = ((SystemBase)this).Dependency;
			((JobHandle)(ref dependency)).Complete();
			TerrainHeightData heightData = m_TerrainSystem.GetHeightData();
			AttractivenessParameterData singleton = ((EntityQuery)(ref m_SettingsQuery)).GetSingleton<AttractivenessParameterData>();
			attractiveness *= 1f + 0.01f * TerrainAttractivenessSystem.EvaluateAttractiveness(transform.m_Position, data3, heightData, singleton, val);
		}
		for (int i = 0; i < val.Length; i++)
		{
			if (val[i] != 0)
			{
				factors.Add(new AttractivenessFactor(i, val[i]));
			}
		}
		val.Dispose();
	}

	public override void OnWriteProperties(IJsonWriter writer)
	{
		writer.PropertyName("attractiveness");
		writer.Write(attractiveness);
		writer.PropertyName("baseAttractiveness");
		writer.Write(baseAttractiveness);
		writer.PropertyName("factors");
		JsonWriterExtensions.ArrayBegin(writer, factors.Count);
		for (int i = 0; i < factors.Count; i++)
		{
			JsonWriterExtensions.Write<AttractivenessFactor>(writer, factors[i]);
		}
		writer.ArrayEnd();
	}

	[Preserve]
	public AttractivenessSection()
	{
	}
}

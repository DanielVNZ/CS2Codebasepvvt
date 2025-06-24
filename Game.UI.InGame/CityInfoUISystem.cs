using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Colossal.Mathematics;
using Colossal.Serialization.Entities;
using Colossal.UI.Binding;
using Game.City;
using Game.Prefabs;
using Game.Simulation;
using Unity.Collections;
using Unity.Entities;
using Unity.Entities.Internal;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Scripting;

namespace Game.UI.InGame;

[CompilerGenerated]
public class CityInfoUISystem : UISystemBase, IDefaultSerializable, ISerializable
{
	private struct TypeHandle
	{
		[ReadOnly]
		public ComponentLookup<Locked> __Game_Prefabs_Locked_RO_ComponentLookup;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void __AssignHandles(ref SystemState state)
		{
			//IL_0003: Unknown result type (might be due to invalid IL or missing references)
			//IL_0008: Unknown result type (might be due to invalid IL or missing references)
			__Game_Prefabs_Locked_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Locked>(true);
		}
	}

	public const string kGroup = "cityInfo";

	private SimulationSystem m_SimulationSystem;

	private ResidentialDemandSystem m_ResidentialDemandSystem;

	private CommercialDemandSystem m_CommercialDemandSystem;

	private IndustrialDemandSystem m_IndustrialDemandSystem;

	private CitySystem m_CitySystem;

	private CitizenHappinessSystem m_CitizenHappinessSystem;

	private RawValueBinding m_ResidentialLowFactors;

	private RawValueBinding m_ResidentialMediumFactors;

	private RawValueBinding m_ResidentialHighFactors;

	private RawValueBinding m_CommercialFactors;

	private RawValueBinding m_IndustrialFactors;

	private RawValueBinding m_OfficeFactors;

	private RawValueBinding m_HappinessFactors;

	private float m_ResidentialLowDemand;

	private float m_ResidentialMediumDemand;

	private float m_ResidentialHighDemand;

	private float m_CommercialDemand;

	private float m_IndustrialDemand;

	private float m_OfficeDemand;

	private uint m_LastFrameIndex;

	private int m_AvgHappiness;

	private UIUpdateState m_UpdateState;

	private TypeHandle __TypeHandle;

	private float m_ResidentialLowDemandBindingValue => MathUtils.Snap(m_ResidentialLowDemand, 0.001f);

	private float m_ResidentialMediumDemandBindingValue => MathUtils.Snap(m_ResidentialMediumDemand, 0.001f);

	private float m_ResidentialHighDemandBindingValue => MathUtils.Snap(m_ResidentialHighDemand, 0.001f);

	private float m_CommercialDemandBindingValue => MathUtils.Snap(m_CommercialDemand, 0.001f);

	private float m_IndustrialDemandBindingValue => MathUtils.Snap(m_IndustrialDemand, 0.001f);

	private float m_OfficeDemandBindingValue => MathUtils.Snap(m_OfficeDemand, 0.001f);

	[Preserve]
	protected override void OnCreate()
	{
		//IL_0179: Unknown result type (might be due to invalid IL or missing references)
		//IL_017e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0180: Expected O, but got Unknown
		//IL_0185: Expected O, but got Unknown
		//IL_01a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01aa: Expected O, but got Unknown
		//IL_01af: Expected O, but got Unknown
		//IL_01cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d4: Expected O, but got Unknown
		//IL_01d9: Expected O, but got Unknown
		//IL_01f7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01fc: Unknown result type (might be due to invalid IL or missing references)
		//IL_01fe: Expected O, but got Unknown
		//IL_0203: Expected O, but got Unknown
		//IL_0221: Unknown result type (might be due to invalid IL or missing references)
		//IL_0226: Unknown result type (might be due to invalid IL or missing references)
		//IL_0228: Expected O, but got Unknown
		//IL_022d: Expected O, but got Unknown
		//IL_024b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0250: Unknown result type (might be due to invalid IL or missing references)
		//IL_0252: Expected O, but got Unknown
		//IL_0257: Expected O, but got Unknown
		//IL_0275: Unknown result type (might be due to invalid IL or missing references)
		//IL_027a: Unknown result type (might be due to invalid IL or missing references)
		//IL_027c: Expected O, but got Unknown
		//IL_0281: Expected O, but got Unknown
		base.OnCreate();
		m_SimulationSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<SimulationSystem>();
		m_ResidentialDemandSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<ResidentialDemandSystem>();
		m_CommercialDemandSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<CommercialDemandSystem>();
		m_IndustrialDemandSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<IndustrialDemandSystem>();
		m_CitySystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<CitySystem>();
		m_CitizenHappinessSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<CitizenHappinessSystem>();
		AddUpdateBinding((IUpdateBinding)(object)new GetterValueBinding<float>("cityInfo", "residentialLowDemand", (Func<float>)(() => m_ResidentialLowDemandBindingValue), (IWriter<float>)null, (EqualityComparer<float>)null));
		AddUpdateBinding((IUpdateBinding)(object)new GetterValueBinding<float>("cityInfo", "residentialMediumDemand", (Func<float>)(() => m_ResidentialMediumDemandBindingValue), (IWriter<float>)null, (EqualityComparer<float>)null));
		AddUpdateBinding((IUpdateBinding)(object)new GetterValueBinding<float>("cityInfo", "residentialHighDemand", (Func<float>)(() => m_ResidentialHighDemandBindingValue), (IWriter<float>)null, (EqualityComparer<float>)null));
		AddUpdateBinding((IUpdateBinding)(object)new GetterValueBinding<float>("cityInfo", "commercialDemand", (Func<float>)(() => m_CommercialDemandBindingValue), (IWriter<float>)null, (EqualityComparer<float>)null));
		AddUpdateBinding((IUpdateBinding)(object)new GetterValueBinding<float>("cityInfo", "industrialDemand", (Func<float>)(() => m_IndustrialDemandBindingValue), (IWriter<float>)null, (EqualityComparer<float>)null));
		AddUpdateBinding((IUpdateBinding)(object)new GetterValueBinding<float>("cityInfo", "officeDemand", (Func<float>)(() => m_OfficeDemandBindingValue), (IWriter<float>)null, (EqualityComparer<float>)null));
		AddUpdateBinding((IUpdateBinding)(object)new GetterValueBinding<int>("cityInfo", "happiness", (Func<int>)(() => m_AvgHappiness), (IWriter<int>)null, (EqualityComparer<int>)null));
		RawValueBinding val = new RawValueBinding("cityInfo", "residentialLowFactors", (Action<IJsonWriter>)WriteResidentialLowFactors);
		RawValueBinding binding = val;
		m_ResidentialLowFactors = val;
		AddBinding((IBinding)(object)binding);
		RawValueBinding val2 = new RawValueBinding("cityInfo", "residentialMediumFactors", (Action<IJsonWriter>)WriteResidentialMediumFactors);
		binding = val2;
		m_ResidentialMediumFactors = val2;
		AddBinding((IBinding)(object)binding);
		RawValueBinding val3 = new RawValueBinding("cityInfo", "residentialHighFactors", (Action<IJsonWriter>)WriteResidentialHighFactors);
		binding = val3;
		m_ResidentialHighFactors = val3;
		AddBinding((IBinding)(object)binding);
		RawValueBinding val4 = new RawValueBinding("cityInfo", "commercialFactors", (Action<IJsonWriter>)WriteCommercialFactors);
		binding = val4;
		m_CommercialFactors = val4;
		AddBinding((IBinding)(object)binding);
		RawValueBinding val5 = new RawValueBinding("cityInfo", "industrialFactors", (Action<IJsonWriter>)WriteIndustrialFactors);
		binding = val5;
		m_IndustrialFactors = val5;
		AddBinding((IBinding)(object)binding);
		RawValueBinding val6 = new RawValueBinding("cityInfo", "officeFactors", (Action<IJsonWriter>)WriteOfficeFactors);
		binding = val6;
		m_OfficeFactors = val6;
		AddBinding((IBinding)(object)binding);
		RawValueBinding val7 = new RawValueBinding("cityInfo", "happinessFactors", (Action<IJsonWriter>)WriteHappinessFactors);
		binding = val7;
		m_HappinessFactors = val7;
		AddBinding((IBinding)(object)binding);
		m_UpdateState = UIUpdateState.Create(((ComponentSystemBase)this).World, 256);
	}

	protected override void OnGameLoaded(Context serializationContext)
	{
		m_UpdateState.ForceUpdate();
	}

	public void Serialize<TWriter>(TWriter writer) where TWriter : IWriter
	{
		float num = m_ResidentialLowDemand;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(num);
		float num2 = m_ResidentialMediumDemand;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(num2);
		float num3 = m_ResidentialHighDemand;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(num3);
		float num4 = m_CommercialDemand;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(num4);
		float num5 = m_IndustrialDemand;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(num5);
		float num6 = m_OfficeDemand;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(num6);
		uint num7 = m_LastFrameIndex;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(num7);
		int num8 = m_AvgHappiness;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(num8);
	}

	public void Deserialize<TReader>(TReader reader) where TReader : IReader
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00eb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f3: Unknown result type (might be due to invalid IL or missing references)
		Context context = ((IReader)reader).context;
		if (((Context)(ref context)).version >= Version.residentialDemandSplitUI)
		{
			ref float reference = ref m_ResidentialLowDemand;
			((IReader)reader/*cast due to .constrained prefix*/).Read(ref reference);
			ref float reference2 = ref m_ResidentialMediumDemand;
			((IReader)reader/*cast due to .constrained prefix*/).Read(ref reference2);
			ref float reference3 = ref m_ResidentialHighDemand;
			((IReader)reader/*cast due to .constrained prefix*/).Read(ref reference3);
		}
		else
		{
			float num = default(float);
			((IReader)reader/*cast due to .constrained prefix*/).Read(ref num);
			m_ResidentialLowDemand = num / 3f;
			m_ResidentialMediumDemand = num / 3f;
			m_ResidentialHighDemand = num / 3f;
		}
		ref float reference4 = ref m_CommercialDemand;
		((IReader)reader/*cast due to .constrained prefix*/).Read(ref reference4);
		ref float reference5 = ref m_IndustrialDemand;
		((IReader)reader/*cast due to .constrained prefix*/).Read(ref reference5);
		ref float reference6 = ref m_OfficeDemand;
		((IReader)reader/*cast due to .constrained prefix*/).Read(ref reference6);
		ref uint reference7 = ref m_LastFrameIndex;
		((IReader)reader/*cast due to .constrained prefix*/).Read(ref reference7);
		context = ((IReader)reader).context;
		if (((Context)(ref context)).version >= Version.populationComponent)
		{
			ref int reference8 = ref m_AvgHappiness;
			((IReader)reader/*cast due to .constrained prefix*/).Read(ref reference8);
		}
	}

	public void SetDefaults(Context context)
	{
		m_ResidentialLowDemand = 0f;
		m_ResidentialMediumDemand = 0f;
		m_ResidentialHighDemand = 0f;
		m_CommercialDemand = 0f;
		m_IndustrialDemand = 0f;
		m_OfficeDemand = 0f;
		m_LastFrameIndex = 0u;
	}

	[Preserve]
	protected override void OnUpdate()
	{
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_0082: Unknown result type (might be due to invalid IL or missing references)
		//IL_0101: Unknown result type (might be due to invalid IL or missing references)
		//IL_0106: Unknown result type (might be due to invalid IL or missing references)
		//IL_010f: Unknown result type (might be due to invalid IL or missing references)
		//IL_011d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0122: Unknown result type (might be due to invalid IL or missing references)
		//IL_012b: Unknown result type (might be due to invalid IL or missing references)
		base.OnUpdate();
		uint num = m_SimulationSystem.frameIndex - m_LastFrameIndex;
		if (num != 0)
		{
			m_LastFrameIndex = m_SimulationSystem.frameIndex;
			m_ResidentialLowDemand = AdvanceSmoothDemand(m_ResidentialLowDemand, m_ResidentialDemandSystem.buildingDemand.x, num);
			m_ResidentialMediumDemand = AdvanceSmoothDemand(m_ResidentialMediumDemand, m_ResidentialDemandSystem.buildingDemand.y, num);
			m_ResidentialHighDemand = AdvanceSmoothDemand(m_ResidentialHighDemand, m_ResidentialDemandSystem.buildingDemand.z, num);
			m_CommercialDemand = AdvanceSmoothDemand(m_CommercialDemand, m_CommercialDemandSystem.buildingDemand, num);
			int target = math.max(m_IndustrialDemandSystem.industrialBuildingDemand, m_IndustrialDemandSystem.storageBuildingDemand);
			m_IndustrialDemand = AdvanceSmoothDemand(m_IndustrialDemand, target, num);
			m_OfficeDemand = AdvanceSmoothDemand(m_OfficeDemand, m_IndustrialDemandSystem.officeBuildingDemand, num);
			EntityManager entityManager = ((ComponentSystemBase)this).EntityManager;
			if (((EntityManager)(ref entityManager)).HasComponent<Population>(m_CitySystem.City))
			{
				entityManager = ((ComponentSystemBase)this).EntityManager;
				m_AvgHappiness = ((EntityManager)(ref entityManager)).GetComponentData<Population>(m_CitySystem.City).m_AverageHappiness;
			}
			else
			{
				m_AvgHappiness = 50;
			}
		}
		if (m_UpdateState.Advance())
		{
			m_ResidentialLowFactors.Update();
			m_ResidentialMediumFactors.Update();
			m_ResidentialHighFactors.Update();
			m_CommercialFactors.Update();
			m_IndustrialFactors.Update();
			m_OfficeFactors.Update();
			m_HappinessFactors.Update();
		}
	}

	private static float AdvanceSmoothDemand(float current, int target, uint delta)
	{
		return math.clamp((float)target / 100f, current - 0.000625f * (float)delta, current + 0.000125f * (float)delta);
	}

	public void RequestUpdate()
	{
		m_UpdateState.ForceUpdate();
	}

	private void WriteResidentialLowFactors(IJsonWriter writer)
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		JobHandle deps;
		NativeArray<int> lowDensityDemandFactors = m_ResidentialDemandSystem.GetLowDensityDemandFactors(out deps);
		WriteDemandFactors(writer, lowDensityDemandFactors, deps);
	}

	private void WriteResidentialMediumFactors(IJsonWriter writer)
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		JobHandle deps;
		NativeArray<int> mediumDensityDemandFactors = m_ResidentialDemandSystem.GetMediumDensityDemandFactors(out deps);
		WriteDemandFactors(writer, mediumDensityDemandFactors, deps);
	}

	private void WriteResidentialHighFactors(IJsonWriter writer)
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		JobHandle deps;
		NativeArray<int> highDensityDemandFactors = m_ResidentialDemandSystem.GetHighDensityDemandFactors(out deps);
		WriteDemandFactors(writer, highDensityDemandFactors, deps);
	}

	private void WriteCommercialFactors(IJsonWriter writer)
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		JobHandle deps;
		NativeArray<int> demandFactors = m_CommercialDemandSystem.GetDemandFactors(out deps);
		WriteDemandFactors(writer, demandFactors, deps);
	}

	private void WriteIndustrialFactors(IJsonWriter writer)
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		JobHandle deps;
		NativeArray<int> industrialDemandFactors = m_IndustrialDemandSystem.GetIndustrialDemandFactors(out deps);
		WriteDemandFactors(writer, industrialDemandFactors, deps);
	}

	private void WriteOfficeFactors(IJsonWriter writer)
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		JobHandle deps;
		NativeArray<int> officeDemandFactors = m_IndustrialDemandSystem.GetOfficeDemandFactors(out deps);
		WriteDemandFactors(writer, officeDemandFactors, deps);
	}

	private void WriteDemandFactors(IJsonWriter writer, NativeArray<int> factors, JobHandle deps)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		((JobHandle)(ref deps)).Complete();
		NativeList<FactorInfo> val = FactorInfo.FromFactorArray(factors, (Allocator)2);
		NativeSortExtension.Sort<FactorInfo>(val);
		try
		{
			int num = math.min(5, val.Length);
			JsonWriterExtensions.ArrayBegin(writer, num);
			for (int i = 0; i < num; i++)
			{
				val[i].WriteDemandFactor(writer);
			}
			writer.ArrayEnd();
		}
		finally
		{
			val.Dispose();
		}
	}

	private void WriteHappinessFactors(IJsonWriter writer)
	{
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		//IL_0070: Unknown result type (might be due to invalid IL or missing references)
		//IL_0073: Unknown result type (might be due to invalid IL or missing references)
		NativeList<FactorInfo> val = default(NativeList<FactorInfo>);
		val._002Ector(25, AllocatorHandle.op_Implicit((Allocator)2));
		EntityQuery entityQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[1] { ComponentType.ReadOnly<HappinessFactorParameterData>() });
		if (!((EntityQuery)(ref entityQuery)).IsEmptyIgnoreFilter)
		{
			Entity singletonEntity = ((EntityQuery)(ref entityQuery)).GetSingletonEntity();
			EntityManager entityManager = ((ComponentSystemBase)this).EntityManager;
			DynamicBuffer<HappinessFactorParameterData> buffer = ((EntityManager)(ref entityManager)).GetBuffer<HappinessFactorParameterData>(singletonEntity, true);
			ComponentLookup<Locked> locked = InternalCompilerInterface.GetComponentLookup<Locked>(ref __TypeHandle.__Game_Prefabs_Locked_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef);
			for (int i = 0; i < 25; i++)
			{
				int num = Mathf.RoundToInt(m_CitizenHappinessSystem.GetHappinessFactor((CitizenHappinessSystem.HappinessFactor)i, buffer, ref locked).x);
				if (num != 0)
				{
					FactorInfo factorInfo = new FactorInfo(i, num);
					val.Add(ref factorInfo);
				}
			}
		}
		NativeSortExtension.Sort<FactorInfo>(val);
		try
		{
			int num2 = math.min(10, val.Length);
			JsonWriterExtensions.ArrayBegin(writer, num2);
			for (int j = 0; j < num2; j++)
			{
				val[j].WriteHappinessFactor(writer);
			}
			writer.ArrayEnd();
		}
		finally
		{
			val.Dispose();
		}
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private void __AssignQueries(ref SystemState state)
	{
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		EntityQueryBuilder val = default(EntityQueryBuilder);
		((EntityQueryBuilder)(ref val))._002Ector(AllocatorHandle.op_Implicit((Allocator)2));
		((EntityQueryBuilder)(ref val)).Dispose();
	}

	protected override void OnCreateForCompiler()
	{
		((ComponentSystemBase)this).OnCreateForCompiler();
		__AssignQueries(ref ((SystemBase)this).CheckedStateRef);
		__TypeHandle.__AssignHandles(ref ((SystemBase)this).CheckedStateRef);
	}

	[Preserve]
	public CityInfoUISystem()
	{
	}
}

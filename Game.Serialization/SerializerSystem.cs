using System;
using System.Collections.Generic;
using System.Linq;
using Colossal;
using Colossal.Entities;
using Colossal.Serialization.Entities;
using Game.Agents;
using Game.Citizens;
using Game.City;
using Game.Common;
using Game.Effects;
using Game.Prefabs;
using Game.Routes;
using Game.Simulation;
using Game.Tools;
using Unity.Collections;
using Unity.Entities;
using UnityEngine.Scripting;

namespace Game.Serialization;

public class SerializerSystem : GameSystemBase
{
	private SaveGameSystem m_SaveGameSystem;

	private LoadGameSystem m_LoadGameSystem;

	private WriteSystem m_WriteSystem;

	private ReadSystem m_ReadSystem;

	private UpdateSystem m_UpdateSystem;

	private ComponentSerializerLibrary m_ComponentSerializerLibrary;

	private SystemSerializerLibrary m_SystemSerializerLibrary;

	private EntityQuery m_Query;

	public ComponentSerializerLibrary componentLibrary => m_ComponentSerializerLibrary;

	public SystemSerializerLibrary systemLibrary => m_SystemSerializerLibrary;

	public int totalSize { get; set; }

	[Preserve]
	protected override void OnCreate()
	{
		base.OnCreate();
		m_SaveGameSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<SaveGameSystem>();
		m_LoadGameSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<LoadGameSystem>();
		m_WriteSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<WriteSystem>();
		m_ReadSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<ReadSystem>();
		m_UpdateSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<UpdateSystem>();
		CreateQuery(Array.Empty<ComponentType>());
	}

	[Preserve]
	protected override void OnDestroy()
	{
		if (m_ComponentSerializerLibrary != null)
		{
			m_ComponentSerializerLibrary.Dispose();
		}
		if (m_SystemSerializerLibrary != null)
		{
			m_SystemSerializerLibrary.Dispose();
		}
		base.OnDestroy();
	}

	[Preserve]
	protected override void OnUpdate()
	{
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Expected O, but got Unknown
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Expected O, but got Unknown
		//IL_007e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_00af: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00db: Unknown result type (might be due to invalid IL or missing references)
		//IL_0106: Unknown result type (might be due to invalid IL or missing references)
		//IL_010b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0127: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d5: Invalid comparison between Unknown and I4
		//IL_01e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01da: Invalid comparison between Unknown and I4
		//IL_01ef: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ed: Unknown result type (might be due to invalid IL or missing references)
		//IL_01dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_0174: Unknown result type (might be due to invalid IL or missing references)
		//IL_0179: Unknown result type (might be due to invalid IL or missing references)
		//IL_0229: Unknown result type (might be due to invalid IL or missing references)
		//IL_0203: Unknown result type (might be due to invalid IL or missing references)
		//IL_0205: Unknown result type (might be due to invalid IL or missing references)
		//IL_020a: Unknown result type (might be due to invalid IL or missing references)
		if (m_ComponentSerializerLibrary == null)
		{
			m_ComponentSerializerLibrary = new ComponentSerializerLibrary();
		}
		if (m_ComponentSerializerLibrary.isDirty)
		{
			List<ComponentType> serializableComponents = default(List<ComponentType>);
			m_ComponentSerializerLibrary.Initialize((SystemBase)(object)this, ref serializableComponents);
			CreateQuery(serializableComponents);
		}
		if (m_SystemSerializerLibrary == null)
		{
			m_SystemSerializerLibrary = new SystemSerializerLibrary();
		}
		if (m_SystemSerializerLibrary.isDirty)
		{
			m_SystemSerializerLibrary.Initialize(((ComponentSystemBase)this).World);
		}
		switch (m_UpdateSystem.currentPhase)
		{
		case SystemUpdatePhase.Serialize:
		{
			EntitySerializer<WriteBuffer> val3 = new EntitySerializer<WriteBuffer>(((ComponentSystemBase)this).EntityManager, m_ComponentSerializerLibrary, m_SystemSerializerLibrary, (IWriteBufferProvider<WriteBuffer>)m_WriteSystem);
			try
			{
				totalSize = 0;
				Context context2 = m_SaveGameSystem.context;
				val3.Serialize<BinaryWriter, FormatTags>(context2, m_Query, (BufferFormat)2, (ComponentType[])(object)new ComponentType[1] { ComponentType.ReadWrite<PrefabData>() });
				break;
			}
			finally
			{
				val3.Dispose();
			}
		}
		case SystemUpdatePhase.Deserialize:
		{
			EntityDeserializer<ReadBuffer> val = new EntityDeserializer<ReadBuffer>(((ComponentSystemBase)this).EntityManager, m_ComponentSerializerLibrary, m_SystemSerializerLibrary, (IReadBufferProvider<ReadBuffer>)m_ReadSystem);
			try
			{
				totalSize = 0;
				Context context = m_LoadGameSystem.context;
				bool num = val.Deserialize<BinaryReader, FormatTags>(ref context, Array.Empty<ComponentType>());
				COSystemBase.baseLog.InfoFormat("Serialized version: {0}", (object)((Context)(ref context)).version);
				if (num)
				{
					string[] names = Enum.GetNames(typeof(FormatTags));
					FormatTags[] array = (FormatTags[])Enum.GetValues(typeof(FormatTags));
					List<string> list = new List<string>(names.Length);
					for (int i = 0; i < array.Length; i++)
					{
						ContextFormat format = ((Context)(ref context)).format;
						if (((ContextFormat)(ref format)).Has<FormatTags>(array[i]))
						{
							list.Add(names[i]);
						}
					}
					COSystemBase.baseLog.InfoFormat("Format tags: {0}", (object)string.Join(", ", list));
				}
				else
				{
					Hash128 instigatorGuid = ((Context)(ref context)).instigatorGuid;
					Purpose purpose = ((Context)(ref context)).purpose;
					Purpose val2 = (((int)purpose == 2) ? ((Purpose)1) : (((int)purpose != 5) ? ((Context)(ref context)).purpose : ((Purpose)4)));
					if (val2 != ((Context)(ref context)).purpose)
					{
						((Context)(ref context)).Dispose();
						((Context)(ref context))._002Ector(val2, Version.current, instigatorGuid, Enum.GetNames(typeof(FormatTags)).Length, (Allocator)4);
					}
				}
				m_LoadGameSystem.context = context;
				break;
			}
			finally
			{
				val.Dispose();
			}
		}
		}
	}

	public void SetDirty()
	{
		if (m_ComponentSerializerLibrary != null)
		{
			m_ComponentSerializerLibrary.SetDirty();
		}
		if (m_SystemSerializerLibrary != null)
		{
			m_SystemSerializerLibrary.SetDirty();
		}
	}

	private void CreateQuery(IEnumerable<ComponentType> serializableComponents)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		//IL_007e: Unknown result type (might be due to invalid IL or missing references)
		//IL_008a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0096: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00de: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0101: Unknown result type (might be due to invalid IL or missing references)
		//IL_012a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0130: Expected O, but got Unknown
		//IL_0145: Unknown result type (might be due to invalid IL or missing references)
		//IL_014a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0151: Unknown result type (might be due to invalid IL or missing references)
		//IL_0156: Unknown result type (might be due to invalid IL or missing references)
		//IL_015d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0162: Unknown result type (might be due to invalid IL or missing references)
		//IL_0169: Unknown result type (might be due to invalid IL or missing references)
		//IL_016e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0175: Unknown result type (might be due to invalid IL or missing references)
		//IL_017a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0186: Unknown result type (might be due to invalid IL or missing references)
		//IL_018b: Unknown result type (might be due to invalid IL or missing references)
		HashSet<ComponentType> hashSet = new HashSet<ComponentType>
		{
			ComponentType.ReadOnly<LoadedIndex>(),
			ComponentType.ReadOnly<PrefabRef>(),
			ComponentType.ReadOnly<ElectricityFlowNode>(),
			ComponentType.ReadOnly<ElectricityFlowEdge>(),
			ComponentType.ReadOnly<WaterPipeNode>(),
			ComponentType.ReadOnly<WaterPipeEdge>(),
			ComponentType.ReadOnly<ServiceRequest>(),
			ComponentType.ReadOnly<Game.Simulation.WaterSourceData>(),
			ComponentType.ReadOnly<Game.City.City>(),
			ComponentType.ReadOnly<SchoolSeeker>(),
			ComponentType.ReadOnly<JobSeeker>(),
			ComponentType.ReadOnly<CityStatistic>(),
			ComponentType.ReadOnly<ServiceBudgetData>(),
			ComponentType.ReadOnly<FloodCounterData>(),
			ComponentType.ReadOnly<CoordinatedMeeting>(),
			ComponentType.ReadOnly<LookingForPartner>(),
			ComponentType.ReadOnly<AtmosphereData>(),
			ComponentType.ReadOnly<BiomeData>(),
			ComponentType.ReadOnly<TimeData>()
		};
		foreach (ComponentType serializableComponent in serializableComponents)
		{
			hashSet.Add(ComponentType.ReadOnly(serializableComponent.TypeIndex));
		}
		EntityQueryDesc[] array = new EntityQueryDesc[1];
		EntityQueryDesc val = new EntityQueryDesc();
		val.Any = hashSet.ToArray();
		val.None = (ComponentType[])(object)new ComponentType[5]
		{
			ComponentType.ReadOnly<NetCompositionData>(),
			ComponentType.ReadOnly<EffectInstance>(),
			ComponentType.ReadOnly<LivePath>(),
			ComponentType.ReadOnly<Temp>(),
			ComponentType.ReadOnly<Deleted>()
		};
		array[0] = val;
		m_Query = ((ComponentSystemBase)this).GetEntityQuery((EntityQueryDesc[])(object)array);
	}

	[Preserve]
	public SerializerSystem()
	{
	}
}

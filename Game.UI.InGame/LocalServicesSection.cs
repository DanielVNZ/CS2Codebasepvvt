using System.Runtime.CompilerServices;
using Colossal.UI.Binding;
using Game.Areas;
using Game.Buildings;
using Game.Common;
using Game.Prefabs;
using Game.Tools;
using Unity.Collections;
using Unity.Entities;
using UnityEngine.Scripting;

namespace Game.UI.InGame;

[CompilerGenerated]
public class LocalServicesSection : InfoSectionBase
{
	private ImageSystem m_ImageSystem;

	private EntityQuery m_ServiceDistrictBuildingQuery;

	protected override string group => "LocalServicesSection";

	private NativeList<Entity> localServiceBuildings { get; set; }

	private NativeList<Entity> prefabs { get; set; }

	protected override void Reset()
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		localServiceBuildings.Clear();
		prefabs.Clear();
	}

	[Preserve]
	protected override void OnCreate()
	{
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
		//IL_007d: Unknown result type (might be due to invalid IL or missing references)
		base.OnCreate();
		m_ImageSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<ImageSystem>();
		m_ServiceDistrictBuildingQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[5]
		{
			ComponentType.ReadOnly<Building>(),
			ComponentType.ReadOnly<PrefabRef>(),
			ComponentType.ReadOnly<ServiceDistrict>(),
			ComponentType.Exclude<Temp>(),
			ComponentType.Exclude<Deleted>()
		});
		localServiceBuildings = new NativeList<Entity>(AllocatorHandle.op_Implicit((Allocator)4));
		prefabs = new NativeList<Entity>(AllocatorHandle.op_Implicit((Allocator)4));
	}

	[Preserve]
	protected override void OnDestroy()
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		base.OnDestroy();
		localServiceBuildings.Dispose();
		prefabs.Dispose();
	}

	[Preserve]
	protected override void OnUpdate()
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		EntityManager entityManager = ((ComponentSystemBase)this).EntityManager;
		int num;
		if (((EntityManager)(ref entityManager)).HasComponent<District>(selectedEntity))
		{
			entityManager = ((ComponentSystemBase)this).EntityManager;
			num = (((EntityManager)(ref entityManager)).HasComponent<Area>(selectedEntity) ? 1 : 0);
		}
		else
		{
			num = 0;
		}
		base.visible = (byte)num != 0;
	}

	protected override void OnProcess()
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		//IL_0076: Unknown result type (might be due to invalid IL or missing references)
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		//IL_0085: Unknown result type (might be due to invalid IL or missing references)
		NativeArray<Entity> val = ((EntityQuery)(ref m_ServiceDistrictBuildingQuery)).ToEntityArray(AllocatorHandle.op_Implicit((Allocator)3));
		NativeArray<PrefabRef> val2 = ((EntityQuery)(ref m_ServiceDistrictBuildingQuery)).ToComponentDataArray<PrefabRef>(AllocatorHandle.op_Implicit((Allocator)3));
		try
		{
			for (int i = 0; i < val.Length; i++)
			{
				EntityManager entityManager = ((ComponentSystemBase)this).EntityManager;
				DynamicBuffer<ServiceDistrict> buffer = ((EntityManager)(ref entityManager)).GetBuffer<ServiceDistrict>(val[i], true);
				for (int j = 0; j < buffer.Length; j++)
				{
					if (buffer[j].m_District == selectedEntity)
					{
						NativeList<Entity> val3 = localServiceBuildings;
						Entity val4 = val[i];
						val3.Add(ref val4);
						val3 = prefabs;
						PrefabRef prefabRef = val2[i];
						val3.Add(ref prefabRef.m_Prefab);
						break;
					}
				}
			}
		}
		finally
		{
			val.Dispose();
			val2.Dispose();
		}
	}

	public override void OnWriteProperties(IJsonWriter writer)
	{
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0073: Unknown result type (might be due to invalid IL or missing references)
		//IL_008f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0094: Unknown result type (might be due to invalid IL or missing references)
		//IL_0098: Unknown result type (might be due to invalid IL or missing references)
		writer.PropertyName("localServiceBuildings");
		JsonWriterExtensions.ArrayBegin(writer, localServiceBuildings.Length);
		for (int i = 0; i < localServiceBuildings.Length; i++)
		{
			writer.TypeBegin("selectedInfo.LocalServiceBuilding");
			writer.PropertyName("name");
			m_NameSystem.BindName(writer, localServiceBuildings[i]);
			writer.PropertyName("serviceIcon");
			writer.Write(m_ImageSystem.GetGroupIcon(prefabs[i]));
			writer.PropertyName("entity");
			UnityWriters.Write(writer, localServiceBuildings[i]);
			writer.TypeEnd();
		}
		writer.ArrayEnd();
	}

	[Preserve]
	public LocalServicesSection()
	{
	}
}

using System;
using System.Runtime.CompilerServices;
using Colossal.Entities;
using Colossal.Serialization.Entities;
using Game.Common;
using Game.Objects;
using Game.Prefabs;
using Game.Serialization;
using Game.Tools;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Scripting;

namespace Game.Simulation;

[CompilerGenerated]
public class DiversitySystem : GameSystemBase, IPostDeserialize
{
	private EntityQuery m_AtmosphereQuery;

	private EntityQuery m_AtmospherePrefabQuery;

	private EntityQuery m_BiomeQuery;

	private EntityQuery m_BiomePrefabQuery;

	private EntityQuery m_EditorContainerQuery;

	public void ApplyAtmospherePreset(Entity atmospherePrefab)
	{
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		AtmosphereData singleton = ((EntityQuery)(ref m_AtmosphereQuery)).GetSingleton<AtmosphereData>();
		singleton.m_AtmospherePrefab = atmospherePrefab;
		((EntityQuery)(ref m_AtmosphereQuery)).SetSingleton<AtmosphereData>(singleton);
	}

	public void ApplyBiomePreset(Entity biomePrefab)
	{
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		BiomeData singleton = ((EntityQuery)(ref m_BiomeQuery)).GetSingleton<BiomeData>();
		singleton.m_BiomePrefab = biomePrefab;
		((EntityQuery)(ref m_BiomeQuery)).SetSingleton<BiomeData>(singleton);
	}

	public unsafe void PostDeserialize(Context context)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		//IL_0077: Unknown result type (might be due to invalid IL or missing references)
		//IL_0079: Unknown result type (might be due to invalid IL or missing references)
		//IL_007e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0082: Unknown result type (might be due to invalid IL or missing references)
		//IL_0087: Unknown result type (might be due to invalid IL or missing references)
		//IL_0089: Unknown result type (might be due to invalid IL or missing references)
		//IL_008e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0092: Unknown result type (might be due to invalid IL or missing references)
		//IL_0093: Unknown result type (might be due to invalid IL or missing references)
		//IL_00af: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0101: Unknown result type (might be due to invalid IL or missing references)
		//IL_0105: Unknown result type (might be due to invalid IL or missing references)
		//IL_010a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0113: Unknown result type (might be due to invalid IL or missing references)
		//IL_0118: Unknown result type (might be due to invalid IL or missing references)
		//IL_011b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0120: Unknown result type (might be due to invalid IL or missing references)
		//IL_0132: Unknown result type (might be due to invalid IL or missing references)
		//IL_0143: Unknown result type (might be due to invalid IL or missing references)
		//IL_0145: Unknown result type (might be due to invalid IL or missing references)
		//IL_015f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0164: Unknown result type (might be due to invalid IL or missing references)
		//IL_0170: Unknown result type (might be due to invalid IL or missing references)
		//IL_0177: Unknown result type (might be due to invalid IL or missing references)
		//IL_0186: Unknown result type (might be due to invalid IL or missing references)
		//IL_018b: Unknown result type (might be due to invalid IL or missing references)
		//IL_018f: Unknown result type (might be due to invalid IL or missing references)
		NativeArray<Entity> val = ((EntityQuery)(ref m_AtmospherePrefabQuery)).ToEntityArray(AllocatorHandle.op_Implicit((Allocator)2));
		if (val.Length == 0)
		{
			COSystemBase.baseLog.InfoFormat("WARNING: PostDeserialize({0}): no Atmosphere prefabs found", (object)context);
			return;
		}
		NativeArray<Entity> val2 = ((EntityQuery)(ref m_BiomePrefabQuery)).ToEntityArray(AllocatorHandle.op_Implicit((Allocator)2));
		if (val2.Length == 0)
		{
			COSystemBase.baseLog.InfoFormat("WARNING: PostDeserialize({0}): no Biome prefabs found", (object)context);
			return;
		}
		EntityManager entityManager;
		if (((EntityQuery)(ref m_AtmosphereQuery)).IsEmptyIgnoreFilter)
		{
			Entity prefab = val[0];
			entityManager = ((ComponentSystemBase)this).EntityManager;
			Entity val3 = ((EntityManager)(ref entityManager)).CreateEntity();
			entityManager = ((ComponentSystemBase)this).EntityManager;
			((EntityManager)(ref entityManager)).AddComponentData<AtmosphereData>(val3, new AtmosphereData(prefab));
		}
		if (((EntityQuery)(ref m_BiomeQuery)).IsEmptyIgnoreFilter)
		{
			Entity prefab2 = val2[0];
			entityManager = ((ComponentSystemBase)this).EntityManager;
			Entity val4 = ((EntityManager)(ref entityManager)).CreateEntity();
			entityManager = ((ComponentSystemBase)this).EntityManager;
			((EntityManager)(ref entityManager)).AddComponentData<BiomeData>(val4, new BiomeData(prefab2));
		}
		if (((EntityQuery)(ref m_EditorContainerQuery)).IsEmptyIgnoreFilter)
		{
			return;
		}
		NativeArray<Entity> val5 = ((EntityQuery)(ref m_EditorContainerQuery)).ToEntityArray(AllocatorHandle.op_Implicit((Allocator)3));
		Enumerator<Entity> enumerator = val5.GetEnumerator();
		try
		{
			Transform transform = default(Transform);
			Owner owner = default(Owner);
			Transform transform2 = default(Transform);
			while (enumerator.MoveNext())
			{
				Entity current = enumerator.Current;
				if (EntitiesExtensions.TryGetComponent<Transform>(((ComponentSystemBase)this).EntityManager, current, ref transform) && ((float3)(ref transform.m_Position)).Equals(float3.zero))
				{
					Entity val6 = current;
					Debug.Log((object)("There is invalid EditorContainer in the map:" + ((object)(*(Entity*)(&val6))/*cast due to .constrained prefix*/).ToString()));
					if (EntitiesExtensions.TryGetComponent<Owner>(((ComponentSystemBase)this).EntityManager, current, ref owner) && EntitiesExtensions.TryGetComponent<Transform>(((ComponentSystemBase)this).EntityManager, owner.m_Owner, ref transform2))
					{
						entityManager = ((ComponentSystemBase)this).EntityManager;
						((EntityManager)(ref entityManager)).SetComponentData<Transform>(current, transform2);
					}
				}
			}
		}
		finally
		{
			((IDisposable)enumerator/*cast due to .constrained prefix*/).Dispose();
		}
		val5.Dispose();
	}

	[Preserve]
	protected override void OnCreate()
	{
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0074: Unknown result type (might be due to invalid IL or missing references)
		//IL_0079: Unknown result type (might be due to invalid IL or missing references)
		//IL_0088: Unknown result type (might be due to invalid IL or missing references)
		//IL_008d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0094: Unknown result type (might be due to invalid IL or missing references)
		//IL_0099: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_00af: Unknown result type (might be due to invalid IL or missing references)
		base.OnCreate();
		m_AtmosphereQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[1] { ComponentType.ReadOnly<AtmosphereData>() });
		m_AtmospherePrefabQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[1] { ComponentType.ReadOnly<AtmospherePrefabData>() });
		m_BiomeQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[1] { ComponentType.ReadOnly<BiomeData>() });
		m_BiomePrefabQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[1] { ComponentType.ReadOnly<BiomePrefabData>() });
		m_EditorContainerQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[3]
		{
			ComponentType.ReadOnly<Game.Tools.EditorContainer>(),
			ComponentType.ReadOnly<Transform>(),
			ComponentType.ReadOnly<Owner>()
		});
	}

	[Preserve]
	protected override void OnUpdate()
	{
	}

	[Preserve]
	public DiversitySystem()
	{
	}
}

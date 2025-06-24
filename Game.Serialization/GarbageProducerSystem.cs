using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Colossal.Entities;
using Colossal.Serialization.Entities;
using Game.Buildings;
using Game.Notifications;
using Game.Prefabs;
using Unity.Collections;
using Unity.Entities;
using UnityEngine.Scripting;

namespace Game.Serialization;

[CompilerGenerated]
public class GarbageProducerSystem : GameSystemBase, IPostDeserialize
{
	[StructLayout(LayoutKind.Sequential, Size = 1)]
	private struct TypeHandle
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void __AssignHandles(ref SystemState state)
		{
		}
	}

	private EntityQuery m_Query;

	private TypeHandle __TypeHandle;

	private EntityQuery __query_31347557_0;

	[Preserve]
	protected override void OnUpdate()
	{
	}

	public void PostDeserialize(Context context)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		//IL_007e: Unknown result type (might be due to invalid IL or missing references)
		//IL_008c: Unknown result type (might be due to invalid IL or missing references)
		//IL_009c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00af: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00de: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ea: Unknown result type (might be due to invalid IL or missing references)
		if (!(((Context)(ref context)).version < Version.garbageProducerFlags))
		{
			return;
		}
		EntityManager entityManager = ((ComponentSystemBase)this).EntityManager;
		EntityQuery val = ((EntityManager)(ref entityManager)).CreateEntityQuery((ComponentType[])(object)new ComponentType[1] { ComponentType.ReadOnly<GarbageProducer>() });
		GarbageParameterData singleton = ((EntityQuery)(ref __query_31347557_0)).GetSingleton<GarbageParameterData>();
		NativeArray<Entity> val2 = ((EntityQuery)(ref val)).ToEntityArray(AllocatorHandle.op_Implicit((Allocator)3));
		DynamicBuffer<IconElement> val3 = default(DynamicBuffer<IconElement>);
		PrefabRef prefabRef = default(PrefabRef);
		GarbageProducer garbageProducer = default(GarbageProducer);
		for (int i = 0; i < val2.Length; i++)
		{
			if (!EntitiesExtensions.TryGetBuffer<IconElement>(((ComponentSystemBase)this).EntityManager, val2[i], true, ref val3))
			{
				continue;
			}
			for (int j = 0; j < val3.Length; j++)
			{
				if (EntitiesExtensions.TryGetComponent<PrefabRef>(((ComponentSystemBase)this).EntityManager, val3[j].m_Icon, ref prefabRef) && prefabRef.m_Prefab == singleton.m_GarbageNotificationPrefab)
				{
					if (EntitiesExtensions.TryGetComponent<GarbageProducer>(((ComponentSystemBase)this).EntityManager, val2[i], ref garbageProducer) && (garbageProducer.m_Flags & GarbageProducerFlags.GarbagePilingUpWarning) == 0)
					{
						garbageProducer.m_Flags |= GarbageProducerFlags.GarbagePilingUpWarning;
						entityManager = ((ComponentSystemBase)this).EntityManager;
						((EntityManager)(ref entityManager)).SetComponentData<GarbageProducer>(val2[i], garbageProducer);
					}
					break;
				}
			}
		}
		val2.Dispose();
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private void __AssignQueries(ref SystemState state)
	{
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		EntityQueryBuilder val = default(EntityQueryBuilder);
		((EntityQueryBuilder)(ref val))._002Ector(AllocatorHandle.op_Implicit((Allocator)2));
		EntityQueryBuilder val2 = ((EntityQueryBuilder)(ref val)).WithAll<GarbageParameterData>();
		val2 = ((EntityQueryBuilder)(ref val2)).WithOptions((EntityQueryOptions)16);
		__query_31347557_0 = ((EntityQueryBuilder)(ref val2)).Build(ref state);
		((EntityQueryBuilder)(ref val)).Reset();
		((EntityQueryBuilder)(ref val)).Dispose();
	}

	protected override void OnCreateForCompiler()
	{
		((ComponentSystemBase)this).OnCreateForCompiler();
		__AssignQueries(ref ((SystemBase)this).CheckedStateRef);
		__TypeHandle.__AssignHandles(ref ((SystemBase)this).CheckedStateRef);
	}

	[Preserve]
	public GarbageProducerSystem()
	{
	}
}

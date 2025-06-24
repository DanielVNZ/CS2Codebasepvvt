using System.Runtime.CompilerServices;
using Colossal.Entities;
using Colossal.Mathematics;
using Colossal.Serialization.Entities;
using Game.Citizens;
using Game.Common;
using Game.Notifications;
using Game.Prefabs;
using Game.Tools;
using Game.UI.InGame;
using Unity.Burst;
using Unity.Burst.Intrinsics;
using Unity.Collections;
using Unity.Entities;
using Unity.Entities.Internal;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Scripting;

namespace Game.Rendering;

[CompilerGenerated]
public class MarkerIconSystem : GameSystemBase, IDefaultSerializable, ISerializable
{
	public enum MarkerType
	{
		Selected,
		Followed
	}

	private struct Overlap
	{
		public Entity m_Entity;

		public Entity m_Other;

		public Overlap(Entity entity, Entity other)
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			//IL_0008: Unknown result type (might be due to invalid IL or missing references)
			//IL_0009: Unknown result type (might be due to invalid IL or missing references)
			m_Entity = entity;
			m_Other = other;
		}
	}

	[BurstCompile]
	private struct FindOverlapIconsJob : IJobChunk
	{
		[ReadOnly]
		public EntityTypeHandle m_EntityType;

		[ReadOnly]
		public ComponentTypeHandle<Icon> m_IconType;

		[ReadOnly]
		public Entity m_Entity1;

		[ReadOnly]
		public Entity m_Entity2;

		[ReadOnly]
		public float3 m_Location1;

		[ReadOnly]
		public float3 m_Location2;

		public ParallelWriter<Overlap> m_OverlapQueue;

		public void Execute(in ArchetypeChunk chunk, int unfilteredChunkIndex, bool useEnabledMask, in v128 chunkEnabledMask)
		{
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0014: Unknown result type (might be due to invalid IL or missing references)
			//IL_0019: Unknown result type (might be due to invalid IL or missing references)
			//IL_002a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0030: Unknown result type (might be due to invalid IL or missing references)
			//IL_0074: Unknown result type (might be due to invalid IL or missing references)
			//IL_007a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0044: Unknown result type (might be due to invalid IL or missing references)
			//IL_004a: Unknown result type (might be due to invalid IL or missing references)
			//IL_008e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0094: Unknown result type (might be due to invalid IL or missing references)
			//IL_005d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0065: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a7: Unknown result type (might be due to invalid IL or missing references)
			//IL_00af: Unknown result type (might be due to invalid IL or missing references)
			NativeArray<Entity> nativeArray = ((ArchetypeChunk)(ref chunk)).GetNativeArray(m_EntityType);
			NativeArray<Icon> nativeArray2 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<Icon>(ref m_IconType);
			for (int i = 0; i < nativeArray2.Length; i++)
			{
				Icon icon = nativeArray2[i];
				if (math.distancesq(icon.m_Location, m_Location1) < 0.01f && nativeArray[i] != m_Entity1)
				{
					m_OverlapQueue.Enqueue(new Overlap(m_Entity1, nativeArray[i]));
				}
				if (math.distancesq(icon.m_Location, m_Location2) < 0.01f && nativeArray[i] != m_Entity2)
				{
					m_OverlapQueue.Enqueue(new Overlap(m_Entity2, nativeArray[i]));
				}
			}
		}

		void IJobChunk.Execute(in ArchetypeChunk chunk, int unfilteredChunkIndex, bool useEnabledMask, in v128 chunkEnabledMask)
		{
			Execute(in chunk, unfilteredChunkIndex, useEnabledMask, in chunkEnabledMask);
		}
	}

	[BurstCompile]
	private struct UpdateMarkerLocationJob : IJob
	{
		[ReadOnly]
		public ComponentLookup<PrefabRef> m_PrefabRefData;

		[ReadOnly]
		public ComponentLookup<NotificationIconDisplayData> m_IconDisplayData;

		[ReadOnly]
		public Entity m_Entity1;

		[ReadOnly]
		public Entity m_Entity2;

		[ReadOnly]
		public float3 m_Location1;

		[ReadOnly]
		public float3 m_Location2;

		[ReadOnly]
		public float3 m_CameraPos;

		[ReadOnly]
		public float3 m_CameraUp;

		public ComponentLookup<Icon> m_IconData;

		public NativeQueue<Overlap> m_OverlapQueue;

		public void Execute()
		{
			//IL_0011: Unknown result type (might be due to invalid IL or missing references)
			//IL_0016: Unknown result type (might be due to invalid IL or missing references)
			//IL_0035: Unknown result type (might be due to invalid IL or missing references)
			//IL_003a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0029: Unknown result type (might be due to invalid IL or missing references)
			//IL_004d: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c8: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ce: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d4: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d9: Unknown result type (might be due to invalid IL or missing references)
			//IL_00de: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e6: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ec: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f7: Unknown result type (might be due to invalid IL or missing references)
			//IL_00fc: Unknown result type (might be due to invalid IL or missing references)
			//IL_0102: Unknown result type (might be due to invalid IL or missing references)
			//IL_0107: Unknown result type (might be due to invalid IL or missing references)
			//IL_0075: Unknown result type (might be due to invalid IL or missing references)
			//IL_007b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0126: Unknown result type (might be due to invalid IL or missing references)
			//IL_012b: Unknown result type (might be due to invalid IL or missing references)
			//IL_011a: Unknown result type (might be due to invalid IL or missing references)
			//IL_009d: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
			//IL_008b: Unknown result type (might be due to invalid IL or missing references)
			//IL_013e: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b3: Unknown result type (might be due to invalid IL or missing references)
			Icon icon = default(Icon);
			Icon icon2 = default(Icon);
			if (m_Entity1 != Entity.Null)
			{
				icon = m_IconData[m_Entity1];
			}
			if (m_Entity2 != Entity.Null)
			{
				icon2 = m_IconData[m_Entity2];
			}
			float num = 0f;
			float num2 = 0f;
			Overlap overlap = default(Overlap);
			while (m_OverlapQueue.TryDequeue(ref overlap))
			{
				if (overlap.m_Entity == m_Entity1)
				{
					num = math.max(num, CalculateOffset(overlap.m_Other));
				}
				if (overlap.m_Entity == m_Entity2)
				{
					num2 = math.max(num2, CalculateOffset(overlap.m_Other));
				}
			}
			icon.m_Location = m_Location1 + m_CameraUp * num;
			icon2.m_Location = m_Location2 + m_CameraUp * num2;
			if (m_Entity1 != Entity.Null)
			{
				m_IconData[m_Entity1] = icon;
			}
			if (m_Entity2 != Entity.Null)
			{
				m_IconData[m_Entity2] = icon2;
			}
		}

		private float CalculateOffset(Entity entity)
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_0013: Unknown result type (might be due to invalid IL or missing references)
			//IL_0021: Unknown result type (might be due to invalid IL or missing references)
			//IL_003b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0041: Unknown result type (might be due to invalid IL or missing references)
			//IL_0047: Unknown result type (might be due to invalid IL or missing references)
			//IL_004d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0053: Unknown result type (might be due to invalid IL or missing references)
			Icon icon = m_IconData[entity];
			PrefabRef prefabRef = m_PrefabRefData[entity];
			NotificationIconDisplayData notificationIconDisplayData = m_IconDisplayData[prefabRef.m_Prefab];
			float num = (float)(int)icon.m_Priority * 0.003921569f;
			float2 val = math.lerp(notificationIconDisplayData.m_MinParams, notificationIconDisplayData.m_MaxParams, num);
			return IconClusterSystem.IconCluster.CalculateRadius(distance: math.distance(icon.m_Location, m_CameraPos), radius: val.x) * 1.5f;
		}
	}

	private struct TypeHandle
	{
		[ReadOnly]
		public EntityTypeHandle __Unity_Entities_Entity_TypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<Icon> __Game_Notifications_Icon_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentLookup<PrefabRef> __Game_Prefabs_PrefabRef_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<NotificationIconDisplayData> __Game_Prefabs_NotificationIconDisplayData_RO_ComponentLookup;

		public ComponentLookup<Icon> __Game_Notifications_Icon_RW_ComponentLookup;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void __AssignHandles(ref SystemState state)
		{
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_000f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0014: Unknown result type (might be due to invalid IL or missing references)
			//IL_001c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0021: Unknown result type (might be due to invalid IL or missing references)
			//IL_0029: Unknown result type (might be due to invalid IL or missing references)
			//IL_002e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0036: Unknown result type (might be due to invalid IL or missing references)
			//IL_003b: Unknown result type (might be due to invalid IL or missing references)
			__Unity_Entities_Entity_TypeHandle = ((SystemState)(ref state)).GetEntityTypeHandle();
			__Game_Notifications_Icon_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<Icon>(true);
			__Game_Prefabs_PrefabRef_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<PrefabRef>(true);
			__Game_Prefabs_NotificationIconDisplayData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<NotificationIconDisplayData>(true);
			__Game_Notifications_Icon_RW_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Icon>(false);
		}
	}

	private Entity m_SelectedMarker;

	private Entity m_FollowedMarker;

	private Entity m_SelectedLocation;

	private Entity m_FollowedLocation;

	private EntityQuery m_ConfigurationQuery;

	private EntityQuery m_IconQuery;

	private CameraUpdateSystem m_CameraUpdateSystem;

	private ToolSystem m_ToolSystem;

	private TypeHandle __TypeHandle;

	[Preserve]
	protected override void OnCreate()
	{
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		//IL_0077: Unknown result type (might be due to invalid IL or missing references)
		base.OnCreate();
		m_ToolSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<ToolSystem>();
		m_CameraUpdateSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<CameraUpdateSystem>();
		m_ConfigurationQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[1] { ComponentType.ReadOnly<IconConfigurationData>() });
		m_IconQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[3]
		{
			ComponentType.ReadOnly<Icon>(),
			ComponentType.Exclude<Deleted>(),
			ComponentType.Exclude<Temp>()
		});
	}

	[Preserve]
	protected override void OnUpdate()
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		//IL_0073: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_009f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_007f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0081: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_0098: Unknown result type (might be due to invalid IL or missing references)
		//IL_009d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00be: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_0109: Unknown result type (might be due to invalid IL or missing references)
		//IL_010a: Unknown result type (might be due to invalid IL or missing references)
		//IL_00de: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e0: Unknown result type (might be due to invalid IL or missing references)
		//IL_0177: Unknown result type (might be due to invalid IL or missing references)
		//IL_017a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0116: Unknown result type (might be due to invalid IL or missing references)
		//IL_0119: Unknown result type (might be due to invalid IL or missing references)
		//IL_0102: Unknown result type (might be due to invalid IL or missing references)
		//IL_0107: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f9: Unknown result type (might be due to invalid IL or missing references)
		//IL_0189: Unknown result type (might be due to invalid IL or missing references)
		//IL_018a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0140: Unknown result type (might be due to invalid IL or missing references)
		//IL_0142: Unknown result type (might be due to invalid IL or missing references)
		//IL_0158: Unknown result type (might be due to invalid IL or missing references)
		//IL_015a: Unknown result type (might be due to invalid IL or missing references)
		//IL_015d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0162: Unknown result type (might be due to invalid IL or missing references)
		//IL_012c: Unknown result type (might be due to invalid IL or missing references)
		//IL_012f: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01fa: Unknown result type (might be due to invalid IL or missing references)
		//IL_0196: Unknown result type (might be due to invalid IL or missing references)
		//IL_0199: Unknown result type (might be due to invalid IL or missing references)
		//IL_020a: Unknown result type (might be due to invalid IL or missing references)
		//IL_020c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0212: Unknown result type (might be due to invalid IL or missing references)
		//IL_0214: Unknown result type (might be due to invalid IL or missing references)
		//IL_021a: Unknown result type (might be due to invalid IL or missing references)
		//IL_021f: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01da: Unknown result type (might be due to invalid IL or missing references)
		//IL_01dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_01af: Unknown result type (might be due to invalid IL or missing references)
		//IL_022c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0231: Unknown result type (might be due to invalid IL or missing references)
		//IL_024b: Unknown result type (might be due to invalid IL or missing references)
		//IL_024d: Unknown result type (might be due to invalid IL or missing references)
		//IL_025a: Unknown result type (might be due to invalid IL or missing references)
		//IL_025f: Unknown result type (might be due to invalid IL or missing references)
		//IL_026f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0274: Unknown result type (might be due to invalid IL or missing references)
		//IL_0279: Unknown result type (might be due to invalid IL or missing references)
		//IL_027e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0283: Unknown result type (might be due to invalid IL or missing references)
		Entity val = m_ToolSystem.selected;
		int elementIndex = m_ToolSystem.selectedIndex;
		Entity val2 = Entity.Null;
		int elementIndex2 = -1;
		if ((Object)(object)m_CameraUpdateSystem.orbitCameraController != (Object)null)
		{
			val2 = m_CameraUpdateSystem.orbitCameraController.followedEntity;
		}
		float3 position = default(float3);
		float3 position2 = default(float3);
		Entity location = Entity.Null;
		Entity location2 = Entity.Null;
		Bounds3 bounds = default(Bounds3);
		Bounds3 bounds2 = default(Bounds3);
		if (val2 != Entity.Null && !SelectedInfoUISystem.TryGetPosition(val2, ((ComponentSystemBase)this).EntityManager, ref elementIndex2, out location, out position2, out bounds, out var rotation))
		{
			location = Entity.Null;
		}
		CurrentTransport currentTransport = default(CurrentTransport);
		if ((val2 == val && elementIndex2 == elementIndex) || (EntitiesExtensions.TryGetComponent<CurrentTransport>(((ComponentSystemBase)this).EntityManager, val2, ref currentTransport) && currentTransport.m_CurrentTransport == val))
		{
			val = Entity.Null;
		}
		if (val != Entity.Null && (!SelectedInfoUISystem.TryGetPosition(val, ((ComponentSystemBase)this).EntityManager, ref elementIndex, out location2, out position, out bounds2, out rotation) || location == location2))
		{
			location2 = Entity.Null;
		}
		if (val2 != Entity.Null)
		{
			if (location != m_FollowedLocation)
			{
				RemoveMarker(ref m_FollowedMarker, location2 == m_FollowedLocation);
			}
			position2.y = bounds.max.y;
			UpdateMarker(ref m_FollowedMarker, val2, MarkerType.Followed, position2, m_SelectedLocation == location);
		}
		else
		{
			RemoveMarker(ref m_FollowedMarker, location2 == m_FollowedLocation);
		}
		if (val != Entity.Null)
		{
			if (location2 != m_SelectedLocation)
			{
				RemoveMarker(ref m_SelectedMarker, location == m_SelectedLocation);
			}
			position.y = bounds2.max.y;
			UpdateMarker(ref m_SelectedMarker, val, MarkerType.Selected, position, m_FollowedLocation == location2);
		}
		else
		{
			RemoveMarker(ref m_SelectedMarker, location == m_SelectedLocation);
		}
		m_FollowedLocation = location;
		m_SelectedLocation = location2;
		if ((m_SelectedMarker != Entity.Null || m_FollowedMarker != Entity.Null) && m_CameraUpdateSystem.activeCameraController != null)
		{
			AdjustLocations(position, position2, float3.op_Implicit(m_CameraUpdateSystem.activeCameraController.position), float3.op_Implicit(Quaternion.Euler(m_CameraUpdateSystem.activeCameraController.rotation) * Vector3.up));
		}
	}

	private void UpdateMarker(ref Entity marker, Entity target, MarkerType markerType, float3 position, bool skipAnimation)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		//IL_006e: Unknown result type (might be due to invalid IL or missing references)
		//IL_007a: Unknown result type (might be due to invalid IL or missing references)
		//IL_007f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0089: Unknown result type (might be due to invalid IL or missing references)
		//IL_008a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0090: Unknown result type (might be due to invalid IL or missing references)
		//IL_0095: Unknown result type (might be due to invalid IL or missing references)
		//IL_0099: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		EntityManager entityManager = ((ComponentSystemBase)this).EntityManager;
		Owner owner = default(Owner);
		if (((EntityManager)(ref entityManager)).HasComponent<Icon>(target) && EntitiesExtensions.TryGetComponent<Owner>(((ComponentSystemBase)this).EntityManager, target, ref owner))
		{
			entityManager = ((ComponentSystemBase)this).EntityManager;
			if (((EntityManager)(ref entityManager)).Exists(owner.m_Owner))
			{
				target = owner.m_Owner;
			}
		}
		if (marker == Entity.Null)
		{
			marker = CreateMarker(target, position, markerType, skipAnimation);
			return;
		}
		entityManager = ((ComponentSystemBase)this).EntityManager;
		Target componentData = ((EntityManager)(ref entityManager)).GetComponentData<Target>(marker);
		if (componentData.m_Target != target)
		{
			componentData.m_Target = target;
			entityManager = ((ComponentSystemBase)this).EntityManager;
			((EntityManager)(ref entityManager)).SetComponentData<Target>(marker, componentData);
		}
	}

	private void RemoveMarker(ref Entity marker, bool skipAnimation)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e7: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_0099: Unknown result type (might be due to invalid IL or missing references)
		//IL_009e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_00af: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_007d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0082: Unknown result type (might be due to invalid IL or missing references)
		//IL_0086: Unknown result type (might be due to invalid IL or missing references)
		if (!(marker != Entity.Null))
		{
			return;
		}
		EntityManager entityManager;
		Game.Notifications.Animation animation = default(Game.Notifications.Animation);
		if (skipAnimation || ((EntityQuery)(ref m_ConfigurationQuery)).IsEmptyIgnoreFilter)
		{
			entityManager = ((ComponentSystemBase)this).EntityManager;
			((EntityManager)(ref entityManager)).AddComponent<Deleted>(marker);
		}
		else if (EntitiesExtensions.TryGetComponent<Game.Notifications.Animation>(((ComponentSystemBase)this).EntityManager, marker, ref animation))
		{
			if (animation.m_Type != Game.Notifications.AnimationType.MarkerDisappear)
			{
				animation.m_Type = Game.Notifications.AnimationType.MarkerDisappear;
				animation.m_Timer = animation.m_Duration - animation.m_Timer;
				entityManager = ((ComponentSystemBase)this).EntityManager;
				((EntityManager)(ref entityManager)).SetComponentData<Game.Notifications.Animation>(marker, animation);
			}
		}
		else
		{
			Entity singletonEntity = ((EntityQuery)(ref m_ConfigurationQuery)).GetSingletonEntity();
			entityManager = ((ComponentSystemBase)this).EntityManager;
			float duration = ((EntityManager)(ref entityManager)).GetBuffer<IconAnimationElement>(singletonEntity, true)[1].m_Duration;
			entityManager = ((ComponentSystemBase)this).EntityManager;
			((EntityManager)(ref entityManager)).AddComponentData<Game.Notifications.Animation>(marker, new Game.Notifications.Animation(Game.Notifications.AnimationType.MarkerDisappear, Time.deltaTime, duration));
		}
		marker = Entity.Null;
	}

	private Entity CreateMarker(Entity target, float3 position, MarkerType markerType, bool skipAnimation)
	{
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_007e: Unknown result type (might be due to invalid IL or missing references)
		//IL_007f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0085: Unknown result type (might be due to invalid IL or missing references)
		//IL_008a: Unknown result type (might be due to invalid IL or missing references)
		//IL_008f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0094: Unknown result type (might be due to invalid IL or missing references)
		//IL_0099: Unknown result type (might be due to invalid IL or missing references)
		//IL_009c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00de: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e7: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_0134: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ff: Unknown result type (might be due to invalid IL or missing references)
		//IL_0104: Unknown result type (might be due to invalid IL or missing references)
		//IL_0116: Unknown result type (might be due to invalid IL or missing references)
		//IL_011b: Unknown result type (might be due to invalid IL or missing references)
		//IL_011f: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		if (((EntityQuery)(ref m_ConfigurationQuery)).IsEmptyIgnoreFilter)
		{
			return Entity.Null;
		}
		Entity singletonEntity = ((EntityQuery)(ref m_ConfigurationQuery)).GetSingletonEntity();
		EntityManager entityManager = ((ComponentSystemBase)this).EntityManager;
		IconConfigurationData componentData = ((EntityManager)(ref entityManager)).GetComponentData<IconConfigurationData>(singletonEntity);
		Entity val;
		switch (markerType)
		{
		case MarkerType.Selected:
			val = componentData.m_SelectedMarker;
			break;
		case MarkerType.Followed:
			val = componentData.m_FollowedMarker;
			break;
		default:
			return Entity.Null;
		}
		entityManager = ((ComponentSystemBase)this).EntityManager;
		NotificationIconData componentData2 = ((EntityManager)(ref entityManager)).GetComponentData<NotificationIconData>(val);
		Icon icon = new Icon
		{
			m_Priority = IconPriority.Info,
			m_Flags = (IconFlags.Unique | IconFlags.OnTop),
			m_Location = position
		};
		entityManager = ((ComponentSystemBase)this).EntityManager;
		Entity val2 = ((EntityManager)(ref entityManager)).CreateEntity(componentData2.m_Archetype);
		entityManager = ((ComponentSystemBase)this).EntityManager;
		((EntityManager)(ref entityManager)).SetComponentData<PrefabRef>(val2, new PrefabRef(val));
		entityManager = ((ComponentSystemBase)this).EntityManager;
		((EntityManager)(ref entityManager)).SetComponentData<Icon>(val2, icon);
		entityManager = ((ComponentSystemBase)this).EntityManager;
		((EntityManager)(ref entityManager)).AddComponentData<Target>(val2, new Target(target));
		entityManager = ((ComponentSystemBase)this).EntityManager;
		((EntityManager)(ref entityManager)).AddComponent<DisallowCluster>(val2);
		if (!skipAnimation)
		{
			entityManager = ((ComponentSystemBase)this).EntityManager;
			float duration = ((EntityManager)(ref entityManager)).GetBuffer<IconAnimationElement>(singletonEntity, true)[0].m_Duration;
			entityManager = ((ComponentSystemBase)this).EntityManager;
			((EntityManager)(ref entityManager)).AddComponentData<Game.Notifications.Animation>(val2, new Game.Notifications.Animation(Game.Notifications.AnimationType.MarkerAppear, Time.deltaTime, duration));
		}
		return val2;
	}

	public void Serialize<TWriter>(TWriter writer) where TWriter : IWriter
	{
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		Entity val = m_SelectedMarker;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(val);
		Entity val2 = m_FollowedMarker;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(val2);
	}

	public void Deserialize<TReader>(TReader reader) where TReader : IReader
	{
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		ref Entity reference = ref m_SelectedMarker;
		((IReader)reader/*cast due to .constrained prefix*/).Read(ref reference);
		ref Entity reference2 = ref m_FollowedMarker;
		((IReader)reader/*cast due to .constrained prefix*/).Read(ref reference2);
		m_SelectedLocation = Entity.Null;
		m_FollowedLocation = Entity.Null;
	}

	public void SetDefaults(Context context)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		m_SelectedMarker = Entity.Null;
		m_FollowedMarker = Entity.Null;
		m_SelectedLocation = Entity.Null;
		m_FollowedLocation = Entity.Null;
	}

	private void AdjustLocations(float3 selectedLocation, float3 followedLocation, float3 cameraPos, float3 cameraUp)
	{
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0073: Unknown result type (might be due to invalid IL or missing references)
		//IL_0074: Unknown result type (might be due to invalid IL or missing references)
		//IL_007d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0082: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f9: Unknown result type (might be due to invalid IL or missing references)
		//IL_0100: Unknown result type (might be due to invalid IL or missing references)
		//IL_0102: Unknown result type (might be due to invalid IL or missing references)
		//IL_011a: Unknown result type (might be due to invalid IL or missing references)
		//IL_011f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0126: Unknown result type (might be due to invalid IL or missing references)
		//IL_0127: Unknown result type (might be due to invalid IL or missing references)
		//IL_0130: Unknown result type (might be due to invalid IL or missing references)
		//IL_0136: Unknown result type (might be due to invalid IL or missing references)
		//IL_013b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0140: Unknown result type (might be due to invalid IL or missing references)
		//IL_0141: Unknown result type (might be due to invalid IL or missing references)
		//IL_0142: Unknown result type (might be due to invalid IL or missing references)
		//IL_0147: Unknown result type (might be due to invalid IL or missing references)
		//IL_014a: Unknown result type (might be due to invalid IL or missing references)
		//IL_014b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0152: Unknown result type (might be due to invalid IL or missing references)
		NativeQueue<Overlap> overlapQueue = default(NativeQueue<Overlap>);
		overlapQueue._002Ector(AllocatorHandle.op_Implicit((Allocator)3));
		FindOverlapIconsJob findOverlapIconsJob = new FindOverlapIconsJob
		{
			m_EntityType = InternalCompilerInterface.GetEntityTypeHandle(ref __TypeHandle.__Unity_Entities_Entity_TypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_IconType = InternalCompilerInterface.GetComponentTypeHandle<Icon>(ref __TypeHandle.__Game_Notifications_Icon_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_Entity1 = m_SelectedMarker,
			m_Entity2 = m_FollowedMarker,
			m_Location1 = selectedLocation,
			m_Location2 = followedLocation,
			m_OverlapQueue = overlapQueue.AsParallelWriter()
		};
		UpdateMarkerLocationJob obj = new UpdateMarkerLocationJob
		{
			m_PrefabRefData = InternalCompilerInterface.GetComponentLookup<PrefabRef>(ref __TypeHandle.__Game_Prefabs_PrefabRef_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_IconDisplayData = InternalCompilerInterface.GetComponentLookup<NotificationIconDisplayData>(ref __TypeHandle.__Game_Prefabs_NotificationIconDisplayData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_Entity1 = m_SelectedMarker,
			m_Entity2 = m_FollowedMarker,
			m_Location1 = selectedLocation,
			m_Location2 = followedLocation,
			m_CameraPos = cameraPos,
			m_CameraUp = cameraUp,
			m_IconData = InternalCompilerInterface.GetComponentLookup<Icon>(ref __TypeHandle.__Game_Notifications_Icon_RW_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_OverlapQueue = overlapQueue
		};
		JobHandle val = JobChunkExtensions.ScheduleParallel<FindOverlapIconsJob>(findOverlapIconsJob, m_IconQuery, ((SystemBase)this).Dependency);
		JobHandle val2 = IJobExtensions.Schedule<UpdateMarkerLocationJob>(obj, val);
		overlapQueue.Dispose(val2);
		((SystemBase)this).Dependency = val2;
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
	public MarkerIconSystem()
	{
	}
}

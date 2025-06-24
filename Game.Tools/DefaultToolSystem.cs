using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Colossal.Collections;
using Colossal.Entities;
using Colossal.Mathematics;
using Game.Areas;
using Game.Audio;
using Game.Buildings;
using Game.Citizens;
using Game.Common;
using Game.Creatures;
using Game.Input;
using Game.Net;
using Game.Notifications;
using Game.Objects;
using Game.Prefabs;
using Game.Rendering;
using Game.Routes;
using Game.Vehicles;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Entities.Internal;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine.Scripting;

namespace Game.Tools;

[CompilerGenerated]
public class DefaultToolSystem : ToolBaseSystem
{
	private enum State
	{
		Default,
		MouseDownPrepare,
		MouseDown,
		Dragging
	}

	[BurstCompile]
	private struct CreateDefinitionsJob : IJob
	{
		[ReadOnly]
		public Entity m_Entity;

		[ReadOnly]
		public float3 m_Position;

		[ReadOnly]
		public bool m_SetPosition;

		[ReadOnly]
		public BufferLookup<ConnectedEdge> m_Edges;

		[ReadOnly]
		public BufferLookup<InstalledUpgrade> m_InstalledUpgrades;

		[ReadOnly]
		public ComponentLookup<LocalTransformCache> m_LocalTransformCacheData;

		[ReadOnly]
		public ComponentLookup<Edge> m_EdgeData;

		[ReadOnly]
		public ComponentLookup<Game.Net.Node> m_NodeData;

		[ReadOnly]
		public ComponentLookup<Curve> m_CurveData;

		[ReadOnly]
		public ComponentLookup<Owner> m_OwnerData;

		[ReadOnly]
		public ComponentLookup<EditorContainer> m_EditorContainerData;

		[ReadOnly]
		public ComponentLookup<Transform> m_TransformData;

		[ReadOnly]
		public ComponentLookup<Game.Objects.Elevation> m_ElevationData;

		[ReadOnly]
		public ComponentLookup<Attached> m_AttachedData;

		[ReadOnly]
		public ComponentLookup<Attachment> m_AttachmentData;

		[ReadOnly]
		public ComponentLookup<Game.Buildings.ServiceUpgrade> m_ServiceUpgradeData;

		[ReadOnly]
		public ComponentLookup<Game.Areas.Lot> m_LotData;

		[ReadOnly]
		public ComponentLookup<Position> m_RoutePositionData;

		[ReadOnly]
		public ComponentLookup<Connected> m_RouteConnectedData;

		[ReadOnly]
		public ComponentLookup<Icon> m_IconData;

		[ReadOnly]
		public ComponentLookup<PrefabRef> m_PrefabRefData;

		[ReadOnly]
		public BufferLookup<Game.Areas.Node> m_AreaNodes;

		[ReadOnly]
		public BufferLookup<Game.Areas.SubArea> m_SubAreas;

		[ReadOnly]
		public BufferLookup<RouteWaypoint> m_RouteWaypoints;

		[ReadOnly]
		public BufferLookup<AggregateElement> m_AggregateElements;

		public EntityCommandBuffer m_CommandBuffer;

		public void Execute()
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_001a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0106: Unknown result type (might be due to invalid IL or missing references)
			//IL_0030: Unknown result type (might be due to invalid IL or missing references)
			//IL_0162: Unknown result type (might be due to invalid IL or missing references)
			//IL_0167: Unknown result type (might be due to invalid IL or missing references)
			//IL_017a: Unknown result type (might be due to invalid IL or missing references)
			//IL_011c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0049: Unknown result type (might be due to invalid IL or missing references)
			//IL_018d: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a5: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ab: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b0: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b9: Unknown result type (might be due to invalid IL or missing references)
			//IL_01be: Unknown result type (might be due to invalid IL or missing references)
			//IL_01c7: Unknown result type (might be due to invalid IL or missing references)
			//IL_01cc: Unknown result type (might be due to invalid IL or missing references)
			//IL_012c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0132: Unknown result type (might be due to invalid IL or missing references)
			//IL_005c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0061: Unknown result type (might be due to invalid IL or missing references)
			//IL_0065: Unknown result type (might be due to invalid IL or missing references)
			//IL_0066: Unknown result type (might be due to invalid IL or missing references)
			//IL_0082: Unknown result type (might be due to invalid IL or missing references)
			//IL_0140: Unknown result type (might be due to invalid IL or missing references)
			//IL_0145: Unknown result type (might be due to invalid IL or missing references)
			//IL_0149: Unknown result type (might be due to invalid IL or missing references)
			//IL_014a: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ce: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d4: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d9: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e7: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f5: Unknown result type (might be due to invalid IL or missing references)
			//IL_0094: Unknown result type (might be due to invalid IL or missing references)
			//IL_01e2: Unknown result type (might be due to invalid IL or missing references)
			//IL_01e7: Unknown result type (might be due to invalid IL or missing references)
			//IL_01e9: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ec: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a8: Unknown result type (might be due to invalid IL or missing references)
			//IL_01f9: Unknown result type (might be due to invalid IL or missing references)
			//IL_01fb: Unknown result type (might be due to invalid IL or missing references)
			Entity val = m_Entity;
			OwnerDefinition ownerDefinition = default(OwnerDefinition);
			bool isParent = false;
			bool attachParentCreated = false;
			Owner owner = default(Owner);
			Attached attached = default(Attached);
			Attachment attachment2 = default(Attachment);
			Transform transform = default(Transform);
			if (m_ServiceUpgradeData.HasComponent(m_Entity) && m_OwnerData.TryGetComponent(m_Entity, ref owner) && m_TransformData.TryGetComponent(owner.m_Owner, ref transform))
			{
				val = owner.m_Owner;
				isParent = true;
				AddEntity(val, Entity.Null, default(OwnerDefinition), isParent: true, attachParentCreated: false);
				Attachment attachment = default(Attachment);
				if (m_AttachmentData.TryGetComponent(val, ref attachment) && m_TransformData.HasComponent(attachment.m_Attached))
				{
					AddEntity(attachment.m_Attached, Entity.Null, default(OwnerDefinition), isParent: true, attachParentCreated: true);
				}
				ownerDefinition = new OwnerDefinition
				{
					m_Prefab = m_PrefabRefData[val].m_Prefab,
					m_Position = transform.m_Position,
					m_Rotation = transform.m_Rotation
				};
			}
			else if (m_AttachedData.TryGetComponent(m_Entity, ref attached) && m_AttachmentData.TryGetComponent(attached.m_Parent, ref attachment2) && attachment2.m_Attached == m_Entity)
			{
				val = attached.m_Parent;
				attachParentCreated = true;
				AddEntity(val, Entity.Null, default(OwnerDefinition), isParent: false, attachParentCreated: false);
			}
			AddEntity(m_Entity, Entity.Null, ownerDefinition, isParent: false, attachParentCreated);
			DynamicBuffer<InstalledUpgrade> val2 = default(DynamicBuffer<InstalledUpgrade>);
			if (!m_InstalledUpgrades.TryGetBuffer(val, ref val2))
			{
				return;
			}
			transform = m_TransformData[val];
			ownerDefinition = new OwnerDefinition
			{
				m_Prefab = m_PrefabRefData[val].m_Prefab,
				m_Position = transform.m_Position,
				m_Rotation = transform.m_Rotation
			};
			for (int i = 0; i < val2.Length; i++)
			{
				Entity val3 = val2[i];
				if (val3 != m_Entity)
				{
					AddEntity(val3, Entity.Null, ownerDefinition, isParent, attachParentCreated: false);
				}
			}
		}

		private void AddEntity(Entity entity, Entity owner, OwnerDefinition ownerDefinition, bool isParent, bool attachParentCreated)
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_000b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0016: Unknown result type (might be due to invalid IL or missing references)
			//IL_0017: Unknown result type (might be due to invalid IL or missing references)
			//IL_0044: Unknown result type (might be due to invalid IL or missing references)
			//IL_0054: Unknown result type (might be due to invalid IL or missing references)
			//IL_0059: Unknown result type (might be due to invalid IL or missing references)
			//IL_0078: Unknown result type (might be due to invalid IL or missing references)
			//IL_006b: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ba: Unknown result type (might be due to invalid IL or missing references)
			//IL_0089: Unknown result type (might be due to invalid IL or missing references)
			//IL_02de: Unknown result type (might be due to invalid IL or missing references)
			//IL_01cb: Unknown result type (might be due to invalid IL or missing references)
			//IL_00af: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c6: Unknown result type (might be due to invalid IL or missing references)
			//IL_00cc: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00da: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f9: Unknown result type (might be due to invalid IL or missing references)
			//IL_00fe: Unknown result type (might be due to invalid IL or missing references)
			//IL_010c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0111: Unknown result type (might be due to invalid IL or missing references)
			//IL_0116: Unknown result type (might be due to invalid IL or missing references)
			//IL_0124: Unknown result type (might be due to invalid IL or missing references)
			//IL_0129: Unknown result type (might be due to invalid IL or missing references)
			//IL_012e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0133: Unknown result type (might be due to invalid IL or missing references)
			//IL_0151: Unknown result type (might be due to invalid IL or missing references)
			//IL_0156: Unknown result type (might be due to invalid IL or missing references)
			//IL_0164: Unknown result type (might be due to invalid IL or missing references)
			//IL_0169: Unknown result type (might be due to invalid IL or missing references)
			//IL_016e: Unknown result type (might be due to invalid IL or missing references)
			//IL_017c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0181: Unknown result type (might be due to invalid IL or missing references)
			//IL_0186: Unknown result type (might be due to invalid IL or missing references)
			//IL_018b: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a7: Unknown result type (might be due to invalid IL or missing references)
			//IL_0099: Unknown result type (might be due to invalid IL or missing references)
			//IL_009f: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
			//IL_0642: Unknown result type (might be due to invalid IL or missing references)
			//IL_02ef: Unknown result type (might be due to invalid IL or missing references)
			//IL_01f1: Unknown result type (might be due to invalid IL or missing references)
			//IL_0205: Unknown result type (might be due to invalid IL or missing references)
			//IL_020c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0213: Unknown result type (might be due to invalid IL or missing references)
			//IL_021a: Unknown result type (might be due to invalid IL or missing references)
			//IL_021f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0224: Unknown result type (might be due to invalid IL or missing references)
			//IL_0244: Unknown result type (might be due to invalid IL or missing references)
			//IL_0245: Unknown result type (might be due to invalid IL or missing references)
			//IL_0253: Unknown result type (might be due to invalid IL or missing references)
			//IL_0258: Unknown result type (might be due to invalid IL or missing references)
			//IL_0266: Unknown result type (might be due to invalid IL or missing references)
			//IL_026b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0288: Unknown result type (might be due to invalid IL or missing references)
			//IL_0289: Unknown result type (might be due to invalid IL or missing references)
			//IL_0297: Unknown result type (might be due to invalid IL or missing references)
			//IL_029c: Unknown result type (might be due to invalid IL or missing references)
			//IL_02aa: Unknown result type (might be due to invalid IL or missing references)
			//IL_02af: Unknown result type (might be due to invalid IL or missing references)
			//IL_02cb: Unknown result type (might be due to invalid IL or missing references)
			//IL_01db: Unknown result type (might be due to invalid IL or missing references)
			//IL_01e1: Unknown result type (might be due to invalid IL or missing references)
			//IL_01e6: Unknown result type (might be due to invalid IL or missing references)
			//IL_07d9: Unknown result type (might be due to invalid IL or missing references)
			//IL_068d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0650: Unknown result type (might be due to invalid IL or missing references)
			//IL_0651: Unknown result type (might be due to invalid IL or missing references)
			//IL_0656: Unknown result type (might be due to invalid IL or missing references)
			//IL_065e: Unknown result type (might be due to invalid IL or missing references)
			//IL_065f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0664: Unknown result type (might be due to invalid IL or missing references)
			//IL_0678: Unknown result type (might be due to invalid IL or missing references)
			//IL_0328: Unknown result type (might be due to invalid IL or missing references)
			//IL_032d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0336: Unknown result type (might be due to invalid IL or missing references)
			//IL_033b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0346: Unknown result type (might be due to invalid IL or missing references)
			//IL_0302: Unknown result type (might be due to invalid IL or missing references)
			//IL_0307: Unknown result type (might be due to invalid IL or missing references)
			//IL_0762: Unknown result type (might be due to invalid IL or missing references)
			//IL_069e: Unknown result type (might be due to invalid IL or missing references)
			//IL_069f: Unknown result type (might be due to invalid IL or missing references)
			//IL_06a4: Unknown result type (might be due to invalid IL or missing references)
			//IL_06ac: Unknown result type (might be due to invalid IL or missing references)
			//IL_06ad: Unknown result type (might be due to invalid IL or missing references)
			//IL_06b2: Unknown result type (might be due to invalid IL or missing references)
			//IL_0793: Unknown result type (might be due to invalid IL or missing references)
			//IL_0770: Unknown result type (might be due to invalid IL or missing references)
			//IL_077e: Unknown result type (might be due to invalid IL or missing references)
			//IL_037b: Unknown result type (might be due to invalid IL or missing references)
			//IL_037c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0384: Unknown result type (might be due to invalid IL or missing references)
			//IL_07a1: Unknown result type (might be due to invalid IL or missing references)
			//IL_07a2: Unknown result type (might be due to invalid IL or missing references)
			//IL_07a7: Unknown result type (might be due to invalid IL or missing references)
			//IL_07af: Unknown result type (might be due to invalid IL or missing references)
			//IL_07b0: Unknown result type (might be due to invalid IL or missing references)
			//IL_07b5: Unknown result type (might be due to invalid IL or missing references)
			//IL_07c9: Unknown result type (might be due to invalid IL or missing references)
			//IL_06e7: Unknown result type (might be due to invalid IL or missing references)
			//IL_06f1: Unknown result type (might be due to invalid IL or missing references)
			//IL_06f6: Unknown result type (might be due to invalid IL or missing references)
			//IL_06ff: Unknown result type (might be due to invalid IL or missing references)
			//IL_0704: Unknown result type (might be due to invalid IL or missing references)
			//IL_0711: Unknown result type (might be due to invalid IL or missing references)
			//IL_041a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0394: Unknown result type (might be due to invalid IL or missing references)
			//IL_039a: Unknown result type (might be due to invalid IL or missing references)
			//IL_039f: Unknown result type (might be due to invalid IL or missing references)
			//IL_03b7: Unknown result type (might be due to invalid IL or missing references)
			//IL_0727: Unknown result type (might be due to invalid IL or missing references)
			//IL_0731: Unknown result type (might be due to invalid IL or missing references)
			//IL_0736: Unknown result type (might be due to invalid IL or missing references)
			//IL_048a: Unknown result type (might be due to invalid IL or missing references)
			//IL_048f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0428: Unknown result type (might be due to invalid IL or missing references)
			//IL_0434: Unknown result type (might be due to invalid IL or missing references)
			//IL_0439: Unknown result type (might be due to invalid IL or missing references)
			//IL_0442: Unknown result type (might be due to invalid IL or missing references)
			//IL_0447: Unknown result type (might be due to invalid IL or missing references)
			//IL_03c7: Unknown result type (might be due to invalid IL or missing references)
			//IL_03cc: Unknown result type (might be due to invalid IL or missing references)
			//IL_04de: Unknown result type (might be due to invalid IL or missing references)
			//IL_049c: Unknown result type (might be due to invalid IL or missing references)
			//IL_04a2: Unknown result type (might be due to invalid IL or missing references)
			//IL_04be: Unknown result type (might be due to invalid IL or missing references)
			//IL_04c3: Unknown result type (might be due to invalid IL or missing references)
			//IL_04cc: Unknown result type (might be due to invalid IL or missing references)
			//IL_04d1: Unknown result type (might be due to invalid IL or missing references)
			//IL_0540: Unknown result type (might be due to invalid IL or missing references)
			//IL_03e7: Unknown result type (might be due to invalid IL or missing references)
			//IL_03d5: Unknown result type (might be due to invalid IL or missing references)
			//IL_03da: Unknown result type (might be due to invalid IL or missing references)
			//IL_0522: Unknown result type (might be due to invalid IL or missing references)
			//IL_0527: Unknown result type (might be due to invalid IL or missing references)
			//IL_0530: Unknown result type (might be due to invalid IL or missing references)
			//IL_0535: Unknown result type (might be due to invalid IL or missing references)
			//IL_04ec: Unknown result type (might be due to invalid IL or missing references)
			//IL_0504: Unknown result type (might be due to invalid IL or missing references)
			//IL_0509: Unknown result type (might be due to invalid IL or missing references)
			//IL_0512: Unknown result type (might be due to invalid IL or missing references)
			//IL_0517: Unknown result type (might be due to invalid IL or missing references)
			//IL_0594: Unknown result type (might be due to invalid IL or missing references)
			//IL_05a2: Unknown result type (might be due to invalid IL or missing references)
			//IL_054e: Unknown result type (might be due to invalid IL or missing references)
			//IL_055a: Unknown result type (might be due to invalid IL or missing references)
			//IL_055f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0568: Unknown result type (might be due to invalid IL or missing references)
			//IL_056d: Unknown result type (might be due to invalid IL or missing references)
			//IL_03f9: Unknown result type (might be due to invalid IL or missing references)
			//IL_03fe: Unknown result type (might be due to invalid IL or missing references)
			//IL_05c0: Unknown result type (might be due to invalid IL or missing references)
			//IL_05c6: Unknown result type (might be due to invalid IL or missing references)
			//IL_05cb: Unknown result type (might be due to invalid IL or missing references)
			//IL_05d4: Unknown result type (might be due to invalid IL or missing references)
			//IL_05d9: Unknown result type (might be due to invalid IL or missing references)
			//IL_05e2: Unknown result type (might be due to invalid IL or missing references)
			//IL_05e7: Unknown result type (might be due to invalid IL or missing references)
			//IL_05fe: Unknown result type (might be due to invalid IL or missing references)
			//IL_0603: Unknown result type (might be due to invalid IL or missing references)
			//IL_060b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0615: Unknown result type (might be due to invalid IL or missing references)
			//IL_0617: Unknown result type (might be due to invalid IL or missing references)
			Entity val = ((EntityCommandBuffer)(ref m_CommandBuffer)).CreateEntity();
			CreationDefinition creationDefinition = new CreationDefinition
			{
				m_Original = entity
			};
			if (isParent)
			{
				creationDefinition.m_Flags |= CreationFlags.Parent | CreationFlags.Duplicate;
			}
			else
			{
				creationDefinition.m_Flags |= CreationFlags.Select;
			}
			((EntityCommandBuffer)(ref m_CommandBuffer)).AddComponent<Updated>(val, default(Updated));
			if (ownerDefinition.m_Prefab != Entity.Null)
			{
				((EntityCommandBuffer)(ref m_CommandBuffer)).AddComponent<OwnerDefinition>(val, ownerDefinition);
			}
			if (m_EdgeData.HasComponent(entity))
			{
				if (m_EditorContainerData.HasComponent(entity))
				{
					creationDefinition.m_SubPrefab = m_EditorContainerData[entity].m_Prefab;
				}
				Edge edge = m_EdgeData[entity];
				NetCourse netCourse = default(NetCourse);
				netCourse.m_Curve = m_CurveData[entity].m_Bezier;
				netCourse.m_Length = MathUtils.Length(netCourse.m_Curve);
				netCourse.m_FixedIndex = -1;
				netCourse.m_StartPosition.m_Entity = edge.m_Start;
				netCourse.m_StartPosition.m_Position = netCourse.m_Curve.a;
				netCourse.m_StartPosition.m_Rotation = NetUtils.GetNodeRotation(MathUtils.StartTangent(netCourse.m_Curve));
				netCourse.m_StartPosition.m_CourseDelta = 0f;
				netCourse.m_EndPosition.m_Entity = edge.m_End;
				netCourse.m_EndPosition.m_Position = netCourse.m_Curve.d;
				netCourse.m_EndPosition.m_Rotation = NetUtils.GetNodeRotation(MathUtils.EndTangent(netCourse.m_Curve));
				netCourse.m_EndPosition.m_CourseDelta = 1f;
				((EntityCommandBuffer)(ref m_CommandBuffer)).AddComponent<NetCourse>(val, netCourse);
			}
			else if (m_NodeData.HasComponent(entity))
			{
				if (m_EditorContainerData.HasComponent(entity))
				{
					creationDefinition.m_SubPrefab = m_EditorContainerData[entity].m_Prefab;
				}
				Game.Net.Node node = m_NodeData[entity];
				NetCourse netCourse2 = new NetCourse
				{
					m_Curve = new Bezier4x3(node.m_Position, node.m_Position, node.m_Position, node.m_Position),
					m_Length = 0f,
					m_FixedIndex = -1,
					m_StartPosition = 
					{
						m_Entity = entity,
						m_Position = node.m_Position,
						m_Rotation = node.m_Rotation,
						m_CourseDelta = 0f
					},
					m_EndPosition = 
					{
						m_Entity = entity,
						m_Position = node.m_Position,
						m_Rotation = node.m_Rotation,
						m_CourseDelta = 1f
					}
				};
				((EntityCommandBuffer)(ref m_CommandBuffer)).AddComponent<NetCourse>(val, netCourse2);
			}
			else if (m_TransformData.HasComponent(entity))
			{
				Transform transform = m_TransformData[entity];
				if (m_SetPosition)
				{
					transform.m_Position = m_Position;
					creationDefinition.m_Flags |= CreationFlags.Dragging;
				}
				ObjectDefinition objectDefinition = new ObjectDefinition
				{
					m_Position = transform.m_Position,
					m_Rotation = transform.m_Rotation
				};
				Game.Objects.Elevation elevation = default(Game.Objects.Elevation);
				if (m_ElevationData.TryGetComponent(entity, ref elevation))
				{
					objectDefinition.m_Elevation = elevation.m_Elevation;
					objectDefinition.m_ParentMesh = ObjectUtils.GetSubParentMesh(elevation.m_Flags);
				}
				else
				{
					objectDefinition.m_ParentMesh = -1;
				}
				Entity val2 = entity;
				if (m_AttachedData.HasComponent(entity))
				{
					creationDefinition.m_Attached = m_AttachedData[entity].m_Parent;
					creationDefinition.m_Flags |= CreationFlags.Attach;
					Attachment attachment = default(Attachment);
					if (m_AttachmentData.TryGetComponent(creationDefinition.m_Attached, ref attachment) && attachment.m_Attached == entity)
					{
						val2 = creationDefinition.m_Attached;
					}
					PrefabRef prefabRef = default(PrefabRef);
					if (attachParentCreated && m_PrefabRefData.TryGetComponent(creationDefinition.m_Attached, ref prefabRef))
					{
						creationDefinition.m_Attached = prefabRef.m_Prefab;
					}
				}
				objectDefinition.m_Probability = 100;
				objectDefinition.m_PrefabSubIndex = -1;
				if (m_LocalTransformCacheData.HasComponent(entity))
				{
					LocalTransformCache localTransformCache = m_LocalTransformCacheData[entity];
					objectDefinition.m_LocalPosition = localTransformCache.m_Position;
					objectDefinition.m_LocalRotation = localTransformCache.m_Rotation;
					objectDefinition.m_ParentMesh = localTransformCache.m_ParentMesh;
					objectDefinition.m_GroupIndex = localTransformCache.m_GroupIndex;
					objectDefinition.m_Probability = localTransformCache.m_Probability;
					objectDefinition.m_PrefabSubIndex = localTransformCache.m_PrefabSubIndex;
				}
				else if (ownerDefinition.m_Prefab != Entity.Null)
				{
					Transform transform2 = ObjectUtils.WorldToLocal(ObjectUtils.InverseTransform(new Transform(ownerDefinition.m_Position, ownerDefinition.m_Rotation)), transform);
					objectDefinition.m_LocalPosition = transform2.m_Position;
					objectDefinition.m_LocalRotation = transform2.m_Rotation;
				}
				else if (m_TransformData.HasComponent(owner))
				{
					Transform transform3 = ObjectUtils.WorldToLocal(ObjectUtils.InverseTransform(m_TransformData[owner]), transform);
					objectDefinition.m_LocalPosition = transform3.m_Position;
					objectDefinition.m_LocalRotation = transform3.m_Rotation;
				}
				else
				{
					objectDefinition.m_LocalPosition = transform.m_Position;
					objectDefinition.m_LocalRotation = transform.m_Rotation;
				}
				if (m_EditorContainerData.HasComponent(entity))
				{
					EditorContainer editorContainer = m_EditorContainerData[entity];
					creationDefinition.m_SubPrefab = editorContainer.m_Prefab;
					objectDefinition.m_Scale = editorContainer.m_Scale;
					objectDefinition.m_Intensity = editorContainer.m_Intensity;
					objectDefinition.m_GroupIndex = editorContainer.m_GroupIndex;
				}
				((EntityCommandBuffer)(ref m_CommandBuffer)).AddComponent<ObjectDefinition>(val, objectDefinition);
				DynamicBuffer<Game.Areas.SubArea> val3 = default(DynamicBuffer<Game.Areas.SubArea>);
				if (m_SubAreas.TryGetBuffer(val2, ref val3))
				{
					OwnerDefinition ownerDefinition2 = new OwnerDefinition
					{
						m_Prefab = m_PrefabRefData[entity].m_Prefab,
						m_Position = transform.m_Position,
						m_Rotation = transform.m_Rotation
					};
					for (int i = 0; i < val3.Length; i++)
					{
						Entity area = val3[i].m_Area;
						if (m_LotData.HasComponent(area))
						{
							AddEntity(area, Entity.Null, ownerDefinition2, isParent, attachParentCreated: false);
						}
					}
				}
			}
			else if (m_AreaNodes.HasBuffer(entity))
			{
				DynamicBuffer<Game.Areas.Node> val4 = m_AreaNodes[entity];
				DynamicBuffer<Game.Areas.Node> val5 = ((EntityCommandBuffer)(ref m_CommandBuffer)).AddBuffer<Game.Areas.Node>(val);
				val5.ResizeUninitialized(val4.Length);
				val5.CopyFrom(val4.AsNativeArray());
			}
			else if (m_RouteWaypoints.HasBuffer(entity))
			{
				DynamicBuffer<RouteWaypoint> val6 = m_RouteWaypoints[entity];
				DynamicBuffer<WaypointDefinition> val7 = ((EntityCommandBuffer)(ref m_CommandBuffer)).AddBuffer<WaypointDefinition>(val);
				val7.ResizeUninitialized(val6.Length);
				for (int j = 0; j < val6.Length; j++)
				{
					RouteWaypoint routeWaypoint = val6[j];
					WaypointDefinition waypointDefinition = new WaypointDefinition
					{
						m_Position = m_RoutePositionData[routeWaypoint.m_Waypoint].m_Position,
						m_Original = routeWaypoint.m_Waypoint
					};
					if (m_RouteConnectedData.HasComponent(routeWaypoint.m_Waypoint))
					{
						waypointDefinition.m_Connection = m_RouteConnectedData[routeWaypoint.m_Waypoint].m_Connected;
					}
					val7[j] = waypointDefinition;
				}
			}
			else if (m_IconData.HasComponent(entity))
			{
				Icon icon = m_IconData[entity];
				((EntityCommandBuffer)(ref m_CommandBuffer)).AddComponent<IconDefinition>(val, new IconDefinition(icon));
			}
			else if (m_AggregateElements.HasBuffer(entity))
			{
				DynamicBuffer<AggregateElement> val8 = m_AggregateElements[entity];
				DynamicBuffer<AggregateElement> val9 = ((EntityCommandBuffer)(ref m_CommandBuffer)).AddBuffer<AggregateElement>(val);
				val9.ResizeUninitialized(val8.Length);
				val9.CopyFrom(val8.AsNativeArray());
			}
			((EntityCommandBuffer)(ref m_CommandBuffer)).AddComponent<CreationDefinition>(val, creationDefinition);
		}
	}

	public struct SelectEntityJob : IJob
	{
		[ReadOnly]
		public NativeList<ArchetypeChunk> m_Chunks;

		[ReadOnly]
		public ComponentTypeHandle<Owner> m_OwnerType;

		[ReadOnly]
		public ComponentTypeHandle<Temp> m_TempType;

		[ReadOnly]
		public ComponentTypeHandle<Attachment> m_AttachmentType;

		[ReadOnly]
		public ComponentTypeHandle<Controller> m_ControllerType;

		[ReadOnly]
		public EntityStorageInfoLookup m_EntityLookup;

		[ReadOnly]
		public ComponentLookup<Temp> m_TempData;

		[ReadOnly]
		public ComponentLookup<Owner> m_OwnerData;

		[ReadOnly]
		public ComponentLookup<Target> m_TargetData;

		[ReadOnly]
		public ComponentLookup<Debug> m_DebugData;

		[ReadOnly]
		public ComponentLookup<Icon> m_IconData;

		[ReadOnly]
		public ComponentLookup<Vehicle> m_VehicleData;

		[ReadOnly]
		public ComponentLookup<Building> m_BuildingData;

		[ReadOnly]
		public bool m_DebugSelect;

		public NativeReference<Entity> m_Selected;

		public EntityCommandBuffer m_CommandBuffer;

		public void Execute()
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			//IL_0005: Unknown result type (might be due to invalid IL or missing references)
			//IL_0014: Unknown result type (might be due to invalid IL or missing references)
			//IL_0019: Unknown result type (might be due to invalid IL or missing references)
			//IL_0022: Unknown result type (might be due to invalid IL or missing references)
			//IL_0027: Unknown result type (might be due to invalid IL or missing references)
			//IL_0030: Unknown result type (might be due to invalid IL or missing references)
			//IL_0035: Unknown result type (might be due to invalid IL or missing references)
			//IL_003f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0044: Unknown result type (might be due to invalid IL or missing references)
			//IL_004e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0053: Unknown result type (might be due to invalid IL or missing references)
			//IL_013f: Unknown result type (might be due to invalid IL or missing references)
			//IL_01f2: Unknown result type (might be due to invalid IL or missing references)
			//IL_0150: Unknown result type (might be due to invalid IL or missing references)
			//IL_005d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0246: Unknown result type (might be due to invalid IL or missing references)
			//IL_0247: Unknown result type (might be due to invalid IL or missing references)
			//IL_0161: Unknown result type (might be due to invalid IL or missing references)
			//IL_0094: Unknown result type (might be due to invalid IL or missing references)
			//IL_0069: Unknown result type (might be due to invalid IL or missing references)
			//IL_0259: Unknown result type (might be due to invalid IL or missing references)
			//IL_0172: Unknown result type (might be due to invalid IL or missing references)
			//IL_0182: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
			//IL_007e: Unknown result type (might be due to invalid IL or missing references)
			//IL_026d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0205: Unknown result type (might be due to invalid IL or missing references)
			//IL_01e6: Unknown result type (might be due to invalid IL or missing references)
			//IL_01eb: Unknown result type (might be due to invalid IL or missing references)
			//IL_0196: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c3: Unknown result type (might be due to invalid IL or missing references)
			//IL_0288: Unknown result type (might be due to invalid IL or missing references)
			//IL_027b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0213: Unknown result type (might be due to invalid IL or missing references)
			//IL_01de: Unknown result type (might be due to invalid IL or missing references)
			//IL_01e3: Unknown result type (might be due to invalid IL or missing references)
			//IL_01aa: Unknown result type (might be due to invalid IL or missing references)
			//IL_01be: Unknown result type (might be due to invalid IL or missing references)
			//IL_0221: Unknown result type (might be due to invalid IL or missing references)
			//IL_01d4: Unknown result type (might be due to invalid IL or missing references)
			//IL_01d9: Unknown result type (might be due to invalid IL or missing references)
			//IL_01cc: Unknown result type (might be due to invalid IL or missing references)
			//IL_01d1: Unknown result type (might be due to invalid IL or missing references)
			//IL_010a: Unknown result type (might be due to invalid IL or missing references)
			//IL_010f: Unknown result type (might be due to invalid IL or missing references)
			//IL_022f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0235: Unknown result type (might be due to invalid IL or missing references)
			//IL_023a: Unknown result type (might be due to invalid IL or missing references)
			Entity val = Entity.Null;
			Owner owner = default(Owner);
			Controller controller = default(Controller);
			Temp temp = default(Temp);
			Attachment attachment = default(Attachment);
			Temp temp2 = default(Temp);
			for (int i = 0; i < m_Chunks.Length; i++)
			{
				ArchetypeChunk val2 = m_Chunks[i];
				NativeArray<Owner> nativeArray = ((ArchetypeChunk)(ref val2)).GetNativeArray<Owner>(ref m_OwnerType);
				NativeArray<Temp> nativeArray2 = ((ArchetypeChunk)(ref val2)).GetNativeArray<Temp>(ref m_TempType);
				NativeArray<Attachment> nativeArray3 = ((ArchetypeChunk)(ref val2)).GetNativeArray<Attachment>(ref m_AttachmentType);
				NativeArray<Controller> nativeArray4 = ((ArchetypeChunk)(ref val2)).GetNativeArray<Controller>(ref m_ControllerType);
				for (int j = 0; j < nativeArray2.Length; j++)
				{
					if (((!CollectionUtils.TryGet<Owner>(nativeArray, j, ref owner) && (!CollectionUtils.TryGet<Controller>(nativeArray4, j, ref controller) || !m_OwnerData.TryGetComponent(controller.m_Controller, ref owner))) || !m_TempData.TryGetComponent(owner.m_Owner, ref temp) || (temp.m_Flags & TempFlags.Select) == 0) && (!CollectionUtils.TryGet<Attachment>(nativeArray3, j, ref attachment) || !m_TempData.TryGetComponent(attachment.m_Attached, ref temp2) || (temp2.m_Flags & TempFlags.Select) == 0))
					{
						Temp temp3 = nativeArray2[j];
						if (((EntityStorageInfoLookup)(ref m_EntityLookup)).Exists(temp3.m_Original) && (temp3.m_Flags & TempFlags.Select) != 0)
						{
							val = temp3.m_Original;
						}
					}
				}
			}
			if (m_IconData.HasComponent(val) && !m_OwnerData.HasComponent(val) && m_TargetData.HasComponent(val))
			{
				Target target = m_TargetData[val];
				if (((EntityStorageInfoLookup)(ref m_EntityLookup)).Exists(target.m_Target))
				{
					if (m_TempData.HasComponent(target.m_Target))
					{
						Temp temp4 = m_TempData[target.m_Target];
						val = ((!((EntityStorageInfoLookup)(ref m_EntityLookup)).Exists(temp4.m_Original)) ? Entity.Null : temp4.m_Original);
					}
					else
					{
						val = target.m_Target;
					}
				}
				else
				{
					val = Entity.Null;
				}
			}
			if (m_IconData.HasComponent(val))
			{
				for (int k = 0; k < 4; k++)
				{
					if (m_VehicleData.HasComponent(val))
					{
						break;
					}
					if (m_BuildingData.HasComponent(val))
					{
						break;
					}
					if (!m_OwnerData.HasComponent(val))
					{
						break;
					}
					val = m_OwnerData[val].m_Owner;
				}
			}
			if (!(val != Entity.Null))
			{
				return;
			}
			m_Selected.Value = val;
			if (m_DebugSelect)
			{
				if (m_DebugData.HasComponent(val))
				{
					((EntityCommandBuffer)(ref m_CommandBuffer)).RemoveComponent<Debug>(val);
				}
				else
				{
					((EntityCommandBuffer)(ref m_CommandBuffer)).AddComponent<Debug>(val, default(Debug));
				}
			}
		}
	}

	private struct TypeHandle
	{
		[ReadOnly]
		public ComponentTypeHandle<Transform> __Game_Objects_Transform_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<Temp> __Game_Tools_Temp_RO_ComponentTypeHandle;

		[ReadOnly]
		public BufferLookup<ConnectedEdge> __Game_Net_ConnectedEdge_RO_BufferLookup;

		[ReadOnly]
		public BufferLookup<InstalledUpgrade> __Game_Buildings_InstalledUpgrade_RO_BufferLookup;

		[ReadOnly]
		public ComponentLookup<LocalTransformCache> __Game_Tools_LocalTransformCache_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Edge> __Game_Net_Edge_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Game.Net.Node> __Game_Net_Node_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Curve> __Game_Net_Curve_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Owner> __Game_Common_Owner_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<EditorContainer> __Game_Tools_EditorContainer_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Transform> __Game_Objects_Transform_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Game.Objects.Elevation> __Game_Objects_Elevation_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Attached> __Game_Objects_Attached_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Attachment> __Game_Objects_Attachment_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Game.Buildings.ServiceUpgrade> __Game_Buildings_ServiceUpgrade_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Game.Areas.Lot> __Game_Areas_Lot_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Position> __Game_Routes_Position_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Connected> __Game_Routes_Connected_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Icon> __Game_Notifications_Icon_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<PrefabRef> __Game_Prefabs_PrefabRef_RO_ComponentLookup;

		[ReadOnly]
		public BufferLookup<Game.Areas.Node> __Game_Areas_Node_RO_BufferLookup;

		[ReadOnly]
		public BufferLookup<Game.Areas.SubArea> __Game_Areas_SubArea_RO_BufferLookup;

		[ReadOnly]
		public BufferLookup<RouteWaypoint> __Game_Routes_RouteWaypoint_RO_BufferLookup;

		[ReadOnly]
		public BufferLookup<AggregateElement> __Game_Net_AggregateElement_RO_BufferLookup;

		[ReadOnly]
		public ComponentTypeHandle<Owner> __Game_Common_Owner_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<Attachment> __Game_Objects_Attachment_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<Controller> __Game_Vehicles_Controller_RO_ComponentTypeHandle;

		[ReadOnly]
		public EntityStorageInfoLookup __EntityStorageInfoLookup;

		[ReadOnly]
		public ComponentLookup<Temp> __Game_Tools_Temp_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Target> __Game_Common_Target_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Debug> __Game_Tools_Debug_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Vehicle> __Game_Vehicles_Vehicle_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Building> __Game_Buildings_Building_RO_ComponentLookup;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void __AssignHandles(ref SystemState state)
		{
			//IL_0003: Unknown result type (might be due to invalid IL or missing references)
			//IL_0008: Unknown result type (might be due to invalid IL or missing references)
			//IL_0010: Unknown result type (might be due to invalid IL or missing references)
			//IL_0015: Unknown result type (might be due to invalid IL or missing references)
			//IL_001d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0022: Unknown result type (might be due to invalid IL or missing references)
			//IL_002a: Unknown result type (might be due to invalid IL or missing references)
			//IL_002f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0037: Unknown result type (might be due to invalid IL or missing references)
			//IL_003c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0044: Unknown result type (might be due to invalid IL or missing references)
			//IL_0049: Unknown result type (might be due to invalid IL or missing references)
			//IL_0051: Unknown result type (might be due to invalid IL or missing references)
			//IL_0056: Unknown result type (might be due to invalid IL or missing references)
			//IL_005e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0063: Unknown result type (might be due to invalid IL or missing references)
			//IL_006b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0070: Unknown result type (might be due to invalid IL or missing references)
			//IL_0078: Unknown result type (might be due to invalid IL or missing references)
			//IL_007d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0085: Unknown result type (might be due to invalid IL or missing references)
			//IL_008a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0092: Unknown result type (might be due to invalid IL or missing references)
			//IL_0097: Unknown result type (might be due to invalid IL or missing references)
			//IL_009f: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b9: Unknown result type (might be due to invalid IL or missing references)
			//IL_00be: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c6: Unknown result type (might be due to invalid IL or missing references)
			//IL_00cb: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d3: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d8: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e5: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ed: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00fa: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ff: Unknown result type (might be due to invalid IL or missing references)
			//IL_0107: Unknown result type (might be due to invalid IL or missing references)
			//IL_010c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0114: Unknown result type (might be due to invalid IL or missing references)
			//IL_0119: Unknown result type (might be due to invalid IL or missing references)
			//IL_0121: Unknown result type (might be due to invalid IL or missing references)
			//IL_0126: Unknown result type (might be due to invalid IL or missing references)
			//IL_012e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0133: Unknown result type (might be due to invalid IL or missing references)
			//IL_013b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0140: Unknown result type (might be due to invalid IL or missing references)
			//IL_0148: Unknown result type (might be due to invalid IL or missing references)
			//IL_014d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0155: Unknown result type (might be due to invalid IL or missing references)
			//IL_015a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0161: Unknown result type (might be due to invalid IL or missing references)
			//IL_0166: Unknown result type (might be due to invalid IL or missing references)
			//IL_016e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0173: Unknown result type (might be due to invalid IL or missing references)
			//IL_017b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0180: Unknown result type (might be due to invalid IL or missing references)
			//IL_0188: Unknown result type (might be due to invalid IL or missing references)
			//IL_018d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0195: Unknown result type (might be due to invalid IL or missing references)
			//IL_019a: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a2: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a7: Unknown result type (might be due to invalid IL or missing references)
			__Game_Objects_Transform_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<Transform>(true);
			__Game_Tools_Temp_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<Temp>(true);
			__Game_Net_ConnectedEdge_RO_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<ConnectedEdge>(true);
			__Game_Buildings_InstalledUpgrade_RO_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<InstalledUpgrade>(true);
			__Game_Tools_LocalTransformCache_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<LocalTransformCache>(true);
			__Game_Net_Edge_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Edge>(true);
			__Game_Net_Node_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Game.Net.Node>(true);
			__Game_Net_Curve_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Curve>(true);
			__Game_Common_Owner_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Owner>(true);
			__Game_Tools_EditorContainer_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<EditorContainer>(true);
			__Game_Objects_Transform_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Transform>(true);
			__Game_Objects_Elevation_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Game.Objects.Elevation>(true);
			__Game_Objects_Attached_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Attached>(true);
			__Game_Objects_Attachment_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Attachment>(true);
			__Game_Buildings_ServiceUpgrade_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Game.Buildings.ServiceUpgrade>(true);
			__Game_Areas_Lot_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Game.Areas.Lot>(true);
			__Game_Routes_Position_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Position>(true);
			__Game_Routes_Connected_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Connected>(true);
			__Game_Notifications_Icon_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Icon>(true);
			__Game_Prefabs_PrefabRef_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<PrefabRef>(true);
			__Game_Areas_Node_RO_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<Game.Areas.Node>(true);
			__Game_Areas_SubArea_RO_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<Game.Areas.SubArea>(true);
			__Game_Routes_RouteWaypoint_RO_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<RouteWaypoint>(true);
			__Game_Net_AggregateElement_RO_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<AggregateElement>(true);
			__Game_Common_Owner_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<Owner>(true);
			__Game_Objects_Attachment_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<Attachment>(true);
			__Game_Vehicles_Controller_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<Controller>(true);
			__EntityStorageInfoLookup = ((SystemState)(ref state)).GetEntityStorageInfoLookup();
			__Game_Tools_Temp_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Temp>(true);
			__Game_Common_Target_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Target>(true);
			__Game_Tools_Debug_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Debug>(true);
			__Game_Vehicles_Vehicle_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Vehicle>(true);
			__Game_Buildings_Building_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Building>(true);
		}
	}

	public const string kToolID = "Default Tool";

	private ToolOutputBarrier m_ToolOutputBarrier;

	private AudioManager m_AudioManager;

	private RenderingSystem m_RenderingSystem;

	private EntityQuery m_DefinitionQuery;

	private EntityQuery m_DragQuery;

	private EntityQuery m_TempQuery;

	private EntityQuery m_InfomodeQuery;

	private EntityQuery m_SoundQuery;

	private EntityQuery m_UpdateQuery;

	private Entity m_LastRaycastEntity;

	private float3 m_MouseDownPosition;

	private State m_State;

	private IProxyAction m_DefaultToolApply;

	private int m_LastSelectedIndex;

	private TypeHandle __TypeHandle;

	public override string toolID => "Default Tool";

	public override bool allowUnderground => true;

	public bool underground { get; set; }

	public bool ignoreErrors { get; set; }

	public bool allowManipulation { get; set; }

	public bool debugSelect { get; set; }

	public bool debugLandValue { get; set; }

	private protected override IEnumerable<IProxyAction> toolActions
	{
		get
		{
			yield return m_DefaultToolApply;
		}
	}

	[Preserve]
	protected override void OnCreate()
	{
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0079: Unknown result type (might be due to invalid IL or missing references)
		//IL_007e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0083: Unknown result type (might be due to invalid IL or missing references)
		//IL_0088: Unknown result type (might be due to invalid IL or missing references)
		//IL_0097: Unknown result type (might be due to invalid IL or missing references)
		//IL_009d: Expected O, but got Unknown
		//IL_00a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_00be: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00db: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0100: Unknown result type (might be due to invalid IL or missing references)
		//IL_0105: Unknown result type (might be due to invalid IL or missing references)
		//IL_010a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0119: Unknown result type (might be due to invalid IL or missing references)
		//IL_011e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0123: Unknown result type (might be due to invalid IL or missing references)
		//IL_0128: Unknown result type (might be due to invalid IL or missing references)
		base.OnCreate();
		m_ToolOutputBarrier = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<ToolOutputBarrier>();
		m_AudioManager = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<AudioManager>();
		m_RenderingSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<RenderingSystem>();
		m_DefinitionQuery = GetDefinitionQuery();
		m_DragQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[2]
		{
			ComponentType.ReadOnly<Temp>(),
			ComponentType.Exclude<Owner>()
		});
		m_TempQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[1] { ComponentType.ReadOnly<Temp>() });
		EntityQueryDesc[] array = new EntityQueryDesc[1];
		EntityQueryDesc val = new EntityQueryDesc();
		val.All = (ComponentType[])(object)new ComponentType[1] { ComponentType.ReadOnly<InfomodeActive>() };
		val.Any = (ComponentType[])(object)new ComponentType[3]
		{
			ComponentType.ReadOnly<InfoviewRouteData>(),
			ComponentType.ReadOnly<InfoviewNetStatusData>(),
			ComponentType.ReadOnly<InfoviewHeatmapData>()
		};
		array[0] = val;
		m_InfomodeQuery = ((ComponentSystemBase)this).GetEntityQuery((EntityQueryDesc[])(object)array);
		m_SoundQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[1] { ComponentType.ReadOnly<ToolUXSoundSettingsData>() });
		m_UpdateQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[1] { ComponentType.ReadOnly<ColorUpdated>() });
		m_DefaultToolApply = InputManager.instance.toolActionCollection.GetActionState("Default Tool", ((object)this).GetType().Name);
	}

	[Preserve]
	protected override void OnStartRunning()
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		base.OnStartRunning();
		m_LastRaycastEntity = Entity.Null;
		SetState(State.Default);
		base.applyMode = ApplyMode.None;
		base.requireUnderground = false;
	}

	private protected override void UpdateActions()
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		using (ProxyAction.DeferStateUpdating())
		{
			base.applyActionOverride = ((m_LastRaycastEntity != Entity.Null) ? m_DefaultToolApply : m_MouseApply);
			base.applyAction.enabled = base.actionsEnabled;
			base.cancelActionOverride = m_MouseCancel;
			base.cancelAction.enabled = base.actionsEnabled;
		}
	}

	public override PrefabBase GetPrefab()
	{
		return null;
	}

	public override bool TrySetPrefab(PrefabBase prefab)
	{
		return false;
	}

	public override void SetUnderground(bool underground)
	{
		this.underground = underground;
	}

	public override void ElevationUp()
	{
		underground = false;
	}

	public override void ElevationDown()
	{
		underground = true;
	}

	public override void ElevationScroll()
	{
		underground = !underground;
	}

	public override void InitializeRaycast()
	{
		base.InitializeRaycast();
		if (underground)
		{
			m_ToolRaycastSystem.collisionMask = CollisionMask.Underground;
		}
		else
		{
			m_ToolRaycastSystem.collisionMask = CollisionMask.OnGround | CollisionMask.Overground;
		}
		if (m_State != State.Default)
		{
			m_ToolRaycastSystem.typeMask = TypeMask.Terrain | TypeMask.Net;
			m_ToolRaycastSystem.netLayerMask = Layer.Road;
			m_ToolRaycastSystem.areaTypeMask = AreaTypeMask.None;
			m_ToolRaycastSystem.iconLayerMask = IconLayerMask.None;
		}
		else
		{
			m_ToolRaycastSystem.typeMask = TypeMask.StaticObjects | TypeMask.MovingObjects | TypeMask.Labels | TypeMask.Icons;
			m_ToolRaycastSystem.raycastFlags |= RaycastFlags.OutsideConnections | RaycastFlags.Decals | RaycastFlags.BuildingLots;
			m_ToolRaycastSystem.netLayerMask = Layer.None;
			m_ToolRaycastSystem.areaTypeMask = AreaTypeMask.None;
			m_ToolRaycastSystem.iconLayerMask = IconLayerMask.Default;
			if (!underground)
			{
				m_ToolRaycastSystem.typeMask |= TypeMask.Areas;
				m_ToolRaycastSystem.areaTypeMask |= AreaTypeMask.Lots;
			}
			if (debugSelect)
			{
				m_ToolRaycastSystem.typeMask |= TypeMask.Net;
				m_ToolRaycastSystem.raycastFlags |= RaycastFlags.SubElements;
				m_ToolRaycastSystem.netLayerMask |= Layer.All;
				if (m_RenderingSystem.markersVisible)
				{
					m_ToolRaycastSystem.raycastFlags |= RaycastFlags.Markers;
				}
			}
			else if (debugLandValue)
			{
				m_ToolRaycastSystem.typeMask |= TypeMask.Terrain;
			}
			if (!((EntityQuery)(ref m_InfomodeQuery)).IsEmptyIgnoreFilter)
			{
				SetInfomodeRaycastSettings();
			}
		}
		if (m_ToolSystem.actionMode.IsEditor())
		{
			m_ToolRaycastSystem.raycastFlags |= RaycastFlags.SubElements | RaycastFlags.Placeholders | RaycastFlags.Markers | RaycastFlags.UpgradeIsMain | RaycastFlags.EditorContainers;
		}
		else
		{
			m_ToolRaycastSystem.raycastFlags |= RaycastFlags.SubBuildings;
		}
	}

	private void SetInfomodeRaycastSettings()
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		NativeArray<Entity> val = ((EntityQuery)(ref m_InfomodeQuery)).ToEntityArray(AllocatorHandle.op_Implicit((Allocator)3));
		try
		{
			InfoviewRouteData infoviewRouteData = default(InfoviewRouteData);
			InfoviewNetStatusData infoviewNetStatusData = default(InfoviewNetStatusData);
			for (int i = 0; i < val.Length; i++)
			{
				Entity val2 = val[i];
				if (EntitiesExtensions.TryGetComponent<InfoviewRouteData>(((ComponentSystemBase)this).EntityManager, val2, ref infoviewRouteData))
				{
					m_ToolRaycastSystem.typeMask |= TypeMask.RouteWaypoints | TypeMask.RouteSegments;
					m_ToolRaycastSystem.routeType = infoviewRouteData.m_Type;
				}
				if (EntitiesExtensions.TryGetComponent<InfoviewNetStatusData>(((ComponentSystemBase)this).EntityManager, val2, ref infoviewNetStatusData))
				{
					switch (infoviewNetStatusData.m_Type)
					{
					case NetStatusType.LowVoltageFlow:
						m_ToolRaycastSystem.typeMask |= TypeMask.Lanes;
						m_ToolRaycastSystem.utilityTypeMask |= UtilityTypes.LowVoltageLine;
						break;
					case NetStatusType.HighVoltageFlow:
						m_ToolRaycastSystem.typeMask |= TypeMask.Lanes;
						m_ToolRaycastSystem.utilityTypeMask |= UtilityTypes.HighVoltageLine;
						break;
					case NetStatusType.PipeWaterFlow:
						m_ToolRaycastSystem.typeMask |= TypeMask.Lanes;
						m_ToolRaycastSystem.utilityTypeMask |= UtilityTypes.WaterPipe;
						break;
					case NetStatusType.PipeSewageFlow:
						m_ToolRaycastSystem.typeMask |= TypeMask.Lanes;
						m_ToolRaycastSystem.utilityTypeMask |= UtilityTypes.SewagePipe;
						break;
					}
				}
			}
		}
		finally
		{
			val.Dispose();
		}
	}

	private void PlaySelectedSound(Entity selected, bool forcePlay = false)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_0090: Unknown result type (might be due to invalid IL or missing references)
		//IL_0095: Unknown result type (might be due to invalid IL or missing references)
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_007d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0082: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_009f: Unknown result type (might be due to invalid IL or missing references)
		Game.Creatures.Resident resident = default(Game.Creatures.Resident);
		Citizen citizen = default(Citizen);
		PrefabRef prefabRef = default(PrefabRef);
		PrefabRef prefabRef2 = default(PrefabRef);
		SelectedSoundData selectedSoundData = default(SelectedSoundData);
		Entity clipEntity = ((EntitiesExtensions.TryGetComponent<Game.Creatures.Resident>(((ComponentSystemBase)this).EntityManager, selected, ref resident) && EntitiesExtensions.TryGetComponent<Citizen>(((ComponentSystemBase)this).EntityManager, resident.m_Citizen, ref citizen) && EntitiesExtensions.TryGetComponent<PrefabRef>(((ComponentSystemBase)this).EntityManager, resident.m_Citizen, ref prefabRef)) ? CitizenUtils.GetCitizenSelectedSound(((ComponentSystemBase)this).EntityManager, resident.m_Citizen, citizen, prefabRef.m_Prefab) : ((!EntitiesExtensions.TryGetComponent<PrefabRef>(((ComponentSystemBase)this).EntityManager, selected, ref prefabRef2) || !EntitiesExtensions.TryGetComponent<SelectedSoundData>(((ComponentSystemBase)this).EntityManager, prefabRef2.m_Prefab, ref selectedSoundData)) ? ((EntityQuery)(ref m_SoundQuery)).GetSingleton<ToolUXSoundSettingsData>().m_SelectEntitySound : selectedSoundData.m_selectedSound));
		if (forcePlay)
		{
			m_AudioManager.PlayUISound(clipEntity);
		}
		else
		{
			m_AudioManager.PlayUISoundIfNotPlaying(clipEntity);
		}
	}

	[Preserve]
	protected override JobHandle OnUpdate(JobHandle inputDeps)
	{
		//IL_00bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ef: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00db: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_009a: Unknown result type (might be due to invalid IL or missing references)
		//IL_009b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_0081: Unknown result type (might be due to invalid IL or missing references)
		//IL_0084: Unknown result type (might be due to invalid IL or missing references)
		//IL_0089: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
		base.requireUnderground = underground;
		m_ForceUpdate |= !((EntityQuery)(ref m_UpdateQuery)).IsEmptyIgnoreFilter;
		if ((m_ToolRaycastSystem.raycastFlags & (RaycastFlags.DebugDisable | RaycastFlags.UIDisable)) == 0)
		{
			JobHandle result = ((m_State == State.Default && base.applyAction.WasPressedThisFrame()) ? Apply(inputDeps, base.applyAction.WasReleasedThisFrame(), base.cancelAction.WasPressedThisFrame()) : ((m_State != State.Default && base.applyAction.WasReleasedThisFrame()) ? Apply(inputDeps) : ((!base.cancelAction.IsInProgress()) ? Update(inputDeps) : Cancel(inputDeps))));
			UpdateActions();
			return result;
		}
		if (m_State == State.Default)
		{
			m_LastRaycastEntity = Entity.Null;
		}
		else if (base.applyAction.WasReleasedThisFrame())
		{
			m_LastRaycastEntity = Entity.Null;
			SetState(State.Default);
		}
		UpdateActions();
		return Clear(inputDeps);
	}

	private JobHandle Clear(JobHandle inputDeps)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		base.applyMode = ApplyMode.Clear;
		return inputDeps;
	}

	private JobHandle Cancel(JobHandle inputDeps)
	{
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		switch (m_State)
		{
		case State.Dragging:
			StopDragging();
			base.applyMode = ApplyMode.None;
			return inputDeps;
		case State.Default:
			base.applyMode = ApplyMode.None;
			m_ToolSystem.selected = Entity.Null;
			return inputDeps;
		default:
			SetState(State.Default);
			base.applyMode = ApplyMode.None;
			return inputDeps;
		}
	}

	private JobHandle Apply(JobHandle inputDeps, bool singleFrameOnly = false, bool toggleSelected = false)
	{
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		switch (m_State)
		{
		case State.Default:
			if (!singleFrameOnly)
			{
				SetState(State.MouseDownPrepare);
			}
			base.applyMode = ApplyMode.None;
			return SelectTempEntity(inputDeps, toggleSelected);
		case State.Dragging:
			StopDragging();
			base.applyMode = ApplyMode.Apply;
			return inputDeps;
		default:
			SetState(State.Default);
			base.applyMode = ApplyMode.None;
			return inputDeps;
		}
	}

	private JobHandle Update(JobHandle inputDeps)
	{
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		//IL_006e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_009e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0086: Unknown result type (might be due to invalid IL or missing references)
		//IL_008b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01da: Unknown result type (might be due to invalid IL or missing references)
		//IL_0192: Unknown result type (might be due to invalid IL or missing references)
		//IL_0197: Unknown result type (might be due to invalid IL or missing references)
		//IL_019c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0101: Unknown result type (might be due to invalid IL or missing references)
		//IL_0106: Unknown result type (might be due to invalid IL or missing references)
		//IL_010b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0110: Unknown result type (might be due to invalid IL or missing references)
		//IL_0115: Unknown result type (might be due to invalid IL or missing references)
		//IL_011a: Unknown result type (might be due to invalid IL or missing references)
		//IL_011f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0124: Unknown result type (might be due to invalid IL or missing references)
		//IL_0129: Unknown result type (might be due to invalid IL or missing references)
		//IL_013c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0141: Unknown result type (might be due to invalid IL or missing references)
		//IL_0146: Unknown result type (might be due to invalid IL or missing references)
		//IL_014b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0151: Unknown result type (might be due to invalid IL or missing references)
		//IL_0156: Unknown result type (might be due to invalid IL or missing references)
		//IL_016f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0174: Unknown result type (might be due to invalid IL or missing references)
		//IL_017a: Unknown result type (might be due to invalid IL or missing references)
		//IL_017f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0186: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01be: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c4: Unknown result type (might be due to invalid IL or missing references)
		switch (m_State)
		{
		case State.Default:
		{
			if (GetRaycastResult(out var entity2, out var hit2, out var forceUpdate) && entity2 == m_LastRaycastEntity && !forceUpdate)
			{
				base.applyMode = ApplyMode.None;
				return inputDeps;
			}
			m_LastRaycastEntity = entity2;
			base.applyMode = ApplyMode.Clear;
			return UpdateDefinitions(inputDeps, entity2, hit2.m_CellIndex.x, default(float3), setPosition: false);
		}
		case State.MouseDownPrepare:
		{
			if (GetRaycastResult(out Entity _, out RaycastHit hit4))
			{
				m_MouseDownPosition = hit4.m_HitPosition;
				SetState(State.MouseDown);
			}
			base.applyMode = ApplyMode.None;
			return inputDeps;
		}
		case State.MouseDown:
		{
			if (GetRaycastResult(out Entity _, out RaycastHit hit3) && math.distance(hit3.m_HitPosition, m_MouseDownPosition) > 1f)
			{
				StartDragging(hit3);
			}
			base.applyMode = ApplyMode.None;
			return inputDeps;
		}
		case State.Dragging:
		{
			if (GetRaycastResult(out Entity _, out RaycastHit hit))
			{
				if (!((EntityQuery)(ref m_DragQuery)).IsEmptyIgnoreFilter)
				{
					NativeArray<ArchetypeChunk> val = ((EntityQuery)(ref m_DragQuery)).ToArchetypeChunkArray(AllocatorHandle.op_Implicit((Allocator)3));
					ArchetypeChunk val2 = val[0];
					Entity val3 = ((ArchetypeChunk)(ref val2)).GetNativeArray(((ComponentSystemBase)this).GetEntityTypeHandle())[0];
					ComponentTypeHandle<Transform> componentTypeHandle = InternalCompilerInterface.GetComponentTypeHandle<Transform>(ref __TypeHandle.__Game_Objects_Transform_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef);
					val2 = val[0];
					Transform transform = ((ArchetypeChunk)(ref val2)).GetNativeArray<Transform>(ref componentTypeHandle)[0];
					val.Dispose();
					EntityCommandBuffer val4 = m_ToolOutputBarrier.CreateCommandBuffer();
					transform.m_Position = hit.m_HitPosition;
					((EntityCommandBuffer)(ref val4)).SetComponent<Transform>(val3, transform);
				}
				else
				{
					EntityManager entityManager = ((ComponentSystemBase)this).EntityManager;
					if (((EntityManager)(ref entityManager)).Exists(m_LastRaycastEntity))
					{
						return UpdateDefinitions(inputDeps, m_LastRaycastEntity, hit.m_CellIndex.x, hit.m_HitPosition, setPosition: true);
					}
				}
			}
			base.applyMode = ApplyMode.None;
			return inputDeps;
		}
		default:
			base.applyMode = ApplyMode.None;
			return inputDeps;
		}
	}

	private void StartDragging(RaycastHit raycastHit)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		//IL_011a: Unknown result type (might be due to invalid IL or missing references)
		//IL_011f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0131: Unknown result type (might be due to invalid IL or missing references)
		//IL_0136: Unknown result type (might be due to invalid IL or missing references)
		//IL_013d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0146: Unknown result type (might be due to invalid IL or missing references)
		//IL_014f: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_0079: Unknown result type (might be due to invalid IL or missing references)
		//IL_007e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0083: Unknown result type (might be due to invalid IL or missing references)
		//IL_0088: Unknown result type (might be due to invalid IL or missing references)
		//IL_008e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0093: Unknown result type (might be due to invalid IL or missing references)
		//IL_00af: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00be: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00de: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f9: Unknown result type (might be due to invalid IL or missing references)
		Entity val = Entity.Null;
		Temp temp = default(Temp);
		Transform transform = default(Transform);
		bool flag = false;
		if (allowManipulation && !((EntityQuery)(ref m_DragQuery)).IsEmptyIgnoreFilter)
		{
			NativeArray<ArchetypeChunk> val2 = ((EntityQuery)(ref m_DragQuery)).ToArchetypeChunkArray(AllocatorHandle.op_Implicit((Allocator)3));
			try
			{
				ArchetypeChunk val3 = val2[0];
				val = ((ArchetypeChunk)(ref val3)).GetNativeArray(((ComponentSystemBase)this).GetEntityTypeHandle())[0];
				ComponentTypeHandle<Temp> componentTypeHandle = InternalCompilerInterface.GetComponentTypeHandle<Temp>(ref __TypeHandle.__Game_Tools_Temp_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef);
				val3 = val2[0];
				temp = ((ArchetypeChunk)(ref val3)).GetNativeArray<Temp>(ref componentTypeHandle)[0];
				ComponentTypeHandle<Transform> componentTypeHandle2 = InternalCompilerInterface.GetComponentTypeHandle<Transform>(ref __TypeHandle.__Game_Objects_Transform_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef);
				val3 = val2[0];
				NativeArray<Transform> nativeArray = ((ArchetypeChunk)(ref val3)).GetNativeArray<Transform>(ref componentTypeHandle2);
				if (nativeArray.Length != 0)
				{
					transform = nativeArray[0];
					EntityManager entityManager = ((ComponentSystemBase)this).EntityManager;
					int num;
					if (!((EntityManager)(ref entityManager)).HasComponent<Moving>(val))
					{
						entityManager = ((ComponentSystemBase)this).EntityManager;
						num = (((EntityManager)(ref entityManager)).HasComponent<Game.Objects.Marker>(val) ? 1 : 0);
					}
					else
					{
						num = 1;
					}
					flag = (byte)num != 0;
				}
				else
				{
					flag = false;
				}
			}
			finally
			{
				val2.Dispose();
			}
		}
		if (flag)
		{
			EntityCommandBuffer val4 = m_ToolOutputBarrier.CreateCommandBuffer();
			temp.m_Flags |= TempFlags.Dragging;
			transform.m_Position = raycastHit.m_HitPosition;
			((EntityCommandBuffer)(ref val4)).SetComponent<Temp>(val, temp);
			((EntityCommandBuffer)(ref val4)).SetComponent<Transform>(val, transform);
			((EntityCommandBuffer)(ref val4)).AddComponent<Updated>(val, default(Updated));
			SetState(State.Dragging);
		}
		else
		{
			SetState(State.Default);
		}
	}

	private void StopDragging()
	{
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		//IL_0069: Unknown result type (might be due to invalid IL or missing references)
		//IL_006e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0086: Unknown result type (might be due to invalid IL or missing references)
		//IL_008b: Unknown result type (might be due to invalid IL or missing references)
		//IL_009c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a5: Unknown result type (might be due to invalid IL or missing references)
		if (!((EntityQuery)(ref m_DragQuery)).IsEmptyIgnoreFilter)
		{
			NativeArray<ArchetypeChunk> val = ((EntityQuery)(ref m_DragQuery)).ToArchetypeChunkArray(AllocatorHandle.op_Implicit((Allocator)3));
			ArchetypeChunk val2 = val[0];
			Entity val3 = ((ArchetypeChunk)(ref val2)).GetNativeArray(((ComponentSystemBase)this).GetEntityTypeHandle())[0];
			ComponentTypeHandle<Temp> componentTypeHandle = InternalCompilerInterface.GetComponentTypeHandle<Temp>(ref __TypeHandle.__Game_Tools_Temp_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef);
			val2 = val[0];
			Temp temp = ((ArchetypeChunk)(ref val2)).GetNativeArray<Temp>(ref componentTypeHandle)[0];
			val.Dispose();
			EntityCommandBuffer val4 = m_ToolOutputBarrier.CreateCommandBuffer();
			temp.m_Flags &= ~TempFlags.Dragging;
			((EntityCommandBuffer)(ref val4)).SetComponent<Temp>(val3, temp);
			((EntityCommandBuffer)(ref val4)).AddComponent<Updated>(val3, default(Updated));
		}
		SetState(State.Default);
	}

	private void SetState(State state)
	{
		m_State = state;
	}

	private JobHandle UpdateDefinitions(JobHandle inputDeps, Entity entity, int index, float3 position, bool setPosition)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f9: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0076: Unknown result type (might be due to invalid IL or missing references)
		//IL_007b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0093: Unknown result type (might be due to invalid IL or missing references)
		//IL_0098: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ef: Unknown result type (might be due to invalid IL or missing references)
		//IL_0107: Unknown result type (might be due to invalid IL or missing references)
		//IL_010c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0124: Unknown result type (might be due to invalid IL or missing references)
		//IL_0129: Unknown result type (might be due to invalid IL or missing references)
		//IL_0141: Unknown result type (might be due to invalid IL or missing references)
		//IL_0146: Unknown result type (might be due to invalid IL or missing references)
		//IL_015e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0163: Unknown result type (might be due to invalid IL or missing references)
		//IL_017b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0180: Unknown result type (might be due to invalid IL or missing references)
		//IL_0198: Unknown result type (might be due to invalid IL or missing references)
		//IL_019d: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ef: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f4: Unknown result type (might be due to invalid IL or missing references)
		//IL_020c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0211: Unknown result type (might be due to invalid IL or missing references)
		//IL_0229: Unknown result type (might be due to invalid IL or missing references)
		//IL_022e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0246: Unknown result type (might be due to invalid IL or missing references)
		//IL_024b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0263: Unknown result type (might be due to invalid IL or missing references)
		//IL_0268: Unknown result type (might be due to invalid IL or missing references)
		//IL_0280: Unknown result type (might be due to invalid IL or missing references)
		//IL_0285: Unknown result type (might be due to invalid IL or missing references)
		//IL_029d: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_02bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_02cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_02dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e4: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_02eb: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f1: Unknown result type (might be due to invalid IL or missing references)
		JobHandle val = DestroyDefinitions(m_DefinitionQuery, m_ToolOutputBarrier, inputDeps);
		if (entity != Entity.Null)
		{
			JobHandle val2 = IJobExtensions.Schedule<CreateDefinitionsJob>(new CreateDefinitionsJob
			{
				m_Entity = entity,
				m_Position = position,
				m_SetPosition = setPosition,
				m_Edges = InternalCompilerInterface.GetBufferLookup<ConnectedEdge>(ref __TypeHandle.__Game_Net_ConnectedEdge_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
				m_InstalledUpgrades = InternalCompilerInterface.GetBufferLookup<InstalledUpgrade>(ref __TypeHandle.__Game_Buildings_InstalledUpgrade_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
				m_LocalTransformCacheData = InternalCompilerInterface.GetComponentLookup<LocalTransformCache>(ref __TypeHandle.__Game_Tools_LocalTransformCache_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_EdgeData = InternalCompilerInterface.GetComponentLookup<Edge>(ref __TypeHandle.__Game_Net_Edge_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_NodeData = InternalCompilerInterface.GetComponentLookup<Game.Net.Node>(ref __TypeHandle.__Game_Net_Node_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_CurveData = InternalCompilerInterface.GetComponentLookup<Curve>(ref __TypeHandle.__Game_Net_Curve_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_OwnerData = InternalCompilerInterface.GetComponentLookup<Owner>(ref __TypeHandle.__Game_Common_Owner_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_EditorContainerData = InternalCompilerInterface.GetComponentLookup<EditorContainer>(ref __TypeHandle.__Game_Tools_EditorContainer_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_TransformData = InternalCompilerInterface.GetComponentLookup<Transform>(ref __TypeHandle.__Game_Objects_Transform_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_ElevationData = InternalCompilerInterface.GetComponentLookup<Game.Objects.Elevation>(ref __TypeHandle.__Game_Objects_Elevation_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_AttachedData = InternalCompilerInterface.GetComponentLookup<Attached>(ref __TypeHandle.__Game_Objects_Attached_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_AttachmentData = InternalCompilerInterface.GetComponentLookup<Attachment>(ref __TypeHandle.__Game_Objects_Attachment_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_ServiceUpgradeData = InternalCompilerInterface.GetComponentLookup<Game.Buildings.ServiceUpgrade>(ref __TypeHandle.__Game_Buildings_ServiceUpgrade_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_LotData = InternalCompilerInterface.GetComponentLookup<Game.Areas.Lot>(ref __TypeHandle.__Game_Areas_Lot_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_RoutePositionData = InternalCompilerInterface.GetComponentLookup<Position>(ref __TypeHandle.__Game_Routes_Position_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_RouteConnectedData = InternalCompilerInterface.GetComponentLookup<Connected>(ref __TypeHandle.__Game_Routes_Connected_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_IconData = InternalCompilerInterface.GetComponentLookup<Icon>(ref __TypeHandle.__Game_Notifications_Icon_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_PrefabRefData = InternalCompilerInterface.GetComponentLookup<PrefabRef>(ref __TypeHandle.__Game_Prefabs_PrefabRef_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_AreaNodes = InternalCompilerInterface.GetBufferLookup<Game.Areas.Node>(ref __TypeHandle.__Game_Areas_Node_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
				m_SubAreas = InternalCompilerInterface.GetBufferLookup<Game.Areas.SubArea>(ref __TypeHandle.__Game_Areas_SubArea_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
				m_RouteWaypoints = InternalCompilerInterface.GetBufferLookup<RouteWaypoint>(ref __TypeHandle.__Game_Routes_RouteWaypoint_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
				m_AggregateElements = InternalCompilerInterface.GetBufferLookup<AggregateElement>(ref __TypeHandle.__Game_Net_AggregateElement_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
				m_CommandBuffer = m_ToolOutputBarrier.CreateCommandBuffer()
			}, inputDeps);
			((EntityCommandBufferSystem)m_ToolOutputBarrier).AddJobHandleForProducer(val2);
			val = JobHandle.CombineDependencies(val, val2);
			m_LastSelectedIndex = index;
		}
		return val;
	}

	private JobHandle SelectTempEntity(JobHandle inputDeps, bool toggleSelected)
	{
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		//IL_0069: Unknown result type (might be due to invalid IL or missing references)
		//IL_0081: Unknown result type (might be due to invalid IL or missing references)
		//IL_0086: Unknown result type (might be due to invalid IL or missing references)
		//IL_009e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fa: Unknown result type (might be due to invalid IL or missing references)
		//IL_0112: Unknown result type (might be due to invalid IL or missing references)
		//IL_0117: Unknown result type (might be due to invalid IL or missing references)
		//IL_012f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0134: Unknown result type (might be due to invalid IL or missing references)
		//IL_014c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0151: Unknown result type (might be due to invalid IL or missing references)
		//IL_0169: Unknown result type (might be due to invalid IL or missing references)
		//IL_016e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0186: Unknown result type (might be due to invalid IL or missing references)
		//IL_018b: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_01bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_01cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f2: Unknown result type (might be due to invalid IL or missing references)
		//IL_0200: Unknown result type (might be due to invalid IL or missing references)
		//IL_0205: Unknown result type (might be due to invalid IL or missing references)
		//IL_020b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0224: Unknown result type (might be due to invalid IL or missing references)
		//IL_022b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0252: Unknown result type (might be due to invalid IL or missing references)
		//IL_0270: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_0295: Unknown result type (might be due to invalid IL or missing references)
		//IL_0286: Unknown result type (might be due to invalid IL or missing references)
		if (((EntityQuery)(ref m_TempQuery)).IsEmptyIgnoreFilter)
		{
			m_ToolSystem.selected = Entity.Null;
			return inputDeps;
		}
		JobHandle val = default(JobHandle);
		NativeList<ArchetypeChunk> chunks = ((EntityQuery)(ref m_TempQuery)).ToArchetypeChunkListAsync(AllocatorHandle.op_Implicit((Allocator)3), ref val);
		NativeReference<Entity> selected = default(NativeReference<Entity>);
		selected._002Ector(AllocatorHandle.op_Implicit((Allocator)3), (NativeArrayOptions)1);
		JobHandle val2 = IJobExtensions.Schedule<SelectEntityJob>(new SelectEntityJob
		{
			m_Chunks = chunks,
			m_OwnerType = InternalCompilerInterface.GetComponentTypeHandle<Owner>(ref __TypeHandle.__Game_Common_Owner_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_TempType = InternalCompilerInterface.GetComponentTypeHandle<Temp>(ref __TypeHandle.__Game_Tools_Temp_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_AttachmentType = InternalCompilerInterface.GetComponentTypeHandle<Attachment>(ref __TypeHandle.__Game_Objects_Attachment_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_ControllerType = InternalCompilerInterface.GetComponentTypeHandle<Controller>(ref __TypeHandle.__Game_Vehicles_Controller_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_EntityLookup = InternalCompilerInterface.GetEntityStorageInfoLookup(ref __TypeHandle.__EntityStorageInfoLookup, ref ((SystemBase)this).CheckedStateRef),
			m_TempData = InternalCompilerInterface.GetComponentLookup<Temp>(ref __TypeHandle.__Game_Tools_Temp_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_OwnerData = InternalCompilerInterface.GetComponentLookup<Owner>(ref __TypeHandle.__Game_Common_Owner_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_TargetData = InternalCompilerInterface.GetComponentLookup<Target>(ref __TypeHandle.__Game_Common_Target_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_DebugData = InternalCompilerInterface.GetComponentLookup<Debug>(ref __TypeHandle.__Game_Tools_Debug_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_IconData = InternalCompilerInterface.GetComponentLookup<Icon>(ref __TypeHandle.__Game_Notifications_Icon_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_VehicleData = InternalCompilerInterface.GetComponentLookup<Vehicle>(ref __TypeHandle.__Game_Vehicles_Vehicle_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_BuildingData = InternalCompilerInterface.GetComponentLookup<Building>(ref __TypeHandle.__Game_Buildings_Building_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_DebugSelect = debugSelect,
			m_Selected = selected,
			m_CommandBuffer = m_ToolOutputBarrier.CreateCommandBuffer()
		}, JobHandle.CombineDependencies(inputDeps, val));
		chunks.Dispose(val2);
		((EntityCommandBufferSystem)m_ToolOutputBarrier).AddJobHandleForProducer(val2);
		((JobHandle)(ref val2)).Complete();
		EntityManager entityManager = ((ComponentSystemBase)this).EntityManager;
		if (!((EntityManager)(ref entityManager)).HasBuffer<AggregateElement>(selected.Value))
		{
			m_LastSelectedIndex = -1;
		}
		if (m_ToolSystem.selected != selected.Value || m_ToolSystem.selectedIndex != m_LastSelectedIndex)
		{
			m_ToolSystem.selected = selected.Value;
			m_ToolSystem.selectedIndex = m_LastSelectedIndex;
			PlaySelectedSound(selected.Value, forcePlay: true);
		}
		else if (toggleSelected)
		{
			m_ToolSystem.selected = Entity.Null;
		}
		else
		{
			PlaySelectedSound(selected.Value);
		}
		selected.Dispose();
		return val2;
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
		base.OnCreateForCompiler();
		__AssignQueries(ref ((SystemBase)this).CheckedStateRef);
		__TypeHandle.__AssignHandles(ref ((SystemBase)this).CheckedStateRef);
	}

	[Preserve]
	public DefaultToolSystem()
	{
	}
}

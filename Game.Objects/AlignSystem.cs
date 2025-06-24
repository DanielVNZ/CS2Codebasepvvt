using System;
using System.Runtime.CompilerServices;
using Colossal.Collections;
using Colossal.Mathematics;
using Game.Common;
using Game.Net;
using Game.Prefabs;
using Game.Rendering;
using Game.Simulation;
using Game.Tools;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Entities.Internal;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine.Scripting;

namespace Game.Objects;

[CompilerGenerated]
public class AlignSystem : GameSystemBase
{
	[BurstCompile]
	private struct AlignJob : IJob
	{
		[ReadOnly]
		public EntityTypeHandle m_EntityType;

		[ReadOnly]
		public ComponentTypeHandle<Aligned> m_AlignedType;

		[ReadOnly]
		public ComponentTypeHandle<Attached> m_AttachedType;

		[ReadOnly]
		public ComponentTypeHandle<Owner> m_OwnerType;

		[ReadOnly]
		public ComponentTypeHandle<Temp> m_TempType;

		[ReadOnly]
		public ComponentTypeHandle<PrefabRef> m_PrefabRefType;

		[ReadOnly]
		public ComponentLookup<Updated> m_UpdatedData;

		[ReadOnly]
		public ComponentLookup<Aligned> m_AlignedData;

		[ReadOnly]
		public ComponentLookup<Attached> m_AttachedData;

		[ReadOnly]
		public ComponentLookup<Owner> m_OwnerData;

		[ReadOnly]
		public ComponentLookup<Curve> m_CurveData;

		[ReadOnly]
		public ComponentLookup<Node> m_NodeData;

		[ReadOnly]
		public ComponentLookup<EdgeGeometry> m_EdgeGeometryData;

		[ReadOnly]
		public ComponentLookup<NodeGeometry> m_NodeGeometryData;

		[ReadOnly]
		public ComponentLookup<Temp> m_TempData;

		[ReadOnly]
		public ComponentLookup<PrefabRef> m_PrefabRefData;

		[ReadOnly]
		public ComponentLookup<PillarData> m_PrefabPillarData;

		[ReadOnly]
		public ComponentLookup<ObjectGeometryData> m_PrefabObjectGeometryData;

		[ReadOnly]
		public ComponentLookup<NetGeometryData> m_PrefabNetGeometryData;

		[ReadOnly]
		public ComponentLookup<StackData> m_PrefabStackData;

		[ReadOnly]
		public ComponentLookup<PlaceableObjectData> m_PrefabPlaceableObjectData;

		[ReadOnly]
		public BufferLookup<Game.Prefabs.SubObject> m_PrefabSubObjects;

		[ReadOnly]
		public BufferLookup<PlaceholderObjectElement> m_PrefabPlaceholderObjects;

		public ComponentLookup<Transform> m_TransformData;

		public ComponentLookup<Stack> m_StackData;

		public BufferLookup<SubObject> m_SubObjects;

		[ReadOnly]
		public NativeList<ArchetypeChunk> m_Chunks;

		[ReadOnly]
		public ComponentTypeSet m_AppliedTypes;

		[ReadOnly]
		public TerrainHeightData m_TerrainHeightData;

		[ReadOnly]
		public WaterSurfaceData m_WaterSurfaceData;

		public EntityCommandBuffer m_CommandBuffer;

		public void Execute()
		{
			//IL_000e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0013: Unknown result type (might be due to invalid IL or missing references)
			//IL_0017: Unknown result type (might be due to invalid IL or missing references)
			//IL_001c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0021: Unknown result type (might be due to invalid IL or missing references)
			//IL_002a: Unknown result type (might be due to invalid IL or missing references)
			//IL_002f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0038: Unknown result type (might be due to invalid IL or missing references)
			//IL_003d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0047: Unknown result type (might be due to invalid IL or missing references)
			//IL_004c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0056: Unknown result type (might be due to invalid IL or missing references)
			//IL_005b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0078: Unknown result type (might be due to invalid IL or missing references)
			//IL_007d: Unknown result type (might be due to invalid IL or missing references)
			//IL_00be: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c3: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d5: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e3: Unknown result type (might be due to invalid IL or missing references)
			//IL_0100: Unknown result type (might be due to invalid IL or missing references)
			for (int i = 0; i < m_Chunks.Length; i++)
			{
				ArchetypeChunk val = m_Chunks[i];
				NativeArray<Entity> nativeArray = ((ArchetypeChunk)(ref val)).GetNativeArray(m_EntityType);
				NativeArray<Aligned> nativeArray2 = ((ArchetypeChunk)(ref val)).GetNativeArray<Aligned>(ref m_AlignedType);
				NativeArray<Attached> nativeArray3 = ((ArchetypeChunk)(ref val)).GetNativeArray<Attached>(ref m_AttachedType);
				NativeArray<Owner> nativeArray4 = ((ArchetypeChunk)(ref val)).GetNativeArray<Owner>(ref m_OwnerType);
				NativeArray<PrefabRef> nativeArray5 = ((ArchetypeChunk)(ref val)).GetNativeArray<PrefabRef>(ref m_PrefabRefType);
				bool isTemp = ((ArchetypeChunk)(ref val)).Has<Temp>(ref m_TempType);
				for (int j = 0; j < nativeArray2.Length; j++)
				{
					Entity val2 = nativeArray[j];
					Aligned aligned = nativeArray2[j];
					Owner owner = nativeArray4[j];
					PrefabRef prefabRef = nativeArray5[j];
					Attached attached = default(Attached);
					if (nativeArray3.Length != 0)
					{
						attached = nativeArray3[j];
					}
					if (attached.m_Parent == Entity.Null)
					{
						Transform transform = m_TransformData[val2];
						Transform transform2 = transform;
						Align(val2, aligned, owner, prefabRef, isTemp, ref transform2);
						if (!transform2.Equals(transform))
						{
							MoveObject(val2, transform, transform2);
						}
					}
				}
			}
		}

		private void MoveObject(Entity entity, Transform oldTransform, Transform newTransform)
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_0013: Unknown result type (might be due to invalid IL or missing references)
			//IL_0036: Unknown result type (might be due to invalid IL or missing references)
			//IL_003b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0042: Unknown result type (might be due to invalid IL or missing references)
			//IL_004e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0053: Unknown result type (might be due to invalid IL or missing references)
			//IL_0061: Unknown result type (might be due to invalid IL or missing references)
			//IL_006f: Unknown result type (might be due to invalid IL or missing references)
			//IL_007d: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
			m_TransformData[entity] = newTransform;
			DynamicBuffer<SubObject> val = default(DynamicBuffer<SubObject>);
			if (!m_SubObjects.TryGetBuffer(entity, ref val))
			{
				return;
			}
			Transform inverseParentTransform = ObjectUtils.InverseTransform(oldTransform);
			Owner owner = default(Owner);
			for (int i = 0; i < val.Length; i++)
			{
				Entity subObject = val[i].m_SubObject;
				if (m_OwnerData.TryGetComponent(subObject, ref owner) && !(owner.m_Owner != entity) && m_UpdatedData.HasComponent(subObject) && !m_AlignedData.HasComponent(subObject))
				{
					Transform transform = m_TransformData[subObject];
					Transform newTransform2 = ObjectUtils.LocalToWorld(newTransform, ObjectUtils.WorldToLocal(inverseParentTransform, transform));
					if (!newTransform2.Equals(transform))
					{
						MoveObject(subObject, transform, newTransform2);
					}
				}
			}
		}

		private void Align(Entity entity, Aligned aligned, Owner owner, PrefabRef prefabRef, bool isTemp, ref Transform transform)
		{
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_0019: Unknown result type (might be due to invalid IL or missing references)
			//IL_002d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0032: Unknown result type (might be due to invalid IL or missing references)
			//IL_0037: Unknown result type (might be due to invalid IL or missing references)
			//IL_0076: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e9: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ee: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ff: Unknown result type (might be due to invalid IL or missing references)
			//IL_008a: Unknown result type (might be due to invalid IL or missing references)
			//IL_03b1: Unknown result type (might be due to invalid IL or missing references)
			//IL_0115: Unknown result type (might be due to invalid IL or missing references)
			//IL_0128: Unknown result type (might be due to invalid IL or missing references)
			//IL_042c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0431: Unknown result type (might be due to invalid IL or missing references)
			//IL_0438: Unknown result type (might be due to invalid IL or missing references)
			//IL_043e: Unknown result type (might be due to invalid IL or missing references)
			//IL_03c4: Unknown result type (might be due to invalid IL or missing references)
			//IL_03d7: Unknown result type (might be due to invalid IL or missing references)
			//IL_0147: Unknown result type (might be due to invalid IL or missing references)
			//IL_014e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0153: Unknown result type (might be due to invalid IL or missing references)
			//IL_015f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0166: Unknown result type (might be due to invalid IL or missing references)
			//IL_016b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0458: Unknown result type (might be due to invalid IL or missing references)
			//IL_045a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0461: Unknown result type (might be due to invalid IL or missing references)
			//IL_0466: Unknown result type (might be due to invalid IL or missing references)
			//IL_0406: Unknown result type (might be due to invalid IL or missing references)
			//IL_040d: Unknown result type (might be due to invalid IL or missing references)
			//IL_033f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0344: Unknown result type (might be due to invalid IL or missing references)
			//IL_0349: Unknown result type (might be due to invalid IL or missing references)
			//IL_0352: Unknown result type (might be due to invalid IL or missing references)
			//IL_0357: Unknown result type (might be due to invalid IL or missing references)
			//IL_035c: Unknown result type (might be due to invalid IL or missing references)
			//IL_01e5: Unknown result type (might be due to invalid IL or missing references)
			//IL_01fa: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a9: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ae: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b3: Unknown result type (might be due to invalid IL or missing references)
			//IL_01bc: Unknown result type (might be due to invalid IL or missing references)
			//IL_01c1: Unknown result type (might be due to invalid IL or missing references)
			//IL_01c6: Unknown result type (might be due to invalid IL or missing references)
			//IL_017e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0183: Unknown result type (might be due to invalid IL or missing references)
			//IL_0188: Unknown result type (might be due to invalid IL or missing references)
			//IL_0191: Unknown result type (might be due to invalid IL or missing references)
			//IL_0196: Unknown result type (might be due to invalid IL or missing references)
			//IL_019b: Unknown result type (might be due to invalid IL or missing references)
			//IL_04f1: Unknown result type (might be due to invalid IL or missing references)
			//IL_04fe: Unknown result type (might be due to invalid IL or missing references)
			//IL_04d6: Unknown result type (might be due to invalid IL or missing references)
			//IL_047e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0484: Unknown result type (might be due to invalid IL or missing references)
			//IL_0489: Unknown result type (might be due to invalid IL or missing references)
			//IL_048e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0497: Unknown result type (might be due to invalid IL or missing references)
			//IL_049d: Unknown result type (might be due to invalid IL or missing references)
			//IL_04a2: Unknown result type (might be due to invalid IL or missing references)
			//IL_04a7: Unknown result type (might be due to invalid IL or missing references)
			//IL_035e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0360: Unknown result type (might be due to invalid IL or missing references)
			//IL_0367: Unknown result type (might be due to invalid IL or missing references)
			//IL_036c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0370: Unknown result type (might be due to invalid IL or missing references)
			//IL_037a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0381: Unknown result type (might be due to invalid IL or missing references)
			//IL_0386: Unknown result type (might be due to invalid IL or missing references)
			//IL_038b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0397: Unknown result type (might be due to invalid IL or missing references)
			//IL_0399: Unknown result type (might be due to invalid IL or missing references)
			//IL_039b: Unknown result type (might be due to invalid IL or missing references)
			//IL_02aa: Unknown result type (might be due to invalid IL or missing references)
			//IL_02bf: Unknown result type (might be due to invalid IL or missing references)
			//IL_02dc: Unknown result type (might be due to invalid IL or missing references)
			//IL_02f1: Unknown result type (might be due to invalid IL or missing references)
			//IL_0309: Unknown result type (might be due to invalid IL or missing references)
			//IL_030e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0315: Unknown result type (might be due to invalid IL or missing references)
			//IL_031a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0323: Unknown result type (might be due to invalid IL or missing references)
			//IL_0328: Unknown result type (might be due to invalid IL or missing references)
			//IL_032f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0334: Unknown result type (might be due to invalid IL or missing references)
			PrefabRef ownerPrefabRef = m_PrefabRefData[owner.m_Owner];
			if (!m_PrefabSubObjects.HasBuffer(ownerPrefabRef.m_Prefab))
			{
				return;
			}
			DynamicBuffer<Game.Prefabs.SubObject> val = m_PrefabSubObjects[ownerPrefabRef.m_Prefab];
			if (val.Length == 0)
			{
				return;
			}
			int num = aligned.m_SubObjectIndex % val.Length;
			Game.Prefabs.SubObject subObject = val[num];
			PillarData pillarData = new PillarData
			{
				m_Type = PillarType.None
			};
			if (m_PrefabPillarData.HasComponent(prefabRef.m_Prefab))
			{
				pillarData = m_PrefabPillarData[prefabRef.m_Prefab];
				switch (pillarData.m_Type)
				{
				case PillarType.Vertical:
					return;
				case PillarType.Standalone:
					subObject.m_Flags |= SubObjectFlags.AnchorTop;
					subObject.m_Flags |= SubObjectFlags.OnGround;
					break;
				case PillarType.Base:
					subObject.m_Flags |= SubObjectFlags.OnGround;
					break;
				}
			}
			Transform transform2 = new Transform(float3.zero, quaternion.identity);
			if (m_CurveData.HasComponent(owner.m_Owner))
			{
				EdgeGeometry edgeGeometry = m_EdgeGeometryData[owner.m_Owner];
				Curve curve = m_CurveData[owner.m_Owner];
				float3 val2;
				float3 val3;
				if ((subObject.m_Flags & SubObjectFlags.MiddlePlacement) == 0)
				{
					if (math.distancesq(transform.m_Position, curve.m_Bezier.a) < math.distancesq(transform.m_Position, curve.m_Bezier.d))
					{
						val2 = edgeGeometry.m_Start.m_Right.a;
						val3 = edgeGeometry.m_Start.m_Left.a;
					}
					else
					{
						val2 = edgeGeometry.m_End.m_Left.d;
						val3 = edgeGeometry.m_End.m_Right.d;
					}
				}
				else if ((subObject.m_Flags & SubObjectFlags.EvenSpacing) != 0)
				{
					float num2 = MathUtils.Length(((Bezier4x3)(ref curve.m_Bezier)).xz);
					int num3 = math.max(1, (int)(num2 / math.max(1f, subObject.m_Position.z) - 0.5f));
					float num4 = (float)(math.clamp(aligned.m_SubObjectIndex / val.Length, 0, num3 - 1) + 1) / (float)(num3 + 1);
					subObject.m_Position.z = 0f;
					if (num4 >= 0.5f)
					{
						num4 -= 0.5f;
						edgeGeometry.m_Start = edgeGeometry.m_End;
					}
					num4 = math.saturate(num4 * 2f);
					Bounds1 val4 = default(Bounds1);
					((Bounds1)(ref val4))._002Ector(0f, 1f);
					Bounds1 val5 = default(Bounds1);
					((Bounds1)(ref val5))._002Ector(0f, 1f);
					MathUtils.ClampLength(((Bezier4x3)(ref edgeGeometry.m_Start.m_Left)).xz, ref val4, num4 * MathUtils.Length(((Bezier4x3)(ref edgeGeometry.m_Start.m_Left)).xz));
					MathUtils.ClampLength(((Bezier4x3)(ref edgeGeometry.m_Start.m_Right)).xz, ref val5, num4 * MathUtils.Length(((Bezier4x3)(ref edgeGeometry.m_Start.m_Right)).xz));
					val2 = MathUtils.Position(edgeGeometry.m_Start.m_Left, val4.max);
					val3 = MathUtils.Position(edgeGeometry.m_Start.m_Right, val5.max);
				}
				else
				{
					val2 = edgeGeometry.m_Start.m_Left.d;
					val3 = edgeGeometry.m_Start.m_Right.d;
				}
				float3 position = math.lerp(val2, val3, 0.5f);
				float3 tangent = default(float3);
				((float3)(ref tangent)).xz = MathUtils.Right(((float3)(ref val3)).xz - ((float3)(ref val2)).xz);
				transform2 = new Transform(position, NetUtils.GetNodeRotation(tangent));
			}
			else if (m_NodeData.HasComponent(owner.m_Owner))
			{
				NodeGeometry nodeGeometry = m_NodeGeometryData[owner.m_Owner];
				Node node = m_NodeData[owner.m_Owner];
				if ((subObject.m_Flags & SubObjectFlags.EdgePlacement) != 0)
				{
					subObject.m_Position.z = 0f;
				}
				transform2 = new Transform(node.m_Position, node.m_Rotation)
				{
					m_Position = 
					{
						y = nodeGeometry.m_Position
					}
				};
			}
			quaternion rotation = transform.m_Rotation;
			transform = ObjectUtils.LocalToWorld(transform2, subObject.m_Position, subObject.m_Rotation);
			if ((subObject.m_Flags & SubObjectFlags.RequireDeadEnd) != 0 && math.dot(math.forward(rotation), math.forward(transform.m_Rotation)) < 0f)
			{
				subObject.m_Rotation = math.mul(quaternion.RotateY((float)Math.PI), subObject.m_Rotation);
				transform.m_Rotation = math.mul(transform.m_Rotation, subObject.m_Rotation);
			}
			if (pillarData.m_Type == PillarType.Horizontal)
			{
				Game.Prefabs.SubObject prefabSubObject = subObject;
				prefabSubObject.m_Flags |= SubObjectFlags.AnchorTop;
				prefabSubObject.m_Flags |= SubObjectFlags.OnGround;
				AlignVerticalPillars(entity, aligned, owner, ownerPrefabRef, transform2, prefabSubObject, isTemp, ref prefabRef, ref transform);
			}
			ObjectGeometryData prefabGeometryData = m_PrefabObjectGeometryData[prefabRef.m_Prefab];
			AlignHeight(entity, owner, prefabRef, subObject, prefabGeometryData, transform2, ref transform);
		}

		private void AlignVerticalPillars(Entity entity, Aligned aligned, Owner owner, PrefabRef ownerPrefabRef, Transform parentTransform, Game.Prefabs.SubObject prefabSubObject, bool isTemp, ref PrefabRef prefabRef, ref Transform transform)
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0011: Unknown result type (might be due to invalid IL or missing references)
			//IL_0012: Unknown result type (might be due to invalid IL or missing references)
			//IL_0034: Unknown result type (might be due to invalid IL or missing references)
			//IL_0035: Unknown result type (might be due to invalid IL or missing references)
			//IL_001f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0020: Unknown result type (might be due to invalid IL or missing references)
			//IL_0021: Unknown result type (might be due to invalid IL or missing references)
			if (FindVerticalPillars(entity, aligned, owner, isTemp, out var pillar, out var pillar2))
			{
				if (pillar2 != Entity.Null)
				{
					AlignDoubleVerticalPillars(entity, pillar, pillar2, owner, ownerPrefabRef, parentTransform, prefabSubObject, ref prefabRef, ref transform);
				}
				else
				{
					AlignSingleVerticalPillar(entity, pillar, selectPrefab: true, owner, ownerPrefabRef, parentTransform, prefabSubObject, ref prefabRef, ref transform);
				}
			}
		}

		private void AlignSingleVerticalPillar(Entity entity, Entity pillar1, bool selectPrefab, Owner owner, PrefabRef ownerPrefabRef, Transform parentTransform, Game.Prefabs.SubObject prefabSubObject, ref PrefabRef prefabRef, ref Transform transform)
		{
			//IL_000e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0029: Unknown result type (might be due to invalid IL or missing references)
			//IL_0031: Unknown result type (might be due to invalid IL or missing references)
			//IL_0036: Unknown result type (might be due to invalid IL or missing references)
			//IL_001c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0050: Unknown result type (might be due to invalid IL or missing references)
			//IL_005e: Unknown result type (might be due to invalid IL or missing references)
			//IL_006a: Unknown result type (might be due to invalid IL or missing references)
			//IL_009c: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c8: Unknown result type (might be due to invalid IL or missing references)
			//IL_00cd: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00dd: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e4: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e9: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ee: Unknown result type (might be due to invalid IL or missing references)
			//IL_00fc: Unknown result type (might be due to invalid IL or missing references)
			//IL_01de: Unknown result type (might be due to invalid IL or missing references)
			//IL_0111: Unknown result type (might be due to invalid IL or missing references)
			//IL_0115: Unknown result type (might be due to invalid IL or missing references)
			//IL_011a: Unknown result type (might be due to invalid IL or missing references)
			//IL_015b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0167: Unknown result type (might be due to invalid IL or missing references)
			//IL_0171: Unknown result type (might be due to invalid IL or missing references)
			//IL_0176: Unknown result type (might be due to invalid IL or missing references)
			//IL_0182: Unknown result type (might be due to invalid IL or missing references)
			//IL_0187: Unknown result type (might be due to invalid IL or missing references)
			//IL_018c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0221: Unknown result type (might be due to invalid IL or missing references)
			//IL_0237: Unknown result type (might be due to invalid IL or missing references)
			//IL_01f8: Unknown result type (might be due to invalid IL or missing references)
			//IL_01fd: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ff: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ab: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b2: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b9: Unknown result type (might be due to invalid IL or missing references)
			//IL_01be: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ca: Unknown result type (might be due to invalid IL or missing references)
			//IL_01cc: Unknown result type (might be due to invalid IL or missing references)
			//IL_01d1: Unknown result type (might be due to invalid IL or missing references)
			//IL_01d6: Unknown result type (might be due to invalid IL or missing references)
			//IL_019a: Unknown result type (might be due to invalid IL or missing references)
			//IL_019c: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a1: Unknown result type (might be due to invalid IL or missing references)
			Attached attached = default(Attached);
			if (m_AttachedData.HasComponent(pillar1))
			{
				attached = m_AttachedData[pillar1];
			}
			Transform transform2 = m_TransformData[pillar1];
			if (attached.m_Parent == Entity.Null)
			{
				transform2 = transform;
			}
			PrefabRef prefabRef2 = m_PrefabRefData[pillar1];
			ObjectGeometryData prefabGeometryData = m_PrefabObjectGeometryData[prefabRef2.m_Prefab];
			float num = prefabGeometryData.m_Size.x * 0.5f;
			float num2 = 0f;
			Bounds1 val = default(Bounds1);
			((Bounds1)(ref val))._002Ector(0f, 1000000f);
			if (m_PrefabNetGeometryData.HasComponent(ownerPrefabRef.m_Prefab))
			{
				num2 = m_PrefabNetGeometryData[ownerPrefabRef.m_Prefab].m_ElevatedWidth - 1f;
			}
			if (attached.m_Parent != Entity.Null)
			{
				float3 val2 = transform2.m_Position - transform.m_Position;
				val2.y = 0f;
				float num3 = math.length(val2);
				if (num3 >= 0.1f)
				{
					val2 /= num3;
					float num4 = (0f - num2) * 0.5f;
					float num5 = math.max(num2 * 0.5f, num3 + num);
					float num6 = (num4 + num5) * 0.5f;
					num2 = num5 - num4;
					((Bounds1)(ref val))._002Ector(float2.op_Implicit(math.abs(num3 - num6)));
					float3 val3 = default(float3);
					((float3)(ref val3)).xz = MathUtils.Left(((float3)(ref val2)).xz);
					if (math.dot(math.forward(parentTransform.m_Rotation), val3) < 0f)
					{
						val3 = -val3;
					}
					ref float3 position = ref transform.m_Position;
					((float3)(ref position)).xz = ((float3)(ref position)).xz + ((float3)(ref val2)).xz * num6;
					transform.m_Rotation = quaternion.LookRotation(val3, math.up());
				}
				AlignRotation(transform.m_Rotation, ref transform2);
			}
			else
			{
				transform2 = transform;
			}
			if (selectPrefab)
			{
				SelectHorizontalPillar(entity, prefabSubObject, num2, val, val, ref prefabRef);
			}
			transform2.m_Position.y = transform.m_Position.y;
			AlignHeight(pillar1, owner, prefabRef2, prefabSubObject, prefabGeometryData, parentTransform, ref transform2);
			m_TransformData[pillar1] = transform2;
		}

		private void AlignDoubleVerticalPillars(Entity entity, Entity pillar1, Entity pillar2, Owner owner, PrefabRef ownerPrefabRef, Transform parentTransform, Game.Prefabs.SubObject prefabSubObject, ref PrefabRef prefabRef, ref Transform transform)
		{
			//IL_0016: Unknown result type (might be due to invalid IL or missing references)
			//IL_0031: Unknown result type (might be due to invalid IL or missing references)
			//IL_0024: Unknown result type (might be due to invalid IL or missing references)
			//IL_0047: Unknown result type (might be due to invalid IL or missing references)
			//IL_004c: Unknown result type (might be due to invalid IL or missing references)
			//IL_003f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0082: Unknown result type (might be due to invalid IL or missing references)
			//IL_008f: Unknown result type (might be due to invalid IL or missing references)
			//IL_009c: Unknown result type (might be due to invalid IL or missing references)
			//IL_00aa: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ba: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ce: Unknown result type (might be due to invalid IL or missing references)
			//IL_00dc: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f0: Unknown result type (might be due to invalid IL or missing references)
			//IL_013a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0059: Unknown result type (might be due to invalid IL or missing references)
			//IL_005e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0166: Unknown result type (might be due to invalid IL or missing references)
			//IL_016b: Unknown result type (might be due to invalid IL or missing references)
			//IL_014e: Unknown result type (might be due to invalid IL or missing references)
			//IL_032e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0333: Unknown result type (might be due to invalid IL or missing references)
			//IL_017b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0180: Unknown result type (might be due to invalid IL or missing references)
			//IL_04ef: Unknown result type (might be due to invalid IL or missing references)
			//IL_04f4: Unknown result type (might be due to invalid IL or missing references)
			//IL_04f6: Unknown result type (might be due to invalid IL or missing references)
			//IL_0507: Unknown result type (might be due to invalid IL or missing references)
			//IL_0515: Unknown result type (might be due to invalid IL or missing references)
			//IL_0343: Unknown result type (might be due to invalid IL or missing references)
			//IL_034a: Unknown result type (might be due to invalid IL or missing references)
			//IL_034f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0354: Unknown result type (might be due to invalid IL or missing references)
			//IL_0362: Unknown result type (might be due to invalid IL or missing references)
			//IL_0190: Unknown result type (might be due to invalid IL or missing references)
			//IL_0196: Unknown result type (might be due to invalid IL or missing references)
			//IL_019b: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a0: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ae: Unknown result type (might be due to invalid IL or missing references)
			//IL_054f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0554: Unknown result type (might be due to invalid IL or missing references)
			//IL_0561: Unknown result type (might be due to invalid IL or missing references)
			//IL_0566: Unknown result type (might be due to invalid IL or missing references)
			//IL_0574: Unknown result type (might be due to invalid IL or missing references)
			//IL_0527: Unknown result type (might be due to invalid IL or missing references)
			//IL_0531: Unknown result type (might be due to invalid IL or missing references)
			//IL_0532: Unknown result type (might be due to invalid IL or missing references)
			//IL_0397: Unknown result type (might be due to invalid IL or missing references)
			//IL_039b: Unknown result type (might be due to invalid IL or missing references)
			//IL_03a0: Unknown result type (might be due to invalid IL or missing references)
			//IL_03e9: Unknown result type (might be due to invalid IL or missing references)
			//IL_03f4: Unknown result type (might be due to invalid IL or missing references)
			//IL_03f9: Unknown result type (might be due to invalid IL or missing references)
			//IL_03fb: Unknown result type (might be due to invalid IL or missing references)
			//IL_040c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0416: Unknown result type (might be due to invalid IL or missing references)
			//IL_042f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0439: Unknown result type (might be due to invalid IL or missing references)
			//IL_043e: Unknown result type (might be due to invalid IL or missing references)
			//IL_044a: Unknown result type (might be due to invalid IL or missing references)
			//IL_044f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0454: Unknown result type (might be due to invalid IL or missing references)
			//IL_0378: Unknown result type (might be due to invalid IL or missing references)
			//IL_0382: Unknown result type (might be due to invalid IL or missing references)
			//IL_0383: Unknown result type (might be due to invalid IL or missing references)
			//IL_01dd: Unknown result type (might be due to invalid IL or missing references)
			//IL_01e1: Unknown result type (might be due to invalid IL or missing references)
			//IL_01e6: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ea: Unknown result type (might be due to invalid IL or missing references)
			//IL_01f6: Unknown result type (might be due to invalid IL or missing references)
			//IL_0202: Unknown result type (might be due to invalid IL or missing references)
			//IL_0207: Unknown result type (might be due to invalid IL or missing references)
			//IL_025d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0273: Unknown result type (might be due to invalid IL or missing references)
			//IL_027e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0283: Unknown result type (might be due to invalid IL or missing references)
			//IL_0285: Unknown result type (might be due to invalid IL or missing references)
			//IL_0290: Unknown result type (might be due to invalid IL or missing references)
			//IL_029a: Unknown result type (might be due to invalid IL or missing references)
			//IL_029f: Unknown result type (might be due to invalid IL or missing references)
			//IL_02ab: Unknown result type (might be due to invalid IL or missing references)
			//IL_02b0: Unknown result type (might be due to invalid IL or missing references)
			//IL_02b5: Unknown result type (might be due to invalid IL or missing references)
			//IL_01be: Unknown result type (might be due to invalid IL or missing references)
			//IL_01c8: Unknown result type (might be due to invalid IL or missing references)
			//IL_01c9: Unknown result type (might be due to invalid IL or missing references)
			//IL_0478: Unknown result type (might be due to invalid IL or missing references)
			//IL_047f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0486: Unknown result type (might be due to invalid IL or missing references)
			//IL_048b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0497: Unknown result type (might be due to invalid IL or missing references)
			//IL_0499: Unknown result type (might be due to invalid IL or missing references)
			//IL_049e: Unknown result type (might be due to invalid IL or missing references)
			//IL_04a3: Unknown result type (might be due to invalid IL or missing references)
			//IL_04ac: Unknown result type (might be due to invalid IL or missing references)
			//IL_04b1: Unknown result type (might be due to invalid IL or missing references)
			//IL_04cb: Unknown result type (might be due to invalid IL or missing references)
			//IL_04d0: Unknown result type (might be due to invalid IL or missing references)
			//IL_04d5: Unknown result type (might be due to invalid IL or missing references)
			//IL_04dd: Unknown result type (might be due to invalid IL or missing references)
			//IL_0462: Unknown result type (might be due to invalid IL or missing references)
			//IL_0464: Unknown result type (might be due to invalid IL or missing references)
			//IL_0469: Unknown result type (might be due to invalid IL or missing references)
			//IL_02da: Unknown result type (might be due to invalid IL or missing references)
			//IL_02e1: Unknown result type (might be due to invalid IL or missing references)
			//IL_02e8: Unknown result type (might be due to invalid IL or missing references)
			//IL_02ed: Unknown result type (might be due to invalid IL or missing references)
			//IL_02f9: Unknown result type (might be due to invalid IL or missing references)
			//IL_02fb: Unknown result type (might be due to invalid IL or missing references)
			//IL_0300: Unknown result type (might be due to invalid IL or missing references)
			//IL_0305: Unknown result type (might be due to invalid IL or missing references)
			//IL_030d: Unknown result type (might be due to invalid IL or missing references)
			//IL_031c: Unknown result type (might be due to invalid IL or missing references)
			//IL_02c3: Unknown result type (might be due to invalid IL or missing references)
			//IL_02c5: Unknown result type (might be due to invalid IL or missing references)
			//IL_02ca: Unknown result type (might be due to invalid IL or missing references)
			//IL_05ab: Unknown result type (might be due to invalid IL or missing references)
			//IL_05b0: Unknown result type (might be due to invalid IL or missing references)
			//IL_05b9: Unknown result type (might be due to invalid IL or missing references)
			//IL_05be: Unknown result type (might be due to invalid IL or missing references)
			//IL_05d8: Unknown result type (might be due to invalid IL or missing references)
			//IL_05dd: Unknown result type (might be due to invalid IL or missing references)
			//IL_05e2: Unknown result type (might be due to invalid IL or missing references)
			//IL_05fc: Unknown result type (might be due to invalid IL or missing references)
			//IL_0601: Unknown result type (might be due to invalid IL or missing references)
			//IL_0606: Unknown result type (might be due to invalid IL or missing references)
			//IL_063c: Unknown result type (might be due to invalid IL or missing references)
			//IL_064f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0667: Unknown result type (might be due to invalid IL or missing references)
			//IL_0674: Unknown result type (might be due to invalid IL or missing references)
			Attached a = default(Attached);
			Attached b = default(Attached);
			if (m_AttachedData.HasComponent(pillar1))
			{
				a = m_AttachedData[pillar1];
			}
			if (m_AttachedData.HasComponent(pillar2))
			{
				b = m_AttachedData[pillar2];
			}
			if (b.m_Parent != Entity.Null && a.m_Parent == Entity.Null)
			{
				CommonUtils.Swap(ref pillar1, ref pillar2);
				CommonUtils.Swap(ref a, ref b);
			}
			Transform transform2 = m_TransformData[pillar1];
			Transform transform3 = m_TransformData[pillar2];
			PrefabRef prefabRef2 = m_PrefabRefData[pillar1];
			PrefabRef prefabRef3 = m_PrefabRefData[pillar2];
			ObjectGeometryData prefabGeometryData = m_PrefabObjectGeometryData[prefabRef2.m_Prefab];
			ObjectGeometryData prefabGeometryData2 = m_PrefabObjectGeometryData[prefabRef3.m_Prefab];
			float num = prefabGeometryData.m_Size.x * 0.5f;
			float num2 = prefabGeometryData2.m_Size.x * 0.5f;
			float num3 = num + num2;
			float num4 = 0f;
			Bounds1 offsetRange = default(Bounds1);
			((Bounds1)(ref offsetRange))._002Ector(0f, 1000000f);
			Bounds1 offsetRange2 = default(Bounds1);
			((Bounds1)(ref offsetRange2))._002Ector(0f, 1000000f);
			if (m_PrefabNetGeometryData.HasComponent(ownerPrefabRef.m_Prefab))
			{
				num4 = m_PrefabNetGeometryData[ownerPrefabRef.m_Prefab].m_ElevatedWidth - 1f;
			}
			if (a.m_Parent != Entity.Null && b.m_Parent != Entity.Null)
			{
				float3 val = transform3.m_Position - transform2.m_Position;
				val.y = 0f;
				float num5 = math.length(val);
				if (num5 < num3)
				{
					RemoveObject(pillar2, b, owner);
					AlignSingleVerticalPillar(entity, pillar1, selectPrefab: true, owner, ownerPrefabRef, parentTransform, prefabSubObject, ref prefabRef, ref transform);
					return;
				}
				val /= num5;
				float num6 = math.dot(((float3)(ref val)).xz, ((float3)(ref parentTransform.m_Position)).xz - ((float3)(ref transform2.m_Position)).xz);
				float num7 = math.min(num6 - num4 * 0.5f, 0f - num);
				float num8 = math.max(num6 + num4 * 0.5f, num5 + num2);
				num6 = (num7 + num8) * 0.5f;
				num4 = num8 - num7;
				((Bounds1)(ref offsetRange))._002Ector(float2.op_Implicit(math.abs(0f - num6)));
				((Bounds1)(ref offsetRange2))._002Ector(float2.op_Implicit(math.abs(num5 - num6)));
				SelectHorizontalPillar(entity, prefabSubObject, num4, offsetRange, offsetRange2, ref prefabRef);
				float3 val2 = default(float3);
				((float3)(ref val2)).xz = MathUtils.Left(((float3)(ref val)).xz);
				if (math.dot(math.forward(parentTransform.m_Rotation), val2) < 0f)
				{
					val2 = -val2;
				}
				((float3)(ref transform.m_Position)).xz = ((float3)(ref transform2.m_Position)).xz + ((float3)(ref val)).xz * num6;
				transform.m_Rotation = quaternion.LookRotation(val2, math.up());
				AlignRotation(transform.m_Rotation, ref transform2);
				AlignRotation(transform.m_Rotation, ref transform3);
			}
			else if (a.m_Parent != Entity.Null)
			{
				float3 val3 = transform2.m_Position - transform.m_Position;
				val3.y = 0f;
				float num9 = math.length(val3);
				if (num9 < num3 * 0.5f)
				{
					RemoveObject(pillar2, b, owner);
					AlignSingleVerticalPillar(entity, pillar1, selectPrefab: true, owner, ownerPrefabRef, parentTransform, prefabSubObject, ref prefabRef, ref transform);
					return;
				}
				val3 /= num9;
				float num10 = math.min((0f - num4) * 0.5f, 0f - num2);
				float num11 = math.max(num4 * 0.5f, num9 + num);
				float num12 = (num10 + num11) * 0.5f;
				num4 = num11 - num10;
				((Bounds1)(ref offsetRange))._002Ector(float2.op_Implicit(math.abs(num9 - num12)));
				SelectHorizontalPillar(entity, prefabSubObject, num4, offsetRange, offsetRange2, ref prefabRef);
				float num13 = 0f - math.max(MathUtils.Center(m_PrefabPillarData[prefabRef.m_Prefab].m_OffsetRange), num3 - num9);
				float3 val4 = default(float3);
				((float3)(ref val4)).xz = MathUtils.Left(((float3)(ref val3)).xz);
				if (math.dot(math.forward(parentTransform.m_Rotation), val4) < 0f)
				{
					val4 = -val4;
					num13 = 0f - num13;
				}
				ref float3 position = ref transform.m_Position;
				((float3)(ref position)).xz = ((float3)(ref position)).xz + ((float3)(ref val3)).xz * num12;
				transform.m_Rotation = quaternion.LookRotation(val4, math.up());
				transform3.m_Rotation = transform.m_Rotation;
				transform3.m_Position = ObjectUtils.LocalToWorld(transform, new float3(num13, 0f, 0f));
				AlignRotation(transform.m_Rotation, ref transform2);
			}
			else
			{
				SelectHorizontalPillar(entity, prefabSubObject, num4, offsetRange, offsetRange2, ref prefabRef);
				PillarData pillarData = m_PrefabPillarData[prefabRef.m_Prefab];
				if (pillarData.m_OffsetRange.min <= 0f)
				{
					RemoveObject(pillar2, b, owner);
					AlignSingleVerticalPillar(entity, pillar1, selectPrefab: false, owner, ownerPrefabRef, parentTransform, prefabSubObject, ref prefabRef, ref transform);
					return;
				}
				Transform inverseParentTransform = ObjectUtils.InverseTransform(parentTransform);
				float x = ObjectUtils.WorldToLocal(inverseParentTransform, transform2.m_Position).x;
				float x2 = ObjectUtils.WorldToLocal(inverseParentTransform, transform3.m_Position).x;
				float num14 = math.max(MathUtils.Center(pillarData.m_OffsetRange), num3 * 0.5f);
				if (x2 >= x)
				{
					x = 0f - num14;
					x2 = num14;
				}
				else
				{
					x = num14;
					x2 = 0f - num14;
				}
				transform2.m_Rotation = transform.m_Rotation;
				transform3.m_Rotation = transform.m_Rotation;
				transform2.m_Position = ObjectUtils.LocalToWorld(transform, new float3(x, 0f, 0f));
				transform3.m_Position = ObjectUtils.LocalToWorld(transform, new float3(x2, 0f, 0f));
			}
			transform2.m_Position.y = transform.m_Position.y;
			transform3.m_Position.y = transform.m_Position.y;
			AlignHeight(pillar1, owner, prefabRef2, prefabSubObject, prefabGeometryData, parentTransform, ref transform2);
			AlignHeight(pillar2, owner, prefabRef3, prefabSubObject, prefabGeometryData2, parentTransform, ref transform3);
			m_TransformData[pillar1] = transform2;
			m_TransformData[pillar2] = transform3;
		}

		private void SelectHorizontalPillar(Entity entity, Game.Prefabs.SubObject prefabSubObject, float targetWidth, Bounds1 offsetRange1, Bounds1 offsetRange2, ref PrefabRef prefabRef)
		{
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_001b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0020: Unknown result type (might be due to invalid IL or missing references)
			//IL_0025: Unknown result type (might be due to invalid IL or missing references)
			//IL_002e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0033: Unknown result type (might be due to invalid IL or missing references)
			//IL_0043: Unknown result type (might be due to invalid IL or missing references)
			//IL_0048: Unknown result type (might be due to invalid IL or missing references)
			//IL_0050: Unknown result type (might be due to invalid IL or missing references)
			//IL_0191: Unknown result type (might be due to invalid IL or missing references)
			//IL_0194: Unknown result type (might be due to invalid IL or missing references)
			//IL_0062: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a2: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a3: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ae: Unknown result type (might be due to invalid IL or missing references)
			//IL_007e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0090: Unknown result type (might be due to invalid IL or missing references)
			//IL_009b: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b3: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e7: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ec: Unknown result type (might be due to invalid IL or missing references)
			//IL_012f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0134: Unknown result type (might be due to invalid IL or missing references)
			//IL_00fc: Unknown result type (might be due to invalid IL or missing references)
			//IL_0105: Unknown result type (might be due to invalid IL or missing references)
			//IL_0112: Unknown result type (might be due to invalid IL or missing references)
			//IL_011c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0144: Unknown result type (might be due to invalid IL or missing references)
			//IL_014d: Unknown result type (might be due to invalid IL or missing references)
			//IL_015a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0164: Unknown result type (might be due to invalid IL or missing references)
			//IL_017d: Unknown result type (might be due to invalid IL or missing references)
			//IL_017f: Unknown result type (might be due to invalid IL or missing references)
			if (!m_PrefabPlaceholderObjects.HasBuffer(prefabSubObject.m_Prefab))
			{
				return;
			}
			DynamicBuffer<PlaceholderObjectElement> val = m_PrefabPlaceholderObjects[prefabSubObject.m_Prefab];
			float num = float.MinValue;
			Entity val2 = prefabRef.m_Prefab;
			for (int i = 0; i < val.Length; i++)
			{
				Entity val3 = val[i].m_Object;
				if (!m_PrefabPillarData.HasComponent(val3))
				{
					continue;
				}
				PillarData pillarData = m_PrefabPillarData[val3];
				if (pillarData.m_Type == PillarType.Horizontal && m_PrefabObjectGeometryData.HasComponent(val3))
				{
					ObjectGeometryData objectGeometryData = m_PrefabObjectGeometryData[val3];
					float max = pillarData.m_OffsetRange.max;
					float num2 = 1f / (1f + math.abs(objectGeometryData.m_Size.x - targetWidth));
					num2 += 0.01f / (1f + math.max(0f, max));
					if (!MathUtils.Intersect(pillarData.m_OffsetRange, offsetRange1))
					{
						num2 -= 1f + math.max(offsetRange1.min - pillarData.m_OffsetRange.max, pillarData.m_OffsetRange.min - offsetRange1.max);
					}
					if (!MathUtils.Intersect(pillarData.m_OffsetRange, offsetRange2))
					{
						num2 -= 1f + math.max(offsetRange2.min - pillarData.m_OffsetRange.max, pillarData.m_OffsetRange.min - offsetRange2.max);
					}
					if (num2 > num)
					{
						num = num2;
						val2 = val3;
					}
				}
			}
			if (val2 != prefabRef.m_Prefab)
			{
				prefabRef.m_Prefab = val2;
				((EntityCommandBuffer)(ref m_CommandBuffer)).SetComponent<PrefabRef>(entity, prefabRef);
			}
		}

		private bool FindVerticalPillars(Entity entity, Aligned aligned, Owner owner, bool isTemp, out Entity pillar1, out Entity pillar2)
		{
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_000e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0013: Unknown result type (might be due to invalid IL or missing references)
			//IL_0019: Unknown result type (might be due to invalid IL or missing references)
			//IL_001e: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a4: Unknown result type (might be due to invalid IL or missing references)
			//IL_003a: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b9: Unknown result type (might be due to invalid IL or missing references)
			//IL_01be: Unknown result type (might be due to invalid IL or missing references)
			//IL_01c3: Unknown result type (might be due to invalid IL or missing references)
			//IL_01da: Unknown result type (might be due to invalid IL or missing references)
			//IL_01df: Unknown result type (might be due to invalid IL or missing references)
			//IL_02fc: Unknown result type (might be due to invalid IL or missing references)
			//IL_0301: Unknown result type (might be due to invalid IL or missing references)
			//IL_0057: Unknown result type (might be due to invalid IL or missing references)
			//IL_005c: Unknown result type (might be due to invalid IL or missing references)
			//IL_01f2: Unknown result type (might be due to invalid IL or missing references)
			//IL_0072: Unknown result type (might be due to invalid IL or missing references)
			//IL_0077: Unknown result type (might be due to invalid IL or missing references)
			//IL_007c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0085: Unknown result type (might be due to invalid IL or missing references)
			//IL_008a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0093: Unknown result type (might be due to invalid IL or missing references)
			//IL_0098: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a7: Unknown result type (might be due to invalid IL or missing references)
			//IL_0209: Unknown result type (might be due to invalid IL or missing references)
			//IL_022b: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b5: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ba: Unknown result type (might be due to invalid IL or missing references)
			//IL_00bc: Unknown result type (might be due to invalid IL or missing references)
			//IL_00be: Unknown result type (might be due to invalid IL or missing references)
			//IL_0242: Unknown result type (might be due to invalid IL or missing references)
			//IL_024c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0252: Unknown result type (might be due to invalid IL or missing references)
			//IL_0269: Unknown result type (might be due to invalid IL or missing references)
			//IL_027d: Unknown result type (might be due to invalid IL or missing references)
			//IL_00eb: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f1: Unknown result type (might be due to invalid IL or missing references)
			//IL_0291: Unknown result type (might be due to invalid IL or missing references)
			//IL_0110: Unknown result type (might be due to invalid IL or missing references)
			//IL_02a4: Unknown result type (might be due to invalid IL or missing references)
			//IL_02a9: Unknown result type (might be due to invalid IL or missing references)
			//IL_0124: Unknown result type (might be due to invalid IL or missing references)
			//IL_02c7: Unknown result type (might be due to invalid IL or missing references)
			//IL_02cc: Unknown result type (might be due to invalid IL or missing references)
			//IL_02b9: Unknown result type (might be due to invalid IL or missing references)
			//IL_02be: Unknown result type (might be due to invalid IL or missing references)
			//IL_0137: Unknown result type (might be due to invalid IL or missing references)
			//IL_013c: Unknown result type (might be due to invalid IL or missing references)
			//IL_02dc: Unknown result type (might be due to invalid IL or missing references)
			//IL_02e1: Unknown result type (might be due to invalid IL or missing references)
			//IL_0155: Unknown result type (might be due to invalid IL or missing references)
			//IL_015a: Unknown result type (might be due to invalid IL or missing references)
			//IL_014a: Unknown result type (might be due to invalid IL or missing references)
			//IL_014c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0168: Unknown result type (might be due to invalid IL or missing references)
			//IL_016a: Unknown result type (might be due to invalid IL or missing references)
			pillar1 = Entity.Null;
			pillar2 = Entity.Null;
			if (owner.m_Owner == Entity.Null)
			{
				return false;
			}
			if (isTemp && !m_TempData.HasComponent(owner.m_Owner))
			{
				for (int i = 0; i < m_Chunks.Length; i++)
				{
					ArchetypeChunk val = m_Chunks[i];
					if (!((ArchetypeChunk)(ref val)).Has<Temp>(ref m_TempType))
					{
						continue;
					}
					NativeArray<Entity> nativeArray = ((ArchetypeChunk)(ref val)).GetNativeArray(m_EntityType);
					NativeArray<Aligned> nativeArray2 = ((ArchetypeChunk)(ref val)).GetNativeArray<Aligned>(ref m_AlignedType);
					NativeArray<Owner> nativeArray3 = ((ArchetypeChunk)(ref val)).GetNativeArray<Owner>(ref m_OwnerType);
					NativeArray<PrefabRef> nativeArray4 = ((ArchetypeChunk)(ref val)).GetNativeArray<PrefabRef>(ref m_PrefabRefType);
					for (int j = 0; j < nativeArray.Length; j++)
					{
						Entity val2 = nativeArray[j];
						if (val2 == entity || nativeArray2[j].m_SubObjectIndex != aligned.m_SubObjectIndex || nativeArray3[j].m_Owner != owner.m_Owner)
						{
							continue;
						}
						PrefabRef prefabRef = nativeArray4[j];
						if (m_PrefabPillarData.HasComponent(prefabRef.m_Prefab) && m_PrefabPillarData[prefabRef.m_Prefab].m_Type == PillarType.Vertical)
						{
							if (pillar1 == Entity.Null)
							{
								pillar1 = val2;
							}
							else if (pillar2 == Entity.Null)
							{
								pillar2 = val2;
							}
						}
					}
				}
			}
			else
			{
				if (!m_SubObjects.HasBuffer(owner.m_Owner))
				{
					return false;
				}
				DynamicBuffer<SubObject> val3 = m_SubObjects[owner.m_Owner];
				for (int k = 0; k < val3.Length; k++)
				{
					SubObject subObject = val3[k];
					if (subObject.m_SubObject == entity || !m_AlignedData.HasComponent(subObject.m_SubObject) || m_AlignedData[subObject.m_SubObject].m_SubObjectIndex != aligned.m_SubObjectIndex || !m_OwnerData.HasComponent(subObject.m_SubObject) || m_OwnerData[subObject.m_SubObject].m_Owner != owner.m_Owner)
					{
						continue;
					}
					PrefabRef prefabRef2 = m_PrefabRefData[subObject.m_SubObject];
					if (m_PrefabPillarData.HasComponent(prefabRef2.m_Prefab) && m_PrefabPillarData[prefabRef2.m_Prefab].m_Type == PillarType.Vertical)
					{
						if (pillar1 == Entity.Null)
						{
							pillar1 = subObject.m_SubObject;
						}
						else if (pillar2 == Entity.Null)
						{
							pillar2 = subObject.m_SubObject;
						}
					}
				}
			}
			return pillar1 != Entity.Null;
		}

		private void AlignHeight(Entity entity, Owner owner, PrefabRef prefabRef, Game.Prefabs.SubObject prefabSubObject, ObjectGeometryData prefabGeometryData, Transform parentTransform, ref Transform transform)
		{
			//IL_001b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0020: Unknown result type (might be due to invalid IL or missing references)
			//IL_0033: Unknown result type (might be due to invalid IL or missing references)
			//IL_006b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0070: Unknown result type (might be due to invalid IL or missing references)
			//IL_007c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0081: Unknown result type (might be due to invalid IL or missing references)
			//IL_0050: Unknown result type (might be due to invalid IL or missing references)
			//IL_0108: Unknown result type (might be due to invalid IL or missing references)
			//IL_00dd: Unknown result type (might be due to invalid IL or missing references)
			//IL_012c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0131: Unknown result type (might be due to invalid IL or missing references)
			//IL_014a: Unknown result type (might be due to invalid IL or missing references)
			//IL_014f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0165: Unknown result type (might be due to invalid IL or missing references)
			//IL_0208: Unknown result type (might be due to invalid IL or missing references)
			//IL_0182: Unknown result type (might be due to invalid IL or missing references)
			//IL_018d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0199: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a6: Unknown result type (might be due to invalid IL or missing references)
			//IL_01c3: Unknown result type (might be due to invalid IL or missing references)
			//IL_01dd: Unknown result type (might be due to invalid IL or missing references)
			//IL_01e2: Unknown result type (might be due to invalid IL or missing references)
			if ((prefabSubObject.m_Flags & SubObjectFlags.AnchorTop) != 0)
			{
				transform.m_Position.y -= prefabGeometryData.m_Bounds.max.y;
				PlaceableObjectData placeableObjectData = default(PlaceableObjectData);
				if (m_PrefabPlaceableObjectData.TryGetComponent(prefabRef.m_Prefab, ref placeableObjectData))
				{
					transform.m_Position.y += placeableObjectData.m_PlacementOffset.y;
				}
			}
			else if ((prefabSubObject.m_Flags & SubObjectFlags.AnchorCenter) != 0)
			{
				float num = (prefabGeometryData.m_Bounds.max.y - prefabGeometryData.m_Bounds.min.y) * 0.5f;
				transform.m_Position.y -= num;
			}
			float terrainHeight = transform.m_Position.y;
			float num2 = transform.m_Position.y;
			if ((prefabSubObject.m_Flags & SubObjectFlags.OnGround) != 0)
			{
				WaterUtils.SampleHeight(ref m_WaterSurfaceData, ref m_TerrainHeightData, transform.m_Position, out terrainHeight, out var waterHeight, out var waterDepth);
				num2 = math.select(terrainHeight, waterHeight, waterDepth >= 0.2f);
			}
			Stack stack = default(Stack);
			if (m_StackData.TryGetComponent(entity, ref stack))
			{
				stack.m_Range.min = num2 - transform.m_Position.y + prefabGeometryData.m_Bounds.min.y;
				stack.m_Range.max = prefabGeometryData.m_Bounds.max.y;
				StackData stackData = default(StackData);
				if (m_PrefabStackData.TryGetComponent(prefabRef.m_Prefab, ref stackData))
				{
					if (num2 > terrainHeight)
					{
						stack.m_Range.min = math.min(stack.m_Range.min, stack.m_Range.max - MathUtils.Size(stackData.m_FirstBounds) - MathUtils.Size(stackData.m_LastBounds));
						stack.m_Range.min = math.max(stack.m_Range.min, terrainHeight - transform.m_Position.y + prefabGeometryData.m_Bounds.min.y);
					}
					BatchDataHelpers.AlignStack(ref stack, stackData, start: false, end: true);
				}
				m_StackData[entity] = stack;
			}
			else if ((prefabSubObject.m_Flags & (SubObjectFlags.AnchorTop | SubObjectFlags.AnchorCenter | SubObjectFlags.OnGround)) == SubObjectFlags.OnGround)
			{
				transform.m_Position.y = num2;
			}
		}

		private void AlignRotation(quaternion targetRotation, ref Transform transform)
		{
			//IL_0005: Unknown result type (might be due to invalid IL or missing references)
			//IL_000b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0010: Unknown result type (might be due to invalid IL or missing references)
			//IL_0015: Unknown result type (might be due to invalid IL or missing references)
			//IL_001b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0021: Unknown result type (might be due to invalid IL or missing references)
			//IL_0026: Unknown result type (might be due to invalid IL or missing references)
			//IL_002b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0031: Unknown result type (might be due to invalid IL or missing references)
			//IL_0037: Unknown result type (might be due to invalid IL or missing references)
			//IL_003c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0041: Unknown result type (might be due to invalid IL or missing references)
			//IL_0042: Unknown result type (might be due to invalid IL or missing references)
			//IL_0044: Unknown result type (might be due to invalid IL or missing references)
			//IL_004f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0050: Unknown result type (might be due to invalid IL or missing references)
			//IL_0058: Unknown result type (might be due to invalid IL or missing references)
			//IL_0059: Unknown result type (might be due to invalid IL or missing references)
			//IL_0061: Unknown result type (might be due to invalid IL or missing references)
			//IL_0062: Unknown result type (might be due to invalid IL or missing references)
			//IL_0073: Unknown result type (might be due to invalid IL or missing references)
			//IL_0074: Unknown result type (might be due to invalid IL or missing references)
			//IL_0082: Unknown result type (might be due to invalid IL or missing references)
			//IL_0083: Unknown result type (might be due to invalid IL or missing references)
			//IL_0091: Unknown result type (might be due to invalid IL or missing references)
			//IL_0092: Unknown result type (might be due to invalid IL or missing references)
			quaternion val = math.mul(quaternion.RotateY((float)Math.PI / 2f), transform.m_Rotation);
			quaternion val2 = math.mul(quaternion.RotateY((float)Math.PI), transform.m_Rotation);
			quaternion val3 = math.mul(quaternion.RotateY(-(float)Math.PI / 2f), transform.m_Rotation);
			float num = MathUtils.RotationAngle(targetRotation, transform.m_Rotation);
			float num2 = MathUtils.RotationAngle(targetRotation, val);
			float num3 = MathUtils.RotationAngle(targetRotation, val2);
			float num4 = MathUtils.RotationAngle(targetRotation, val3);
			if (num2 < num)
			{
				num = num2;
				transform.m_Rotation = val;
			}
			if (num3 < num)
			{
				num = num3;
				transform.m_Rotation = val2;
			}
			if (num4 < num)
			{
				num = num4;
				transform.m_Rotation = val3;
			}
		}

		private void RemoveObject(Entity entity, Attached attached, Owner owner)
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_0018: Unknown result type (might be due to invalid IL or missing references)
			//IL_001f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0025: Unknown result type (might be due to invalid IL or missing references)
			//IL_0068: Unknown result type (might be due to invalid IL or missing references)
			//IL_0038: Unknown result type (might be due to invalid IL or missing references)
			//IL_007b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0080: Unknown result type (might be due to invalid IL or missing references)
			//IL_0085: Unknown result type (might be due to invalid IL or missing references)
			//IL_004b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0050: Unknown result type (might be due to invalid IL or missing references)
			//IL_0055: Unknown result type (might be due to invalid IL or missing references)
			((EntityCommandBuffer)(ref m_CommandBuffer)).RemoveComponent(entity, ref m_AppliedTypes);
			((EntityCommandBuffer)(ref m_CommandBuffer)).AddComponent<Deleted>(entity);
			if (attached.m_Parent != owner.m_Owner && m_SubObjects.HasBuffer(attached.m_Parent))
			{
				CollectionUtils.RemoveValue<SubObject>(m_SubObjects[attached.m_Parent], new SubObject(entity));
			}
			if (m_SubObjects.HasBuffer(owner.m_Owner))
			{
				CollectionUtils.RemoveValue<SubObject>(m_SubObjects[owner.m_Owner], new SubObject(entity));
			}
		}
	}

	private struct TypeHandle
	{
		[ReadOnly]
		public EntityTypeHandle __Unity_Entities_Entity_TypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<Aligned> __Game_Objects_Aligned_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<Attached> __Game_Objects_Attached_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<Owner> __Game_Common_Owner_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<Temp> __Game_Tools_Temp_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<PrefabRef> __Game_Prefabs_PrefabRef_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentLookup<Updated> __Game_Common_Updated_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Aligned> __Game_Objects_Aligned_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Attached> __Game_Objects_Attached_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Owner> __Game_Common_Owner_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Curve> __Game_Net_Curve_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Node> __Game_Net_Node_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<EdgeGeometry> __Game_Net_EdgeGeometry_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<NodeGeometry> __Game_Net_NodeGeometry_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Temp> __Game_Tools_Temp_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<PrefabRef> __Game_Prefabs_PrefabRef_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<PillarData> __Game_Prefabs_PillarData_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<ObjectGeometryData> __Game_Prefabs_ObjectGeometryData_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<NetGeometryData> __Game_Prefabs_NetGeometryData_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<StackData> __Game_Prefabs_StackData_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<PlaceableObjectData> __Game_Prefabs_PlaceableObjectData_RO_ComponentLookup;

		[ReadOnly]
		public BufferLookup<Game.Prefabs.SubObject> __Game_Prefabs_SubObject_RO_BufferLookup;

		[ReadOnly]
		public BufferLookup<PlaceholderObjectElement> __Game_Prefabs_PlaceholderObjectElement_RO_BufferLookup;

		public ComponentLookup<Transform> __Game_Objects_Transform_RW_ComponentLookup;

		public ComponentLookup<Stack> __Game_Objects_Stack_RW_ComponentLookup;

		public BufferLookup<SubObject> __Game_Objects_SubObject_RW_BufferLookup;

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
			//IL_0043: Unknown result type (might be due to invalid IL or missing references)
			//IL_0048: Unknown result type (might be due to invalid IL or missing references)
			//IL_0050: Unknown result type (might be due to invalid IL or missing references)
			//IL_0055: Unknown result type (might be due to invalid IL or missing references)
			//IL_005d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0062: Unknown result type (might be due to invalid IL or missing references)
			//IL_006a: Unknown result type (might be due to invalid IL or missing references)
			//IL_006f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0077: Unknown result type (might be due to invalid IL or missing references)
			//IL_007c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0084: Unknown result type (might be due to invalid IL or missing references)
			//IL_0089: Unknown result type (might be due to invalid IL or missing references)
			//IL_0091: Unknown result type (might be due to invalid IL or missing references)
			//IL_0096: Unknown result type (might be due to invalid IL or missing references)
			//IL_009e: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ab: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b8: Unknown result type (might be due to invalid IL or missing references)
			//IL_00bd: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c5: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ca: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d7: Unknown result type (might be due to invalid IL or missing references)
			//IL_00df: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e4: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ec: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f9: Unknown result type (might be due to invalid IL or missing references)
			//IL_00fe: Unknown result type (might be due to invalid IL or missing references)
			//IL_0106: Unknown result type (might be due to invalid IL or missing references)
			//IL_010b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0113: Unknown result type (might be due to invalid IL or missing references)
			//IL_0118: Unknown result type (might be due to invalid IL or missing references)
			//IL_0120: Unknown result type (might be due to invalid IL or missing references)
			//IL_0125: Unknown result type (might be due to invalid IL or missing references)
			//IL_012d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0132: Unknown result type (might be due to invalid IL or missing references)
			//IL_013a: Unknown result type (might be due to invalid IL or missing references)
			//IL_013f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0147: Unknown result type (might be due to invalid IL or missing references)
			//IL_014c: Unknown result type (might be due to invalid IL or missing references)
			__Unity_Entities_Entity_TypeHandle = ((SystemState)(ref state)).GetEntityTypeHandle();
			__Game_Objects_Aligned_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<Aligned>(true);
			__Game_Objects_Attached_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<Attached>(true);
			__Game_Common_Owner_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<Owner>(true);
			__Game_Tools_Temp_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<Temp>(true);
			__Game_Prefabs_PrefabRef_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<PrefabRef>(true);
			__Game_Common_Updated_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Updated>(true);
			__Game_Objects_Aligned_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Aligned>(true);
			__Game_Objects_Attached_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Attached>(true);
			__Game_Common_Owner_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Owner>(true);
			__Game_Net_Curve_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Curve>(true);
			__Game_Net_Node_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Node>(true);
			__Game_Net_EdgeGeometry_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<EdgeGeometry>(true);
			__Game_Net_NodeGeometry_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<NodeGeometry>(true);
			__Game_Tools_Temp_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Temp>(true);
			__Game_Prefabs_PrefabRef_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<PrefabRef>(true);
			__Game_Prefabs_PillarData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<PillarData>(true);
			__Game_Prefabs_ObjectGeometryData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<ObjectGeometryData>(true);
			__Game_Prefabs_NetGeometryData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<NetGeometryData>(true);
			__Game_Prefabs_StackData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<StackData>(true);
			__Game_Prefabs_PlaceableObjectData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<PlaceableObjectData>(true);
			__Game_Prefabs_SubObject_RO_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<Game.Prefabs.SubObject>(true);
			__Game_Prefabs_PlaceholderObjectElement_RO_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<PlaceholderObjectElement>(true);
			__Game_Objects_Transform_RW_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Transform>(false);
			__Game_Objects_Stack_RW_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Stack>(false);
			__Game_Objects_SubObject_RW_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<SubObject>(false);
		}
	}

	private ModificationBarrier4 m_ModificationBarrier;

	private TerrainSystem m_TerrainSystem;

	private WaterSystem m_WaterSystem;

	private EntityQuery m_UpdateQuery;

	private ComponentTypeSet m_AppliedTypes;

	private TypeHandle __TypeHandle;

	[Preserve]
	protected override void OnCreate()
	{
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0070: Unknown result type (might be due to invalid IL or missing references)
		//IL_0075: Unknown result type (might be due to invalid IL or missing references)
		//IL_007a: Unknown result type (might be due to invalid IL or missing references)
		//IL_007f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0084: Unknown result type (might be due to invalid IL or missing references)
		//IL_008b: Unknown result type (might be due to invalid IL or missing references)
		base.OnCreate();
		m_ModificationBarrier = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<ModificationBarrier4>();
		m_TerrainSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<TerrainSystem>();
		m_WaterSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<WaterSystem>();
		m_UpdateQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[3]
		{
			ComponentType.ReadOnly<Updated>(),
			ComponentType.ReadOnly<Aligned>(),
			ComponentType.Exclude<Deleted>()
		});
		m_AppliedTypes = new ComponentTypeSet(ComponentType.ReadWrite<Applied>(), ComponentType.ReadWrite<Created>(), ComponentType.ReadWrite<Updated>());
		((ComponentSystemBase)this).RequireForUpdate(m_UpdateQuery);
	}

	[Preserve]
	protected override void OnUpdate()
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_0069: Unknown result type (might be due to invalid IL or missing references)
		//IL_006e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0086: Unknown result type (might be due to invalid IL or missing references)
		//IL_008b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fa: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ff: Unknown result type (might be due to invalid IL or missing references)
		//IL_0117: Unknown result type (might be due to invalid IL or missing references)
		//IL_011c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0134: Unknown result type (might be due to invalid IL or missing references)
		//IL_0139: Unknown result type (might be due to invalid IL or missing references)
		//IL_0151: Unknown result type (might be due to invalid IL or missing references)
		//IL_0156: Unknown result type (might be due to invalid IL or missing references)
		//IL_016e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0173: Unknown result type (might be due to invalid IL or missing references)
		//IL_018b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0190: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ff: Unknown result type (might be due to invalid IL or missing references)
		//IL_0204: Unknown result type (might be due to invalid IL or missing references)
		//IL_021c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0221: Unknown result type (might be due to invalid IL or missing references)
		//IL_0239: Unknown result type (might be due to invalid IL or missing references)
		//IL_023e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0256: Unknown result type (might be due to invalid IL or missing references)
		//IL_025b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0273: Unknown result type (might be due to invalid IL or missing references)
		//IL_0278: Unknown result type (might be due to invalid IL or missing references)
		//IL_0290: Unknown result type (might be due to invalid IL or missing references)
		//IL_0295: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_02cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e7: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_0304: Unknown result type (might be due to invalid IL or missing references)
		//IL_0309: Unknown result type (might be due to invalid IL or missing references)
		//IL_0310: Unknown result type (might be due to invalid IL or missing references)
		//IL_0311: Unknown result type (might be due to invalid IL or missing references)
		//IL_0319: Unknown result type (might be due to invalid IL or missing references)
		//IL_031e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0352: Unknown result type (might be due to invalid IL or missing references)
		//IL_0357: Unknown result type (might be due to invalid IL or missing references)
		//IL_035f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0364: Unknown result type (might be due to invalid IL or missing references)
		//IL_0365: Unknown result type (might be due to invalid IL or missing references)
		//IL_0366: Unknown result type (might be due to invalid IL or missing references)
		//IL_036b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0370: Unknown result type (might be due to invalid IL or missing references)
		//IL_0373: Unknown result type (might be due to invalid IL or missing references)
		//IL_0374: Unknown result type (might be due to invalid IL or missing references)
		//IL_0380: Unknown result type (might be due to invalid IL or missing references)
		//IL_038c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0398: Unknown result type (might be due to invalid IL or missing references)
		//IL_039f: Unknown result type (might be due to invalid IL or missing references)
		JobHandle val = default(JobHandle);
		NativeList<ArchetypeChunk> chunks = ((EntityQuery)(ref m_UpdateQuery)).ToArchetypeChunkListAsync(AllocatorHandle.op_Implicit((Allocator)3), ref val);
		JobHandle deps;
		JobHandle val2 = IJobExtensions.Schedule<AlignJob>(new AlignJob
		{
			m_EntityType = InternalCompilerInterface.GetEntityTypeHandle(ref __TypeHandle.__Unity_Entities_Entity_TypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_AlignedType = InternalCompilerInterface.GetComponentTypeHandle<Aligned>(ref __TypeHandle.__Game_Objects_Aligned_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_AttachedType = InternalCompilerInterface.GetComponentTypeHandle<Attached>(ref __TypeHandle.__Game_Objects_Attached_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_OwnerType = InternalCompilerInterface.GetComponentTypeHandle<Owner>(ref __TypeHandle.__Game_Common_Owner_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_TempType = InternalCompilerInterface.GetComponentTypeHandle<Temp>(ref __TypeHandle.__Game_Tools_Temp_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_PrefabRefType = InternalCompilerInterface.GetComponentTypeHandle<PrefabRef>(ref __TypeHandle.__Game_Prefabs_PrefabRef_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_UpdatedData = InternalCompilerInterface.GetComponentLookup<Updated>(ref __TypeHandle.__Game_Common_Updated_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_AlignedData = InternalCompilerInterface.GetComponentLookup<Aligned>(ref __TypeHandle.__Game_Objects_Aligned_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_AttachedData = InternalCompilerInterface.GetComponentLookup<Attached>(ref __TypeHandle.__Game_Objects_Attached_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_OwnerData = InternalCompilerInterface.GetComponentLookup<Owner>(ref __TypeHandle.__Game_Common_Owner_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_CurveData = InternalCompilerInterface.GetComponentLookup<Curve>(ref __TypeHandle.__Game_Net_Curve_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_NodeData = InternalCompilerInterface.GetComponentLookup<Node>(ref __TypeHandle.__Game_Net_Node_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_EdgeGeometryData = InternalCompilerInterface.GetComponentLookup<EdgeGeometry>(ref __TypeHandle.__Game_Net_EdgeGeometry_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_NodeGeometryData = InternalCompilerInterface.GetComponentLookup<NodeGeometry>(ref __TypeHandle.__Game_Net_NodeGeometry_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_TempData = InternalCompilerInterface.GetComponentLookup<Temp>(ref __TypeHandle.__Game_Tools_Temp_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_PrefabRefData = InternalCompilerInterface.GetComponentLookup<PrefabRef>(ref __TypeHandle.__Game_Prefabs_PrefabRef_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_PrefabPillarData = InternalCompilerInterface.GetComponentLookup<PillarData>(ref __TypeHandle.__Game_Prefabs_PillarData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_PrefabObjectGeometryData = InternalCompilerInterface.GetComponentLookup<ObjectGeometryData>(ref __TypeHandle.__Game_Prefabs_ObjectGeometryData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_PrefabNetGeometryData = InternalCompilerInterface.GetComponentLookup<NetGeometryData>(ref __TypeHandle.__Game_Prefabs_NetGeometryData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_PrefabStackData = InternalCompilerInterface.GetComponentLookup<StackData>(ref __TypeHandle.__Game_Prefabs_StackData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_PrefabPlaceableObjectData = InternalCompilerInterface.GetComponentLookup<PlaceableObjectData>(ref __TypeHandle.__Game_Prefabs_PlaceableObjectData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_PrefabSubObjects = InternalCompilerInterface.GetBufferLookup<Game.Prefabs.SubObject>(ref __TypeHandle.__Game_Prefabs_SubObject_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
			m_PrefabPlaceholderObjects = InternalCompilerInterface.GetBufferLookup<PlaceholderObjectElement>(ref __TypeHandle.__Game_Prefabs_PlaceholderObjectElement_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
			m_TransformData = InternalCompilerInterface.GetComponentLookup<Transform>(ref __TypeHandle.__Game_Objects_Transform_RW_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_StackData = InternalCompilerInterface.GetComponentLookup<Stack>(ref __TypeHandle.__Game_Objects_Stack_RW_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_SubObjects = InternalCompilerInterface.GetBufferLookup<SubObject>(ref __TypeHandle.__Game_Objects_SubObject_RW_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
			m_Chunks = chunks,
			m_AppliedTypes = m_AppliedTypes,
			m_TerrainHeightData = m_TerrainSystem.GetHeightData(waitForPending: true),
			m_WaterSurfaceData = m_WaterSystem.GetSurfaceData(out deps),
			m_CommandBuffer = m_ModificationBarrier.CreateCommandBuffer()
		}, JobHandle.CombineDependencies(((SystemBase)this).Dependency, val, deps));
		chunks.Dispose(val2);
		((EntityCommandBufferSystem)m_ModificationBarrier).AddJobHandleForProducer(val2);
		m_TerrainSystem.AddCPUHeightReader(val2);
		m_WaterSystem.AddSurfaceReader(val2);
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
	public AlignSystem()
	{
	}
}

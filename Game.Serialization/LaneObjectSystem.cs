using System.Runtime.CompilerServices;
using Colossal.Collections;
using Colossal.Mathematics;
using Game.Common;
using Game.Creatures;
using Game.Net;
using Game.Objects;
using Game.Prefabs;
using Game.Simulation;
using Game.Vehicles;
using Unity.Burst;
using Unity.Burst.Intrinsics;
using Unity.Collections;
using Unity.Entities;
using Unity.Entities.Internal;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine.Scripting;

namespace Game.Serialization;

[CompilerGenerated]
public class LaneObjectSystem : GameSystemBase
{
	[BurstCompile]
	private struct LaneObjectJob : IJobChunk
	{
		[ReadOnly]
		public EntityTypeHandle m_EntityType;

		[ReadOnly]
		public ComponentTypeHandle<CarCurrentLane> m_CarCurrentLaneType;

		[ReadOnly]
		public ComponentTypeHandle<CarTrailerLane> m_CarTrailerLaneType;

		[ReadOnly]
		public ComponentTypeHandle<ParkedCar> m_ParkedCarType;

		[ReadOnly]
		public ComponentTypeHandle<ParkedTrain> m_ParkedTrainType;

		[ReadOnly]
		public ComponentTypeHandle<WatercraftCurrentLane> m_WatercraftCurrentLaneType;

		[ReadOnly]
		public ComponentTypeHandle<AircraftCurrentLane> m_AircraftCurrentLaneType;

		[ReadOnly]
		public ComponentTypeHandle<TrainCurrentLane> m_TrainCurrentLaneType;

		[ReadOnly]
		public ComponentTypeHandle<HumanCurrentLane> m_HumanCurrentLaneType;

		[ReadOnly]
		public ComponentTypeHandle<AnimalCurrentLane> m_AnimalCurrentLaneType;

		[ReadOnly]
		public ComponentTypeHandle<Transform> m_TransformType;

		[ReadOnly]
		public ComponentTypeHandle<PrefabRef> m_PrefabRefType;

		[ReadOnly]
		public BufferTypeHandle<BlockedLane> m_BlockedLaneType;

		[ReadOnly]
		public ComponentLookup<ObjectGeometryData> m_ObjectGeometryData;

		public BufferLookup<LaneObject> m_LaneObjects;

		public NativeQuadTree<Entity, QuadTreeBoundsXZ> m_SearchTree;

		public void Execute(in ArchetypeChunk chunk, int unfilteredChunkIndex, bool useEnabledMask, in v128 chunkEnabledMask)
		{
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0014: Unknown result type (might be due to invalid IL or missing references)
			//IL_0019: Unknown result type (might be due to invalid IL or missing references)
			//IL_0021: Unknown result type (might be due to invalid IL or missing references)
			//IL_0026: Unknown result type (might be due to invalid IL or missing references)
			//IL_002e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0033: Unknown result type (might be due to invalid IL or missing references)
			//IL_003b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0040: Unknown result type (might be due to invalid IL or missing references)
			//IL_0049: Unknown result type (might be due to invalid IL or missing references)
			//IL_004e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0057: Unknown result type (might be due to invalid IL or missing references)
			//IL_005c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0065: Unknown result type (might be due to invalid IL or missing references)
			//IL_006a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0073: Unknown result type (might be due to invalid IL or missing references)
			//IL_0078: Unknown result type (might be due to invalid IL or missing references)
			//IL_0081: Unknown result type (might be due to invalid IL or missing references)
			//IL_0086: Unknown result type (might be due to invalid IL or missing references)
			//IL_008f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0094: Unknown result type (might be due to invalid IL or missing references)
			//IL_009d: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ab: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00be: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c3: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d8: Unknown result type (might be due to invalid IL or missing references)
			//IL_0129: Unknown result type (might be due to invalid IL or missing references)
			//IL_0137: Unknown result type (might be due to invalid IL or missing references)
			//IL_013e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0145: Unknown result type (might be due to invalid IL or missing references)
			//IL_014a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0152: Unknown result type (might be due to invalid IL or missing references)
			//IL_0154: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ec: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f6: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ff: Unknown result type (might be due to invalid IL or missing references)
			//IL_0168: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b9: Unknown result type (might be due to invalid IL or missing references)
			//IL_01be: Unknown result type (might be due to invalid IL or missing references)
			//IL_01d3: Unknown result type (might be due to invalid IL or missing references)
			//IL_017c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0181: Unknown result type (might be due to invalid IL or missing references)
			//IL_0186: Unknown result type (might be due to invalid IL or missing references)
			//IL_018f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0224: Unknown result type (might be due to invalid IL or missing references)
			//IL_0232: Unknown result type (might be due to invalid IL or missing references)
			//IL_0239: Unknown result type (might be due to invalid IL or missing references)
			//IL_0240: Unknown result type (might be due to invalid IL or missing references)
			//IL_0245: Unknown result type (might be due to invalid IL or missing references)
			//IL_024d: Unknown result type (might be due to invalid IL or missing references)
			//IL_024f: Unknown result type (might be due to invalid IL or missing references)
			//IL_01e7: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ec: Unknown result type (might be due to invalid IL or missing references)
			//IL_01f1: Unknown result type (might be due to invalid IL or missing references)
			//IL_01fa: Unknown result type (might be due to invalid IL or missing references)
			//IL_0263: Unknown result type (might be due to invalid IL or missing references)
			//IL_02b4: Unknown result type (might be due to invalid IL or missing references)
			//IL_02b9: Unknown result type (might be due to invalid IL or missing references)
			//IL_02ce: Unknown result type (might be due to invalid IL or missing references)
			//IL_0277: Unknown result type (might be due to invalid IL or missing references)
			//IL_027c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0281: Unknown result type (might be due to invalid IL or missing references)
			//IL_028a: Unknown result type (might be due to invalid IL or missing references)
			//IL_031f: Unknown result type (might be due to invalid IL or missing references)
			//IL_032d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0334: Unknown result type (might be due to invalid IL or missing references)
			//IL_033b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0340: Unknown result type (might be due to invalid IL or missing references)
			//IL_0348: Unknown result type (might be due to invalid IL or missing references)
			//IL_034a: Unknown result type (might be due to invalid IL or missing references)
			//IL_02e2: Unknown result type (might be due to invalid IL or missing references)
			//IL_02e7: Unknown result type (might be due to invalid IL or missing references)
			//IL_02ec: Unknown result type (might be due to invalid IL or missing references)
			//IL_02f5: Unknown result type (might be due to invalid IL or missing references)
			//IL_0376: Unknown result type (might be due to invalid IL or missing references)
			//IL_037b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0390: Unknown result type (might be due to invalid IL or missing references)
			//IL_03e1: Unknown result type (might be due to invalid IL or missing references)
			//IL_03ef: Unknown result type (might be due to invalid IL or missing references)
			//IL_03f6: Unknown result type (might be due to invalid IL or missing references)
			//IL_03fd: Unknown result type (might be due to invalid IL or missing references)
			//IL_0402: Unknown result type (might be due to invalid IL or missing references)
			//IL_040a: Unknown result type (might be due to invalid IL or missing references)
			//IL_040c: Unknown result type (might be due to invalid IL or missing references)
			//IL_03a4: Unknown result type (might be due to invalid IL or missing references)
			//IL_03a9: Unknown result type (might be due to invalid IL or missing references)
			//IL_03ae: Unknown result type (might be due to invalid IL or missing references)
			//IL_03b7: Unknown result type (might be due to invalid IL or missing references)
			//IL_0420: Unknown result type (might be due to invalid IL or missing references)
			//IL_0471: Unknown result type (might be due to invalid IL or missing references)
			//IL_0476: Unknown result type (might be due to invalid IL or missing references)
			//IL_048b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0434: Unknown result type (might be due to invalid IL or missing references)
			//IL_0439: Unknown result type (might be due to invalid IL or missing references)
			//IL_043e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0447: Unknown result type (might be due to invalid IL or missing references)
			//IL_04c4: Unknown result type (might be due to invalid IL or missing references)
			//IL_049f: Unknown result type (might be due to invalid IL or missing references)
			//IL_04a4: Unknown result type (might be due to invalid IL or missing references)
			//IL_04a9: Unknown result type (might be due to invalid IL or missing references)
			//IL_04b2: Unknown result type (might be due to invalid IL or missing references)
			//IL_04fd: Unknown result type (might be due to invalid IL or missing references)
			//IL_050b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0512: Unknown result type (might be due to invalid IL or missing references)
			//IL_0519: Unknown result type (might be due to invalid IL or missing references)
			//IL_051e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0526: Unknown result type (might be due to invalid IL or missing references)
			//IL_0528: Unknown result type (might be due to invalid IL or missing references)
			//IL_0554: Unknown result type (might be due to invalid IL or missing references)
			//IL_0559: Unknown result type (might be due to invalid IL or missing references)
			//IL_057e: Unknown result type (might be due to invalid IL or missing references)
			//IL_059e: Unknown result type (might be due to invalid IL or missing references)
			//IL_05aa: Unknown result type (might be due to invalid IL or missing references)
			//IL_058c: Unknown result type (might be due to invalid IL or missing references)
			//IL_058e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0590: Unknown result type (might be due to invalid IL or missing references)
			//IL_05c3: Unknown result type (might be due to invalid IL or missing references)
			//IL_05f9: Unknown result type (might be due to invalid IL or missing references)
			//IL_05fe: Unknown result type (might be due to invalid IL or missing references)
			//IL_061e: Unknown result type (might be due to invalid IL or missing references)
			//IL_05d1: Unknown result type (might be due to invalid IL or missing references)
			//IL_05d3: Unknown result type (might be due to invalid IL or missing references)
			//IL_05d5: Unknown result type (might be due to invalid IL or missing references)
			//IL_0639: Unknown result type (might be due to invalid IL or missing references)
			//IL_0640: Unknown result type (might be due to invalid IL or missing references)
			//IL_062c: Unknown result type (might be due to invalid IL or missing references)
			//IL_062e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0630: Unknown result type (might be due to invalid IL or missing references)
			//IL_0654: Unknown result type (might be due to invalid IL or missing references)
			//IL_068d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0692: Unknown result type (might be due to invalid IL or missing references)
			//IL_06a7: Unknown result type (might be due to invalid IL or missing references)
			//IL_0662: Unknown result type (might be due to invalid IL or missing references)
			//IL_0664: Unknown result type (might be due to invalid IL or missing references)
			//IL_0666: Unknown result type (might be due to invalid IL or missing references)
			//IL_06f8: Unknown result type (might be due to invalid IL or missing references)
			//IL_0706: Unknown result type (might be due to invalid IL or missing references)
			//IL_070d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0714: Unknown result type (might be due to invalid IL or missing references)
			//IL_0719: Unknown result type (might be due to invalid IL or missing references)
			//IL_0721: Unknown result type (might be due to invalid IL or missing references)
			//IL_0723: Unknown result type (might be due to invalid IL or missing references)
			//IL_06bb: Unknown result type (might be due to invalid IL or missing references)
			//IL_06c0: Unknown result type (might be due to invalid IL or missing references)
			//IL_06c5: Unknown result type (might be due to invalid IL or missing references)
			//IL_06ce: Unknown result type (might be due to invalid IL or missing references)
			//IL_074f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0754: Unknown result type (might be due to invalid IL or missing references)
			//IL_0769: Unknown result type (might be due to invalid IL or missing references)
			//IL_07ba: Unknown result type (might be due to invalid IL or missing references)
			//IL_07c8: Unknown result type (might be due to invalid IL or missing references)
			//IL_07cf: Unknown result type (might be due to invalid IL or missing references)
			//IL_07d6: Unknown result type (might be due to invalid IL or missing references)
			//IL_07db: Unknown result type (might be due to invalid IL or missing references)
			//IL_07e3: Unknown result type (might be due to invalid IL or missing references)
			//IL_07e5: Unknown result type (might be due to invalid IL or missing references)
			//IL_077d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0782: Unknown result type (might be due to invalid IL or missing references)
			//IL_0787: Unknown result type (might be due to invalid IL or missing references)
			//IL_0790: Unknown result type (might be due to invalid IL or missing references)
			//IL_080e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0813: Unknown result type (might be due to invalid IL or missing references)
			//IL_0819: Unknown result type (might be due to invalid IL or missing references)
			//IL_081e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0838: Unknown result type (might be due to invalid IL or missing references)
			//IL_084c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0851: Unknown result type (might be due to invalid IL or missing references)
			//IL_0856: Unknown result type (might be due to invalid IL or missing references)
			//IL_085a: Unknown result type (might be due to invalid IL or missing references)
			NativeArray<Entity> nativeArray = ((ArchetypeChunk)(ref chunk)).GetNativeArray(m_EntityType);
			NativeArray<Transform> nativeArray2 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<Transform>(ref m_TransformType);
			NativeArray<PrefabRef> nativeArray3 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<PrefabRef>(ref m_PrefabRefType);
			NativeArray<CarCurrentLane> nativeArray4 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<CarCurrentLane>(ref m_CarCurrentLaneType);
			NativeArray<CarTrailerLane> nativeArray5 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<CarTrailerLane>(ref m_CarTrailerLaneType);
			NativeArray<ParkedCar> nativeArray6 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<ParkedCar>(ref m_ParkedCarType);
			NativeArray<ParkedTrain> nativeArray7 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<ParkedTrain>(ref m_ParkedTrainType);
			NativeArray<WatercraftCurrentLane> nativeArray8 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<WatercraftCurrentLane>(ref m_WatercraftCurrentLaneType);
			NativeArray<AircraftCurrentLane> nativeArray9 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<AircraftCurrentLane>(ref m_AircraftCurrentLaneType);
			NativeArray<TrainCurrentLane> nativeArray10 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<TrainCurrentLane>(ref m_TrainCurrentLaneType);
			NativeArray<HumanCurrentLane> nativeArray11 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<HumanCurrentLane>(ref m_HumanCurrentLaneType);
			NativeArray<AnimalCurrentLane> nativeArray12 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<AnimalCurrentLane>(ref m_AnimalCurrentLaneType);
			BufferAccessor<BlockedLane> bufferAccessor = ((ArchetypeChunk)(ref chunk)).GetBufferAccessor<BlockedLane>(ref m_BlockedLaneType);
			for (int i = 0; i < nativeArray4.Length; i++)
			{
				Entity val = nativeArray[i];
				CarCurrentLane carCurrentLane = nativeArray4[i];
				if (m_LaneObjects.HasBuffer(carCurrentLane.m_Lane))
				{
					NetUtils.AddLaneObject(m_LaneObjects[carCurrentLane.m_Lane], val, ((float3)(ref carCurrentLane.m_CurvePosition)).xy);
				}
				else
				{
					Transform transform = nativeArray2[i];
					PrefabRef prefabRef = nativeArray3[i];
					ObjectGeometryData geometryData = m_ObjectGeometryData[prefabRef.m_Prefab];
					Bounds3 bounds = ObjectUtils.CalculateBounds(transform.m_Position, transform.m_Rotation, geometryData);
					m_SearchTree.Add(val, new QuadTreeBoundsXZ(bounds));
				}
				if (m_LaneObjects.HasBuffer(carCurrentLane.m_ChangeLane))
				{
					NetUtils.AddLaneObject(m_LaneObjects[carCurrentLane.m_ChangeLane], val, ((float3)(ref carCurrentLane.m_CurvePosition)).xy);
				}
			}
			for (int j = 0; j < nativeArray5.Length; j++)
			{
				Entity val2 = nativeArray[j];
				CarTrailerLane carTrailerLane = nativeArray5[j];
				if (m_LaneObjects.HasBuffer(carTrailerLane.m_Lane))
				{
					NetUtils.AddLaneObject(m_LaneObjects[carTrailerLane.m_Lane], val2, ((float2)(ref carTrailerLane.m_CurvePosition)).xy);
				}
				else
				{
					Transform transform2 = nativeArray2[j];
					PrefabRef prefabRef2 = nativeArray3[j];
					ObjectGeometryData geometryData2 = m_ObjectGeometryData[prefabRef2.m_Prefab];
					Bounds3 bounds2 = ObjectUtils.CalculateBounds(transform2.m_Position, transform2.m_Rotation, geometryData2);
					m_SearchTree.Add(val2, new QuadTreeBoundsXZ(bounds2));
				}
				if (m_LaneObjects.HasBuffer(carTrailerLane.m_NextLane))
				{
					NetUtils.AddLaneObject(m_LaneObjects[carTrailerLane.m_NextLane], val2, ((float2)(ref carTrailerLane.m_NextPosition)).xy);
				}
			}
			for (int k = 0; k < nativeArray6.Length; k++)
			{
				Entity val3 = nativeArray[k];
				ParkedCar parkedCar = nativeArray6[k];
				if (m_LaneObjects.HasBuffer(parkedCar.m_Lane))
				{
					NetUtils.AddLaneObject(m_LaneObjects[parkedCar.m_Lane], val3, float2.op_Implicit(parkedCar.m_CurvePosition));
					continue;
				}
				Transform transform3 = nativeArray2[k];
				PrefabRef prefabRef3 = nativeArray3[k];
				ObjectGeometryData geometryData3 = m_ObjectGeometryData[prefabRef3.m_Prefab];
				Bounds3 bounds3 = ObjectUtils.CalculateBounds(transform3.m_Position, transform3.m_Rotation, geometryData3);
				m_SearchTree.Add(val3, new QuadTreeBoundsXZ(bounds3));
			}
			for (int l = 0; l < nativeArray8.Length; l++)
			{
				Entity val4 = nativeArray[l];
				WatercraftCurrentLane watercraftCurrentLane = nativeArray8[l];
				if (m_LaneObjects.HasBuffer(watercraftCurrentLane.m_Lane))
				{
					NetUtils.AddLaneObject(m_LaneObjects[watercraftCurrentLane.m_Lane], val4, ((float3)(ref watercraftCurrentLane.m_CurvePosition)).xy);
				}
				else
				{
					Transform transform4 = nativeArray2[l];
					PrefabRef prefabRef4 = nativeArray3[l];
					ObjectGeometryData geometryData4 = m_ObjectGeometryData[prefabRef4.m_Prefab];
					Bounds3 bounds4 = ObjectUtils.CalculateBounds(transform4.m_Position, transform4.m_Rotation, geometryData4);
					m_SearchTree.Add(val4, new QuadTreeBoundsXZ(bounds4));
				}
				if (m_LaneObjects.HasBuffer(watercraftCurrentLane.m_ChangeLane))
				{
					NetUtils.AddLaneObject(m_LaneObjects[watercraftCurrentLane.m_ChangeLane], val4, ((float3)(ref watercraftCurrentLane.m_CurvePosition)).xy);
				}
			}
			for (int m = 0; m < nativeArray9.Length; m++)
			{
				Entity val5 = nativeArray[m];
				AircraftCurrentLane aircraftCurrentLane = nativeArray9[m];
				if (m_LaneObjects.HasBuffer(aircraftCurrentLane.m_Lane))
				{
					NetUtils.AddLaneObject(m_LaneObjects[aircraftCurrentLane.m_Lane], val5, ((float3)(ref aircraftCurrentLane.m_CurvePosition)).xy);
				}
				if (!m_LaneObjects.HasBuffer(aircraftCurrentLane.m_Lane) || (aircraftCurrentLane.m_LaneFlags & AircraftLaneFlags.Flying) != 0)
				{
					Transform transform5 = nativeArray2[m];
					PrefabRef prefabRef5 = nativeArray3[m];
					ObjectGeometryData geometryData5 = m_ObjectGeometryData[prefabRef5.m_Prefab];
					Bounds3 bounds5 = ObjectUtils.CalculateBounds(transform5.m_Position, transform5.m_Rotation, geometryData5);
					m_SearchTree.Add(val5, new QuadTreeBoundsXZ(bounds5));
				}
			}
			DynamicBuffer<LaneObject> buffer = default(DynamicBuffer<LaneObject>);
			for (int n = 0; n < nativeArray10.Length; n++)
			{
				Entity laneObject = nativeArray[n];
				TrainCurrentLane currentLane = nativeArray10[n];
				TrainNavigationHelpers.GetCurvePositions(ref currentLane, out var pos, out var pos2);
				if (m_LaneObjects.TryGetBuffer(currentLane.m_Front.m_Lane, ref buffer))
				{
					NetUtils.AddLaneObject(buffer, laneObject, pos);
				}
				if (currentLane.m_Rear.m_Lane != currentLane.m_Front.m_Lane && m_LaneObjects.TryGetBuffer(currentLane.m_Rear.m_Lane, ref buffer))
				{
					NetUtils.AddLaneObject(buffer, laneObject, pos2);
				}
			}
			DynamicBuffer<LaneObject> buffer2 = default(DynamicBuffer<LaneObject>);
			for (int num = 0; num < nativeArray7.Length; num++)
			{
				Entity laneObject2 = nativeArray[num];
				ParkedTrain parkedTrain = nativeArray7[num];
				TrainNavigationHelpers.GetCurvePositions(ref parkedTrain, out var pos3, out var pos4);
				if (m_LaneObjects.TryGetBuffer(parkedTrain.m_FrontLane, ref buffer2))
				{
					NetUtils.AddLaneObject(buffer2, laneObject2, pos3);
				}
				if (parkedTrain.m_RearLane != parkedTrain.m_FrontLane && m_LaneObjects.TryGetBuffer(parkedTrain.m_RearLane, ref buffer2))
				{
					NetUtils.AddLaneObject(buffer2, laneObject2, pos4);
				}
			}
			for (int num2 = 0; num2 < nativeArray11.Length; num2++)
			{
				Entity val6 = nativeArray[num2];
				HumanCurrentLane humanCurrentLane = nativeArray11[num2];
				if (m_LaneObjects.HasBuffer(humanCurrentLane.m_Lane))
				{
					NetUtils.AddLaneObject(m_LaneObjects[humanCurrentLane.m_Lane], val6, ((float2)(ref humanCurrentLane.m_CurvePosition)).xx);
					continue;
				}
				Transform transform6 = nativeArray2[num2];
				PrefabRef prefabRef6 = nativeArray3[num2];
				ObjectGeometryData geometryData6 = m_ObjectGeometryData[prefabRef6.m_Prefab];
				Bounds3 bounds6 = ObjectUtils.CalculateBounds(transform6.m_Position, transform6.m_Rotation, geometryData6);
				m_SearchTree.Add(val6, new QuadTreeBoundsXZ(bounds6));
			}
			for (int num3 = 0; num3 < nativeArray12.Length; num3++)
			{
				Entity val7 = nativeArray[num3];
				AnimalCurrentLane animalCurrentLane = nativeArray12[num3];
				if (m_LaneObjects.HasBuffer(animalCurrentLane.m_Lane))
				{
					NetUtils.AddLaneObject(m_LaneObjects[animalCurrentLane.m_Lane], val7, ((float2)(ref animalCurrentLane.m_CurvePosition)).xx);
					continue;
				}
				Transform transform7 = nativeArray2[num3];
				PrefabRef prefabRef7 = nativeArray3[num3];
				ObjectGeometryData geometryData7 = m_ObjectGeometryData[prefabRef7.m_Prefab];
				Bounds3 bounds7 = ObjectUtils.CalculateBounds(transform7.m_Position, transform7.m_Rotation, geometryData7);
				m_SearchTree.Add(val7, new QuadTreeBoundsXZ(bounds7));
			}
			for (int num4 = 0; num4 < bufferAccessor.Length; num4++)
			{
				Entity laneObject3 = nativeArray[num4];
				DynamicBuffer<BlockedLane> val8 = bufferAccessor[num4];
				for (int num5 = 0; num5 < val8.Length; num5++)
				{
					BlockedLane blockedLane = val8[num5];
					if (m_LaneObjects.HasBuffer(blockedLane.m_Lane))
					{
						NetUtils.AddLaneObject(m_LaneObjects[blockedLane.m_Lane], laneObject3, blockedLane.m_CurvePosition);
					}
				}
			}
		}

		void IJobChunk.Execute(in ArchetypeChunk chunk, int unfilteredChunkIndex, bool useEnabledMask, in v128 chunkEnabledMask)
		{
			Execute(in chunk, unfilteredChunkIndex, useEnabledMask, in chunkEnabledMask);
		}
	}

	private struct TypeHandle
	{
		[ReadOnly]
		public EntityTypeHandle __Unity_Entities_Entity_TypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<CarCurrentLane> __Game_Vehicles_CarCurrentLane_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<CarTrailerLane> __Game_Vehicles_CarTrailerLane_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<ParkedCar> __Game_Vehicles_ParkedCar_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<ParkedTrain> __Game_Vehicles_ParkedTrain_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<WatercraftCurrentLane> __Game_Vehicles_WatercraftCurrentLane_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<AircraftCurrentLane> __Game_Vehicles_AircraftCurrentLane_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<TrainCurrentLane> __Game_Vehicles_TrainCurrentLane_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<HumanCurrentLane> __Game_Creatures_HumanCurrentLane_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<AnimalCurrentLane> __Game_Creatures_AnimalCurrentLane_RO_ComponentTypeHandle;

		[ReadOnly]
		public BufferTypeHandle<BlockedLane> __Game_Objects_BlockedLane_RO_BufferTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<Transform> __Game_Objects_Transform_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<PrefabRef> __Game_Prefabs_PrefabRef_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentLookup<ObjectGeometryData> __Game_Prefabs_ObjectGeometryData_RO_ComponentLookup;

		public BufferLookup<LaneObject> __Game_Net_LaneObject_RW_BufferLookup;

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
			__Unity_Entities_Entity_TypeHandle = ((SystemState)(ref state)).GetEntityTypeHandle();
			__Game_Vehicles_CarCurrentLane_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<CarCurrentLane>(true);
			__Game_Vehicles_CarTrailerLane_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<CarTrailerLane>(true);
			__Game_Vehicles_ParkedCar_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<ParkedCar>(true);
			__Game_Vehicles_ParkedTrain_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<ParkedTrain>(true);
			__Game_Vehicles_WatercraftCurrentLane_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<WatercraftCurrentLane>(true);
			__Game_Vehicles_AircraftCurrentLane_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<AircraftCurrentLane>(true);
			__Game_Vehicles_TrainCurrentLane_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<TrainCurrentLane>(true);
			__Game_Creatures_HumanCurrentLane_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<HumanCurrentLane>(true);
			__Game_Creatures_AnimalCurrentLane_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<AnimalCurrentLane>(true);
			__Game_Objects_BlockedLane_RO_BufferTypeHandle = ((SystemState)(ref state)).GetBufferTypeHandle<BlockedLane>(true);
			__Game_Objects_Transform_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<Transform>(true);
			__Game_Prefabs_PrefabRef_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<PrefabRef>(true);
			__Game_Prefabs_ObjectGeometryData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<ObjectGeometryData>(true);
			__Game_Net_LaneObject_RW_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<LaneObject>(false);
		}
	}

	private Game.Objects.SearchSystem m_ObjectSearchSystem;

	private EntityQuery m_Query;

	private TypeHandle __TypeHandle;

	[Preserve]
	protected override void OnCreate()
	{
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Expected O, but got Unknown
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		//IL_0079: Unknown result type (might be due to invalid IL or missing references)
		//IL_007e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0085: Unknown result type (might be due to invalid IL or missing references)
		//IL_008a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0091: Unknown result type (might be due to invalid IL or missing references)
		//IL_0096: Unknown result type (might be due to invalid IL or missing references)
		//IL_009e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00af: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bb: Unknown result type (might be due to invalid IL or missing references)
		base.OnCreate();
		m_ObjectSearchSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<Game.Objects.SearchSystem>();
		EntityQueryDesc[] array = new EntityQueryDesc[1];
		EntityQueryDesc val = new EntityQueryDesc();
		val.Any = (ComponentType[])(object)new ComponentType[10]
		{
			ComponentType.ReadOnly<CarCurrentLane>(),
			ComponentType.ReadOnly<CarTrailerLane>(),
			ComponentType.ReadOnly<ParkedCar>(),
			ComponentType.ReadOnly<ParkedTrain>(),
			ComponentType.ReadOnly<WatercraftCurrentLane>(),
			ComponentType.ReadOnly<AircraftCurrentLane>(),
			ComponentType.ReadOnly<TrainCurrentLane>(),
			ComponentType.ReadOnly<HumanCurrentLane>(),
			ComponentType.ReadOnly<AnimalCurrentLane>(),
			ComponentType.ReadOnly<BlockedLane>()
		};
		array[0] = val;
		m_Query = ((ComponentSystemBase)this).GetEntityQuery((EntityQueryDesc[])(object)array);
		((ComponentSystemBase)this).RequireForUpdate(m_Query);
	}

	[Preserve]
	protected override void OnUpdate()
	{
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		//IL_0077: Unknown result type (might be due to invalid IL or missing references)
		//IL_008f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0094: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00eb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0103: Unknown result type (might be due to invalid IL or missing references)
		//IL_0108: Unknown result type (might be due to invalid IL or missing references)
		//IL_0120: Unknown result type (might be due to invalid IL or missing references)
		//IL_0125: Unknown result type (might be due to invalid IL or missing references)
		//IL_013d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0142: Unknown result type (might be due to invalid IL or missing references)
		//IL_015a: Unknown result type (might be due to invalid IL or missing references)
		//IL_015f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0177: Unknown result type (might be due to invalid IL or missing references)
		//IL_017c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0194: Unknown result type (might be due to invalid IL or missing references)
		//IL_0199: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_01de: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ef: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f6: Unknown result type (might be due to invalid IL or missing references)
		JobHandle dependencies;
		JobHandle val = JobChunkExtensions.Schedule<LaneObjectJob>(new LaneObjectJob
		{
			m_EntityType = InternalCompilerInterface.GetEntityTypeHandle(ref __TypeHandle.__Unity_Entities_Entity_TypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_CarCurrentLaneType = InternalCompilerInterface.GetComponentTypeHandle<CarCurrentLane>(ref __TypeHandle.__Game_Vehicles_CarCurrentLane_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_CarTrailerLaneType = InternalCompilerInterface.GetComponentTypeHandle<CarTrailerLane>(ref __TypeHandle.__Game_Vehicles_CarTrailerLane_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_ParkedCarType = InternalCompilerInterface.GetComponentTypeHandle<ParkedCar>(ref __TypeHandle.__Game_Vehicles_ParkedCar_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_ParkedTrainType = InternalCompilerInterface.GetComponentTypeHandle<ParkedTrain>(ref __TypeHandle.__Game_Vehicles_ParkedTrain_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_WatercraftCurrentLaneType = InternalCompilerInterface.GetComponentTypeHandle<WatercraftCurrentLane>(ref __TypeHandle.__Game_Vehicles_WatercraftCurrentLane_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_AircraftCurrentLaneType = InternalCompilerInterface.GetComponentTypeHandle<AircraftCurrentLane>(ref __TypeHandle.__Game_Vehicles_AircraftCurrentLane_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_TrainCurrentLaneType = InternalCompilerInterface.GetComponentTypeHandle<TrainCurrentLane>(ref __TypeHandle.__Game_Vehicles_TrainCurrentLane_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_HumanCurrentLaneType = InternalCompilerInterface.GetComponentTypeHandle<HumanCurrentLane>(ref __TypeHandle.__Game_Creatures_HumanCurrentLane_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_AnimalCurrentLaneType = InternalCompilerInterface.GetComponentTypeHandle<AnimalCurrentLane>(ref __TypeHandle.__Game_Creatures_AnimalCurrentLane_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_BlockedLaneType = InternalCompilerInterface.GetBufferTypeHandle<BlockedLane>(ref __TypeHandle.__Game_Objects_BlockedLane_RO_BufferTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_TransformType = InternalCompilerInterface.GetComponentTypeHandle<Transform>(ref __TypeHandle.__Game_Objects_Transform_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_PrefabRefType = InternalCompilerInterface.GetComponentTypeHandle<PrefabRef>(ref __TypeHandle.__Game_Prefabs_PrefabRef_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_ObjectGeometryData = InternalCompilerInterface.GetComponentLookup<ObjectGeometryData>(ref __TypeHandle.__Game_Prefabs_ObjectGeometryData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_LaneObjects = InternalCompilerInterface.GetBufferLookup<LaneObject>(ref __TypeHandle.__Game_Net_LaneObject_RW_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
			m_SearchTree = m_ObjectSearchSystem.GetMovingSearchTree(readOnly: false, out dependencies)
		}, m_Query, JobHandle.CombineDependencies(((SystemBase)this).Dependency, dependencies));
		m_ObjectSearchSystem.AddMovingSearchTreeWriter(val);
		((SystemBase)this).Dependency = val;
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
	public LaneObjectSystem()
	{
	}
}

using System.Runtime.CompilerServices;
using Colossal.Mathematics;
using Game.Common;
using Game.Objects;
using Game.Prefabs;
using Game.Rendering;
using Game.Tools;
using Game.Vehicles;
using Unity.Burst;
using Unity.Burst.Intrinsics;
using Unity.Collections;
using Unity.Entities;
using Unity.Entities.Internal;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine.Scripting;

namespace Game.Simulation;

[CompilerGenerated]
public class TrainMoveSystem : GameSystemBase
{
	[BurstCompile]
	private struct UpdateTransformDataJob : IJobChunk
	{
		[ReadOnly]
		public ComponentTypeHandle<Train> m_TrainType;

		[ReadOnly]
		public ComponentTypeHandle<TrainCurrentLane> m_CurrentLaneType;

		[ReadOnly]
		public ComponentTypeHandle<TrainNavigation> m_NavigationType;

		[ReadOnly]
		public ComponentTypeHandle<PrefabRef> m_PrefabRefType;

		public ComponentTypeHandle<Moving> m_MovingType;

		public ComponentTypeHandle<Transform> m_TransformType;

		public BufferTypeHandle<TransformFrame> m_TransformFrameType;

		public BufferTypeHandle<TrainBogieFrame> m_BogieFrameType;

		[ReadOnly]
		public ComponentLookup<TrainData> m_PrefabTrainData;

		[ReadOnly]
		public uint m_TransformFrameIndex;

		public void Execute(in ArchetypeChunk chunk, int unfilteredChunkIndex, bool useEnabledMask, in v128 chunkEnabledMask)
		{
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
			//IL_00bc: Unknown result type (might be due to invalid IL or missing references)
			//IL_00dc: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e8: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ed: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f2: Unknown result type (might be due to invalid IL or missing references)
			//IL_0124: Unknown result type (might be due to invalid IL or missing references)
			//IL_0116: Unknown result type (might be due to invalid IL or missing references)
			//IL_011b: Unknown result type (might be due to invalid IL or missing references)
			//IL_014e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0153: Unknown result type (might be due to invalid IL or missing references)
			//IL_0155: Unknown result type (might be due to invalid IL or missing references)
			//IL_015a: Unknown result type (might be due to invalid IL or missing references)
			//IL_015f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0161: Unknown result type (might be due to invalid IL or missing references)
			//IL_0166: Unknown result type (might be due to invalid IL or missing references)
			//IL_016a: Unknown result type (might be due to invalid IL or missing references)
			//IL_016f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0137: Unknown result type (might be due to invalid IL or missing references)
			//IL_013c: Unknown result type (might be due to invalid IL or missing references)
			//IL_013e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0143: Unknown result type (might be due to invalid IL or missing references)
			//IL_0196: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a2: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a7: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ac: Unknown result type (might be due to invalid IL or missing references)
			//IL_01d1: Unknown result type (might be due to invalid IL or missing references)
			//IL_01d6: Unknown result type (might be due to invalid IL or missing references)
			//IL_01df: Unknown result type (might be due to invalid IL or missing references)
			//IL_01e4: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ed: Unknown result type (might be due to invalid IL or missing references)
			//IL_01f2: Unknown result type (might be due to invalid IL or missing references)
			//IL_0208: Unknown result type (might be due to invalid IL or missing references)
			//IL_020d: Unknown result type (might be due to invalid IL or missing references)
			//IL_021b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0220: Unknown result type (might be due to invalid IL or missing references)
			//IL_0229: Unknown result type (might be due to invalid IL or missing references)
			//IL_022e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0243: Unknown result type (might be due to invalid IL or missing references)
			//IL_0248: Unknown result type (might be due to invalid IL or missing references)
			//IL_017c: Unknown result type (might be due to invalid IL or missing references)
			//IL_017e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0183: Unknown result type (might be due to invalid IL or missing references)
			//IL_0188: Unknown result type (might be due to invalid IL or missing references)
			NativeArray<PrefabRef> nativeArray = ((ArchetypeChunk)(ref chunk)).GetNativeArray<PrefabRef>(ref m_PrefabRefType);
			NativeArray<Train> nativeArray2 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<Train>(ref m_TrainType);
			NativeArray<TrainCurrentLane> nativeArray3 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<TrainCurrentLane>(ref m_CurrentLaneType);
			NativeArray<TrainNavigation> nativeArray4 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<TrainNavigation>(ref m_NavigationType);
			NativeArray<Moving> nativeArray5 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<Moving>(ref m_MovingType);
			NativeArray<Transform> nativeArray6 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<Transform>(ref m_TransformType);
			BufferAccessor<TransformFrame> bufferAccessor = ((ArchetypeChunk)(ref chunk)).GetBufferAccessor<TransformFrame>(ref m_TransformFrameType);
			BufferAccessor<TrainBogieFrame> bufferAccessor2 = ((ArchetypeChunk)(ref chunk)).GetBufferAccessor<TrainBogieFrame>(ref m_BogieFrameType);
			for (int i = 0; i < ((ArchetypeChunk)(ref chunk)).Count; i++)
			{
				PrefabRef prefabRef = nativeArray[i];
				Train train = nativeArray2[i];
				TrainCurrentLane trainCurrentLane = nativeArray3[i];
				TrainNavigation trainNavigation = nativeArray4[i];
				Moving moving = nativeArray5[i];
				Transform transform = nativeArray6[i];
				TrainData prefabTrainData = m_PrefabTrainData[prefabRef.m_Prefab];
				VehicleUtils.CalculateTrainNavigationPivots(transform, prefabTrainData, out var pivot, out var pivot2);
				float3 val = trainNavigation.m_Rear.m_Position - trainNavigation.m_Front.m_Position;
				bool flag = (train.m_Flags & Game.Vehicles.TrainFlags.Reversed) != 0;
				if (flag)
				{
					CommonUtils.Swap(ref pivot, ref pivot2);
					prefabTrainData.m_BogieOffsets = ((float2)(ref prefabTrainData.m_BogieOffsets)).yx;
				}
				if (!MathUtils.TryNormalize(ref val, prefabTrainData.m_BogieOffsets.x))
				{
					val = transform.m_Position - pivot;
				}
				transform.m_Position = trainNavigation.m_Front.m_Position + val;
				float3 val2 = math.select(-val, val, flag);
				if (MathUtils.TryNormalize(ref val2))
				{
					transform.m_Rotation = quaternion.LookRotationSafe(val2, math.up());
				}
				moving.m_Velocity = trainNavigation.m_Front.m_Direction + trainNavigation.m_Rear.m_Direction;
				MathUtils.TryNormalize(ref moving.m_Velocity, trainNavigation.m_Speed);
				TransformFrame transformFrame = new TransformFrame
				{
					m_Position = transform.m_Position,
					m_Velocity = moving.m_Velocity,
					m_Rotation = transform.m_Rotation
				};
				TrainBogieFrame trainBogieFrame = new TrainBogieFrame
				{
					m_FrontLane = trainCurrentLane.m_Front.m_Lane,
					m_RearLane = trainCurrentLane.m_Rear.m_Lane
				};
				DynamicBuffer<TransformFrame> val3 = bufferAccessor[i];
				val3[(int)m_TransformFrameIndex] = transformFrame;
				DynamicBuffer<TrainBogieFrame> val4 = bufferAccessor2[i];
				val4[(int)m_TransformFrameIndex] = trainBogieFrame;
				nativeArray5[i] = moving;
				nativeArray6[i] = transform;
			}
		}

		void IJobChunk.Execute(in ArchetypeChunk chunk, int unfilteredChunkIndex, bool useEnabledMask, in v128 chunkEnabledMask)
		{
			Execute(in chunk, unfilteredChunkIndex, useEnabledMask, in chunkEnabledMask);
		}
	}

	[BurstCompile]
	private struct UpdateLayoutDataJob : IJobChunk
	{
		[ReadOnly]
		public ComponentTypeHandle<PseudoRandomSeed> m_PseudoRandomSeedType;

		[ReadOnly]
		public BufferTypeHandle<LayoutElement> m_LayoutElementType;

		[ReadOnly]
		public ComponentLookup<Train> m_TrainData;

		[ReadOnly]
		public ComponentLookup<TrainCurrentLane> m_CurrentLaneData;

		[NativeDisableParallelForRestriction]
		public BufferLookup<TransformFrame> m_TransformFrames;

		[ReadOnly]
		public uint m_TransformFrameIndex;

		[ReadOnly]
		public float m_DayLightBrightness;

		public ParallelWriter m_CommandBuffer;

		public void Execute(in ArchetypeChunk chunk, int unfilteredChunkIndex, bool useEnabledMask, in v128 chunkEnabledMask)
		{
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0014: Unknown result type (might be due to invalid IL or missing references)
			//IL_0019: Unknown result type (might be due to invalid IL or missing references)
			//IL_002d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0032: Unknown result type (might be due to invalid IL or missing references)
			//IL_0048: Unknown result type (might be due to invalid IL or missing references)
			//IL_004d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0055: Unknown result type (might be due to invalid IL or missing references)
			//IL_0062: Unknown result type (might be due to invalid IL or missing references)
			//IL_0064: Unknown result type (might be due to invalid IL or missing references)
			//IL_0069: Unknown result type (might be due to invalid IL or missing references)
			//IL_0071: Unknown result type (might be due to invalid IL or missing references)
			//IL_008f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0094: Unknown result type (might be due to invalid IL or missing references)
			//IL_024f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0254: Unknown result type (might be due to invalid IL or missing references)
			//IL_0277: Unknown result type (might be due to invalid IL or missing references)
			//IL_0284: Unknown result type (might be due to invalid IL or missing references)
			//IL_0286: Unknown result type (might be due to invalid IL or missing references)
			//IL_028b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0210: Unknown result type (might be due to invalid IL or missing references)
			//IL_0347: Unknown result type (might be due to invalid IL or missing references)
			NativeArray<PseudoRandomSeed> nativeArray = ((ArchetypeChunk)(ref chunk)).GetNativeArray<PseudoRandomSeed>(ref m_PseudoRandomSeedType);
			BufferAccessor<LayoutElement> bufferAccessor = ((ArchetypeChunk)(ref chunk)).GetBufferAccessor<LayoutElement>(ref m_LayoutElementType);
			for (int i = 0; i < ((ArchetypeChunk)(ref chunk)).Count; i++)
			{
				PseudoRandomSeed pseudoRandomSeed = nativeArray[i];
				DynamicBuffer<LayoutElement> val = bufferAccessor[i];
				if (val.Length == 0)
				{
					continue;
				}
				Entity vehicle = val[0].m_Vehicle;
				Train train = m_TrainData[vehicle];
				DynamicBuffer<TransformFrame> val2 = m_TransformFrames[vehicle];
				TrainCurrentLane trainCurrentLane = m_CurrentLaneData[vehicle];
				TransformFlags transformFlags = TransformFlags.InteriorLights;
				TransformFlags transformFlags2 = TransformFlags.InteriorLights;
				Random random = pseudoRandomSeed.GetRandom(PseudoRandomSeed.kLightState);
				if (m_DayLightBrightness + ((Random)(ref random)).NextFloat(-0.05f, 0.05f) < 0.25f && (trainCurrentLane.m_Front.m_LaneFlags & TrainLaneFlags.HighBeams) != 0)
				{
					transformFlags |= TransformFlags.MainLights | TransformFlags.ExtraLights;
					transformFlags2 |= TransformFlags.MainLights | TransformFlags.ExtraLights;
				}
				else
				{
					transformFlags |= TransformFlags.MainLights;
					transformFlags2 |= TransformFlags.MainLights;
				}
				if ((trainCurrentLane.m_Front.m_LaneFlags & TrainLaneFlags.TurnLeft) != 0)
				{
					transformFlags |= TransformFlags.TurningLeft;
					transformFlags2 |= TransformFlags.TurningRight;
				}
				if ((trainCurrentLane.m_Front.m_LaneFlags & TrainLaneFlags.TurnRight) != 0)
				{
					transformFlags |= TransformFlags.TurningRight;
					transformFlags2 |= TransformFlags.TurningLeft;
				}
				if ((train.m_Flags & Game.Vehicles.TrainFlags.BoardingLeft) != 0)
				{
					transformFlags |= TransformFlags.BoardingLeft;
					transformFlags2 |= TransformFlags.BoardingRight;
				}
				if ((train.m_Flags & Game.Vehicles.TrainFlags.BoardingRight) != 0)
				{
					transformFlags |= TransformFlags.BoardingRight;
					transformFlags2 |= TransformFlags.BoardingLeft;
				}
				TransformFrame transformFrame = val2[(int)m_TransformFrameIndex];
				TransformFlags flags = transformFrame.m_Flags;
				if ((train.m_Flags & Game.Vehicles.TrainFlags.Reversed) != 0)
				{
					transformFrame.m_Flags = transformFlags2;
				}
				else
				{
					transformFrame.m_Flags = transformFlags;
				}
				if ((train.m_Flags & Game.Vehicles.TrainFlags.Pantograph) != 0)
				{
					transformFrame.m_Flags |= TransformFlags.Pantograph;
				}
				if (((flags ^ transformFrame.m_Flags) & (TransformFlags.MainLights | TransformFlags.ExtraLights)) != 0)
				{
					TransformFlags transformFlags3 = (TransformFlags)0u;
					TransformFlags transformFlags4 = (TransformFlags)0u;
					for (int j = 0; j < val2.Length; j++)
					{
						TransformFlags flags2 = val2[j].m_Flags;
						transformFlags3 |= flags2;
						transformFlags4 |= ((j == (int)m_TransformFrameIndex) ? transformFrame.m_Flags : flags2);
					}
					if (((transformFlags3 ^ transformFlags4) & (TransformFlags.MainLights | TransformFlags.ExtraLights)) != 0)
					{
						((ParallelWriter)(ref m_CommandBuffer)).AddComponent<EffectsUpdated>(unfilteredChunkIndex, vehicle, default(EffectsUpdated));
					}
				}
				val2[(int)m_TransformFrameIndex] = transformFrame;
				transformFlags = (TransformFlags)((uint)transformFlags & 0xFFFFFFFCu);
				transformFlags2 = (TransformFlags)((uint)transformFlags2 & 0xFFFFFFFCu);
				for (int k = 1; k < val.Length; k++)
				{
					Entity vehicle2 = val[k].m_Vehicle;
					if (k == val.Length - 1)
					{
						transformFlags |= TransformFlags.RearLights;
						transformFlags2 |= TransformFlags.RearLights;
					}
					Train train2 = m_TrainData[vehicle2];
					DynamicBuffer<TransformFrame> val3 = m_TransformFrames[vehicle2];
					TransformFrame transformFrame2 = val3[(int)m_TransformFrameIndex];
					TransformFlags flags3 = transformFrame2.m_Flags;
					if ((train2.m_Flags & Game.Vehicles.TrainFlags.Reversed) != 0)
					{
						transformFrame2.m_Flags = transformFlags2;
					}
					else
					{
						transformFrame2.m_Flags = transformFlags;
					}
					if ((train2.m_Flags & Game.Vehicles.TrainFlags.Pantograph) != 0)
					{
						transformFrame2.m_Flags |= TransformFlags.Pantograph;
					}
					if (((flags3 ^ transformFrame2.m_Flags) & (TransformFlags.MainLights | TransformFlags.ExtraLights)) != 0)
					{
						TransformFlags transformFlags5 = (TransformFlags)0u;
						TransformFlags transformFlags6 = (TransformFlags)0u;
						for (int l = 0; l < val3.Length; l++)
						{
							TransformFlags flags4 = val3[l].m_Flags;
							transformFlags5 |= flags4;
							transformFlags6 |= ((l == (int)m_TransformFrameIndex) ? transformFrame2.m_Flags : flags4);
						}
						if (((transformFlags5 ^ transformFlags6) & (TransformFlags.MainLights | TransformFlags.ExtraLights)) != 0)
						{
							((ParallelWriter)(ref m_CommandBuffer)).AddComponent<EffectsUpdated>(unfilteredChunkIndex, vehicle2, default(EffectsUpdated));
						}
					}
					val3[(int)m_TransformFrameIndex] = transformFrame2;
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
		public ComponentTypeHandle<Train> __Game_Vehicles_Train_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<TrainCurrentLane> __Game_Vehicles_TrainCurrentLane_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<TrainNavigation> __Game_Vehicles_TrainNavigation_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<PrefabRef> __Game_Prefabs_PrefabRef_RO_ComponentTypeHandle;

		public ComponentTypeHandle<Moving> __Game_Objects_Moving_RW_ComponentTypeHandle;

		public ComponentTypeHandle<Transform> __Game_Objects_Transform_RW_ComponentTypeHandle;

		public BufferTypeHandle<TransformFrame> __Game_Objects_TransformFrame_RW_BufferTypeHandle;

		public BufferTypeHandle<TrainBogieFrame> __Game_Vehicles_TrainBogieFrame_RW_BufferTypeHandle;

		[ReadOnly]
		public ComponentLookup<TrainData> __Game_Prefabs_TrainData_RO_ComponentLookup;

		[ReadOnly]
		public ComponentTypeHandle<PseudoRandomSeed> __Game_Common_PseudoRandomSeed_RO_ComponentTypeHandle;

		[ReadOnly]
		public BufferTypeHandle<LayoutElement> __Game_Vehicles_LayoutElement_RO_BufferTypeHandle;

		[ReadOnly]
		public ComponentLookup<Train> __Game_Vehicles_Train_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<TrainCurrentLane> __Game_Vehicles_TrainCurrentLane_RO_ComponentLookup;

		public BufferLookup<TransformFrame> __Game_Objects_TransformFrame_RW_BufferLookup;

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
			__Game_Vehicles_Train_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<Train>(true);
			__Game_Vehicles_TrainCurrentLane_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<TrainCurrentLane>(true);
			__Game_Vehicles_TrainNavigation_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<TrainNavigation>(true);
			__Game_Prefabs_PrefabRef_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<PrefabRef>(true);
			__Game_Objects_Moving_RW_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<Moving>(false);
			__Game_Objects_Transform_RW_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<Transform>(false);
			__Game_Objects_TransformFrame_RW_BufferTypeHandle = ((SystemState)(ref state)).GetBufferTypeHandle<TransformFrame>(false);
			__Game_Vehicles_TrainBogieFrame_RW_BufferTypeHandle = ((SystemState)(ref state)).GetBufferTypeHandle<TrainBogieFrame>(false);
			__Game_Prefabs_TrainData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<TrainData>(true);
			__Game_Common_PseudoRandomSeed_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<PseudoRandomSeed>(true);
			__Game_Vehicles_LayoutElement_RO_BufferTypeHandle = ((SystemState)(ref state)).GetBufferTypeHandle<LayoutElement>(true);
			__Game_Vehicles_Train_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Train>(true);
			__Game_Vehicles_TrainCurrentLane_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<TrainCurrentLane>(true);
			__Game_Objects_TransformFrame_RW_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<TransformFrame>(false);
		}
	}

	private SimulationSystem m_SimulationSystem;

	private LightingSystem m_LightingSystem;

	private EndFrameBarrier m_EndFrameBarrier;

	private EntityQuery m_TrainQuery;

	private EntityQuery m_LayoutQuery;

	private TypeHandle __TypeHandle;

	public override int GetUpdateInterval(SystemUpdatePhase phase)
	{
		return 16;
	}

	public override int GetUpdateOffset(SystemUpdatePhase phase)
	{
		return 3;
	}

	[Preserve]
	protected override void OnCreate()
	{
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0073: Unknown result type (might be due to invalid IL or missing references)
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
		//IL_007f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0084: Unknown result type (might be due to invalid IL or missing references)
		//IL_008b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0090: Unknown result type (might be due to invalid IL or missing references)
		//IL_0097: Unknown result type (might be due to invalid IL or missing references)
		//IL_009c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00de: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fd: Unknown result type (might be due to invalid IL or missing references)
		//IL_0102: Unknown result type (might be due to invalid IL or missing references)
		//IL_0107: Unknown result type (might be due to invalid IL or missing references)
		//IL_010c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0113: Unknown result type (might be due to invalid IL or missing references)
		base.OnCreate();
		m_SimulationSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<SimulationSystem>();
		m_LightingSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<LightingSystem>();
		m_EndFrameBarrier = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<EndFrameBarrier>();
		m_TrainQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[8]
		{
			ComponentType.ReadOnly<Train>(),
			ComponentType.ReadOnly<TrainNavigation>(),
			ComponentType.ReadWrite<Transform>(),
			ComponentType.ReadWrite<Moving>(),
			ComponentType.ReadWrite<TransformFrame>(),
			ComponentType.Exclude<Deleted>(),
			ComponentType.Exclude<Temp>(),
			ComponentType.Exclude<TripSource>()
		});
		m_LayoutQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[7]
		{
			ComponentType.ReadOnly<Train>(),
			ComponentType.ReadOnly<TrainNavigation>(),
			ComponentType.ReadOnly<LayoutElement>(),
			ComponentType.ReadWrite<TransformFrame>(),
			ComponentType.Exclude<Deleted>(),
			ComponentType.Exclude<Temp>(),
			ComponentType.Exclude<TripSource>()
		});
		((ComponentSystemBase)this).RequireForUpdate(m_TrainQuery);
	}

	[Preserve]
	protected override void OnUpdate()
	{
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0083: Unknown result type (might be due to invalid IL or missing references)
		//IL_0088: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00da: Unknown result type (might be due to invalid IL or missing references)
		//IL_00df: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0114: Unknown result type (might be due to invalid IL or missing references)
		//IL_0119: Unknown result type (might be due to invalid IL or missing references)
		//IL_0144: Unknown result type (might be due to invalid IL or missing references)
		//IL_0149: Unknown result type (might be due to invalid IL or missing references)
		//IL_0161: Unknown result type (might be due to invalid IL or missing references)
		//IL_0166: Unknown result type (might be due to invalid IL or missing references)
		//IL_017e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0183: Unknown result type (might be due to invalid IL or missing references)
		//IL_019b: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ed: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01fb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0201: Unknown result type (might be due to invalid IL or missing references)
		//IL_0206: Unknown result type (might be due to invalid IL or missing references)
		//IL_020b: Unknown result type (might be due to invalid IL or missing references)
		//IL_020d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0212: Unknown result type (might be due to invalid IL or missing references)
		//IL_0213: Unknown result type (might be due to invalid IL or missing references)
		//IL_0218: Unknown result type (might be due to invalid IL or missing references)
		//IL_021f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0226: Unknown result type (might be due to invalid IL or missing references)
		uint transformFrameIndex = m_SimulationSystem.frameIndex / 16 % 4;
		UpdateTransformDataJob updateTransformDataJob = new UpdateTransformDataJob
		{
			m_TrainType = InternalCompilerInterface.GetComponentTypeHandle<Train>(ref __TypeHandle.__Game_Vehicles_Train_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_CurrentLaneType = InternalCompilerInterface.GetComponentTypeHandle<TrainCurrentLane>(ref __TypeHandle.__Game_Vehicles_TrainCurrentLane_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_NavigationType = InternalCompilerInterface.GetComponentTypeHandle<TrainNavigation>(ref __TypeHandle.__Game_Vehicles_TrainNavigation_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_PrefabRefType = InternalCompilerInterface.GetComponentTypeHandle<PrefabRef>(ref __TypeHandle.__Game_Prefabs_PrefabRef_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_MovingType = InternalCompilerInterface.GetComponentTypeHandle<Moving>(ref __TypeHandle.__Game_Objects_Moving_RW_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_TransformType = InternalCompilerInterface.GetComponentTypeHandle<Transform>(ref __TypeHandle.__Game_Objects_Transform_RW_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_TransformFrameType = InternalCompilerInterface.GetBufferTypeHandle<TransformFrame>(ref __TypeHandle.__Game_Objects_TransformFrame_RW_BufferTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_BogieFrameType = InternalCompilerInterface.GetBufferTypeHandle<TrainBogieFrame>(ref __TypeHandle.__Game_Vehicles_TrainBogieFrame_RW_BufferTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_PrefabTrainData = InternalCompilerInterface.GetComponentLookup<TrainData>(ref __TypeHandle.__Game_Prefabs_TrainData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_TransformFrameIndex = transformFrameIndex
		};
		UpdateLayoutDataJob updateLayoutDataJob = new UpdateLayoutDataJob
		{
			m_PseudoRandomSeedType = InternalCompilerInterface.GetComponentTypeHandle<PseudoRandomSeed>(ref __TypeHandle.__Game_Common_PseudoRandomSeed_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_LayoutElementType = InternalCompilerInterface.GetBufferTypeHandle<LayoutElement>(ref __TypeHandle.__Game_Vehicles_LayoutElement_RO_BufferTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_TrainData = InternalCompilerInterface.GetComponentLookup<Train>(ref __TypeHandle.__Game_Vehicles_Train_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_CurrentLaneData = InternalCompilerInterface.GetComponentLookup<TrainCurrentLane>(ref __TypeHandle.__Game_Vehicles_TrainCurrentLane_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_TransformFrames = InternalCompilerInterface.GetBufferLookup<TransformFrame>(ref __TypeHandle.__Game_Objects_TransformFrame_RW_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
			m_TransformFrameIndex = transformFrameIndex,
			m_DayLightBrightness = m_LightingSystem.dayLightBrightness
		};
		EntityCommandBuffer val = m_EndFrameBarrier.CreateCommandBuffer();
		updateLayoutDataJob.m_CommandBuffer = ((EntityCommandBuffer)(ref val)).AsParallelWriter();
		UpdateLayoutDataJob updateLayoutDataJob2 = updateLayoutDataJob;
		JobHandle val2 = JobChunkExtensions.ScheduleParallel<UpdateTransformDataJob>(updateTransformDataJob, m_TrainQuery, ((SystemBase)this).Dependency);
		JobHandle val3 = JobChunkExtensions.ScheduleParallel<UpdateLayoutDataJob>(updateLayoutDataJob2, m_LayoutQuery, val2);
		m_EndFrameBarrier.AddJobHandleForProducer(val3);
		((SystemBase)this).Dependency = val3;
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
	public TrainMoveSystem()
	{
	}
}

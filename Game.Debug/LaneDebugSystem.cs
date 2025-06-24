using System.Runtime.CompilerServices;
using Colossal;
using Colossal.Mathematics;
using Game.Common;
using Game.Net;
using Game.Rendering;
using Game.Tools;
using Unity.Burst;
using Unity.Burst.Intrinsics;
using Unity.Collections;
using Unity.Entities;
using Unity.Entities.Internal;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Scripting;

namespace Game.Debug;

[CompilerGenerated]
public class LaneDebugSystem : BaseDebugSystem
{
	[BurstCompile]
	private struct LaneGizmoJob : IJobChunk
	{
		[ReadOnly]
		public bool m_StandaloneOption;

		[ReadOnly]
		public bool m_SlaveOption;

		[ReadOnly]
		public bool m_MasterOption;

		[ReadOnly]
		public bool m_ConnectionOption;

		[ReadOnly]
		public bool m_OverlapOption;

		[ReadOnly]
		public bool m_ReservedOption;

		[ReadOnly]
		public bool m_BlockageOption;

		[ReadOnly]
		public bool m_ConditionOption;

		[ReadOnly]
		public bool m_SignalsOption;

		[ReadOnly]
		public bool m_PriorityOption;

		[ReadOnly]
		public ComponentTypeHandle<Curve> m_CurveType;

		[ReadOnly]
		public ComponentTypeHandle<CarLane> m_CarLaneType;

		[ReadOnly]
		public ComponentTypeHandle<TrackLane> m_TrackLaneType;

		[ReadOnly]
		public ComponentTypeHandle<ParkingLane> m_ParkingLaneType;

		[ReadOnly]
		public ComponentTypeHandle<PedestrianLane> m_PedestrianLaneType;

		[ReadOnly]
		public ComponentTypeHandle<ConnectionLane> m_ConnectionLaneType;

		[ReadOnly]
		public ComponentTypeHandle<MasterLane> m_MasterLaneType;

		[ReadOnly]
		public ComponentTypeHandle<SlaveLane> m_SlaveLaneType;

		[ReadOnly]
		public ComponentTypeHandle<LaneReservation> m_LaneReservationType;

		[ReadOnly]
		public ComponentTypeHandle<LaneCondition> m_LaneConditionType;

		[ReadOnly]
		public ComponentTypeHandle<LaneSignal> m_LaneSignalType;

		[ReadOnly]
		public ComponentTypeHandle<Temp> m_TempType;

		[ReadOnly]
		public BufferTypeHandle<LaneObject> m_LaneObjectType;

		[ReadOnly]
		public BufferTypeHandle<LaneOverlap> m_LaneOverlapType;

		public GizmoBatcher m_GizmoBatcher;

		public void Execute(in ArchetypeChunk chunk, int unfilteredChunkIndex, bool useEnabledMask, in v128 chunkEnabledMask)
		{
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0014: Unknown result type (might be due to invalid IL or missing references)
			//IL_0019: Unknown result type (might be due to invalid IL or missing references)
			//IL_0021: Unknown result type (might be due to invalid IL or missing references)
			//IL_0026: Unknown result type (might be due to invalid IL or missing references)
			//IL_0307: Unknown result type (might be due to invalid IL or missing references)
			//IL_030c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0315: Unknown result type (might be due to invalid IL or missing references)
			//IL_031a: Unknown result type (might be due to invalid IL or missing references)
			//IL_031c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0321: Unknown result type (might be due to invalid IL or missing references)
			//IL_0323: Unknown result type (might be due to invalid IL or missing references)
			//IL_0328: Unknown result type (might be due to invalid IL or missing references)
			//IL_032a: Unknown result type (might be due to invalid IL or missing references)
			//IL_032f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0ee5: Unknown result type (might be due to invalid IL or missing references)
			//IL_0eea: Unknown result type (might be due to invalid IL or missing references)
			//IL_0ef3: Unknown result type (might be due to invalid IL or missing references)
			//IL_0ef8: Unknown result type (might be due to invalid IL or missing references)
			//IL_0f01: Unknown result type (might be due to invalid IL or missing references)
			//IL_0f06: Unknown result type (might be due to invalid IL or missing references)
			//IL_015a: Unknown result type (might be due to invalid IL or missing references)
			//IL_015f: Unknown result type (might be due to invalid IL or missing references)
			//IL_009a: Unknown result type (might be due to invalid IL or missing references)
			//IL_009f: Unknown result type (might be due to invalid IL or missing references)
			//IL_116d: Unknown result type (might be due to invalid IL or missing references)
			//IL_1172: Unknown result type (might be due to invalid IL or missing references)
			//IL_117b: Unknown result type (might be due to invalid IL or missing references)
			//IL_1180: Unknown result type (might be due to invalid IL or missing references)
			//IL_0376: Unknown result type (might be due to invalid IL or missing references)
			//IL_037d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0382: Unknown result type (might be due to invalid IL or missing references)
			//IL_0384: Unknown result type (might be due to invalid IL or missing references)
			//IL_038b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0390: Unknown result type (might be due to invalid IL or missing references)
			//IL_0392: Unknown result type (might be due to invalid IL or missing references)
			//IL_0399: Unknown result type (might be due to invalid IL or missing references)
			//IL_039e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0216: Unknown result type (might be due to invalid IL or missing references)
			//IL_021b: Unknown result type (might be due to invalid IL or missing references)
			//IL_1281: Unknown result type (might be due to invalid IL or missing references)
			//IL_1286: Unknown result type (might be due to invalid IL or missing references)
			//IL_0f28: Unknown result type (might be due to invalid IL or missing references)
			//IL_0f2d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0716: Unknown result type (might be due to invalid IL or missing references)
			//IL_071b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0724: Unknown result type (might be due to invalid IL or missing references)
			//IL_0729: Unknown result type (might be due to invalid IL or missing references)
			//IL_1199: Unknown result type (might be due to invalid IL or missing references)
			//IL_119e: Unknown result type (might be due to invalid IL or missing references)
			//IL_02c0: Unknown result type (might be due to invalid IL or missing references)
			//IL_02ce: Unknown result type (might be due to invalid IL or missing references)
			//IL_0192: Unknown result type (might be due to invalid IL or missing references)
			//IL_0199: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d6: Unknown result type (might be due to invalid IL or missing references)
			//IL_00dd: Unknown result type (might be due to invalid IL or missing references)
			//IL_12aa: Unknown result type (might be due to invalid IL or missing references)
			//IL_12af: Unknown result type (might be due to invalid IL or missing references)
			//IL_14db: Unknown result type (might be due to invalid IL or missing references)
			//IL_14e0: Unknown result type (might be due to invalid IL or missing references)
			//IL_074d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0752: Unknown result type (might be due to invalid IL or missing references)
			//IL_024e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0255: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a7: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ae: Unknown result type (might be due to invalid IL or missing references)
			//IL_00eb: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f2: Unknown result type (might be due to invalid IL or missing references)
			//IL_1489: Unknown result type (might be due to invalid IL or missing references)
			//IL_1497: Unknown result type (might be due to invalid IL or missing references)
			//IL_143b: Unknown result type (might be due to invalid IL or missing references)
			//IL_145d: Unknown result type (might be due to invalid IL or missing references)
			//IL_11b2: Unknown result type (might be due to invalid IL or missing references)
			//IL_11ca: Unknown result type (might be due to invalid IL or missing references)
			//IL_0f6f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0f80: Unknown result type (might be due to invalid IL or missing references)
			//IL_0263: Unknown result type (might be due to invalid IL or missing references)
			//IL_026a: Unknown result type (might be due to invalid IL or missing references)
			//IL_12da: Unknown result type (might be due to invalid IL or missing references)
			//IL_12e1: Unknown result type (might be due to invalid IL or missing references)
			//IL_12eb: Unknown result type (might be due to invalid IL or missing references)
			//IL_122c: Unknown result type (might be due to invalid IL or missing references)
			//IL_123a: Unknown result type (might be due to invalid IL or missing references)
			//IL_1210: Unknown result type (might be due to invalid IL or missing references)
			//IL_121e: Unknown result type (might be due to invalid IL or missing references)
			//IL_11e9: Unknown result type (might be due to invalid IL or missing references)
			//IL_11f7: Unknown result type (might be due to invalid IL or missing references)
			//IL_0ff2: Unknown result type (might be due to invalid IL or missing references)
			//IL_0ff9: Unknown result type (might be due to invalid IL or missing references)
			//IL_0fa2: Unknown result type (might be due to invalid IL or missing references)
			//IL_0fa9: Unknown result type (might be due to invalid IL or missing references)
			//IL_101b: Unknown result type (might be due to invalid IL or missing references)
			//IL_102c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0424: Unknown result type (might be due to invalid IL or missing references)
			//IL_042e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0438: Unknown result type (might be due to invalid IL or missing references)
			//IL_03f0: Unknown result type (might be due to invalid IL or missing references)
			//IL_0401: Unknown result type (might be due to invalid IL or missing references)
			//IL_040b: Unknown result type (might be due to invalid IL or missing references)
			//IL_1325: Unknown result type (might be due to invalid IL or missing references)
			//IL_132c: Unknown result type (might be due to invalid IL or missing references)
			//IL_134c: Unknown result type (might be due to invalid IL or missing references)
			//IL_1353: Unknown result type (might be due to invalid IL or missing references)
			//IL_135d: Unknown result type (might be due to invalid IL or missing references)
			//IL_153c: Unknown result type (might be due to invalid IL or missing references)
			//IL_1546: Unknown result type (might be due to invalid IL or missing references)
			//IL_154b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0fc6: Unknown result type (might be due to invalid IL or missing references)
			//IL_0fe1: Unknown result type (might be due to invalid IL or missing references)
			//IL_10fe: Unknown result type (might be due to invalid IL or missing references)
			//IL_1105: Unknown result type (might be due to invalid IL or missing references)
			//IL_106f: Unknown result type (might be due to invalid IL or missing references)
			//IL_1076: Unknown result type (might be due to invalid IL or missing references)
			//IL_07cd: Unknown result type (might be due to invalid IL or missing references)
			//IL_07d7: Unknown result type (might be due to invalid IL or missing references)
			//IL_07a3: Unknown result type (might be due to invalid IL or missing references)
			//IL_07b4: Unknown result type (might be due to invalid IL or missing references)
			//IL_0458: Unknown result type (might be due to invalid IL or missing references)
			//IL_045f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0508: Unknown result type (might be due to invalid IL or missing references)
			//IL_0512: Unknown result type (might be due to invalid IL or missing references)
			//IL_051c: Unknown result type (might be due to invalid IL or missing references)
			//IL_04b3: Unknown result type (might be due to invalid IL or missing references)
			//IL_04c4: Unknown result type (might be due to invalid IL or missing references)
			//IL_04ce: Unknown result type (might be due to invalid IL or missing references)
			//IL_130d: Unknown result type (might be due to invalid IL or missing references)
			//IL_1314: Unknown result type (might be due to invalid IL or missing references)
			//IL_13d3: Unknown result type (might be due to invalid IL or missing references)
			//IL_13da: Unknown result type (might be due to invalid IL or missing references)
			//IL_157b: Unknown result type (might be due to invalid IL or missing references)
			//IL_1580: Unknown result type (might be due to invalid IL or missing references)
			//IL_158d: Unknown result type (might be due to invalid IL or missing references)
			//IL_1592: Unknown result type (might be due to invalid IL or missing references)
			//IL_1566: Unknown result type (might be due to invalid IL or missing references)
			//IL_156b: Unknown result type (might be due to invalid IL or missing references)
			//IL_1572: Unknown result type (might be due to invalid IL or missing references)
			//IL_1577: Unknown result type (might be due to invalid IL or missing references)
			//IL_10b4: Unknown result type (might be due to invalid IL or missing references)
			//IL_10cf: Unknown result type (might be due to invalid IL or missing references)
			//IL_09db: Unknown result type (might be due to invalid IL or missing references)
			//IL_09e5: Unknown result type (might be due to invalid IL or missing references)
			//IL_0990: Unknown result type (might be due to invalid IL or missing references)
			//IL_09a1: Unknown result type (might be due to invalid IL or missing references)
			//IL_048d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0494: Unknown result type (might be due to invalid IL or missing references)
			//IL_0478: Unknown result type (might be due to invalid IL or missing references)
			//IL_047f: Unknown result type (might be due to invalid IL or missing references)
			//IL_055d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0564: Unknown result type (might be due to invalid IL or missing references)
			//IL_139d: Unknown result type (might be due to invalid IL or missing references)
			//IL_13a4: Unknown result type (might be due to invalid IL or missing references)
			//IL_1594: Unknown result type (might be due to invalid IL or missing references)
			//IL_159b: Unknown result type (might be due to invalid IL or missing references)
			//IL_081d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0836: Unknown result type (might be due to invalid IL or missing references)
			//IL_084f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0866: Unknown result type (might be due to invalid IL or missing references)
			//IL_086b: Unknown result type (might be due to invalid IL or missing references)
			//IL_086e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0875: Unknown result type (might be due to invalid IL or missing references)
			//IL_0877: Unknown result type (might be due to invalid IL or missing references)
			//IL_061e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0623: Unknown result type (might be due to invalid IL or missing references)
			//IL_0627: Unknown result type (might be due to invalid IL or missing references)
			//IL_062c: Unknown result type (might be due to invalid IL or missing references)
			//IL_062e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0633: Unknown result type (might be due to invalid IL or missing references)
			//IL_063c: Unknown result type (might be due to invalid IL or missing references)
			//IL_065c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0666: Unknown result type (might be due to invalid IL or missing references)
			//IL_066b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0673: Unknown result type (might be due to invalid IL or missing references)
			//IL_0675: Unknown result type (might be due to invalid IL or missing references)
			//IL_067a: Unknown result type (might be due to invalid IL or missing references)
			//IL_067c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0681: Unknown result type (might be due to invalid IL or missing references)
			//IL_0683: Unknown result type (might be due to invalid IL or missing references)
			//IL_0688: Unknown result type (might be due to invalid IL or missing references)
			//IL_0695: Unknown result type (might be due to invalid IL or missing references)
			//IL_0697: Unknown result type (might be due to invalid IL or missing references)
			//IL_069c: Unknown result type (might be due to invalid IL or missing references)
			//IL_069e: Unknown result type (might be due to invalid IL or missing references)
			//IL_06a3: Unknown result type (might be due to invalid IL or missing references)
			//IL_06a5: Unknown result type (might be due to invalid IL or missing references)
			//IL_06aa: Unknown result type (might be due to invalid IL or missing references)
			//IL_06b7: Unknown result type (might be due to invalid IL or missing references)
			//IL_06b9: Unknown result type (might be due to invalid IL or missing references)
			//IL_06bb: Unknown result type (might be due to invalid IL or missing references)
			//IL_06c2: Unknown result type (might be due to invalid IL or missing references)
			//IL_05ce: Unknown result type (might be due to invalid IL or missing references)
			//IL_05d5: Unknown result type (might be due to invalid IL or missing references)
			//IL_059b: Unknown result type (might be due to invalid IL or missing references)
			//IL_05a2: Unknown result type (might be due to invalid IL or missing references)
			//IL_1636: Unknown result type (might be due to invalid IL or missing references)
			//IL_163b: Unknown result type (might be due to invalid IL or missing references)
			//IL_1642: Unknown result type (might be due to invalid IL or missing references)
			//IL_1647: Unknown result type (might be due to invalid IL or missing references)
			//IL_164f: Unknown result type (might be due to invalid IL or missing references)
			//IL_1651: Unknown result type (might be due to invalid IL or missing references)
			//IL_1653: Unknown result type (might be due to invalid IL or missing references)
			//IL_1655: Unknown result type (might be due to invalid IL or missing references)
			//IL_165a: Unknown result type (might be due to invalid IL or missing references)
			//IL_15a9: Unknown result type (might be due to invalid IL or missing references)
			//IL_15b0: Unknown result type (might be due to invalid IL or missing references)
			//IL_15b5: Unknown result type (might be due to invalid IL or missing references)
			//IL_15ba: Unknown result type (might be due to invalid IL or missing references)
			//IL_15bc: Unknown result type (might be due to invalid IL or missing references)
			//IL_15be: Unknown result type (might be due to invalid IL or missing references)
			//IL_15c3: Unknown result type (might be due to invalid IL or missing references)
			//IL_15c5: Unknown result type (might be due to invalid IL or missing references)
			//IL_15c7: Unknown result type (might be due to invalid IL or missing references)
			//IL_15c9: Unknown result type (might be due to invalid IL or missing references)
			//IL_15ce: Unknown result type (might be due to invalid IL or missing references)
			//IL_15d6: Unknown result type (might be due to invalid IL or missing references)
			//IL_15d8: Unknown result type (might be due to invalid IL or missing references)
			//IL_15dd: Unknown result type (might be due to invalid IL or missing references)
			//IL_15df: Unknown result type (might be due to invalid IL or missing references)
			//IL_15e4: Unknown result type (might be due to invalid IL or missing references)
			//IL_15f1: Unknown result type (might be due to invalid IL or missing references)
			//IL_15f3: Unknown result type (might be due to invalid IL or missing references)
			//IL_15f8: Unknown result type (might be due to invalid IL or missing references)
			//IL_15fa: Unknown result type (might be due to invalid IL or missing references)
			//IL_15ff: Unknown result type (might be due to invalid IL or missing references)
			//IL_160c: Unknown result type (might be due to invalid IL or missing references)
			//IL_1615: Unknown result type (might be due to invalid IL or missing references)
			//IL_161c: Unknown result type (might be due to invalid IL or missing references)
			//IL_162a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0899: Unknown result type (might be due to invalid IL or missing references)
			//IL_08a0: Unknown result type (might be due to invalid IL or missing references)
			//IL_0e05: Unknown result type (might be due to invalid IL or missing references)
			//IL_0e0a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0e0e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0e13: Unknown result type (might be due to invalid IL or missing references)
			//IL_0e15: Unknown result type (might be due to invalid IL or missing references)
			//IL_0e1a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0e23: Unknown result type (might be due to invalid IL or missing references)
			//IL_0e49: Unknown result type (might be due to invalid IL or missing references)
			//IL_0e4b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0e50: Unknown result type (might be due to invalid IL or missing references)
			//IL_0e52: Unknown result type (might be due to invalid IL or missing references)
			//IL_0e57: Unknown result type (might be due to invalid IL or missing references)
			//IL_0e59: Unknown result type (might be due to invalid IL or missing references)
			//IL_0e5e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0e6e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0e70: Unknown result type (might be due to invalid IL or missing references)
			//IL_0e75: Unknown result type (might be due to invalid IL or missing references)
			//IL_0e77: Unknown result type (might be due to invalid IL or missing references)
			//IL_0e7c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0e7e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0e83: Unknown result type (might be due to invalid IL or missing references)
			//IL_0e93: Unknown result type (might be due to invalid IL or missing references)
			//IL_0e95: Unknown result type (might be due to invalid IL or missing references)
			//IL_0e97: Unknown result type (might be due to invalid IL or missing references)
			//IL_0e9e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0a4c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0a65: Unknown result type (might be due to invalid IL or missing references)
			//IL_0a7e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0a95: Unknown result type (might be due to invalid IL or missing references)
			//IL_0a9a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0a9d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0aa4: Unknown result type (might be due to invalid IL or missing references)
			//IL_0aa6: Unknown result type (might be due to invalid IL or missing references)
			//IL_08c2: Unknown result type (might be due to invalid IL or missing references)
			//IL_08dd: Unknown result type (might be due to invalid IL or missing references)
			//IL_0ae9: Unknown result type (might be due to invalid IL or missing references)
			//IL_0af0: Unknown result type (might be due to invalid IL or missing references)
			//IL_0935: Unknown result type (might be due to invalid IL or missing references)
			//IL_093c: Unknown result type (might be due to invalid IL or missing references)
			//IL_08fa: Unknown result type (might be due to invalid IL or missing references)
			//IL_0915: Unknown result type (might be due to invalid IL or missing references)
			//IL_096a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0971: Unknown result type (might be due to invalid IL or missing references)
			//IL_0955: Unknown result type (might be due to invalid IL or missing references)
			//IL_095c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0d44: Unknown result type (might be due to invalid IL or missing references)
			//IL_0d4b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0ce8: Unknown result type (might be due to invalid IL or missing references)
			//IL_0d03: Unknown result type (might be due to invalid IL or missing references)
			//IL_0b90: Unknown result type (might be due to invalid IL or missing references)
			//IL_0bb1: Unknown result type (might be due to invalid IL or missing references)
			//IL_0bb6: Unknown result type (might be due to invalid IL or missing references)
			//IL_0bbb: Unknown result type (might be due to invalid IL or missing references)
			//IL_0bbf: Unknown result type (might be due to invalid IL or missing references)
			//IL_0bc9: Unknown result type (might be due to invalid IL or missing references)
			//IL_0bce: Unknown result type (might be due to invalid IL or missing references)
			//IL_0bd2: Unknown result type (might be due to invalid IL or missing references)
			//IL_0bdc: Unknown result type (might be due to invalid IL or missing references)
			//IL_0be1: Unknown result type (might be due to invalid IL or missing references)
			//IL_0be6: Unknown result type (might be due to invalid IL or missing references)
			//IL_0bee: Unknown result type (might be due to invalid IL or missing references)
			//IL_0bf0: Unknown result type (might be due to invalid IL or missing references)
			//IL_0c0b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0b42: Unknown result type (might be due to invalid IL or missing references)
			//IL_0b5d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0db5: Unknown result type (might be due to invalid IL or missing references)
			//IL_0dbc: Unknown result type (might be due to invalid IL or missing references)
			//IL_0d82: Unknown result type (might be due to invalid IL or missing references)
			//IL_0d89: Unknown result type (might be due to invalid IL or missing references)
			//IL_0c2b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0c2d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0c34: Unknown result type (might be due to invalid IL or missing references)
			//IL_0c42: Unknown result type (might be due to invalid IL or missing references)
			//IL_0c44: Unknown result type (might be due to invalid IL or missing references)
			//IL_0c46: Unknown result type (might be due to invalid IL or missing references)
			//IL_0ca8: Unknown result type (might be due to invalid IL or missing references)
			//IL_0caa: Unknown result type (might be due to invalid IL or missing references)
			//IL_0cb1: Unknown result type (might be due to invalid IL or missing references)
			//IL_0cbf: Unknown result type (might be due to invalid IL or missing references)
			//IL_0cc1: Unknown result type (might be due to invalid IL or missing references)
			//IL_0cc3: Unknown result type (might be due to invalid IL or missing references)
			//IL_0c6f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0c71: Unknown result type (might be due to invalid IL or missing references)
			//IL_0c78: Unknown result type (might be due to invalid IL or missing references)
			//IL_0c86: Unknown result type (might be due to invalid IL or missing references)
			//IL_0c88: Unknown result type (might be due to invalid IL or missing references)
			//IL_0c8a: Unknown result type (might be due to invalid IL or missing references)
			NativeArray<Curve> nativeArray = ((ArchetypeChunk)(ref chunk)).GetNativeArray<Curve>(ref m_CurveType);
			BufferAccessor<LaneObject> bufferAccessor = ((ArchetypeChunk)(ref chunk)).GetBufferAccessor<LaneObject>(ref m_LaneObjectType);
			BufferAccessor<LaneOverlap> bufferAccessor2 = ((ArchetypeChunk)(ref chunk)).GetBufferAccessor<LaneOverlap>(ref m_LaneOverlapType);
			if (((ArchetypeChunk)(ref chunk)).Has<Temp>(ref m_TempType))
			{
				if (((ArchetypeChunk)(ref chunk)).Has<CarLane>(ref m_CarLaneType))
				{
					bool flag = ((ArchetypeChunk)(ref chunk)).Has<MasterLane>(ref m_MasterLaneType);
					bool flag2 = ((ArchetypeChunk)(ref chunk)).Has<SlaveLane>(ref m_SlaveLaneType);
					if ((flag && m_MasterOption) || (flag2 && m_SlaveOption) || (!flag && !flag2 && m_StandaloneOption))
					{
						NativeArray<CarLane> nativeArray2 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<CarLane>(ref m_CarLaneType);
						for (int i = 0; i < nativeArray.Length; i++)
						{
							CarLane carLane = nativeArray2[i];
							Curve curve = nativeArray[i];
							if ((carLane.m_Flags & CarLaneFlags.Twoway) != 0 || curve.m_Length <= 0.1f)
							{
								m_GizmoBatcher.DrawCurve(curve, Color.blue);
							}
							else
							{
								m_GizmoBatcher.DrawFlowCurve(curve, Color.blue, curve.m_Length * 0.5f + 0.5f, reverse: false, 1, -1, 1f);
							}
						}
					}
				}
				else if (((ArchetypeChunk)(ref chunk)).Has<TrackLane>(ref m_TrackLaneType))
				{
					if (m_StandaloneOption)
					{
						NativeArray<TrackLane> nativeArray3 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<TrackLane>(ref m_TrackLaneType);
						for (int j = 0; j < nativeArray3.Length; j++)
						{
							TrackLane trackLane = nativeArray3[j];
							Curve curve2 = nativeArray[j];
							if ((trackLane.m_Flags & TrackLaneFlags.Twoway) != 0 || curve2.m_Length <= 0.1f)
							{
								m_GizmoBatcher.DrawCurve(curve2, Color.blue);
							}
							else
							{
								m_GizmoBatcher.DrawFlowCurve(curve2, Color.blue, curve2.m_Length * 0.5f + 0.5f, reverse: false, 1, -1, 1f);
							}
						}
					}
				}
				else if (((ArchetypeChunk)(ref chunk)).Has<ParkingLane>(ref m_ParkingLaneType))
				{
					if (m_StandaloneOption)
					{
						NativeArray<ParkingLane> nativeArray4 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<ParkingLane>(ref m_ParkingLaneType);
						for (int k = 0; k < nativeArray4.Length; k++)
						{
							ParkingLane parkingLane = nativeArray4[k];
							Curve curve3 = nativeArray[k];
							if ((parkingLane.m_Flags & ParkingLaneFlags.SecondaryStart) != 0 || curve3.m_Length <= 0.1f)
							{
								m_GizmoBatcher.DrawCurve(curve3, Color.blue);
							}
							else
							{
								m_GizmoBatcher.DrawFlowCurve(curve3, Color.blue, curve3.m_Length * 0.5f + 0.5f, reverse: false, 1, -1, 1f);
							}
						}
					}
				}
				else if (m_StandaloneOption)
				{
					for (int l = 0; l < nativeArray.Length; l++)
					{
						m_GizmoBatcher.DrawCurve(nativeArray[l], Color.blue);
					}
				}
			}
			else if (((ArchetypeChunk)(ref chunk)).Has<CarLane>(ref m_CarLaneType))
			{
				NativeArray<CarLane> nativeArray5 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<CarLane>(ref m_CarLaneType);
				NativeArray<LaneSignal> nativeArray6 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<LaneSignal>(ref m_LaneSignalType);
				Color red = Color.red;
				Color yellow = Color.yellow;
				Color val = Color.cyan;
				if (((ArchetypeChunk)(ref chunk)).Has<TrackLane>(ref m_TrackLaneType))
				{
					((Color)(ref val))._002Ector(0.5f, 1f, 1f, 1f);
				}
				if (((ArchetypeChunk)(ref chunk)).Has<MasterLane>(ref m_MasterLaneType))
				{
					if (m_MasterOption)
					{
						red *= 0.5f;
						yellow *= 0.5f;
						val *= 0.5f;
						float3 val3 = default(float3);
						for (int m = 0; m < nativeArray5.Length; m++)
						{
							Curve curve4 = nativeArray[m];
							CarLane carLane2 = nativeArray5[m];
							if ((carLane2.m_Flags & CarLaneFlags.Twoway) != 0 || curve4.m_Length <= 0.1f)
							{
								if (m_SignalsOption && nativeArray6.Length != 0)
								{
									m_GizmoBatcher.DrawCurve(curve4, GetSignalColor(nativeArray6[m]) * 0.5f);
								}
								else if (m_PriorityOption)
								{
									m_GizmoBatcher.DrawCurve(curve4, GetPriorityColor(carLane2) * 0.5f);
								}
								else if ((carLane2.m_Flags & CarLaneFlags.Forbidden) != 0)
								{
									m_GizmoBatcher.DrawCurve(curve4, red);
								}
								else if ((carLane2.m_Flags & CarLaneFlags.Unsafe) != 0)
								{
									m_GizmoBatcher.DrawCurve(curve4, yellow);
								}
								else
								{
									m_GizmoBatcher.DrawCurve(curve4, val);
								}
							}
							else if (m_SignalsOption && nativeArray6.Length != 0)
							{
								m_GizmoBatcher.DrawFlowCurve(curve4, GetSignalColor(nativeArray6[m]) * 0.5f, curve4.m_Length * 0.5f + 0.5f, reverse: false, 1, -1, 1f);
							}
							else if (m_PriorityOption)
							{
								m_GizmoBatcher.DrawFlowCurve(curve4, GetPriorityColor(carLane2) * 0.5f, curve4.m_Length * 0.5f + 0.5f, reverse: false, 1, -1, 1f);
							}
							else if ((carLane2.m_Flags & CarLaneFlags.Forbidden) != 0)
							{
								m_GizmoBatcher.DrawFlowCurve(curve4, red, curve4.m_Length * 0.5f + 0.5f, reverse: false, 1, -1, 1f);
							}
							else if ((carLane2.m_Flags & CarLaneFlags.Unsafe) != 0)
							{
								m_GizmoBatcher.DrawFlowCurve(curve4, yellow, curve4.m_Length * 0.5f + 0.5f, reverse: false, 1, -1, 1f);
							}
							else
							{
								m_GizmoBatcher.DrawFlowCurve(curve4, val, curve4.m_Length * 0.5f + 0.5f, reverse: false, 1, -1, 1f);
							}
							if (m_BlockageOption && carLane2.m_BlockageEnd >= carLane2.m_BlockageStart)
							{
								Bounds1 blockageBounds = carLane2.blockageBounds;
								Bezier4x3 val2 = MathUtils.Cut(curve4.m_Bezier, blockageBounds);
								float num = curve4.m_Length * MathUtils.Size(blockageBounds);
								((float3)(ref val3))._002Ector(0f, 1f, 0f);
								Color val4 = Color.red * 0.5f;
								((GizmoBatcher)(ref m_GizmoBatcher)).DrawLine(val2.a, val2.a + val3, val4);
								((GizmoBatcher)(ref m_GizmoBatcher)).DrawLine(val2.d, val2.d + val3, val4);
								((GizmoBatcher)(ref m_GizmoBatcher)).DrawCurve(val2 + val3, num, val4, -1);
							}
						}
					}
				}
				else
				{
					bool flag3 = ((ArchetypeChunk)(ref chunk)).Has<SlaveLane>(ref m_SlaveLaneType);
					if ((flag3 && m_SlaveOption) || (!flag3 && m_StandaloneOption))
					{
						NativeArray<LaneReservation> nativeArray7 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<LaneReservation>(ref m_LaneReservationType);
						NativeArray<LaneCondition> nativeArray8 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<LaneCondition>(ref m_LaneConditionType);
						Bezier4x3 val6 = default(Bezier4x3);
						Bezier4x3 val7 = default(Bezier4x3);
						float3 val12 = default(float3);
						for (int n = 0; n < nativeArray5.Length; n++)
						{
							Curve curve5 = nativeArray[n];
							CarLane carLane3 = nativeArray5[n];
							DynamicBuffer<LaneObject> val5 = bufferAccessor[n];
							LaneReservation laneReservation = nativeArray7[n];
							float offset = laneReservation.GetOffset();
							int priority = laneReservation.GetPriority();
							if ((carLane3.m_Flags & CarLaneFlags.Twoway) != 0 || curve5.m_Length <= 0.1f)
							{
								if (m_SignalsOption && nativeArray6.Length != 0)
								{
									m_GizmoBatcher.DrawCurve(curve5, GetSignalColor(nativeArray6[n]));
								}
								else if (m_PriorityOption)
								{
									m_GizmoBatcher.DrawCurve(curve5, GetPriorityColor(carLane3));
								}
								else if (m_ConditionOption && nativeArray8.Length != 0)
								{
									LaneCondition laneCondition = nativeArray8[n];
									float4 vector = RenderingUtils.Lerp(new float4(0f, 1f, 0f, 1f), new float4(1f, 1f, 0f, 1f), new float4(1f, 0f, 0f, 1f), math.saturate(laneCondition.m_Wear / 10f));
									m_GizmoBatcher.DrawCurve(curve5, RenderingUtils.ToColor(vector));
								}
								else if (val5.Length != 0 && m_ReservedOption)
								{
									m_GizmoBatcher.DrawCurve(curve5, Color.magenta);
								}
								else if (offset > 0f && m_ReservedOption)
								{
									m_GizmoBatcher.DrawCurve(curve5, new Color(0.5f, 0f, 1f, 1f));
								}
								else if (priority != 0 && m_ReservedOption)
								{
									m_GizmoBatcher.DrawCurve(curve5, new Color(0f, 0.5f, 1f, 1f));
								}
								else if ((carLane3.m_Flags & CarLaneFlags.Forbidden) != 0)
								{
									m_GizmoBatcher.DrawCurve(curve5, red);
								}
								else if ((carLane3.m_Flags & CarLaneFlags.Unsafe) != 0)
								{
									m_GizmoBatcher.DrawCurve(curve5, yellow);
								}
								else
								{
									m_GizmoBatcher.DrawCurve(curve5, val);
								}
							}
							else if (m_SignalsOption && nativeArray6.Length != 0)
							{
								m_GizmoBatcher.DrawFlowCurve(curve5, GetSignalColor(nativeArray6[n]), curve5.m_Length * 0.5f + 0.5f, reverse: false, 1, -1, 1f);
							}
							else if (m_PriorityOption)
							{
								m_GizmoBatcher.DrawFlowCurve(curve5, GetPriorityColor(carLane3), curve5.m_Length * 0.5f + 0.5f, reverse: false, 1, -1, 1f);
							}
							else if (m_ConditionOption && nativeArray8.Length != 0)
							{
								LaneCondition laneCondition2 = nativeArray8[n];
								float4 vector2 = RenderingUtils.Lerp(new float4(0f, 1f, 0f, 1f), new float4(1f, 1f, 0f, 1f), new float4(1f, 0f, 0f, 1f), math.saturate(laneCondition2.m_Wear / 10f));
								m_GizmoBatcher.DrawFlowCurve(curve5, RenderingUtils.ToColor(vector2), curve5.m_Length * 0.5f + 0.5f, reverse: false, 1, -1, 1f);
							}
							else if (val5.Length != 0 && m_ReservedOption)
							{
								m_GizmoBatcher.DrawFlowCurve(curve5, Color.magenta, curve5.m_Length * 0.5f + 0.5f, reverse: false, 1, -1, 1f);
							}
							else if (offset > 0f && m_ReservedOption)
							{
								if (offset == 1f)
								{
									m_GizmoBatcher.DrawFlowCurve(curve5, new Color(0.5f, 0f, 1f, 1f), curve5.m_Length * 0.5f + 0.5f, reverse: false, 1, -1, 1f);
								}
								else
								{
									MathUtils.Divide(curve5.m_Bezier, ref val6, ref val7, offset);
									float2 val8 = curve5.m_Length * new float2(offset, 1f - offset);
									float3 val9 = MathUtils.Position(curve5.m_Bezier, 0.5f);
									float3 val10 = math.normalize(MathUtils.Tangent(curve5.m_Bezier, 0.5f));
									((GizmoBatcher)(ref m_GizmoBatcher)).DrawCurve(val6, val8.x, new Color(0.5f, 0f, 1f, 1f), -1);
									if ((carLane3.m_Flags & CarLaneFlags.Forbidden) != 0)
									{
										((GizmoBatcher)(ref m_GizmoBatcher)).DrawCurve(val7, val8.y, red, -1);
										((GizmoBatcher)(ref m_GizmoBatcher)).DrawArrowHead(val9, val10, red, 1f, 25f, 16);
									}
									else if ((carLane3.m_Flags & CarLaneFlags.Unsafe) != 0)
									{
										((GizmoBatcher)(ref m_GizmoBatcher)).DrawCurve(val7, val8.y, yellow, -1);
										((GizmoBatcher)(ref m_GizmoBatcher)).DrawArrowHead(val9, val10, yellow, 1f, 25f, 16);
									}
									else
									{
										((GizmoBatcher)(ref m_GizmoBatcher)).DrawCurve(val7, val8.y, val, -1);
										((GizmoBatcher)(ref m_GizmoBatcher)).DrawArrowHead(val9, val10, val, 1f, 25f, 16);
									}
								}
							}
							else if (priority != 0 && m_ReservedOption)
							{
								m_GizmoBatcher.DrawFlowCurve(curve5, new Color(0f, 0.5f, 1f, 1f), curve5.m_Length * 0.5f + 0.5f, reverse: false, 1, -1, 1f);
							}
							else if ((carLane3.m_Flags & CarLaneFlags.Forbidden) != 0)
							{
								m_GizmoBatcher.DrawFlowCurve(curve5, red, curve5.m_Length * 0.5f + 0.5f, reverse: false, 1, -1, 1f);
							}
							else if ((carLane3.m_Flags & CarLaneFlags.Unsafe) != 0)
							{
								m_GizmoBatcher.DrawFlowCurve(curve5, yellow, curve5.m_Length * 0.5f + 0.5f, reverse: false, 1, -1, 1f);
							}
							else
							{
								m_GizmoBatcher.DrawFlowCurve(curve5, val, curve5.m_Length * 0.5f + 0.5f, reverse: false, 1, -1, 1f);
							}
							if (m_BlockageOption && carLane3.m_BlockageEnd >= carLane3.m_BlockageStart)
							{
								Bounds1 blockageBounds2 = carLane3.blockageBounds;
								Bezier4x3 val11 = MathUtils.Cut(curve5.m_Bezier, blockageBounds2);
								float num2 = curve5.m_Length * MathUtils.Size(blockageBounds2);
								((float3)(ref val12))._002Ector(0f, 1f, 0f);
								((GizmoBatcher)(ref m_GizmoBatcher)).DrawLine(val11.a, val11.a + val12, Color.red);
								((GizmoBatcher)(ref m_GizmoBatcher)).DrawLine(val11.d, val11.d + val12, Color.red);
								((GizmoBatcher)(ref m_GizmoBatcher)).DrawCurve(val11 + val12, num2, Color.red, -1);
							}
						}
					}
				}
			}
			else if (((ArchetypeChunk)(ref chunk)).Has<TrackLane>(ref m_TrackLaneType))
			{
				if (m_StandaloneOption)
				{
					NativeArray<TrackLane> nativeArray9 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<TrackLane>(ref m_TrackLaneType);
					NativeArray<LaneReservation> nativeArray10 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<LaneReservation>(ref m_LaneReservationType);
					NativeArray<LaneSignal> nativeArray11 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<LaneSignal>(ref m_LaneSignalType);
					for (int num3 = 0; num3 < nativeArray9.Length; num3++)
					{
						TrackLane trackLane2 = nativeArray9[num3];
						Curve curve6 = nativeArray[num3];
						DynamicBuffer<LaneObject> val13 = bufferAccessor[num3];
						int priority2 = nativeArray10[num3].GetPriority();
						if ((trackLane2.m_Flags & TrackLaneFlags.Twoway) != 0 || curve6.m_Length <= 0.1f)
						{
							if (m_SignalsOption && nativeArray11.Length != 0)
							{
								m_GizmoBatcher.DrawCurve(curve6, GetSignalColor(nativeArray11[num3]));
							}
							else if (val13.Length != 0 && m_ReservedOption)
							{
								m_GizmoBatcher.DrawCurve(curve6, Color.magenta);
							}
							else if (priority2 != 0 && m_ReservedOption)
							{
								m_GizmoBatcher.DrawCurve(curve6, new Color(0.5f, 0f, 1f, 1f));
							}
							else
							{
								m_GizmoBatcher.DrawCurve(curve6, Color.white);
							}
						}
						else if (m_SignalsOption && nativeArray11.Length != 0)
						{
							m_GizmoBatcher.DrawFlowCurve(curve6, GetSignalColor(nativeArray11[num3]), curve6.m_Length * 0.5f + 0.5f, reverse: false, 1, -1, 1f);
						}
						else if (val13.Length != 0 && m_ReservedOption)
						{
							m_GizmoBatcher.DrawFlowCurve(curve6, Color.magenta, curve6.m_Length * 0.5f + 0.5f, reverse: false, 1, -1, 1f);
						}
						else if (priority2 != 0 && m_ReservedOption)
						{
							m_GizmoBatcher.DrawFlowCurve(curve6, new Color(0.5f, 0f, 1f, 1f), curve6.m_Length * 0.5f + 0.5f, reverse: false, 1, -1, 1f);
						}
						else
						{
							m_GizmoBatcher.DrawFlowCurve(curve6, Color.white, curve6.m_Length * 0.5f + 0.5f, reverse: false, 1, -1, 1f);
						}
					}
				}
			}
			else if (((ArchetypeChunk)(ref chunk)).Has<PedestrianLane>(ref m_PedestrianLaneType))
			{
				if (m_StandaloneOption)
				{
					NativeArray<PedestrianLane> nativeArray12 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<PedestrianLane>(ref m_PedestrianLaneType);
					NativeArray<LaneSignal> nativeArray13 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<LaneSignal>(ref m_LaneSignalType);
					for (int num4 = 0; num4 < nativeArray12.Length; num4++)
					{
						PedestrianLane pedestrianLane = nativeArray12[num4];
						DynamicBuffer<LaneObject> val14 = bufferAccessor[num4];
						if (m_SignalsOption && nativeArray13.Length != 0)
						{
							m_GizmoBatcher.DrawCurve(nativeArray[num4], GetSignalColor(nativeArray13[num4]));
						}
						else if (val14.Length != 0 && m_ReservedOption)
						{
							m_GizmoBatcher.DrawCurve(nativeArray[num4], Color.magenta);
						}
						else if ((pedestrianLane.m_Flags & PedestrianLaneFlags.Unsafe) != 0)
						{
							m_GizmoBatcher.DrawCurve(nativeArray[num4], Color.yellow);
						}
						else
						{
							m_GizmoBatcher.DrawCurve(nativeArray[num4], Color.green);
						}
					}
				}
			}
			else if (((ArchetypeChunk)(ref chunk)).Has<ParkingLane>(ref m_ParkingLaneType))
			{
				if (m_StandaloneOption)
				{
					NativeArray<ParkingLane> nativeArray14 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<ParkingLane>(ref m_ParkingLaneType);
					for (int num5 = 0; num5 < nativeArray14.Length; num5++)
					{
						ParkingLane parkingLane2 = nativeArray14[num5];
						Curve curve7 = nativeArray[num5];
						DynamicBuffer<LaneObject> val15 = bufferAccessor[num5];
						if ((parkingLane2.m_Flags & ParkingLaneFlags.SecondaryStart) != 0 || curve7.m_Length <= 0.1f)
						{
							if ((parkingLane2.m_Flags & ParkingLaneFlags.VirtualLane) != 0)
							{
								m_GizmoBatcher.DrawCurve(curve7, Color.black * 0.5f);
							}
							else if (val15.Length != 0 && m_ReservedOption)
							{
								m_GizmoBatcher.DrawCurve(curve7, Color.magenta);
							}
							else
							{
								m_GizmoBatcher.DrawCurve(curve7, Color.black);
							}
						}
						else if ((parkingLane2.m_Flags & ParkingLaneFlags.VirtualLane) != 0)
						{
							m_GizmoBatcher.DrawFlowCurve(curve7, Color.black * 0.5f, curve7.m_Length * 0.5f + 0.5f, reverse: false, 1, -1, 1f);
						}
						else if (val15.Length != 0 && m_ReservedOption)
						{
							m_GizmoBatcher.DrawFlowCurve(curve7, Color.magenta, curve7.m_Length * 0.5f + 0.5f, reverse: false, 1, -1, 1f);
						}
						else
						{
							m_GizmoBatcher.DrawFlowCurve(curve7, Color.black, curve7.m_Length * 0.5f + 0.5f, reverse: false, 1, -1, 1f);
						}
					}
				}
			}
			else if (((ArchetypeChunk)(ref chunk)).Has<ConnectionLane>(ref m_ConnectionLaneType))
			{
				if (m_ConnectionOption)
				{
					for (int num6 = 0; num6 < nativeArray.Length; num6++)
					{
						m_GizmoBatcher.DrawCurve(nativeArray[num6], new Color(1f, 0f, 0.5f, 0.5f));
					}
				}
			}
			else if (m_StandaloneOption)
			{
				for (int num7 = 0; num7 < nativeArray.Length; num7++)
				{
					m_GizmoBatcher.DrawCurve(nativeArray[num7], Color.gray);
				}
			}
			if (!m_OverlapOption || !((ArchetypeChunk)(ref chunk)).Has<LaneOverlap>(ref m_LaneOverlapType))
			{
				return;
			}
			float3 val17 = default(float3);
			for (int num8 = 0; num8 < bufferAccessor2.Length; num8++)
			{
				DynamicBuffer<LaneOverlap> val16 = bufferAccessor2[num8];
				if (val16.Length == 0)
				{
					continue;
				}
				Curve curve8 = nativeArray[num8];
				for (int num9 = 0; num9 < val16.Length; num9++)
				{
					LaneOverlap laneOverlap = val16[num9];
					((float3)(ref val17))._002Ector(0f, 0.2f + (float)num9 * 0.2f, 0f);
					float2 val18 = new float2((float)(int)laneOverlap.m_ThisStart, (float)(int)laneOverlap.m_ThisEnd) * 0.003921569f;
					float num10 = (float)(int)laneOverlap.m_Parallelism * (1f / 128f);
					Color val19 = ((!(num10 < 1f)) ? Color.Lerp(Color.yellow, Color.green, num10 - 1f) : Color.Lerp(Color.red, Color.yellow, num10));
					if (val18.y != val18.x)
					{
						Bezier4x3 val20 = MathUtils.Cut(curve8.m_Bezier, ((float2)(ref val18)).xy);
						Segment val21 = MathUtils.Line(val20);
						val20 += val17;
						((GizmoBatcher)(ref m_GizmoBatcher)).DrawLine(val21.a, val20.a, val19);
						((GizmoBatcher)(ref m_GizmoBatcher)).DrawLine(val21.b, val20.d, val19);
						((GizmoBatcher)(ref m_GizmoBatcher)).DrawCurve(val20, curve8.m_Length * math.abs(val18.y - val18.x), val19, -1);
					}
					else
					{
						float3 val22 = MathUtils.Position(curve8.m_Bezier, val18.x);
						((GizmoBatcher)(ref m_GizmoBatcher)).DrawLine(val22, val22 + val17, val19);
					}
				}
			}
		}

		private Color GetSignalColor(LaneSignal laneSignal)
		{
			//IL_0021: Unknown result type (might be due to invalid IL or missing references)
			//IL_0027: Unknown result type (might be due to invalid IL or missing references)
			//IL_002d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0033: Unknown result type (might be due to invalid IL or missing references)
			//IL_0039: Unknown result type (might be due to invalid IL or missing references)
			return (Color)(laneSignal.m_Signal switch
			{
				LaneSignalType.Stop => Color.red, 
				LaneSignalType.SafeStop => Color.yellow, 
				LaneSignalType.Yield => Color.magenta, 
				LaneSignalType.Go => Color.green, 
				_ => Color.black, 
			});
		}

		private Color GetPriorityColor(CarLane carLane)
		{
			//IL_000e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0022: Unknown result type (might be due to invalid IL or missing references)
			//IL_0036: Unknown result type (might be due to invalid IL or missing references)
			//IL_004c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0046: Unknown result type (might be due to invalid IL or missing references)
			if ((carLane.m_Flags & CarLaneFlags.Stop) != 0)
			{
				return Color.red;
			}
			if ((carLane.m_Flags & CarLaneFlags.Yield) != 0)
			{
				return Color.magenta;
			}
			if ((carLane.m_Flags & CarLaneFlags.RightOfWay) != 0)
			{
				return Color.green;
			}
			if ((carLane.m_Flags & CarLaneFlags.Unsafe) != 0)
			{
				return Color.yellow;
			}
			return Color.cyan;
		}

		void IJobChunk.Execute(in ArchetypeChunk chunk, int unfilteredChunkIndex, bool useEnabledMask, in v128 chunkEnabledMask)
		{
			Execute(in chunk, unfilteredChunkIndex, useEnabledMask, in chunkEnabledMask);
		}
	}

	private struct TypeHandle
	{
		[ReadOnly]
		public ComponentTypeHandle<Curve> __Game_Net_Curve_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<CarLane> __Game_Net_CarLane_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<TrackLane> __Game_Net_TrackLane_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<ParkingLane> __Game_Net_ParkingLane_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<PedestrianLane> __Game_Net_PedestrianLane_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<ConnectionLane> __Game_Net_ConnectionLane_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<MasterLane> __Game_Net_MasterLane_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<SlaveLane> __Game_Net_SlaveLane_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<LaneReservation> __Game_Net_LaneReservation_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<LaneCondition> __Game_Net_LaneCondition_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<LaneSignal> __Game_Net_LaneSignal_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<Temp> __Game_Tools_Temp_RO_ComponentTypeHandle;

		[ReadOnly]
		public BufferTypeHandle<LaneObject> __Game_Net_LaneObject_RO_BufferTypeHandle;

		[ReadOnly]
		public BufferTypeHandle<LaneOverlap> __Game_Net_LaneOverlap_RO_BufferTypeHandle;

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
			__Game_Net_Curve_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<Curve>(true);
			__Game_Net_CarLane_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<CarLane>(true);
			__Game_Net_TrackLane_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<TrackLane>(true);
			__Game_Net_ParkingLane_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<ParkingLane>(true);
			__Game_Net_PedestrianLane_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<PedestrianLane>(true);
			__Game_Net_ConnectionLane_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<ConnectionLane>(true);
			__Game_Net_MasterLane_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<MasterLane>(true);
			__Game_Net_SlaveLane_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<SlaveLane>(true);
			__Game_Net_LaneReservation_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<LaneReservation>(true);
			__Game_Net_LaneCondition_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<LaneCondition>(true);
			__Game_Net_LaneSignal_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<LaneSignal>(true);
			__Game_Tools_Temp_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<Temp>(true);
			__Game_Net_LaneObject_RO_BufferTypeHandle = ((SystemState)(ref state)).GetBufferTypeHandle<LaneObject>(true);
			__Game_Net_LaneOverlap_RO_BufferTypeHandle = ((SystemState)(ref state)).GetBufferTypeHandle<LaneOverlap>(true);
		}
	}

	private EntityQuery m_LaneQuery;

	private GizmosSystem m_GizmosSystem;

	private Option m_StandaloneOption;

	private Option m_SlaveOption;

	private Option m_MasterOption;

	private Option m_ConnectionOption;

	private Option m_OverlapOption;

	private Option m_ReservedOption;

	private Option m_BlockageOption;

	private Option m_ConditionOption;

	private Option m_SignalsOption;

	private Option m_PriorityOption;

	private TypeHandle __TypeHandle;

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
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		base.OnCreate();
		m_GizmosSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<GizmosSystem>();
		m_LaneQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[4]
		{
			ComponentType.ReadOnly<Lane>(),
			ComponentType.ReadOnly<Curve>(),
			ComponentType.Exclude<Deleted>(),
			ComponentType.Exclude<Hidden>()
		});
		((ComponentSystemBase)this).RequireForUpdate(m_LaneQuery);
		m_StandaloneOption = AddOption("Standalone Lanes", defaultEnabled: true);
		m_SlaveOption = AddOption("Slave Lanes", defaultEnabled: true);
		m_MasterOption = AddOption("Master Lanes", defaultEnabled: false);
		m_ConnectionOption = AddOption("Connection Lanes", defaultEnabled: false);
		m_OverlapOption = AddOption("Draw Overlaps", defaultEnabled: false);
		m_ReservedOption = AddOption("Draw Reserved", defaultEnabled: true);
		m_BlockageOption = AddOption("Draw Blocked", defaultEnabled: true);
		m_ConditionOption = AddOption("Draw Condition", defaultEnabled: false);
		m_SignalsOption = AddOption("Draw Signals", defaultEnabled: false);
		m_PriorityOption = AddOption("Draw Priorities", defaultEnabled: false);
		((ComponentSystemBase)this).Enabled = false;
	}

	[Preserve]
	protected override void OnUpdate()
	{
		//IL_00cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f1: Unknown result type (might be due to invalid IL or missing references)
		//IL_0109: Unknown result type (might be due to invalid IL or missing references)
		//IL_010e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0126: Unknown result type (might be due to invalid IL or missing references)
		//IL_012b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0143: Unknown result type (might be due to invalid IL or missing references)
		//IL_0148: Unknown result type (might be due to invalid IL or missing references)
		//IL_0160: Unknown result type (might be due to invalid IL or missing references)
		//IL_0165: Unknown result type (might be due to invalid IL or missing references)
		//IL_017d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0182: Unknown result type (might be due to invalid IL or missing references)
		//IL_019a: Unknown result type (might be due to invalid IL or missing references)
		//IL_019f: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f6: Unknown result type (might be due to invalid IL or missing references)
		//IL_020e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0213: Unknown result type (might be due to invalid IL or missing references)
		//IL_022b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0230: Unknown result type (might be due to invalid IL or missing references)
		//IL_0248: Unknown result type (might be due to invalid IL or missing references)
		//IL_024d: Unknown result type (might be due to invalid IL or missing references)
		//IL_025c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0261: Unknown result type (might be due to invalid IL or missing references)
		//IL_0268: Unknown result type (might be due to invalid IL or missing references)
		//IL_026e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0273: Unknown result type (might be due to invalid IL or missing references)
		//IL_0274: Unknown result type (might be due to invalid IL or missing references)
		//IL_0279: Unknown result type (might be due to invalid IL or missing references)
		//IL_027e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0285: Unknown result type (might be due to invalid IL or missing references)
		//IL_028c: Unknown result type (might be due to invalid IL or missing references)
		JobHandle val2 = default(JobHandle);
		JobHandle val = JobChunkExtensions.ScheduleParallel<LaneGizmoJob>(new LaneGizmoJob
		{
			m_StandaloneOption = m_StandaloneOption.enabled,
			m_SlaveOption = m_SlaveOption.enabled,
			m_MasterOption = m_MasterOption.enabled,
			m_ConnectionOption = m_ConnectionOption.enabled,
			m_OverlapOption = m_OverlapOption.enabled,
			m_ReservedOption = m_ReservedOption.enabled,
			m_BlockageOption = m_BlockageOption.enabled,
			m_ConditionOption = m_ConditionOption.enabled,
			m_SignalsOption = m_SignalsOption.enabled,
			m_PriorityOption = m_PriorityOption.enabled,
			m_CurveType = InternalCompilerInterface.GetComponentTypeHandle<Curve>(ref __TypeHandle.__Game_Net_Curve_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_CarLaneType = InternalCompilerInterface.GetComponentTypeHandle<CarLane>(ref __TypeHandle.__Game_Net_CarLane_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_TrackLaneType = InternalCompilerInterface.GetComponentTypeHandle<TrackLane>(ref __TypeHandle.__Game_Net_TrackLane_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_ParkingLaneType = InternalCompilerInterface.GetComponentTypeHandle<ParkingLane>(ref __TypeHandle.__Game_Net_ParkingLane_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_PedestrianLaneType = InternalCompilerInterface.GetComponentTypeHandle<PedestrianLane>(ref __TypeHandle.__Game_Net_PedestrianLane_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_ConnectionLaneType = InternalCompilerInterface.GetComponentTypeHandle<ConnectionLane>(ref __TypeHandle.__Game_Net_ConnectionLane_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_MasterLaneType = InternalCompilerInterface.GetComponentTypeHandle<MasterLane>(ref __TypeHandle.__Game_Net_MasterLane_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_SlaveLaneType = InternalCompilerInterface.GetComponentTypeHandle<SlaveLane>(ref __TypeHandle.__Game_Net_SlaveLane_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_LaneReservationType = InternalCompilerInterface.GetComponentTypeHandle<LaneReservation>(ref __TypeHandle.__Game_Net_LaneReservation_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_LaneConditionType = InternalCompilerInterface.GetComponentTypeHandle<LaneCondition>(ref __TypeHandle.__Game_Net_LaneCondition_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_LaneSignalType = InternalCompilerInterface.GetComponentTypeHandle<LaneSignal>(ref __TypeHandle.__Game_Net_LaneSignal_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_TempType = InternalCompilerInterface.GetComponentTypeHandle<Temp>(ref __TypeHandle.__Game_Tools_Temp_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_LaneObjectType = InternalCompilerInterface.GetBufferTypeHandle<LaneObject>(ref __TypeHandle.__Game_Net_LaneObject_RO_BufferTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_LaneOverlapType = InternalCompilerInterface.GetBufferTypeHandle<LaneOverlap>(ref __TypeHandle.__Game_Net_LaneOverlap_RO_BufferTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_GizmoBatcher = m_GizmosSystem.GetGizmosBatcher(ref val2)
		}, m_LaneQuery, JobHandle.CombineDependencies(((SystemBase)this).Dependency, val2));
		m_GizmosSystem.AddGizmosBatcherWriter(val);
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
	public LaneDebugSystem()
	{
	}
}

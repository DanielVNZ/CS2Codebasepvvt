using System.Runtime.CompilerServices;
using Colossal;
using Colossal.Mathematics;
using Game.Areas;
using Game.Common;
using Game.Creatures;
using Game.Net;
using Game.Objects;
using Game.Prefabs;
using Game.Routes;
using Game.Simulation;
using Game.Tools;
using Game.Vehicles;
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
public class NavigationDebugSystem : BaseDebugSystem
{
	[BurstCompile]
	private struct NavigationGizmoJob : IJobChunk
	{
		[ReadOnly]
		public bool m_HumanOption;

		[ReadOnly]
		public bool m_AnimalOption;

		[ReadOnly]
		public bool m_CarOption;

		[ReadOnly]
		public bool m_TrainOption;

		[ReadOnly]
		public bool m_WatercraftOption;

		[ReadOnly]
		public bool m_AircraftOption;

		[ReadOnly]
		public float m_TimeOffset;

		[ReadOnly]
		public Entity m_Selected;

		[ReadOnly]
		public EntityTypeHandle m_EntityType;

		[ReadOnly]
		public ComponentTypeHandle<CarCurrentLane> m_CarCurrentLaneType;

		[ReadOnly]
		public BufferTypeHandle<CarNavigationLane> m_CarNavigationLaneType;

		[ReadOnly]
		public ComponentTypeHandle<WatercraftCurrentLane> m_WatercraftCurrentLaneType;

		[ReadOnly]
		public BufferTypeHandle<WatercraftNavigationLane> m_WatercraftNavigationLaneType;

		[ReadOnly]
		public ComponentTypeHandle<AircraftCurrentLane> m_AircraftCurrentLaneType;

		[ReadOnly]
		public BufferTypeHandle<AircraftNavigationLane> m_AircraftNavigationLaneType;

		[ReadOnly]
		public ComponentTypeHandle<Train> m_TrainType;

		[ReadOnly]
		public ComponentTypeHandle<TrainCurrentLane> m_TrainCurrentLaneType;

		[ReadOnly]
		public BufferTypeHandle<TrainNavigationLane> m_TrainNavigationLaneType;

		[ReadOnly]
		public ComponentTypeHandle<HumanCurrentLane> m_HumanCurrentLaneType;

		[ReadOnly]
		public ComponentTypeHandle<AnimalCurrentLane> m_AnimalCurrentLaneType;

		[ReadOnly]
		public ComponentTypeHandle<Transform> m_TransformType;

		[ReadOnly]
		public ComponentTypeHandle<PrefabRef> m_PrefabRefType;

		[ReadOnly]
		public ComponentLookup<Transform> m_TransformData;

		[ReadOnly]
		public ComponentLookup<Curve> m_CurveData;

		[ReadOnly]
		public ComponentLookup<ObjectGeometryData> m_PrefabObjectGeometryData;

		[ReadOnly]
		public ComponentLookup<TrainData> m_PrefabTrainData;

		public GizmoBatcher m_GizmoBatcher;

		public void Execute(in ArchetypeChunk chunk, int unfilteredChunkIndex, bool useEnabledMask, in v128 chunkEnabledMask)
		{
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			//IL_000e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0013: Unknown result type (might be due to invalid IL or missing references)
			//IL_0025: Unknown result type (might be due to invalid IL or missing references)
			//IL_002a: Unknown result type (might be due to invalid IL or missing references)
			//IL_002f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0086: Unknown result type (might be due to invalid IL or missing references)
			//IL_008b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0039: Unknown result type (might be due to invalid IL or missing references)
			//IL_003f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0362: Unknown result type (might be due to invalid IL or missing references)
			//IL_0367: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a5: Unknown result type (might be due to invalid IL or missing references)
			//IL_063e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0643: Unknown result type (might be due to invalid IL or missing references)
			//IL_037c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0381: Unknown result type (might be due to invalid IL or missing references)
			//IL_08fb: Unknown result type (might be due to invalid IL or missing references)
			//IL_0900: Unknown result type (might be due to invalid IL or missing references)
			//IL_0658: Unknown result type (might be due to invalid IL or missing references)
			//IL_065d: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d7: Unknown result type (might be due to invalid IL or missing references)
			//IL_00dc: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e6: Unknown result type (might be due to invalid IL or missing references)
			//IL_0bc4: Unknown result type (might be due to invalid IL or missing references)
			//IL_0bc9: Unknown result type (might be due to invalid IL or missing references)
			//IL_0915: Unknown result type (might be due to invalid IL or missing references)
			//IL_091a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0923: Unknown result type (might be due to invalid IL or missing references)
			//IL_0928: Unknown result type (might be due to invalid IL or missing references)
			//IL_0931: Unknown result type (might be due to invalid IL or missing references)
			//IL_0936: Unknown result type (might be due to invalid IL or missing references)
			//IL_03b3: Unknown result type (might be due to invalid IL or missing references)
			//IL_03b8: Unknown result type (might be due to invalid IL or missing references)
			//IL_03c2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00fd: Unknown result type (might be due to invalid IL or missing references)
			//IL_010b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0117: Unknown result type (might be due to invalid IL or missing references)
			//IL_011c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0121: Unknown result type (might be due to invalid IL or missing references)
			//IL_0125: Unknown result type (might be due to invalid IL or missing references)
			//IL_0131: Unknown result type (might be due to invalid IL or missing references)
			//IL_0136: Unknown result type (might be due to invalid IL or missing references)
			//IL_013b: Unknown result type (might be due to invalid IL or missing references)
			//IL_013d: Unknown result type (might be due to invalid IL or missing references)
			//IL_013f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0144: Unknown result type (might be due to invalid IL or missing references)
			//IL_0147: Unknown result type (might be due to invalid IL or missing references)
			//IL_0166: Unknown result type (might be due to invalid IL or missing references)
			//IL_0172: Unknown result type (might be due to invalid IL or missing references)
			//IL_017d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0188: Unknown result type (might be due to invalid IL or missing references)
			//IL_0194: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a6: Unknown result type (might be due to invalid IL or missing references)
			//IL_0c9b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0ca0: Unknown result type (might be due to invalid IL or missing references)
			//IL_068f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0694: Unknown result type (might be due to invalid IL or missing references)
			//IL_069e: Unknown result type (might be due to invalid IL or missing references)
			//IL_03d9: Unknown result type (might be due to invalid IL or missing references)
			//IL_03e7: Unknown result type (might be due to invalid IL or missing references)
			//IL_03f3: Unknown result type (might be due to invalid IL or missing references)
			//IL_03f8: Unknown result type (might be due to invalid IL or missing references)
			//IL_03fd: Unknown result type (might be due to invalid IL or missing references)
			//IL_0401: Unknown result type (might be due to invalid IL or missing references)
			//IL_040d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0412: Unknown result type (might be due to invalid IL or missing references)
			//IL_0417: Unknown result type (might be due to invalid IL or missing references)
			//IL_0419: Unknown result type (might be due to invalid IL or missing references)
			//IL_041b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0420: Unknown result type (might be due to invalid IL or missing references)
			//IL_0423: Unknown result type (might be due to invalid IL or missing references)
			//IL_0442: Unknown result type (might be due to invalid IL or missing references)
			//IL_044e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0459: Unknown result type (might be due to invalid IL or missing references)
			//IL_0464: Unknown result type (might be due to invalid IL or missing references)
			//IL_0470: Unknown result type (might be due to invalid IL or missing references)
			//IL_0482: Unknown result type (might be due to invalid IL or missing references)
			//IL_026c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0271: Unknown result type (might be due to invalid IL or missing references)
			//IL_0273: Unknown result type (might be due to invalid IL or missing references)
			//IL_0278: Unknown result type (might be due to invalid IL or missing references)
			//IL_01bd: Unknown result type (might be due to invalid IL or missing references)
			//IL_01cb: Unknown result type (might be due to invalid IL or missing references)
			//IL_01d7: Unknown result type (might be due to invalid IL or missing references)
			//IL_01dc: Unknown result type (might be due to invalid IL or missing references)
			//IL_01e1: Unknown result type (might be due to invalid IL or missing references)
			//IL_01e3: Unknown result type (might be due to invalid IL or missing references)
			//IL_01e5: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ea: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ed: Unknown result type (might be due to invalid IL or missing references)
			//IL_01f8: Unknown result type (might be due to invalid IL or missing references)
			//IL_0204: Unknown result type (might be due to invalid IL or missing references)
			//IL_0214: Unknown result type (might be due to invalid IL or missing references)
			//IL_0216: Unknown result type (might be due to invalid IL or missing references)
			//IL_021b: Unknown result type (might be due to invalid IL or missing references)
			//IL_021d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0222: Unknown result type (might be due to invalid IL or missing references)
			//IL_0234: Unknown result type (might be due to invalid IL or missing references)
			//IL_0239: Unknown result type (might be due to invalid IL or missing references)
			//IL_023b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0240: Unknown result type (might be due to invalid IL or missing references)
			//IL_0242: Unknown result type (might be due to invalid IL or missing references)
			//IL_0253: Unknown result type (might be due to invalid IL or missing references)
			//IL_0258: Unknown result type (might be due to invalid IL or missing references)
			//IL_0982: Unknown result type (might be due to invalid IL or missing references)
			//IL_099b: Unknown result type (might be due to invalid IL or missing references)
			//IL_077e: Unknown result type (might be due to invalid IL or missing references)
			//IL_06b5: Unknown result type (might be due to invalid IL or missing references)
			//IL_06c3: Unknown result type (might be due to invalid IL or missing references)
			//IL_06cf: Unknown result type (might be due to invalid IL or missing references)
			//IL_06d4: Unknown result type (might be due to invalid IL or missing references)
			//IL_06d9: Unknown result type (might be due to invalid IL or missing references)
			//IL_06dd: Unknown result type (might be due to invalid IL or missing references)
			//IL_06e9: Unknown result type (might be due to invalid IL or missing references)
			//IL_06ee: Unknown result type (might be due to invalid IL or missing references)
			//IL_06f3: Unknown result type (might be due to invalid IL or missing references)
			//IL_06f5: Unknown result type (might be due to invalid IL or missing references)
			//IL_06f7: Unknown result type (might be due to invalid IL or missing references)
			//IL_06fc: Unknown result type (might be due to invalid IL or missing references)
			//IL_06ff: Unknown result type (might be due to invalid IL or missing references)
			//IL_071e: Unknown result type (might be due to invalid IL or missing references)
			//IL_072a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0735: Unknown result type (might be due to invalid IL or missing references)
			//IL_0740: Unknown result type (might be due to invalid IL or missing references)
			//IL_074c: Unknown result type (might be due to invalid IL or missing references)
			//IL_075e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0763: Unknown result type (might be due to invalid IL or missing references)
			//IL_0765: Unknown result type (might be due to invalid IL or missing references)
			//IL_076a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0548: Unknown result type (might be due to invalid IL or missing references)
			//IL_054d: Unknown result type (might be due to invalid IL or missing references)
			//IL_054f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0554: Unknown result type (might be due to invalid IL or missing references)
			//IL_0499: Unknown result type (might be due to invalid IL or missing references)
			//IL_04a7: Unknown result type (might be due to invalid IL or missing references)
			//IL_04b3: Unknown result type (might be due to invalid IL or missing references)
			//IL_04b8: Unknown result type (might be due to invalid IL or missing references)
			//IL_04bd: Unknown result type (might be due to invalid IL or missing references)
			//IL_04bf: Unknown result type (might be due to invalid IL or missing references)
			//IL_04c1: Unknown result type (might be due to invalid IL or missing references)
			//IL_04c6: Unknown result type (might be due to invalid IL or missing references)
			//IL_04c9: Unknown result type (might be due to invalid IL or missing references)
			//IL_04d4: Unknown result type (might be due to invalid IL or missing references)
			//IL_04e0: Unknown result type (might be due to invalid IL or missing references)
			//IL_04f0: Unknown result type (might be due to invalid IL or missing references)
			//IL_04f2: Unknown result type (might be due to invalid IL or missing references)
			//IL_04f7: Unknown result type (might be due to invalid IL or missing references)
			//IL_04f9: Unknown result type (might be due to invalid IL or missing references)
			//IL_04fe: Unknown result type (might be due to invalid IL or missing references)
			//IL_0510: Unknown result type (might be due to invalid IL or missing references)
			//IL_0515: Unknown result type (might be due to invalid IL or missing references)
			//IL_0517: Unknown result type (might be due to invalid IL or missing references)
			//IL_051c: Unknown result type (might be due to invalid IL or missing references)
			//IL_051e: Unknown result type (might be due to invalid IL or missing references)
			//IL_052f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0534: Unknown result type (might be due to invalid IL or missing references)
			//IL_0c0b: Unknown result type (might be due to invalid IL or missing references)
			//IL_09b7: Unknown result type (might be due to invalid IL or missing references)
			//IL_0795: Unknown result type (might be due to invalid IL or missing references)
			//IL_079f: Unknown result type (might be due to invalid IL or missing references)
			//IL_07a4: Unknown result type (might be due to invalid IL or missing references)
			//IL_07ae: Unknown result type (might be due to invalid IL or missing references)
			//IL_07b3: Unknown result type (might be due to invalid IL or missing references)
			//IL_07b5: Unknown result type (might be due to invalid IL or missing references)
			//IL_0ce2: Unknown result type (might be due to invalid IL or missing references)
			//IL_0c1f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0c2d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0c34: Unknown result type (might be due to invalid IL or missing references)
			//IL_0c39: Unknown result type (might be due to invalid IL or missing references)
			//IL_0c3e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0c41: Unknown result type (might be due to invalid IL or missing references)
			//IL_0c4c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0c53: Unknown result type (might be due to invalid IL or missing references)
			//IL_0c65: Unknown result type (might be due to invalid IL or missing references)
			//IL_0c6a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0c6c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0c71: Unknown result type (might be due to invalid IL or missing references)
			//IL_09d3: Unknown result type (might be due to invalid IL or missing references)
			//IL_09ec: Unknown result type (might be due to invalid IL or missing references)
			//IL_09f8: Unknown result type (might be due to invalid IL or missing references)
			//IL_0a09: Unknown result type (might be due to invalid IL or missing references)
			//IL_0a0e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0a13: Unknown result type (might be due to invalid IL or missing references)
			//IL_0a15: Unknown result type (might be due to invalid IL or missing references)
			//IL_0a26: Unknown result type (might be due to invalid IL or missing references)
			//IL_0a2b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0a30: Unknown result type (might be due to invalid IL or missing references)
			//IL_029d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0cf6: Unknown result type (might be due to invalid IL or missing references)
			//IL_0d04: Unknown result type (might be due to invalid IL or missing references)
			//IL_0d0b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0d10: Unknown result type (might be due to invalid IL or missing references)
			//IL_0d15: Unknown result type (might be due to invalid IL or missing references)
			//IL_0d18: Unknown result type (might be due to invalid IL or missing references)
			//IL_0d23: Unknown result type (might be due to invalid IL or missing references)
			//IL_0d2a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0d3c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0d41: Unknown result type (might be due to invalid IL or missing references)
			//IL_0d43: Unknown result type (might be due to invalid IL or missing references)
			//IL_0d48: Unknown result type (might be due to invalid IL or missing references)
			//IL_0a54: Unknown result type (might be due to invalid IL or missing references)
			//IL_0a73: Unknown result type (might be due to invalid IL or missing references)
			//IL_0a84: Unknown result type (might be due to invalid IL or missing references)
			//IL_0a94: Unknown result type (might be due to invalid IL or missing references)
			//IL_0a96: Unknown result type (might be due to invalid IL or missing references)
			//IL_0a98: Unknown result type (might be due to invalid IL or missing references)
			//IL_0a9d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0aad: Unknown result type (might be due to invalid IL or missing references)
			//IL_0aaf: Unknown result type (might be due to invalid IL or missing references)
			//IL_0ab1: Unknown result type (might be due to invalid IL or missing references)
			//IL_0ab6: Unknown result type (might be due to invalid IL or missing references)
			//IL_07da: Unknown result type (might be due to invalid IL or missing references)
			//IL_0579: Unknown result type (might be due to invalid IL or missing references)
			//IL_02b4: Unknown result type (might be due to invalid IL or missing references)
			//IL_02c2: Unknown result type (might be due to invalid IL or missing references)
			//IL_02c9: Unknown result type (might be due to invalid IL or missing references)
			//IL_02ce: Unknown result type (might be due to invalid IL or missing references)
			//IL_02d3: Unknown result type (might be due to invalid IL or missing references)
			//IL_02d6: Unknown result type (might be due to invalid IL or missing references)
			//IL_02e1: Unknown result type (might be due to invalid IL or missing references)
			//IL_02e8: Unknown result type (might be due to invalid IL or missing references)
			//IL_02f2: Unknown result type (might be due to invalid IL or missing references)
			//IL_02f4: Unknown result type (might be due to invalid IL or missing references)
			//IL_02f9: Unknown result type (might be due to invalid IL or missing references)
			//IL_02fb: Unknown result type (might be due to invalid IL or missing references)
			//IL_0ad0: Unknown result type (might be due to invalid IL or missing references)
			//IL_0ad5: Unknown result type (might be due to invalid IL or missing references)
			//IL_0ad7: Unknown result type (might be due to invalid IL or missing references)
			//IL_0ad9: Unknown result type (might be due to invalid IL or missing references)
			//IL_0ade: Unknown result type (might be due to invalid IL or missing references)
			//IL_0875: Unknown result type (might be due to invalid IL or missing references)
			//IL_07f1: Unknown result type (might be due to invalid IL or missing references)
			//IL_07ff: Unknown result type (might be due to invalid IL or missing references)
			//IL_0806: Unknown result type (might be due to invalid IL or missing references)
			//IL_080b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0810: Unknown result type (might be due to invalid IL or missing references)
			//IL_0813: Unknown result type (might be due to invalid IL or missing references)
			//IL_081e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0825: Unknown result type (might be due to invalid IL or missing references)
			//IL_082f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0831: Unknown result type (might be due to invalid IL or missing references)
			//IL_0836: Unknown result type (might be due to invalid IL or missing references)
			//IL_0838: Unknown result type (might be due to invalid IL or missing references)
			//IL_0590: Unknown result type (might be due to invalid IL or missing references)
			//IL_059e: Unknown result type (might be due to invalid IL or missing references)
			//IL_05a5: Unknown result type (might be due to invalid IL or missing references)
			//IL_05aa: Unknown result type (might be due to invalid IL or missing references)
			//IL_05af: Unknown result type (might be due to invalid IL or missing references)
			//IL_05b2: Unknown result type (might be due to invalid IL or missing references)
			//IL_05bd: Unknown result type (might be due to invalid IL or missing references)
			//IL_05c4: Unknown result type (might be due to invalid IL or missing references)
			//IL_05ce: Unknown result type (might be due to invalid IL or missing references)
			//IL_05d0: Unknown result type (might be due to invalid IL or missing references)
			//IL_05d5: Unknown result type (might be due to invalid IL or missing references)
			//IL_05d7: Unknown result type (might be due to invalid IL or missing references)
			//IL_0325: Unknown result type (might be due to invalid IL or missing references)
			//IL_0327: Unknown result type (might be due to invalid IL or missing references)
			//IL_032c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0312: Unknown result type (might be due to invalid IL or missing references)
			//IL_0314: Unknown result type (might be due to invalid IL or missing references)
			//IL_0316: Unknown result type (might be due to invalid IL or missing references)
			//IL_031b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0889: Unknown result type (might be due to invalid IL or missing references)
			//IL_0893: Unknown result type (might be due to invalid IL or missing references)
			//IL_0898: Unknown result type (might be due to invalid IL or missing references)
			//IL_089a: Unknown result type (might be due to invalid IL or missing references)
			//IL_089c: Unknown result type (might be due to invalid IL or missing references)
			//IL_089e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0862: Unknown result type (might be due to invalid IL or missing references)
			//IL_0864: Unknown result type (might be due to invalid IL or missing references)
			//IL_0869: Unknown result type (might be due to invalid IL or missing references)
			//IL_084f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0851: Unknown result type (might be due to invalid IL or missing references)
			//IL_0853: Unknown result type (might be due to invalid IL or missing references)
			//IL_0858: Unknown result type (might be due to invalid IL or missing references)
			//IL_0601: Unknown result type (might be due to invalid IL or missing references)
			//IL_0603: Unknown result type (might be due to invalid IL or missing references)
			//IL_0608: Unknown result type (might be due to invalid IL or missing references)
			//IL_05ee: Unknown result type (might be due to invalid IL or missing references)
			//IL_05f0: Unknown result type (might be due to invalid IL or missing references)
			//IL_05f2: Unknown result type (might be due to invalid IL or missing references)
			//IL_05f7: Unknown result type (might be due to invalid IL or missing references)
			//IL_0afb: Unknown result type (might be due to invalid IL or missing references)
			//IL_08c3: Unknown result type (might be due to invalid IL or missing references)
			//IL_08c5: Unknown result type (might be due to invalid IL or missing references)
			//IL_08b5: Unknown result type (might be due to invalid IL or missing references)
			//IL_08b7: Unknown result type (might be due to invalid IL or missing references)
			//IL_08b9: Unknown result type (might be due to invalid IL or missing references)
			//IL_0b12: Unknown result type (might be due to invalid IL or missing references)
			//IL_0b20: Unknown result type (might be due to invalid IL or missing references)
			//IL_0b27: Unknown result type (might be due to invalid IL or missing references)
			//IL_0b2c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0b31: Unknown result type (might be due to invalid IL or missing references)
			//IL_0b34: Unknown result type (might be due to invalid IL or missing references)
			//IL_0b3f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0b46: Unknown result type (might be due to invalid IL or missing references)
			//IL_0b87: Unknown result type (might be due to invalid IL or missing references)
			//IL_0b89: Unknown result type (might be due to invalid IL or missing references)
			//IL_0b8e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0b54: Unknown result type (might be due to invalid IL or missing references)
			//IL_0b56: Unknown result type (might be due to invalid IL or missing references)
			//IL_0b5b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0b5d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0b74: Unknown result type (might be due to invalid IL or missing references)
			//IL_0b76: Unknown result type (might be due to invalid IL or missing references)
			//IL_0b78: Unknown result type (might be due to invalid IL or missing references)
			//IL_0b7d: Unknown result type (might be due to invalid IL or missing references)
			NativeArray<Transform> nativeArray = ((ArchetypeChunk)(ref chunk)).GetNativeArray<Transform>(ref m_TransformType);
			int num;
			int num2;
			if (m_Selected != Entity.Null)
			{
				num = (num2 = -1);
				NativeArray<Entity> nativeArray2 = ((ArchetypeChunk)(ref chunk)).GetNativeArray(m_EntityType);
				for (int i = 0; i < nativeArray2.Length; i++)
				{
					if (nativeArray2[i] == m_Selected)
					{
						num = i;
						num2 = i + 1;
						break;
					}
				}
				if (num == -1)
				{
					return;
				}
			}
			else
			{
				num = 0;
				num2 = ((ArchetypeChunk)(ref chunk)).Count;
			}
			if (m_CarOption)
			{
				NativeArray<CarCurrentLane> nativeArray3 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<CarCurrentLane>(ref m_CarCurrentLaneType);
				if (nativeArray3.Length != 0)
				{
					BufferAccessor<CarNavigationLane> bufferAccessor = ((ArchetypeChunk)(ref chunk)).GetBufferAccessor<CarNavigationLane>(ref m_CarNavigationLaneType);
					float timeOffset = m_TimeOffset * 10f;
					for (int j = num; j < num2; j++)
					{
						CarCurrentLane carCurrentLane = nativeArray3[j];
						Transform transform = nativeArray[j];
						DynamicBuffer<CarNavigationLane> val = bufferAccessor[j];
						if (!m_CurveData.HasComponent(carCurrentLane.m_Lane))
						{
							continue;
						}
						Curve curve = m_CurveData[carCurrentLane.m_Lane];
						Bezier4x3 val2 = MathUtils.Cut(curve.m_Bezier, ((float3)(ref carCurrentLane.m_CurvePosition)).xy);
						Bezier4x3 val3 = MathUtils.Cut(curve.m_Bezier, ((float3)(ref carCurrentLane.m_CurvePosition)).yz);
						float3 d = val3.d;
						DrawNavigationCurve(val2, curve.m_Length, timeOffset, new Color(1f, 0.5f, 0f, 1f), ((float3)(ref carCurrentLane.m_CurvePosition)).xy);
						DrawNavigationCurve(val3, curve.m_Length, timeOffset, Color.yellow, ((float3)(ref carCurrentLane.m_CurvePosition)).yz);
						if (m_CurveData.HasComponent(carCurrentLane.m_ChangeLane))
						{
							curve = m_CurveData[carCurrentLane.m_ChangeLane];
							Bezier4x3 val4 = MathUtils.Cut(curve.m_Bezier, ((float3)(ref carCurrentLane.m_CurvePosition)).xz);
							d = val4.d;
							DrawNavigationCurve(val4, curve.m_Length, timeOffset, Color.magenta, ((float3)(ref carCurrentLane.m_CurvePosition)).xz);
							((GizmoBatcher)(ref m_GizmoBatcher)).DrawLine(val2.a, val4.a, Color.magenta);
							((GizmoBatcher)(ref m_GizmoBatcher)).DrawLine(transform.m_Position, math.lerp(val2.a, val4.a, math.saturate(carCurrentLane.m_ChangeProgress)), Color.red);
						}
						else
						{
							((GizmoBatcher)(ref m_GizmoBatcher)).DrawLine(transform.m_Position, val2.a, Color.red);
						}
						for (int k = 0; k < val.Length; k++)
						{
							CarNavigationLane carNavigationLane = val[k];
							if (!m_CurveData.HasComponent(carNavigationLane.m_Lane))
							{
								break;
							}
							curve = m_CurveData[carNavigationLane.m_Lane];
							val2 = MathUtils.Cut(curve.m_Bezier, carNavigationLane.m_CurvePosition);
							DrawNavigationCurve(val2, curve.m_Length, timeOffset, Color.green, carNavigationLane.m_CurvePosition);
							if (math.lengthsq(val2.a - d) > 1f)
							{
								((GizmoBatcher)(ref m_GizmoBatcher)).DrawLine(d, val2.a, Color.magenta);
							}
							d = val2.d;
						}
					}
				}
			}
			if (m_WatercraftOption)
			{
				NativeArray<WatercraftCurrentLane> nativeArray4 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<WatercraftCurrentLane>(ref m_WatercraftCurrentLaneType);
				if (nativeArray4.Length != 0)
				{
					BufferAccessor<WatercraftNavigationLane> bufferAccessor2 = ((ArchetypeChunk)(ref chunk)).GetBufferAccessor<WatercraftNavigationLane>(ref m_WatercraftNavigationLaneType);
					float timeOffset2 = m_TimeOffset * 10f;
					for (int l = num; l < num2; l++)
					{
						WatercraftCurrentLane watercraftCurrentLane = nativeArray4[l];
						Transform transform2 = nativeArray[l];
						DynamicBuffer<WatercraftNavigationLane> val5 = bufferAccessor2[l];
						if (!m_CurveData.HasComponent(watercraftCurrentLane.m_Lane))
						{
							continue;
						}
						Curve curve2 = m_CurveData[watercraftCurrentLane.m_Lane];
						Bezier4x3 val6 = MathUtils.Cut(curve2.m_Bezier, ((float3)(ref watercraftCurrentLane.m_CurvePosition)).xy);
						Bezier4x3 val7 = MathUtils.Cut(curve2.m_Bezier, ((float3)(ref watercraftCurrentLane.m_CurvePosition)).yz);
						float3 d2 = val7.d;
						DrawNavigationCurve(val6, curve2.m_Length, timeOffset2, new Color(1f, 0.5f, 0f, 1f), ((float3)(ref watercraftCurrentLane.m_CurvePosition)).xy);
						DrawNavigationCurve(val7, curve2.m_Length, timeOffset2, Color.yellow, ((float3)(ref watercraftCurrentLane.m_CurvePosition)).yz);
						if (m_CurveData.HasComponent(watercraftCurrentLane.m_ChangeLane))
						{
							curve2 = m_CurveData[watercraftCurrentLane.m_ChangeLane];
							Bezier4x3 val8 = MathUtils.Cut(curve2.m_Bezier, ((float3)(ref watercraftCurrentLane.m_CurvePosition)).xz);
							d2 = val8.d;
							DrawNavigationCurve(val8, curve2.m_Length, timeOffset2, Color.magenta, ((float3)(ref watercraftCurrentLane.m_CurvePosition)).xz);
							((GizmoBatcher)(ref m_GizmoBatcher)).DrawLine(val6.a, val8.a, Color.magenta);
							((GizmoBatcher)(ref m_GizmoBatcher)).DrawLine(transform2.m_Position, math.lerp(val6.a, val8.a, math.saturate(watercraftCurrentLane.m_ChangeProgress)), Color.red);
						}
						else
						{
							((GizmoBatcher)(ref m_GizmoBatcher)).DrawLine(transform2.m_Position, val6.a, Color.red);
						}
						for (int m = 0; m < val5.Length; m++)
						{
							WatercraftNavigationLane watercraftNavigationLane = val5[m];
							if (!m_CurveData.HasComponent(watercraftNavigationLane.m_Lane))
							{
								break;
							}
							curve2 = m_CurveData[watercraftNavigationLane.m_Lane];
							val6 = MathUtils.Cut(curve2.m_Bezier, watercraftNavigationLane.m_CurvePosition);
							DrawNavigationCurve(val6, curve2.m_Length, timeOffset2, Color.green, watercraftNavigationLane.m_CurvePosition);
							if (math.lengthsq(val6.a - d2) > 1f)
							{
								((GizmoBatcher)(ref m_GizmoBatcher)).DrawLine(d2, val6.a, Color.magenta);
							}
							d2 = val6.d;
						}
					}
				}
			}
			if (m_AircraftOption)
			{
				NativeArray<AircraftCurrentLane> nativeArray5 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<AircraftCurrentLane>(ref m_AircraftCurrentLaneType);
				if (nativeArray5.Length != 0)
				{
					BufferAccessor<AircraftNavigationLane> bufferAccessor3 = ((ArchetypeChunk)(ref chunk)).GetBufferAccessor<AircraftNavigationLane>(ref m_AircraftNavigationLaneType);
					float timeOffset3 = m_TimeOffset * 10f;
					for (int n = num; n < num2; n++)
					{
						AircraftCurrentLane aircraftCurrentLane = nativeArray5[n];
						Transform transform3 = nativeArray[n];
						DynamicBuffer<AircraftNavigationLane> val9 = bufferAccessor3[n];
						float3 val12;
						if (m_CurveData.HasComponent(aircraftCurrentLane.m_Lane))
						{
							Curve curve3 = m_CurveData[aircraftCurrentLane.m_Lane];
							Bezier4x3 val10 = MathUtils.Cut(curve3.m_Bezier, ((float3)(ref aircraftCurrentLane.m_CurvePosition)).xy);
							Bezier4x3 val11 = MathUtils.Cut(curve3.m_Bezier, ((float3)(ref aircraftCurrentLane.m_CurvePosition)).yz);
							val12 = val11.d;
							DrawNavigationCurve(val10, curve3.m_Length, timeOffset3, new Color(1f, 0.5f, 0f, 1f), ((float3)(ref aircraftCurrentLane.m_CurvePosition)).xy);
							DrawNavigationCurve(val11, curve3.m_Length, timeOffset3, Color.yellow, ((float3)(ref aircraftCurrentLane.m_CurvePosition)).yz);
							((GizmoBatcher)(ref m_GizmoBatcher)).DrawLine(transform3.m_Position, val10.a, Color.red);
						}
						else
						{
							if (!m_TransformData.HasComponent(aircraftCurrentLane.m_Lane))
							{
								continue;
							}
							val12 = m_TransformData[aircraftCurrentLane.m_Lane].m_Position;
							((GizmoBatcher)(ref m_GizmoBatcher)).DrawLine(transform3.m_Position, val12, Color.red);
						}
						for (int num3 = 0; num3 < val9.Length; num3++)
						{
							AircraftNavigationLane aircraftNavigationLane = val9[num3];
							if (m_CurveData.HasComponent(aircraftNavigationLane.m_Lane))
							{
								Curve curve4 = m_CurveData[aircraftNavigationLane.m_Lane];
								Bezier4x3 val13 = MathUtils.Cut(curve4.m_Bezier, aircraftNavigationLane.m_CurvePosition);
								DrawNavigationCurve(val13, curve4.m_Length, timeOffset3, Color.green, aircraftNavigationLane.m_CurvePosition);
								if (math.lengthsq(val13.a - val12) > 1f)
								{
									((GizmoBatcher)(ref m_GizmoBatcher)).DrawLine(val12, val13.a, Color.magenta);
								}
								val12 = val13.d;
								continue;
							}
							if (!m_TransformData.HasComponent(aircraftCurrentLane.m_Lane))
							{
								break;
							}
							float3 position = m_TransformData[aircraftCurrentLane.m_Lane].m_Position;
							if (math.lengthsq(position - val12) > 1f)
							{
								((GizmoBatcher)(ref m_GizmoBatcher)).DrawLine(val12, position, Color.magenta);
							}
							val12 = position;
						}
					}
				}
			}
			if (m_TrainOption)
			{
				NativeArray<TrainCurrentLane> nativeArray6 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<TrainCurrentLane>(ref m_TrainCurrentLaneType);
				if (nativeArray6.Length != 0)
				{
					NativeArray<Train> nativeArray7 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<Train>(ref m_TrainType);
					NativeArray<PrefabRef> nativeArray8 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<PrefabRef>(ref m_PrefabRefType);
					BufferAccessor<TrainNavigationLane> bufferAccessor4 = ((ArchetypeChunk)(ref chunk)).GetBufferAccessor<TrainNavigationLane>(ref m_TrainNavigationLaneType);
					float timeOffset4 = m_TimeOffset * 10f;
					for (int num4 = num; num4 < num2; num4++)
					{
						Train train = nativeArray7[num4];
						TrainCurrentLane trainCurrentLane = nativeArray6[num4];
						Transform transform4 = nativeArray[num4];
						PrefabRef prefabRef = nativeArray8[num4];
						TrainData prefabTrainData = m_PrefabTrainData[prefabRef.m_Prefab];
						if (!m_CurveData.HasComponent(trainCurrentLane.m_Front.m_Lane) || !m_CurveData.HasComponent(trainCurrentLane.m_Rear.m_Lane))
						{
							continue;
						}
						Curve curve5 = m_CurveData[trainCurrentLane.m_Front.m_Lane];
						Curve curve6 = m_CurveData[trainCurrentLane.m_Rear.m_Lane];
						Bezier4x3 val14 = MathUtils.Cut(curve5.m_Bezier, ((float4)(ref trainCurrentLane.m_Front.m_CurvePosition)).yw);
						Bezier4x3 val15 = MathUtils.Cut(curve6.m_Bezier, ((float4)(ref trainCurrentLane.m_Rear.m_CurvePosition)).yw);
						VehicleUtils.CalculateTrainNavigationPivots(transform4, prefabTrainData, out var pivot, out var pivot2);
						if ((train.m_Flags & Game.Vehicles.TrainFlags.Reversed) != 0)
						{
							CommonUtils.Swap(ref pivot, ref pivot2);
						}
						DrawNavigationCurve(val14, curve5.m_Length, timeOffset4, new Color(1f, 0.5f, 0f, 1f), ((float4)(ref trainCurrentLane.m_Front.m_CurvePosition)).yw);
						((GizmoBatcher)(ref m_GizmoBatcher)).DrawLine(pivot, val14.a, Color.red);
						((GizmoBatcher)(ref m_GizmoBatcher)).DrawLine(pivot2, val15.a, Color.red);
						if (bufferAccessor4.Length == 0)
						{
							continue;
						}
						DynamicBuffer<TrainNavigationLane> val16 = bufferAccessor4[num4];
						float3 d3 = val14.d;
						for (int num5 = 0; num5 < val16.Length; num5++)
						{
							TrainNavigationLane trainNavigationLane = val16[num5];
							if (!m_CurveData.HasComponent(trainNavigationLane.m_Lane))
							{
								break;
							}
							curve5 = m_CurveData[trainNavigationLane.m_Lane];
							val14 = MathUtils.Cut(curve5.m_Bezier, trainNavigationLane.m_CurvePosition);
							DrawNavigationCurve(val14, curve5.m_Length, timeOffset4, Color.green, trainNavigationLane.m_CurvePosition);
							if (num5 != 0 && math.lengthsq(val14.a - d3) > 1f)
							{
								((GizmoBatcher)(ref m_GizmoBatcher)).DrawLine(d3, val14.a, Color.magenta);
							}
							d3 = val14.d;
						}
					}
				}
			}
			if (m_HumanOption)
			{
				NativeArray<HumanCurrentLane> nativeArray9 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<HumanCurrentLane>(ref m_HumanCurrentLaneType);
				if (nativeArray9.Length != 0)
				{
					float timeOffset5 = m_TimeOffset * 5f;
					for (int num6 = num; num6 < num2; num6++)
					{
						HumanCurrentLane humanCurrentLane = nativeArray9[num6];
						Transform transform5 = nativeArray[num6];
						if (m_CurveData.HasComponent(humanCurrentLane.m_Lane))
						{
							Curve curve7 = m_CurveData[humanCurrentLane.m_Lane];
							Bezier4x3 val17 = MathUtils.Cut(curve7.m_Bezier, humanCurrentLane.m_CurvePosition);
							DrawNavigationCurve(val17, curve7.m_Length, timeOffset5, Color.yellow, humanCurrentLane.m_CurvePosition);
							((GizmoBatcher)(ref m_GizmoBatcher)).DrawLine(transform5.m_Position, val17.a, Color.red);
						}
					}
				}
			}
			if (!m_AnimalOption)
			{
				return;
			}
			NativeArray<AnimalCurrentLane> nativeArray10 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<AnimalCurrentLane>(ref m_AnimalCurrentLaneType);
			if (nativeArray10.Length == 0)
			{
				return;
			}
			float timeOffset6 = m_TimeOffset * 5f;
			for (int num7 = num; num7 < num2; num7++)
			{
				AnimalCurrentLane animalCurrentLane = nativeArray10[num7];
				Transform transform6 = nativeArray[num7];
				if (m_CurveData.HasComponent(animalCurrentLane.m_Lane))
				{
					Curve curve8 = m_CurveData[animalCurrentLane.m_Lane];
					Bezier4x3 val18 = MathUtils.Cut(curve8.m_Bezier, animalCurrentLane.m_CurvePosition);
					DrawNavigationCurve(val18, curve8.m_Length, timeOffset6, Color.yellow, animalCurrentLane.m_CurvePosition);
					((GizmoBatcher)(ref m_GizmoBatcher)).DrawLine(transform6.m_Position, val18.a, Color.red);
				}
			}
		}

		private void DrawNavigationCurve(Bezier4x3 curve, float totalLength, float timeOffset, Color color, float2 curveDelta)
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0008: Unknown result type (might be due to invalid IL or missing references)
			//IL_0045: Unknown result type (might be due to invalid IL or missing references)
			//IL_0047: Unknown result type (might be due to invalid IL or missing references)
			//IL_0025: Unknown result type (might be due to invalid IL or missing references)
			//IL_0027: Unknown result type (might be due to invalid IL or missing references)
			float num = totalLength * math.abs(curveDelta.x - curveDelta.y);
			if (num >= 1f)
			{
				((GizmoBatcher)(ref m_GizmoBatcher)).DrawFlowCurve(curve, num, color, timeOffset, false, 1, -1, 1f, 25f, 16);
			}
			else
			{
				((GizmoBatcher)(ref m_GizmoBatcher)).DrawCurve(curve, num, color, -1);
			}
		}

		void IJobChunk.Execute(in ArchetypeChunk chunk, int unfilteredChunkIndex, bool useEnabledMask, in v128 chunkEnabledMask)
		{
			Execute(in chunk, unfilteredChunkIndex, useEnabledMask, in chunkEnabledMask);
		}
	}

	[BurstCompile]
	private struct SelectedNavigationGizmoJob : IJob
	{
		[ReadOnly]
		public Entity m_Selected;

		[ReadOnly]
		public ComponentLookup<CarCurrentLane> m_CarCurrentLaneType;

		[ReadOnly]
		public BufferLookup<CarNavigationLane> m_CarNavigationLaneType;

		[ReadOnly]
		public ComponentLookup<CarNavigation> m_CarNavigationType;

		[ReadOnly]
		public ComponentLookup<WatercraftCurrentLane> m_WatercraftCurrentLaneType;

		[ReadOnly]
		public BufferLookup<WatercraftNavigationLane> m_WatercraftNavigationLaneType;

		[ReadOnly]
		public ComponentLookup<WatercraftNavigation> m_WatercraftNavigationType;

		[ReadOnly]
		public ComponentLookup<AircraftCurrentLane> m_AircraftCurrentLaneType;

		[ReadOnly]
		public BufferLookup<AircraftNavigationLane> m_AircraftNavigationLaneType;

		[ReadOnly]
		public ComponentLookup<AircraftNavigation> m_AircraftNavigationType;

		[ReadOnly]
		public ComponentLookup<TrainCurrentLane> m_TrainCurrentLaneType;

		[ReadOnly]
		public BufferLookup<TrainNavigationLane> m_TrainNavigationLaneType;

		[ReadOnly]
		public BufferLookup<LayoutElement> m_LayoutElementType;

		[ReadOnly]
		public ComponentLookup<Blocker> m_BlockerType;

		[ReadOnly]
		public ComponentLookup<HumanCurrentLane> m_HumanCurrentLaneType;

		[ReadOnly]
		public ComponentLookup<AnimalCurrentLane> m_AnimalCurrentLaneType;

		[ReadOnly]
		public ComponentLookup<Target> m_TargetType;

		[ReadOnly]
		public ComponentLookup<PseudoRandomSeed> m_PseudoRandomSeedType;

		[ReadOnly]
		public ComponentLookup<Curve> m_CurveData;

		[ReadOnly]
		public ComponentLookup<Lane> m_LaneData;

		[ReadOnly]
		public ComponentLookup<Game.Net.CarLane> m_CarLaneData;

		[ReadOnly]
		public ComponentLookup<Game.Net.TrackLane> m_TrackLaneData;

		[ReadOnly]
		public ComponentLookup<Game.Net.ParkingLane> m_ParkingLaneData;

		[ReadOnly]
		public ComponentLookup<LaneReservation> m_LaneReservationData;

		[ReadOnly]
		public ComponentLookup<LaneCondition> m_LaneConditionData;

		[ReadOnly]
		public ComponentLookup<LaneSignal> m_LaneSignalData;

		[ReadOnly]
		public ComponentLookup<SlaveLane> m_SlaveLaneData;

		[ReadOnly]
		public ComponentLookup<AreaLane> m_AreaLaneData;

		[ReadOnly]
		public ComponentLookup<Transform> m_TransformDataFromEntity;

		[ReadOnly]
		public ComponentLookup<Moving> m_MovingDataFromEntity;

		[ReadOnly]
		public ComponentLookup<Car> m_CarDataFromEntity;

		[ReadOnly]
		public ComponentLookup<Watercraft> m_WatercraftDataFromEntity;

		[ReadOnly]
		public ComponentLookup<Aircraft> m_AircraftDataFromEntity;

		[ReadOnly]
		public ComponentLookup<Train> m_TrainDataFromEntity;

		[ReadOnly]
		public ComponentLookup<Controller> m_ControllerDataFromEntity;

		[ReadOnly]
		public ComponentLookup<Owner> m_OwnerDataFromEntity;

		[ReadOnly]
		public ComponentLookup<Unspawned> m_UnspawnedData;

		[ReadOnly]
		public ComponentLookup<TrainCurrentLane> m_TrainCurrentLaneData;

		[ReadOnly]
		public ComponentLookup<Creature> m_CreatureData;

		[ReadOnly]
		public ComponentLookup<Position> m_PositionData;

		[ReadOnly]
		public ComponentLookup<PrefabRef> m_PrefabRefDataFromEntity;

		[ReadOnly]
		public ComponentLookup<CarData> m_PrefabCarData;

		[ReadOnly]
		public ComponentLookup<TrainData> m_PrefabTrainData;

		[ReadOnly]
		public ComponentLookup<WatercraftData> m_PrefabWatercraftData;

		[ReadOnly]
		public ComponentLookup<AircraftData> m_PrefabAircraftData;

		[ReadOnly]
		public ComponentLookup<ObjectGeometryData> m_PrefabObjectGeometryData;

		[ReadOnly]
		public ComponentLookup<BuildingData> m_PrefabBuildingData;

		[ReadOnly]
		public ComponentLookup<NetLaneData> m_PrefabNetLaneData;

		[ReadOnly]
		public ComponentLookup<ParkingLaneData> m_PrefabParkingLaneData;

		[ReadOnly]
		public BufferLookup<LaneObject> m_LaneObjects;

		[ReadOnly]
		public BufferLookup<LaneOverlap> m_LaneOverlaps;

		[ReadOnly]
		public BufferLookup<Game.Net.SubLane> m_SubLanes;

		[ReadOnly]
		public BufferLookup<Game.Areas.Node> m_AreaNodes;

		[ReadOnly]
		public uint m_SimulationFrame;

		public GizmoBatcher m_GizmoBatcher;

		public void Execute()
		{
			//IL_0008: Unknown result type (might be due to invalid IL or missing references)
			//IL_0015: Unknown result type (might be due to invalid IL or missing references)
			//IL_0ce2: Unknown result type (might be due to invalid IL or missing references)
			//IL_002d: Unknown result type (might be due to invalid IL or missing references)
			//IL_181d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0cfa: Unknown result type (might be due to invalid IL or missing references)
			//IL_004d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0060: Unknown result type (might be due to invalid IL or missing references)
			//IL_0072: Unknown result type (might be due to invalid IL or missing references)
			//IL_0085: Unknown result type (might be due to invalid IL or missing references)
			//IL_0098: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ab: Unknown result type (might be due to invalid IL or missing references)
			//IL_00be: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d6: Unknown result type (might be due to invalid IL or missing references)
			//IL_00db: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e5: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f9: Unknown result type (might be due to invalid IL or missing references)
			//IL_0106: Unknown result type (might be due to invalid IL or missing references)
			//IL_1f19: Unknown result type (might be due to invalid IL or missing references)
			//IL_1835: Unknown result type (might be due to invalid IL or missing references)
			//IL_0d1a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0d2d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0d3f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0d51: Unknown result type (might be due to invalid IL or missing references)
			//IL_0d64: Unknown result type (might be due to invalid IL or missing references)
			//IL_0d77: Unknown result type (might be due to invalid IL or missing references)
			//IL_0d8a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0d8f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0d94: Unknown result type (might be due to invalid IL or missing references)
			//IL_0d9e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0db2: Unknown result type (might be due to invalid IL or missing references)
			//IL_0dbf: Unknown result type (might be due to invalid IL or missing references)
			//IL_2773: Unknown result type (might be due to invalid IL or missing references)
			//IL_1f31: Unknown result type (might be due to invalid IL or missing references)
			//IL_184d: Unknown result type (might be due to invalid IL or missing references)
			//IL_1860: Unknown result type (might be due to invalid IL or missing references)
			//IL_1872: Unknown result type (might be due to invalid IL or missing references)
			//IL_1884: Unknown result type (might be due to invalid IL or missing references)
			//IL_1897: Unknown result type (might be due to invalid IL or missing references)
			//IL_18aa: Unknown result type (might be due to invalid IL or missing references)
			//IL_18bd: Unknown result type (might be due to invalid IL or missing references)
			//IL_18c2: Unknown result type (might be due to invalid IL or missing references)
			//IL_18c7: Unknown result type (might be due to invalid IL or missing references)
			//IL_18d1: Unknown result type (might be due to invalid IL or missing references)
			//IL_18e5: Unknown result type (might be due to invalid IL or missing references)
			//IL_18f9: Unknown result type (might be due to invalid IL or missing references)
			//IL_2bae: Unknown result type (might be due to invalid IL or missing references)
			//IL_278b: Unknown result type (might be due to invalid IL or missing references)
			//IL_279f: Unknown result type (might be due to invalid IL or missing references)
			//IL_1f43: Unknown result type (might be due to invalid IL or missing references)
			//IL_1f48: Unknown result type (might be due to invalid IL or missing references)
			//IL_1f51: Unknown result type (might be due to invalid IL or missing references)
			//IL_19c9: Unknown result type (might be due to invalid IL or missing references)
			//IL_1910: Unknown result type (might be due to invalid IL or missing references)
			//IL_191e: Unknown result type (might be due to invalid IL or missing references)
			//IL_192a: Unknown result type (might be due to invalid IL or missing references)
			//IL_192f: Unknown result type (might be due to invalid IL or missing references)
			//IL_1934: Unknown result type (might be due to invalid IL or missing references)
			//IL_1938: Unknown result type (might be due to invalid IL or missing references)
			//IL_1944: Unknown result type (might be due to invalid IL or missing references)
			//IL_1949: Unknown result type (might be due to invalid IL or missing references)
			//IL_194e: Unknown result type (might be due to invalid IL or missing references)
			//IL_1950: Unknown result type (might be due to invalid IL or missing references)
			//IL_1952: Unknown result type (might be due to invalid IL or missing references)
			//IL_1957: Unknown result type (might be due to invalid IL or missing references)
			//IL_195f: Unknown result type (might be due to invalid IL or missing references)
			//IL_197c: Unknown result type (might be due to invalid IL or missing references)
			//IL_198d: Unknown result type (might be due to invalid IL or missing references)
			//IL_1996: Unknown result type (might be due to invalid IL or missing references)
			//IL_19a9: Unknown result type (might be due to invalid IL or missing references)
			//IL_19ae: Unknown result type (might be due to invalid IL or missing references)
			//IL_19b0: Unknown result type (might be due to invalid IL or missing references)
			//IL_19b5: Unknown result type (might be due to invalid IL or missing references)
			//IL_0196: Unknown result type (might be due to invalid IL or missing references)
			//IL_019b: Unknown result type (might be due to invalid IL or missing references)
			//IL_2bc3: Unknown result type (might be due to invalid IL or missing references)
			//IL_2bd1: Unknown result type (might be due to invalid IL or missing references)
			//IL_2bd6: Unknown result type (might be due to invalid IL or missing references)
			//IL_2b7b: Unknown result type (might be due to invalid IL or missing references)
			//IL_2b80: Unknown result type (might be due to invalid IL or missing references)
			//IL_27b6: Unknown result type (might be due to invalid IL or missing references)
			//IL_27c0: Unknown result type (might be due to invalid IL or missing references)
			//IL_27c5: Unknown result type (might be due to invalid IL or missing references)
			//IL_27cf: Unknown result type (might be due to invalid IL or missing references)
			//IL_27e1: Unknown result type (might be due to invalid IL or missing references)
			//IL_27e3: Unknown result type (might be due to invalid IL or missing references)
			//IL_27e8: Unknown result type (might be due to invalid IL or missing references)
			//IL_27ec: Unknown result type (might be due to invalid IL or missing references)
			//IL_27f8: Unknown result type (might be due to invalid IL or missing references)
			//IL_1f7e: Unknown result type (might be due to invalid IL or missing references)
			//IL_1f8d: Unknown result type (might be due to invalid IL or missing references)
			//IL_1f9a: Unknown result type (might be due to invalid IL or missing references)
			//IL_1fa9: Unknown result type (might be due to invalid IL or missing references)
			//IL_1fba: Unknown result type (might be due to invalid IL or missing references)
			//IL_1fce: Unknown result type (might be due to invalid IL or missing references)
			//IL_19dd: Unknown result type (might be due to invalid IL or missing references)
			//IL_19e7: Unknown result type (might be due to invalid IL or missing references)
			//IL_19ec: Unknown result type (might be due to invalid IL or missing references)
			//IL_19f6: Unknown result type (might be due to invalid IL or missing references)
			//IL_19fb: Unknown result type (might be due to invalid IL or missing references)
			//IL_19fd: Unknown result type (might be due to invalid IL or missing references)
			//IL_0e42: Unknown result type (might be due to invalid IL or missing references)
			//IL_0e47: Unknown result type (might be due to invalid IL or missing references)
			//IL_2be5: Unknown result type (might be due to invalid IL or missing references)
			//IL_2b8f: Unknown result type (might be due to invalid IL or missing references)
			//IL_28b9: Unknown result type (might be due to invalid IL or missing references)
			//IL_28c3: Unknown result type (might be due to invalid IL or missing references)
			//IL_28ca: Unknown result type (might be due to invalid IL or missing references)
			//IL_28d4: Unknown result type (might be due to invalid IL or missing references)
			//IL_28de: Unknown result type (might be due to invalid IL or missing references)
			//IL_280d: Unknown result type (might be due to invalid IL or missing references)
			//IL_281c: Unknown result type (might be due to invalid IL or missing references)
			//IL_2825: Unknown result type (might be due to invalid IL or missing references)
			//IL_2834: Unknown result type (might be due to invalid IL or missing references)
			//IL_283d: Unknown result type (might be due to invalid IL or missing references)
			//IL_284c: Unknown result type (might be due to invalid IL or missing references)
			//IL_285c: Unknown result type (might be due to invalid IL or missing references)
			//IL_285e: Unknown result type (might be due to invalid IL or missing references)
			//IL_2863: Unknown result type (might be due to invalid IL or missing references)
			//IL_2865: Unknown result type (might be due to invalid IL or missing references)
			//IL_286a: Unknown result type (might be due to invalid IL or missing references)
			//IL_287a: Unknown result type (might be due to invalid IL or missing references)
			//IL_287c: Unknown result type (might be due to invalid IL or missing references)
			//IL_2881: Unknown result type (might be due to invalid IL or missing references)
			//IL_2883: Unknown result type (might be due to invalid IL or missing references)
			//IL_2888: Unknown result type (might be due to invalid IL or missing references)
			//IL_2898: Unknown result type (might be due to invalid IL or missing references)
			//IL_289a: Unknown result type (might be due to invalid IL or missing references)
			//IL_289f: Unknown result type (might be due to invalid IL or missing references)
			//IL_28a1: Unknown result type (might be due to invalid IL or missing references)
			//IL_28a6: Unknown result type (might be due to invalid IL or missing references)
			//IL_202c: Unknown result type (might be due to invalid IL or missing references)
			//IL_2002: Unknown result type (might be due to invalid IL or missing references)
			//IL_2007: Unknown result type (might be due to invalid IL or missing references)
			//IL_2015: Unknown result type (might be due to invalid IL or missing references)
			//IL_201a: Unknown result type (might be due to invalid IL or missing references)
			//IL_1f71: Unknown result type (might be due to invalid IL or missing references)
			//IL_1f76: Unknown result type (might be due to invalid IL or missing references)
			//IL_0e4b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0e50: Unknown result type (might be due to invalid IL or missing references)
			//IL_034d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0352: Unknown result type (might be due to invalid IL or missing references)
			//IL_035a: Unknown result type (might be due to invalid IL or missing references)
			//IL_035f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0367: Unknown result type (might be due to invalid IL or missing references)
			//IL_036c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0374: Unknown result type (might be due to invalid IL or missing references)
			//IL_0379: Unknown result type (might be due to invalid IL or missing references)
			//IL_0381: Unknown result type (might be due to invalid IL or missing references)
			//IL_0386: Unknown result type (might be due to invalid IL or missing references)
			//IL_038e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0393: Unknown result type (might be due to invalid IL or missing references)
			//IL_039b: Unknown result type (might be due to invalid IL or missing references)
			//IL_03a0: Unknown result type (might be due to invalid IL or missing references)
			//IL_03a8: Unknown result type (might be due to invalid IL or missing references)
			//IL_03ad: Unknown result type (might be due to invalid IL or missing references)
			//IL_03b5: Unknown result type (might be due to invalid IL or missing references)
			//IL_03ba: Unknown result type (might be due to invalid IL or missing references)
			//IL_03c2: Unknown result type (might be due to invalid IL or missing references)
			//IL_03c7: Unknown result type (might be due to invalid IL or missing references)
			//IL_03cf: Unknown result type (might be due to invalid IL or missing references)
			//IL_03d4: Unknown result type (might be due to invalid IL or missing references)
			//IL_03dc: Unknown result type (might be due to invalid IL or missing references)
			//IL_03e1: Unknown result type (might be due to invalid IL or missing references)
			//IL_03e9: Unknown result type (might be due to invalid IL or missing references)
			//IL_03ee: Unknown result type (might be due to invalid IL or missing references)
			//IL_03f6: Unknown result type (might be due to invalid IL or missing references)
			//IL_03fb: Unknown result type (might be due to invalid IL or missing references)
			//IL_0403: Unknown result type (might be due to invalid IL or missing references)
			//IL_0408: Unknown result type (might be due to invalid IL or missing references)
			//IL_0410: Unknown result type (might be due to invalid IL or missing references)
			//IL_0415: Unknown result type (might be due to invalid IL or missing references)
			//IL_041d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0422: Unknown result type (might be due to invalid IL or missing references)
			//IL_042a: Unknown result type (might be due to invalid IL or missing references)
			//IL_042f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0437: Unknown result type (might be due to invalid IL or missing references)
			//IL_043c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0444: Unknown result type (might be due to invalid IL or missing references)
			//IL_0449: Unknown result type (might be due to invalid IL or missing references)
			//IL_0451: Unknown result type (might be due to invalid IL or missing references)
			//IL_0456: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b4: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b9: Unknown result type (might be due to invalid IL or missing references)
			//IL_01c1: Unknown result type (might be due to invalid IL or missing references)
			//IL_01c6: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ce: Unknown result type (might be due to invalid IL or missing references)
			//IL_01d3: Unknown result type (might be due to invalid IL or missing references)
			//IL_01db: Unknown result type (might be due to invalid IL or missing references)
			//IL_01e0: Unknown result type (might be due to invalid IL or missing references)
			//IL_01e8: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ed: Unknown result type (might be due to invalid IL or missing references)
			//IL_01f5: Unknown result type (might be due to invalid IL or missing references)
			//IL_01fa: Unknown result type (might be due to invalid IL or missing references)
			//IL_0202: Unknown result type (might be due to invalid IL or missing references)
			//IL_0207: Unknown result type (might be due to invalid IL or missing references)
			//IL_020f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0214: Unknown result type (might be due to invalid IL or missing references)
			//IL_021c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0221: Unknown result type (might be due to invalid IL or missing references)
			//IL_0229: Unknown result type (might be due to invalid IL or missing references)
			//IL_022e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0236: Unknown result type (might be due to invalid IL or missing references)
			//IL_023b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0244: Unknown result type (might be due to invalid IL or missing references)
			//IL_0249: Unknown result type (might be due to invalid IL or missing references)
			//IL_2993: Unknown result type (might be due to invalid IL or missing references)
			//IL_29a2: Unknown result type (might be due to invalid IL or missing references)
			//IL_29ab: Unknown result type (might be due to invalid IL or missing references)
			//IL_29ba: Unknown result type (might be due to invalid IL or missing references)
			//IL_29c3: Unknown result type (might be due to invalid IL or missing references)
			//IL_29d2: Unknown result type (might be due to invalid IL or missing references)
			//IL_29e2: Unknown result type (might be due to invalid IL or missing references)
			//IL_29f1: Unknown result type (might be due to invalid IL or missing references)
			//IL_29fa: Unknown result type (might be due to invalid IL or missing references)
			//IL_2a09: Unknown result type (might be due to invalid IL or missing references)
			//IL_2a12: Unknown result type (might be due to invalid IL or missing references)
			//IL_2a21: Unknown result type (might be due to invalid IL or missing references)
			//IL_28f0: Unknown result type (might be due to invalid IL or missing references)
			//IL_28ff: Unknown result type (might be due to invalid IL or missing references)
			//IL_2908: Unknown result type (might be due to invalid IL or missing references)
			//IL_2917: Unknown result type (might be due to invalid IL or missing references)
			//IL_2920: Unknown result type (might be due to invalid IL or missing references)
			//IL_292f: Unknown result type (might be due to invalid IL or missing references)
			//IL_293f: Unknown result type (might be due to invalid IL or missing references)
			//IL_294e: Unknown result type (might be due to invalid IL or missing references)
			//IL_2957: Unknown result type (might be due to invalid IL or missing references)
			//IL_2966: Unknown result type (might be due to invalid IL or missing references)
			//IL_296f: Unknown result type (might be due to invalid IL or missing references)
			//IL_297e: Unknown result type (might be due to invalid IL or missing references)
			//IL_2046: Unknown result type (might be due to invalid IL or missing references)
			//IL_1a25: Unknown result type (might be due to invalid IL or missing references)
			//IL_1b24: Unknown result type (might be due to invalid IL or missing references)
			//IL_0e6c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0474: Unknown result type (might be due to invalid IL or missing references)
			//IL_046b: Unknown result type (might be due to invalid IL or missing references)
			//IL_2a2d: Unknown result type (might be due to invalid IL or missing references)
			//IL_2a34: Unknown result type (might be due to invalid IL or missing references)
			//IL_2a39: Unknown result type (might be due to invalid IL or missing references)
			//IL_2060: Unknown result type (might be due to invalid IL or missing references)
			//IL_2077: Unknown result type (might be due to invalid IL or missing references)
			//IL_2084: Unknown result type (might be due to invalid IL or missing references)
			//IL_2095: Unknown result type (might be due to invalid IL or missing references)
			//IL_209a: Unknown result type (might be due to invalid IL or missing references)
			//IL_209f: Unknown result type (might be due to invalid IL or missing references)
			//IL_20a3: Unknown result type (might be due to invalid IL or missing references)
			//IL_20b4: Unknown result type (might be due to invalid IL or missing references)
			//IL_20b9: Unknown result type (might be due to invalid IL or missing references)
			//IL_20be: Unknown result type (might be due to invalid IL or missing references)
			//IL_20cc: Unknown result type (might be due to invalid IL or missing references)
			//IL_20dd: Unknown result type (might be due to invalid IL or missing references)
			//IL_20f6: Unknown result type (might be due to invalid IL or missing references)
			//IL_210e: Unknown result type (might be due to invalid IL or missing references)
			//IL_211f: Unknown result type (might be due to invalid IL or missing references)
			//IL_2121: Unknown result type (might be due to invalid IL or missing references)
			//IL_2123: Unknown result type (might be due to invalid IL or missing references)
			//IL_2128: Unknown result type (might be due to invalid IL or missing references)
			//IL_2138: Unknown result type (might be due to invalid IL or missing references)
			//IL_213a: Unknown result type (might be due to invalid IL or missing references)
			//IL_213c: Unknown result type (might be due to invalid IL or missing references)
			//IL_2141: Unknown result type (might be due to invalid IL or missing references)
			//IL_2152: Unknown result type (might be due to invalid IL or missing references)
			//IL_1abd: Unknown result type (might be due to invalid IL or missing references)
			//IL_1a3c: Unknown result type (might be due to invalid IL or missing references)
			//IL_1a4a: Unknown result type (might be due to invalid IL or missing references)
			//IL_1a51: Unknown result type (might be due to invalid IL or missing references)
			//IL_1a56: Unknown result type (might be due to invalid IL or missing references)
			//IL_1a5b: Unknown result type (might be due to invalid IL or missing references)
			//IL_1a63: Unknown result type (might be due to invalid IL or missing references)
			//IL_1a6c: Unknown result type (might be due to invalid IL or missing references)
			//IL_1a77: Unknown result type (might be due to invalid IL or missing references)
			//IL_1a79: Unknown result type (might be due to invalid IL or missing references)
			//IL_1a7e: Unknown result type (might be due to invalid IL or missing references)
			//IL_1a80: Unknown result type (might be due to invalid IL or missing references)
			//IL_1b4d: Unknown result type (might be due to invalid IL or missing references)
			//IL_1b52: Unknown result type (might be due to invalid IL or missing references)
			//IL_1b5c: Unknown result type (might be due to invalid IL or missing references)
			//IL_1078: Unknown result type (might be due to invalid IL or missing references)
			//IL_107d: Unknown result type (might be due to invalid IL or missing references)
			//IL_1085: Unknown result type (might be due to invalid IL or missing references)
			//IL_108a: Unknown result type (might be due to invalid IL or missing references)
			//IL_1092: Unknown result type (might be due to invalid IL or missing references)
			//IL_1097: Unknown result type (might be due to invalid IL or missing references)
			//IL_109f: Unknown result type (might be due to invalid IL or missing references)
			//IL_10a4: Unknown result type (might be due to invalid IL or missing references)
			//IL_10ac: Unknown result type (might be due to invalid IL or missing references)
			//IL_10b1: Unknown result type (might be due to invalid IL or missing references)
			//IL_10b9: Unknown result type (might be due to invalid IL or missing references)
			//IL_10be: Unknown result type (might be due to invalid IL or missing references)
			//IL_10c6: Unknown result type (might be due to invalid IL or missing references)
			//IL_10cb: Unknown result type (might be due to invalid IL or missing references)
			//IL_10d3: Unknown result type (might be due to invalid IL or missing references)
			//IL_10d8: Unknown result type (might be due to invalid IL or missing references)
			//IL_10e0: Unknown result type (might be due to invalid IL or missing references)
			//IL_10e5: Unknown result type (might be due to invalid IL or missing references)
			//IL_10ed: Unknown result type (might be due to invalid IL or missing references)
			//IL_10f2: Unknown result type (might be due to invalid IL or missing references)
			//IL_10fa: Unknown result type (might be due to invalid IL or missing references)
			//IL_10ff: Unknown result type (might be due to invalid IL or missing references)
			//IL_1107: Unknown result type (might be due to invalid IL or missing references)
			//IL_110c: Unknown result type (might be due to invalid IL or missing references)
			//IL_1114: Unknown result type (might be due to invalid IL or missing references)
			//IL_1119: Unknown result type (might be due to invalid IL or missing references)
			//IL_0f28: Unknown result type (might be due to invalid IL or missing references)
			//IL_0f2d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0f35: Unknown result type (might be due to invalid IL or missing references)
			//IL_0f3a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0f42: Unknown result type (might be due to invalid IL or missing references)
			//IL_0f47: Unknown result type (might be due to invalid IL or missing references)
			//IL_0f4f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0f54: Unknown result type (might be due to invalid IL or missing references)
			//IL_0f5c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0f61: Unknown result type (might be due to invalid IL or missing references)
			//IL_0f69: Unknown result type (might be due to invalid IL or missing references)
			//IL_0f6e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0f76: Unknown result type (might be due to invalid IL or missing references)
			//IL_0f7b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0f83: Unknown result type (might be due to invalid IL or missing references)
			//IL_0f88: Unknown result type (might be due to invalid IL or missing references)
			//IL_0f90: Unknown result type (might be due to invalid IL or missing references)
			//IL_0f95: Unknown result type (might be due to invalid IL or missing references)
			//IL_0f9e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0fa3: Unknown result type (might be due to invalid IL or missing references)
			//IL_0e85: Unknown result type (might be due to invalid IL or missing references)
			//IL_0e99: Unknown result type (might be due to invalid IL or missing references)
			//IL_0ea7: Unknown result type (might be due to invalid IL or missing references)
			//IL_0eae: Unknown result type (might be due to invalid IL or missing references)
			//IL_0eb8: Unknown result type (might be due to invalid IL or missing references)
			//IL_0ebd: Unknown result type (might be due to invalid IL or missing references)
			//IL_0ec1: Unknown result type (might be due to invalid IL or missing references)
			//IL_0ec6: Unknown result type (might be due to invalid IL or missing references)
			//IL_0479: Unknown result type (might be due to invalid IL or missing references)
			//IL_0480: Unknown result type (might be due to invalid IL or missing references)
			//IL_0481: Unknown result type (might be due to invalid IL or missing references)
			//IL_0504: Unknown result type (might be due to invalid IL or missing references)
			//IL_0506: Unknown result type (might be due to invalid IL or missing references)
			//IL_0523: Unknown result type (might be due to invalid IL or missing references)
			//IL_053f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0544: Unknown result type (might be due to invalid IL or missing references)
			//IL_02c4: Unknown result type (might be due to invalid IL or missing references)
			//IL_02fc: Unknown result type (might be due to invalid IL or missing references)
			//IL_0302: Unknown result type (might be due to invalid IL or missing references)
			//IL_2ae9: Unknown result type (might be due to invalid IL or missing references)
			//IL_2aeb: Unknown result type (might be due to invalid IL or missing references)
			//IL_2af0: Unknown result type (might be due to invalid IL or missing references)
			//IL_2af2: Unknown result type (might be due to invalid IL or missing references)
			//IL_2af7: Unknown result type (might be due to invalid IL or missing references)
			//IL_2b07: Unknown result type (might be due to invalid IL or missing references)
			//IL_2b09: Unknown result type (might be due to invalid IL or missing references)
			//IL_2b0e: Unknown result type (might be due to invalid IL or missing references)
			//IL_2b10: Unknown result type (might be due to invalid IL or missing references)
			//IL_2b15: Unknown result type (might be due to invalid IL or missing references)
			//IL_2b25: Unknown result type (might be due to invalid IL or missing references)
			//IL_2b27: Unknown result type (might be due to invalid IL or missing references)
			//IL_2b2c: Unknown result type (might be due to invalid IL or missing references)
			//IL_2b2e: Unknown result type (might be due to invalid IL or missing references)
			//IL_2b33: Unknown result type (might be due to invalid IL or missing references)
			//IL_2b43: Unknown result type (might be due to invalid IL or missing references)
			//IL_2b45: Unknown result type (might be due to invalid IL or missing references)
			//IL_2b4a: Unknown result type (might be due to invalid IL or missing references)
			//IL_2b4c: Unknown result type (might be due to invalid IL or missing references)
			//IL_2b51: Unknown result type (might be due to invalid IL or missing references)
			//IL_2b61: Unknown result type (might be due to invalid IL or missing references)
			//IL_2b63: Unknown result type (might be due to invalid IL or missing references)
			//IL_2b68: Unknown result type (might be due to invalid IL or missing references)
			//IL_2b6a: Unknown result type (might be due to invalid IL or missing references)
			//IL_2b6f: Unknown result type (might be due to invalid IL or missing references)
			//IL_2a4e: Unknown result type (might be due to invalid IL or missing references)
			//IL_2a50: Unknown result type (might be due to invalid IL or missing references)
			//IL_2a55: Unknown result type (might be due to invalid IL or missing references)
			//IL_2a57: Unknown result type (might be due to invalid IL or missing references)
			//IL_2a5c: Unknown result type (might be due to invalid IL or missing references)
			//IL_2a6c: Unknown result type (might be due to invalid IL or missing references)
			//IL_2a6e: Unknown result type (might be due to invalid IL or missing references)
			//IL_2a73: Unknown result type (might be due to invalid IL or missing references)
			//IL_2a75: Unknown result type (might be due to invalid IL or missing references)
			//IL_2a7a: Unknown result type (might be due to invalid IL or missing references)
			//IL_2a8a: Unknown result type (might be due to invalid IL or missing references)
			//IL_2a8c: Unknown result type (might be due to invalid IL or missing references)
			//IL_2a91: Unknown result type (might be due to invalid IL or missing references)
			//IL_2a93: Unknown result type (might be due to invalid IL or missing references)
			//IL_2a98: Unknown result type (might be due to invalid IL or missing references)
			//IL_2aa8: Unknown result type (might be due to invalid IL or missing references)
			//IL_2aaa: Unknown result type (might be due to invalid IL or missing references)
			//IL_2aaf: Unknown result type (might be due to invalid IL or missing references)
			//IL_2ab1: Unknown result type (might be due to invalid IL or missing references)
			//IL_2ab6: Unknown result type (might be due to invalid IL or missing references)
			//IL_2ac6: Unknown result type (might be due to invalid IL or missing references)
			//IL_2ac8: Unknown result type (might be due to invalid IL or missing references)
			//IL_2acd: Unknown result type (might be due to invalid IL or missing references)
			//IL_2acf: Unknown result type (might be due to invalid IL or missing references)
			//IL_2ad4: Unknown result type (might be due to invalid IL or missing references)
			//IL_1ad1: Unknown result type (might be due to invalid IL or missing references)
			//IL_1adb: Unknown result type (might be due to invalid IL or missing references)
			//IL_1ae0: Unknown result type (might be due to invalid IL or missing references)
			//IL_1ae2: Unknown result type (might be due to invalid IL or missing references)
			//IL_1ae4: Unknown result type (might be due to invalid IL or missing references)
			//IL_1ae6: Unknown result type (might be due to invalid IL or missing references)
			//IL_1aaa: Unknown result type (might be due to invalid IL or missing references)
			//IL_1aac: Unknown result type (might be due to invalid IL or missing references)
			//IL_1ab1: Unknown result type (might be due to invalid IL or missing references)
			//IL_1a97: Unknown result type (might be due to invalid IL or missing references)
			//IL_1a99: Unknown result type (might be due to invalid IL or missing references)
			//IL_1a9b: Unknown result type (might be due to invalid IL or missing references)
			//IL_1aa0: Unknown result type (might be due to invalid IL or missing references)
			//IL_1b73: Unknown result type (might be due to invalid IL or missing references)
			//IL_1b85: Unknown result type (might be due to invalid IL or missing references)
			//IL_1b99: Unknown result type (might be due to invalid IL or missing references)
			//IL_1ba5: Unknown result type (might be due to invalid IL or missing references)
			//IL_1bac: Unknown result type (might be due to invalid IL or missing references)
			//IL_1bb6: Unknown result type (might be due to invalid IL or missing references)
			//IL_1bbb: Unknown result type (might be due to invalid IL or missing references)
			//IL_1bbf: Unknown result type (might be due to invalid IL or missing references)
			//IL_1bc4: Unknown result type (might be due to invalid IL or missing references)
			//IL_1138: Unknown result type (might be due to invalid IL or missing references)
			//IL_112f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0ed4: Unknown result type (might be due to invalid IL or missing references)
			//IL_0ed9: Unknown result type (might be due to invalid IL or missing references)
			//IL_0edb: Unknown result type (might be due to invalid IL or missing references)
			//IL_0ee9: Unknown result type (might be due to invalid IL or missing references)
			//IL_0f02: Unknown result type (might be due to invalid IL or missing references)
			//IL_0f07: Unknown result type (might be due to invalid IL or missing references)
			//IL_055c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0568: Unknown result type (might be due to invalid IL or missing references)
			//IL_1b0b: Unknown result type (might be due to invalid IL or missing references)
			//IL_1b0d: Unknown result type (might be due to invalid IL or missing references)
			//IL_1afd: Unknown result type (might be due to invalid IL or missing references)
			//IL_1aff: Unknown result type (might be due to invalid IL or missing references)
			//IL_1b01: Unknown result type (might be due to invalid IL or missing references)
			//IL_1c35: Unknown result type (might be due to invalid IL or missing references)
			//IL_1c3a: Unknown result type (might be due to invalid IL or missing references)
			//IL_1bd2: Unknown result type (might be due to invalid IL or missing references)
			//IL_1bd7: Unknown result type (might be due to invalid IL or missing references)
			//IL_1bd9: Unknown result type (might be due to invalid IL or missing references)
			//IL_1be7: Unknown result type (might be due to invalid IL or missing references)
			//IL_1c00: Unknown result type (might be due to invalid IL or missing references)
			//IL_1c05: Unknown result type (might be due to invalid IL or missing references)
			//IL_113d: Unknown result type (might be due to invalid IL or missing references)
			//IL_118a: Unknown result type (might be due to invalid IL or missing references)
			//IL_118c: Unknown result type (might be due to invalid IL or missing references)
			//IL_1193: Unknown result type (might be due to invalid IL or missing references)
			//IL_11ad: Unknown result type (might be due to invalid IL or missing references)
			//IL_11af: Unknown result type (might be due to invalid IL or missing references)
			//IL_1027: Unknown result type (might be due to invalid IL or missing references)
			//IL_102d: Unknown result type (might be due to invalid IL or missing references)
			//IL_09ca: Unknown result type (might be due to invalid IL or missing references)
			//IL_09cf: Unknown result type (might be due to invalid IL or missing references)
			//IL_0597: Unknown result type (might be due to invalid IL or missing references)
			//IL_05ad: Unknown result type (might be due to invalid IL or missing references)
			//IL_0321: Unknown result type (might be due to invalid IL or missing references)
			//IL_0327: Unknown result type (might be due to invalid IL or missing references)
			//IL_2174: Unknown result type (might be due to invalid IL or missing references)
			//IL_2179: Unknown result type (might be due to invalid IL or missing references)
			//IL_2181: Unknown result type (might be due to invalid IL or missing references)
			//IL_2192: Unknown result type (might be due to invalid IL or missing references)
			//IL_2201: Unknown result type (might be due to invalid IL or missing references)
			//IL_2212: Unknown result type (might be due to invalid IL or missing references)
			//IL_2217: Unknown result type (might be due to invalid IL or missing references)
			//IL_222d: Unknown result type (might be due to invalid IL or missing references)
			//IL_2232: Unknown result type (might be due to invalid IL or missing references)
			//IL_223a: Unknown result type (might be due to invalid IL or missing references)
			//IL_223f: Unknown result type (might be due to invalid IL or missing references)
			//IL_2247: Unknown result type (might be due to invalid IL or missing references)
			//IL_224c: Unknown result type (might be due to invalid IL or missing references)
			//IL_2254: Unknown result type (might be due to invalid IL or missing references)
			//IL_2259: Unknown result type (might be due to invalid IL or missing references)
			//IL_2261: Unknown result type (might be due to invalid IL or missing references)
			//IL_2266: Unknown result type (might be due to invalid IL or missing references)
			//IL_226e: Unknown result type (might be due to invalid IL or missing references)
			//IL_2273: Unknown result type (might be due to invalid IL or missing references)
			//IL_227b: Unknown result type (might be due to invalid IL or missing references)
			//IL_2280: Unknown result type (might be due to invalid IL or missing references)
			//IL_2288: Unknown result type (might be due to invalid IL or missing references)
			//IL_228d: Unknown result type (might be due to invalid IL or missing references)
			//IL_2295: Unknown result type (might be due to invalid IL or missing references)
			//IL_229a: Unknown result type (might be due to invalid IL or missing references)
			//IL_22a2: Unknown result type (might be due to invalid IL or missing references)
			//IL_22a7: Unknown result type (might be due to invalid IL or missing references)
			//IL_22af: Unknown result type (might be due to invalid IL or missing references)
			//IL_22b4: Unknown result type (might be due to invalid IL or missing references)
			//IL_22bc: Unknown result type (might be due to invalid IL or missing references)
			//IL_22c1: Unknown result type (might be due to invalid IL or missing references)
			//IL_22c9: Unknown result type (might be due to invalid IL or missing references)
			//IL_22ce: Unknown result type (might be due to invalid IL or missing references)
			//IL_22d6: Unknown result type (might be due to invalid IL or missing references)
			//IL_22db: Unknown result type (might be due to invalid IL or missing references)
			//IL_22e3: Unknown result type (might be due to invalid IL or missing references)
			//IL_22e8: Unknown result type (might be due to invalid IL or missing references)
			//IL_22f0: Unknown result type (might be due to invalid IL or missing references)
			//IL_22f5: Unknown result type (might be due to invalid IL or missing references)
			//IL_22fd: Unknown result type (might be due to invalid IL or missing references)
			//IL_2302: Unknown result type (might be due to invalid IL or missing references)
			//IL_233a: Unknown result type (might be due to invalid IL or missing references)
			//IL_233c: Unknown result type (might be due to invalid IL or missing references)
			//IL_2343: Unknown result type (might be due to invalid IL or missing references)
			//IL_2345: Unknown result type (might be due to invalid IL or missing references)
			//IL_2364: Unknown result type (might be due to invalid IL or missing references)
			//IL_2372: Unknown result type (might be due to invalid IL or missing references)
			//IL_2374: Unknown result type (might be due to invalid IL or missing references)
			//IL_237d: Unknown result type (might be due to invalid IL or missing references)
			//IL_237f: Unknown result type (might be due to invalid IL or missing references)
			//IL_2384: Unknown result type (might be due to invalid IL or missing references)
			//IL_1c47: Unknown result type (might be due to invalid IL or missing references)
			//IL_1c4c: Unknown result type (might be due to invalid IL or missing references)
			//IL_1c54: Unknown result type (might be due to invalid IL or missing references)
			//IL_1c59: Unknown result type (might be due to invalid IL or missing references)
			//IL_1c61: Unknown result type (might be due to invalid IL or missing references)
			//IL_1c66: Unknown result type (might be due to invalid IL or missing references)
			//IL_1c6e: Unknown result type (might be due to invalid IL or missing references)
			//IL_1c73: Unknown result type (might be due to invalid IL or missing references)
			//IL_1c7b: Unknown result type (might be due to invalid IL or missing references)
			//IL_1c80: Unknown result type (might be due to invalid IL or missing references)
			//IL_1c88: Unknown result type (might be due to invalid IL or missing references)
			//IL_1c8d: Unknown result type (might be due to invalid IL or missing references)
			//IL_1c95: Unknown result type (might be due to invalid IL or missing references)
			//IL_1c9a: Unknown result type (might be due to invalid IL or missing references)
			//IL_1ca2: Unknown result type (might be due to invalid IL or missing references)
			//IL_1ca7: Unknown result type (might be due to invalid IL or missing references)
			//IL_1caf: Unknown result type (might be due to invalid IL or missing references)
			//IL_1cb4: Unknown result type (might be due to invalid IL or missing references)
			//IL_1cbc: Unknown result type (might be due to invalid IL or missing references)
			//IL_1cc1: Unknown result type (might be due to invalid IL or missing references)
			//IL_1cc9: Unknown result type (might be due to invalid IL or missing references)
			//IL_1cce: Unknown result type (might be due to invalid IL or missing references)
			//IL_1cd6: Unknown result type (might be due to invalid IL or missing references)
			//IL_1cdb: Unknown result type (might be due to invalid IL or missing references)
			//IL_11c7: Unknown result type (might be due to invalid IL or missing references)
			//IL_11d4: Unknown result type (might be due to invalid IL or missing references)
			//IL_09e3: Unknown result type (might be due to invalid IL or missing references)
			//IL_09f7: Unknown result type (might be due to invalid IL or missing references)
			//IL_0a14: Unknown result type (might be due to invalid IL or missing references)
			//IL_0a19: Unknown result type (might be due to invalid IL or missing references)
			//IL_0a20: Unknown result type (might be due to invalid IL or missing references)
			//IL_0a25: Unknown result type (might be due to invalid IL or missing references)
			//IL_0997: Unknown result type (might be due to invalid IL or missing references)
			//IL_099e: Unknown result type (might be due to invalid IL or missing references)
			//IL_09a3: Unknown result type (might be due to invalid IL or missing references)
			//IL_09af: Unknown result type (might be due to invalid IL or missing references)
			//IL_05c3: Unknown result type (might be due to invalid IL or missing references)
			//IL_05cd: Unknown result type (might be due to invalid IL or missing references)
			//IL_05d2: Unknown result type (might be due to invalid IL or missing references)
			//IL_05db: Unknown result type (might be due to invalid IL or missing references)
			//IL_05ed: Unknown result type (might be due to invalid IL or missing references)
			//IL_05ef: Unknown result type (might be due to invalid IL or missing references)
			//IL_05f4: Unknown result type (might be due to invalid IL or missing references)
			//IL_05f8: Unknown result type (might be due to invalid IL or missing references)
			//IL_0604: Unknown result type (might be due to invalid IL or missing references)
			//IL_1cfa: Unknown result type (might be due to invalid IL or missing references)
			//IL_1cf1: Unknown result type (might be due to invalid IL or missing references)
			//IL_163b: Unknown result type (might be due to invalid IL or missing references)
			//IL_1640: Unknown result type (might be due to invalid IL or missing references)
			//IL_1204: Unknown result type (might be due to invalid IL or missing references)
			//IL_121b: Unknown result type (might be due to invalid IL or missing references)
			//IL_104c: Unknown result type (might be due to invalid IL or missing references)
			//IL_1052: Unknown result type (might be due to invalid IL or missing references)
			//IL_0a49: Unknown result type (might be due to invalid IL or missing references)
			//IL_0a4e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0a3a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0a3f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0a41: Unknown result type (might be due to invalid IL or missing references)
			//IL_0a46: Unknown result type (might be due to invalid IL or missing references)
			//IL_06ca: Unknown result type (might be due to invalid IL or missing references)
			//IL_06d4: Unknown result type (might be due to invalid IL or missing references)
			//IL_06e0: Unknown result type (might be due to invalid IL or missing references)
			//IL_06ea: Unknown result type (might be due to invalid IL or missing references)
			//IL_06f4: Unknown result type (might be due to invalid IL or missing references)
			//IL_0619: Unknown result type (might be due to invalid IL or missing references)
			//IL_0628: Unknown result type (might be due to invalid IL or missing references)
			//IL_0631: Unknown result type (might be due to invalid IL or missing references)
			//IL_0640: Unknown result type (might be due to invalid IL or missing references)
			//IL_0649: Unknown result type (might be due to invalid IL or missing references)
			//IL_0658: Unknown result type (might be due to invalid IL or missing references)
			//IL_0668: Unknown result type (might be due to invalid IL or missing references)
			//IL_066a: Unknown result type (might be due to invalid IL or missing references)
			//IL_066f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0671: Unknown result type (might be due to invalid IL or missing references)
			//IL_0676: Unknown result type (might be due to invalid IL or missing references)
			//IL_0686: Unknown result type (might be due to invalid IL or missing references)
			//IL_0688: Unknown result type (might be due to invalid IL or missing references)
			//IL_068d: Unknown result type (might be due to invalid IL or missing references)
			//IL_068f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0694: Unknown result type (might be due to invalid IL or missing references)
			//IL_06a4: Unknown result type (might be due to invalid IL or missing references)
			//IL_06a6: Unknown result type (might be due to invalid IL or missing references)
			//IL_06ab: Unknown result type (might be due to invalid IL or missing references)
			//IL_06ad: Unknown result type (might be due to invalid IL or missing references)
			//IL_06b2: Unknown result type (might be due to invalid IL or missing references)
			//IL_23a1: Unknown result type (might be due to invalid IL or missing references)
			//IL_1cff: Unknown result type (might be due to invalid IL or missing references)
			//IL_1d37: Unknown result type (might be due to invalid IL or missing references)
			//IL_1d39: Unknown result type (might be due to invalid IL or missing references)
			//IL_1d40: Unknown result type (might be due to invalid IL or missing references)
			//IL_1d5a: Unknown result type (might be due to invalid IL or missing references)
			//IL_1d5c: Unknown result type (might be due to invalid IL or missing references)
			//IL_164f: Unknown result type (might be due to invalid IL or missing references)
			//IL_1654: Unknown result type (might be due to invalid IL or missing references)
			//IL_1607: Unknown result type (might be due to invalid IL or missing references)
			//IL_160e: Unknown result type (might be due to invalid IL or missing references)
			//IL_1613: Unknown result type (might be due to invalid IL or missing references)
			//IL_161f: Unknown result type (might be due to invalid IL or missing references)
			//IL_1232: Unknown result type (might be due to invalid IL or missing references)
			//IL_123c: Unknown result type (might be due to invalid IL or missing references)
			//IL_1241: Unknown result type (might be due to invalid IL or missing references)
			//IL_124b: Unknown result type (might be due to invalid IL or missing references)
			//IL_125d: Unknown result type (might be due to invalid IL or missing references)
			//IL_125f: Unknown result type (might be due to invalid IL or missing references)
			//IL_1264: Unknown result type (might be due to invalid IL or missing references)
			//IL_1268: Unknown result type (might be due to invalid IL or missing references)
			//IL_1274: Unknown result type (might be due to invalid IL or missing references)
			//IL_0ad5: Unknown result type (might be due to invalid IL or missing references)
			//IL_0adb: Unknown result type (might be due to invalid IL or missing references)
			//IL_0ae0: Unknown result type (might be due to invalid IL or missing references)
			//IL_0ae2: Unknown result type (might be due to invalid IL or missing references)
			//IL_0a61: Unknown result type (might be due to invalid IL or missing references)
			//IL_0a75: Unknown result type (might be due to invalid IL or missing references)
			//IL_0a96: Unknown result type (might be due to invalid IL or missing references)
			//IL_0a9c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0aa2: Unknown result type (might be due to invalid IL or missing references)
			//IL_0aa7: Unknown result type (might be due to invalid IL or missing references)
			//IL_0aa9: Unknown result type (might be due to invalid IL or missing references)
			//IL_07a9: Unknown result type (might be due to invalid IL or missing references)
			//IL_07b8: Unknown result type (might be due to invalid IL or missing references)
			//IL_07c1: Unknown result type (might be due to invalid IL or missing references)
			//IL_07d0: Unknown result type (might be due to invalid IL or missing references)
			//IL_07d9: Unknown result type (might be due to invalid IL or missing references)
			//IL_07e8: Unknown result type (might be due to invalid IL or missing references)
			//IL_07f8: Unknown result type (might be due to invalid IL or missing references)
			//IL_0807: Unknown result type (might be due to invalid IL or missing references)
			//IL_0810: Unknown result type (might be due to invalid IL or missing references)
			//IL_081f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0828: Unknown result type (might be due to invalid IL or missing references)
			//IL_0837: Unknown result type (might be due to invalid IL or missing references)
			//IL_0706: Unknown result type (might be due to invalid IL or missing references)
			//IL_0715: Unknown result type (might be due to invalid IL or missing references)
			//IL_071e: Unknown result type (might be due to invalid IL or missing references)
			//IL_072d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0736: Unknown result type (might be due to invalid IL or missing references)
			//IL_0745: Unknown result type (might be due to invalid IL or missing references)
			//IL_0755: Unknown result type (might be due to invalid IL or missing references)
			//IL_0764: Unknown result type (might be due to invalid IL or missing references)
			//IL_076d: Unknown result type (might be due to invalid IL or missing references)
			//IL_077c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0785: Unknown result type (might be due to invalid IL or missing references)
			//IL_0794: Unknown result type (might be due to invalid IL or missing references)
			//IL_23b8: Unknown result type (might be due to invalid IL or missing references)
			//IL_23c3: Unknown result type (might be due to invalid IL or missing references)
			//IL_23ca: Unknown result type (might be due to invalid IL or missing references)
			//IL_23cf: Unknown result type (might be due to invalid IL or missing references)
			//IL_23d4: Unknown result type (might be due to invalid IL or missing references)
			//IL_23dd: Unknown result type (might be due to invalid IL or missing references)
			//IL_23e9: Unknown result type (might be due to invalid IL or missing references)
			//IL_2402: Unknown result type (might be due to invalid IL or missing references)
			//IL_2406: Unknown result type (might be due to invalid IL or missing references)
			//IL_1d9d: Unknown result type (might be due to invalid IL or missing references)
			//IL_1da2: Unknown result type (might be due to invalid IL or missing references)
			//IL_1d74: Unknown result type (might be due to invalid IL or missing references)
			//IL_1d81: Unknown result type (might be due to invalid IL or missing references)
			//IL_168e: Unknown result type (might be due to invalid IL or missing references)
			//IL_1695: Unknown result type (might be due to invalid IL or missing references)
			//IL_1664: Unknown result type (might be due to invalid IL or missing references)
			//IL_166b: Unknown result type (might be due to invalid IL or missing references)
			//IL_1672: Unknown result type (might be due to invalid IL or missing references)
			//IL_133a: Unknown result type (might be due to invalid IL or missing references)
			//IL_1344: Unknown result type (might be due to invalid IL or missing references)
			//IL_1350: Unknown result type (might be due to invalid IL or missing references)
			//IL_135a: Unknown result type (might be due to invalid IL or missing references)
			//IL_1364: Unknown result type (might be due to invalid IL or missing references)
			//IL_1289: Unknown result type (might be due to invalid IL or missing references)
			//IL_1298: Unknown result type (might be due to invalid IL or missing references)
			//IL_12a1: Unknown result type (might be due to invalid IL or missing references)
			//IL_12b0: Unknown result type (might be due to invalid IL or missing references)
			//IL_12b9: Unknown result type (might be due to invalid IL or missing references)
			//IL_12c8: Unknown result type (might be due to invalid IL or missing references)
			//IL_12d8: Unknown result type (might be due to invalid IL or missing references)
			//IL_12da: Unknown result type (might be due to invalid IL or missing references)
			//IL_12df: Unknown result type (might be due to invalid IL or missing references)
			//IL_12e1: Unknown result type (might be due to invalid IL or missing references)
			//IL_12e6: Unknown result type (might be due to invalid IL or missing references)
			//IL_12f6: Unknown result type (might be due to invalid IL or missing references)
			//IL_12f8: Unknown result type (might be due to invalid IL or missing references)
			//IL_12fd: Unknown result type (might be due to invalid IL or missing references)
			//IL_12ff: Unknown result type (might be due to invalid IL or missing references)
			//IL_1304: Unknown result type (might be due to invalid IL or missing references)
			//IL_1314: Unknown result type (might be due to invalid IL or missing references)
			//IL_1316: Unknown result type (might be due to invalid IL or missing references)
			//IL_131b: Unknown result type (might be due to invalid IL or missing references)
			//IL_131d: Unknown result type (might be due to invalid IL or missing references)
			//IL_1322: Unknown result type (might be due to invalid IL or missing references)
			//IL_0c8a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0c8f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0843: Unknown result type (might be due to invalid IL or missing references)
			//IL_084a: Unknown result type (might be due to invalid IL or missing references)
			//IL_084f: Unknown result type (might be due to invalid IL or missing references)
			//IL_2448: Unknown result type (might be due to invalid IL or missing references)
			//IL_244a: Unknown result type (might be due to invalid IL or missing references)
			//IL_244f: Unknown result type (might be due to invalid IL or missing references)
			//IL_2415: Unknown result type (might be due to invalid IL or missing references)
			//IL_2417: Unknown result type (might be due to invalid IL or missing references)
			//IL_241c: Unknown result type (might be due to invalid IL or missing references)
			//IL_241e: Unknown result type (might be due to invalid IL or missing references)
			//IL_247e: Unknown result type (might be due to invalid IL or missing references)
			//IL_2483: Unknown result type (might be due to invalid IL or missing references)
			//IL_248b: Unknown result type (might be due to invalid IL or missing references)
			//IL_249a: Unknown result type (might be due to invalid IL or missing references)
			//IL_24ab: Unknown result type (might be due to invalid IL or missing references)
			//IL_24c9: Unknown result type (might be due to invalid IL or missing references)
			//IL_24de: Unknown result type (might be due to invalid IL or missing references)
			//IL_24f3: Unknown result type (might be due to invalid IL or missing references)
			//IL_2508: Unknown result type (might be due to invalid IL or missing references)
			//IL_1db3: Unknown result type (might be due to invalid IL or missing references)
			//IL_1dba: Unknown result type (might be due to invalid IL or missing references)
			//IL_17e2: Unknown result type (might be due to invalid IL or missing references)
			//IL_17e7: Unknown result type (might be due to invalid IL or missing references)
			//IL_1419: Unknown result type (might be due to invalid IL or missing references)
			//IL_1428: Unknown result type (might be due to invalid IL or missing references)
			//IL_1431: Unknown result type (might be due to invalid IL or missing references)
			//IL_1440: Unknown result type (might be due to invalid IL or missing references)
			//IL_1449: Unknown result type (might be due to invalid IL or missing references)
			//IL_1458: Unknown result type (might be due to invalid IL or missing references)
			//IL_1468: Unknown result type (might be due to invalid IL or missing references)
			//IL_1477: Unknown result type (might be due to invalid IL or missing references)
			//IL_1480: Unknown result type (might be due to invalid IL or missing references)
			//IL_148f: Unknown result type (might be due to invalid IL or missing references)
			//IL_1498: Unknown result type (might be due to invalid IL or missing references)
			//IL_14a7: Unknown result type (might be due to invalid IL or missing references)
			//IL_1376: Unknown result type (might be due to invalid IL or missing references)
			//IL_1385: Unknown result type (might be due to invalid IL or missing references)
			//IL_138e: Unknown result type (might be due to invalid IL or missing references)
			//IL_139d: Unknown result type (might be due to invalid IL or missing references)
			//IL_13a6: Unknown result type (might be due to invalid IL or missing references)
			//IL_13b5: Unknown result type (might be due to invalid IL or missing references)
			//IL_13c5: Unknown result type (might be due to invalid IL or missing references)
			//IL_13d4: Unknown result type (might be due to invalid IL or missing references)
			//IL_13dd: Unknown result type (might be due to invalid IL or missing references)
			//IL_13ec: Unknown result type (might be due to invalid IL or missing references)
			//IL_13f5: Unknown result type (might be due to invalid IL or missing references)
			//IL_1404: Unknown result type (might be due to invalid IL or missing references)
			//IL_0c9e: Unknown result type (might be due to invalid IL or missing references)
			//IL_08ff: Unknown result type (might be due to invalid IL or missing references)
			//IL_0901: Unknown result type (might be due to invalid IL or missing references)
			//IL_0906: Unknown result type (might be due to invalid IL or missing references)
			//IL_0908: Unknown result type (might be due to invalid IL or missing references)
			//IL_090d: Unknown result type (might be due to invalid IL or missing references)
			//IL_091d: Unknown result type (might be due to invalid IL or missing references)
			//IL_091f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0924: Unknown result type (might be due to invalid IL or missing references)
			//IL_0926: Unknown result type (might be due to invalid IL or missing references)
			//IL_092b: Unknown result type (might be due to invalid IL or missing references)
			//IL_093b: Unknown result type (might be due to invalid IL or missing references)
			//IL_093d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0942: Unknown result type (might be due to invalid IL or missing references)
			//IL_0944: Unknown result type (might be due to invalid IL or missing references)
			//IL_0949: Unknown result type (might be due to invalid IL or missing references)
			//IL_0959: Unknown result type (might be due to invalid IL or missing references)
			//IL_095b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0960: Unknown result type (might be due to invalid IL or missing references)
			//IL_0962: Unknown result type (might be due to invalid IL or missing references)
			//IL_0967: Unknown result type (might be due to invalid IL or missing references)
			//IL_0977: Unknown result type (might be due to invalid IL or missing references)
			//IL_0979: Unknown result type (might be due to invalid IL or missing references)
			//IL_097e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0980: Unknown result type (might be due to invalid IL or missing references)
			//IL_0985: Unknown result type (might be due to invalid IL or missing references)
			//IL_0864: Unknown result type (might be due to invalid IL or missing references)
			//IL_0866: Unknown result type (might be due to invalid IL or missing references)
			//IL_086b: Unknown result type (might be due to invalid IL or missing references)
			//IL_086d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0872: Unknown result type (might be due to invalid IL or missing references)
			//IL_0882: Unknown result type (might be due to invalid IL or missing references)
			//IL_0884: Unknown result type (might be due to invalid IL or missing references)
			//IL_0889: Unknown result type (might be due to invalid IL or missing references)
			//IL_088b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0890: Unknown result type (might be due to invalid IL or missing references)
			//IL_08a0: Unknown result type (might be due to invalid IL or missing references)
			//IL_08a2: Unknown result type (might be due to invalid IL or missing references)
			//IL_08a7: Unknown result type (might be due to invalid IL or missing references)
			//IL_08a9: Unknown result type (might be due to invalid IL or missing references)
			//IL_08ae: Unknown result type (might be due to invalid IL or missing references)
			//IL_08be: Unknown result type (might be due to invalid IL or missing references)
			//IL_08c0: Unknown result type (might be due to invalid IL or missing references)
			//IL_08c5: Unknown result type (might be due to invalid IL or missing references)
			//IL_08c7: Unknown result type (might be due to invalid IL or missing references)
			//IL_08cc: Unknown result type (might be due to invalid IL or missing references)
			//IL_08dc: Unknown result type (might be due to invalid IL or missing references)
			//IL_08de: Unknown result type (might be due to invalid IL or missing references)
			//IL_08e3: Unknown result type (might be due to invalid IL or missing references)
			//IL_08e5: Unknown result type (might be due to invalid IL or missing references)
			//IL_08ea: Unknown result type (might be due to invalid IL or missing references)
			//IL_2435: Unknown result type (might be due to invalid IL or missing references)
			//IL_2437: Unknown result type (might be due to invalid IL or missing references)
			//IL_2439: Unknown result type (might be due to invalid IL or missing references)
			//IL_243e: Unknown result type (might be due to invalid IL or missing references)
			//IL_2580: Unknown result type (might be due to invalid IL or missing references)
			//IL_2595: Unknown result type (might be due to invalid IL or missing references)
			//IL_25aa: Unknown result type (might be due to invalid IL or missing references)
			//IL_25bf: Unknown result type (might be due to invalid IL or missing references)
			//IL_25cb: Unknown result type (might be due to invalid IL or missing references)
			//IL_1ee5: Unknown result type (might be due to invalid IL or missing references)
			//IL_1eea: Unknown result type (might be due to invalid IL or missing references)
			//IL_17f6: Unknown result type (might be due to invalid IL or missing references)
			//IL_14b3: Unknown result type (might be due to invalid IL or missing references)
			//IL_14ba: Unknown result type (might be due to invalid IL or missing references)
			//IL_14bf: Unknown result type (might be due to invalid IL or missing references)
			//IL_0cc7: Unknown result type (might be due to invalid IL or missing references)
			//IL_0ccc: Unknown result type (might be due to invalid IL or missing references)
			//IL_0c7e: Unknown result type (might be due to invalid IL or missing references)
			//IL_273f: Unknown result type (might be due to invalid IL or missing references)
			//IL_2744: Unknown result type (might be due to invalid IL or missing references)
			//IL_1ef9: Unknown result type (might be due to invalid IL or missing references)
			//IL_17d6: Unknown result type (might be due to invalid IL or missing references)
			//IL_156f: Unknown result type (might be due to invalid IL or missing references)
			//IL_1571: Unknown result type (might be due to invalid IL or missing references)
			//IL_1576: Unknown result type (might be due to invalid IL or missing references)
			//IL_1578: Unknown result type (might be due to invalid IL or missing references)
			//IL_157d: Unknown result type (might be due to invalid IL or missing references)
			//IL_158d: Unknown result type (might be due to invalid IL or missing references)
			//IL_158f: Unknown result type (might be due to invalid IL or missing references)
			//IL_1594: Unknown result type (might be due to invalid IL or missing references)
			//IL_1596: Unknown result type (might be due to invalid IL or missing references)
			//IL_159b: Unknown result type (might be due to invalid IL or missing references)
			//IL_15ab: Unknown result type (might be due to invalid IL or missing references)
			//IL_15ad: Unknown result type (might be due to invalid IL or missing references)
			//IL_15b2: Unknown result type (might be due to invalid IL or missing references)
			//IL_15b4: Unknown result type (might be due to invalid IL or missing references)
			//IL_15b9: Unknown result type (might be due to invalid IL or missing references)
			//IL_15c9: Unknown result type (might be due to invalid IL or missing references)
			//IL_15cb: Unknown result type (might be due to invalid IL or missing references)
			//IL_15d0: Unknown result type (might be due to invalid IL or missing references)
			//IL_15d2: Unknown result type (might be due to invalid IL or missing references)
			//IL_15d7: Unknown result type (might be due to invalid IL or missing references)
			//IL_15e7: Unknown result type (might be due to invalid IL or missing references)
			//IL_15e9: Unknown result type (might be due to invalid IL or missing references)
			//IL_15ee: Unknown result type (might be due to invalid IL or missing references)
			//IL_15f0: Unknown result type (might be due to invalid IL or missing references)
			//IL_15f5: Unknown result type (might be due to invalid IL or missing references)
			//IL_14d4: Unknown result type (might be due to invalid IL or missing references)
			//IL_14d6: Unknown result type (might be due to invalid IL or missing references)
			//IL_14db: Unknown result type (might be due to invalid IL or missing references)
			//IL_14dd: Unknown result type (might be due to invalid IL or missing references)
			//IL_14e2: Unknown result type (might be due to invalid IL or missing references)
			//IL_14f2: Unknown result type (might be due to invalid IL or missing references)
			//IL_14f4: Unknown result type (might be due to invalid IL or missing references)
			//IL_14f9: Unknown result type (might be due to invalid IL or missing references)
			//IL_14fb: Unknown result type (might be due to invalid IL or missing references)
			//IL_1500: Unknown result type (might be due to invalid IL or missing references)
			//IL_1510: Unknown result type (might be due to invalid IL or missing references)
			//IL_1512: Unknown result type (might be due to invalid IL or missing references)
			//IL_1517: Unknown result type (might be due to invalid IL or missing references)
			//IL_1519: Unknown result type (might be due to invalid IL or missing references)
			//IL_151e: Unknown result type (might be due to invalid IL or missing references)
			//IL_152e: Unknown result type (might be due to invalid IL or missing references)
			//IL_1530: Unknown result type (might be due to invalid IL or missing references)
			//IL_1535: Unknown result type (might be due to invalid IL or missing references)
			//IL_1537: Unknown result type (might be due to invalid IL or missing references)
			//IL_153c: Unknown result type (might be due to invalid IL or missing references)
			//IL_154c: Unknown result type (might be due to invalid IL or missing references)
			//IL_154e: Unknown result type (might be due to invalid IL or missing references)
			//IL_1553: Unknown result type (might be due to invalid IL or missing references)
			//IL_1555: Unknown result type (might be due to invalid IL or missing references)
			//IL_155a: Unknown result type (might be due to invalid IL or missing references)
			//IL_2753: Unknown result type (might be due to invalid IL or missing references)
			//IL_1ed9: Unknown result type (might be due to invalid IL or missing references)
			//IL_16cf: Unknown result type (might be due to invalid IL or missing references)
			//IL_16d5: Unknown result type (might be due to invalid IL or missing references)
			//IL_16db: Unknown result type (might be due to invalid IL or missing references)
			//IL_16e1: Unknown result type (might be due to invalid IL or missing references)
			//IL_16e7: Unknown result type (might be due to invalid IL or missing references)
			//IL_0b74: Unknown result type (might be due to invalid IL or missing references)
			//IL_0b79: Unknown result type (might be due to invalid IL or missing references)
			//IL_1df0: Unknown result type (might be due to invalid IL or missing references)
			//IL_1df6: Unknown result type (might be due to invalid IL or missing references)
			//IL_1dfc: Unknown result type (might be due to invalid IL or missing references)
			//IL_1e02: Unknown result type (might be due to invalid IL or missing references)
			//IL_1e08: Unknown result type (might be due to invalid IL or missing references)
			//IL_1745: Unknown result type (might be due to invalid IL or missing references)
			//IL_174a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0bb3: Unknown result type (might be due to invalid IL or missing references)
			//IL_0bb9: Unknown result type (might be due to invalid IL or missing references)
			//IL_0bc5: Unknown result type (might be due to invalid IL or missing references)
			//IL_0bcb: Unknown result type (might be due to invalid IL or missing references)
			//IL_0be4: Unknown result type (might be due to invalid IL or missing references)
			//IL_0bf0: Unknown result type (might be due to invalid IL or missing references)
			//IL_0c06: Unknown result type (might be due to invalid IL or missing references)
			//IL_0c1d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0c24: Unknown result type (might be due to invalid IL or missing references)
			//IL_0c2d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0c32: Unknown result type (might be due to invalid IL or missing references)
			//IL_0c46: Unknown result type (might be due to invalid IL or missing references)
			//IL_2609: Unknown result type (might be due to invalid IL or missing references)
			//IL_2615: Unknown result type (might be due to invalid IL or missing references)
			//IL_1e62: Unknown result type (might be due to invalid IL or missing references)
			//IL_1e67: Unknown result type (might be due to invalid IL or missing references)
			//IL_1762: Unknown result type (might be due to invalid IL or missing references)
			//IL_1769: Unknown result type (might be due to invalid IL or missing references)
			//IL_1775: Unknown result type (might be due to invalid IL or missing references)
			//IL_177c: Unknown result type (might be due to invalid IL or missing references)
			//IL_1790: Unknown result type (might be due to invalid IL or missing references)
			//IL_17a7: Unknown result type (might be due to invalid IL or missing references)
			//IL_17ae: Unknown result type (might be due to invalid IL or missing references)
			//IL_1e7b: Unknown result type (might be due to invalid IL or missing references)
			//IL_1e82: Unknown result type (might be due to invalid IL or missing references)
			//IL_1e95: Unknown result type (might be due to invalid IL or missing references)
			//IL_1eac: Unknown result type (might be due to invalid IL or missing references)
			//IL_1eb3: Unknown result type (might be due to invalid IL or missing references)
			//IL_26d6: Unknown result type (might be due to invalid IL or missing references)
			//IL_26ed: Unknown result type (might be due to invalid IL or missing references)
			//IL_26f4: Unknown result type (might be due to invalid IL or missing references)
			//IL_26c0: Unknown result type (might be due to invalid IL or missing references)
			//IL_26c5: Unknown result type (might be due to invalid IL or missing references)
			//IL_2660: Unknown result type (might be due to invalid IL or missing references)
			float num = 4f / 15f;
			NativeList<Entity> tempBuffer = default(NativeList<Entity>);
			CarCurrentLane carCurrentLane = default(CarCurrentLane);
			Moving moving = default(Moving);
			CarLaneSelectBuffer buffer;
			DynamicBuffer<CarNavigationLane> val;
			CarData carData2;
			CarLaneSpeedIterator carLaneSpeedIterator;
			Game.Net.CarLaneFlags laneFlags;
			if (m_CarCurrentLaneType.TryGetComponent(m_Selected, ref carCurrentLane) && m_MovingDataFromEntity.TryGetComponent(m_Selected, ref moving))
			{
				buffer = default(CarLaneSelectBuffer);
				Transform transform = m_TransformDataFromEntity[m_Selected];
				_ = m_TargetType[m_Selected];
				Car carData = m_CarDataFromEntity[m_Selected];
				PseudoRandomSeed randomSeed = m_PseudoRandomSeedType[m_Selected];
				PrefabRef prefabRef = m_PrefabRefDataFromEntity[m_Selected];
				CarNavigation carNavigation = m_CarNavigationType[m_Selected];
				Blocker blocker = m_BlockerType[m_Selected];
				val = m_CarNavigationLaneType[m_Selected];
				carData2 = m_PrefabCarData[prefabRef.m_Prefab];
				ObjectGeometryData objectGeometryData = m_PrefabObjectGeometryData[prefabRef.m_Prefab];
				float num2 = math.length(moving.m_Velocity);
				int priority = VehicleUtils.GetPriority(carData);
				VehicleUtils.GetDrivingStyle(m_SimulationFrame, randomSeed, out var safetyTime);
				if ((carCurrentLane.m_LaneFlags & Game.Vehicles.CarLaneFlags.Connection) != 0)
				{
					carData2.m_MaxSpeed = 277.77777f;
					carData2.m_Acceleration = 277.77777f;
					carData2.m_Braking = 277.77777f;
				}
				else
				{
					num2 = math.min(num2, carData2.m_MaxSpeed);
				}
				Bounds1 val2 = default(Bounds1);
				if ((carCurrentLane.m_LaneFlags & (Game.Vehicles.CarLaneFlags.Connection | Game.Vehicles.CarLaneFlags.ResetSpeed)) != 0)
				{
					((Bounds1)(ref val2))._002Ector(0f, carData2.m_MaxSpeed);
				}
				else
				{
					val2 = VehicleUtils.CalculateSpeedRange(carData2, num2, num);
				}
				if (val.Length != 0)
				{
					CarLaneSelectIterator carLaneSelectIterator = new CarLaneSelectIterator
					{
						m_OwnerData = m_OwnerDataFromEntity,
						m_LaneData = m_LaneData,
						m_CarLaneData = m_CarLaneData,
						m_SlaveLaneData = m_SlaveLaneData,
						m_LaneReservationData = m_LaneReservationData,
						m_MovingData = m_MovingDataFromEntity,
						m_CarData = m_CarDataFromEntity,
						m_ControllerData = m_ControllerDataFromEntity,
						m_Lanes = m_SubLanes,
						m_LaneObjects = m_LaneObjects,
						m_Entity = m_Selected,
						m_Blocker = blocker.m_Blocker,
						m_Priority = priority,
						m_ForbidLaneFlags = VehicleUtils.GetForbiddenLaneFlags(carData),
						m_PreferLaneFlags = VehicleUtils.GetPreferredLaneFlags(carData)
					};
					carLaneSelectIterator.SetBuffer(ref buffer);
					CarNavigationLane carNavigationLane = val[val.Length - 1];
					carLaneSelectIterator.CalculateLaneCosts(carNavigationLane, val.Length - 1);
					for (int num3 = val.Length - 2; num3 >= 0; num3--)
					{
						CarNavigationLane carNavigationLane2 = val[num3];
						if (m_LaneData.HasComponent(carNavigationLane.m_Lane))
						{
							carLaneSelectIterator.CalculateLaneCosts(carNavigationLane2, carNavigationLane, num3);
						}
						carNavigationLane = carNavigationLane2;
					}
					CarCurrentLane currentLaneData = carCurrentLane;
					carLaneSelectIterator.DrawLaneCosts(currentLaneData, val[0], m_CurveData, m_GizmoBatcher);
					for (int i = 0; i < val.Length; i++)
					{
						CarNavigationLane navLaneData = val[i];
						carLaneSelectIterator.DrawLaneCosts(navLaneData, m_CurveData, m_GizmoBatcher);
					}
				}
				carLaneSpeedIterator = new CarLaneSpeedIterator
				{
					m_TransformData = m_TransformDataFromEntity,
					m_MovingData = m_MovingDataFromEntity,
					m_CarData = m_CarDataFromEntity,
					m_TrainData = m_TrainDataFromEntity,
					m_ControllerData = m_ControllerDataFromEntity,
					m_LaneReservationData = m_LaneReservationData,
					m_LaneConditionData = m_LaneConditionData,
					m_LaneSignalData = m_LaneSignalData,
					m_CurveData = m_CurveData,
					m_CarLaneData = m_CarLaneData,
					m_ParkingLaneData = m_ParkingLaneData,
					m_UnspawnedData = m_UnspawnedData,
					m_CreatureData = m_CreatureData,
					m_PrefabRefData = m_PrefabRefDataFromEntity,
					m_PrefabObjectGeometryData = m_PrefabObjectGeometryData,
					m_PrefabCarData = m_PrefabCarData,
					m_PrefabTrainData = m_PrefabTrainData,
					m_PrefabParkingLaneData = m_PrefabParkingLaneData,
					m_LaneOverlapData = m_LaneOverlaps,
					m_LaneObjectData = m_LaneObjects,
					m_Entity = m_Selected,
					m_Ignore = (((carCurrentLane.m_LaneFlags & Game.Vehicles.CarLaneFlags.IgnoreBlocker) != 0) ? blocker.m_Blocker : Entity.Null),
					m_TempBuffer = tempBuffer,
					m_Priority = priority,
					m_TimeStep = num,
					m_SafeTimeStep = num + safetyTime,
					m_DistanceOffset = math.select(0f, math.max(-0.5f, -0.5f * math.lengthsq(1.5f - num2)), num2 < 1.5f),
					m_SpeedLimitFactor = VehicleUtils.GetSpeedLimitFactor(carData),
					m_CurrentSpeed = num2,
					m_PrefabCar = carData2,
					m_PrefabObjectGeometry = objectGeometryData,
					m_SpeedRange = val2,
					m_PushBlockers = ((carCurrentLane.m_LaneFlags & Game.Vehicles.CarLaneFlags.PushBlockers) != 0),
					m_MaxSpeed = val2.max,
					m_CanChangeLane = 1f,
					m_CurrentPosition = transform.m_Position
				};
				if ((carCurrentLane.m_LaneFlags & (Game.Vehicles.CarLaneFlags.TransformTarget | Game.Vehicles.CarLaneFlags.ParkingSpace)) != 0)
				{
					carLaneSpeedIterator.IterateTarget(carNavigation.m_TargetPosition);
					DrawBlocker(carCurrentLane.m_Lane, carLaneSpeedIterator.m_MaxSpeed / carData2.m_MaxSpeed);
					return;
				}
				if ((carCurrentLane.m_LaneFlags & Game.Vehicles.CarLaneFlags.Area) != 0)
				{
					carLaneSpeedIterator.IterateTarget(carNavigation.m_TargetPosition, 11.111112f);
					if (m_AreaLaneData.HasComponent(carCurrentLane.m_Lane))
					{
						Entity owner = m_OwnerDataFromEntity[carCurrentLane.m_Lane].m_Owner;
						AreaLane areaLane = m_AreaLaneData[carCurrentLane.m_Lane];
						DynamicBuffer<Game.Areas.Node> val3 = m_AreaNodes[owner];
						if (areaLane.m_Nodes.y == areaLane.m_Nodes.z)
						{
							Triangle3 val4 = default(Triangle3);
							((Triangle3)(ref val4))._002Ector(val3[areaLane.m_Nodes.x].m_Position, val3[areaLane.m_Nodes.y].m_Position, val3[areaLane.m_Nodes.w].m_Position);
							((GizmoBatcher)(ref m_GizmoBatcher)).DrawLine(val4.a, val4.b, Color.cyan);
							((GizmoBatcher)(ref m_GizmoBatcher)).DrawLine(val4.b, val4.c, Color.cyan);
							((GizmoBatcher)(ref m_GizmoBatcher)).DrawLine(val4.c, val4.a, Color.cyan);
						}
						else
						{
							bool4 val5 = default(bool4);
							((bool4)(ref val5))._002Ector(((float3)(ref carCurrentLane.m_CurvePosition)).yz < 0.5f, ((float3)(ref carCurrentLane.m_CurvePosition)).yz > 0.5f);
							Triangle3 val6 = default(Triangle3);
							Triangle3 val7 = default(Triangle3);
							if (val5.w)
							{
								((Triangle3)(ref val6))._002Ector(val3[areaLane.m_Nodes.z].m_Position, val3[areaLane.m_Nodes.y].m_Position, val3[areaLane.m_Nodes.x].m_Position);
								((Triangle3)(ref val7))._002Ector(val3[areaLane.m_Nodes.y].m_Position, val3[areaLane.m_Nodes.z].m_Position, val3[areaLane.m_Nodes.w].m_Position);
							}
							else
							{
								((Triangle3)(ref val6))._002Ector(val3[areaLane.m_Nodes.y].m_Position, val3[areaLane.m_Nodes.z].m_Position, val3[areaLane.m_Nodes.w].m_Position);
								((Triangle3)(ref val7))._002Ector(val3[areaLane.m_Nodes.z].m_Position, val3[areaLane.m_Nodes.y].m_Position, val3[areaLane.m_Nodes.x].m_Position);
							}
							if (math.any(((bool4)(ref val5)).xy & ((bool4)(ref val5)).wz))
							{
								((GizmoBatcher)(ref m_GizmoBatcher)).DrawLine(val6.a, val6.b, Color.blue);
								((GizmoBatcher)(ref m_GizmoBatcher)).DrawLine(val6.b, val6.c, Color.cyan);
								((GizmoBatcher)(ref m_GizmoBatcher)).DrawLine(val6.c, val6.a, Color.cyan);
								((GizmoBatcher)(ref m_GizmoBatcher)).DrawLine(val7.b, val7.c, Color.green);
								((GizmoBatcher)(ref m_GizmoBatcher)).DrawLine(val7.c, val7.a, Color.green);
							}
							else
							{
								((GizmoBatcher)(ref m_GizmoBatcher)).DrawLine(val6.b, val6.c, Color.yellow);
								((GizmoBatcher)(ref m_GizmoBatcher)).DrawLine(val6.c, val6.a, Color.yellow);
								((GizmoBatcher)(ref m_GizmoBatcher)).DrawLine(val7.a, val7.b, Color.blue);
								((GizmoBatcher)(ref m_GizmoBatcher)).DrawLine(val7.b, val7.c, Color.cyan);
								((GizmoBatcher)(ref m_GizmoBatcher)).DrawLine(val7.c, val7.a, Color.cyan);
							}
						}
					}
					((GizmoBatcher)(ref m_GizmoBatcher)).DrawLine(transform.m_Position, carNavigation.m_TargetPosition, Color.red);
					DrawBlocker(carCurrentLane.m_Lane, carLaneSpeedIterator.m_MaxSpeed / carData2.m_MaxSpeed);
					return;
				}
				if (carCurrentLane.m_Lane == Entity.Null)
				{
					return;
				}
				PrefabRef prefabRef2 = m_PrefabRefDataFromEntity[carCurrentLane.m_Lane];
				NetLaneData prefabLaneData = m_PrefabNetLaneData[prefabRef2.m_Prefab];
				float laneOffset = VehicleUtils.GetLaneOffset(objectGeometryData, prefabLaneData, carCurrentLane.m_LanePosition);
				Entity nextLane = Entity.Null;
				float2 nextOffset = float2.op_Implicit(0f);
				if (val.Length > 0)
				{
					CarNavigationLane carNavigationLane3 = val[0];
					nextLane = carNavigationLane3.m_Lane;
					nextOffset = carNavigationLane3.m_CurvePosition;
				}
				if (carCurrentLane.m_ChangeLane != Entity.Null)
				{
					PrefabRef prefabRef3 = m_PrefabRefDataFromEntity[carCurrentLane.m_ChangeLane];
					NetLaneData prefabLaneData2 = m_PrefabNetLaneData[prefabRef3.m_Prefab];
					float laneOffset2 = VehicleUtils.GetLaneOffset(objectGeometryData, prefabLaneData2, 0f - carCurrentLane.m_LanePosition);
					if (!carLaneSpeedIterator.IterateFirstLane(carCurrentLane.m_Lane, carCurrentLane.m_ChangeLane, carCurrentLane.m_CurvePosition, nextLane, nextOffset, carCurrentLane.m_ChangeProgress, laneOffset, laneOffset2, (carCurrentLane.m_LaneFlags & Game.Vehicles.CarLaneFlags.RequestSpace) != 0, out laneFlags))
					{
						goto IL_0b01;
					}
				}
				else if (!carLaneSpeedIterator.IterateFirstLane(carCurrentLane.m_Lane, carCurrentLane.m_CurvePosition, nextLane, nextOffset, laneOffset, (carCurrentLane.m_LaneFlags & Game.Vehicles.CarLaneFlags.RequestSpace) != 0, out laneFlags))
				{
					goto IL_0b01;
				}
				goto IL_0c88;
			}
			goto IL_0cdb;
			IL_276c:
			HumanCurrentLane humanCurrentLane = default(HumanCurrentLane);
			if (m_HumanCurrentLaneType.TryGetComponent(m_Selected, ref humanCurrentLane))
			{
				Blocker blocker2 = m_BlockerType[m_Selected];
				if (m_AreaLaneData.HasComponent(humanCurrentLane.m_Lane))
				{
					Entity owner2 = m_OwnerDataFromEntity[humanCurrentLane.m_Lane].m_Owner;
					AreaLane areaLane2 = m_AreaLaneData[humanCurrentLane.m_Lane];
					DynamicBuffer<Game.Areas.Node> val8 = m_AreaNodes[owner2];
					if (areaLane2.m_Nodes.y == areaLane2.m_Nodes.z)
					{
						Triangle3 val9 = default(Triangle3);
						((Triangle3)(ref val9))._002Ector(val8[areaLane2.m_Nodes.x].m_Position, val8[areaLane2.m_Nodes.y].m_Position, val8[areaLane2.m_Nodes.w].m_Position);
						((GizmoBatcher)(ref m_GizmoBatcher)).DrawLine(val9.a, val9.b, Color.cyan);
						((GizmoBatcher)(ref m_GizmoBatcher)).DrawLine(val9.b, val9.c, Color.cyan);
						((GizmoBatcher)(ref m_GizmoBatcher)).DrawLine(val9.c, val9.a, Color.cyan);
					}
					else
					{
						bool4 val10 = default(bool4);
						((bool4)(ref val10))._002Ector(humanCurrentLane.m_CurvePosition < 0.5f, humanCurrentLane.m_CurvePosition > 0.5f);
						Triangle3 val11 = default(Triangle3);
						Triangle3 val12 = default(Triangle3);
						if (val10.w)
						{
							((Triangle3)(ref val11))._002Ector(val8[areaLane2.m_Nodes.z].m_Position, val8[areaLane2.m_Nodes.y].m_Position, val8[areaLane2.m_Nodes.x].m_Position);
							((Triangle3)(ref val12))._002Ector(val8[areaLane2.m_Nodes.y].m_Position, val8[areaLane2.m_Nodes.z].m_Position, val8[areaLane2.m_Nodes.w].m_Position);
						}
						else
						{
							((Triangle3)(ref val11))._002Ector(val8[areaLane2.m_Nodes.y].m_Position, val8[areaLane2.m_Nodes.z].m_Position, val8[areaLane2.m_Nodes.w].m_Position);
							((Triangle3)(ref val12))._002Ector(val8[areaLane2.m_Nodes.z].m_Position, val8[areaLane2.m_Nodes.y].m_Position, val8[areaLane2.m_Nodes.x].m_Position);
						}
						if (math.any(((bool4)(ref val10)).xy & ((bool4)(ref val10)).wz))
						{
							((GizmoBatcher)(ref m_GizmoBatcher)).DrawLine(val11.a, val11.b, Color.blue);
							((GizmoBatcher)(ref m_GizmoBatcher)).DrawLine(val11.b, val11.c, Color.cyan);
							((GizmoBatcher)(ref m_GizmoBatcher)).DrawLine(val11.c, val11.a, Color.cyan);
							((GizmoBatcher)(ref m_GizmoBatcher)).DrawLine(val12.b, val12.c, Color.green);
							((GizmoBatcher)(ref m_GizmoBatcher)).DrawLine(val12.c, val12.a, Color.green);
						}
						else
						{
							((GizmoBatcher)(ref m_GizmoBatcher)).DrawLine(val11.b, val11.c, Color.yellow);
							((GizmoBatcher)(ref m_GizmoBatcher)).DrawLine(val11.c, val11.a, Color.yellow);
							((GizmoBatcher)(ref m_GizmoBatcher)).DrawLine(val12.a, val12.b, Color.blue);
							((GizmoBatcher)(ref m_GizmoBatcher)).DrawLine(val12.b, val12.c, Color.cyan);
							((GizmoBatcher)(ref m_GizmoBatcher)).DrawLine(val12.c, val12.a, Color.cyan);
						}
					}
				}
				if (blocker2.m_Blocker != Entity.Null)
				{
					DrawBlocker(blocker2.m_Blocker, (float)(int)blocker2.m_MaxSpeed * 0.003921569f);
				}
			}
			AnimalCurrentLane animalCurrentLane = default(AnimalCurrentLane);
			if (m_AnimalCurrentLaneType.TryGetComponent(m_Selected, ref animalCurrentLane))
			{
				Blocker blocker3 = m_BlockerType[m_Selected];
				if (blocker3.m_Blocker != Entity.Null)
				{
					DrawBlocker(blocker3.m_Blocker, (float)(int)blocker3.m_MaxSpeed * 0.003921569f);
				}
			}
			if (tempBuffer.IsCreated)
			{
				tempBuffer.Dispose();
			}
			return;
			IL_0c88:
			if (carLaneSpeedIterator.m_Blocker != Entity.Null)
			{
				DrawBlocker(carLaneSpeedIterator.m_Blocker, carLaneSpeedIterator.m_MaxSpeed / carData2.m_MaxSpeed);
			}
			if (carLaneSpeedIterator.m_TempBuffer.IsCreated)
			{
				tempBuffer = carLaneSpeedIterator.m_TempBuffer;
				tempBuffer.Clear();
			}
			buffer.Dispose();
			goto IL_0cdb;
			IL_16a4:
			int num4 = 0;
			DynamicBuffer<WatercraftNavigationLane> val13;
			WatercraftLaneSpeedIterator watercraftLaneSpeedIterator;
			WatercraftCurrentLane watercraftCurrentLane = default(WatercraftCurrentLane);
			bool needSignal;
			while (true)
			{
				if (num4 < val13.Length)
				{
					WatercraftNavigationLane watercraftNavigationLane = val13[num4];
					if ((watercraftNavigationLane.m_Flags & (WatercraftLaneFlags.TransformTarget | WatercraftLaneFlags.Area)) == 0)
					{
						if ((watercraftNavigationLane.m_Flags & WatercraftLaneFlags.Connection) != 0)
						{
							watercraftLaneSpeedIterator.m_PrefabWatercraft.m_MaxSpeed = 277.77777f;
							watercraftLaneSpeedIterator.m_PrefabWatercraft.m_Acceleration = 277.77777f;
							watercraftLaneSpeedIterator.m_PrefabWatercraft.m_Braking = 277.77777f;
							watercraftLaneSpeedIterator.m_SpeedRange = new Bounds1(0f, 277.77777f);
						}
						else if ((watercraftCurrentLane.m_LaneFlags & WatercraftLaneFlags.Connection) != 0)
						{
							goto IL_17d2;
						}
						bool flag = (watercraftNavigationLane.m_Lane == watercraftCurrentLane.m_Lane) | (watercraftNavigationLane.m_Lane == watercraftCurrentLane.m_ChangeLane);
						float minOffset = math.select(-1f, watercraftCurrentLane.m_CurvePosition.y, flag);
						if (watercraftLaneSpeedIterator.IterateNextLane(watercraftNavigationLane.m_Lane, watercraftNavigationLane.m_CurvePosition, minOffset, out needSignal))
						{
							break;
						}
						num4++;
						continue;
					}
					VehicleUtils.CalculateTransformPosition(ref watercraftLaneSpeedIterator.m_CurrentPosition, watercraftNavigationLane.m_Lane, m_TransformDataFromEntity, m_PositionData, m_PrefabRefDataFromEntity, m_PrefabBuildingData);
				}
				goto IL_17d2;
				IL_17d2:
				watercraftLaneSpeedIterator.IterateTarget(watercraftLaneSpeedIterator.m_CurrentPosition);
				break;
			}
			goto IL_17e0;
			IL_0cdb:
			WatercraftLaneSelectBuffer buffer2;
			WatercraftData watercraftData;
			float3 val15;
			if (m_WatercraftCurrentLaneType.TryGetComponent(m_Selected, ref watercraftCurrentLane) && m_MovingDataFromEntity.TryGetComponent(m_Selected, ref moving))
			{
				buffer2 = default(WatercraftLaneSelectBuffer);
				Transform transform2 = m_TransformDataFromEntity[m_Selected];
				_ = m_TargetType[m_Selected];
				_ = m_WatercraftDataFromEntity[m_Selected];
				PrefabRef prefabRef4 = m_PrefabRefDataFromEntity[m_Selected];
				WatercraftNavigation watercraftNavigation = m_WatercraftNavigationType[m_Selected];
				Blocker blocker4 = m_BlockerType[m_Selected];
				val13 = m_WatercraftNavigationLaneType[m_Selected];
				watercraftData = m_PrefabWatercraftData[prefabRef4.m_Prefab];
				ObjectGeometryData prefabObjectGeometry = m_PrefabObjectGeometryData[prefabRef4.m_Prefab];
				float num5 = math.length(moving.m_Velocity);
				int priority2 = VehicleUtils.GetPriority(watercraftData);
				if ((watercraftCurrentLane.m_LaneFlags & WatercraftLaneFlags.Connection) != 0)
				{
					watercraftData.m_MaxSpeed = 277.77777f;
					watercraftData.m_Acceleration = 277.77777f;
					watercraftData.m_Braking = 277.77777f;
				}
				else
				{
					num5 = math.min(num5, watercraftData.m_MaxSpeed);
				}
				Bounds1 val14 = default(Bounds1);
				if ((watercraftCurrentLane.m_LaneFlags & (WatercraftLaneFlags.ResetSpeed | WatercraftLaneFlags.Connection)) != 0)
				{
					((Bounds1)(ref val14))._002Ector(0f, watercraftData.m_MaxSpeed);
				}
				else
				{
					val14 = VehicleUtils.CalculateSpeedRange(watercraftData, num5, num);
				}
				float3 position = transform2.m_Position;
				Curve curve = default(Curve);
				if ((watercraftCurrentLane.m_LaneFlags & (WatercraftLaneFlags.TransformTarget | WatercraftLaneFlags.Area)) == 0 && m_CurveData.TryGetComponent(watercraftCurrentLane.m_Lane, ref curve))
				{
					PrefabRef prefabRef5 = m_PrefabRefDataFromEntity[watercraftCurrentLane.m_Lane];
					NetLaneData netLaneData = m_PrefabNetLaneData[prefabRef5.m_Prefab];
					val15 = MathUtils.Tangent(curve.m_Bezier, watercraftCurrentLane.m_CurvePosition.x);
					float2 xz = ((float3)(ref val15)).xz;
					if (MathUtils.TryNormalize(ref xz))
					{
						((float3)(ref position)).xz = ((float3)(ref position)).xz - MathUtils.Right(xz) * ((netLaneData.m_Width - prefabObjectGeometry.m_Size.x) * watercraftCurrentLane.m_LanePosition * 0.5f);
					}
				}
				if (val13.Length != 0)
				{
					WatercraftLaneSelectIterator watercraftLaneSelectIterator = new WatercraftLaneSelectIterator
					{
						m_OwnerData = m_OwnerDataFromEntity,
						m_LaneData = m_LaneData,
						m_SlaveLaneData = m_SlaveLaneData,
						m_LaneReservationData = m_LaneReservationData,
						m_MovingData = m_MovingDataFromEntity,
						m_WatercraftData = m_WatercraftDataFromEntity,
						m_Lanes = m_SubLanes,
						m_LaneObjects = m_LaneObjects,
						m_Entity = m_Selected,
						m_Blocker = blocker4.m_Blocker,
						m_Priority = priority2
					};
					watercraftLaneSelectIterator.SetBuffer(ref buffer2);
					WatercraftNavigationLane watercraftNavigationLane2 = val13[val13.Length - 1];
					watercraftLaneSelectIterator.CalculateLaneCosts(watercraftNavigationLane2, val13.Length - 1);
					for (int num6 = val13.Length - 2; num6 >= 0; num6--)
					{
						WatercraftNavigationLane watercraftNavigationLane3 = val13[num6];
						watercraftLaneSelectIterator.CalculateLaneCosts(watercraftNavigationLane3, watercraftNavigationLane2, num6);
						watercraftNavigationLane2 = watercraftNavigationLane3;
					}
					WatercraftCurrentLane currentLaneData2 = watercraftCurrentLane;
					watercraftLaneSelectIterator.DrawLaneCosts(currentLaneData2, val13[0], m_CurveData, m_GizmoBatcher);
					for (int j = 0; j < val13.Length; j++)
					{
						WatercraftNavigationLane navLaneData2 = val13[j];
						watercraftLaneSelectIterator.DrawLaneCosts(navLaneData2, m_CurveData, m_GizmoBatcher);
					}
				}
				watercraftLaneSpeedIterator = new WatercraftLaneSpeedIterator
				{
					m_TransformData = m_TransformDataFromEntity,
					m_MovingData = m_MovingDataFromEntity,
					m_WatercraftData = m_WatercraftDataFromEntity,
					m_LaneReservationData = m_LaneReservationData,
					m_LaneSignalData = m_LaneSignalData,
					m_CurveData = m_CurveData,
					m_CarLaneData = m_CarLaneData,
					m_PrefabRefData = m_PrefabRefDataFromEntity,
					m_PrefabObjectGeometryData = m_PrefabObjectGeometryData,
					m_PrefabWatercraftData = m_PrefabWatercraftData,
					m_LaneOverlapData = m_LaneOverlaps,
					m_LaneObjectData = m_LaneObjects,
					m_Entity = m_Selected,
					m_Ignore = (((watercraftCurrentLane.m_LaneFlags & WatercraftLaneFlags.IgnoreBlocker) != 0) ? blocker4.m_Blocker : Entity.Null),
					m_Priority = priority2,
					m_TimeStep = num,
					m_SafeTimeStep = num + 0.5f,
					m_SpeedLimitFactor = 1f,
					m_CurrentSpeed = num5,
					m_PrefabWatercraft = watercraftData,
					m_PrefabObjectGeometry = prefabObjectGeometry,
					m_SpeedRange = val14,
					m_MaxSpeed = val14.max,
					m_CanChangeLane = 1f,
					m_CurrentPosition = position
				};
				if ((watercraftCurrentLane.m_LaneFlags & WatercraftLaneFlags.TransformTarget) != 0)
				{
					watercraftLaneSpeedIterator.IterateTarget(watercraftNavigation.m_TargetPosition);
					DrawBlocker(watercraftCurrentLane.m_Lane, watercraftLaneSpeedIterator.m_MaxSpeed / watercraftData.m_MaxSpeed);
					return;
				}
				if ((watercraftCurrentLane.m_LaneFlags & WatercraftLaneFlags.Area) != 0)
				{
					watercraftLaneSpeedIterator.IterateTarget(watercraftNavigation.m_TargetPosition, 11.111112f);
					if (m_AreaLaneData.HasComponent(watercraftCurrentLane.m_Lane))
					{
						Entity owner3 = m_OwnerDataFromEntity[watercraftCurrentLane.m_Lane].m_Owner;
						AreaLane areaLane3 = m_AreaLaneData[watercraftCurrentLane.m_Lane];
						DynamicBuffer<Game.Areas.Node> val16 = m_AreaNodes[owner3];
						if (areaLane3.m_Nodes.y == areaLane3.m_Nodes.z)
						{
							Triangle3 val17 = default(Triangle3);
							((Triangle3)(ref val17))._002Ector(val16[areaLane3.m_Nodes.x].m_Position, val16[areaLane3.m_Nodes.y].m_Position, val16[areaLane3.m_Nodes.w].m_Position);
							((GizmoBatcher)(ref m_GizmoBatcher)).DrawLine(val17.a, val17.b, Color.cyan);
							((GizmoBatcher)(ref m_GizmoBatcher)).DrawLine(val17.b, val17.c, Color.cyan);
							((GizmoBatcher)(ref m_GizmoBatcher)).DrawLine(val17.c, val17.a, Color.cyan);
						}
						else
						{
							bool4 val18 = default(bool4);
							((bool4)(ref val18))._002Ector(((float3)(ref watercraftCurrentLane.m_CurvePosition)).yz < 0.5f, ((float3)(ref watercraftCurrentLane.m_CurvePosition)).yz > 0.5f);
							Triangle3 val19 = default(Triangle3);
							Triangle3 val20 = default(Triangle3);
							if (val18.w)
							{
								((Triangle3)(ref val19))._002Ector(val16[areaLane3.m_Nodes.z].m_Position, val16[areaLane3.m_Nodes.y].m_Position, val16[areaLane3.m_Nodes.x].m_Position);
								((Triangle3)(ref val20))._002Ector(val16[areaLane3.m_Nodes.y].m_Position, val16[areaLane3.m_Nodes.z].m_Position, val16[areaLane3.m_Nodes.w].m_Position);
							}
							else
							{
								((Triangle3)(ref val19))._002Ector(val16[areaLane3.m_Nodes.y].m_Position, val16[areaLane3.m_Nodes.z].m_Position, val16[areaLane3.m_Nodes.w].m_Position);
								((Triangle3)(ref val20))._002Ector(val16[areaLane3.m_Nodes.z].m_Position, val16[areaLane3.m_Nodes.y].m_Position, val16[areaLane3.m_Nodes.x].m_Position);
							}
							if (math.any(((bool4)(ref val18)).xy & ((bool4)(ref val18)).wz))
							{
								((GizmoBatcher)(ref m_GizmoBatcher)).DrawLine(val19.a, val19.b, Color.blue);
								((GizmoBatcher)(ref m_GizmoBatcher)).DrawLine(val19.b, val19.c, Color.cyan);
								((GizmoBatcher)(ref m_GizmoBatcher)).DrawLine(val19.c, val19.a, Color.cyan);
								((GizmoBatcher)(ref m_GizmoBatcher)).DrawLine(val20.b, val20.c, Color.green);
								((GizmoBatcher)(ref m_GizmoBatcher)).DrawLine(val20.c, val20.a, Color.green);
							}
							else
							{
								((GizmoBatcher)(ref m_GizmoBatcher)).DrawLine(val19.b, val19.c, Color.yellow);
								((GizmoBatcher)(ref m_GizmoBatcher)).DrawLine(val19.c, val19.a, Color.yellow);
								((GizmoBatcher)(ref m_GizmoBatcher)).DrawLine(val20.a, val20.b, Color.blue);
								((GizmoBatcher)(ref m_GizmoBatcher)).DrawLine(val20.b, val20.c, Color.cyan);
								((GizmoBatcher)(ref m_GizmoBatcher)).DrawLine(val20.c, val20.a, Color.cyan);
							}
						}
					}
					((GizmoBatcher)(ref m_GizmoBatcher)).DrawLine(transform2.m_Position, watercraftNavigation.m_TargetPosition, Color.red);
					DrawBlocker(carCurrentLane.m_Lane, watercraftLaneSpeedIterator.m_MaxSpeed / watercraftData.m_MaxSpeed);
					return;
				}
				if (watercraftCurrentLane.m_Lane == Entity.Null)
				{
					return;
				}
				if (watercraftCurrentLane.m_ChangeLane != Entity.Null)
				{
					if (!watercraftLaneSpeedIterator.IterateFirstLane(watercraftCurrentLane.m_Lane, watercraftCurrentLane.m_ChangeLane, watercraftCurrentLane.m_CurvePosition, watercraftCurrentLane.m_ChangeProgress))
					{
						goto IL_16a4;
					}
				}
				else if (!watercraftLaneSpeedIterator.IterateFirstLane(watercraftCurrentLane.m_Lane, watercraftCurrentLane.m_CurvePosition))
				{
					goto IL_16a4;
				}
				goto IL_17e0;
			}
			goto IL_1816;
			IL_17e0:
			if (watercraftLaneSpeedIterator.m_Blocker != Entity.Null)
			{
				DrawBlocker(watercraftLaneSpeedIterator.m_Blocker, watercraftLaneSpeedIterator.m_MaxSpeed / watercraftData.m_MaxSpeed);
			}
			buffer2.Dispose();
			goto IL_1816;
			IL_1816:
			AircraftCurrentLane aircraftCurrentLane = default(AircraftCurrentLane);
			if (m_AircraftCurrentLaneType.TryGetComponent(m_Selected, ref aircraftCurrentLane) && m_MovingDataFromEntity.TryGetComponent(m_Selected, ref moving))
			{
				Transform transform3 = m_TransformDataFromEntity[m_Selected];
				_ = m_TargetType[m_Selected];
				_ = m_AircraftDataFromEntity[m_Selected];
				PrefabRef prefabRef6 = m_PrefabRefDataFromEntity[m_Selected];
				AircraftNavigation aircraftNavigation = m_AircraftNavigationType[m_Selected];
				Blocker blocker5 = m_BlockerType[m_Selected];
				DynamicBuffer<AircraftNavigationLane> val21 = m_AircraftNavigationLaneType[m_Selected];
				AircraftData aircraftData = m_PrefabAircraftData[prefabRef6.m_Prefab];
				ObjectGeometryData prefabObjectGeometry2 = m_PrefabObjectGeometryData[prefabRef6.m_Prefab];
				float3 val24;
				if (m_CurveData.HasComponent(aircraftCurrentLane.m_Lane))
				{
					Curve curve2 = m_CurveData[aircraftCurrentLane.m_Lane];
					Bezier4x3 val22 = MathUtils.Cut(curve2.m_Bezier, ((float3)(ref aircraftCurrentLane.m_CurvePosition)).xy);
					Bezier4x3 val23 = MathUtils.Cut(curve2.m_Bezier, ((float3)(ref aircraftCurrentLane.m_CurvePosition)).yz);
					val24 = val23.d;
					((GizmoBatcher)(ref m_GizmoBatcher)).DrawCurve(val22, curve2.m_Length, new Color(1f, 0.5f, 0f, 1f), -1);
					((GizmoBatcher)(ref m_GizmoBatcher)).DrawCurve(val23, curve2.m_Length, Color.yellow, -1);
					((GizmoBatcher)(ref m_GizmoBatcher)).DrawLine(transform3.m_Position, val22.a, Color.red);
				}
				else
				{
					if (!m_TransformDataFromEntity.HasComponent(aircraftCurrentLane.m_Lane))
					{
						return;
					}
					val24 = m_TransformDataFromEntity[aircraftCurrentLane.m_Lane].m_Position;
					((GizmoBatcher)(ref m_GizmoBatcher)).DrawLine(transform3.m_Position, val24, Color.red);
				}
				for (int k = 0; k < val21.Length; k++)
				{
					AircraftNavigationLane aircraftNavigationLane = val21[k];
					if (m_CurveData.HasComponent(aircraftNavigationLane.m_Lane))
					{
						Curve curve3 = m_CurveData[aircraftNavigationLane.m_Lane];
						Bezier4x3 val25 = MathUtils.Cut(curve3.m_Bezier, aircraftNavigationLane.m_CurvePosition);
						((GizmoBatcher)(ref m_GizmoBatcher)).DrawCurve(val25, curve3.m_Length, Color.green, -1);
						if (math.lengthsq(val25.a - val24) > 1f)
						{
							((GizmoBatcher)(ref m_GizmoBatcher)).DrawLine(val24, val25.a, Color.magenta);
						}
						val24 = val25.d;
						continue;
					}
					if (!m_TransformDataFromEntity.HasComponent(aircraftCurrentLane.m_Lane))
					{
						break;
					}
					float3 position2 = m_TransformDataFromEntity[aircraftCurrentLane.m_Lane].m_Position;
					if (math.lengthsq(position2 - val24) > 1f)
					{
						((GizmoBatcher)(ref m_GizmoBatcher)).DrawLine(val24, position2, Color.magenta);
					}
					val24 = position2;
				}
				float currentSpeed = math.length(moving.m_Velocity);
				int priority3 = VehicleUtils.GetPriority(aircraftData);
				if ((aircraftCurrentLane.m_LaneFlags & AircraftLaneFlags.Flying) == 0)
				{
					float3 position3 = transform3.m_Position;
					if (m_CurveData.HasComponent(aircraftCurrentLane.m_Lane))
					{
						Curve curve4 = m_CurveData[aircraftCurrentLane.m_Lane];
						PrefabRef prefabRef7 = m_PrefabRefDataFromEntity[aircraftCurrentLane.m_Lane];
						NetLaneData netLaneData2 = m_PrefabNetLaneData[prefabRef7.m_Prefab];
						val15 = MathUtils.Tangent(curve4.m_Bezier, aircraftCurrentLane.m_CurvePosition.x);
						float2 xz2 = ((float3)(ref val15)).xz;
						if (MathUtils.TryNormalize(ref xz2))
						{
							((float3)(ref position3)).xz = ((float3)(ref position3)).xz - MathUtils.Right(xz2) * ((netLaneData2.m_Width - prefabObjectGeometry2.m_Size.x) * aircraftCurrentLane.m_LanePosition * 0.5f);
						}
					}
					Bounds1 val26 = default(Bounds1);
					if ((aircraftCurrentLane.m_LaneFlags & (AircraftLaneFlags.Connection | AircraftLaneFlags.ResetSpeed)) != 0)
					{
						((Bounds1)(ref val26))._002Ector(0f, aircraftData.m_GroundMaxSpeed);
					}
					else
					{
						val26 = VehicleUtils.CalculateSpeedRange(aircraftData, currentSpeed, num);
					}
					AircraftLaneSpeedIterator aircraftLaneSpeedIterator = new AircraftLaneSpeedIterator
					{
						m_TransformData = m_TransformDataFromEntity,
						m_MovingData = m_MovingDataFromEntity,
						m_AircraftData = m_AircraftDataFromEntity,
						m_LaneReservationData = m_LaneReservationData,
						m_CurveData = m_CurveData,
						m_CarLaneData = m_CarLaneData,
						m_PrefabRefData = m_PrefabRefDataFromEntity,
						m_PrefabObjectGeometryData = m_PrefabObjectGeometryData,
						m_PrefabAircraftData = m_PrefabAircraftData,
						m_LaneOverlapData = m_LaneOverlaps,
						m_LaneObjectData = m_LaneObjects,
						m_Entity = m_Selected,
						m_Ignore = (((aircraftCurrentLane.m_LaneFlags & AircraftLaneFlags.IgnoreBlocker) != 0) ? blocker5.m_Blocker : Entity.Null),
						m_Priority = priority3,
						m_TimeStep = num,
						m_SafeTimeStep = num + 0.5f,
						m_PrefabAircraft = aircraftData,
						m_PrefabObjectGeometry = prefabObjectGeometry2,
						m_SpeedRange = val26,
						m_MaxSpeed = val26.max,
						m_CanChangeLane = 1f,
						m_CurrentPosition = position3
					};
					if ((aircraftCurrentLane.m_LaneFlags & AircraftLaneFlags.TransformTarget) != 0)
					{
						aircraftLaneSpeedIterator.IterateTarget(aircraftNavigation.m_TargetPosition);
						DrawBlocker(aircraftCurrentLane.m_Lane, aircraftLaneSpeedIterator.m_MaxSpeed / aircraftData.m_GroundMaxSpeed);
						return;
					}
					if (aircraftCurrentLane.m_Lane == Entity.Null)
					{
						return;
					}
					if (!aircraftLaneSpeedIterator.IterateFirstLane(aircraftCurrentLane.m_Lane, aircraftCurrentLane.m_CurvePosition))
					{
						int num7 = 0;
						while (true)
						{
							if (num7 < val21.Length)
							{
								AircraftNavigationLane aircraftNavigationLane2 = val21[num7];
								if ((aircraftNavigationLane2.m_Flags & AircraftLaneFlags.TransformTarget) == 0)
								{
									if ((aircraftNavigationLane2.m_Flags & AircraftLaneFlags.Connection) != 0)
									{
										aircraftLaneSpeedIterator.m_PrefabAircraft.m_GroundMaxSpeed = 277.77777f;
										aircraftLaneSpeedIterator.m_PrefabAircraft.m_GroundAcceleration = 277.77777f;
										aircraftLaneSpeedIterator.m_PrefabAircraft.m_GroundBraking = 277.77777f;
										aircraftLaneSpeedIterator.m_SpeedRange = new Bounds1(0f, 277.77777f);
									}
									else if ((aircraftCurrentLane.m_LaneFlags & AircraftLaneFlags.Connection) != 0)
									{
										goto IL_1ed5;
									}
									bool flag2 = aircraftNavigationLane2.m_Lane == aircraftCurrentLane.m_Lane;
									float minOffset2 = math.select(-1f, aircraftCurrentLane.m_CurvePosition.y, flag2);
									if (aircraftLaneSpeedIterator.IterateNextLane(aircraftNavigationLane2.m_Lane, aircraftNavigationLane2.m_CurvePosition, minOffset2))
									{
										break;
									}
									num7++;
									continue;
								}
								VehicleUtils.CalculateTransformPosition(ref aircraftLaneSpeedIterator.m_CurrentPosition, aircraftNavigationLane2.m_Lane, m_TransformDataFromEntity, m_PositionData, m_PrefabRefDataFromEntity, m_PrefabBuildingData);
							}
							goto IL_1ed5;
							IL_1ed5:
							aircraftLaneSpeedIterator.IterateTarget(aircraftLaneSpeedIterator.m_CurrentPosition);
							break;
						}
					}
					if (aircraftLaneSpeedIterator.m_Blocker != Entity.Null)
					{
						DrawBlocker(aircraftLaneSpeedIterator.m_Blocker, aircraftLaneSpeedIterator.m_MaxSpeed / aircraftData.m_GroundMaxSpeed);
					}
				}
			}
			TrainCurrentLane trainCurrentLane = default(TrainCurrentLane);
			TrainData trainData;
			TrainLaneSpeedIterator trainLaneSpeedIterator;
			if (m_TrainCurrentLaneType.TryGetComponent(m_Selected, ref trainCurrentLane) && m_MovingDataFromEntity.TryGetComponent(m_Selected, ref moving))
			{
				Entity val27 = m_Selected;
				DynamicBuffer<LayoutElement> val28 = default(DynamicBuffer<LayoutElement>);
				if (m_LayoutElementType.TryGetBuffer(m_Selected, ref val28))
				{
					if (val28.Length == 0)
					{
						return;
					}
					val27 = val28[0].m_Vehicle;
				}
				Transform transform4 = m_TransformDataFromEntity[val27];
				Train train = m_TrainDataFromEntity[val27];
				TrainCurrentLane trainCurrentLane2 = m_TrainCurrentLaneData[val27];
				PrefabRef prefabRef8 = m_PrefabRefDataFromEntity[val27];
				trainData = m_PrefabTrainData[prefabRef8.m_Prefab];
				ObjectGeometryData prefabObjectGeometry3 = m_PrefabObjectGeometryData[prefabRef8.m_Prefab];
				VehicleUtils.CalculateTrainNavigationPivots(transform4, trainData, out var pivot, out var pivot2);
				if ((train.m_Flags & Game.Vehicles.TrainFlags.Reversed) != 0)
				{
					CommonUtils.Swap(ref pivot, ref pivot2);
					trainData.m_BogieOffsets = ((float2)(ref trainData.m_BogieOffsets)).yx;
					trainData.m_AttachOffsets = ((float2)(ref trainData.m_AttachOffsets)).yx;
				}
				if (!m_CurveData.HasComponent(trainCurrentLane2.m_Front.m_Lane) || !m_CurveData.HasComponent(trainCurrentLane2.m_Rear.m_Lane))
				{
					return;
				}
				Curve curve5 = m_CurveData[trainCurrentLane2.m_Front.m_Lane];
				Curve curve6 = m_CurveData[trainCurrentLane2.m_Rear.m_Lane];
				Bezier4x3 val29 = MathUtils.Cut(curve5.m_Bezier, ((float4)(ref trainCurrentLane2.m_Front.m_CurvePosition)).yw);
				Bezier4x3 val30 = MathUtils.Cut(curve6.m_Bezier, ((float4)(ref trainCurrentLane2.m_Rear.m_CurvePosition)).yw);
				float num8 = curve5.m_Length * math.abs(trainCurrentLane2.m_Front.m_CurvePosition.w - trainCurrentLane2.m_Front.m_CurvePosition.y);
				((GizmoBatcher)(ref m_GizmoBatcher)).DrawCurve(val29, num8, new Color(1f, 0.5f, 0f, 1f), -1);
				((GizmoBatcher)(ref m_GizmoBatcher)).DrawLine(pivot, val29.a, Color.red);
				((GizmoBatcher)(ref m_GizmoBatcher)).DrawLine(pivot2, val30.a, Color.red);
				DynamicBuffer<TrainNavigationLane> val31 = default(DynamicBuffer<TrainNavigationLane>);
				if (m_TrainNavigationLaneType.TryGetBuffer(m_Selected, ref val31))
				{
					for (int l = 1; l < val28.Length; l++)
					{
						Entity vehicle = val28[l].m_Vehicle;
						PrefabRef prefabRef9 = m_PrefabRefDataFromEntity[vehicle];
						TrainData trainData2 = m_PrefabTrainData[prefabRef9.m_Prefab];
						trainData.m_MaxSpeed = math.min(trainData.m_MaxSpeed, trainData2.m_MaxSpeed);
						trainData.m_Acceleration = math.min(trainData.m_Acceleration, trainData2.m_Acceleration);
						trainData.m_Braking = math.min(trainData.m_Braking, trainData2.m_Braking);
					}
					float currentSpeed2 = math.length(moving.m_Velocity);
					Bounds1 val32 = VehicleUtils.CalculateSpeedRange(trainData, currentSpeed2, num);
					int priority4 = VehicleUtils.GetPriority(trainData);
					trainLaneSpeedIterator = new TrainLaneSpeedIterator
					{
						m_TransformData = m_TransformDataFromEntity,
						m_MovingData = m_MovingDataFromEntity,
						m_CarData = m_CarDataFromEntity,
						m_TrainData = m_TrainDataFromEntity,
						m_LaneReservationData = m_LaneReservationData,
						m_LaneSignalData = m_LaneSignalData,
						m_CreatureData = m_CreatureData,
						m_CurveData = m_CurveData,
						m_TrackLaneData = m_TrackLaneData,
						m_ControllerData = m_ControllerDataFromEntity,
						m_PrefabRefData = m_PrefabRefDataFromEntity,
						m_PrefabObjectGeometryData = m_PrefabObjectGeometryData,
						m_PrefabCarData = m_PrefabCarData,
						m_PrefabTrainData = m_PrefabTrainData,
						m_LaneOverlapData = m_LaneOverlaps,
						m_LaneObjectData = m_LaneObjects,
						m_Controller = m_Selected,
						m_Priority = priority4,
						m_TimeStep = num,
						m_SafeTimeStep = num + 0.5f,
						m_CurrentSpeed = currentSpeed2,
						m_PrefabTrain = trainData,
						m_SpeedRange = val32,
						m_RearPosition = pivot2,
						m_PushBlockers = ((trainCurrentLane2.m_Front.m_LaneFlags & TrainLaneFlags.PushBlockers) != 0),
						m_MaxSpeed = val32.max,
						m_CurrentPosition = pivot
					};
					float3 d = val29.d;
					for (int m = 0; m < val31.Length; m++)
					{
						TrainNavigationLane trainNavigationLane = val31[m];
						if (!m_CurveData.HasComponent(trainNavigationLane.m_Lane))
						{
							break;
						}
						Curve curve7 = m_CurveData[trainNavigationLane.m_Lane];
						val29 = MathUtils.Cut(curve7.m_Bezier, trainNavigationLane.m_CurvePosition);
						num8 = curve7.m_Length * math.abs(trainNavigationLane.m_CurvePosition.x - trainNavigationLane.m_CurvePosition.y);
						((GizmoBatcher)(ref m_GizmoBatcher)).DrawCurve(val29, num8, Color.green, -1);
						if (m != 0 && math.lengthsq(val29.a - d) > 1f)
						{
							((GizmoBatcher)(ref m_GizmoBatcher)).DrawLine(d, val29.a, Color.magenta);
						}
						d = val29.d;
					}
					for (int num9 = val28.Length - 1; num9 >= 1; num9--)
					{
						Entity vehicle2 = val28[num9].m_Vehicle;
						TrainCurrentLane trainCurrentLane3 = m_TrainCurrentLaneData[vehicle2];
						PrefabRef prefabRef10 = m_PrefabRefDataFromEntity[vehicle2];
						TrainData prefabTrain = m_PrefabTrainData[prefabRef10.m_Prefab];
						trainLaneSpeedIterator.m_PrefabTrain = prefabTrain;
						trainLaneSpeedIterator.IteratePrevLane(trainCurrentLane3.m_RearCache.m_Lane, out needSignal);
						trainLaneSpeedIterator.IteratePrevLane(trainCurrentLane3.m_Rear.m_Lane, out needSignal);
						trainLaneSpeedIterator.IteratePrevLane(trainCurrentLane3.m_FrontCache.m_Lane, out needSignal);
						trainLaneSpeedIterator.IteratePrevLane(trainCurrentLane3.m_Front.m_Lane, out needSignal);
					}
					bool flag3 = (trainCurrentLane2.m_Front.m_LaneFlags & TrainLaneFlags.Exclusive) != 0;
					bool skipCurrent = false;
					if (!flag3 && val31.Length != 0)
					{
						skipCurrent = (val31[0].m_Flags & (TrainLaneFlags.Reserved | TrainLaneFlags.Exclusive)) == (TrainLaneFlags.Reserved | TrainLaneFlags.Exclusive);
					}
					trainLaneSpeedIterator.m_PrefabTrain = trainData;
					trainLaneSpeedIterator.m_PrefabObjectGeometry = prefabObjectGeometry3;
					trainLaneSpeedIterator.IteratePrevLane(trainCurrentLane2.m_RearCache.m_Lane, out needSignal);
					trainLaneSpeedIterator.IteratePrevLane(trainCurrentLane2.m_Rear.m_Lane, out needSignal);
					trainLaneSpeedIterator.IteratePrevLane(trainCurrentLane2.m_FrontCache.m_Lane, out needSignal);
					if (!trainLaneSpeedIterator.IterateFirstLane(trainCurrentLane2.m_Front.m_Lane, trainCurrentLane2.m_Front.m_CurvePosition, flag3, ignoreObstacles: false, skipCurrent, out needSignal))
					{
						if ((trainCurrentLane2.m_Front.m_LaneFlags & (TrainLaneFlags.EndOfPath | TrainLaneFlags.Return)) == 0)
						{
							int num10 = 0;
							while (num10 < val31.Length)
							{
								TrainNavigationLane trainNavigationLane2 = val31[num10];
								bool flag4 = trainNavigationLane2.m_Lane == trainCurrentLane2.m_Front.m_Lane;
								if ((trainNavigationLane2.m_Flags & (TrainLaneFlags.Reserved | TrainLaneFlags.Connection)) == 0)
								{
									while ((trainNavigationLane2.m_Flags & (TrainLaneFlags.EndOfPath | TrainLaneFlags.BlockReserve)) == 0 && ++num10 < val31.Length)
									{
										trainNavigationLane2 = val31[num10];
									}
									trainLaneSpeedIterator.IterateTarget(trainNavigationLane2.m_Lane, flag4);
								}
								else
								{
									if ((trainNavigationLane2.m_Flags & TrainLaneFlags.Connection) != 0)
									{
										trainLaneSpeedIterator.m_PrefabTrain.m_MaxSpeed = 277.77777f;
										trainLaneSpeedIterator.m_PrefabTrain.m_Acceleration = 277.77777f;
										trainLaneSpeedIterator.m_PrefabTrain.m_Braking = 277.77777f;
										trainLaneSpeedIterator.m_SpeedRange = new Bounds1(0f, 277.77777f);
									}
									float minOffset3 = math.select(-1f, trainCurrentLane2.m_Front.m_CurvePosition.z, flag4);
									if (!trainLaneSpeedIterator.IterateNextLane(trainNavigationLane2.m_Lane, trainNavigationLane2.m_CurvePosition, minOffset3, (trainNavigationLane2.m_Flags & TrainLaneFlags.Exclusive) != 0, flag4, out needSignal))
									{
										if ((trainNavigationLane2.m_Flags & (TrainLaneFlags.EndOfPath | TrainLaneFlags.Return)) != 0)
										{
											break;
										}
										num10++;
										continue;
									}
								}
								goto IL_273d;
							}
						}
						trainLaneSpeedIterator.IterateTarget();
					}
					goto IL_273d;
				}
			}
			goto IL_276c;
			IL_273d:
			if (trainLaneSpeedIterator.m_Blocker != Entity.Null)
			{
				DrawBlocker(trainLaneSpeedIterator.m_Blocker, trainLaneSpeedIterator.m_MaxSpeed / trainData.m_MaxSpeed);
			}
			goto IL_276c;
			IL_0b01:
			int num11 = 0;
			while (true)
			{
				if (num11 < val.Length)
				{
					CarNavigationLane carNavigationLane4 = val[num11];
					if ((carNavigationLane4.m_Flags & (Game.Vehicles.CarLaneFlags.TransformTarget | Game.Vehicles.CarLaneFlags.Area)) == 0)
					{
						if ((carNavigationLane4.m_Flags & Game.Vehicles.CarLaneFlags.Connection) != 0)
						{
							carLaneSpeedIterator.m_PrefabCar.m_MaxSpeed = 277.77777f;
							carLaneSpeedIterator.m_PrefabCar.m_Acceleration = 277.77777f;
							carLaneSpeedIterator.m_PrefabCar.m_Braking = 277.77777f;
							carLaneSpeedIterator.m_SpeedRange = new Bounds1(0f, 277.77777f);
						}
						else
						{
							if ((carCurrentLane.m_LaneFlags & Game.Vehicles.CarLaneFlags.Connection) != 0)
							{
								goto IL_0c7a;
							}
							if ((carNavigationLane4.m_Flags & Game.Vehicles.CarLaneFlags.Interruption) != 0)
							{
								carLaneSpeedIterator.m_PrefabCar.m_MaxSpeed = 3f;
							}
						}
						bool flag5 = (carNavigationLane4.m_Lane == carCurrentLane.m_Lane) | (carNavigationLane4.m_Lane == carCurrentLane.m_ChangeLane);
						float num12 = math.select(-1f, 2f, carNavigationLane4.m_CurvePosition.y < carNavigationLane4.m_CurvePosition.x);
						num12 = math.select(num12, carCurrentLane.m_CurvePosition.y, flag5);
						if (carLaneSpeedIterator.IterateNextLane(carNavigationLane4.m_Lane, carNavigationLane4.m_CurvePosition, num12, val.AsNativeArray().GetSubArray(num11 + 1, val.Length - 1 - num11), (carNavigationLane4.m_Flags & Game.Vehicles.CarLaneFlags.RequestSpace) != 0, ref laneFlags, out needSignal))
						{
							break;
						}
						num11++;
						continue;
					}
				}
				goto IL_0c7a;
				IL_0c7a:
				carLaneSpeedIterator.IterateTarget(carLaneSpeedIterator.m_CurrentPosition);
				break;
			}
			goto IL_0c88;
		}

		private void DrawBlocker(Entity blocker, float speedFactor)
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_0017: Unknown result type (might be due to invalid IL or missing references)
			//IL_0024: Unknown result type (might be due to invalid IL or missing references)
			//IL_0032: Unknown result type (might be due to invalid IL or missing references)
			//IL_0048: Unknown result type (might be due to invalid IL or missing references)
			//IL_0053: Unknown result type (might be due to invalid IL or missing references)
			//IL_0058: Unknown result type (might be due to invalid IL or missing references)
			//IL_0063: Unknown result type (might be due to invalid IL or missing references)
			//IL_0068: Unknown result type (might be due to invalid IL or missing references)
			//IL_006c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0072: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d9: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f6: Unknown result type (might be due to invalid IL or missing references)
			//IL_00fc: Unknown result type (might be due to invalid IL or missing references)
			//IL_0101: Unknown result type (might be due to invalid IL or missing references)
			//IL_008c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0094: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
			//IL_00af: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ca: Unknown result type (might be due to invalid IL or missing references)
			if (!m_TransformDataFromEntity.HasComponent(blocker))
			{
				return;
			}
			Transform transform = m_TransformDataFromEntity[blocker];
			PrefabRef prefabRef = m_PrefabRefDataFromEntity[blocker];
			if (m_PrefabObjectGeometryData.HasComponent(prefabRef.m_Prefab))
			{
				ObjectGeometryData objectGeometryData = m_PrefabObjectGeometryData[prefabRef.m_Prefab];
				Color val = Color.Lerp(Color.red, Color.yellow, math.saturate(speedFactor));
				float4x4 val2 = default(float4x4);
				((float4x4)(ref val2))._002Ector(transform.m_Rotation, transform.m_Position);
				if ((objectGeometryData.m_Flags & Game.Objects.GeometryFlags.Circular) != Game.Objects.GeometryFlags.None)
				{
					((GizmoBatcher)(ref m_GizmoBatcher)).DrawWireCylinder(val2, new float3(0f, objectGeometryData.m_Size.y * 0.5f, 0f), objectGeometryData.m_Size.x * 0.5f, objectGeometryData.m_Size.y, val, 36);
				}
				else
				{
					((GizmoBatcher)(ref m_GizmoBatcher)).DrawWireCube(val2, new float3(0f, objectGeometryData.m_Size.y * 0.5f, 0f), objectGeometryData.m_Size, val);
				}
			}
		}
	}

	private struct TypeHandle
	{
		[ReadOnly]
		public EntityTypeHandle __Unity_Entities_Entity_TypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<CarCurrentLane> __Game_Vehicles_CarCurrentLane_RO_ComponentTypeHandle;

		[ReadOnly]
		public BufferTypeHandle<CarNavigationLane> __Game_Vehicles_CarNavigationLane_RO_BufferTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<WatercraftCurrentLane> __Game_Vehicles_WatercraftCurrentLane_RO_ComponentTypeHandle;

		[ReadOnly]
		public BufferTypeHandle<WatercraftNavigationLane> __Game_Vehicles_WatercraftNavigationLane_RO_BufferTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<AircraftCurrentLane> __Game_Vehicles_AircraftCurrentLane_RO_ComponentTypeHandle;

		[ReadOnly]
		public BufferTypeHandle<AircraftNavigationLane> __Game_Vehicles_AircraftNavigationLane_RO_BufferTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<Train> __Game_Vehicles_Train_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<TrainCurrentLane> __Game_Vehicles_TrainCurrentLane_RO_ComponentTypeHandle;

		[ReadOnly]
		public BufferTypeHandle<TrainNavigationLane> __Game_Vehicles_TrainNavigationLane_RO_BufferTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<HumanCurrentLane> __Game_Creatures_HumanCurrentLane_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<AnimalCurrentLane> __Game_Creatures_AnimalCurrentLane_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<Transform> __Game_Objects_Transform_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<PrefabRef> __Game_Prefabs_PrefabRef_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentLookup<Transform> __Game_Objects_Transform_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Curve> __Game_Net_Curve_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<ObjectGeometryData> __Game_Prefabs_ObjectGeometryData_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<TrainData> __Game_Prefabs_TrainData_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<CarCurrentLane> __Game_Vehicles_CarCurrentLane_RO_ComponentLookup;

		[ReadOnly]
		public BufferLookup<CarNavigationLane> __Game_Vehicles_CarNavigationLane_RO_BufferLookup;

		[ReadOnly]
		public ComponentLookup<CarNavigation> __Game_Vehicles_CarNavigation_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<WatercraftCurrentLane> __Game_Vehicles_WatercraftCurrentLane_RO_ComponentLookup;

		[ReadOnly]
		public BufferLookup<WatercraftNavigationLane> __Game_Vehicles_WatercraftNavigationLane_RO_BufferLookup;

		[ReadOnly]
		public ComponentLookup<WatercraftNavigation> __Game_Vehicles_WatercraftNavigation_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<AircraftCurrentLane> __Game_Vehicles_AircraftCurrentLane_RO_ComponentLookup;

		[ReadOnly]
		public BufferLookup<AircraftNavigationLane> __Game_Vehicles_AircraftNavigationLane_RO_BufferLookup;

		[ReadOnly]
		public ComponentLookup<AircraftNavigation> __Game_Vehicles_AircraftNavigation_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<TrainCurrentLane> __Game_Vehicles_TrainCurrentLane_RO_ComponentLookup;

		[ReadOnly]
		public BufferLookup<TrainNavigationLane> __Game_Vehicles_TrainNavigationLane_RO_BufferLookup;

		[ReadOnly]
		public BufferLookup<LayoutElement> __Game_Vehicles_LayoutElement_RO_BufferLookup;

		[ReadOnly]
		public ComponentLookup<Blocker> __Game_Vehicles_Blocker_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<HumanCurrentLane> __Game_Creatures_HumanCurrentLane_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<AnimalCurrentLane> __Game_Creatures_AnimalCurrentLane_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Target> __Game_Common_Target_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<PseudoRandomSeed> __Game_Common_PseudoRandomSeed_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Lane> __Game_Net_Lane_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Game.Net.CarLane> __Game_Net_CarLane_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Game.Net.TrackLane> __Game_Net_TrackLane_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Game.Net.ParkingLane> __Game_Net_ParkingLane_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<LaneReservation> __Game_Net_LaneReservation_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<LaneCondition> __Game_Net_LaneCondition_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<LaneSignal> __Game_Net_LaneSignal_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<SlaveLane> __Game_Net_SlaveLane_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<AreaLane> __Game_Net_AreaLane_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Moving> __Game_Objects_Moving_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Car> __Game_Vehicles_Car_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Watercraft> __Game_Vehicles_Watercraft_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Aircraft> __Game_Vehicles_Aircraft_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Train> __Game_Vehicles_Train_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Controller> __Game_Vehicles_Controller_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Owner> __Game_Common_Owner_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Unspawned> __Game_Objects_Unspawned_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Creature> __Game_Creatures_Creature_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Position> __Game_Routes_Position_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<PrefabRef> __Game_Prefabs_PrefabRef_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<CarData> __Game_Prefabs_CarData_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<WatercraftData> __Game_Prefabs_WatercraftData_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<AircraftData> __Game_Prefabs_AircraftData_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<BuildingData> __Game_Prefabs_BuildingData_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<NetLaneData> __Game_Prefabs_NetLaneData_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<ParkingLaneData> __Game_Prefabs_ParkingLaneData_RO_ComponentLookup;

		[ReadOnly]
		public BufferLookup<LaneObject> __Game_Net_LaneObject_RO_BufferLookup;

		[ReadOnly]
		public BufferLookup<LaneOverlap> __Game_Net_LaneOverlap_RO_BufferLookup;

		[ReadOnly]
		public BufferLookup<Game.Net.SubLane> __Game_Net_SubLane_RO_BufferLookup;

		[ReadOnly]
		public BufferLookup<Game.Areas.Node> __Game_Areas_Node_RO_BufferLookup;

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
			//IL_0154: Unknown result type (might be due to invalid IL or missing references)
			//IL_0159: Unknown result type (might be due to invalid IL or missing references)
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
			//IL_01af: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b4: Unknown result type (might be due to invalid IL or missing references)
			//IL_01bc: Unknown result type (might be due to invalid IL or missing references)
			//IL_01c1: Unknown result type (might be due to invalid IL or missing references)
			//IL_01c9: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ce: Unknown result type (might be due to invalid IL or missing references)
			//IL_01d6: Unknown result type (might be due to invalid IL or missing references)
			//IL_01db: Unknown result type (might be due to invalid IL or missing references)
			//IL_01e3: Unknown result type (might be due to invalid IL or missing references)
			//IL_01e8: Unknown result type (might be due to invalid IL or missing references)
			//IL_01f0: Unknown result type (might be due to invalid IL or missing references)
			//IL_01f5: Unknown result type (might be due to invalid IL or missing references)
			//IL_01fd: Unknown result type (might be due to invalid IL or missing references)
			//IL_0202: Unknown result type (might be due to invalid IL or missing references)
			//IL_020a: Unknown result type (might be due to invalid IL or missing references)
			//IL_020f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0217: Unknown result type (might be due to invalid IL or missing references)
			//IL_021c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0224: Unknown result type (might be due to invalid IL or missing references)
			//IL_0229: Unknown result type (might be due to invalid IL or missing references)
			//IL_0231: Unknown result type (might be due to invalid IL or missing references)
			//IL_0236: Unknown result type (might be due to invalid IL or missing references)
			//IL_023e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0243: Unknown result type (might be due to invalid IL or missing references)
			//IL_024b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0250: Unknown result type (might be due to invalid IL or missing references)
			//IL_0258: Unknown result type (might be due to invalid IL or missing references)
			//IL_025d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0265: Unknown result type (might be due to invalid IL or missing references)
			//IL_026a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0272: Unknown result type (might be due to invalid IL or missing references)
			//IL_0277: Unknown result type (might be due to invalid IL or missing references)
			//IL_027f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0284: Unknown result type (might be due to invalid IL or missing references)
			//IL_028c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0291: Unknown result type (might be due to invalid IL or missing references)
			//IL_0299: Unknown result type (might be due to invalid IL or missing references)
			//IL_029e: Unknown result type (might be due to invalid IL or missing references)
			//IL_02a6: Unknown result type (might be due to invalid IL or missing references)
			//IL_02ab: Unknown result type (might be due to invalid IL or missing references)
			//IL_02b3: Unknown result type (might be due to invalid IL or missing references)
			//IL_02b8: Unknown result type (might be due to invalid IL or missing references)
			//IL_02c0: Unknown result type (might be due to invalid IL or missing references)
			//IL_02c5: Unknown result type (might be due to invalid IL or missing references)
			//IL_02cd: Unknown result type (might be due to invalid IL or missing references)
			//IL_02d2: Unknown result type (might be due to invalid IL or missing references)
			//IL_02da: Unknown result type (might be due to invalid IL or missing references)
			//IL_02df: Unknown result type (might be due to invalid IL or missing references)
			//IL_02e7: Unknown result type (might be due to invalid IL or missing references)
			//IL_02ec: Unknown result type (might be due to invalid IL or missing references)
			//IL_02f4: Unknown result type (might be due to invalid IL or missing references)
			//IL_02f9: Unknown result type (might be due to invalid IL or missing references)
			//IL_0301: Unknown result type (might be due to invalid IL or missing references)
			//IL_0306: Unknown result type (might be due to invalid IL or missing references)
			//IL_030e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0313: Unknown result type (might be due to invalid IL or missing references)
			//IL_031b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0320: Unknown result type (might be due to invalid IL or missing references)
			//IL_0328: Unknown result type (might be due to invalid IL or missing references)
			//IL_032d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0335: Unknown result type (might be due to invalid IL or missing references)
			//IL_033a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0342: Unknown result type (might be due to invalid IL or missing references)
			//IL_0347: Unknown result type (might be due to invalid IL or missing references)
			__Unity_Entities_Entity_TypeHandle = ((SystemState)(ref state)).GetEntityTypeHandle();
			__Game_Vehicles_CarCurrentLane_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<CarCurrentLane>(true);
			__Game_Vehicles_CarNavigationLane_RO_BufferTypeHandle = ((SystemState)(ref state)).GetBufferTypeHandle<CarNavigationLane>(true);
			__Game_Vehicles_WatercraftCurrentLane_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<WatercraftCurrentLane>(true);
			__Game_Vehicles_WatercraftNavigationLane_RO_BufferTypeHandle = ((SystemState)(ref state)).GetBufferTypeHandle<WatercraftNavigationLane>(true);
			__Game_Vehicles_AircraftCurrentLane_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<AircraftCurrentLane>(true);
			__Game_Vehicles_AircraftNavigationLane_RO_BufferTypeHandle = ((SystemState)(ref state)).GetBufferTypeHandle<AircraftNavigationLane>(true);
			__Game_Vehicles_Train_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<Train>(true);
			__Game_Vehicles_TrainCurrentLane_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<TrainCurrentLane>(true);
			__Game_Vehicles_TrainNavigationLane_RO_BufferTypeHandle = ((SystemState)(ref state)).GetBufferTypeHandle<TrainNavigationLane>(true);
			__Game_Creatures_HumanCurrentLane_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<HumanCurrentLane>(true);
			__Game_Creatures_AnimalCurrentLane_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<AnimalCurrentLane>(true);
			__Game_Objects_Transform_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<Transform>(true);
			__Game_Prefabs_PrefabRef_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<PrefabRef>(true);
			__Game_Objects_Transform_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Transform>(true);
			__Game_Net_Curve_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Curve>(true);
			__Game_Prefabs_ObjectGeometryData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<ObjectGeometryData>(true);
			__Game_Prefabs_TrainData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<TrainData>(true);
			__Game_Vehicles_CarCurrentLane_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<CarCurrentLane>(true);
			__Game_Vehicles_CarNavigationLane_RO_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<CarNavigationLane>(true);
			__Game_Vehicles_CarNavigation_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<CarNavigation>(true);
			__Game_Vehicles_WatercraftCurrentLane_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<WatercraftCurrentLane>(true);
			__Game_Vehicles_WatercraftNavigationLane_RO_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<WatercraftNavigationLane>(true);
			__Game_Vehicles_WatercraftNavigation_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<WatercraftNavigation>(true);
			__Game_Vehicles_AircraftCurrentLane_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<AircraftCurrentLane>(true);
			__Game_Vehicles_AircraftNavigationLane_RO_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<AircraftNavigationLane>(true);
			__Game_Vehicles_AircraftNavigation_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<AircraftNavigation>(true);
			__Game_Vehicles_TrainCurrentLane_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<TrainCurrentLane>(true);
			__Game_Vehicles_TrainNavigationLane_RO_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<TrainNavigationLane>(true);
			__Game_Vehicles_LayoutElement_RO_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<LayoutElement>(true);
			__Game_Vehicles_Blocker_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Blocker>(true);
			__Game_Creatures_HumanCurrentLane_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<HumanCurrentLane>(true);
			__Game_Creatures_AnimalCurrentLane_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<AnimalCurrentLane>(true);
			__Game_Common_Target_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Target>(true);
			__Game_Common_PseudoRandomSeed_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<PseudoRandomSeed>(true);
			__Game_Net_Lane_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Lane>(true);
			__Game_Net_CarLane_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Game.Net.CarLane>(true);
			__Game_Net_TrackLane_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Game.Net.TrackLane>(true);
			__Game_Net_ParkingLane_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Game.Net.ParkingLane>(true);
			__Game_Net_LaneReservation_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<LaneReservation>(true);
			__Game_Net_LaneCondition_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<LaneCondition>(true);
			__Game_Net_LaneSignal_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<LaneSignal>(true);
			__Game_Net_SlaveLane_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<SlaveLane>(true);
			__Game_Net_AreaLane_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<AreaLane>(true);
			__Game_Objects_Moving_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Moving>(true);
			__Game_Vehicles_Car_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Car>(true);
			__Game_Vehicles_Watercraft_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Watercraft>(true);
			__Game_Vehicles_Aircraft_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Aircraft>(true);
			__Game_Vehicles_Train_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Train>(true);
			__Game_Vehicles_Controller_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Controller>(true);
			__Game_Common_Owner_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Owner>(true);
			__Game_Objects_Unspawned_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Unspawned>(true);
			__Game_Creatures_Creature_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Creature>(true);
			__Game_Routes_Position_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Position>(true);
			__Game_Prefabs_PrefabRef_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<PrefabRef>(true);
			__Game_Prefabs_CarData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<CarData>(true);
			__Game_Prefabs_WatercraftData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<WatercraftData>(true);
			__Game_Prefabs_AircraftData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<AircraftData>(true);
			__Game_Prefabs_BuildingData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<BuildingData>(true);
			__Game_Prefabs_NetLaneData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<NetLaneData>(true);
			__Game_Prefabs_ParkingLaneData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<ParkingLaneData>(true);
			__Game_Net_LaneObject_RO_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<LaneObject>(true);
			__Game_Net_LaneOverlap_RO_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<LaneOverlap>(true);
			__Game_Net_SubLane_RO_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<Game.Net.SubLane>(true);
			__Game_Areas_Node_RO_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<Game.Areas.Node>(true);
		}
	}

	private EntityQuery m_NavigationQuery;

	private GizmosSystem m_GizmosSystem;

	private SimulationSystem m_SimulationSystem;

	private ToolSystem m_ToolSystem;

	private Option m_HumanOption;

	private Option m_AnimalOption;

	private Option m_CarOption;

	private Option m_TrainOption;

	private Option m_WatercraftOption;

	private Option m_AircraftOption;

	private TypeHandle __TypeHandle;

	[Preserve]
	protected override void OnCreate()
	{
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Expected O, but got Unknown
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		//IL_0076: Unknown result type (might be due to invalid IL or missing references)
		//IL_007b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0082: Unknown result type (might be due to invalid IL or missing references)
		//IL_0087: Unknown result type (might be due to invalid IL or missing references)
		//IL_008e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0093: Unknown result type (might be due to invalid IL or missing references)
		//IL_009a: Unknown result type (might be due to invalid IL or missing references)
		//IL_009f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00db: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ec: Unknown result type (might be due to invalid IL or missing references)
		base.OnCreate();
		m_GizmosSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<GizmosSystem>();
		m_SimulationSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<SimulationSystem>();
		m_ToolSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<ToolSystem>();
		EntityQueryDesc[] array = new EntityQueryDesc[1];
		EntityQueryDesc val = new EntityQueryDesc();
		val.All = (ComponentType[])(object)new ComponentType[2]
		{
			ComponentType.ReadOnly<Object>(),
			ComponentType.ReadOnly<Moving>()
		};
		val.Any = (ComponentType[])(object)new ComponentType[6]
		{
			ComponentType.ReadOnly<CarCurrentLane>(),
			ComponentType.ReadOnly<TrainCurrentLane>(),
			ComponentType.ReadOnly<HumanCurrentLane>(),
			ComponentType.ReadOnly<WatercraftCurrentLane>(),
			ComponentType.ReadOnly<AircraftCurrentLane>(),
			ComponentType.ReadOnly<AnimalCurrentLane>()
		};
		val.None = (ComponentType[])(object)new ComponentType[2]
		{
			ComponentType.ReadOnly<Temp>(),
			ComponentType.ReadOnly<Deleted>()
		};
		array[0] = val;
		m_NavigationQuery = ((ComponentSystemBase)this).GetEntityQuery((EntityQueryDesc[])(object)array);
		m_HumanOption = AddOption("Humans", defaultEnabled: true);
		m_AnimalOption = AddOption("Animals", defaultEnabled: true);
		m_CarOption = AddOption("Cars", defaultEnabled: true);
		m_TrainOption = AddOption("Trains", defaultEnabled: true);
		m_WatercraftOption = AddOption("Ships", defaultEnabled: true);
		m_AircraftOption = AddOption("Aircrafts", defaultEnabled: true);
		((ComponentSystemBase)this).Enabled = false;
	}

	[Preserve]
	protected override void OnUpdate()
	{
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		if (!((EntityQuery)(ref m_NavigationQuery)).IsEmptyIgnoreFilter)
		{
			if (m_ToolSystem.selected != Entity.Null)
			{
				((SystemBase)this).Dependency = JobHandle.CombineDependencies(DrawNavigationGizmos(m_NavigationQuery, ((SystemBase)this).Dependency), DrawSelectedGizmos(((SystemBase)this).Dependency));
			}
			else
			{
				((SystemBase)this).Dependency = DrawNavigationGizmos(m_NavigationQuery, ((SystemBase)this).Dependency);
			}
		}
	}

	private JobHandle DrawNavigationGizmos(EntityQuery group, JobHandle inputDeps)
	{
		//IL_0088: Unknown result type (might be due to invalid IL or missing references)
		//IL_008d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00df: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0101: Unknown result type (might be due to invalid IL or missing references)
		//IL_0119: Unknown result type (might be due to invalid IL or missing references)
		//IL_011e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0136: Unknown result type (might be due to invalid IL or missing references)
		//IL_013b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0153: Unknown result type (might be due to invalid IL or missing references)
		//IL_0158: Unknown result type (might be due to invalid IL or missing references)
		//IL_0170: Unknown result type (might be due to invalid IL or missing references)
		//IL_0175: Unknown result type (might be due to invalid IL or missing references)
		//IL_018d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0192: Unknown result type (might be due to invalid IL or missing references)
		//IL_01aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_01af: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_0201: Unknown result type (might be due to invalid IL or missing references)
		//IL_0206: Unknown result type (might be due to invalid IL or missing references)
		//IL_021e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0223: Unknown result type (might be due to invalid IL or missing references)
		//IL_023b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0240: Unknown result type (might be due to invalid IL or missing references)
		//IL_0258: Unknown result type (might be due to invalid IL or missing references)
		//IL_025d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0275: Unknown result type (might be due to invalid IL or missing references)
		//IL_027a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0292: Unknown result type (might be due to invalid IL or missing references)
		//IL_0297: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_02be: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_02cb: Unknown result type (might be due to invalid IL or missing references)
		JobHandle val2 = default(JobHandle);
		JobHandle val = JobChunkExtensions.ScheduleParallel<NavigationGizmoJob>(new NavigationGizmoJob
		{
			m_HumanOption = m_HumanOption.enabled,
			m_AnimalOption = m_AnimalOption.enabled,
			m_CarOption = m_CarOption.enabled,
			m_TrainOption = m_TrainOption.enabled,
			m_WatercraftOption = m_WatercraftOption.enabled,
			m_AircraftOption = m_AircraftOption.enabled,
			m_TimeOffset = Time.realtimeSinceStartup,
			m_Selected = m_ToolSystem.selected,
			m_EntityType = InternalCompilerInterface.GetEntityTypeHandle(ref __TypeHandle.__Unity_Entities_Entity_TypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_CarCurrentLaneType = InternalCompilerInterface.GetComponentTypeHandle<CarCurrentLane>(ref __TypeHandle.__Game_Vehicles_CarCurrentLane_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_CarNavigationLaneType = InternalCompilerInterface.GetBufferTypeHandle<CarNavigationLane>(ref __TypeHandle.__Game_Vehicles_CarNavigationLane_RO_BufferTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_WatercraftCurrentLaneType = InternalCompilerInterface.GetComponentTypeHandle<WatercraftCurrentLane>(ref __TypeHandle.__Game_Vehicles_WatercraftCurrentLane_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_WatercraftNavigationLaneType = InternalCompilerInterface.GetBufferTypeHandle<WatercraftNavigationLane>(ref __TypeHandle.__Game_Vehicles_WatercraftNavigationLane_RO_BufferTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_AircraftCurrentLaneType = InternalCompilerInterface.GetComponentTypeHandle<AircraftCurrentLane>(ref __TypeHandle.__Game_Vehicles_AircraftCurrentLane_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_AircraftNavigationLaneType = InternalCompilerInterface.GetBufferTypeHandle<AircraftNavigationLane>(ref __TypeHandle.__Game_Vehicles_AircraftNavigationLane_RO_BufferTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_TrainType = InternalCompilerInterface.GetComponentTypeHandle<Train>(ref __TypeHandle.__Game_Vehicles_Train_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_TrainCurrentLaneType = InternalCompilerInterface.GetComponentTypeHandle<TrainCurrentLane>(ref __TypeHandle.__Game_Vehicles_TrainCurrentLane_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_TrainNavigationLaneType = InternalCompilerInterface.GetBufferTypeHandle<TrainNavigationLane>(ref __TypeHandle.__Game_Vehicles_TrainNavigationLane_RO_BufferTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_HumanCurrentLaneType = InternalCompilerInterface.GetComponentTypeHandle<HumanCurrentLane>(ref __TypeHandle.__Game_Creatures_HumanCurrentLane_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_AnimalCurrentLaneType = InternalCompilerInterface.GetComponentTypeHandle<AnimalCurrentLane>(ref __TypeHandle.__Game_Creatures_AnimalCurrentLane_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_TransformType = InternalCompilerInterface.GetComponentTypeHandle<Transform>(ref __TypeHandle.__Game_Objects_Transform_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_PrefabRefType = InternalCompilerInterface.GetComponentTypeHandle<PrefabRef>(ref __TypeHandle.__Game_Prefabs_PrefabRef_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_TransformData = InternalCompilerInterface.GetComponentLookup<Transform>(ref __TypeHandle.__Game_Objects_Transform_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_CurveData = InternalCompilerInterface.GetComponentLookup<Curve>(ref __TypeHandle.__Game_Net_Curve_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_PrefabObjectGeometryData = InternalCompilerInterface.GetComponentLookup<ObjectGeometryData>(ref __TypeHandle.__Game_Prefabs_ObjectGeometryData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_PrefabTrainData = InternalCompilerInterface.GetComponentLookup<TrainData>(ref __TypeHandle.__Game_Prefabs_TrainData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_GizmoBatcher = m_GizmosSystem.GetGizmosBatcher(ref val2)
		}, group, JobHandle.CombineDependencies(inputDeps, val2));
		m_GizmosSystem.AddGizmosBatcherWriter(val);
		return val;
	}

	private JobHandle DrawSelectedGizmos(JobHandle inputDeps)
	{
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0084: Unknown result type (might be due to invalid IL or missing references)
		//IL_0089: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00be: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00db: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fd: Unknown result type (might be due to invalid IL or missing references)
		//IL_0115: Unknown result type (might be due to invalid IL or missing references)
		//IL_011a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0132: Unknown result type (might be due to invalid IL or missing references)
		//IL_0137: Unknown result type (might be due to invalid IL or missing references)
		//IL_014f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0154: Unknown result type (might be due to invalid IL or missing references)
		//IL_016c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0171: Unknown result type (might be due to invalid IL or missing references)
		//IL_0189: Unknown result type (might be due to invalid IL or missing references)
		//IL_018e: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01fd: Unknown result type (might be due to invalid IL or missing references)
		//IL_0202: Unknown result type (might be due to invalid IL or missing references)
		//IL_021a: Unknown result type (might be due to invalid IL or missing references)
		//IL_021f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0237: Unknown result type (might be due to invalid IL or missing references)
		//IL_023c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0254: Unknown result type (might be due to invalid IL or missing references)
		//IL_0259: Unknown result type (might be due to invalid IL or missing references)
		//IL_0271: Unknown result type (might be due to invalid IL or missing references)
		//IL_0276: Unknown result type (might be due to invalid IL or missing references)
		//IL_028e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0293: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_02cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e5: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_0302: Unknown result type (might be due to invalid IL or missing references)
		//IL_0307: Unknown result type (might be due to invalid IL or missing references)
		//IL_031f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0324: Unknown result type (might be due to invalid IL or missing references)
		//IL_033c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0341: Unknown result type (might be due to invalid IL or missing references)
		//IL_0359: Unknown result type (might be due to invalid IL or missing references)
		//IL_035e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0376: Unknown result type (might be due to invalid IL or missing references)
		//IL_037b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0393: Unknown result type (might be due to invalid IL or missing references)
		//IL_0398: Unknown result type (might be due to invalid IL or missing references)
		//IL_03b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_03b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_03cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_03d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_03ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_03ef: Unknown result type (might be due to invalid IL or missing references)
		//IL_0407: Unknown result type (might be due to invalid IL or missing references)
		//IL_040c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0424: Unknown result type (might be due to invalid IL or missing references)
		//IL_0429: Unknown result type (might be due to invalid IL or missing references)
		//IL_0441: Unknown result type (might be due to invalid IL or missing references)
		//IL_0446: Unknown result type (might be due to invalid IL or missing references)
		//IL_045e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0463: Unknown result type (might be due to invalid IL or missing references)
		//IL_047b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0480: Unknown result type (might be due to invalid IL or missing references)
		//IL_0498: Unknown result type (might be due to invalid IL or missing references)
		//IL_049d: Unknown result type (might be due to invalid IL or missing references)
		//IL_04b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_04ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_04d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_04d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_04ef: Unknown result type (might be due to invalid IL or missing references)
		//IL_04f4: Unknown result type (might be due to invalid IL or missing references)
		//IL_050c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0511: Unknown result type (might be due to invalid IL or missing references)
		//IL_0529: Unknown result type (might be due to invalid IL or missing references)
		//IL_052e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0546: Unknown result type (might be due to invalid IL or missing references)
		//IL_054b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0563: Unknown result type (might be due to invalid IL or missing references)
		//IL_0568: Unknown result type (might be due to invalid IL or missing references)
		//IL_0580: Unknown result type (might be due to invalid IL or missing references)
		//IL_0585: Unknown result type (might be due to invalid IL or missing references)
		//IL_059d: Unknown result type (might be due to invalid IL or missing references)
		//IL_05a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_05ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_05bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_05d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_05dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_05f4: Unknown result type (might be due to invalid IL or missing references)
		//IL_05f9: Unknown result type (might be due to invalid IL or missing references)
		//IL_0608: Unknown result type (might be due to invalid IL or missing references)
		//IL_060d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0625: Unknown result type (might be due to invalid IL or missing references)
		//IL_0626: Unknown result type (might be due to invalid IL or missing references)
		//IL_0627: Unknown result type (might be due to invalid IL or missing references)
		//IL_062c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0631: Unknown result type (might be due to invalid IL or missing references)
		//IL_0638: Unknown result type (might be due to invalid IL or missing references)
		//IL_063e: Unknown result type (might be due to invalid IL or missing references)
		JobHandle val2 = default(JobHandle);
		JobHandle val = IJobExtensions.Schedule<SelectedNavigationGizmoJob>(new SelectedNavigationGizmoJob
		{
			m_Selected = m_ToolSystem.selected,
			m_CarCurrentLaneType = InternalCompilerInterface.GetComponentLookup<CarCurrentLane>(ref __TypeHandle.__Game_Vehicles_CarCurrentLane_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_CarNavigationLaneType = InternalCompilerInterface.GetBufferLookup<CarNavigationLane>(ref __TypeHandle.__Game_Vehicles_CarNavigationLane_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
			m_CarNavigationType = InternalCompilerInterface.GetComponentLookup<CarNavigation>(ref __TypeHandle.__Game_Vehicles_CarNavigation_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_WatercraftCurrentLaneType = InternalCompilerInterface.GetComponentLookup<WatercraftCurrentLane>(ref __TypeHandle.__Game_Vehicles_WatercraftCurrentLane_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_WatercraftNavigationLaneType = InternalCompilerInterface.GetBufferLookup<WatercraftNavigationLane>(ref __TypeHandle.__Game_Vehicles_WatercraftNavigationLane_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
			m_WatercraftNavigationType = InternalCompilerInterface.GetComponentLookup<WatercraftNavigation>(ref __TypeHandle.__Game_Vehicles_WatercraftNavigation_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_AircraftCurrentLaneType = InternalCompilerInterface.GetComponentLookup<AircraftCurrentLane>(ref __TypeHandle.__Game_Vehicles_AircraftCurrentLane_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_AircraftNavigationLaneType = InternalCompilerInterface.GetBufferLookup<AircraftNavigationLane>(ref __TypeHandle.__Game_Vehicles_AircraftNavigationLane_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
			m_AircraftNavigationType = InternalCompilerInterface.GetComponentLookup<AircraftNavigation>(ref __TypeHandle.__Game_Vehicles_AircraftNavigation_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_TrainCurrentLaneType = InternalCompilerInterface.GetComponentLookup<TrainCurrentLane>(ref __TypeHandle.__Game_Vehicles_TrainCurrentLane_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_TrainNavigationLaneType = InternalCompilerInterface.GetBufferLookup<TrainNavigationLane>(ref __TypeHandle.__Game_Vehicles_TrainNavigationLane_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
			m_LayoutElementType = InternalCompilerInterface.GetBufferLookup<LayoutElement>(ref __TypeHandle.__Game_Vehicles_LayoutElement_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
			m_BlockerType = InternalCompilerInterface.GetComponentLookup<Blocker>(ref __TypeHandle.__Game_Vehicles_Blocker_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_HumanCurrentLaneType = InternalCompilerInterface.GetComponentLookup<HumanCurrentLane>(ref __TypeHandle.__Game_Creatures_HumanCurrentLane_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_AnimalCurrentLaneType = InternalCompilerInterface.GetComponentLookup<AnimalCurrentLane>(ref __TypeHandle.__Game_Creatures_AnimalCurrentLane_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_TargetType = InternalCompilerInterface.GetComponentLookup<Target>(ref __TypeHandle.__Game_Common_Target_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_PseudoRandomSeedType = InternalCompilerInterface.GetComponentLookup<PseudoRandomSeed>(ref __TypeHandle.__Game_Common_PseudoRandomSeed_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_CurveData = InternalCompilerInterface.GetComponentLookup<Curve>(ref __TypeHandle.__Game_Net_Curve_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_LaneData = InternalCompilerInterface.GetComponentLookup<Lane>(ref __TypeHandle.__Game_Net_Lane_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_CarLaneData = InternalCompilerInterface.GetComponentLookup<Game.Net.CarLane>(ref __TypeHandle.__Game_Net_CarLane_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_TrackLaneData = InternalCompilerInterface.GetComponentLookup<Game.Net.TrackLane>(ref __TypeHandle.__Game_Net_TrackLane_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_ParkingLaneData = InternalCompilerInterface.GetComponentLookup<Game.Net.ParkingLane>(ref __TypeHandle.__Game_Net_ParkingLane_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_LaneReservationData = InternalCompilerInterface.GetComponentLookup<LaneReservation>(ref __TypeHandle.__Game_Net_LaneReservation_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_LaneConditionData = InternalCompilerInterface.GetComponentLookup<LaneCondition>(ref __TypeHandle.__Game_Net_LaneCondition_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_LaneSignalData = InternalCompilerInterface.GetComponentLookup<LaneSignal>(ref __TypeHandle.__Game_Net_LaneSignal_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_SlaveLaneData = InternalCompilerInterface.GetComponentLookup<SlaveLane>(ref __TypeHandle.__Game_Net_SlaveLane_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_AreaLaneData = InternalCompilerInterface.GetComponentLookup<AreaLane>(ref __TypeHandle.__Game_Net_AreaLane_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_TransformDataFromEntity = InternalCompilerInterface.GetComponentLookup<Transform>(ref __TypeHandle.__Game_Objects_Transform_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_MovingDataFromEntity = InternalCompilerInterface.GetComponentLookup<Moving>(ref __TypeHandle.__Game_Objects_Moving_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_CarDataFromEntity = InternalCompilerInterface.GetComponentLookup<Car>(ref __TypeHandle.__Game_Vehicles_Car_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_WatercraftDataFromEntity = InternalCompilerInterface.GetComponentLookup<Watercraft>(ref __TypeHandle.__Game_Vehicles_Watercraft_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_AircraftDataFromEntity = InternalCompilerInterface.GetComponentLookup<Aircraft>(ref __TypeHandle.__Game_Vehicles_Aircraft_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_TrainDataFromEntity = InternalCompilerInterface.GetComponentLookup<Train>(ref __TypeHandle.__Game_Vehicles_Train_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_ControllerDataFromEntity = InternalCompilerInterface.GetComponentLookup<Controller>(ref __TypeHandle.__Game_Vehicles_Controller_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_OwnerDataFromEntity = InternalCompilerInterface.GetComponentLookup<Owner>(ref __TypeHandle.__Game_Common_Owner_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_UnspawnedData = InternalCompilerInterface.GetComponentLookup<Unspawned>(ref __TypeHandle.__Game_Objects_Unspawned_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_TrainCurrentLaneData = InternalCompilerInterface.GetComponentLookup<TrainCurrentLane>(ref __TypeHandle.__Game_Vehicles_TrainCurrentLane_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_CreatureData = InternalCompilerInterface.GetComponentLookup<Creature>(ref __TypeHandle.__Game_Creatures_Creature_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_PositionData = InternalCompilerInterface.GetComponentLookup<Position>(ref __TypeHandle.__Game_Routes_Position_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_PrefabRefDataFromEntity = InternalCompilerInterface.GetComponentLookup<PrefabRef>(ref __TypeHandle.__Game_Prefabs_PrefabRef_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_PrefabCarData = InternalCompilerInterface.GetComponentLookup<CarData>(ref __TypeHandle.__Game_Prefabs_CarData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_PrefabWatercraftData = InternalCompilerInterface.GetComponentLookup<WatercraftData>(ref __TypeHandle.__Game_Prefabs_WatercraftData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_PrefabAircraftData = InternalCompilerInterface.GetComponentLookup<AircraftData>(ref __TypeHandle.__Game_Prefabs_AircraftData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_PrefabTrainData = InternalCompilerInterface.GetComponentLookup<TrainData>(ref __TypeHandle.__Game_Prefabs_TrainData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_PrefabObjectGeometryData = InternalCompilerInterface.GetComponentLookup<ObjectGeometryData>(ref __TypeHandle.__Game_Prefabs_ObjectGeometryData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_PrefabBuildingData = InternalCompilerInterface.GetComponentLookup<BuildingData>(ref __TypeHandle.__Game_Prefabs_BuildingData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_PrefabNetLaneData = InternalCompilerInterface.GetComponentLookup<NetLaneData>(ref __TypeHandle.__Game_Prefabs_NetLaneData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_PrefabParkingLaneData = InternalCompilerInterface.GetComponentLookup<ParkingLaneData>(ref __TypeHandle.__Game_Prefabs_ParkingLaneData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_LaneObjects = InternalCompilerInterface.GetBufferLookup<LaneObject>(ref __TypeHandle.__Game_Net_LaneObject_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
			m_LaneOverlaps = InternalCompilerInterface.GetBufferLookup<LaneOverlap>(ref __TypeHandle.__Game_Net_LaneOverlap_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
			m_SubLanes = InternalCompilerInterface.GetBufferLookup<Game.Net.SubLane>(ref __TypeHandle.__Game_Net_SubLane_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
			m_AreaNodes = InternalCompilerInterface.GetBufferLookup<Game.Areas.Node>(ref __TypeHandle.__Game_Areas_Node_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
			m_GizmoBatcher = m_GizmosSystem.GetGizmosBatcher(ref val2),
			m_SimulationFrame = m_SimulationSystem.frameIndex
		}, JobHandle.CombineDependencies(inputDeps, val2));
		m_GizmosSystem.AddGizmosBatcherWriter(val);
		return val;
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
	public NavigationDebugSystem()
	{
	}
}

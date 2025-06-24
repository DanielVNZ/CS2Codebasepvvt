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
public class AircraftMoveSystem : GameSystemBase
{
	[BurstCompile]
	private struct AircraftMoveJob : IJobChunk
	{
		[ReadOnly]
		public EntityTypeHandle m_EntityType;

		[ReadOnly]
		public ComponentTypeHandle<Aircraft> m_AircraftType;

		[ReadOnly]
		public ComponentTypeHandle<Helicopter> m_HelicopterType;

		[ReadOnly]
		public ComponentTypeHandle<AircraftNavigation> m_NavigationType;

		[ReadOnly]
		public ComponentTypeHandle<AircraftCurrentLane> m_CurrentLaneType;

		[ReadOnly]
		public ComponentTypeHandle<PseudoRandomSeed> m_PseudoRandomSeedType;

		[ReadOnly]
		public ComponentTypeHandle<PrefabRef> m_PrefabRefType;

		public ComponentTypeHandle<Moving> m_MovingType;

		public ComponentTypeHandle<Transform> m_TransformType;

		public BufferTypeHandle<TransformFrame> m_TransformFrameType;

		[ReadOnly]
		public uint m_TransformFrameIndex;

		[ReadOnly]
		public float m_DayLightBrightness;

		[ReadOnly]
		public ComponentLookup<HelicopterData> m_PrefabHelicopterData;

		[ReadOnly]
		public ComponentLookup<AirplaneData> m_PrefabAirplaneData;

		public ParallelWriter m_CommandBuffer;

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
			//IL_072d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0732: Unknown result type (might be due to invalid IL or missing references)
			//IL_073c: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ee: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f3: Unknown result type (might be due to invalid IL or missing references)
			//IL_00fd: Unknown result type (might be due to invalid IL or missing references)
			//IL_0cc2: Unknown result type (might be due to invalid IL or missing references)
			//IL_0cc9: Unknown result type (might be due to invalid IL or missing references)
			//IL_0cce: Unknown result type (might be due to invalid IL or missing references)
			//IL_0cd3: Unknown result type (might be due to invalid IL or missing references)
			//IL_0ce4: Unknown result type (might be due to invalid IL or missing references)
			//IL_0ceb: Unknown result type (might be due to invalid IL or missing references)
			//IL_0cf2: Unknown result type (might be due to invalid IL or missing references)
			//IL_0cf7: Unknown result type (might be due to invalid IL or missing references)
			//IL_0cfc: Unknown result type (might be due to invalid IL or missing references)
			//IL_0d0f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0d11: Unknown result type (might be due to invalid IL or missing references)
			//IL_0d18: Unknown result type (might be due to invalid IL or missing references)
			//IL_0d25: Unknown result type (might be due to invalid IL or missing references)
			//IL_0d2a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0d2e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0d33: Unknown result type (might be due to invalid IL or missing references)
			//IL_0d35: Unknown result type (might be due to invalid IL or missing references)
			//IL_0d3a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0d3e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0d43: Unknown result type (might be due to invalid IL or missing references)
			//IL_0d47: Unknown result type (might be due to invalid IL or missing references)
			//IL_0d4c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0764: Unknown result type (might be due to invalid IL or missing references)
			//IL_0778: Unknown result type (might be due to invalid IL or missing references)
			//IL_077d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0782: Unknown result type (might be due to invalid IL or missing references)
			//IL_078b: Unknown result type (might be due to invalid IL or missing references)
			//IL_079a: Unknown result type (might be due to invalid IL or missing references)
			//IL_079f: Unknown result type (might be due to invalid IL or missing references)
			//IL_07a4: Unknown result type (might be due to invalid IL or missing references)
			//IL_07ad: Unknown result type (might be due to invalid IL or missing references)
			//IL_07b2: Unknown result type (might be due to invalid IL or missing references)
			//IL_07b6: Unknown result type (might be due to invalid IL or missing references)
			//IL_07bd: Unknown result type (might be due to invalid IL or missing references)
			//IL_07c2: Unknown result type (might be due to invalid IL or missing references)
			//IL_07c7: Unknown result type (might be due to invalid IL or missing references)
			//IL_04c5: Unknown result type (might be due to invalid IL or missing references)
			//IL_04cc: Unknown result type (might be due to invalid IL or missing references)
			//IL_04d1: Unknown result type (might be due to invalid IL or missing references)
			//IL_04d6: Unknown result type (might be due to invalid IL or missing references)
			//IL_04e7: Unknown result type (might be due to invalid IL or missing references)
			//IL_04ee: Unknown result type (might be due to invalid IL or missing references)
			//IL_04f5: Unknown result type (might be due to invalid IL or missing references)
			//IL_04fa: Unknown result type (might be due to invalid IL or missing references)
			//IL_04ff: Unknown result type (might be due to invalid IL or missing references)
			//IL_0512: Unknown result type (might be due to invalid IL or missing references)
			//IL_0514: Unknown result type (might be due to invalid IL or missing references)
			//IL_051b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0528: Unknown result type (might be due to invalid IL or missing references)
			//IL_052d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0531: Unknown result type (might be due to invalid IL or missing references)
			//IL_0536: Unknown result type (might be due to invalid IL or missing references)
			//IL_0538: Unknown result type (might be due to invalid IL or missing references)
			//IL_053d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0541: Unknown result type (might be due to invalid IL or missing references)
			//IL_0546: Unknown result type (might be due to invalid IL or missing references)
			//IL_054a: Unknown result type (might be due to invalid IL or missing references)
			//IL_054f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0125: Unknown result type (might be due to invalid IL or missing references)
			//IL_012c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0131: Unknown result type (might be due to invalid IL or missing references)
			//IL_0136: Unknown result type (might be due to invalid IL or missing references)
			//IL_0138: Unknown result type (might be due to invalid IL or missing references)
			//IL_013c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0142: Unknown result type (might be due to invalid IL or missing references)
			//IL_0144: Unknown result type (might be due to invalid IL or missing references)
			//IL_0149: Unknown result type (might be due to invalid IL or missing references)
			//IL_014b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0d69: Unknown result type (might be due to invalid IL or missing references)
			//IL_0d70: Unknown result type (might be due to invalid IL or missing references)
			//IL_0d75: Unknown result type (might be due to invalid IL or missing references)
			//IL_0d7a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0d57: Unknown result type (might be due to invalid IL or missing references)
			//IL_0d59: Unknown result type (might be due to invalid IL or missing references)
			//IL_0d5e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0d63: Unknown result type (might be due to invalid IL or missing references)
			//IL_07e6: Unknown result type (might be due to invalid IL or missing references)
			//IL_07e8: Unknown result type (might be due to invalid IL or missing references)
			//IL_07f1: Unknown result type (might be due to invalid IL or missing references)
			//IL_07f3: Unknown result type (might be due to invalid IL or missing references)
			//IL_07f8: Unknown result type (might be due to invalid IL or missing references)
			//IL_07fa: Unknown result type (might be due to invalid IL or missing references)
			//IL_07fe: Unknown result type (might be due to invalid IL or missing references)
			//IL_080c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0811: Unknown result type (might be due to invalid IL or missing references)
			//IL_056c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0573: Unknown result type (might be due to invalid IL or missing references)
			//IL_0578: Unknown result type (might be due to invalid IL or missing references)
			//IL_057d: Unknown result type (might be due to invalid IL or missing references)
			//IL_057f: Unknown result type (might be due to invalid IL or missing references)
			//IL_055a: Unknown result type (might be due to invalid IL or missing references)
			//IL_055c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0561: Unknown result type (might be due to invalid IL or missing references)
			//IL_0566: Unknown result type (might be due to invalid IL or missing references)
			//IL_01aa: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b3: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b8: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ba: Unknown result type (might be due to invalid IL or missing references)
			//IL_01be: Unknown result type (might be due to invalid IL or missing references)
			//IL_01c3: Unknown result type (might be due to invalid IL or missing references)
			//IL_01c8: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ca: Unknown result type (might be due to invalid IL or missing references)
			//IL_01d6: Unknown result type (might be due to invalid IL or missing references)
			//IL_01db: Unknown result type (might be due to invalid IL or missing references)
			//IL_01e5: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ea: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ec: Unknown result type (might be due to invalid IL or missing references)
			//IL_01f1: Unknown result type (might be due to invalid IL or missing references)
			//IL_0213: Unknown result type (might be due to invalid IL or missing references)
			//IL_0180: Unknown result type (might be due to invalid IL or missing references)
			//IL_0187: Unknown result type (might be due to invalid IL or missing references)
			//IL_018d: Unknown result type (might be due to invalid IL or missing references)
			//IL_018f: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a0: Unknown result type (might be due to invalid IL or missing references)
			//IL_0d9c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0d85: Unknown result type (might be due to invalid IL or missing references)
			//IL_0d87: Unknown result type (might be due to invalid IL or missing references)
			//IL_0d8c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0d91: Unknown result type (might be due to invalid IL or missing references)
			//IL_0914: Unknown result type (might be due to invalid IL or missing references)
			//IL_0916: Unknown result type (might be due to invalid IL or missing references)
			//IL_0946: Unknown result type (might be due to invalid IL or missing references)
			//IL_0948: Unknown result type (might be due to invalid IL or missing references)
			//IL_094a: Unknown result type (might be due to invalid IL or missing references)
			//IL_094c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0953: Unknown result type (might be due to invalid IL or missing references)
			//IL_0958: Unknown result type (might be due to invalid IL or missing references)
			//IL_095f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0964: Unknown result type (might be due to invalid IL or missing references)
			//IL_096c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0971: Unknown result type (might be due to invalid IL or missing references)
			//IL_0976: Unknown result type (might be due to invalid IL or missing references)
			//IL_0978: Unknown result type (might be due to invalid IL or missing references)
			//IL_0981: Unknown result type (might be due to invalid IL or missing references)
			//IL_0986: Unknown result type (might be due to invalid IL or missing references)
			//IL_098f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0994: Unknown result type (might be due to invalid IL or missing references)
			//IL_0999: Unknown result type (might be due to invalid IL or missing references)
			//IL_099d: Unknown result type (might be due to invalid IL or missing references)
			//IL_09a2: Unknown result type (might be due to invalid IL or missing references)
			//IL_09a6: Unknown result type (might be due to invalid IL or missing references)
			//IL_09af: Unknown result type (might be due to invalid IL or missing references)
			//IL_09b4: Unknown result type (might be due to invalid IL or missing references)
			//IL_09c2: Unknown result type (might be due to invalid IL or missing references)
			//IL_09cf: Unknown result type (might be due to invalid IL or missing references)
			//IL_09db: Unknown result type (might be due to invalid IL or missing references)
			//IL_0a20: Unknown result type (might be due to invalid IL or missing references)
			//IL_0a22: Unknown result type (might be due to invalid IL or missing references)
			//IL_0a24: Unknown result type (might be due to invalid IL or missing references)
			//IL_08b5: Unknown result type (might be due to invalid IL or missing references)
			//IL_0846: Unknown result type (might be due to invalid IL or missing references)
			//IL_0848: Unknown result type (might be due to invalid IL or missing references)
			//IL_084a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0858: Unknown result type (might be due to invalid IL or missing references)
			//IL_085d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0862: Unknown result type (might be due to invalid IL or missing references)
			//IL_0867: Unknown result type (might be due to invalid IL or missing references)
			//IL_0874: Unknown result type (might be due to invalid IL or missing references)
			//IL_0879: Unknown result type (might be due to invalid IL or missing references)
			//IL_0886: Unknown result type (might be due to invalid IL or missing references)
			//IL_088b: Unknown result type (might be due to invalid IL or missing references)
			//IL_089d: Unknown result type (might be due to invalid IL or missing references)
			//IL_08a2: Unknown result type (might be due to invalid IL or missing references)
			//IL_05a8: Unknown result type (might be due to invalid IL or missing references)
			//IL_05aa: Unknown result type (might be due to invalid IL or missing references)
			//IL_05b6: Unknown result type (might be due to invalid IL or missing references)
			//IL_05be: Unknown result type (might be due to invalid IL or missing references)
			//IL_05c0: Unknown result type (might be due to invalid IL or missing references)
			//IL_05c9: Unknown result type (might be due to invalid IL or missing references)
			//IL_05ce: Unknown result type (might be due to invalid IL or missing references)
			//IL_05d7: Unknown result type (might be due to invalid IL or missing references)
			//IL_05dc: Unknown result type (might be due to invalid IL or missing references)
			//IL_05ef: Unknown result type (might be due to invalid IL or missing references)
			//IL_05f1: Unknown result type (might be due to invalid IL or missing references)
			//IL_05f3: Unknown result type (might be due to invalid IL or missing references)
			//IL_05f8: Unknown result type (might be due to invalid IL or missing references)
			//IL_0591: Unknown result type (might be due to invalid IL or missing references)
			//IL_0595: Unknown result type (might be due to invalid IL or missing references)
			//IL_059a: Unknown result type (might be due to invalid IL or missing references)
			//IL_059f: Unknown result type (might be due to invalid IL or missing references)
			//IL_05a4: Unknown result type (might be due to invalid IL or missing references)
			//IL_023d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0242: Unknown result type (might be due to invalid IL or missing references)
			//IL_0247: Unknown result type (might be due to invalid IL or missing references)
			//IL_024b: Unknown result type (might be due to invalid IL or missing references)
			//IL_022d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0e84: Unknown result type (might be due to invalid IL or missing references)
			//IL_0e86: Unknown result type (might be due to invalid IL or missing references)
			//IL_0e92: Unknown result type (might be due to invalid IL or missing references)
			//IL_0db3: Unknown result type (might be due to invalid IL or missing references)
			//IL_0db8: Unknown result type (might be due to invalid IL or missing references)
			//IL_0dbd: Unknown result type (might be due to invalid IL or missing references)
			//IL_0dbf: Unknown result type (might be due to invalid IL or missing references)
			//IL_0dc4: Unknown result type (might be due to invalid IL or missing references)
			//IL_0dc6: Unknown result type (might be due to invalid IL or missing references)
			//IL_0dd1: Unknown result type (might be due to invalid IL or missing references)
			//IL_0dd5: Unknown result type (might be due to invalid IL or missing references)
			//IL_0de1: Unknown result type (might be due to invalid IL or missing references)
			//IL_0de8: Unknown result type (might be due to invalid IL or missing references)
			//IL_0ded: Unknown result type (might be due to invalid IL or missing references)
			//IL_0df2: Unknown result type (might be due to invalid IL or missing references)
			//IL_0df4: Unknown result type (might be due to invalid IL or missing references)
			//IL_0e01: Unknown result type (might be due to invalid IL or missing references)
			//IL_0e10: Unknown result type (might be due to invalid IL or missing references)
			//IL_0e15: Unknown result type (might be due to invalid IL or missing references)
			//IL_0e1a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0e24: Unknown result type (might be due to invalid IL or missing references)
			//IL_0e29: Unknown result type (might be due to invalid IL or missing references)
			//IL_0e2b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0e30: Unknown result type (might be due to invalid IL or missing references)
			//IL_0e37: Unknown result type (might be due to invalid IL or missing references)
			//IL_0a68: Unknown result type (might be due to invalid IL or missing references)
			//IL_0a72: Unknown result type (might be due to invalid IL or missing references)
			//IL_0a77: Unknown result type (might be due to invalid IL or missing references)
			//IL_0a7e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0a84: Unknown result type (might be due to invalid IL or missing references)
			//IL_0a86: Unknown result type (might be due to invalid IL or missing references)
			//IL_0a90: Unknown result type (might be due to invalid IL or missing references)
			//IL_0a94: Unknown result type (might be due to invalid IL or missing references)
			//IL_0a99: Unknown result type (might be due to invalid IL or missing references)
			//IL_0a9b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0aa0: Unknown result type (might be due to invalid IL or missing references)
			//IL_0aaf: Unknown result type (might be due to invalid IL or missing references)
			//IL_0ab4: Unknown result type (might be due to invalid IL or missing references)
			//IL_0ab8: Unknown result type (might be due to invalid IL or missing references)
			//IL_0ad7: Unknown result type (might be due to invalid IL or missing references)
			//IL_0ad9: Unknown result type (might be due to invalid IL or missing references)
			//IL_0ade: Unknown result type (might be due to invalid IL or missing references)
			//IL_0ae0: Unknown result type (might be due to invalid IL or missing references)
			//IL_0ae2: Unknown result type (might be due to invalid IL or missing references)
			//IL_0ae4: Unknown result type (might be due to invalid IL or missing references)
			//IL_0ae9: Unknown result type (might be due to invalid IL or missing references)
			//IL_0aeb: Unknown result type (might be due to invalid IL or missing references)
			//IL_0aed: Unknown result type (might be due to invalid IL or missing references)
			//IL_0aef: Unknown result type (might be due to invalid IL or missing references)
			//IL_0b03: Unknown result type (might be due to invalid IL or missing references)
			//IL_0b08: Unknown result type (might be due to invalid IL or missing references)
			//IL_0b0d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0b0f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0b11: Unknown result type (might be due to invalid IL or missing references)
			//IL_0b13: Unknown result type (might be due to invalid IL or missing references)
			//IL_0b18: Unknown result type (might be due to invalid IL or missing references)
			//IL_0b1c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0b21: Unknown result type (might be due to invalid IL or missing references)
			//IL_0b26: Unknown result type (might be due to invalid IL or missing references)
			//IL_0b28: Unknown result type (might be due to invalid IL or missing references)
			//IL_0b2d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0b2f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0b3a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0b3e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0b4a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0b51: Unknown result type (might be due to invalid IL or missing references)
			//IL_0b56: Unknown result type (might be due to invalid IL or missing references)
			//IL_0b5b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0b5d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0b6a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0b79: Unknown result type (might be due to invalid IL or missing references)
			//IL_0b7e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0b83: Unknown result type (might be due to invalid IL or missing references)
			//IL_0b8d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0b92: Unknown result type (might be due to invalid IL or missing references)
			//IL_0b94: Unknown result type (might be due to invalid IL or missing references)
			//IL_0b99: Unknown result type (might be due to invalid IL or missing references)
			//IL_0ba0: Unknown result type (might be due to invalid IL or missing references)
			//IL_0a3c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0a4b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0a50: Unknown result type (might be due to invalid IL or missing references)
			//IL_0a5c: Unknown result type (might be due to invalid IL or missing references)
			//IL_08ea: Unknown result type (might be due to invalid IL or missing references)
			//IL_08f1: Unknown result type (might be due to invalid IL or missing references)
			//IL_08f7: Unknown result type (might be due to invalid IL or missing references)
			//IL_08f9: Unknown result type (might be due to invalid IL or missing references)
			//IL_090a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0601: Unknown result type (might be due to invalid IL or missing references)
			//IL_0606: Unknown result type (might be due to invalid IL or missing references)
			//IL_0257: Unknown result type (might be due to invalid IL or missing references)
			//IL_0263: Unknown result type (might be due to invalid IL or missing references)
			//IL_026a: Unknown result type (might be due to invalid IL or missing references)
			//IL_026f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0278: Unknown result type (might be due to invalid IL or missing references)
			//IL_027d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0282: Unknown result type (might be due to invalid IL or missing references)
			//IL_0286: Unknown result type (might be due to invalid IL or missing references)
			//IL_02a5: Unknown result type (might be due to invalid IL or missing references)
			//IL_02a7: Unknown result type (might be due to invalid IL or missing references)
			//IL_02ac: Unknown result type (might be due to invalid IL or missing references)
			//IL_02ae: Unknown result type (might be due to invalid IL or missing references)
			//IL_02b0: Unknown result type (might be due to invalid IL or missing references)
			//IL_02b2: Unknown result type (might be due to invalid IL or missing references)
			//IL_02b7: Unknown result type (might be due to invalid IL or missing references)
			//IL_02b9: Unknown result type (might be due to invalid IL or missing references)
			//IL_02bb: Unknown result type (might be due to invalid IL or missing references)
			//IL_02bd: Unknown result type (might be due to invalid IL or missing references)
			//IL_02d1: Unknown result type (might be due to invalid IL or missing references)
			//IL_02d6: Unknown result type (might be due to invalid IL or missing references)
			//IL_02db: Unknown result type (might be due to invalid IL or missing references)
			//IL_02dd: Unknown result type (might be due to invalid IL or missing references)
			//IL_02df: Unknown result type (might be due to invalid IL or missing references)
			//IL_02e1: Unknown result type (might be due to invalid IL or missing references)
			//IL_02e6: Unknown result type (might be due to invalid IL or missing references)
			//IL_02ea: Unknown result type (might be due to invalid IL or missing references)
			//IL_02ef: Unknown result type (might be due to invalid IL or missing references)
			//IL_02f4: Unknown result type (might be due to invalid IL or missing references)
			//IL_02f6: Unknown result type (might be due to invalid IL or missing references)
			//IL_02fb: Unknown result type (might be due to invalid IL or missing references)
			//IL_02fd: Unknown result type (might be due to invalid IL or missing references)
			//IL_0308: Unknown result type (might be due to invalid IL or missing references)
			//IL_030c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0318: Unknown result type (might be due to invalid IL or missing references)
			//IL_031f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0324: Unknown result type (might be due to invalid IL or missing references)
			//IL_0329: Unknown result type (might be due to invalid IL or missing references)
			//IL_032b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0338: Unknown result type (might be due to invalid IL or missing references)
			//IL_0347: Unknown result type (might be due to invalid IL or missing references)
			//IL_034c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0351: Unknown result type (might be due to invalid IL or missing references)
			//IL_035b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0360: Unknown result type (might be due to invalid IL or missing references)
			//IL_0362: Unknown result type (might be due to invalid IL or missing references)
			//IL_0367: Unknown result type (might be due to invalid IL or missing references)
			//IL_036e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0e9a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0e9c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0ea5: Unknown result type (might be due to invalid IL or missing references)
			//IL_0eaa: Unknown result type (might be due to invalid IL or missing references)
			//IL_0eb3: Unknown result type (might be due to invalid IL or missing references)
			//IL_0eb8: Unknown result type (might be due to invalid IL or missing references)
			//IL_0e4e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0e55: Unknown result type (might be due to invalid IL or missing references)
			//IL_0e5f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0e64: Unknown result type (might be due to invalid IL or missing references)
			//IL_0e6a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0e6f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0e71: Unknown result type (might be due to invalid IL or missing references)
			//IL_0e76: Unknown result type (might be due to invalid IL or missing references)
			//IL_0e7b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0beb: Unknown result type (might be due to invalid IL or missing references)
			//IL_0bf2: Unknown result type (might be due to invalid IL or missing references)
			//IL_0bf9: Unknown result type (might be due to invalid IL or missing references)
			//IL_0bfe: Unknown result type (might be due to invalid IL or missing references)
			//IL_0c03: Unknown result type (might be due to invalid IL or missing references)
			//IL_0c09: Unknown result type (might be due to invalid IL or missing references)
			//IL_0c0e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0c15: Unknown result type (might be due to invalid IL or missing references)
			//IL_0c1a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0c23: Unknown result type (might be due to invalid IL or missing references)
			//IL_0c28: Unknown result type (might be due to invalid IL or missing references)
			//IL_0c31: Unknown result type (might be due to invalid IL or missing references)
			//IL_0c36: Unknown result type (might be due to invalid IL or missing references)
			//IL_0bb7: Unknown result type (might be due to invalid IL or missing references)
			//IL_0bbe: Unknown result type (might be due to invalid IL or missing references)
			//IL_0bc8: Unknown result type (might be due to invalid IL or missing references)
			//IL_0bcd: Unknown result type (might be due to invalid IL or missing references)
			//IL_0bd3: Unknown result type (might be due to invalid IL or missing references)
			//IL_0bd8: Unknown result type (might be due to invalid IL or missing references)
			//IL_0bda: Unknown result type (might be due to invalid IL or missing references)
			//IL_0bdf: Unknown result type (might be due to invalid IL or missing references)
			//IL_0be4: Unknown result type (might be due to invalid IL or missing references)
			//IL_03b9: Unknown result type (might be due to invalid IL or missing references)
			//IL_03c0: Unknown result type (might be due to invalid IL or missing references)
			//IL_03c7: Unknown result type (might be due to invalid IL or missing references)
			//IL_03cc: Unknown result type (might be due to invalid IL or missing references)
			//IL_03d1: Unknown result type (might be due to invalid IL or missing references)
			//IL_03d7: Unknown result type (might be due to invalid IL or missing references)
			//IL_03dc: Unknown result type (might be due to invalid IL or missing references)
			//IL_03e3: Unknown result type (might be due to invalid IL or missing references)
			//IL_03e8: Unknown result type (might be due to invalid IL or missing references)
			//IL_03f1: Unknown result type (might be due to invalid IL or missing references)
			//IL_03f6: Unknown result type (might be due to invalid IL or missing references)
			//IL_03ff: Unknown result type (might be due to invalid IL or missing references)
			//IL_0404: Unknown result type (might be due to invalid IL or missing references)
			//IL_0385: Unknown result type (might be due to invalid IL or missing references)
			//IL_038c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0396: Unknown result type (might be due to invalid IL or missing references)
			//IL_039b: Unknown result type (might be due to invalid IL or missing references)
			//IL_03a1: Unknown result type (might be due to invalid IL or missing references)
			//IL_03a6: Unknown result type (might be due to invalid IL or missing references)
			//IL_03a8: Unknown result type (might be due to invalid IL or missing references)
			//IL_03ad: Unknown result type (might be due to invalid IL or missing references)
			//IL_03b2: Unknown result type (might be due to invalid IL or missing references)
			//IL_0cb4: Unknown result type (might be due to invalid IL or missing references)
			//IL_0cb6: Unknown result type (might be due to invalid IL or missing references)
			//IL_0f36: Unknown result type (might be due to invalid IL or missing references)
			//IL_0f38: Unknown result type (might be due to invalid IL or missing references)
			//IL_0f3a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0f3f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0f48: Unknown result type (might be due to invalid IL or missing references)
			//IL_0f4d: Unknown result type (might be due to invalid IL or missing references)
			//IL_068f: Unknown result type (might be due to invalid IL or missing references)
			//IL_04b7: Unknown result type (might be due to invalid IL or missing references)
			//IL_04b9: Unknown result type (might be due to invalid IL or missing references)
			//IL_0fd6: Unknown result type (might be due to invalid IL or missing references)
			NativeArray<Entity> nativeArray = ((ArchetypeChunk)(ref chunk)).GetNativeArray(m_EntityType);
			NativeArray<PrefabRef> nativeArray2 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<PrefabRef>(ref m_PrefabRefType);
			NativeArray<Aircraft> nativeArray3 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<Aircraft>(ref m_AircraftType);
			NativeArray<AircraftNavigation> nativeArray4 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<AircraftNavigation>(ref m_NavigationType);
			NativeArray<AircraftCurrentLane> nativeArray5 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<AircraftCurrentLane>(ref m_CurrentLaneType);
			NativeArray<PseudoRandomSeed> nativeArray6 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<PseudoRandomSeed>(ref m_PseudoRandomSeedType);
			NativeArray<Moving> nativeArray7 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<Moving>(ref m_MovingType);
			NativeArray<Transform> nativeArray8 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<Transform>(ref m_TransformType);
			BufferAccessor<TransformFrame> bufferAccessor = ((ArchetypeChunk)(ref chunk)).GetBufferAccessor<TransformFrame>(ref m_TransformFrameType);
			float num = 4f / 15f;
			if (((ArchetypeChunk)(ref chunk)).Has<Helicopter>(ref m_HelicopterType))
			{
				float3 val5 = default(float3);
				float3 val10 = default(float3);
				float num3 = default(float);
				for (int i = 0; i < ((ArchetypeChunk)(ref chunk)).Count; i++)
				{
					PrefabRef prefabRef = nativeArray2[i];
					Aircraft aircraft = nativeArray3[i];
					AircraftNavigation aircraftNavigation = nativeArray4[i];
					AircraftCurrentLane aircraftCurrentLane = nativeArray5[i];
					PseudoRandomSeed pseudoRandomSeed = nativeArray6[i];
					Moving moving = nativeArray7[i];
					Transform transform = nativeArray8[i];
					Random random = pseudoRandomSeed.GetRandom(PseudoRandomSeed.kLightState);
					HelicopterData helicopterData = m_PrefabHelicopterData[prefabRef.m_Prefab];
					TransformFrame transformFrame = default(TransformFrame);
					if ((aircraftCurrentLane.m_LaneFlags & AircraftLaneFlags.Flying) != 0)
					{
						float3 val = aircraftNavigation.m_TargetPosition - transform.m_Position;
						float3 val2 = val;
						float3 val3 = default(float3);
						val = math.normalizesafe(val2, val3);
						float num2 = math.asin(math.saturate(val.y));
						if (aircraftNavigation.m_MinClimbAngle > num2)
						{
							val.y = math.sin(aircraftNavigation.m_MinClimbAngle);
							((float3)(ref val)).xz = math.normalizesafe(((float3)(ref val)).xz, default(float2)) * math.cos(aircraftNavigation.m_MinClimbAngle);
						}
						val *= aircraftNavigation.m_MaxSpeed;
						float3 val4 = val - moving.m_Velocity;
						val4 = MathUtils.ClampLength(val4, helicopterData.m_FlyingAcceleration * num);
						ref float3 velocity = ref moving.m_Velocity;
						velocity += val4;
						((float3)(ref val5))._002Ector(0f, 0f, 1f);
						if (math.lengthsq(((float3)(ref moving.m_Velocity)).xz) >= 1f)
						{
							((float3)(ref val5)).xz = ((float3)(ref moving.m_Velocity)).xz;
						}
						else
						{
							val3 = math.forward(transform.m_Rotation);
							((float3)(ref val5)).xz = ((float3)(ref val3)).xz;
						}
						float3 val6 = moving.m_Velocity * helicopterData.m_VelocitySwayFactor * num + val4 * helicopterData.m_AccelerationSwayFactor;
						val6.y = math.max(val6.y, 0f) + 9.81f * num;
						val6 = math.normalize(val6);
						float3 val7 = math.cross(val5, val6);
						val5 = math.normalizesafe(math.cross(val6, val7), new float3(0f, 0f, 1f));
						quaternion val8 = quaternion.LookRotationSafe(val5, val6);
						quaternion val9 = math.mul(math.inverse(transform.m_Rotation), val8);
						MathUtils.AxisAngle(val9, ref val10, ref num3);
						float3 val11 = val10 * num3 * helicopterData.m_FlyingAngularAcceleration - moving.m_AngularVelocity;
						val11 = math.clamp(val11, float3.op_Implicit((0f - helicopterData.m_FlyingAngularAcceleration) * num), float3.op_Implicit(helicopterData.m_FlyingAngularAcceleration * num));
						ref float3 angularVelocity = ref moving.m_AngularVelocity;
						angularVelocity += val11;
						float num4 = math.length(moving.m_AngularVelocity);
						if (num4 > 1E-05f)
						{
							val9 = quaternion.AxisAngle(moving.m_AngularVelocity / num4, num4 * num);
							transform.m_Rotation = math.normalize(math.mul(transform.m_Rotation, val9));
						}
						float3 val12 = transform.m_Position + moving.m_Velocity * num;
						transformFrame.m_Position = math.lerp(transform.m_Position, val12, 0.5f);
						transformFrame.m_Velocity = moving.m_Velocity;
						transformFrame.m_Rotation = transform.m_Rotation;
						float num5 = m_DayLightBrightness + ((Random)(ref random)).NextFloat(-0.05f, 0.05f);
						transformFrame.m_Flags = TransformFlags.InteriorLights | TransformFlags.Flying;
						if ((aircraftCurrentLane.m_LaneFlags & AircraftLaneFlags.Landing) != 0)
						{
							transformFrame.m_Flags |= TransformFlags.WarningLights | TransformFlags.Landing;
							if (num5 < 0.5f)
							{
								transformFrame.m_Flags |= TransformFlags.ExtraLights;
							}
						}
						else if ((aircraftCurrentLane.m_LaneFlags & AircraftLaneFlags.TakingOff) != 0)
						{
							transformFrame.m_Flags |= TransformFlags.WarningLights | TransformFlags.TakingOff;
						}
						if (num5 < 0.5f)
						{
							transformFrame.m_Flags |= TransformFlags.MainLights;
						}
						if ((aircraft.m_Flags & AircraftFlags.Working) != 0)
						{
							transformFrame.m_Flags |= TransformFlags.WorkLights;
						}
						transform.m_Position = val12;
					}
					else
					{
						float3 val13 = aircraftNavigation.m_TargetPosition - transform.m_Position;
						MathUtils.TryNormalize(ref val13, aircraftNavigation.m_MaxSpeed);
						float3 velocity2 = val13 * 8f + moving.m_Velocity;
						MathUtils.TryNormalize(ref velocity2, aircraftNavigation.m_MaxSpeed);
						moving.m_Velocity = velocity2;
						float3 val14 = moving.m_Velocity * (num * 0.5f);
						float3 val15 = transform.m_Position + val14;
						float3 targetDirection = aircraftNavigation.m_TargetDirection;
						quaternion rotation = transform.m_Rotation;
						if (MathUtils.TryNormalize(ref targetDirection))
						{
							rotation = quaternion.LookRotationSafe(targetDirection, math.up());
						}
						else
						{
							float3 val16 = aircraftNavigation.m_TargetPosition - transform.m_Position;
							float num6 = math.length(val16);
							if (num6 >= 1f)
							{
								rotation = quaternion.LookRotationSafe(val16 / num6, math.up());
							}
						}
						transform.m_Rotation = rotation;
						moving.m_AngularVelocity = default(float3);
						transformFrame.m_Position = val15;
						transformFrame.m_Velocity = moving.m_Velocity;
						transformFrame.m_Rotation = transform.m_Rotation;
						transformFrame.m_Flags = TransformFlags.InteriorLights | TransformFlags.WarningLights;
						transform.m_Position = val15 + val14;
					}
					DynamicBuffer<TransformFrame> val17 = bufferAccessor[i];
					if (((val17[(int)m_TransformFrameIndex].m_Flags ^ transformFrame.m_Flags) & (TransformFlags.MainLights | TransformFlags.ExtraLights | TransformFlags.WorkLights | TransformFlags.TakingOff | TransformFlags.Landing | TransformFlags.Flying)) != 0)
					{
						TransformFlags transformFlags = (TransformFlags)0u;
						TransformFlags transformFlags2 = (TransformFlags)0u;
						for (int j = 0; j < val17.Length; j++)
						{
							TransformFlags flags = val17[j].m_Flags;
							transformFlags |= flags;
							transformFlags2 |= ((j == (int)m_TransformFrameIndex) ? transformFrame.m_Flags : flags);
						}
						if (((transformFlags ^ transformFlags2) & (TransformFlags.MainLights | TransformFlags.ExtraLights | TransformFlags.WorkLights | TransformFlags.TakingOff | TransformFlags.Landing | TransformFlags.Flying)) != 0)
						{
							((ParallelWriter)(ref m_CommandBuffer)).AddComponent<EffectsUpdated>(unfilteredChunkIndex, nativeArray[i], default(EffectsUpdated));
						}
					}
					val17[(int)m_TransformFrameIndex] = transformFrame;
					nativeArray7[i] = moving;
					nativeArray8[i] = transform;
				}
				return;
			}
			float3 val29 = default(float3);
			float num15 = default(float);
			float3 val38 = default(float3);
			float num17 = default(float);
			for (int k = 0; k < ((ArchetypeChunk)(ref chunk)).Count; k++)
			{
				PrefabRef prefabRef2 = nativeArray2[k];
				AircraftNavigation aircraftNavigation2 = nativeArray4[k];
				AircraftCurrentLane aircraftCurrentLane2 = nativeArray5[k];
				PseudoRandomSeed pseudoRandomSeed2 = nativeArray6[k];
				Moving moving2 = nativeArray7[k];
				Transform transform2 = nativeArray8[k];
				Random random2 = pseudoRandomSeed2.GetRandom(PseudoRandomSeed.kLightState);
				AirplaneData airplaneData = m_PrefabAirplaneData[prefabRef2.m_Prefab];
				TransformFrame transformFrame2 = default(TransformFrame);
				if ((aircraftCurrentLane2.m_LaneFlags & AircraftLaneFlags.Flying) != 0)
				{
					float3 val18 = math.normalizesafe(moving2.m_Velocity, new float3(0f, 0f, 1f));
					float2 val19 = math.normalizesafe(((float3)(ref moving2.m_Velocity)).xz, new float2(0f, 1f));
					float2 xz = ((float3)(ref aircraftNavigation2.m_TargetDirection)).xz;
					float3 val20 = aircraftNavigation2.m_TargetPosition - transform2.m_Position;
					float num7 = aircraftNavigation2.m_MaxSpeed / airplaneData.m_FlyingTurning;
					if (MathUtils.TryNormalize(ref xz))
					{
						float num8 = math.dot(val19, xz);
						float2 val21 = MathUtils.Right(xz);
						float num9 = math.dot(val21, ((float3)(ref val20)).xz);
						if (math.dot(((float3)(ref val20)).xz, xz) >= 0f)
						{
							num8 = math.max(num8, 0f);
							if (math.abs(num9) <= num7)
							{
								num8 = math.max(num8, 1f);
							}
						}
						val21 = math.select(val21, -val21, num9 > 0f);
						((float3)(ref val20)).xz = ((float3)(ref val20)).xz + val21 * (num7 * (1f - num8));
						((float3)(ref val20)).xz = ((float3)(ref val20)).xz - xz * (num7 * (1f - math.abs(num8)));
					}
					if (MathUtils.TryNormalize(ref val20))
					{
						float num10 = math.asin(math.saturate(val20.y));
						if (aircraftNavigation2.m_MinClimbAngle > num10)
						{
							val20.y = math.sin(aircraftNavigation2.m_MinClimbAngle);
							((float3)(ref val20)).xz = math.normalizesafe(((float3)(ref val20)).xz, default(float2)) * math.cos(aircraftNavigation2.m_MinClimbAngle);
						}
					}
					float num11 = math.acos(math.clamp(math.dot(val20, val18), -1f, 1f));
					num11 = math.min(num11, airplaneData.m_FlyingTurning * num);
					float3 val22 = math.normalizesafe(val20 - val18 * math.dot(val18, val20), new float3(((float3)(ref val18)).zy, 0f - val18.x));
					float3 val23 = val18 * math.cos(num11) + val22 * math.sin(num11);
					float3 velocity3 = moving2.m_Velocity;
					moving2.m_Velocity = val23 * aircraftNavigation2.m_MaxSpeed;
					float num12 = (aircraftNavigation2.m_MaxSpeed - airplaneData.m_FlyingSpeed.x) / (airplaneData.m_FlyingSpeed.y - airplaneData.m_FlyingSpeed.x);
					num12 = math.saturate(1f - num12);
					num12 *= num12;
					float num13 = math.lerp(0f - airplaneData.m_ClimbAngle, airplaneData.m_SlowPitchAngle, num12);
					float num14 = math.sin(num13);
					float3 val24 = val23;
					if (val24.y < num14)
					{
						val24.y = num14;
						((float3)(ref val24)).xz = math.normalizesafe(((float3)(ref val24)).xz, new float2(0f, 1f)) * math.cos(num13);
					}
					float3 val25 = default(float3);
					((float3)(ref val25)).xz = math.normalizesafe(MathUtils.Right(((float3)(ref val24)).xz), default(float2));
					float3 val26 = val25 * (math.dot(moving2.m_Velocity - velocity3, val25) * airplaneData.m_TurningRollFactor);
					val26.y = math.max(val26.y, 0f) + 9.81f * num;
					val26 = math.normalize(val26);
					val25 = math.cross(val24, val26);
					val26 = math.normalizesafe(math.cross(val25, val24), new float3(0f, 1f, 0f));
					quaternion val27 = quaternion.LookRotationSafe(val24, val26);
					quaternion val28 = math.mul(math.inverse(transform2.m_Rotation), val27);
					MathUtils.AxisAngle(val28, ref val29, ref num15);
					float3 val30 = val29 * num15 * airplaneData.m_FlyingAngularAcceleration - moving2.m_AngularVelocity;
					val30 = math.clamp(val30, float3.op_Implicit((0f - airplaneData.m_FlyingAngularAcceleration) * num), float3.op_Implicit(airplaneData.m_FlyingAngularAcceleration * num));
					ref float3 angularVelocity2 = ref moving2.m_AngularVelocity;
					angularVelocity2 += val30;
					float num16 = math.length(moving2.m_AngularVelocity);
					if (num16 > 1E-05f)
					{
						val28 = quaternion.AxisAngle(moving2.m_AngularVelocity / num16, num16 * num);
						transform2.m_Rotation = math.normalize(math.mul(transform2.m_Rotation, val28));
					}
					float3 val31 = transform2.m_Position + moving2.m_Velocity * num;
					transformFrame2.m_Position = math.lerp(transform2.m_Position, val31, 0.5f);
					transformFrame2.m_Velocity = moving2.m_Velocity;
					transformFrame2.m_Rotation = transform2.m_Rotation;
					transformFrame2.m_Flags = TransformFlags.InteriorLights | TransformFlags.Flying;
					if ((aircraftCurrentLane2.m_LaneFlags & AircraftLaneFlags.Landing) != 0)
					{
						transformFrame2.m_Flags |= TransformFlags.WarningLights | TransformFlags.Landing;
						if (m_DayLightBrightness + ((Random)(ref random2)).NextFloat(-0.05f, 0.05f) < 0.5f)
						{
							transformFrame2.m_Flags |= TransformFlags.ExtraLights;
						}
					}
					else if ((aircraftCurrentLane2.m_LaneFlags & AircraftLaneFlags.TakingOff) != 0)
					{
						transformFrame2.m_Flags |= TransformFlags.WarningLights | TransformFlags.TakingOff;
					}
					transform2.m_Position = val31;
				}
				else
				{
					float3 val32 = aircraftNavigation2.m_TargetPosition - transform2.m_Position;
					MathUtils.TryNormalize(ref val32, aircraftNavigation2.m_MaxSpeed);
					float3 velocity4 = val32 * 8f + moving2.m_Velocity;
					MathUtils.TryNormalize(ref velocity4, aircraftNavigation2.m_MaxSpeed);
					moving2.m_Velocity = velocity4;
					float3 val33 = moving2.m_Velocity * (num * 0.5f);
					float3 val34 = transform2.m_Position + val33;
					float3 targetDirection2 = aircraftNavigation2.m_TargetDirection;
					quaternion val35 = transform2.m_Rotation;
					if (MathUtils.TryNormalize(ref targetDirection2))
					{
						val35 = quaternion.LookRotationSafe(targetDirection2, math.up());
					}
					else
					{
						float3 val36 = aircraftNavigation2.m_TargetPosition - transform2.m_Position;
						if (MathUtils.TryNormalize(ref val36))
						{
							val35 = quaternion.LookRotationSafe(val36, math.up());
						}
					}
					if (aircraftNavigation2.m_MaxSpeed > airplaneData.m_FlyingSpeed.x * 0.9f)
					{
						quaternion val37 = math.mul(math.inverse(transform2.m_Rotation), val35);
						MathUtils.AxisAngle(val37, ref val38, ref num17);
						float3 val39 = val38 * num17 * airplaneData.m_FlyingAngularAcceleration - moving2.m_AngularVelocity;
						val39 = math.clamp(val39, float3.op_Implicit((0f - airplaneData.m_FlyingAngularAcceleration) * num), float3.op_Implicit(airplaneData.m_FlyingAngularAcceleration * num));
						ref float3 angularVelocity3 = ref moving2.m_AngularVelocity;
						angularVelocity3 += val39;
						float num18 = math.length(moving2.m_AngularVelocity);
						if (num18 > 1E-05f)
						{
							val37 = quaternion.AxisAngle(moving2.m_AngularVelocity / num18, num18 * num);
							transform2.m_Rotation = math.normalize(math.mul(transform2.m_Rotation, val37));
						}
					}
					else
					{
						transform2.m_Rotation = val35;
						moving2.m_AngularVelocity = default(float3);
					}
					transformFrame2.m_Position = val34;
					transformFrame2.m_Velocity = moving2.m_Velocity;
					transformFrame2.m_Rotation = transform2.m_Rotation;
					transformFrame2.m_Flags = TransformFlags.InteriorLights | TransformFlags.WarningLights;
					if ((aircraftCurrentLane2.m_LaneFlags & AircraftLaneFlags.Landing) != 0)
					{
						transformFrame2.m_Flags |= TransformFlags.Landing;
					}
					else if ((aircraftCurrentLane2.m_LaneFlags & AircraftLaneFlags.TakingOff) != 0)
					{
						transformFrame2.m_Flags |= TransformFlags.TakingOff;
					}
					if (m_DayLightBrightness + ((Random)(ref random2)).NextFloat(-0.05f, 0.05f) < 0.5f)
					{
						transformFrame2.m_Flags |= TransformFlags.MainLights;
					}
					transform2.m_Position = val34 + val33;
				}
				DynamicBuffer<TransformFrame> val40 = bufferAccessor[k];
				if (((val40[(int)m_TransformFrameIndex].m_Flags ^ transformFrame2.m_Flags) & (TransformFlags.MainLights | TransformFlags.ExtraLights | TransformFlags.TakingOff | TransformFlags.Landing | TransformFlags.Flying)) != 0)
				{
					TransformFlags transformFlags3 = (TransformFlags)0u;
					TransformFlags transformFlags4 = (TransformFlags)0u;
					for (int l = 0; l < val40.Length; l++)
					{
						TransformFlags flags2 = val40[l].m_Flags;
						transformFlags3 |= flags2;
						transformFlags4 |= ((l == (int)m_TransformFrameIndex) ? transformFrame2.m_Flags : flags2);
					}
					if (((transformFlags3 ^ transformFlags4) & (TransformFlags.MainLights | TransformFlags.ExtraLights | TransformFlags.TakingOff | TransformFlags.Landing | TransformFlags.Flying)) != 0)
					{
						((ParallelWriter)(ref m_CommandBuffer)).AddComponent<EffectsUpdated>(unfilteredChunkIndex, nativeArray[k], default(EffectsUpdated));
					}
				}
				val40[(int)m_TransformFrameIndex] = transformFrame2;
				nativeArray7[k] = moving2;
				nativeArray8[k] = transform2;
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
		public ComponentTypeHandle<Aircraft> __Game_Vehicles_Aircraft_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<Helicopter> __Game_Vehicles_Helicopter_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<AircraftNavigation> __Game_Vehicles_AircraftNavigation_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<AircraftCurrentLane> __Game_Vehicles_AircraftCurrentLane_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<PseudoRandomSeed> __Game_Common_PseudoRandomSeed_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<PrefabRef> __Game_Prefabs_PrefabRef_RO_ComponentTypeHandle;

		public ComponentTypeHandle<Transform> __Game_Objects_Transform_RW_ComponentTypeHandle;

		public BufferTypeHandle<TransformFrame> __Game_Objects_TransformFrame_RW_BufferTypeHandle;

		[ReadOnly]
		public ComponentLookup<HelicopterData> __Game_Prefabs_HelicopterData_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<AirplaneData> __Game_Prefabs_AirplaneData_RO_ComponentLookup;

		public ComponentTypeHandle<Moving> __Game_Objects_Moving_RW_ComponentTypeHandle;

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
			__Unity_Entities_Entity_TypeHandle = ((SystemState)(ref state)).GetEntityTypeHandle();
			__Game_Vehicles_Aircraft_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<Aircraft>(true);
			__Game_Vehicles_Helicopter_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<Helicopter>(true);
			__Game_Vehicles_AircraftNavigation_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<AircraftNavigation>(true);
			__Game_Vehicles_AircraftCurrentLane_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<AircraftCurrentLane>(true);
			__Game_Common_PseudoRandomSeed_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<PseudoRandomSeed>(true);
			__Game_Prefabs_PrefabRef_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<PrefabRef>(true);
			__Game_Objects_Transform_RW_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<Transform>(false);
			__Game_Objects_TransformFrame_RW_BufferTypeHandle = ((SystemState)(ref state)).GetBufferTypeHandle<TransformFrame>(false);
			__Game_Prefabs_HelicopterData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<HelicopterData>(true);
			__Game_Prefabs_AirplaneData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<AirplaneData>(true);
			__Game_Objects_Moving_RW_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<Moving>(false);
		}
	}

	private SimulationSystem m_SimulationSystem;

	private LightingSystem m_LightingSystem;

	private EntityQuery m_AircraftQuery;

	private EndFrameBarrier m_EndFrameBarrier;

	private TypeHandle __TypeHandle;

	public override int GetUpdateInterval(SystemUpdatePhase phase)
	{
		return 16;
	}

	public override int GetUpdateOffset(SystemUpdatePhase phase)
	{
		return 10;
	}

	[Preserve]
	protected override void OnCreate()
	{
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0074: Unknown result type (might be due to invalid IL or missing references)
		//IL_0079: Unknown result type (might be due to invalid IL or missing references)
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		//IL_0085: Unknown result type (might be due to invalid IL or missing references)
		//IL_008c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0091: Unknown result type (might be due to invalid IL or missing references)
		//IL_0098: Unknown result type (might be due to invalid IL or missing references)
		//IL_009d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b3: Unknown result type (might be due to invalid IL or missing references)
		base.OnCreate();
		m_SimulationSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<SimulationSystem>();
		m_LightingSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<LightingSystem>();
		m_EndFrameBarrier = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<EndFrameBarrier>();
		m_AircraftQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[9]
		{
			ComponentType.ReadOnly<Aircraft>(),
			ComponentType.ReadOnly<AircraftNavigation>(),
			ComponentType.ReadOnly<UpdateFrame>(),
			ComponentType.ReadWrite<Transform>(),
			ComponentType.ReadWrite<Moving>(),
			ComponentType.ReadWrite<TransformFrame>(),
			ComponentType.Exclude<Deleted>(),
			ComponentType.Exclude<Temp>(),
			ComponentType.Exclude<TripSource>()
		});
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
		//IL_0195: Unknown result type (might be due to invalid IL or missing references)
		//IL_019a: Unknown result type (might be due to invalid IL or missing references)
		//IL_019d: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01af: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c7: Unknown result type (might be due to invalid IL or missing references)
		AircraftMoveJob aircraftMoveJob = new AircraftMoveJob
		{
			m_EntityType = InternalCompilerInterface.GetEntityTypeHandle(ref __TypeHandle.__Unity_Entities_Entity_TypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_AircraftType = InternalCompilerInterface.GetComponentTypeHandle<Aircraft>(ref __TypeHandle.__Game_Vehicles_Aircraft_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_HelicopterType = InternalCompilerInterface.GetComponentTypeHandle<Helicopter>(ref __TypeHandle.__Game_Vehicles_Helicopter_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_NavigationType = InternalCompilerInterface.GetComponentTypeHandle<AircraftNavigation>(ref __TypeHandle.__Game_Vehicles_AircraftNavigation_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_CurrentLaneType = InternalCompilerInterface.GetComponentTypeHandle<AircraftCurrentLane>(ref __TypeHandle.__Game_Vehicles_AircraftCurrentLane_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_PseudoRandomSeedType = InternalCompilerInterface.GetComponentTypeHandle<PseudoRandomSeed>(ref __TypeHandle.__Game_Common_PseudoRandomSeed_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_PrefabRefType = InternalCompilerInterface.GetComponentTypeHandle<PrefabRef>(ref __TypeHandle.__Game_Prefabs_PrefabRef_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_TransformType = InternalCompilerInterface.GetComponentTypeHandle<Transform>(ref __TypeHandle.__Game_Objects_Transform_RW_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_TransformFrameType = InternalCompilerInterface.GetBufferTypeHandle<TransformFrame>(ref __TypeHandle.__Game_Objects_TransformFrame_RW_BufferTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_PrefabHelicopterData = InternalCompilerInterface.GetComponentLookup<HelicopterData>(ref __TypeHandle.__Game_Prefabs_HelicopterData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_PrefabAirplaneData = InternalCompilerInterface.GetComponentLookup<AirplaneData>(ref __TypeHandle.__Game_Prefabs_AirplaneData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_MovingType = InternalCompilerInterface.GetComponentTypeHandle<Moving>(ref __TypeHandle.__Game_Objects_Moving_RW_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_TransformFrameIndex = m_SimulationSystem.frameIndex / 16 % 4,
			m_DayLightBrightness = m_LightingSystem.dayLightBrightness
		};
		EntityCommandBuffer val = m_EndFrameBarrier.CreateCommandBuffer();
		aircraftMoveJob.m_CommandBuffer = ((EntityCommandBuffer)(ref val)).AsParallelWriter();
		JobHandle val2 = JobChunkExtensions.ScheduleParallel<AircraftMoveJob>(aircraftMoveJob, m_AircraftQuery, ((SystemBase)this).Dependency);
		m_EndFrameBarrier.AddJobHandleForProducer(val2);
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
	public AircraftMoveSystem()
	{
	}
}

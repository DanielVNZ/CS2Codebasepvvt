using System.Runtime.CompilerServices;
using Colossal.Mathematics;
using Game.Common;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Entities.Internal;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Scripting;

namespace Game.Prefabs;

[CompilerGenerated]
public class VehicleInitializeSystem : GameSystemBase
{
	[BurstCompile]
	private struct InitializeVehiclesJob : IJobParallelFor
	{
		[ReadOnly]
		public ComponentTypeHandle<ObjectGeometryData> m_ObjectGeometryType;

		[ReadOnly]
		public ComponentTypeHandle<CarTrailerData> m_CarTrailerType;

		[ReadOnly]
		public ComponentTypeHandle<MultipleUnitTrainData> m_MultipleUnitTrainType;

		[ReadOnly]
		public BufferTypeHandle<SubMesh> m_SubmeshType;

		public ComponentTypeHandle<CarData> m_CarType;

		public ComponentTypeHandle<TrainData> m_TrainType;

		public ComponentTypeHandle<SwayingData> m_SwayingType;

		public ComponentTypeHandle<VehicleData> m_VehicleType;

		[ReadOnly]
		public BufferLookup<ProceduralBone> m_ProceduralBones;

		[DeallocateOnJobCompletion]
		[ReadOnly]
		public NativeArray<ArchetypeChunk> m_Chunks;

		public void Execute(int index)
		{
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0015: Unknown result type (might be due to invalid IL or missing references)
			//IL_001a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0023: Unknown result type (might be due to invalid IL or missing references)
			//IL_0028: Unknown result type (might be due to invalid IL or missing references)
			//IL_0031: Unknown result type (might be due to invalid IL or missing references)
			//IL_0036: Unknown result type (might be due to invalid IL or missing references)
			//IL_003f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0044: Unknown result type (might be due to invalid IL or missing references)
			//IL_004e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0053: Unknown result type (might be due to invalid IL or missing references)
			//IL_005d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0062: Unknown result type (might be due to invalid IL or missing references)
			//IL_006c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0071: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ad: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b2: Unknown result type (might be due to invalid IL or missing references)
			//IL_017d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0187: Unknown result type (might be due to invalid IL or missing references)
			//IL_0196: Unknown result type (might be due to invalid IL or missing references)
			//IL_019b: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a2: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a7: Unknown result type (might be due to invalid IL or missing references)
			//IL_01aa: Unknown result type (might be due to invalid IL or missing references)
			//IL_01af: Unknown result type (might be due to invalid IL or missing references)
			//IL_00cf: Unknown result type (might be due to invalid IL or missing references)
			//IL_02d7: Unknown result type (might be due to invalid IL or missing references)
			//IL_01c1: Unknown result type (might be due to invalid IL or missing references)
			//IL_01c6: Unknown result type (might be due to invalid IL or missing references)
			//IL_02f9: Unknown result type (might be due to invalid IL or missing references)
			//IL_02e2: Unknown result type (might be due to invalid IL or missing references)
			//IL_02e9: Unknown result type (might be due to invalid IL or missing references)
			//IL_0600: Unknown result type (might be due to invalid IL or missing references)
			//IL_060a: Unknown result type (might be due to invalid IL or missing references)
			//IL_031f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0304: Unknown result type (might be due to invalid IL or missing references)
			//IL_030b: Unknown result type (might be due to invalid IL or missing references)
			//IL_01e3: Unknown result type (might be due to invalid IL or missing references)
			//IL_0358: Unknown result type (might be due to invalid IL or missing references)
			//IL_0348: Unknown result type (might be due to invalid IL or missing references)
			//IL_034c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0351: Unknown result type (might be due to invalid IL or missing references)
			//IL_0356: Unknown result type (might be due to invalid IL or missing references)
			//IL_0637: Unknown result type (might be due to invalid IL or missing references)
			//IL_063c: Unknown result type (might be due to invalid IL or missing references)
			//IL_03ab: Unknown result type (might be due to invalid IL or missing references)
			//IL_03ba: Unknown result type (might be due to invalid IL or missing references)
			//IL_03bf: Unknown result type (might be due to invalid IL or missing references)
			//IL_03c4: Unknown result type (might be due to invalid IL or missing references)
			//IL_03c8: Unknown result type (might be due to invalid IL or missing references)
			//IL_0366: Unknown result type (might be due to invalid IL or missing references)
			//IL_036d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0372: Unknown result type (might be due to invalid IL or missing references)
			//IL_037c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0381: Unknown result type (might be due to invalid IL or missing references)
			//IL_0386: Unknown result type (might be due to invalid IL or missing references)
			//IL_0388: Unknown result type (might be due to invalid IL or missing references)
			//IL_038f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0397: Unknown result type (might be due to invalid IL or missing references)
			//IL_0713: Unknown result type (might be due to invalid IL or missing references)
			//IL_0715: Unknown result type (might be due to invalid IL or missing references)
			//IL_071f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0721: Unknown result type (might be due to invalid IL or missing references)
			//IL_072c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0733: Unknown result type (might be due to invalid IL or missing references)
			//IL_0738: Unknown result type (might be due to invalid IL or missing references)
			//IL_073d: Unknown result type (might be due to invalid IL or missing references)
			//IL_03df: Unknown result type (might be due to invalid IL or missing references)
			//IL_03e6: Unknown result type (might be due to invalid IL or missing references)
			//IL_03ed: Unknown result type (might be due to invalid IL or missing references)
			//IL_03f2: Unknown result type (might be due to invalid IL or missing references)
			//IL_03f7: Unknown result type (might be due to invalid IL or missing references)
			//IL_03fc: Unknown result type (might be due to invalid IL or missing references)
			//IL_0403: Unknown result type (might be due to invalid IL or missing references)
			//IL_040a: Unknown result type (might be due to invalid IL or missing references)
			//IL_040f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0414: Unknown result type (might be due to invalid IL or missing references)
			//IL_042b: Unknown result type (might be due to invalid IL or missing references)
			//IL_044e: Unknown result type (might be due to invalid IL or missing references)
			//IL_046a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0485: Unknown result type (might be due to invalid IL or missing references)
			//IL_048c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0491: Unknown result type (might be due to invalid IL or missing references)
			//IL_0496: Unknown result type (might be due to invalid IL or missing references)
			//IL_04b8: Unknown result type (might be due to invalid IL or missing references)
			//IL_04ba: Unknown result type (might be due to invalid IL or missing references)
			//IL_04bf: Unknown result type (might be due to invalid IL or missing references)
			//IL_04c8: Unknown result type (might be due to invalid IL or missing references)
			//IL_04d8: Unknown result type (might be due to invalid IL or missing references)
			//IL_04e4: Unknown result type (might be due to invalid IL or missing references)
			//IL_04eb: Unknown result type (might be due to invalid IL or missing references)
			//IL_04f0: Unknown result type (might be due to invalid IL or missing references)
			//IL_04f2: Unknown result type (might be due to invalid IL or missing references)
			//IL_04f7: Unknown result type (might be due to invalid IL or missing references)
			//IL_04fc: Unknown result type (might be due to invalid IL or missing references)
			//IL_0509: Unknown result type (might be due to invalid IL or missing references)
			//IL_050e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0515: Unknown result type (might be due to invalid IL or missing references)
			//IL_051c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0523: Unknown result type (might be due to invalid IL or missing references)
			//IL_0528: Unknown result type (might be due to invalid IL or missing references)
			//IL_052d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0532: Unknown result type (might be due to invalid IL or missing references)
			//IL_053c: Unknown result type (might be due to invalid IL or missing references)
			//IL_078d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0792: Unknown result type (might be due to invalid IL or missing references)
			//IL_079e: Unknown result type (might be due to invalid IL or missing references)
			//IL_07a3: Unknown result type (might be due to invalid IL or missing references)
			//IL_07b5: Unknown result type (might be due to invalid IL or missing references)
			//IL_07be: Unknown result type (might be due to invalid IL or missing references)
			//IL_07ce: Unknown result type (might be due to invalid IL or missing references)
			//IL_07d5: Unknown result type (might be due to invalid IL or missing references)
			//IL_07da: Unknown result type (might be due to invalid IL or missing references)
			//IL_07df: Unknown result type (might be due to invalid IL or missing references)
			//IL_0753: Unknown result type (might be due to invalid IL or missing references)
			//IL_0769: Unknown result type (might be due to invalid IL or missing references)
			//IL_0778: Unknown result type (might be due to invalid IL or missing references)
			//IL_077d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0782: Unknown result type (might be due to invalid IL or missing references)
			//IL_0659: Unknown result type (might be due to invalid IL or missing references)
			//IL_0545: Unknown result type (might be due to invalid IL or missing references)
			//IL_0547: Unknown result type (might be due to invalid IL or missing references)
			//IL_0551: Unknown result type (might be due to invalid IL or missing references)
			//IL_0553: Unknown result type (might be due to invalid IL or missing references)
			//IL_0560: Unknown result type (might be due to invalid IL or missing references)
			//IL_0222: Unknown result type (might be due to invalid IL or missing references)
			//IL_0227: Unknown result type (might be due to invalid IL or missing references)
			//IL_0254: Unknown result type (might be due to invalid IL or missing references)
			//IL_0256: Unknown result type (might be due to invalid IL or missing references)
			//IL_0258: Unknown result type (might be due to invalid IL or missing references)
			//IL_025d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0260: Unknown result type (might be due to invalid IL or missing references)
			//IL_023a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0241: Unknown result type (might be due to invalid IL or missing references)
			//IL_0246: Unknown result type (might be due to invalid IL or missing references)
			//IL_0248: Unknown result type (might be due to invalid IL or missing references)
			//IL_024d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0252: Unknown result type (might be due to invalid IL or missing references)
			//IL_028e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0290: Unknown result type (might be due to invalid IL or missing references)
			//IL_0292: Unknown result type (might be due to invalid IL or missing references)
			//IL_0297: Unknown result type (might be due to invalid IL or missing references)
			//IL_029c: Unknown result type (might be due to invalid IL or missing references)
			//IL_02a1: Unknown result type (might be due to invalid IL or missing references)
			//IL_02a3: Unknown result type (might be due to invalid IL or missing references)
			//IL_02a5: Unknown result type (might be due to invalid IL or missing references)
			//IL_026b: Unknown result type (might be due to invalid IL or missing references)
			//IL_026d: Unknown result type (might be due to invalid IL or missing references)
			//IL_026f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0274: Unknown result type (might be due to invalid IL or missing references)
			//IL_0279: Unknown result type (might be due to invalid IL or missing references)
			//IL_027e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0280: Unknown result type (might be due to invalid IL or missing references)
			//IL_0282: Unknown result type (might be due to invalid IL or missing references)
			//IL_0693: Unknown result type (might be due to invalid IL or missing references)
			//IL_0698: Unknown result type (might be due to invalid IL or missing references)
			//IL_06c5: Unknown result type (might be due to invalid IL or missing references)
			//IL_06c7: Unknown result type (might be due to invalid IL or missing references)
			//IL_06c9: Unknown result type (might be due to invalid IL or missing references)
			//IL_06ce: Unknown result type (might be due to invalid IL or missing references)
			//IL_06ab: Unknown result type (might be due to invalid IL or missing references)
			//IL_06b2: Unknown result type (might be due to invalid IL or missing references)
			//IL_06b7: Unknown result type (might be due to invalid IL or missing references)
			//IL_06b9: Unknown result type (might be due to invalid IL or missing references)
			//IL_06be: Unknown result type (might be due to invalid IL or missing references)
			//IL_06c3: Unknown result type (might be due to invalid IL or missing references)
			ArchetypeChunk val = m_Chunks[index];
			NativeArray<ObjectGeometryData> nativeArray = ((ArchetypeChunk)(ref val)).GetNativeArray<ObjectGeometryData>(ref m_ObjectGeometryType);
			NativeArray<CarTrailerData> nativeArray2 = ((ArchetypeChunk)(ref val)).GetNativeArray<CarTrailerData>(ref m_CarTrailerType);
			NativeArray<CarData> nativeArray3 = ((ArchetypeChunk)(ref val)).GetNativeArray<CarData>(ref m_CarType);
			NativeArray<TrainData> nativeArray4 = ((ArchetypeChunk)(ref val)).GetNativeArray<TrainData>(ref m_TrainType);
			NativeArray<SwayingData> nativeArray5 = ((ArchetypeChunk)(ref val)).GetNativeArray<SwayingData>(ref m_SwayingType);
			NativeArray<VehicleData> nativeArray6 = ((ArchetypeChunk)(ref val)).GetNativeArray<VehicleData>(ref m_VehicleType);
			BufferAccessor<SubMesh> bufferAccessor = ((ArchetypeChunk)(ref val)).GetBufferAccessor<SubMesh>(ref m_SubmeshType);
			bool flag = ((ArchetypeChunk)(ref val)).Has<MultipleUnitTrainData>(ref m_MultipleUnitTrainType);
			DynamicBuffer<ProceduralBone> val3 = default(DynamicBuffer<ProceduralBone>);
			for (int i = 0; i < nativeArray6.Length; i++)
			{
				VehicleData vehicleData = nativeArray6[i];
				vehicleData.m_SteeringBoneIndex = -1;
				if (bufferAccessor.Length != 0)
				{
					DynamicBuffer<SubMesh> val2 = bufferAccessor[i];
					int num = 0;
					for (int j = 0; j < val2.Length; j++)
					{
						SubMesh subMesh = val2[j];
						if (!m_ProceduralBones.TryGetBuffer(subMesh.m_SubMesh, ref val3))
						{
							continue;
						}
						for (int k = 0; k < val3.Length; k++)
						{
							if (val3[k].m_Type == BoneType.SteeringRotation)
							{
								vehicleData.m_SteeringBoneIndex = num + k;
							}
						}
						num += val3.Length;
					}
				}
				nativeArray6[i] = vehicleData;
			}
			Bounds3 val4 = default(Bounds3);
			DynamicBuffer<ProceduralBone> bones = default(DynamicBuffer<ProceduralBone>);
			for (int l = 0; l < nativeArray3.Length; l++)
			{
				ObjectGeometryData objectGeometryData = nativeArray[l];
				CarData carData = nativeArray3[l];
				SwayingData swayingData = nativeArray5[l];
				((Bounds3)(ref val4))._002Ector(float3.op_Implicit(float.MaxValue), float3.op_Implicit(float.MinValue));
				float3 val5 = float3.op_Implicit(0f);
				float3 val6 = float3.op_Implicit(0f);
				int3 val7 = int3.op_Implicit(0);
				if (bufferAccessor.Length != 0)
				{
					DynamicBuffer<SubMesh> val8 = bufferAccessor[l];
					for (int m = 0; m < val8.Length; m++)
					{
						SubMesh subMesh2 = val8[m];
						if (!m_ProceduralBones.TryGetBuffer(subMesh2.m_SubMesh, ref bones))
						{
							continue;
						}
						for (int n = 0; n < bones.Length; n++)
						{
							ProceduralBone bone = bones[n];
							BoneType type = bone.m_Type;
							if ((uint)(type - 4) <= 1u || type == BoneType.FixedTire)
							{
								float3 val9 = bone.m_ObjectPosition;
								if ((subMesh2.m_Flags & SubMeshFlags.HasTransform) != 0)
								{
									val9 = subMesh2.m_Position + math.rotate(subMesh2.m_Rotation, val9);
								}
								val4 |= val9;
								if (HasSteering(bones, bone))
								{
									val6 += val9;
									int2 yz = ((int3)(ref val7)).yz;
									((int3)(ref val7)).yz = int2.op_Increment(yz);
								}
								else
								{
									val5 += val9;
									int2 yz = ((int3)(ref val7)).xz;
									((int3)(ref val7)).xz = int2.op_Increment(yz);
								}
							}
						}
					}
				}
				if (val7.x != 0)
				{
					carData.m_PivotOffset = val5.z / (float)val7.x;
				}
				else if (val7.y != 0)
				{
					carData.m_PivotOffset = val6.z / (float)val7.y;
				}
				else
				{
					carData.m_PivotOffset = objectGeometryData.m_Size.z * -0.2f;
				}
				if (nativeArray2.Length != 0)
				{
					CarTrailerData carTrailerData = nativeArray2[l];
					val4 |= carTrailerData.m_AttachPosition;
				}
				float2 val10;
				float num2;
				if (val7.z != 0)
				{
					val10 = math.max(float2.op_Implicit(0.5f), MathUtils.Size(((Bounds3)(ref val4)).xz) * 0.5f);
					num2 = (val5.y + val6.y) / (float)val7.z;
				}
				else
				{
					val10 = ((float3)(ref objectGeometryData.m_Size)).xz * new float2(0.45f, 0.3f);
					num2 = objectGeometryData.m_Size.y * 0.25f;
				}
				float3 val11 = math.max(float3.op_Implicit(1f), objectGeometryData.m_Size * objectGeometryData.m_Size);
				float3 val12 = math.max(float3.op_Implicit(1f), swayingData.m_SpringFactors);
				swayingData.m_SpringFactors.x *= 1f + objectGeometryData.m_Size.y * (1f / 3f);
				val12.x *= 1f + objectGeometryData.m_Size.y * (1f / 6f);
				((float3)(ref swayingData.m_VelocityFactors)).xz = (objectGeometryData.m_Size.y * 0.5f - num2) * 12f / (((float3)(ref val11)).yy + ((float3)(ref val11)).xz);
				swayingData.m_VelocityFactors.y = 1f;
				swayingData.m_DampingFactors = 1f / val12;
				swayingData.m_MaxPosition = math.length(objectGeometryData.m_Size) * 3f / (new float3(val10.x, 1f, val10.y) * val12);
				ref float3 springFactors = ref swayingData.m_SpringFactors;
				((float3)(ref springFactors)).xz = ((float3)(ref springFactors)).xz * (val10 * 12f / (((float3)(ref val11)).yy + ((float3)(ref val11)).xz));
				if (val7.z != 0 && val4.max.x - val4.min.x < objectGeometryData.m_Size.x * 0.1f)
				{
					swayingData.m_VelocityFactors.x *= -0.4f;
					swayingData.m_SpringFactors.x *= 0.1f;
					swayingData.m_MaxPosition.x *= 5f;
				}
				nativeArray3[l] = carData;
				nativeArray5[l] = swayingData;
			}
			Bounds3 val13 = default(Bounds3);
			DynamicBuffer<ProceduralBone> val15 = default(DynamicBuffer<ProceduralBone>);
			float2 val17 = default(float2);
			for (int num3 = 0; num3 < nativeArray4.Length; num3++)
			{
				ObjectGeometryData objectGeometryData2 = nativeArray[num3];
				TrainData trainData = nativeArray4[num3];
				((Bounds3)(ref val13))._002Ector(float3.op_Implicit(float.MaxValue), float3.op_Implicit(float.MinValue));
				int num4 = 0;
				if (flag)
				{
					trainData.m_TrainFlags |= TrainFlags.MultiUnit;
				}
				if (bufferAccessor.Length != 0)
				{
					DynamicBuffer<SubMesh> val14 = bufferAccessor[num3];
					for (int num5 = 0; num5 < val14.Length; num5++)
					{
						SubMesh subMesh3 = val14[num5];
						if (!m_ProceduralBones.TryGetBuffer(subMesh3.m_SubMesh, ref val15))
						{
							continue;
						}
						for (int num6 = 0; num6 < val15.Length; num6++)
						{
							ProceduralBone proceduralBone = val15[num6];
							switch (proceduralBone.m_Type)
							{
							case BoneType.TrainBogie:
							{
								float3 val16 = proceduralBone.m_ObjectPosition;
								if ((subMesh3.m_Flags & SubMeshFlags.HasTransform) != 0)
								{
									val16 = subMesh3.m_Position + math.rotate(subMesh3.m_Rotation, val16);
								}
								val13 |= val16;
								num4++;
								break;
							}
							case BoneType.PantographRotation:
								trainData.m_TrainFlags |= TrainFlags.Pantograph;
								break;
							}
						}
					}
				}
				if (num4 >= 2)
				{
					trainData.m_BogieOffsets = new float2(val13.max.z, 0f - val13.min.z) - trainData.m_BogieOffsets;
				}
				else if (num4 == 1)
				{
					float num7 = MathUtils.Size(((Bounds3)(ref objectGeometryData2.m_Bounds)).x) * 0.5f;
					trainData.m_BogieOffsets = MathUtils.Center(((Bounds3)(ref val13)).z) + num7 - trainData.m_BogieOffsets;
				}
				else
				{
					((float2)(ref val17))._002Ector(objectGeometryData2.m_Bounds.max.z, 0f - objectGeometryData2.m_Bounds.min.z);
					trainData.m_BogieOffsets = val17 - MathUtils.Size(((Bounds3)(ref objectGeometryData2.m_Bounds)).z) * 0.15f - trainData.m_BogieOffsets;
				}
				nativeArray4[num3] = trainData;
			}
		}

		private bool HasSteering(DynamicBuffer<ProceduralBone> bones, ProceduralBone bone)
		{
			if (bone.m_Type == BoneType.SteeringTire || bone.m_Type == BoneType.SteeringRotation || bone.m_Type == BoneType.SteeringSuspension)
			{
				return true;
			}
			while (bone.m_ParentIndex >= 0)
			{
				bone = bones[bone.m_ParentIndex];
				if (bone.m_Type == BoneType.SteeringTire || bone.m_Type == BoneType.SteeringRotation || bone.m_Type == BoneType.SteeringSuspension)
				{
					return true;
				}
			}
			return false;
		}
	}

	private struct TypeHandle
	{
		[ReadOnly]
		public ComponentTypeHandle<PrefabData> __Game_Prefabs_PrefabData_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<ObjectGeometryData> __Game_Prefabs_ObjectGeometryData_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<MultipleUnitTrainData> __Game_Prefabs_MultipleUnitTrainData_RO_ComponentTypeHandle;

		public ComponentTypeHandle<TrainData> __Game_Prefabs_TrainData_RW_ComponentTypeHandle;

		public ComponentTypeHandle<CarData> __Game_Prefabs_CarData_RW_ComponentTypeHandle;

		public ComponentTypeHandle<SwayingData> __Game_Prefabs_SwayingData_RW_ComponentTypeHandle;

		public ComponentTypeHandle<VehicleData> __Game_Prefabs_VehicleData_RW_ComponentTypeHandle;

		public ComponentTypeHandle<CarTractorData> __Game_Prefabs_CarTractorData_RW_ComponentTypeHandle;

		public ComponentTypeHandle<CarTrailerData> __Game_Prefabs_CarTrailerData_RW_ComponentTypeHandle;

		[ReadOnly]
		public BufferTypeHandle<SubMesh> __Game_Prefabs_SubMesh_RO_BufferTypeHandle;

		[ReadOnly]
		public BufferLookup<ProceduralBone> __Game_Prefabs_ProceduralBone_RO_BufferLookup;

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
			__Game_Prefabs_PrefabData_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<PrefabData>(true);
			__Game_Prefabs_ObjectGeometryData_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<ObjectGeometryData>(true);
			__Game_Prefabs_MultipleUnitTrainData_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<MultipleUnitTrainData>(true);
			__Game_Prefabs_TrainData_RW_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<TrainData>(false);
			__Game_Prefabs_CarData_RW_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<CarData>(false);
			__Game_Prefabs_SwayingData_RW_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<SwayingData>(false);
			__Game_Prefabs_VehicleData_RW_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<VehicleData>(false);
			__Game_Prefabs_CarTractorData_RW_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<CarTractorData>(false);
			__Game_Prefabs_CarTrailerData_RW_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<CarTrailerData>(false);
			__Game_Prefabs_SubMesh_RO_BufferTypeHandle = ((SystemState)(ref state)).GetBufferTypeHandle<SubMesh>(true);
			__Game_Prefabs_ProceduralBone_RO_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<ProceduralBone>(true);
		}
	}

	private EntityQuery m_PrefabQuery;

	private PrefabSystem m_PrefabSystem;

	private TypeHandle __TypeHandle;

	[Preserve]
	protected override void OnCreate()
	{
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Expected O, but got Unknown
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
		//IL_007d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0089: Unknown result type (might be due to invalid IL or missing references)
		//IL_008e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0095: Unknown result type (might be due to invalid IL or missing references)
		base.OnCreate();
		m_PrefabSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<PrefabSystem>();
		EntityQueryDesc[] array = new EntityQueryDesc[1];
		EntityQueryDesc val = new EntityQueryDesc();
		val.All = (ComponentType[])(object)new ComponentType[2]
		{
			ComponentType.ReadOnly<Created>(),
			ComponentType.ReadOnly<PrefabData>()
		};
		val.Any = (ComponentType[])(object)new ComponentType[4]
		{
			ComponentType.ReadWrite<CarData>(),
			ComponentType.ReadWrite<TrainData>(),
			ComponentType.ReadWrite<CarTractorData>(),
			ComponentType.ReadWrite<CarTrailerData>()
		};
		array[0] = val;
		m_PrefabQuery = ((ComponentSystemBase)this).GetEntityQuery((EntityQueryDesc[])(object)array);
		((ComponentSystemBase)this).RequireForUpdate(m_PrefabQuery);
	}

	[Preserve]
	protected override void OnUpdate()
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		//IL_0085: Unknown result type (might be due to invalid IL or missing references)
		//IL_0098: Unknown result type (might be due to invalid IL or missing references)
		//IL_009d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fd: Unknown result type (might be due to invalid IL or missing references)
		//IL_0111: Unknown result type (might be due to invalid IL or missing references)
		//IL_0116: Unknown result type (might be due to invalid IL or missing references)
		//IL_011c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0121: Unknown result type (might be due to invalid IL or missing references)
		//IL_0127: Unknown result type (might be due to invalid IL or missing references)
		//IL_012c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0132: Unknown result type (might be due to invalid IL or missing references)
		//IL_0137: Unknown result type (might be due to invalid IL or missing references)
		//IL_013d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0142: Unknown result type (might be due to invalid IL or missing references)
		//IL_0148: Unknown result type (might be due to invalid IL or missing references)
		//IL_014d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0153: Unknown result type (might be due to invalid IL or missing references)
		//IL_0158: Unknown result type (might be due to invalid IL or missing references)
		//IL_015e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0163: Unknown result type (might be due to invalid IL or missing references)
		//IL_04fb: Unknown result type (might be due to invalid IL or missing references)
		//IL_04fc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0503: Unknown result type (might be due to invalid IL or missing references)
		//IL_0505: Unknown result type (might be due to invalid IL or missing references)
		//IL_050c: Unknown result type (might be due to invalid IL or missing references)
		//IL_050d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0514: Unknown result type (might be due to invalid IL or missing references)
		//IL_0516: Unknown result type (might be due to invalid IL or missing references)
		//IL_051d: Unknown result type (might be due to invalid IL or missing references)
		//IL_051f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0526: Unknown result type (might be due to invalid IL or missing references)
		//IL_0528: Unknown result type (might be due to invalid IL or missing references)
		//IL_052f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0531: Unknown result type (might be due to invalid IL or missing references)
		//IL_0538: Unknown result type (might be due to invalid IL or missing references)
		//IL_053a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0552: Unknown result type (might be due to invalid IL or missing references)
		//IL_0557: Unknown result type (might be due to invalid IL or missing references)
		//IL_055e: Unknown result type (might be due to invalid IL or missing references)
		//IL_055f: Unknown result type (might be due to invalid IL or missing references)
		//IL_056f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0574: Unknown result type (might be due to invalid IL or missing references)
		//IL_0579: Unknown result type (might be due to invalid IL or missing references)
		//IL_057c: Unknown result type (might be due to invalid IL or missing references)
		//IL_019d: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_0213: Unknown result type (might be due to invalid IL or missing references)
		//IL_0218: Unknown result type (might be due to invalid IL or missing references)
		//IL_021d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0226: Unknown result type (might be due to invalid IL or missing references)
		//IL_022b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0232: Unknown result type (might be due to invalid IL or missing references)
		//IL_0236: Unknown result type (might be due to invalid IL or missing references)
		//IL_023b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0240: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e8: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ed: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f2: Unknown result type (might be due to invalid IL or missing references)
		//IL_0300: Unknown result type (might be due to invalid IL or missing references)
		//IL_0305: Unknown result type (might be due to invalid IL or missing references)
		//IL_0389: Unknown result type (might be due to invalid IL or missing references)
		//IL_039c: Unknown result type (might be due to invalid IL or missing references)
		//IL_03a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_03db: Unknown result type (might be due to invalid IL or missing references)
		//IL_03e0: Unknown result type (might be due to invalid IL or missing references)
		//IL_0462: Unknown result type (might be due to invalid IL or missing references)
		//IL_0475: Unknown result type (might be due to invalid IL or missing references)
		//IL_047a: Unknown result type (might be due to invalid IL or missing references)
		//IL_04b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_04b9: Unknown result type (might be due to invalid IL or missing references)
		NativeArray<ArchetypeChunk> chunks = ((EntityQuery)(ref m_PrefabQuery)).ToArchetypeChunkArray(AllocatorHandle.op_Implicit((Allocator)3));
		ComponentTypeHandle<PrefabData> componentTypeHandle = InternalCompilerInterface.GetComponentTypeHandle<PrefabData>(ref __TypeHandle.__Game_Prefabs_PrefabData_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef);
		ComponentTypeHandle<ObjectGeometryData> componentTypeHandle2 = InternalCompilerInterface.GetComponentTypeHandle<ObjectGeometryData>(ref __TypeHandle.__Game_Prefabs_ObjectGeometryData_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef);
		ComponentTypeHandle<MultipleUnitTrainData> componentTypeHandle3 = InternalCompilerInterface.GetComponentTypeHandle<MultipleUnitTrainData>(ref __TypeHandle.__Game_Prefabs_MultipleUnitTrainData_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef);
		ComponentTypeHandle<TrainData> componentTypeHandle4 = InternalCompilerInterface.GetComponentTypeHandle<TrainData>(ref __TypeHandle.__Game_Prefabs_TrainData_RW_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef);
		ComponentTypeHandle<CarData> componentTypeHandle5 = InternalCompilerInterface.GetComponentTypeHandle<CarData>(ref __TypeHandle.__Game_Prefabs_CarData_RW_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef);
		ComponentTypeHandle<SwayingData> componentTypeHandle6 = InternalCompilerInterface.GetComponentTypeHandle<SwayingData>(ref __TypeHandle.__Game_Prefabs_SwayingData_RW_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef);
		ComponentTypeHandle<VehicleData> componentTypeHandle7 = InternalCompilerInterface.GetComponentTypeHandle<VehicleData>(ref __TypeHandle.__Game_Prefabs_VehicleData_RW_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef);
		ComponentTypeHandle<CarTractorData> componentTypeHandle8 = InternalCompilerInterface.GetComponentTypeHandle<CarTractorData>(ref __TypeHandle.__Game_Prefabs_CarTractorData_RW_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef);
		ComponentTypeHandle<CarTrailerData> componentTypeHandle9 = InternalCompilerInterface.GetComponentTypeHandle<CarTrailerData>(ref __TypeHandle.__Game_Prefabs_CarTrailerData_RW_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef);
		BufferTypeHandle<SubMesh> bufferTypeHandle = InternalCompilerInterface.GetBufferTypeHandle<SubMesh>(ref __TypeHandle.__Game_Prefabs_SubMesh_RO_BufferTypeHandle, ref ((SystemBase)this).CheckedStateRef);
		((SystemBase)this).CompleteDependency();
		float2 val2 = default(float2);
		for (int i = 0; i < chunks.Length; i++)
		{
			ArchetypeChunk val = chunks[i];
			NativeArray<PrefabData> nativeArray = ((ArchetypeChunk)(ref val)).GetNativeArray<PrefabData>(ref componentTypeHandle);
			NativeArray<ObjectGeometryData> nativeArray2 = ((ArchetypeChunk)(ref val)).GetNativeArray<ObjectGeometryData>(ref componentTypeHandle2);
			NativeArray<TrainData> nativeArray3 = ((ArchetypeChunk)(ref val)).GetNativeArray<TrainData>(ref componentTypeHandle4);
			NativeArray<CarData> nativeArray4 = ((ArchetypeChunk)(ref val)).GetNativeArray<CarData>(ref componentTypeHandle5);
			NativeArray<SwayingData> nativeArray5 = ((ArchetypeChunk)(ref val)).GetNativeArray<SwayingData>(ref componentTypeHandle6);
			NativeArray<CarTractorData> nativeArray6 = ((ArchetypeChunk)(ref val)).GetNativeArray<CarTractorData>(ref componentTypeHandle8);
			NativeArray<CarTrailerData> nativeArray7 = ((ArchetypeChunk)(ref val)).GetNativeArray<CarTrailerData>(ref componentTypeHandle9);
			for (int j = 0; j < nativeArray3.Length; j++)
			{
				TrainPrefab prefab = m_PrefabSystem.GetPrefab<TrainPrefab>(nativeArray[j]);
				ObjectGeometryData objectGeometryData = nativeArray2[j];
				TrainData trainData = nativeArray3[j];
				((float2)(ref val2))._002Ector(objectGeometryData.m_Bounds.max.z, 0f - objectGeometryData.m_Bounds.min.z);
				trainData.m_TrackType = prefab.m_TrackType;
				trainData.m_EnergyType = prefab.m_EnergyType;
				trainData.m_MaxSpeed = prefab.m_MaxSpeed / 3.6f;
				trainData.m_Acceleration = prefab.m_Acceleration;
				trainData.m_Braking = prefab.m_Braking;
				trainData.m_Turning = math.radians(prefab.m_Turning);
				trainData.m_BogieOffsets = prefab.m_BogieOffset;
				trainData.m_AttachOffsets = val2 - prefab.m_AttachOffset;
				nativeArray3[j] = trainData;
			}
			for (int k = 0; k < nativeArray4.Length; k++)
			{
				CarBasePrefab prefab2 = m_PrefabSystem.GetPrefab<CarBasePrefab>(nativeArray[k]);
				CarData carData = nativeArray4[k];
				SwayingData swayingData = nativeArray5[k];
				carData.m_SizeClass = prefab2.m_SizeClass;
				carData.m_EnergyType = prefab2.m_EnergyType;
				carData.m_MaxSpeed = prefab2.m_MaxSpeed / 3.6f;
				carData.m_Acceleration = prefab2.m_Acceleration;
				carData.m_Braking = prefab2.m_Braking;
				carData.m_Turning = math.radians(prefab2.m_Turning);
				swayingData.m_SpringFactors = float3.op_Implicit(prefab2.m_Stiffness);
				nativeArray4[k] = carData;
				nativeArray5[k] = swayingData;
			}
			for (int l = 0; l < nativeArray6.Length; l++)
			{
				CarTractor component = m_PrefabSystem.GetPrefab<VehiclePrefab>(nativeArray[l]).GetComponent<CarTractor>();
				ObjectGeometryData objectGeometryData2 = nativeArray2[l];
				CarTractorData carTractorData = nativeArray6[l];
				carTractorData.m_TrailerType = component.m_TrailerType;
				((float3)(ref carTractorData.m_AttachPosition)).xy = ((float3)(ref component.m_AttachOffset)).xy;
				carTractorData.m_AttachPosition.z = objectGeometryData2.m_Bounds.min.z + component.m_AttachOffset.z;
				if ((Object)(object)component.m_FixedTrailer != (Object)null)
				{
					carTractorData.m_FixedTrailer = m_PrefabSystem.GetEntity(component.m_FixedTrailer);
				}
				nativeArray6[l] = carTractorData;
			}
			for (int m = 0; m < nativeArray7.Length; m++)
			{
				CarTrailerPrefab prefab3 = m_PrefabSystem.GetPrefab<CarTrailerPrefab>(nativeArray[m]);
				ObjectGeometryData objectGeometryData3 = nativeArray2[m];
				CarTrailerData carTrailerData = nativeArray7[m];
				carTrailerData.m_TrailerType = prefab3.m_TrailerType;
				carTrailerData.m_MovementType = prefab3.m_MovementType;
				((float3)(ref carTrailerData.m_AttachPosition)).xy = ((float3)(ref prefab3.m_AttachOffset)).xy;
				carTrailerData.m_AttachPosition.z = objectGeometryData3.m_Bounds.max.z - prefab3.m_AttachOffset.z;
				if ((Object)(object)prefab3.m_FixedTractor != (Object)null)
				{
					carTrailerData.m_FixedTractor = m_PrefabSystem.GetEntity(prefab3.m_FixedTractor);
				}
				nativeArray7[m] = carTrailerData;
			}
		}
		JobHandle dependency = IJobParallelForExtensions.Schedule<InitializeVehiclesJob>(new InitializeVehiclesJob
		{
			m_ObjectGeometryType = componentTypeHandle2,
			m_CarTrailerType = componentTypeHandle9,
			m_MultipleUnitTrainType = componentTypeHandle3,
			m_SubmeshType = bufferTypeHandle,
			m_CarType = componentTypeHandle5,
			m_TrainType = componentTypeHandle4,
			m_SwayingType = componentTypeHandle6,
			m_VehicleType = componentTypeHandle7,
			m_ProceduralBones = InternalCompilerInterface.GetBufferLookup<ProceduralBone>(ref __TypeHandle.__Game_Prefabs_ProceduralBone_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
			m_Chunks = chunks
		}, chunks.Length, 1, ((SystemBase)this).Dependency);
		((SystemBase)this).Dependency = dependency;
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
	public VehicleInitializeSystem()
	{
	}
}

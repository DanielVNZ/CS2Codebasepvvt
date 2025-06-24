using System;
using System.Runtime.CompilerServices;
using Colossal.Collections;
using Colossal.Serialization.Entities;
using Game.Common;
using Game.Creatures;
using Game.Prefabs;
using Game.Tools;
using Unity.Burst;
using Unity.Burst.Intrinsics;
using Unity.Collections;
using Unity.Entities;
using Unity.Entities.Internal;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine.Scripting;

namespace Game.Rendering;

[CompilerGenerated]
public class MeshGroupSystem : GameSystemBase
{
	[BurstCompile]
	private struct SetMeshGroupsJob : IJobChunk
	{
		[ReadOnly]
		public ComponentTypeHandle<PseudoRandomSeed> m_PseudoRandomSeedType;

		[ReadOnly]
		public ComponentTypeHandle<Temp> m_TempType;

		[ReadOnly]
		public ComponentTypeHandle<Human> m_HumanType;

		[ReadOnly]
		public ComponentTypeHandle<CurrentVehicle> m_CurrentVehicleType;

		[ReadOnly]
		public ComponentTypeHandle<PrefabRef> m_PrefabRefType;

		public BufferTypeHandle<MeshGroup> m_MeshGroupType;

		public BufferTypeHandle<MeshBatch> m_MeshBatchType;

		[ReadOnly]
		public ComponentLookup<PrefabRef> m_PrefabRefData;

		[ReadOnly]
		public ComponentLookup<Human> m_HumanData;

		[ReadOnly]
		public ComponentLookup<CurrentVehicle> m_CurrentVehicleData;

		[ReadOnly]
		public BufferLookup<SubMeshGroup> m_SubMeshGroups;

		[ReadOnly]
		public BufferLookup<SubMesh> m_SubMeshes;

		[ReadOnly]
		public BufferLookup<OverlayElement> m_OverlayElements;

		[ReadOnly]
		public BufferLookup<ActivityLocationElement> m_ActivityLocations;

		[ReadOnly]
		public RandomSeed m_RandomSeed;

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
			//IL_0077: Unknown result type (might be due to invalid IL or missing references)
			//IL_007d: Unknown result type (might be due to invalid IL or missing references)
			//IL_006e: Unknown result type (might be due to invalid IL or missing references)
			//IL_007f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0083: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a5: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ab: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
			//IL_010e: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e4: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d6: Unknown result type (might be due to invalid IL or missing references)
			//IL_0127: Unknown result type (might be due to invalid IL or missing references)
			//IL_012c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0131: Unknown result type (might be due to invalid IL or missing references)
			//IL_013c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0159: Unknown result type (might be due to invalid IL or missing references)
			//IL_014f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0154: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ce: Unknown result type (might be due to invalid IL or missing references)
			//IL_0167: Unknown result type (might be due to invalid IL or missing references)
			//IL_016c: Unknown result type (might be due to invalid IL or missing references)
			//IL_03b6: Unknown result type (might be due to invalid IL or missing references)
			//IL_03b8: Unknown result type (might be due to invalid IL or missing references)
			//IL_01e6: Unknown result type (might be due to invalid IL or missing references)
			//IL_0180: Unknown result type (might be due to invalid IL or missing references)
			//IL_038a: Unknown result type (might be due to invalid IL or missing references)
			//IL_038c: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a2: Unknown result type (might be due to invalid IL or missing references)
			//IL_02b9: Unknown result type (might be due to invalid IL or missing references)
			//IL_02c5: Unknown result type (might be due to invalid IL or missing references)
			//IL_02d7: Unknown result type (might be due to invalid IL or missing references)
			//IL_02e3: Unknown result type (might be due to invalid IL or missing references)
			//IL_02f3: Unknown result type (might be due to invalid IL or missing references)
			//IL_0332: Unknown result type (might be due to invalid IL or missing references)
			//IL_0314: Unknown result type (might be due to invalid IL or missing references)
			NativeArray<PseudoRandomSeed> nativeArray = ((ArchetypeChunk)(ref chunk)).GetNativeArray<PseudoRandomSeed>(ref m_PseudoRandomSeedType);
			NativeArray<Temp> nativeArray2 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<Temp>(ref m_TempType);
			NativeArray<Human> nativeArray3 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<Human>(ref m_HumanType);
			NativeArray<CurrentVehicle> nativeArray4 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<CurrentVehicle>(ref m_CurrentVehicleType);
			NativeArray<PrefabRef> nativeArray5 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<PrefabRef>(ref m_PrefabRefType);
			BufferAccessor<MeshGroup> bufferAccessor = ((ArchetypeChunk)(ref chunk)).GetBufferAccessor<MeshGroup>(ref m_MeshGroupType);
			BufferAccessor<MeshBatch> bufferAccessor2 = ((ArchetypeChunk)(ref chunk)).GetBufferAccessor<MeshBatch>(ref m_MeshBatchType);
			Random val = (Random)((nativeArray.Length != 0) ? default(Random) : m_RandomSeed.GetRandom(unfilteredChunkIndex));
			NativeList<MeshGroup> val2 = default(NativeList<MeshGroup>);
			DynamicBuffer<SubMeshGroup> val3 = default(DynamicBuffer<SubMeshGroup>);
			PseudoRandomSeed pseudoRandomSeed = default(PseudoRandomSeed);
			Temp temp = default(Temp);
			Human human = default(Human);
			CurrentVehicle currentVehicle = default(CurrentVehicle);
			Human human2 = default(Human);
			CurrentVehicle currentVehicle2 = default(CurrentVehicle);
			for (int i = 0; i < bufferAccessor.Length; i++)
			{
				PrefabRef prefabRef = nativeArray5[i];
				DynamicBuffer<MeshGroup> newGroups = bufferAccessor[i];
				DynamicBuffer<MeshBatch> batches = bufferAccessor2[i];
				MeshGroup oldGroup = default(MeshGroup);
				int length = newGroups.Length;
				if (length > 1)
				{
					if (!val2.IsCreated)
					{
						val2._002Ector(length, AllocatorHandle.op_Implicit((Allocator)2));
					}
					val2.AddRange(newGroups.AsNativeArray());
				}
				else if (length == 1)
				{
					oldGroup = newGroups[0];
				}
				newGroups.Clear();
				if (m_SubMeshGroups.TryGetBuffer(prefabRef.m_Prefab, ref val3))
				{
					DynamicBuffer<SubMesh> val4 = m_SubMeshes[prefabRef.m_Prefab];
					int num = 0;
					int num2 = 0;
					int num3 = 0;
					if (CollectionUtils.TryGet<PseudoRandomSeed>(nativeArray, i, ref pseudoRandomSeed))
					{
						val = pseudoRandomSeed.GetRandom(PseudoRandomSeed.kMeshGroup);
					}
					MeshGroupFlags meshGroupFlags = (MeshGroupFlags)0u;
					if (CollectionUtils.TryGet<Temp>(nativeArray2, i, ref temp) && temp.m_Original != Entity.Null)
					{
						if (m_HumanData.TryGetComponent(temp.m_Original, ref human))
						{
							meshGroupFlags |= GetHumanFlags(human);
						}
						meshGroupFlags = ((!m_CurrentVehicleData.TryGetComponent(temp.m_Original, ref currentVehicle)) ? (meshGroupFlags | MeshGroupFlags.ForbidMotorcycle) : (meshGroupFlags | GetCurrentVehicleFlags(currentVehicle)));
					}
					else
					{
						if (CollectionUtils.TryGet<Human>(nativeArray3, i, ref human2))
						{
							meshGroupFlags |= GetHumanFlags(human2);
						}
						meshGroupFlags = ((!CollectionUtils.TryGet<CurrentVehicle>(nativeArray4, i, ref currentVehicle2)) ? (meshGroupFlags | MeshGroupFlags.ForbidMotorcycle) : (meshGroupFlags | GetCurrentVehicleFlags(currentVehicle2)));
					}
					while (num < val3.Length)
					{
						SubMeshGroup subMeshGroup = val3[num];
						int num4 = num;
						num += subMeshGroup.m_SubGroupCount;
						if (subMeshGroup.m_SubGroupCount <= 0 || num + subMeshGroup.m_SubGroupCount >= 65536)
						{
							throw new Exception("Invalid m_SubGroupCount!");
						}
						if ((subMeshGroup.m_Flags & meshGroupFlags) != subMeshGroup.m_Flags)
						{
							continue;
						}
						num4 += ((Random)(ref val)).NextInt(subMeshGroup.m_SubGroupCount);
						subMeshGroup = val3[num4];
						newGroups.Add(new MeshGroup
						{
							m_SubMeshGroup = (ushort)num4,
							m_MeshOffset = (byte)num2,
							m_ColorOffset = (byte)num3
						});
						num2 += subMeshGroup.m_SubMeshRange.y - subMeshGroup.m_SubMeshRange.x;
						num3 += subMeshGroup.m_SubMeshRange.y - subMeshGroup.m_SubMeshRange.x;
						for (int j = subMeshGroup.m_SubMeshRange.x; j < subMeshGroup.m_SubMeshRange.y; j++)
						{
							SubMesh subMesh = val4[j];
							if (m_OverlayElements.HasBuffer(subMesh.m_SubMesh))
							{
								num3 += 8;
								break;
							}
						}
					}
				}
				else
				{
					newGroups.Add(new MeshGroup
					{
						m_SubMeshGroup = ushort.MaxValue,
						m_MeshOffset = 0
					});
				}
				if (length > 1)
				{
					for (int k = 0; k < val2.Length; k++)
					{
						TryRemoveBatches(val2[k], k, newGroups, batches);
					}
					val2.Clear();
				}
				else if (length == 1)
				{
					TryRemoveBatches(oldGroup, 0, newGroups, batches);
				}
			}
			if (val2.IsCreated)
			{
				val2.Dispose();
			}
		}

		private void TryRemoveBatches(MeshGroup oldGroup, int groupIndex, DynamicBuffer<MeshGroup> newGroups, DynamicBuffer<MeshBatch> batches)
		{
			for (int i = 0; i < newGroups.Length; i++)
			{
				if (newGroups[i].m_SubMeshGroup == oldGroup.m_SubMeshGroup)
				{
					return;
				}
			}
			for (int j = 0; j < batches.Length; j++)
			{
				MeshBatch meshBatch = batches[j];
				if (meshBatch.m_MeshGroup == groupIndex)
				{
					meshBatch.m_MeshGroup = byte.MaxValue;
					meshBatch.m_MeshIndex = byte.MaxValue;
					meshBatch.m_TileIndex = byte.MaxValue;
					batches[j] = meshBatch;
				}
			}
		}

		public static MeshGroupFlags GetHumanFlags(Human human)
		{
			MeshGroupFlags meshGroupFlags = (MeshGroupFlags)0u;
			meshGroupFlags = (((human.m_Flags & HumanFlags.Cold) == 0) ? (meshGroupFlags | MeshGroupFlags.RequireWarm) : (meshGroupFlags | MeshGroupFlags.RequireCold));
			if ((human.m_Flags & HumanFlags.Homeless) != 0)
			{
				return meshGroupFlags | MeshGroupFlags.RequireHomeless;
			}
			return meshGroupFlags | MeshGroupFlags.RequireHome;
		}

		private MeshGroupFlags GetCurrentVehicleFlags(CurrentVehicle currentVehicle)
		{
			//IL_000a: Unknown result type (might be due to invalid IL or missing references)
			//IL_001f: Unknown result type (might be due to invalid IL or missing references)
			MeshGroupFlags meshGroupFlags = MeshGroupFlags.ForbidMotorcycle;
			PrefabRef prefabRef = default(PrefabRef);
			DynamicBuffer<ActivityLocationElement> val = default(DynamicBuffer<ActivityLocationElement>);
			if (m_PrefabRefData.TryGetComponent(currentVehicle.m_Vehicle, ref prefabRef) && m_ActivityLocations.TryGetBuffer(prefabRef.m_Prefab, ref val))
			{
				ActivityMask activityMask = new ActivityMask(ActivityType.Driving);
				for (int i = 0; i < val.Length; i++)
				{
					if ((val[i].m_ActivityMask.m_Mask & activityMask.m_Mask) != 0)
					{
						meshGroupFlags = (MeshGroupFlags)((uint)meshGroupFlags & 0xFFFFFFDFu);
						meshGroupFlags |= MeshGroupFlags.RequireMotorcycle;
					}
				}
			}
			return meshGroupFlags;
		}

		void IJobChunk.Execute(in ArchetypeChunk chunk, int unfilteredChunkIndex, bool useEnabledMask, in v128 chunkEnabledMask)
		{
			Execute(in chunk, unfilteredChunkIndex, useEnabledMask, in chunkEnabledMask);
		}
	}

	private struct TypeHandle
	{
		[ReadOnly]
		public ComponentTypeHandle<PseudoRandomSeed> __Game_Common_PseudoRandomSeed_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<Temp> __Game_Tools_Temp_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<Human> __Game_Creatures_Human_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<CurrentVehicle> __Game_Creatures_CurrentVehicle_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<PrefabRef> __Game_Prefabs_PrefabRef_RO_ComponentTypeHandle;

		public BufferTypeHandle<MeshGroup> __Game_Rendering_MeshGroup_RW_BufferTypeHandle;

		public BufferTypeHandle<MeshBatch> __Game_Rendering_MeshBatch_RW_BufferTypeHandle;

		[ReadOnly]
		public ComponentLookup<PrefabRef> __Game_Prefabs_PrefabRef_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Human> __Game_Creatures_Human_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<CurrentVehicle> __Game_Creatures_CurrentVehicle_RO_ComponentLookup;

		[ReadOnly]
		public BufferLookup<SubMeshGroup> __Game_Prefabs_SubMeshGroup_RO_BufferLookup;

		[ReadOnly]
		public BufferLookup<SubMesh> __Game_Prefabs_SubMesh_RO_BufferLookup;

		[ReadOnly]
		public BufferLookup<OverlayElement> __Game_Prefabs_OverlayElement_RO_BufferLookup;

		[ReadOnly]
		public BufferLookup<ActivityLocationElement> __Game_Prefabs_ActivityLocationElement_RO_BufferLookup;

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
			__Game_Common_PseudoRandomSeed_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<PseudoRandomSeed>(true);
			__Game_Tools_Temp_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<Temp>(true);
			__Game_Creatures_Human_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<Human>(true);
			__Game_Creatures_CurrentVehicle_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<CurrentVehicle>(true);
			__Game_Prefabs_PrefabRef_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<PrefabRef>(true);
			__Game_Rendering_MeshGroup_RW_BufferTypeHandle = ((SystemState)(ref state)).GetBufferTypeHandle<MeshGroup>(false);
			__Game_Rendering_MeshBatch_RW_BufferTypeHandle = ((SystemState)(ref state)).GetBufferTypeHandle<MeshBatch>(false);
			__Game_Prefabs_PrefabRef_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<PrefabRef>(true);
			__Game_Creatures_Human_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Human>(true);
			__Game_Creatures_CurrentVehicle_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<CurrentVehicle>(true);
			__Game_Prefabs_SubMeshGroup_RO_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<SubMeshGroup>(true);
			__Game_Prefabs_SubMesh_RO_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<SubMesh>(true);
			__Game_Prefabs_OverlayElement_RO_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<OverlayElement>(true);
			__Game_Prefabs_ActivityLocationElement_RO_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<ActivityLocationElement>(true);
		}
	}

	private EntityQuery m_UpdateQuery;

	private EntityQuery m_AllQuery;

	private bool m_Loaded;

	private TypeHandle __TypeHandle;

	[Preserve]
	protected override void OnCreate()
	{
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Expected O, but got Unknown
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		//IL_0077: Unknown result type (might be due to invalid IL or missing references)
		base.OnCreate();
		EntityQueryDesc[] array = new EntityQueryDesc[1];
		EntityQueryDesc val = new EntityQueryDesc();
		val.All = (ComponentType[])(object)new ComponentType[1] { ComponentType.ReadOnly<MeshGroup>() };
		val.Any = (ComponentType[])(object)new ComponentType[2]
		{
			ComponentType.ReadOnly<Updated>(),
			ComponentType.ReadOnly<BatchesUpdated>()
		};
		array[0] = val;
		m_UpdateQuery = ((ComponentSystemBase)this).GetEntityQuery((EntityQueryDesc[])(object)array);
		m_AllQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[1] { ComponentType.ReadOnly<MeshGroup>() });
	}

	protected override void OnGameLoaded(Context serializationContext)
	{
		m_Loaded = true;
	}

	private bool GetLoaded()
	{
		if (m_Loaded)
		{
			m_Loaded = false;
			return true;
		}
		return false;
	}

	[Preserve]
	protected override void OnUpdate()
	{
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
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
		//IL_01cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01da: Unknown result type (might be due to invalid IL or missing references)
		EntityQuery val = (GetLoaded() ? m_AllQuery : m_UpdateQuery);
		if (!((EntityQuery)(ref val)).IsEmptyIgnoreFilter)
		{
			JobHandle dependency = JobChunkExtensions.ScheduleParallel<SetMeshGroupsJob>(new SetMeshGroupsJob
			{
				m_PseudoRandomSeedType = InternalCompilerInterface.GetComponentTypeHandle<PseudoRandomSeed>(ref __TypeHandle.__Game_Common_PseudoRandomSeed_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
				m_TempType = InternalCompilerInterface.GetComponentTypeHandle<Temp>(ref __TypeHandle.__Game_Tools_Temp_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
				m_HumanType = InternalCompilerInterface.GetComponentTypeHandle<Human>(ref __TypeHandle.__Game_Creatures_Human_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
				m_CurrentVehicleType = InternalCompilerInterface.GetComponentTypeHandle<CurrentVehicle>(ref __TypeHandle.__Game_Creatures_CurrentVehicle_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
				m_PrefabRefType = InternalCompilerInterface.GetComponentTypeHandle<PrefabRef>(ref __TypeHandle.__Game_Prefabs_PrefabRef_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
				m_MeshGroupType = InternalCompilerInterface.GetBufferTypeHandle<MeshGroup>(ref __TypeHandle.__Game_Rendering_MeshGroup_RW_BufferTypeHandle, ref ((SystemBase)this).CheckedStateRef),
				m_MeshBatchType = InternalCompilerInterface.GetBufferTypeHandle<MeshBatch>(ref __TypeHandle.__Game_Rendering_MeshBatch_RW_BufferTypeHandle, ref ((SystemBase)this).CheckedStateRef),
				m_PrefabRefData = InternalCompilerInterface.GetComponentLookup<PrefabRef>(ref __TypeHandle.__Game_Prefabs_PrefabRef_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_HumanData = InternalCompilerInterface.GetComponentLookup<Human>(ref __TypeHandle.__Game_Creatures_Human_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_CurrentVehicleData = InternalCompilerInterface.GetComponentLookup<CurrentVehicle>(ref __TypeHandle.__Game_Creatures_CurrentVehicle_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_SubMeshGroups = InternalCompilerInterface.GetBufferLookup<SubMeshGroup>(ref __TypeHandle.__Game_Prefabs_SubMeshGroup_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
				m_SubMeshes = InternalCompilerInterface.GetBufferLookup<SubMesh>(ref __TypeHandle.__Game_Prefabs_SubMesh_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
				m_OverlayElements = InternalCompilerInterface.GetBufferLookup<OverlayElement>(ref __TypeHandle.__Game_Prefabs_OverlayElement_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
				m_ActivityLocations = InternalCompilerInterface.GetBufferLookup<ActivityLocationElement>(ref __TypeHandle.__Game_Prefabs_ActivityLocationElement_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
				m_RandomSeed = RandomSeed.Next()
			}, val, ((SystemBase)this).Dependency);
			((SystemBase)this).Dependency = dependency;
		}
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
	public MeshGroupSystem()
	{
	}
}

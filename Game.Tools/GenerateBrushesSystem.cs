using System.Runtime.CompilerServices;
using Colossal.Mathematics;
using Game.Common;
using Game.Prefabs;
using Unity.Burst;
using Unity.Burst.Intrinsics;
using Unity.Collections;
using Unity.Entities;
using Unity.Entities.Internal;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Scripting;

namespace Game.Tools;

[CompilerGenerated]
public class GenerateBrushesSystem : GameSystemBase
{
	[BurstCompile]
	private struct GenerateBrushesJob : IJobChunk
	{
		[ReadOnly]
		public ComponentTypeHandle<CreationDefinition> m_CreationDefinitionType;

		[ReadOnly]
		public ComponentTypeHandle<BrushDefinition> m_BrushDefinitionType;

		[ReadOnly]
		public ComponentLookup<PrefabRef> m_PrefabRefData;

		[ReadOnly]
		public ComponentLookup<BrushData> m_PrefabBrushData;

		[ReadOnly]
		public ComponentLookup<TerraformingData> m_PrefabTerraformingData;

		public ParallelWriter m_CommandBuffer;

		public void Execute(in ArchetypeChunk chunk, int unfilteredChunkIndex, bool useEnabledMask, in v128 chunkEnabledMask)
		{
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0014: Unknown result type (might be due to invalid IL or missing references)
			//IL_0019: Unknown result type (might be due to invalid IL or missing references)
			//IL_003f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0044: Unknown result type (might be due to invalid IL or missing references)
			//IL_004a: Unknown result type (might be due to invalid IL or missing references)
			//IL_004f: Unknown result type (might be due to invalid IL or missing references)
			//IL_009c: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b3: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b8: Unknown result type (might be due to invalid IL or missing references)
			//IL_0063: Unknown result type (might be due to invalid IL or missing references)
			//IL_0080: Unknown result type (might be due to invalid IL or missing references)
			//IL_008a: Unknown result type (might be due to invalid IL or missing references)
			//IL_008f: Unknown result type (might be due to invalid IL or missing references)
			//IL_010e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0138: Unknown result type (might be due to invalid IL or missing references)
			//IL_0187: Unknown result type (might be due to invalid IL or missing references)
			//IL_018c: Unknown result type (might be due to invalid IL or missing references)
			//IL_01e9: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ee: Unknown result type (might be due to invalid IL or missing references)
			//IL_01f7: Unknown result type (might be due to invalid IL or missing references)
			//IL_01fc: Unknown result type (might be due to invalid IL or missing references)
			//IL_020a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0216: Unknown result type (might be due to invalid IL or missing references)
			//IL_021b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0229: Unknown result type (might be due to invalid IL or missing references)
			//IL_022e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0233: Unknown result type (might be due to invalid IL or missing references)
			//IL_023c: Unknown result type (might be due to invalid IL or missing references)
			//IL_024c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0266: Unknown result type (might be due to invalid IL or missing references)
			NativeArray<CreationDefinition> nativeArray = ((ArchetypeChunk)(ref chunk)).GetNativeArray<CreationDefinition>(ref m_CreationDefinitionType);
			NativeArray<BrushDefinition> nativeArray2 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<BrushDefinition>(ref m_BrushDefinitionType);
			TerraformingData terraformingData = default(TerraformingData);
			for (int i = 0; i < nativeArray.Length; i++)
			{
				CreationDefinition creationDefinition = nativeArray[i];
				BrushDefinition brushDefinition = nativeArray2[i];
				PrefabRef prefabRef = new PrefabRef
				{
					m_Prefab = creationDefinition.m_Prefab
				};
				if (creationDefinition.m_Original != Entity.Null)
				{
					((ParallelWriter)(ref m_CommandBuffer)).AddComponent<Hidden>(unfilteredChunkIndex, creationDefinition.m_Original, default(Hidden));
					prefabRef.m_Prefab = m_PrefabRefData[creationDefinition.m_Original].m_Prefab;
				}
				BrushData brushData = m_PrefabBrushData[prefabRef.m_Prefab];
				Temp temp = new Temp
				{
					m_Original = creationDefinition.m_Original
				};
				temp.m_Flags |= TempFlags.Essential;
				if ((creationDefinition.m_Flags & CreationFlags.Delete) != 0)
				{
					temp.m_Flags |= TempFlags.Delete;
				}
				else if ((creationDefinition.m_Flags & CreationFlags.Select) != 0)
				{
					temp.m_Flags |= TempFlags.Select;
				}
				else
				{
					temp.m_Flags |= TempFlags.Create;
				}
				if (!m_PrefabTerraformingData.TryGetComponent(brushDefinition.m_Tool, ref terraformingData) || terraformingData.m_Target == TerraformingTarget.None)
				{
					temp.m_Flags |= TempFlags.Cancel;
				}
				float num = MathUtils.Length(brushDefinition.m_Line);
				float num2 = brushDefinition.m_Strength * brushDefinition.m_Strength;
				int num3 = 1 + Mathf.FloorToInt(num / (brushDefinition.m_Size * 0.25f));
				float num4 = brushDefinition.m_Time / (float)num3;
				Brush brush = new Brush
				{
					m_Tool = brushDefinition.m_Tool,
					m_Angle = brushDefinition.m_Angle,
					m_Size = brushDefinition.m_Size,
					m_Strength = (num2 * num2 * (1f - num4) + num2 * num4) * math.sign(brushDefinition.m_Strength),
					m_Opacity = 1f / (float)num3,
					m_Target = brushDefinition.m_Target,
					m_Start = brushDefinition.m_Start
				};
				for (int j = 1; j <= num3; j++)
				{
					brush.m_Position = MathUtils.Position(brushDefinition.m_Line, (float)j / (float)num3);
					Entity val = ((ParallelWriter)(ref m_CommandBuffer)).CreateEntity(unfilteredChunkIndex, brushData.m_Archetype);
					((ParallelWriter)(ref m_CommandBuffer)).SetComponent<PrefabRef>(unfilteredChunkIndex, val, prefabRef);
					((ParallelWriter)(ref m_CommandBuffer)).SetComponent<Brush>(unfilteredChunkIndex, val, brush);
					if ((creationDefinition.m_Flags & CreationFlags.Permanent) == 0)
					{
						((ParallelWriter)(ref m_CommandBuffer)).AddComponent<Temp>(unfilteredChunkIndex, val, temp);
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
		public ComponentTypeHandle<CreationDefinition> __Game_Tools_CreationDefinition_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<BrushDefinition> __Game_Tools_BrushDefinition_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentLookup<PrefabRef> __Game_Prefabs_PrefabRef_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<BrushData> __Game_Prefabs_BrushData_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<TerraformingData> __Game_Prefabs_TerraformingData_RO_ComponentLookup;

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
			__Game_Tools_CreationDefinition_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<CreationDefinition>(true);
			__Game_Tools_BrushDefinition_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<BrushDefinition>(true);
			__Game_Prefabs_PrefabRef_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<PrefabRef>(true);
			__Game_Prefabs_BrushData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<BrushData>(true);
			__Game_Prefabs_TerraformingData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<TerraformingData>(true);
		}
	}

	private ModificationBarrier1 m_ModificationBarrier;

	private EntityQuery m_DefinitionQuery;

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
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		base.OnCreate();
		m_ModificationBarrier = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<ModificationBarrier1>();
		EntityQueryDesc[] array = new EntityQueryDesc[1];
		EntityQueryDesc val = new EntityQueryDesc();
		val.All = (ComponentType[])(object)new ComponentType[2]
		{
			ComponentType.ReadOnly<CreationDefinition>(),
			ComponentType.ReadOnly<Updated>()
		};
		val.Any = (ComponentType[])(object)new ComponentType[1] { ComponentType.ReadOnly<BrushDefinition>() };
		array[0] = val;
		m_DefinitionQuery = ((ComponentSystemBase)this).GetEntityQuery((EntityQueryDesc[])(object)array);
		((ComponentSystemBase)this).RequireForUpdate(m_DefinitionQuery);
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
		//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00be: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d4: Unknown result type (might be due to invalid IL or missing references)
		GenerateBrushesJob generateBrushesJob = new GenerateBrushesJob
		{
			m_CreationDefinitionType = InternalCompilerInterface.GetComponentTypeHandle<CreationDefinition>(ref __TypeHandle.__Game_Tools_CreationDefinition_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_BrushDefinitionType = InternalCompilerInterface.GetComponentTypeHandle<BrushDefinition>(ref __TypeHandle.__Game_Tools_BrushDefinition_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_PrefabRefData = InternalCompilerInterface.GetComponentLookup<PrefabRef>(ref __TypeHandle.__Game_Prefabs_PrefabRef_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_PrefabBrushData = InternalCompilerInterface.GetComponentLookup<BrushData>(ref __TypeHandle.__Game_Prefabs_BrushData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_PrefabTerraformingData = InternalCompilerInterface.GetComponentLookup<TerraformingData>(ref __TypeHandle.__Game_Prefabs_TerraformingData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef)
		};
		EntityCommandBuffer val = m_ModificationBarrier.CreateCommandBuffer();
		generateBrushesJob.m_CommandBuffer = ((EntityCommandBuffer)(ref val)).AsParallelWriter();
		GenerateBrushesJob generateBrushesJob2 = generateBrushesJob;
		((SystemBase)this).Dependency = JobChunkExtensions.ScheduleParallel<GenerateBrushesJob>(generateBrushesJob2, m_DefinitionQuery, ((SystemBase)this).Dependency);
		((EntityCommandBufferSystem)m_ModificationBarrier).AddJobHandleForProducer(((SystemBase)this).Dependency);
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
	public GenerateBrushesSystem()
	{
	}
}

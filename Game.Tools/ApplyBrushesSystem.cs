using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Colossal.Entities;
using Colossal.Mathematics;
using Colossal.Serialization.Entities;
using Game.Areas;
using Game.Common;
using Game.Prefabs;
using Game.Rendering;
using Game.Simulation;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Entities.Internal;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Scripting;

namespace Game.Tools;

[CompilerGenerated]
public class ApplyBrushesSystem : GameSystemBase
{
	private interface ICellModifier<TCell> where TCell : struct, ISerializable
	{
		void Apply(ref TCell cell, float strength);
	}

	private struct NaturalResourcesModifier : ICellModifier<NaturalResourceCell>
	{
		private MapFeature m_MapFeature;

		public NaturalResourcesModifier(MapFeature mapFeature)
		{
			m_MapFeature = mapFeature;
		}

		public void Apply(ref NaturalResourceCell cell, float strength)
		{
			switch (m_MapFeature)
			{
			case MapFeature.Ore:
				Apply(ref cell.m_Ore, strength);
				break;
			case MapFeature.Oil:
				Apply(ref cell.m_Oil, strength);
				break;
			case MapFeature.FertileLand:
				Apply(ref cell.m_Fertility, strength);
				break;
			case MapFeature.Forest:
				break;
			}
		}

		private void Apply(ref NaturalResourceAmount cellData, float strength)
		{
			float amount = (float)(int)cellData.m_Base * 0.0001f;
			Apply(ref amount, strength);
			cellData.m_Base = (ushort)math.clamp(Mathf.RoundToInt(amount * 10000f), 0, 10000);
		}

		private void Apply(ref float amount, float strength)
		{
			amount += strength;
		}
	}

	[StructLayout(LayoutKind.Sequential, Size = 1)]
	private struct GroundWaterModifier : ICellModifier<GroundWater>
	{
		public void Apply(ref GroundWater cell, float strength)
		{
			float amount = (float)cell.m_Amount * 0.0001f;
			Apply(ref amount, strength);
			cell.m_Amount = (short)math.clamp(Mathf.RoundToInt(amount * 10000f), 0, 10000);
			cell.m_Max = cell.m_Amount;
		}

		private void Apply(ref float amount, float strength)
		{
			amount += strength;
		}
	}

	[BurstCompile]
	private struct ApplyCellMapBrushJob<TCell, TModifier> : IJobParallelFor where TCell : struct, ISerializable where TModifier : ICellModifier<TCell>
	{
		[ReadOnly]
		public int4 m_Coords;

		[ReadOnly]
		public Brush m_Brush;

		[ReadOnly]
		public Entity m_Prefab;

		[ReadOnly]
		public TerraformingType m_TerraformingType;

		[ReadOnly]
		public TModifier m_CellModifier;

		[ReadOnly]
		public float4 m_TextureSizeAdd;

		[ReadOnly]
		public float2 m_CellSize;

		[ReadOnly]
		public int2 m_TextureSize;

		[ReadOnly]
		public ComponentLookup<BrushData> m_BrushData;

		[ReadOnly]
		public BufferLookup<BrushCell> m_BrushCells;

		[NativeDisableParallelForRestriction]
		public NativeArray<TCell> m_Buffer;

		public void Execute(int index)
		{
			//IL_004c: Unknown result type (might be due to invalid IL or missing references)
			//IL_004d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0073: Unknown result type (might be due to invalid IL or missing references)
			//IL_0078: Unknown result type (might be due to invalid IL or missing references)
			//IL_0088: Unknown result type (might be due to invalid IL or missing references)
			//IL_008d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0092: Unknown result type (might be due to invalid IL or missing references)
			//IL_0096: Unknown result type (might be due to invalid IL or missing references)
			//IL_009b: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ab: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b5: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b9: Unknown result type (might be due to invalid IL or missing references)
			//IL_00be: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c6: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d9: Unknown result type (might be due to invalid IL or missing references)
			//IL_00de: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e3: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e7: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ed: Unknown result type (might be due to invalid IL or missing references)
			//IL_0110: Unknown result type (might be due to invalid IL or missing references)
			//IL_0115: Unknown result type (might be due to invalid IL or missing references)
			//IL_011a: Unknown result type (might be due to invalid IL or missing references)
			//IL_011f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0126: Unknown result type (might be due to invalid IL or missing references)
			//IL_0128: Unknown result type (might be due to invalid IL or missing references)
			//IL_012d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0131: Unknown result type (might be due to invalid IL or missing references)
			//IL_0136: Unknown result type (might be due to invalid IL or missing references)
			//IL_013a: Unknown result type (might be due to invalid IL or missing references)
			//IL_013f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0149: Unknown result type (might be due to invalid IL or missing references)
			//IL_014e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0152: Unknown result type (might be due to invalid IL or missing references)
			//IL_0157: Unknown result type (might be due to invalid IL or missing references)
			//IL_01cf: Unknown result type (might be due to invalid IL or missing references)
			//IL_01d0: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ed: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ee: Unknown result type (might be due to invalid IL or missing references)
			//IL_01f3: Unknown result type (might be due to invalid IL or missing references)
			//IL_01f4: Unknown result type (might be due to invalid IL or missing references)
			//IL_0202: Unknown result type (might be due to invalid IL or missing references)
			//IL_0207: Unknown result type (might be due to invalid IL or missing references)
			//IL_020f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0214: Unknown result type (might be due to invalid IL or missing references)
			//IL_021c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0221: Unknown result type (might be due to invalid IL or missing references)
			//IL_0229: Unknown result type (might be due to invalid IL or missing references)
			//IL_022e: Unknown result type (might be due to invalid IL or missing references)
			//IL_023d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0242: Unknown result type (might be due to invalid IL or missing references)
			//IL_024a: Unknown result type (might be due to invalid IL or missing references)
			//IL_024f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0257: Unknown result type (might be due to invalid IL or missing references)
			//IL_025c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0264: Unknown result type (might be due to invalid IL or missing references)
			//IL_0269: Unknown result type (might be due to invalid IL or missing references)
			//IL_0274: Unknown result type (might be due to invalid IL or missing references)
			//IL_027b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0282: Unknown result type (might be due to invalid IL or missing references)
			//IL_0289: Unknown result type (might be due to invalid IL or missing references)
			//IL_0290: Unknown result type (might be due to invalid IL or missing references)
			//IL_0295: Unknown result type (might be due to invalid IL or missing references)
			//IL_0297: Unknown result type (might be due to invalid IL or missing references)
			//IL_029c: Unknown result type (might be due to invalid IL or missing references)
			//IL_029e: Unknown result type (might be due to invalid IL or missing references)
			//IL_02a3: Unknown result type (might be due to invalid IL or missing references)
			//IL_02a8: Unknown result type (might be due to invalid IL or missing references)
			//IL_02ad: Unknown result type (might be due to invalid IL or missing references)
			//IL_02af: Unknown result type (might be due to invalid IL or missing references)
			//IL_02b2: Unknown result type (might be due to invalid IL or missing references)
			//IL_02be: Unknown result type (might be due to invalid IL or missing references)
			//IL_02c4: Unknown result type (might be due to invalid IL or missing references)
			//IL_02c9: Unknown result type (might be due to invalid IL or missing references)
			//IL_02ce: Unknown result type (might be due to invalid IL or missing references)
			//IL_02d7: Unknown result type (might be due to invalid IL or missing references)
			//IL_03f2: Unknown result type (might be due to invalid IL or missing references)
			//IL_02e5: Unknown result type (might be due to invalid IL or missing references)
			//IL_02e9: Unknown result type (might be due to invalid IL or missing references)
			//IL_02f1: Unknown result type (might be due to invalid IL or missing references)
			//IL_02f9: Unknown result type (might be due to invalid IL or missing references)
			//IL_02fe: Unknown result type (might be due to invalid IL or missing references)
			//IL_0300: Unknown result type (might be due to invalid IL or missing references)
			//IL_0306: Unknown result type (might be due to invalid IL or missing references)
			//IL_030e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0316: Unknown result type (might be due to invalid IL or missing references)
			//IL_031b: Unknown result type (might be due to invalid IL or missing references)
			//IL_031d: Unknown result type (might be due to invalid IL or missing references)
			//IL_03de: Unknown result type (might be due to invalid IL or missing references)
			//IL_032f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0358: Unknown result type (might be due to invalid IL or missing references)
			//IL_035c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0364: Unknown result type (might be due to invalid IL or missing references)
			//IL_036c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0371: Unknown result type (might be due to invalid IL or missing references)
			//IL_0373: Unknown result type (might be due to invalid IL or missing references)
			//IL_0379: Unknown result type (might be due to invalid IL or missing references)
			//IL_0381: Unknown result type (might be due to invalid IL or missing references)
			//IL_0389: Unknown result type (might be due to invalid IL or missing references)
			//IL_038e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0392: Unknown result type (might be due to invalid IL or missing references)
			//IL_0394: Unknown result type (might be due to invalid IL or missing references)
			//IL_0396: Unknown result type (might be due to invalid IL or missing references)
			//IL_039b: Unknown result type (might be due to invalid IL or missing references)
			//IL_039d: Unknown result type (might be due to invalid IL or missing references)
			//IL_039f: Unknown result type (might be due to invalid IL or missing references)
			//IL_03a4: Unknown result type (might be due to invalid IL or missing references)
			//IL_03a6: Unknown result type (might be due to invalid IL or missing references)
			//IL_03a8: Unknown result type (might be due to invalid IL or missing references)
			//IL_03ad: Unknown result type (might be due to invalid IL or missing references)
			//IL_03af: Unknown result type (might be due to invalid IL or missing references)
			//IL_03b1: Unknown result type (might be due to invalid IL or missing references)
			//IL_03bb: Unknown result type (might be due to invalid IL or missing references)
			//IL_03bc: Unknown result type (might be due to invalid IL or missing references)
			int num = m_Coords.y + index;
			Bounds2 val = default(Bounds2);
			val.min.y = ((float)num - m_TextureSizeAdd.y) * m_CellSize.y - m_Brush.m_Position.z;
			val.max.y = val.min.y + m_CellSize.y;
			quaternion val2 = quaternion.RotateY(m_Brush.m_Angle);
			float3 val3 = math.mul(val2, new float3(1f, 0f, 0f));
			float2 xz = ((float3)(ref val3)).xz;
			val3 = math.mul(val2, new float3(0f, 0f, 1f));
			float2 xz2 = ((float3)(ref val3)).xz;
			BrushData brushData = m_BrushData[m_Prefab];
			DynamicBuffer<BrushCell> val4 = m_BrushCells[m_Prefab];
			if (math.any(brushData.m_Resolution == 0) || val4.Length == 0)
			{
				return;
			}
			float2 val5 = m_Brush.m_Size / float2.op_Implicit(brushData.m_Resolution);
			float2 val6 = 1f / val5;
			float4 xyxy = ((float2)(ref val6)).xyxy;
			val6 = float2.op_Implicit(brushData.m_Resolution) * 0.5f;
			float4 xyxy2 = ((float2)(ref val6)).xyxy;
			float num2 = m_Brush.m_Strength / (m_CellSize.x * m_CellSize.y);
			float4 val7 = default(float4);
			float4 val8 = default(float4);
			float4 val9 = default(float4);
			Quad2 val15 = default(Quad2);
			float num5 = default(float);
			for (int i = m_Coords.x; i <= m_Coords.z; i++)
			{
				val.min.x = ((float)i - m_TextureSizeAdd.x) * m_CellSize.x - m_Brush.m_Position.x;
				val.max.x = val.min.x + m_CellSize.x;
				((float4)(ref val7))._002Ector(val.min, val.max);
				((float4)(ref val8))._002Ector(math.dot(((float4)(ref val7)).xy, xz), math.dot(((float4)(ref val7)).xw, xz), math.dot(((float4)(ref val7)).zy, xz), math.dot(((float4)(ref val7)).zw, xz));
				((float4)(ref val9))._002Ector(math.dot(((float4)(ref val7)).xy, xz2), math.dot(((float4)(ref val7)).xw, xz2), math.dot(((float4)(ref val7)).zy, xz2), math.dot(((float4)(ref val7)).zw, xz2));
				int4 val10 = (int4)math.floor(new float4(math.cmin(val8), math.cmin(val9), math.cmax(val8), math.cmax(val9)) * xyxy + xyxy2);
				val10 = math.clamp(val10, int4.op_Implicit(0), ((int2)(ref brushData.m_Resolution)).xyxy - 1);
				float num3 = 0f;
				for (int j = val10.y; j <= val10.w; j++)
				{
					float2 val11 = xz2 * (((float)j - xyxy2.y) * val5.y);
					float2 val12 = xz2 * (((float)(j + 1) - xyxy2.y) * val5.y);
					for (int k = val10.x; k <= val10.z; k++)
					{
						int num4 = k + brushData.m_Resolution.x * j;
						BrushCell brushCell = val4[num4];
						if (brushCell.m_Opacity >= 0.0001f)
						{
							float2 val13 = xz * (((float)k - xyxy2.x) * val5.x);
							float2 val14 = xz * (((float)(k + 1) - xyxy2.x) * val5.x);
							((Quad2)(ref val15))._002Ector(val11 + val13, val11 + val14, val12 + val14, val12 + val13);
							if (MathUtils.Intersect(val, val15, ref num5))
							{
								num3 += brushCell.m_Opacity * num5;
							}
						}
					}
				}
				num3 *= num2;
				if (math.abs(num3) >= 0.0001f)
				{
					int num6 = i + m_TextureSize.x * num;
					TCell cell = m_Buffer[num6];
					m_CellModifier.Apply(ref cell, num3);
					m_Buffer[num6] = cell;
				}
			}
		}
	}

	private struct TypeHandle
	{
		[ReadOnly]
		public EntityTypeHandle __Unity_Entities_Entity_TypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<Brush> __Game_Tools_Brush_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<PrefabRef> __Game_Prefabs_PrefabRef_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentLookup<BrushData> __Game_Prefabs_BrushData_RO_ComponentLookup;

		[ReadOnly]
		public BufferLookup<BrushCell> __Game_Prefabs_BrushCell_RO_BufferLookup;

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
			__Unity_Entities_Entity_TypeHandle = ((SystemState)(ref state)).GetEntityTypeHandle();
			__Game_Tools_Brush_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<Brush>(true);
			__Game_Prefabs_PrefabRef_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<PrefabRef>(true);
			__Game_Prefabs_BrushData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<BrushData>(true);
			__Game_Prefabs_BrushCell_RO_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<BrushCell>(true);
		}
	}

	private ToolOutputBarrier m_ToolOutputBarrier;

	private NaturalResourceSystem m_NaturalResourceSystem;

	private GroundWaterSystem m_GroundWaterSystem;

	private TerrainSystem m_TerrainSystem;

	private TerrainMaterialSystem m_TerrainMaterialSystem;

	private PrefabSystem m_PrefabSystem;

	private EntityQuery m_TempQuery;

	private ComponentTypeSet m_AppliedDeletedTypes;

	private TypeHandle __TypeHandle;

	[Preserve]
	protected override void OnCreate()
	{
		//IL_0076: Unknown result type (might be due to invalid IL or missing references)
		//IL_007b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0082: Unknown result type (might be due to invalid IL or missing references)
		//IL_0087: Unknown result type (might be due to invalid IL or missing references)
		//IL_008c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0091: Unknown result type (might be due to invalid IL or missing references)
		//IL_0097: Unknown result type (might be due to invalid IL or missing references)
		//IL_009c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ad: Unknown result type (might be due to invalid IL or missing references)
		base.OnCreate();
		m_ToolOutputBarrier = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<ToolOutputBarrier>();
		m_NaturalResourceSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<NaturalResourceSystem>();
		m_GroundWaterSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<GroundWaterSystem>();
		m_TerrainSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<TerrainSystem>();
		m_TerrainMaterialSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<TerrainMaterialSystem>();
		m_PrefabSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<PrefabSystem>();
		m_TempQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[2]
		{
			ComponentType.ReadOnly<Temp>(),
			ComponentType.ReadOnly<Brush>()
		});
		m_AppliedDeletedTypes = new ComponentTypeSet(ComponentType.ReadWrite<Applied>(), ComponentType.ReadWrite<Deleted>());
		((ComponentSystemBase)this).RequireForUpdate(m_TempQuery);
	}

	[Preserve]
	protected override void OnUpdate()
	{
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0262: Unknown result type (might be due to invalid IL or missing references)
		//IL_007e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0083: Unknown result type (might be due to invalid IL or missing references)
		//IL_0087: Unknown result type (might be due to invalid IL or missing references)
		//IL_0088: Unknown result type (might be due to invalid IL or missing references)
		//IL_008d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0093: Unknown result type (might be due to invalid IL or missing references)
		//IL_0098: Unknown result type (might be due to invalid IL or missing references)
		//IL_009e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_0223: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ff: Unknown result type (might be due to invalid IL or missing references)
		//IL_0114: Unknown result type (might be due to invalid IL or missing references)
		//IL_0127: Unknown result type (might be due to invalid IL or missing references)
		//IL_013d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0142: Unknown result type (might be due to invalid IL or missing references)
		//IL_0147: Unknown result type (might be due to invalid IL or missing references)
		//IL_014e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0161: Unknown result type (might be due to invalid IL or missing references)
		//IL_0177: Unknown result type (might be due to invalid IL or missing references)
		//IL_017c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0181: Unknown result type (might be due to invalid IL or missing references)
		//IL_0188: Unknown result type (might be due to invalid IL or missing references)
		//IL_019b: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_01bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f6: Unknown result type (might be due to invalid IL or missing references)
		//IL_0217: Unknown result type (might be due to invalid IL or missing references)
		EntityTypeHandle entityTypeHandle = InternalCompilerInterface.GetEntityTypeHandle(ref __TypeHandle.__Unity_Entities_Entity_TypeHandle, ref ((SystemBase)this).CheckedStateRef);
		ComponentTypeHandle<Brush> componentTypeHandle = InternalCompilerInterface.GetComponentTypeHandle<Brush>(ref __TypeHandle.__Game_Tools_Brush_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef);
		ComponentTypeHandle<PrefabRef> componentTypeHandle2 = InternalCompilerInterface.GetComponentTypeHandle<PrefabRef>(ref __TypeHandle.__Game_Prefabs_PrefabRef_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef);
		EntityCommandBuffer val = m_ToolOutputBarrier.CreateCommandBuffer();
		JobHandle val2 = default(JobHandle);
		NativeArray<ArchetypeChunk> val3 = ((EntityQuery)(ref m_TempQuery)).ToArchetypeChunkArray(AllocatorHandle.op_Implicit((Allocator)3));
		try
		{
			((SystemBase)this).CompleteDependency();
			TerraformingData terraformingData = default(TerraformingData);
			for (int i = 0; i < val3.Length; i++)
			{
				ArchetypeChunk val4 = val3[i];
				NativeArray<Entity> nativeArray = ((ArchetypeChunk)(ref val4)).GetNativeArray(entityTypeHandle);
				NativeArray<Brush> nativeArray2 = ((ArchetypeChunk)(ref val4)).GetNativeArray<Brush>(ref componentTypeHandle);
				NativeArray<PrefabRef> nativeArray3 = ((ArchetypeChunk)(ref val4)).GetNativeArray<PrefabRef>(ref componentTypeHandle2);
				for (int j = 0; j < nativeArray.Length; j++)
				{
					Entity val5 = nativeArray[j];
					Brush brush = nativeArray2[j];
					PrefabRef prefabRef = nativeArray3[j];
					if (EntitiesExtensions.TryGetComponent<TerraformingData>(((ComponentSystemBase)this).EntityManager, brush.m_Tool, ref terraformingData))
					{
						switch (terraformingData.m_Target)
						{
						case TerraformingTarget.Ore:
							val2 = JobHandle.CombineDependencies(val2, ApplyCellMapBrush<NaturalResourceCell, NaturalResourcesModifier>((CellMapSystem<NaturalResourceCell>)m_NaturalResourceSystem, new NaturalResourcesModifier(MapFeature.Ore), brush, prefabRef.m_Prefab, terraformingData.m_Type, default(ApplyCellMapBrushJob<NaturalResourceCell, NaturalResourcesModifier>)));
							break;
						case TerraformingTarget.Oil:
							val2 = JobHandle.CombineDependencies(val2, ApplyCellMapBrush<NaturalResourceCell, NaturalResourcesModifier>((CellMapSystem<NaturalResourceCell>)m_NaturalResourceSystem, new NaturalResourcesModifier(MapFeature.Oil), brush, prefabRef.m_Prefab, terraformingData.m_Type, default(ApplyCellMapBrushJob<NaturalResourceCell, NaturalResourcesModifier>)));
							break;
						case TerraformingTarget.FertileLand:
							val2 = JobHandle.CombineDependencies(val2, ApplyCellMapBrush<NaturalResourceCell, NaturalResourcesModifier>((CellMapSystem<NaturalResourceCell>)m_NaturalResourceSystem, new NaturalResourcesModifier(MapFeature.FertileLand), brush, prefabRef.m_Prefab, terraformingData.m_Type, default(ApplyCellMapBrushJob<NaturalResourceCell, NaturalResourcesModifier>)));
							break;
						case TerraformingTarget.GroundWater:
							val2 = JobHandle.CombineDependencies(val2, ApplyCellMapBrush<GroundWater, GroundWaterModifier>((CellMapSystem<GroundWater>)m_GroundWaterSystem, default(GroundWaterModifier), brush, prefabRef.m_Prefab, terraformingData.m_Type, default(ApplyCellMapBrushJob<GroundWater, GroundWaterModifier>)));
							break;
						case TerraformingTarget.Height:
							ApplyHeight(brush, prefabRef.m_Prefab, terraformingData.m_Type);
							break;
						case TerraformingTarget.Material:
							ApplyMaterial(brush, prefabRef.m_Prefab);
							break;
						}
					}
					((EntityCommandBuffer)(ref val)).AddComponent(val5, ref m_AppliedDeletedTypes);
				}
			}
		}
		finally
		{
			val3.Dispose();
			((SystemBase)this).Dependency = val2;
		}
	}

	private void ApplyMaterial(Brush brush, Entity prefab)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		m_TerrainMaterialSystem.GetOrAddMaterialIndex(brush.m_Tool);
	}

	private void ApplyHeight(Brush brush, Entity prefab, TerraformingType terraformingType)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		Bounds2 bounds = ToolUtils.GetBounds(brush);
		BrushPrefab prefab2 = m_PrefabSystem.GetPrefab<BrushPrefab>(prefab);
		if ((terraformingType != TerraformingType.Level && terraformingType != TerraformingType.Slope) || !(brush.m_Strength < 0f))
		{
			if (terraformingType == TerraformingType.Soften && brush.m_Strength < 0f)
			{
				brush.m_Strength = math.abs(brush.m_Strength) * 2f;
			}
			m_TerrainSystem.ApplyBrush(terraformingType, bounds, brush, (Texture)(object)prefab2.m_Texture);
		}
	}

	private JobHandle ApplyCellMapBrush<TCell, TModifier>(CellMapSystem<TCell> cellMapSystem, TModifier modifier, Brush brush, Entity prefab, TerraformingType terraformingType, ApplyCellMapBrushJob<TCell, TModifier> applyCellMapBrushJob) where TCell : struct, ISerializable where TModifier : ICellModifier<TCell>
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		//IL_0069: Unknown result type (might be due to invalid IL or missing references)
		//IL_006e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0073: Unknown result type (might be due to invalid IL or missing references)
		//IL_0075: Unknown result type (might be due to invalid IL or missing references)
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
		//IL_0084: Unknown result type (might be due to invalid IL or missing references)
		//IL_008a: Unknown result type (might be due to invalid IL or missing references)
		//IL_008f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0094: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00da: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ff: Unknown result type (might be due to invalid IL or missing references)
		//IL_0104: Unknown result type (might be due to invalid IL or missing references)
		//IL_011c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0121: Unknown result type (might be due to invalid IL or missing references)
		//IL_0129: Unknown result type (might be due to invalid IL or missing references)
		//IL_012e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0139: Unknown result type (might be due to invalid IL or missing references)
		//IL_0140: Unknown result type (might be due to invalid IL or missing references)
		//IL_014b: Unknown result type (might be due to invalid IL or missing references)
		//IL_014c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0151: Unknown result type (might be due to invalid IL or missing references)
		//IL_0154: Unknown result type (might be due to invalid IL or missing references)
		//IL_015b: Unknown result type (might be due to invalid IL or missing references)
		Bounds2 bounds = ToolUtils.GetBounds(brush);
		JobHandle dependencies;
		CellMapData<TCell> data = cellMapSystem.GetData(readOnly: false, out dependencies);
		float2 val = 1f / data.m_CellSize;
		float4 xyxy = ((float2)(ref val)).xyxy;
		val = float2.op_Implicit(data.m_TextureSize) * 0.5f;
		float4 xyxy2 = ((float2)(ref val)).xyxy;
		int4 val2 = (int4)math.floor(new float4(bounds.min, bounds.max) * xyxy + xyxy2);
		val2 = math.clamp(val2, int4.op_Implicit(0), ((int2)(ref data.m_TextureSize)).xyxy - 1);
		applyCellMapBrushJob = new ApplyCellMapBrushJob<TCell, TModifier>
		{
			m_Coords = val2,
			m_Brush = brush,
			m_Prefab = prefab,
			m_TerraformingType = terraformingType,
			m_CellModifier = modifier,
			m_TextureSizeAdd = xyxy2,
			m_CellSize = data.m_CellSize,
			m_TextureSize = data.m_TextureSize,
			m_BrushData = InternalCompilerInterface.GetComponentLookup<BrushData>(ref __TypeHandle.__Game_Prefabs_BrushData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_BrushCells = InternalCompilerInterface.GetBufferLookup<BrushCell>(ref __TypeHandle.__Game_Prefabs_BrushCell_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
			m_Buffer = data.m_Buffer
		};
		JobHandle val3 = IJobParallelForExtensions.Schedule<ApplyCellMapBrushJob<TCell, TModifier>>(applyCellMapBrushJob, val2.w - val2.y + 1, 1, dependencies);
		cellMapSystem.AddWriter(val3);
		return val3;
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
	public ApplyBrushesSystem()
	{
	}
}

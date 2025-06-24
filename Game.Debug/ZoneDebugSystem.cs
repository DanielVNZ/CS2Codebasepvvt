using System.Runtime.CompilerServices;
using Colossal;
using Game.Common;
using Game.Prefabs;
using Game.Tools;
using Game.Zones;
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
public class ZoneDebugSystem : BaseDebugSystem
{
	[BurstCompile]
	private struct BlockGizmoJob : IJobChunk
	{
		[ReadOnly]
		public bool m_PivotOption;

		[ReadOnly]
		public bool m_GridOption;

		[ReadOnly]
		public bool m_LotOption;

		[ReadOnly]
		public ZonePrefabs m_ZonePrefabs;

		[ReadOnly]
		public ComponentTypeHandle<Block> m_BlockType;

		[ReadOnly]
		public ComponentTypeHandle<Temp> m_TempType;

		[ReadOnly]
		public BufferTypeHandle<VacantLot> m_VacantLotType;

		[ReadOnly]
		public ComponentTypeHandle<Error> m_ErrorType;

		[ReadOnly]
		public ComponentLookup<ZoneData> m_PrefabZoneData;

		public GizmoBatcher m_GizmoBatcher;

		public void Execute(in ArchetypeChunk chunk, int unfilteredChunkIndex, bool useEnabledMask, in v128 chunkEnabledMask)
		{
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0014: Unknown result type (might be due to invalid IL or missing references)
			//IL_0019: Unknown result type (might be due to invalid IL or missing references)
			//IL_0028: Unknown result type (might be due to invalid IL or missing references)
			//IL_002d: Unknown result type (might be due to invalid IL or missing references)
			//IL_002e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0033: Unknown result type (might be due to invalid IL or missing references)
			//IL_0052: Unknown result type (might be due to invalid IL or missing references)
			//IL_0057: Unknown result type (might be due to invalid IL or missing references)
			//IL_0058: Unknown result type (might be due to invalid IL or missing references)
			//IL_005d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0044: Unknown result type (might be due to invalid IL or missing references)
			//IL_0049: Unknown result type (might be due to invalid IL or missing references)
			//IL_004a: Unknown result type (might be due to invalid IL or missing references)
			//IL_004f: Unknown result type (might be due to invalid IL or missing references)
			//IL_005e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0068: Unknown result type (might be due to invalid IL or missing references)
			//IL_006d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0092: Unknown result type (might be due to invalid IL or missing references)
			//IL_009c: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c3: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d6: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e7: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f8: Unknown result type (might be due to invalid IL or missing references)
			//IL_00fd: Unknown result type (might be due to invalid IL or missing references)
			//IL_0106: Unknown result type (might be due to invalid IL or missing references)
			//IL_0112: Unknown result type (might be due to invalid IL or missing references)
			//IL_0117: Unknown result type (might be due to invalid IL or missing references)
			//IL_011c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0125: Unknown result type (might be due to invalid IL or missing references)
			//IL_0131: Unknown result type (might be due to invalid IL or missing references)
			//IL_0136: Unknown result type (might be due to invalid IL or missing references)
			//IL_013b: Unknown result type (might be due to invalid IL or missing references)
			//IL_013d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0146: Unknown result type (might be due to invalid IL or missing references)
			//IL_0152: Unknown result type (might be due to invalid IL or missing references)
			//IL_0157: Unknown result type (might be due to invalid IL or missing references)
			//IL_0159: Unknown result type (might be due to invalid IL or missing references)
			//IL_0162: Unknown result type (might be due to invalid IL or missing references)
			//IL_016e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0173: Unknown result type (might be due to invalid IL or missing references)
			//IL_01d0: Unknown result type (might be due to invalid IL or missing references)
			//IL_0279: Unknown result type (might be due to invalid IL or missing references)
			//IL_027e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0284: Unknown result type (might be due to invalid IL or missing references)
			//IL_0296: Unknown result type (might be due to invalid IL or missing references)
			//IL_02a9: Unknown result type (might be due to invalid IL or missing references)
			//IL_02bb: Unknown result type (might be due to invalid IL or missing references)
			//IL_02cd: Unknown result type (might be due to invalid IL or missing references)
			//IL_02d3: Unknown result type (might be due to invalid IL or missing references)
			//IL_02d5: Unknown result type (might be due to invalid IL or missing references)
			//IL_02d7: Unknown result type (might be due to invalid IL or missing references)
			//IL_02dc: Unknown result type (might be due to invalid IL or missing references)
			//IL_02e1: Unknown result type (might be due to invalid IL or missing references)
			//IL_017a: Unknown result type (might be due to invalid IL or missing references)
			//IL_017c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0187: Unknown result type (might be due to invalid IL or missing references)
			//IL_018c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0191: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ab: Unknown result type (might be due to invalid IL or missing references)
			//IL_019b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0237: Unknown result type (might be due to invalid IL or missing references)
			//IL_02fd: Unknown result type (might be due to invalid IL or missing references)
			//IL_0309: Unknown result type (might be due to invalid IL or missing references)
			//IL_030e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0315: Unknown result type (might be due to invalid IL or missing references)
			//IL_031a: Unknown result type (might be due to invalid IL or missing references)
			//IL_031f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0329: Unknown result type (might be due to invalid IL or missing references)
			//IL_032e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0332: Unknown result type (might be due to invalid IL or missing references)
			//IL_0337: Unknown result type (might be due to invalid IL or missing references)
			//IL_0339: Unknown result type (might be due to invalid IL or missing references)
			//IL_0340: Unknown result type (might be due to invalid IL or missing references)
			//IL_0345: Unknown result type (might be due to invalid IL or missing references)
			//IL_034a: Unknown result type (might be due to invalid IL or missing references)
			//IL_034c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0353: Unknown result type (might be due to invalid IL or missing references)
			//IL_0358: Unknown result type (might be due to invalid IL or missing references)
			//IL_035d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0366: Unknown result type (might be due to invalid IL or missing references)
			//IL_0372: Unknown result type (might be due to invalid IL or missing references)
			//IL_0377: Unknown result type (might be due to invalid IL or missing references)
			//IL_037c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0386: Unknown result type (might be due to invalid IL or missing references)
			//IL_038b: Unknown result type (might be due to invalid IL or missing references)
			//IL_038d: Unknown result type (might be due to invalid IL or missing references)
			//IL_038f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0394: Unknown result type (might be due to invalid IL or missing references)
			//IL_0396: Unknown result type (might be due to invalid IL or missing references)
			//IL_039b: Unknown result type (might be due to invalid IL or missing references)
			//IL_03b0: Unknown result type (might be due to invalid IL or missing references)
			//IL_03bf: Unknown result type (might be due to invalid IL or missing references)
			//IL_03c4: Unknown result type (might be due to invalid IL or missing references)
			//IL_03d8: Unknown result type (might be due to invalid IL or missing references)
			//IL_03f0: Unknown result type (might be due to invalid IL or missing references)
			//IL_0404: Unknown result type (might be due to invalid IL or missing references)
			//IL_0409: Unknown result type (might be due to invalid IL or missing references)
			//IL_0412: Unknown result type (might be due to invalid IL or missing references)
			//IL_0419: Unknown result type (might be due to invalid IL or missing references)
			//IL_041e: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ac: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b4: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b6: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b8: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ba: Unknown result type (might be due to invalid IL or missing references)
			//IL_01bf: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a7: Unknown result type (might be due to invalid IL or missing references)
			//IL_01e1: Unknown result type (might be due to invalid IL or missing references)
			//IL_01e3: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ee: Unknown result type (might be due to invalid IL or missing references)
			//IL_01f3: Unknown result type (might be due to invalid IL or missing references)
			//IL_01f8: Unknown result type (might be due to invalid IL or missing references)
			//IL_0440: Unknown result type (might be due to invalid IL or missing references)
			//IL_0442: Unknown result type (might be due to invalid IL or missing references)
			//IL_0444: Unknown result type (might be due to invalid IL or missing references)
			//IL_0451: Unknown result type (might be due to invalid IL or missing references)
			//IL_0456: Unknown result type (might be due to invalid IL or missing references)
			//IL_045b: Unknown result type (might be due to invalid IL or missing references)
			//IL_045d: Unknown result type (might be due to invalid IL or missing references)
			//IL_046a: Unknown result type (might be due to invalid IL or missing references)
			//IL_046f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0474: Unknown result type (might be due to invalid IL or missing references)
			//IL_0476: Unknown result type (might be due to invalid IL or missing references)
			//IL_0478: Unknown result type (might be due to invalid IL or missing references)
			//IL_047a: Unknown result type (might be due to invalid IL or missing references)
			//IL_048d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0492: Unknown result type (might be due to invalid IL or missing references)
			//IL_0497: Unknown result type (might be due to invalid IL or missing references)
			//IL_0499: Unknown result type (might be due to invalid IL or missing references)
			//IL_04ac: Unknown result type (might be due to invalid IL or missing references)
			//IL_04b1: Unknown result type (might be due to invalid IL or missing references)
			//IL_04b6: Unknown result type (might be due to invalid IL or missing references)
			//IL_04be: Unknown result type (might be due to invalid IL or missing references)
			//IL_04c0: Unknown result type (might be due to invalid IL or missing references)
			//IL_04c2: Unknown result type (might be due to invalid IL or missing references)
			//IL_0212: Unknown result type (might be due to invalid IL or missing references)
			//IL_0202: Unknown result type (might be due to invalid IL or missing references)
			//IL_04d7: Unknown result type (might be due to invalid IL or missing references)
			//IL_04d9: Unknown result type (might be due to invalid IL or missing references)
			//IL_04db: Unknown result type (might be due to invalid IL or missing references)
			//IL_04e8: Unknown result type (might be due to invalid IL or missing references)
			//IL_04ed: Unknown result type (might be due to invalid IL or missing references)
			//IL_04f2: Unknown result type (might be due to invalid IL or missing references)
			//IL_04f4: Unknown result type (might be due to invalid IL or missing references)
			//IL_0501: Unknown result type (might be due to invalid IL or missing references)
			//IL_0506: Unknown result type (might be due to invalid IL or missing references)
			//IL_050b: Unknown result type (might be due to invalid IL or missing references)
			//IL_050d: Unknown result type (might be due to invalid IL or missing references)
			//IL_050f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0511: Unknown result type (might be due to invalid IL or missing references)
			//IL_0524: Unknown result type (might be due to invalid IL or missing references)
			//IL_0529: Unknown result type (might be due to invalid IL or missing references)
			//IL_052e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0530: Unknown result type (might be due to invalid IL or missing references)
			//IL_0543: Unknown result type (might be due to invalid IL or missing references)
			//IL_0548: Unknown result type (might be due to invalid IL or missing references)
			//IL_054d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0555: Unknown result type (might be due to invalid IL or missing references)
			//IL_0557: Unknown result type (might be due to invalid IL or missing references)
			//IL_0559: Unknown result type (might be due to invalid IL or missing references)
			//IL_0213: Unknown result type (might be due to invalid IL or missing references)
			//IL_021b: Unknown result type (might be due to invalid IL or missing references)
			//IL_021d: Unknown result type (might be due to invalid IL or missing references)
			//IL_021f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0221: Unknown result type (might be due to invalid IL or missing references)
			//IL_0226: Unknown result type (might be due to invalid IL or missing references)
			//IL_020e: Unknown result type (might be due to invalid IL or missing references)
			//IL_056b: Unknown result type (might be due to invalid IL or missing references)
			//IL_056d: Unknown result type (might be due to invalid IL or missing references)
			//IL_056f: Unknown result type (might be due to invalid IL or missing references)
			//IL_057c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0581: Unknown result type (might be due to invalid IL or missing references)
			//IL_0586: Unknown result type (might be due to invalid IL or missing references)
			//IL_0588: Unknown result type (might be due to invalid IL or missing references)
			//IL_058a: Unknown result type (might be due to invalid IL or missing references)
			//IL_058c: Unknown result type (might be due to invalid IL or missing references)
			//IL_059f: Unknown result type (might be due to invalid IL or missing references)
			//IL_05a4: Unknown result type (might be due to invalid IL or missing references)
			//IL_05a9: Unknown result type (might be due to invalid IL or missing references)
			//IL_05b1: Unknown result type (might be due to invalid IL or missing references)
			//IL_05b3: Unknown result type (might be due to invalid IL or missing references)
			//IL_05b5: Unknown result type (might be due to invalid IL or missing references)
			NativeArray<Block> nativeArray = ((ArchetypeChunk)(ref chunk)).GetNativeArray<Block>(ref m_BlockType);
			BufferAccessor<VacantLot> bufferAccessor = ((ArchetypeChunk)(ref chunk)).GetBufferAccessor<VacantLot>(ref m_VacantLotType);
			Color val;
			Color val2;
			if (((ArchetypeChunk)(ref chunk)).Has<Error>(ref m_ErrorType))
			{
				val = Color.red;
				val2 = Color.red;
			}
			else if (((ArchetypeChunk)(ref chunk)).Has<Temp>(ref m_TempType))
			{
				val = Color.blue;
				val2 = Color.blue;
			}
			else
			{
				val = Color.cyan;
				val2 = Color.white;
			}
			Color val3 = Color.gray * 0.5f;
			float3 val4 = default(float3);
			float3 val5 = default(float3);
			for (int i = 0; i < nativeArray.Length; i++)
			{
				Block block = nativeArray[i];
				if (m_PivotOption)
				{
					((GizmoBatcher)(ref m_GizmoBatcher)).DrawWireNode(block.m_Position, 4f, val);
				}
				if (m_GridOption)
				{
					((float3)(ref val4))._002Ector(0f - block.m_Direction.y, 0f, block.m_Direction.x);
					((float3)(ref val5))._002Ector(block.m_Direction.x, 0f, block.m_Direction.y);
					float3 val6 = block.m_Position - val4 * (4f * (float)block.m_Size.x) - val5 * (4f * (float)block.m_Size.y);
					float3 val7 = val4 * (8f * (float)block.m_Size.x);
					float3 val8 = val5 * (8f * (float)block.m_Size.y);
					for (int j = 0; j <= block.m_Size.x; j++)
					{
						float3 val9 = val6 + val4 * ((float)j * 8f);
						Color val10 = ((j == 0 || j == block.m_Size.x) ? val2 : val3);
						((GizmoBatcher)(ref m_GizmoBatcher)).DrawLine(val9, val9 + val8, val10);
					}
					for (int k = 0; k <= block.m_Size.y; k++)
					{
						float3 val11 = val6 + val5 * ((float)k * 8f);
						Color val12 = ((k == 0 || k == block.m_Size.y) ? val2 : val3);
						((GizmoBatcher)(ref m_GizmoBatcher)).DrawLine(val11, val11 + val7, val12);
					}
				}
			}
			if (!m_LotOption)
			{
				return;
			}
			float3 val14 = default(float3);
			float3 val15 = default(float3);
			for (int l = 0; l < bufferAccessor.Length; l++)
			{
				Block block2 = nativeArray[l];
				DynamicBuffer<VacantLot> val13 = bufferAccessor[l];
				((float3)(ref val14))._002Ector(0f - block2.m_Direction.y, 0f, block2.m_Direction.x);
				((float3)(ref val15))._002Ector(0f - block2.m_Direction.x, 0f, 0f - block2.m_Direction.y);
				float4x4 val16 = float4x4.LookAt(default(float3), val15, math.up());
				for (int m = 0; m < val13.Length; m++)
				{
					VacantLot vacantLot = val13[m];
					float2 val17 = float2.op_Implicit(((int4)(ref vacantLot.m_Area)).xz + ((int4)(ref vacantLot.m_Area)).yw - block2.m_Size) * 4f;
					float3 val18 = block2.m_Position + val14 * val17.x + val15 * val17.y;
					float2 val19 = float2.op_Implicit(((int4)(ref vacantLot.m_Area)).yw - ((int4)(ref vacantLot.m_Area)).xz) * 8f;
					float4x4 val20 = math.mul(float4x4.Translate(val18), val16);
					ZoneData zoneData = m_PrefabZoneData[m_ZonePrefabs[vacantLot.m_Type]];
					Color zoneColor = GetZoneColor(zoneData);
					float num = math.min((float)(int)zoneData.m_MaxHeight, (float)vacantLot.m_Height - block2.m_Position.y);
					((GizmoBatcher)(ref m_GizmoBatcher)).DrawWireCube(val20, new float3(0f, num * 0.5f, 0f), new float3(val19.x, num, val19.y), zoneColor);
					val18.y += num;
					if ((vacantLot.m_Flags & LotFlags.CornerLeft) != 0)
					{
						float3 val21 = val18 - val14 * (val19.x * 0.5f) - val15 * (val19.y * 0.5f);
						float3 val22 = val18 - val14 * (val19.x * 0.5f - 2f) - val15 * (val19.y * 0.5f - 2f);
						((GizmoBatcher)(ref m_GizmoBatcher)).DrawLine(val21, val22, zoneColor);
					}
					if ((vacantLot.m_Flags & LotFlags.CornerRight) != 0)
					{
						float3 val23 = val18 + val14 * (val19.x * 0.5f) - val15 * (val19.y * 0.5f);
						float3 val24 = val18 + val14 * (val19.x * 0.5f - 2f) - val15 * (val19.y * 0.5f - 2f);
						((GizmoBatcher)(ref m_GizmoBatcher)).DrawLine(val23, val24, zoneColor);
					}
					if ((vacantLot.m_Flags & (LotFlags.CornerLeft | LotFlags.CornerRight)) == 0)
					{
						float3 val25 = val18 - val15 * (val19.y * 0.5f);
						float3 val26 = val18 - val15 * (val19.y * 0.5f - 2f);
						((GizmoBatcher)(ref m_GizmoBatcher)).DrawLine(val25, val26, zoneColor);
					}
				}
			}
		}

		private Color GetZoneColor(ZoneData zoneData)
		{
			//IL_001d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0023: Unknown result type (might be due to invalid IL or missing references)
			//IL_0029: Unknown result type (might be due to invalid IL or missing references)
			//IL_002f: Unknown result type (might be due to invalid IL or missing references)
			return (Color)(zoneData.m_AreaType switch
			{
				AreaType.Residential => Color.green, 
				AreaType.Commercial => Color.cyan, 
				AreaType.Industrial => Color.yellow, 
				_ => Color.white, 
			});
		}

		void IJobChunk.Execute(in ArchetypeChunk chunk, int unfilteredChunkIndex, bool useEnabledMask, in v128 chunkEnabledMask)
		{
			Execute(in chunk, unfilteredChunkIndex, useEnabledMask, in chunkEnabledMask);
		}
	}

	private struct TypeHandle
	{
		[ReadOnly]
		public ComponentTypeHandle<Block> __Game_Zones_Block_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<Temp> __Game_Tools_Temp_RO_ComponentTypeHandle;

		[ReadOnly]
		public BufferTypeHandle<VacantLot> __Game_Zones_VacantLot_RO_BufferTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<Error> __Game_Tools_Error_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentLookup<ZoneData> __Game_Prefabs_ZoneData_RO_ComponentLookup;

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
			__Game_Zones_Block_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<Block>(true);
			__Game_Tools_Temp_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<Temp>(true);
			__Game_Zones_VacantLot_RO_BufferTypeHandle = ((SystemState)(ref state)).GetBufferTypeHandle<VacantLot>(true);
			__Game_Tools_Error_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<Error>(true);
			__Game_Prefabs_ZoneData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<ZoneData>(true);
		}
	}

	private EntityQuery m_BlockGroup;

	private GizmosSystem m_GizmosSystem;

	private ZoneSystem m_ZoneSystem;

	private Option m_PivotOption;

	private Option m_GridOption;

	private Option m_LotOption;

	private TypeHandle __TypeHandle;

	[Preserve]
	protected override void OnCreate()
	{
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_0096: Unknown result type (might be due to invalid IL or missing references)
		base.OnCreate();
		m_GizmosSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<GizmosSystem>();
		m_ZoneSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<ZoneSystem>();
		m_BlockGroup = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[3]
		{
			ComponentType.ReadOnly<Block>(),
			ComponentType.Exclude<Deleted>(),
			ComponentType.Exclude<Hidden>()
		});
		m_PivotOption = AddOption("Draw Pivots", defaultEnabled: false);
		m_GridOption = AddOption("Draw Grids", defaultEnabled: true);
		m_LotOption = AddOption("Vacant Lots", defaultEnabled: true);
		((ComponentSystemBase)this).RequireForUpdate(m_BlockGroup);
		((ComponentSystemBase)this).Enabled = false;
	}

	[Preserve]
	protected override JobHandle OnUpdate(JobHandle inputDeps)
	{
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		//IL_0085: Unknown result type (might be due to invalid IL or missing references)
		//IL_009d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00eb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fe: Unknown result type (might be due to invalid IL or missing references)
		//IL_0103: Unknown result type (might be due to invalid IL or missing references)
		//IL_0108: Unknown result type (might be due to invalid IL or missing references)
		//IL_010f: Unknown result type (might be due to invalid IL or missing references)
		//IL_011b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0121: Unknown result type (might be due to invalid IL or missing references)
		JobHandle val2 = default(JobHandle);
		JobHandle val = JobChunkExtensions.ScheduleParallel<BlockGizmoJob>(new BlockGizmoJob
		{
			m_PivotOption = m_PivotOption.enabled,
			m_GridOption = m_GridOption.enabled,
			m_LotOption = m_LotOption.enabled,
			m_ZonePrefabs = m_ZoneSystem.GetPrefabs(),
			m_BlockType = InternalCompilerInterface.GetComponentTypeHandle<Block>(ref __TypeHandle.__Game_Zones_Block_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_TempType = InternalCompilerInterface.GetComponentTypeHandle<Temp>(ref __TypeHandle.__Game_Tools_Temp_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_VacantLotType = InternalCompilerInterface.GetBufferTypeHandle<VacantLot>(ref __TypeHandle.__Game_Zones_VacantLot_RO_BufferTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_ErrorType = InternalCompilerInterface.GetComponentTypeHandle<Error>(ref __TypeHandle.__Game_Tools_Error_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_PrefabZoneData = InternalCompilerInterface.GetComponentLookup<ZoneData>(ref __TypeHandle.__Game_Prefabs_ZoneData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_GizmoBatcher = m_GizmosSystem.GetGizmosBatcher(ref val2)
		}, m_BlockGroup, JobHandle.CombineDependencies(inputDeps, val2));
		m_ZoneSystem.AddPrefabsReader(val);
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
	public ZoneDebugSystem()
	{
	}
}

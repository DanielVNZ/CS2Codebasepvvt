using System.Runtime.CompilerServices;
using Colossal;
using Game.Common;
using Game.Net;
using Game.Objects;
using Game.Prefabs;
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

namespace Game.Rendering;

[CompilerGenerated]
public class EditorGizmoSystem : GameSystemBase
{
	[BurstCompile]
	private struct EditorGizmoJob : IJobChunk
	{
		[ReadOnly]
		public ComponentTypeHandle<Node> m_NetNodeType;

		[ReadOnly]
		public ComponentTypeHandle<Curve> m_NetCurveType;

		[ReadOnly]
		public ComponentTypeHandle<Transform> m_TransformType;

		[ReadOnly]
		public ComponentTypeHandle<Temp> m_TempType;

		[ReadOnly]
		public ComponentTypeHandle<Error> m_ErrorType;

		[ReadOnly]
		public ComponentTypeHandle<Warning> m_WarningType;

		[ReadOnly]
		public ComponentTypeHandle<Highlighted> m_HighlightedType;

		[ReadOnly]
		public ComponentTypeHandle<CullingInfo> m_CullingInfoType;

		[ReadOnly]
		public Color m_HoveredColor;

		[ReadOnly]
		public Color m_ErrorColor;

		[ReadOnly]
		public Color m_WarningColor;

		[ReadOnly]
		public NativeList<PreCullingData> m_CullingData;

		public GizmoBatcher m_GizmoBatcher;

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
			//IL_0054: Unknown result type (might be due to invalid IL or missing references)
			//IL_0059: Unknown result type (might be due to invalid IL or missing references)
			//IL_005c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0061: Unknown result type (might be due to invalid IL or missing references)
			//IL_0064: Unknown result type (might be due to invalid IL or missing references)
			//IL_0069: Unknown result type (might be due to invalid IL or missing references)
			//IL_006c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0071: Unknown result type (might be due to invalid IL or missing references)
			//IL_008a: Unknown result type (might be due to invalid IL or missing references)
			//IL_008f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0092: Unknown result type (might be due to invalid IL or missing references)
			//IL_0097: Unknown result type (might be due to invalid IL or missing references)
			//IL_009a: Unknown result type (might be due to invalid IL or missing references)
			//IL_009f: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a7: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ea: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ef: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f6: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f8: Unknown result type (might be due to invalid IL or missing references)
			//IL_00fd: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ff: Unknown result type (might be due to invalid IL or missing references)
			//IL_0104: Unknown result type (might be due to invalid IL or missing references)
			//IL_00bd: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c5: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ca: Unknown result type (might be due to invalid IL or missing references)
			//IL_00cd: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d5: Unknown result type (might be due to invalid IL or missing references)
			//IL_00da: Unknown result type (might be due to invalid IL or missing references)
			//IL_0122: Unknown result type (might be due to invalid IL or missing references)
			//IL_0124: Unknown result type (might be due to invalid IL or missing references)
			//IL_016b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0170: Unknown result type (might be due to invalid IL or missing references)
			//IL_0175: Unknown result type (might be due to invalid IL or missing references)
			//IL_017a: Unknown result type (might be due to invalid IL or missing references)
			//IL_017e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0183: Unknown result type (might be due to invalid IL or missing references)
			//IL_0188: Unknown result type (might be due to invalid IL or missing references)
			//IL_018d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0191: Unknown result type (might be due to invalid IL or missing references)
			//IL_0196: Unknown result type (might be due to invalid IL or missing references)
			//IL_019b: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a0: Unknown result type (might be due to invalid IL or missing references)
			//IL_01aa: Unknown result type (might be due to invalid IL or missing references)
			//IL_01af: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b1: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b8: Unknown result type (might be due to invalid IL or missing references)
			//IL_01bd: Unknown result type (might be due to invalid IL or missing references)
			//IL_01bf: Unknown result type (might be due to invalid IL or missing references)
			//IL_01c4: Unknown result type (might be due to invalid IL or missing references)
			//IL_01d3: Unknown result type (might be due to invalid IL or missing references)
			//IL_01d8: Unknown result type (might be due to invalid IL or missing references)
			//IL_01da: Unknown result type (might be due to invalid IL or missing references)
			//IL_01e1: Unknown result type (might be due to invalid IL or missing references)
			//IL_01e6: Unknown result type (might be due to invalid IL or missing references)
			//IL_01e8: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ed: Unknown result type (might be due to invalid IL or missing references)
			//IL_01fc: Unknown result type (might be due to invalid IL or missing references)
			//IL_0201: Unknown result type (might be due to invalid IL or missing references)
			//IL_0203: Unknown result type (might be due to invalid IL or missing references)
			//IL_020a: Unknown result type (might be due to invalid IL or missing references)
			//IL_020f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0211: Unknown result type (might be due to invalid IL or missing references)
			//IL_0216: Unknown result type (might be due to invalid IL or missing references)
			//IL_0247: Unknown result type (might be due to invalid IL or missing references)
			//IL_0249: Unknown result type (might be due to invalid IL or missing references)
			//IL_0157: Unknown result type (might be due to invalid IL or missing references)
			//IL_015c: Unknown result type (might be due to invalid IL or missing references)
			//IL_028c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0293: Unknown result type (might be due to invalid IL or missing references)
			//IL_02c8: Unknown result type (might be due to invalid IL or missing references)
			//IL_02ca: Unknown result type (might be due to invalid IL or missing references)
			//IL_02cc: Unknown result type (might be due to invalid IL or missing references)
			//IL_02ce: Unknown result type (might be due to invalid IL or missing references)
			//IL_02d0: Unknown result type (might be due to invalid IL or missing references)
			//IL_02d2: Unknown result type (might be due to invalid IL or missing references)
			//IL_0279: Unknown result type (might be due to invalid IL or missing references)
			//IL_027e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0329: Unknown result type (might be due to invalid IL or missing references)
			//IL_032e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0333: Unknown result type (might be due to invalid IL or missing references)
			//IL_0338: Unknown result type (might be due to invalid IL or missing references)
			//IL_033c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0341: Unknown result type (might be due to invalid IL or missing references)
			//IL_0346: Unknown result type (might be due to invalid IL or missing references)
			//IL_034b: Unknown result type (might be due to invalid IL or missing references)
			//IL_034f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0354: Unknown result type (might be due to invalid IL or missing references)
			//IL_0359: Unknown result type (might be due to invalid IL or missing references)
			//IL_035e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0368: Unknown result type (might be due to invalid IL or missing references)
			//IL_036d: Unknown result type (might be due to invalid IL or missing references)
			//IL_036f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0376: Unknown result type (might be due to invalid IL or missing references)
			//IL_037b: Unknown result type (might be due to invalid IL or missing references)
			//IL_037d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0382: Unknown result type (might be due to invalid IL or missing references)
			//IL_039d: Unknown result type (might be due to invalid IL or missing references)
			//IL_03a2: Unknown result type (might be due to invalid IL or missing references)
			//IL_03a4: Unknown result type (might be due to invalid IL or missing references)
			//IL_03ab: Unknown result type (might be due to invalid IL or missing references)
			//IL_03b0: Unknown result type (might be due to invalid IL or missing references)
			//IL_03b2: Unknown result type (might be due to invalid IL or missing references)
			//IL_03b7: Unknown result type (might be due to invalid IL or missing references)
			//IL_03d2: Unknown result type (might be due to invalid IL or missing references)
			//IL_03d7: Unknown result type (might be due to invalid IL or missing references)
			//IL_03d9: Unknown result type (might be due to invalid IL or missing references)
			//IL_03e0: Unknown result type (might be due to invalid IL or missing references)
			//IL_03e5: Unknown result type (might be due to invalid IL or missing references)
			//IL_03e7: Unknown result type (might be due to invalid IL or missing references)
			//IL_03ec: Unknown result type (might be due to invalid IL or missing references)
			//IL_0305: Unknown result type (might be due to invalid IL or missing references)
			//IL_030a: Unknown result type (might be due to invalid IL or missing references)
			//IL_030d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0312: Unknown result type (might be due to invalid IL or missing references)
			//IL_0315: Unknown result type (might be due to invalid IL or missing references)
			//IL_031a: Unknown result type (might be due to invalid IL or missing references)
			NativeArray<Node> nativeArray = ((ArchetypeChunk)(ref chunk)).GetNativeArray<Node>(ref m_NetNodeType);
			NativeArray<Curve> nativeArray2 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<Curve>(ref m_NetCurveType);
			NativeArray<Transform> nativeArray3 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<Transform>(ref m_TransformType);
			NativeArray<Temp> nativeArray4 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<Temp>(ref m_TempType);
			NativeArray<CullingInfo> nativeArray5 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<CullingInfo>(ref m_CullingInfoType);
			bool flag;
			Color val;
			Color val2;
			Color val3;
			Color val4;
			if (((ArchetypeChunk)(ref chunk)).Has<Error>(ref m_ErrorType))
			{
				flag = false;
				val = m_ErrorColor;
				val2 = m_ErrorColor;
				val3 = m_ErrorColor;
				val4 = m_ErrorColor;
			}
			else if (((ArchetypeChunk)(ref chunk)).Has<Warning>(ref m_WarningType))
			{
				flag = false;
				val = m_WarningColor;
				val2 = m_WarningColor;
				val3 = m_WarningColor;
				val4 = m_WarningColor;
			}
			else if (((ArchetypeChunk)(ref chunk)).Has<Highlighted>(ref m_HighlightedType))
			{
				flag = false;
				val = m_HoveredColor;
				val2 = m_HoveredColor;
				val3 = m_HoveredColor;
				val4 = m_HoveredColor;
			}
			else
			{
				flag = nativeArray4.Length != 0;
				val = Color.white;
				val2 = Color.red;
				val3 = Color.green;
				val4 = Color.blue;
			}
			for (int i = 0; i < nativeArray.Length; i++)
			{
				if (!IsNearCamera(nativeArray5[i]))
				{
					continue;
				}
				Color val5 = val;
				if (flag)
				{
					Temp temp = nativeArray4[i];
					if ((temp.m_Flags & TempFlags.Hidden) != 0)
					{
						continue;
					}
					if ((temp.m_Flags & (TempFlags.Create | TempFlags.Delete | TempFlags.Select | TempFlags.Modify | TempFlags.Replace)) != 0)
					{
						val5 = m_HoveredColor;
					}
				}
				Node node = nativeArray[i];
				float3 val6 = math.rotate(node.m_Rotation, math.right());
				float3 val7 = math.rotate(node.m_Rotation, math.up());
				float3 val8 = math.rotate(node.m_Rotation, math.forward());
				((GizmoBatcher)(ref m_GizmoBatcher)).DrawLine(node.m_Position - val6, node.m_Position + val6, val5);
				((GizmoBatcher)(ref m_GizmoBatcher)).DrawLine(node.m_Position - val7, node.m_Position + val7, val5);
				((GizmoBatcher)(ref m_GizmoBatcher)).DrawLine(node.m_Position - val8, node.m_Position + val8, val5);
			}
			for (int j = 0; j < nativeArray2.Length; j++)
			{
				if (!IsNearCamera(nativeArray5[j]))
				{
					continue;
				}
				Color color = val;
				if (flag)
				{
					Temp temp2 = nativeArray4[j];
					if ((temp2.m_Flags & TempFlags.Hidden) != 0)
					{
						continue;
					}
					if ((temp2.m_Flags & (TempFlags.Create | TempFlags.Delete | TempFlags.Select | TempFlags.Modify | TempFlags.Replace)) != 0)
					{
						color = m_HoveredColor;
					}
				}
				Curve curve = nativeArray2[j];
				m_GizmoBatcher.DrawCurve(curve, color);
			}
			for (int k = 0; k < nativeArray3.Length; k++)
			{
				if (!IsNearCamera(nativeArray5[k]))
				{
					continue;
				}
				Color val9 = val2;
				Color val10 = val3;
				Color val11 = val4;
				if (flag)
				{
					Temp temp3 = nativeArray4[k];
					if ((temp3.m_Flags & TempFlags.Hidden) != 0)
					{
						continue;
					}
					if ((temp3.m_Flags & (TempFlags.Create | TempFlags.Delete | TempFlags.Select | TempFlags.Modify | TempFlags.Replace)) != 0)
					{
						val9 = m_HoveredColor;
						val10 = m_HoveredColor;
						val11 = m_HoveredColor;
					}
				}
				Transform transform = nativeArray3[k];
				float3 val12 = math.rotate(transform.m_Rotation, math.right());
				float3 val13 = math.rotate(transform.m_Rotation, math.up());
				float3 val14 = math.rotate(transform.m_Rotation, math.forward());
				((GizmoBatcher)(ref m_GizmoBatcher)).DrawArrow(transform.m_Position - val12, transform.m_Position + val12, val9, 0.4f, 25f, 16);
				((GizmoBatcher)(ref m_GizmoBatcher)).DrawArrow(transform.m_Position - val13, transform.m_Position + val13, val10, 0.4f, 25f, 16);
				((GizmoBatcher)(ref m_GizmoBatcher)).DrawArrow(transform.m_Position - val14, transform.m_Position + val14, val11, 0.4f, 25f, 16);
			}
		}

		private bool IsNearCamera(CullingInfo cullingInfo)
		{
			if (cullingInfo.m_CullingIndex != 0)
			{
				return (m_CullingData[cullingInfo.m_CullingIndex].m_Flags & PreCullingFlags.NearCamera) != 0;
			}
			return false;
		}

		void IJobChunk.Execute(in ArchetypeChunk chunk, int unfilteredChunkIndex, bool useEnabledMask, in v128 chunkEnabledMask)
		{
			Execute(in chunk, unfilteredChunkIndex, useEnabledMask, in chunkEnabledMask);
		}
	}

	private struct TypeHandle
	{
		[ReadOnly]
		public ComponentTypeHandle<Node> __Game_Net_Node_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<Curve> __Game_Net_Curve_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<Transform> __Game_Objects_Transform_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<Temp> __Game_Tools_Temp_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<Error> __Game_Tools_Error_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<Warning> __Game_Tools_Warning_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<Highlighted> __Game_Tools_Highlighted_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<CullingInfo> __Game_Rendering_CullingInfo_RO_ComponentTypeHandle;

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
			__Game_Net_Node_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<Node>(true);
			__Game_Net_Curve_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<Curve>(true);
			__Game_Objects_Transform_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<Transform>(true);
			__Game_Tools_Temp_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<Temp>(true);
			__Game_Tools_Error_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<Error>(true);
			__Game_Tools_Warning_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<Warning>(true);
			__Game_Tools_Highlighted_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<Highlighted>(true);
			__Game_Rendering_CullingInfo_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<CullingInfo>(true);
		}
	}

	private EntityQuery m_RenderQuery;

	private EntityQuery m_RenderingSettingsQuery;

	private GizmosSystem m_GizmosSystem;

	private PreCullingSystem m_PreCullingSystem;

	private RenderingSystem m_RenderingSystem;

	private TypeHandle __TypeHandle;

	[Preserve]
	protected override void OnCreate()
	{
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0079: Unknown result type (might be due to invalid IL or missing references)
		//IL_007e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0083: Unknown result type (might be due to invalid IL or missing references)
		//IL_0088: Unknown result type (might be due to invalid IL or missing references)
		//IL_008f: Unknown result type (might be due to invalid IL or missing references)
		base.OnCreate();
		m_GizmosSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<GizmosSystem>();
		m_PreCullingSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<PreCullingSystem>();
		m_RenderingSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<RenderingSystem>();
		m_RenderQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[3]
		{
			ComponentType.ReadOnly<Game.Tools.EditorContainer>(),
			ComponentType.Exclude<Deleted>(),
			ComponentType.Exclude<Hidden>()
		});
		m_RenderingSettingsQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[1] { ComponentType.ReadOnly<RenderingSettingsData>() });
		((ComponentSystemBase)this).RequireForUpdate(m_RenderQuery);
	}

	[Preserve]
	protected override void OnUpdate()
	{
		//IL_00ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_0104: Unknown result type (might be due to invalid IL or missing references)
		//IL_0109: Unknown result type (might be due to invalid IL or missing references)
		//IL_0121: Unknown result type (might be due to invalid IL or missing references)
		//IL_0126: Unknown result type (might be due to invalid IL or missing references)
		//IL_013e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0143: Unknown result type (might be due to invalid IL or missing references)
		//IL_015b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0160: Unknown result type (might be due to invalid IL or missing references)
		//IL_0178: Unknown result type (might be due to invalid IL or missing references)
		//IL_017d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0195: Unknown result type (might be due to invalid IL or missing references)
		//IL_019a: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01db: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ef: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01fb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0203: Unknown result type (might be due to invalid IL or missing references)
		//IL_0210: Unknown result type (might be due to invalid IL or missing references)
		//IL_0218: Unknown result type (might be due to invalid IL or missing references)
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
		//IL_007d: Unknown result type (might be due to invalid IL or missing references)
		//IL_007f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0084: Unknown result type (might be due to invalid IL or missing references)
		//IL_0085: Unknown result type (might be due to invalid IL or missing references)
		//IL_008a: Unknown result type (might be due to invalid IL or missing references)
		if (!m_RenderingSystem.hideOverlay)
		{
			Color hoveredColor = default(Color);
			((Color)(ref hoveredColor))._002Ector(0.5f, 0.5f, 1f, 1f);
			Color errorColor = default(Color);
			((Color)(ref errorColor))._002Ector(1f, 0.5f, 0.5f, 1f);
			Color warningColor = default(Color);
			((Color)(ref warningColor))._002Ector(1f, 1f, 0.5f, 1f);
			if (!((EntityQuery)(ref m_RenderingSettingsQuery)).IsEmptyIgnoreFilter)
			{
				RenderingSettingsData singleton = ((EntityQuery)(ref m_RenderingSettingsQuery)).GetSingleton<RenderingSettingsData>();
				hoveredColor = singleton.m_HoveredColor;
				errorColor = singleton.m_ErrorColor;
				warningColor = singleton.m_WarningColor;
				hoveredColor.a = 1f;
				errorColor.a = 1f;
				warningColor.a = 1f;
			}
			JobHandle dependencies;
			JobHandle val2 = default(JobHandle);
			JobHandle val = JobChunkExtensions.ScheduleParallel<EditorGizmoJob>(new EditorGizmoJob
			{
				m_NetNodeType = InternalCompilerInterface.GetComponentTypeHandle<Node>(ref __TypeHandle.__Game_Net_Node_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
				m_NetCurveType = InternalCompilerInterface.GetComponentTypeHandle<Curve>(ref __TypeHandle.__Game_Net_Curve_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
				m_TransformType = InternalCompilerInterface.GetComponentTypeHandle<Transform>(ref __TypeHandle.__Game_Objects_Transform_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
				m_TempType = InternalCompilerInterface.GetComponentTypeHandle<Temp>(ref __TypeHandle.__Game_Tools_Temp_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
				m_ErrorType = InternalCompilerInterface.GetComponentTypeHandle<Error>(ref __TypeHandle.__Game_Tools_Error_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
				m_WarningType = InternalCompilerInterface.GetComponentTypeHandle<Warning>(ref __TypeHandle.__Game_Tools_Warning_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
				m_HighlightedType = InternalCompilerInterface.GetComponentTypeHandle<Highlighted>(ref __TypeHandle.__Game_Tools_Highlighted_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
				m_CullingInfoType = InternalCompilerInterface.GetComponentTypeHandle<CullingInfo>(ref __TypeHandle.__Game_Rendering_CullingInfo_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
				m_HoveredColor = hoveredColor,
				m_ErrorColor = errorColor,
				m_WarningColor = warningColor,
				m_CullingData = m_PreCullingSystem.GetCullingData(readOnly: true, out dependencies),
				m_GizmoBatcher = m_GizmosSystem.GetGizmosBatcher(ref val2)
			}, m_RenderQuery, JobHandle.CombineDependencies(((SystemBase)this).Dependency, dependencies, val2));
			m_GizmosSystem.AddGizmosBatcherWriter(val);
			m_PreCullingSystem.AddCullingDataReader(val);
			((SystemBase)this).Dependency = val;
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
	public EditorGizmoSystem()
	{
	}
}

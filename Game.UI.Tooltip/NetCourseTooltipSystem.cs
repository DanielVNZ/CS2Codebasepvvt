using System;
using System.Runtime.CompilerServices;
using Colossal.Mathematics;
using Game.Tools;
using Unity.Collections;
using Unity.Entities;
using Unity.Entities.Internal;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Scripting;

namespace Game.UI.Tooltip;

[CompilerGenerated]
public class NetCourseTooltipSystem : TooltipSystemBase
{
	private struct TypeHandle
	{
		[ReadOnly]
		public ComponentTypeHandle<NetCourse> __Game_Tools_NetCourse_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<CreationDefinition> __Game_Tools_CreationDefinition_RO_ComponentTypeHandle;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void __AssignHandles(ref SystemState state)
		{
			//IL_0003: Unknown result type (might be due to invalid IL or missing references)
			//IL_0008: Unknown result type (might be due to invalid IL or missing references)
			//IL_0010: Unknown result type (might be due to invalid IL or missing references)
			//IL_0015: Unknown result type (might be due to invalid IL or missing references)
			__Game_Tools_NetCourse_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<NetCourse>(true);
			__Game_Tools_CreationDefinition_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<CreationDefinition>(true);
		}
	}

	private const float kMinLength = 12f;

	private ToolSystem m_ToolSystem;

	private NetToolSystem m_NetTool;

	private EntityQuery m_NetCourseQuery;

	private TooltipGroup m_Group;

	private FloatTooltip m_Length;

	private FloatTooltip m_Slope;

	private TypeHandle __TypeHandle;

	[Preserve]
	protected override void OnCreate()
	{
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		base.OnCreate();
		m_ToolSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<ToolSystem>();
		m_NetTool = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<NetToolSystem>();
		m_NetCourseQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[2]
		{
			ComponentType.ReadOnly<CreationDefinition>(),
			ComponentType.ReadOnly<NetCourse>()
		});
		((ComponentSystemBase)this).RequireForUpdate(m_NetCourseQuery);
		m_Length = new FloatTooltip
		{
			icon = "Media/Glyphs/Length.svg",
			unit = "length"
		};
		m_Slope = new FloatTooltip
		{
			icon = "Media/Glyphs/Slope.svg",
			unit = "percentageSingleFraction",
			signed = true
		};
		m_Group = new TooltipGroup
		{
			path = "tempNetEdgeStart",
			horizontalAlignment = TooltipGroup.Alignment.Center,
			verticalAlignment = TooltipGroup.Alignment.Center,
			category = TooltipGroup.Category.Network
		};
		m_Group.children.Add(m_Length);
		m_Group.children.Add(m_Slope);
	}

	[Preserve]
	protected override void OnUpdate()
	{
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
		//IL_007d: Unknown result type (might be due to invalid IL or missing references)
		//IL_008f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0094: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00be: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01db: Unknown result type (might be due to invalid IL or missing references)
		//IL_01fb: Unknown result type (might be due to invalid IL or missing references)
		//IL_023c: Unknown result type (might be due to invalid IL or missing references)
		//IL_024c: Unknown result type (might be due to invalid IL or missing references)
		//IL_024f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0254: Unknown result type (might be due to invalid IL or missing references)
		//IL_025b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0260: Unknown result type (might be due to invalid IL or missing references)
		//IL_0268: Unknown result type (might be due to invalid IL or missing references)
		//IL_026d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0271: Unknown result type (might be due to invalid IL or missing references)
		//IL_0280: Unknown result type (might be due to invalid IL or missing references)
		//IL_014d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0152: Unknown result type (might be due to invalid IL or missing references)
		//IL_0154: Unknown result type (might be due to invalid IL or missing references)
		//IL_0159: Unknown result type (might be due to invalid IL or missing references)
		//IL_015d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0162: Unknown result type (might be due to invalid IL or missing references)
		//IL_0166: Unknown result type (might be due to invalid IL or missing references)
		if (m_ToolSystem.activeTool != m_NetTool || m_NetTool.mode == NetToolSystem.Mode.Replace || !((Object)(object)Camera.main != (Object)null))
		{
			return;
		}
		((SystemBase)this).CompleteDependency();
		NativeList<NetCourse> courses = default(NativeList<NetCourse>);
		courses._002Ector(((EntityQuery)(ref m_NetCourseQuery)).CalculateEntityCount(), AllocatorHandle.op_Implicit((Allocator)2));
		NativeArray<ArchetypeChunk> val = ((EntityQuery)(ref m_NetCourseQuery)).ToArchetypeChunkArray(AllocatorHandle.op_Implicit((Allocator)2));
		try
		{
			ComponentTypeHandle<NetCourse> componentTypeHandle = InternalCompilerInterface.GetComponentTypeHandle<NetCourse>(ref __TypeHandle.__Game_Tools_NetCourse_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef);
			ComponentTypeHandle<CreationDefinition> componentTypeHandle2 = InternalCompilerInterface.GetComponentTypeHandle<CreationDefinition>(ref __TypeHandle.__Game_Tools_CreationDefinition_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef);
			float num = 0f;
			float num2 = 0f;
			Enumerator<ArchetypeChunk> enumerator = val.GetEnumerator();
			try
			{
				float2 val2 = default(float2);
				while (enumerator.MoveNext())
				{
					ArchetypeChunk current = enumerator.Current;
					NativeArray<NetCourse> nativeArray = ((ArchetypeChunk)(ref current)).GetNativeArray<NetCourse>(ref componentTypeHandle);
					NativeArray<CreationDefinition> nativeArray2 = ((ArchetypeChunk)(ref current)).GetNativeArray<CreationDefinition>(ref componentTypeHandle2);
					for (int i = 0; i < nativeArray.Length; i++)
					{
						NetCourse netCourse = nativeArray[i];
						CreationDefinition creationDefinition = nativeArray2[i];
						if (!(creationDefinition.m_Original != Entity.Null) && (creationDefinition.m_Flags & (CreationFlags.Permanent | CreationFlags.Delete | CreationFlags.Upgrade | CreationFlags.Invert | CreationFlags.Align)) == 0 && (netCourse.m_StartPosition.m_Flags & CoursePosFlags.IsParallel) == 0)
						{
							num += netCourse.m_Length;
							((float2)(ref val2))._002Ector(netCourse.m_StartPosition.m_CourseDelta, netCourse.m_EndPosition.m_CourseDelta);
							Bezier4x3 val3 = MathUtils.Cut(netCourse.m_Curve, val2);
							Bezier4x2 xz = ((Bezier4x3)(ref val3)).xz;
							num2 += MathUtils.Length(xz);
							courses.Add(ref netCourse);
						}
					}
				}
			}
			finally
			{
				((IDisposable)enumerator/*cast due to .constrained prefix*/).Dispose();
			}
			m_Length.value = num2;
			if (courses.Length != 0 && num2 >= 12f)
			{
				float y = courses[0].m_StartPosition.m_Position.y;
				float y2 = courses[courses.Length - 1].m_EndPosition.m_Position.y;
				float num3 = 100f * (y2 - y) / num2;
				m_Slope.value = math.select(num3, 0f, math.abs(num3) < 0.05f);
				SortCourses(courses);
				float length = num / 2f;
				bool onScreen;
				float2 val4 = TooltipSystemBase.WorldToTooltipPos(float3.op_Implicit(GetWorldPosition(courses, length)), out onScreen);
				float2 position = m_Group.position;
				if (!((float2)(ref position)).Equals(val4))
				{
					m_Group.position = val4;
					m_Group.SetChildrenChanged();
				}
				if (onScreen)
				{
					AddGroup(m_Group);
					return;
				}
				AddMouseTooltip(m_Length);
				AddMouseTooltip(m_Slope);
			}
		}
		finally
		{
			courses.Dispose();
			val.Dispose();
		}
	}

	private static float3 GetWorldPosition(NativeList<NetCourse> courses, float length)
	{
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_009a: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		//IL_0069: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
		float num = 0f - length;
		Enumerator<NetCourse> enumerator = courses.GetEnumerator();
		try
		{
			while (enumerator.MoveNext())
			{
				NetCourse current = enumerator.Current;
				num += current.m_Length;
				if (num >= 0f && current.m_Length != 0f)
				{
					float num2 = math.lerp(current.m_StartPosition.m_CourseDelta, current.m_EndPosition.m_CourseDelta, 1f - num / current.m_Length);
					return MathUtils.Position(current.m_Curve, num2);
				}
			}
		}
		finally
		{
			((IDisposable)enumerator/*cast due to .constrained prefix*/).Dispose();
		}
		return courses[courses.Length - 1].m_EndPosition.m_Position;
	}

	private static void SortCourses(NativeList<NetCourse> courses)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		NativeArray<NetCourse> val = courses.AsArray();
		for (int i = 0; i < val.Length; i++)
		{
			NetCourse netCourse = courses[i];
			if ((netCourse.m_StartPosition.m_Flags & CoursePosFlags.IsFirst) != 0)
			{
				courses[i] = courses[0];
				courses[0] = netCourse;
				break;
			}
		}
		for (int j = 0; j < courses.Length - 1; j++)
		{
			NetCourse netCourse2 = courses[j];
			for (int k = j + 1; k < courses.Length; k++)
			{
				NetCourse netCourse3 = courses[k];
				if (((float3)(ref netCourse2.m_EndPosition.m_Position)).Equals(netCourse3.m_StartPosition.m_Position))
				{
					courses[k] = courses[j + 1];
					courses[j + 1] = netCourse3;
					break;
				}
			}
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
	public NetCourseTooltipSystem()
	{
	}
}

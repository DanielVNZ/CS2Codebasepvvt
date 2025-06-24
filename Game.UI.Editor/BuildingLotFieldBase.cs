using System;
using Colossal.Mathematics;
using Game.Prefabs;
using Game.Reflection;
using Game.UI.Widgets;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace Game.UI.Editor;

public abstract class BuildingLotFieldBase : IFieldBuilderFactory
{
	private static readonly int kMaxSize = 1500;

	private static readonly int kMinSize = 1;

	public abstract FieldBuilder TryCreate(Type memberType, object[] attributes);

	protected FieldBuilder TryCreate(Type memberType, object[] attributes, bool horizontal)
	{
		return delegate(IValueAccessor accessor)
		{
			IntInputField intInputField = new IntInputField
			{
				displayName = (horizontal ? "Lot Width" : "Lot Depth"),
				accessor = new CastAccessor<int>(accessor, (object input) => (int)input, (int input) => input),
				min = kMinSize,
				max = kMaxSize
			};
			if (TryGetBuildingPrefab(accessor, out var prefab))
			{
				Button button1 = new Button
				{
					displayName = (horizontal ? "Expand Left" : "Expand Front")
				};
				button1.action = delegate
				{
					//IL_003d: Unknown result type (might be due to invalid IL or missing references)
					AddCells(prefab, button1, new int2(horizontal ? (-1) : 0, (!horizontal) ? (-1) : 0));
				};
				button1.disabled = () => (!horizontal) ? (prefab.m_LotDepth >= kMaxSize) : (prefab.m_LotWidth >= kMaxSize);
				Button button2 = new Button
				{
					displayName = (horizontal ? "Expand Right" : "Expand Back")
				};
				button2.action = delegate
				{
					//IL_003d: Unknown result type (might be due to invalid IL or missing references)
					AddCells(prefab, button2, new int2(horizontal ? 1 : 0, (!horizontal) ? 1 : 0));
				};
				button2.disabled = () => (!horizontal) ? (prefab.m_LotDepth >= kMaxSize) : (prefab.m_LotWidth >= kMaxSize);
				Button button3 = new Button
				{
					displayName = (horizontal ? "Shrink Left" : "Shrink Front")
				};
				button3.action = delegate
				{
					//IL_003d: Unknown result type (might be due to invalid IL or missing references)
					AddCells(prefab, button3, new int2(horizontal ? (-1) : 0, (!horizontal) ? (-1) : 0), -1);
				};
				button3.disabled = () => (!horizontal) ? (prefab.m_LotDepth <= kMinSize) : (prefab.m_LotWidth <= kMinSize);
				Button button4 = new Button
				{
					displayName = (horizontal ? "Shrink Right" : "Shrink Back")
				};
				button4.action = delegate
				{
					//IL_003d: Unknown result type (might be due to invalid IL or missing references)
					AddCells(prefab, button4, new int2(horizontal ? 1 : 0, (!horizontal) ? 1 : 0), -1);
				};
				button4.disabled = () => (!horizontal) ? (prefab.m_LotDepth <= kMinSize) : (prefab.m_LotWidth <= kMinSize);
				return new Column
				{
					children = new IWidget[3]
					{
						intInputField,
						new ButtonRow
						{
							children = new Button[2] { button1, button3 }
						},
						new ButtonRow
						{
							children = new Button[2] { button2, button4 }
						}
					}
				};
			}
			return intInputField;
		};
	}

	private static void AddCells(BuildingPrefab prefab, IWidget widget, int2 dir, int count = 1)
	{
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		//IL_007d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0089: Unknown result type (might be due to invalid IL or missing references)
		//IL_009f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ed: Unknown result type (might be due to invalid IL or missing references)
		//IL_0107: Unknown result type (might be due to invalid IL or missing references)
		//IL_010c: Unknown result type (might be due to invalid IL or missing references)
		//IL_010d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0112: Unknown result type (might be due to invalid IL or missing references)
		//IL_0157: Unknown result type (might be due to invalid IL or missing references)
		//IL_015c: Unknown result type (might be due to invalid IL or missing references)
		//IL_015d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0162: Unknown result type (might be due to invalid IL or missing references)
		//IL_0208: Unknown result type (might be due to invalid IL or missing references)
		//IL_0217: Unknown result type (might be due to invalid IL or missing references)
		//IL_021c: Unknown result type (might be due to invalid IL or missing references)
		//IL_021d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0222: Unknown result type (might be due to invalid IL or missing references)
		//IL_0230: Unknown result type (might be due to invalid IL or missing references)
		//IL_0235: Unknown result type (might be due to invalid IL or missing references)
		//IL_0236: Unknown result type (might be due to invalid IL or missing references)
		//IL_023b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0249: Unknown result type (might be due to invalid IL or missing references)
		//IL_024e: Unknown result type (might be due to invalid IL or missing references)
		//IL_024f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0254: Unknown result type (might be due to invalid IL or missing references)
		//IL_0262: Unknown result type (might be due to invalid IL or missing references)
		//IL_0267: Unknown result type (might be due to invalid IL or missing references)
		//IL_0268: Unknown result type (might be due to invalid IL or missing references)
		//IL_026d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0272: Unknown result type (might be due to invalid IL or missing references)
		//IL_0274: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_02cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_02de: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e4: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f7: Unknown result type (might be due to invalid IL or missing references)
		//IL_02fc: Unknown result type (might be due to invalid IL or missing references)
		//IL_02fd: Unknown result type (might be due to invalid IL or missing references)
		//IL_0302: Unknown result type (might be due to invalid IL or missing references)
		//IL_0310: Unknown result type (might be due to invalid IL or missing references)
		//IL_0315: Unknown result type (might be due to invalid IL or missing references)
		//IL_0316: Unknown result type (might be due to invalid IL or missing references)
		//IL_031b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0320: Unknown result type (might be due to invalid IL or missing references)
		//IL_0322: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_0360: Unknown result type (might be due to invalid IL or missing references)
		//IL_0365: Unknown result type (might be due to invalid IL or missing references)
		//IL_0366: Unknown result type (might be due to invalid IL or missing references)
		//IL_036b: Unknown result type (might be due to invalid IL or missing references)
		int2 val = default(int2);
		((int2)(ref val))._002Ector(prefab.m_LotWidth, prefab.m_LotDepth);
		int2 val2 = default(int2);
		((int2)(ref val2))._002Ector(val.x + math.abs(dir.x) * count, val.y + math.abs(dir.y) * count);
		float2 val3 = 4f * float2.op_Implicit(dir) * float2.op_Implicit(val2 - val);
		float3 val4 = default(float3);
		((float3)(ref val4))._002Ector(val3.x, 0f, val3.y);
		prefab.m_LotWidth = val2.x;
		prefab.m_LotDepth = val2.y;
		Entity entity = World.DefaultGameObjectInjectionWorld.GetOrCreateSystemManaged<PrefabSystem>().GetEntity(prefab);
		EntityManager entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
		ObjectMeshInfo[] meshes = prefab.m_Meshes;
		foreach (ObjectMeshInfo obj in meshes)
		{
			obj.m_Position += val4;
		}
		DynamicBuffer<SubMesh> buffer = ((EntityManager)(ref entityManager)).GetBuffer<SubMesh>(entity, false);
		for (int j = 0; j < buffer.Length; j++)
		{
			SubMesh subMesh = buffer[j];
			ref float3 position = ref subMesh.m_Position;
			position += val4;
			buffer[j] = subMesh;
		}
		if (prefab.TryGet<ObjectSubObjects>(out var component))
		{
			for (int num = component.m_SubObjects.Length - 1; num >= 0; num--)
			{
				ObjectSubObjectInfo obj2 = component.m_SubObjects[num];
				obj2.m_Position += val4;
			}
		}
		if (prefab.TryGet<ObjectSubAreas>(out var component2))
		{
			for (int num2 = component2.m_SubAreas.Length - 1; num2 >= 0; num2--)
			{
				ObjectSubAreaInfo objectSubAreaInfo = component2.m_SubAreas[num2];
				for (int k = 0; k < objectSubAreaInfo.m_NodePositions.Length; k++)
				{
					ref float3 reference = ref objectSubAreaInfo.m_NodePositions[k];
					reference += val4;
				}
			}
		}
		if (prefab.TryGet<ObjectSubLanes>(out var component3))
		{
			for (int num3 = component3.m_SubLanes.Length - 1; num3 >= 0; num3--)
			{
				ObjectSubLaneInfo objectSubLaneInfo = component3.m_SubLanes[num3];
				objectSubLaneInfo.m_BezierCurve = new Bezier4x3
				{
					a = objectSubLaneInfo.m_BezierCurve.a + val4,
					b = objectSubLaneInfo.m_BezierCurve.b + val4,
					c = objectSubLaneInfo.m_BezierCurve.c + val4,
					d = objectSubLaneInfo.m_BezierCurve.d + val4
				};
			}
		}
		if (prefab.TryGet<ObjectSubNets>(out var component4))
		{
			for (int num4 = component4.m_SubNets.Length - 1; num4 >= 0; num4--)
			{
				ObjectSubNetInfo objectSubNetInfo = component4.m_SubNets[num4];
				objectSubNetInfo.m_BezierCurve = new Bezier4x3
				{
					a = objectSubNetInfo.m_BezierCurve.a + val4,
					b = objectSubNetInfo.m_BezierCurve.b + val4,
					c = objectSubNetInfo.m_BezierCurve.c + val4,
					d = objectSubNetInfo.m_BezierCurve.d + val4
				};
			}
		}
		if (prefab.TryGet<EffectSource>(out var component5))
		{
			for (int num5 = component5.m_Effects.Count - 1; num5 >= 0; num5--)
			{
				EffectSource.EffectSettings effectSettings = component5.m_Effects[num5];
				effectSettings.m_PositionOffset += val4;
			}
		}
		World.DefaultGameObjectInjectionWorld.GetExistingSystemManaged<EditorPanelUISystem>()?.OnValueChanged(widget);
	}

	private static bool TryGetBuildingPrefab(IValueAccessor accessor, out BuildingPrefab prefab)
	{
		if (accessor is FieldAccessor { parent: ObjectAccessor<object> parent })
		{
			prefab = parent.GetValue() as BuildingPrefab;
			return (Object)(object)prefab != (Object)null;
		}
		prefab = null;
		return false;
	}
}

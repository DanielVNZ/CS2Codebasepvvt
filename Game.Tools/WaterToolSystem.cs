using System.Runtime.CompilerServices;
using Colossal.Entities;
using Colossal.Mathematics;
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
using UnityEngine.Scripting;

namespace Game.Tools;

[CompilerGenerated]
public class WaterToolSystem : ToolBaseSystem
{
	public enum Attribute
	{
		None,
		Location,
		Radius,
		Rate,
		Height
	}

	private enum State
	{
		Default,
		MouseDown,
		Dragging
	}

	[BurstCompile]
	private struct CreateDefinitionsJob : IJob
	{
		[ReadOnly]
		public ControlPoint m_StartPoint;

		[ReadOnly]
		public ControlPoint m_RaycastPoint;

		[ReadOnly]
		public State m_State;

		[ReadOnly]
		public Attribute m_Attribute;

		[ReadOnly]
		public ComponentLookup<Game.Simulation.WaterSourceData> m_WaterSourceData;

		[ReadOnly]
		public float3 m_PositionOffset;

		public EntityCommandBuffer m_CommandBuffer;

		public void Execute()
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			//IL_0005: Unknown result type (might be due to invalid IL or missing references)
			//IL_0008: Unknown result type (might be due to invalid IL or missing references)
			//IL_002f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0034: Unknown result type (might be due to invalid IL or missing references)
			//IL_003b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0040: Unknown result type (might be due to invalid IL or missing references)
			//IL_0049: Unknown result type (might be due to invalid IL or missing references)
			//IL_004e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0055: Unknown result type (might be due to invalid IL or missing references)
			//IL_005a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0063: Unknown result type (might be due to invalid IL or missing references)
			//IL_0068: Unknown result type (might be due to invalid IL or missing references)
			//IL_006f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0074: Unknown result type (might be due to invalid IL or missing references)
			//IL_007b: Unknown result type (might be due to invalid IL or missing references)
			//IL_008e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0093: Unknown result type (might be due to invalid IL or missing references)
			//IL_009f: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
			//IL_01dc: Unknown result type (might be due to invalid IL or missing references)
			//IL_01dd: Unknown result type (might be due to invalid IL or missing references)
			//IL_0229: Unknown result type (might be due to invalid IL or missing references)
			//IL_0238: Unknown result type (might be due to invalid IL or missing references)
			//IL_0247: Unknown result type (might be due to invalid IL or missing references)
			//IL_0146: Unknown result type (might be due to invalid IL or missing references)
			//IL_0156: Unknown result type (might be due to invalid IL or missing references)
			Entity val = Entity.Null;
			float3 position = default(float3);
			switch (m_State)
			{
			case State.Default:
				val = m_RaycastPoint.m_OriginalEntity;
				position = m_RaycastPoint.m_Position;
				break;
			case State.MouseDown:
				val = m_StartPoint.m_OriginalEntity;
				position = m_StartPoint.m_Position;
				break;
			case State.Dragging:
				val = m_StartPoint.m_OriginalEntity;
				position = m_RaycastPoint.m_Position;
				break;
			}
			Game.Simulation.WaterSourceData waterSourceData = default(Game.Simulation.WaterSourceData);
			if (!m_WaterSourceData.TryGetComponent(val, ref waterSourceData))
			{
				return;
			}
			Entity val2 = ((EntityCommandBuffer)(ref m_CommandBuffer)).CreateEntity();
			CreationDefinition creationDefinition = new CreationDefinition
			{
				m_Original = val
			};
			creationDefinition.m_Flags |= CreationFlags.Select;
			if (m_State == State.Dragging)
			{
				creationDefinition.m_Flags |= CreationFlags.Dragging;
				switch (m_Attribute)
				{
				case Attribute.Location:
					if (waterSourceData.m_ConstantDepth == 1 || waterSourceData.m_ConstantDepth == 2)
					{
						waterSourceData.m_Amount += m_RaycastPoint.m_Position.y - m_StartPoint.m_Position.y;
					}
					break;
				case Attribute.Radius:
					waterSourceData.m_Radius = math.clamp(math.distance(((float3)(ref m_RaycastPoint.m_HitPosition)).xz, ((float3)(ref m_StartPoint.m_Position)).xz), 1f, 20000f);
					break;
				case Attribute.Rate:
					waterSourceData.m_Amount = math.clamp(m_RaycastPoint.m_HitPosition.y - m_StartPoint.m_Position.y, 1f, 1000f);
					break;
				case Attribute.Height:
					waterSourceData.m_Amount = m_RaycastPoint.m_HitPosition.y - m_PositionOffset.y;
					break;
				}
			}
			WaterSourceDefinition waterSourceDefinition = new WaterSourceDefinition
			{
				m_Position = position,
				m_ConstantDepth = waterSourceData.m_ConstantDepth,
				m_Amount = waterSourceData.m_Amount,
				m_Radius = waterSourceData.m_Radius,
				m_Multiplier = waterSourceData.m_Multiplier,
				m_Polluted = waterSourceData.m_Polluted
			};
			((EntityCommandBuffer)(ref m_CommandBuffer)).AddComponent<CreationDefinition>(val2, creationDefinition);
			((EntityCommandBuffer)(ref m_CommandBuffer)).AddComponent<WaterSourceDefinition>(val2, waterSourceDefinition);
			((EntityCommandBuffer)(ref m_CommandBuffer)).AddComponent<Updated>(val2, default(Updated));
		}
	}

	private struct TypeHandle
	{
		[ReadOnly]
		public ComponentLookup<Game.Simulation.WaterSourceData> __Game_Simulation_WaterSourceData_RO_ComponentLookup;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void __AssignHandles(ref SystemState state)
		{
			//IL_0003: Unknown result type (might be due to invalid IL or missing references)
			//IL_0008: Unknown result type (might be due to invalid IL or missing references)
			__Game_Simulation_WaterSourceData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Game.Simulation.WaterSourceData>(true);
		}
	}

	public const string kToolID = "Water Tool";

	private TerrainSystem m_TerrainSystem;

	private WaterSystem m_WaterSystem;

	private CameraUpdateSystem m_CameraUpdateSystem;

	private ToolOutputBarrier m_ToolOutputBarrier;

	private EntityQuery m_DefinitionQuery;

	private ControlPoint m_RaycastPoint;

	private ControlPoint m_StartPoint;

	private State m_State;

	private TypeHandle __TypeHandle;

	public override string toolID => "Water Tool";

	public Attribute attribute { get; private set; }

	[Preserve]
	protected override void OnCreate()
	{
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		base.OnCreate();
		m_TerrainSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<TerrainSystem>();
		m_WaterSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<WaterSystem>();
		m_CameraUpdateSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<CameraUpdateSystem>();
		m_ToolOutputBarrier = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<ToolOutputBarrier>();
		m_DefinitionQuery = GetDefinitionQuery();
	}

	[Preserve]
	protected override void OnStartRunning()
	{
		base.OnStartRunning();
		m_RaycastPoint = default(ControlPoint);
		m_State = State.Default;
		attribute = Attribute.None;
	}

	private protected override void UpdateActions()
	{
		base.applyAction.enabled = base.actionsEnabled;
		base.secondaryApplyAction.enabled = base.actionsEnabled;
	}

	public override PrefabBase GetPrefab()
	{
		return null;
	}

	public override bool TrySetPrefab(PrefabBase prefab)
	{
		return false;
	}

	public override void InitializeRaycast()
	{
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_007f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0091: Unknown result type (might be due to invalid IL or missing references)
		base.InitializeRaycast();
		if (m_State == State.Dragging)
		{
			if (attribute != Attribute.Location)
			{
				return;
			}
			m_ToolRaycastSystem.typeMask = TypeMask.Terrain;
			m_ToolRaycastSystem.raycastFlags |= RaycastFlags.Outside;
			Game.Simulation.WaterSourceData waterSourceData = default(Game.Simulation.WaterSourceData);
			if (EntitiesExtensions.TryGetComponent<Game.Simulation.WaterSourceData>(((ComponentSystemBase)this).EntityManager, m_StartPoint.m_OriginalEntity, ref waterSourceData))
			{
				float num = waterSourceData.m_Amount;
				if (waterSourceData.m_ConstantDepth > 0)
				{
					TerrainHeightData data = m_TerrainSystem.GetHeightData();
					num += m_TerrainSystem.positionOffset.y - TerrainUtils.SampleHeight(ref data, m_StartPoint.m_Position);
				}
				m_ToolRaycastSystem.rayOffset = new float3(0f, 0f - num, 0f);
			}
		}
		else
		{
			m_ToolRaycastSystem.typeMask = TypeMask.WaterSources;
		}
	}

	[Preserve]
	protected override JobHandle OnUpdate(JobHandle inputDeps)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_0093: Unknown result type (might be due to invalid IL or missing references)
		//IL_009f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_0075: Unknown result type (might be due to invalid IL or missing references)
		//IL_0077: Unknown result type (might be due to invalid IL or missing references)
		//IL_007e: Unknown result type (might be due to invalid IL or missing references)
		//IL_007f: Unknown result type (might be due to invalid IL or missing references)
		UpdateInfoview(Entity.Null);
		GetAvailableSnapMask(out m_SnapOnMask, out m_SnapOffMask);
		if ((m_ToolRaycastSystem.raycastFlags & (RaycastFlags.DebugDisable | RaycastFlags.UIDisable)) == 0)
		{
			if (m_State != State.Default)
			{
				if (base.applyAction.WasPressedThisFrame() || base.applyAction.WasReleasedThisFrame())
				{
					return Apply(inputDeps);
				}
				if (base.secondaryApplyAction.WasPressedThisFrame() || base.secondaryApplyAction.WasReleasedThisFrame())
				{
					return Cancel(inputDeps);
				}
				return Update(inputDeps);
			}
			if (base.secondaryApplyAction.WasPressedThisFrame())
			{
				return Cancel(inputDeps, base.secondaryApplyAction.WasReleasedThisFrame());
			}
			if (base.applyAction.WasPressedThisFrame())
			{
				return Apply(inputDeps, base.applyAction.WasReleasedThisFrame());
			}
			return Update(inputDeps);
		}
		return Clear(inputDeps);
	}

	public override void GetAvailableSnapMask(out Snap onMask, out Snap offMask)
	{
		base.GetAvailableSnapMask(out onMask, out offMask);
		onMask |= Snap.ContourLines;
		offMask |= Snap.ContourLines;
	}

	private JobHandle Clear(JobHandle inputDeps)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		base.applyMode = ApplyMode.Clear;
		return inputDeps;
	}

	private JobHandle Cancel(JobHandle inputDeps, bool singleFrameOnly = false)
	{
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		switch (m_State)
		{
		case State.Default:
			base.applyMode = ApplyMode.None;
			return inputDeps;
		case State.MouseDown:
			m_State = State.Default;
			base.applyMode = ApplyMode.Clear;
			return inputDeps;
		case State.Dragging:
			m_State = State.Default;
			base.applyMode = ApplyMode.Clear;
			return inputDeps;
		default:
			return Update(inputDeps);
		}
	}

	private JobHandle Apply(JobHandle inputDeps, bool singleFrameOnly = false)
	{
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_007d: Unknown result type (might be due to invalid IL or missing references)
		//IL_007e: Unknown result type (might be due to invalid IL or missing references)
		//IL_007a: Unknown result type (might be due to invalid IL or missing references)
		switch (m_State)
		{
		case State.Default:
			if (m_RaycastPoint.m_OriginalEntity != Entity.Null && !singleFrameOnly)
			{
				m_State = State.MouseDown;
				m_StartPoint = m_RaycastPoint;
			}
			base.applyMode = ApplyMode.None;
			return inputDeps;
		case State.MouseDown:
			m_State = State.Default;
			base.applyMode = ApplyMode.Clear;
			return inputDeps;
		case State.Dragging:
			m_State = State.Default;
			base.applyMode = (GetAllowApply() ? ApplyMode.Apply : ApplyMode.Clear);
			return inputDeps;
		default:
			return Update(inputDeps);
		}
	}

	private JobHandle Update(JobHandle inputDeps)
	{
		//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_007d: Unknown result type (might be due to invalid IL or missing references)
		//IL_007e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0083: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_0085: Unknown result type (might be due to invalid IL or missing references)
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		if (GetRaycastResult(out var controlPoint))
		{
			if (m_RaycastPoint.Equals(controlPoint))
			{
				base.applyMode = ApplyMode.None;
				return inputDeps;
			}
			if (m_State == State.Default)
			{
				attribute = GetAttribute(controlPoint);
			}
			base.applyMode = ApplyMode.Clear;
			m_RaycastPoint = controlPoint;
			if (m_State == State.MouseDown && math.distance(controlPoint.m_HitPosition, m_StartPoint.m_HitPosition) >= 1f)
			{
				inputDeps = UpdateDefinitions(inputDeps);
				m_State = State.Dragging;
			}
			else
			{
				inputDeps = UpdateDefinitions(inputDeps);
			}
			return inputDeps;
		}
		if (m_RaycastPoint.Equals(default(ControlPoint)))
		{
			base.applyMode = ApplyMode.None;
			return inputDeps;
		}
		base.applyMode = ApplyMode.Clear;
		m_RaycastPoint = default(ControlPoint);
		if (m_State == State.MouseDown)
		{
			inputDeps = UpdateDefinitions(inputDeps);
			m_State = State.Dragging;
		}
		else
		{
			if (m_State == State.Default)
			{
				attribute = Attribute.None;
			}
			inputDeps = UpdateDefinitions(inputDeps);
		}
		return inputDeps;
	}

	private Attribute GetAttribute(ControlPoint controlPoint)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0070: Unknown result type (might be due to invalid IL or missing references)
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		//IL_007c: Unknown result type (might be due to invalid IL or missing references)
		//IL_007d: Unknown result type (might be due to invalid IL or missing references)
		Game.Simulation.WaterSourceData waterSourceData = default(Game.Simulation.WaterSourceData);
		if (EntitiesExtensions.TryGetComponent<Game.Simulation.WaterSourceData>(((ComponentSystemBase)this).EntityManager, controlPoint.m_OriginalEntity, ref waterSourceData) && m_CameraUpdateSystem.TryGetViewer(out var viewer))
		{
			float2 val = ((float3)(ref controlPoint.m_HitPosition)).xz - ((float3)(ref controlPoint.m_Position)).xz;
			if (math.length(val) < waterSourceData.m_Radius * 0.9f)
			{
				return Attribute.Location;
			}
			float3 right = viewer.right;
			float2 xz = ((float3)(ref right)).xz;
			float2 val2 = MathUtils.Left(xz);
			if (math.abs(math.dot(xz, val)) > math.abs(math.dot(val2, val)))
			{
				return Attribute.Radius;
			}
			if (waterSourceData.m_ConstantDepth != 0)
			{
				return Attribute.Height;
			}
			return Attribute.Rate;
		}
		return Attribute.None;
	}

	protected override bool GetRaycastResult(out ControlPoint controlPoint)
	{
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		//IL_010b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0110: Unknown result type (might be due to invalid IL or missing references)
		//IL_0117: Unknown result type (might be due to invalid IL or missing references)
		//IL_0086: Unknown result type (might be due to invalid IL or missing references)
		//IL_008b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0125: Unknown result type (might be due to invalid IL or missing references)
		//IL_0126: Unknown result type (might be due to invalid IL or missing references)
		//IL_012d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0132: Unknown result type (might be due to invalid IL or missing references)
		//IL_0134: Unknown result type (might be due to invalid IL or missing references)
		//IL_0135: Unknown result type (might be due to invalid IL or missing references)
		//IL_013c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0141: Unknown result type (might be due to invalid IL or missing references)
		//IL_0145: Unknown result type (might be due to invalid IL or missing references)
		//IL_0155: Unknown result type (might be due to invalid IL or missing references)
		//IL_0161: Unknown result type (might be due to invalid IL or missing references)
		//IL_0171: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_009e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0188: Unknown result type (might be due to invalid IL or missing references)
		//IL_018a: Unknown result type (might be due to invalid IL or missing references)
		//IL_017e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0180: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00eb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f0: Unknown result type (might be due to invalid IL or missing references)
		Game.Simulation.WaterSourceData waterSourceData = default(Game.Simulation.WaterSourceData);
		if (m_State == State.Dragging && attribute != Attribute.None && attribute != Attribute.Location && EntitiesExtensions.TryGetComponent<Game.Simulation.WaterSourceData>(((ComponentSystemBase)this).EntityManager, m_StartPoint.m_OriginalEntity, ref waterSourceData) && m_CameraUpdateSystem.TryGetViewer(out var viewer))
		{
			TerrainHeightData data = m_TerrainSystem.GetHeightData();
			Segment val = ToolRaycastSystem.CalculateRaycastLine(viewer.camera);
			controlPoint = m_StartPoint;
			float2 val2 = default(float2);
			if (attribute == Attribute.Radius)
			{
				float3 position = m_StartPoint.m_Position;
				if (waterSourceData.m_ConstantDepth > 0)
				{
					position.y = m_TerrainSystem.positionOffset.y + waterSourceData.m_Amount;
				}
				else
				{
					position.y = TerrainUtils.SampleHeight(ref data, position) + waterSourceData.m_Amount;
				}
				float num = default(float);
				if (MathUtils.Intersect(((Segment)(ref val)).y, position.y, ref num))
				{
					controlPoint.m_HitPosition = MathUtils.Position(val, num);
				}
			}
			else if (MathUtils.Intersect(new Circle2(waterSourceData.m_Radius, ((float3)(ref m_StartPoint.m_Position)).xz), ((Segment)(ref val)).xz, ref val2))
			{
				float3 hitPosition = MathUtils.Position(val, val2.x);
				float3 hitPosition2 = MathUtils.Position(val, val2.y);
				if (math.distancesq(((float3)(ref hitPosition)).xz, ((float3)(ref m_StartPoint.m_HitPosition)).xz) <= math.distancesq(((float3)(ref hitPosition2)).xz, ((float3)(ref m_StartPoint.m_HitPosition)).xz))
				{
					controlPoint.m_HitPosition = hitPosition;
				}
				else
				{
					controlPoint.m_HitPosition = hitPosition2;
				}
			}
			return true;
		}
		return base.GetRaycastResult(out controlPoint);
	}

	private JobHandle UpdateDefinitions(JobHandle inputDeps)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_007d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0082: Unknown result type (might be due to invalid IL or missing references)
		//IL_008f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0094: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c6: Unknown result type (might be due to invalid IL or missing references)
		JobHandle val = DestroyDefinitions(m_DefinitionQuery, m_ToolOutputBarrier, inputDeps);
		if (m_RaycastPoint.m_OriginalEntity != Entity.Null)
		{
			JobHandle val2 = IJobExtensions.Schedule<CreateDefinitionsJob>(new CreateDefinitionsJob
			{
				m_StartPoint = m_StartPoint,
				m_RaycastPoint = m_RaycastPoint,
				m_State = m_State,
				m_Attribute = attribute,
				m_WaterSourceData = InternalCompilerInterface.GetComponentLookup<Game.Simulation.WaterSourceData>(ref __TypeHandle.__Game_Simulation_WaterSourceData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_PositionOffset = m_TerrainSystem.positionOffset,
				m_CommandBuffer = m_ToolOutputBarrier.CreateCommandBuffer()
			}, inputDeps);
			((EntityCommandBufferSystem)m_ToolOutputBarrier).AddJobHandleForProducer(val2);
			val = JobHandle.CombineDependencies(val, val2);
		}
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
		base.OnCreateForCompiler();
		__AssignQueries(ref ((SystemBase)this).CheckedStateRef);
		__TypeHandle.__AssignHandles(ref ((SystemBase)this).CheckedStateRef);
	}

	[Preserve]
	public WaterToolSystem()
	{
	}
}

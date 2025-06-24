using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Colossal.Mathematics;
using Colossal.Serialization.Entities;
using Game.Audio;
using Game.Common;
using Game.Input;
using Game.Net;
using Game.Prefabs;
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
public class TerrainToolSystem : ToolBaseSystem
{
	private enum State
	{
		Default,
		Adding,
		Removing
	}

	[BurstCompile]
	private struct CreateDefinitionsJob : IJob
	{
		[ReadOnly]
		public Entity m_Prefab;

		[ReadOnly]
		public Entity m_Brush;

		[ReadOnly]
		public float m_Size;

		[ReadOnly]
		public float m_Angle;

		[ReadOnly]
		public float m_Strength;

		[ReadOnly]
		public float m_Time;

		[ReadOnly]
		public float3 m_Target;

		[ReadOnly]
		public float3 m_ApplyStart;

		[ReadOnly]
		public ControlPoint m_StartPoint;

		[ReadOnly]
		public ControlPoint m_EndPoint;

		public EntityCommandBuffer m_CommandBuffer;

		public void Execute()
		{
			//IL_0022: Unknown result type (might be due to invalid IL or missing references)
			//IL_0027: Unknown result type (might be due to invalid IL or missing references)
			//IL_0037: Unknown result type (might be due to invalid IL or missing references)
			//IL_003c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0083: Unknown result type (might be due to invalid IL or missing references)
			//IL_008e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0093: Unknown result type (might be due to invalid IL or missing references)
			//IL_0098: Unknown result type (might be due to invalid IL or missing references)
			//IL_005f: Unknown result type (might be due to invalid IL or missing references)
			//IL_006a: Unknown result type (might be due to invalid IL or missing references)
			//IL_006f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0074: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d4: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d9: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e6: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f6: Unknown result type (might be due to invalid IL or missing references)
			//IL_00fd: Unknown result type (might be due to invalid IL or missing references)
			//IL_010a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0117: Unknown result type (might be due to invalid IL or missing references)
			if (!m_EndPoint.Equals(default(ControlPoint)))
			{
				CreationDefinition creationDefinition = new CreationDefinition
				{
					m_Prefab = m_Brush
				};
				BrushDefinition brushDefinition = new BrushDefinition
				{
					m_Tool = m_Prefab
				};
				if (m_StartPoint.Equals(default(ControlPoint)))
				{
					brushDefinition.m_Line = new Segment(m_EndPoint.m_Position, m_EndPoint.m_Position);
				}
				else
				{
					brushDefinition.m_Line = new Segment(m_StartPoint.m_Position, m_EndPoint.m_Position);
				}
				brushDefinition.m_Size = m_Size;
				brushDefinition.m_Angle = m_Angle;
				brushDefinition.m_Strength = m_Strength;
				brushDefinition.m_Time = m_Time;
				brushDefinition.m_Target = m_Target;
				brushDefinition.m_Start = m_ApplyStart;
				Entity val = ((EntityCommandBuffer)(ref m_CommandBuffer)).CreateEntity();
				((EntityCommandBuffer)(ref m_CommandBuffer)).AddComponent<CreationDefinition>(val, creationDefinition);
				((EntityCommandBuffer)(ref m_CommandBuffer)).AddComponent<BrushDefinition>(val, brushDefinition);
				((EntityCommandBuffer)(ref m_CommandBuffer)).AddComponent<Updated>(val, default(Updated));
			}
		}
	}

	private struct TypeHandle
	{
		[ReadOnly]
		public ComponentTypeHandle<Brush> __Game_Tools_Brush_RO_ComponentTypeHandle;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void __AssignHandles(ref SystemState state)
		{
			//IL_0003: Unknown result type (might be due to invalid IL or missing references)
			//IL_0008: Unknown result type (might be due to invalid IL or missing references)
			__Game_Tools_Brush_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<Brush>(true);
		}
	}

	public const string kToolID = "Terrain Tool";

	public const string kTerrainToolKeyGroup = "tool/terrain";

	private AudioManager m_AudioManager;

	private AudioSource m_AudioSource;

	private ToolOutputBarrier m_ToolOutputBarrier;

	private EntityQuery m_DefinitionQuery;

	private EntityQuery m_BrushQuery;

	private EntityQuery m_TempQuery;

	private EntityQuery m_SoundQuery;

	private EntityQuery m_VisibleQuery;

	private IProxyAction m_EraseMaterial;

	private IProxyAction m_EraseResource;

	private IProxyAction m_FastSoften;

	private IProxyAction m_LevelTerrain;

	private IProxyAction m_LowerTerrain;

	private IProxyAction m_PaintMaterial;

	private IProxyAction m_PaintResource;

	private IProxyAction m_RaiseTerrain;

	private IProxyAction m_SetLevelTarget;

	private IProxyAction m_SetSlopeTarget;

	private IProxyAction m_SlopeTerrain;

	private IProxyAction m_SoftenTerrain;

	private ControlPoint m_RaycastPoint;

	private ControlPoint m_StartPoint;

	private float3 m_TargetPosition;

	private float3 m_ApplyPosition;

	private bool m_TargetSet;

	private State m_State;

	private TypeHandle __TypeHandle;

	public override string toolID => "Terrain Tool";

	public TerraformingPrefab prefab { get; private set; }

	public override bool brushing => true;

	private protected override IEnumerable<IProxyAction> toolActions
	{
		get
		{
			yield return m_EraseMaterial;
			yield return m_EraseResource;
			yield return m_FastSoften;
			yield return m_LevelTerrain;
			yield return m_LowerTerrain;
			yield return m_PaintMaterial;
			yield return m_PaintResource;
			yield return m_RaiseTerrain;
			yield return m_SetLevelTarget;
			yield return m_SetSlopeTarget;
			yield return m_SlopeTerrain;
			yield return m_SoftenTerrain;
		}
	}

	public float brushHeight
	{
		get
		{
			if (!m_TargetSet)
			{
				return WaterSystem.SeaLevel;
			}
			return m_TargetPosition.y;
		}
		set
		{
			m_TargetPosition.y = value;
			m_TargetSet = true;
		}
	}

	public void SetPrefab(TerraformingPrefab value)
	{
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		m_TargetSet = false;
		m_TargetPosition = new float3(0f, 0f, 0f);
		m_ApplyPosition = new float3(0f, 0f, 0f);
		prefab = value;
		if (((ComponentSystemBase)this).Enabled)
		{
			UpdateActions();
		}
	}

	[Preserve]
	protected override void OnCreate()
	{
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		//IL_0085: Unknown result type (might be due to invalid IL or missing references)
		//IL_008c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0091: Unknown result type (might be due to invalid IL or missing references)
		//IL_0096: Unknown result type (might be due to invalid IL or missing references)
		//IL_009b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_00af: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b9: Unknown result type (might be due to invalid IL or missing references)
		base.OnCreate();
		m_AudioManager = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<AudioManager>();
		m_ToolOutputBarrier = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<ToolOutputBarrier>();
		m_DefinitionQuery = GetDefinitionQuery();
		m_BrushQuery = GetBrushQuery();
		m_VisibleQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[3]
		{
			ComponentType.ReadOnly<Brush>(),
			ComponentType.Exclude<Hidden>(),
			ComponentType.Exclude<Deleted>()
		});
		m_TempQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[2]
		{
			ComponentType.ReadOnly<Brush>(),
			ComponentType.ReadOnly<Temp>()
		});
		m_SoundQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[1] { ComponentType.ReadOnly<ToolUXSoundSettingsData>() });
		m_EraseMaterial = InputManager.instance.toolActionCollection.GetActionState("Erase Material", "TerrainToolSystem");
		m_EraseResource = InputManager.instance.toolActionCollection.GetActionState("Erase Resource", "TerrainToolSystem");
		m_FastSoften = InputManager.instance.toolActionCollection.GetActionState("Fast Soften", "TerrainToolSystem");
		m_LevelTerrain = InputManager.instance.toolActionCollection.GetActionState("Level Terrain", "TerrainToolSystem");
		m_LowerTerrain = InputManager.instance.toolActionCollection.GetActionState("Lower Terrain", "TerrainToolSystem");
		m_PaintMaterial = InputManager.instance.toolActionCollection.GetActionState("Paint Material", "TerrainToolSystem");
		m_PaintResource = InputManager.instance.toolActionCollection.GetActionState("Paint Resource", "TerrainToolSystem");
		m_RaiseTerrain = InputManager.instance.toolActionCollection.GetActionState("Raise Terrain", "TerrainToolSystem");
		m_SetLevelTarget = InputManager.instance.toolActionCollection.GetActionState("Set Level Target", "TerrainToolSystem");
		m_SetSlopeTarget = InputManager.instance.toolActionCollection.GetActionState("Set Slope Target", "TerrainToolSystem");
		m_SlopeTerrain = InputManager.instance.toolActionCollection.GetActionState("Slope Terrain", "TerrainToolSystem");
		m_SoftenTerrain = InputManager.instance.toolActionCollection.GetActionState("Soften Terrain", "TerrainToolSystem");
		base.brushSize = 100f;
		base.brushAngle = 0f;
		base.brushStrength = 0.5f;
	}

	protected override void OnGameLoaded(Context serializationContext)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		base.OnGameLoaded(serializationContext);
		base.brushType = FindDefaultBrush(m_BrushQuery);
		base.brushSize = 100f;
		base.brushAngle = 0f;
		base.brushStrength = 0.5f;
	}

	public void SetDisableFX()
	{
		if ((Object)(object)m_AudioSource != (Object)null)
		{
			m_AudioManager.StopExclusiveUISound(m_AudioSource);
			m_AudioSource = null;
		}
	}

	[Preserve]
	protected override void OnStartRunning()
	{
		base.OnStartRunning();
		m_RaycastPoint = default(ControlPoint);
		m_StartPoint = default(ControlPoint);
		m_State = State.Default;
	}

	private protected override void UpdateActions()
	{
		using (ProxyAction.DeferStateUpdating())
		{
			base.applyAction.enabled = base.actionsEnabled;
			base.secondaryApplyAction.enabled = base.actionsEnabled;
			if (prefab.m_Type == TerraformingType.Shift)
			{
				if (prefab.m_Target == TerraformingTarget.Height)
				{
					base.applyActionOverride = m_RaiseTerrain;
					base.secondaryApplyActionOverride = m_LowerTerrain;
				}
				else if (prefab.m_Target == TerraformingTarget.Material)
				{
					base.applyActionOverride = m_PaintMaterial;
					base.secondaryApplyActionOverride = m_EraseMaterial;
				}
				else
				{
					base.applyActionOverride = m_PaintResource;
					base.secondaryApplyActionOverride = m_EraseResource;
				}
			}
			else if (prefab.m_Type == TerraformingType.Level)
			{
				base.applyActionOverride = m_LevelTerrain;
				base.secondaryApplyActionOverride = m_SetLevelTarget;
			}
			else if (prefab.m_Type == TerraformingType.Slope)
			{
				base.applyActionOverride = m_SlopeTerrain;
				base.secondaryApplyActionOverride = m_SetSlopeTarget;
			}
			else if (prefab.m_Type == TerraformingType.Soften)
			{
				base.applyActionOverride = m_SoftenTerrain;
				base.secondaryApplyActionOverride = m_FastSoften;
			}
			else
			{
				base.applyActionOverride = null;
				base.secondaryApplyActionOverride = null;
			}
		}
	}

	public override PrefabBase GetPrefab()
	{
		return prefab;
	}

	public override bool TrySetPrefab(PrefabBase prefab)
	{
		if (prefab is TerraformingPrefab terraformingPrefab)
		{
			SetPrefab(terraformingPrefab);
			return true;
		}
		return false;
	}

	public override void InitializeRaycast()
	{
		base.InitializeRaycast();
		if ((Object)(object)prefab != (Object)null && (Object)(object)base.brushType != (Object)null)
		{
			m_ToolRaycastSystem.typeMask = TypeMask.Terrain;
			m_ToolRaycastSystem.raycastFlags |= RaycastFlags.Outside;
		}
		else
		{
			m_ToolRaycastSystem.typeMask = TypeMask.None;
		}
	}

	[Preserve]
	protected override JobHandle OnUpdate(JobHandle inputDeps)
	{
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_015f: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_0076: Unknown result type (might be due to invalid IL or missing references)
		//IL_0124: Unknown result type (might be due to invalid IL or missing references)
		//IL_0130: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e5: Unknown result type (might be due to invalid IL or missing references)
		//IL_0157: Unknown result type (might be due to invalid IL or missing references)
		//IL_0158: Unknown result type (might be due to invalid IL or missing references)
		//IL_0144: Unknown result type (might be due to invalid IL or missing references)
		//IL_0150: Unknown result type (might be due to invalid IL or missing references)
		//IL_0106: Unknown result type (might be due to invalid IL or missing references)
		//IL_0108: Unknown result type (might be due to invalid IL or missing references)
		//IL_010f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0110: Unknown result type (might be due to invalid IL or missing references)
		if ((Object)(object)base.brushType == (Object)null)
		{
			base.brushType = FindDefaultBrush(m_BrushQuery);
		}
		base.requireNet = Layer.Road | Layer.TrainTrack | Layer.Pathway | Layer.TramTrack | Layer.SubwayTrack | Layer.PublicTransportRoad;
		base.requirePipelines = true;
		if (m_FocusChanged)
		{
			return inputDeps;
		}
		if ((Object)(object)prefab != (Object)null && (Object)(object)base.brushType != (Object)null && m_HasFocus)
		{
			UpdateInfoview(m_PrefabSystem.GetEntity(prefab));
			GetAvailableSnapMask(out m_SnapOnMask, out m_SnapOffMask);
			if (m_State != State.Default && !base.applyAction.enabled)
			{
				m_State = State.Default;
			}
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
		}
		else
		{
			UpdateInfoview(Entity.Null);
		}
		if (m_State != State.Default && (base.applyAction.WasReleasedThisFrame() || base.secondaryApplyAction.WasReleasedThisFrame() || !m_HasFocus))
		{
			m_StartPoint = default(ControlPoint);
			m_State = State.Default;
		}
		return Clear(inputDeps);
	}

	public override void GetAvailableSnapMask(out Snap onMask, out Snap offMask)
	{
		base.GetAvailableSnapMask(out onMask, out offMask);
		if ((Object)(object)prefab != (Object)null && prefab.m_Target == TerraformingTarget.Height)
		{
			onMask |= Snap.ContourLines;
			offMask |= Snap.ContourLines;
		}
	}

	private JobHandle Clear(JobHandle inputDeps)
	{
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		base.applyMode = ApplyMode.Clear;
		SetDisableFX();
		return inputDeps;
	}

	private JobHandle Cancel(JobHandle inputDeps, bool singleFrameOnly = false)
	{
		//IL_0135: Unknown result type (might be due to invalid IL or missing references)
		//IL_0136: Unknown result type (might be due to invalid IL or missing references)
		//IL_0100: Unknown result type (might be due to invalid IL or missing references)
		//IL_0101: Unknown result type (might be due to invalid IL or missing references)
		//IL_009d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_00af: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_0073: Unknown result type (might be due to invalid IL or missing references)
		if (m_State == State.Default)
		{
			base.applyMode = ((prefab.m_Type != TerraformingType.Slope && GetAllowApply()) ? ApplyMode.Apply : ApplyMode.Clear);
			if (!singleFrameOnly)
			{
				m_StartPoint = m_RaycastPoint;
				m_State = State.Removing;
			}
			if ((Object)(object)m_AudioSource == (Object)null && !m_ToolSystem.actionMode.IsEditor())
			{
				m_AudioSource = m_AudioManager.PlayExclusiveUISound(((EntityQuery)(ref m_SoundQuery)).GetSingleton<ToolUXSoundSettingsData>().m_TerraformSound);
			}
			GetRaycastResult(out m_RaycastPoint);
			m_TargetSet = true;
			m_TargetPosition = m_RaycastPoint.m_HitPosition;
			inputDeps = InvertBrushes(m_TempQuery, inputDeps);
			return UpdateDefinitions(inputDeps);
		}
		if (m_State == State.Removing)
		{
			base.applyMode = (GetAllowApply() ? ApplyMode.Apply : ApplyMode.Clear);
			m_StartPoint = default(ControlPoint);
			m_State = State.Default;
			GetRaycastResult(out m_RaycastPoint);
			SetDisableFX();
			return UpdateDefinitions(inputDeps);
		}
		base.applyMode = ApplyMode.Clear;
		m_StartPoint = default(ControlPoint);
		m_State = State.Default;
		GetRaycastResult(out m_RaycastPoint);
		SetDisableFX();
		return UpdateDefinitions(inputDeps);
	}

	private JobHandle Apply(JobHandle inputDeps, bool singleFrameOnly = false)
	{
		//IL_011f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0120: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_00eb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0096: Unknown result type (might be due to invalid IL or missing references)
		//IL_009b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_0073: Unknown result type (might be due to invalid IL or missing references)
		if (m_State == State.Default)
		{
			base.applyMode = ((prefab.m_Type != TerraformingType.Slope && GetAllowApply()) ? ApplyMode.Apply : ApplyMode.Clear);
			if (!singleFrameOnly)
			{
				m_StartPoint = m_RaycastPoint;
				m_State = State.Adding;
			}
			if ((Object)(object)m_AudioSource == (Object)null && !m_ToolSystem.actionMode.IsEditor())
			{
				m_AudioSource = m_AudioManager.PlayExclusiveUISound(((EntityQuery)(ref m_SoundQuery)).GetSingleton<ToolUXSoundSettingsData>().m_TerraformSound);
			}
			GetRaycastResult(out m_RaycastPoint);
			m_ApplyPosition = m_RaycastPoint.m_HitPosition;
			return UpdateDefinitions(inputDeps);
		}
		if (m_State == State.Adding)
		{
			base.applyMode = (GetAllowApply() ? ApplyMode.Apply : ApplyMode.Clear);
			m_StartPoint = default(ControlPoint);
			m_State = State.Default;
			GetRaycastResult(out m_RaycastPoint);
			SetDisableFX();
			return UpdateDefinitions(inputDeps);
		}
		base.applyMode = ApplyMode.Clear;
		m_StartPoint = default(ControlPoint);
		m_State = State.Default;
		GetRaycastResult(out m_RaycastPoint);
		SetDisableFX();
		return UpdateDefinitions(inputDeps);
	}

	private JobHandle Update(JobHandle inputDeps)
	{
		//IL_00aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_0086: Unknown result type (might be due to invalid IL or missing references)
		//IL_0087: Unknown result type (might be due to invalid IL or missing references)
		//IL_0100: Unknown result type (might be due to invalid IL or missing references)
		//IL_0101: Unknown result type (might be due to invalid IL or missing references)
		//IL_006e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		if (GetRaycastResult(out var controlPoint))
		{
			if (m_State != State.Default)
			{
				base.applyMode = (GetAllowApply() ? ApplyMode.Apply : ApplyMode.Clear);
				m_StartPoint = m_RaycastPoint;
				m_RaycastPoint = controlPoint;
				return UpdateDefinitions(inputDeps);
			}
			if (m_RaycastPoint.Equals(controlPoint))
			{
				if (HaveBrushSettingsChanged())
				{
					base.applyMode = ApplyMode.Clear;
					return UpdateDefinitions(inputDeps);
				}
				base.applyMode = ApplyMode.None;
				return inputDeps;
			}
			base.applyMode = ApplyMode.Clear;
			m_StartPoint = controlPoint;
			m_RaycastPoint = controlPoint;
			return UpdateDefinitions(inputDeps);
		}
		if (m_RaycastPoint.Equals(default(ControlPoint)))
		{
			base.applyMode = ApplyMode.None;
			return inputDeps;
		}
		if (m_State != State.Default)
		{
			base.applyMode = (GetAllowApply() ? ApplyMode.Apply : ApplyMode.Clear);
			m_StartPoint = m_RaycastPoint;
			m_RaycastPoint = default(ControlPoint);
		}
		else
		{
			base.applyMode = ApplyMode.Clear;
			m_StartPoint = default(ControlPoint);
			m_RaycastPoint = default(ControlPoint);
		}
		return UpdateDefinitions(inputDeps);
	}

	private bool HaveBrushSettingsChanged()
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		NativeArray<ArchetypeChunk> val = ((EntityQuery)(ref m_VisibleQuery)).ToArchetypeChunkArray(AllocatorHandle.op_Implicit((Allocator)3));
		try
		{
			ComponentTypeHandle<Brush> componentTypeHandle = InternalCompilerInterface.GetComponentTypeHandle<Brush>(ref __TypeHandle.__Game_Tools_Brush_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef);
			for (int i = 0; i < val.Length; i++)
			{
				ArchetypeChunk val2 = val[i];
				NativeArray<Brush> nativeArray = ((ArchetypeChunk)(ref val2)).GetNativeArray<Brush>(ref componentTypeHandle);
				for (int j = 0; j < nativeArray.Length; j++)
				{
					if (!nativeArray[j].m_Size.Equals(base.brushSize))
					{
						return true;
					}
				}
			}
			return false;
		}
		finally
		{
			val.Dispose();
		}
	}

	private JobHandle UpdateDefinitions(JobHandle inputDeps)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_013e: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		//IL_0069: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ef: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0109: Unknown result type (might be due to invalid IL or missing references)
		//IL_010e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0114: Unknown result type (might be due to invalid IL or missing references)
		//IL_0115: Unknown result type (might be due to invalid IL or missing references)
		//IL_011a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0121: Unknown result type (might be due to invalid IL or missing references)
		//IL_0127: Unknown result type (might be due to invalid IL or missing references)
		//IL_0128: Unknown result type (might be due to invalid IL or missing references)
		//IL_0129: Unknown result type (might be due to invalid IL or missing references)
		//IL_012e: Unknown result type (might be due to invalid IL or missing references)
		JobHandle val = DestroyDefinitions(m_DefinitionQuery, m_ToolOutputBarrier, inputDeps);
		if ((Object)(object)prefab != (Object)null && (Object)(object)base.brushType != (Object)null)
		{
			JobHandle val2 = IJobExtensions.Schedule<CreateDefinitionsJob>(new CreateDefinitionsJob
			{
				m_Prefab = m_PrefabSystem.GetEntity(prefab),
				m_Brush = m_PrefabSystem.GetEntity(base.brushType),
				m_Size = base.brushSize,
				m_Angle = math.radians(base.brushAngle),
				m_Strength = ((m_State == State.Removing) ? (0f - base.brushStrength) : base.brushStrength),
				m_Time = Time.deltaTime,
				m_StartPoint = m_StartPoint,
				m_EndPoint = m_RaycastPoint,
				m_Target = (m_TargetSet ? m_TargetPosition : m_RaycastPoint.m_HitPosition),
				m_ApplyStart = m_ApplyPosition,
				m_CommandBuffer = m_ToolOutputBarrier.CreateCommandBuffer()
			}, inputDeps);
			((EntityCommandBufferSystem)m_ToolOutputBarrier).AddJobHandleForProducer(val2);
			val = JobHandle.CombineDependencies(val, val2);
			if (base.applyMode == ApplyMode.Apply)
			{
				EnsureCachedBrushData();
			}
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
	public TerrainToolSystem()
	{
	}
}

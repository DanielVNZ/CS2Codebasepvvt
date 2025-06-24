using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using Colossal.Annotations;
using Colossal.Serialization.Entities;
using Game.Input;
using Game.Prefabs;
using Game.SceneFlow;
using Game.Serialization;
using Unity.Collections;
using Unity.Entities;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Scripting;

namespace Game.Tools;

[CompilerGenerated]
public class ToolSystem : GameSystemBase, IPreDeserialize
{
	protected const string kToolKeyGroup = "tool";

	protected const string kToolCancelKeyGroup = "tool cancel";

	protected const string kToolApplyKeyAction = "tool apply";

	protected const string kToolCancelKeyAction = "tool cancel";

	public Action<ToolBaseSystem> EventToolChanged;

	public Action<PrefabBase> EventPrefabChanged;

	public Action<InfoviewPrefab> EventInfoviewChanged;

	public Action EventInfomodesChanged;

	private ToolBaseSystem m_ActiveTool;

	private Entity m_Selected;

	private ToolBaseSystem m_LastTool;

	private InfoviewPrefab m_CurrentInfoview;

	private InfoviewPrefab m_LastToolInfoview;

	private PrefabSystem m_PrefabSystem;

	private UpdateSystem m_UpdateSystem;

	private ToolRaycastSystem m_ToolRaycastSystem;

	private DefaultToolSystem m_DefaultToolSystem;

	private List<ToolBaseSystem> m_Tools;

	private List<InfomodePrefab> m_LastToolInfomodes;

	private Dictionary<InfoviewPrefab, List<InfomodeInfo>> m_InfomodeMap;

	private Vector4[] m_InfomodeColors;

	private Vector4[] m_InfomodeParams;

	private int[] m_InfomodeCounts;

	private NativeList<Entity> m_Infomodes;

	private float m_InfoviewTimer;

	private bool m_FullUpdateRequired;

	private bool m_InfoviewUpdateRequired;

	private bool m_IsUpdating;

	private InputBarrier m_ToolActionBarrier;

	private Dictionary<ProxyAction, InputBarrier> m_MouseToolBarriers;

	public ToolBaseSystem activeTool
	{
		get
		{
			return m_ActiveTool;
		}
		set
		{
			if (value != m_ActiveTool)
			{
				m_ActiveTool = value;
				RequireFullUpdate();
				EventToolChanged?.Invoke(value);
			}
		}
	}

	public Entity selected
	{
		get
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			return m_Selected;
		}
		set
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			m_Selected = value;
		}
	}

	public int selectedIndex { get; set; }

	[CanBeNull]
	public PrefabBase activePrefab => m_ActiveTool.GetPrefab();

	public InfoviewPrefab infoview
	{
		get
		{
			return m_CurrentInfoview;
		}
		set
		{
			if ((Object)(object)value != (Object)(object)m_CurrentInfoview)
			{
				SetInfoview(value, null);
			}
		}
	}

	public InfoviewPrefab activeInfoview
	{
		get
		{
			if (!((Object)(object)m_CurrentInfoview != (Object)null) || !m_CurrentInfoview.isValid)
			{
				return null;
			}
			return m_CurrentInfoview;
		}
	}

	public GameMode actionMode { get; private set; } = GameMode.Other;

	public ApplyMode applyMode
	{
		get
		{
			if (m_LastTool == null)
			{
				return ApplyMode.None;
			}
			return m_LastTool.applyMode;
		}
	}

	public bool ignoreErrors { get; set; }

	public bool fullUpdateRequired { get; private set; }

	public List<ToolBaseSystem> tools
	{
		get
		{
			if (m_Tools == null)
			{
				m_Tools = new List<ToolBaseSystem>();
			}
			return m_Tools;
		}
	}

	protected override void OnGamePreload(Purpose purpose, GameMode mode)
	{
		actionMode = mode;
	}

	[Preserve]
	protected override void OnCreate()
	{
		//IL_0099: Unknown result type (might be due to invalid IL or missing references)
		//IL_009e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
		base.OnCreate();
		m_PrefabSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<PrefabSystem>();
		m_UpdateSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<UpdateSystem>();
		m_ToolRaycastSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<ToolRaycastSystem>();
		m_DefaultToolSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<DefaultToolSystem>();
		activeTool = m_DefaultToolSystem;
		m_LastToolInfomodes = new List<InfomodePrefab>();
		m_InfomodeMap = new Dictionary<InfoviewPrefab, List<InfomodeInfo>>();
		m_InfomodeColors = (Vector4[])(object)new Vector4[303];
		m_InfomodeParams = (Vector4[])(object)new Vector4[101];
		m_InfomodeCounts = new int[3];
		m_Infomodes = new NativeList<Entity>(10, AllocatorHandle.op_Implicit((Allocator)4));
		m_ToolActionBarrier = InputManager.instance.CreateMapBarrier("Tool", "ToolSystem");
		m_ToolActionBarrier.blocked = true;
		m_MouseToolBarriers = InputManager.instance.FindActionMap("Tool").actions.Values.Where((ProxyAction a) => a.isMouseAction).ToDictionary((ProxyAction i) => i, (ProxyAction i) => new InputBarrier("Mouse Tool", i, InputManager.DeviceType.Mouse));
		ProxyAction proxyAction = default(ProxyAction);
		InputBarrier inputBarrier = default(InputBarrier);
		foreach (KeyValuePair<ProxyAction, InputBarrier> item in m_MouseToolBarriers)
		{
			item.Deconstruct(ref proxyAction, ref inputBarrier);
			ProxyAction action = proxyAction;
			action.onInteraction += delegate(ProxyAction _, InputActionPhase phase)
			{
				//IL_0000: Unknown result type (might be due to invalid IL or missing references)
				//IL_0002: Invalid comparison between Unknown and I4
				if ((int)phase == 4 && m_MouseToolBarriers.TryGetValue(action, out var value))
				{
					value.blocked = ShouldBlockBarrier(InputManager.instance.activeControlScheme, InputManager.instance.mouseOverUI);
				}
			};
		}
		InputManager.instance.EventControlSchemeChanged += delegate(InputManager.ControlScheme activeControlScheme)
		{
			RefreshInputBarrier(activeControlScheme, InputManager.instance.mouseOverUI);
		};
		InputManager.instance.EventMouseOverUIChanged += delegate(bool mouseOverUI)
		{
			RefreshInputBarrier(InputManager.instance.activeControlScheme, mouseOverUI);
		};
		Shader.SetGlobalInt("colossal_InfoviewOn", 0);
	}

	protected override void OnGameLoaded(Context serializationContext)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		base.OnGameLoaded(serializationContext);
		ClearInfomodes();
		m_CurrentInfoview = null;
		Shader.SetGlobalInt("colossal_InfoviewOn", 0);
	}

	[Preserve]
	protected override void OnDestroy()
	{
		m_Infomodes.Dispose();
		Shader.SetGlobalInt("colossal_InfoviewOn", 0);
		m_ToolActionBarrier.Dispose();
		ProxyAction proxyAction = default(ProxyAction);
		InputBarrier inputBarrier = default(InputBarrier);
		foreach (KeyValuePair<ProxyAction, InputBarrier> item in m_MouseToolBarriers)
		{
			item.Deconstruct(ref proxyAction, ref inputBarrier);
			inputBarrier.Dispose();
		}
		m_MouseToolBarriers = null;
		base.OnDestroy();
	}

	[Preserve]
	protected override void OnUpdate()
	{
		m_ToolActionBarrier.blocked = !GameManager.instance.gameMode.IsGameOrEditor() || GameManager.instance.isGameLoading;
		m_IsUpdating = true;
		m_UpdateSystem.Update(SystemUpdatePhase.PreTool);
		ToolUpdate();
		m_UpdateSystem.Update(SystemUpdatePhase.PostTool);
		fullUpdateRequired = m_FullUpdateRequired;
		m_FullUpdateRequired = false;
		m_IsUpdating = false;
	}

	public bool ActivatePrefabTool([CanBeNull] PrefabBase prefab)
	{
		if ((Object)(object)prefab != (Object)null)
		{
			foreach (ToolBaseSystem tool in tools)
			{
				if (tool.TrySetPrefab(prefab))
				{
					activeTool = tool;
					return true;
				}
			}
		}
		activeTool = m_DefaultToolSystem;
		return false;
	}

	private void RefreshInputBarrier(InputManager.ControlScheme activeControlScheme, bool mouseOverUI)
	{
		bool flag = ShouldBlockBarrier(activeControlScheme, mouseOverUI);
		ProxyAction proxyAction = default(ProxyAction);
		InputBarrier inputBarrier = default(InputBarrier);
		foreach (KeyValuePair<ProxyAction, InputBarrier> item in m_MouseToolBarriers)
		{
			item.Deconstruct(ref proxyAction, ref inputBarrier);
			ProxyAction proxyAction2 = proxyAction;
			inputBarrier.blocked = flag && !proxyAction2.IsInProgress();
		}
	}

	private bool ShouldBlockBarrier(InputManager.ControlScheme activeControlScheme, bool mouseOverUI)
	{
		return activeControlScheme == InputManager.ControlScheme.KeyboardAndMouse && mouseOverUI;
	}

	public void RequireFullUpdate()
	{
		if (m_IsUpdating)
		{
			m_FullUpdateRequired = true;
		}
		else
		{
			fullUpdateRequired = true;
		}
	}

	private void ToolUpdate()
	{
		//IL_0185: Unknown result type (might be due to invalid IL or missing references)
		//IL_018a: Unknown result type (might be due to invalid IL or missing references)
		//IL_018d: Unknown result type (might be due to invalid IL or missing references)
		m_InfoviewTimer += Time.deltaTime;
		m_InfoviewTimer %= 60f;
		if (activeTool != m_LastTool)
		{
			if (m_LastTool != null)
			{
				((ComponentSystemBase)m_LastTool).Enabled = false;
				((ComponentSystemBase)m_LastTool).Update();
			}
			m_LastTool = activeTool;
		}
		InfoviewPrefab infoviewPrefab = null;
		List<InfomodePrefab> list = null;
		if (m_LastTool != null)
		{
			((ComponentSystemBase)m_LastTool).Enabled = true;
		}
		m_UpdateSystem.Update(SystemUpdatePhase.ToolUpdate);
		if (m_LastTool != null)
		{
			infoviewPrefab = m_LastTool.infoview;
			list = m_LastTool.infomodes;
		}
		if ((Object)(object)infoviewPrefab != (Object)(object)m_LastToolInfoview)
		{
			SetInfoview(infoviewPrefab, list);
			m_LastToolInfoview = infoviewPrefab;
			m_LastToolInfomodes.Clear();
			if (list != null)
			{
				m_LastToolInfomodes.AddRange(list);
			}
		}
		else if ((Object)(object)infoviewPrefab != (Object)null && (Object)(object)infoviewPrefab == (Object)(object)activeInfoview)
		{
			if ((list != null && list.Count != 0) || m_LastToolInfomodes.Count != 0)
			{
				List<InfomodeInfo> infomodes = GetInfomodes(infoviewPrefab);
				for (int i = 0; i < infomodes.Count; i++)
				{
					InfomodeInfo infomodeInfo = infomodes[i];
					if (infomodeInfo.m_Supplemental || (infomodeInfo.m_Optional && actionMode.IsGame()))
					{
						bool num = m_LastToolInfomodes.Contains(infomodeInfo.m_Mode);
						bool flag = list?.Contains(infomodeInfo.m_Mode) ?? false;
						if (num != flag)
						{
							Entity entity = m_PrefabSystem.GetEntity(infomodeInfo.m_Mode);
							SetInfomodeActive(entity, flag, infomodeInfo.m_Priority);
						}
					}
				}
			}
			m_LastToolInfomodes.Clear();
			if (list != null)
			{
				m_LastToolInfomodes.AddRange(list);
			}
		}
		if (m_InfoviewUpdateRequired)
		{
			m_InfoviewUpdateRequired = false;
			UpdateInfoviewColors();
		}
		Shader.SetGlobalFloat("colossal_InfoviewTime", m_InfoviewTimer);
	}

	private void SetInfoview(InfoviewPrefab value, List<InfomodePrefab> infomodes)
	{
		//IL_007d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0082: Unknown result type (might be due to invalid IL or missing references)
		//IL_0084: Unknown result type (might be due to invalid IL or missing references)
		m_CurrentInfoview = value;
		ClearInfomodes();
		if ((Object)(object)activeInfoview != (Object)null)
		{
			List<InfomodeInfo> infomodes2 = GetInfomodes(value);
			for (int i = 0; i < infomodes2.Count; i++)
			{
				InfomodeInfo infomodeInfo = infomodes2[i];
				if ((!infomodeInfo.m_Supplemental || (infomodes != null && infomodes.Contains(infomodeInfo.m_Mode))) && (!infomodeInfo.m_Optional || !actionMode.IsGame() || infomodes == null || infomodes.Contains(infomodeInfo.m_Mode)))
				{
					Entity entity = m_PrefabSystem.GetEntity(infomodeInfo.m_Mode);
					Activate(entity, infomodeInfo.m_Mode, infomodeInfo.m_Priority);
					m_Infomodes.Add(ref entity);
				}
			}
		}
		m_InfoviewTimer = 0f;
		m_InfoviewUpdateRequired = true;
		EventInfoviewChanged?.Invoke(value);
	}

	private void ClearInfomodes()
	{
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		for (int i = 0; i < m_InfomodeCounts.Length; i++)
		{
			m_InfomodeCounts[i] = 0;
		}
		EntityManager entityManager = ((ComponentSystemBase)this).EntityManager;
		((EntityManager)(ref entityManager)).RemoveComponent<InfomodeActive>(m_Infomodes.AsArray());
		m_Infomodes.Clear();
	}

	private void UpdateInfoviewColors()
	{
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		//IL_008f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0094: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ed: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f7: Unknown result type (might be due to invalid IL or missing references)
		//IL_010e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0113: Unknown result type (might be due to invalid IL or missing references)
		//IL_0118: Unknown result type (might be due to invalid IL or missing references)
		//IL_012f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0134: Unknown result type (might be due to invalid IL or missing references)
		//IL_0139: Unknown result type (might be due to invalid IL or missing references)
		//IL_0152: Unknown result type (might be due to invalid IL or missing references)
		//IL_0157: Unknown result type (might be due to invalid IL or missing references)
		//IL_0175: Unknown result type (might be due to invalid IL or missing references)
		//IL_017a: Unknown result type (might be due to invalid IL or missing references)
		//IL_017f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0196: Unknown result type (might be due to invalid IL or missing references)
		//IL_019b: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01da: Unknown result type (might be due to invalid IL or missing references)
		//IL_01df: Unknown result type (might be due to invalid IL or missing references)
		//IL_0228: Unknown result type (might be due to invalid IL or missing references)
		//IL_023f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0256: Unknown result type (might be due to invalid IL or missing references)
		//IL_0278: Unknown result type (might be due to invalid IL or missing references)
		//IL_027d: Unknown result type (might be due to invalid IL or missing references)
		if ((Object)(object)activeInfoview != (Object)null)
		{
			m_InfomodeColors[0] = Color.op_Implicit(((Color)(ref activeInfoview.m_DefaultColor)).linear);
			m_InfomodeColors[1] = Color.op_Implicit(((Color)(ref activeInfoview.m_DefaultColor)).linear);
			m_InfomodeColors[2] = Color.op_Implicit(((Color)(ref activeInfoview.m_SecondaryColor)).linear);
			m_InfomodeParams[0] = new Vector4(1f, 0f, 0f, 0f);
			for (int i = 0; i < m_Infomodes.Length; i++)
			{
				Entity val = m_Infomodes[i];
				InfomodePrefab prefab = m_PrefabSystem.GetPrefab<InfomodePrefab>(val);
				EntityManager entityManager = ((ComponentSystemBase)this).EntityManager;
				InfomodeActive componentData = ((EntityManager)(ref entityManager)).GetComponentData<InfomodeActive>(val);
				prefab.GetColors(out var color, out var color2, out var color3, out var steps, out var speed, out var tiling, out var fill);
				m_InfomodeColors[componentData.m_Index * 3] = Color.op_Implicit(((Color)(ref color)).linear);
				m_InfomodeColors[componentData.m_Index * 3 + 1] = Color.op_Implicit(((Color)(ref color2)).linear);
				m_InfomodeColors[componentData.m_Index * 3 + 2] = Color.op_Implicit(((Color)(ref color3)).linear);
				m_InfomodeParams[componentData.m_Index] = new Vector4(steps, speed, tiling, fill);
				if (componentData.m_SecondaryIndex != -1)
				{
					m_InfomodeColors[componentData.m_SecondaryIndex * 3] = Color.op_Implicit(((Color)(ref color)).linear);
					m_InfomodeColors[componentData.m_SecondaryIndex * 3 + 1] = Color.op_Implicit(((Color)(ref color2)).linear);
					m_InfomodeColors[componentData.m_SecondaryIndex * 3 + 2] = Color.op_Implicit(((Color)(ref color3)).linear);
					m_InfomodeParams[componentData.m_SecondaryIndex] = new Vector4(steps, speed, tiling, fill);
				}
			}
			for (int j = 0; j < m_InfomodeCounts.Length; j++)
			{
				for (int k = m_InfomodeCounts[j]; k < 4; k++)
				{
					int num = 1 + j * 4 + k;
					m_InfomodeColors[num * 3] = default(Vector4);
					m_InfomodeColors[num * 3 + 1] = default(Vector4);
					m_InfomodeColors[num * 3 + 2] = default(Vector4);
					m_InfomodeParams[num] = new Vector4(1f, 0f, 0f, 0f);
				}
			}
			Shader.SetGlobalInt("colossal_InfoviewOn", 1);
			Shader.SetGlobalVectorArray("colossal_InfomodeColors", m_InfomodeColors);
			Shader.SetGlobalVectorArray("colossal_InfomodeParams", m_InfomodeParams);
		}
		else
		{
			Shader.SetGlobalInt("colossal_InfoviewOn", 0);
		}
	}

	[CanBeNull]
	public List<InfomodeInfo> GetInfomodes(InfoviewPrefab infoview)
	{
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		if ((Object)(object)infoview == (Object)null)
		{
			return null;
		}
		if (!m_InfomodeMap.TryGetValue(infoview, out var value))
		{
			value = new List<InfomodeInfo>();
			DynamicBuffer<InfoviewMode> buffer = m_PrefabSystem.GetBuffer<InfoviewMode>((PrefabBase)infoview, isReadOnly: true);
			for (int i = 0; i < buffer.Length; i++)
			{
				InfoviewMode infoviewMode = buffer[i];
				InfomodeInfo item = new InfomodeInfo
				{
					m_Mode = m_PrefabSystem.GetPrefab<InfomodePrefab>(infoviewMode.m_Mode),
					m_Priority = infoviewMode.m_Priority,
					m_Supplemental = infoviewMode.m_Supplemental,
					m_Optional = infoviewMode.m_Optional
				};
				value.Add(item);
			}
			value.Sort();
			m_InfomodeMap.Add(infoview, value);
		}
		return value;
	}

	public List<InfomodeInfo> GetInfoviewInfomodes()
	{
		return GetInfomodes(activeInfoview);
	}

	public bool IsInfomodeActive(InfomodePrefab prefab)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		Entity entity = m_PrefabSystem.GetEntity(prefab);
		EntityManager entityManager = ((ComponentSystemBase)this).EntityManager;
		return ((EntityManager)(ref entityManager)).HasComponent<InfomodeActive>(entity);
	}

	public void SetInfomodeActive(InfomodePrefab prefab, bool active, int priority)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		Entity entity = m_PrefabSystem.GetEntity(prefab);
		SetInfomodeActive(entity, active, priority);
	}

	public void SetInfomodeActive(Entity entity, bool active, int priority)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_0076: Unknown result type (might be due to invalid IL or missing references)
		//IL_007b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_008c: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_0145: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0101: Unknown result type (might be due to invalid IL or missing references)
		//IL_0104: Unknown result type (might be due to invalid IL or missing references)
		//IL_010e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0121: Unknown result type (might be due to invalid IL or missing references)
		EntityManager entityManager = ((ComponentSystemBase)this).EntityManager;
		if (!((EntityManager)(ref entityManager)).Exists(entity))
		{
			return;
		}
		if (!active)
		{
			int num = NativeListExtensions.IndexOf<Entity, Entity>(m_Infomodes, entity);
			if (num >= 0)
			{
				entityManager = ((ComponentSystemBase)this).EntityManager;
				InfomodeActive componentData = ((EntityManager)(ref entityManager)).GetComponentData<InfomodeActive>(entity);
				InfomodePrefab prefab = m_PrefabSystem.GetPrefab<InfomodePrefab>(entity);
				Deactivate(entity, prefab, componentData);
				m_Infomodes.RemoveAtSwapBack(num);
				m_InfoviewUpdateRequired = true;
				EventInfomodesChanged?.Invoke();
			}
		}
		else
		{
			if (NativeListExtensions.Contains<Entity, Entity>(m_Infomodes, entity))
			{
				return;
			}
			InfomodePrefab prefab2 = m_PrefabSystem.GetPrefab<InfomodePrefab>(entity);
			int secondaryGroup;
			int colorGroup = prefab2.GetColorGroup(out secondaryGroup);
			bool flag = false;
			for (int i = 0; i < m_Infomodes.Length; i++)
			{
				Entity val = m_Infomodes[i];
				InfomodePrefab prefab3 = m_PrefabSystem.GetPrefab<InfomodePrefab>(val);
				int secondaryGroup2;
				int colorGroup2 = prefab3.GetColorGroup(out secondaryGroup2);
				if ((colorGroup2 == colorGroup || colorGroup2 == secondaryGroup || secondaryGroup2 == colorGroup || (secondaryGroup2 == secondaryGroup && secondaryGroup2 != -1)) && !prefab2.CanActivateBoth(prefab3))
				{
					entityManager = ((ComponentSystemBase)this).EntityManager;
					InfomodeActive componentData2 = ((EntityManager)(ref entityManager)).GetComponentData<InfomodeActive>(val);
					Deactivate(val, prefab3, componentData2);
					m_Infomodes[i] = entity;
					flag = true;
					break;
				}
			}
			Activate(entity, prefab2, priority);
			if (!flag)
			{
				m_Infomodes.Add(ref entity);
			}
			m_InfoviewUpdateRequired = true;
			EventInfomodesChanged?.Invoke();
		}
	}

	private void Activate(Entity entity, InfomodePrefab prefab, int priority)
	{
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		int secondaryGroup;
		int colorGroup = prefab.GetColorGroup(out secondaryGroup);
		int index = colorGroup * 4 + ++m_InfomodeCounts[colorGroup];
		int secondaryIndex = -1;
		if (secondaryGroup != -1)
		{
			secondaryIndex = secondaryGroup * 4 + ++m_InfomodeCounts[secondaryGroup];
		}
		EntityManager entityManager = ((ComponentSystemBase)this).EntityManager;
		((EntityManager)(ref entityManager)).AddComponentData<InfomodeActive>(entity, new InfomodeActive(priority, index, secondaryIndex));
	}

	private void Deactivate(Entity entity, InfomodePrefab prefab, InfomodeActive infomodeActive)
	{
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		int secondaryGroup;
		int colorGroup = prefab.GetColorGroup(out secondaryGroup);
		Deactivate(colorGroup, infomodeActive.m_Index);
		if (secondaryGroup != -1)
		{
			Deactivate(secondaryGroup, infomodeActive.m_SecondaryIndex);
		}
		EntityManager entityManager = ((ComponentSystemBase)this).EntityManager;
		((EntityManager)(ref entityManager)).RemoveComponent<InfomodeActive>(entity);
	}

	private void Deactivate(int colorGroup, int activeIndex)
	{
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
		//IL_007d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0081: Unknown result type (might be due to invalid IL or missing references)
		int num = colorGroup * 4 + m_InfomodeCounts[colorGroup]--;
		if (activeIndex >= num)
		{
			return;
		}
		for (int i = 0; i < m_Infomodes.Length; i++)
		{
			Entity val = m_Infomodes[i];
			EntityManager entityManager = ((ComponentSystemBase)this).EntityManager;
			InfomodeActive componentData = ((EntityManager)(ref entityManager)).GetComponentData<InfomodeActive>(val);
			if (componentData.m_Index == num)
			{
				componentData.m_Index = activeIndex;
				entityManager = ((ComponentSystemBase)this).EntityManager;
				((EntityManager)(ref entityManager)).SetComponentData<InfomodeActive>(val, componentData);
				break;
			}
			if (componentData.m_SecondaryIndex == num)
			{
				componentData.m_SecondaryIndex = activeIndex;
				entityManager = ((ComponentSystemBase)this).EntityManager;
				((EntityManager)(ref entityManager)).SetComponentData<InfomodeActive>(val, componentData);
				break;
			}
		}
	}

	public void PreDeserialize(Context context)
	{
		ClearInfomodes();
		activeTool = m_DefaultToolSystem;
	}

	[Preserve]
	public ToolSystem()
	{
	}
}

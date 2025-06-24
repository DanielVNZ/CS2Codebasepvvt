using System.Collections.Generic;
using Colossal.Serialization.Entities;
using Game.Prefabs;
using Game.SceneFlow;
using Game.Settings;
using Unity.Entities;
using UnityEngine.Scripting;

namespace Game.Tutorials;

public class EditorTutorialSystem : TutorialSystem
{
	protected override Dictionary<string, bool> ShownTutorials => SharedSettings.instance.editor.shownTutorials;

	public override bool tutorialEnabled
	{
		get
		{
			return SharedSettings.instance.editor.showTutorials;
		}
		set
		{
			SharedSettings.instance.editor.showTutorials = value;
			if (!value)
			{
				base.mode = TutorialMode.Default;
			}
		}
	}

	[Preserve]
	protected override void OnCreate()
	{
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
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
		//IL_0098: Unknown result type (might be due to invalid IL or missing references)
		//IL_009d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00da: Unknown result type (might be due to invalid IL or missing references)
		//IL_00df: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00eb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fe: Unknown result type (might be due to invalid IL or missing references)
		//IL_0103: Unknown result type (might be due to invalid IL or missing references)
		//IL_010a: Unknown result type (might be due to invalid IL or missing references)
		//IL_010f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0114: Unknown result type (might be due to invalid IL or missing references)
		//IL_0119: Unknown result type (might be due to invalid IL or missing references)
		//IL_0128: Unknown result type (might be due to invalid IL or missing references)
		//IL_012d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0134: Unknown result type (might be due to invalid IL or missing references)
		//IL_0139: Unknown result type (might be due to invalid IL or missing references)
		//IL_0140: Unknown result type (might be due to invalid IL or missing references)
		//IL_0145: Unknown result type (might be due to invalid IL or missing references)
		//IL_014c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0151: Unknown result type (might be due to invalid IL or missing references)
		//IL_0158: Unknown result type (might be due to invalid IL or missing references)
		//IL_015d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0164: Unknown result type (might be due to invalid IL or missing references)
		//IL_0169: Unknown result type (might be due to invalid IL or missing references)
		//IL_0170: Unknown result type (might be due to invalid IL or missing references)
		//IL_0175: Unknown result type (might be due to invalid IL or missing references)
		//IL_017a: Unknown result type (might be due to invalid IL or missing references)
		//IL_017f: Unknown result type (might be due to invalid IL or missing references)
		//IL_018e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0193: Unknown result type (might be due to invalid IL or missing references)
		//IL_019a: Unknown result type (might be due to invalid IL or missing references)
		//IL_019f: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b5: Unknown result type (might be due to invalid IL or missing references)
		base.OnCreate();
		m_Setting = SharedSettings.instance.editor;
		m_TutorialQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[2]
		{
			ComponentType.ReadOnly<TutorialData>(),
			ComponentType.ReadOnly<EditorTutorial>()
		});
		m_ActiveTutorialQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[3]
		{
			ComponentType.ReadOnly<TutorialData>(),
			ComponentType.ReadOnly<TutorialActive>(),
			ComponentType.ReadOnly<EditorTutorial>()
		});
		m_PendingTutorialListQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[5]
		{
			ComponentType.ReadOnly<TutorialListData>(),
			ComponentType.ReadOnly<TutorialRef>(),
			ComponentType.ReadOnly<TutorialActivated>(),
			ComponentType.Exclude<TutorialCompleted>(),
			ComponentType.ReadOnly<EditorTutorial>()
		});
		m_PendingTutorialQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[6]
		{
			ComponentType.ReadOnly<TutorialData>(),
			ComponentType.ReadOnly<TutorialPhaseRef>(),
			ComponentType.ReadOnly<TutorialActivated>(),
			ComponentType.Exclude<TutorialActive>(),
			ComponentType.Exclude<TutorialCompleted>(),
			ComponentType.ReadOnly<EditorTutorial>()
		});
		m_PendingPriorityTutorialQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[7]
		{
			ComponentType.ReadOnly<TutorialData>(),
			ComponentType.ReadOnly<TutorialPhaseRef>(),
			ComponentType.ReadOnly<TutorialActivated>(),
			ComponentType.ReadOnly<ReplaceActiveData>(),
			ComponentType.Exclude<TutorialActive>(),
			ComponentType.Exclude<TutorialCompleted>(),
			ComponentType.ReadOnly<EditorTutorial>()
		});
		m_LockedTutorialQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[3]
		{
			ComponentType.ReadOnly<TutorialData>(),
			ComponentType.ReadOnly<Locked>(),
			ComponentType.ReadOnly<EditorTutorial>()
		});
	}

	public override void OnResetTutorials()
	{
		ShownTutorials.Clear();
		base.OnResetTutorials();
		if (GameManager.instance.gameMode.IsEditor())
		{
			m_Mode = TutorialMode.ListIntro;
		}
	}

	protected override void OnGamePreload(Purpose purpose, GameMode gameMode)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		base.OnGamePreload(purpose, gameMode);
		((ComponentSystemBase)this).Enabled = gameMode.IsEditor();
	}

	protected override void OnGameLoadingComplete(Purpose purpose, GameMode gameMode)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		base.OnGameLoadingComplete(purpose, gameMode);
		if (gameMode == GameMode.Editor && tutorialEnabled && !ShownTutorials.ContainsKey(TutorialSystem.kListIntroKey))
		{
			m_Mode = TutorialMode.ListIntro;
		}
	}

	[Preserve]
	public EditorTutorialSystem()
	{
	}
}

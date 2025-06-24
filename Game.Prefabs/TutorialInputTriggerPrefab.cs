using System;
using System.Collections.Generic;
using Game.Tutorials;
using Unity.Entities;

namespace Game.Prefabs;

[ComponentMenu("Tutorials/Triggers/", new Type[] { })]
public class TutorialInputTriggerPrefab : TutorialTriggerPrefabBase
{
	[Serializable]
	public struct InputAction
	{
		public string m_Map;

		public string m_Action;
	}

	public InputAction[] m_Actions;

	public override void GetPrefabComponents(HashSet<ComponentType> components)
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		base.GetPrefabComponents(components);
		components.Add(ComponentType.ReadWrite<InputTriggerData>());
	}
}

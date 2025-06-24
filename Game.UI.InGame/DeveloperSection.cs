using System.Collections.Generic;
using Colossal.UI.Binding;
using UnityEngine.Scripting;

namespace Game.UI.InGame;

public class DeveloperSection : InfoSectionBase, ISubsectionProvider, ISectionSource, IJsonWritable
{
	protected override string group => "DeveloperSection";

	public List<ISubsectionSource> subsections { get; private set; }

	protected override bool displayForDestroyedObjects => true;

	protected override bool displayForOutsideConnections => true;

	protected override bool displayForUnderConstruction => true;

	protected override bool displayForUpgrades => true;

	public void AddSubsection(ISubsectionSource subsection)
	{
		subsections.Add(subsection);
	}

	[Preserve]
	protected override void OnCreate()
	{
		base.OnCreate();
		subsections = new List<ISubsectionSource>();
	}

	protected override void Reset()
	{
	}

	private bool Visible()
	{
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		bool result = false;
		for (int i = 0; i < subsections.Count; i++)
		{
			if (subsections[i].DisplayFor(selectedEntity, selectedPrefab))
			{
				result = true;
				break;
			}
		}
		return result;
	}

	[Preserve]
	protected override void OnUpdate()
	{
		base.visible = Visible();
	}

	protected override void OnProcess()
	{
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		for (int i = 0; i < subsections.Count; i++)
		{
			if (subsections[i].DisplayFor(selectedEntity, selectedPrefab))
			{
				subsections[i].OnRequestUpdate(selectedEntity, selectedPrefab);
			}
		}
	}

	private int GetSubsectionCount()
	{
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		int num = 0;
		for (int i = 0; i < subsections.Count; i++)
		{
			if (subsections[i].DisplayFor(selectedEntity, selectedPrefab))
			{
				num++;
			}
		}
		return num;
	}

	public override void OnWriteProperties(IJsonWriter writer)
	{
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		writer.PropertyName("subsections");
		JsonWriterExtensions.ArrayBegin(writer, GetSubsectionCount());
		for (int i = 0; i < subsections.Count; i++)
		{
			if (subsections[i].DisplayFor(selectedEntity, selectedPrefab))
			{
				JsonWriterExtensions.Write<ISubsectionSource>(writer, subsections[i]);
			}
		}
		writer.ArrayEnd();
	}

	[Preserve]
	public DeveloperSection()
	{
	}
}

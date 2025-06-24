using Colossal.Entities;
using Colossal.UI.Binding;
using Game.Routes;
using Unity.Entities;
using UnityEngine.Scripting;

namespace Game.UI.InGame;

public abstract class VehicleWithLineSection : VehicleSection
{
	protected Entity lineEntity { get; set; }

	protected override void Reset()
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		base.Reset();
		lineEntity = Entity.Null;
	}

	protected override void OnProcess()
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		CurrentRoute currentRoute = default(CurrentRoute);
		lineEntity = (EntitiesExtensions.TryGetComponent<CurrentRoute>(((ComponentSystemBase)this).EntityManager, selectedEntity, ref currentRoute) ? currentRoute.m_Route : Entity.Null);
		base.OnProcess();
	}

	public override void OnWriteProperties(IJsonWriter writer)
	{
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		base.OnWriteProperties(writer);
		writer.PropertyName("line");
		if (lineEntity == Entity.Null)
		{
			writer.WriteNull();
		}
		else
		{
			m_NameSystem.BindName(writer, lineEntity);
		}
		writer.PropertyName("lineEntity");
		UnityWriters.Write(writer, lineEntity);
	}

	[Preserve]
	protected VehicleWithLineSection()
	{
	}
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using Colossal.Mathematics;
using Colossal.UI.Binding;
using Game.UI.Widgets;
using Unity.Entities;

namespace Game.UI.Editor;

public class SeasonWheel : Widget
{
	public struct Season : IJsonWritable, IEquatable<Season>
	{
		public Entity entity;

		public Bounds1 startTimeOfYear;

		public float temperature;

		public void Write(IJsonWriter writer)
		{
			//IL_0028: Unknown result type (might be due to invalid IL or missing references)
			//IL_003f: Unknown result type (might be due to invalid IL or missing references)
			writer.TypeBegin(GetType().FullName);
			writer.PropertyName("entity");
			UnityWriters.Write(writer, entity);
			writer.PropertyName("startTimeOfYear");
			MathematicsWriters.Write(writer, startTimeOfYear);
			writer.PropertyName("temperature");
			writer.Write(temperature);
			writer.TypeEnd();
		}

		public bool Equals(Season other)
		{
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			if (((Bounds1)(ref startTimeOfYear)).Equals(other.startTimeOfYear))
			{
				return temperature.Equals(other.temperature);
			}
			return false;
		}

		public override bool Equals(object obj)
		{
			if (obj is Season other)
			{
				return Equals(other);
			}
			return false;
		}

		public override int GetHashCode()
		{
			return (((object)System.Runtime.CompilerServices.Unsafe.As<Bounds1, Bounds1>(ref startTimeOfYear)/*cast due to .constrained prefix*/).GetHashCode() * 397) ^ temperature.GetHashCode();
		}

		public static bool operator ==(Season left, Season right)
		{
			return left.Equals(right);
		}

		public static bool operator !=(Season left, Season right)
		{
			return !left.Equals(right);
		}
	}

	public interface IAdapter
	{
		Entity selectedSeason { get; set; }

		IEnumerable<Season> seasons { get; }

		void SetStartTimeOfYear(Entity season, Bounds1 startTimeOfYear);
	}

	public class Bindings : IWidgetBindingFactory
	{
		public IEnumerable<IBinding> CreateBindings(string group, IReader<IWidget> pathResolver, ValueChangedCallback onValueChanged)
		{
			yield return (IBinding)(object)new TriggerBinding<IWidget, Entity>(group, "setSelectedSeason", (Action<IWidget, Entity>)delegate(IWidget widget, Entity season)
			{
				//IL_0010: Unknown result type (might be due to invalid IL or missing references)
				if (widget is SeasonWheel seasonWheel)
				{
					seasonWheel.adapter.selectedSeason = season;
				}
			}, pathResolver, (IReader<Entity>)null);
			yield return (IBinding)(object)new TriggerBinding<IWidget, Entity, Bounds1>(group, "setSeasonStartTimeOfYear", (Action<IWidget, Entity, Bounds1>)delegate(IWidget widget, Entity season, Bounds1 startTimeOfYear)
			{
				//IL_0010: Unknown result type (might be due to invalid IL or missing references)
				//IL_0011: Unknown result type (might be due to invalid IL or missing references)
				if (widget is SeasonWheel seasonWheel)
				{
					seasonWheel.adapter.SetStartTimeOfYear(season, startTimeOfYear);
					onValueChanged(widget);
				}
			}, pathResolver, (IReader<Entity>)null, (IReader<Bounds1>)null);
		}
	}

	private Entity m_SelectedSeason;

	private List<Season> m_Seasons = new List<Season>();

	public IAdapter adapter { get; set; }

	protected override WidgetChanges Update()
	{
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		WidgetChanges widgetChanges = base.Update();
		if (adapter.selectedSeason != m_SelectedSeason)
		{
			widgetChanges |= WidgetChanges.Properties;
			m_SelectedSeason = adapter.selectedSeason;
		}
		if (!m_Seasons.SequenceEqual(adapter.seasons))
		{
			widgetChanges |= WidgetChanges.Properties;
			m_Seasons.Clear();
			m_Seasons.AddRange(adapter.seasons);
		}
		return widgetChanges;
	}

	protected override void WriteProperties(IJsonWriter writer)
	{
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		base.WriteProperties(writer);
		writer.PropertyName("selectedSeason");
		UnityWriters.Write(writer, m_SelectedSeason);
		writer.PropertyName("seasons");
		JsonWriterExtensions.Write<Season>(writer, (IList<Season>)m_Seasons);
	}
}

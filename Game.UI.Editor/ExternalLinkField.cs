using System;
using System.Collections.Generic;
using System.Linq;
using Colossal.PSI.Common;
using Colossal.UI.Binding;
using Game.UI.Menu;
using Game.UI.Widgets;

namespace Game.UI.Editor;

public class ExternalLinkField : Widget
{
	public class Bindings : IWidgetBindingFactory
	{
		public IEnumerable<IBinding> CreateBindings(string group, IReader<IWidget> pathResolver, ValueChangedCallback onValueChanged)
		{
			yield return (IBinding)(object)new TriggerBinding<IWidget, int, string, string>(group, "setExternalLink", (Action<IWidget, int, string, string>)delegate(IWidget widget, int index, string type, string url)
			{
				if (widget is ExternalLinkField externalLinkField)
				{
					externalLinkField.SetValue(index, type, url);
					onValueChanged(widget);
				}
			}, pathResolver, (IReader<int>)null, (IReader<string>)null, (IReader<string>)null);
			yield return (IBinding)(object)new TriggerBinding<IWidget, int>(group, "removeExternalLink", (Action<IWidget, int>)delegate(IWidget widget, int index)
			{
				if (widget is ExternalLinkField externalLinkField)
				{
					externalLinkField.Remove(index);
					onValueChanged(widget);
				}
			}, pathResolver, (IReader<int>)null);
			yield return (IBinding)(object)new TriggerBinding<IWidget>(group, "addExternalLink", (Action<IWidget>)delegate(IWidget widget)
			{
				if (widget is ExternalLinkField externalLinkField)
				{
					externalLinkField.Add();
					onValueChanged(widget);
				}
			}, pathResolver);
		}
	}

	private static readonly ExternalLinkData kDefaultLink = new ExternalLinkData
	{
		m_Type = ExternalLinkInfo.kAcceptedTypes[0].m_Type,
		m_URL = string.Empty
	};

	private static readonly string[] kAcceptedTypes = ExternalLinkInfo.kAcceptedTypes.Select((ExternalLinkInfo info) => info.m_Type).ToArray();

	public List<ExternalLinkData> links { get; set; } = new List<ExternalLinkData>();

	public int maxLinks { get; set; } = 5;

	protected override void WriteProperties(IJsonWriter writer)
	{
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		base.WriteProperties(writer);
		writer.PropertyName("links");
		JsonWriterExtensions.ArrayBegin(writer, links.Count);
		foreach (ExternalLinkData link in links)
		{
			WriteExternalLink(writer, link);
		}
		writer.ArrayEnd();
		writer.PropertyName("acceptedTypes");
		JsonWriterExtensions.Write(writer, kAcceptedTypes);
		writer.PropertyName("maxLinks");
		writer.Write(maxLinks);
	}

	private void WriteExternalLink(IJsonWriter writer, ExternalLinkData link)
	{
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		writer.TypeBegin("ExternalLinkData");
		writer.PropertyName("type");
		writer.Write(link.m_Type);
		writer.PropertyName("url");
		writer.Write(link.m_URL);
		writer.PropertyName("error");
		writer.Write(!AssetUploadUtils.ValidateExternalLink(link));
		writer.PropertyName("lockType");
		writer.Write(AssetUploadUtils.LockLinkType(link.m_URL, out var _));
		writer.TypeEnd();
	}

	private void SetValue(int index, string type, string url)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		ExternalLinkData val = new ExternalLinkData
		{
			m_Type = type,
			m_URL = url
		};
		if (AssetUploadUtils.LockLinkType(val.m_URL, out var type2))
		{
			val.m_Type = type2;
		}
		links[index] = val;
		SetPropertiesChanged();
	}

	private void Remove(int index)
	{
		links.RemoveAt(index);
		SetPropertiesChanged();
	}

	private void Add()
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		links.Add(kDefaultLink);
		SetPropertiesChanged();
	}
}

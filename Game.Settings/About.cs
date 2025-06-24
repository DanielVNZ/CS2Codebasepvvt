using System;
using System.Collections.Generic;
using System.Text;
using ATL;
using cohtml.Net;
using Colossal;
using Colossal.Core;
using Colossal.PSI.Common;
using Colossal.UI;
using Game.UI.Menu;
using UnityEngine;

namespace Game.Settings;

public class About : Setting
{
	public const string kName = "About";

	private const string kGameGroup = "Game";

	private const string kContentGroup = "Content";

	[SettingsUISection("kGameGroup")]
	public string gameVersion
	{
		get
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			//IL_0005: Unknown result type (might be due to invalid IL or missing references)
			Version current = Version.current;
			return ((Version)(ref current)).fullVersion;
		}
	}

	[SettingsUISection("kGameGroup")]
	public string gameConfiguration
	{
		get
		{
			if (!Debug.isDebugBuild)
			{
				return "Release";
			}
			return "Development";
		}
	}

	public string coreVersion
	{
		get
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			//IL_0005: Unknown result type (might be due to invalid IL or missing references)
			Version current = Version.current;
			return ((Version)(ref current)).fullVersion;
		}
	}

	public string uiVersion
	{
		get
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			//IL_0005: Unknown result type (might be due to invalid IL or missing references)
			Version current = Version.current;
			return ((Version)(ref current)).fullVersion;
		}
	}

	public string unityVersion => Application.unityVersion;

	public string cohtmlVersion => Versioning.Build.ToString();

	public string atlVersion => Version.getVersion();

	public override void SetDefaults()
	{
	}

	public override AutomaticSettings.SettingPageData GetPageData(string id, bool addPrefix)
	{
		//IL_0233: Unknown result type (might be due to invalid IL or missing references)
		//IL_0238: Unknown result type (might be due to invalid IL or missing references)
		AutomaticSettings.SettingPageData pageData = base.GetPageData(id, addPrefix);
		foreach (IPlatformServiceIntegration platformServiceIntegration in PlatformManager.instance.platformServiceIntegrations)
		{
			StringBuilder stringBuilder = new StringBuilder();
			platformServiceIntegration.LogVersion(stringBuilder);
			string[] array = stringBuilder.ToString().Split(Environment.NewLine, StringSplitOptions.None);
			int num = 0;
			string[] array2 = array;
			foreach (string line in array2)
			{
				int sep = line.IndexOf(":", StringComparison.Ordinal);
				if (sep != -1)
				{
					AutomaticSettings.ManualProperty property = new AutomaticSettings.ManualProperty(typeof(About), typeof(string), platformServiceIntegration.name)
					{
						canRead = true,
						canWrite = false,
						attributes = 
						{
							(Attribute)new SettingsUIPathAttribute($"{((object)platformServiceIntegration).GetType().Name}{num++}.{platformServiceIntegration.name}"),
							(Attribute)new SettingsUIDisplayNameAttribute((string)null, line.Substring(0, sep))
						},
						getter = (object obj2) => line.Substring(sep + 1)
					};
					AutomaticSettings.SettingItemData item = new AutomaticSettings.SettingItemData(AutomaticSettings.WidgetType.StringField, this, property, pageData.prefix);
					pageData["General"].AddItem(item);
				}
			}
		}
		pageData.AddGroup("Content");
		foreach (IDlc dlc in PlatformManager.instance.EnumerateLocalDLCs())
		{
			AutomaticSettings.ManualProperty obj = new AutomaticSettings.ManualProperty(typeof(About), typeof(string), dlc.internalName)
			{
				canRead = true,
				canWrite = false,
				attributes = 
				{
					(Attribute)new SettingsUIPathAttribute(dlc.internalName),
					(Attribute)new SettingsUIDisplayNameAttribute((string)null, dlc.internalName + GetOwnershipCheckString(dlc))
				}
			};
			List<Attribute> attributes = obj.attributes;
			Version version = dlc.version;
			attributes.Add(new SettingsUIDescriptionAttribute((string)null, ((Version)(ref version)).fullVersion + GetOwnershipString(dlc)));
			obj.getter = delegate
			{
				//IL_0006: Unknown result type (might be due to invalid IL or missing references)
				//IL_000b: Unknown result type (might be due to invalid IL or missing references)
				Version version2 = dlc.version;
				return ((Version)(ref version2)).fullVersion;
			};
			AutomaticSettings.ManualProperty property2 = obj;
			AutomaticSettings.SettingItemData item2 = new AutomaticSettings.SettingItemData(AutomaticSettings.WidgetType.StringField, this, property2, pageData.prefix)
			{
				simpleGroup = "Content"
			};
			pageData["General"].AddItem(item2);
		}
		return pageData;
		static string GetOwnershipCheckString(IDlc val)
		{
			if (!PlatformManager.instance.IsDlcOwned(val))
			{
				return "*";
			}
			return string.Empty;
		}
		static string GetOwnershipString(IDlc val)
		{
			if (!PlatformManager.instance.IsDlcOwned(val))
			{
				return "\n*The Content is available on disk but not currently owned";
			}
			return string.Empty;
		}
	}
}

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Runtime.CompilerServices;
using Colossal;
using Colossal.IO.AssetDatabase;
using Colossal.PSI.Common;

namespace Game.Assets;

public class SaveGameMetadata : Metadata<SaveInfo>
{
	public const string kExtension = ".SaveGameMetadata";

	public static readonly Func<string> kPersistentLocation = () => "Saves/" + PlatformManager.instance.userSpecificPath;

	public bool isValidSaveGame
	{
		get
		{
			if (((AssetData)this).isValid && (AssetData)(object)base.target.saveGameData != (IAssetData)null)
			{
				if (!base.target.isReadonly)
				{
					return ((AssetData)base.target.saveGameData).isValid;
				}
				return false;
			}
			return false;
		}
	}

	public override IEnumerable<string> modTags
	{
		get
		{
			foreach (string item in _003C_003En__0())
			{
				yield return item;
			}
			yield return "Savegame";
		}
	}

	protected override void OnPostLoad()
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Invalid comparison between Unknown and I4
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_0087: Unknown result type (might be due to invalid IL or missing references)
		//IL_013e: Unknown result type (might be due to invalid IL or missing references)
		//IL_017f: Unknown result type (might be due to invalid IL or missing references)
		if ((int)((AssetData)this).state != 4)
		{
			return;
		}
		if (!((IDataSourceAccessor)((AssetData)this).database).dataSource.Contains(Identifier.op_Implicit(((AssetData)this).id)))
		{
			return;
		}
		base.target.id = ((AssetData)this).identifier;
		SourceMeta meta = ((AssetData)this).GetMeta();
		base.target.metaData = this;
		PackageAsset val = default(PackageAsset);
		if (((SourceMeta)(ref meta)).packaged && ((IAssetDatabase)((AssetData)this).database).TryGetAsset<PackageAsset>(meta.package, ref val))
		{
			base.target.displayName = ((AssetData)val).GetMeta().displayName;
		}
		else
		{
			base.target.displayName = meta.displayName;
		}
		base.target.path = ((AssetData)this).id.uri;
		base.target.isReadonly = !meta.belongsToCurrentUser;
		base.target.lastModified = meta.lastWriteTime.ToLocalTime();
		base.target.cloudTarget = meta.remoteStorageSourceName;
		if (!((AssetData)(object)base.target.saveGameData == (IAssetData)null))
		{
			return;
		}
		SaveGameData saveGameData = default(SaveGameData);
		if (((IAssetDatabase)((AssetData)this).database).TryGetAsset<SaveGameData>(Hash128.CreateGuid(Path.ChangeExtension(meta.path, SaveGameData.kExtensions[1])), ref saveGameData))
		{
			base.target.saveGameData = saveGameData;
		}
		else if (((SourceMeta)(ref meta)).packaged)
		{
			base.target.saveGameData = ((IAssetDatabase)((AssetData)this).database).GetAsset<SaveGameData>(SearchFilter<SaveGameData>.ByCondition((Func<SaveGameData, bool>)((SaveGameData a) => ((AssetData)a).GetMeta().package == meta.package), false));
		}
	}

	[CompilerGenerated]
	[DebuggerHidden]
	private IEnumerable<string> _003C_003En__0()
	{
		return ((AssetData)this).modTags;
	}
}

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using CliWrap;
using CliWrap.EventStream;
using Colossal;
using Colossal.IO;
using Colossal.Json;
using Colossal.Win32;
using Game.UI.Localization;
using Mono.Options;

namespace Game.Modding.Toolchain.Dependencies;

public class UnityModProjectDependency : BaseDependency
{
	public const string kProjectName = "UnityModsProject";

	public const string kProjectVersionTxt = "ProjectSettings/ProjectVersion.txt";

	public const string kProjectSettingsAsset = "ProjectSettings/ProjectSettings.asset";

	public const string kProjectPackageManifest = "Packages/manifest.json";

	public const string kProjectPackageLock = "Packages/packages-lock.json";

	public static readonly string kProjectUnzipPath = ToolchainDependencyManager.kUserToolingPath + "/UnityModsProject";

	public static readonly string kProjectZipPath = ToolchainDependencyManager.kGameToolingPath + "/UnityModsProject.zip";

	public static readonly string kModProjectsUnityVersionPath = kProjectUnzipPath + "/ProjectSettings/ProjectVersion.txt";

	public static readonly string kModProjectsVersionPath = kProjectUnzipPath + "/ProjectSettings/ProjectSettings.asset";

	public static readonly string kModProjectPackages = kProjectUnzipPath + "/Packages/packages-lock.json";

	public static bool isUnityOpened => IsUnityOpenWithModsProject(kProjectUnzipPath);

	public override string name => "Unity Mod Project";

	public override string icon => "Media/Menu/ColossalLogo.svg";

	public override string version
	{
		get
		{
			//IL_001f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0024: Unknown result type (might be due to invalid IL or missing references)
			if (LongFile.Exists(kModProjectsVersionPath) && base.version == null)
			{
				Version val = ReadYAMLVersion(LongFile.ReadAllLines(kModProjectsVersionPath));
				version = ((Version)(ref val)).shortVersion;
			}
			return base.version;
		}
		protected set
		{
			base.version = value;
		}
	}

	public override IEnumerable<string> envVariables
	{
		get
		{
			yield return "CSII_PATHSET";
			yield return "CSII_UNITYMODPROJECTPATH";
			yield return "CSII_UNITYVERSION";
			yield return "CSII_ENTITIESVERSION";
			yield return "CSII_ASSEMBLYSEARCHPATH";
		}
	}

	public override Type[] dependsOnInstallation => new Type[2]
	{
		typeof(UnityDependency),
		typeof(UnityLicenseDependency)
	};

	public override LocalizedString installDescr => LocalizedString.Id("Options.WARN_TOOLCHAIN_INSTALL_MOD_PROJECT");

	public override Task<bool> IsInstalled(CancellationToken token)
	{
		return Task.FromResult(LongDirectory.Exists(kProjectUnzipPath + "/Library") && LongFile.Exists(kModProjectsUnityVersionPath));
	}

	public override Task<bool> IsUpToDate(CancellationToken token)
	{
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		try
		{
			Version val = ReadUnityProjectVersion(kModProjectsUnityVersionPath);
			Version val2 = default(Version);
			((Version)(ref val2))._002Ector(UnityDependency.sUnityVersion);
			Version val3 = ReadYAMLVersion(LongFile.ReadAllLines(kModProjectsVersionPath));
			Version val4 = ReadYAMLVersion(ZipUtilities.ExtractAllLines(kProjectZipPath, "ProjectSettings/ProjectSettings.asset"));
			if (val < val2 || val3 < val4)
			{
				return Task.FromResult(result: false);
			}
			if (!LongFile.Exists(kModProjectPackages))
			{
				return Task.FromResult(result: false);
			}
			Variant obj = JSON.Load(LongFile.ReadAllText(kModProjectPackages));
			Variant val5 = JSON.Load(ZipUtilities.ExtractAllText(kProjectZipPath, "Packages/manifest.json"));
			Variant obj2 = obj.TryGet("dependencies");
			ProxyObject val6 = (ProxyObject)(object)((obj2 is ProxyObject) ? obj2 : null);
			Variant obj3 = val5.TryGet("dependencies");
			ProxyObject val7 = (ProxyObject)(object)((obj3 is ProxyObject) ? obj3 : null);
			if (val6 == null || val7 == null)
			{
				return Task.FromResult(result: false);
			}
			Variant val8 = default(Variant);
			Variant val10 = default(Variant);
			foreach (KeyValuePair<string, Variant> item in (IEnumerable<KeyValuePair<string, Variant>>)val7)
			{
				if (!((Variant)val6).TryGetValue(item.Key, ref val8))
				{
					return Task.FromResult(result: false);
				}
				ProxyObject val9 = (ProxyObject)(object)((val8 is ProxyObject) ? val8 : null);
				if (val9 == null)
				{
					return Task.FromResult(result: false);
				}
				if (!((Variant)val9).TryGetValue("version", ref val10) || !val10.Equals(item.Value))
				{
					return Task.FromResult(result: false);
				}
			}
			return Task.FromResult(result: true);
		}
		catch (Exception ex)
		{
			IToolchainDependency.log.Error(ex, (object)"Error during up-to-date check");
			return Task.FromResult(result: false);
		}
	}

	public override Task<bool> NeedDownload(CancellationToken token)
	{
		return Task.FromResult(result: false);
	}

	public override Task Download(CancellationToken token)
	{
		return Task.CompletedTask;
	}

	public override async Task Install(CancellationToken token)
	{
		token.ThrowIfCancellationRequested();
		string zipPath = kProjectZipPath;
		string unzipPath = kProjectUnzipPath;
		try
		{
			if (isUnityOpened)
			{
				IToolchainDependency.log.Debug((object)"Waiting for close Unity");
				base.state = new IToolchainDependency.State(DependencyState.Installing, "WaitingUnityClose");
				await AsyncUtils.WaitForAction((Func<bool>)(() => !isUnityOpened), token, 100).ConfigureAwait(continueOnCapturedContext: false);
			}
			token.ThrowIfCancellationRequested();
			IToolchainDependency.log.DebugFormat("Deploy Mods project from '{0}' to '{1}'", (object)zipPath, (object)unzipPath);
			base.state = new IToolchainDependency.State(DependencyState.Installing, "InstallingModProject");
			if (LongDirectory.Exists(unzipPath))
			{
				await AsyncUtils.DeleteDirectoryAsync(unzipPath, true, token, 100, 5000).ConfigureAwait(continueOnCapturedContext: false);
			}
			await Task.Run(delegate
			{
				ZipUtilities.Unzip(zipPath, unzipPath);
			}, token).ConfigureAwait(continueOnCapturedContext: false);
			IToolchainDependency.log.DebugFormat("Launching Unity ({0})", (object)UnityDependency.unityExe);
			Command val = Cli.Wrap(UnityDependency.unityExe).WithArguments((IEnumerable<string>)new string[5] { "-projectPath", unzipPath, "-logFile", "-", "-quit" });
			Enumerator<CommandEvent> asyncEnumerator = TaskAsyncEnumerableExtensions.ConfigureAwait<CommandEvent>(EventStreamCommandExtensions.ListenAsync(val, token), false).GetAsyncEnumerator();
			try
			{
				while (await asyncEnumerator.MoveNextAsync())
				{
					CommandEvent current = asyncEnumerator.Current;
					StandardOutputCommandEvent val2 = (StandardOutputCommandEvent)(object)((current is StandardOutputCommandEvent) ? current : null);
					if (val2 == null)
					{
						StandardErrorCommandEvent val3 = (StandardErrorCommandEvent)(object)((current is StandardErrorCommandEvent) ? current : null);
						if (val3 != null)
						{
							IToolchainDependency.log.Error((object)val3.Text);
						}
					}
					else
					{
						IToolchainDependency.log.Debug((object)val2.Text);
					}
				}
			}
			finally
			{
				await asyncEnumerator.DisposeAsync();
			}
		}
		catch (OperationCanceledException)
		{
			throw;
		}
		catch (ToolchainException)
		{
			throw;
		}
		catch (Exception innerException)
		{
			throw new ToolchainException(ToolchainError.Install, this, innerException);
		}
	}

	public override async Task Uninstall(CancellationToken token)
	{
		token.ThrowIfCancellationRequested();
		try
		{
			IToolchainDependency.log.Debug((object)"Deleting Mods project");
			base.state = new IToolchainDependency.State(DependencyState.Installing, "RemovingModProject");
			string text = kProjectUnzipPath;
			if (LongDirectory.Exists(text))
			{
				await AsyncUtils.DeleteDirectoryAsync(text, true, token, 100, 5000).ConfigureAwait(continueOnCapturedContext: false);
			}
		}
		catch (OperationCanceledException)
		{
			throw;
		}
		catch (ToolchainException)
		{
			throw;
		}
		catch (Exception innerException)
		{
			throw new ToolchainException(ToolchainError.Uninstall, this, innerException);
		}
	}

	public override Task<List<IToolchainDependency.DiskSpaceRequirements>> GetRequiredDiskSpace(CancellationToken token)
	{
		return Task.FromResult(new List<IToolchainDependency.DiskSpaceRequirements>
		{
			new IToolchainDependency.DiskSpaceRequirements
			{
				m_Path = kProjectUnzipPath,
				m_Size = 1073741824L
			}
		});
	}

	private static Version ReadUnityProjectVersion(string path)
	{
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		return new Version(LongFile.ReadAllLines(path)[0].Split(':', StringSplitOptions.None)[1].Trim());
	}

	private static Version ReadYAMLVersion(IEnumerable<string> lines)
	{
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		foreach (string line in lines)
		{
			if (line.Contains("bundleVersion:"))
			{
				return new Version(line.Split(':', StringSplitOptions.None)[1].Trim());
			}
		}
		throw new Exception();
	}

	private static bool IsUnityOpenWithModsProject(string projectPath)
	{
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		try
		{
			Process[] processesByName = Process.GetProcessesByName("unity");
			string text = default(string);
			for (int i = 0; i < processesByName.Length; i++)
			{
				int num = ProcessCommandLine.Retrieve(processesByName[i], ref text, (Parameter)0);
				if (num == 0)
				{
					string openProjectPath = string.Empty;
					new OptionSet().Add("projectpath=", "", (Action<string>)delegate(string option)
					{
						openProjectPath = option;
					}).Parse((IEnumerable<string>)ProcessCommandLine.CommandLineToArgs(text));
					if (!string.IsNullOrEmpty(openProjectPath) && Path.GetFullPath(openProjectPath) == Path.GetFullPath(projectPath))
					{
						return true;
					}
				}
				else
				{
					IToolchainDependency.log.DebugFormat("Unable to get command line ({0}): {1}", (object)num, (object)ProcessCommandLine.ErrorToString(num));
				}
			}
		}
		catch (Exception ex)
		{
			IToolchainDependency.log.Warn(ex);
		}
		return false;
	}
}

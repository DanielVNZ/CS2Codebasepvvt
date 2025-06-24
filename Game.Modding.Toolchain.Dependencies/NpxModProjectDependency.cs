using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using CliWrap;
using Colossal;

namespace Game.Modding.Toolchain.Dependencies;

public class NpxModProjectDependency : BaseDependency
{
	private const string kProjectName = "UI Mod project template";

	private const string kModuleNamespace = "@colossalorder";

	private const string kModuleName = "create-csii-ui-mod";

	private static readonly string kNpxPackagePath = Path.GetFullPath(Path.Combine(ToolchainDependencyManager.kGameToolingPath, "npx-create-csii-ui-mod"));

	public override Type[] dependsOnInstallation => new Type[1] { typeof(NodeJSDependency) };

	public override Type[] dependsOnUninstallation => new Type[1] { typeof(NodeJSDependency) };

	public override IEnumerable<string> envVariables
	{
		get
		{
			yield return "CSII_PATHSET";
			yield return "CSII_USERDATAPATH";
		}
	}

	public override string name => "UI template";

	public override string icon => "Media/Menu/ColossalLogo.svg";

	private async Task<string> GetGlobalNodeModulePath(CancellationToken token)
	{
		string path = string.Empty;
		List<string> errorText = new List<string>();
		try
		{
			await Cli.Wrap("npm").WithArguments("config get prefix").WithStandardOutputPipe(PipeTarget.ToDelegate((Action<string>)delegate(string l)
			{
				path = l;
			}))
				.WithStandardErrorPipe(PipeTarget.ToDelegate((Action<string>)delegate(string l)
				{
					errorText.Add(l);
				}))
				.WithValidation((CommandResultValidation)0)
				.ExecuteAsync(token)
				.ConfigureAwait(false);
		}
		catch (Win32Exception ex)
		{
			if (ex.ErrorCode != -2147467259)
			{
				IToolchainDependency.log.Error((Exception)ex, (object)"Failed to get global npm module path");
			}
			if (errorText.Count > 0)
			{
				IToolchainDependency.log.Warn((object)string.Join<string>('\n', (IEnumerable<string>)errorText));
			}
		}
		catch (Exception ex2)
		{
			IToolchainDependency.log.Error(ex2, (object)"Failed to get global npm module path");
			if (errorText.Count > 0)
			{
				IToolchainDependency.log.Warn((object)string.Join<string>('\n', (IEnumerable<string>)errorText));
			}
		}
		return path;
	}

	public override async Task<bool> IsInstalled(CancellationToken token)
	{
		string text = await GetGlobalNodeModulePath(token).ConfigureAwait(continueOnCapturedContext: false);
		if (LongDirectory.Exists(text))
		{
			return LongDirectory.Exists(Path.GetFullPath(Path.Combine(text, "node_modules", "@colossalorder")));
		}
		return false;
	}

	public override async Task<bool> IsUpToDate(CancellationToken token)
	{
		string text = await GetGlobalNodeModulePath(token).ConfigureAwait(continueOnCapturedContext: false);
		string text2 = default(string);
		if (LongDirectory.Exists(text) && LongFile.TryGetSymlinkTarget(Path.Combine(text, "node_modules", "@colossalorder", "create-csii-ui-mod"), ref text2))
		{
			return text2 == kNpxPackagePath;
		}
		return true;
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
		List<string> output = new List<string>();
		List<string> errorText = new List<string>();
		try
		{
			IToolchainDependency.log.DebugFormat("Installing {0}", (object)"UI Mod project template");
			base.state = new IToolchainDependency.State(DependencyState.Installing, "InstallingNpxModsTemplate");
			await Cli.Wrap("npm").WithArguments("link").WithWorkingDirectory(kNpxPackagePath)
				.WithStandardOutputPipe(PipeTarget.ToDelegate((Action<string>)delegate(string l)
				{
					output.Add(l);
				}))
				.WithStandardErrorPipe(PipeTarget.ToDelegate((Action<string>)delegate(string l)
				{
					errorText.Add(l);
				}))
				.WithValidation((CommandResultValidation)0)
				.ExecuteAsync(token)
				.ConfigureAwait(false);
			if (errorText.Count > 0)
			{
				IToolchainDependency.log.WarnFormat("{0}\n\n{1}", (object)string.Join<string>('\n', (IEnumerable<string>)output), (object)string.Join<string>('\n', (IEnumerable<string>)errorText));
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

	private static async Task DeleteNpxModule(string globalNodeModulePath, string moduleNamespace, string moduleName, CancellationToken token)
	{
		if (string.IsNullOrEmpty(globalNodeModulePath))
		{
			throw new ArgumentException("Directory path cannot be null or empty.", "globalNodeModulePath");
		}
		if (string.IsNullOrEmpty(moduleName))
		{
			throw new ArgumentException("File prefix cannot be null or empty.", "moduleName");
		}
		if (!Directory.Exists(globalNodeModulePath))
		{
			throw new DirectoryNotFoundException("The specified directory was not found: " + globalNodeModulePath);
		}
		string[] files = LongDirectory.GetFiles(globalNodeModulePath, moduleName + "*", SearchOption.TopDirectoryOnly);
		string[] array = files;
		for (int i = 0; i < array.Length; i++)
		{
			await AsyncUtils.DeleteFileAsync(array[i], token, 100, 5000).ConfigureAwait(continueOnCapturedContext: false);
		}
		await AsyncUtils.DeleteDirectoryAsync(Path.Combine(globalNodeModulePath, "node_modules", moduleNamespace), true, token, 100, 5000).ConfigureAwait(continueOnCapturedContext: false);
	}

	public override async Task Uninstall(CancellationToken token)
	{
		token.ThrowIfCancellationRequested();
		try
		{
			IToolchainDependency.log.DebugFormat("Deleting {0}", (object)"UI Mod project template");
			base.state = new IToolchainDependency.State(DependencyState.Removing, "RemovingNpxModsTemplate");
			string text = await GetGlobalNodeModulePath(token).ConfigureAwait(continueOnCapturedContext: false);
			if (LongDirectory.Exists(text))
			{
				await DeleteNpxModule(text, "@colossalorder", "create-csii-ui-mod", token);
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
		return Task.FromResult(new List<IToolchainDependency.DiskSpaceRequirements>());
	}
}

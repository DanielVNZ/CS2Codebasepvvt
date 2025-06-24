using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using CliWrap;
using Colossal;
using Game.UI.Localization;

namespace Game.Modding.Toolchain.Dependencies;

public class UnityLicenseDependency : BaseDependency
{
	private static readonly string kSerialBasedLicenseFile = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData), "Unity", "Unity_lic.ulf");

	private static readonly string kNamedUserLicenseFile = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Unity", "licenses", "UnityEntitlementLicense.xml");

	public override string name => "Unity license";

	public override string icon => "Media/Toolchain/Unity.svg";

	public override bool confirmUninstallation => true;

	public override LocalizedString installDescr => LocalizedString.Id("Options.WARN_TOOLCHAIN_INSTALL_UNITY_LICENSE");

	public override LocalizedString uninstallMessage => LocalizedString.Id("Options.WARN_TOOLCHAIN_UNITY_LICENSE_RETURN");

	public override Type[] dependsOnInstallation => new Type[1] { typeof(UnityDependency) };

	public override Type[] dependsOnUninstallation => new Type[1] { typeof(UnityDependency) };

	public bool licenseExists
	{
		get
		{
			if (!File.Exists(kSerialBasedLicenseFile))
			{
				return File.Exists(kNamedUserLicenseFile);
			}
			return true;
		}
	}

	public override Task<bool> IsInstalled(CancellationToken token)
	{
		return Task.FromResult(licenseExists);
	}

	public override Task<bool> IsUpToDate(CancellationToken token)
	{
		return Task.FromResult(result: true);
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
		try
		{
			IToolchainDependency.log.Debug((object)"Waiting for Unity license");
			base.state = new IToolchainDependency.State(DependencyState.Installing, "WaitingUnityLicense");
			Cli.Wrap(UnityDependency.unityExe).WithArguments((IEnumerable<string>)new string[3]
			{
				"-projectPath",
				UnityModProjectDependency.kProjectUnzipPath,
				"-quit"
			}).WithValidation((CommandResultValidation)0)
				.ExecuteAsync(token);
			await AsyncUtils.WaitForAction((Func<bool>)(() => licenseExists), token, 100).ConfigureAwait(continueOnCapturedContext: false);
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
			if (LongFile.Exists(kSerialBasedLicenseFile))
			{
				IToolchainDependency.log.Debug((object)"Return Unity license");
				base.state = new IToolchainDependency.State(DependencyState.Removing, "ReturningUnityLicense");
				await Cli.Wrap(UnityDependency.unityExe).WithArguments((IEnumerable<string>)new string[2] { "-returnlicense", "-quit" }).WithStandardOutputPipe(PipeTarget.ToDelegate((Action<string>)delegate(string l)
				{
					IToolchainDependency.log.Debug((object)l);
				}))
					.WithStandardErrorPipe(PipeTarget.ToDelegate((Action<string>)delegate(string l)
					{
						IToolchainDependency.log.Error((object)l);
					}))
					.ExecuteAsync(token)
					.ConfigureAwait(false);
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

	public override LocalizedString GetLocalizedState(bool includeProgress)
	{
		return base.state.m_State switch
		{
			DependencyState.Installed => LocalizedString.Id("Options.STATE_TOOLCHAIN[Activated]"), 
			DependencyState.Installing => LocalizedString.Id("Options.STATE_TOOLCHAIN[WaitingForActivation]"), 
			DependencyState.NotInstalled => LocalizedString.Id("Options.STATE_TOOLCHAIN[NotActivated]"), 
			DependencyState.Removing => LocalizedString.Id("Options.STATE_TOOLCHAIN[Returning]"), 
			_ => IToolchainDependency.GetLocalizedState(base.state, includeProgress), 
		};
	}
}

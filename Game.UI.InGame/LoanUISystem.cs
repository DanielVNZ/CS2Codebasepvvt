using System;
using System.Collections.Generic;
using Colossal.Serialization.Entities;
using Colossal.UI.Binding;
using Game.Tools;
using Unity.Entities;
using UnityEngine.Scripting;

namespace Game.UI.InGame;

public class LoanUISystem : UISystemBase
{
	public class LoanWriter : IWriter<LoanInfo>
	{
		public void Write(IJsonWriter writer, LoanInfo value)
		{
			writer.TypeBegin("loan.Loan");
			writer.PropertyName("amount");
			writer.Write(value.m_Amount);
			writer.PropertyName("dailyInterestRate");
			writer.Write(value.m_DailyInterestRate);
			writer.PropertyName("dailyPayment");
			writer.Write(value.m_DailyPayment);
			writer.TypeEnd();
		}
	}

	private const string kGroup = "loan";

	private GetterValueBinding<int> m_LoanLimitBinding;

	private GetterValueBinding<LoanInfo> m_CurrentLoanBinding;

	private GetterValueBinding<LoanInfo> m_LoanOfferBinding;

	private ILoanSystem m_LoanSystem;

	private int m_RequestedOfferDifference;

	[Preserve]
	protected override void OnCreate()
	{
		//IL_00dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e6: Expected O, but got Unknown
		//IL_00fd: Unknown result type (might be due to invalid IL or missing references)
		//IL_0107: Expected O, but got Unknown
		base.OnCreate();
		m_LoanSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<LoanSystem>();
		AddBinding((IBinding)(object)(m_LoanLimitBinding = new GetterValueBinding<int>("loan", "loanLimit", (Func<int>)(() => m_LoanSystem.Creditworthiness), (IWriter<int>)null, (EqualityComparer<int>)null)));
		AddBinding((IBinding)(object)(m_CurrentLoanBinding = new GetterValueBinding<LoanInfo>("loan", "currentLoan", (Func<LoanInfo>)(() => m_LoanSystem.CurrentLoan), (IWriter<LoanInfo>)new LoanWriter(), (EqualityComparer<LoanInfo>)null)));
		AddBinding((IBinding)(object)(m_LoanOfferBinding = new GetterValueBinding<LoanInfo>("loan", "loanOffer", (Func<LoanInfo>)(() => m_LoanSystem.RequestLoanOffer(m_LoanSystem.CurrentLoan.m_Amount + m_RequestedOfferDifference)), (IWriter<LoanInfo>)new LoanWriter(), (EqualityComparer<LoanInfo>)null)));
		AddBinding((IBinding)(object)new TriggerBinding<int>("loan", "requestLoanOffer", (Action<int>)RequestLoanOffer, (IReader<int>)null));
		AddBinding((IBinding)new TriggerBinding("loan", "acceptLoanOffer", (Action)AcceptLoanOffer));
		AddBinding((IBinding)new TriggerBinding("loan", "resetLoanOffer", (Action)ResetLoanOffer));
	}

	protected override void OnGameLoaded(Context serializationContext)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		base.OnGameLoaded(serializationContext);
		m_RequestedOfferDifference = 0;
	}

	[Preserve]
	protected override void OnUpdate()
	{
		m_LoanLimitBinding.Update();
		m_CurrentLoanBinding.Update();
		m_LoanOfferBinding.Update();
	}

	private void RequestLoanOffer(int amount)
	{
		LoanInfo loanInfo = m_LoanSystem.RequestLoanOffer(amount);
		LoanInfo currentLoan = m_LoanSystem.CurrentLoan;
		m_RequestedOfferDifference = loanInfo.m_Amount - currentLoan.m_Amount;
	}

	private void AcceptLoanOffer()
	{
		m_LoanSystem.ChangeLoan(m_LoanSystem.CurrentLoan.m_Amount + m_RequestedOfferDifference);
		m_RequestedOfferDifference = 0;
	}

	private void ResetLoanOffer()
	{
		m_RequestedOfferDifference = 0;
	}

	[Preserve]
	public LoanUISystem()
	{
	}
}

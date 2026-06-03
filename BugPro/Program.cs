using Stateless;

namespace BugPro;

public class Bug
{
    public enum BugStatus
    {
        New,
        Assigned,
        InProgress,
        Fixed,
        Verified,
        Closed,
        Reopened,
        Rejected,
        Deferred
    }

    public enum BugAction
    {
        Assign,
        StartWork,
        Fix,
        Confirm,
        Close,
        Reopen,
        Reject,
        Defer,
        Renew
    }

    private readonly StateMachine<BugStatus, BugAction> _fsm;
    private readonly StateMachine<BugStatus, BugAction>.TriggerWithParameters<string> _assignTrigger;

    public Bug(BugStatus initialStatus = BugStatus.New)
    {
        _fsm = new StateMachine<BugStatus, BugAction>(initialStatus);
        _assignTrigger = _fsm.SetTriggerParameters<string>(BugAction.Assign);

        _fsm.Configure(BugStatus.New)
            .Permit(BugAction.Assign, BugStatus.Assigned)
            .Permit(BugAction.Reject, BugStatus.Rejected)
            .Permit(BugAction.Defer, BugStatus.Deferred);

        _fsm.Configure(BugStatus.Assigned)
            .OnEntryFrom(_assignTrigger, assignee => Console.WriteLine($"  Assigned to: {assignee}"))
            .Permit(BugAction.StartWork, BugStatus.InProgress)
            .Permit(BugAction.Reject, BugStatus.Rejected)
            .Permit(BugAction.Defer, BugStatus.Deferred);

        _fsm.Configure(BugStatus.InProgress)
            .Permit(BugAction.Fix, BugStatus.Fixed)
            .Permit(BugAction.Defer, BugStatus.Deferred);

        _fsm.Configure(BugStatus.Fixed)
            .Permit(BugAction.Confirm, BugStatus.Verified)
            .Permit(BugAction.Reopen, BugStatus.Reopened);

        _fsm.Configure(BugStatus.Verified)
            .Permit(BugAction.Close, BugStatus.Closed)
            .Permit(BugAction.Reopen, BugStatus.Reopened);

        _fsm.Configure(BugStatus.Closed)
            .Permit(BugAction.Reopen, BugStatus.Reopened);

        _fsm.Configure(BugStatus.Reopened)
            .Permit(BugAction.Assign, BugStatus.Assigned)
            .Permit(BugAction.Reject, BugStatus.Rejected);

        _fsm.Configure(BugStatus.Rejected)
            .Permit(BugAction.Renew, BugStatus.New);

        _fsm.Configure(BugStatus.Deferred)
            .Permit(BugAction.Renew, BugStatus.New);
    }

    public BugStatus CurrentStatus => _fsm.State;

    public void AssignTo(string assignee) => _fsm.Fire(_assignTrigger, assignee);
    public void StartWorking() => _fsm.Fire(BugAction.StartWork);
    public void MarkAsFixed() => _fsm.Fire(BugAction.Fix);
    public void ConfirmFix() => _fsm.Fire(BugAction.Confirm);
    public void CloseBug() => _fsm.Fire(BugAction.Close);
    public void ReopenBug() => _fsm.Fire(BugAction.Reopen);
    public void RejectBug() => _fsm.Fire(BugAction.Reject);
    public void DeferBug() => _fsm.Fire(BugAction.Defer);
    public void ActivateAgain() => _fsm.Fire(BugAction.Renew);
}

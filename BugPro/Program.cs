using Stateless;

namespace BugTracking;

public class Ticket
{
    public enum TicketStatus
    {
        Open, Allocated, InWork, Resolved, Reviewed, Done,
        Reopened, Declined, Suspended
    }

    public enum TicketAction
    {
        Allocate, Begin, Resolve, Accept, Complete,
        Reactivate, Decline, Postpone, Restore
    }

    private readonly StateMachine<TicketStatus, TicketAction> _stateMachine;
    private readonly StateMachine<TicketStatus, TicketAction>.TriggerWithParameters<string> _allocateTrigger;

    public Ticket(TicketStatus initialStatus = TicketStatus.Open)
    {
        _stateMachine = new StateMachine<TicketStatus, TicketAction>(initialStatus);
        _allocateTrigger = _stateMachine.SetTriggerParameters<string>(TicketAction.Allocate);

        _stateMachine.Configure(TicketStatus.Open)
            .Permit(TicketAction.Allocate, TicketStatus.Allocated)
            .Permit(TicketAction.Decline, TicketStatus.Declined)
            .Permit(TicketAction.Postpone, TicketStatus.Suspended);

        _stateMachine.Configure(TicketStatus.Allocated)
            .OnEntryFrom(_allocateTrigger, assignee => Console.WriteLine($"  Allocated to: {assignee}"))
            .Permit(TicketAction.Begin, TicketStatus.InWork)
            .Permit(TicketAction.Decline, TicketStatus.Declined)
            .Permit(TicketAction.Postpone, TicketStatus.Suspended);

        _stateMachine.Configure(TicketStatus.InWork)
            .Permit(TicketAction.Resolve, TicketStatus.Resolved)
            .Permit(TicketAction.Postpone, TicketStatus.Suspended);

        _stateMachine.Configure(TicketStatus.Resolved)
            .Permit(TicketAction.Accept, TicketStatus.Reviewed)
            .Permit(TicketAction.Reactivate, TicketStatus.Reopened);

        _stateMachine.Configure(TicketStatus.Reviewed)
            .Permit(TicketAction.Complete, TicketStatus.Done)
            .Permit(TicketAction.Reactivate, TicketStatus.Reopened);

        _stateMachine.Configure(TicketStatus.Done)
            .Permit(TicketAction.Reactivate, TicketStatus.Reopened);

        _stateMachine.Configure(TicketStatus.Reopened)
            .Permit(TicketAction.Allocate, TicketStatus.Allocated)
            .Permit(TicketAction.Decline, TicketStatus.Declined);

        _stateMachine.Configure(TicketStatus.Declined)
            .Permit(TicketAction.Restore, TicketStatus.Open);

        _stateMachine.Configure(TicketStatus.Suspended)
            .Permit(TicketAction.Restore, TicketStatus.Open);
    }

    public TicketStatus CurrentStatus => _stateMachine.State;

    public void AllocateTo(string assignee) => _stateMachine.Fire(_allocateTrigger, assignee);
    public void BeginWork() => _stateMachine.Fire(TicketAction.Begin);
    public void MarkResolved() => _stateMachine.Fire(TicketAction.Resolve);
    public void AcceptResolution() => _stateMachine.Fire(TicketAction.Accept);
    public void CompleteTicket() => _stateMachine.Fire(TicketAction.Complete);
    public void ReactivateTicket() => _stateMachine.Fire(TicketAction.Reactivate);
    public void DeclineTicket() => _stateMachine.Fire(TicketAction.Decline);
    public void PostponeTicket() => _stateMachine.Fire(TicketAction.Postpone);
    public void RestoreTicket() => _stateMachine.Fire(TicketAction.Restore);
}

public static class Program
{
    public static void Main()
    {
        Console.WriteLine("Ticket state machine demo");
        var ticket = new Ticket();
        Console.WriteLine($"Initial state: {ticket.CurrentStatus}");
        ticket.AllocateTo("Anna Smith");
        Console.WriteLine($"After allocate: {ticket.CurrentStatus}");
        ticket.BeginWork();
        Console.WriteLine($"After start work: {ticket.CurrentStatus}");
        ticket.MarkResolved();
        Console.WriteLine($"After resolve: {ticket.CurrentStatus}");
        ticket.AcceptResolution();
        Console.WriteLine($"After accept: {ticket.CurrentStatus}");
        ticket.CompleteTicket();
        Console.WriteLine($"After complete: {ticket.CurrentStatus}");
    }
}

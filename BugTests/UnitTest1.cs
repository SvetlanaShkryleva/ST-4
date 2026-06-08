using BugTracking;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace TicketTests;

[TestClass]
public class TicketWorkflowTests
{
    private Ticket _ticket = null!;

    [TestInitialize]
    public void PrepareTicket()
    {
        _ticket = new Ticket(Ticket.TicketStatus.Open);
    }

    [TestMethod]
    public void NewTicket_ShouldStartInOpenStatus()
    {
        Assert.AreEqual(Ticket.TicketStatus.Open, _ticket.CurrentStatus);
    }

    [TestMethod]
    public void Allocate_FromOpen_MovesToAllocated()
    {
        _ticket.AllocateTo("TesterABC");
        Assert.AreEqual(Ticket.TicketStatus.Allocated, _ticket.CurrentStatus);
    }

    [TestMethod]
    public void Decline_FromOpen_GoesToDeclined()
    {
        _ticket.DeclineTicket();
        Assert.AreEqual(Ticket.TicketStatus.Declined, _ticket.CurrentStatus);
    }

    [TestMethod]
    public void Postpone_FromOpen_GoesToSuspended()
    {
        _ticket.PostponeTicket();
        Assert.AreEqual(Ticket.TicketStatus.Suspended, _ticket.CurrentStatus);
    }

    [TestMethod]
    public void BeginWork_FromAllocated_TransitionsToInWork()
    {
        _ticket.AllocateTo("DeveloperX");
        _ticket.BeginWork();
        Assert.AreEqual(Ticket.TicketStatus.InWork, _ticket.CurrentStatus);
    }

    [TestMethod]
    public void Resolve_FromInWork_BecomesResolved()
    {
        _ticket.AllocateTo("Dev");
        _ticket.BeginWork();
        _ticket.MarkResolved();
        Assert.AreEqual(Ticket.TicketStatus.Resolved, _ticket.CurrentStatus);
    }

    [TestMethod]
    public void Accept_FromResolved_ChangesToReviewed()
    {
        _ticket.AllocateTo("Dev");
        _ticket.BeginWork();
        _ticket.MarkResolved();
        _ticket.AcceptResolution();
        Assert.AreEqual(Ticket.TicketStatus.Reviewed, _ticket.CurrentStatus);
    }

    [TestMethod]
    public void Complete_FromReviewed_EndsInDone()
    {
        _ticket.AllocateTo("Dev");
        _ticket.BeginWork();
        _ticket.MarkResolved();
        _ticket.AcceptResolution();
        _ticket.CompleteTicket();
        Assert.AreEqual(Ticket.TicketStatus.Done, _ticket.CurrentStatus);
    }

    [TestMethod]
    public void Reactivate_FromDone_ReturnsToReopened()
    {
        _ticket.AllocateTo("Dev");
        _ticket.BeginWork();
        _ticket.MarkResolved();
        _ticket.AcceptResolution();
        _ticket.CompleteTicket();
        _ticket.ReactivateTicket();
        Assert.AreEqual(Ticket.TicketStatus.Reopened, _ticket.CurrentStatus);
    }

    [TestMethod]
    public void Decline_FromAllocated_TransitionsToDeclined()
    {
        _ticket.AllocateTo("Tester");
        _ticket.DeclineTicket();
        Assert.AreEqual(Ticket.TicketStatus.Declined, _ticket.CurrentStatus);
    }

    [TestMethod]
    public void Restore_FromDeclined_GoesBackToOpen()
    {
        _ticket.DeclineTicket();
        _ticket.RestoreTicket();
        Assert.AreEqual(Ticket.TicketStatus.Open, _ticket.CurrentStatus);
    }

    [TestMethod]
    public void Restore_FromSuspended_ResetsToOpen()
    {
        _ticket.PostponeTicket();
        _ticket.RestoreTicket();
        Assert.AreEqual(Ticket.TicketStatus.Open, _ticket.CurrentStatus);
    }

    [TestMethod]
    public void Reactivate_FromResolved_GoesToReopened()
    {
        _ticket.AllocateTo("Dev");
        _ticket.BeginWork();
        _ticket.MarkResolved();
        _ticket.ReactivateTicket();
        Assert.AreEqual(Ticket.TicketStatus.Reopened, _ticket.CurrentStatus);
    }

    [TestMethod]
    public void Reactivate_FromReviewed_GoesToReopened()
    {
        _ticket.AllocateTo("Dev");
        _ticket.BeginWork();
        _ticket.MarkResolved();
        _ticket.AcceptResolution();
        _ticket.ReactivateTicket();
        Assert.AreEqual(Ticket.TicketStatus.Reopened, _ticket.CurrentStatus);
    }

    [TestMethod]
    public void Decline_FromReopened_MovesToDeclined()
    {
        _ticket.AllocateTo("Dev");
        _ticket.BeginWork();
        _ticket.MarkResolved();
        _ticket.AcceptResolution();
        _ticket.CompleteTicket();
        _ticket.ReactivateTicket();
        _ticket.DeclineTicket();
        Assert.AreEqual(Ticket.TicketStatus.Declined, _ticket.CurrentStatus);
    }

    [TestMethod]
    public void FullSequence_ShouldFinishInDone()
    {
        _ticket.AllocateTo("QA");
        _ticket.BeginWork();
        _ticket.MarkResolved();
        _ticket.AcceptResolution();
        _ticket.CompleteTicket();
        Assert.AreEqual(Ticket.TicketStatus.Done, _ticket.CurrentStatus);
    }

    [TestMethod]
    [ExpectedException(typeof(InvalidOperationException))]
    public void BeginWork_WhenOpen_ThrowsException()
    {
        _ticket.BeginWork();
    }

    [TestMethod]
    [ExpectedException(typeof(InvalidOperationException))]
    public void Resolve_WhenAllocated_NotAllowed()
    {
        _ticket.AllocateTo("Dev");
        _ticket.MarkResolved();
    }

    [TestMethod]
    [ExpectedException(typeof(InvalidOperationException))]
    public void Accept_WhenInWork_Throws()
    {
        _ticket.AllocateTo("Dev");
        _ticket.BeginWork();
        _ticket.AcceptResolution();
    }

    [TestMethod]
    [ExpectedException(typeof(InvalidOperationException))]
    public void Complete_WhenResolved_Throws()
    {
        _ticket.AllocateTo("Dev");
        _ticket.BeginWork();
        _ticket.MarkResolved();
        _ticket.CompleteTicket();
    }

    [TestMethod]
    [ExpectedException(typeof(InvalidOperationException))]
    public void Decline_WhenDone_Throws()
    {
        _ticket.AllocateTo("Dev");
        _ticket.BeginWork();
        _ticket.MarkResolved();
        _ticket.AcceptResolution();
        _ticket.CompleteTicket();
        _ticket.DeclineTicket();
    }

    [TestMethod]
    [ExpectedException(typeof(InvalidOperationException))]
    public void Reactivate_WhenOpen_Throws()
    {
        _ticket.ReactivateTicket();
    }

    [TestMethod]
    [ExpectedException(typeof(InvalidOperationException))]
    public void Complete_WhenReopened_Throws()
    {
        _ticket.AllocateTo("Dev");
        _ticket.BeginWork();
        _ticket.MarkResolved();
        _ticket.ReactivateTicket();
        _ticket.CompleteTicket();
    }

    [DataTestMethod]
    [DataRow(Ticket.TicketStatus.Allocated, Ticket.TicketAction.Begin, Ticket.TicketStatus.InWork)]
    [DataRow(Ticket.TicketStatus.Resolved, Ticket.TicketAction.Accept, Ticket.TicketStatus.Reviewed)]
    [DataRow(Ticket.TicketStatus.Reviewed, Ticket.TicketAction.Complete, Ticket.TicketStatus.Done)]
    [DataRow(Ticket.TicketStatus.Reopened, Ticket.TicketAction.Allocate, Ticket.TicketStatus.Allocated)]
    public void ValidTransitions_ShouldSucceed(Ticket.TicketStatus from, Ticket.TicketAction action, Ticket.TicketStatus to)
    {
        var ticket = new Ticket(from);
        switch (action)
        {
            case Ticket.TicketAction.Allocate: ticket.AllocateTo("TestUser"); break;
            case Ticket.TicketAction.Begin: ticket.BeginWork(); break;
            case Ticket.TicketAction.Resolve: ticket.MarkResolved(); break;
            case Ticket.TicketAction.Accept: ticket.AcceptResolution(); break;
            case Ticket.TicketAction.Complete: ticket.CompleteTicket(); break;
            case Ticket.TicketAction.Reactivate: ticket.ReactivateTicket(); break;
            case Ticket.TicketAction.Decline: ticket.DeclineTicket(); break;
            case Ticket.TicketAction.Postpone: ticket.PostponeTicket(); break;
            case Ticket.TicketAction.Restore: ticket.RestoreTicket(); break;
        }
        Assert.AreEqual(to, ticket.CurrentStatus);
    }
}

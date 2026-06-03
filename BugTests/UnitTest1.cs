using BugPro;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace BugTests;

[TestClass]
public class BugWorkflowTests
{
    private Bug _bug = null!;

    [TestInitialize]
    public void PrepareBug()
    {
        _bug = new Bug(Bug.BugStatus.New);
    }

    [TestMethod]
    public void NewlyCreatedBug_ShouldBeInNewStatus()
    {
        Assert.AreEqual(Bug.BugStatus.New, _bug.CurrentStatus);
    }

    [TestMethod]
    public void Assign_TransitionsFromNewToAssigned()
    {
        _bug.AssignTo("Tester99");
        Assert.AreEqual(Bug.BugStatus.Assigned, _bug.CurrentStatus);
    }

    [TestMethod]
    public void Reject_FromNew_MovesToRejected()
    {
        _bug.RejectBug();
        Assert.AreEqual(Bug.BugStatus.Rejected, _bug.CurrentStatus);
    }

    [TestMethod]
    public void Defer_FromNew_GoesToDeferred()
    {
        _bug.DeferBug();
        Assert.AreEqual(Bug.BugStatus.Deferred, _bug.CurrentStatus);
    }

    [TestMethod]
    public void StartWorking_FromAssigned_LeadsToInProgress()
    {
        _bug.AssignTo("Dev123");
        _bug.StartWorking();
        Assert.AreEqual(Bug.BugStatus.InProgress, _bug.CurrentStatus);
    }

    [TestMethod]
    public void Fix_FromInProgress_BecomesFixed()
    {
        _bug.AssignTo("Dev123");
        _bug.StartWorking();
        _bug.MarkAsFixed();
        Assert.AreEqual(Bug.BugStatus.Fixed, _bug.CurrentStatus);
    }

    [TestMethod]
    public void ConfirmFix_FromFixed_ChangesToVerified()
    {
        _bug.AssignTo("Dev123");
        _bug.StartWorking();
        _bug.MarkAsFixed();
        _bug.ConfirmFix();
        Assert.AreEqual(Bug.BugStatus.Verified, _bug.CurrentStatus);
    }

    [TestMethod]
    public void Close_FromVerified_EndsInClosed()
    {
        _bug.AssignTo("Dev123");
        _bug.StartWorking();
        _bug.MarkAsFixed();
        _bug.ConfirmFix();
        _bug.CloseBug();
        Assert.AreEqual(Bug.BugStatus.Closed, _bug.CurrentStatus);
    }

    [TestMethod]
    public void Reopen_FromClosed_ReturnsToReopened()
    {
        _bug.AssignTo("Dev123");
        _bug.StartWorking();
        _bug.MarkAsFixed();
        _bug.ConfirmFix();
        _bug.CloseBug();
        _bug.ReopenBug();
        Assert.AreEqual(Bug.BugStatus.Reopened, _bug.CurrentStatus);
    }

    [TestMethod]
    public void Reject_FromAssigned_TransitionsToRejected()
    {
        _bug.AssignTo("Dev123");
        _bug.RejectBug();
        Assert.AreEqual(Bug.BugStatus.Rejected, _bug.CurrentStatus);
    }

    [TestMethod]
    public void Reactivate_FromRejected_GoesBackToNew()
    {
        _bug.RejectBug();
        _bug.ActivateAgain();
        Assert.AreEqual(Bug.BugStatus.New, _bug.CurrentStatus);
    }

    [TestMethod]
    public void Reactivate_FromDeferred_ResetsToNew()
    {
        _bug.DeferBug();
        _bug.ActivateAgain();
        Assert.AreEqual(Bug.BugStatus.New, _bug.CurrentStatus);
    }

    [TestMethod]
    public void Reopen_FromFixed_GoesToReopened()
    {
        _bug.AssignTo("Dev123");
        _bug.StartWorking();
        _bug.MarkAsFixed();
        _bug.ReopenBug();
        Assert.AreEqual(Bug.BugStatus.Reopened, _bug.CurrentStatus);
    }

    [TestMethod]
    public void Reopen_FromVerified_GoesToReopened()
    {
        _bug.AssignTo("Dev123");
        _bug.StartWorking();
        _bug.MarkAsFixed();
        _bug.ConfirmFix();
        _bug.ReopenBug();
        Assert.AreEqual(Bug.BugStatus.Reopened, _bug.CurrentStatus);
    }

    [TestMethod]
    public void Reject_FromReopened_MovesToRejected()
    {
        _bug.AssignTo("Dev123");
        _bug.StartWorking();
        _bug.MarkAsFixed();
        _bug.ConfirmFix();
        _bug.CloseBug();
        _bug.ReopenBug();
        _bug.RejectBug();
        Assert.AreEqual(Bug.BugStatus.Rejected, _bug.CurrentStatus);
    }

    [TestMethod]
    public void FullSequence_ShouldFinishInClosed()
    {
        _bug.AssignTo("QA");
        _bug.StartWorking();
        _bug.MarkAsFixed();
        _bug.ConfirmFix();
        _bug.CloseBug();
        Assert.AreEqual(Bug.BugStatus.Closed, _bug.CurrentStatus);
    }

    [TestMethod]
    [ExpectedException(typeof(InvalidOperationException))]
    public void StartWorking_WhenNew_ThrowsException()
    {
        _bug.StartWorking();
    }

    [TestMethod]
    [ExpectedException(typeof(InvalidOperationException))]
    public void Fix_WhenAssigned_NotAllowed()
    {
        _bug.AssignTo("Dev");
        _bug.MarkAsFixed();
    }

    [TestMethod]
    [ExpectedException(typeof(InvalidOperationException))]
    public void ConfirmFix_WhenInProgress_Throws()
    {
        _bug.AssignTo("Dev");
        _bug.StartWorking();
        _bug.ConfirmFix();
    }

    [TestMethod]
    [ExpectedException(typeof(InvalidOperationException))]
    public void Close_WhenFixed_Throws()
    {
        _bug.AssignTo("Dev");
        _bug.StartWorking();
        _bug.MarkAsFixed();
        _bug.CloseBug();
    }

    [TestMethod]
    [ExpectedException(typeof(InvalidOperationException))]
    public void Reject_WhenClosed_Throws()
    {
        _bug.AssignTo("Dev");
        _bug.StartWorking();
        _bug.MarkAsFixed();
        _bug.ConfirmFix();
        _bug.CloseBug();
        _bug.RejectBug();
    }

    [TestMethod]
    [ExpectedException(typeof(InvalidOperationException))]
    public void Reopen_WhenNew_Throws()
    {
        _bug.ReopenBug();
    }

    [TestMethod]
    [ExpectedException(typeof(InvalidOperationException))]
    public void Close_WhenReopened_Throws()
    {
        _bug.AssignTo("Dev");
        _bug.StartWorking();
        _bug.MarkAsFixed();
        _bug.ReopenBug();
        _bug.CloseBug();
    }

    [DataTestMethod]
    [DataRow(Bug.BugStatus.Assigned, Bug.BugAction.StartWork, Bug.BugStatus.InProgress)]
    [DataRow(Bug.BugStatus.Fixed, Bug.BugAction.Confirm, Bug.BugStatus.Verified)]
    [DataRow(Bug.BugStatus.Verified, Bug.BugAction.Close, Bug.BugStatus.Closed)]
    [DataRow(Bug.BugStatus.Reopened, Bug.BugAction.Assign, Bug.BugStatus.Assigned)]
    public void ValidTransitions_ShouldSucceed(Bug.BugStatus from, Bug.BugAction action, Bug.BugStatus to)
    {
        var bug = new Bug(from);
        switch (action)
        {
            case Bug.BugAction.Assign: bug.AssignTo("Test"); break;
            case Bug.BugAction.StartWork: bug.StartWorking(); break;
            case Bug.BugAction.Fix: bug.MarkAsFixed(); break;
            case Bug.BugAction.Confirm: bug.ConfirmFix(); break;
            case Bug.BugAction.Close: bug.CloseBug(); break;
            case Bug.BugAction.Reopen: bug.ReopenBug(); break;
            case Bug.BugAction.Reject: bug.RejectBug(); break;
            case Bug.BugAction.Defer: bug.DeferBug(); break;
            case Bug.BugAction.Renew: bug.ActivateAgain(); break;
        }
        Assert.AreEqual(to, bug.CurrentStatus);
    }
}

using System;
using System.Collections.Generic;
using LTExercise.LogAnalysis;
using Microsoft.AspNetCore.Http.Features;
using Moq;
using Xunit;
using Xunit.Abstractions;

namespace LTExerciseTest;

public class LtExerciseUnitTests
{
    private readonly ITestOutputHelper _testOutputHelper;

    public LtExerciseUnitTests(ITestOutputHelper testOutputHelper)
    {
        _testOutputHelper = testOutputHelper;
    }
    [Fact]
    public void OverLappingLogins_ReturnsSharedSessions()
    {
        //arrange
        var expected1 = new SharedSessionResponse("alice,bob", 250, 300);
        var expected2 = new SharedSessionResponse("alice,bob", 1300, 1350);

        var repo = new Mock<ILogRepository>();
        var logLists = new List<LogRecord>()
        {
            new LogRecord("alice", 100, "login"),
            new LogRecord("alice", 300, "logout"),
            new LogRecord("alice", 1300, "login"),
            new LogRecord("alice", 1800, "logout"),
            new LogRecord("bob", 250, "login"),
            new LogRecord("bob", 1350, "logout"),
            new LogRecord("carol", 100, "login"),
            new LogRecord("carol", 500, "logout"),
            new LogRecord("charlie", 350, "login"),
            new LogRecord("charlie", 2400, "logout"),
            new LogRecord("frank", 1320, "login"),
            new LogRecord("frank", 2400, "logout"),
        };
        repo.Setup(r => r.GetLogs()).Returns(logLists);
        var logService = new LogService(repo.Object);
        //act
        var sharedSessions = logService.GetMaxSharedUserSessions(0, 2401);
        //assert
        Assert.Equal(2, sharedSessions.Count);
        Assert.Equal(expected1, sharedSessions[0]);
        Assert.Equal(expected2, sharedSessions[1]);
    }
    [Fact]
    public void SameLoginTime_ReturnsSharedSession()
    {
        //arrange
        var expected1 = new SharedSessionResponse("charlie,frank", 1320, 1400);
        var expected2 = new SharedSessionResponse("charlie,frank", 2000, 2200);

        var repo = new Mock<ILogRepository>();
        var logLists = new List<LogRecord>()
        {
            new LogRecord("alice", 100, "login"),
            new LogRecord("alice", 300, "logout"),
            new LogRecord("alice", 1350, "login"),
            new LogRecord("alice", 1800, "logout"),
            new LogRecord("bob", 250, "login"),
            new LogRecord("bob", 1350, "logout"),
            new LogRecord("carol", 100, "login"),
            new LogRecord("carol", 500, "logout"),
            new LogRecord("charlie", 1320, "login"),
            new LogRecord("charlie", 1400, "logout"),
            new LogRecord("charlie", 2000, "login"),
            new LogRecord("charlie", 2200, "logout"),
            new LogRecord("frank", 1320, "login"),
            new LogRecord("frank", 2400, "logout"),
            
        };
        repo.Setup(r => r.GetLogs()).Returns(logLists);
        var logService = new LogService(repo.Object);
        //act
        var sharedSessions = logService.GetMaxSharedUserSessions(0, 2401);
        //assert
        Assert.Equal(2, sharedSessions.Count);
        Assert.Equal(expected1, sharedSessions[0]);
        Assert.Equal(expected2, sharedSessions[1]);
    }
    [Fact]
    public void SameLoginLogoutTime_ReturnsNoSharedSession()
    {
        //arrange
        var repo = new Mock<ILogRepository>();
        var logLists = new List<LogRecord>()
        {
            new LogRecord("alice", 100, "login"),
            new LogRecord("alice", 300, "logout"),
            new LogRecord("alice", 1350, "login"),
            new LogRecord("alice", 1800, "logout"),
            new LogRecord("bob", 250, "login"),
            new LogRecord("bob", 1350, "logout"),
            new LogRecord("carol", 100, "login"),
            new LogRecord("carol", 500, "logout"),
            new LogRecord("charlie", 1320, "login"),
            new LogRecord("charlie", 1400, "logout"),
            new LogRecord("frank", 1320, "login"),
            new LogRecord("frank", 2400, "logout"),
            
        };
        repo.Setup(r => r.GetLogs()).Returns(logLists);
        var logService = new LogService(repo.Object);
        //act
        var sharedSessions = logService.GetMaxSharedUserSessions(0, 2401);
        //assert
        Assert.Empty(sharedSessions);
    }
}
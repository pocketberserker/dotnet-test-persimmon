﻿using Microsoft.Extensions.Testing.Abstractions;
using System;
using System.Linq;

namespace Persimmon.Runner
{
    public class TestResultWrapper
    {
        private readonly dynamic testResult;
        private readonly TestCaseWrapper testCase;
        private readonly DateTimeOffset endTime;

        public TestResultWrapper(object testResult)
        {
            this.testResult = testResult;
            testCase = new TestCaseWrapper(this.testResult.TestCase);
            endTime = DateTimeOffset.Now;
        }

        public Exception[] Exceptions
        {
            get
            {
                return testResult.Exceptions;
            }
        }

        public TestResult TestResult
        {
            get
            {
                var result = new TestResult(testCase.Test)
                {
                    DisplayName = testCase.Test.DisplayName,
                    Duration = testResult.Duration,
                    EndTime = endTime,
                    ComputerName = Environment.MachineName
                };
                Exception[] exns = Exceptions;
                string[] failures = testResult.FailureMessages;
                string[] skips = testResult.SkipMessages;
                if (exns.Any())
                {
                    result.Outcome = TestOutcome.Failed;
                    result.ErrorMessage = exns[0].Message;
                    result.ErrorStackTrace = exns[0].StackTrace;
                }
                else if (skips.Any())
                {
                    result.Outcome = TestOutcome.Skipped;
                    foreach (var msg in skips) result.Messages.Add(msg);
                }
                else if (failures.Any())
                {
                    result.Outcome = TestOutcome.Failed;
                    foreach (var msg in failures) result.Messages.Add(msg);
                }
                else result.Outcome = TestOutcome.Passed;
                return result;
            }
        }
    }
}

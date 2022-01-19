﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.TestPlatform.ObjectModel;
using Microsoft.VisualStudio.TestPlatform.ObjectModel.Adapter;
using YamlDotNet.RepresentationModel;

namespace TestAdapterTest
{
    public class YamlTestCaseRunner
    {
        public static TestOutcome RunAndRecordTestCase(TestCase test, IFrameworkHandle frameworkHandle)
        {
            RunTest(test, out var stdOut, out var stdErr, out var additional, out var debugTrace, out var outcome);
            RecordResult(test, frameworkHandle, stdOut, stdErr, additional, debugTrace, outcome);
            return outcome;
        }

        #region private methods
        
        private static TestOutcome RunTest(TestCase test, out string stdOut, out string stdErr, out string additional, out string debugTrace, out TestOutcome outcome)
        {
            var command = TestProperties.Get(test, "command");
            var script = TestProperties.Get(test, "script");
            var expect = TestProperties.Get(test, "expect");
            var notExpect = TestProperties.Get(test, "not-expect");
            var logExpect = TestProperties.Get(test, "log-expect");
            var logNotExpect = TestProperties.Get(test, "log-not-expect");

            var simulate = TestProperties.Get(test, "simulate");
            return string.IsNullOrEmpty(simulate)
                ? RunTestCase(test, command, script, expect, notExpect, logExpect, logNotExpect, out stdOut, out stdErr, out additional, out debugTrace, out outcome)
                : SimulateTestCase(test, simulate, command, script, expect, notExpect, logExpect, logNotExpect, out stdOut, out stdErr, out additional, out debugTrace, out outcome);
        }

        private static TestOutcome RunTestCase(TestCase test, string command, string script, string expect, string notExpect, string logExpect, string logNotExpect, out string stdOut, out string stdErr, out string additional, out string debugTrace, out TestOutcome outcome)
        {
            // TODO: Actually run the test case here...
            stdOut = stdErr = additional = debugTrace = null;
            return outcome = TestOutcome.Skipped;
        }

        private static TestOutcome SimulateTestCase(TestCase test, string simulate, string command, string script, string expect, string notExpect, string logExpect, string logNotExpect, out string stdOut, out string stdErr, out string additional, out string debugTrace, out TestOutcome outcome)
        {
            var sb = new StringBuilder();
            sb.AppendLine($"command='{command?.Replace("\n", "\\n")}'");
            sb.AppendLine($"script='{script?.Replace("\n", "\\n")}'");
            sb.AppendLine($"expect='{expect?.Replace("\n", "\\n")}'");
            sb.AppendLine($"not-expect='{notExpect?.Replace("\n", "\\n")}'");
            sb.AppendLine($"log-expect='{logExpect?.Replace("\n", "\\n")}'");
            sb.AppendLine($"log-not-expect='{logNotExpect?.Replace("\n", "\\n")}'");
            stdOut = sb.ToString();
            stdErr = "STDERR";
            additional = "ADDITIONAL-INFO";
            debugTrace = "DEBUG-TRACE";

            outcome = simulate.ToLower() == "passed" ? TestOutcome.Passed : TestOutcome.Failed;
            if (outcome == TestOutcome.Passed)
            {
                stdErr = null;
                debugTrace = null;
            }

            return outcome;
        }

        private static void RecordResult(TestCase test, IFrameworkHandle frameworkHandle, string stdOut, string stdErr, string additional, string debugTrace, TestOutcome outcome)
        {
            var result = new TestResult(test) { Outcome = outcome };
            result.Messages.Add(new TestResultMessage(TestResultMessage.StandardOutCategory, stdOut));
            result.Messages.Add(new TestResultMessage(TestResultMessage.StandardErrorCategory, stdErr));
            result.Messages.Add(new TestResultMessage(TestResultMessage.AdditionalInfoCategory, additional));
            result.Messages.Add(new TestResultMessage(TestResultMessage.DebugTraceCategory, debugTrace));

            frameworkHandle.RecordResult(result);
        }

        #endregion
    }
}
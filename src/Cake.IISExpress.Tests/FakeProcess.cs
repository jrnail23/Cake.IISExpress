using System;
using System.Collections.Generic;
using System.Linq;
using Cake.Core.IO;

namespace Cake.IISExpress.Tests
{
    public class FakeProcess : IProcess
    {
        private Action<ProcessOutputReceivedEventArgs> _handleErrorOutputAction;

        private Action<ProcessExitedEventArgs> _handleExitedAction;

        private Action<ProcessOutputReceivedEventArgs> _handleStandardOutputAction;

        public virtual void Dispose()
        {
        }

        public virtual void WaitForExit()
        {
        }

        public virtual bool WaitForExit(int milliseconds)
        {
            return true;
        }

        public virtual int GetExitCode()
        {
            return 0;
        }

        public virtual IEnumerable<string> GetStandardOutput()
        {
            return Enumerable.Empty<string>();
        }

        public virtual IEnumerable<string> GetStandardError()
        {
            return Enumerable.Empty<string>();
        }

        public virtual void Kill()
        {
        }

        public virtual void HandleExited(Action<ProcessExitedEventArgs> action)
        {
            _handleExitedAction = action;
        }

        public virtual void HandleErrorOutput(Action<ProcessOutputReceivedEventArgs> action)
        {
            _handleErrorOutputAction = action;
        }

        public virtual void HandleStandardOutput(Action<ProcessOutputReceivedEventArgs> action)
        {
            _handleStandardOutputAction = action;
        }

        public virtual int ProcessId { get; set; }
        public virtual bool HasExited { get; set; }

        public virtual void TriggerExited(int exitCode)
        {
            if (_handleExitedAction != null)
            {
                _handleExitedAction(new ProcessExitedEventArgs(exitCode));
            }
        }

        public virtual void TriggerErrorOutput(string errorOutput)
        {
            if (_handleErrorOutputAction != null)
            {
                _handleErrorOutputAction(new ProcessOutputReceivedEventArgs(errorOutput));
            }
        }

        public virtual void TriggerStandardOutput(string standardOutput)
        {
            if (_handleStandardOutputAction != null)
            {
                _handleStandardOutputAction(new ProcessOutputReceivedEventArgs(standardOutput));
            }
        }
    }
}
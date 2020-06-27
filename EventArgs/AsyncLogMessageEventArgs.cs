using DSharpPlus;
using DSharpPlus.EventArgs;
using System;

namespace sisbase.EventArgs {
    public class AsyncLogMessageEventArgs : AsyncEventArgs {
        //
        // Summary:
        //     Gets the level of the message.
        public LogLevel Level { get; internal set; }
        //
        // Summary:
        //     Gets the name of the application which generated the message.
        public string Application { get; internal set; }
        //
        // Summary:
        //     Gets the sent message.
        public string Message { get; internal set; }
        //
        // Summary:
        //     Gets the exception of the message.
        public Exception Exception { get; internal set; }
        //
        // Summary:
        //     Gets the timestamp of the message.
        public DateTime Timestamp { get; internal set; }
        internal AsyncLogMessageEventArgs(DebugLogMessageEventArgs dspargs) {
            Level = dspargs.Level;
            Application = dspargs.Application;
            Message = dspargs.Message;
            Exception = dspargs.Exception;
            Timestamp = dspargs.Timestamp;
        }
    }
}

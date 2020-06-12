using System;
using StripeTerminal;

namespace Xamarin.Stripe.Terminal.Forms
{
    public class TerminalServiceReaderDisplay : SCPReaderDisplayDelegate
    {
        private readonly Action<string> NotifyHandler;

        public TerminalServiceReaderDisplay(Action<string> notifyHandler)
        {
            NotifyHandler = notifyHandler;
        }

        public override void DidRequestReaderDisplayMessage(SCPTerminal terminal, SCPReaderDisplayMessage displayMessage)
        {
            var message = SCPTerminal.StringFromReaderDisplayMessage(displayMessage);
            NotifyHandler(message);
        }

        public override void DidRequestReaderInput(SCPTerminal terminal, SCPReaderInputOptions inputOptions)
        {
            var message = SCPTerminal.StringFromReaderInputOptions(inputOptions);
            NotifyHandler(message);
        }
    }

    public class TerminalServiceTerminal : SCPTerminalDelegate
    {
        private readonly Action<string> NotifyHandler;

        public TerminalServiceTerminal(Action<string> notifyHandler)
        {
            NotifyHandler = notifyHandler;
        }

        public override void Terminal(SCPTerminal terminal, SCPReader reader)
        {
            NotifyHandler("Disconnected");
        }
    }

    public class ReaderSoftwareUpdate : SCPReaderSoftwareUpdateDelegate
    {
        private readonly Action<float> NotifyHandler;

        public ReaderSoftwareUpdate(Action<float> notifyHandler)
        {
            NotifyHandler = notifyHandler;
        }

        public override void DidReportReaderSoftwareUpdateProgress(SCPTerminal terminal, float progress)
        {
            NotifyHandler(progress);
        }
    }
}

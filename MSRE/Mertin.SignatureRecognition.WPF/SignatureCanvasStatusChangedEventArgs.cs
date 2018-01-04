using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Mertin.SignatureRecognition.WPF
{
    /// <summary>
    /// The event argumets object for the SignatureCanvas.StatusChanged event.
    /// </summary>
    public class SignatureCanvasStatusChangedEventArgs : RoutedEventArgs
    {
        SignatureCanvasStatus status;
        internal SignatureCanvasStatusChangedEventArgs(RoutedEvent routedEvent, SignatureCanvasStatus status)
            : base(routedEvent)
        {
            this.status = status;
        }
        /// <summary>
        /// Gets the new status of the SignatureCanvas control.
        /// </summary>
        public SignatureCanvasStatus Status
        {
            get { return status; }
        }
    }
}

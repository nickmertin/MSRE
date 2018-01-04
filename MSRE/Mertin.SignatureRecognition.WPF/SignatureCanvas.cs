using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Ink;
using System.Windows.Input;

namespace Mertin.SignatureRecognition.WPF
{
    /// <summary>
    /// Enables a user to input a signature using an InkCanvas.
    /// </summary>
    [TemplatePart(Name = "PART_Start", Type = typeof(ButtonBase))]
    [TemplatePart(Name = "PART_Go", Type = typeof(ButtonBase))]
    [TemplatePart(Name = "PART_Draw", Type = typeof(InkCanvas))]
    [TemplateVisualState(Name = "WaitingForStart", GroupName = "SignatureInputStates")]
    [TemplateVisualState(Name = "Drawing", GroupName = "SignatureInputStates")]
    [TemplateVisualState(Name = "Done", GroupName = "SignatureInputStates")]
    public unsafe sealed class SignatureCanvas : Control
    {
        static readonly DependencyPropertyKey StatusKey = DependencyProperty.RegisterReadOnly("Status", typeof(SignatureCanvasStatus), typeof(SignatureCanvas), new PropertyMetadata(SignatureCanvasStatus.WaitingForStart));
        /// <summary>
        /// Represents the AutoReset dependency property.
        /// </summary>
        static public readonly DependencyProperty AutoResetProperty = DependencyProperty.Register("AutoReset", typeof(bool), typeof(SignatureCanvas));
        /// <summary>
        /// Represents the Status readonly dependency property.
        /// </summary>
        static public readonly DependencyProperty StatusProperty = StatusKey.DependencyProperty;
        /// <summary>
        /// Represents the Resetting routed event.
        /// </summary>
        static public readonly RoutedEvent ResettingEvent = EventManager.RegisterRoutedEvent("Resetting", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(SignatureCanvas));
        /// <summary>
        /// Represents the AutoResetChanged routed event.
        /// </summary>
        static public readonly RoutedEvent AutoResetChangedEvent = EventManager.RegisterRoutedEvent("AutoResetChanged", RoutingStrategy.Bubble, typeof(EventHandler<SignatureCanvasStatusChangedEventArgs>), typeof(SignatureCanvas));
        /// <summary>
        /// Represents the StatusChanged routed event.
        /// </summary>
        static public readonly RoutedEvent StatusChangedEvent = EventManager.RegisterRoutedEvent("StatusChanged", RoutingStrategy.Bubble, typeof(SignatureCanvasStatusChangedEventArgs), typeof(SignatureCanvas));
        ButtonBase _start, _go;
        InkCanvas _draw;
        Signature? s;
        DateTime dt;
        List<IntPtr> ts;
        /// <summary>
        /// Creates a new instance of the SignatureCanvas control.
        /// </summary>
        public SignatureCanvas()
        {
            (_start = GetTemplateChild("PART_Start") as ButtonBase).Click += Start;
            (_go = GetTemplateChild("PART_Go") as ButtonBase).Click += Finish;
            (_draw = GetTemplateChild("PART_Draw") as InkCanvas).StrokeCollected += Draw;
            _draw.EditingMode = InkCanvasEditingMode.Ink;
            _draw.EditingModeInverted = InkCanvasEditingMode.None;
        }
        private void Start(object sender, RoutedEventArgs e)
        {
            ts = new List<IntPtr>();
            dt = DateTime.Now;
            Status = SignatureCanvasStatus.Drawing;
        }
        private void Finish(object sender, RoutedEventArgs e)
        {
            Status = SignatureCanvasStatus.Done;
            Signature.SignatureData* data = stackalloc Signature.SignatureData[_draw.Strokes.Count];
            double* _d = stackalloc double[5];
            bool* _b = stackalloc bool[2];
            ulong* _u = stackalloc ulong[2];
            for (int i = 0; i < _draw.Strokes.Count; ++i)
            {
                data[i] = new Signature.SignatureData();
                for (int j = 2; j < _draw.Strokes[i].StylusPoints.Count - 1; ++j)
                    if ((Math.Abs(Util.GetAngle(_draw.Strokes[i].StylusPoints[j - 2].ToPoint(), _draw.Strokes[i].StylusPoints[j - 1].ToPoint()) - Util.GetAngle(_draw.Strokes[i].StylusPoints[j - 1].ToPoint(), _draw.Strokes[i].StylusPoints[j].ToPoint())) < Config.at & Math.Abs(_draw.Strokes[i].StylusPoints[j - 1].PressureFactor - _draw.Strokes[i].StylusPoints[j].PressureFactor) < Config.pt) | (_draw.Strokes[i].StylusPoints[j - 1].X == _draw.Strokes[i].StylusPoints[j].X & _draw.Strokes[i].StylusPoints[j - 1].Y == _draw.Strokes[i].StylusPoints[j].Y))
                    {
                        _draw.Strokes[i].StylusPoints.RemoveAt(j);
                        --j;
                    }
                data[i].TimeOffset = (*(TimeSpan*)ts[i]).TotalMilliseconds;
                data[i].StartingAngle = Util.GetAngle(_draw.Strokes[i].StylusPoints[0].ToPoint(), _draw.Strokes[i].StylusPoints[1].ToPoint());
                data[i].StartingPressure = _draw.Strokes[i].StylusPoints[0].PressureFactor;
                _d[0] = _d[1] = _d[3] = 0;
                _d[2] = Util.GetAngle(_draw.Strokes[i].StylusPoints[0].ToPoint(), _draw.Strokes[i].StylusPoints[1].ToPoint());
                _d[4] = _draw.Strokes[i].StylusPoints[0].PressureFactor;
                _b[0] = Util.GetAngle(_draw.Strokes[i].StylusPoints[0].ToPoint(), _draw.Strokes[i].StylusPoints[1].ToPoint()) > Util.GetAngle(_draw.Strokes[i].StylusPoints[1].ToPoint(), _draw.Strokes[i].StylusPoints[2].ToPoint());
                _b[1] = _draw.Strokes[i].StylusPoints[0].PressureFactor > _draw.Strokes[i].StylusPoints[1].PressureFactor;
                for (int j = 1; j < _draw.Strokes[i].StylusPoints.Count; ++j)
                {
                    *_d += Util.GetDistance(_draw.Strokes[i].StylusPoints[j - 1].ToPoint(), _draw.Strokes[i].StylusPoints[j].ToPoint());
                    if (j > 1)
                        _d[1] += Math.Abs(_d[2] - (_d[2] = Util.GetAngle(_draw.Strokes[i].StylusPoints[j - 1].ToPoint(), _draw.Strokes[i].StylusPoints[j].ToPoint())));
                    _d[2] += Math.Abs(_d[4] - (_d[4] = _draw.Strokes[i].StylusPoints[j].PressureFactor));
                    if (j > 1)
                        if (*_b != (*_b = Util.GetAngle(_draw.Strokes[i].StylusPoints[j - 2].ToPoint(), _draw.Strokes[i].StylusPoints[j - 1].ToPoint()) > Util.GetAngle(_draw.Strokes[i].StylusPoints[j - 1].ToPoint(), _draw.Strokes[i].StylusPoints[j].ToPoint())))
                            ++_u[0];
                    if (_b[1] != (_b[1] = _draw.Strokes[i].StylusPoints[0].PressureFactor > _draw.Strokes[i].StylusPoints[1].PressureFactor))
                        ++_u[1];
                }
                data[i].Length = *_d;
                data[i].AngleChange = _d[1];
                data[i].AngleWobble = _u[0];
                data[i].PressureChange = _d[3];
                data[i].PressureWobble = _u[1];
            }
            _d = null;
            _b = null;
            _u = null;
            s = new Signature(data, _draw.Strokes.Count);
            Status = SignatureCanvasStatus.Done;
            if (AutoReset)
                Reset();
        }
        private void Draw(object sender, InkCanvasStrokeCollectedEventArgs e)
        {
            TimeSpan* t = stackalloc TimeSpan[1];
            (*t) = DateTime.Now - dt;
            ts.Add((IntPtr)t);
        }
        protected override Size ArrangeOverride(Size arrangeBounds)
        {
            return new Size(Math.Max(arrangeBounds.Width, 100), Math.Max(arrangeBounds.Height, 60));
        }
        protected override Size MeasureOverride(Size constraint)
        {
            return new Size(Math.Max(constraint.Width, 100), Math.Max(constraint.Height, 60));
        }
        /// <summary>
        /// Returns the InkCanvas to its initial state. Clears the drawing canvas, but does not delete signature data form memory.
        /// </summary>
        public void Reset()
        {
            RaiseEvent(new RoutedEventArgs(ResettingEvent));
            _draw.Strokes.Clear();
        }
        /// <summary>
        /// Erases all signature data from memory.
        /// </summary>
        public void Erase()
        {
            s = null;
        }
        /// <summary>
        /// Gets or sets a value that indicates whether Reset should be called immediately after the signature is entered.
        /// </summary>
        public bool AutoReset
        {
            get { return (bool)GetValue(AutoResetProperty); }
            set
            {
                SetValue(AutoResetProperty, value);
                RaiseEvent(new RoutedEventArgs(AutoResetChangedEvent));
            }
        }
        /// <summary>
        /// Gets the current status of the SignatureCanvas.
        /// </summary>
        public SignatureCanvasStatus Status
        {
            get { return (SignatureCanvasStatus)GetValue(StatusProperty); }
            private set
            {
                SetValue(StatusKey, value);
                VisualStateManager.GoToState(this, value.ToString(), true);
                RaiseEvent(new SignatureCanvasStatusChangedEventArgs(StatusChangedEvent, Status));
            }
        }
        /// <summary>
        /// Gets the Signature object created by the last entered signature, or null if either no signature has been entered, or if it has been erased.
        /// </summary>
        public Signature? Signature
        {
            get { return s; }
        }
        /// <summary>
        /// Occurs when Reset is called, either manually or automatically.
        /// </summary>
        public event RoutedEventHandler Resetting
        {
            add { AddHandler(ResettingEvent, value); }
            remove { RemoveHandler(ResettingEvent, value); }
        }
        /// <summary>
        /// Occurs when the status of the SignatureCanvas, available through the Status property, changes.
        /// </summary>
        public event EventHandler<SignatureCanvasStatusChangedEventArgs> StatusChanged
        {
            add { AddHandler(StatusChangedEvent, value); }
            remove { RemoveHandler(StatusChangedEvent, value); }
        }
    }
    /// <summary>
    /// Defines the three possible states in which a SignatureCanvas can be.
    /// </summary>
    public enum SignatureCanvasStatus : byte
    {
        /// <summary>
        /// The SignatureCanvas is waiting for the &lt;Start&gt; button to be pressed. This is the initial state.
        /// </summary>
        WaitingForStart,
        /// <summary>
        /// The SignatureCanvas is accepting input.
        /// </summary>
        Drawing,
        /// <summary>
        /// The signature has been entered, and the SignatureCanvas is waiting to be reset.
        /// </summary>
        Done
    }
}

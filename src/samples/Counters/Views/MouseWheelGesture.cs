using System;
using System.Windows.Input;

namespace Views
{
    public class MouseWheelGesture : MouseGesture
    {
        protected MouseWheelGesture(Func<MouseWheelEventArgs, bool> predicate,
            ModifierKeys modifiers = ModifierKeys.None) : base(MouseAction.WheelClick, modifiers)
        {
            _predicate = predicate;
        }

        private readonly Func<MouseWheelEventArgs, bool> _predicate;

        public override bool Matches(object targetElement, InputEventArgs inputEventArgs)
        {
            if (!base.Matches(targetElement, inputEventArgs)) return false;
            var args = inputEventArgs as MouseWheelEventArgs;
            return args != null && _predicate(args);
        }
    }

    public class MouseWheelUp : MouseWheelGesture
    {
        public MouseWheelUp(ModifierKeys modifiers = ModifierKeys.None) : base(e => e.Delta > 0, modifiers)
        {
        }

        public MouseWheelUp() : this(ModifierKeys.None) { }
    }

    public class MouseWheelDown : MouseWheelGesture
    {
        public MouseWheelDown(ModifierKeys modifiers = ModifierKeys.None) : base(e => e.Delta < 0, modifiers)
        {
        }

        public MouseWheelDown() : this(ModifierKeys.None) { }
    }
}
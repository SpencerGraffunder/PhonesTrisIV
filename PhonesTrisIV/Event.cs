using System;
namespace PhonesTrisIV
{
    public class Event
    {
        EventType Type;
        public ControlType Control { get; private set; }
        public Event(EventType type, ControlType control)
        {
            this.Type = type;
            this.Control = control;
        }
    }
}

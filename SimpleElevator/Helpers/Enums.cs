using System.ComponentModel;

namespace SimpleElevator.Helpers
{
    public enum Direction
    {
        [Description("up")]
        Up = 1,
        [Description("down")]
        Down
    }

    public enum StopAction
    {
        [Description("Loading passengers")]
        Loading = 1,
        [Description("Unloading passengers")]
        Unloading,
        [Description("Idle")]
        Idle
    }

    public enum ElevatorCar {
        [Description("Car 1")]
        [Color(ConsoleColor.Blue)]
        One = 1,
        [Description("Car 2")]
        [Color(ConsoleColor.Yellow)]
        Two,
        [Description("Car 3")]
        [Color(ConsoleColor.Red)]
        Three,
        [Color(ConsoleColor.Green)]
        [Description("Car 4")]
        Four,
    }

    public static class EnumExtensions
    {
        public static string GetDescription(this Enum value)
        {
            var field = value.GetType().GetField(value.ToString());
            var attribute = (DescriptionAttribute)Attribute.GetCustomAttribute(field, typeof(DescriptionAttribute));
            return attribute == null ? value.ToString() : attribute.Description;
        }

        public static ConsoleColor GetColor(this Enum enumValue)
        {
            var type = enumValue.GetType();
            var memberInfo = type.GetMember(enumValue.ToString());
            if (memberInfo.Length > 0)
            {
                var attributes = memberInfo[0].GetCustomAttributes(typeof(ColorAttribute), false);
                if (attributes.Length > 0)
                {
                    return ((ColorAttribute)attributes[0]).Color;
                }
            }
            return ConsoleColor.White; // Default color
        }
    }

    [AttributeUsage(AttributeTargets.Field, Inherited = false, AllowMultiple = false)]
    sealed class ColorAttribute : Attribute
    {
        public ConsoleColor Color { get; }

        public ColorAttribute(ConsoleColor color)
        {
            this.Color = color;
        }
    }
}

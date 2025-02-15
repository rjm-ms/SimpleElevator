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
        [Name("Car 1")]
        [Color(ConsoleColor.Blue)]
        One = 1,
        [Name("Car 2")]
        [Color(ConsoleColor.Yellow)]
        Two,
        [Name("Car 3")]
        [Color(ConsoleColor.Magenta)]
        Three,
        [Name("Car 4")]
        [Color(ConsoleColor.Green)]
        Four,
    }

    public static class EnumExtensions
    {
        public static string GetName(this Enum value)
        {
            var field = value.GetType().GetField(value.ToString());
            var attribute = (NameAttribute)Attribute.GetCustomAttribute(field, typeof(NameAttribute));
            return attribute == null ? value.ToString() : attribute.Name;
        }

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

    [AttributeUsage(AttributeTargets.Field, Inherited = false, AllowMultiple = false)]
    sealed class NameAttribute : Attribute
    {
        public string Name { get; }

        public NameAttribute(string name)
        {
            this.Name = name;
        }
    }
}

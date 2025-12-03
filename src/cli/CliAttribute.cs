namespace Vidmake.src.cli {
    
    [AttributeUsage(AttributeTargets.Property)]
    public class CliOptionAttribute : Attribute
    {
        public string Name { get; }
        public string? ShortName { get; }
        public string? Description { get; }

        public CliOptionAttribute(string name, string? shortName = null, string? description = null)
        {
            Name = name;
            ShortName = shortName;
            Description = description;
        }
    }
}
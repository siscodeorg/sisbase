namespace sisbase.Utils
{
    /// <summary>
    /// The base interface for all systems
    /// </summary>

    public interface ISystem
    {
        string Name { get; set; }
        string Description { get; set; }
        bool Status { get; set; }
        void Activate();
        void Deactivate();

        /// <summary>
        /// Will be executed before any attachment (if exists)
        /// </summary>
        void Execute();
    }
}
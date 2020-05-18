namespace sisbase.Utils {
	/// <summary>
	/// The base interface for all systems
	/// </summary>

	public interface ISystem {
		/// <summary>
		/// Name of the system
		/// </summary>
		string Name { get; set; }

		/// <summary>
		/// Description of the system
		/// </summary>
		string Description { get; set; }

		/// <summary>
		/// Status of the system
		/// </summary>
		bool Status { get; set; }

		/// <summary>
		/// Pre-initiazation code.
		/// </summary>
		void Activate();

		/// <summary>
		/// Disables the system.
		/// </summary>
		void Deactivate();

		/// <summary>
		/// Will be executed before any attachment (if exists)
		/// </summary>
		void Execute();
	}
}
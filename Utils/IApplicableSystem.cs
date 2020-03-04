namespace sisbase.Utils
{
	/// <summary>
	/// If this is not implemented by a system, it will not be available for use.
	/// </summary>
	public interface ISystem
	{
		public string Name { get; set; }
		public string Description { get; set; }
		public bool Status { get; set; }
		public void Activate();
		public void Deactivate();
		/// <summary>
		/// Will be executed before any attachment (if exists)
		/// </summary>
		public void Execute();

	}
}
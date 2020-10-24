namespace K4os.BoolEx
{
	public class CanonicalResult
	{
		public bool? Constant { get; }

		protected CanonicalResult(bool? constant) { Constant = constant; }
		public CanonicalResult(bool constant) { Constant = constant; }

		public static CanonicalResult<T> Create<T>(bool constant) =>
			new CanonicalResult<T>(constant);

		public static CanonicalResult<T> Create<T>(T[][] data) => 
			new CanonicalResult<T>(data);
	}
	
	public class CanonicalResult<T>: CanonicalResult
	{
		public T[][] Expression { get; }

		public CanonicalResult(T[][] expression): base(null) { Expression = expression; }

		public CanonicalResult(bool constant): base(constant) { Expression = null; }
	}
}

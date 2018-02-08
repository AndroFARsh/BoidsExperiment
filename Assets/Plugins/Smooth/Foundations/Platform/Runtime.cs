using UnityEngine;
using System;

namespace Smooth.Platform {

	/// <summary>
	/// Helper class that provides information about the target platform.
	/// </summary>
	public static class Runtime {

		/// <summary>
		/// The target runtime platform.
		/// </summary>
		public static readonly RuntimePlatform Platform = Application.platform;

		/// <summary>
		/// The base platform of the target runtime.
		/// </summary>
		public static readonly BasePlatform BasePlatform = Platform.ToBasePlatform();

		/// <summary>
		/// True if the base platform supports JIT compilation; otherwise false.
		/// </summary>
		public static readonly bool HasJit = BasePlatform.HasJit();

		/// <summary>
		/// True if the base platform does not support JIT compilation; otherwise false.
		/// </summary>
		public static readonly bool NoJit = !HasJit;

	}
}
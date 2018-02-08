using UnityEngine;
using System;

namespace Smooth.Platform {

	/// <summary>
	/// Enumeration representing the base platforms for Unity builds.
	/// </summary>
	public enum BasePlatform {
		None = 0,
		Android = 100,
		Bb10 = 200,
		Flash = 300,
		Ios = 400,
		Linux = 500,
		Metro = 600,
		NaCl = 700,
		Osx = 800,
		Ps3 = 900,
		Tizen = 1000,
		Wii = 1100,
		Windows = 1200,
		Wp8 = 1300,
		Xbox360 = 1400,
        Ps4 = 1500,
        XboxOne = 1600,
        SmartTv = 1700,
        WebGl = 1800
	}

	/// <summary>
	/// Extension methods related to the runtime / base platform.
	/// </summary>
	public static class PlatformExtensions {

		/// <summary>
		/// Returns the base platform for the specified runtime platform.
		/// </summary>
		public static BasePlatform ToBasePlatform(this RuntimePlatform runtimePlatform) {
			switch (runtimePlatform) {
			case RuntimePlatform.IPhonePlayer:
				return BasePlatform.Ios;
			case RuntimePlatform.Android:
				return BasePlatform.Android;
			case RuntimePlatform.WindowsEditor:
			case RuntimePlatform.WindowsPlayer:
				return BasePlatform.Windows;
			case RuntimePlatform.OSXEditor:
			case RuntimePlatform.OSXPlayer:
				return BasePlatform.Osx;
			case RuntimePlatform.LinuxPlayer:
				return BasePlatform.Linux;
			case RuntimePlatform.XBOX360:
				return BasePlatform.Xbox360;
			case RuntimePlatform.PS3:
				return BasePlatform.Ps3;
#if !UNITY_3_5 && !UNITY_4_0 && !UNITY_4_1
			case RuntimePlatform.WP8Player:
				return BasePlatform.Wp8;
			case RuntimePlatform.WSAPlayerX86:
			case RuntimePlatform.WSAPlayerX64:
			case RuntimePlatform.WSAPlayerARM:
				return BasePlatform.Metro;
			case RuntimePlatform.BlackBerryPlayer:
				return BasePlatform.Bb10;
			case RuntimePlatform.TizenPlayer:
				return BasePlatform.Tizen;
            case RuntimePlatform.WebGLPlayer:
                return BasePlatform.WebGl;
#endif
			default:
				return BasePlatform.None;
			}
		}

		/// <summary>
		/// Returns true if the specified platform supports JIT compilation; otherwise, false.
		/// </summary>
		public static bool HasJit(this RuntimePlatform runtimePlatform) {
			return (
				runtimePlatform != RuntimePlatform.IPhonePlayer &&
				runtimePlatform != RuntimePlatform.PS3 &&
				runtimePlatform != RuntimePlatform.XBOX360 &&
				runtimePlatform != RuntimePlatform.PS4);
		}

		/// <summary>
		/// Returns true if the specified platform supports JIT compilation; otherwise, false.
		/// </summary>
		public static bool HasJit(this BasePlatform basePlatform) {
			return (
				basePlatform != BasePlatform.Ios &&
				basePlatform != BasePlatform.Ps3 &&
				basePlatform != BasePlatform.Xbox360 &&
				basePlatform != BasePlatform.Ps4);
		}

		/// <summary>
		/// Returns true if the specified platform does not support JIT compilation; otherwise, false.
		/// </summary>
		public static bool NoJit(this RuntimePlatform runtimePlatform) {
			return !HasJit(runtimePlatform);
		}
		
		/// <summary>
		/// Returns true if the specified platform does not support JIT compilation; otherwise, false.
		/// </summary>
		public static bool NoJit(this BasePlatform basePlatform) {
			return !HasJit(basePlatform);
		}
	}
}

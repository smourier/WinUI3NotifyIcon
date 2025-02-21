#if DEBUG
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Runtime.Versioning;

[assembly: AssemblyConfiguration("DEBUG")]
#else
[assembly: AssemblyConfiguration("RELEASE")]
#endif

[assembly: AssemblyTitle("WinUI3NotifyIcon")]
[assembly: AssemblyProduct("WinUI3NotifyIcon")]
[assembly: AssemblyCopyright("Copyright (C) 2024-2025 Simon Mourier. All rights reserved.")]
[assembly: AssemblyCulture("")]
[assembly: AssemblyDescription("Sample Code")]
[assembly: AssemblyCompany("Simon Mourier")]
[assembly: Guid("cb70ee43-76c3-4023-aa71-be04e24fa33d")]
[assembly: DisableRuntimeMarshalling]
[assembly: SupportedOSPlatform("Windows10.0.15063.0")]
[assembly: AssemblyVersion("1.1.0.0")]
[assembly: AssemblyFileVersion("1.1.0.0")]
[assembly: AssemblyInformationalVersion("1.1.0.0")]

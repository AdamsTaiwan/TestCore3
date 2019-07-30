# TestCore3
Solution to test on Core 3

This solution currently work worked with Core 3 Preview 6 with 0 errors/warnings.
After removing Preview 6 and installing Preview 7 the following errors occurred even after rebooting windows, removing obj/bin folders, do a solution clean, nuget restore and rebuild.

```
1>------ Build started: Project: AppBase0500, Configuration: Debug Any CPU ------
1>You are using a preview version of .NET Core. See: https://aka.ms/dotnet-core-preview
1>AppLoadContext.cs(12,36,12,37): warning CS8632: The annotation for nullable reference types should only be used in code within a '#nullable' annotations context.
1>BaseClasses\NotifyBase.cs(33,96,33,97): warning CS8632: The annotation for nullable reference types should only be used in code within a '#nullable' annotations context.
1>BaseClasses\WindowBaseApp.cs(258,40,258,41): warning CS8632: The annotation for nullable reference types should only be used in code within a '#nullable' annotations context.
1>BaseClasses\WindowBaseApp.cs(40,30,40,31): warning CS8632: The annotation for nullable reference types should only be used in code within a '#nullable' annotations context.
1>Utilities\Extensions.cs(29,24,29,25): warning CS8632: The annotation for nullable reference types should only be used in code within a '#nullable' annotations context.
1>Utilities\Extensions.cs(29,23,29,25): error CS8627: A nullable type parameter must be known to be a value type or non-nullable reference type. Consider adding a 'class', 'struct', or type constraint.
1>Utilities\Extensions.cs(63,78,63,79): warning CS8632: The annotation for nullable reference types should only be used in code within a '#nullable' annotations context.
1>Utilities\Extensions.cs(63,39,63,40): warning CS8632: The annotation for nullable reference types should only be used in code within a '#nullable' annotations context.
1>BaseClasses\WindowBaseApp.cs(42,24,42,25): warning CS8632: The annotation for nullable reference types should only be used in code within a '#nullable' annotations context.
1>Utilities\Extensions.cs(188,24,188,25): warning CS8632: The annotation for nullable reference types should only be used in code within a '#nullable' annotations context.
1>Utilities\Extensions.cs(188,23,188,25): error CS8627: A nullable type parameter must be known to be a value type or non-nullable reference type. Consider adding a 'class', 'struct', or type constraint.
1>Utilities\Extensions.cs(300,54,300,58): error CS1069: The type name 'Icon' could not be found in the namespace 'System.Drawing'. This type has been forwarded to assembly 'System.Drawing.Common, Version=4.0.1.0, Culture=neutral, PublicKeyToken=cc7b13ffcd2ddd51' Consider adding a reference to that assembly.
1>Done building project "AppBase0500_r4rs3tsd_wpftmp.csproj" -- FAILED.
2>------ Build started: Project: P01, Configuration: Debug Any CPU ------
2>CSC : error CS0006: Metadata file '...\TestCore3\Shared\AppBase0500\bin\Debug\netcoreapp3.0\AppBase0500.dll' could not be found
2>Done building project "P01_bu4noxjn_wpftmp.csproj" -- FAILED.
3>------ Build started: Project: ApStudio5, Configuration: Debug Any CPU ------
3>...\TestCore3\Hosts\ApStudio5\MainWindow.xaml(1,19): error MC3074: The tag 'WindowBaseApp' does not exist in XML namespace 'clr-namespace:AppBase0500;assembly=AppBase0500'. Line 1 Position 19.
3>Done building project "ApStudio5.csproj" -- FAILED.
========== Build: 0 succeeded, 3 failed, 0 up-to-date, 0 skipped ==========
```

the Output is same with both with Vs2019 16.2 and 16.3 and on different machines.
I left the solution with the errors as is.  While debugging I commented out the error code and tried to solve each problem but without success.

//////////////////////////////////////////////////////////////////////
// VERSION
//////////////////////////////////////////////////////////////////////

//////////////////////////////////////////////////////////////////////
// ARGUMENTS
//////////////////////////////////////////////////////////////////////

var target = Argument("target", "Default");
var configuration = Argument("configuration", "Release");
var toolpath = Argument("toolpath", @"");
var solutionName = "XNSubstitute.sln";
var mainDll = "TddXt.XNSubstitute.Root.dll";

var defaultNugetPackSettings = new DotNetPackSettings 
{
	IncludeSymbols = true,
	Configuration = "Release",
	OutputDirectory = "./nuget",
	ArgumentCustomization = args=>args.Append("--include-symbols -p:SymbolPackageFormat=snupkg")
};


//////////////////////////////////////////////////////////////////////
// PREPARATION
//////////////////////////////////////////////////////////////////////

// Define directories.
var buildDir = Directory("./build") + Directory(configuration);
var publishDir = Directory("./publish");
var srcDir = Directory("./src");
var buildNetStandardDir = buildDir + Directory("netstandard2.0");
var srcNetStandardDir = srcDir + Directory("netstandard2.0");

//////////////////////////////////////////////////////////////////////
// TASKS
//////////////////////////////////////////////////////////////////////

Task("Clean")
    .Does(() =>
{
	DotNetClean("./src/netstandard2.0/Root", new DotNetCleanSettings
     {
         Configuration = "Debug",
     });
	DotNetClean("./src/netstandard2.0/Root", new DotNetCleanSettings
     {
         Configuration = "Release",
     });
    CleanDirectory("./nuget");
});

Task("Build")
    .IsDependentOn("Clean")
    .Does(() =>
{
    DotNetBuild("./src/netstandard2.0/Root", new DotNetBuildSettings
     {
         Configuration = configuration,
         OutputDirectory = buildNetStandardDir,
     });
});

Task("Run-Unit-Tests")
    .IsDependentOn("Build")
    .Does(() =>
{
    var projectFiles = GetFiles(srcNetStandardDir.ToString() + "/**/*Specification.csproj");
    foreach(var file in projectFiles)
    {
        DotNetTest(file.FullPath, new DotNetTestSettings           
        {
           Configuration = configuration
        });
    }
});

Task("Pack")
    .IsDependentOn("Build")
    .Does(() => 
    {
		DotNetPack(srcNetStandardDir + File("Root"), defaultNugetPackSettings);
    });


//////////////////////////////////////////////////////////////////////
// TASK TARGETS
//////////////////////////////////////////////////////////////////////

Task("Default")
    .IsDependentOn("Build")
    .IsDependentOn("Run-Unit-Tests")
    .IsDependentOn("Pack");

//////////////////////////////////////////////////////////////////////
// EXECUTION
//////////////////////////////////////////////////////////////////////

RunTarget(target);
var target = Argument("target", "Build");
var configuration = Argument("configuration", "Release");

//////////////////////////////////////////////////////////////////////
// TASKS
//////////////////////////////////////////////////////////////////////

Task("Clean")
    .WithCriteria(c => HasArgument("rebuild"))
    .Does(() =>
{
    CleanDirectory($"./VoxelSpace/bin/{configuration}");
});

Task("Build")
    .IsDependentOn("Clean")
    .Does(() =>
{
    MSBuild("./VoxelSpace/VoxelSpace.csproj", new MSBuildSettings {
        Configuration = configuration,
    });
    MSBuild("./MonoGame/MonoGame.Framework/MonoGame.Framework.WindowsDX.csproj", new MSBuildSettings {
        Configuration = configuration,
    });
});

//////////////////////////////////////////////////////////////////////
// EXECUTION
//////////////////////////////////////////////////////////////////////

RunTarget(target);
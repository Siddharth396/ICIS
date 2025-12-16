#module nuget:?package=Cake.DotNetTool.Module&version=0.4.0
#tool dotnet:?package=GitVersion.Tool&Version=5.8.1

var target = Argument("target", "Default");
var configuration = Argument("configuration", "Release");

GitVersion version;
var solutionFolder="./";
var buildDir = Directory("./build");


Task("Clean")
    .Does(() =>
{
    CleanDirectory(buildDir);
    CleanDirectories("src/**/obj");
    CleanDirectories("src/**/bin");
});

Task("Test")
    .IsDependentOn("Clean")
	.Does(()=>{
	   DotNetCoreTest(solutionFolder, new DotNetCoreTestSettings
	   {
		 Configuration=configuration,
	   });
	});

Task("Version")
    .IsDependentOn("Test")
    .Does(() => 
    {
        version = GitVersion(new GitVersionSettings {
            NoFetch = true,
        });
        TeamCity.SetBuildNumber(version.AssemblySemVer);
    });

Task("Package")
    .IsDependentOn("Version")
    .Does( () =>
{
    var settings = new DotNetCorePackSettings
     {
         Configuration = "Release",
         OutputDirectory = buildDir,
         ArgumentCustomization = args => args
            .Append($"/p:Version={version.AssemblySemVer}")
     };

     DotNetCorePack("src/GenericKafkaProducer/Icis.GenericKafkaProducer.csproj", settings);   
     DotNetCorePack("src/SpecificKafkaProducer/Icis.SpecificKafkaProducer.csproj", settings);   
});

Task("Default")
    .IsDependentOn("Package");

RunTarget(target);

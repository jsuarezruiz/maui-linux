#r "nuget: Fsdk, Version=0.6.0"

let args = fsi.CommandLineArgs

Fsdk.Network.GetNugetPrereleaseVersionFromBaseVersion args.[1]
|> System.Console.WriteLine

# Using dotnet-trace speedscope integration

[Speedscope docs](https://github.com/jlfwong/speedscope#navigation)

To start collecting a trace, use:

`dotnet trace collect --name $name --output $name.nettrace`

If you want to collect information about the startup sequence of the process, use something like:
`DOTNET_DefaultDiagnosticPortSuspend=1 ./bin/net7.0/alloc`
`dotnet trace collect --name alloc --output alloc.nettrace --resume-runtime`

Then convert it to speedscope format:

`dotnet trace convert --format speedscope --output $name.json $name.nettrace`

Then open it in your favourite browser:

`open -a $BROWSER https://speedscope.app` and upload the trace.

Demo sample projects:
* cpu-spin - simplest possible report
* alloc - showcases allocations surface as `UNMANAGED_CODE_TIME`
* cpu-background - showcases the utility of navigating through threads
* cpu-wait - wait time and how to look into it

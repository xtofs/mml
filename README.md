
# Meta Modeling Language for OData CSDL 



## the `generator` project

the demo project
- parses a .mml file
- generates c# code to construct a model in memory

```sh
    dotnet watch run --project generator --no-hot-reload
```

## the `generated` project
- uses the generated code to built an CSDL model in memory
- writes the model as text in a tree like form
- writes a mermaid diagram of the constructed model

```sh
    dotnet watch run --project generated --no-hot-reload
```

# Meta Modeling Language for OData CSDL 



## the `demo` project

the demo project
- parses a .mml file
- generates c# code to construct a model in memory

## the `generated` project
- uses the generated code to built an CSDL model in memory
- writes the model as text in a tree like form
- writes a mermaid diagram of the constructed model

```sh
    dotnet watch run --project demo --no-hot-reload
```
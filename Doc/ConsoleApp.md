# Console app
VanillaTransformer ships also in the form of console app which can be used directly from the command line.  You can download it from the [Releases page](https://github.com/cezarypiatek/VanillaTransformer/releases). 


Here's the list of accepted options (they are the counterparts of MsBuildTask parameters):

`-t, --TransformConfiguration`  transformations.xml path

`-f, --ConfigurationFormat` format of the transform configuration file. Available options: 'deployment', 'default'

`-h, --PlaceholderPattern` placeholder pattern e.g.: ${KEY}

`-r, --ProjectRootPath` (optional) root folder path (exec path will be taken if not provided)

`-p, --PatternFile` pattern file path 

`-s, --ValuesSource` value file path

`-o, --OutputPath` output file path

`-?, --help` Print list of accepted parameters 

## Examples

Transform multiple pattern files using `TransformConfiguration` file:

```shell
VanillaTransformer.Console.exe --TransformConfiguration "../Configs/transformations.xml" --ConfigurationFormat "deployment"
```

Transform single pattern file:

```shell
VanillaTransformer.Console.exe -p "../Configs/Transformation settings\Web.pattern.config" -o "../Configs/Web.config" -s "../Configs\Transformation settings\Web.values.dev.config" -h "${KEY}"
```
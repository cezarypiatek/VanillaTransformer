The `Deployment` format of  `TransformConfiguration` file is an `XML` file that consists of the following sections: 

- `apps` (mandatory)
- `environments` (mandatory)
- `transformations` (mandatory)
- `postTransformations` (optional)

File structure should look as follows:

```xml
<root>
    <apps></apps>
    <environments></environments>
    <transformations></transformations>
    <postTransformations></postTransformations>
</root>
```

## `apps` section
The `Apps` section is responsible for defining sets of configuration files templates. Every `template` node have the following attributes:

- `name` (mandatory) - an unique name of the template inside a single application. Used later mainly for generating output file name.
- `pattern` (mandatory) - path to the configuration template file
- `placeholder` (optional) - allows for overriding placeholder pattern for specific template

The `Apps` section can contain one or more `app` nodes. 
An Example definition of templates can look as follows:

```xml
<root>
    <apps>
        <app name="SampleApp1">
            <templates>
                <template name="appsettings.json" pattern="appsettings.json.template" placeholder="[%KEY%]" />
                <template name="nlog.config" pattern="nlog.config.template" />
            </templates>
        </app>
    </apps>
</root>
```

## `environments` section

The `Environments` section allows for defining the structure of our deployment environments. Every `environment` node can contain `values` section which provides values for placeholder replacements. 
```xml
<root>
    <environments>
        <environment name="TEST">
            <values>
                <add key="Key1">TestValue1</add>
                <add key="Key2">TestValue1</add>
            </values>               
        </environment>
        <environment name="PROD">
            <values>
                <add key="Key1">ProdValue1</add>
                <add key="Key2">ProdValue1</add>
            </values>               
        </environment>
    </environments>
</root>
```

If we need to generate configuration per each machine, we can add additional level to our configuration by defining `machines` section. There is an option to override/add values on the machine level by adding `values` node.

```xml
<root>
    <environments>
        <environment name="TEST">
                <values></values>
                <machines>
                    <machine name="MACHINE-TEST-01">
                        <values></values>
                    </machine>
                    <machine name="MACHINE-TEST-02"></machine>                    
                </machines>
        </environment>
        <environment name="PROD">
                <values></values>
                <machines>
                    <machine name="MACHINE-PROD-01">
                        <values></values>
                    </machine>
                    <machine name="MACHINE-PROD-02"></machine>                    
                </machines>
        </environment>
    </environments>
</root>
```

If you have more thane one `app` defined in `apps` section, and you want to constrain deployment to specific environment or machine, you can do that by defining `apps` attribute on the given level. The `apps` attribute accept semicolon-separated list of the application names defined in the `apps` section. If there is no `apps` attribute, all the applications which haven't constrain so far in the current hierarchy will be used for config transformations for given environment/machine.

```xml
<root>
    <apps>
        <app name="SampleApp1"></app>
        <app name="SampleApp2"></app>
    </apps>
    <environments>
        <environment name="TEST">
                <machines>
                    <machine name="MACHINE-TEST-01" />
                    <machine name="MACHINE-TEST-02" apps="SampleApp1" />
                </machines>
        </environment>
        <environment name="PROD" apps="SampleApp2">
                <values></values>
                <machines>
                    <machine name="MACHINE-PROD-01">
                        <values></values>
                    </machine>
                    <machine name="MACHINE-PROD-02"></machine>                    
                </machines>
        </environment>
    </environments>
</root>
```

Using the example configuration from the above, VanillaTransformer prepares the following configuration sets:

- `SampleApp1\TEST\MACHINE-TEST-01`
- `SampleApp1\TEST\MACHINE-TEST-02`
- `SampleApp2\TEST\MACHINE-TEST-01`
- `SampleApp2\PROD\MACHINE-PROD-01`
- `SampleApp2\PROD\MACHINE-PROD-02`

## `transformations` section

The `transformations` section allows defining the output path for the transformed configurations in the form of path template. Every of the following tokes: `{app}`, `{environment}`, `{machine}` and `{template}` will be replaced with the name of the corresponding configuration element.

```xml
<root>
    <transformations>
        <transformation output="{app}\{environment}\{machine}\{template}" />
    </transformations>
</root>
```

There is also an option to output transformation results to `zip` archives:

```xml
<root>
    <transformations>
        <transformation archive="{app}\{environment}\{machine}.zip" output="{template}" />
    </transformations>
</root>
```

This will produce a set of zip archives with all config files per application/environment/machine.

## `postTransformations` section

```xml
<root>
    <postTransformations>
        <postTransformation fileExtension="config">
             <add name="StripXMLComments" />
             <add name="ReFormatXML" />
        </postTransformation>
        <postTransformation fileExtension="json">
             <add name="ReFormatJSON" />
        </postTransformation>
    </postTransformations>
</root>
```
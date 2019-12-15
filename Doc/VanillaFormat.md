**Example transformation configuration file**
```XML
<?xml version="1.0" encoding="utf-8" ?>
<root>
  <transformationGroup pattern="Configs\NHibernate.pattern.config">
    <transformation values="NHibernate.values.dev.config" output="NHibernate.config" />
    <transformation values="NHibernate.values.staging.config" output="Configs\Transformed\Staging\NHibernate.config" />
    <transformation values="NHibernate.values.prod.config" output="Configs\Transformed\Prod\NHibernate.config" />
  </transformationGroup>
  <transformationGroup pattern="Configs\Web.pattern.config">
    <transformation values="Web.values.dev.config" output="Web.config" />
    <transformation values="Web.values.staging.config" output="Configs\Transformed\Staging\Web.config" />
    <transformation values="Web.values.prod.config" output="Configs\Transformed\Prod\Web.config" />
  </transformationGroup>
</root>
```
You can also define inline values for transformation instead of putting it in separated files.
```XML
<?xml version="1.0" encoding="utf-8" ?>
<root>
  <transformationGroup pattern="Configs\NHibernate.pattern.config">
    <transformation output="NHibernate.config">
    	<values>
    		<ConnectionString>Data Source=localhost;Database=TestDB;Integrated Security=true;</ConnectionString>
    	</values>
    </transformation>
    <transformation output="Configs\Transformed\NHibernate.config">
    	<values>
    		<ConnectionString>Data Source=localhost;Database=StagingDB;Integrated Security=true;</ConnectionString>
    	</values>
    </transformation>
  </transformationGroup>
</root>
```

### Output to Zip archive
You can create zip archive with complete configuration files set for given environment after transformations using archive attribute as follows:
```XML
<?xml version="1.0" encoding="utf-8" ?>
<root>
  <transformationGroup pattern="Configs\NHibernate.pattern.config">
    <transformation values="NHibernate.values.dev.config" output="NHibernate.config" />
    <transformation values="NHibernate.values.test.config" output="NHibernate.config" archive="Configs\Transformed\Test.zip"/>
    <transformation values="NHibernate.values.staging.config" output="NHibernate.config" archive="Configs\Transformed\Staging.zip"/>
    <transformation values="NHibernate.values.prod.config" output="NHibernate.config" archive="Configs\Transformed\Prod.zip" />
  </transformationGroup>
  <transformationGroup pattern="Configs\Web.pattern.config">
    <transformation values="Web.values.dev.config" output="Web.config" />
    <transformation values="Web.values.test.config" output="Web.config" archive="Configs\Transformed\Test.zip" />
    <transformation values="Web.values.staging.config" output="Web.config" archive="Configs\Transformed\Staging.zip" />
    <transformation values="Web.values.prod.config" output="Web.config" archive="Configs\Transformed\Prod.zip" />
  </transformationGroup>
   <transformationGroup pattern="Configs\Log4net.pattern.config">
    <transformation values="log4net.values.dev.config" output="log4net.config" />
    <transformation values="log4net.values.test.config" output="log4net.config" archive="Configs\Transformed\Test.zip" />
    <transformation values="log4net.values.staging.config" output="log4net.config" archive="Configs\Transformed\Staging.zip" />
    <transformation values="log4net.values.prod.config" output="log4net.config" archive="Configs\Transformed\Prod.zip" />
  </transformationGroup>
</root>
```

this will create the following zip files
- Test.zip (NHibernate.config, Web.config, log4net.config)
- Staging.zip (NHibernate.config, Web.config, log4net.config) 
- Prod.zip (NHibernate.config, Web.config, log4net.config)
 
## Post-Transformations
VanillaTransformer supports post-transformations which are applied to transformed configuration in the form of pipeline.
You can add post-transformations on any level of VanillaTransformer configuration (root, transformationGroup or transformation level) in the following manner:

```XML
<?xml version="1.0" encoding="utf-8" ?>
<root>
  <postTransformations>
    <add name="StripXMLComments" />
    <add name="ReFormatXML" />
  </postTransformations>
  <transformationGroup pattern="Configs\NHibernate.pattern.config">
    <transformation values="Configs\NHibernate.values.dev.config" output="NHibernate.config" />
    <transformation values="Configs\NHibernate.values.prod.config" output="Configs\Transformed\NHibernate.prod.config" />
  </transformationGroup>
  <transformationGroup pattern="Configs\Web.pattern.config">
    <transformation values="Configs\Web.values.dev.config" output="Web.config" />
    <transformation values="Configs\Web.values.prod.config" output="Configs\Transformed\Web.prod.config" />
  </transformationGroup>
</root>
```

You can also modify post-transformations set defined on higher level using "add" , "remove", and "clear" tags as follows
```XML
<?xml version="1.0" encoding="utf-8" ?>
<root>
  <postTransformations>
    <add name="StripXMLComments" />
    <add name="ReFormatXML" />
  </postTransformations>
  <transformationGroup pattern="Configs\NHibernate.pattern.config">
    <transformation values="Configs\NHibernate.values.dev.config" output="NHibernate.config">
      <postTransformations>
        <remove name="StripXMLComments" />
      </postTransformations>
    </transformation>
    <transformation values="Configs\NHibernate.values.prod.config" output="Configs\Transformed\NHibernate.prod.config" />
  </transformationGroup>
  <transformationGroup pattern="Configs\Web.pattern.config">
    <postTransformations>
      <clear />
    </postTransformations>
    <transformation values="Configs\Web.values.dev.config" output="Web.config" >
      <postTransformations>
        <add name="ReFormatXML" />
      </postTransformations>
    </transformation>
    <transformation values="Configs\Web.values.prod.config" output="Configs\Transformed\Web.prod.config" />
  </transformationGroup>
</root>
```
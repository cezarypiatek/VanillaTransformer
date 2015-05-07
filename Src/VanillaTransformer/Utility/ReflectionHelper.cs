using System;
using System.Linq;
using System.Reflection;

namespace VanillaTransformer.Utility
{
    public static class ReflectionHelper
    {
        public static TDemandedType GetInstanceOf<TDemandedType>(string typeName,params  object[] constructorParameters) where TDemandedType:class
        {
            var valuesProviderType = Assembly.GetExecutingAssembly()
                .GetTypes()
                .FirstOrDefault(type => type.Name == typeName);

            if (valuesProviderType == null)
            {
                var typeNotFoundMessage = string.Format("Cannot find configuration values provider called '{0}'", typeName);
                throw new ApplicationException(typeNotFoundMessage);
            }
            var demandedTypeInstance = Activator.CreateInstance(valuesProviderType, constructorParameters) as TDemandedType;
            if (demandedTypeInstance == null)
            {
                var invalidImplementationMessage = string.Format("The given type '{0}' should implement {1} interface",typeName, typeof(TDemandedType).Name);
                throw new ApplicationException(invalidImplementationMessage);
            }

            //TODO:Use Activator.CreateInstance(typeName,assemblyName);
            return demandedTypeInstance;
        }
    }
}
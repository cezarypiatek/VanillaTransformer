using VanillaTransformer.Core;
using VanillaTransformer.Core.Transformers;
using VanillaTransformer.Core.Utility;
using VanillaTransformer.Core.ValuesProviders;
using Xunit;

namespace VanillaTransformer.Tests.UtilityTest
{
    public class ReflectionHelperTest
    {
        [Fact]
        public void should_be_able_to_create_instance_of_class_with_default_constructor()
        {
            //ARRANGE
            var className = typeof (DollarPlaceholderTransformer).Name;
            
            //ACT
            var result = ReflectionHelper.GetInstanceOf<ITransformer>(className);

            //ASSERT
            Assert.NotNull(result);
        }

        [Fact]
        public void should_be_able_to_create_instance_of_class_with_constructor_with_parameters()
        {
            //ARRANGE
            var className = typeof(XmlFileConfigurationValuesProvider).Name;

            //ACT
            var result = ReflectionHelper.GetInstanceOf<IValuesProvider>(className,"text.xml");

            //ASSERT
            Assert.NotNull(result);
        }
    }
}

using NUnit.Framework;
using VanillaTransformer.Transformers;
using VanillaTransformer.Utility;
using VanillaTransformer.ValuesProviders;

namespace VanillaTransformer.Tests.UtilityTest
{
    


    [TestFixture]
    public class ReflectionHelperTest
    {
        [Test]
        public void should_be_able_to_create_instance_of_class_with_default_constructor()
        {
            //ARRANGE
            var className = typeof (DollarPlaceholderTransformer).Name;
            
            //ACT
            var result = ReflectionHelper.GetInstanceOf<ITransformer>(className);

            //ASSERT
            Assert.IsNotNull(result);
        }

        [Test]
        public void should_be_able_to_create_instance_of_class_with_constructor_with_parameters()
        {
            //ARRANGE
            var className = typeof(XmlFileConfigurationValuesProvider).Name;

            //ACT
            var result = ReflectionHelper.GetInstanceOf<IValuesProvider>(className,"text.xml");

            //ASSERT
            Assert.IsNotNull(result);
        }
    }
}

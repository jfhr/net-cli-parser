using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CliParser.UnitTests
{
    [TestClass]
    public class CliOptionTest
    {
        [TestMethod]
        public void ParseEmpty_Expect_DefaultValues()
        {
            var args = new string[0];

            var options = CliParser.Parse<SampleOptions>(args);

            Assert.IsFalse(options.Flag);
            Assert.IsNull(options.NamedOption);
        }
        
        [TestMethod]
        public void Parse_Expect_SetValues()
        {
            var args = new string[] { "--named-option", "foobar", "-f" };

            var options = CliParser.Parse<SampleOptions>(args);

            Assert.IsTrue(options.Flag);
            Assert.AreEqual("foobar", options.NamedOption);
        }

        [TestMethod, ExpectedException(typeof(CliOptionsException), AllowDerivedTypes = true)]
        public void NameWithoutValue_Expect_Exception()
        {
            var args = new string[] { "--named-option", };

            CliParser.Parse<SampleOptions>(args);
        }
        
        [TestMethod, ExpectedException(typeof(CliOptionsException), AllowDerivedTypes = true)]
        public void MissingRequiredValue_Expect_Exception()
        {
            var args = new string[0];
            
            CliParser.Parse<SampleOptionsWithRequiredValue>(args);
        }
    }


    class SampleOptions
    {
        [CliOption("--named-option", "-no")]
        public string NamedOption { get; set; }

        [CliOption("--flag", "-f")]
        public bool Flag { get; set; }
    }


    class SampleOptionsWithRequiredValue
    {
        [CliOption("--named-option", "-no", Required = true)]
        public string RequiredOption { get; set; }
    }
}
using NUnit.Framework;
using System;
using TestsGenerator;

namespace NUnitTestProject
{
    public class Tests
    {
        private TestGenerator generator;
        private readonly string nomethodsPath = "D:\\5sem\\spp\\lab4\\SppLab4\\Classes\\no_methods.cs";
        private readonly string noclassesPath = "D:\\5sem\\spp\\lab4\\SppLab4\\Classes\\no_classes.cs";
        private readonly string noPublicMethodsPath = "D:\\5sem\\spp\\lab4\\SppLab4\\Classes\\no_public_methods.cs";
        private readonly string validPath = "D:\\5sem\\spp\\lab4\\SppLab4\\Classes\\valid.cs";

        [SetUp]
        public void Setup()
        {
            generator = new TestGenerator();
        }

        [Test]
        public void GetTests_SourceStringIsNull ()
        {
            Assert.Throws<ArgumentNullException>(() => generator.GetTests(null));
        }

        [Test]
        public void GetTests_EmptyString()
        {
            var result = generator.GetTests(string.Empty);
            Assert.IsTrue(result.Result.code == string.Empty);
            Assert.AreEqual(result.Result.fileName, "Empty.cs");
        }

        [Test]
        public void GetTests_NoMethods()
        {
            var result = generator.GetTests(nomethodsPath);
            Assert.IsFalse(result.Result.code.Contains("[Test]"));
        }

        [Test]
        public void GetTests_NoClasses()
        {
            var result = generator.GetTests(noclassesPath);
            Assert.IsFalse(result.Result.code.Contains("[TestFixture]"));
        }

        [Test]
        public void GetTests_NoPublicMethods()
        {
            var result = generator.GetTests(noPublicMethodsPath);
            Assert.IsFalse(result.Result.code.Contains("[Test]"));
        }

        [Test]
        public void GetTests_ValidSource()
        {
            var result = generator.GetTests(validPath);
            Assert.IsFalse(result.Result.code.Contains("[TestFuxture]"));
            Assert.IsFalse(result.Result.code.Contains("[Test]"));
        }

    }
}
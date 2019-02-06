using EchoConsole;
using NUnit.Framework;

namespace EchoConsoleTests
{
    [TestFixture]
    public class CalculatorTest
    {
        [Test]
        [TestCase(1, 1, ExpectedResult = 2)]
        [TestCase(2, 1, ExpectedResult = 3)]
        [TestCase(2, 2, ExpectedResult = 4)]
        [TestCase(3, 2, ExpectedResult = 5)]
        [TestCase(3, 3, ExpectedResult = 6)]
        [TestCase(3, 4, ExpectedResult = 7)]
        public int AddTwoNumbers_ShouldReturn_addedNumbers(int firstInteger, int secondInteger)
        {
            //Arrange: create a new calculator to use
            Calculator calc = new Calculator();

            //act: instruct the calculator to add upp our integers
            var actualResult = calc.Add(firstInteger, secondInteger);

            //Assert: validat that the calculator worked
            return actualResult;
        }
    }
}

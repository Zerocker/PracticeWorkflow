using System;
using System.Text.RegularExpressions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Quaer.Engine;
using Quaer.Model;

namespace Testing
{
    [TestClass]
    public class NumberTest
    {
        [TestMethod]
        public void GetNumberFromString()
        {
            string test = "+7-914-111-1111";
            Number number = new Number(test);

            Assert.AreEqual(number.Country, 7);
            Assert.AreEqual(number.Def, 914);
            Assert.AreEqual(number.Subscriber, 1111111);
        }

        [TestMethod]
        public void GetNumberFromInt()
        {
            long test = 79141111111;
            Number number = new Number(test);

            Assert.AreEqual(number.Country, 7);
            Assert.AreEqual(number.Def, 914);
            Assert.AreEqual(number.Subscriber, 1111111);
        }

        [TestMethod]
        public void GenerateCorrectRegex()
        {
            long test = 79141111111;
            Number number = new Number(test);

            string pattern = number.GenerateRegex();
            Assert.AreEqual(pattern, "\\+?[7]*[\\s.\\(-]?914[\\s.\\)-]?111[\\s.-]?11[\\s.-]?11");

            Regex regex = new Regex(pattern);

            string text = "89141111111, +79141111111, 8(914)111-11-11, +7(914)111-11-11";
            var matches = regex.Matches(text);
            Assert.AreEqual(matches.Count, 4);
        }
    }
}

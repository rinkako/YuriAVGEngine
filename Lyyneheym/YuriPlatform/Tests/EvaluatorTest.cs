using System;
using NUnit.Framework;
using Yuri.PlatformCore.VM;
using Yuri.PlatformCore.Evaluator;

namespace Yuri.Tests
{
    [TestFixture]
    internal class EvaluatorTest
    {
        [Test]
        public void ProcessNumberConstant()
        {
            SimpleContext sc = new SimpleContext("");
            IEvaluator etor = new PolishEvaluator();

            var res = etor.Eval("1 2 +", sc);
            Assert.AreEqual(res, 3);

            res = etor.Eval("5 8 -", sc);
            Assert.AreEqual(res, -3);

            res = etor.Eval("2.33 3.33 +", sc);
            Assert.AreEqual(res, 5.66);

            res = etor.Eval("2.33 3.33 -", sc);
            Assert.AreEqual(res, -1);
        }

        [Test]
        public void ProcessNumberVariableConstant()
        {
            SimpleContext sc = new SimpleContext("");
            IEvaluator etor = new PolishEvaluator();

            sc.Assign("a", 5);
            sc.Assign("b", 12);
            sc.Assign("na", -5);
            sc.Assign("nb", -12);

            var res = etor.Eval("$a $b +", sc);
            Assert.AreEqual(res, 17);

            res = etor.Eval("$a $nb +", sc);
            Assert.AreEqual(res, -7);

            res = etor.Eval("$b $na -", sc);
            Assert.AreEqual(res, 17);

            res = etor.Eval("$na $nb -", sc);
            Assert.AreEqual(res, 7);

            res = etor.Eval("2 $na 3 + -", sc);
            Assert.AreEqual(res, 4);
        }

        [Test]
        public void ProcessStringConstant()
        {
            SimpleContext sc = new SimpleContext("");
            IEvaluator etor = new PolishEvaluator();

            var res = etor.Eval("\"he\" \"llo\" +", sc);
            Assert.AreEqual(res, "hello");

            res = etor.Eval("\"hello\" \"~\" \"world\" + +", sc);
            Assert.AreEqual(res, "hello~world");
        }

        [Test]
        public void ProcessStringVariableConstant()
        {
            SimpleContext sc = new SimpleContext("");
            IEvaluator etor = new PolishEvaluator();

            sc.Assign("str1", "hello");
            sc.Assign("str2", "world");
            sc.Assign("spacer", " ");

            var res = etor.Eval("$str1 $str2 +", sc);
            Assert.AreEqual(res, "helloworld");

            res = etor.Eval("$str1 $spacer $str2 + +", sc);
            Assert.AreEqual(res, "hello world");

            res = etor.Eval("\"TEST\" $spacer $str2 + +", sc);
            Assert.AreEqual(res, "TEST world");
        }

        [Test]
        public void ProcessStringNumberConstant()
        {
            SimpleContext sc = new SimpleContext("");
            IEvaluator etor = new PolishEvaluator();
            
            var res = etor.Eval("\"testnumber:\" 233 +", sc);
            Assert.AreEqual(res, "testnumber:233");

            res = etor.Eval("\"testnumber:\" 233 1000 + +", sc);
            Assert.AreEqual(res, "testnumber:1233");
        }

        [Test]
        public void ProcessStringNumberVariableConstant()
        {
            SimpleContext sc = new SimpleContext("");
            IEvaluator etor = new PolishEvaluator();
            sc.Assign("str1", "hello");
            sc.Assign("str2", "world");
            sc.Assign("num1", 1000);
            sc.Assign("num2", 24);

            var res = etor.Eval("\"testnumber:\" $num1 $num2 + +", sc);
            Assert.AreEqual(res, "testnumber:1024");

            res = etor.Eval("\"testnumber:\" $num1 501 - +", sc);
            Assert.AreEqual(res, "testnumber:499");

            res = etor.Eval("$str1 $num1 $str2 $num2 + + +", sc);
            Assert.AreEqual(res, "hello1000world24");
        }
    }
}

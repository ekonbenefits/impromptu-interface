using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ImpromptuInterface;
using NUnit.Framework;
using Word = Microsoft.Office.Interop.Word;

namespace UnitTestImpromptuInterface
{
    [TestClass, TestFixture]
    public class Com : Helper
    {
        
        [Test, TestMethod]
        public void GetComDisplayNames()
        {
            var wordApp = new Word.Application();

            var docs = wordApp.Documents;

            var names =Impromptu.GetMemberNames(docs);

            Assert.AreEqual(4,names.Count());

            wordApp.Quit();
        }

        [Test, TestMethod]
        public void GetComVar()
        {
            var wordApp = new Word.Application();

            var docs = wordApp.Documents;

            var count = Impromptu.InvokeGet(docs,"Count");

            Assert.AreEqual(0,count);

            wordApp.Quit();
        }
    }
}

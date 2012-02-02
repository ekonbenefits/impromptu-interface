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


            wordApp.Quit();
        }
    }
}

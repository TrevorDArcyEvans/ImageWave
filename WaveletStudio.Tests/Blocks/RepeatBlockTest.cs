﻿using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using WaveletStudio.Blocks;
using WaveletStudio.Functions;

namespace WaveletStudio.Tests.Blocks
{
    [TestClass]
    public class RepeatBlockTest
    {
        [TestMethod]
        public void RepeatBlockTestExecute2()
        {
            var signalBlock = new ImportFromTextBlock { Text = "1, 9, 0, 1, 2, 5, -4, 4" };
            var repeatBlock = new RepeatBlock{ FrameSize = 4, RepetitionCount = 1};

            signalBlock.ConnectTo(repeatBlock);
            signalBlock.Execute();

            Console.WriteLine(repeatBlock.OutputNodes[0].Object[0].ToString(0));
            //Output: 1 9 0 1 1 9 0 1 2 5 -4 4 2 5 -4 4
            Assert.AreEqual("1 9 0 1 1 9 0 1 2 5 -4 4 2 5 -4 4", repeatBlock.OutputNodes[0].Object[0].ToString(0));
        }

        [TestMethod]
        public void RepeatBlockTestExecute()
        {
            var signalBlock = new ImportFromTextBlock{ColumnSeparator = " "};
            var block = new RepeatBlock();
            block.Execute();
            
            signalBlock.ConnectTo(block);
            Assert.IsNotNull(block.Name);
            Assert.IsNotNull(block.Description);
            Assert.IsNotNull(block.ProcessingType);

            block.RepetitionCount = 2;
            block.FrameSize = 3;
            signalBlock.Text = "1 2 3 4";
            signalBlock.Execute();
            Assert.AreEqual("1 2 3 1 2 3 1 2 3 4 4 4", block.OutputNodes[0].Object.ToString(0));

            var block2 = (RepeatBlock)block.Clone();
            block2.RepetitionCount = 3;
            signalBlock.Text = "1 2";
            block.ConnectTo(block2);
            signalBlock.Execute();
            Assert.AreEqual("1 2 1 2 1 2", block.OutputNodes[0].Object.ToString(0));
            Assert.AreEqual("1 2 1 1 2 1 1 2 1 1 2 1 2 1 2 2 1 2 2 1 2 2 1 2", block2.OutputNodes[0].Object.ToString(0));

            block.Cascade = false;
            block2 = (RepeatBlock)block.Clone();
            block.ConnectTo(block2);
            signalBlock.Execute();
            Assert.AreEqual("", block2.OutputNodes[0].Object.ToString(0, " "));
            Assert.AreEqual(0, block2.OutputNodes[0].Object.Count);
        }
    }
}

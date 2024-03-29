﻿using Microsoft.VisualStudio.TestTools.UnitTesting;
using WaveletStudio.Blocks;
using WaveletStudio.Functions;

namespace WaveletStudio.Tests.Blocks
{
    [TestClass]
    public class ImportFromCSVBlockTest
    {
        [TestMethod]
        [DeploymentItem("example.csv")]
        public void TestSignalFromCSVBlockExecute()
        {
            var csvBlock = new ImportFromCSVBlock {IgnoreFirstRow = true, SignalNameInFirstColumn = true};
            csvBlock.Execute();
            Assert.IsNotNull(csvBlock.Name);
            Assert.IsNotNull(csvBlock.Description);
            Assert.IsNotNull(csvBlock.ProcessingType);
            Assert.AreEqual(3, csvBlock.OutputNodes[0].Object.Count);
            
            Assert.AreEqual("Signal1", csvBlock.OutputNodes[0].Object[0].Name);
            Assert.IsTrue(TestUtils.SequenceEquals(new[] { 1.1, 9.12355, 0.123456, 0 }, csvBlock.OutputNodes[0].Object[0].Samples));

            Assert.AreEqual("Signal2", csvBlock.OutputNodes[0].Object[1].Name);
            Assert.IsTrue(TestUtils.SequenceEquals(new[] { -1.1, 0.123456, 0 }, csvBlock.OutputNodes[0].Object[1].Samples));

            Assert.AreEqual("Signal3", csvBlock.OutputNodes[0].Object[2].Name);
            Assert.IsTrue(TestUtils.SequenceEquals(new[] { -1.1, 9.12355, 0.123456, 0 }, csvBlock.OutputNodes[0].Object[2].Samples));


            csvBlock.Cascade = false;
            var scalarBlock = new ScalarOperationBlock { Operation = WaveMath.OperationEnum.Sum, Value = 1.5 };
            csvBlock.OutputNodes[0].ConnectTo(scalarBlock.InputNodes[0]);
            csvBlock.Execute();
            Assert.AreEqual(0, scalarBlock.OutputNodes[0].Object.Count);

            var csvBlock2 = (ImportFromCSVBlock)csvBlock.Clone();
            csvBlock2.OutputNodes[0].ConnectTo(scalarBlock.InputNodes[0]);
            csvBlock2.Cascade = true;
            csvBlock2.Execute();
            Assert.AreEqual(3, csvBlock2.OutputNodes[0].Object.Count);

            csvBlock2.FilePath = "nonononono.csv";
            csvBlock2.Cascade = true;
            csvBlock2.Execute();
            Assert.AreEqual(0, csvBlock2.OutputNodes[0].Object.Count);

            csvBlock2.FilePath = "waveletstudio.tests.dll";
            csvBlock2.Cascade = true;
            csvBlock2.Execute();
            Assert.AreEqual(0, csvBlock2.OutputNodes[0].Object.Count);
        }
    }
}

﻿using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using WaveletStudio.Blocks;
using WaveletStudio.Functions;

namespace WaveletStudio.Tests.Blocks
{
    [TestClass]
    public class GenerateSignalBlockTest
    {
        [TestMethod]
        public void TestGenerateSignalBlockExecute()
        {
            var signalBlock = new GenerateSignalBlock{TemplateName = "Binary", Start = 0, Finish = 5, SamplingRate = 1, IgnoreLastSample = true};
            signalBlock.Execute();
            
            var newSignal = (GenerateSignalBlock)signalBlock.Clone();
            newSignal.Finish = 6;
            newSignal.Execute();

            Assert.IsNotNull(signalBlock.Name);
            Assert.AreEqual("Binary", signalBlock.GetAssemblyClassName());
            Assert.AreEqual("A=1, F=60, φ=0, D=0; x=0...5, fs=1", signalBlock.Description);            
            Assert.IsNotNull(signalBlock.ProcessingType);

            Assert.AreNotSame(signalBlock.OutputNodes[0], newSignal.OutputNodes[0]);
            Assert.AreNotSame(signalBlock.OutputNodes[0].Object, newSignal.OutputNodes[0].Object);

            Assert.AreEqual("0.0 1.0 0.0 1.0 0.0 1.0", newSignal.OutputNodes[0].Object.ToString(1));
            Assert.AreEqual("0.0 1.0 0.0 1.0 0.0", signalBlock.OutputNodes[0].Object.ToString(1));

            signalBlock.Cascade = false;
            var scalarBlock = new ScalarOperationBlock { Operation = WaveMath.OperationEnum.Sum, Value = 1.5 };
            signalBlock.OutputNodes[0].ConnectTo(scalarBlock.InputNodes[0]);            
            signalBlock.Execute();
            Assert.AreEqual(0, scalarBlock.OutputNodes[0].Object.Count);

            try
            {
                //force exception
                signalBlock.TemplateName = "Nonono";
                Assert.Fail("Exception not thrown!");
            }
            catch (Exception)
            {
                Assert.IsTrue(true, "Exception thrown! Yeay!");
            }
            
        }
    }
}

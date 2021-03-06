/*  Wavelet Studio Signal Processing Library - www.waveletstudio.net
    Copyright (C) 2011, 2012 Walter V. S. de Amorim - The Wavelet Studio Initiative

    Wavelet Studio is free software: you can redistribute it and/or modify
    it under the terms of the GNU Lesser General Public License as published by
    the Free Software Foundation, either version 3 of the License, or
    (at your option) any later version.

    Wavelet Studio is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    GNU General Public License for more details.

    You should have received a copy of the GNU General Public License
    along with this program.  If not, see <http://www.gnu.org/licenses/>. 
*/

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using WaveletStudio.Blocks.CustomAttributes;
using WaveletStudio.Functions;
using WaveletStudio.Properties;

namespace WaveletStudio.Blocks
{
    /// <summary>
    /// Gets the normal distribution of a signal
    /// </summary>
    [Serializable]
    [ExcludeFromCodeCoverage]
    internal class NormalDistributionBlock : BlockBase
    {
        /// <summary>
        /// Constructor
        /// </summary>
        public NormalDistributionBlock()
        {
            BlockBase root = this;
            CreateNodes(ref root);
        }
        
        /// <summary>
        /// Name of the block
        /// </summary>
        public override string Name
        {
            get { return "Normal"; }
        }

        /// <summary>
        /// Description
        /// </summary>
        public override string Description
        {
            get { return Resources.NormalDescription; }
        }

        /// <summary>
        /// Processing type
        /// </summary>
        public override ProcessingTypeEnum ProcessingType { get { return ProcessingTypeEnum.Operation; } }

        /// <summary>
        /// Mean of the signal to be used in the calc. 0 for automatic.
        /// </summary>
        [Parameter]
        public double Mean { get; set; }

        /// <summary>
        /// Standard deviation to be used in the calc. 0 for automatic.
        /// </summary>
        [Parameter]
        public double StandardDeviation { get; set; }

        /// <summary>
        /// Executes the block
        /// </summary>
        public override void Execute()
        {
            var inputNode = InputNodes[0].ConnectingNode as BlockOutputNode;
            if (inputNode == null || inputNode.Object == null)
                return;

            OutputNodes[0].Object.Clear();
            foreach (var signal in inputNode.Object)
            {
                var output = signal.Copy();
                //var mean = Mean;
                //var deviation = StandardDeviation;
                //if (Math.Abs(mean) < float.Epsilon)
                //    mean = WaveMath.Mean(signal.Samples);
                //if (Math.Abs(deviation) < float.Epsilon)
                //    deviation = WaveMath.StandardDeviation(signal.Samples);
                output.Samples = NormalDistribution(signal.Samples);
                OutputNodes[0].Object.Add(output);
            }            
            if (Cascade && OutputNodes[0].ConnectingNode != null)
                OutputNodes[0].ConnectingNode.Root.Execute();
        }

        public static double[] NormalDistribution(double[] x)
        {
            var mean = WaveMath.Mean(x);
            var deviation = WaveMath.StandardDeviation(x);

            var samples = new List<KeyValuePair<int, double>>();
            var result = MemoryPool.Pool.New<double>(x.Length);
            for (var i = 0; i < x.Length; i++)
            {
                var norm = WaveMath.ProbabilityDensityFunction(x[i], mean, deviation);
                samples.Add(new KeyValuePair<int, double>(i, norm));
                result[i] = norm;
            }
            return samples.OrderBy(it => it.Value).Select(it => it.Value).ToArray();
        }

        /// <summary>
        /// Creates the input and output nodes
        /// </summary>
        /// <param name="root"></param>
        protected override sealed void CreateNodes(ref BlockBase root)
        {
            root.InputNodes = new List<BlockInputNode>();
            root.OutputNodes = new List<BlockOutputNode>();
            root.InputNodes.Add(new BlockInputNode(ref root, Resources.Signal, Resources.In));
            root.OutputNodes.Add(new BlockOutputNode(ref root, Resources.Output, Resources.Out));
        }

        /// <summary>
        /// Clones this block
        /// </summary>
        /// <returns></returns>
        public override BlockBase Clone()
        {
            return MemberwiseClone();            
        }

        /// <summary>
        /// Clones this block but mantains the links
        /// </summary>
        /// <returns></returns>
        public override BlockBase CloneWithLinks()
        {
            return MemberwiseCloneWithLinks();
        }
    }
}
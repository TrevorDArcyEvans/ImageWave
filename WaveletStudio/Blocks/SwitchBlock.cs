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
using System.Linq;
using WaveletStudio.Blocks.CustomAttributes;
using WaveletStudio.Functions;
using WaveletStudio.Properties;

namespace WaveletStudio.Blocks
{
    /// <summary>
    /// <para>Switch output between first input (A) and third input (B) based on value of second input or the threshold value, using the specified switch criteria.</para>
    /// <para>For example, let’s consider the following scenario:</para>
    /// <code>
    /// Signal A: 1, 3, 4, -4, 8, 3, 2, -10
    /// Signal B: 3, -2, 4, -6, 7, 1, 4, 3
    /// Threshold: 2
    /// Switch Criteria: Select B when B is greater than threshold
    /// </code>
    /// <para>The Switch Criteria is a parameter defining when the value of the signal B will be selected, instead of selecting the value of the signal A. The block will output a new signal with the folowing samples:</para>
    /// <code>
    /// 3 3 4 -4 7 3 4 3
    /// </code>
    /// <para>The #1 sample (3) was selected from signal B (the value from B is greater than the threshold);</para>
    /// <para>The #2 sample (3) was selected from signal A (the value from B is not greater than the threshold);</para>
    /// <para>The #3 sample (4) was selected from signal B (the value from B is greater than the threshold);</para>
    /// <para>and so on.</para>
    /// <para>Image: http://i.imgur.com/V34q36D.png </para>
    /// <para>InOutGraph: http://i.imgur.com/16EmhjY.png </para>
    /// <para>Title: Switch </para>
    /// <para>Inputs: 0 - Signal A</para>
    /// <para>Inputs: 1 - Threshold signal(optional)</para>
    /// <para>Inputs: 2 - Signal B</para>
    /// <para>Outputs: This block outputs a single signal or a signal list of switched samples.</para>
    /// <example>
    ///     <code>
    ///         var signal1 = new ImportFromTextBlock { Text = "1, 3, 4, -4, 8, 3, 2, -10" };
    ///         var signal2 = new ImportFromTextBlock { Text = "3, -2, 4, -6, 7, 1, 4, 3" };
    ///         var block = new SwitchBlock 
    ///         { 
    ///             StaticThreshold = 2, 
    ///             SwitchCriteria = WaveMath.SwitchCriteriaEnum.BIsGreaterThanThreshold 
    ///         };
    ///         signal1.OutputNodes[0].ConnectTo(block.InputNodes[0]);
    ///         signal2.OutputNodes[0].ConnectTo(block.InputNodes[2]);
    ///         signal1.Execute();
    ///         signal2.Execute();
    ///         
    ///         Console.WriteLine(block.OutputNodes[0].Object.ToString(0));
    ///         //Output: 3 3 4 -4 7 3 4 3
    ///     </code>
    /// </example>
    /// </summary>
    [Serializable]
    public class SwitchBlock : BlockBase
    {
        /// <summary>
        /// Constructor
        /// </summary>
        public SwitchBlock()
        {
            BlockBase root = this;
            CreateNodes(ref root);
            SwitchCriteria = WaveMath.SwitchCriteriaEnum.BIsGreaterOrEqualsThanThreshold;
        }
        
        /// <summary>
        /// Name of the block
        /// </summary>
        public override string Name
        {
            get { return Resources.Switch; }
        }

        /// <summary>
        /// Defines when the value of the signal B will be selected, instead of selecting the value of the signal A
        /// </summary>
        [Parameter]
        public WaveMath.SwitchCriteriaEnum SwitchCriteria { get; set; }

        /// <summary>
        /// Assign the switch threshold that determines which input the block passes to the output. You can use this parameter to set a static value, or use the second input of the block to define a signal as a threshold.
        /// </summary>
        [Parameter]
        public double StaticThreshold { get; set; }        

        /// <summary>
        /// Description
        /// </summary>
        public override string Description
        {
            get { return Resources.SwitchDescription; }
        }

        /// <summary>
        /// Processing type
        /// </summary>
        public override ProcessingTypeEnum ProcessingType { get { return ProcessingTypeEnum.Routing; } }

        /// <summary>
        /// Executes the block
        /// </summary>
        public override void Execute()
        {
            OutputNodes[0].Object.Clear();
            var input1 = InputNodes[0].ConnectingNode as BlockOutputNode;
            var thresholdSignal = InputNodes[1].ConnectingNode as BlockOutputNode;
            var input3 = InputNodes[2].ConnectingNode as BlockOutputNode;
            var empty1 = input1 == null || input1.Object == null || input1.Object.Count == 0;
            var empty3 = input3 == null || input3.Object == null || input3.Object.Count == 0;
            var useThresholdSignal = thresholdSignal != null && thresholdSignal.Object != null && thresholdSignal.Object.Count != 0;
            
            if (!empty1 || !empty3)
            {
                if (empty1)
                {
                    OutputNodes[0].Object.AddRange(input3.Object.Select(it => it.Clone()));
                }
                else if (empty3)
                {
                    OutputNodes[0].Object.AddRange(input1.Object.Select(it => it.Clone()));
                }
                else
                {
                    var size = Math.Max(input3.Object.Count, input1.Object.Count);
                    for (var i = 0; i < size; i++)
                    {
                        if (input1.Object.Count > i && input3.Object.Count > i)
                        {
                            var output = input1.Object[i].Copy();
                            if (useThresholdSignal && thresholdSignal.Object.Count > i)
                            {
                                output.Samples = thresholdSignal.Object[i].Samples.Length == 1 ? 
                                                WaveMath.Switch(input1.Object[i].Samples, input3.Object[i].Samples, thresholdSignal.Object[i].Samples[0], SwitchCriteria) :
                                                WaveMath.Switch(input1.Object[i].Samples, input3.Object[i].Samples, thresholdSignal.Object[i].Samples, SwitchCriteria);
                            }                            
                            else if (useThresholdSignal && thresholdSignal.Object.Count == 1)
                            {
                                output.Samples = thresholdSignal.Object[0].Samples.Length == 1 ?
                                                WaveMath.Switch(input1.Object[i].Samples, input3.Object[i].Samples, thresholdSignal.Object[0].Samples[0], SwitchCriteria) :
                                                WaveMath.Switch(input1.Object[i].Samples, input3.Object[i].Samples, thresholdSignal.Object[0].Samples, SwitchCriteria);
                            }                            
                            else
                                output.Samples = WaveMath.Switch(input1.Object[i].Samples, input3.Object[i].Samples, StaticThreshold, SwitchCriteria);
                            OutputNodes[0].Object.Add(output);
                        }
                        else if(input1.Object.Count > i)
                        {
                            OutputNodes[0].Object.Add(input1.Object[i].Clone());
                        }
                        else if(input3.Object.Count > i)
                        {
                            OutputNodes[0].Object.Add(input3.Object[i].Clone());
                        }
                    }                                             
                }                
            }            
            if (Cascade && OutputNodes[0].ConnectingNode != null)
                OutputNodes[0].ConnectingNode.Root.Execute();
        }

        /// <summary>
        /// Creates the input and output nodes
        /// </summary>
        /// <param name="root"></param>
        protected override sealed void CreateNodes(ref BlockBase root)
        {
            root.InputNodes = new List<BlockInputNode>
                                  {
                                      new BlockInputNode(ref root, Resources.Input + "1", Resources.In + "1"),
                                      new BlockInputNode(ref root, Resources.Input + "2", Resources.In + "2"),
                                      new BlockInputNode(ref root, Resources.Input + "3", Resources.In + "3")
                                  };
            root.OutputNodes = BlockOutputNode.CreateSingleOutput(ref root);            
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

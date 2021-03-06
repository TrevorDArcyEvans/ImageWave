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
using System.Linq;
using WaveletStudio.Blocks.CustomAttributes;
using WaveletStudio.Functions;
using WaveletStudio.Properties;

namespace WaveletStudio.Blocks
{
    /// <summary>
    /// <para>Removes duplicated samples in a signal.</para>
    /// <para> </para>
    /// <para>For example, if we have a signal with 8 samples like this one:</para>
    /// <code>1, 3, -4, 8, 3, 4, 1, -3</code>
    /// <para>the block will output a new signal with the folowing samples:</para>
    /// <code>1, 3, -4, 8, 4, -3</code>
    /// <para>Image: http://i.imgur.com/fVvqcwZ.png </para>
    /// <para>InOutGraph: http://i.imgur.com/HdzUh01.png </para>
    /// <para>Title: Unique </para>
    /// <example>
    ///     <code>
    ///         var signal = new ImportFromTextBlock { Text = "1, 3, -4, 8, 3, 4, 1, -3" };
    ///         var block = new UniqueBlock 
    ///         { 
    ///             SortSamples = false 
    ///         };
    ///         
    ///         signal.ConnectTo(block);
    ///         signal.Execute();
    ///         
    ///         Console.WriteLine(block.Output[0].ToString(0));
    ///         //Output: 1, 3, -4, 8, 4, -3
    ///     </code>
    /// </example>
    /// </summary>
    [Serializable]
    public class UniqueBlock : BlockBase
    {
        /// <summary>
        /// Constructor
        /// </summary>
        public UniqueBlock()
        {
            BlockBase root = this;
            CreateNodes(ref root);
            SortSamples = true;
        }
        
        /// <summary>
        /// Name of the block
        /// </summary>
        public override string Name
        {
            get { return Resources.Unique; }
        }

        /// <summary>
        /// Description
        /// </summary>
        public override string Description
        {
            get { return Resources.UniqueDescription; }
        }

        /// <summary>
        /// Processing type
        /// </summary>
        public override ProcessingTypeEnum ProcessingType { get { return ProcessingTypeEnum.Operation; } }

        /// <summary>
        /// If true, the block sorts the samples after remove the duplicated samples. Default value is true.
        /// </summary>
        [Parameter]
        public bool SortSamples { get; set; }

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
                output.Samples = SortSamples ? WaveMath.UniqueSorted(signal.Samples) : signal.Samples.Distinct().ToArray();
                OutputNodes[0].Object.Add(output);
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
            root.InputNodes = BlockInputNode.CreateSingleInputSignal(ref root);
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
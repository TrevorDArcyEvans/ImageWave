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
using WaveletStudio.Blocks.CustomAttributes;
using WaveletStudio.Functions;
using WaveletStudio.Properties;

namespace WaveletStudio.Blocks
{
    /// <summary>
    /// <para>Increases the sampling rate of a signal using linear, nearest, cubic, Newton’s or polynomial interpolation methods.</para>
    /// <para>Image: http://i.imgur.com/qxPtNRn.png </para>
    /// <para>InOutGraph: http://i.imgur.com/sGvyqsu.png </para>
    /// <para>Title: Interpolation </para>
    /// <example>
    ///     <code>
    ///         //Creates a signal with 4 samples
    ///         var signal = new ImportFromTextBlock { Text = "14, 20, 11, 41" };
    ///         var block = new InterpolationBlock
    ///         {
    ///             Factor = 10, //(will insert 9 samples)
    ///             Mode=InterpolationModeEnum.Polynomial
    ///         };
    ///                     
    ///         //Connect and execute blocks
    ///         signal.ConnectTo(block);
    ///         signal.Execute();
    ///         
    ///         Console.WriteLine(block.Output[0].ToString(1));
    ///     </code>
    /// </example>
    /// </summary>
    [Serializable]
    public class InterpolationBlock : BlockBase
    {
        /// <summary>
        /// Constructor
        /// </summary>
        public InterpolationBlock()
        {
            Mode = InterpolationModeEnum.Cubic;
            Factor = 5;
            BlockBase root = this;
            CreateNodes(ref root);
        }
        
        /// <summary>
        /// Name of the block
        /// </summary>
        public override string Name
        {
            get { return Resources.Interpolate; }
        }

        /// <summary>
        /// Description of the block
        /// </summary>
        public override string Description
        {
            get { return Resources.InterpolateDescription; }
        }
        
        /// <summary>
        /// Processing type
        /// </summary>
        public override ProcessingTypeEnum ProcessingType { get { return ProcessingTypeEnum.Operation; } }

        /// <summary>
        /// Defines the interpolation method used by the block:
        /// </summary>
        [Parameter]
        public InterpolationModeEnum Mode { get; set; }

        /// <summary>
        /// Defines the interpolation factor used in the interpolation function or how many 
        /// samples will be inserted between samples of the signal. Default value is 5 (insert 4 samples).
        /// </summary>
        [Parameter]
        public uint Factor { get; set; }

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
                OutputNodes[0].Object.Add(WaveMath.Interpolate(signal, Factor, Mode));
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

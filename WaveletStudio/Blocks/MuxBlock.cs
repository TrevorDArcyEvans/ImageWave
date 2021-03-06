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
using WaveletStudio.Blocks.CustomAttributes;
using WaveletStudio.Properties;

namespace WaveletStudio.Blocks
{
    /// <summary>
    /// <para>Combine several input signals into vector.</para>
    /// <para>For example, if we connect 3 blocks to the MuxBlock, it will output a single signal list, with 3 items.</para>
    /// <para>Image: http://i.imgur.com/d3hUAji.png </para>
    /// <para>InOutGraph: http://i.imgur.com/Ds4ezaH.png </para>
    /// <para>Title: Mux</para>
    /// <example>
    ///     <code>
    ///         var signal1 = new ImportFromTextBlock { Text = "1, 7, 3, 1" };
    ///         var signal2 = new ImportFromTextBlock { Text = "5, 7, 2, 8" };
    ///         var signal3 = new ImportFromTextBlock { Text = "9, 8, 4, 3" };
    ///         var block = new MuxBlock { InputCount = 3 };
    ///         signal1.ConnectTo(block);
    ///         signal2.ConnectTo(block);
    ///         signal3.ConnectTo(block);
    ///         signal1.Execute();
    ///         signal2.Execute();
    ///         signal3.Execute();
    ///     
    ///         Console.WriteLine(block.OutputNodes[0].Object.Count);
    ///         Console.WriteLine(block.Output[0, 0].ToString(0));
    ///         Console.WriteLine(block.Output[0, 1].ToString(0));
    ///         Console.WriteLine(block.Output[0, 2].ToString(0));
    ///         
    ///         //Output:
    ///         //3
    ///         //1 7 3 1
    ///         //5 7 2 8
    ///         //9 8 4 3
    ///     </code>
    /// </example>
    /// </summary>
    [Serializable]
    public class MuxBlock : BlockBase
    {
        /// <summary>
        /// Constructor
        /// </summary>
        public MuxBlock()
        {
            InputCount = 3;
        }
        
        /// <summary>
        /// Name of the block
        /// </summary>
        public override string Name
        {
            get { return "Mux"; }
        }

        private uint _inputCount;
        /// <summary>
        /// Number of inputs
        /// </summary> //
        [Parameter]        
        public uint InputCount
        {
            get { return _inputCount; }
            set
            {
                _inputCount = value;
                BlockBase root = this;
                CreateNodes(ref root);
            }
        }

        /// <summary>
        /// Signal names used in the output (optional, one per line)
        /// </summary>
        [TextParameter]
        public string SignalNames { get; set; }

        /// <summary>
        /// Description
        /// </summary>
        public override string Description
        {
            get { return Resources.MuxDescription; }
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
            var signalNames = (SignalNames ?? "").Split(new[] {'\r', '\n'}, StringSplitOptions.RemoveEmptyEntries);
            var nameIndex = 0;
            foreach (var item in InputNodes)
            {
                var inputNode = item.ConnectingNode as BlockOutputNode;
                if (inputNode == null || inputNode.Object.Count == 0) 
                    continue;
                foreach (var signal in inputNode.Object)
                {
                    var clonedSignal = signal.Clone();
                    if(nameIndex < signalNames.Length)
                    {
                        clonedSignal.Name = signalNames[nameIndex];
                        nameIndex++;
                    }
                    OutputNodes[0].Object.Add(clonedSignal);
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
            root.OutputNodes = BlockOutputNode.CreateSingleOutput(ref root);
            root.InputNodes = new List<BlockInputNode>();
            for (var i = 1; i <= _inputCount; i++)
            {
                root.InputNodes.Add(new BlockInputNode(ref root, Resources.Input + i, Resources.In + i));
            }                       
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

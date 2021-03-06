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
using System.Globalization;
using System.Linq;
using System.Text;
using WaveletStudio.Blocks.CustomAttributes;

namespace WaveletStudio.Blocks
{
    /// <summary>
    /// Defines a processing block
    /// </summary>
    [Serializable]
    public abstract class BlockBase : GlobalizedObject
    {
        private string _currentDirectory;

        /// <summary>
        /// Current opened file
        /// </summary>
        public string CurrentDirectory
        {
            get
            {
                if (string.IsNullOrEmpty(_currentDirectory))
                    _currentDirectory = Utils.AssemblyDirectory;
                return _currentDirectory;
            }
            set { _currentDirectory = value; }
        }

        /// <summary>
        /// Generated data (used only in text-based blocks)
        /// </summary>
        public StringBuilder GeneratedData { get; protected set; }

        /// <summary>
        /// Inputs
        /// </summary>
        public List<BlockInputNode> InputNodes = new List<BlockInputNode>();

        /// <summary>
        /// Outputs
        /// </summary>
        public List<BlockOutputNode> OutputNodes = new List<BlockOutputNode>();

        /// <summary>
        /// Id of the block (changes for each instance)
        /// </summary>
        public Guid Id { get; set; }
        
        /// <summary>
        /// Name of the block
        /// </summary>
        public abstract string Name { get; }

        /// <summary>
        /// Description of the block
        /// </summary>
        public abstract string Description { get; }

        /// <summary>
        /// Defines if the Execute method should be cascaded to the connected blocks
        /// </summary>  
        public bool Cascade { get; set; }

        /// <summary>
        /// Type of action done by this block
        /// </summary>
        public abstract ProcessingTypeEnum ProcessingType { get; }

        /// <summary>
        /// Execute the block
        /// </summary>
        public abstract void Execute();

        /// <summary>
        /// Creates the input and output nodes
        /// </summary>
        /// <param name="root"></param>
        protected abstract void CreateNodes(ref BlockBase root);

        /// <summary>
        /// Type of action done by the block
        /// </summary>
        public enum ProcessingTypeEnum
        {
            /// <summary>
            /// The block loads an external signal
            /// </summary>
            LoadSignal,
            /// <summary>
            /// The block creates a signal
            /// </summary>
            CreateSignal,
            /// <summary>
            /// The block makes some operation in a block
            /// </summary>
            Operation,
            /// <summary>
            /// Logic operations
            /// </summary>
            Logic,
            /// <summary>
            /// The block insert disturbances or noise to the signal
            /// </summary>
            InsertDisturbance,
            /// <summary>
            /// The block exports the signal
            /// </summary>
            Export,
            /// <summary>
            /// The block operates a transform in the signal
            /// </summary>
            Transform,
            /// <summary>
            /// Signal routing
            /// </summary>
            Routing
        }

        private BlockInputSignalBridge _input;

        /// <summary>
        /// Gets a signal in the input connector
        /// </summary>
        public BlockInputSignalBridge Input
        {
            get
            {
                return _input ?? (_input = new BlockInputSignalBridge(this));
            }
        }

        private BlockOutputSignalBridge _output;

        /// <summary>
        /// Gets a signal in the output connector
        /// </summary>
        public BlockOutputSignalBridge Output
        {
            get
            {
                return _output ?? (_output = new BlockOutputSignalBridge(this));
            }
        }
        
        /// <summary>
        /// Constructor
        /// </summary>
        protected BlockBase()
        {
            Id = Guid.NewGuid();
            Cascade = true;
            InputNodes = new List<BlockInputNode>();
            OutputNodes = new List<BlockOutputNode>();
        }

        /// <summary>
        /// Clone this block to a new one
        /// </summary>
        /// <returns></returns>
        public abstract BlockBase Clone();

        /// <summary>
        /// Clones this block but mantains the links
        /// </summary>
        /// <returns></returns>
        public abstract BlockBase CloneWithLinks();

        /// <summary>
        /// Clones this object
        /// </summary>
        /// <returns></returns>
        protected new BlockBase MemberwiseClone()
        {
            var block = (BlockBase)base.MemberwiseClone();
            block.ResetShortcuts();
            CreateNodes(ref block);
            return block;
        }

        private void ResetShortcuts()
        {
            _input = null;
            _output = null;
        }

        /// <summary>
        /// Clones this object
        /// </summary>
        /// <returns></returns>
        protected BlockBase MemberwiseCloneWithLinks()
        {
            var block = (BlockBase)base.MemberwiseClone();
            block.InputNodes = new List<BlockInputNode>();
            foreach (var node in InputNodes)
            {
                block.InputNodes.Add(node.Clone());
            }
            block.OutputNodes = new List<BlockOutputNode>();
            foreach (var blockOutputNode in OutputNodes)
            {
                block.OutputNodes.Add(blockOutputNode.Clone());
            }
            block.ResetShortcuts();
            return block;
        }

        /// <summary>
        /// Connects to another block
        /// </summary>
        /// <param name="block"></param>
        public void ConnectTo(BlockBase block)
        {
            if(OutputNodes.Count == 0 || block.InputNodes.Count == 0)
                return;

            var outputNode = OutputNodes.FirstOrDefault(it => it.ConnectingNode == null);
            var inputNode = block.InputNodes.FirstOrDefault(it => it.ConnectingNode == null);
            if (outputNode == null)
                outputNode = OutputNodes[0];
            if (inputNode == null)
                inputNode = block.InputNodes[0];

            outputNode.ConnectTo(inputNode);
        }

        /// <summary>
        /// Gets the name of a block
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static string GetName(Type type)
        {
            if (type.BaseType != typeof(BlockBase))
                return "";
            var block = (BlockBase)Activator.CreateInstance(type);
            return block.Name;
        }

        /// <summary>
        /// Gets the name of the processing type
        /// </summary>
        public string GetProcessingTypeName()
        {
            return GetProcessingTypeName(ProcessingType);
        }

        /// <summary>
        /// Gets the name of a processing type
        /// </summary>
        public static string GetProcessingTypeName(ProcessingTypeEnum processingType)
        {
            return Enum.GetName(typeof(ProcessingTypeEnum), processingType);
        }

        /// <summary>
        /// Gets the name of the class
        /// </summary>
        /// <returns></returns>
        public virtual string GetAssemblyClassName()
        {
            return GetType().Name;
        }

        /// <summary>
        /// Returns wether the block has parameters.
        /// </summary>
        /// <returns></returns>
        public bool HasParameters()
        {
            return GetType().GetProperties().Any(property => property.GetCustomAttributes(true).OfType<Parameter>().Any());
        }
    }
}

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
using System.IO;
using WaveletStudio.Blocks.CustomAttributes;
using WaveletStudio.Properties;

namespace WaveletStudio.Blocks
{
    /// <summary>
    /// <para>Generates a signal based on a CSV file.</para>
    /// <para>A CSV (comma-separated values) is a text file with the data (samples) separated with commas or another char. Each line in the file represents a signal. For example:</para>
    /// <code>
    ///     signal_name,sample1,sample2,sample3,sample4
    ///     Signal1, 1.1, 9.12, 0.123, 0
    ///     Signal2, 1.1, 4.56, 0.123, -45
    /// </code>
    /// <para>This example shows a file with 2 signals, with 4 samples in each one. The first column in the file is optional and represents the name of the signal. The header is optional too.</para>
    /// <para>Image: http://i.imgur.com/ApwmTG2.png </para>
    /// <para>InOutGraph: http://i.imgur.com/yUw6HcU.png </para>
    /// <para>Title: Import CSV</para>
    /// <para>This block has no inputs.</para>
    /// <example>
    ///     <code>
    ///         File.WriteAllText(@"C:\Temp\File.csv", "0, 2, -1, 4.1, 3, -1, 4, 0");
    ///     
    ///         var block = new ImportFromCSVBlock
    ///         {
    ///             ColumnSeparator = ",",
    ///             SignalStart = 0,
    ///             SamplingInterval = 0.1,
    ///             IgnoreFirstRow = false,
    ///             SignalNameInFirstColumn = false,
    ///             FilePath = @"C:\Temp\File.csv"
    ///         };
    ///         block.Execute();
    ///         
    ///         Console.WriteLine(block.Output[0].ToString(1, ","));
    ///         
    ///         //Console Output: 0.0, 2.0, -1.0, 4.1, 3.0, -1.0, 4.0, 0.0
    ///     </code>
    /// </example>
    /// </summary>
    [SingleInputOutputBlock]
    [Serializable]
    public class ImportFromCSVBlock : BlockBase
    {
        /// <summary>
        /// Constructor
        /// </summary>
        public ImportFromCSVBlock()
        {
            BlockBase root = this;
            CreateNodes(ref root);
            FilePath = "example.csv";
            ColumnSeparator = ",";
            SignalStart = 0;
            SamplingInterval = 1;
        }

        /// <summary>
        /// Name
        /// </summary>
        public override string Name { get { return Resources.ImportFromCSV; } }

        /// <summary>
        /// Description
        /// </summary>
        public override string Description { get { return Resources.ImportFromCSVDescription; } }

        /// <summary>
        /// Processing type
        /// </summary>
        public override ProcessingTypeEnum ProcessingType { get { return ProcessingTypeEnum.LoadSignal; } }

        /// <summary>
        /// Absolute or relative path to the file
        /// </summary>
        [Parameter]
        public string FilePath { get; set; }

        /// <summary>
        /// Column separator
        /// </summary>
        [Parameter]
        public string ColumnSeparator { get; set; }

        /// <summary>
        /// Signal start
        /// </summary>
        [Parameter]
        public int SignalStart { get; set; }

        private int _samplingRate;

        private double _samplingInterval;

        /// <summary>
        /// Sampling interval
        /// </summary>
        [Parameter]
        public double SamplingInterval 
        {
            get { return _samplingInterval; }
            set
            {
                _samplingInterval = value;
                if (Math.Abs(value - 0d) > double.Epsilon)
                {
                    _samplingRate = Convert.ToInt32(Math.Round(1 / value));   
                }
            }
        }

        /// <summary>
        /// Ignore first row when reading the file
        /// </summary>
        [Parameter]
        public bool IgnoreFirstRow { get; set; }

        /// <summary>
        /// If true, the first column contains the name of the signal
        /// </summary>
        [Parameter]
        public bool SignalNameInFirstColumn { get; set; }

        /// <summary>
        /// Executes the block
        /// </summary>
        public override void Execute()
        {
            OutputNodes[0].Object.Clear();
            var filePath = FilePath;
            if (!Path.IsPathRooted(filePath))
                filePath = Path.Combine(CurrentDirectory, FilePath);
            if(!File.Exists(filePath))
                filePath = Path.Combine(Utils.AssemblyDirectory, FilePath);
            if(!File.Exists(filePath))
                return;

            var lineNumber = 0;
            var lines = File.ReadAllLines(filePath);
            foreach (var line in lines)
            {
                lineNumber++;
                if (lineNumber == 1 && IgnoreFirstRow)
                    continue;
                var signal = ParseLine(line);
                if (signal == null) 
                    continue;
                if (signal.Name == "")
                    signal.Name = "Line " + lineNumber;
                OutputNodes[0].Object.Add(signal);
            }
            if (Cascade && OutputNodes[0].ConnectingNode != null)
                OutputNodes[0].ConnectingNode.Root.Execute();            
        }

        private Signal ParseLine(string line)
        {
            if(string.IsNullOrWhiteSpace(line))
                return null;

            var values = new List<double>();
            var samples = line.Split(new[] {ColumnSeparator}, StringSplitOptions.RemoveEmptyEntries);
            var columnNumber = 0;
            var signalName = "";
            foreach (var sampleString in samples)
            {
                columnNumber++;
                if (columnNumber == 1 && SignalNameInFirstColumn)
                {
                    signalName = sampleString;
                    continue;
                }
                double value;
                if (double.TryParse(sampleString, System.Globalization.NumberStyles.Number, System.Globalization.CultureInfo.InvariantCulture, out value))
                    values.Add(value);
            }
            if(values.Count == 0)
                return null;

            var signal = new Signal(values.ToArray())
                             {
                                 Name = signalName,
                                 Start = SignalStart,
                                 Finish = SignalStart + SamplingInterval*values.Count - SamplingInterval,
                                 SamplingRate = _samplingRate,
                                 SamplingInterval = SamplingInterval
                             };
            return signal;
        }

        /// <summary>
        /// Creates the input and output nodes
        /// </summary>
        /// <param name="root"></param>
        protected override sealed void CreateNodes(ref BlockBase root)
        {
            root.OutputNodes = BlockOutputNode.CreateSingleOutputSignal(ref root);
        }


        /// <summary>
        /// Clone the block, including the template
        /// </summary>
        /// <returns></returns>
        public override BlockBase Clone()
        {
            var block = (ImportFromCSVBlock)MemberwiseClone();
            block.Execute();
            return block;
        }

        /// <summary>
        /// Clones this block but mantains the links
        /// </summary>
        /// <returns></returns>
        public override BlockBase CloneWithLinks()
        {
            var block = (ImportFromCSVBlock)MemberwiseCloneWithLinks();
            block.Execute();
            return block;
        }
    }
}

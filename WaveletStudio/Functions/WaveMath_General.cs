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

namespace WaveletStudio.Functions
{
    /// <summary>
    /// Common math and statistical operations
    /// </summary>
    public static partial class WaveMath
    {
        /// <summary>
        /// Calculates the absolute value of a signal
        /// </summary>
        /// <param name="signal"></param>
        /// <param name="samples"></param>
        /// <returns></returns>
        public static void Abs(ref Signal signal, double[] samples)
        {
            if (signal.IsComplex)
            {                
                signal.Samples = AbsFromComplex(samples);
                signal.IsComplex = false;
            }
            else
            {
                signal.Samples = Abs(samples);
            }            
        }
        
        /// <summary>
        /// Calculates the absolute value of an array
        /// </summary>
        /// <param name="samples"></param>
        /// <returns></returns>
        public static double[] Abs(double[] samples)
        {
            var newArray = MemoryPool.Pool.New<double>(samples.Length);
            for (var i = 0; i < samples.Length; i++)
            {
                newArray[i] = Math.Abs(samples[i]);
            }
            return newArray;
        }

        /// <summary>
        /// Calculates the absolute value of a complex array
        /// </summary>
        /// <param name="samples"></param>
        /// <returns></returns>
        public static double[] AbsFromComplex(double[] samples)
        {
            return AbsFromComplex(samples, 0, samples.Length);
        }

        /// <summary>
        /// Calculates the absolute value of a complex array
        /// </summary>
        /// <returns></returns>
        public static double[] AbsFromComplex(double[] samples, int start, int size)
        {
            var newArray = MemoryPool.Pool.New<double>(Convert.ToInt32(Math.Ceiling(size / 2d)));
            for (var i = start; i < size; i = i + 2)
            {
                var real = samples[i];
                var complex = i + 1 < size ? samples[i + 1] : 0;
                newArray[i / 2] = Math.Sqrt(Math.Pow(real, 2) + Math.Pow(complex, 2));
            }
            return newArray;
        }

        /// <summary>
        /// Normalize an array dividing each sample by the array length
        /// </summary>
        /// <param name="samples"></param>
        /// <param name="samplesLength"></param>
        /// <returns></returns>
        public static double[] Normalize(double[] samples, int samplesLength)
        {
            var newArray = (double[])samples.Clone();
            for (var i = 0; i < newArray.Length; i++)
            {
                newArray[i] /= samplesLength;
            }
            return newArray;
        }

        /// <summary>
        /// Sort and removes the repeated values in an array
        /// </summary>
        /// <param name="array"></param>
        /// <returns></returns>
        public static double[] UniqueSorted(double[] array)
        {
            var newArray = (double[])array.Clone();
            Array.Sort(newArray);

            var unqCount = 1;
            for (var i = 1; i < newArray.Length; i++)
                if (Math.Abs(newArray[i - 1] - newArray[i]) > Double.Epsilon)
                    unqCount++;

            var unq = MemoryPool.Pool.New<double>(unqCount);

            unq[0] = newArray[0];
            for (int i = 1, j = 1; i < newArray.Length; i++)
            {
                if (Math.Abs(newArray[i - 1] - newArray[i]) <= Double.Epsilon)
                    continue;
                unq[j] = newArray[i];
                j++;
            }
            return unq;
        }

        /// <summary>
        /// Scales a number based in its minimun and maximum
        /// </summary>
        /// <param name="x"></param>
        /// <param name="currentMin"></param>
        /// <param name="currentMax"></param>
        /// <param name="newMin"></param>
        /// <param name="newMax"></param>
        /// <returns></returns>
        public static double Scale(double x, double currentMin, double currentMax, double newMin, double newMax)
        {
            return newMin + (x - currentMin) / (currentMax - currentMin) * (newMax - newMin);
        }

        /// <summary>
        /// Scales a signal
        /// </summary>
        public static Signal Scale(Signal signal, double amplitudeScalingFactor, double timeScalingFactor)
        {
            var output = signal.Clone();
            if (Math.Abs(amplitudeScalingFactor - 1) > double.Epsilon)
            {
                for (var i = 0; i < output.SamplesCount; i++)
                {
                    output[i] *= amplitudeScalingFactor;
                }
            }
            if (Math.Abs(timeScalingFactor - 1) > double.Epsilon && timeScalingFactor > 0)
            {
                output.Finish *= timeScalingFactor;
                output.SamplingInterval *= timeScalingFactor;
            }
            return output;
        }

        /// <summary>
        /// Decreases the sampling rate of the input by keeping every odd sample starting with the first sample.
        /// </summary>
        /// <param name="input"></param>
        /// <param name="factor"></param>
        /// <param name="invert"></param>
        /// <returns></returns>
        public static double[] DownSample(double[] input, int factor = 2, bool invert = false)
        {
            var size = invert ? Convert.ToInt32(Math.Ceiling((double)input.Length / factor)) : input.Length / factor;
            var result = MemoryPool.Pool.New<double>(size);
            var j = 0;            
            for (var i = 0; i < input.Length; i++)
            {
                if (!invert && i % factor == 0)
                    continue;
                if (invert && i % factor != 0)
                    continue;                
                result[j] = input[i];
                j++;
                if(j>=result.Length)
                    break;
            }
            return result;
        }

        /// <summary>
        /// Increases the sampling rate of the input by inserting n-1 zeros between samples. 
        /// </summary>
        /// <returns></returns>
        public static double[] UpSample(double[] input, int factor = 2, bool paddRight = true)
        {
            //TODO: implement offset
            if (input == null || input.Length == 0)
            {
                return new double[0];
            }
            var size = input.Length * factor;
            var result = MemoryPool.Pool.New<double>(size - (paddRight ? factor-1 : 0));
            for (var i = 0; i < input.Length; i++)
            {
                result[i * factor] = input[i];
            }
            return result;
        }

        /// <summary>
        /// Shift signal
        /// </summary>
        /// <param name="signal"></param>
        /// <param name="delay"></param>
        public static void Shift(ref Signal signal, double delay)
        {
            signal.Start = Convert.ToDouble(Convert.ToDecimal(signal.Start) + Convert.ToDecimal(delay));
            signal.Finish = Convert.ToDouble(Convert.ToDecimal(signal.Finish) + Convert.ToDecimal(delay));
        }

        /// <summary>
        /// Invert an array
        /// </summary>
        public static double[] Invert(double[] input)
        {
            if (input == null)
                return null;
            var output = (double[])input.Clone();
            Array.Reverse(output);
            return output;
        }

        /// <summary>
        /// Returns true if the value of parameter x is a power of 2
        /// </summary>
        public static bool IsPowerOf2(int x)
        {
            return (x != 0) && ((x & (x - 1)) == 0);
        }

        /// <summary>
        /// Repeat samples
        /// </summary>
        public static Signal Repeat(Signal signal, uint frameSize, uint repetitionCount, bool keepSamplingRate)
        {
            var output = signal.Copy();
            output.Samples = Repeat(signal.Samples, frameSize, repetitionCount);
            if (keepSamplingRate)
                output.Finish = output.Start + output.SamplesCount * output.SamplingInterval - output.SamplingInterval;
            else
                output.SamplingInterval = Math.Abs(signal.Finish - signal.Start) / output.SamplesCount;

            return output;
        }

        /// <summary>
        /// Repeat samples
        /// </summary>
        public static double[] Repeat(double[] input, uint frameSize, uint repetitionCount)
        {
            if (input == null)
                return null;
            if (frameSize == 0 || repetitionCount == 0)
                return (double[])input.Clone();
            var output = new List<double>();
            for (uint i = 0; i < input.Length; i = i + frameSize)
            {
                for (var k = 0; k <= repetitionCount; k++)
                {
                    for (var j = 0; j < frameSize; j++)
                    {
                        if (i + j >= input.Length)
                            break;
                        output.Add(input[i + j]);
                    }    
                }                
            }
            return output.ToArray();
        }

        /// <summary>
        /// Limits the range of a number by the maximun and minimun values
        /// </summary>
        public static double LimitRange(double value, double minValue, double maxValue)
        {
            return Math.Max(minValue, Math.Min(value, maxValue));
        }

        /// <summary>
        /// Gets all the samples of the signal separated with a space
        /// </summary>
        /// <param name="signalList"></param>
        /// <param name="precision"></param>
        /// <returns></returns>
        public static string ToString(this List<Signal> signalList, int precision)
        {
            return signalList.Count == 0 ? "" : signalList[0].ToString(precision);
        }

        /// <summary>
        /// Gets all the samples of the signal separated with the specified separator
        /// </summary>
        /// <param name="signalList"></param>
        /// <param name="precision"></param>
        /// <param name="separator"></param>
        /// <returns></returns>
        public static string ToString(this List<Signal> signalList, int precision, string separator)
        {
            return signalList.Count == 0 ? "" : signalList[0].ToString(precision, separator);
        }
    }

    /// <summary>
    /// FFT Computation mode
    /// </summary>
    public enum ManagedFFTModeEnum
    {
        /// <summary>
        /// Store the trigonometric values in a table (faster)
        /// </summary>
        UseLookupTable,
        /// <summary>
        /// Dynamicaly compute the trigonometric values (use less memory)
        /// </summary>
        DynamicTrigonometricValues
    }
}

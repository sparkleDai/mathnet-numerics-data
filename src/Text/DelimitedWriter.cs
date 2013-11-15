﻿// <copyright file="DelimitedWriter.cs" company="Math.NET">
// Math.NET Numerics, part of the Math.NET Project
// http://numerics.mathdotnet.com
// http://github.com/mathnet/mathnet-numerics
// http://mathnetnumerics.codeplex.com
//
// Copyright (c) 2009-2013 Math.NET
//
// Permission is hereby granted, free of charge, to any person
// obtaining a copy of this software and associated documentation
// files (the "Software"), to deal in the Software without
// restriction, including without limitation the rights to use,
// copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the
// Software is furnished to do so, subject to the following
// conditions:
//
// The above copyright notice and this permission notice shall be
// included in all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES
// OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT
// HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY,
// WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING
// FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR
// OTHER DEALINGS IN THE SOFTWARE.
// </copyright>

using System;
using System.Collections.Generic;
using System.IO;
using MathNet.Numerics.LinearAlgebra;

namespace MathNet.Numerics.Data.Text
{
    /// <summary>
    /// Writes an <see cref="Matrix{TDataType}"/> to delimited text file. If the user does not
    /// specify a delimiter, a tab separator is used.
    /// </summary>
    public class DelimitedWriter
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DelimitedWriter"/> class, using a comma as the delimiter.
        /// </summary>
        public DelimitedWriter()
        {
            Delimiter = ",";
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DelimitedWriter"/> class, using the given delimiter.
        /// </summary>
        public DelimitedWriter(string delimiter)
        {
            Delimiter = delimiter;
        }

        /// <summary>
        /// The delimiter to use.
        /// </summary>
        public string Delimiter { get; private set; }

        /// <summary>
        /// Gets or sets column headers. Headers are only written if the header list is neither <c>null</c> nor empty.
        /// </summary>
        public IList<string> ColumnHeaders { get; set; }

        /// <summary>
        /// Gets or sets the number format to use when writing numbers.
        /// </summary>
        public string Format { get; set; }

        /// <summary>
        /// Gets or sets the number format culture provider to use when writing numbers.
        /// </summary>
        public IFormatProvider FormatProvider { get; set; }

        /// <summary>
        /// Writes the given <see cref="Matrix{DataType}"/> to the given file. If the file already exists, 
        /// the file will be overwritten.
        /// </summary>
        /// <param name="matrix">The matrix to write.</param>
        /// <param name="filePath">The file to write the matrix to.</param>
        /// <exception cref="ArgumentNullException">If either <paramref name="matrix"/> or <paramref name="filePath"/> is <c>null</c>.</exception>
        /// <typeparam name="TDataType">The data type of the Matrix. It can be either: double, float, Complex, or Complex32.</typeparam>
        public void WriteMatrix<TDataType>(Matrix<TDataType> matrix, string filePath)
            where TDataType : struct, IEquatable<TDataType>, IFormattable
        {
            WriteFile(matrix, filePath, Delimiter, ColumnHeaders, Format, FormatProvider);
        }

        /// <summary>
        /// Writes the given <see cref="Matrix{DataType}"/> to the given stream.
        /// </summary>
        /// <param name="matrix">The matrix to write.</param>
        /// <param name="stream">The <see cref="Stream"/> to write the matrix to.</param>
        /// <exception cref="ArgumentNullException">If either <paramref name="matrix"/> or <paramref name="stream"/> is <c>null</c>.</exception>
        /// <typeparam name="TDataType">The data type of the Matrix. It can be either: double, float, Complex, or Complex32.</typeparam>
        public void WriteMatrix<TDataType>(Matrix<TDataType> matrix, Stream stream)
            where TDataType : struct, IEquatable<TDataType>, IFormattable
        {
            WriteStream(matrix, stream, Delimiter, ColumnHeaders, Format, FormatProvider);
        }

        /// <summary>
        /// Writes the given <see cref="Matrix{TDataType}"/> to the given <see cref="TextWriter"/>.
        /// </summary>
        /// <param name="matrix">The matrix to write.</param>
        /// <param name="writer">The <see cref="TextWriter"/> to write the matrix to.</param>
        /// <exception cref="ArgumentNullException">If either <paramref name="matrix"/> or <paramref name="writer"/> is <c>null</c>.</exception>
        /// <typeparam name="TDataType">The data type of the Matrix. It can be either: double, float, Complex, or Complex32.</typeparam>
        public void WriteMatrix<TDataType>(Matrix<TDataType> matrix, TextWriter writer)
            where TDataType : struct, IEquatable<TDataType>, IFormattable
        {
            Write(matrix, writer, Delimiter, ColumnHeaders, Format, FormatProvider);
        }

        /// <summary>
        /// Writes a matrix to the given TextWriter. Optionally accepts custom column headers, delimiter, number format and culture.
        /// </summary>
        /// <param name="matrix">The matrix to write.</param>
        /// <param name="writer">The <see cref="TextWriter"/> to write the matrix to. Default: ",".</param>
        /// <param name="delimiter">Number delimiter to write between numbers of the same line.</param>
        /// <param name="columnHeaders">Custom column header. Headers are only written if non-null and non-empty headers are provided. Default: null.</param>
        /// <param name="format">The number format to use on each element. Default: null.</param>
        /// <param name="formatProvider">The culture to use. Default: null.</param>
        /// <exception cref="ArgumentNullException">If either <paramref name="matrix"/> or <paramref name="writer"/> is <c>null</c>.</exception>
        /// <typeparam name="TDataType">The data type of the Matrix. It can be either: Double, Single, Complex, or Complex32.</typeparam>
        public static void Write<TDataType>(Matrix<TDataType> matrix, TextWriter writer, string delimiter = ",", IList<string> columnHeaders = null, string format = null, IFormatProvider formatProvider = null)
            where TDataType : struct, IEquatable<TDataType>, IFormattable
        {
            if (matrix == null)
            {
                throw new ArgumentNullException("matrix");
            }

            if (writer == null)
            {
                throw new ArgumentNullException("writer");
            }

            if (columnHeaders != null && columnHeaders.Count > 0)
            {
                for (var i = 0; i < columnHeaders.Count - 1; i++)
                {
                    writer.Write(columnHeaders[i]);
                    writer.Write(delimiter);
                }

                writer.WriteLine(columnHeaders[columnHeaders.Count - 1]);
            }

            var cols = matrix.ColumnCount - 1;
            var rows = matrix.RowCount - 1;
            for (var i = 0; i < matrix.RowCount; i++)
            {
                for (var j = 0; j < matrix.ColumnCount; j++)
                {
                    writer.Write(matrix[i, j].ToString(format, formatProvider));
                    if (j != cols)
                    {
                        writer.Write(delimiter);
                    }
                }

                if (i != rows)
                {
                    writer.Write(Environment.NewLine);
                }
            }
        }

        /// <summary>
        /// Writes a matrix to the given file. Optionally accepts custom column headers, delimiter, number format and culture.
        /// </summary>
        /// <param name="matrix">The matrix to write.</param>
        /// <param name="filePath">The path and name of the file to write the matrix to. If the file already exists, the file will be overwritten.</param>
        /// <param name="delimiter">Number delimiter to write between numbers of the same line. Default: ",".</param>
        /// <param name="columnHeaders">Custom column header. Headers are only written if non-null and non-empty headers are provided. Default: null.</param>
        /// <param name="format">The number format to use on each element. Default: null.</param>
        /// <param name="formatProvider">The culture to use. Default: null.</param>
        /// <exception cref="ArgumentNullException">If either <paramref name="matrix"/> or <paramref name="filePath"/> is <c>null</c>.</exception>
        /// <typeparam name="TDataType">The data type of the Matrix. It can be either: Double, Single, Complex, or Complex32.</typeparam>
        public static void WriteFile<TDataType>(Matrix<TDataType> matrix, string filePath, string delimiter = ",", IList<string> columnHeaders = null, string format = null, IFormatProvider formatProvider = null)
            where TDataType : struct, IEquatable<TDataType>, IFormattable
        {
            using (var writer = new StreamWriter(filePath))
            {
                Write(matrix, writer, delimiter, columnHeaders, format, formatProvider);
            }
        }

        /// <summary>
        /// Writes a matrix to the given stream. Optionally accepts custom column headers, delimiter, number format and culture.
        /// </summary>
        /// <param name="matrix">The matrix to write.</param>
        /// <param name="stream">The <see cref="Stream"/> to write the matrix to.</param>
        /// <param name="delimiter">Number delimiter to write between numbers of the same line. Default: ",".</param>
        /// <param name="columnHeaders">Custom column header. Headers are only written if non-null and non-empty headers are provided. Default: null.</param>
        /// <param name="format">The number format to use on each element. Default: null.</param>
        /// <param name="formatProvider">The culture to use. Default: null.</param>
        /// <exception cref="ArgumentNullException">If either <paramref name="matrix"/> or <paramref name="stream"/> is <c>null</c>.</exception>
        /// <typeparam name="TDataType">The data type of the Matrix. It can be either: Double, Single, Complex, or Complex32.</typeparam>
        public static void WriteStream<TDataType>(Matrix<TDataType> matrix, Stream stream, string delimiter = ",", IList<string> columnHeaders = null, string format = null, IFormatProvider formatProvider = null)
            where TDataType : struct, IEquatable<TDataType>, IFormattable
        {
            using (var writer = new StreamWriter(stream))
            {
                Write(matrix, writer, delimiter, columnHeaders, format, formatProvider);
            }
        }
    }
}

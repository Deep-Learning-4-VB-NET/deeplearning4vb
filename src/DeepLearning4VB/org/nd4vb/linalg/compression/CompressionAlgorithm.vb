Imports System
Imports System.Runtime.CompilerServices

'
' *  ******************************************************************************
' *  *
' *  *
' *  * This program and the accompanying materials are made available under the
' *  * terms of the Apache License, Version 2.0 which is available at
' *  * https://www.apache.org/licenses/LICENSE-2.0.
' *  *
' *  *  See the NOTICE file distributed with this work for additional
' *  *  information regarding copyright ownership.
' *  * Unless required by applicable law or agreed to in writing, software
' *  * distributed under the License is distributed on an "AS IS" BASIS, WITHOUT
' *  * WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied. See the
' *  * License for the specific language governing permissions and limitations
' *  * under the License.
' *  *
' *  * SPDX-License-Identifier: Apache-2.0
' *  *****************************************************************************
'

Namespace org.nd4vb.linalg.compression

    ''' <summary>
    ''' Compression algorithm enum. The values are the Java ordinals; Java's static methods are
    ''' <see cref="CompressionAlgorithmExtensions.CompressionAlgorithmFromString"/>,
    ''' <see cref="CompressionAlgorithmExtensions.CompressionAlgorithmValues"/> and
    ''' <see cref="CompressionAlgorithmExtensions.CompressionAlgorithmValueOf"/>.
    ''' </summary>
    Public Enum CompressionAlgorithm
        FLOAT8 = 0
        FLOAT16 = 1
        GZIP = 2
        INT8 = 3
        INT16 = 4
        NOOP = 5
        UNIT8 = 6
        CUSTOM = 7
    End Enum

    ''' <summary>The Java static methods of the <c>CompressionAlgorithm</c> enum.</summary>
    Public Module CompressionAlgorithmExtensions

        Private ReadOnly AllValues As CompressionAlgorithm() = {CompressionAlgorithm.FLOAT8, CompressionAlgorithm.FLOAT16, CompressionAlgorithm.GZIP, CompressionAlgorithm.INT8, CompressionAlgorithm.INT16, CompressionAlgorithm.NOOP, CompressionAlgorithm.UNIT8, CompressionAlgorithm.CUSTOM}

        ''' <summary>
        ''' Return the appropriate compression algorithm from the given string (Java <c>fromString</c>; case-insensitive).
        ''' Like Java, "FP16" maps to <c>FLOAT8</c> and "FP32" to <c>FLOAT16</c>.
        ''' </summary>
        ''' <param name="algorithm"> the algorithm to return </param>
        ''' <returns> the compression algorithm from the given string </returns>
        ''' <exception cref="ArgumentException"> if the algorithm is invalid </exception>
        Public Function CompressionAlgorithmFromString(ByVal algorithm As String) As CompressionAlgorithm
            If algorithm Is Nothing Then Throw New ArgumentNullException(NameOf(algorithm))
            Select Case algorithm.ToUpperInvariant()
                Case "FP16"
                    Return CompressionAlgorithm.FLOAT8
                Case "FP32"
                    Return CompressionAlgorithm.FLOAT16
                Case "FLOAT8"
                    Return CompressionAlgorithm.FLOAT8
                Case "FLOAT16"
                    Return CompressionAlgorithm.FLOAT16
                Case "GZIP"
                    Return CompressionAlgorithm.GZIP
                Case "INT8"
                    Return CompressionAlgorithm.INT8
                Case "INT16"
                    Return CompressionAlgorithm.INT16
                Case "NOOP"
                    Return CompressionAlgorithm.NOOP
                Case "UNIT8"
                    Return CompressionAlgorithm.UNIT8
                Case "CUSTOM"
                    Return CompressionAlgorithm.CUSTOM
                Case Else
                    Throw New ArgumentException("Wrong algorithm " & algorithm)
            End Select
        End Function

        ''' <summary>All values in declaration order (Java <c>values()</c>).</summary>
        Public Function CompressionAlgorithmValues() As CompressionAlgorithm()
            Return DirectCast(AllValues.Clone(), CompressionAlgorithm())
        End Function

        ''' <summary>
        ''' The value with exactly this name (Java <c>valueOf</c>; case-sensitive, numeric strings are rejected).
        ''' </summary>
        ''' <exception cref="ArgumentException"> if there is no such value </exception>
        Public Function CompressionAlgorithmValueOf(ByVal name As String) As CompressionAlgorithm
            If name Is Nothing Then Throw New ArgumentNullException(NameOf(name))
            For Each v As CompressionAlgorithm In AllValues
                If String.Equals(v.ToString(), name, StringComparison.Ordinal) Then
                    Return v
                End If
            Next v
            Throw New ArgumentException("No enum constant org.nd4j.linalg.compression.CompressionAlgorithm." & name)
        End Function

        ''' <summary>The Java ordinal.</summary>
        <Extension>
        Public Function ordinal(ByVal algorithm As CompressionAlgorithm) As Integer
            Return CInt(algorithm)
        End Function

    End Module

End Namespace

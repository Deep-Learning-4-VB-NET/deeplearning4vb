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

Imports System.Collections.Generic
Imports System.Text
Imports DeepLearning4VB.org.nd4vb.common.function
Imports WritableT = DeepLearning4VB.org.datavec.api.writable.Writable

Namespace org.datavec.local.transforms.misc

    ''' <summary>
    ''' Writes a sequence as delimited text: each time step as with <see cref="WritablesToStringFunction"/>, time steps
    ''' separated by the time step delimiter (a line feed by default).
    ''' </summary>
    Public Class SequenceWritablesToStringFunction
        Implements [Function](Of IList(Of IList(Of WritableT)), String)

        Public Const DEFAULT_DELIMITER As String = ","
        Public Const DEFAULT_TIME_STEP_DELIMITER As String = vbLf

        Private ReadOnly delimiter As String
        Private ReadOnly timeStepDelimiter As String
        Private ReadOnly quote As String

        ''' <summary>
        ''' Function with default delimiters ("," and "\n")
        ''' </summary>
        Public Sub New()
            Me.New(DEFAULT_DELIMITER)
        End Sub

        ''' <summary>
        ''' function with default delimiter ("\n") between time steps </summary>
        ''' <param name="delim"> Delimiter between values within a single time step </param>
        Public Sub New(ByVal delim As String)
            Me.New(delim, DEFAULT_TIME_STEP_DELIMITER, Nothing)
        End Sub

        ''' <param name="delim">             The delimiter between column values in a single time step (for example, ",") </param>
        ''' <param name="timeStepDelimiter"> The delimiter between time steps (for example, "\n") </param>
        Public Sub New(ByVal delim As String, ByVal timeStepDelimiter As String)
            Me.New(delim, timeStepDelimiter, Nothing)
        End Sub

        ''' <param name="delim">             The delimiter between column values in a single time step (for example, ",") </param>
        ''' <param name="timeStepDelimiter"> The delimiter between time steps (for example, "\n") </param>
        ''' <param name="quote">             The quote for values containing the delimiter, or Nothing for no quoting </param>
        Public Sub New(ByVal delim As String, ByVal timeStepDelimiter As String, ByVal quote As String)
            Me.delimiter = delim
            Me.timeStepDelimiter = timeStepDelimiter
            Me.quote = quote
        End Sub

        Public Overridable Function apply(ByVal c As IList(Of IList(Of WritableT))) As String Implements [Function](Of IList(Of IList(Of WritableT)), String).apply
            Dim sb As New StringBuilder()
            Dim firstLine As Boolean = True
            For Each timeStep As IList(Of WritableT) In c
                If Not firstLine Then
                    sb.Append(timeStepDelimiter)
                End If
                WritablesToStringFunction.append(timeStep, sb, delimiter, quote)
                firstLine = False
            Next timeStep

            Return sb.ToString()
        End Function

    End Class

End Namespace

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

Imports System
Imports System.Text.RegularExpressions
Imports DeepLearning4VB.org.nd4j.common.function
Imports HyperLogLogPlus = DeepLearning4VB.com.clearspring.analytics.stream.cardinality.HyperLogLogPlus
Imports NullWritable = DeepLearning4VB.org.datavec.api.writable.NullWritable
Imports StringMetaData = DeepLearning4VB.org.datavec.api.transform.metadata.StringMetaData
Imports StringQuality = DeepLearning4VB.org.datavec.api.transform.quality.columns.StringQuality
Imports WritableT = DeepLearning4VB.org.datavec.api.writable.Writable

Namespace org.datavec.api.transform.analysis.quality.[string]

    ''' <summary>
    ''' Folds one value into a string quality report: missing when it is a <see cref="NullWritable"/>, otherwise
    ''' valid/invalid as decided by the column metadata; counts of empty, alphabetic, numerical, word-character and
    ''' whitespace-only values; and the value is offered to the distinct-count sketch.
    ''' <para>The character classes are Java's (ASCII) ones, and as in Java the whole string must match, so "alphabetic"
    ''' counts single letters only (the Java pattern is <c>[a-zA-Z]</c> without a quantifier).</para>
    ''' </summary>
    ' Java: @AllArgsConstructor
    <Serializable>
    Public Class StringQualityAddFunction
        Implements BiFunction(Of StringQuality, WritableT, StringQuality)

        ' Java str.matches("[a-zA-Z]"), "\d+", "\w+" and "\s+": whole-string matches with Java's ASCII classes
        ' (.NET's \d, \w and \s also match non-ASCII digits, letters and spaces).
        Private Shared ReadOnly AlphabeticPattern As New Regex("\A[a-zA-Z]\z")
        Private Shared ReadOnly NumericalPattern As New Regex("\A[0-9]+\z")
        Private Shared ReadOnly WordPattern As New Regex("\A[a-zA-Z_0-9]+\z")
        Private Shared ReadOnly WhitespacePattern As New Regex("\A[ \t\n\x0B\f\r]+\z")

        Private ReadOnly meta As StringMetaData

        Public Sub New(ByVal meta As StringMetaData)
            Me.meta = meta
        End Sub

        Public Overridable Function apply(ByVal v1 As StringQuality, ByVal writable As WritableT) As StringQuality Implements BiFunction(Of StringQuality, WritableT, StringQuality).apply
            Dim valid As Long = v1.getCountValid()
            Dim invalid As Long = v1.getCountInvalid()
            Dim countMissing As Long = v1.getCountMissing()
            Dim countTotal As Long = v1.getCountTotal() + 1
            Dim empty As Long = v1.getCountEmptyString()
            Dim alphabetic As Long = v1.getCountAlphabetic()
            Dim numerical As Long = v1.getCountNumerical()
            Dim word As Long = v1.getCountWordCharacter()
            Dim whitespaceOnly As Long = v1.getCountWhitespace()
            Dim hll As HyperLogLogPlus = v1.getHll()

            Dim str As String = writable.ToString()

            If TypeOf writable Is NullWritable Then
                countMissing += 1
            ElseIf meta.isValid(writable) Then
                valid += 1
            Else
                invalid += 1
            End If

            If str Is Nothing OrElse str.Length = 0 Then
                empty += 1
            Else
                If AlphabeticPattern.IsMatch(str) Then
                    alphabetic += 1
                End If
                If NumericalPattern.IsMatch(str) Then
                    numerical += 1
                End If
                If WordPattern.IsMatch(str) Then
                    word += 1
                End If
                If WhitespacePattern.IsMatch(str) Then
                    whitespaceOnly += 1
                End If
            End If

            hll.offer(str)
            Return New StringQuality(valid, invalid, countMissing, countTotal, empty, alphabetic, numerical, word, whitespaceOnly, hll)
        End Function
    End Class

End Namespace

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
Imports DeepLearning4VB.org.nd4j.common.function
Imports DoubleMetaData = DeepLearning4VB.org.datavec.api.transform.metadata.DoubleMetaData
Imports DoubleQuality = DeepLearning4VB.org.datavec.api.transform.quality.columns.DoubleQuality
Imports NullWritable = DeepLearning4VB.org.datavec.api.writable.NullWritable
Imports TextT = DeepLearning4VB.org.datavec.api.writable.Text
Imports WritableT = DeepLearning4VB.org.datavec.api.writable.Writable

Namespace org.datavec.api.transform.analysis.quality.real

    ''' <summary>
    ''' Folds one value into a double quality report: valid/missing/invalid as decided by the column metadata (missing
    ''' = <see cref="NullWritable"/> or empty <see cref="TextT"/>), plus the counts of values whose text is not a double
    ''' (Java's <c>Double.parseDouble</c>), is NaN or is infinite.
    ''' </summary>
    ' Java: @AllArgsConstructor
    <Serializable>
    Public Class RealQualityAddFunction
        Implements BiFunction(Of DoubleQuality, WritableT, DoubleQuality)

        Private ReadOnly meta As DoubleMetaData

        Public Sub New(ByVal meta As DoubleMetaData)
            Me.meta = meta
        End Sub

        Public Overridable Function apply(ByVal v1 As DoubleQuality, ByVal writable As WritableT) As DoubleQuality Implements BiFunction(Of DoubleQuality, WritableT, DoubleQuality).apply

            Dim valid As Long = v1.getCountValid()
            Dim invalid As Long = v1.getCountInvalid()
            Dim countMissing As Long = v1.getCountMissing()
            Dim countTotal As Long = v1.getCountTotal() + 1
            Dim nonReal As Long = v1.getCountNonReal()
            Dim nan As Long = v1.getCountNaN()
            Dim infinite As Long = v1.getCountInfinite()

            If meta.isValid(writable) Then
                valid += 1
            ElseIf TypeOf writable Is NullWritable OrElse (TypeOf writable Is TextT AndAlso (writable.ToString() Is Nothing OrElse writable.ToString().Length = 0)) Then
                countMissing += 1
            Else
                invalid += 1
            End If

            Dim str As String = writable.ToString()
            Dim d As Double
            If JavaNumbers.tryParseDouble(str, d) Then
                If Double.IsNaN(d) Then
                    nan += 1
                End If
                If Double.IsInfinity(d) Then
                    infinite += 1
                End If
            Else
                nonReal += 1
            End If

            Return New DoubleQuality(valid, invalid, countMissing, countTotal, nonReal, nan, infinite)
        End Function
    End Class

End Namespace

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
Imports DeepLearning4VB.org.nd4vb.common.function
Imports IntegerMetaData = DeepLearning4VB.org.datavec.api.transform.metadata.IntegerMetaData
Imports IntegerQuality = DeepLearning4VB.org.datavec.api.transform.quality.columns.IntegerQuality
Imports NullWritable = DeepLearning4VB.org.datavec.api.writable.NullWritable
Imports TextT = DeepLearning4VB.org.datavec.api.writable.Text
Imports WritableT = DeepLearning4VB.org.datavec.api.writable.Writable

Namespace org.datavec.api.transform.analysis.quality.[integer]

    ''' <summary>
    ''' Folds one value into an integer quality report: valid/missing/invalid as decided by the column metadata (missing
    ''' = <see cref="NullWritable"/> or empty <see cref="TextT"/>), plus a count of the values whose text is not an int
    ''' as Java's <c>Integer.parseInt</c> reads it (so a missing value is also counted as non-integer).
    ''' </summary>
    ' Java: @AllArgsConstructor
    <Serializable>
    Public Class IntegerQualityAddFunction
        Implements BiFunction(Of IntegerQuality, WritableT, IntegerQuality)

        Private ReadOnly meta As IntegerMetaData

        Public Sub New(ByVal meta As IntegerMetaData)
            Me.meta = meta
        End Sub

        Public Overridable Function apply(ByVal v1 As IntegerQuality, ByVal writable As WritableT) As IntegerQuality Implements BiFunction(Of IntegerQuality, WritableT, IntegerQuality).apply

            Dim valid As Long = v1.getCountValid()
            Dim invalid As Long = v1.getCountInvalid()
            Dim countMissing As Long = v1.getCountMissing()
            Dim countTotal As Long = v1.getCountTotal() + 1
            Dim nonInteger As Long = v1.getCountNonInteger()

            If meta.isValid(writable) Then
                valid += 1
            ElseIf TypeOf writable Is NullWritable OrElse (TypeOf writable Is TextT AndAlso (writable.ToString() Is Nothing OrElse writable.ToString().Length = 0)) Then
                countMissing += 1
            Else
                invalid += 1
            End If

            Dim str As String = writable.ToString()
            Dim parsed As Integer
            If Not JavaNumbers.tryParseInt(str, parsed) Then
                nonInteger += 1
            End If

            Return New IntegerQuality(valid, invalid, countMissing, countTotal, nonInteger)
        End Function
    End Class

End Namespace

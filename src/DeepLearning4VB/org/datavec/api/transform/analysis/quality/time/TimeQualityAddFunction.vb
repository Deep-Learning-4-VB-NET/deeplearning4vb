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
Imports NullWritable = DeepLearning4VB.org.datavec.api.writable.NullWritable
Imports TextT = DeepLearning4VB.org.datavec.api.writable.Text
Imports TimeMetaData = DeepLearning4VB.org.datavec.api.transform.metadata.TimeMetaData
Imports TimeQuality = DeepLearning4VB.org.datavec.api.transform.quality.columns.TimeQuality
Imports WritableT = DeepLearning4VB.org.datavec.api.writable.Writable

Namespace org.datavec.api.transform.analysis.quality.time

    ''' <summary>
    ''' Folds one value into a time quality report: valid when the column metadata accepts it, missing when it is a
    ''' <see cref="NullWritable"/> or an empty <see cref="TextT"/>, invalid otherwise.
    ''' </summary>
    ' Java: @AllArgsConstructor
    <Serializable>
    Public Class TimeQualityAddFunction
        Implements BiFunction(Of TimeQuality, WritableT, TimeQuality)

        Private ReadOnly meta As TimeMetaData

        Public Sub New(ByVal meta As TimeMetaData)
            Me.meta = meta
        End Sub

        Public Overridable Function apply(ByVal v1 As TimeQuality, ByVal writable As WritableT) As TimeQuality Implements BiFunction(Of TimeQuality, WritableT, TimeQuality).apply

            Dim valid As Long = v1.getCountValid()
            Dim invalid As Long = v1.getCountInvalid()
            Dim countMissing As Long = v1.getCountMissing()
            Dim countTotal As Long = v1.getCountTotal() + 1

            If meta.isValid(writable) Then
                valid += 1
            ElseIf TypeOf writable Is NullWritable OrElse (TypeOf writable Is TextT AndAlso (writable.ToString() Is Nothing OrElse writable.ToString().Length = 0)) Then
                countMissing += 1
            Else
                invalid += 1
            End If

            Return New TimeQuality(valid, invalid, countMissing, countTotal)
        End Function
    End Class

End Namespace

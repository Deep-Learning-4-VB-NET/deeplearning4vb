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

Namespace org.datavec.api.transform.split

    ''' <summary>A random train/test split strategy with the given fraction of examples in the training set.</summary>
    ' Java: @AllArgsConstructor @Data
    <Serializable>
    Public Class RandomSplit
        Implements SplitStrategy

        Private fractionTrain As Double

        Public Sub New()
        End Sub

        Public Sub New(ByVal fractionTrain As Double)
            Me.fractionTrain = fractionTrain
        End Sub

        ''' <summary>Java <c>getFractionTrain()</c> (Lombok).</summary>
        Public Function getFractionTrain() As Double
            Return fractionTrain
        End Function

        ''' <summary>Java <c>setFractionTrain()</c> (Lombok).</summary>
        Public Sub setFractionTrain(ByVal value As Double)
            fractionTrain = value
        End Sub

        Public Overrides Function Equals(ByVal obj As Object) As Boolean
            Dim other As RandomSplit = TryCast(obj, RandomSplit)
            Return other IsNot Nothing AndAlso fractionTrain.Equals(other.fractionTrain)
        End Function

        Public Overrides Function GetHashCode() As Integer
            Return fractionTrain.GetHashCode()
        End Function

        Public Overrides Function ToString() As String
            Return "RandomSplit(fractionTrain=" & fractionTrain.ToString(Globalization.CultureInfo.InvariantCulture) & ")"
        End Function

    End Class

End Namespace

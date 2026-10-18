Imports System
Imports DataType = DeepLearning4VB.org.nd4vb.linalg.api.buffer.DataType
Imports Nd4vbT = DeepLearning4VB.org.nd4vb.linalg.factory.Nd4vb

'
' *  ******************************************************************************
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

Namespace org.nd4vb.linalg.indexing.conditions

    ''' <summary>
    ''' True when the input differs from the value. Like Java, the comparison is in double precision when the default data type (<c>Nd4vb.dataType()</c>) is DOUBLE and in single precision otherwise.
    ''' Condition mode 11 (see <see cref="Condition"/>).
    ''' </summary>
    Public Class NotEqualsCondition
        Inherits BaseCondition

        ''' <param name="value"> the value to compare with (a boxed .NET number) </param>
        Public Sub New(ByVal value As Object)
            MyBase.New(value)
        End Sub

        ''' <summary>A no-op, as in Java: the value is fixed when the condition is created.</summary>
        Public Overrides Sub setValue(ByVal value As Object)
            'no op where we can pass values in
        End Sub

        Public Overrides Function condtionNum() As Integer
            Return 11
        End Function

        Public Overrides Function apply(ByVal input As Object) As Boolean
            If Nd4vbT.dataType() = DataType.DOUBLE Then
                Return doubleValue(input) <> doubleValue(conditionValue)
            Else
                Return floatValue(input) <> floatValue(conditionValue)
            End If
        End Function

    End Class

End Namespace

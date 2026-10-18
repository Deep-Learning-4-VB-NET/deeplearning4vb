Imports System

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
    ''' The negation of a condition.
    ''' </summary>
    Public Class [Not]
        Implements Condition

        Private ReadOnly opposite As Condition

        ''' <param name="condition"> the condition to negate </param>
        Public Sub New(ByVal condition As Condition)
            Me.opposite = condition
        End Sub

        ''' <summary>-1: composite conditions have no native mode.</summary>
        Public Overridable Function condtionNum() As Integer Implements Condition.condtionNum
            Return -1
        End Function

        ''' <summary>-1: composite conditions have no value.</summary>
        Public Overridable ReadOnly Property Value As Double Implements Condition.Value
            Get
                Return -1
            End Get
        End Property

        ''' <summary>A no-op (Java's default <c>Condition.setValue</c>).</summary>
        Public Overridable Sub setValue(ByVal value As Object) Implements Condition.setValue
        End Sub

        Public Overridable Function epsThreshold() As Double Implements Condition.epsThreshold
            Return 0.0
        End Function

        Public Overridable Function apply(ByVal input As Object) As Boolean Implements Condition.apply
            Return Not opposite.apply(input)
        End Function

    End Class

End Namespace

Imports System
Imports System.Globalization
Imports Nd4vbT = DeepLearning4VB.org.nd4vb.linalg.factory.Nd4vb

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

Namespace org.nd4vb.linalg.indexing.conditions

    ''' <summary>
    ''' Base class of the conditions that compare with one value.
    ''' <para>
    ''' Java's <c>Number</c> is <see cref="Object"/> holding a boxed .NET number; <see cref="doubleValue"/> and
    ''' <see cref="floatValue"/> stand in for <c>Number.doubleValue()</c> and <c>Number.floatValue()</c>. Java's field
    ''' <c>value</c> is <see cref="conditionValue"/> (VB names are case-insensitive, so it would clash with
    ''' <see cref="Value"/>).
    ''' </para>
    ''' </summary>
    Public MustInherit Class BaseCondition
        Implements Condition

        ''' <summary>The value the condition compares with (Java's <c>value</c>).</summary>
        Protected Friend conditionValue As Object

        ''' <param name="value"> the value to compare with (a boxed .NET number) </param>
        Public Sub New(ByVal value As Object)
            Me.conditionValue = value
        End Sub

        ''' <summary>Evaluates the condition for one value (Java <c>apply(Number)</c>).</summary>
        Public MustOverride Function apply(ByVal input As Object) As Boolean Implements Condition.apply

        ''' <summary>The condition mode of the native side (see <see cref="Condition"/>).</summary>
        Public MustOverride Function condtionNum() As Integer Implements Condition.condtionNum

        ''' <summary>Replaces the value (most subclasses make this a no-op, as in Java).</summary>
        Public Overridable Sub setValue(ByVal value As Object) Implements Condition.setValue
            Me.conditionValue = value
        End Sub

        ''' <summary>The value as a <see cref="Double"/> (Java <c>getValue()</c>).</summary>
        Public Overridable ReadOnly Property Value As Double Implements Condition.Value
            Get
                Return doubleValue(conditionValue)
            End Get
        End Property

        ''' <summary>The epsilon of the equality conditions: <c>Nd4vb.EPS_THRESHOLD</c> unless overridden.</summary>
        Public Overridable Function epsThreshold() As Double Implements Condition.epsThreshold
            Return Nd4vbT.EPS_THRESHOLD
        End Function

        ''' <summary>Java <c>Number.doubleValue()</c> for a boxed .NET number.</summary>
        Protected Shared Function doubleValue(ByVal number As Object) As Double
            Return Convert.ToDouble(number, CultureInfo.InvariantCulture)
        End Function

        ''' <summary>Java <c>Number.floatValue()</c> for a boxed .NET number.</summary>
        Protected Shared Function floatValue(ByVal number As Object) As Single
            Return Convert.ToSingle(number, CultureInfo.InvariantCulture)
        End Function

    End Class

End Namespace

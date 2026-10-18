Imports System
Imports SysMath = System.Math
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
    ''' True when the input is not within epsilon of the value: <c>|x - value| &gt;= eps</c>, compared in single precision.
    ''' Condition mode 1 (see <see cref="Condition"/>).
    ''' </summary>
    ''' <remarks>Deliberate difference: Java's <c>apply</c> tested <c>|x - value| &lt; EPS_THRESHOLD</c>, the same as
    ''' <see cref="EpsilonEquals"/>, which contradicts the condition and its native mode 1; this port returns the
    ''' opposite of <see cref="EpsilonEquals"/> and uses <see cref="epsThreshold"/>.</remarks>
    Public Class EpsilonNotEquals
        Inherits BaseCondition

        Private ReadOnly eps As Double = Nd4vbT.EPS_THRESHOLD

        ''' <summary>A condition with the value 0 and the default epsilon.</summary>
        Public Sub New()
            Me.New(0.0)
        End Sub

        ''' <summary>A condition with the default epsilon (<c>Nd4vb.EPS_THRESHOLD</c>).</summary>
        Public Sub New(ByVal value As Object)
            Me.New(value, Nd4vbT.EPS_THRESHOLD)
        End Sub

        ''' <param name="value"> the value to compare with (a boxed .NET number) </param>
        ''' <param name="eps"> the epsilon (a boxed .NET number) </param>
        Public Sub New(ByVal value As Object, ByVal eps As Object)
            MyBase.New(value)
            Me.eps = doubleValue(eps)
        End Sub

        ''' <summary>A no-op, as in Java: the value is fixed when the condition is created.</summary>
        Public Overrides Sub setValue(ByVal value As Object)
            'no op where we can pass values in
        End Sub

        Public Overrides Function epsThreshold() As Double
            Return Me.eps
        End Function

        Public Overrides Function condtionNum() As Integer
            Return 1
        End Function

        Public Overrides Function apply(ByVal input As Object) As Boolean
            Return SysMath.Abs(floatValue(input) - floatValue(conditionValue)) >= eps
        End Function

    End Class

End Namespace

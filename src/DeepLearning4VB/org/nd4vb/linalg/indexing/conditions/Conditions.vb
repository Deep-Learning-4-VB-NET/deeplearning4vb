Imports System
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
    ''' Static factory for conditions.
    ''' <para>
    ''' Java's <c>isInfinite()</c>, <c>isNan()</c> and <c>isFinite()</c> are the read-only properties <see cref="Infinite"/>,
    ''' <see cref="Nan"/> and <see cref="Finite"/>; Java <c>Number</c> is <see cref="Object"/> holding a boxed .NET number.
    ''' The <c>equals</c> factories are declared <c>Shadows</c> because VB names are case-insensitive and they would
    ''' otherwise clash with <see cref="Object.Equals(Object)"/>.
    ''' </para>
    ''' </summary>
    Public Class Conditions

        ''' <summary>A new <see cref="IsInfinite"/> condition.</summary>
        Public Shared ReadOnly Property Infinite As Condition
            Get
                Return New IsInfinite()
            End Get
        End Property

        ''' <summary>A new <see cref="IsNaN"/> condition.</summary>
        Public Shared ReadOnly Property Nan As Condition
            Get
                Return New IsNaN()
            End Get
        End Property

        ''' <summary>A new <see cref="IsFinite"/> condition.</summary>
        Public Shared ReadOnly Property Finite As Condition
            Get
                Return New IsFinite()
            End Get
        End Property

        ''' <summary>A new <see cref="NotFinite"/> condition (infinite or NaN).</summary>
        Public Shared Function notFinite() As Condition
            Return New NotFinite()
        End Function

        ''' <summary>A new <see cref="EpsilonNotEquals"/> condition with the default epsilon. The value is 0 (in a pairwise MatchCondition the value is not used).</summary>
        Public Shared Function epsNotEquals() As Condition
            ' in case of pairwise MatchCondition we don't really care about number here
            Return epsNotEquals(0.0)
        End Function

        ''' <summary>A new <see cref="EpsilonNotEquals"/> condition with the default epsilon.</summary>
        Public Shared Function epsNotEquals(ByVal value As Object) As Condition
            Return New EpsilonNotEquals(value)
        End Function

        ''' <summary>A new <see cref="EpsilonEquals"/> condition with the value 0 and the default epsilon.</summary>
        Public Shared Function epsEquals() As Condition
            ' in case of pairwise MatchCondition we don't really care about number here
            Return epsEquals(0.0)
        End Function

        ''' <summary>A new <see cref="EpsilonEquals"/> condition with the default epsilon (<c>Nd4vb.EPS_THRESHOLD</c>).</summary>
        Public Shared Function epsEquals(ByVal value As Object) As Condition
            Return epsEquals(value, Nd4vbT.EPS_THRESHOLD)
        End Function

        ''' <summary>A new <see cref="EpsilonEquals"/> condition.</summary>
        Public Shared Function epsEquals(ByVal value As Object, ByVal epsilon As Object) As Condition
            Return New EpsilonEquals(value, Convert.ToDouble(epsilon, Globalization.CultureInfo.InvariantCulture))
        End Function

        ''' <summary>A new <see cref="EqualsCondition"/> condition. The value is 0 (in a pairwise MatchCondition the value is not used).</summary>
        Public Shared Shadows Function equals() As Condition
            ' in case of pairwise MatchCondition we don't really care about number here
            Return equals(0.0)
        End Function

        ''' <summary>A new <see cref="EqualsCondition"/> condition.</summary>
        Public Shared Shadows Function equals(ByVal value As Object) As Condition
            Return New EqualsCondition(value)
        End Function

        ''' <summary>A new <see cref="NotEqualsCondition"/> condition. The value is 0 (in a pairwise MatchCondition the value is not used).</summary>
        Public Shared Function notEquals() As Condition
            ' in case of pairwise MatchCondition we don't really care about number here
            Return notEquals(0.0)
        End Function

        ''' <summary>A new <see cref="NotEqualsCondition"/> condition.</summary>
        Public Shared Function notEquals(ByVal value As Object) As Condition
            Return New NotEqualsCondition(value)
        End Function

        ''' <summary>A new <see cref="GreaterThan"/> condition. The value is 0 (in a pairwise MatchCondition the value is not used).</summary>
        Public Shared Function greaterThan() As Condition
            ' in case of pairwise MatchCondition we don't really care about number here
            Return greaterThan(0.0)
        End Function

        ''' <summary>A new <see cref="GreaterThan"/> condition.</summary>
        Public Shared Function greaterThan(ByVal value As Object) As Condition
            Return New GreaterThan(value)
        End Function

        ''' <summary>A new <see cref="LessThan"/> condition. The value is 0 (in a pairwise MatchCondition the value is not used).</summary>
        Public Shared Function lessThan() As Condition
            ' in case of pairwise MatchCondition we don't really care about number here
            Return lessThan(0.0)
        End Function

        ''' <summary>A new <see cref="LessThan"/> condition.</summary>
        Public Shared Function lessThan(ByVal value As Object) As Condition
            Return New LessThan(value)
        End Function

        ''' <summary>A new <see cref="LessThanOrEqual"/> condition. The value is 0 (in a pairwise MatchCondition the value is not used).</summary>
        Public Shared Function lessThanOrEqual() As Condition
            ' in case of pairwise MatchCondition we don't really care about number here
            Return lessThanOrEqual(0.0)
        End Function

        ''' <summary>A new <see cref="LessThanOrEqual"/> condition.</summary>
        Public Shared Function lessThanOrEqual(ByVal value As Object) As Condition
            Return New LessThanOrEqual(value)
        End Function

        ''' <summary>A new <see cref="GreaterThanOrEqual"/> condition. The value is 0 (in a pairwise MatchCondition the value is not used).</summary>
        Public Shared Function greaterThanOrEqual() As Condition
            ' in case of pairwise MatchCondition we don't really care about number here
            Return greaterThanOrEqual(0.0)
        End Function

        ''' <summary>A new <see cref="GreaterThanOrEqual"/> condition.</summary>
        Public Shared Function greaterThanOrEqual(ByVal value As Object) As Condition
            Return New GreaterThanOrEqual(value)
        End Function

        ''' <summary>A new <see cref="AbsValueGreaterOrEqualsThan"/> condition. The value is 0 (in a pairwise MatchCondition the value is not used).</summary>
        Public Shared Function absGreaterThanOrEqual() As Condition
            ' in case of pairwise MatchCondition we don't really care about number here
            Return absGreaterThanOrEqual(0.0)
        End Function

        ''' <summary>A new <see cref="AbsValueGreaterOrEqualsThan"/> condition.</summary>
        Public Shared Function absGreaterThanOrEqual(ByVal value As Object) As Condition
            Return New AbsValueGreaterOrEqualsThan(value)
        End Function

        ''' <summary>A new <see cref="AbsValueLessOrEqualsThan"/> condition. The value is 0 (in a pairwise MatchCondition the value is not used).</summary>
        Public Shared Function absLessThanOrEqual() As Condition
            ' in case of pairwise MatchCondition we don't really care about number here
            Return absLessThanOrEqual(0.0)
        End Function

        ''' <summary>A new <see cref="AbsValueLessOrEqualsThan"/> condition.</summary>
        Public Shared Function absLessThanOrEqual(ByVal value As Object) As Condition
            Return New AbsValueLessOrEqualsThan(value)
        End Function

        ''' <summary>A new <see cref="AbsValueGreaterThan"/> condition. The value is 0 (in a pairwise MatchCondition the value is not used).</summary>
        Public Shared Function absGreaterThan() As Condition
            ' in case of pairwise MatchCondition we don't really care about number here
            Return absGreaterThan(0.0)
        End Function

        ''' <summary>A new <see cref="AbsValueGreaterThan"/> condition.</summary>
        Public Shared Function absGreaterThan(ByVal value As Object) As Condition
            Return New AbsValueGreaterThan(value)
        End Function

        ''' <summary>A new <see cref="AbsValueLessThan"/> condition. The value is 0 (in a pairwise MatchCondition the value is not used).</summary>
        Public Shared Function absLessThan() As Condition
            ' in case of pairwise MatchCondition we don't really care about number here
            Return absLessThan(0.0)
        End Function

        ''' <summary>A new <see cref="AbsValueLessThan"/> condition.</summary>
        Public Shared Function absLessThan(ByVal value As Object) As Condition
            Return New AbsValueLessThan(value)
        End Function

    End Class

End Namespace

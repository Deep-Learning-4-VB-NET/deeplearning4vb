Imports System
Imports System.Collections.Generic
Imports ND4VBIllegalStateException = DeepLearning4VB.org.nd4vb.linalg.exception.ND4VBIllegalStateException
Imports Logger = DeepLearning4VB.org.slf4j.Logger
Imports LoggerFactory = DeepLearning4VB.org.slf4j.LoggerFactory
Imports System.Reflection

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

Namespace org.nd4vb.linalg.api.ops.impl.layers.convolution.config

    ''' <summary>
    ''' The base class of the convolution and pooling configurations (Java <c>BaseConvolutionConfig</c>): the property
    ''' map of the configuration (<see cref="toProperties"/>), reflective get and set of its fields for model import, and
    ''' validation.
    ''' <para>
    ''' Differences from Java: <c>java.lang.reflect.Field</c> is <see cref="FieldInfo"/>; a failed reflective access is
    ''' a <see cref="FieldAccessException"/> instead of <c>IllegalAccessException</c>; Java's <c>Number</c> values are the
    ''' boxed .NET numeric types, converted with Java's narrowing rules (truncation toward zero).
    ''' </para>
    ''' </summary>
    Public MustInherit Class BaseConvolutionConfig

        Private Shared ReadOnly log As Logger = LoggerFactory.getLogger(GetType(BaseConvolutionConfig))

        ''' <summary>The configuration as a map of property names to values (the op's <c>propertiesForFunction</c>).</summary>
        Public MustOverride Function toProperties() As IDictionary(Of String, Object)

        ''' <summary>
        ''' Get the value for a given property
        ''' for this function
        ''' </summary>
        ''' <param name="property"> the property to get </param>
        ''' <returns> the value for the function if it exists </returns>
        Public Overridable Function getValue(ByVal [property] As FieldInfo) As Object
            Return [property].GetValue(Me)
        End Function

        ''' <summary>
        ''' Set the value for this function.
        ''' Note that if value is null an <see cref="ND4VBIllegalStateException"/>
        ''' will be thrown.
        ''' </summary>
        ''' <param name="target"> the target field </param>
        ''' <param name="value">  the value to set </param>
        Public Overridable Sub setValueFor(ByVal target As FieldInfo, ByVal value As Object)
            If value Is Nothing Then
                Throw New ND4VBIllegalStateException("Unable to set field " & target.Name & " using null value!")
            End If

            value = ensureProperType(target, value)

            Try
                target.SetValue(Me, value)
            Catch e As FieldAccessException
                log.error("", e)
            End Try
        End Sub

        ''' <summary>True for the boxed numeric types that stand in for Java's <c>Number</c>.</summary>
        Private Shared Function isNumber(ByVal value As Object) As Boolean
            Return TypeOf value Is Double OrElse TypeOf value Is Single OrElse TypeOf value Is Long OrElse
                TypeOf value Is Integer OrElse TypeOf value Is Short OrElse TypeOf value Is SByte OrElse
                TypeOf value Is Byte OrElse TypeOf value Is Decimal
        End Function

        ''' <summary>Java's <c>Number.longValue()</c>: truncation toward zero, saturation, NaN gives 0.</summary>
        Private Shared Function longValue(ByVal value As Object) As Long
            If TypeOf value Is Double OrElse TypeOf value Is Single OrElse TypeOf value Is Decimal Then
                Dim d As Double = Convert.ToDouble(value)
                If Double.IsNaN(d) Then Return 0L
                If d >= 9.2233720368547758E+18 Then Return Long.MaxValue
                If d <= -9.2233720368547758E+18 Then Return Long.MinValue
                Return CLng(Math.Truncate(d))
            End If
            Return Convert.ToInt64(value)
        End Function

        ''' <summary>Java's <c>Number.intValue()</c>: truncation toward zero for floating values, wrap-around for integers.</summary>
        Private Shared Function intValue(ByVal value As Object) As Integer
            If TypeOf value Is Double OrElse TypeOf value Is Single OrElse TypeOf value Is Decimal Then
                Dim d As Double = Convert.ToDouble(value)
                If Double.IsNaN(d) Then Return 0
                If d >= Integer.MaxValue Then Return Integer.MaxValue
                If d <= Integer.MinValue Then Return Integer.MinValue
                Return CInt(Math.Truncate(d))
            End If
            Return Arrays.WrapToInt32(longValue(value))
        End Function

        ''' <summary>Wraps a single number in the one-element array a field of array type expects.</summary>
        Private Function ensureProperType(ByVal targetType As FieldInfo, ByVal value As Object) As Object
            Dim firstClass As Type = targetType.FieldType
            Dim valueType As Type = value.GetType()

            If Not firstClass.Equals(valueType) Then
                If firstClass.Equals(GetType(Integer())) Then
                    If isNumber(value) Then
                        value = intValue(value)
                    End If
                    Return New Integer() {DirectCast(value, Integer)}
                ElseIf firstClass.Equals(GetType(Integer?())) Then
                    If isNumber(value) Then
                        value = intValue(value)
                    End If
                    Return New Integer?() {DirectCast(value, Integer)}
                ElseIf firstClass.Equals(GetType(Long())) Then
                    If isNumber(value) Then
                        value = longValue(value)
                    End If
                    Return New Long() {DirectCast(value, Long)}
                ElseIf firstClass.Equals(GetType(Long?())) Then
                    If isNumber(value) Then
                        value = longValue(value)
                    End If
                    Return New Long?() {DirectCast(value, Long)}
                ElseIf firstClass.Equals(GetType(Double())) Then
                    If isNumber(value) Then
                        value = Convert.ToDouble(value)
                    End If
                    Return New Double() {DirectCast(value, Double)}
                ElseIf firstClass.Equals(GetType(Double?())) Then
                    If isNumber(value) Then
                        value = Convert.ToDouble(value)
                    End If
                    Return New Double?() {DirectCast(value, Double)}
                ElseIf firstClass.Equals(GetType(Single())) Then
                    If isNumber(value) Then
                        value = Convert.ToSingle(value)
                    End If
                    Return New Single() {DirectCast(value, Single)}
                ElseIf firstClass.Equals(GetType(Single?())) Then
                    If isNumber(value) Then
                        value = Convert.ToSingle(value)
                    End If
                    Return New Single?() {DirectCast(value, Single)}
                End If
            End If

            Return value
        End Function

        ''' <summary>Checks the configuration; called by the constructors that set every field.</summary>
        Protected Friend MustOverride Sub validate()

    End Class

End Namespace

Imports System
Imports System.Collections.Generic
Imports ArrayUtil = DeepLearning4VB.org.nd4vb.common.util.ArrayUtil

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

Namespace org.nd4vb.linalg.api.ops.impl.layers.recurrent.config

    ''' <summary>Java <c>org.nd4j.linalg.api.ops.impl.layers.recurrent.config.LSTMConfiguration</c>.</summary>
    Public Class LSTMConfiguration

        ''' <summary>
        ''' Whether to provide peephole connections.
        ''' </summary>
        Private peepHole As Boolean 'IArg(0)

        ''' <summary>
        ''' The data format of the input.  Only used in <seealso cref="LSTMLayer"/>, ignored in <seealso cref="LSTMBlockCell"/>.
        ''' </summary>
        Private dataFormat As RnnDataFormat = RnnDataFormat.TNS 'IArg(1) (only for lstmBlock, not lstmBlockCell)

        ''' <summary>
        ''' The bias added to forget gates in order to reduce the scale of forgetting in the beginning of the training.
        ''' </summary>
        Private forgetBias As Double 'TArg(0)

        ''' <summary>
        ''' Clipping value for cell state, if it is not equal to zero, then cell state is clipped.
        ''' </summary>
        Private clippingCellValue As Double 'TArg(1)

        Public Overridable Function toProperties(ByVal includeDataFormat As Boolean) As IDictionary(Of String, Object)
            Dim ret As IDictionary(Of String, Object) = New JavaCompat.LinkedHashMap(Of String, Object)()
            ret("peepHole") = peepHole
            ret("clippingCellValue") = clippingCellValue
            ret("forgetBias") = forgetBias
            If includeDataFormat Then
                ret("dataFormat") = dataFormat
            End If
            Return ret
        End Function

        Public Overridable Function iArgs(ByVal includeDataFormat As Boolean) As Integer()
            If includeDataFormat Then
                Return New Integer(){ArrayUtil.fromBoolean(peepHole), CInt(dataFormat)}
            Else
                Return New Integer(){ArrayUtil.fromBoolean(peepHole)}
            End If
        End Function

        Public Overridable Function tArgs() As Double()
            Return New Double() {forgetBias, clippingCellValue}
        End Function

        ' ---------------------------------------------------------------- Lombok members

        ''' <summary>Lombok getter of <c>peepHole</c>.</summary>
        Public Overridable Function isPeepHole() As Boolean
            Return peepHole
        End Function

        ''' <summary>Lombok setter of <c>peepHole</c>.</summary>
        Public Overridable Sub setPeepHole(ByVal value As Boolean)
            Me.peepHole = value
        End Sub

        ''' <summary>Lombok getter of <c>dataFormat</c>.</summary>
        Public Overridable Function getDataFormat() As RnnDataFormat
            Return dataFormat
        End Function

        ''' <summary>Lombok setter of <c>dataFormat</c>.</summary>
        Public Overridable Sub setDataFormat(ByVal value As RnnDataFormat)
            Me.dataFormat = value
        End Sub

        ''' <summary>Lombok getter of <c>forgetBias</c>.</summary>
        Public Overridable Function getForgetBias() As Double
            Return forgetBias
        End Function

        ''' <summary>Lombok setter of <c>forgetBias</c>.</summary>
        Public Overridable Sub setForgetBias(ByVal value As Double)
            Me.forgetBias = value
        End Sub

        ''' <summary>Lombok getter of <c>clippingCellValue</c>.</summary>
        Public Overridable Function getClippingCellValue() As Double
            Return clippingCellValue
        End Function

        ''' <summary>Lombok setter of <c>clippingCellValue</c>.</summary>
        Public Overridable Sub setClippingCellValue(ByVal value As Double)
            Me.clippingCellValue = value
        End Sub

        ''' <summary>Lombok equals over the fields.</summary>
        Public Overrides Function Equals(ByVal o As Object) As Boolean
            If o Is Me Then
                Return True
            End If
            Dim other As LSTMConfiguration = TryCast(o, LSTMConfiguration)
            If other Is Nothing Then
                Return False
            End If
            If Not Object.Equals(peepHole, other.peepHole) Then
                Return False
            End If
            If Not Object.Equals(dataFormat, other.dataFormat) Then
                Return False
            End If
            If Not Object.Equals(forgetBias, other.forgetBias) Then
                Return False
            End If
            If Not Object.Equals(clippingCellValue, other.clippingCellValue) Then
                Return False
            End If
            Return True
        End Function

        ''' <summary>Lombok hashCode over the fields.</summary>
        Public Overrides Function GetHashCode() As Integer
            Dim result As Integer = 1
            result = result * 59 + If(CObj(peepHole) Is Nothing, 43, peepHole.GetHashCode())
            result = result * 59 + If(CObj(dataFormat) Is Nothing, 43, dataFormat.GetHashCode())
            result = result * 59 + If(CObj(forgetBias) Is Nothing, 43, forgetBias.GetHashCode())
            result = result * 59 + If(CObj(clippingCellValue) Is Nothing, 43, clippingCellValue.GetHashCode())
            Return result
        End Function

        ''' <summary>Lombok toString over the fields.</summary>
        Public Overrides Function ToString() As String
            Return "LSTMConfiguration(peepHole=" & Arrays.FormatElement(peepHole) & ", dataFormat=" & Arrays.FormatElement(dataFormat) & ", forgetBias=" & Arrays.FormatElement(forgetBias) & ", clippingCellValue=" & Arrays.FormatElement(clippingCellValue) & ")"
        End Function

        ''' <summary>Lombok's builder.</summary>
        Public Shared Function builder() As LSTMConfigurationBuilder
            Return New LSTMConfigurationBuilder()
        End Function

        ''' <summary>The <c>@Builder.Default</c> value of <c>dataFormat</c>.</summary>
        Private Shared Function Default_dataFormat() As RnnDataFormat
            Return RnnDataFormat.TNS
        End Function

        ''' <summary>Lombok's builder of <see cref="LSTMConfiguration"/>; unset fields keep their defaults.</summary>
        Public Class LSTMConfigurationBuilder

            Private _peepHole As Boolean
            Private _dataFormat As RnnDataFormat = Default_dataFormat()
            Private _forgetBias As Double
            Private _clippingCellValue As Double

            Public Function peepHole(ByVal value As Boolean) As LSTMConfigurationBuilder
                _peepHole = value
                Return Me
            End Function

            Public Function dataFormat(ByVal value As RnnDataFormat) As LSTMConfigurationBuilder
                _dataFormat = value
                Return Me
            End Function

            Public Function forgetBias(ByVal value As Double) As LSTMConfigurationBuilder
                _forgetBias = value
                Return Me
            End Function

            Public Function clippingCellValue(ByVal value As Double) As LSTMConfigurationBuilder
                _clippingCellValue = value
                Return Me
            End Function

            Public Function build() As LSTMConfiguration
                Dim result As New LSTMConfiguration()
                result.peepHole = _peepHole
                result.dataFormat = _dataFormat
                result.forgetBias = _forgetBias
                result.clippingCellValue = _clippingCellValue
                Return result
            End Function

        End Class

    End Class

End Namespace

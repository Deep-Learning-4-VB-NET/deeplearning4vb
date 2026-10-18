Imports System
Imports System.Collections.Generic
Imports Pooling2D = DeepLearning4VB.org.nd4vb.linalg.api.ops.impl.layers.convolution.Pooling2D
Imports ConvConfigUtil = DeepLearning4VB.org.nd4vb.linalg.util.ConvConfigUtil

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

    ''' <summary>Java <c>org.nd4j.linalg.api.ops.impl.layers.convolution.config.Pooling2DConfig</c>.</summary>
    Public Class Pooling2DConfig
        Inherits BaseConvolutionConfig

        Public Sub New()
        End Sub

        Private kH As Long = -1, kW As Long = -1
        Private sH As Long = 1, sW As Long = 1
        Private pH As Long = 0, pW As Long = 0
        ''' <summary>
        ''' Extra is an optional parameter mainly for use with pnorm right now.
        ''' All pooling implementations take 9 parameters save pnorm.
        ''' Pnorm takes 10 and is cast to an int.
        ''' </summary>
        Private extra As Double
        Private type As Pooling2D.Pooling2DType = Pooling2D.Pooling2DType.MAX
        Private divisor As Pooling2D.Divisor = Pooling2D.Divisor.EXCLUDE_PADDING
        Private isSameModeField As Boolean
        Private dH As Long = 1
        Private dW As Long = 1
        Private isNHWCField As Boolean = False

        Public Sub New(ByVal kH As Long, ByVal kW As Long, ByVal sH As Long, ByVal sW As Long, ByVal pH As Long, ByVal pW As Long, ByVal extra As Double, ByVal type As Pooling2D.Pooling2DType, ByVal divisor As Pooling2D.Divisor, ByVal isSameMode As Boolean, ByVal dH As Long, ByVal dW As Long, ByVal isNHWC As Boolean)
            Me.kH = kH
            Me.kW = kW
            Me.sH = sH
            Me.sW = sW
            Me.pH = pH
            Me.pW = pW
            Me.extra = extra
            Me.type = type
            Me.divisor = divisor
            Me.isSameModeField = isSameMode
            Me.dH = dH
            Me.dW = dW
            Me.isNHWCField = isNHWC

            validate()
        End Sub

        Public Overrides Function toProperties() As IDictionary(Of String, Object)
            Dim ret As IDictionary(Of String, Object) = New JavaCompat.LinkedHashMap(Of String, Object)()
            ret("kH") = kH
            ret("kW") = kW
            ret("sH") = sH
            ret("sW") = sW
            ret("pH") = pH
            ret("pW") = pW
            ret("extra") = extra
            ret("type") = type.ToString()
            ret("isSameMode") = isSameModeField
            ret("dH") = dH
            ret("dW") = dW
            ret("isNHWC") = isNHWCField
            Return ret
        End Function

        Protected Friend Overrides Sub validate()
            ConvConfigUtil.validate2D(kH, kW, sH, sW, pH, pW, dH, dW)

            'TODO check other args?
        End Sub

        ' ---------------------------------------------------------------- Lombok members

        ''' <summary>Lombok getter of <c>kH</c>.</summary>
        Public Overridable Function getKH() As Long
            Return kH
        End Function

        ''' <summary>Lombok setter of <c>kH</c>.</summary>
        Public Overridable Sub setKH(ByVal value As Long)
            Me.kH = value
        End Sub

        ''' <summary>Lombok getter of <c>kW</c>.</summary>
        Public Overridable Function getKW() As Long
            Return kW
        End Function

        ''' <summary>Lombok setter of <c>kW</c>.</summary>
        Public Overridable Sub setKW(ByVal value As Long)
            Me.kW = value
        End Sub

        ''' <summary>Lombok getter of <c>sH</c>.</summary>
        Public Overridable Function getSH() As Long
            Return sH
        End Function

        ''' <summary>Lombok setter of <c>sH</c>.</summary>
        Public Overridable Sub setSH(ByVal value As Long)
            Me.sH = value
        End Sub

        ''' <summary>Lombok getter of <c>sW</c>.</summary>
        Public Overridable Function getSW() As Long
            Return sW
        End Function

        ''' <summary>Lombok setter of <c>sW</c>.</summary>
        Public Overridable Sub setSW(ByVal value As Long)
            Me.sW = value
        End Sub

        ''' <summary>Lombok getter of <c>pH</c>.</summary>
        Public Overridable Function getPH() As Long
            Return pH
        End Function

        ''' <summary>Lombok setter of <c>pH</c>.</summary>
        Public Overridable Sub setPH(ByVal value As Long)
            Me.pH = value
        End Sub

        ''' <summary>Lombok getter of <c>pW</c>.</summary>
        Public Overridable Function getPW() As Long
            Return pW
        End Function

        ''' <summary>Lombok setter of <c>pW</c>.</summary>
        Public Overridable Sub setPW(ByVal value As Long)
            Me.pW = value
        End Sub

        ''' <summary>Lombok getter of <c>extra</c>.</summary>
        Public Overridable Function getExtra() As Double
            Return extra
        End Function

        ''' <summary>Lombok setter of <c>extra</c>.</summary>
        Public Overridable Sub setExtra(ByVal value As Double)
            Me.extra = value
        End Sub

        ''' <summary>Lombok getter of <c>type</c>.</summary>
        Public Overridable Shadows Function [getType]() As Pooling2D.Pooling2DType
            Return type
        End Function

        ''' <summary>Lombok setter of <c>type</c>.</summary>
        Public Overridable Sub setType(ByVal value As Pooling2D.Pooling2DType)
            Me.type = value
        End Sub

        ''' <summary>Lombok getter of <c>divisor</c>.</summary>
        Public Overridable Function getDivisor() As Pooling2D.Divisor
            Return divisor
        End Function

        ''' <summary>Lombok setter of <c>divisor</c>.</summary>
        Public Overridable Sub setDivisor(ByVal value As Pooling2D.Divisor)
            Me.divisor = value
        End Sub

        ''' <summary>Lombok getter of <c>isSameMode</c>.</summary>
        Public Overridable Function isSameMode() As Boolean
            Return isSameModeField
        End Function

        ''' <summary>Lombok setter of <c>isSameMode</c>.</summary>
        Public Overridable Sub setSameMode(ByVal value As Boolean)
            Me.isSameModeField = value
        End Sub

        ''' <summary>Lombok getter of <c>dH</c>.</summary>
        Public Overridable Function getDH() As Long
            Return dH
        End Function

        ''' <summary>Lombok setter of <c>dH</c>.</summary>
        Public Overridable Sub setDH(ByVal value As Long)
            Me.dH = value
        End Sub

        ''' <summary>Lombok getter of <c>dW</c>.</summary>
        Public Overridable Function getDW() As Long
            Return dW
        End Function

        ''' <summary>Lombok setter of <c>dW</c>.</summary>
        Public Overridable Sub setDW(ByVal value As Long)
            Me.dW = value
        End Sub

        ''' <summary>Lombok getter of <c>isNHWC</c>.</summary>
        Public Overridable Function isNHWC() As Boolean
            Return isNHWCField
        End Function

        ''' <summary>Lombok setter of <c>isNHWC</c>.</summary>
        Public Overridable Sub setNHWC(ByVal value As Boolean)
            Me.isNHWCField = value
        End Sub

        ''' <summary>Lombok equals over the fields.</summary>
        Public Overrides Function Equals(ByVal o As Object) As Boolean
            If o Is Me Then
                Return True
            End If
            Dim other As Pooling2DConfig = TryCast(o, Pooling2DConfig)
            If other Is Nothing Then
                Return False
            End If
            If Not Object.Equals(kH, other.kH) Then
                Return False
            End If
            If Not Object.Equals(kW, other.kW) Then
                Return False
            End If
            If Not Object.Equals(sH, other.sH) Then
                Return False
            End If
            If Not Object.Equals(sW, other.sW) Then
                Return False
            End If
            If Not Object.Equals(pH, other.pH) Then
                Return False
            End If
            If Not Object.Equals(pW, other.pW) Then
                Return False
            End If
            If Not Object.Equals(extra, other.extra) Then
                Return False
            End If
            If Not Object.Equals(type, other.type) Then
                Return False
            End If
            If Not Object.Equals(divisor, other.divisor) Then
                Return False
            End If
            If Not Object.Equals(isSameModeField, other.isSameModeField) Then
                Return False
            End If
            If Not Object.Equals(dH, other.dH) Then
                Return False
            End If
            If Not Object.Equals(dW, other.dW) Then
                Return False
            End If
            If Not Object.Equals(isNHWCField, other.isNHWCField) Then
                Return False
            End If
            Return True
        End Function

        ''' <summary>Lombok hashCode over the fields.</summary>
        Public Overrides Function GetHashCode() As Integer
            Dim result As Integer = 1
            result = result * 59 + If(CObj(kH) Is Nothing, 43, kH.GetHashCode())
            result = result * 59 + If(CObj(kW) Is Nothing, 43, kW.GetHashCode())
            result = result * 59 + If(CObj(sH) Is Nothing, 43, sH.GetHashCode())
            result = result * 59 + If(CObj(sW) Is Nothing, 43, sW.GetHashCode())
            result = result * 59 + If(CObj(pH) Is Nothing, 43, pH.GetHashCode())
            result = result * 59 + If(CObj(pW) Is Nothing, 43, pW.GetHashCode())
            result = result * 59 + If(CObj(extra) Is Nothing, 43, extra.GetHashCode())
            result = result * 59 + If(CObj(type) Is Nothing, 43, type.GetHashCode())
            result = result * 59 + If(CObj(divisor) Is Nothing, 43, divisor.GetHashCode())
            result = result * 59 + If(CObj(isSameModeField) Is Nothing, 43, isSameModeField.GetHashCode())
            result = result * 59 + If(CObj(dH) Is Nothing, 43, dH.GetHashCode())
            result = result * 59 + If(CObj(dW) Is Nothing, 43, dW.GetHashCode())
            result = result * 59 + If(CObj(isNHWCField) Is Nothing, 43, isNHWCField.GetHashCode())
            Return result
        End Function

        ''' <summary>Lombok toString over the fields.</summary>
        Public Overrides Function ToString() As String
            Return "Pooling2DConfig(kH=" & Arrays.FormatElement(kH) & ", kW=" & Arrays.FormatElement(kW) & ", sH=" & Arrays.FormatElement(sH) & ", sW=" & Arrays.FormatElement(sW) & ", pH=" & Arrays.FormatElement(pH) & ", pW=" & Arrays.FormatElement(pW) & ", extra=" & Arrays.FormatElement(extra) & ", type=" & Arrays.FormatElement(type) & ", divisor=" & Arrays.FormatElement(divisor) & ", isSameMode=" & Arrays.FormatElement(isSameModeField) & ", dH=" & Arrays.FormatElement(dH) & ", dW=" & Arrays.FormatElement(dW) & ", isNHWC=" & Arrays.FormatElement(isNHWCField) & ")"
        End Function

        ''' <summary>Lombok's builder.</summary>
        Public Shared Function builder() As Pooling2DConfigBuilder
            Return New Pooling2DConfigBuilder()
        End Function

        ''' <summary>The <c>@Builder.Default</c> value of <c>kH</c>.</summary>
        Private Shared Function Default_kH() As Long
            Return -1
        End Function

        ''' <summary>The <c>@Builder.Default</c> value of <c>kW</c>.</summary>
        Private Shared Function Default_kW() As Long
            Return -1
        End Function

        ''' <summary>The <c>@Builder.Default</c> value of <c>sH</c>.</summary>
        Private Shared Function Default_sH() As Long
            Return 1
        End Function

        ''' <summary>The <c>@Builder.Default</c> value of <c>sW</c>.</summary>
        Private Shared Function Default_sW() As Long
            Return 1
        End Function

        ''' <summary>The <c>@Builder.Default</c> value of <c>pH</c>.</summary>
        Private Shared Function Default_pH() As Long
            Return 0
        End Function

        ''' <summary>The <c>@Builder.Default</c> value of <c>pW</c>.</summary>
        Private Shared Function Default_pW() As Long
            Return 0
        End Function

        ''' <summary>The <c>@Builder.Default</c> value of <c>type</c>.</summary>
        Private Shared Function Default_type() As Pooling2D.Pooling2DType
            Return Pooling2D.Pooling2DType.MAX
        End Function

        ''' <summary>The <c>@Builder.Default</c> value of <c>divisor</c>.</summary>
        Private Shared Function Default_divisor() As Pooling2D.Divisor
            Return Pooling2D.Divisor.EXCLUDE_PADDING
        End Function

        ''' <summary>The <c>@Builder.Default</c> value of <c>dH</c>.</summary>
        Private Shared Function Default_dH() As Long
            Return 1
        End Function

        ''' <summary>The <c>@Builder.Default</c> value of <c>dW</c>.</summary>
        Private Shared Function Default_dW() As Long
            Return 1
        End Function

        ''' <summary>The <c>@Builder.Default</c> value of <c>isNHWC</c>.</summary>
        Private Shared Function Default_isNHWC() As Boolean
            Return False
        End Function

        ''' <summary>Lombok's builder of <see cref="Pooling2DConfig"/>; unset fields keep their defaults.</summary>
        Public Class Pooling2DConfigBuilder

            Private _kH As Long = Default_kH()
            Private _kW As Long = Default_kW()
            Private _sH As Long = Default_sH()
            Private _sW As Long = Default_sW()
            Private _pH As Long = Default_pH()
            Private _pW As Long = Default_pW()
            Private _extra As Double
            Private _type As Pooling2D.Pooling2DType = Default_type()
            Private _divisor As Pooling2D.Divisor = Default_divisor()
            Private _isSameMode As Boolean
            Private _dH As Long = Default_dH()
            Private _dW As Long = Default_dW()
            Private _isNHWC As Boolean = Default_isNHWC()

            Public Function kH(ByVal value As Long) As Pooling2DConfigBuilder
                _kH = value
                Return Me
            End Function

            Public Function kW(ByVal value As Long) As Pooling2DConfigBuilder
                _kW = value
                Return Me
            End Function

            Public Function sH(ByVal value As Long) As Pooling2DConfigBuilder
                _sH = value
                Return Me
            End Function

            Public Function sW(ByVal value As Long) As Pooling2DConfigBuilder
                _sW = value
                Return Me
            End Function

            Public Function pH(ByVal value As Long) As Pooling2DConfigBuilder
                _pH = value
                Return Me
            End Function

            Public Function pW(ByVal value As Long) As Pooling2DConfigBuilder
                _pW = value
                Return Me
            End Function

            Public Function extra(ByVal value As Double) As Pooling2DConfigBuilder
                _extra = value
                Return Me
            End Function

            Public Function type(ByVal value As Pooling2D.Pooling2DType) As Pooling2DConfigBuilder
                _type = value
                Return Me
            End Function

            Public Function divisor(ByVal value As Pooling2D.Divisor) As Pooling2DConfigBuilder
                _divisor = value
                Return Me
            End Function

            Public Function isSameMode(ByVal value As Boolean) As Pooling2DConfigBuilder
                _isSameMode = value
                Return Me
            End Function

            Public Function dH(ByVal value As Long) As Pooling2DConfigBuilder
                _dH = value
                Return Me
            End Function

            Public Function dW(ByVal value As Long) As Pooling2DConfigBuilder
                _dW = value
                Return Me
            End Function

            Public Function isNHWC(ByVal value As Boolean) As Pooling2DConfigBuilder
                _isNHWC = value
                Return Me
            End Function

            Public Function build() As Pooling2DConfig
                Return New Pooling2DConfig(_kH, _kW, _sH, _sW, _pH, _pW, _extra, _type, _divisor, _isSameMode, _dH, _dW, _isNHWC)
            End Function

        End Class

    End Class

End Namespace

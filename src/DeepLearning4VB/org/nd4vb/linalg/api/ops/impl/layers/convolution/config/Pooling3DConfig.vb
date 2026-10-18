Imports System
Imports System.Collections.Generic
Imports Pooling3D = DeepLearning4VB.org.nd4vb.linalg.api.ops.impl.layers.convolution.Pooling3D
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

    ''' <summary>Java <c>org.nd4j.linalg.api.ops.impl.layers.convolution.config.Pooling3DConfig</c>.</summary>
    Public Class Pooling3DConfig
        Inherits BaseConvolutionConfig

        Public Sub New()
        End Sub

        Private kD As Long = -1, kW As Long = -1, kH As Long = -1 ' kernel
        Private sD As Long = 1, sW As Long = 1, sH As Long = 1 ' strides
        Private pD As Long = 0, pW As Long = 0, pH As Long = 0 ' padding
        ' dilation
        Private dD As Long = 1
        Private dW As Long = 1
        Private dH As Long = 1
        Private type As Pooling3D.Pooling3DType = Pooling3D.Pooling3DType.MAX
        Private isSameModeField As Boolean
        Private isNCDHWField As Boolean = True

        Public Sub New(ByVal kD As Long, ByVal kW As Long, ByVal kH As Long, ByVal sD As Long, ByVal sW As Long, ByVal sH As Long, ByVal pD As Long, ByVal pW As Long, ByVal pH As Long, ByVal dD As Long, ByVal dW As Long, ByVal dH As Long, ByVal type As Pooling3D.Pooling3DType, ByVal isSameMode As Boolean, ByVal isNCDHW As Boolean)
            Me.kD = kD
            Me.kW = kW
            Me.kH = kH
            Me.sD = sD
            Me.sW = sW
            Me.sH = sH
            Me.pD = pD
            Me.pW = pW
            Me.pH = pH
            Me.dD = dD
            Me.dW = dW
            Me.dH = dH
            Me.type = type
            Me.isSameModeField = isSameMode
            Me.isNCDHWField = isNCDHW

            validate()
        End Sub

        Public Overrides Function toProperties() As IDictionary(Of String, Object)
            Dim ret As IDictionary(Of String, Object) = New JavaCompat.LinkedHashMap(Of String, Object)()
            ret("kD") = kD
            ret("kW") = kW
            ret("kH") = kH
            ret("sD") = sD
            ret("sW") = sW
            ret("sH") = sH
            ret("pD") = pD
            ret("pW") = pW
            ret("pH") = pH
            ret("dD") = dD
            ret("dW") = dW
            ret("dH") = dH
            ret("type") = type.ToString()
            ret("isSameMode") = isSameModeField
            Return ret

        End Function

        Protected Friend Overrides Sub validate()
            ConvConfigUtil.validate3D(kH, kW, kD, sH, sW, sD, pH, pW, pD, dH, dW, dD)

            'TODO check other args
        End Sub

        ' ---------------------------------------------------------------- Lombok members

        ''' <summary>Lombok getter of <c>kD</c>.</summary>
        Public Overridable Function getKD() As Long
            Return kD
        End Function

        ''' <summary>Lombok setter of <c>kD</c>.</summary>
        Public Overridable Sub setKD(ByVal value As Long)
            Me.kD = value
        End Sub

        ''' <summary>Lombok getter of <c>kW</c>.</summary>
        Public Overridable Function getKW() As Long
            Return kW
        End Function

        ''' <summary>Lombok setter of <c>kW</c>.</summary>
        Public Overridable Sub setKW(ByVal value As Long)
            Me.kW = value
        End Sub

        ''' <summary>Lombok getter of <c>kH</c>.</summary>
        Public Overridable Function getKH() As Long
            Return kH
        End Function

        ''' <summary>Lombok setter of <c>kH</c>.</summary>
        Public Overridable Sub setKH(ByVal value As Long)
            Me.kH = value
        End Sub

        ''' <summary>Lombok getter of <c>sD</c>.</summary>
        Public Overridable Function getSD() As Long
            Return sD
        End Function

        ''' <summary>Lombok setter of <c>sD</c>.</summary>
        Public Overridable Sub setSD(ByVal value As Long)
            Me.sD = value
        End Sub

        ''' <summary>Lombok getter of <c>sW</c>.</summary>
        Public Overridable Function getSW() As Long
            Return sW
        End Function

        ''' <summary>Lombok setter of <c>sW</c>.</summary>
        Public Overridable Sub setSW(ByVal value As Long)
            Me.sW = value
        End Sub

        ''' <summary>Lombok getter of <c>sH</c>.</summary>
        Public Overridable Function getSH() As Long
            Return sH
        End Function

        ''' <summary>Lombok setter of <c>sH</c>.</summary>
        Public Overridable Sub setSH(ByVal value As Long)
            Me.sH = value
        End Sub

        ''' <summary>Lombok getter of <c>pD</c>.</summary>
        Public Overridable Function getPD() As Long
            Return pD
        End Function

        ''' <summary>Lombok setter of <c>pD</c>.</summary>
        Public Overridable Sub setPD(ByVal value As Long)
            Me.pD = value
        End Sub

        ''' <summary>Lombok getter of <c>pW</c>.</summary>
        Public Overridable Function getPW() As Long
            Return pW
        End Function

        ''' <summary>Lombok setter of <c>pW</c>.</summary>
        Public Overridable Sub setPW(ByVal value As Long)
            Me.pW = value
        End Sub

        ''' <summary>Lombok getter of <c>pH</c>.</summary>
        Public Overridable Function getPH() As Long
            Return pH
        End Function

        ''' <summary>Lombok setter of <c>pH</c>.</summary>
        Public Overridable Sub setPH(ByVal value As Long)
            Me.pH = value
        End Sub

        ''' <summary>Lombok getter of <c>dD</c>.</summary>
        Public Overridable Function getDD() As Long
            Return dD
        End Function

        ''' <summary>Lombok setter of <c>dD</c>.</summary>
        Public Overridable Sub setDD(ByVal value As Long)
            Me.dD = value
        End Sub

        ''' <summary>Lombok getter of <c>dW</c>.</summary>
        Public Overridable Function getDW() As Long
            Return dW
        End Function

        ''' <summary>Lombok setter of <c>dW</c>.</summary>
        Public Overridable Sub setDW(ByVal value As Long)
            Me.dW = value
        End Sub

        ''' <summary>Lombok getter of <c>dH</c>.</summary>
        Public Overridable Function getDH() As Long
            Return dH
        End Function

        ''' <summary>Lombok setter of <c>dH</c>.</summary>
        Public Overridable Sub setDH(ByVal value As Long)
            Me.dH = value
        End Sub

        ''' <summary>Lombok getter of <c>type</c>.</summary>
        Public Overridable Shadows Function [getType]() As Pooling3D.Pooling3DType
            Return type
        End Function

        ''' <summary>Lombok setter of <c>type</c>.</summary>
        Public Overridable Sub setType(ByVal value As Pooling3D.Pooling3DType)
            Me.type = value
        End Sub

        ''' <summary>Lombok getter of <c>isSameMode</c>.</summary>
        Public Overridable Function isSameMode() As Boolean
            Return isSameModeField
        End Function

        ''' <summary>Lombok setter of <c>isSameMode</c>.</summary>
        Public Overridable Sub setSameMode(ByVal value As Boolean)
            Me.isSameModeField = value
        End Sub

        ''' <summary>Lombok getter of <c>isNCDHW</c>.</summary>
        Public Overridable Function isNCDHW() As Boolean
            Return isNCDHWField
        End Function

        ''' <summary>Lombok setter of <c>isNCDHW</c>.</summary>
        Public Overridable Sub setNCDHW(ByVal value As Boolean)
            Me.isNCDHWField = value
        End Sub

        ''' <summary>Lombok equals over the fields.</summary>
        Public Overrides Function Equals(ByVal o As Object) As Boolean
            If o Is Me Then
                Return True
            End If
            Dim other As Pooling3DConfig = TryCast(o, Pooling3DConfig)
            If other Is Nothing Then
                Return False
            End If
            If Not Object.Equals(kD, other.kD) Then
                Return False
            End If
            If Not Object.Equals(kW, other.kW) Then
                Return False
            End If
            If Not Object.Equals(kH, other.kH) Then
                Return False
            End If
            If Not Object.Equals(sD, other.sD) Then
                Return False
            End If
            If Not Object.Equals(sW, other.sW) Then
                Return False
            End If
            If Not Object.Equals(sH, other.sH) Then
                Return False
            End If
            If Not Object.Equals(pD, other.pD) Then
                Return False
            End If
            If Not Object.Equals(pW, other.pW) Then
                Return False
            End If
            If Not Object.Equals(pH, other.pH) Then
                Return False
            End If
            If Not Object.Equals(dD, other.dD) Then
                Return False
            End If
            If Not Object.Equals(dW, other.dW) Then
                Return False
            End If
            If Not Object.Equals(dH, other.dH) Then
                Return False
            End If
            If Not Object.Equals(type, other.type) Then
                Return False
            End If
            If Not Object.Equals(isSameModeField, other.isSameModeField) Then
                Return False
            End If
            If Not Object.Equals(isNCDHWField, other.isNCDHWField) Then
                Return False
            End If
            Return True
        End Function

        ''' <summary>Lombok hashCode over the fields.</summary>
        Public Overrides Function GetHashCode() As Integer
            Dim result As Integer = 1
            result = result * 59 + If(CObj(kD) Is Nothing, 43, kD.GetHashCode())
            result = result * 59 + If(CObj(kW) Is Nothing, 43, kW.GetHashCode())
            result = result * 59 + If(CObj(kH) Is Nothing, 43, kH.GetHashCode())
            result = result * 59 + If(CObj(sD) Is Nothing, 43, sD.GetHashCode())
            result = result * 59 + If(CObj(sW) Is Nothing, 43, sW.GetHashCode())
            result = result * 59 + If(CObj(sH) Is Nothing, 43, sH.GetHashCode())
            result = result * 59 + If(CObj(pD) Is Nothing, 43, pD.GetHashCode())
            result = result * 59 + If(CObj(pW) Is Nothing, 43, pW.GetHashCode())
            result = result * 59 + If(CObj(pH) Is Nothing, 43, pH.GetHashCode())
            result = result * 59 + If(CObj(dD) Is Nothing, 43, dD.GetHashCode())
            result = result * 59 + If(CObj(dW) Is Nothing, 43, dW.GetHashCode())
            result = result * 59 + If(CObj(dH) Is Nothing, 43, dH.GetHashCode())
            result = result * 59 + If(CObj(type) Is Nothing, 43, type.GetHashCode())
            result = result * 59 + If(CObj(isSameModeField) Is Nothing, 43, isSameModeField.GetHashCode())
            result = result * 59 + If(CObj(isNCDHWField) Is Nothing, 43, isNCDHWField.GetHashCode())
            Return result
        End Function

        ''' <summary>Lombok toString over the fields.</summary>
        Public Overrides Function ToString() As String
            Return "Pooling3DConfig(kD=" & Arrays.FormatElement(kD) & ", kW=" & Arrays.FormatElement(kW) & ", kH=" & Arrays.FormatElement(kH) & ", sD=" & Arrays.FormatElement(sD) & ", sW=" & Arrays.FormatElement(sW) & ", sH=" & Arrays.FormatElement(sH) & ", pD=" & Arrays.FormatElement(pD) & ", pW=" & Arrays.FormatElement(pW) & ", pH=" & Arrays.FormatElement(pH) & ", dD=" & Arrays.FormatElement(dD) & ", dW=" & Arrays.FormatElement(dW) & ", dH=" & Arrays.FormatElement(dH) & ", type=" & Arrays.FormatElement(type) & ", isSameMode=" & Arrays.FormatElement(isSameModeField) & ", isNCDHW=" & Arrays.FormatElement(isNCDHWField) & ")"
        End Function

        ''' <summary>Lombok's builder.</summary>
        Public Shared Function builder() As Pooling3DConfigBuilder
            Return New Pooling3DConfigBuilder()
        End Function

        ''' <summary>The <c>@Builder.Default</c> value of <c>kD</c>.</summary>
        Private Shared Function Default_kD() As Long
            Return -1
        End Function

        ''' <summary>The <c>@Builder.Default</c> value of <c>kW</c>.</summary>
        Private Shared Function Default_kW() As Long
            Return -1
        End Function

        ''' <summary>The <c>@Builder.Default</c> value of <c>kH</c>.</summary>
        Private Shared Function Default_kH() As Long
            Return -1
        End Function

        ''' <summary>The <c>@Builder.Default</c> value of <c>sD</c>.</summary>
        Private Shared Function Default_sD() As Long
            Return 1
        End Function

        ''' <summary>The <c>@Builder.Default</c> value of <c>sW</c>.</summary>
        Private Shared Function Default_sW() As Long
            Return 1
        End Function

        ''' <summary>The <c>@Builder.Default</c> value of <c>sH</c>.</summary>
        Private Shared Function Default_sH() As Long
            Return 1
        End Function

        ''' <summary>The <c>@Builder.Default</c> value of <c>pD</c>.</summary>
        Private Shared Function Default_pD() As Long
            Return 0
        End Function

        ''' <summary>The <c>@Builder.Default</c> value of <c>pW</c>.</summary>
        Private Shared Function Default_pW() As Long
            Return 0
        End Function

        ''' <summary>The <c>@Builder.Default</c> value of <c>pH</c>.</summary>
        Private Shared Function Default_pH() As Long
            Return 0
        End Function

        ''' <summary>The <c>@Builder.Default</c> value of <c>dD</c>.</summary>
        Private Shared Function Default_dD() As Long
            Return 1
        End Function

        ''' <summary>The <c>@Builder.Default</c> value of <c>dW</c>.</summary>
        Private Shared Function Default_dW() As Long
            Return 1
        End Function

        ''' <summary>The <c>@Builder.Default</c> value of <c>dH</c>.</summary>
        Private Shared Function Default_dH() As Long
            Return 1
        End Function

        ''' <summary>The <c>@Builder.Default</c> value of <c>type</c>.</summary>
        Private Shared Function Default_type() As Pooling3D.Pooling3DType
            Return Pooling3D.Pooling3DType.MAX
        End Function

        ''' <summary>The <c>@Builder.Default</c> value of <c>isNCDHW</c>.</summary>
        Private Shared Function Default_isNCDHW() As Boolean
            Return True
        End Function

        ''' <summary>Lombok's builder of <see cref="Pooling3DConfig"/>; unset fields keep their defaults.</summary>
        Public Class Pooling3DConfigBuilder

            Private _kD As Long = Default_kD()
            Private _kW As Long = Default_kW()
            Private _kH As Long = Default_kH()
            Private _sD As Long = Default_sD()
            Private _sW As Long = Default_sW()
            Private _sH As Long = Default_sH()
            Private _pD As Long = Default_pD()
            Private _pW As Long = Default_pW()
            Private _pH As Long = Default_pH()
            Private _dD As Long = Default_dD()
            Private _dW As Long = Default_dW()
            Private _dH As Long = Default_dH()
            Private _type As Pooling3D.Pooling3DType = Default_type()
            Private _isSameMode As Boolean
            Private _isNCDHW As Boolean = Default_isNCDHW()

            Public Function kD(ByVal value As Long) As Pooling3DConfigBuilder
                _kD = value
                Return Me
            End Function

            Public Function kW(ByVal value As Long) As Pooling3DConfigBuilder
                _kW = value
                Return Me
            End Function

            Public Function kH(ByVal value As Long) As Pooling3DConfigBuilder
                _kH = value
                Return Me
            End Function

            Public Function sD(ByVal value As Long) As Pooling3DConfigBuilder
                _sD = value
                Return Me
            End Function

            Public Function sW(ByVal value As Long) As Pooling3DConfigBuilder
                _sW = value
                Return Me
            End Function

            Public Function sH(ByVal value As Long) As Pooling3DConfigBuilder
                _sH = value
                Return Me
            End Function

            Public Function pD(ByVal value As Long) As Pooling3DConfigBuilder
                _pD = value
                Return Me
            End Function

            Public Function pW(ByVal value As Long) As Pooling3DConfigBuilder
                _pW = value
                Return Me
            End Function

            Public Function pH(ByVal value As Long) As Pooling3DConfigBuilder
                _pH = value
                Return Me
            End Function

            Public Function dD(ByVal value As Long) As Pooling3DConfigBuilder
                _dD = value
                Return Me
            End Function

            Public Function dW(ByVal value As Long) As Pooling3DConfigBuilder
                _dW = value
                Return Me
            End Function

            Public Function dH(ByVal value As Long) As Pooling3DConfigBuilder
                _dH = value
                Return Me
            End Function

            Public Function type(ByVal value As Pooling3D.Pooling3DType) As Pooling3DConfigBuilder
                _type = value
                Return Me
            End Function

            Public Function isSameMode(ByVal value As Boolean) As Pooling3DConfigBuilder
                _isSameMode = value
                Return Me
            End Function

            Public Function isNCDHW(ByVal value As Boolean) As Pooling3DConfigBuilder
                _isNCDHW = value
                Return Me
            End Function

            Public Function build() As Pooling3DConfig
                Return New Pooling3DConfig(_kD, _kW, _kH, _sD, _sW, _sH, _pD, _pW, _pH, _dD, _dW, _dH, _type, _isSameMode, _isNCDHW)
            End Function

        End Class

    End Class

End Namespace

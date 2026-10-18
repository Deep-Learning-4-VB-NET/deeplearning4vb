Imports System
Imports System.Collections.Generic
Imports Preconditions = DeepLearning4VB.org.nd4vb.common.base.Preconditions
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

    ''' <summary>Java <c>org.nd4j.linalg.api.ops.impl.layers.convolution.config.DeConv3DConfig</c>.</summary>
    Public Class DeConv3DConfig
        Inherits BaseConvolutionConfig

        Public Sub New()
        End Sub

        Public Const NCDHW As String = "NCDHW"
        Public Const NDHWC As String = "NDHWC"

        Private kD As Long = -1L
        Private kH As Long = -1L
        Private kW As Long = -1L
        Private sD As Long = 1L
        Private sH As Long = 1L
        Private sW As Long = 1L
        Private pD As Long = 0
        Private pH As Long = 0
        Private pW As Long = 0
        Private dD As Long = 1L
        Private dH As Long = 1L
        Private dW As Long = 1L
        Private isSameModeField As Boolean = False
        Private dataFormat As String = NCDHW

        Public Sub New(ByVal kD As Long, ByVal kH As Long, ByVal kW As Long, ByVal sD As Long, ByVal sH As Long, ByVal sW As Long, ByVal pD As Long, ByVal pH As Long, ByVal pW As Long, ByVal dD As Long, ByVal dH As Long, ByVal dW As Long, ByVal isSameMode As Boolean, ByVal dataFormat As String)
            Me.kD = kD
            Me.kH = kH
            Me.kW = kW
            Me.sD = sD
            Me.sH = sH
            Me.sW = sW
            Me.pD = pD
            Me.pH = pH
            Me.pW = pW
            Me.dD = dD
            Me.dH = dH
            Me.dW = dW
            Me.isSameModeField = isSameMode
            Me.dataFormat = dataFormat

            validate()
        End Sub

        Public Overrides Function toProperties() As IDictionary(Of String, Object)
            Dim ret As IDictionary(Of String, Object) = New JavaCompat.LinkedHashMap(Of String, Object)()
            ret("kD") = kD
            ret("kH") = kH
            ret("kW") = kW
            ret("sD") = sD
            ret("sH") = sH
            ret("sW") = sW
            ret("pD") = pD
            ret("pH") = pH
            ret("pW") = pW
            ret("dD") = dD
            ret("dH") = dH
            ret("dW") = dW
            ret("isSameMode") = isSameModeField
            ret("dataFormat") = dataFormat
            Return ret
        End Function

        Protected Friend Overrides Sub validate()
            ConvConfigUtil.validate3D(kH, kW, kD, sH, sW, sD, pH, pW, pD, dH, dW, dD)
            Preconditions.checkArgument(dataFormat IsNot Nothing, "Data format can't be null")
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

        ''' <summary>Lombok getter of <c>sD</c>.</summary>
        Public Overridable Function getSD() As Long
            Return sD
        End Function

        ''' <summary>Lombok setter of <c>sD</c>.</summary>
        Public Overridable Sub setSD(ByVal value As Long)
            Me.sD = value
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

        ''' <summary>Lombok getter of <c>pD</c>.</summary>
        Public Overridable Function getPD() As Long
            Return pD
        End Function

        ''' <summary>Lombok setter of <c>pD</c>.</summary>
        Public Overridable Sub setPD(ByVal value As Long)
            Me.pD = value
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

        ''' <summary>Lombok getter of <c>dD</c>.</summary>
        Public Overridable Function getDD() As Long
            Return dD
        End Function

        ''' <summary>Lombok setter of <c>dD</c>.</summary>
        Public Overridable Sub setDD(ByVal value As Long)
            Me.dD = value
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

        ''' <summary>Lombok getter of <c>isSameMode</c>.</summary>
        Public Overridable Function isSameMode() As Boolean
            Return isSameModeField
        End Function

        ''' <summary>Lombok setter of <c>isSameMode</c>.</summary>
        Public Overridable Sub setSameMode(ByVal value As Boolean)
            Me.isSameModeField = value
        End Sub

        ''' <summary>Lombok getter of <c>dataFormat</c>.</summary>
        Public Overridable Function getDataFormat() As String
            Return dataFormat
        End Function

        ''' <summary>Lombok setter of <c>dataFormat</c>.</summary>
        Public Overridable Sub setDataFormat(ByVal value As String)
            Me.dataFormat = value
        End Sub

        ''' <summary>Lombok equals over the fields.</summary>
        Public Overrides Function Equals(ByVal o As Object) As Boolean
            If o Is Me Then
                Return True
            End If
            Dim other As DeConv3DConfig = TryCast(o, DeConv3DConfig)
            If other Is Nothing Then
                Return False
            End If
            If Not Object.Equals(kD, other.kD) Then
                Return False
            End If
            If Not Object.Equals(kH, other.kH) Then
                Return False
            End If
            If Not Object.Equals(kW, other.kW) Then
                Return False
            End If
            If Not Object.Equals(sD, other.sD) Then
                Return False
            End If
            If Not Object.Equals(sH, other.sH) Then
                Return False
            End If
            If Not Object.Equals(sW, other.sW) Then
                Return False
            End If
            If Not Object.Equals(pD, other.pD) Then
                Return False
            End If
            If Not Object.Equals(pH, other.pH) Then
                Return False
            End If
            If Not Object.Equals(pW, other.pW) Then
                Return False
            End If
            If Not Object.Equals(dD, other.dD) Then
                Return False
            End If
            If Not Object.Equals(dH, other.dH) Then
                Return False
            End If
            If Not Object.Equals(dW, other.dW) Then
                Return False
            End If
            If Not Object.Equals(isSameModeField, other.isSameModeField) Then
                Return False
            End If
            If Not Object.Equals(dataFormat, other.dataFormat) Then
                Return False
            End If
            Return True
        End Function

        ''' <summary>Lombok hashCode over the fields.</summary>
        Public Overrides Function GetHashCode() As Integer
            Dim result As Integer = 1
            result = result * 59 + If(CObj(kD) Is Nothing, 43, kD.GetHashCode())
            result = result * 59 + If(CObj(kH) Is Nothing, 43, kH.GetHashCode())
            result = result * 59 + If(CObj(kW) Is Nothing, 43, kW.GetHashCode())
            result = result * 59 + If(CObj(sD) Is Nothing, 43, sD.GetHashCode())
            result = result * 59 + If(CObj(sH) Is Nothing, 43, sH.GetHashCode())
            result = result * 59 + If(CObj(sW) Is Nothing, 43, sW.GetHashCode())
            result = result * 59 + If(CObj(pD) Is Nothing, 43, pD.GetHashCode())
            result = result * 59 + If(CObj(pH) Is Nothing, 43, pH.GetHashCode())
            result = result * 59 + If(CObj(pW) Is Nothing, 43, pW.GetHashCode())
            result = result * 59 + If(CObj(dD) Is Nothing, 43, dD.GetHashCode())
            result = result * 59 + If(CObj(dH) Is Nothing, 43, dH.GetHashCode())
            result = result * 59 + If(CObj(dW) Is Nothing, 43, dW.GetHashCode())
            result = result * 59 + If(CObj(isSameModeField) Is Nothing, 43, isSameModeField.GetHashCode())
            result = result * 59 + If(CObj(dataFormat) Is Nothing, 43, dataFormat.GetHashCode())
            Return result
        End Function

        ''' <summary>Lombok toString over the fields.</summary>
        Public Overrides Function ToString() As String
            Return "DeConv3DConfig(kD=" & Arrays.FormatElement(kD) & ", kH=" & Arrays.FormatElement(kH) & ", kW=" & Arrays.FormatElement(kW) & ", sD=" & Arrays.FormatElement(sD) & ", sH=" & Arrays.FormatElement(sH) & ", sW=" & Arrays.FormatElement(sW) & ", pD=" & Arrays.FormatElement(pD) & ", pH=" & Arrays.FormatElement(pH) & ", pW=" & Arrays.FormatElement(pW) & ", dD=" & Arrays.FormatElement(dD) & ", dH=" & Arrays.FormatElement(dH) & ", dW=" & Arrays.FormatElement(dW) & ", isSameMode=" & Arrays.FormatElement(isSameModeField) & ", dataFormat=" & Arrays.FormatElement(dataFormat) & ")"
        End Function

        ''' <summary>Lombok's builder.</summary>
        Public Shared Function builder() As DeConv3DConfigBuilder
            Return New DeConv3DConfigBuilder()
        End Function

        ''' <summary>The <c>@Builder.Default</c> value of <c>kD</c>.</summary>
        Private Shared Function Default_kD() As Long
            Return -1L
        End Function

        ''' <summary>The <c>@Builder.Default</c> value of <c>kH</c>.</summary>
        Private Shared Function Default_kH() As Long
            Return -1L
        End Function

        ''' <summary>The <c>@Builder.Default</c> value of <c>kW</c>.</summary>
        Private Shared Function Default_kW() As Long
            Return -1L
        End Function

        ''' <summary>The <c>@Builder.Default</c> value of <c>sD</c>.</summary>
        Private Shared Function Default_sD() As Long
            Return 1L
        End Function

        ''' <summary>The <c>@Builder.Default</c> value of <c>sH</c>.</summary>
        Private Shared Function Default_sH() As Long
            Return 1L
        End Function

        ''' <summary>The <c>@Builder.Default</c> value of <c>sW</c>.</summary>
        Private Shared Function Default_sW() As Long
            Return 1L
        End Function

        ''' <summary>The <c>@Builder.Default</c> value of <c>pD</c>.</summary>
        Private Shared Function Default_pD() As Long
            Return 0
        End Function

        ''' <summary>The <c>@Builder.Default</c> value of <c>pH</c>.</summary>
        Private Shared Function Default_pH() As Long
            Return 0
        End Function

        ''' <summary>The <c>@Builder.Default</c> value of <c>pW</c>.</summary>
        Private Shared Function Default_pW() As Long
            Return 0
        End Function

        ''' <summary>The <c>@Builder.Default</c> value of <c>dD</c>.</summary>
        Private Shared Function Default_dD() As Long
            Return 1L
        End Function

        ''' <summary>The <c>@Builder.Default</c> value of <c>dH</c>.</summary>
        Private Shared Function Default_dH() As Long
            Return 1L
        End Function

        ''' <summary>The <c>@Builder.Default</c> value of <c>dW</c>.</summary>
        Private Shared Function Default_dW() As Long
            Return 1L
        End Function

        ''' <summary>The <c>@Builder.Default</c> value of <c>isSameMode</c>.</summary>
        Private Shared Function Default_isSameMode() As Boolean
            Return False
        End Function

        ''' <summary>The <c>@Builder.Default</c> value of <c>dataFormat</c>.</summary>
        Private Shared Function Default_dataFormat() As String
            Return NCDHW
        End Function

        ''' <summary>Lombok's builder of <see cref="DeConv3DConfig"/>; unset fields keep their defaults.</summary>
        Public Class DeConv3DConfigBuilder

            Private _kD As Long = Default_kD()
            Private _kH As Long = Default_kH()
            Private _kW As Long = Default_kW()
            Private _sD As Long = Default_sD()
            Private _sH As Long = Default_sH()
            Private _sW As Long = Default_sW()
            Private _pD As Long = Default_pD()
            Private _pH As Long = Default_pH()
            Private _pW As Long = Default_pW()
            Private _dD As Long = Default_dD()
            Private _dH As Long = Default_dH()
            Private _dW As Long = Default_dW()
            Private _isSameMode As Boolean = Default_isSameMode()
            Private _dataFormat As String = Default_dataFormat()

            Public Function kD(ByVal value As Long) As DeConv3DConfigBuilder
                _kD = value
                Return Me
            End Function

            Public Function kH(ByVal value As Long) As DeConv3DConfigBuilder
                _kH = value
                Return Me
            End Function

            Public Function kW(ByVal value As Long) As DeConv3DConfigBuilder
                _kW = value
                Return Me
            End Function

            Public Function sD(ByVal value As Long) As DeConv3DConfigBuilder
                _sD = value
                Return Me
            End Function

            Public Function sH(ByVal value As Long) As DeConv3DConfigBuilder
                _sH = value
                Return Me
            End Function

            Public Function sW(ByVal value As Long) As DeConv3DConfigBuilder
                _sW = value
                Return Me
            End Function

            Public Function pD(ByVal value As Long) As DeConv3DConfigBuilder
                _pD = value
                Return Me
            End Function

            Public Function pH(ByVal value As Long) As DeConv3DConfigBuilder
                _pH = value
                Return Me
            End Function

            Public Function pW(ByVal value As Long) As DeConv3DConfigBuilder
                _pW = value
                Return Me
            End Function

            Public Function dD(ByVal value As Long) As DeConv3DConfigBuilder
                _dD = value
                Return Me
            End Function

            Public Function dH(ByVal value As Long) As DeConv3DConfigBuilder
                _dH = value
                Return Me
            End Function

            Public Function dW(ByVal value As Long) As DeConv3DConfigBuilder
                _dW = value
                Return Me
            End Function

            Public Function isSameMode(ByVal value As Boolean) As DeConv3DConfigBuilder
                _isSameMode = value
                Return Me
            End Function

            Public Function dataFormat(ByVal value As String) As DeConv3DConfigBuilder
                _dataFormat = value
                Return Me
            End Function

            Public Function build() As DeConv3DConfig
                Return New DeConv3DConfig(_kD, _kH, _kW, _sD, _sH, _sW, _pD, _pH, _pW, _dD, _dH, _dW, _isSameMode, _dataFormat)
            End Function

        End Class

    End Class

End Namespace

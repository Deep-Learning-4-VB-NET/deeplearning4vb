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

    ''' <summary>Java <c>org.nd4j.linalg.api.ops.impl.layers.convolution.config.Conv1DConfig</c>.</summary>
    Public Class Conv1DConfig
        Inherits BaseConvolutionConfig

        Public Sub New()
        End Sub

        Public Const NCW As String = "NCW"
        Public Const NWC As String = "NWC"

        Private k As Long = -1L
        Private s As Long = 1 ' strides
        Private p As Long = 0 ' padding
        Private d As Long = 1 ' dilation
        Private dataFormat As String = NCW
        Private paddingMode As PaddingMode

        Public Sub New(ByVal k As Long, ByVal s As Long, ByVal p As Long, ByVal d As Long, ByVal dataFormat As String, ByVal paddingMode As PaddingMode)
            Me.k = k
            Me.s = s
            Me.p = p
            Me.d = d
            Me.dataFormat = dataFormat
            Me.paddingMode = paddingMode

            validate()
        End Sub

        Public Sub New(ByVal k As Long, ByVal s As Long, ByVal p As Long, ByVal dataFormat As String, ByVal isSameMode As Boolean)
            Me.k = k
            Me.s = s
            Me.p = p
            Me.dataFormat = dataFormat
            Me.paddingMode = If(isSameMode, PaddingMode.SAME, PaddingMode.VALID)

            validate()
        End Sub

        ''' <summary>True for the NWC data format (Java <c>isNWC()</c>).</summary>
        Public Overridable Function isNWC() As Boolean
            Preconditions.checkState(dataFormat.Equals(NCW, StringComparison.OrdinalIgnoreCase) OrElse dataFormat.Equals(NWC, StringComparison.OrdinalIgnoreCase), "Data format must be one of %s or %s, got %s", NCW, NWC, dataFormat)
            Return dataFormat.Equals(NWC, StringComparison.OrdinalIgnoreCase)
        End Function

        Public Overridable Sub isNWC(ByVal isNWC As Boolean)
            If isNWC Then
                dataFormat = NWC
            Else
                dataFormat = NCW
            End If
        End Sub

        Public Overrides Function toProperties() As IDictionary(Of String, Object)
            Dim ret As IDictionary(Of String, Object) = New JavaCompat.LinkedHashMap(Of String, Object)()
            ret("k") = k
            ret("s") = s
            ret("p") = p
            ret("d") = d
            ret("paddingMode") = paddingMode
            ret("dataFormat") = dataFormat
            Return ret
        End Function

        Protected Friend Overrides Sub validate()
            ConvConfigUtil.validate1D(k, s, p, d)
            Preconditions.checkArgument(dataFormat IsNot Nothing, "Data format can't be null")
        End Sub

        ' ---------------------------------------------------------------- Lombok members

        ''' <summary>Lombok getter of <c>k</c>.</summary>
        Public Overridable Function getK() As Long
            Return k
        End Function

        ''' <summary>Lombok setter of <c>k</c>.</summary>
        Public Overridable Sub setK(ByVal value As Long)
            Me.k = value
        End Sub

        ''' <summary>Lombok getter of <c>s</c>.</summary>
        Public Overridable Function getS() As Long
            Return s
        End Function

        ''' <summary>Lombok setter of <c>s</c>.</summary>
        Public Overridable Sub setS(ByVal value As Long)
            Me.s = value
        End Sub

        ''' <summary>Lombok getter of <c>p</c>.</summary>
        Public Overridable Function getP() As Long
            Return p
        End Function

        ''' <summary>Lombok setter of <c>p</c>.</summary>
        Public Overridable Sub setP(ByVal value As Long)
            Me.p = value
        End Sub

        ''' <summary>Lombok getter of <c>d</c>.</summary>
        Public Overridable Function getD() As Long
            Return d
        End Function

        ''' <summary>Lombok setter of <c>d</c>.</summary>
        Public Overridable Sub setD(ByVal value As Long)
            Me.d = value
        End Sub

        ''' <summary>Lombok getter of <c>dataFormat</c>.</summary>
        Public Overridable Function getDataFormat() As String
            Return dataFormat
        End Function

        ''' <summary>Lombok setter of <c>dataFormat</c>.</summary>
        Public Overridable Sub setDataFormat(ByVal value As String)
            Me.dataFormat = value
        End Sub

        ''' <summary>Lombok getter of <c>paddingMode</c>.</summary>
        Public Overridable Function getPaddingMode() As PaddingMode
            Return paddingMode
        End Function

        ''' <summary>Lombok setter of <c>paddingMode</c>.</summary>
        Public Overridable Sub setPaddingMode(ByVal value As PaddingMode)
            Me.paddingMode = value
        End Sub

        ''' <summary>Lombok equals over the fields.</summary>
        Public Overrides Function Equals(ByVal o As Object) As Boolean
            If o Is Me Then
                Return True
            End If
            Dim other As Conv1DConfig = TryCast(o, Conv1DConfig)
            If other Is Nothing Then
                Return False
            End If
            If Not Object.Equals(k, other.k) Then
                Return False
            End If
            If Not Object.Equals(s, other.s) Then
                Return False
            End If
            If Not Object.Equals(p, other.p) Then
                Return False
            End If
            If Not Object.Equals(d, other.d) Then
                Return False
            End If
            If Not Object.Equals(dataFormat, other.dataFormat) Then
                Return False
            End If
            If Not Object.Equals(paddingMode, other.paddingMode) Then
                Return False
            End If
            Return True
        End Function

        ''' <summary>Lombok hashCode over the fields.</summary>
        Public Overrides Function GetHashCode() As Integer
            Dim result As Integer = 1
            result = result * 59 + If(CObj(k) Is Nothing, 43, k.GetHashCode())
            result = result * 59 + If(CObj(s) Is Nothing, 43, s.GetHashCode())
            result = result * 59 + If(CObj(p) Is Nothing, 43, p.GetHashCode())
            result = result * 59 + If(CObj(d) Is Nothing, 43, d.GetHashCode())
            result = result * 59 + If(CObj(dataFormat) Is Nothing, 43, dataFormat.GetHashCode())
            result = result * 59 + If(CObj(paddingMode) Is Nothing, 43, paddingMode.GetHashCode())
            Return result
        End Function

        ''' <summary>Lombok toString over the fields.</summary>
        Public Overrides Function ToString() As String
            Return "Conv1DConfig(k=" & Arrays.FormatElement(k) & ", s=" & Arrays.FormatElement(s) & ", p=" & Arrays.FormatElement(p) & ", d=" & Arrays.FormatElement(d) & ", dataFormat=" & Arrays.FormatElement(dataFormat) & ", paddingMode=" & Arrays.FormatElement(paddingMode) & ")"
        End Function

        ''' <summary>Lombok's builder.</summary>
        Public Shared Function builder() As Conv1DConfigBuilder
            Return New Conv1DConfigBuilder()
        End Function

        ''' <summary>The <c>@Builder.Default</c> value of <c>k</c>.</summary>
        Private Shared Function Default_k() As Long
            Return -1L
        End Function

        ''' <summary>The <c>@Builder.Default</c> value of <c>s</c>.</summary>
        Private Shared Function Default_s() As Long
            Return 1
        End Function

        ''' <summary>The <c>@Builder.Default</c> value of <c>p</c>.</summary>
        Private Shared Function Default_p() As Long
            Return 0
        End Function

        ''' <summary>The <c>@Builder.Default</c> value of <c>d</c>.</summary>
        Private Shared Function Default_d() As Long
            Return 1
        End Function

        ''' <summary>The <c>@Builder.Default</c> value of <c>dataFormat</c>.</summary>
        Private Shared Function Default_dataFormat() As String
            Return NCW
        End Function

        ''' <summary>Lombok's builder of <see cref="Conv1DConfig"/>; unset fields keep their defaults.</summary>
        Public Class Conv1DConfigBuilder

            Private _k As Long = Default_k()
            Private _s As Long = Default_s()
            Private _p As Long = Default_p()
            Private _d As Long = Default_d()
            Private _dataFormat As String = Default_dataFormat()
            Private _paddingMode As PaddingMode

            Public Function k(ByVal value As Long) As Conv1DConfigBuilder
                _k = value
                Return Me
            End Function

            Public Function s(ByVal value As Long) As Conv1DConfigBuilder
                _s = value
                Return Me
            End Function

            Public Function p(ByVal value As Long) As Conv1DConfigBuilder
                _p = value
                Return Me
            End Function

            Public Function d(ByVal value As Long) As Conv1DConfigBuilder
                _d = value
                Return Me
            End Function

            Public Function dataFormat(ByVal value As String) As Conv1DConfigBuilder
                _dataFormat = value
                Return Me
            End Function

            Public Function paddingMode(ByVal value As PaddingMode) As Conv1DConfigBuilder
                _paddingMode = value
                Return Me
            End Function

            Public Function build() As Conv1DConfig
                Return New Conv1DConfig(_k, _s, _p, _d, _dataFormat, _paddingMode)
            End Function

        End Class

    End Class

End Namespace

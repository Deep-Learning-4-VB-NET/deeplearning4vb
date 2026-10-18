Imports System
Imports System.Collections.Generic
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

    ''' <summary>Java <c>org.nd4j.linalg.api.ops.impl.layers.convolution.config.LocalResponseNormalizationConfig</c>.</summary>
    Public Class LocalResponseNormalizationConfig
        Inherits BaseConvolutionConfig

        Public Sub New()
        End Sub

        Private alpha, beta, bias As Double
        Private depth As Integer

        Public Sub New(ByVal alpha As Double, ByVal beta As Double, ByVal bias As Double, ByVal depth As Integer)
            Me.alpha = alpha
            Me.beta = beta
            Me.bias = bias
            Me.depth = depth

            validate()
        End Sub

        Public Overrides Function toProperties() As IDictionary(Of String, Object)
            Dim ret As IDictionary(Of String, Object) = New JavaCompat.LinkedHashMap(Of String, Object)()
            ret("alpha") = alpha
            ret("beta") = beta
            ret("bias") = bias
            ret("depth") = depth
            Return ret
        End Function

        Protected Friend Overrides Sub validate()
            ConvConfigUtil.validateLRN(alpha, beta, bias, depth)
        End Sub

        ' ---------------------------------------------------------------- Lombok members

        ''' <summary>Lombok getter of <c>alpha</c>.</summary>
        Public Overridable Function getAlpha() As Double
            Return alpha
        End Function

        ''' <summary>Lombok setter of <c>alpha</c>.</summary>
        Public Overridable Sub setAlpha(ByVal value As Double)
            Me.alpha = value
        End Sub

        ''' <summary>Lombok getter of <c>beta</c>.</summary>
        Public Overridable Function getBeta() As Double
            Return beta
        End Function

        ''' <summary>Lombok setter of <c>beta</c>.</summary>
        Public Overridable Sub setBeta(ByVal value As Double)
            Me.beta = value
        End Sub

        ''' <summary>Lombok getter of <c>bias</c>.</summary>
        Public Overridable Function getBias() As Double
            Return bias
        End Function

        ''' <summary>Lombok setter of <c>bias</c>.</summary>
        Public Overridable Sub setBias(ByVal value As Double)
            Me.bias = value
        End Sub

        ''' <summary>Lombok getter of <c>depth</c>.</summary>
        Public Overridable Function getDepth() As Integer
            Return depth
        End Function

        ''' <summary>Lombok setter of <c>depth</c>.</summary>
        Public Overridable Sub setDepth(ByVal value As Integer)
            Me.depth = value
        End Sub

        ''' <summary>Lombok equals over the fields.</summary>
        Public Overrides Function Equals(ByVal o As Object) As Boolean
            If o Is Me Then
                Return True
            End If
            Dim other As LocalResponseNormalizationConfig = TryCast(o, LocalResponseNormalizationConfig)
            If other Is Nothing Then
                Return False
            End If
            If Not Object.Equals(alpha, other.alpha) Then
                Return False
            End If
            If Not Object.Equals(beta, other.beta) Then
                Return False
            End If
            If Not Object.Equals(bias, other.bias) Then
                Return False
            End If
            If Not Object.Equals(depth, other.depth) Then
                Return False
            End If
            Return True
        End Function

        ''' <summary>Lombok hashCode over the fields.</summary>
        Public Overrides Function GetHashCode() As Integer
            Dim result As Integer = 1
            result = result * 59 + If(CObj(alpha) Is Nothing, 43, alpha.GetHashCode())
            result = result * 59 + If(CObj(beta) Is Nothing, 43, beta.GetHashCode())
            result = result * 59 + If(CObj(bias) Is Nothing, 43, bias.GetHashCode())
            result = result * 59 + If(CObj(depth) Is Nothing, 43, depth.GetHashCode())
            Return result
        End Function

        ''' <summary>Lombok toString over the fields.</summary>
        Public Overrides Function ToString() As String
            Return "LocalResponseNormalizationConfig(alpha=" & Arrays.FormatElement(alpha) & ", beta=" & Arrays.FormatElement(beta) & ", bias=" & Arrays.FormatElement(bias) & ", depth=" & Arrays.FormatElement(depth) & ")"
        End Function

        ''' <summary>Lombok's builder.</summary>
        Public Shared Function builder() As LocalResponseNormalizationConfigBuilder
            Return New LocalResponseNormalizationConfigBuilder()
        End Function

        ''' <summary>Lombok's builder of <see cref="LocalResponseNormalizationConfig"/>; unset fields keep their defaults.</summary>
        Public Class LocalResponseNormalizationConfigBuilder

            Private _alpha As Double
            Private _beta As Double
            Private _bias As Double
            Private _depth As Integer

            Public Function alpha(ByVal value As Double) As LocalResponseNormalizationConfigBuilder
                _alpha = value
                Return Me
            End Function

            Public Function beta(ByVal value As Double) As LocalResponseNormalizationConfigBuilder
                _beta = value
                Return Me
            End Function

            Public Function bias(ByVal value As Double) As LocalResponseNormalizationConfigBuilder
                _bias = value
                Return Me
            End Function

            Public Function depth(ByVal value As Integer) As LocalResponseNormalizationConfigBuilder
                _depth = value
                Return Me
            End Function

            Public Function build() As LocalResponseNormalizationConfig
                Return New LocalResponseNormalizationConfig(_alpha, _beta, _bias, _depth)
            End Function

        End Class

    End Class

End Namespace

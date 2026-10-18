Imports System
Imports INDArray = DeepLearning4VB.org.nd4vb.linalg.api.ndarray.INDArray
#If SAMEDIFF Then
Imports SDVariable = DeepLearning4VB.org.nd4vb.autodiff.samediff.SDVariable
#End If

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

Namespace org.nd4vb.linalg.api.ops.impl.layers.recurrent.weights

    ''' <summary>Java <c>org.nd4j.linalg.api.ops.impl.layers.recurrent.weights.SRUWeights</c>.</summary>
    Public Class SRUWeights
        Inherits RNNWeights

#If SAMEDIFF Then
		''' <summary>
		''' Weights, with shape [inSize, 3*inSize].
		''' </summary>
		Private weights As SDVariable
#End If

        Private iWeights As INDArray

#If SAMEDIFF Then
		''' <summary>
		''' Biases, with shape [2*inSize].
		''' </summary>
		Private bias As SDVariable
#End If

        Private iBias As INDArray

#If SAMEDIFF Then
		Public Overrides Function args() As SDVariable()
			Return New SDVariable(){weights, bias}
		End Function
#End If

        Public Overrides Function arrayArgs() As INDArray()
            Return New INDArray(){iWeights, iBias}
        End Function

        ' ---------------------------------------------------------------- Lombok members

#If SAMEDIFF Then
        ''' <summary>Lombok getter of <c>weights</c>.</summary>
        Public Overridable Function getWeights() As SDVariable
            Return weights
        End Function

        ''' <summary>Lombok setter of <c>weights</c>.</summary>
        Public Overridable Sub setWeights(ByVal value As SDVariable)
            Me.weights = value
        End Sub
#End If

        ''' <summary>Lombok getter of <c>iWeights</c>.</summary>
        Public Overridable Function getIWeights() As INDArray
            Return iWeights
        End Function

        ''' <summary>Lombok setter of <c>iWeights</c>.</summary>
        Public Overridable Sub setIWeights(ByVal value As INDArray)
            Me.iWeights = value
        End Sub

#If SAMEDIFF Then
        ''' <summary>Lombok getter of <c>bias</c>.</summary>
        Public Overridable Function getBias() As SDVariable
            Return bias
        End Function

        ''' <summary>Lombok setter of <c>bias</c>.</summary>
        Public Overridable Sub setBias(ByVal value As SDVariable)
            Me.bias = value
        End Sub
#End If

        ''' <summary>Lombok getter of <c>iBias</c>.</summary>
        Public Overridable Function getIBias() As INDArray
            Return iBias
        End Function

        ''' <summary>Lombok setter of <c>iBias</c>.</summary>
        Public Overridable Sub setIBias(ByVal value As INDArray)
            Me.iBias = value
        End Sub

        ''' <summary>Lombok equals over the fields and the base class.</summary>
        Public Overrides Function Equals(ByVal o As Object) As Boolean
            If o Is Me Then
                Return True
            End If
            Dim other As SRUWeights = TryCast(o, SRUWeights)
            If other Is Nothing Then
                Return False
            End If
            If Not MyBase.Equals(o) Then
                Return False
            End If
            If Not Object.Equals(iWeights, other.iWeights) Then
                Return False
            End If
            If Not Object.Equals(iBias, other.iBias) Then
                Return False
            End If
            Return True
        End Function

        ''' <summary>Lombok hashCode over the fields.</summary>
        Public Overrides Function GetHashCode() As Integer
            Dim result As Integer = MyBase.GetHashCode()
            result = result * 59 + If(CObj(iWeights) Is Nothing, 43, iWeights.GetHashCode())
            result = result * 59 + If(CObj(iBias) Is Nothing, 43, iBias.GetHashCode())
            Return result
        End Function

        ''' <summary>Lombok toString over the fields (fields inside a fence are left out).</summary>
        Public Overrides Function ToString() As String
            Return "SRUWeights(iWeights=" & Arrays.FormatElement(iWeights) & ", iBias=" & Arrays.FormatElement(iBias) & ")"
        End Function

        ''' <summary>Lombok's builder.</summary>
        Public Shared Function builder() As SRUWeightsBuilder
            Return New SRUWeightsBuilder()
        End Function

        ''' <summary>Lombok's builder of <see cref="SRUWeights"/>; unset fields keep their defaults.</summary>
        Public Class SRUWeightsBuilder

#If SAMEDIFF Then
            Private _weights As SDVariable
#End If
            Private _iWeights As INDArray
#If SAMEDIFF Then
            Private _bias As SDVariable
#End If
            Private _iBias As INDArray

#If SAMEDIFF Then
            Public Function weights(ByVal value As SDVariable) As SRUWeightsBuilder
                _weights = value
                Return Me
            End Function

#End If
            Public Function iWeights(ByVal value As INDArray) As SRUWeightsBuilder
                _iWeights = value
                Return Me
            End Function

#If SAMEDIFF Then
            Public Function bias(ByVal value As SDVariable) As SRUWeightsBuilder
                _bias = value
                Return Me
            End Function

#End If
            Public Function iBias(ByVal value As INDArray) As SRUWeightsBuilder
                _iBias = value
                Return Me
            End Function

            Public Function build() As SRUWeights
                Dim result As New SRUWeights()
#If SAMEDIFF Then
                result.weights = _weights
#End If
                result.iWeights = _iWeights
#If SAMEDIFF Then
                result.bias = _bias
#End If
                result.iBias = _iBias
                Return result
            End Function

        End Class

    End Class

End Namespace

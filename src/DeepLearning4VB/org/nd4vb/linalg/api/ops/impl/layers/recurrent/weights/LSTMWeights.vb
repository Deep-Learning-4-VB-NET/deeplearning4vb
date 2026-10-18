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

    ''' <summary>Java <c>org.nd4j.linalg.api.ops.impl.layers.recurrent.weights.LSTMWeights</c>.</summary>
    Public Class LSTMWeights
        Inherits RNNWeights

#If SAMEDIFF Then
		''' <summary>
		''' Input to hidden weights and hidden to hidden weights, with a shape of [inSize + numUnits, 4*numUnits].
		''' 
		''' Input to hidden and hidden to hidden are concatenated in dimension 0,
		''' so the input to hidden weights are [:inSize, :] and the hidden to hidden weights are [inSize:, :].
		''' </summary>
		Private weights As SDVariable
#End If

        Private iWeights As INDArray

#If SAMEDIFF Then
		''' <summary>
		''' Cell peephole (t-1) connections to input modulation gate, with a shape of [numUnits].
		''' </summary>
		Private inputPeepholeWeights As SDVariable
#End If

        Private iInputPeepholeWeights As INDArray

#If SAMEDIFF Then
		''' <summary>
		''' Cell peephole (t-1) connections to forget gate, with a shape of [numUnits].
		''' </summary>
		Private forgetPeepholeWeights As SDVariable
#End If

        Private iForgetPeepholeWeights As INDArray

#If SAMEDIFF Then
		''' <summary>
		''' Cell peephole (t) connections to output gate, with a shape of [numUnits].
		''' </summary>
		Private outputPeepholeWeights As SDVariable
#End If

        Private iOutputPeepholeWeights As INDArray

#If SAMEDIFF Then
		''' <summary>
		''' Input to hidden and hidden to hidden biases, with shape [1, 4*numUnits].
		''' </summary>
		Private bias As SDVariable
#End If

        Private iBias As INDArray

#If SAMEDIFF Then
		Public Overrides Function args() As SDVariable()
			Return filterNonNull(weights, inputPeepholeWeights, forgetPeepholeWeights, outputPeepholeWeights, bias)
		End Function
#End If

        Public Overrides Function arrayArgs() As INDArray()
            Return filterNonNull(iWeights, iInputPeepholeWeights, iForgetPeepholeWeights, iOutputPeepholeWeights, iBias)
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
        ''' <summary>Lombok getter of <c>inputPeepholeWeights</c>.</summary>
        Public Overridable Function getInputPeepholeWeights() As SDVariable
            Return inputPeepholeWeights
        End Function

        ''' <summary>Lombok setter of <c>inputPeepholeWeights</c>.</summary>
        Public Overridable Sub setInputPeepholeWeights(ByVal value As SDVariable)
            Me.inputPeepholeWeights = value
        End Sub
#End If

        ''' <summary>Lombok getter of <c>iInputPeepholeWeights</c>.</summary>
        Public Overridable Function getIInputPeepholeWeights() As INDArray
            Return iInputPeepholeWeights
        End Function

        ''' <summary>Lombok setter of <c>iInputPeepholeWeights</c>.</summary>
        Public Overridable Sub setIInputPeepholeWeights(ByVal value As INDArray)
            Me.iInputPeepholeWeights = value
        End Sub

#If SAMEDIFF Then
        ''' <summary>Lombok getter of <c>forgetPeepholeWeights</c>.</summary>
        Public Overridable Function getForgetPeepholeWeights() As SDVariable
            Return forgetPeepholeWeights
        End Function

        ''' <summary>Lombok setter of <c>forgetPeepholeWeights</c>.</summary>
        Public Overridable Sub setForgetPeepholeWeights(ByVal value As SDVariable)
            Me.forgetPeepholeWeights = value
        End Sub
#End If

        ''' <summary>Lombok getter of <c>iForgetPeepholeWeights</c>.</summary>
        Public Overridable Function getIForgetPeepholeWeights() As INDArray
            Return iForgetPeepholeWeights
        End Function

        ''' <summary>Lombok setter of <c>iForgetPeepholeWeights</c>.</summary>
        Public Overridable Sub setIForgetPeepholeWeights(ByVal value As INDArray)
            Me.iForgetPeepholeWeights = value
        End Sub

#If SAMEDIFF Then
        ''' <summary>Lombok getter of <c>outputPeepholeWeights</c>.</summary>
        Public Overridable Function getOutputPeepholeWeights() As SDVariable
            Return outputPeepholeWeights
        End Function

        ''' <summary>Lombok setter of <c>outputPeepholeWeights</c>.</summary>
        Public Overridable Sub setOutputPeepholeWeights(ByVal value As SDVariable)
            Me.outputPeepholeWeights = value
        End Sub
#End If

        ''' <summary>Lombok getter of <c>iOutputPeepholeWeights</c>.</summary>
        Public Overridable Function getIOutputPeepholeWeights() As INDArray
            Return iOutputPeepholeWeights
        End Function

        ''' <summary>Lombok setter of <c>iOutputPeepholeWeights</c>.</summary>
        Public Overridable Sub setIOutputPeepholeWeights(ByVal value As INDArray)
            Me.iOutputPeepholeWeights = value
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
            Dim other As LSTMWeights = TryCast(o, LSTMWeights)
            If other Is Nothing Then
                Return False
            End If
            If Not MyBase.Equals(o) Then
                Return False
            End If
            If Not Object.Equals(iWeights, other.iWeights) Then
                Return False
            End If
            If Not Object.Equals(iInputPeepholeWeights, other.iInputPeepholeWeights) Then
                Return False
            End If
            If Not Object.Equals(iForgetPeepholeWeights, other.iForgetPeepholeWeights) Then
                Return False
            End If
            If Not Object.Equals(iOutputPeepholeWeights, other.iOutputPeepholeWeights) Then
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
            result = result * 59 + If(CObj(iInputPeepholeWeights) Is Nothing, 43, iInputPeepholeWeights.GetHashCode())
            result = result * 59 + If(CObj(iForgetPeepholeWeights) Is Nothing, 43, iForgetPeepholeWeights.GetHashCode())
            result = result * 59 + If(CObj(iOutputPeepholeWeights) Is Nothing, 43, iOutputPeepholeWeights.GetHashCode())
            result = result * 59 + If(CObj(iBias) Is Nothing, 43, iBias.GetHashCode())
            Return result
        End Function

        ''' <summary>Lombok toString over the fields (fields inside a fence are left out).</summary>
        Public Overrides Function ToString() As String
            Return "LSTMWeights(iWeights=" & Arrays.FormatElement(iWeights) & ", iInputPeepholeWeights=" & Arrays.FormatElement(iInputPeepholeWeights) & ", iForgetPeepholeWeights=" & Arrays.FormatElement(iForgetPeepholeWeights) & ", iOutputPeepholeWeights=" & Arrays.FormatElement(iOutputPeepholeWeights) & ", iBias=" & Arrays.FormatElement(iBias) & ")"
        End Function

        ''' <summary>Lombok's builder.</summary>
        Public Shared Function builder() As LSTMWeightsBuilder
            Return New LSTMWeightsBuilder()
        End Function

        ''' <summary>Lombok's builder of <see cref="LSTMWeights"/>; unset fields keep their defaults.</summary>
        Public Class LSTMWeightsBuilder

#If SAMEDIFF Then
            Private _weights As SDVariable
#End If
            Private _iWeights As INDArray
#If SAMEDIFF Then
            Private _inputPeepholeWeights As SDVariable
#End If
            Private _iInputPeepholeWeights As INDArray
#If SAMEDIFF Then
            Private _forgetPeepholeWeights As SDVariable
#End If
            Private _iForgetPeepholeWeights As INDArray
#If SAMEDIFF Then
            Private _outputPeepholeWeights As SDVariable
#End If
            Private _iOutputPeepholeWeights As INDArray
#If SAMEDIFF Then
            Private _bias As SDVariable
#End If
            Private _iBias As INDArray

#If SAMEDIFF Then
            Public Function weights(ByVal value As SDVariable) As LSTMWeightsBuilder
                _weights = value
                Return Me
            End Function

#End If
            Public Function iWeights(ByVal value As INDArray) As LSTMWeightsBuilder
                _iWeights = value
                Return Me
            End Function

#If SAMEDIFF Then
            Public Function inputPeepholeWeights(ByVal value As SDVariable) As LSTMWeightsBuilder
                _inputPeepholeWeights = value
                Return Me
            End Function

#End If
            Public Function iInputPeepholeWeights(ByVal value As INDArray) As LSTMWeightsBuilder
                _iInputPeepholeWeights = value
                Return Me
            End Function

#If SAMEDIFF Then
            Public Function forgetPeepholeWeights(ByVal value As SDVariable) As LSTMWeightsBuilder
                _forgetPeepholeWeights = value
                Return Me
            End Function

#End If
            Public Function iForgetPeepholeWeights(ByVal value As INDArray) As LSTMWeightsBuilder
                _iForgetPeepholeWeights = value
                Return Me
            End Function

#If SAMEDIFF Then
            Public Function outputPeepholeWeights(ByVal value As SDVariable) As LSTMWeightsBuilder
                _outputPeepholeWeights = value
                Return Me
            End Function

#End If
            Public Function iOutputPeepholeWeights(ByVal value As INDArray) As LSTMWeightsBuilder
                _iOutputPeepholeWeights = value
                Return Me
            End Function

#If SAMEDIFF Then
            Public Function bias(ByVal value As SDVariable) As LSTMWeightsBuilder
                _bias = value
                Return Me
            End Function

#End If
            Public Function iBias(ByVal value As INDArray) As LSTMWeightsBuilder
                _iBias = value
                Return Me
            End Function

            Public Function build() As LSTMWeights
                Dim result As New LSTMWeights()
#If SAMEDIFF Then
                result.weights = _weights
#End If
                result.iWeights = _iWeights
#If SAMEDIFF Then
                result.inputPeepholeWeights = _inputPeepholeWeights
#End If
                result.iInputPeepholeWeights = _iInputPeepholeWeights
#If SAMEDIFF Then
                result.forgetPeepholeWeights = _forgetPeepholeWeights
#End If
                result.iForgetPeepholeWeights = _iForgetPeepholeWeights
#If SAMEDIFF Then
                result.outputPeepholeWeights = _outputPeepholeWeights
#End If
                result.iOutputPeepholeWeights = _iOutputPeepholeWeights
#If SAMEDIFF Then
                result.bias = _bias
#End If
                result.iBias = _iBias
                Return result
            End Function

        End Class

    End Class

End Namespace

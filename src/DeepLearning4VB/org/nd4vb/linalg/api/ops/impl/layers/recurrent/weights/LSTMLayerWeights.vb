Imports System
Imports Preconditions = DeepLearning4VB.org.nd4vb.common.base.Preconditions
Imports INDArray = DeepLearning4VB.org.nd4vb.linalg.api.ndarray.INDArray
Imports ArrayUtil = DeepLearning4VB.org.nd4vb.common.util.ArrayUtil
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

    ''' <summary>Java <c>org.nd4j.linalg.api.ops.impl.layers.recurrent.weights.LSTMLayerWeights</c>.</summary>
    Public Class LSTMLayerWeights
        Inherits RNNWeights

#If SAMEDIFF Then
		''' <summary>
		''' Input to hidden weights with a shape of [inSize, 4*numUnits].
		''' 
		''' Input to hidden and hidden to hidden are concatenated in dimension 0,
		''' so the input to hidden weights are [:inSize, :] and the hidden to hidden weights are [inSize:, :].
		''' </summary>
		Private weights As SDVariable
#End If

        Private iWeights As INDArray

#If SAMEDIFF Then
		''' <summary>
		''' hidden to hidden weights (aka "recurrent weights", with a shape of [numUnits, 4*numUnits].
		''' 
		''' </summary>
		Private rWeights As SDVariable
#End If

        Private irWeights As INDArray

#If SAMEDIFF Then
		''' <summary>
		''' Peephole weights, with a shape of [3*numUnits].
		''' </summary>
		Private peepholeWeights As SDVariable
#End If

        Private iPeepholeWeights As INDArray

#If SAMEDIFF Then
		''' <summary>
		''' Input to hidden and hidden to hidden biases, with shape [4*numUnits].
		''' </summary>
		Private bias As SDVariable
#End If

        Private iBias As INDArray

#If SAMEDIFF Then
		Public Overrides Function args() As SDVariable()
			Return filterNonNull(weights, rWeights, peepholeWeights, bias)
		End Function
#End If

        Public Overrides Function arrayArgs() As INDArray()
            Return filterNonNull(iWeights, irWeights, iPeepholeWeights, iBias)
        End Function

#If SAMEDIFF Then
		Public Overrides Function argsWithInputs(ParamArray ByVal inputs() As SDVariable) As SDVariable()
			Preconditions.checkArgument(inputs.Length = 4, "Expected 4 inputs, got %s", inputs.Length) 'Order: x, seqLen, yLast, cLast
			'lstmLayer c++ op expects: x, Wx, Wr, Wp, b, seqLen, yLast, cLast
			Return ArrayUtil.filterNull(inputs(0), weights, rWeights, bias, inputs(1), inputs(2), inputs(3), peepholeWeights)
		End Function
#End If

        Public Overrides Function argsWithInputs(ParamArray ByVal inputs() As INDArray) As INDArray()
            Preconditions.checkArgument(inputs.Length = 4, "Expected 4 inputs, got %s", inputs.Length) 'Order: x, seqLen, yLast, cLast
            'lstmLayer c++ op expects: x, Wx, Wr, Wp, b, seqLen, yLast, cLast
            Return ArrayUtil.filterNull(inputs(0), iWeights, irWeights, iBias, inputs(1), inputs(2), inputs(3), iPeepholeWeights)
        End Function

        Public Overridable Function hasBias() As Boolean
#If SAMEDIFF Then
            Return (bias IsNot Nothing OrElse iBias IsNot Nothing)
#Else
            Return iBias IsNot Nothing
#End If
        End Function

        Public Overridable Function hasPH() As Boolean
#If SAMEDIFF Then
            Return (peepholeWeights IsNot Nothing OrElse iPeepholeWeights IsNot Nothing)
#Else
            Return iPeepholeWeights IsNot Nothing
#End If
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
        ''' <summary>Lombok getter of <c>rWeights</c>.</summary>
        Public Overridable Function getRWeights() As SDVariable
            Return rWeights
        End Function

        ''' <summary>Lombok setter of <c>rWeights</c>.</summary>
        Public Overridable Sub setRWeights(ByVal value As SDVariable)
            Me.rWeights = value
        End Sub
#End If

        ''' <summary>Lombok getter of <c>irWeights</c>.</summary>
        Public Overridable Function getIrWeights() As INDArray
            Return irWeights
        End Function

        ''' <summary>Lombok setter of <c>irWeights</c>.</summary>
        Public Overridable Sub setIrWeights(ByVal value As INDArray)
            Me.irWeights = value
        End Sub

#If SAMEDIFF Then
        ''' <summary>Lombok getter of <c>peepholeWeights</c>.</summary>
        Public Overridable Function getPeepholeWeights() As SDVariable
            Return peepholeWeights
        End Function

        ''' <summary>Lombok setter of <c>peepholeWeights</c>.</summary>
        Public Overridable Sub setPeepholeWeights(ByVal value As SDVariable)
            Me.peepholeWeights = value
        End Sub
#End If

        ''' <summary>Lombok getter of <c>iPeepholeWeights</c>.</summary>
        Public Overridable Function getIPeepholeWeights() As INDArray
            Return iPeepholeWeights
        End Function

        ''' <summary>Lombok setter of <c>iPeepholeWeights</c>.</summary>
        Public Overridable Sub setIPeepholeWeights(ByVal value As INDArray)
            Me.iPeepholeWeights = value
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
            Dim other As LSTMLayerWeights = TryCast(o, LSTMLayerWeights)
            If other Is Nothing Then
                Return False
            End If
            If Not MyBase.Equals(o) Then
                Return False
            End If
            If Not Object.Equals(iWeights, other.iWeights) Then
                Return False
            End If
            If Not Object.Equals(irWeights, other.irWeights) Then
                Return False
            End If
            If Not Object.Equals(iPeepholeWeights, other.iPeepholeWeights) Then
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
            result = result * 59 + If(CObj(irWeights) Is Nothing, 43, irWeights.GetHashCode())
            result = result * 59 + If(CObj(iPeepholeWeights) Is Nothing, 43, iPeepholeWeights.GetHashCode())
            result = result * 59 + If(CObj(iBias) Is Nothing, 43, iBias.GetHashCode())
            Return result
        End Function

        ''' <summary>Lombok toString over the fields (fields inside a fence are left out).</summary>
        Public Overrides Function ToString() As String
            Return "LSTMLayerWeights(iWeights=" & Arrays.FormatElement(iWeights) & ", irWeights=" & Arrays.FormatElement(irWeights) & ", iPeepholeWeights=" & Arrays.FormatElement(iPeepholeWeights) & ", iBias=" & Arrays.FormatElement(iBias) & ")"
        End Function

        ''' <summary>Lombok's builder.</summary>
        Public Shared Function builder() As LSTMLayerWeightsBuilder
            Return New LSTMLayerWeightsBuilder()
        End Function

        ''' <summary>Lombok's builder of <see cref="LSTMLayerWeights"/>; unset fields keep their defaults.</summary>
        Public Class LSTMLayerWeightsBuilder

#If SAMEDIFF Then
            Private _weights As SDVariable
#End If
            Private _iWeights As INDArray
#If SAMEDIFF Then
            Private _rWeights As SDVariable
#End If
            Private _irWeights As INDArray
#If SAMEDIFF Then
            Private _peepholeWeights As SDVariable
#End If
            Private _iPeepholeWeights As INDArray
#If SAMEDIFF Then
            Private _bias As SDVariable
#End If
            Private _iBias As INDArray

#If SAMEDIFF Then
            Public Function weights(ByVal value As SDVariable) As LSTMLayerWeightsBuilder
                _weights = value
                Return Me
            End Function

#End If
            Public Function iWeights(ByVal value As INDArray) As LSTMLayerWeightsBuilder
                _iWeights = value
                Return Me
            End Function

#If SAMEDIFF Then
            Public Function rWeights(ByVal value As SDVariable) As LSTMLayerWeightsBuilder
                _rWeights = value
                Return Me
            End Function

#End If
            Public Function irWeights(ByVal value As INDArray) As LSTMLayerWeightsBuilder
                _irWeights = value
                Return Me
            End Function

#If SAMEDIFF Then
            Public Function peepholeWeights(ByVal value As SDVariable) As LSTMLayerWeightsBuilder
                _peepholeWeights = value
                Return Me
            End Function

#End If
            Public Function iPeepholeWeights(ByVal value As INDArray) As LSTMLayerWeightsBuilder
                _iPeepholeWeights = value
                Return Me
            End Function

#If SAMEDIFF Then
            Public Function bias(ByVal value As SDVariable) As LSTMLayerWeightsBuilder
                _bias = value
                Return Me
            End Function

#End If
            Public Function iBias(ByVal value As INDArray) As LSTMLayerWeightsBuilder
                _iBias = value
                Return Me
            End Function

            Public Function build() As LSTMLayerWeights
                Dim result As New LSTMLayerWeights()
#If SAMEDIFF Then
                result.weights = _weights
#End If
                result.iWeights = _iWeights
#If SAMEDIFF Then
                result.rWeights = _rWeights
#End If
                result.irWeights = _irWeights
#If SAMEDIFF Then
                result.peepholeWeights = _peepholeWeights
#End If
                result.iPeepholeWeights = _iPeepholeWeights
#If SAMEDIFF Then
                result.bias = _bias
#End If
                result.iBias = _iBias
                Return result
            End Function

        End Class

    End Class

End Namespace

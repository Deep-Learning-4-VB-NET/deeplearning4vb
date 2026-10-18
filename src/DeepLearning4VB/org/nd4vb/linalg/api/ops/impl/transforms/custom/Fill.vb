Imports System
Imports System.Collections.Generic
Imports Preconditions = DeepLearning4VB.org.nd4vb.common.base.Preconditions
Imports DataType = DeepLearning4VB.org.nd4vb.linalg.api.buffer.DataType
Imports INDArray = DeepLearning4VB.org.nd4vb.linalg.api.ndarray.INDArray
Imports DynamicCustomOp = DeepLearning4VB.org.nd4vb.linalg.api.ops.DynamicCustomOp
Imports Op = DeepLearning4VB.org.nd4vb.linalg.api.ops.Op
Imports ND4VBIllegalStateException = DeepLearning4VB.org.nd4vb.linalg.exception.ND4VBIllegalStateException
Imports Nd4vbT = DeepLearning4VB.org.nd4vb.linalg.factory.Nd4vb
#If SAMEDIFF Then
Imports SDVariable = DeepLearning4VB.org.nd4vb.autodiff.samediff.SDVariable
#End If
#If MODEL_IMPORT Then
Imports Onnx = onnx.Onnx
Imports DataTypeAdapter = DeepLearning4VB.org.nd4vb.imports.descriptors.properties.adapters.DataTypeAdapter
Imports AttrValue = org.tensorflow.framework.AttrValue
Imports GraphDef = org.tensorflow.framework.GraphDef
Imports NodeDef = org.tensorflow.framework.NodeDef
#End If
#If SAMEDIFF OrElse MODEL_IMPORT Then
Imports SameDiff = DeepLearning4VB.org.nd4vb.autodiff.samediff.SameDiff
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

Namespace org.nd4vb.linalg.api.ops.impl.transforms.custom

    ''' <summary>
    ''' Fill an array of given "shape" with the provided "value", e.g.
    ''' shape [2, 2] and value 42 returns [[42, 42], [42, 42]].
    ''' 
    ''' @author Max Pumperla
    ''' </summary>
    Public Class Fill
        Inherits DynamicCustomOp

        Private value As Double
        Private dtype As DataType

        Public Sub New()
        End Sub

#If SAMEDIFF Then
		Public Sub New(ByVal sameDiff As SameDiff, ByVal shape As SDVariable, ByVal dtype As DataType, ByVal value As Double)
			MyBase.New(Nothing,sameDiff, New SDVariable() {shape}, False)
			Me.value = value
			Me.dtype = dtype
			addArgs()
		End Sub
#End If

        Public Sub New(ByVal shape As INDArray, ByVal dtype As DataType, ByVal value As Double)
            MyBase.New(New INDArray(){shape, Nd4vbT.scalar(dtype, value)}, Nothing)
            Me.value = value
            Me.dtype = dtype
        End Sub

        Public Sub New(ByVal shape As INDArray, ByVal result As INDArray, ByVal value As Double)
            MyBase.New(Nothing, shape, result, JavaCollections.singletonList(value), Nothing)
            Me.value = value
        End Sub

        Public Sub New(ByVal shape As INDArray, ByVal value As INDArray, ByVal result As INDArray)
            MyBase.New(Nothing, New INDArray(){shape, value}, New INDArray(){result})
        End Sub

        Protected Friend Overridable Sub addArgs()
            addTArgument(value)
        End Sub

#If MODEL_IMPORT Then
		Public Overrides Sub initFromTensorFlow(ByVal nodeDef As NodeDef, ByVal initWith As SameDiff, ByVal attributesForNode As IDictionary(Of String, AttrValue), ByVal graph As GraphDef)
			Dim dt As org.tensorflow.framework.DataType = attributesForNode("T").getType()
			Me.dtype = DataTypeAdapter.dtypeConv(dt)
		End Sub

		Public Overrides Sub initFromOnnx(ByVal node As Onnx.NodeProto, ByVal initWith As SameDiff, ByVal attributesForNode As IDictionary(Of String, Onnx.AttributeProto), ByVal graph As Onnx.GraphProto)
			MyBase.initFromOnnx(node, initWith, attributesForNode, graph)
		End Sub
#End If

        Public Overrides Sub assertValidForExecution()
            Dim descriptor = Me.Descriptor
            If descriptor.getNumInputs() > 0 AndAlso numInputArguments() > 2 OrElse numInputArguments() < 1 Then
                Throw New ND4VBIllegalStateException("Op failure for " & opName() & " Number of inputs is invalid for execution. Specified " & numInputArguments() & " but should be " & descriptor.getNumInputs())
            End If

            If descriptor.getNumOutputs() > 0 AndAlso numOutputArguments() <> descriptor.getNumOutputs() Then
                Throw New ND4VBIllegalStateException("Op failure for " & opName() & " Number of outputs is invalid for execution. Specified " & numOutputArguments() & " but should be " & descriptor.getNumInputs())
            End If

            '< 0 means dynamic size
            If descriptor.getNumIArgs() >= 0 AndAlso numIArguments() <> descriptor.getNumIArgs() Then
                Throw New ND4VBIllegalStateException("Op failure for " & opName() & " Number of integer arguments is invalid for execution. Specified " & numIArguments() & " but should be " & descriptor.getNumIArgs())
            End If

            If descriptor.getNumTArgs() >= 0 AndAlso numTArguments() < 1 Then
                Throw New ND4VBIllegalStateException("Op failure for " & opName() & " Number of inputs is invalid for execution. Specified " & numTArguments() & " but should be " & descriptor.getNumTArgs())
            End If

        End Sub

        Public Overrides Function opName() As String
            Return "fill"
        End Function

        Public Overrides Function onnxName() As String
            Return "ConstantFill"
        End Function

        Public Overrides Function tensorflowName() As String
            Return "Fill"
        End Function

        Public Overrides Function opType() As Op.Type
            Return Op.Type.CUSTOM
        End Function

#If SAMEDIFF Then
		Public Overrides Function doDiff(ByVal gradients As IList(Of SDVariable)) As IList(Of SDVariable)
			Return Collections.singletonList(sameDiff.zerosLike(arg()))
		End Function
#End If

        Public Overrides Function calculateOutputDataTypes(ByVal dataTypes As IList(Of DataType)) As IList(Of DataType)
            If dArguments.Count > 0 Then
                Return New List(Of DataType) From {dArguments(0)}
            End If
            '1 or 2 possible: 2 for TF import (fill with specified value
            Preconditions.checkState(dataTypes IsNot Nothing AndAlso (dataTypes.Count = 1 OrElse dataTypes.Count = 2), "Expected 1 or 2 input datatypes for %s, got %s", Me.GetType(), dataTypes)
            Preconditions.checkNotNull(dtype, "Output datatype was null (not set)")
            Return New List(Of DataType) From {dtype}
        End Function

    End Class

End Namespace

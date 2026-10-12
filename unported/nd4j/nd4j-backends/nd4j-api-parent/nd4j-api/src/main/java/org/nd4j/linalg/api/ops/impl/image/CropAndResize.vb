Imports System
Imports System.Collections.Generic
Imports NoArgsConstructor = lombok.NoArgsConstructor
Imports NonNull = lombok.NonNull
Imports SDVariable = org.nd4j.autodiff.samediff.SDVariable
Imports SameDiff = org.nd4j.autodiff.samediff.SameDiff
Imports Preconditions = org.nd4j.common.base.Preconditions
Imports DataType = org.nd4j.linalg.api.buffer.DataType
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports DynamicCustomOp = org.nd4j.linalg.api.ops.DynamicCustomOp
Imports AttrValue = org.tensorflow.framework.AttrValue
Imports GraphDef = org.tensorflow.framework.GraphDef
Imports NodeDef = org.tensorflow.framework.NodeDef

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

Namespace org.nd4j.linalg.api.ops.impl.image


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @NoArgsConstructor public class CropAndResize extends org.nd4j.linalg.api.ops.DynamicCustomOp
	Public Class CropAndResize
		Inherits DynamicCustomOp

		Public Enum Method
			BILINEAR
			NEAREST
		End Enum
		Protected Friend method As Method = Method.BILINEAR
		Protected Friend extrapolationValue As Double = 0.0

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public CropAndResize(@NonNull SameDiff sameDiff, @NonNull SDVariable image, @NonNull SDVariable cropBoxes, @NonNull SDVariable boxIndices, @NonNull SDVariable cropOutSize, @NonNull Method method, double extrapolationValue)
		Public Sub New(ByVal sameDiff As SameDiff, ByVal image As SDVariable, ByVal cropBoxes As SDVariable, ByVal boxIndices As SDVariable, ByVal cropOutSize As SDVariable, ByVal method As Method, ByVal extrapolationValue As Double)
			MyBase.New(sameDiff, New SDVariable(){image, cropBoxes, boxIndices, cropOutSize})
			Me.method = method
			Me.extrapolationValue = extrapolationValue
			addArgs()
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public CropAndResize(@NonNull SameDiff sameDiff, org.nd4j.autodiff.samediff.SDVariable image, org.nd4j.autodiff.samediff.SDVariable cropBoxes, org.nd4j.autodiff.samediff.SDVariable boxIndices, org.nd4j.autodiff.samediff.SDVariable cropOutSize, double extrapolationValue)
		Public Sub New(ByVal sameDiff As SameDiff, ByVal image As SDVariable, ByVal cropBoxes As SDVariable, ByVal boxIndices As SDVariable, ByVal cropOutSize As SDVariable, ByVal extrapolationValue As Double)
			Me.New(sameDiff, image, cropBoxes, boxIndices, cropOutSize, Nothing, extrapolationValue)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public CropAndResize(@NonNull INDArray image, @NonNull INDArray cropBoxes, @NonNull INDArray boxIndices, @NonNull INDArray cropOutSize, @NonNull Method method, double extrapolationValue, org.nd4j.linalg.api.ndarray.INDArray output)
		Public Sub New(ByVal image As INDArray, ByVal cropBoxes As INDArray, ByVal boxIndices As INDArray, ByVal cropOutSize As INDArray, ByVal method As Method, ByVal extrapolationValue As Double, ByVal output As INDArray)
			MyBase.New(New INDArray(){image, cropBoxes, boxIndices, cropOutSize}, Nothing)
			Preconditions.checkArgument(image.rank() = 4, "Input image must be rank 4 with shape [batch, height, width, channels], got %ndShape", image)
			Preconditions.checkArgument(cropBoxes.rank() = 2 AndAlso cropBoxes.size(1) = 4, "Crop boxes must be rank 4 with shape [num_boxes, 5], got %ndShape", cropBoxes)
			Preconditions.checkArgument(boxIndices.rank() = 1 AndAlso cropBoxes.size(0) = boxIndices.size(0), "Box indices must be rank 1 array with shape [num_boxes] (same as cropBoxes.size(0), got array with shape %ndShape", boxIndices)
			Me.method = method
			Me.extrapolationValue = extrapolationValue
			addArgs()
			outputArguments_Conflict.Add(output)
		End Sub

		Public Sub New(ByVal image As INDArray, ByVal cropBoxes As INDArray, ByVal boxIndices As INDArray, ByVal cropOutSize As INDArray, ByVal extrapolationValue As Double)
			Me.New(image, cropBoxes, boxIndices, cropOutSize, Nothing, extrapolationValue, Nothing)
		End Sub

		Public Overrides Function opName() As String
			Return "crop_and_resize"
		End Function

		Public Overrides Function tensorflowName() As String
			Return "CropAndResize"
		End Function

		Public Overrides Sub initFromTensorFlow(ByVal nodeDef As NodeDef, ByVal initWith As SameDiff, ByVal attributesForNode As IDictionary(Of String, AttrValue), ByVal graph As GraphDef)
			Dim method As String = attributesForNode("method").getS().toStringUtf8()
			If method.Equals("nearest", StringComparison.OrdinalIgnoreCase) Then
				Me.method = Method.NEAREST
			Else
				Me.method = Method.BILINEAR
			End If

			If attributesForNode.ContainsKey("extrapolation_value") Then
				extrapolationValue = attributesForNode("extrapolation_value").getF()
			End If

			addArgs()
		End Sub

		Protected Friend Overridable Sub addArgs()
			addIArgument(If(method = Method.BILINEAR, 0, 1))
			addTArgument(extrapolationValue)
		End Sub

		Public Overrides Function doDiff(ByVal f1 As IList(Of SDVariable)) As IList(Of SDVariable)
			'TODO we can probably skip this sometimes...
			Dim [out] As IList(Of SDVariable) = New List(Of SDVariable)()
			For Each v As SDVariable In args()
				[out].Add(sameDiff.zerosLike(v))
			Next v
			Return [out]
		End Function

		Public Overrides Function calculateOutputDataTypes(ByVal inputDataTypes As IList(Of DataType)) As IList(Of DataType)
			Preconditions.checkState(inputDataTypes IsNot Nothing AndAlso inputDataTypes.Count = 4, "Expected 4 input datatypes for %s, got %s", Me.GetType(), inputDataTypes)
			Return Collections.singletonList(DataType.FLOAT) 'TF import: always returns float32...
		End Function
	End Class

End Namespace
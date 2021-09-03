Imports System
Imports Preconditions = org.nd4j.common.base.Preconditions
Imports DataType = org.nd4j.linalg.api.buffer.DataType
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports CustomOp = org.nd4j.linalg.api.ops.CustomOp
Imports DynamicCustomOp = org.nd4j.linalg.api.ops.DynamicCustomOp
Imports LongShapeDescriptor = org.nd4j.linalg.api.shape.LongShapeDescriptor
Imports DataSet = org.nd4j.linalg.dataset.api.DataSet
Imports DataSetPreProcessor = org.nd4j.linalg.dataset.api.DataSetPreProcessor
Imports Nd4j = org.nd4j.linalg.factory.Nd4j

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

Namespace org.nd4j.linalg.dataset.api.preprocessor

	<Serializable>
	Public Class CropAndResizeDataSetPreProcessor
		Implements DataSetPreProcessor

		Public Enum ResizeMethod
			Bilinear
			NearestNeighbor
		End Enum

		Private ReadOnly resizedShape() As Long
		Private ReadOnly indices As INDArray
		Private ReadOnly resize As INDArray
		Private ReadOnly boxes As INDArray
		Private ReadOnly method As Integer

		''' 
		''' <param name="originalHeight"> Height of the input datasets </param>
		''' <param name="originalWidth"> Width of the input datasets </param>
		''' <param name="cropYStart"> y coord of the starting point on the input datasets </param>
		''' <param name="cropXStart"> x coord of the starting point on the input datasets </param>
		''' <param name="resizedHeight"> Height of the output dataset </param>
		''' <param name="resizedWidth"> Width of the output dataset </param>
		''' <param name="numChannels"> </param>
		''' <param name="resizeMethod"> </param>
		Public Sub New(ByVal originalHeight As Integer, ByVal originalWidth As Integer, ByVal cropYStart As Integer, ByVal cropXStart As Integer, ByVal resizedHeight As Integer, ByVal resizedWidth As Integer, ByVal numChannels As Integer, ByVal resizeMethod As ResizeMethod)
			Preconditions.checkArgument(originalHeight > 0, "originalHeight must be greater than 0, got %s", originalHeight)
			Preconditions.checkArgument(originalWidth > 0, "originalWidth must be greater than 0, got %s", originalWidth)
			Preconditions.checkArgument(cropYStart >= 0, "cropYStart must be greater or equal to 0, got %s", cropYStart)
			Preconditions.checkArgument(cropXStart >= 0, "cropXStart must be greater or equal to 0, got %s", cropXStart)
			Preconditions.checkArgument(resizedHeight > 0, "resizedHeight must be greater than 0, got %s", resizedHeight)
			Preconditions.checkArgument(resizedWidth > 0, "resizedWidth must be greater than 0, got %s", resizedWidth)
			Preconditions.checkArgument(numChannels > 0, "numChannels must be greater than 0, got %s", numChannels)

			resizedShape = New Long() { 1, resizedHeight, resizedWidth, numChannels }

			boxes = Nd4j.create(New Single() { CSng(cropYStart) / CSng(originalHeight), CSng(cropXStart) / CSng(originalWidth), CSng(cropYStart + resizedHeight) / CSng(originalHeight), CSng(cropXStart + resizedWidth) / CSng(originalWidth) }, New Long() { 1, 4 }, DataType.FLOAT)
			indices = Nd4j.create(New Integer() { 0 }, New Long() { 1, 1 }, DataType.INT)

			resize = Nd4j.create(New Integer() { resizedHeight, resizedWidth }, New Long() { 1, 2 }, DataType.INT)
			method = If(resizeMethod = ResizeMethod.Bilinear, 0, 1)
		End Sub

		''' <summary>
		''' NOTE: The data format must be NHWC
		''' </summary>
		Public Overridable Sub preProcess(ByVal dataSet As DataSet)
			Preconditions.checkNotNull(dataSet, "Encountered null dataSet")

			If dataSet.Empty Then
				Return
			End If

			Dim input As INDArray = dataSet.Features
			Dim output As INDArray = Nd4j.create(LongShapeDescriptor.fromShape(resizedShape, input.dataType()), False)

			Dim op As CustomOp = DynamicCustomOp.builder("crop_and_resize").addInputs(input, boxes, indices, resize).addIntegerArguments(method).addOutputs(output).build()
			Nd4j.Executioner.exec(op)

			dataSet.Features = output
		End Sub
	End Class

End Namespace
Imports System
Imports val = lombok.val
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports Shape = org.nd4j.linalg.api.shape.Shape
Imports DataSet = org.nd4j.linalg.dataset.api.DataSet
Imports DataSetPreProcessor = org.nd4j.linalg.dataset.api.DataSetPreProcessor

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
	Public Class ImageFlatteningDataSetPreProcessor
		Implements DataSetPreProcessor

		Public Overridable Sub preProcess(ByVal toPreProcess As DataSet)
			Dim input As INDArray = toPreProcess.Features
			If input.rank() = 2 Then
				Return 'No op: should usually never happen in a properly configured data pipeline
			End If

			'Assume input is standard rank 4 activations - i.e., CNN image data
			'First: we require input to be in c order. But c order (as declared in array order) isn't enough; also need strides to be correct
			If input.ordering() <> "c"c OrElse Not Shape.strideDescendingCAscendingF(input) Then
				input = input.dup("c"c)
			End If

			Dim inShape As val = input.shape() '[miniBatch,depthOut,outH,outW]
			Dim outShape As val = New Long() {inShape(0), inShape(1) * inShape(2) * inShape(3)}

			Dim reshaped As INDArray = input.reshape("c"c, outShape)
			toPreProcess.Features = reshaped
		End Sub
	End Class

End Namespace
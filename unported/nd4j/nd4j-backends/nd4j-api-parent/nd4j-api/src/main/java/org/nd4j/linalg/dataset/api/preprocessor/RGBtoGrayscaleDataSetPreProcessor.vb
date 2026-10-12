Imports System
Imports Preconditions = org.nd4j.common.base.Preconditions
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
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
	Public Class RGBtoGrayscaleDataSetPreProcessor
		Implements DataSetPreProcessor

		Private Const RED_RATIO As Single = 0.3f
		Private Const GREEN_RATIO As Single = 0.59f
		Private Const BLUE_RATIO As Single = 0.11f

		Public Overridable Sub preProcess(ByVal dataSet As DataSet)
			Preconditions.checkNotNull(dataSet, "Encountered null dataSet")

			If dataSet.Empty Then
				Return
			End If

			Dim originalFeatures As INDArray = dataSet.Features
			Dim originalShape() As Long = originalFeatures.shape()

			' result shape is NHW
			Dim result As INDArray = Nd4j.create(originalShape(0), originalShape(2), originalShape(3))

			Dim n As Long = 0
			Dim numExamples As Long = originalShape(0)
			Do While n < numExamples
				' Extract channels
				Dim itemFeatures As INDArray = originalFeatures.slice(n, 0) ' shape is CHW
				Dim R As INDArray = itemFeatures.slice(0, 0) ' shape is HW
				Dim G As INDArray = itemFeatures.slice(1, 0)
				Dim B As INDArray = itemFeatures.slice(2, 0)

				' Convert
				R.muli(RED_RATIO)
				G.muli(GREEN_RATIO)
				B.muli(BLUE_RATIO)
				R.addi(G).addi(B)

				result.putSlice(CInt(n), R)
				n += 1
			Loop

			dataSet.Features = result
		End Sub
	End Class

End Namespace
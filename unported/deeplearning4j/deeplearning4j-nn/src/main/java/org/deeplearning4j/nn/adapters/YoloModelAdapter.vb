Imports System
Imports System.Collections.Generic
Imports AllArgsConstructor = lombok.AllArgsConstructor
Imports Builder = lombok.Builder
Imports NoArgsConstructor = lombok.NoArgsConstructor
Imports val = lombok.val
Imports Model = org.deeplearning4j.nn.api.Model
Imports org.deeplearning4j.nn.api
Imports ComputationGraph = org.deeplearning4j.nn.graph.ComputationGraph
Imports DetectedObject = org.deeplearning4j.nn.layers.objdetect.DetectedObject
Imports Yolo2OutputLayer = org.deeplearning4j.nn.layers.objdetect.Yolo2OutputLayer
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports ND4JIllegalStateException = org.nd4j.linalg.exception.ND4JIllegalStateException

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

Namespace org.deeplearning4j.nn.adapters


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Builder @AllArgsConstructor @NoArgsConstructor public class YoloModelAdapter implements org.deeplearning4j.nn.api.ModelAdapter<java.util.List<org.deeplearning4j.nn.layers.objdetect.DetectedObject>>
	<Serializable>
	Public Class YoloModelAdapter
		Implements ModelAdapter(Of IList(Of DetectedObject))

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Builder.@Default private int outputLayerIndex = 0;
		Private outputLayerIndex As Integer = 0
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Builder.@Default private int outputIndex = 0;
		Private outputIndex As Integer = 0
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Builder.@Default private double detectionThreshold = 0.5;
		Private detectionThreshold As Double = 0.5

		Public Overridable Function apply(ByVal model As Model, ByVal inputs() As INDArray, ByVal masks() As INDArray, ByVal labelsMasks() As INDArray) As IList(Of DetectedObject)
			If TypeOf model Is ComputationGraph Then
				Dim blindLayer As val = DirectCast(model, ComputationGraph).getOutputLayer(outputLayerIndex)
				If TypeOf blindLayer Is Yolo2OutputLayer Then
					Dim output As val = DirectCast(model, ComputationGraph).output(False, inputs, masks, labelsMasks)
					Return CType(blindLayer, Yolo2OutputLayer).getPredictedObjects(output(outputIndex), detectionThreshold)
				Else
					Throw New ND4JIllegalStateException("Output layer with index [" & outputLayerIndex & "] is NOT Yolo2OutputLayer")
				End If
			Else
				Throw New ND4JIllegalStateException("Yolo2 model must be ComputationGraph")
			End If
		End Function

		Public Overridable Function apply(ParamArray ByVal outputs() As INDArray) As IList(Of DetectedObject)
			Throw New System.NotSupportedException("Please use apply(Model, INDArray[], INDArray[]) signature")
		End Function
	End Class

End Namespace
Imports System
Imports System.Collections.Generic
Imports Data = lombok.Data
Imports NoArgsConstructor = lombok.NoArgsConstructor
Imports GraphVertex = org.deeplearning4j.nn.conf.graph.GraphVertex
Imports SDVertexParams = org.deeplearning4j.nn.conf.layers.samediff.SDVertexParams
Imports SameDiffVertex = org.deeplearning4j.nn.conf.layers.samediff.SameDiffVertex
Imports DefaultParamInitializer = org.deeplearning4j.nn.params.DefaultParamInitializer
Imports WeightInit = org.deeplearning4j.nn.weights.WeightInit
Imports WeightInitUtil = org.deeplearning4j.nn.weights.WeightInitUtil
Imports SDVariable = org.nd4j.autodiff.samediff.SDVariable
Imports SameDiff = org.nd4j.autodiff.samediff.SameDiff
Imports Activation = org.nd4j.linalg.activations.Activation
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray

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

Namespace org.deeplearning4j.nn.layers.samediff.testlayers


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @NoArgsConstructor @Data public class SameDiffDenseVertex extends org.deeplearning4j.nn.conf.layers.samediff.SameDiffVertex
	<Serializable>
	Public Class SameDiffDenseVertex
		Inherits SameDiffVertex

		Private nIn As Integer
		Private nOut As Integer
		Private activation As Activation
		Private weightInit As WeightInit

		Public Sub New(ByVal nIn As Integer, ByVal nOut As Integer, ByVal activation As Activation, ByVal weightInit As WeightInit)
			Me.nIn = nIn
			Me.nOut = nOut
			Me.activation = activation
			Me.weightInit = weightInit
		End Sub

		Public Overrides Function defineVertex(ByVal sameDiff As SameDiff, ByVal layerInput As IDictionary(Of String, SDVariable), ByVal paramTable As IDictionary(Of String, SDVariable), ByVal maskVars As IDictionary(Of String, SDVariable)) As SDVariable
			Dim weights As SDVariable = paramTable(DefaultParamInitializer.WEIGHT_KEY)
			Dim bias As SDVariable = paramTable(DefaultParamInitializer.BIAS_KEY)

			Dim mmul As SDVariable = sameDiff.mmul("mmul", layerInput("in"), weights)
			Dim z As SDVariable = mmul.add("z", bias)
			Return activation.asSameDiff("out", sameDiff, z)
		End Function

		Public Overrides Sub defineParametersAndInputs(ByVal params As SDVertexParams)
			params.defineInputs("in")
			params.addWeightParam("W", nIn, nOut)
			params.addBiasParam("b", 1, nOut)
		End Sub

		Public Overrides Sub initializeParameters(ByVal params As IDictionary(Of String, INDArray))
			'Normally use 'c' order, but use 'f' for direct comparison to DL4J DenseLayer
			WeightInitUtil.initWeights(nIn, nOut, New Long(){nIn, nOut}, weightInit, Nothing, "f"c, params("W"))
			params("b").assign(0.0)
		End Sub

		Public Overrides Function paramReshapeOrder(ByVal paramName As String) As Char
			Return "f"c 'To match DL4J DenseLayer - for easy comparison
		End Function

		Public Overrides Function clone() As GraphVertex
			Return New SameDiffDenseVertex(nIn, nOut, activation, weightInit)
		End Function
	End Class

End Namespace
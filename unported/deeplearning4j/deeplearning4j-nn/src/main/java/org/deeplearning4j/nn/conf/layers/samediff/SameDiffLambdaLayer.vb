Imports System
Imports System.Collections.Generic
Imports InputType = org.deeplearning4j.nn.conf.inputs.InputType
Imports SDVariable = org.nd4j.autodiff.samediff.SDVariable
Imports SameDiff = org.nd4j.autodiff.samediff.SameDiff
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

Namespace org.deeplearning4j.nn.conf.layers.samediff


	<Serializable>
	Public MustInherit Class SameDiffLambdaLayer
		Inherits SameDiffLayer

		''' <summary>
		''' The defineLayer method is used to define the forward pass for the layer
		''' </summary>
		''' <param name="sameDiff">   SameDiff instance to use to define the vertex </param>
		''' <param name="layerInput"> Layer input variable </param>
		''' <returns> The output variable (corresponding to the output activations for the layer) </returns>
		Public MustOverride Function defineLayer(ByVal sameDiff As SameDiff, ByVal layerInput As SDVariable) As SDVariable

		Public Overrides Function defineLayer(ByVal sameDiff As SameDiff, ByVal layerInput As SDVariable, ByVal paramTable As IDictionary(Of String, SDVariable), ByVal mask As SDVariable) As SDVariable
			Return defineLayer(sameDiff, layerInput)
		End Function

		Public Overrides Function getOutputType(ByVal layerIndex As Integer, ByVal inputType As InputType) As InputType
			'TODO let's try to infer output shape from input shape, using SameDiff + DefineLayer
			Throw New System.NotSupportedException("Override SameDiffLamdaLayer.getOutputType to use OutputType functionality")
		End Function

		Public Overrides Sub defineParameters(ByVal params As SDLayerParams)
			'No op: lambda layer doesn't have parameters
		End Sub

		Public Overrides Sub initializeParameters(ByVal params As IDictionary(Of String, INDArray))
			'No op: lambda layer doesn't have parameters
		End Sub
	End Class

End Namespace
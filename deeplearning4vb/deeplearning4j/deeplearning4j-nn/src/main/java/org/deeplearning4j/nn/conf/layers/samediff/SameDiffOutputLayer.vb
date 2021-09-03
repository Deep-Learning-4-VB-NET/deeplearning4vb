Imports System
Imports System.Collections.Generic
Imports NeuralNetConfiguration = org.deeplearning4j.nn.conf.NeuralNetConfiguration
Imports TrainingListener = org.deeplearning4j.optimize.api.TrainingListener
Imports SDVariable = org.nd4j.autodiff.samediff.SDVariable
Imports SameDiff = org.nd4j.autodiff.samediff.SameDiff
Imports DataType = org.nd4j.linalg.api.buffer.DataType
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
	Public MustInherit Class SameDiffOutputLayer
		Inherits AbstractSameDiffLayer


		Protected Friend Sub New()
			'No op constructor for Jackson
		End Sub

		''' <summary>
		''' Define the output layer </summary>
		''' <param name="sameDiff">   SameDiff instance </param>
		''' <param name="layerInput"> Input to the layer </param>
		''' <param name="labels">     Labels variable (or null if <seealso cref="labelsRequired()"/> returns false </param>
		''' <param name="paramTable"> Parameter table - keys as defined by <seealso cref="defineParameters(SDLayerParams)"/> </param>
		''' <returns> The final layer variable corresponding to the score/loss during forward pass. This must be a single scalar value. </returns>
		Public MustOverride Function defineLayer(ByVal sameDiff As SameDiff, ByVal layerInput As SDVariable, ByVal labels As SDVariable, ByVal paramTable As IDictionary(Of String, SDVariable)) As SDVariable

		''' <summary>
		''' Output layers should terminate in a single scalar value (i.e., a score) - however, sometimes the output activations
		''' (such as softmax probabilities) need to be returned. When this is the case, we need to know the name of the
		''' SDVariable that corresponds to these.<br>
		''' If the final network activations are just the input to the layer, simply return "input"
		''' </summary>
		''' <returns> The name of the activations to return when performing forward pass </returns>
		Public MustOverride Function activationsVertexName() As String

		''' <summary>
		''' Whether labels are required for calculating the score. Defaults to true - however, if the score
		''' can be calculated without labels (for example, in some output layers used for unsupervised learning)
		''' this can be set to false. </summary>
		''' <returns> True if labels are required to calculate the score/output, false otherwise. </returns>
		Public Overridable Function labelsRequired() As Boolean
			Return True
		End Function

		'==================================================================================================================

		Public Overrides Function instantiate(ByVal conf As NeuralNetConfiguration, ByVal trainingListeners As ICollection(Of TrainingListener), ByVal layerIndex As Integer, ByVal layerParamsView As INDArray, ByVal initializeParams As Boolean, ByVal networkDataType As DataType) As org.deeplearning4j.nn.api.Layer
			Dim ret As New org.deeplearning4j.nn.layers.samediff.SameDiffOutputLayer(conf, networkDataType)
			ret.Index = layerIndex
			ret.ParamsViewArray = layerParamsView
			Dim paramTable As IDictionary(Of String, INDArray) = initializer().init(conf, layerParamsView, initializeParams)
			ret.ParamTable = paramTable
			ret.Conf = conf
			Return ret
		End Function

	End Class

End Namespace
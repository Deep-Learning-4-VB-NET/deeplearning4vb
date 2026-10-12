Imports System
Imports System.Collections.Generic
Imports Data = lombok.Data
Imports EqualsAndHashCode = lombok.EqualsAndHashCode
Imports NoArgsConstructor = lombok.NoArgsConstructor
Imports ToString = lombok.ToString
Imports Layer = org.deeplearning4j.nn.api.Layer
Imports ParamInitializer = org.deeplearning4j.nn.api.ParamInitializer
Imports NeuralNetConfiguration = org.deeplearning4j.nn.conf.NeuralNetConfiguration
Imports DefaultParamInitializer = org.deeplearning4j.nn.params.DefaultParamInitializer
Imports TrainingListener = org.deeplearning4j.optimize.api.TrainingListener
Imports ActivationSoftmax = org.nd4j.linalg.activations.impl.ActivationSoftmax
Imports DataType = org.nd4j.linalg.api.buffer.DataType
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports ILossFunction = org.nd4j.linalg.lossfunctions.ILossFunction
Imports LossFunction = org.nd4j.linalg.lossfunctions.LossFunctions.LossFunction

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

Namespace org.deeplearning4j.nn.conf.layers


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Data @NoArgsConstructor @ToString(callSuper = true) @EqualsAndHashCode(callSuper = true) public class OutputLayer extends BaseOutputLayer
	<Serializable>
	Public Class OutputLayer
		Inherits BaseOutputLayer

		Protected Friend Sub New(ByVal builder As Builder)
			MyBase.New(builder)
			initializeConstraints(builder)
		End Sub

		Public Overrides Function instantiate(ByVal conf As NeuralNetConfiguration, ByVal trainingListeners As ICollection(Of TrainingListener), ByVal layerIndex As Integer, ByVal layerParamsView As INDArray, ByVal initializeParams As Boolean, ByVal networkDataType As DataType) As Layer
			LayerValidation.assertNInNOutSet("OutputLayer", LayerName, layerIndex, getNIn(), getNOut())

			Dim ret As New org.deeplearning4j.nn.layers.OutputLayer(conf, networkDataType)
			ret.setListeners(trainingListeners)
			ret.Index = layerIndex
			ret.ParamsViewArray = layerParamsView
			Dim paramTable As IDictionary(Of String, INDArray) = initializer().init(conf, layerParamsView, initializeParams)
			ret.ParamTable = paramTable
			ret.Conf = conf
			Return ret
		End Function

		Public Overrides Function initializer() As ParamInitializer
			Return DefaultParamInitializer.Instance
		End Function

		Public Class Builder
			Inherits BaseOutputLayer.Builder(Of Builder)

			Public Sub New()
				'Set default activation function to softmax (to match default loss function MCXENT)
				Me.setActivationFn(New ActivationSoftmax())
			End Sub

			''' <param name="lossFunction"> Loss function for the output layer </param>
			Public Sub New(ByVal lossFunction As LossFunction)
				MyBase.lossFunction(lossFunction)
				'Set default activation function to softmax (for consistent behaviour with no-arg constructor)
				Me.setActivationFn(New ActivationSoftmax())
			End Sub

			''' <param name="lossFunction"> Loss function for the output layer </param>
			Public Sub New(ByVal lossFunction As ILossFunction)
				Me.setLossFn(lossFunction)
				'Set default activation function to softmax (for consistent behaviour with no-arg constructor)
				Me.setActivationFn(New ActivationSoftmax())
			End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Override @SuppressWarnings("unchecked") public OutputLayer build()
			Public Overrides Function build() As OutputLayer
				Return New OutputLayer(Me)
			End Function
		End Class
	End Class


End Namespace
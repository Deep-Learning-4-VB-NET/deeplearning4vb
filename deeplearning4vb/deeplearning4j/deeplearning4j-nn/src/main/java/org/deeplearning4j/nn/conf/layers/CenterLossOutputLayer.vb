Imports System
Imports System.Collections.Generic
Imports lombok
Imports Layer = org.deeplearning4j.nn.api.Layer
Imports ParamInitializer = org.deeplearning4j.nn.api.ParamInitializer
Imports NeuralNetConfiguration = org.deeplearning4j.nn.conf.NeuralNetConfiguration
Imports InputType = org.deeplearning4j.nn.conf.inputs.InputType
Imports LayerMemoryReport = org.deeplearning4j.nn.conf.memory.LayerMemoryReport
Imports MemoryReport = org.deeplearning4j.nn.conf.memory.MemoryReport
Imports CenterLossParamInitializer = org.deeplearning4j.nn.params.CenterLossParamInitializer
Imports TrainingListener = org.deeplearning4j.optimize.api.TrainingListener
Imports ActivationSoftmax = org.nd4j.linalg.activations.impl.ActivationSoftmax
Imports DataType = org.nd4j.linalg.api.buffer.DataType
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports IUpdater = org.nd4j.linalg.learning.config.IUpdater
Imports NoOp = org.nd4j.linalg.learning.config.NoOp
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
'ORIGINAL LINE: @Data @NoArgsConstructor @ToString(callSuper = true) @EqualsAndHashCode(callSuper = true) public class CenterLossOutputLayer extends BaseOutputLayer
	<Serializable>
	Public Class CenterLossOutputLayer
		Inherits BaseOutputLayer

'JAVA TO VB CONVERTER NOTE: The field alpha was renamed since Visual Basic does not allow fields to have the same name as other class members:
		Protected Friend alpha_Conflict As Double
'JAVA TO VB CONVERTER NOTE: The field lambda was renamed since Visual Basic does not allow fields to have the same name as other class members:
		Protected Friend lambda_Conflict As Double
'JAVA TO VB CONVERTER NOTE: The field gradientCheck was renamed since Visual Basic does not allow fields to have the same name as other class members:
		Protected Friend gradientCheck_Conflict As Boolean

		Protected Friend Sub New(ByVal builder As Builder)
			MyBase.New(builder)
			Me.alpha_Conflict = builder.alpha_Conflict
			Me.lambda_Conflict = builder.lambda_Conflict
			Me.gradientCheck_Conflict = builder.gradientCheck_Conflict
			initializeConstraints(builder)
		End Sub

		Public Overrides Function instantiate(ByVal conf As NeuralNetConfiguration, ByVal trainingListeners As ICollection(Of TrainingListener), ByVal layerIndex As Integer, ByVal layerParamsView As INDArray, ByVal initializeParams As Boolean, ByVal networkDataType As DataType) As Layer
			LayerValidation.assertNInNOutSet("CenterLossOutputLayer", getLayerName(), layerIndex, getNIn(), getNOut())

			Dim ret As Layer = New org.deeplearning4j.nn.layers.training.CenterLossOutputLayer(conf, networkDataType)
			ret.setListeners(trainingListeners)
			ret.Index = layerIndex
			ret.ParamsViewArray = layerParamsView
			Dim paramTable As IDictionary(Of String, INDArray) = initializer().init(conf, layerParamsView, initializeParams)
			ret.ParamTable = paramTable
			ret.Conf = conf
			Return ret
		End Function

		Public Overrides Function initializer() As ParamInitializer
			Return CenterLossParamInitializer.Instance
		End Function

		Public Overrides Function getUpdaterByParam(ByVal paramName As String) As IUpdater
			' center loss utilizes alpha directly for this so any updater can be used for other layers
			Select Case paramName
				Case CenterLossParamInitializer.CENTER_KEY
					Return New NoOp()
				Case Else
					Return iUpdater
			End Select
		End Function

		Public Overridable ReadOnly Property Alpha As Double
			Get
				Return alpha_Conflict
			End Get
		End Property

		Public Overridable ReadOnly Property Lambda As Double
			Get
				Return lambda_Conflict
			End Get
		End Property

		Public Overridable ReadOnly Property GradientCheck As Boolean
			Get
				Return gradientCheck_Conflict
			End Get
		End Property

		Public Overrides Function getMemoryReport(ByVal inputType As InputType) As LayerMemoryReport
			'Basically a dense layer, with some extra params...
			Dim outputType As InputType = getOutputType(-1, inputType)

			Dim nParamsW As val = nIn * nOut
			Dim nParamsB As val = nOut
			Dim nParamsCenter As val = nIn * nOut
			Dim numParams As val = nParamsW + nParamsB + nParamsCenter

			Dim updaterStateSize As Integer = CInt(getUpdaterByParam(CenterLossParamInitializer.WEIGHT_KEY).stateSize(nParamsW) + getUpdaterByParam(CenterLossParamInitializer.BIAS_KEY).stateSize(nParamsB) + getUpdaterByParam(CenterLossParamInitializer.CENTER_KEY).stateSize(nParamsCenter))

			Dim trainSizeFixed As Integer = 0
			Dim trainSizeVariable As Integer = 0
			If getIDropout() IsNot Nothing Then
				If False Then
					'TODO drop connect
					'Dup the weights... note that this does NOT depend on the minibatch size...
					trainSizeVariable += 0 'TODO
				Else
					'Assume we dup the input
					trainSizeVariable += inputType.arrayElementsPerExample()
				End If
			End If

			'Also, during backprop: we do a preOut call -> gives us activations size equal to the output size
			' which is modified in-place by activation function backprop
			' then we have 'epsilonNext' which is equivalent to input size
			trainSizeVariable += outputType.arrayElementsPerExample()

			Return (New LayerMemoryReport.Builder(layerName, GetType(CenterLossOutputLayer), inputType, outputType)).standardMemory(numParams, updaterStateSize).workingMemory(0, 0, trainSizeFixed, trainSizeVariable).cacheMemory(MemoryReport.CACHE_MODE_ALL_ZEROS, MemoryReport.CACHE_MODE_ALL_ZEROS).build()
		End Function

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Getter @Setter public static class Builder extends BaseOutputLayer.Builder<Builder>
		Public Class Builder
			Inherits BaseOutputLayer.Builder(Of Builder)

'JAVA TO VB CONVERTER NOTE: The field alpha was renamed since Visual Basic does not allow fields to have the same name as other class members:
			Protected Friend alpha_Conflict As Double = 0.05
'JAVA TO VB CONVERTER NOTE: The field lambda was renamed since Visual Basic does not allow fields to have the same name as other class members:
			Protected Friend lambda_Conflict As Double = 2e-4
'JAVA TO VB CONVERTER NOTE: The field gradientCheck was renamed since Visual Basic does not allow fields to have the same name as other class members:
			Protected Friend gradientCheck_Conflict As Boolean = False

			Public Sub New()
				Me.setActivationFn(New ActivationSoftmax())
			End Sub

			Public Sub New(ByVal lossFunction As LossFunction)
				MyBase.lossFunction(lossFunction)
			End Sub

			Public Sub New(ByVal lossFunction As ILossFunction)
				Me.setLossFn(lossFunction)
			End Sub

'JAVA TO VB CONVERTER NOTE: The parameter alpha was renamed since Visual Basic will not allow parameters with the same name as their enclosing function or property:
			Public Overridable Function alpha(ByVal alpha_Conflict As Double) As Builder
				Me.setAlpha(alpha_Conflict)
				Return Me
			End Function

'JAVA TO VB CONVERTER NOTE: The parameter lambda was renamed since Visual Basic will not allow parameters with the same name as their enclosing function or property:
			Public Overridable Function lambda(ByVal lambda_Conflict As Double) As Builder
				Me.setLambda(lambda_Conflict)
				Return Me
			End Function

			Public Overridable Function gradientCheck(ByVal isGradientCheck As Boolean) As Builder
				Me.setGradientCheck(isGradientCheck)
				Return Me
			End Function

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Override @SuppressWarnings("unchecked") public CenterLossOutputLayer build()
			Public Overrides Function build() As CenterLossOutputLayer
				Return New CenterLossOutputLayer(Me)
			End Function
		End Class
	End Class


End Namespace
Imports System
Imports System.Collections.Generic
Imports ParamInitializer = org.deeplearning4j.nn.api.ParamInitializer
Imports GradientNormalization = org.deeplearning4j.nn.conf.GradientNormalization
Imports InputPreProcessor = org.deeplearning4j.nn.conf.InputPreProcessor
Imports NeuralNetConfiguration = org.deeplearning4j.nn.conf.NeuralNetConfiguration
Imports RNNFormat = org.deeplearning4j.nn.conf.RNNFormat
Imports InputType = org.deeplearning4j.nn.conf.inputs.InputType
Imports Layer = org.deeplearning4j.nn.conf.layers.Layer
Imports LayerMemoryReport = org.deeplearning4j.nn.conf.memory.LayerMemoryReport
Imports TFOpLayerImpl = org.deeplearning4j.nn.modelimport.keras.layers.TFOpLayerImpl
Imports EmptyParamInitializer = org.deeplearning4j.nn.params.EmptyParamInitializer
Imports TrainingListener = org.deeplearning4j.optimize.api.TrainingListener
Imports DataType = org.nd4j.linalg.api.buffer.DataType
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports Nd4j = org.nd4j.linalg.factory.Nd4j
Imports Regularization = org.nd4j.linalg.learning.regularization.Regularization

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

Namespace org.deeplearning4j.nn.modelimport.keras.layers



	<Serializable>
	Public Class TFOpLayer
		Inherits Layer

		Private nodeDef As System.Collections.IDictionary
		Private constants As System.Collections.IDictionary
		Public Sub New(ByVal nodeDef As System.Collections.IDictionary, ByVal constants As System.Collections.IDictionary)
			MyBase.New()
			Me.nodeDef = nodeDef
			Me.constants = constants
		End Sub

		Public Overrides Function initializer() As ParamInitializer
			Return EmptyParamInitializer.Instance
		End Function
		Public Overrides Function getPreProcessorForInputType(ByVal inputType As InputType) As InputPreProcessor
			Return Nothing
		End Function

		Public Overrides Function isPretrainParam(ByVal param As String) As Boolean
			Return False
		End Function

		Public Overrides Function getOutputType(ByVal idx As Integer, ByVal inputType As InputType) As InputType
			Dim shape() As Long = inputType.getShape(True)
			Dim tempLayer As New TFOpLayerImpl(nodeDef, constants, Nothing, Nothing)
			Dim outputShape() As Long = tempLayer.getOutputShape(shape)
			If outputShape.Length = 3 Then
				Return InputType.recurrent(outputShape(2), outputShape(1), RNNFormat.NWC)
			End If
			Return InputType.inferInputType(Nd4j.create(outputShape))

		End Function

		Public Overrides Sub setNIn(ByVal inputType As InputType, ByVal override As Boolean)
		End Sub
		Public Overrides ReadOnly Property GradientNormalization As GradientNormalization
			Get
				Return Nothing
			End Get
		End Property
		Public Overrides Function instantiate(ByVal conf As NeuralNetConfiguration, ByVal trainingListeners As ICollection(Of TrainingListener), ByVal layerIndex As Integer, ByVal layerParamsView As INDArray, ByVal initializeParams As Boolean, ByVal networkDataType As DataType) As org.deeplearning4j.nn.api.Layer

			Dim tfOpLayerImpl As New TFOpLayerImpl(nodeDef, constants, conf, networkDataType)
			tfOpLayerImpl.setListeners(trainingListeners)
			tfOpLayerImpl.Index = layerIndex
			Return tfOpLayerImpl
		End Function

		Public Overrides ReadOnly Property GradientNormalizationThreshold As Double
			Get
				Return 0.0
			End Get
		End Property
		Public Overrides Function getRegularizationByParam(ByVal paramName As String) As IList(Of Regularization)
			Return Nothing
		End Function
		Public Overrides Function getMemoryReport(ByVal inputType As InputType) As LayerMemoryReport
			Return New LayerMemoryReport() 'TODO
		End Function





	End Class

End Namespace
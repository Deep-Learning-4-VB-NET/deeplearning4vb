Imports System
Imports System.Collections.Generic
Imports Data = lombok.Data
Imports Getter = lombok.Getter
Imports Setter = lombok.Setter
Imports Layer = org.deeplearning4j.nn.api.Layer
Imports ParamInitializer = org.deeplearning4j.nn.api.ParamInitializer
Imports CNN2DFormat = org.deeplearning4j.nn.conf.CNN2DFormat
Imports GradientNormalization = org.deeplearning4j.nn.conf.GradientNormalization
Imports InputPreProcessor = org.deeplearning4j.nn.conf.InputPreProcessor
Imports NeuralNetConfiguration = org.deeplearning4j.nn.conf.NeuralNetConfiguration
Imports InputType = org.deeplearning4j.nn.conf.inputs.InputType
Imports LayerMemoryReport = org.deeplearning4j.nn.conf.memory.LayerMemoryReport
Imports FeedForwardToCnnPreProcessor = org.deeplearning4j.nn.conf.preprocessor.FeedForwardToCnnPreProcessor
Imports EmptyParamInitializer = org.deeplearning4j.nn.params.EmptyParamInitializer
Imports TrainingListener = org.deeplearning4j.optimize.api.TrainingListener
Imports DataType = org.nd4j.linalg.api.buffer.DataType
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports Regularization = org.nd4j.linalg.learning.regularization.Regularization
Imports ILossFunction = org.nd4j.linalg.lossfunctions.ILossFunction
Imports LossL2 = org.nd4j.linalg.lossfunctions.impl.LossL2
Imports NDArrayTextSerializer = org.nd4j.serde.jackson.shaded.NDArrayTextSerializer
Imports JsonDeserialize = org.nd4j.shade.jackson.databind.annotation.JsonDeserialize
Imports JsonSerialize = org.nd4j.shade.jackson.databind.annotation.JsonSerialize

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

Namespace org.deeplearning4j.nn.conf.layers.objdetect


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Data public class Yolo2OutputLayer extends org.deeplearning4j.nn.conf.layers.Layer
	<Serializable>
	Public Class Yolo2OutputLayer
		Inherits org.deeplearning4j.nn.conf.layers.Layer

		Private lambdaCoord As Double
		Private lambdaNoObj As Double
		Private lossPositionScale As ILossFunction
		Private lossClassPredictions As ILossFunction
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @JsonSerialize(using = org.nd4j.serde.jackson.shaded.NDArrayTextSerializer.class) @JsonDeserialize(using = BoundingBoxesDeserializer.class) private org.nd4j.linalg.api.ndarray.INDArray boundingBoxes;
		Private boundingBoxes As INDArray

		Private format As CNN2DFormat = CNN2DFormat.NCHW 'Default for serialization of old formats

		Private Sub New()
			'No-arg constructor for Jackson JSON
		End Sub

		Private Sub New(ByVal builder As Builder)
			MyBase.New(builder)
			Me.lambdaCoord = builder.lambdaCoord_Conflict
			Me.lambdaNoObj = builder.lambdaNoObj_Conflict
			Me.lossPositionScale = builder.lossPositionScale_Conflict
			Me.lossClassPredictions = builder.lossClassPredictions_Conflict
			Me.boundingBoxes = builder.boundingBoxes
		End Sub

		Public Overrides Function instantiate(ByVal conf As NeuralNetConfiguration, ByVal trainingListeners As ICollection(Of TrainingListener), ByVal layerIndex As Integer, ByVal layerParamsView As INDArray, ByVal initializeParams As Boolean, ByVal networkDataType As DataType) As Layer
			Dim ret As New org.deeplearning4j.nn.layers.objdetect.Yolo2OutputLayer(conf, networkDataType)
			ret.setListeners(trainingListeners)
			ret.Index = layerIndex
			ret.ParamsViewArray = layerParamsView
			Dim paramTable As IDictionary(Of String, INDArray) = initializer().init(conf, layerParamsView, initializeParams)
			ret.ParamTable = paramTable
			ret.Conf = conf
			Return ret
		End Function

		Public Overrides Function initializer() As ParamInitializer
			Return EmptyParamInitializer.Instance
		End Function

		Public Overrides Function getOutputType(ByVal layerIndex As Integer, ByVal inputType As InputType) As InputType
			Return inputType 'Same shape output as input
		End Function

		Public Overrides Sub setNIn(ByVal inputType As InputType, ByVal override As Boolean)
			Dim c As InputType.InputTypeConvolutional = DirectCast(inputType, InputType.InputTypeConvolutional)
			Me.format = c.getFormat()
		End Sub

		Public Overrides Function getPreProcessorForInputType(ByVal inputType As InputType) As InputPreProcessor
			Select Case inputType.getType()
				Case FF, RNN
					Throw New System.NotSupportedException("Cannot use FF or RNN input types")
				Case CNN
					Return Nothing
				Case CNNFlat
					Dim cf As InputType.InputTypeConvolutionalFlat = DirectCast(inputType, InputType.InputTypeConvolutionalFlat)
					Return New FeedForwardToCnnPreProcessor(cf.getHeight(), cf.getWidth(), cf.getDepth())
				Case Else
					Return Nothing
			End Select
		End Function

		Public Overrides Function getRegularizationByParam(ByVal paramName As String) As IList(Of Regularization)
			'Not applicable
			Return Nothing
		End Function

		Public Overrides Function isPretrainParam(ByVal paramName As String) As Boolean
			Return False 'No params
		End Function

		Public Overrides ReadOnly Property GradientNormalization As GradientNormalization
			Get
				Return GradientNormalization.None
			End Get
		End Property

		Public Overrides ReadOnly Property GradientNormalizationThreshold As Double
			Get
				Return 1.0
			End Get
		End Property

		Public Overrides Function getMemoryReport(ByVal inputType As InputType) As LayerMemoryReport
			Dim numValues As Long = inputType.arrayElementsPerExample()

			'This is a VERY rough estimate...
			Return (New LayerMemoryReport.Builder(layerName, GetType(Yolo2OutputLayer), inputType, inputType)).standardMemory(0, 0).workingMemory(0, numValues, 0, 6 * numValues).cacheMemory(0, 0).build()
		End Function

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Getter @Setter public static class Builder extends org.deeplearning4j.nn.conf.layers.Layer.Builder<Builder>
		Public Class Builder
			Inherits org.deeplearning4j.nn.conf.layers.Layer.Builder(Of Builder)

			''' <summary>
			''' Loss function coefficient for position and size/scale components of the loss function. Default (as per
			''' paper): 5
			''' 
			''' </summary>
'JAVA TO VB CONVERTER NOTE: The field lambdaCoord was renamed since Visual Basic does not allow fields to have the same name as other class members:
			Friend lambdaCoord_Conflict As Double = 5

			''' <summary>
			''' Loss function coefficient for the "no object confidence" components of the loss function. Default (as per
			''' paper): 0.5
			''' 
			''' </summary>
'JAVA TO VB CONVERTER NOTE: The field lambdaNoObj was renamed since Visual Basic does not allow fields to have the same name as other class members:
			Friend lambdaNoObj_Conflict As Double = 0.5

			''' <summary>
			''' Loss function for position/scale component of the loss function
			''' 
			''' </summary>
'JAVA TO VB CONVERTER NOTE: The field lossPositionScale was renamed since Visual Basic does not allow fields to have the same name as other class members:
			Friend lossPositionScale_Conflict As ILossFunction = New LossL2()

			''' <summary>
			''' Loss function for the class predictions - defaults to L2 loss (i.e., sum of squared errors, as per the
			''' paper), however Loss MCXENT could also be used (which is more common for classification).
			''' 
			''' </summary>
'JAVA TO VB CONVERTER NOTE: The field lossClassPredictions was renamed since Visual Basic does not allow fields to have the same name as other class members:
			Friend lossClassPredictions_Conflict As ILossFunction = New LossL2()

			''' <summary>
			''' Bounding box priors dimensions [width, height]. For N bounding boxes, input has shape [rows, columns] = [N,
			''' 2] Note that dimensions should be specified as fraction of grid size. For example, a network with 13x13
			''' output, a value of 1.0 would correspond to one grid cell; a value of 13 would correspond to the entire
			''' image.
			''' 
			''' </summary>
			Friend boundingBoxes As INDArray

			''' <summary>
			''' Loss function coefficient for position and size/scale components of the loss function. Default (as per
			''' paper): 5
			''' </summary>
			''' <param name="lambdaCoord"> Lambda value for size/scale component of loss function </param>
'JAVA TO VB CONVERTER NOTE: The parameter lambdaCoord was renamed since Visual Basic will not allow parameters with the same name as their enclosing function or property:
			Public Overridable Function lambdaCoord(ByVal lambdaCoord_Conflict As Double) As Builder
				Me.setLambdaCoord(lambdaCoord_Conflict)
				Return Me
			End Function

			''' <summary>
			''' Loss function coefficient for the "no object confidence" components of the loss function. Default (as per
			''' paper): 0.5
			''' </summary>
			''' <param name="lambdaNoObj"> Lambda value for no-object (confidence) component of the loss function </param>
'JAVA TO VB CONVERTER NOTE: The parameter lambdaNoObj was renamed since Visual Basic will not allow parameters with the same name as their enclosing function or property:
			Public Overridable Function lambdaNoObj(ByVal lambdaNoObj_Conflict As Double) As Builder
				Me.setLambdaNoObj(lambdaNoObj_Conflict)
				Return Me
			End Function

			''' <summary>
			''' Loss function for position/scale component of the loss function
			''' </summary>
			''' <param name="lossPositionScale"> Loss function for position/scale </param>
'JAVA TO VB CONVERTER NOTE: The parameter lossPositionScale was renamed since Visual Basic will not allow parameters with the same name as their enclosing function or property:
			Public Overridable Function lossPositionScale(ByVal lossPositionScale_Conflict As ILossFunction) As Builder
				Me.setLossPositionScale(lossPositionScale_Conflict)
				Return Me
			End Function

			''' <summary>
			''' Loss function for the class predictions - defaults to L2 loss (i.e., sum of squared errors, as per the
			''' paper), however Loss MCXENT could also be used (which is more common for classification).
			''' </summary>
			''' <param name="lossClassPredictions"> Loss function for the class prediction error component of the YOLO loss function </param>
'JAVA TO VB CONVERTER NOTE: The parameter lossClassPredictions was renamed since Visual Basic will not allow parameters with the same name as their enclosing function or property:
			Public Overridable Function lossClassPredictions(ByVal lossClassPredictions_Conflict As ILossFunction) As Builder
				Me.setLossClassPredictions(lossClassPredictions_Conflict)
				Return Me
			End Function

			''' <summary>
			''' Bounding box priors dimensions [width, height]. For N bounding boxes, input has shape [rows, columns] = [N,
			''' 2] Note that dimensions should be specified as fraction of grid size. For example, a network with 13x13
			''' output, a value of 1.0 would correspond to one grid cell; a value of 13 would correspond to the entire
			''' image.
			''' </summary>
			''' <param name="boundingBoxes"> Bounding box prior dimensions (width, height) </param>
			Public Overridable Function boundingBoxPriors(ByVal boundingBoxes As INDArray) As Builder
				Me.setBoundingBoxes(boundingBoxes)
				Return Me
			End Function

			Public Overrides Function build() As Yolo2OutputLayer
				If boundingBoxes Is Nothing Then
					Throw New System.InvalidOperationException("Bounding boxes have not been set")
				End If

				If boundingBoxes.rank() <> 2 OrElse boundingBoxes.size(1) <> 2 Then
					Throw New System.InvalidOperationException("Bounding box priors must have shape [nBoxes, 2]. Has shape: " & Arrays.toString(boundingBoxes.shape()))
				End If

				Return New Yolo2OutputLayer(Me)
			End Function
		End Class
	End Class

End Namespace
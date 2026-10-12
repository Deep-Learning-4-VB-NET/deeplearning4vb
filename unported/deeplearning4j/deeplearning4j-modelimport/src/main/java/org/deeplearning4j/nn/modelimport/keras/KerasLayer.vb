Imports System
Imports System.Collections.Generic
Imports Microsoft.VisualBasic
Imports System.Linq
Imports Getter = lombok.Getter
Imports Slf4j = lombok.extern.slf4j.Slf4j
Imports StringUtils = org.apache.commons.lang3.StringUtils
Imports InputPreProcessor = org.deeplearning4j.nn.conf.InputPreProcessor
Imports GraphVertex = org.deeplearning4j.nn.conf.graph.GraphVertex
Imports InputType = org.deeplearning4j.nn.conf.inputs.InputType
Imports FeedForwardLayer = org.deeplearning4j.nn.conf.layers.FeedForwardLayer
Imports Layer = org.deeplearning4j.nn.conf.layers.Layer
Imports SameDiffLambdaLayer = org.deeplearning4j.nn.conf.layers.samediff.SameDiffLambdaLayer
Imports KerasLayerConfiguration = org.deeplearning4j.nn.modelimport.keras.config.KerasLayerConfiguration
Imports KerasLayerConfigurationFactory = org.deeplearning4j.nn.modelimport.keras.config.KerasLayerConfigurationFactory
Imports InvalidKerasConfigurationException = org.deeplearning4j.nn.modelimport.keras.exceptions.InvalidKerasConfigurationException
Imports UnsupportedKerasConfigurationException = org.deeplearning4j.nn.modelimport.keras.exceptions.UnsupportedKerasConfigurationException
Imports KerasConvolutionUtils = org.deeplearning4j.nn.modelimport.keras.layers.convolutional.KerasConvolutionUtils
Imports KerasLayerUtils = org.deeplearning4j.nn.modelimport.keras.utils.KerasLayerUtils
Imports KerasRegularizerUtils = org.deeplearning4j.nn.modelimport.keras.utils.KerasRegularizerUtils
Imports ArrayUtil = org.nd4j.common.util.ArrayUtil
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

Namespace org.deeplearning4j.nn.modelimport.keras


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Slf4j public class KerasLayer
	Public Class KerasLayer

		Private Const LAYER_FIELD_KERAS_VERSION As String = "keras_version"
		Friend Shared ReadOnly customLayers As IDictionary(Of String, Type) = New Dictionary(Of String, Type)()
		Friend Shared ReadOnly lambdaLayers As IDictionary(Of String, SameDiffLambdaLayer) = New Dictionary(Of String, SameDiffLambdaLayer)()


		Public Enum DimOrder
			NONE
			THEANO
			TENSORFLOW
		End Enum
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Getter protected String className;
'JAVA TO VB CONVERTER NOTE: The field className was renamed since Visual Basic does not allow fields to have the same name as other class members:
		Protected Friend className_Conflict As String ' Keras layer class name
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Getter protected String layerName;
'JAVA TO VB CONVERTER NOTE: The field layerName was renamed since Visual Basic does not allow fields to have the same name as other class members:
		Protected Friend layerName_Conflict As String ' Keras layer name
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Getter protected int[] inputShape;
'JAVA TO VB CONVERTER NOTE: The field inputShape was renamed since Visual Basic does not allow fields to have the same name as other class members:
		Protected Friend inputShape_Conflict() As Integer ' Keras layer input shape
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Getter protected DimOrder dimOrder;
'JAVA TO VB CONVERTER NOTE: The field dimOrder was renamed since Visual Basic does not allow fields to have the same name as other class members:
		Protected Friend dimOrder_Conflict As DimOrder ' Keras layer backend dimension order
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Getter protected List<String> inboundLayerNames;
'JAVA TO VB CONVERTER NOTE: The field inboundLayerNames was renamed since Visual Basic does not allow fields to have the same name as other class members:
		Protected Friend inboundLayerNames_Conflict As IList(Of String) ' List of inbound layers
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Getter protected List<String> outboundLayerNames;
		Protected Friend outboundLayerNames As IList(Of String) 'List of outbound layers
'JAVA TO VB CONVERTER NOTE: The field layer was renamed since Visual Basic does not allow fields to have the same name as other class members:
		Protected Friend layer_Conflict As Layer ' Resulting DL4J layer
'JAVA TO VB CONVERTER NOTE: The field vertex was renamed since Visual Basic does not allow fields to have the same name as other class members:
		Protected Friend vertex_Conflict As GraphVertex ' Resulting DL4J vertex
'JAVA TO VB CONVERTER NOTE: The field weights was renamed since Visual Basic does not allow fields to have the same name as other class members:
		Protected Friend weights_Conflict As IDictionary(Of String, INDArray) ' Weights
		Protected Friend weightL1Regularization As Double = 0.0 ' L1 regularization
		Protected Friend weightL2Regularization As Double = 0.0 ' L2 regularization
		Protected Friend dropout As Double = 1.0 ' Dropout
		Protected Friend kerasMajorVersion As Integer? = 2 ' Set 2 as default for now
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Getter protected org.deeplearning4j.nn.modelimport.keras.config.KerasLayerConfiguration conf;
		Protected Friend conf As KerasLayerConfiguration
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Getter protected Map<String, Object> originalLayerConfig;
		Protected Friend originalLayerConfig As IDictionary(Of String, Object)

		''' <summary>
		''' Constructor with Keras version only.
		''' </summary>
		''' <param name="kerasVersion"> major Keras version (1 or 2) </param>
		''' <exception cref="UnsupportedKerasConfigurationException"> Unsupported Keras configuration </exception>
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: protected KerasLayer(System.Nullable<Integer> kerasVersion) throws org.deeplearning4j.nn.modelimport.keras.exceptions.UnsupportedKerasConfigurationException
		Protected Friend Sub New(ByVal kerasVersion As Integer?)
			Me.className_Conflict = Nothing
			Me.layerName_Conflict = Nothing
			Me.inputShape_Conflict = Nothing
			Me.dimOrder_Conflict = DimOrder.NONE
			Me.inboundLayerNames_Conflict = New List(Of String)()
			Me.outboundLayerNames = New List(Of String)()
			Me.layer_Conflict = Nothing
			Me.vertex_Conflict = Nothing
			Me.weights_Conflict = Nothing
			Me.kerasMajorVersion = kerasVersion
			Me.conf = KerasLayerConfigurationFactory.get(Me.kerasMajorVersion)
		End Sub

		''' <summary>
		''' Default constructor.
		''' </summary>
		''' <exception cref="UnsupportedKerasConfigurationException"> Unsupported Keras configuration </exception>
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: protected KerasLayer() throws org.deeplearning4j.nn.modelimport.keras.exceptions.UnsupportedKerasConfigurationException
		Protected Friend Sub New()
			Me.className_Conflict = Nothing
			Me.layerName_Conflict = Nothing
			Me.inputShape_Conflict = Nothing
			Me.dimOrder_Conflict = DimOrder.NONE
			Me.inboundLayerNames_Conflict = New List(Of String)()
			Me.outboundLayerNames = New List(Of String)()
			Me.layer_Conflict = Nothing
			Me.vertex_Conflict = Nothing
			Me.weights_Conflict = Nothing
			Me.conf = KerasLayerConfigurationFactory.get(Me.kerasMajorVersion)

		End Sub

		''' <summary>
		''' Constructor.
		''' </summary>
		''' <param name="layerConfig"> dictionary containing Keras layer configuration </param>
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: protected KerasLayer(Map<String, Object> layerConfig) throws InvalidKerasConfigurationException, org.deeplearning4j.nn.modelimport.keras.exceptions.UnsupportedKerasConfigurationException
		Protected Friend Sub New(ByVal layerConfig As IDictionary(Of String, Object))
			Me.New(layerConfig, True)
		End Sub

		''' <summary>
		''' Constructor. "enforceTrainingConfig" parameter controls whether layer is built for
		''' training. This controls behavior of certain exceptions. In training mode, passing
		''' an unsupported regularizer will generate an error. In non-training mode, it
		''' generates only a warning.
		''' </summary>
		''' <param name="layerConfig">           dictionary containing Keras layer configuration </param>
		''' <param name="enforceTrainingConfig"> whether layer should be built for training (controls certain exceptions) </param>
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: protected KerasLayer(Map<String, Object> layerConfig, boolean enforceTrainingConfig) throws InvalidKerasConfigurationException, org.deeplearning4j.nn.modelimport.keras.exceptions.UnsupportedKerasConfigurationException
		Protected Friend Sub New(ByVal layerConfig As IDictionary(Of String, Object), ByVal enforceTrainingConfig As Boolean)
			Me.originalLayerConfig = layerConfig
			Me.kerasMajorVersion = DirectCast(layerConfig(LAYER_FIELD_KERAS_VERSION), Integer?)
			Me.conf = KerasLayerConfigurationFactory.get(Me.kerasMajorVersion)
			Me.className_Conflict = KerasLayerUtils.getClassNameFromConfig(layerConfig, conf)
			If Me.className_Conflict Is Nothing Then
				Throw New InvalidKerasConfigurationException("Keras layer class name is missing")
			End If
			Me.layerName_Conflict = KerasLayerUtils.getLayerNameFromConfig(layerConfig, conf)
			If Me.layerName_Conflict Is Nothing Then
				Throw New InvalidKerasConfigurationException("Keras layer class name is missing")
			End If
			Me.inputShape_Conflict = KerasLayerUtils.getInputShapeFromConfig(layerConfig, conf)
			Me.dimOrder_Conflict = KerasLayerUtils.getDimOrderFromConfig(layerConfig, conf)
			Me.inboundLayerNames_Conflict = KerasLayerUtils.getInboundLayerNamesFromConfig(layerConfig, conf)
			Me.outboundLayerNames = KerasLayerUtils.getOutboundLayerNamesFromConfig(layerConfig,conf)
			Me.layer_Conflict = Nothing
			Me.vertex_Conflict = Nothing
			Me.weights_Conflict = Nothing

			Me.weightL1Regularization = KerasRegularizerUtils.getWeightRegularizerFromConfig(layerConfig, conf, conf.getLAYER_FIELD_W_REGULARIZER(), conf.getREGULARIZATION_TYPE_L1())
			Me.weightL2Regularization = KerasRegularizerUtils.getWeightRegularizerFromConfig(layerConfig, conf, conf.getLAYER_FIELD_W_REGULARIZER(), conf.getREGULARIZATION_TYPE_L2())
			Me.dropout = KerasLayerUtils.getDropoutFromConfig(layerConfig, conf)
			KerasLayerUtils.checkForUnsupportedConfigurations(layerConfig, enforceTrainingConfig, conf)
		End Sub

		''' <summary>
		''' Register a lambda layer
		''' </summary>
		''' <param name="lambdaLayerName">   name of the lambda layer in the serialized Keras model </param>
		''' <param name="sameDiffLambdaLayer"> SameDiffLambdaLayer instance to map to Keras Lambda layer </param>
		Public Shared Sub registerLambdaLayer(ByVal lambdaLayerName As String, ByVal sameDiffLambdaLayer As SameDiffLambdaLayer)
			lambdaLayers(lambdaLayerName) = sameDiffLambdaLayer
		End Sub

		''' <summary>
		''' Clear all lambda layers
		''' 
		''' </summary>
		Public Shared Sub clearLambdaLayers()
			lambdaLayers.Clear()
		End Sub

		''' <summary>
		''' Register a custom layer
		''' </summary>
		''' <param name="layerName">   name of custom layer class </param>
		''' <param name="configClass"> class of custom layer </param>
		Public Shared Sub registerCustomLayer(ByVal layerName As String, ByVal configClass As Type)
			customLayers(layerName) = configClass
		End Sub

		''' <summary>
		''' Clear all custom layers
		''' 
		''' </summary>
		Public Shared Sub clearCustomLayers()
			customLayers.Clear()
		End Sub

		''' <summary>
		''' Get Keras major version of this layer.
		''' </summary>
		''' <returns> Keras version as integer </returns>
		Public Overridable ReadOnly Property KerasMajorVersion As Integer?
			Get
				Return Me.kerasMajorVersion
			End Get
		End Property

		''' <summary>
		''' Get Keras layer class name.
		''' </summary>
		''' <returns> Keras layer class name </returns>
		Public Overridable ReadOnly Property ClassName As String
			Get
				Return Me.className_Conflict
			End Get
		End Property

		''' <summary>
		''' Get Keras layer name.
		''' </summary>
		''' <returns> layer name </returns>
		Public Overridable ReadOnly Property LayerName As String
			Get
				Return Me.layerName_Conflict
			End Get
		End Property

		''' <summary>
		''' Get layer input shape.
		''' </summary>
		''' <returns> input shape </returns>
		Public Overridable ReadOnly Property InputShape As Integer()
			Get
				If Me.inputShape_Conflict Is Nothing Then
					Return Nothing
				End If
				Return CType(Me.inputShape_Conflict.Clone(), Integer())
			End Get
		End Property

		''' <summary>
		''' Get Keras layer backend dimension order.
		''' </summary>
		''' <returns> Keras layer (backend) dimension order </returns>
		Public Overridable Property DimOrder As DimOrder
			Get
				Return Me.dimOrder_Conflict
			End Get
			Set(ByVal dimOrder As DimOrder)
				Me.dimOrder_Conflict = dimOrder
			End Set
		End Property


		''' <summary>
		''' Get list of inbound layers.
		''' </summary>
		''' <returns> list of inbound layer names </returns>
		Public Overridable Property InboundLayerNames As IList(Of String)
			Get
				If Me.inboundLayerNames_Conflict Is Nothing Then
					Me.inboundLayerNames_Conflict = New List(Of String)()
				End If
				Return Me.inboundLayerNames_Conflict
			End Get
			Set(ByVal inboundLayerNames As IList(Of String))
				Me.inboundLayerNames_Conflict = New List(Of String)(inboundLayerNames)
			End Set
		End Property


		''' <summary>
		''' Returns number of trainable parameters in layer.
		''' </summary>
		''' <returns> number of trainable parameters </returns>
		Public Overridable ReadOnly Property NumParams As Integer
			Get
				Return 0
			End Get
		End Property

		''' <summary>
		''' Indicates whether layer uses regularization.
		''' </summary>
		''' <returns> boolean </returns>
		Public Overridable Function usesRegularization() As Boolean
			Return (Me.weightL1Regularization > 0.0 OrElse Me.weightL2Regularization > 0.0 OrElse Me.dropout < 1.0)
		End Function

		''' <summary>
		''' Set weights for Keras layer.
		''' </summary>
		''' <param name="weights"> Map of named NDArrays </param>
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: public void setWeights(Map<String, org.nd4j.linalg.api.ndarray.INDArray> weights) throws org.deeplearning4j.nn.modelimport.keras.exceptions.InvalidKerasConfigurationException
		Public Overridable Property Weights As IDictionary(Of String, INDArray)
			Set(ByVal weights As IDictionary(Of String, INDArray))
				'no op
			End Set
			Get
				Return Me.weights_Conflict
			End Get
		End Property


		''' <summary>
		''' Copy Keras layer weights to DL4J Layer.
		''' </summary>
		''' <param name="layer"> DL4J layer </param>
		''' <exception cref="InvalidKerasConfigurationException"> Invalid Keras configuration </exception>
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: public void copyWeightsToLayer(org.deeplearning4j.nn.api.Layer layer) throws org.deeplearning4j.nn.modelimport.keras.exceptions.InvalidKerasConfigurationException
		Public Overridable Sub copyWeightsToLayer(ByVal layer As org.deeplearning4j.nn.api.Layer)
			If Me.NumParams > 0 Then
				Dim dl4jLayerName As String = layer.conf().getLayer().getLayerName()
				Dim kerasLayerName As String = Me.LayerName
				Dim msg As String = "Error when attempting to copy weights from Keras layer " & kerasLayerName & " to DL4J layer " & dl4jLayerName

				If getWeights() Is Nothing Then
					Throw New InvalidKerasConfigurationException(msg & "(weights is null)")
				End If

				Dim paramsInLayer As ISet(Of String) = New HashSet(Of String)(layer.paramTable().Keys)
				Dim paramsInKerasLayer As ISet(Of String) = New HashSet(Of String)(Me.weights_Conflict.Keys)

				' Check for parameters in layer for which we don't have weights. 
				paramsInLayer.RemoveAll(paramsInKerasLayer)
				If paramsInLayer.Count > 0 Then
					Dim joinedParamsInLayer As String = StringUtils.join(paramsInLayer, ", ")
					Throw New InvalidKerasConfigurationException(msg & "(no stored weights for parameters: " & joinedParamsInLayer & ")")
				End If

				' Check for parameters NOT in layer for which we DO have weights. 
				paramsInKerasLayer.RemoveAll(layer.paramTable().Keys)
				If paramsInKerasLayer.Count > 0 Then
					Dim joinedParamsInKerasLayer As String = StringUtils.join(paramsInKerasLayer, ", ")
					Throw New InvalidKerasConfigurationException(msg & "(found no parameters named: " & joinedParamsInKerasLayer & ")")
				End If

				' Copy weights. 
				For Each paramName As String In layer.paramTable().Keys
					Try
						Dim dl4jWeights() As Long = layer.paramTable()(paramName).shape()
						Dim kerasWeights() As Long = weights(paramName).shape()
						Dim variable As INDArray = Me.weights_Conflict(paramName)
						If Not dl4jWeights.SequenceEqual(kerasWeights) AndAlso ArrayUtil.prod(dl4jWeights) = ArrayUtil.prod(kerasWeights) Then
							layer.setParam(paramName, variable.reshape(dl4jWeights))
						Else
							layer.setParam(paramName, variable)

						End If

					Catch e As Exception
						log.error(e.Message)
						Throw New InvalidKerasConfigurationException(e.Message & vbLf & "Tried to set weights for layer with name " & Me.LayerName & ", of " & layer.conf().getLayer().GetType() & "." & vbLf & "Failed to set weights for parameter " & paramName & vbLf & "Expected shape for this parameter: " & layer.getParam(paramName).shapeInfoToString() & ", " & vbLf & "got: " & Me.weights_Conflict(paramName).shapeInfoToString())
					End Try
				Next paramName
			End If
		End Sub

		''' <summary>
		''' Whether this Keras layer maps to a DL4J Layer.
		''' </summary>
		''' <returns> true or false </returns>
		Public Overridable Property Layer As Boolean
			Get
				Return Me.layer_Conflict IsNot Nothing
			End Get
			Set(ByVal layer As Layer)
				Me.layer_Conflict = layer
			End Set
		End Property

		''' <summary>
		''' Gets corresponding DL4J Layer, if any.
		''' </summary>
		''' <returns> DL4J Layer </returns>
		''' <seealso cref= org.deeplearning4j.nn.api.Layer </seealso>
		Public Overridable ReadOnly Property Layer As Layer
			Get
				Return Me.layer_Conflict
			End Get
		End Property


		''' <summary>
		''' Whether this Keras layer maps to a DL4J Vertex.
		''' </summary>
		''' <returns> true or false </returns>
		Public Overridable ReadOnly Property Vertex As Boolean
			Get
				Return Me.vertex_Conflict IsNot Nothing
			End Get
		End Property

		''' <summary>
		''' Gets corresponding DL4J Vertex, if any.
		''' </summary>
		''' <returns> DL4J Vertex </returns>
		''' <seealso cref= org.deeplearning4j.nn.conf.graph.GraphVertex </seealso>
		Public Overridable ReadOnly Property Vertex As GraphVertex
			Get
				Return Me.vertex_Conflict
			End Get
		End Property

		''' <summary>
		''' Whether this Keras layer maps to a DL4J InputPreProcessor.
		''' </summary>
		''' <returns> true or false </returns>
		Public Overridable ReadOnly Property InputPreProcessor As Boolean
			Get
				Return False
			End Get
		End Property



		''' <summary>
		''' Some DL4J layers need explicit specification of number of inputs, which Keras does infer.
		''' This method searches through previous layers until a FeedForwardLayer is found. These layers
		''' have nOut values that subsequently correspond to the nIn value of this layer.
		''' </summary>
		''' <param name="previousLayers">
		''' @return </param>
		''' <exception cref="UnsupportedKerasConfigurationException"> </exception>
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: protected long getNInFromConfig(Map<String, ? extends KerasLayer> previousLayers) throws org.deeplearning4j.nn.modelimport.keras.exceptions.UnsupportedKerasConfigurationException
		Protected Friend Overridable Function getNInFromConfig(Of T1 As KerasLayer)(ByVal previousLayers As IDictionary(Of T1)) As Long
			Dim size As Integer = previousLayers.Count
			Dim count As Integer = 0
			Dim nIn As Long
			Dim inboundLayerName As String = inboundLayerNames(0)
			Do While count <= size
				If previousLayers.ContainsKey(inboundLayerName) Then
					Dim inbound As KerasLayer = previousLayers(inboundLayerName)
					Try
						Dim ffLayer As FeedForwardLayer = DirectCast(inbound.Layer, FeedForwardLayer)
						nIn = ffLayer.getNOut()
						If nIn > 0 Then
							Return nIn
						End If
						count += 1
						inboundLayerName = inbound.getInboundLayerNames()(0)
					Catch e As Exception
						inboundLayerName = inbound.getInboundLayerNames()(0)
					End Try
				End If
			Loop
			Throw New UnsupportedKerasConfigurationException("Could not determine number of input channels for" & "depthwise convolution.")
		End Function


		''' <summary>
		''' Gets appropriate DL4J InputPreProcessor for given InputTypes.
		''' </summary>
		''' <param name="inputType"> Array of InputTypes </param>
		''' <returns> DL4J InputPreProcessor </returns>
		''' <exception cref="InvalidKerasConfigurationException"> Invalid Keras configuration </exception>
		''' <seealso cref= org.deeplearning4j.nn.conf.InputPreProcessor </seealso>
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: public org.deeplearning4j.nn.conf.InputPreProcessor getInputPreprocessor(org.deeplearning4j.nn.conf.inputs.InputType... inputType) throws org.deeplearning4j.nn.modelimport.keras.exceptions.InvalidKerasConfigurationException
		Public Overridable Function getInputPreprocessor(ParamArray ByVal inputType() As InputType) As InputPreProcessor
			Dim preprocessor As InputPreProcessor = Nothing
			If Me.layer_Conflict IsNot Nothing Then
				If inputType.Length > 1 Then
					Dim toUse As InputType = Nothing
					For i As Integer = 0 To inputType.Length - 1
						If inputType(i) IsNot Nothing Then
							If toUse Is Nothing Then
								toUse = inputType(i)
							ElseIf Not toUse.Equals(inputType(i)) Then
								Throw New InvalidKerasConfigurationException("Keras layer of type """ & Me.className_Conflict & """ accepts only one input")
							End If
						End If
					Next i

					If toUse Is Nothing Then
						Throw New InvalidKerasConfigurationException("Keras layer of type """ & Me.className_Conflict & " did not have any inputs!")
					End If

					preprocessor = Me.layer_Conflict.getPreProcessorForInputType(toUse)

				Else
					preprocessor = Me.layer_Conflict.getPreProcessorForInputType(inputType(0))
				End If
			End If
			Return preprocessor
		End Function

		''' <summary>
		''' Get layer output type.
		''' </summary>
		''' <param name="inputType"> Array of InputTypes </param>
		''' <returns> output type as InputType </returns>
		''' <exception cref="InvalidKerasConfigurationException"> Invalid Keras configuration </exception>
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: public org.deeplearning4j.nn.conf.inputs.InputType getOutputType(org.deeplearning4j.nn.conf.inputs.InputType... inputType) throws InvalidKerasConfigurationException, org.deeplearning4j.nn.modelimport.keras.exceptions.UnsupportedKerasConfigurationException
		Public Overridable Function getOutputType(ParamArray ByVal inputType() As InputType) As InputType
			Throw New System.NotSupportedException("Cannot determine output type for Keras layer of type " & Me.className_Conflict)
		End Function

		''' <summary>
		''' Indicates whether this layer a valid inbound layer. Currently, only
		''' (known) DL4J Layers and inputs are valid inbound layers. "Preprocessor"
		''' layers (reshaping, merging, etc.) are replaced by their own inbound layers.
		''' </summary>
		''' <returns> boolean indicating whether layer is valid inbound layer </returns>
		''' <seealso cref= org.deeplearning4j.nn.api.Layer </seealso>
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: public boolean isValidInboundLayer() throws org.deeplearning4j.nn.modelimport.keras.exceptions.InvalidKerasConfigurationException
		Public Overridable ReadOnly Property ValidInboundLayer As Boolean
			Get
				Return (Layer IsNot Nothing OrElse Vertex IsNot Nothing OrElse getInputPreprocessor() IsNot Nothing OrElse Me.className_Conflict.Equals(conf.getLAYER_CLASS_NAME_INPUT()))
			End Get
		End Property
	End Class

End Namespace
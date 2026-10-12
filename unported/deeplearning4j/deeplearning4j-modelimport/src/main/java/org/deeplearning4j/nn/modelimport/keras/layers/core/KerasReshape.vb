Imports System.Collections.Generic
Imports val = lombok.val
Imports CNN2DFormat = org.deeplearning4j.nn.conf.CNN2DFormat
Imports InputPreProcessor = org.deeplearning4j.nn.conf.InputPreProcessor
Imports InputType = org.deeplearning4j.nn.conf.inputs.InputType
Imports KerasLayer = org.deeplearning4j.nn.modelimport.keras.KerasLayer
Imports InvalidKerasConfigurationException = org.deeplearning4j.nn.modelimport.keras.exceptions.InvalidKerasConfigurationException
Imports UnsupportedKerasConfigurationException = org.deeplearning4j.nn.modelimport.keras.exceptions.UnsupportedKerasConfigurationException
Imports ReshapePreprocessor = org.deeplearning4j.nn.modelimport.keras.preprocessors.ReshapePreprocessor
Imports KerasLayerUtils = org.deeplearning4j.nn.modelimport.keras.utils.KerasLayerUtils

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

Namespace org.deeplearning4j.nn.modelimport.keras.layers.core



	''' <summary>
	''' Imports Reshape layer from Keras
	''' 
	''' @author Max Pumperla
	''' </summary>
	Public Class KerasReshape
		Inherits KerasLayer

		Private targetShape() As Long

		''' <summary>
		''' Constructor from parsed Keras layer configuration dictionary.
		''' </summary>
		''' <param name="layerConfig"> dictionary containing Keras layer configuration </param>
		''' <exception cref="InvalidKerasConfigurationException">     Invalid Keras config </exception>
		''' <exception cref="UnsupportedKerasConfigurationException"> Unsupported Keras config </exception>
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: public KerasReshape(java.util.Map<String, Object> layerConfig) throws InvalidKerasConfigurationException, org.deeplearning4j.nn.modelimport.keras.exceptions.UnsupportedKerasConfigurationException
		Public Sub New(ByVal layerConfig As IDictionary(Of String, Object))
			Me.New(layerConfig, True)
		End Sub

		Private Function listToLongArray(ByVal list As IList(Of Integer)) As Long()
			Dim retVal(list.Count - 1) As Long
			For i As Integer = 0 To list.Count - 1
				retVal(i) = list(i)
			Next i
			Return retVal
		End Function
		''' <summary>
		''' Constructor from parsed Keras layer configuration dictionary.
		''' </summary>
		''' <param name="layerConfig">           dictionary containing Keras layer configuration </param>
		''' <param name="enforceTrainingConfig"> whether to enforce training-related configuration options </param>
		''' <exception cref="InvalidKerasConfigurationException">     Invalid Keras config </exception>
		''' <exception cref="UnsupportedKerasConfigurationException"> Unsupported Keras config </exception>
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: public KerasReshape(java.util.Map<String, Object> layerConfig, boolean enforceTrainingConfig) throws InvalidKerasConfigurationException, org.deeplearning4j.nn.modelimport.keras.exceptions.UnsupportedKerasConfigurationException
		Public Sub New(ByVal layerConfig As IDictionary(Of String, Object), ByVal enforceTrainingConfig As Boolean)
			MyBase.New(layerConfig, enforceTrainingConfig)
			Dim innerConfig As IDictionary(Of String, Object) = KerasLayerUtils.getInnerLayerConfigFromConfig(layerConfig, conf)
			Dim targetShape As String = "target_shape"
			If innerConfig.ContainsKey(targetShape) Then
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @SuppressWarnings("unchecked") java.util.List<Integer> targetShapeList = (java.util.List<Integer>) innerConfig.get(targetShape);
				Dim targetShapeList As IList(Of Integer) = DirectCast(innerConfig(targetShape), IList(Of Integer))
				Me.targetShape = listToLongArray(targetShapeList)
			End If
		End Sub

		''' <summary>
		''' Whether this Keras layer maps to a DL4J InputPreProcessor.
		''' </summary>
		''' <returns> true </returns>
		Public Overrides ReadOnly Property InputPreProcessor As Boolean
			Get
				Return True
			End Get
		End Property

		''' <summary>
		''' Gets appropriate DL4J InputPreProcessor for given InputTypes.
		''' </summary>
		''' <param name="inputType"> Array of InputTypes </param>
		''' <returns> DL4J InputPreProcessor </returns>
		''' <exception cref="InvalidKerasConfigurationException"> Invalid Keras config </exception>
		''' <seealso cref= org.deeplearning4j.nn.conf.InputPreProcessor </seealso>
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: @Override public org.deeplearning4j.nn.conf.InputPreProcessor getInputPreprocessor(org.deeplearning4j.nn.conf.inputs.InputType... inputType) throws org.deeplearning4j.nn.modelimport.keras.exceptions.InvalidKerasConfigurationException
		Public Overrides Function getInputPreprocessor(ParamArray ByVal inputType() As InputType) As InputPreProcessor
			If inputType.Length > 1 Then
				Throw New InvalidKerasConfigurationException("Keras Reshape layer accepts only one input (received " & inputType.Length & ")")
			End If
			Dim preprocessor As InputPreProcessor = Nothing
			If TypeOf inputType(0) Is InputType.InputTypeConvolutional Then
				Dim it As InputType.InputTypeConvolutional = DirectCast(inputType(0), InputType.InputTypeConvolutional)
				Dim inputShape As val = New Long(){it.getChannels(), it.getHeight(), it.getWidth()}
				Dim dimOrder As val = Me.DimOrder
				If dimOrder = DimOrder.THEANO OrElse dimOrder = DimOrder.NONE AndAlso kerasMajorVersion = 1 Then
					If targetShape.Length = 2 Then ' edge caseKeras
						targetShape = New Long(){targetShape(1), targetShape(0)}
					Else
						targetShape = New Long(){targetShape(1), targetShape(0), targetShape(2)}
					End If
					preprocessor = New ReshapePreprocessor(inputShape, targetShape, False, CNN2DFormat.NCHW)
				Else ' (dimOrder == DimOrder.TENSORFLOW || dimOrder == DimOrder.NONE && kerasMajorVersion == 2)
					preprocessor = New ReshapePreprocessor(inputShape, targetShape, False, CNN2DFormat.NHWC)
				End If

			ElseIf TypeOf inputType(0) Is InputType.InputTypeConvolutional3D Then
				Dim it As InputType.InputTypeConvolutional3D = DirectCast(inputType(0), InputType.InputTypeConvolutional3D)
				Dim inputShape As val = New Long() { it.getDepth(), it.getHeight(), it.getWidth(), it.getChannels() }
				Dim dimOrder As val = Me.DimOrder
				If dimOrder = DimOrder.THEANO OrElse dimOrder = DimOrder.NONE AndAlso kerasMajorVersion = 1 Then
					If targetShape.Length = 3 Then ' Keras edge case
						targetShape = New Long() { targetShape(1), targetShape(0), targetShape(2) }
					Else
						targetShape = New Long() { targetShape(2), targetShape(1), targetShape(0), targetShape(3) }
					End If
					preprocessor = New ReshapePreprocessor(inputShape, targetShape, False, Nothing)
				Else
					If inputShape(0) <> targetShape(0) Then
						targetShape = New Long() { targetShape(3), targetShape(0), targetShape(1), targetShape(2) }
					End If
					preprocessor = New ReshapePreprocessor(inputShape, targetShape, False, Nothing)
				End If
			ElseIf TypeOf inputType(0) Is InputType.InputTypeRecurrent Then
				Dim it As InputType.InputTypeRecurrent = DirectCast(inputType(0), InputType.InputTypeRecurrent)
				Dim inputShape As val = New Long(){it.getSize(), it.getTimeSeriesLength()}
				preprocessor = New ReshapePreprocessor(inputShape, Me.targetShape, False, Nothing)
			ElseIf TypeOf inputType(0) Is InputType.InputTypeFeedForward Then
				Dim it As InputType.InputTypeFeedForward = DirectCast(inputType(0), InputType.InputTypeFeedForward)
				Dim inputShape As val = New Long(){it.getSize()}
				If targetShape.Length = 3 Then
					targetShape = targetShapeForDimOrder(inputShape, targetShape)
				End If
				preprocessor = New ReshapePreprocessor(inputShape, Me.targetShape, False, Nothing)
			End If
			Return preprocessor
		End Function

		Public Overridable Function targetShapeForDimOrder(ByVal inputShape() As Long, ByVal targetShape() As Long) As Long()
			If dimOrder_Conflict = DimOrder.THEANO OrElse dimOrder_Conflict = DimOrder.NONE AndAlso kerasMajorVersion = 1 Then
				If dimOrder_Conflict = DimOrder.NONE Then
					targetShape = New Long(){targetShape(2), targetShape(0), targetShape(1)}
				Else
					targetShape = New Long(){targetShape(1), targetShape(2), targetShape(0)}
				End If
			Else
				If inputShape(0) <> targetShape(0) Then
					targetShape = New Long(){targetShape(0), targetShape(1), targetShape(2)}
				End If
			End If
			Return targetShape
		End Function

		''' <summary>
		''' Get layer output type.
		''' </summary>
		''' <param name="inputType"> Array of InputTypes </param>
		''' <returns> output type as InputType </returns>
		''' <exception cref="InvalidKerasConfigurationException"> Invalid Keras config </exception>
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: @Override public org.deeplearning4j.nn.conf.inputs.InputType getOutputType(org.deeplearning4j.nn.conf.inputs.InputType... inputType) throws org.deeplearning4j.nn.modelimport.keras.exceptions.InvalidKerasConfigurationException
		Public Overrides Function getOutputType(ParamArray ByVal inputType() As InputType) As InputType
			If inputType.Length > 1 Then
				Throw New InvalidKerasConfigurationException("Keras Reshape layer accepts only one input (received " & inputType.Length & ")")
			End If
			Dim reshape As ReshapePreprocessor = DirectCast(getInputPreprocessor(inputType), ReshapePreprocessor)
			Return reshape.getOutputType(inputType(0))
		End Function
	End Class
End Namespace
Imports System
Imports System.Collections.Generic
Imports lombok
Imports LayerConstraint = org.deeplearning4j.nn.api.layers.LayerConstraint
Imports InputPreProcessor = org.deeplearning4j.nn.conf.InputPreProcessor
Imports RNNFormat = org.deeplearning4j.nn.conf.RNNFormat
Imports Distribution = org.deeplearning4j.nn.conf.distribution.Distribution
Imports InputType = org.deeplearning4j.nn.conf.inputs.InputType
Imports IWeightInit = org.deeplearning4j.nn.weights.IWeightInit
Imports WeightInit = org.deeplearning4j.nn.weights.WeightInit
Imports WeightInitDistribution = org.deeplearning4j.nn.weights.WeightInitDistribution

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
'ORIGINAL LINE: @Data @NoArgsConstructor @ToString(callSuper = true) @EqualsAndHashCode(callSuper = true) public abstract class BaseRecurrentLayer extends FeedForwardLayer
	<Serializable>
	Public MustInherit Class BaseRecurrentLayer
		Inherits FeedForwardLayer

		Protected Friend weightInitFnRecurrent As IWeightInit
		Protected Friend rnnDataFormat As RNNFormat

		Protected Friend Sub New(ByVal builder As Builder)
			MyBase.New(builder)
			Me.weightInitFnRecurrent = builder.weightInitFnRecurrent
			Me.rnnDataFormat = builder.rnnDataFormat
		End Sub

		Public Overrides Function getOutputType(ByVal layerIndex As Integer, ByVal inputType As InputType) As InputType
			If inputType Is Nothing OrElse inputType.getType() <> InputType.Type.RNN Then
				Throw New System.InvalidOperationException("Invalid input for RNN layer (layer index = " & layerIndex & ", layer name = """ & getLayerName() & """): expect RNN input type with size > 0. Got: " & inputType)
			End If

			Dim itr As InputType.InputTypeRecurrent = DirectCast(inputType, InputType.InputTypeRecurrent)

			Return InputType.recurrent(nOut, itr.getTimeSeriesLength(), itr.getFormat())
		End Function

		Public Overrides Sub setNIn(ByVal inputType As InputType, ByVal override As Boolean)
			If inputType Is Nothing OrElse inputType.getType() <> InputType.Type.RNN Then
				Throw New System.InvalidOperationException("Invalid input for RNN layer (layer name = """ & getLayerName() & """): expect RNN input type with size > 0. Got: " & inputType)
			End If

			Dim r As InputType.InputTypeRecurrent = DirectCast(inputType, InputType.InputTypeRecurrent)
			If nIn <= 0 OrElse override Then
				Me.nIn = r.getSize()
			End If

			If rnnDataFormat = Nothing OrElse override Then
				Me.rnnDataFormat = r.getFormat()
			End If
		End Sub

		Public Overrides Function getPreProcessorForInputType(ByVal inputType As InputType) As InputPreProcessor
			Return InputTypeUtil.getPreprocessorForInputTypeRnnLayers(inputType, rnnDataFormat,getLayerName())
		End Function

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @NoArgsConstructor @Getter @Setter public static abstract class Builder<T extends Builder<T>> extends FeedForwardLayer.Builder<T>
		Public MustInherit Class Builder(Of T As Builder(Of T))
			Inherits FeedForwardLayer.Builder(Of T)

			''' <summary>
			''' Set the format of data expected by the RNN. NCW = [miniBatchSize, size, timeSeriesLength],
			''' NWC = [miniBatchSize, timeSeriesLength, size]. Defaults to NCW.
			''' </summary>
			Protected Friend rnnDataFormat As RNNFormat = RNNFormat.NCW

			''' <summary>
			''' Set constraints to be applied to the RNN recurrent weight parameters of this layer. Default: no
			''' constraints.<br> Constraints can be used to enforce certain conditions (non-negativity of parameters,
			''' max-norm regularization, etc). These constraints are applied at each iteration, after the parameters have
			''' been updated.
			''' </summary>
			Protected Friend recurrentConstraints As IList(Of LayerConstraint)

			''' <summary>
			''' Set constraints to be applied to the RNN input weight parameters of this layer. Default: no constraints.<br>
			''' Constraints can be used to enforce certain conditions (non-negativity of parameters, max-norm regularization,
			''' etc). These constraints are applied at each iteration, after the parameters have been updated.
			''' 
			''' </summary>
			Protected Friend inputWeightConstraints As IList(Of LayerConstraint)

			''' <summary>
			''' Set the weight initialization for the recurrent weights. Not that if this is not set explicitly, the same
			''' weight initialization as the layer input weights is also used for the recurrent weights.
			''' 
			''' </summary>
			Protected Friend weightInitFnRecurrent As IWeightInit

			''' <summary>
			''' Set constraints to be applied to the RNN recurrent weight parameters of this layer. Default: no
			''' constraints.<br> Constraints can be used to enforce certain conditions (non-negativity of parameters,
			''' max-norm regularization, etc). These constraints are applied at each iteration, after the parameters have
			''' been updated.
			''' </summary>
			''' <param name="constraints"> Constraints to apply to the recurrent weight parameters of this layer </param>
			Public Overridable Function constrainRecurrent(ParamArray ByVal constraints() As LayerConstraint) As T
				Me.setRecurrentConstraints(Arrays.asList(constraints))
				Return CType(Me, T)
			End Function

			''' <summary>
			''' Set constraints to be applied to the RNN input weight parameters of this layer. Default: no constraints.<br>
			''' Constraints can be used to enforce certain conditions (non-negativity of parameters, max-norm regularization,
			''' etc). These constraints are applied at each iteration, after the parameters have been updated.
			''' </summary>
			''' <param name="constraints"> Constraints to apply to the input weight parameters of this layer </param>
			Public Overridable Function constrainInputWeights(ParamArray ByVal constraints() As LayerConstraint) As T
				Me.setInputWeightConstraints(Arrays.asList(constraints))
				Return CType(Me, T)
			End Function

			''' <summary>
			''' Set the weight initialization for the recurrent weights. Not that if this is not set explicitly, the same
			''' weight initialization as the layer input weights is also used for the recurrent weights.
			''' </summary>
			''' <param name="weightInit"> Weight initialization for the recurrent weights only. </param>
			Public Overridable Function weightInitRecurrent(ByVal weightInit As IWeightInit) As T
				Me.setWeightInitFnRecurrent(weightInit)
				Return CType(Me, T)
			End Function

			''' <summary>
			''' Set the weight initialization for the recurrent weights. Not that if this is not set explicitly, the same
			''' weight initialization as the layer input weights is also used for the recurrent weights.
			''' </summary>
			''' <param name="weightInit"> Weight initialization for the recurrent weights only. </param>
			Public Overridable Function weightInitRecurrent(ByVal weightInit As WeightInit) As T
				If weightInit = WeightInit.DISTRIBUTION Then
					Throw New System.NotSupportedException("Not supported!, Use weightInit(Distribution distribution) instead!")
				End If

				Me.setWeightInitFnRecurrent(weightInit.getWeightInitFunction())
				Return CType(Me, T)
			End Function

			''' <summary>
			''' Set the weight initialization for the recurrent weights, based on the specified distribution. Not that if
			''' this is not set explicitly, the same weight initialization as the layer input weights is also used for the
			''' recurrent weights.
			''' </summary>
			''' <param name="dist"> Distribution to use for initializing the recurrent weights </param>
			Public Overridable Function weightInitRecurrent(ByVal dist As Distribution) As T
				Me.setWeightInitFnRecurrent(New WeightInitDistribution(dist))
				Return CType(Me, T)
			End Function

			Public Overridable Function dataFormat(ByVal rnnDataFormat As RNNFormat) As T
				Me.rnnDataFormat = rnnDataFormat
				Return CType(Me, T)
			End Function
		End Class
	End Class

End Namespace
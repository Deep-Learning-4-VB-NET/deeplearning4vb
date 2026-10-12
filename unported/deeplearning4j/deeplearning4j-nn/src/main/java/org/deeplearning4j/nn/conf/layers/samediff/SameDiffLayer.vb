Imports System
Imports System.Collections.Generic
Imports lombok
Imports Layer = org.deeplearning4j.nn.api.Layer
Imports MaskState = org.deeplearning4j.nn.api.MaskState
Imports NeuralNetConfiguration = org.deeplearning4j.nn.conf.NeuralNetConfiguration
Imports IWeightInit = org.deeplearning4j.nn.weights.IWeightInit
Imports WeightInit = org.deeplearning4j.nn.weights.WeightInit
Imports TrainingListener = org.deeplearning4j.optimize.api.TrainingListener
Imports SDVariable = org.nd4j.autodiff.samediff.SDVariable
Imports SameDiff = org.nd4j.autodiff.samediff.SameDiff
Imports DataType = org.nd4j.linalg.api.buffer.DataType
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports org.nd4j.common.primitives

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


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Data @EqualsAndHashCode(callSuper = true) public abstract class SameDiffLayer extends AbstractSameDiffLayer
	<Serializable>
	Public MustInherit Class SameDiffLayer
		Inherits AbstractSameDiffLayer

		Protected Friend weightInit As WeightInit
		Protected Friend paramWeightInit As IDictionary(Of String, IWeightInit)

		Protected Friend Sub New(ByVal builder As Builder)
			MyBase.New(builder)
			Me.weightInit = builder.weightInit
			Me.paramWeightInit = builder.paramWeightInit
		End Sub

		Protected Friend Sub New()
			'No op constructor for Jackson
		End Sub

		''' <summary>
		''' Define the layer
		''' </summary>
		''' <param name="sameDiff"> SameDiff instance </param>
		''' <param name="layerInput"> Input to the layer </param>
		''' <param name="paramTable"> Parameter table - keys as defined by <seealso cref="defineParameters(SDLayerParams)"/> </param>
		''' <param name="mask"> Optional, maybe null. Mask to apply if supported </param>
		''' <returns> The final layer variable corresponding to the activations/output from the forward pass </returns>
		Public MustOverride Function defineLayer(ByVal sameDiff As SameDiff, ByVal layerInput As SDVariable, ByVal paramTable As IDictionary(Of String, SDVariable), ByVal mask As SDVariable) As SDVariable

		''' <seealso cref= Layer#feedForwardMaskArray(INDArray, MaskState, int) </seealso>
		Public Overridable Function feedForwardMaskArray(ByVal maskArray As INDArray, ByVal currentMaskState As MaskState, ByVal minibatchSize As Integer) As Pair(Of INDArray, MaskState)
			Return New Pair(Of INDArray, MaskState)(maskArray, currentMaskState)
		End Function

		''' <summary>
		''' Validate input arrays to confirm that they fulfill the assumptions of the layer. If they don't, throw an exception. </summary>
		''' <param name="input"> input to the layer </param>
		Public Overridable Sub validateInput(ByVal input As INDArray)
		End Sub

		'==================================================================================================================

		Public Overrides Function instantiate(ByVal conf As NeuralNetConfiguration, ByVal trainingListeners As ICollection(Of TrainingListener), ByVal layerIndex As Integer, ByVal layerParamsView As INDArray, ByVal initializeParams As Boolean, ByVal networkDataType As DataType) As Layer
			Dim ret As New org.deeplearning4j.nn.layers.samediff.SameDiffLayer(conf, networkDataType)
			ret.Index = layerIndex
			ret.ParamsViewArray = layerParamsView
			Dim paramTable As IDictionary(Of String, INDArray) = initializer().init(conf, layerParamsView, initializeParams)
			ret.ParamTable = paramTable
			ret.Conf = conf
			Return ret
		End Function

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @SuppressWarnings("unchecked") @Getter @Setter public static abstract class Builder<T extends Builder<T>> extends AbstractSameDiffLayer.Builder<T>
		Public MustInherit Class Builder(Of T As Builder(Of T))
			Inherits AbstractSameDiffLayer.Builder(Of T)

'JAVA TO VB CONVERTER NOTE: The field weightInit was renamed since Visual Basic does not allow fields to have the same name as other class members:
			Protected Friend weightInit_Conflict As WeightInit = WeightInit.XAVIER
			Protected Friend paramWeightInit As IDictionary(Of String, IWeightInit)

			''' <param name="weightInit"> Weight initialization to use for the layer </param>
'JAVA TO VB CONVERTER NOTE: The parameter weightInit was renamed since Visual Basic will not allow parameters with the same name as their enclosing function or property:
			Public Overridable Function weightInit(ByVal weightInit_Conflict As WeightInit) As T
				Me.setWeightInit(weightInit_Conflict)
				Return CType(Me, T)
			End Function

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public T weightInit(@NonNull String param, @NonNull IWeightInit weightInit)
'JAVA TO VB CONVERTER NOTE: The parameter weightInit was renamed since Visual Basic will not allow parameters with the same name as their enclosing function or property:
			Public Overridable Function weightInit(ByVal param As String, ByVal weightInit_Conflict As IWeightInit) As T
				If paramWeightInit Is Nothing Then
					paramWeightInit = New Dictionary(Of String, IWeightInit)()
				End If
				paramWeightInit(param) = weightInit_Conflict
				Return CType(Me, T)
			End Function
		End Class
	End Class

End Namespace
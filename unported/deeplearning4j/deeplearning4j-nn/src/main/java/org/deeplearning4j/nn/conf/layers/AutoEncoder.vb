Imports System
Imports System.Collections.Generic
Imports lombok
Imports Layer = org.deeplearning4j.nn.api.Layer
Imports ParamInitializer = org.deeplearning4j.nn.api.ParamInitializer
Imports NeuralNetConfiguration = org.deeplearning4j.nn.conf.NeuralNetConfiguration
Imports InputType = org.deeplearning4j.nn.conf.inputs.InputType
Imports LayerMemoryReport = org.deeplearning4j.nn.conf.memory.LayerMemoryReport
Imports MemoryReport = org.deeplearning4j.nn.conf.memory.MemoryReport
Imports PretrainParamInitializer = org.deeplearning4j.nn.params.PretrainParamInitializer
Imports TrainingListener = org.deeplearning4j.optimize.api.TrainingListener
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

Namespace org.deeplearning4j.nn.conf.layers


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Data @NoArgsConstructor @ToString(callSuper = true) @EqualsAndHashCode(callSuper = true) public class AutoEncoder extends BasePretrainNetwork
	<Serializable>
	Public Class AutoEncoder
		Inherits BasePretrainNetwork

		Protected Friend corruptionLevel As Double
		Protected Friend sparsity As Double

		' Builder
		Private Sub New(ByVal builder As Builder)
			MyBase.New(builder)
			Me.corruptionLevel = builder.corruptionLevel_Conflict
			Me.sparsity = builder.sparsity_Conflict
			initializeConstraints(builder)
		End Sub

		Public Overrides Function instantiate(ByVal conf As NeuralNetConfiguration, ByVal trainingListeners As ICollection(Of TrainingListener), ByVal layerIndex As Integer, ByVal layerParamsView As INDArray, ByVal initializeParams As Boolean, ByVal networkDataType As DataType) As Layer
			Dim ret As New org.deeplearning4j.nn.layers.feedforward.autoencoder.AutoEncoder(conf, networkDataType)
			ret.setListeners(trainingListeners)
			ret.Index = layerIndex
			ret.ParamsViewArray = layerParamsView
			Dim paramTable As IDictionary(Of String, INDArray) = initializer().init(conf, layerParamsView, initializeParams)
			ret.ParamTable = paramTable
			ret.Conf = conf
			Return ret
		End Function

		Public Overrides Function initializer() As ParamInitializer
			Return PretrainParamInitializer.Instance
		End Function

		Public Overrides Function getMemoryReport(ByVal inputType As InputType) As LayerMemoryReport
			'Because of supervised + unsupervised modes: we'll assume unsupervised, which has the larger memory requirements
			Dim outputType As InputType = getOutputType(-1, inputType)

			Dim actElementsPerEx As val = outputType.arrayElementsPerExample() + inputType.arrayElementsPerExample()
			Dim numParams As val = initializer().numParams(Me)
			Dim updaterStateSize As val = CInt(Math.Truncate(getIUpdater().stateSize(numParams)))

			Dim trainSizePerEx As Integer = 0
			If getIDropout() IsNot Nothing Then
				If False Then
					'TODO drop connect
					'Dup the weights... note that this does NOT depend on the minibatch size...
				Else
					'Assume we dup the input
					trainSizePerEx += inputType.arrayElementsPerExample()
				End If
			End If

			'Also, during backprop: we do a preOut call -> gives us activations size equal to the output size
			' which is modified in-place by loss function
			trainSizePerEx += actElementsPerEx

			Return (New LayerMemoryReport.Builder(layerName, GetType(AutoEncoder), inputType, outputType)).standardMemory(numParams, updaterStateSize).workingMemory(0, 0, 0, trainSizePerEx).cacheMemory(MemoryReport.CACHE_MODE_ALL_ZEROS, MemoryReport.CACHE_MODE_ALL_ZEROS).build()
		End Function

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @AllArgsConstructor @Getter @Setter public static class Builder extends BasePretrainNetwork.Builder<Builder>
		Public Class Builder
			Inherits BasePretrainNetwork.Builder(Of Builder)

			''' <summary>
			''' Level of corruption - 0.0 (none) to 1.0 (all values corrupted)
			''' 
			''' </summary>
'JAVA TO VB CONVERTER NOTE: The field corruptionLevel was renamed since Visual Basic does not allow fields to have the same name as other class members:
			Friend corruptionLevel_Conflict As Double = 3e-1f

			''' <summary>
			''' Autoencoder sparity parameter
			''' 
			''' </summary>
'JAVA TO VB CONVERTER NOTE: The field sparsity was renamed since Visual Basic does not allow fields to have the same name as other class members:
			Friend sparsity_Conflict As Double = 0f

			Public Sub New()
			End Sub

			''' <summary>
			''' Builder - sets the level of corruption - 0.0 (none) to 1.0 (all values corrupted)
			''' </summary>
			''' <param name="corruptionLevel"> Corruption level (0 to 1) </param>
			Public Sub New(ByVal corruptionLevel As Double)
				Me.setCorruptionLevel(corruptionLevel)
			End Sub

			''' <summary>
			''' Level of corruption - 0.0 (none) to 1.0 (all values corrupted)
			''' </summary>
			''' <param name="corruptionLevel"> Corruption level (0 to 1) </param>
'JAVA TO VB CONVERTER NOTE: The parameter corruptionLevel was renamed since Visual Basic will not allow parameters with the same name as their enclosing function or property:
			Public Overridable Function corruptionLevel(ByVal corruptionLevel_Conflict As Double) As Builder
				Me.setCorruptionLevel(corruptionLevel_Conflict)
				Return Me
			End Function

			''' <summary>
			''' Autoencoder sparity parameter
			''' </summary>
			''' <param name="sparsity"> Sparsity </param>
'JAVA TO VB CONVERTER NOTE: The parameter sparsity was renamed since Visual Basic will not allow parameters with the same name as their enclosing function or property:
			Public Overridable Function sparsity(ByVal sparsity_Conflict As Double) As Builder
				Me.setSparsity(sparsity_Conflict)
				Return Me
			End Function

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Override @SuppressWarnings("unchecked") public AutoEncoder build()
			Public Overrides Function build() As AutoEncoder
				Return New AutoEncoder(Me)
			End Function
		End Class
	End Class

End Namespace
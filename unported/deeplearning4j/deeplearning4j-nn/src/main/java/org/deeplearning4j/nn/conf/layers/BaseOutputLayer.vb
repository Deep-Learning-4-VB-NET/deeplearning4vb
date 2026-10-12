Imports System
Imports lombok
Imports InputType = org.deeplearning4j.nn.conf.inputs.InputType
Imports LayerMemoryReport = org.deeplearning4j.nn.conf.memory.LayerMemoryReport
Imports MemoryReport = org.deeplearning4j.nn.conf.memory.MemoryReport
Imports ILossFunction = org.nd4j.linalg.lossfunctions.ILossFunction
Imports LossFunction = org.nd4j.linalg.lossfunctions.LossFunctions.LossFunction
Imports LossBinaryXENT = org.nd4j.linalg.lossfunctions.impl.LossBinaryXENT
Imports LossMCXENT = org.nd4j.linalg.lossfunctions.impl.LossMCXENT
Imports LossMSE = org.nd4j.linalg.lossfunctions.impl.LossMSE
Imports LossNegativeLogLikelihood = org.nd4j.linalg.lossfunctions.impl.LossNegativeLogLikelihood

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
'ORIGINAL LINE: @Data @NoArgsConstructor @ToString(callSuper = true) @EqualsAndHashCode(callSuper = true) public abstract class BaseOutputLayer extends FeedForwardLayer
	<Serializable>
	Public MustInherit Class BaseOutputLayer
		Inherits FeedForwardLayer

		Protected Friend lossFn As ILossFunction
'JAVA TO VB CONVERTER NOTE: The field hasBias was renamed since Visual Basic does not allow fields to have the same name as other class members:
		Protected Friend hasBias_Conflict As Boolean = True

		Protected Friend Sub New(ByVal builder As Builder)
			MyBase.New(builder)
			Me.lossFn = builder.lossFn
			Me.hasBias_Conflict = builder.hasBias
		End Sub

		Public Overridable Function hasBias() As Boolean
			Return hasBias_Conflict
		End Function

		Public Overrides Function getMemoryReport(ByVal inputType As InputType) As LayerMemoryReport
			'Basically a dense layer...
			Dim outputType As InputType = getOutputType(-1, inputType)

			Dim numParams As val = initializer().numParams(Me)
			Dim updaterStateSize As val = CInt(Math.Truncate(getIUpdater().stateSize(numParams)))

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

			Return (New LayerMemoryReport.Builder(layerName, GetType(OutputLayer), inputType, outputType)).standardMemory(numParams, updaterStateSize).workingMemory(0, 0, trainSizeFixed, trainSizeVariable).cacheMemory(MemoryReport.CACHE_MODE_ALL_ZEROS, MemoryReport.CACHE_MODE_ALL_ZEROS).build()
		End Function


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Getter @Setter public static abstract class Builder<T extends Builder<T>> extends FeedForwardLayer.Builder<T>
		Public MustInherit Class Builder(Of T As Builder(Of T))
			Inherits FeedForwardLayer.Builder(Of T)

			''' <summary>
			''' Loss function for the output layer
			''' </summary>
			Protected Friend lossFn As ILossFunction = New LossMCXENT()

			''' <summary>
			''' If true (default): include bias parameters in the model. False: no bias.
			''' 
			''' </summary>
'JAVA TO VB CONVERTER NOTE: The field hasBias was renamed since Visual Basic does not allow fields to have the same name as other class members:
			Friend hasBias_Conflict As Boolean = True

			Public Sub New()
			End Sub

			''' <param name="lossFunction"> Loss function for the output layer </param>
			Public Sub New(ByVal lossFunction As LossFunction)
				Me.lossFunction(lossFunction)
			End Sub

			''' <param name="lossFunction"> Loss function for the output layer </param>
			Public Sub New(ByVal lossFunction As ILossFunction)
				Me.setLossFn(lossFunction)
			End Sub

			''' <param name="lossFunction"> Loss function for the output layer </param>
'JAVA TO VB CONVERTER NOTE: The parameter lossFunction was renamed since Visual Basic will not allow parameters with the same name as their enclosing function or property:
			Public Overridable Function lossFunction(ByVal lossFunction_Conflict As LossFunction) As T
				Return lossFunction(lossFunction_Conflict.getILossFunction())
			End Function

			''' <summary>
			''' If true (default): include bias parameters in the model. False: no bias.
			''' </summary>
			''' <param name="hasBias"> If true: include bias parameters in this model </param>
'JAVA TO VB CONVERTER NOTE: The parameter hasBias was renamed since Visual Basic will not allow parameters with the same name as their enclosing function or property:
			Public Overridable Function hasBias(ByVal hasBias_Conflict As Boolean) As T
				Me.setHasBias(hasBias_Conflict)
				Return CType(Me, T)
			End Function

			''' <param name="lossFunction"> Loss function for the output layer </param>
'JAVA TO VB CONVERTER NOTE: The parameter lossFunction was renamed since Visual Basic will not allow parameters with the same name as their enclosing function or property:
			Public Overridable Function lossFunction(ByVal lossFunction_Conflict As ILossFunction) As T
				Me.setLossFn(lossFunction_Conflict)
				Return CType(Me, T)
			End Function
		End Class
	End Class

End Namespace
Imports System
Imports Data = lombok.Data
Imports EqualsAndHashCode = lombok.EqualsAndHashCode
Imports NoArgsConstructor = lombok.NoArgsConstructor
Imports InputPreProcessor = org.deeplearning4j.nn.conf.InputPreProcessor
Imports NeuralNetConfiguration = org.deeplearning4j.nn.conf.NeuralNetConfiguration
Imports InputType = org.deeplearning4j.nn.conf.inputs.InputType
Imports InvalidInputTypeException = org.deeplearning4j.nn.conf.inputs.InvalidInputTypeException
Imports MemoryReport = org.deeplearning4j.nn.conf.memory.MemoryReport
Imports ComputationGraph = org.deeplearning4j.nn.graph.ComputationGraph
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

Namespace org.deeplearning4j.nn.conf.graph


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @NoArgsConstructor @Data @EqualsAndHashCode(callSuper = false) public class LayerVertex extends GraphVertex
	<Serializable>
	Public Class LayerVertex
		Inherits GraphVertex

		Private layerConf As NeuralNetConfiguration
'JAVA TO VB CONVERTER NOTE: The field preProcessor was renamed since Visual Basic does not allow fields to have the same name as other class members:
		Private preProcessor_Conflict As InputPreProcessor
		'Set outputVertex to true when Layer is an OutputLayer, OR For use in specialized situations like reinforcement learning
		' For RL situations, this Layer insn't an OutputLayer, but is the last layer in a graph, that gets its error/epsilon
		' passed in externally
		Private outputVertex As Boolean


		Public Sub New(ByVal layerConf As NeuralNetConfiguration, ByVal preProcessor As InputPreProcessor)
			Me.layerConf = layerConf
			Me.preProcessor_Conflict = preProcessor
		End Sub

		Public Overridable ReadOnly Property PreProcessor As InputPreProcessor
			Get
				Return Me.preProcessor_Conflict
			End Get
		End Property

		Public Overrides Function clone() As GraphVertex
			Return New LayerVertex(layerConf.clone(), (If(preProcessor_Conflict IsNot Nothing, preProcessor_Conflict.clone(), Nothing)))
		End Function

		Public Overrides Function Equals(ByVal o As Object) As Boolean
			If Not (TypeOf o Is LayerVertex) Then
				Return False
			End If
			Dim lv As LayerVertex = DirectCast(o, LayerVertex)
			If (layerConf Is Nothing AndAlso lv.layerConf IsNot Nothing) OrElse (layerConf IsNot Nothing AndAlso lv.layerConf Is Nothing) Then
				Return False
			End If
			If layerConf IsNot Nothing AndAlso Not layerConf.Equals(lv.layerConf) Then
				Return False
			End If
			If preProcessor_Conflict Is Nothing AndAlso lv.preProcessor_Conflict IsNot Nothing OrElse preProcessor_Conflict IsNot Nothing AndAlso lv.preProcessor_Conflict Is Nothing Then
				Return False
			End If
			Return preProcessor_Conflict Is Nothing OrElse preProcessor_Conflict.Equals(lv.preProcessor_Conflict)
		End Function

		Public Overrides Function GetHashCode() As Integer
			Return layerConf.GetHashCode() Xor (If(preProcessor_Conflict IsNot Nothing, preProcessor_Conflict.GetHashCode(), 0))
		End Function

		Public Overrides Function numParams(ByVal backprop As Boolean) As Long
			Return layerConf.getLayer().initializer().numParams(layerConf)
		End Function

		Public Overrides Function minVertexInputs() As Integer
			Return 1
		End Function

		Public Overrides Function maxVertexInputs() As Integer
			Return 1
		End Function

		Public Overrides Function instantiate(ByVal graph As ComputationGraph, ByVal name As String, ByVal idx As Integer, ByVal paramsView As INDArray, ByVal initializeParams As Boolean, ByVal networkDatatype As DataType) As org.deeplearning4j.nn.graph.vertex.GraphVertex
			'Now, we need to work out if this vertex is an output vertex or not...
			Dim isOutput As Boolean = graph.Configuration.getNetworkOutputs().contains(name)

			Dim layer As org.deeplearning4j.nn.api.Layer = layerConf.getLayer().instantiate(layerConf, Nothing, idx, paramsView, initializeParams, networkDatatype)

			If layer Is Nothing Then
				Throw New System.InvalidOperationException("Encountered null layer during initialization for layer:" & layerConf.getLayer().GetType().Name & " initialization returned null layer?")
			End If

			Return New org.deeplearning4j.nn.graph.vertex.impl.LayerVertex(graph, name, idx, layer, preProcessor_Conflict, isOutput, networkDatatype)
		End Function

'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: @Override public org.deeplearning4j.nn.conf.inputs.InputType getOutputType(int layerIndex, org.deeplearning4j.nn.conf.inputs.InputType... vertexInputs) throws org.deeplearning4j.nn.conf.inputs.InvalidInputTypeException
		Public Overrides Function getOutputType(ByVal layerIndex As Integer, ParamArray ByVal vertexInputs() As InputType) As InputType
			If vertexInputs.Length <> 1 Then
				Throw New InvalidInputTypeException("LayerVertex expects exactly one input. Got: " & Arrays.toString(vertexInputs))
			End If

			'Assume any necessary preprocessors have already been added
			Dim afterPreprocessor As InputType
			If preProcessor_Conflict Is Nothing Then
				afterPreprocessor = vertexInputs(0)
			Else
				afterPreprocessor = preProcessor_Conflict.getOutputType(vertexInputs(0))
			End If

			Dim ret As InputType = layerConf.getLayer().getOutputType(layerIndex, afterPreprocessor)
			Return ret
		End Function

		Public Overrides Function getMemoryReport(ParamArray ByVal inputTypes() As InputType) As MemoryReport
			If inputTypes.Length <> 1 Then
				Throw New System.ArgumentException("Only one input supported for layer vertices: got " & Arrays.toString(inputTypes))
			End If
			Dim it As InputType
			If preProcessor_Conflict IsNot Nothing Then
				it = preProcessor_Conflict.getOutputType(inputTypes(0))
			Else
				it = inputTypes(0)
			End If
			'TODO preprocessor memory
			Return layerConf.getLayer().getMemoryReport(it)
		End Function

		Public Overrides WriteOnly Property DataType As DataType
			Set(ByVal dataType As DataType)
				layerConf.getLayer().setDataType(dataType)
			End Set
		End Property
	End Class

End Namespace
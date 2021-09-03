Imports System
Imports InputType = org.deeplearning4j.nn.conf.inputs.InputType
Imports InvalidInputTypeException = org.deeplearning4j.nn.conf.inputs.InvalidInputTypeException
Imports MemoryReport = org.deeplearning4j.nn.conf.memory.MemoryReport
Imports ComputationGraph = org.deeplearning4j.nn.graph.ComputationGraph
Imports DataType = org.nd4j.linalg.api.buffer.DataType
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports JsonTypeInfo = org.nd4j.shade.jackson.annotation.JsonTypeInfo

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
'ORIGINAL LINE: @JsonTypeInfo(use = JsonTypeInfo.Id.@CLASS, include = JsonTypeInfo.@As.@PROPERTY, property = "@class") public abstract class GraphVertex implements Cloneable, java.io.Serializable
	<Serializable>
	Public MustInherit Class GraphVertex
		Implements ICloneable

		Public MustOverride Overrides Function clone() As GraphVertex

		Public MustOverride Overrides Function Equals(ByVal o As Object) As Boolean

		Public MustOverride Overrides Function GetHashCode() As Integer

		Public MustOverride Function numParams(ByVal backprop As Boolean) As Long

		''' <returns> The Smallest valid number of inputs to this vertex </returns>
		Public MustOverride Function minVertexInputs() As Integer

		''' <returns> The largest valid number of inputs to this vertex </returns>
		Public MustOverride Function maxVertexInputs() As Integer

		''' <summary>
		''' Create a <seealso cref="org.deeplearning4j.nn.graph.vertex.GraphVertex"/> instance, for the given computation graph,
		''' given the configuration instance.
		''' </summary>
		''' <param name="graph">            The computation graph that this GraphVertex is to be part of </param>
		''' <param name="name">             The name of the GraphVertex object </param>
		''' <param name="idx">              The index of the GraphVertex </param>
		''' <param name="paramsView">       A view of the full parameters array </param>
		''' <param name="initializeParams"> If true: initialize the parameters. If false: make no change to the values in the paramsView array </param>
		''' <param name="networkDatatype"> </param>
		''' <returns> The implementation GraphVertex object (i.e., implementation, no the configuration) </returns>
		Public MustOverride Function instantiate(ByVal graph As ComputationGraph, ByVal name As String, ByVal idx As Integer, ByVal paramsView As INDArray, ByVal initializeParams As Boolean, ByVal networkDatatype As DataType) As org.deeplearning4j.nn.graph.vertex.GraphVertex

		''' <summary>
		''' Determine the type of output for this GraphVertex, given the specified inputs. Given that a GraphVertex may do arbitrary
		''' processing or modifications of the inputs, the output types can be quite different to the input type(s).<br>
		''' This is generally used to determine when to add preprocessors, as well as the input sizes etc for layers
		''' </summary>
		''' <param name="layerIndex"> The index of the layer (if appropriate/necessary). </param>
		''' <param name="vertexInputs"> The inputs to this vertex </param>
		''' <returns> The type of output for this vertex </returns>
		''' <exception cref="InvalidInputTypeException"> If the input type is invalid for this type of GraphVertex </exception>
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: public abstract org.deeplearning4j.nn.conf.inputs.InputType getOutputType(int layerIndex, org.deeplearning4j.nn.conf.inputs.InputType... vertexInputs) throws org.deeplearning4j.nn.conf.inputs.InvalidInputTypeException;
		Public MustOverride Function getOutputType(ByVal layerIndex As Integer, ParamArray ByVal vertexInputs() As InputType) As InputType

		''' <summary>
		''' This is a report of the estimated memory consumption for the given vertex
		''' </summary>
		''' <param name="inputTypes"> Input types to the vertex. Memory consumption is often a function of the input type </param>
		''' <returns> Memory report for the vertex </returns>
		Public MustOverride Function getMemoryReport(ParamArray ByVal inputTypes() As InputType) As MemoryReport


		Public Overridable WriteOnly Property DataType As DataType
			Set(ByVal dataType As DataType)
				'No-op for most layers
			End Set
		End Property

	End Class

End Namespace
Imports System.Collections.Generic
Imports ExecutorConfiguration = org.nd4j.autodiff.execution.conf.ExecutorConfiguration
Imports SameDiff = org.nd4j.autodiff.samediff.SameDiff
Imports SDVariable = org.nd4j.autodiff.samediff.SDVariable
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

Namespace org.nd4j.autodiff.execution


	Public Class BasicGraphExecutioner
		Implements GraphExecutioner

		''' <summary>
		''' This method returns Type of this executioner
		''' 
		''' @return
		''' </summary>
		Public Overridable ReadOnly Property ExecutionerType As Type Implements GraphExecutioner.getExecutionerType
			Get
				Return Type.LOCAL
			End Get
		End Property

		''' <summary>
		''' This method executes given graph and returns results
		''' </summary>
		''' <param name="graph">
		''' @return </param>
		Public Overridable Function executeGraph(ByVal graph As SameDiff, ByVal configuration As ExecutorConfiguration) As INDArray() Implements GraphExecutioner.executeGraph
	'        return new INDArray[]{graph.execAndEndResult()};
			Throw New System.NotSupportedException("Not yet reimplemented")
		End Function

		''' 
		''' <param name="diff">
		''' @return </param>
		Public Overridable Function convertToFlatBuffers(ByVal diff As SameDiff, ByVal configuration As ExecutorConfiguration) As ByteBuffer Implements GraphExecutioner.convertToFlatBuffers
			Throw New System.NotSupportedException()
		End Function


		''' <summary>
		''' This method executes given graph and returns results
		''' 
		''' PLEASE NOTE: Default configuration is used
		''' </summary>
		''' <param name="sd">
		''' @return </param>
		Public Overridable Function executeGraph(ByVal sd As SameDiff) As INDArray() Implements GraphExecutioner.executeGraph
			Return executeGraph(sd, New ExecutorConfiguration())
		End Function

		Public Overridable Function reuseGraph(ByVal graph As SameDiff, ByVal inputs As IDictionary(Of Integer, INDArray)) As INDArray() Implements GraphExecutioner.reuseGraph
			Throw New System.NotSupportedException()
		End Function

		''' <summary>
		''' This method executes
		''' </summary>
		''' <param name="id"> </param>
		''' <param name="variables">
		''' @return </param>
		Public Overridable Function executeGraph(ByVal id As Integer, ParamArray ByVal variables() As SDVariable) As INDArray() Implements GraphExecutioner.executeGraph
			' TODO: to be implemented
			Throw New System.NotSupportedException("Not implemented yet")
		End Function

		''' <summary>
		''' This method stores given graph for future execution
		''' </summary>
		''' <param name="graph">
		''' @return </param>
		Public Overridable Function registerGraph(ByVal graph As SameDiff) As Integer Implements GraphExecutioner.registerGraph
			' TODO: to be implemented
			Throw New System.NotSupportedException("Not implemented yet")
		End Function

		Public Overridable Function importProto(ByVal file As File) As INDArray() Implements GraphExecutioner.importProto
			' TODO: to be implemented
			Throw New System.NotSupportedException("Not implemented yet")
		End Function
	End Class

End Namespace
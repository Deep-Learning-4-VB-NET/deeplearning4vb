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


	Public Interface GraphExecutioner
		Friend Enum Type
			''' <summary>
			''' Executor runs on the same box
			''' </summary>
			LOCAL

			''' <summary>
			''' Executor runs somewhere else
			''' </summary>
			REMOTE
		End Enum

		''' <summary>
		''' This method returns Type of this executioner
		''' 
		''' @return
		''' </summary>
		ReadOnly Property ExecutionerType As Type

		''' <summary>
		''' This method executes given graph and returns results
		''' </summary>
		''' <param name="graph">
		''' @return </param>
		Function executeGraph(ByVal graph As SameDiff, ByVal configuration As ExecutorConfiguration) As INDArray()

		Function executeGraph(ByVal graph As SameDiff) As INDArray()

		Function reuseGraph(ByVal graph As SameDiff, ByVal inputs As IDictionary(Of Integer, INDArray)) As INDArray()

		''' <summary>
		''' This method converts given SameDiff instance to FlatBuffers representation
		''' </summary>
		''' <param name="diff">
		''' @return </param>
		Function convertToFlatBuffers(ByVal diff As SameDiff, ByVal configuration As ExecutorConfiguration) As ByteBuffer

		''' <summary>
		''' This method executes </summary>
		''' <param name="id"> </param>
		''' <param name="variables">
		''' @return </param>
		Function executeGraph(ByVal id As Integer, ParamArray ByVal variables() As SDVariable) As INDArray()


		''' <summary>
		''' This method stores given graph for future execution
		''' </summary>
		''' <param name="graph">
		''' @return </param>
		Function registerGraph(ByVal graph As SameDiff) As Integer

		''' <summary>
		''' This method executes TF graph
		''' 
		''' PLEASE NOTE: This feature is under development yet
		''' </summary>
		''' <param name="file">
		''' @return </param>
		Function importProto(ByVal file As File) As INDArray()


	End Interface

End Namespace
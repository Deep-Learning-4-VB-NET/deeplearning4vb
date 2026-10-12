Imports System.Collections.Generic
Imports DataBuffer = org.nd4j.linalg.api.buffer.DataBuffer
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

Namespace org.nd4j.linalg.api.ops.aggregates


	Public Interface Aggregate

		''' 
		''' <summary>
		''' @return
		''' </summary>
		Function name() As String

		''' 
		''' <summary>
		''' @return
		''' </summary>
		Function opNum() As Integer


		''' 
		''' <param name="result"> </param>
		Property FinalResult As Number


		''' 
		''' <summary>
		''' @return
		''' </summary>
		ReadOnly Property Arguments As IList(Of INDArray)

		''' 
		''' <summary>
		''' @return
		''' </summary>
		ReadOnly Property Shapes As IList(Of DataBuffer)

		''' 
		''' <summary>
		''' @return
		''' </summary>
		ReadOnly Property IndexingArguments As IList(Of Integer)

		''' 
		''' <summary>
		''' @return
		''' </summary>
		ReadOnly Property RealArguments As IList(Of Number)

		''' 
		''' <summary>
		''' @return
		''' </summary>
		ReadOnly Property IntArrayArguments As IList(Of Integer())

	'    
	'       Methods related to batch memory manipulations
	'     

		''' <summary>
		''' This method returns maximum number of shapes being passed per Aggregate
		''' 
		''' @return
		''' </summary>
		Function maxArguments() As Integer

		''' <summary>
		''' This method returns maximum number of shapes being passed per Aggregate
		''' 
		''' @return
		''' </summary>
		Function maxShapes() As Integer

		''' <summary>
		''' This method returns maximum number of IntArrays being passed per Aggregate
		''' 
		''' @return
		''' </summary>
		Function maxIntArrays() As Integer

		''' <summary>
		''' This method returns maximum length for IntArrays, if any
		''' 
		''' @return
		''' </summary>
		Function maxIntArraySize() As Integer

		''' <summary>
		''' This method returns maximum number of IndexArguments per Aggregate
		''' 
		''' @return
		''' </summary>
		Function maxIndexArguments() As Integer

		''' <summary>
		''' This method returns maximum number of real (float/double) per Aggregate
		''' 
		''' @return
		''' </summary>
		Function maxRealArguments() As Integer

		''' <summary>
		''' This method returns amount of memory required for batch creation for this specific Aggregate
		''' @return
		''' </summary>
		ReadOnly Property RequiredBatchMemorySize As Long

		''' <summary>
		''' This method returns amount of shared memory required for this specific Aggregate.
		''' PLEASE NOTE: this method is especially important for
		''' CUDA backend. On CPU backend it might be ignored, depending on Aggregate.
		''' 
		''' @return
		''' </summary>
		ReadOnly Property SharedMemorySize As Integer

		''' <summary>
		''' This method returns desired number of threads per Aggregate instance
		''' PLEASE NOTE: this method is especially important for CUDA backend.
		''' On CPU backend it might be ignored, depending on Aggregate.
		''' 
		''' @return
		''' </summary>
		ReadOnly Property ThreadsPerInstance As Integer
	End Interface

End Namespace
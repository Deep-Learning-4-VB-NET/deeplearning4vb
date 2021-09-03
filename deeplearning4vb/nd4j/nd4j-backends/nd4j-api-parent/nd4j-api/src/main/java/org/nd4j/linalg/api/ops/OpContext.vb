Imports System.Collections.Generic
Imports Pointer = org.bytedeco.javacpp.Pointer
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

Namespace org.nd4j.linalg.api.ops


	Public Interface OpContext
		Inherits AutoCloseable

		''' <summary>
		''' This method sets integer arguments required for operation </summary>
		''' <param name="arguments"> </param>
		Property IArguments As Long()


		Function numIArguments() As Integer

		''' <summary>
		''' This method sets floating point arguments required for operation </summary>
		''' <param name="arguments"> </param>
		Property TArguments As Double()
		Function numTArguments() As Integer

		''' <summary>
		''' This method sets data type arguments required for operation </summary>
		''' <param name="arguments"> </param>
		Property DArguments As DataType()
		Function numDArguments() As Integer

		''' <summary>
		''' This method sets boolean arguments required for operation </summary>
		''' <param name="arguments"> </param>
		Property BArguments As Boolean()
		Function numBArguments() As Integer

		''' <summary>
		''' This method sets root-level seed for rng </summary>
		''' <param name="seed"> </param>
		Sub setRngStates(ByVal rootState As Long, ByVal nodeState As Long)

		''' <summary>
		''' This method returns RNG states, root first node second
		''' @return
		''' </summary>
		ReadOnly Property RngStates As Pair(Of Long, Long)

		''' <summary>
		''' This method adds INDArray as input argument for future op call
		''' </summary>
		''' <param name="index"> </param>
		''' <param name="array"> </param>
		Sub setInputArray(ByVal index As Integer, ByVal array As INDArray)

		''' <summary>
		''' This method sets provided arrays as input arrays </summary>
		''' <param name="arrays"> </param>
		Property InputArrays As IList(Of INDArray)

		''' <summary>
		''' This method sets provided arrays as input arrays </summary>
		''' <param name="arrays"> </param>
		WriteOnly Property InputArrays As INDArray()


		Function numInputArguments() As Integer

		Function getInputArray(ByVal idx As Integer) As INDArray

		''' <summary>
		''' This method adds INDArray as output for future op call </summary>
		''' <param name="index"> </param>
		''' <param name="array"> </param>
		Sub setOutputArray(ByVal index As Integer, ByVal array As INDArray)

		''' <summary>
		''' This method sets provided arrays as output arrays </summary>
		''' <param name="arrays"> </param>
		Property OutputArrays As IList(Of INDArray)

		''' <summary>
		''' This method sets provided arrays as output arrays </summary>
		''' <param name="arrays"> </param>
		WriteOnly Property OutputArrays As INDArray()


		Function getOutputArray(ByVal i As Integer) As INDArray

		Function numOutputArguments() As Integer

		''' <summary>
		''' This method returns pointer to context, to be used during native op execution
		''' @return
		''' </summary>
		Function contextPointer() As Pointer

		''' <summary>
		''' This method allows to set op as inplace </summary>
		''' <param name="reallyInplace"> </param>
		Sub markInplace(ByVal reallyInplace As Boolean)

		''' <summary>
		''' This method allows to enable/disable use of platform helpers within ops. I.e. mkldnn or cuDNN.
		''' PLEASE NOTE: default value is True
		''' </summary>
		''' <param name="reallyAllow"> </param>
		Sub allowHelpers(ByVal reallyAllow As Boolean)

		''' <summary>
		''' This methos allows to disape outputs validation via shape function </summary>
		''' <param name="reallyOverride"> </param>
		Sub shapeFunctionOverride(ByVal reallyOverride As Boolean)

		''' <summary>
		''' This method returns current execution mode for Context
		''' @return
		''' </summary>
		Property ExecutionMode As ExecutionMode


		''' <summary>
		''' This method removes all in/out arrays from this OpContext
		''' </summary>
		Sub purge()
	End Interface

End Namespace
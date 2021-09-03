Imports System.Collections.Generic
Imports DataType = org.nd4j.linalg.api.buffer.DataType
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports LongShapeDescriptor = org.nd4j.linalg.api.shape.LongShapeDescriptor

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


	Public Interface CustomOp
		''' <summary>
		''' This method returns op opName as string
		''' @return
		''' </summary>
		Function opName() As String

		''' <summary>
		''' This method returns LongHash of the opName()
		''' @return
		''' </summary>
		Function opHash() As Long

		''' <summary>
		''' This method returns true if op is supposed to be executed inplace
		''' @return
		''' </summary>
		ReadOnly Property InplaceCall As Boolean

		Function outputArguments() As IList(Of INDArray)

		Function inputArguments() As IList(Of INDArray)

		Function iArgs() As Long()

		Function tArgs() As Double()

		Function bArgs() As Boolean()

		Function dArgs() As DataType()

		Sub addTArgument(ParamArray ByVal arg() As Double)

		Function sArgs() As String()

		Sub addIArgument(ParamArray ByVal arg() As Integer)

		Sub addIArgument(ParamArray ByVal arg() As Long)

		Sub addBArgument(ParamArray ByVal arg() As Boolean)

		Sub addDArgument(ParamArray ByVal arg() As DataType)

		Sub removeIArgument(ByVal arg As Integer?)

		Sub addSArgument(ParamArray ByVal args() As String)

		Sub removeSArgument(ByVal argument As String)

		Function getSArgument(ByVal index As Integer) As String

		Function getBArgument(ByVal index As Integer) As Boolean?

		Function getIArgument(ByVal index As Integer) As Long?

		Function numIArguments() As Integer

		Sub removeTArgument(ByVal arg As Double?)

		Function getTArgument(ByVal index As Integer) As Double?

		Function numTArguments() As Integer

		Function numBArguments() As Integer

		Function numDArguments() As Integer

		Function numSArguments() As Integer

		Sub addInputArgument(ParamArray ByVal arg() As INDArray)

		Sub removeInputArgument(ByVal arg As INDArray)

		Function getInputArgument(ByVal index As Integer) As INDArray

		Function numInputArguments() As Integer


		Sub addOutputArgument(ParamArray ByVal arg() As INDArray)

		Sub removeOutputArgument(ByVal arg As INDArray)

		Function getOutputArgument(ByVal index As Integer) As INDArray

		Function numOutputArguments() As Integer


		''' <summary>
		''' Calculate the output shape for this op </summary>
		''' <returns> Output array shapes </returns>
		Function calculateOutputShape() As IList(Of LongShapeDescriptor)

		''' <summary>
		''' Calculate the output shape for this op </summary>
		''' <returns> Output array shapes </returns>
		Function calculateOutputShape(ByVal opContext As OpContext) As IList(Of LongShapeDescriptor)

		''' <summary>
		''' Get the custom op descriptor if one is available.
		''' @return
		''' </summary>
		ReadOnly Property Descriptor As CustomOpDescriptor

		''' <summary>
		''' Asserts a valid state for execution,
		''' otherwise throws an <seealso cref="org.nd4j.linalg.exception.ND4JIllegalStateException"/>
		''' </summary>
		Sub assertValidForExecution()

		''' <summary>
		''' Clear the input and output INDArrays, if any are set
		''' </summary>
		Sub clearArrays()
	End Interface

End Namespace
Imports System
Imports System.Collections.Generic
Imports Builder = lombok.Builder
Imports Data = lombok.Data

'
' *  ******************************************************************************
' *  *
' *  *
' *  * This program and the accompanying materials are made available under the
' *  * terms of the Apache License, Version 2.0 which is available at
' *  * https://www.apache.org/licenses/LICENSE-2.0.
' *  *
' *  * See the NOTICE file distributed with this work for additional
' *  * information regarding copyright ownership.
' *  * Unless required by applicable law or agreed to in writing, software
' *  * distributed under the License is distributed on an "AS IS" BASIS, WITHOUT
' *  * WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied. See the
' *  * License for the specific language governing permissions and limitations
' *  * under the License.
' *  *
' *  * SPDX-License-Identifier: Apache-2.0
' *  *****************************************************************************
' 

Namespace org.nd4j.descriptor


	''' <summary>
	''' The op descriptor for the libnd4j code base.
	''' Each op represents a serialized version of
	''' <seealso cref="org.nd4j.linalg.api.ops.DynamicCustomOp"/>
	''' that has naming metadata attached.
	''' 
	''' @author Adam Gibson
	''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Data @Builder(toBuilder = true) public class OpDeclarationDescriptor implements java.io.Serializable
	<Serializable>
	Public Class OpDeclarationDescriptor
		Private name As String
		Private nIn, nOut, tArgs, iArgs As Integer
		Private inplaceAble As Boolean
		Private inArgNames As IList(Of String)
		Private outArgNames As IList(Of String)
		Private tArgNames As IList(Of String)
		Private iArgNames As IList(Of String)
		Private bArgNames As IList(Of String)


		Private opDeclarationType As OpDeclarationType
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Builder.@Default private java.util.Map<String,Boolean> argOptional = new java.util.HashMap<>();
		Private argOptional As IDictionary(Of String, Boolean) = New Dictionary(Of String, Boolean)()


		Public Enum OpDeclarationType
			CUSTOM_OP_IMPL
			BOOLEAN_OP_IMPL
			LIST_OP_IMPL
			LOGIC_OP_IMPL
			OP_IMPL
			DIVERGENT_OP_IMPL
			CONFIGURABLE_OP_IMPL
			REDUCTION_OP_IMPL
			BROADCASTABLE_OP_IMPL
			BROADCASTABLE_BOOL_OP_IMPL
			LEGACY_XYZ
			PLATFORM_IMPL
		End Enum



		Public Overridable Sub validate()
			If nIn >= 0 AndAlso nIn <> inArgNames.Count AndAlso Not VariableInputSize Then
				Console.Error.WriteLine("In arg names was not equal to number of inputs found for op " & name)
			End If

			If nOut >= 0 AndAlso nOut <> outArgNames.Count AndAlso Not VariableOutputSize Then
				Console.Error.WriteLine("Output arg names was not equal to number of outputs found for op " & name)
			End If

			If tArgs >= 0 AndAlso tArgs <> tArgNames.Count AndAlso Not VariableTArgs Then
				Console.Error.WriteLine("T arg names was not equal to number of T found for op " & name)
			End If
			If iArgs >= 0 AndAlso iArgs <> iArgNames.Count AndAlso Not VariableIntArgs Then
				Console.Error.WriteLine("Integer arg names was not equal to number of integer args found for op " & name)
			End If
		End Sub


		''' <summary>
		''' Returns true if there is a variable number
		''' of integer arguments for an op
		''' @return
		''' </summary>
		Public Overridable ReadOnly Property VariableIntArgs As Boolean
			Get
				Return iArgs < 0
			End Get
		End Property

		''' <summary>
		''' Returns true if there is a variable
		''' number of t arguments for an op
		''' @return
		''' </summary>
		Public Overridable ReadOnly Property VariableTArgs As Boolean
			Get
				Return tArgs < 0
			End Get
		End Property

		''' <summary>
		''' Returns true if the number of outputs is variable size
		''' @return
		''' </summary>
		Public Overridable ReadOnly Property VariableOutputSize As Boolean
			Get
				Return nOut < 0
			End Get
		End Property

		''' <summary>
		''' Returns true if the number of
		''' inputs is variable size
		''' @return
		''' </summary>
		Public Overridable ReadOnly Property VariableInputSize As Boolean
			Get
				Return nIn < 0
			End Get
		End Property


	End Class

End Namespace
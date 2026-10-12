Imports System.Collections.Generic
Imports Getter = lombok.Getter
Imports NonNull = lombok.NonNull
Imports Setter = lombok.Setter
Imports val = lombok.val
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

Namespace org.nd4j.linalg.api.ops


	Public MustInherit Class BaseOpContext
		Implements OpContext

		Public MustOverride Property ExecutionMode Implements OpContext.setExecutionMode As ExecutionMode
		Public MustOverride Sub shapeFunctionOverride(ByVal reallyOverride As Boolean)
		Public MustOverride Sub allowHelpers(ByVal reallyAllow As Boolean)
		Public MustOverride Sub markInplace(ByVal reallyInplace As Boolean)
		Public MustOverride Function contextPointer() As org.bytedeco.javacpp.Pointer Implements OpContext.contextPointer
		Public MustOverride ReadOnly Property RngStates As org.nd4j.common.primitives.Pair(Of Long, Long) Implements OpContext.getRngStates
		Public MustOverride Sub setRngStates(ByVal rootState As Long, ByVal nodeState As Long)
		Protected Friend fastpath_in As IDictionary(Of Integer, INDArray) = New Dictionary(Of Integer, INDArray)()
		Protected Friend fastpath_out As IDictionary(Of Integer, INDArray) = New Dictionary(Of Integer, INDArray)()

		Protected Friend fastpath_t As IList(Of Double) = New List(Of Double)()
		Protected Friend fastpath_b As IList(Of Boolean) = New List(Of Boolean)()
		Protected Friend fastpath_i As IList(Of Long) = New List(Of Long)()
		Protected Friend fastpath_d As IList(Of DataType) = New List(Of DataType)()

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Setter() @Getter protected ExecutionMode executionMode = ExecutionMode.UNDEFINED;
		Protected Friend executionMode As ExecutionMode = ExecutionMode.UNDEFINED

		Public Overridable Property IArguments Implements OpContext.setIArguments As Long()
			Set(ByVal arguments() As Long)
				fastpath_i.Clear()
				For Each v As val In arguments
					fastpath_i.Add(v)
				Next v
			End Set
			Get
				Return fastpath_i
			End Get
		End Property


		Public Overridable Function numIArguments() As Integer Implements OpContext.numIArguments
			Return fastpath_i.Count
		End Function

		Public Overridable Property TArguments Implements OpContext.setTArguments As Double()
			Set(ByVal arguments() As Double)
				fastpath_t.Clear()
				For Each v As val In arguments
					fastpath_t.Add(v)
				Next v
			End Set
			Get
				Return fastpath_t
			End Get
		End Property


		Public Overridable Function numTArguments() As Integer Implements OpContext.numTArguments
			Return fastpath_t.Count
		End Function

		Public Overridable Property BArguments Implements OpContext.setBArguments As Boolean()
			Set(ByVal arguments() As Boolean)
				fastpath_b.Clear()
				For Each v As val In arguments
					fastpath_b.Add(v)
				Next v
			End Set
			Get
				Return fastpath_b
			End Get
		End Property


		Public Overridable Function numBArguments() As Integer Implements OpContext.numBArguments
			Return fastpath_b.Count
		End Function

		Public Overridable Property DArguments Implements OpContext.setDArguments As DataType()
			Set(ByVal arguments() As DataType)
				fastpath_d.Clear()
				For Each v As val In arguments
					fastpath_d.Add(v)
				Next v
			End Set
			Get
				Return fastpath_d
			End Get
		End Property


		Public Overridable Function numDArguments() As Integer Implements OpContext.numDArguments
			Return fastpath_d.Count
		End Function

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Override public void setInputArray(int index, @NonNull INDArray array)
		Public Overridable Sub setInputArray(ByVal index As Integer, ByVal array As INDArray)
			fastpath_in(index) = array
		End Sub

		Public Overridable Property InputArrays As IList(Of INDArray)
			Get
				Dim result As val = New List(Of INDArray)()
				For e As Integer = 0 To Integer.MaxValue - 1
					Dim arr As val = fastpath_in(e)
					If arr IsNot Nothing Then
						result.add(arr)
					Else
						Exit For
					End If
				Next e
    
				Return result
			End Get
			Set(ByVal arrays As IList(Of INDArray))
				For e As Integer = 0 To arrays.Count - 1
					setInputArray(e, arrays(e))
				Next e
			End Set
		End Property

		Public Overridable Function numInputArguments() As Integer Implements OpContext.numInputArguments
			Return fastpath_in.Count
		End Function

		Public Overridable Function getInputArray(ByVal idx As Integer) As INDArray Implements OpContext.getInputArray
			Return fastpath_in(idx)
		End Function

		Public Overridable Property OutputArrays As IList(Of INDArray)
			Get
				Dim result As val = New List(Of INDArray)()
				For e As Integer = 0 To Integer.MaxValue - 1
					Dim arr As val = fastpath_out(e)
					If arr IsNot Nothing Then
						result.add(arr)
					Else
						Exit For
					End If
				Next e
    
				Return result
			End Get
			Set(ByVal arrays As IList(Of INDArray))
				For e As Integer = 0 To arrays.Count - 1
					setOutputArray(e, arrays(e))
				Next e
			End Set
		End Property

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Override public void setOutputArray(int index, @NonNull INDArray array)
		Public Overridable Sub setOutputArray(ByVal index As Integer, ByVal array As INDArray)
			fastpath_out(index) = array
		End Sub

		Public Overridable Function getOutputArray(ByVal i As Integer) As INDArray Implements OpContext.getOutputArray
			Return fastpath_out(i)
		End Function

		Public Overridable Function numOutputArguments() As Integer Implements OpContext.numOutputArguments
			Return fastpath_out.Count
		End Function



		Public Overridable WriteOnly Property InputArrays Implements OpContext.setInputArrays As INDArray()
			Set(ByVal arrays() As INDArray)
				For e As Integer = 0 To arrays.Length - 1
					setInputArray(e, arrays(e))
				Next e
			End Set
		End Property

		Public Overridable WriteOnly Property OutputArrays Implements OpContext.setOutputArrays As INDArray()
			Set(ByVal arrays() As INDArray)
				For e As Integer = 0 To arrays.Length - 1
					setOutputArray(e, arrays(e))
				Next e
			End Set
		End Property

		Public Overridable Sub purge() Implements OpContext.purge
			fastpath_in.Clear()
			fastpath_out.Clear()
		End Sub
	End Class

End Namespace
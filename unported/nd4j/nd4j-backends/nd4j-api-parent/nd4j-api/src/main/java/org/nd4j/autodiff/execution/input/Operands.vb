Imports System.Collections.Generic
Imports lombok
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

Namespace org.nd4j.autodiff.execution.input


	Public Class Operands
		Private map As IDictionary(Of NodeDescriptor, INDArray) = New LinkedHashMap(Of NodeDescriptor, INDArray)()

		''' <summary>
		''' This method allows to pass array to the node identified by its name
		''' </summary>
		''' <param name="id"> </param>
		''' <param name="array">
		''' @return </param>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public Operands addArgument(@NonNull String id, @NonNull INDArray array)
		Public Overridable Function addArgument(ByVal id As String, ByVal array As INDArray) As Operands
			map(NodeDescriptor.builder().name(id).build()) = array
			Return Me
		End Function

		''' <summary>
		''' This method allows to pass array to the node identified by numeric id
		''' </summary>
		''' <param name="id"> </param>
		''' <param name="array">
		''' @return </param>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public Operands addArgument(int id, @NonNull INDArray array)
		Public Overridable Function addArgument(ByVal id As Integer, ByVal array As INDArray) As Operands
			map(NodeDescriptor.builder().id(id).build()) = array
			Return Me
		End Function

		''' <summary>
		''' This method allows to pass array to multi-output node in the graph
		''' </summary>
		''' <param name="id"> </param>
		''' <param name="index"> </param>
		''' <param name="array">
		''' @return </param>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public Operands addArgument(int id, int index, @NonNull INDArray array)
		Public Overridable Function addArgument(ByVal id As Integer, ByVal index As Integer, ByVal array As INDArray) As Operands
			map(NodeDescriptor.builder().id(id).index(index).build()) = array
			Return Me
		End Function

		''' <summary>
		''' This method allows to pass array to multi-output node in the graph
		''' </summary>
		''' <param name="id"> </param>
		''' <param name="index"> </param>
		''' <param name="array">
		''' @return </param>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public Operands addArgument(String name, int id, int index, @NonNull INDArray array)
		Public Overridable Function addArgument(ByVal name As String, ByVal id As Integer, ByVal index As Integer, ByVal array As INDArray) As Operands
			map(NodeDescriptor.builder().name(name).id(id).index(index).build()) = array
			Return Me
		End Function

		''' <summary>
		''' This method returns array identified its name </summary>
		''' <param name="name">
		''' @return </param>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public org.nd4j.linalg.api.ndarray.INDArray getById(@NonNull String name)
		Public Overridable Function getById(ByVal name As String) As INDArray
			Return map(NodeDescriptor.builder().name(name).build())
		End Function

		''' <summary>
		''' This method returns array identified its numeric id </summary>
		''' <param name="name">
		''' @return </param>
		Public Overridable Function getById(ByVal id As Integer) As INDArray
			Return map(NodeDescriptor.builder().id(id).build())
		End Function

		''' <summary>
		''' This method returns array identified its numeric id and index </summary>
		''' <param name="name">
		''' @return </param>
		Public Overridable Function getById(ByVal id As Integer, ByVal index As Integer) As INDArray
			Return map(NodeDescriptor.builder().id(id).index(index).build())
		End Function

		''' <summary>
		''' This method return operands as array, in order of addition
		''' @return
		''' </summary>
		Public Overridable Function asArray() As INDArray()
			Dim val As val = map.Values
			Dim res As val = New INDArray(val.size() - 1){}
			Dim cnt As Integer = 0
			For Each v As val In val
'JAVA TO VB CONVERTER WARNING: An assignment within expression was extracted from the following statement:
'ORIGINAL LINE: res[cnt++] = v;
				res(cnt) = v
					cnt += 1
			Next v

			Return res
		End Function

		''' <summary>
		''' This method returns contents of this entity as collection of key->value pairs
		''' @return
		''' </summary>
		Public Overridable Function asCollection() As ICollection(Of Pair(Of NodeDescriptor, INDArray))
			Dim c As val = New HashSet(Of Pair(Of NodeDescriptor, INDArray))()
			For Each k As val In map.Keys
				c.add(Pair.makePair(k, map(k)))
			Next k

			Return c
		End Function

		''' <summary>
		''' This method returns number of values in this entity
		''' @return
		''' </summary>
		Public Overridable Function size() As Integer
			Return map.Count
		End Function

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @NoArgsConstructor @AllArgsConstructor @Builder @Data public static class NodeDescriptor
		Public Class NodeDescriptor
			Friend name As String
			Friend id As Integer
			Friend index As Integer
		End Class
	End Class

End Namespace
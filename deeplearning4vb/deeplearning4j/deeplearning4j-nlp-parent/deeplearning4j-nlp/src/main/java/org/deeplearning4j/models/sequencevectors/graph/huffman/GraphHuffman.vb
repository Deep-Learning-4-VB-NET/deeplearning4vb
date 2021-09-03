Imports System
Imports System.Collections.Generic
Imports System.Text
Imports AllArgsConstructor = lombok.AllArgsConstructor

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

Namespace org.deeplearning4j.models.sequencevectors.graph.huffman


	Public Class GraphHuffman
		Implements BinaryTree

		Private ReadOnly MAX_CODE_LENGTH As Integer
		Private ReadOnly codes() As Long
		Private ReadOnly codeLength() As SByte
		Private ReadOnly innerNodePathToLeaf()() As Integer

		''' <param name="nVertices"> number of vertices in the graph that this Huffman tree is being built for </param>
		Public Sub New(ByVal nVertices As Integer)
			Me.New(nVertices, 64)
		End Sub

		''' <param name="nVertices"> nVertices number of vertices in the graph that this Huffman tree is being built for </param>
		''' <param name="maxCodeLength"> MAX_CODE_LENGTH for Huffman tree </param>
		Public Sub New(ByVal nVertices As Integer, ByVal maxCodeLength As Integer)
			Me.codes = New Long(nVertices - 1){}
			Me.codeLength = New SByte(nVertices - 1){}
'JAVA TO VB CONVERTER NOTE: The following call to the 'RectangularArrays' helper class reproduces the rectangular array initialization that is automatic in Java:
'ORIGINAL LINE: this.innerNodePathToLeaf = new Integer[nVertices][0]
			Me.innerNodePathToLeaf = RectangularArrays.RectangularIntegerArray(nVertices, 0)
			Me.MAX_CODE_LENGTH = maxCodeLength
		End Sub

		''' <summary>
		''' Build the Huffman tree given an array of vertex degrees </summary>
		''' <param name="vertexDegree"> vertexDegree[i] = degree of ith vertex </param>
		Public Overridable Sub buildTree(ByVal vertexDegree() As Integer)
			Dim pq As New PriorityQueue(Of Node)()
			For i As Integer = 0 To vertexDegree.Length - 1
				pq.add(New Node(i, vertexDegree(i), Nothing, Nothing))
			Next i

			Do While pq.size() > 1
				Dim left As Node = pq.remove()
				Dim right As Node = pq.remove()
				Dim newNode As New Node(-1, left.count + right.count, left, right)
				pq.add(newNode)
			Loop

			'Eventually: only one node left -> full tree
			Dim tree As Node = pq.remove()

			'Now: convert tree into binary codes. Traverse tree (preorder traversal) -> record path (left/right) -> code
			Dim innerNodePath(MAX_CODE_LENGTH - 1) As Integer
			traverse(tree, 0L, CSByte(0), -1, innerNodePath, 0)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @AllArgsConstructor private static class Node implements Comparable<Node>
		Private Class Node
			Implements IComparable(Of Node)

			Friend ReadOnly vertexIdx As Integer
			Friend ReadOnly count As Long
			Friend left As Node
			Friend right As Node

			Public Overridable Function CompareTo(ByVal o As Node) As Integer Implements IComparable(Of Node).CompareTo
				Return Long.compare(count, o.count)
			End Function
		End Class

		Private Function traverse(ByVal node As Node, ByVal codeSoFar As Long, ByVal codeLengthSoFar As SByte, ByVal innerNodeCount As Integer, ByVal innerNodePath() As Integer, ByVal currDepth As Integer) As Integer
			If codeLengthSoFar >= MAX_CODE_LENGTH Then
				Throw New Exception("Cannot generate code: code length exceeds " & MAX_CODE_LENGTH & " bits")
			End If
			If node.left Is Nothing AndAlso node.right Is Nothing Then
				'Leaf node
				codes(node.vertexIdx) = codeSoFar
				codeLength(node.vertexIdx) = codeLengthSoFar
				innerNodePathToLeaf(node.vertexIdx) = Arrays.CopyOf(innerNodePath, currDepth)
				Return innerNodeCount
			End If

			'This is an inner node. It's index is 'innerNodeCount'
			innerNodeCount += 1
			innerNodePath(currDepth) = innerNodeCount

			Dim codeLeft As Long = setBit(codeSoFar, codeLengthSoFar, False)
			innerNodeCount = traverse(node.left, codeLeft, CSByte(codeLengthSoFar + 1), innerNodeCount, innerNodePath, currDepth + 1)

			Dim codeRight As Long = setBit(codeSoFar, codeLengthSoFar, True)
			innerNodeCount = traverse(node.right, codeRight, CSByte(codeLengthSoFar + 1), innerNodeCount, innerNodePath, currDepth + 1)
			Return innerNodeCount
		End Function

		Private Shared Function setBit(ByVal [in] As Long, ByVal bitNum As Integer, ByVal value As Boolean) As Long
			If value Then
				Return ([in] Or 1L << bitNum) 'Bit mask |: 00010000
			Else
				Return ([in] And Not (1 << bitNum)) 'Bit mask &: 11101111
			End If
		End Function

		Private Shared Function getBit(ByVal [in] As Long, ByVal bitNum As Integer) As Boolean
			Dim mask As Long = 1L << bitNum
			Return ([in] And mask) <> 0L
		End Function

		Public Overridable Function getCode(ByVal vertexNum As Integer) As Long Implements BinaryTree.getCode
			Return codes(vertexNum)
		End Function

		Public Overridable Function getCodeLength(ByVal vertexNum As Integer) As Integer Implements BinaryTree.getCodeLength
			Return codeLength(vertexNum)
		End Function

		Public Overridable Function getCodeString(ByVal vertexNum As Integer) As String Implements BinaryTree.getCodeString
			Dim code As Long = codes(vertexNum)
			Dim len As Integer = codeLength(vertexNum)
			Dim sb As New StringBuilder()
			For i As Integer = 0 To len - 1
				sb.Append(If(getBit(code, i), "1", "0"))
			Next i

			Return sb.ToString()
		End Function

		Public Overridable Function getCodeList(ByVal vertexNum As Integer) As IList(Of Integer)
			Dim result As IList(Of Integer) = New List(Of Integer)()
			Dim code As Long = codes(vertexNum)
			Dim len As Integer = codeLength(vertexNum)
			For i As Integer = 0 To len - 1
				result.Add(If(getBit(code, i), 1, 0))
			Next i
			Return result
		End Function

		Public Overridable Function getPathInnerNodes(ByVal vertexNum As Integer) As Integer() Implements BinaryTree.getPathInnerNodes
			Return innerNodePathToLeaf(vertexNum)
		End Function
	End Class

End Namespace
Imports System
Imports System.Collections.Generic
Imports System.Linq
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

Namespace org.deeplearning4j.nn.layers.feedforward.autoencoder.recursive



	<Serializable>
	Public Class Tree

'JAVA TO VB CONVERTER NOTE: The field vector was renamed since Visual Basic does not allow fields to have the same name as other class members:
		Private vector_Conflict As INDArray
'JAVA TO VB CONVERTER NOTE: The field prediction was renamed since Visual Basic does not allow fields to have the same name as other class members:
		Private prediction_Conflict As INDArray
'JAVA TO VB CONVERTER NOTE: The field children was renamed since Visual Basic does not allow fields to have the same name as other class members:
		Private children_Conflict As IList(Of Tree)
'JAVA TO VB CONVERTER NOTE: The field error was renamed since Visual Basic does not allow fields to have the same name as other class members:
		Private error_Conflict As Double
'JAVA TO VB CONVERTER NOTE: The field parent was renamed since Visual Basic does not allow fields to have the same name as other class members:
		Private parent_Conflict As Tree
'JAVA TO VB CONVERTER NOTE: The field headWord was renamed since Visual Basic does not allow fields to have the same name as other class members:
		Private headWord_Conflict As String
'JAVA TO VB CONVERTER NOTE: The field value was renamed since Visual Basic does not allow fields to have the same name as other class members:
		Private value_Conflict As String
'JAVA TO VB CONVERTER NOTE: The field label was renamed since Visual Basic does not allow fields to have the same name as other class members:
		Private label_Conflict As String
'JAVA TO VB CONVERTER NOTE: The field type was renamed since Visual Basic does not allow fields to have the same name as other class members:
		Private type_Conflict As String
'JAVA TO VB CONVERTER NOTE: The field goldLabel was renamed since Visual Basic does not allow fields to have the same name as other class members:
		Private goldLabel_Conflict As Integer
'JAVA TO VB CONVERTER NOTE: The field tokens was renamed since Visual Basic does not allow fields to have the same name as other class members:
		Private tokens_Conflict As IList(Of String)
'JAVA TO VB CONVERTER NOTE: The field tags was renamed since Visual Basic does not allow fields to have the same name as other class members:
		Private tags_Conflict As IList(Of String)
'JAVA TO VB CONVERTER NOTE: The field parse was renamed since Visual Basic does not allow fields to have the same name as other class members:
		Private parse_Conflict As String
'JAVA TO VB CONVERTER NOTE: The field begin was renamed since Visual Basic does not allow fields to have the same name as other class members:
'JAVA TO VB CONVERTER NOTE: The field end was renamed since Visual Basic does not allow fields to have the same name as other class members:
		Private begin_Conflict, end_Conflict As Integer

		''' <summary>
		''' Clone constructor (all but the children) </summary>
		''' <param name="tree"> the tree to clone </param>
		Public Sub New(ByVal tree As Tree)
			[Error] = tree.error_Conflict
			Value = tree.value_Conflict
			Vector = tree.vector_Conflict
			Parse = tree.parse_Conflict
			Label = tree.label_Conflict
			GoldLabel = tree.goldLabel_Conflict
			Prediction = tree.prediction_Conflict
			Tags = tree.tags_Conflict
			Begin = tree.begin_Conflict
			[End] = tree.end_Conflict
			Type = tree.type_Conflict
		End Sub

		Public Sub New(ByVal parent As Tree, ByVal tokens As IList(Of String))
			Me.parent_Conflict = parent
			Me.tokens_Conflict = tokens
			children_Conflict = New List(Of Tree)()
		End Sub

		Public Sub New(ByVal tokens As IList(Of String))
			children_Conflict = New List(Of Tree)()
			Me.tokens_Conflict = tokens
		End Sub

		''' <summary>
		''' The type of node; mainly extra meta data
		''' @return
		''' </summary>
		Public Overridable Function [getType]() As String
			Return type_Conflict
		End Function

		Public Overridable WriteOnly Property Type As String
			Set(ByVal type As String)
				Me.type_Conflict = type
			End Set
		End Property

		''' <summary>
		''' Returns all of the labels for this node and all of its children (recursively) </summary>
		''' <returns> all of the labels of this node and its children recursively </returns>
		Public Overridable Function yield() As IList(Of String)
			Return yield(New List(Of String)())

		End Function

		''' <summary>
		''' Returns the list of labels for this node and
		''' all of its children recursively </summary>
		''' <param name="labels"> the labels to add to </param>
		''' <returns> the list of labels for this node and
		''' all of its children recursively </returns>
		Private Function yield(ByVal labels As IList(Of String)) As IList(Of String)
			labels.Add(label_Conflict)
			For Each t As Tree In children()
				CType(labels, List(Of String)).AddRange(t.yield())
			Next t
			Return labels
		End Function


		Public Overridable WriteOnly Property GoldLabel As Integer
			Set(ByVal goldLabel As Integer)
				Me.goldLabel_Conflict = goldLabel
			End Set
		End Property

		Public Overridable Function goldLabel() As Integer
			Return goldLabel_Conflict
		End Function

		Public Overridable WriteOnly Property Label As String
			Set(ByVal label As String)
				Me.label_Conflict = label
			End Set
		End Property

		Public Overridable Function label() As String
			Return label_Conflict
		End Function


		Public Overridable Function value() As String
			Return value_Conflict
		End Function


		Public Overridable WriteOnly Property Value As String
			Set(ByVal value As String)
				Me.value_Conflict = value
    
			End Set
		End Property


		''' <summary>
		''' Returns whether the node has any children or not </summary>
		''' <returns> whether the node has any children or not </returns>
		Public Overridable ReadOnly Property Leaf As Boolean
			Get
				Return children_Conflict Is Nothing OrElse children_Conflict.Count = 0
			End Get
		End Property

		Public Overridable Function children() As IList(Of Tree)
			If children_Conflict Is Nothing Then
				children_Conflict = New List(Of Tree)()
			End If

			Return children_Conflict
		End Function

		''' <summary>
		''' Node has one child that is a leaf </summary>
		''' <returns> whether the node has one child and the child is a leaf </returns>
		Public Overridable ReadOnly Property PreTerminal As Boolean
			Get
				If children_Conflict Is Nothing AndAlso label_Conflict IsNot Nothing AndAlso Not label_Conflict.Equals("TOP") Then
					children_Conflict = New List(Of Tree)()
				End If
				If children_Conflict IsNot Nothing AndAlso children_Conflict.Count = 1 Then
					Dim child As Tree = children(0)
					Return child IsNot Nothing AndAlso child.Leaf
				End If
				Return False
			End Get
		End Property


		Public Overridable Function firstChild() As Tree
			Return If(children_Conflict.Count = 0, Nothing, children(0))
		End Function

		Public Overridable Function lastChild() As Tree
			Return If(children_Conflict.Count = 0, Nothing, children(children_Conflict.Count - 1))
		End Function

		''' <summary>
		''' Finds the channels of the tree.  The channels is defined as the length
		''' of the longest path from this node to a leaf node.  Leaf nodes
		''' have channels zero.  POS tags have channels 1. Phrasal nodes have
		''' channels &gt;= 2.
		''' </summary>
		''' <returns> the channels </returns>
		Public Overridable Function depth() As Integer
			If Leaf Then
				Return 0
			End If
			Dim maxDepth As Integer = 0
			Dim kids As IList(Of Tree) = children()
			For Each kid As Tree In kids
				Dim curDepth As Integer = kid.depth()
				If curDepth > maxDepth Then
					maxDepth = curDepth
				End If
			Next kid
			Return maxDepth + 1
		End Function

		''' <summary>
		''' Returns the distance between this node
		''' and the specified subnode </summary>
		''' <param name="node"> the node to get the distance from </param>
		''' <returns> the distance between the 2 nodes </returns>
		Public Overridable Function depth(ByVal node As Tree) As Integer
			Dim p As Tree = node.parent(Me)
			If Me Is node Then
				Return 0
			End If
			If p Is Nothing Then
				Return -1
			End If
'JAVA TO VB CONVERTER NOTE: The local variable depth was renamed since Visual Basic will not allow local variables with the same name as their enclosing function or property:
			Dim depth_Conflict As Integer = 1
			Do While Me IsNot p
				p = p.parent(Me)
				depth_Conflict += 1
			Loop
			Return depth_Conflict
		End Function

		''' <summary>
		''' Returns the parent of the passed in tree via traversal </summary>
		''' <param name="root"> the root node </param>
		''' <returns> the tree to traverse </returns>
		Public Overridable Function parent(ByVal root As Tree) As Tree
			Dim kids As IList(Of Tree) = root.children()
			Return traverse(root, kids, Me)
		End Function


		'traverses the tree by recursion
		Private Shared Function traverse(ByVal parent As Tree, ByVal kids As IList(Of Tree), ByVal node As Tree) As Tree
			For Each kid As Tree In kids
				If kid Is node Then
					Return parent
				End If

				Dim ret As Tree = node.parent(kid)
				If ret IsNot Nothing Then
					Return ret
				End If
			Next kid
			Return Nothing
		End Function

		''' <summary>
		''' Returns the ancestor of the given tree </summary>
		''' <param name="height"> </param>
		''' <param name="root"> </param>
		''' <returns> <seealso cref="Tree"/> </returns>
		Public Overridable Function ancestor(ByVal height As Integer, ByVal root As Tree) As Tree
			If height < 0 Then
				Throw New System.ArgumentException("ancestor: height cannot be negative")
			End If
			If height = 0 Then
				Return Me
			End If
			Dim par As Tree = parent(root)
			If par Is Nothing Then
				Return Nothing
			End If
			Return par.ancestor(height - 1, root)
		End Function


		''' <summary>
		''' Returns the total prediction error for this
		''' tree and its children </summary>
		''' <returns> the total error for this tree and its children </returns>
		Public Overridable Function errorSum() As Double
			If Leaf Then
				Return 0.0
			ElseIf PreTerminal Then
				Return [error]()
			Else
				Dim [error] As Double = 0.0
				For Each child As Tree In children()
					[error] += child.errorSum()
				Next child
				Return Me.error() + [error]
			End If
		End Function


		''' <summary>
		''' Gets the leaves of the tree.  All leaves nodes are returned as a list
		''' ordered by the natural left to right order of the tree.  Null values,
		''' if any, are inserted into the list like any other value.
		''' </summary>
		''' <returns> a <code>List</code> of the leaves. </returns>
		Public Overridable Function getLeaves(Of T As Tree)() As IList(Of T)
			Return getLeaves(New List(Of T)())
		End Function

		''' <summary>
		''' Gets the leaves of the tree.
		''' </summary>
		''' <param name="list"> The list in which the leaves of the tree will be
		'''             placed. Normally, this will be empty when the routine is called,
		'''             but if not, the new yield is added to the end of the list. </param>
		''' <returns> a <code>List</code> of the leaves. </returns>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @SuppressWarnings("unchecked") public <T extends Tree> java.util.List<T> getLeaves(java.util.List<T> list)
		Public Overridable Function getLeaves(Of T As Tree)(ByVal list As IList(Of T)) As IList(Of T)
			If Leaf Then
				list.Add(CType(Me, T))
			Else
				For Each kid As Tree In children()
					kid.getLeaves(list)
				Next kid
			End If
			Return list
		End Function

		Public Overrides Function clone() As Tree
			Dim ret As New Tree(Me)
			ret.connect(New List(Of Tree)(children()))
			Return ret
		End Function


		''' <summary>
		''' Returns the prediction error for this node </summary>
		''' <returns> the prediction error for this node </returns>
		Public Overridable Function [error]() As Double
			Return error_Conflict
		End Function

		Public Overridable WriteOnly Property Error As Double
			Set(ByVal [error] As Double)
				Me.error_Conflict = [error]
			End Set
		End Property

		Public Overridable Property Tokens As IList(Of String)
			Get
				Return tokens_Conflict
			End Get
			Set(ByVal tokens As IList(Of String))
				Me.tokens_Conflict = tokens
			End Set
		End Property


		Public Overridable WriteOnly Property Parent As Tree
			Set(ByVal parent As Tree)
				Me.parent_Conflict = parent
			End Set
		End Property

		Public Overridable Function parent() As Tree
			Return parent_Conflict
		End Function

		Public Overridable Function vector() As INDArray
			Return vector_Conflict
		End Function

		Public Overridable WriteOnly Property Vector As INDArray
			Set(ByVal vector As INDArray)
				Me.vector_Conflict = vector
			End Set
		End Property

		Public Overridable Function prediction() As INDArray
			Return prediction_Conflict
		End Function

		Public Overridable WriteOnly Property Prediction As INDArray
			Set(ByVal prediction As INDArray)
				Me.prediction_Conflict = prediction
			End Set
		End Property

		Public Overridable Function tags() As IList(Of String)
			Return tags_Conflict
		End Function

		Public Overridable WriteOnly Property Tags As IList(Of String)
			Set(ByVal tags As IList(Of String))
				Me.tags_Conflict = tags
			End Set
		End Property

		Public Overridable ReadOnly Property Children As IList(Of Tree)
			Get
				Return children_Conflict
			End Get
		End Property

		Public Overridable Property HeadWord As String
			Get
				Return headWord_Conflict
			End Get
			Set(ByVal headWord As String)
				Me.headWord_Conflict = headWord
			End Set
		End Property


		Public Overridable WriteOnly Property Parse As String
			Set(ByVal parse As String)
				Me.parse_Conflict = parse
			End Set
		End Property

		''' <summary>
		''' Connects the given trees
		''' and sets the parents of the children </summary>
		''' <param name="children">  the children to connect with </param>
		Public Overridable Sub connect(ByVal children As IList(Of Tree))
			Me.children_Conflict = children
			For Each t As Tree In children
				t.Parent = Me
			Next t
		End Sub

		Public Overridable Property Begin As Integer
			Get
				Return begin_Conflict
			End Get
			Set(ByVal begin As Integer)
				Me.begin_Conflict = begin
			End Set
		End Property


		Public Overridable Property End As Integer
			Get
				Return end_Conflict
			End Get
			Set(ByVal [end] As Integer)
				Me.end_Conflict = [end]
			End Set
		End Property


		Public Overrides Function ToString() As String
			Return "Tree{" & "error=" & error_Conflict & ", parent=" & parent_Conflict & ", headWord='" & headWord_Conflict & "'"c & ", value='" & value_Conflict & "'"c & ", label='" & label_Conflict & "'"c & ", type='" & type_Conflict & "'"c & ", goldLabel=" & goldLabel_Conflict & ", parse='" & parse_Conflict & "'"c & ", begin=" & begin_Conflict & ", end=" & end_Conflict & "}"c
		End Function

		Public Overrides Function Equals(ByVal o As Object) As Boolean
			If Me Is o Then
				Return True
			End If
			If Not (TypeOf o Is Tree) Then
				Return False
			End If

			Dim tree As Tree = DirectCast(o, Tree)

			If begin_Conflict <> tree.begin_Conflict Then
				Return False
			End If
			If end_Conflict <> tree.end_Conflict Then
				Return False
			End If
			If tree.error_Conflict.CompareTo(error_Conflict) <> 0 Then
				Return False
			End If
			If goldLabel_Conflict <> tree.goldLabel_Conflict Then
				Return False
			End If
			If If(headWord_Conflict IsNot Nothing, Not headWord_Conflict.Equals(tree.headWord_Conflict), tree.headWord_Conflict IsNot Nothing) Then
				Return False
			End If
			If If(label_Conflict IsNot Nothing, Not label_Conflict.Equals(tree.label_Conflict), tree.label_Conflict IsNot Nothing) Then
				Return False
			End If
			If If(parse_Conflict IsNot Nothing, Not parse_Conflict.Equals(tree.parse_Conflict), tree.parse_Conflict IsNot Nothing) Then
				Return False
			End If
			If If(prediction_Conflict IsNot Nothing, Not prediction_Conflict.Equals(tree.prediction_Conflict), tree.prediction_Conflict IsNot Nothing) Then
				Return False
			End If
'JAVA TO VB CONVERTER WARNING: LINQ 'SequenceEqual' is not always identical to Java AbstractList 'equals':
'ORIGINAL LINE: if (tags != null ? !tags.equals(tree.tags) : tree.tags != null)
			If If(tags_Conflict IsNot Nothing, Not tags_Conflict.SequenceEqual(tree.tags_Conflict), tree.tags_Conflict IsNot Nothing) Then
				Return False
			End If
'JAVA TO VB CONVERTER WARNING: LINQ 'SequenceEqual' is not always identical to Java AbstractList 'equals':
'ORIGINAL LINE: if (tokens != null ? !tokens.equals(tree.tokens) : tree.tokens != null)
			If If(tokens_Conflict IsNot Nothing, Not tokens_Conflict.SequenceEqual(tree.tokens_Conflict), tree.tokens_Conflict IsNot Nothing) Then
				Return False
			End If
			If If(type_Conflict IsNot Nothing, Not type_Conflict.Equals(tree.type_Conflict), tree.type_Conflict IsNot Nothing) Then
				Return False
			End If
			If If(value_Conflict IsNot Nothing, Not value_Conflict.Equals(tree.value_Conflict), tree.value_Conflict IsNot Nothing) Then
				Return False
			End If
			Return Not (If(vector_Conflict IsNot Nothing, Not vector_Conflict.Equals(tree.vector_Conflict), tree.vector_Conflict IsNot Nothing))

		End Function

		Public Overrides Function GetHashCode() As Integer
			Dim result As Integer
			Dim temp As Long
			result = If(vector_Conflict IsNot Nothing, vector_Conflict.GetHashCode(), 0)
			result = 31 * result + (If(prediction_Conflict IsNot Nothing, prediction_Conflict.GetHashCode(), 0))
			temp = System.BitConverter.DoubleToInt64Bits(error_Conflict)
			result = 31 * result + CInt(temp Xor (CLng(CULng(temp) >> 32)))
			result = 31 * result + (If(headWord_Conflict IsNot Nothing, headWord_Conflict.GetHashCode(), 0))
			result = 31 * result + (If(value_Conflict IsNot Nothing, value_Conflict.GetHashCode(), 0))
			result = 31 * result + (If(label_Conflict IsNot Nothing, label_Conflict.GetHashCode(), 0))
			result = 31 * result + (If(type_Conflict IsNot Nothing, type_Conflict.GetHashCode(), 0))
			result = 31 * result + goldLabel_Conflict
			result = 31 * result + (If(tokens_Conflict IsNot Nothing, tokens_Conflict.GetHashCode(), 0))
			result = 31 * result + (If(tags_Conflict IsNot Nothing, tags_Conflict.GetHashCode(), 0))
			result = 31 * result + (If(parse_Conflict IsNot Nothing, parse_Conflict.GetHashCode(), 0))
			result = 31 * result + begin_Conflict
			result = 31 * result + end_Conflict
			Return result
		End Function
	End Class

End Namespace
Imports System
Imports System.Collections.Generic
Imports System.Text
Imports lombok
Imports Slf4j = lombok.extern.slf4j.Slf4j
Imports org.nd4j.common.primitives
Imports SerializationUtils = org.nd4j.common.util.SerializationUtils
Imports MeshBuildMode = org.nd4j.parameterserver.distributed.v2.enums.MeshBuildMode
Imports NodeStatus = org.nd4j.parameterserver.distributed.enums.NodeStatus

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

Namespace org.nd4j.parameterserver.distributed.v2.util



'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Slf4j public class MeshOrganizer implements java.io.Serializable
	<Serializable>
	Public Class MeshOrganizer
		Private Const serialVersionUID As Long = 1L

		Private buildMode As MeshBuildMode = MeshBuildMode.MESH

		' this value determines max number of direct downstream connections for any given node (affects root node as well)
		Public Const MAX_DOWNSTREAMS As Integer = 8

		' max distance from root
		Public Const MAX_DEPTH As Integer = 5

		' just shortcut to the root node of the tree
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Getter(AccessLevel.@PUBLIC) private Node rootNode = new Node(true);
		Private rootNode As New Node(True)

		' SortedSet, with sort by number of downstreams
		<NonSerialized>
		Private sortedNodes As IList(Of Node) = New List(Of Node)()

		' flattened map of the tree, ID -> Node
		<NonSerialized>
		Private nodeMap As IDictionary(Of String, Node) = New Dictionary(Of String, Node)()

		' this field is used
'JAVA TO VB CONVERTER NOTE: The field version was renamed since Visual Basic does not allow fields to have the same name as other class members:
		Private version_Conflict As Long = 0L

		Public Sub New()
			For e As Integer = 0 To MAX_DOWNSTREAMS - 1
				fillQueue.AddLast(rootNode)
			Next e
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Deprecated public MeshOrganizer(@NonNull MeshBuildMode mode)
		<Obsolete>
		Public Sub New(ByVal mode As MeshBuildMode)
			Me.New()
			Me.buildMode = mode
		End Sub

		' queue with future leafs
		<NonSerialized>
		Protected Friend fillQueue As LinkedList(Of Node) = New LinkedTransferQueue(Of Node)()

		Public Overridable ReadOnly Property Version As Long
			Get
				Return version_Conflict
			End Get
		End Property

		''' <summary>
		''' This method adds new node to the network
		''' 
		''' PLEASE NOTE: Default port 40123 is used </summary>
		''' <param name="ip"> </param>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public Node addNode(@NonNull String ip)
		Public Overridable Function addNode(ByVal ip As String) As Node
			Return addNode(ip, 40123)
		End Function

		''' <summary>
		''' This methods adds new node to the network
		''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public Node addNode(@NonNull String ip, @NonNull int port)
		Public Overridable Function addNode(ByVal ip As String, ByVal port As Integer) As Node
'JAVA TO VB CONVERTER NOTE: The variable node was renamed since it may cause conflicts with calls to static members of the user-defined type with this name:
			Dim node_Conflict As val = Node.builder().id(ip).port(port).upstream(Nothing).build()

			 Return Me.addNode(node_Conflict)
		End Function

		''' <summary>
		''' This method returns absolutely independent copy of this Mesh
		''' @return
		''' </summary>
		Public Overridable Function clone() As MeshOrganizer
			Dim b As val = SerializationUtils.toByteArray(Me)
			Return SerializationUtils.fromByteArray(b)
		End Function

		''' <summary>
		''' This method adds new node to the mesh
		''' </summary>
		''' <param name="node">
		''' @return </param>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public synchronized Node addNode(@NonNull Node node)
'JAVA TO VB CONVERTER NOTE: The parameter node was renamed since it may cause conflicts with calls to static members of the user-defined type with this name:
		Public Overridable Function addNode(ByVal node_Conflict As Node) As Node
			SyncLock Me
				version_Conflict += 1
        
				If buildMode = MeshBuildMode.MESH Then
					' :)
					Dim candidate As val = fillQueue.RemoveFirst()
        
					' adding node to the candidate
					candidate.addDownstreamNode(node_Conflict)
        
					' adding this node for future connections
					For e As Integer = 0 To MAX_DOWNSTREAMS - 1
						fillQueue.AddLast(node_Conflict)
					Next e
        
					sortedNodes.Add(node_Conflict)
					sortedNodes.Sort()
				Else
					rootNode.addDownstreamNode(node_Conflict)
				End If
        
				' after all we add this node to the flattened map, for future access
				nodeMap(node_Conflict.getId()) = node_Conflict
        
				Return node_Conflict
			End SyncLock
		End Function

		''' <summary>
		''' This method marks Node (specified by IP) as offline, and remaps its downstreams
		''' </summary>
		''' <param name="ip"> </param>
		''' <exception cref="NoSuchElementException"> </exception>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public void markNodeOffline(@NonNull String ip) throws NoSuchElementException
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Public Overridable Sub markNodeOffline(ByVal ip As String)
			markNodeOffline(getNodeById(ip))
		End Sub

		''' <summary>
		''' This method marks given Node as offline, remapping its downstreams </summary>
		''' <param name="node"> </param>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public void markNodeOffline(@NonNull Node node)
'JAVA TO VB CONVERTER NOTE: The parameter node was renamed since it may cause conflicts with calls to static members of the user-defined type with this name:
		Public Overridable Sub markNodeOffline(ByVal node_Conflict As Node)
			SyncLock node_Conflict
				node_Conflict.status(NodeStatus.OFFLINE)

				For Each n As val In node_Conflict.getDownstreamNodes()
					remapNode(n)
				Next n
			End SyncLock
		End Sub

		Public Overrides Function Equals(ByVal o As Object) As Boolean
			If Me Is o Then
				Return True
			End If
			If o Is Nothing OrElse Me.GetType() <> o.GetType() Then
				Return False
			End If
			Dim that As MeshOrganizer = DirectCast(o, MeshOrganizer)

			Dim bm As val = buildMode = that.buildMode
			Dim rn As val = Objects.equals(rootNode, that.rootNode)

			Return bm AndAlso rn
		End Function

		Public Overrides Function GetHashCode() As Integer
			Return Objects.hash(buildMode, rootNode)
		End Function

		''' <summary>
		''' This method reconnects given node to another node
		''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public void remapNode(@NonNull String ip)
		Public Overridable Sub remapNode(ByVal ip As String)
			remapNode(getNodeById(ip))
		End Sub

		''' <summary>
		''' This method reconnects given node to another node
		''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public void remapNodeAndDownstreams(@NonNull String ip)
		Public Overridable Sub remapNodeAndDownstreams(ByVal ip As String)
			remapNodeAndDownstreams(getNodeById(ip))
		End Sub

		''' <summary>
		''' This method remaps node and its downstreams somewhere </summary>
		''' <param name="node"> </param>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public synchronized void remapNodeAndDownstreams(@NonNull Node node)
'JAVA TO VB CONVERTER NOTE: The parameter node was renamed since it may cause conflicts with calls to static members of the user-defined type with this name:
		Public Overridable Sub remapNodeAndDownstreams(ByVal node_Conflict As Node)
			SyncLock Me
				version_Conflict += 1
				node_Conflict.UpstreamNode = Me.rootNode
        
				For Each n As val In node_Conflict.getDownstreamNodes()
					Me.rootNode.addDownstreamNode(n)
					node_Conflict.removeFromDownstreams(n)
				Next n
			End SyncLock
		End Sub

		''' <summary>
		''' This method reconnects given node to another node
		''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public synchronized void remapNode(@NonNull Node node)
'JAVA TO VB CONVERTER NOTE: The parameter node was renamed since it may cause conflicts with calls to static members of the user-defined type with this name:
		Public Overridable Sub remapNode(ByVal node_Conflict As Node)
			SyncLock Me
				version_Conflict += 1
        
				If buildMode = MeshBuildMode.MESH Then
					node_Conflict.UpstreamNode.removeFromDownstreams(node_Conflict)
        
					Dim m As Boolean = False
					For Each n As val In sortedNodes
						' we dont want to remap node to itself
						If Not Objects.equals(n, node_Conflict) AndAlso n.status().Equals(NodeStatus.ONLINE) Then
							n.addDownstreamNode(node_Conflict)
							m = True
							Exit For
						End If
					Next n
        
					' if we were unable to find good enough node - we'll map this node to the rootNode
					If Not m Then
						rootNode.addDownstreamNode(node_Conflict)
					End If
        
					' i hope we won't deadlock here? :)
					SyncLock Me
						sortedNodes.Sort()
					End SyncLock
				ElseIf buildMode = MeshBuildMode.PLAIN Then
					' nothing to do here
				End If
			End SyncLock
		End Sub

		''' <summary>
		''' This method removes  node from tree
		''' </summary>
		Public Overridable Sub removeNode()
			' TODO: implement this one
			Throw New System.NotSupportedException()
		End Sub


'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: private void readObject(java.io.ObjectInputStream ois) throws ClassNotFoundException, java.io.IOException
		Private Sub readObject(ByVal ois As ObjectInputStream)
			' default deserialization
			ois.defaultReadObject()

			Dim desc As val = rootNode.getDescendantNodes()

			nodeMap = New Dictionary(Of String, Node)()

			For Each d As val In desc
				nodeMap(d.getId()) = d
			Next d
		End Sub


		''' <summary>
		''' This method returns true, if node is known
		''' @return
		''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public boolean isKnownNode(@NonNull String id)
		Public Overridable Function isKnownNode(ByVal id As String) As Boolean
			If rootNode.getId() Is Nothing Then
				Return False
			End If

			If rootNode.getId().Equals(id) Then
				Return True
			End If

			Return nodeMap.ContainsKey(id)
		End Function

		''' <summary>
		''' This method returns upstream connection for a given node
		''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public Node getUpstreamForNode(@NonNull String ip) throws NoSuchElementException
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Public Overridable Function getUpstreamForNode(ByVal ip As String) As Node
'JAVA TO VB CONVERTER NOTE: The variable node was renamed since it may cause conflicts with calls to static members of the user-defined type with this name:
			Dim node_Conflict As val = getNodeById(ip)

			Return node_Conflict.getUpstreamNode()
		End Function

		''' <summary>
		''' This method returns downstream connections for a given node
		''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public Collection<Node> getDownstreamsForNode(@NonNull String ip) throws NoSuchElementException
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Public Overridable Function getDownstreamsForNode(ByVal ip As String) As ICollection(Of Node)
'JAVA TO VB CONVERTER NOTE: The variable node was renamed since it may cause conflicts with calls to static members of the user-defined type with this name:
			Dim node_Conflict As val = getNodeById(ip)

			Return node_Conflict.getDownstreamNodes()
		End Function

		''' <summary>
		''' This method returns total number of nodes below given one
		''' @return
		''' </summary>
		Public Overridable Function numberOfDescendantsOfNode() As Long
			Return rootNode.numberOfDescendants()
		End Function

		''' <summary>
		''' This method returns total number of nodes in this mesh
		''' 
		''' PLESE NOTE: this method INCLUDES root node
		''' @return
		''' </summary>
		Public Overridable Function totalNodes() As Long
			Return rootNode.numberOfDescendants() + 1
		End Function

		''' <summary>
		''' This method returns size of flattened map of nodes.
		''' Suited for tests.
		''' 
		''' @return
		''' </summary>
		Protected Friend Overridable Function flatSize() As Long
			Return CLng(nodeMap.Count)
		End Function

		''' <summary>
		''' This method returns our mesh as collection of nodes
		''' @return
		''' </summary>
		Public Overridable Function flatNodes() As ICollection(Of Node)
			Return nodeMap.Values
		End Function


		''' <summary>
		''' This method returns Node representing given IP
		''' @return
		''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public Node getNodeById(@NonNull String id) throws NoSuchElementException
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Public Overridable Function getNodeById(ByVal id As String) As Node
			If id.Equals(rootNode.getId()) Then
				Return rootNode
			End If

'JAVA TO VB CONVERTER NOTE: The variable node was renamed since it may cause conflicts with calls to static members of the user-defined type with this name:
			Dim node_Conflict As val = nodeMap(id)
			If node_Conflict Is Nothing Then
				log.info("Existing nodes: [{}]", Me.flatNodes())
				Throw New NoSuchElementException(id)
			End If

			Return node_Conflict
		End Function

		''' <summary>
		''' This class represents basic tree node
		''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @NoArgsConstructor @AllArgsConstructor @Builder public static class Node implements java.io.Serializable, Comparable<Node>
		<Serializable>
		Public Class Node
			Implements IComparable(Of Node)

			Friend Const serialVersionUID As Long = 1L

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Getter(AccessLevel.@PUBLIC) @Setter(AccessLevel.@PROTECTED) @Builder.@Default private boolean rootNode = false;
			Friend rootNode As Boolean = False

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Getter @Setter private String id;
			Friend id As String

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Getter @Setter private int port;
			Friend port As Integer

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Getter(AccessLevel.NONE) @Setter(AccessLevel.NONE) private Node upstream;
			Friend upstream As Node

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Getter(AccessLevel.NONE) @Setter(AccessLevel.NONE) private final List<Node> downstream = new java.util.concurrent.CopyOnWriteArrayList<>();
			Friend ReadOnly downstream As IList(Of Node) = New CopyOnWriteArrayList(Of Node)()

			Friend position As New AtomicInteger(0)

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Getter(AccessLevel.NONE) @Setter(AccessLevel.NONE) @Builder.@Default private org.nd4j.common.primitives.Atomic<org.nd4j.parameterserver.distributed.enums.NodeStatus> status = new org.nd4j.common.primitives.Atomic<>(org.nd4j.parameterserver.distributed.enums.NodeStatus.ONLINE);
'JAVA TO VB CONVERTER NOTE: The field status was renamed since Visual Basic does not allow fields to have the same name as other class members:
			Friend status_Conflict As New Atomic(Of NodeStatus)(NodeStatus.ONLINE)

			''' <summary>
			''' This method returns current status of this node
			''' @return
			''' </summary>
			Public Overridable Function status() As NodeStatus
				SyncLock Me
					Return status_Conflict.get()
				End SyncLock
			End Function

			''' <summary>
			''' This method ret </summary>
			''' <param name="status"> </param>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: protected synchronized void status(@NonNull NodeStatus status)
			Protected Friend Overridable Sub status(ByVal status As NodeStatus)
				SyncLock Me
					Me.status_Conflict.set(status)
				End SyncLock
			End Sub




			''' <summary>
			''' This method return candidate for new connection
			''' </summary>
			''' <param name="node">
			''' @return </param>
'JAVA TO VB CONVERTER NOTE: The parameter node was renamed since it may cause conflicts with calls to static members of the user-defined type with this name:
			Protected Friend Overridable Function getNextCandidate(ByVal node_Conflict As Node) As Node
				' if there's no candidates - just connect to this node
				If downstream.Count = 0 Then
					Return Me
				End If

				If node_Conflict Is Nothing Then
					Return downstream(0)
				End If

				' TODO: we can get rid of flat scan here, but it's one-off step anyway...

				' we return next node after this node
				Dim b As Boolean = False
				For Each v As val In downstream
					If b Then
						Return v
					End If

					If Objects.equals(node_Conflict, v) Then
						b = True
					End If
				Next v

				Return Nothing
			End Function

			Protected Friend Sub New(ByVal rootNode As Boolean)
				Me.rootNode = rootNode
			End Sub

			''' <summary>
			''' This method adds downstream node to the list of connections </summary>
			''' <param name="node">
			''' @return </param>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public Node addDownstreamNode(@NonNull Node node)
'JAVA TO VB CONVERTER NOTE: The parameter node was renamed since it may cause conflicts with calls to static members of the user-defined type with this name:
			Public Overridable Function addDownstreamNode(ByVal node_Conflict As Node) As Node
				Me.downstream.Add(node_Conflict)
				node_Conflict.UpstreamNode = Me
				Return node_Conflict
			End Function

			''' <summary>
			''' This method pushes node to the bottom of this node downstream </summary>
			''' <param name="node">
			''' @return </param>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: protected Node pushDownstreamNode(@NonNull Node node)
'JAVA TO VB CONVERTER NOTE: The parameter node was renamed since it may cause conflicts with calls to static members of the user-defined type with this name:
			Protected Friend Overridable Function pushDownstreamNode(ByVal node_Conflict As Node) As Node
				If isRootNode() Then
					If downstream.Count = 0 Then
						Return addDownstreamNode(node_Conflict)
					Else
						' we should find first not full sub-branch
						For Each d As val In downstream
							If d.numberOfDescendants() < MeshOrganizer.MAX_DEPTH * MeshOrganizer.MAX_DOWNSTREAMS Then
								Return d.pushDownstreamNode(node_Conflict)
							End If
						Next d

						 ' if we're here - we'll have to add new branch to the root
						Return addDownstreamNode(node_Conflict)
					End If
				Else
					Dim distance As val = distanceFromRoot()

					For Each d As val In downstream
						If d.numberOfDescendants() < MeshOrganizer.MAX_DOWNSTREAMS * (MeshOrganizer.MAX_DEPTH - distance) Then
							Return d.pushDownstreamNode(node_Conflict)
						End If
					Next d

					Return addDownstreamNode(node_Conflict)
				End If
			End Function

			''' <summary>
			''' This method allows to set master node for this node </summary>
			''' <param name="node">
			''' @return </param>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: protected Node setUpstreamNode(@NonNull Node node)
'JAVA TO VB CONVERTER NOTE: The parameter node was renamed since it may cause conflicts with calls to static members of the user-defined type with this name:
			Protected Friend Overridable Function setUpstreamNode(ByVal node_Conflict As Node) As Node
				Me.upstream = node_Conflict
				Return node_Conflict
			End Function

			''' <summary>
			''' This method returns the node this one it connected to
			''' @return
			''' </summary>
			Public Overridable ReadOnly Property UpstreamNode As Node
				Get
					Return upstream
				End Get
			End Property

			''' <summary>
			''' This method returns number of downstream nodes connected to this node
			''' @return
			''' </summary>
			Public Overridable Function numberOfDescendants() As Long
				Dim cnt As val = New AtomicLong(downstream.Count)

				For Each n As val In downstream
					cnt.addAndGet(n.numberOfDescendants())
				Next n

				Return cnt.get()
			End Function

			''' <summary>
			''' This method returns number of nodes that has direct connection for this node
			''' @return
			''' </summary>
			Public Overridable Function numberOfDownstreams() As Long
				Return downstream.Count
			End Function

			''' <summary>
			''' This method returns collection of nodes that have direct connection to this node
			''' @return
			''' </summary>
			Public Overridable ReadOnly Property DownstreamNodes As ICollection(Of Node)
				Get
					Return downstream
				End Get
			End Property

			''' <summary>
			''' This method returns all nodes
			''' @return
			''' </summary>
			Public Overridable ReadOnly Property DescendantNodes As ICollection(Of Node)
				Get
					Dim result As val = New List(Of Node)(getDownstreamNodes())
					For Each n As val In downstream
						result.addAll(n.getDescendantNodes())
					Next n
    
					Return result
				End Get
			End Property

			''' <summary>
			''' This method returns number of hops between
			''' @return
			''' </summary>
			Public Overridable Function distanceFromRoot() As Integer
				If upstream.isRootNode() Then
					Return 1
				Else
					Return upstream.distanceFromRoot() + 1
				End If
			End Function

			Public Overrides Function Equals(ByVal o As Object) As Boolean
				If Me Is o Then
					Return True
				End If
				If o Is Nothing OrElse Me.GetType() <> o.GetType() Then
					Return False
				End If
'JAVA TO VB CONVERTER NOTE: The variable node was renamed since it may cause conflicts with calls to static members of the user-defined type with this name:
				Dim node_Conflict As Node = DirectCast(o, Node)

				Dim rn As val = If(Me.upstream Is Nothing, "root", Me.upstream.getId())
				Dim [on] As val = If(node_Conflict.upstream Is Nothing, "root", node_Conflict.upstream.getId())

				Return rootNode = node_Conflict.rootNode AndAlso port = node_Conflict.port AndAlso Objects.equals(id, node_Conflict.id) AndAlso Objects.equals(downstream, node_Conflict.downstream) AndAlso Objects.equals(status_Conflict, node_Conflict.status_Conflict) AndAlso Objects.equals(rn, [on])

			End Function

			Public Overrides Function GetHashCode() As Integer
				Return Objects.hash(If(upstream Is Nothing, "root", upstream.getId()), rootNode, id, port, downstream, status_Conflict)
			End Function

			Public Overrides Function ToString() As String
				Dim builder As val = New StringBuilder()
				If downstream Is Nothing OrElse downstream.Count = 0 Then
					builder.append("none")
				Else
					For Each n As val In downstream
						builder.append("[").append(n.getId()).append("], ")
					Next n
				End If
				' downstreams: [" + builder.toString() +"]; ]
				' upstreamId: [" + upstreamId + "];
				Dim strId As val = If(id Is Nothing, "null", id)
				Dim upstreamId As val = If(upstream Is Nothing, "none", upstream.getId())
				Return "[ Id: [" & strId & "]; ]"
			End Function

			''' <summary>
			''' This method remove all downstreams for a given node
			''' </summary>
			Public Overridable Sub truncateDownstreams()
				downstream.Clear()
			End Sub

			''' <summary>
			''' This method removes </summary>
			''' <param name="node"> </param>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public synchronized void removeFromDownstreams(@NonNull Node node)
'JAVA TO VB CONVERTER NOTE: The parameter node was renamed since it may cause conflicts with calls to static members of the user-defined type with this name:
			Public Overridable Sub removeFromDownstreams(ByVal node_Conflict As Node)
				SyncLock Me
					Dim r As val = downstream.Remove(node_Conflict)
        
					If Not r Then
						Throw New NoSuchElementException(node_Conflict.getId())
					End If
				End SyncLock
			End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Override public int compareTo(@NonNull Node o)
			Public Overridable Function CompareTo(ByVal o As Node) As Integer Implements IComparable(Of Node).CompareTo
				Return Long.compare(Me.numberOfDownstreams(), o.numberOfDownstreams())
			End Function
		End Class
	End Class

End Namespace
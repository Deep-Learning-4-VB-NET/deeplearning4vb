Imports System.Collections.Generic
Imports System.IO
Imports Slf4j = lombok.extern.slf4j.Slf4j
Imports val = lombok.val
Imports Tag = org.junit.jupiter.api.Tag
Imports Test = org.junit.jupiter.api.Test
Imports Timeout = org.junit.jupiter.api.Timeout
Imports BaseND4JTest = org.nd4j.common.tests.BaseND4JTest
Imports NativeTag = org.nd4j.common.tests.tags.NativeTag
Imports TagNames = org.nd4j.common.tests.tags.TagNames
Imports SerializationUtils = org.nd4j.common.util.SerializationUtils
Imports MeshBuildMode = org.nd4j.parameterserver.distributed.v2.enums.MeshBuildMode
Imports org.junit.jupiter.api.Assertions

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
'ORIGINAL LINE: @Slf4j @Tag(TagNames.FILE_IO) @Tag(TagNames.DIST_SYSTEMS) @NativeTag public class MeshOrganizerTest extends org.nd4j.common.tests.BaseND4JTest
	Public Class MeshOrganizerTest
		Inherits BaseND4JTest

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test() @Timeout(1000L) public void testDescendantsCount_1()
		Public Overridable Sub testDescendantsCount_1()
			Dim node As val = MeshOrganizer.Node.builder().build()

			Dim eNode As val = MeshOrganizer.Node.builder().build()
			eNode.addDownstreamNode(MeshOrganizer.Node.builder().build())

			node.addDownstreamNode(MeshOrganizer.Node.builder().build())
			node.addDownstreamNode(eNode)
			node.addDownstreamNode(MeshOrganizer.Node.builder().build())

			assertEquals(4, node.numberOfDescendants())
			assertEquals(3, node.numberOfDownstreams())
			assertEquals(1, eNode.numberOfDownstreams())
			assertEquals(1, eNode.numberOfDescendants())
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testDistanceFromRoot_1()
		Public Overridable Sub testDistanceFromRoot_1()
			Dim rootNode As val = New MeshOrganizer.Node(True)

			Dim node0 As val = rootNode.addDownstreamNode(New MeshOrganizer.Node())
			Dim node1 As val = node0.addDownstreamNode(New MeshOrganizer.Node())

			assertEquals(2, node1.distanceFromRoot())

			Dim node2 As val = node1.addDownstreamNode(New MeshOrganizer.Node())

			assertEquals(3, node2.distanceFromRoot())
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testNextCandidate_1()
		Public Overridable Sub testNextCandidate_1()
			Dim rootNode As val = New MeshOrganizer.Node(True)

			Dim node0 As val = rootNode.addDownstreamNode(New MeshOrganizer.Node())
			Dim node1 As val = rootNode.addDownstreamNode(New MeshOrganizer.Node())
			Dim node2 As val = rootNode.addDownstreamNode(New MeshOrganizer.Node())

			Dim c1_0 As val = node1.getNextCandidate(Nothing)
			assertEquals(node1, c1_0)

			Dim nn As val = c1_0.addDownstreamNode(New MeshOrganizer.Node())
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testPushDownstream_1()
		Public Overridable Sub testPushDownstream_1()
			Dim rootNode As val = New MeshOrganizer.Node(True)

			Dim e As Integer = 0
			Do While e < MeshOrganizer.MAX_DOWNSTREAMS * MeshOrganizer.MAX_DEPTH * 2
				rootNode.pushDownstreamNode(New MeshOrganizer.Node())
				e += 1
			Loop

			assertEquals(2, rootNode.numberOfDownstreams())
			assertEquals(MeshOrganizer.MAX_DOWNSTREAMS * MeshOrganizer.MAX_DEPTH * 2, rootNode.numberOfDescendants())
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testBasicMesh_3()
		Public Overridable Sub testBasicMesh_3()
			Dim mesh As val = New MeshOrganizer(MeshBuildMode.MESH)

			Dim node1 As val = mesh.addNode("192.168.1.1")
			Dim node2 As val = mesh.addNode("192.168.2.1")
			Dim node3 As val = mesh.addNode("192.168.2.2")

			assertEquals(4, mesh.totalNodes())
			assertEquals(3, mesh.getRootNode().numberOfDownstreams())

			Dim node4 As val = mesh.addNode("192.168.2.3")
			Dim node5 As val = mesh.addNode("192.168.2.4")
			Dim node6 As val = mesh.addNode("192.168.2.5")

			assertEquals(0, node1.numberOfDownstreams())
			assertEquals(0, node4.numberOfDownstreams())
			assertEquals(0, node5.numberOfDownstreams())

			assertEquals(0, node2.numberOfDownstreams())
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testBasicMesh_4()
		Public Overridable Sub testBasicMesh_4()
			Dim mesh As val = New MeshOrganizer(MeshBuildMode.MESH)

			' smoke test
			For e As Integer = 0 To 8191
				mesh.addNode(System.Guid.randomUUID().ToString())
			Next e

			' 8192 nodes + root node
			assertEquals(8193, mesh.totalNodes())

			' and now we'll make sure there's no nodes with number of downstreams > MAX_DOWNSTREAMS
			For Each v As val In mesh.flatNodes()
				assertTrue(v.numberOfDownstreams() <= MeshOrganizer.MAX_DOWNSTREAMS)
				assertTrue(v.distanceFromRoot() <= MeshOrganizer.MAX_DEPTH + 1)
			Next v
		End Sub


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testRemap_1() throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Public Overridable Sub testRemap_1()
			Dim mesh As val = New MeshOrganizer(MeshBuildMode.MESH)

			For e As Integer = 0 To MeshOrganizer.MAX_DOWNSTREAMS - 1
				mesh.addNode(e.ToString())
			Next e


			mesh.markNodeOffline("3")

			assertEquals(MeshOrganizer.MAX_DOWNSTREAMS, mesh.getRootNode().numberOfDownstreams())

			Dim node4 As val = mesh.addNode("192.168.1.7")
			Dim node1 As val = mesh.getNodeById("0")

			assertEquals(1, node1.numberOfDownstreams())

			mesh.remapNode(node4)

			'assertEquals(1, node1.numberOfDownstreams());
			assertEquals(0, node1.numberOfDownstreams())
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testRemap_2() throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Public Overridable Sub testRemap_2()
			Dim mesh As val = New MeshOrganizer(MeshBuildMode.MESH)
			mesh.getRootNode().setId("ROOT_NODE")
			Dim nodes As val = New List(Of MeshOrganizer.Node)()

			For e As Integer = 0 To 8191
				Dim node As val = mesh.addNode(System.Guid.randomUUID().ToString())
				nodes.add(node)
			Next e

	'        for (val n:nodes)
	'            log.info("Number of downstreams: [{}]", n.numberOfDownstreams());

			log.info("Going for first clone")
			Dim clone1 As val = mesh.clone()
			assertEquals(mesh, clone1)

			Dim badNode As val = nodes.get(119)
			mesh.remapNode(badNode.getId())

			log.info("Going for second clone")
			Dim clone2 As val = mesh.clone()
			assertEquals(mesh, clone2)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testRemap_3() throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Public Overridable Sub testRemap_3()
			Dim mesh As val = New MeshOrganizer(MeshBuildMode.MESH)
			mesh.getRootNode().setId("ROOT_NODE")
			Dim nodes As val = New List(Of MeshOrganizer.Node)()

			For e As Integer = 0 To 511
				Dim node As val = mesh.addNode(e.ToString())
				nodes.add(node)
			Next e

			Dim node As val = nodes.get(8)
			assertNotNull(node.getUpstreamNode())
			assertEquals(MeshOrganizer.MAX_DOWNSTREAMS, node.getDownstreamNodes().size())

			log.info("Node ID: {}; Upstream ID: {}; Downstreams: {}", node.getId(), node.getUpstreamNode().getId(), node.getDownstreamNodes())

			' saving current downstream IDs for later check
			Dim ids As val = New List(Of String)()
			node.getDownstreamNodes().forEach(Sub(n) ids.add(n.getId()))


			' reconnecting
			mesh.remapNodeAndDownstreams(node)

			' failed node gets connected to the root node
			assertEquals(mesh.getRootNode(), node.getUpstreamNode())

			' and its downstreams are remapped across the mesh
			log.info("Node ID: {}; Upstream ID: {}; Downstreams: {}", node.getId(), node.getUpstreamNode().getId(), node.getDownstreamNodes())
			assertEquals(0, node.getDownstreamNodes().size())

			' we're making sure downstream nodes were properly updated
			For Each i As val In ids
				Dim n As val = mesh.getNodeById(i)
				assertNotNull(n)
				' ensuring upstream was properly changed
				assertNotEquals(node.getId(), n.getUpstreamNode().getId())

				' ensuring new upstream is aware of new downstream
				assertTrue(n.getUpstreamNode().getDownstreamNodes().contains(n))
			Next i
		End Sub


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testEquality_1() throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Public Overridable Sub testEquality_1()
			Dim node1 As val = MeshOrganizer.Node.builder().id("192.168.0.1").port(38912).build()

			Dim node2 As val = MeshOrganizer.Node.builder().id("192.168.0.1").port(38912).build()

			Dim node3 As val = MeshOrganizer.Node.builder().id("192.168.0.1").port(38913).build()

			Dim node4 As val = MeshOrganizer.Node.builder().id("192.168.0.2").port(38912).build()

			assertEquals(node1, node2)

			assertNotEquals(node1, node3)
			assertNotEquals(node1, node4)
			assertNotEquals(node3, node4)
		End Sub


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testEquality_2() throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Public Overridable Sub testEquality_2()
			Dim node1 As val = MeshOrganizer.Node.builder().id("192.168.0.1").port(38912).build()

			Dim node2 As val = MeshOrganizer.Node.builder().id("192.168.0.1").port(38912).build()

			Dim node3 As val = MeshOrganizer.Node.builder().id("192.168.0.1").port(38912).build()

			node1.addDownstreamNode(MeshOrganizer.Node.builder().id("192.168.1.3").build()).addDownstreamNode(MeshOrganizer.Node.builder().id("192.168.1.4").build()).addDownstreamNode(MeshOrganizer.Node.builder().id("192.168.1.5").build())
			node2.addDownstreamNode(MeshOrganizer.Node.builder().id("192.168.1.3").build()).addDownstreamNode(MeshOrganizer.Node.builder().id("192.168.1.4").build()).addDownstreamNode(MeshOrganizer.Node.builder().id("192.168.1.5").build())
			node3.addDownstreamNode(MeshOrganizer.Node.builder().id("192.168.1.3").build()).addDownstreamNode(MeshOrganizer.Node.builder().id("192.168.1.4").build())

			assertEquals(node1, node2)
			assertNotEquals(node1, node3)
		End Sub


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testEquality_3() throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Public Overridable Sub testEquality_3()
			Dim mesh1 As val = New MeshOrganizer()
			Dim mesh2 As val = New MeshOrganizer()
			Dim mesh3 As val = New MeshOrganizer(MeshBuildMode.PLAIN)
			Dim mesh4 As val = New MeshOrganizer(MeshBuildMode.PLAIN)

			assertEquals(mesh1, mesh2)
			assertNotEquals(mesh1, mesh3)
			assertNotEquals(mesh1, mesh4)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testEquality_4() throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Public Overridable Sub testEquality_4()
			Dim mesh1 As val = New MeshOrganizer(MeshBuildMode.MESH)
			Dim mesh2 As val = New MeshOrganizer(MeshBuildMode.MESH)
			Dim mesh3 As val = New MeshOrganizer(MeshBuildMode.PLAIN)

			mesh1.addNode("192.168.1.1")
			mesh2.addNode("192.168.1.1")
			mesh3.addNode("192.168.1.1")

			assertEquals(mesh1, mesh2)
			assertNotEquals(mesh1, mesh3)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testClone_1() throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Public Overridable Sub testClone_1()
			Dim mesh1 As val = New MeshOrganizer(MeshBuildMode.MESH)

			For e As Integer = 0 To 8191
				mesh1.addNode(System.Guid.randomUUID().ToString())
			Next e

			Dim mesh2 As val = mesh1.clone()
			assertEquals(mesh1, mesh2)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testSerialization_1() throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Public Overridable Sub testSerialization_1()
			Dim mesh1 As val = New MeshOrganizer(MeshBuildMode.MESH)

			For e As Integer = 0 To 999
				mesh1.addNode(System.Guid.randomUUID().ToString())
			Next e


			Using baos As lombok.val = New MemoryStream(), 
				SerializationUtils.serialize(mesh1, baos)

				Using bais As lombok.val = New MemoryStream(baos.toByteArray())

					Dim mesh2 As MeshOrganizer = SerializationUtils.deserialize(bais)
					assertEquals(mesh1, mesh2)
				End Using
			End Using
		End Sub

	End Class
End Namespace
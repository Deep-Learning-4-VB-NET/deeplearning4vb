Imports System
Imports System.Collections.Generic
Imports System.Text
Imports Slf4j = lombok.extern.slf4j.Slf4j
Imports RandomUtils = org.apache.commons.lang3.RandomUtils
Imports org.junit.jupiter.api
Imports BaseND4JTest = org.nd4j.common.tests.BaseND4JTest
Imports NativeTag = org.nd4j.common.tests.tags.NativeTag
Imports TagNames = org.nd4j.common.tests.tags.TagNames
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

Namespace org.nd4j.parameterserver.distributed.util


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Slf4j @Disabled @Tag(TagNames.FILE_IO) @Tag(TagNames.DIST_SYSTEMS) @NativeTag public class NetworkOrganizerTest extends org.nd4j.common.tests.BaseND4JTest
	Public Class NetworkOrganizerTest
		Inherits BaseND4JTest

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @BeforeEach public void setUp() throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Public Overridable Sub setUp()

		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @AfterEach public void tearDown() throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Public Overridable Sub tearDown()

		End Sub



'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testSimpleSelection1() throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Public Overridable Sub testSimpleSelection1()
			Dim organizer As New NetworkOrganizer("127.0.0.0/24")
			Dim list As IList(Of String) = organizer.getSubset(1)

			assertEquals(1, list.Count)
			assertEquals("127.0.0.1", list(0))
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testSimpleSelection2() throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Public Overridable Sub testSimpleSelection2()
			Dim organizer As New NetworkOrganizer("127.0.0.0/24")
			Dim ip As String = organizer.MatchingAddress

			assertEquals("127.0.0.1", ip)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testSelectionUniformNetworkC1()
		Public Overridable Sub testSelectionUniformNetworkC1()
			Dim collection As IList(Of NetworkInformation) = New List(Of NetworkInformation)()

			For i As Integer = 1 To 127
				Dim information As New NetworkInformation()

				information.addIpAddress("192.168.0." & i)
				information.addIpAddress(RandomIp)

				collection.Add(information)
			Next i

			Dim discoverer As New NetworkOrganizer(collection, "192.168.0.0/24")

			' check for primary subset (aka Shards)
			Dim shards As IList(Of String) = discoverer.getSubset(10)

			assertEquals(10, shards.Count)

			For Each ip As String In shards
				assertNotEquals(Nothing, ip)
				assertTrue(ip.StartsWith("192.168.0", StringComparison.Ordinal))
			Next ip


			' check for secondary subset (aka Backup)
			Dim backup As IList(Of String) = discoverer.getSubset(10, shards)
			assertEquals(10, backup.Count)
			For Each ip As String In backup
				assertNotEquals(Nothing, ip)
				assertTrue(ip.StartsWith("192.168.0", StringComparison.Ordinal))
				assertFalse(shards.Contains(ip))
			Next ip
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testSelectionSingleBox1() throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Public Overridable Sub testSelectionSingleBox1()
			Dim collection As IList(Of NetworkInformation) = New List(Of NetworkInformation)()
			Dim information As New NetworkInformation()
			information.addIpAddress("192.168.21.12")
			information.addIpAddress("10.0.27.19")
			collection.Add(information)

			Dim organizer As New NetworkOrganizer(collection, "192.168.0.0/16")

			Dim shards As IList(Of String) = organizer.getSubset(10)
			assertEquals(1, shards.Count)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testSelectionSingleBox2() throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Public Overridable Sub testSelectionSingleBox2()
			Dim collection As IList(Of NetworkInformation) = New List(Of NetworkInformation)()
			Dim information As New NetworkInformation()
			information.addIpAddress("192.168.72.12")
			information.addIpAddress("10.2.88.19")
			collection.Add(information)

			Dim organizer As New NetworkOrganizer(collection)

			Dim shards As IList(Of String) = organizer.getSubset(10)
			assertEquals(1, shards.Count)
		End Sub


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testSelectionDisjointNetworkC1()
		Public Overridable Sub testSelectionDisjointNetworkC1()
			Dim collection As IList(Of NetworkInformation) = New List(Of NetworkInformation)()

			For i As Integer = 1 To 127
				Dim information As New NetworkInformation()

				If i < 20 Then
					information.addIpAddress("172.12.0." & i)
				End If

				information.addIpAddress(RandomIp)

				collection.Add(information)
			Next i

			Dim discoverer As New NetworkOrganizer(collection, "172.12.0.0/24")

			' check for primary subset (aka Shards)
			Dim shards As IList(Of String) = discoverer.getSubset(10)

			assertEquals(10, shards.Count)

			Dim backup As IList(Of String) = discoverer.getSubset(10, shards)

			' we expect 9 here, thus backups will be either incomplete or complex sharding will be used for them

			assertEquals(9, backup.Count)
			For Each ip As String In backup
				assertNotEquals(Nothing, ip)
				assertTrue(ip.StartsWith("172.12.0", StringComparison.Ordinal))
				assertFalse(shards.Contains(ip))
			Next ip
		End Sub


		''' <summary>
		''' In this test we'll check shards selection in "casual" AWS setup
		''' By default AWS box has only one IP from 172.16.0.0/12 space + local loopback IP, which isn't exposed
		''' </summary>
		''' <exception cref="Exception"> </exception>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testSelectionWithoutMaskB1() throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Public Overridable Sub testSelectionWithoutMaskB1()
			Dim collection As IList(Of NetworkInformation) = New List(Of NetworkInformation)()

			' we imitiate 512 cluster nodes here
			For i As Integer = 0 To 511
				Dim information As New NetworkInformation()

				information.addIpAddress(RandomAwsIp)
				collection.Add(information)
			Next i

			Dim organizer As New NetworkOrganizer(collection)

			Dim shards As IList(Of String) = organizer.getSubset(10)

			assertEquals(10, shards.Count)

			Dim backup As IList(Of String) = organizer.getSubset(10, shards)

			assertEquals(10, backup.Count)
			For Each ip As String In backup
				assertNotEquals(Nothing, ip)
				assertTrue(ip.StartsWith("172.", StringComparison.Ordinal))
				assertFalse(shards.Contains(ip))
			Next ip
		End Sub

		''' <summary>
		''' In this test we check for environment which has AWS-like setup:
		'''  1) Each box has IP address from 172.16.0.0/12 range
		'''  2) Within original homogenous network, we have 3 separate networks:
		'''      A) 192.168.0.X
		'''      B) 10.0.12.X
		'''      C) 10.172.12.X
		''' </summary>
		''' <exception cref="Exception"> </exception>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testSelectionWithoutMaskB2() throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Public Overridable Sub testSelectionWithoutMaskB2()
			Dim collection As IList(Of NetworkInformation) = New List(Of NetworkInformation)()

			' we imitiate 512 cluster nodes here
			For i As Integer = 0 To 511
				Dim information As New NetworkInformation()

				information.addIpAddress(RandomAwsIp)

				If i < 30 Then
					information.addIpAddress("192.168.0." & i)
				ElseIf i < 95 Then
					information.addIpAddress("10.0.12." & i)
				ElseIf i < 255 Then
					information.addIpAddress("10.172.12." & i)
				End If

				collection.Add(information)
			Next i

			Dim organizer As New NetworkOrganizer(collection)

			Dim shards As IList(Of String) = organizer.getSubset(15)

			assertEquals(15, shards.Count)

			For Each ip As String In shards
				assertNotEquals(Nothing, ip)
				assertTrue(ip.StartsWith("172.", StringComparison.Ordinal))
			Next ip

			Dim backup As IList(Of String) = organizer.getSubset(15, shards)

			For Each ip As String In backup
				assertNotEquals(Nothing, ip)
				assertTrue(ip.StartsWith("172.", StringComparison.Ordinal))
				assertFalse(shards.Contains(ip))
			Next ip

		End Sub


		''' <summary>
		''' Here we just check formatting for octets
		''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testFormat1() throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Public Overridable Sub testFormat1()
			For i As Integer = 0 To 255
				Dim octet As String = NetworkOrganizer.toBinaryOctet(i)
				assertEquals(8, octet.Length)
				log.trace("i: {}; Octet: {}", i, octet)
			Next i
		End Sub


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testFormat2() throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Public Overridable Sub testFormat2()
			For i As Integer = 0 To 999
				Dim octets As String = NetworkOrganizer.convertIpToOctets(RandomIp)

				' we just expect 8 bits per bloc, 4 blocks in total, plus 3 dots between blocks
				assertEquals(35, octets.Length)
			Next i
		End Sub


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testNetTree1() throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Public Overridable Sub testNetTree1()
			Dim ips As IList(Of String) = New List(Of String) From {"192.168.0.1", "192.168.0.2"}

			Dim tree As New NetworkOrganizer.VirtualTree()

			For Each ip As String In ips
				tree.map(NetworkOrganizer.convertIpToOctets(ip))
			Next ip

			assertEquals(2, tree.UniqueBranches)
			assertEquals(2, tree.TotalBranches)

			log.info("rewind: {}", tree.HottestNetwork)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testNetTree2() throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Public Overridable Sub testNetTree2()
			Dim ips As IList(Of String) = New List(Of String) From {"192.168.12.2", "192.168.0.2", "192.168.0.2", "192.168.62.92"}

			Dim tree As New NetworkOrganizer.VirtualTree()

			For Each ip As String In ips
				tree.map(NetworkOrganizer.convertIpToOctets(ip))
			Next ip

			assertEquals(3, tree.UniqueBranches)
			assertEquals(4, tree.TotalBranches)
		End Sub

		''' <summary>
		''' This test is just a naive test for counters
		''' </summary>
		''' <exception cref="Exception"> </exception>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testNetTree3() throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Public Overridable Sub testNetTree3()
			Dim ips As IList(Of String) = New List(Of String)()

			Dim tree As New NetworkOrganizer.VirtualTree()

			For i As Integer = 0 To 2999
				ips.Add(RandomIp)
			Next i


			For i As Integer = 0 To 19
				ips.Add("192.168.12." & i)
			Next i

			Collections.shuffle(ips)

			Dim uniqueIps As ISet(Of String) = New HashSet(Of String)(ips)

			For Each ip As String In uniqueIps
				tree.map(NetworkOrganizer.convertIpToOctets(ip))
			Next ip

			assertEquals(uniqueIps.Count, tree.TotalBranches)
			assertEquals(uniqueIps.Count, tree.UniqueBranches)

		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testNetTree4() throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Public Overridable Sub testNetTree4()
			Dim ips As IList(Of String) = New List(Of String) From {"192.168.12.2", "192.168.0.2", "192.168.0.2", "192.168.62.92", "5.3.4.5"}

			Dim tree As New NetworkOrganizer.VirtualTree()

			For Each ip As String In ips
				tree.map(NetworkOrganizer.convertIpToOctets(ip))
			Next ip

			assertEquals(4, tree.UniqueBranches)
			assertEquals(5, tree.TotalBranches)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testNetTree5() throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Public Overridable Sub testNetTree5()
			Dim ips As IList(Of String) = New List(Of String)()

			Dim tree As New NetworkOrganizer.VirtualTree()

			For i As Integer = 0 To 253
				ips.Add(RandomIp)
			Next i


			For i As Integer = 1 To 254
				ips.Add("192.168.12." & i)
			Next i

			Collections.shuffle(ips)

			Dim uniqueIps As ISet(Of String) = New HashSet(Of String)(ips)

			For Each ip As String In uniqueIps
				tree.map(NetworkOrganizer.convertIpToOctets(ip))
			Next ip

			assertEquals(508, uniqueIps.Count)

			assertEquals(uniqueIps.Count, tree.TotalBranches)
			assertEquals(uniqueIps.Count, tree.UniqueBranches)

			''' <summary>
			''' Now the most important part here. we should get 192.168.12. as the most "popular" branch
			''' </summary>

			Dim networkA As String = tree.HottestNetworkA

			assertEquals("11000000", networkA)

			Dim networkAB As String = tree.HottestNetworkAB

			'        assertEquals("11000000.10101000", networkAB);
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test @Disabled("AB 2019/05/30 - Intermittent issue or flaky test - see issue #7657") public void testNetTree6() throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Public Overridable Sub testNetTree6()
			Dim ips As IList(Of String) = New List(Of String)()

			Dim tree As New NetworkOrganizer.VirtualTree()

			For i As Integer = 0 To 253
				ips.Add(RandomIp)
			Next i


			For i As Integer = 1 To 254
				ips.Add(RandomAwsIp)
			Next i

			Collections.shuffle(ips)

			Dim uniqueIps As ISet(Of String) = New HashSet(Of String)(ips)

			For Each ip As String In uniqueIps
				tree.map(NetworkOrganizer.convertIpToOctets(ip))
			Next ip

			assertEquals(508, uniqueIps.Count)

			assertEquals(uniqueIps.Count, tree.TotalBranches)
			assertEquals(uniqueIps.Count, tree.UniqueBranches)

			''' <summary>
			''' Now the most important part here. we should get 192.168.12. as the most "popular" branch
			''' </summary>

			Dim networkA As String = tree.HottestNetworkA

			assertEquals("10101100", networkA)

			Dim networkAB As String = tree.HottestNetworkAB

			'  assertEquals("10101100.00010000", networkAB);
		End Sub

		Protected Friend Overridable ReadOnly Property RandomIp As String
			Get
				Dim builder As New StringBuilder()
    
				builder.Append(RandomUtils.nextInt(1, 172)).Append(".")
				builder.Append(RandomUtils.nextInt(0, 255)).Append(".")
				builder.Append(RandomUtils.nextInt(0, 255)).Append(".")
				builder.Append(RandomUtils.nextInt(1, 255))
    
				Return builder.ToString()
			End Get
		End Property

		Protected Friend Overridable ReadOnly Property RandomAwsIp As String
			Get
				Dim builder As New StringBuilder("172.")
    
				builder.Append(RandomUtils.nextInt(16, 32)).Append(".")
				builder.Append(RandomUtils.nextInt(0, 255)).Append(".")
				builder.Append(RandomUtils.nextInt(1, 255))
    
				Return builder.ToString()
			End Get
		End Property
	End Class

End Namespace
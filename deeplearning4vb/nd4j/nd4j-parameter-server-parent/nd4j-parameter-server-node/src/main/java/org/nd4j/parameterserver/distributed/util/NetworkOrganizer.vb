Imports System
Imports System.Collections.Generic
Imports System.Text
Imports NonNull = lombok.NonNull
Imports Slf4j = lombok.extern.slf4j.Slf4j
Imports SubnetUtils = org.apache.commons.net.util.SubnetUtils
Imports ND4JIllegalStateException = org.nd4j.linalg.exception.ND4JIllegalStateException

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
'ORIGINAL LINE: @Slf4j public class NetworkOrganizer
	Public Class NetworkOrganizer
		Protected Friend informationCollection As IList(Of NetworkInformation)
		Protected Friend networkMask As String
		Protected Friend tree As New VirtualTree()

		''' <summary>
		''' This constructor is NOT implemented yet
		''' </summary>
		''' <param name="infoSet"> </param>
		' TODO: implement this one properly, we should build mask out of list of Ips
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: protected NetworkOrganizer(@NonNull Collection<NetworkInformation> infoSet)
		Protected Friend Sub New(ByVal infoSet As ICollection(Of NetworkInformation))
			Me.New(infoSet, Nothing)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public NetworkOrganizer(@NonNull Collection<NetworkInformation> infoSet, String mask)
		Public Sub New(ByVal infoSet As ICollection(Of NetworkInformation), ByVal mask As String)
			informationCollection = New List(Of NetworkInformation)(infoSet)
			networkMask = mask
		End Sub


		''' <summary>
		''' This constructor builds format from own
		''' </summary>
		''' <param name="networkMask"> </param>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public NetworkOrganizer(@NonNull String networkMask)
		Public Sub New(ByVal networkMask As String)
			Me.informationCollection = buildLocalInformation()
			Me.networkMask = networkMask
		End Sub

		Protected Friend Overridable Function buildLocalInformation() As IList(Of NetworkInformation)
			Dim list As IList(Of NetworkInformation) = New List(Of NetworkInformation)()
			Dim netInfo As New NetworkInformation()
			Try
				Dim interfaces As IList(Of NetworkInterface) = Collections.list(NetworkInterface.getNetworkInterfaces())

				For Each networkInterface As NetworkInterface In interfaces
					If Not networkInterface.isUp() Then
						Continue For
					End If

					For Each address As InterfaceAddress In networkInterface.getInterfaceAddresses()
						Dim addr As String = address.getAddress().getHostAddress()

						If addr Is Nothing OrElse addr.Length = 0 OrElse addr.Contains(":") Then
							Continue For
						End If

						netInfo.getIpAddresses().add(addr)
					Next address
				Next networkInterface
				list.Add(netInfo)
			Catch e As Exception
				Throw New Exception(e)
			End Try

			Return list
		End Function

		''' <summary>
		''' This method returns local IP address that matches given network mask.
		''' To be used with single-argument constructor only.
		''' 
		''' @return
		''' </summary>
		Public Overridable ReadOnly Property MatchingAddress As String
			Get
				If informationCollection.Count > 1 Then
					Me.informationCollection = buildLocalInformation()
				End If
    
				Dim list As IList(Of String) = getSubset(1)
				If list.Count < 1 Then
					Throw New ND4JIllegalStateException("Unable to find network interface matching requested mask: " & networkMask)
				End If
    
				If list.Count > 1 Then
					log.warn("We have {} local IPs matching given netmask [{}]", list.Count, networkMask)
				End If
    
				Return list(0)
			End Get
		End Property

		''' <summary>
		''' This method returns specified number of IP addresses from original list of addresses
		''' </summary>
		''' <param name="numShards">
		''' @return </param>
		Public Overridable Function getSubset(ByVal numShards As Integer) As IList(Of String)
			Return getSubset(numShards, Nothing)
		End Function


		''' <summary>
		''' This method returns specified number of IP addresses from original list of addresses, that are NOT listen in primary collection
		''' </summary>
		''' <param name="numShards"> </param>
		''' <param name="primary"> Collection of IP addresses that shouldn't be in result
		''' @return </param>
		Public Overridable Function getSubset(ByVal numShards As Integer, ByVal primary As ICollection(Of String)) As IList(Of String)
			''' <summary>
			''' If netmask in unset, we'll use manual
			''' </summary>
			If networkMask Is Nothing Then
				Return getIntersections(numShards, primary)
			End If

			Dim addresses As IList(Of String) = New List(Of String)()

			Dim utils As New SubnetUtils(networkMask)

			Collections.shuffle(informationCollection)

			For Each information As NetworkInformation In informationCollection
				For Each ip As String In information.getIpAddresses()
					If primary IsNot Nothing AndAlso primary.Contains(ip) Then
						Continue For
					End If

					If utils.getInfo().isInRange(ip) Then
						log.debug("Picked {} as {}", ip,If(primary Is Nothing, "Shard", "Backup"))
						addresses.Add(ip)
					End If

					If addresses.Count >= numShards Then
						Exit For
					End If
				Next ip

				If addresses.Count >= numShards Then
					Exit For
				End If
			Next information

			Return addresses
		End Function

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: protected static String convertIpToOctets(@NonNull String ip)
		Protected Friend Shared Function convertIpToOctets(ByVal ip As String) As String
			Dim octets() As String = ip.Split("\.", True)
			If octets.Length <> 4 Then
				Throw New System.NotSupportedException()
			End If

			Dim builder As New StringBuilder()

			For i As Integer = 0 To 2
				builder.Append(toBinaryOctet(octets(i))).Append(".")
			Next i
			builder.Append(toBinaryOctet(octets(3)))

			Return builder.ToString()
		End Function

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: protected static String toBinaryOctet(@NonNull Integer value)
		Protected Friend Shared Function toBinaryOctet(ByVal value As Integer) As String
			If value < 0 OrElse value > 255 Then
				Throw New ND4JIllegalStateException("IP octets cant hold values below 0 or above 255")
			End If
			Dim octetBase As String = Integer.toBinaryString(value)
			Dim builder As New StringBuilder()
			For i As Integer = 0 To (8 - octetBase.Length) - 1
				builder.Append("0")
			Next i
			builder.Append(octetBase)

			Return builder.ToString()
		End Function

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: protected static String toBinaryOctet(@NonNull String value)
		Protected Friend Shared Function toBinaryOctet(ByVal value As String) As String
			Return toBinaryOctet(Integer.Parse(value))
		End Function

		''' <summary>
		''' This method returns specified numbers of IP's by parsing original list of trees into some form of binary tree
		''' </summary>
		''' <param name="numShards"> </param>
		''' <param name="primary">
		''' @return </param>
		Protected Friend Overridable Function getIntersections(ByVal numShards As Integer, ByVal primary As ICollection(Of String)) As IList(Of String)
			''' <summary>
			''' Since each ip address can be represented in 4-byte sequence, 1 byte per value, with leading order - we'll use that to build tree
			''' </summary>
			If primary Is Nothing Then
				For Each information As NetworkInformation In informationCollection
					For Each ip As String In information.getIpAddresses()
						' first we get binary representation for each IP
						Dim octet As String = convertIpToOctets(ip)

						' then we map each of them into virtual "tree", to find most popular networks within cluster
						tree.map(octet)
					Next ip
				Next information

				' we get most "popular" A network from tree now
				Dim octetA As String = tree.HottestNetworkA

				Dim candidates As IList(Of String) = New List(Of String)()

				Dim matchCount As New AtomicInteger(0)
				For Each node As NetworkInformation In informationCollection
					For Each ip As String In node.getIpAddresses()
						Dim octet As String = convertIpToOctets(ip)

						' calculating matches
						If octet.StartsWith(octetA, StringComparison.Ordinal) Then
							matchCount.incrementAndGet()
							candidates.Add(ip)
							Exit For
						End If
					Next ip
				Next node

				''' <summary>
				''' TODO: improve this. we just need to iterate over popular networks instead of single top A network
				''' </summary>
				If matchCount.get() <> informationCollection.Count Then
					Throw New ND4JIllegalStateException("Mismatching A class")
				End If

				Collections.shuffle(candidates)

				Return candidates.GetRange(0, Math.Min(numShards, candidates.Count))
			Else
				' if primary isn't null, we expect network to be already filtered
				Dim octetA As String = tree.HottestNetworkA

				Dim candidates As IList(Of String) = New List(Of String)()

				For Each node As NetworkInformation In informationCollection
					For Each ip As String In node.getIpAddresses()
						Dim octet As String = convertIpToOctets(ip)

						' calculating matches
						If octet.StartsWith(octetA, StringComparison.Ordinal) AndAlso Not primary.Contains(ip) Then
							candidates.Add(ip)
							Exit For
						End If
					Next ip
				Next node

				Collections.shuffle(candidates)

				Return candidates.GetRange(0, Math.Min(numShards, candidates.Count))
			End If
		End Function



		Public Class VirtualTree
			' funny but we'll have max of 2 sub-nodes on node
			Protected Friend nodes As IDictionary(Of Char, VirtualNode) = New Dictionary(Of Char, VirtualNode)()

			''' <summary>
			''' PLEASE NOTE: This method expects binary octets inside the string argument
			''' </summary>
			''' <param name="string"> </param>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public void map(@NonNull String string)
			Public Overridable Sub map(ByVal [string] As String)
				Dim chars() As String = [string].Split("", True)
				Dim ch As Char? = chars(0).Chars(0)

				If ch.Value <> "0"c AndAlso ch.Value <> "1"c Then
					Throw New ND4JIllegalStateException("VirtualTree expects binary octets as input")
				End If

				If Not nodes.ContainsKey(ch) Then
					nodes(ch) = New VirtualNode(ch, Nothing)
				End If

				nodes(ch).map(chars, 1)
			End Sub

			Public Overridable ReadOnly Property UniqueBranches As Integer
				Get
					Dim cnt As New AtomicInteger(nodes.Count)
					For Each node As VirtualNode In nodes.Values
						cnt.addAndGet(node.NumDivergents)
					Next node
					Return cnt.get()
				End Get
			End Property

			Public Overridable ReadOnly Property TotalBranches As Integer
				Get
					Dim cnt As New AtomicInteger(0)
					For Each node As VirtualNode In nodes.Values
						cnt.addAndGet(node.Counter)
					Next node
					Return cnt.get()
				End Get
			End Property

			Public Overridable ReadOnly Property HottestNetwork As String
				Get
					Dim max As Integer = 0
					Dim key As Char? = Nothing
					For Each node As VirtualNode In nodes.Values
						If node.Counter > max Then
							max = node.Counter
							key = node.ownChar
						End If
					Next node
					Dim topNode As VirtualNode = nodes(key).getHottestNode(max)
    
    
					Return topNode.rewind()
				End Get
			End Property

			Protected Friend Overridable ReadOnly Property HottestNode As VirtualNode
				Get
					Dim max As Integer = 0
					Dim key As Char? = Nothing
					For Each node As VirtualNode In nodes.Values
						If node.Counter > max Then
							max = node.Counter
							key = node.ownChar
						End If
					Next node
    
					Return nodes(key)
				End Get
			End Property

			Public Overridable ReadOnly Property HottestNetworkA As String
				Get
					Dim builder As New StringBuilder()
    
					Dim depth As Integer = 0
					Dim startingNode As VirtualNode = HottestNode
    
					If startingNode Is Nothing Then
						Throw New ND4JIllegalStateException("VirtualTree wasn't properly initialized, and doesn't have any information within")
					End If
    
					builder.Append(startingNode.ownChar)
    
					For i As Integer = 0 To 6
						startingNode = startingNode.HottestNode
						builder.Append(startingNode.ownChar)
					Next i
    
					Return builder.ToString()
				End Get
			End Property

			''' <summary>
			''' This method returns FULL A octet + B octet UP TO FIRST SIGNIFICANT BIT
			''' @return
			''' </summary>
			Public Overridable ReadOnly Property HottestNetworkAB As String
				Get
					Dim builder As New StringBuilder()
    
					Dim depth As Integer = 0
					Dim startingNode As VirtualNode = HottestNode
    
					If startingNode Is Nothing Then
						Throw New ND4JIllegalStateException("VirtualTree wasn't properly initialized, and doesn't have any information within")
					End If
    
					builder.Append(startingNode.ownChar)
    
					' building first octet
					For i As Integer = 0 To 6
						startingNode = startingNode.HottestNode
						builder.Append(startingNode.ownChar)
					Next i
    
					' adding dot after first octet
					startingNode = startingNode.HottestNode
					builder.Append(startingNode.ownChar)
    
					' building partial octet for subnet B
					''' <summary>
					''' basically we want widest possible match here
					''' </summary>
					For i As Integer = 0 To 7
    
					Next i
    
					Return builder.ToString()
				End Get
			End Property
		End Class


		Public Class VirtualNode
			Protected Friend nodes As IDictionary(Of Char, VirtualNode) = New Dictionary(Of Char, VirtualNode)()
			Protected Friend ReadOnly ownChar As Char?
'JAVA TO VB CONVERTER NOTE: The field counter was renamed since Visual Basic does not allow fields to have the same name as other class members:
			Protected Friend counter_Conflict As Integer = 0
			Protected Friend parentNode As VirtualNode

			Public Sub New(ByVal character As Char?, ByVal parentNode As VirtualNode)
				Me.ownChar = character
				Me.parentNode = parentNode
			End Sub

			Public Overridable Sub map(ByVal chars() As String, ByVal position As Integer)
				counter_Conflict += 1
				If position < chars.Length Then
					Dim ch As Char? = chars(position).Chars(0)
					If Not nodes.ContainsKey(ch) Then
						nodes(ch) = New VirtualNode(ch, Me)
					End If

					nodes(ch).map(chars, position + 1)
				End If
			End Sub

			Protected Friend Overridable ReadOnly Property NumDivergents As Integer
				Get
					If nodes.Count = 0 Then
						Return 0
					End If
    
					Dim cnt As New AtomicInteger(nodes.Count - 1)
					For Each node As VirtualNode In nodes.Values
						cnt.addAndGet(node.NumDivergents)
					Next node
					Return cnt.get()
				End Get
			End Property


			Protected Friend Overridable ReadOnly Property DiscriminatedCount As Integer
				Get
					If nodes.Count = 0 AndAlso counter_Conflict = 1 Then
						Return 0
					End If
    
					Dim cnt As New AtomicInteger(Math.Max(0, counter_Conflict - 1))
					For Each node As VirtualNode In nodes.Values
						cnt.addAndGet(node.DiscriminatedCount)
					Next node
					Return cnt.get()
				End Get
			End Property

			Protected Friend Overridable ReadOnly Property Counter As Integer
				Get
					Return counter_Conflict
				End Get
			End Property

			''' <summary>
			''' This method returns most popular sub-node
			''' @return
			''' </summary>
			Protected Friend Overridable Function getHottestNode(ByVal threshold As Integer) As VirtualNode
				For Each node As VirtualNode In nodes.Values
					If node.Counter >= threshold Then
						Return node.getHottestNode(threshold)
					End If
				Next node

				Return Me
			End Function

			Protected Friend Overridable ReadOnly Property HottestNode As VirtualNode
				Get
					Dim max As Integer = 0
					Dim ch As Char? = Nothing
					For Each node As VirtualNode In nodes.Values
						If node.Counter > max Then
							ch = node.ownChar
							max = node.Counter
						End If
					Next node
    
					Return nodes(ch)
				End Get
			End Property

			Protected Friend Overridable Function rewind() As String
				Dim builder As New StringBuilder()

				Dim lastNode As VirtualNode = Me
				lastNode = lastNode.parentNode
'JAVA TO VB CONVERTER WARNING: An assignment within expression was extracted from the following statement:
'ORIGINAL LINE: while ((lastNode = lastNode.parentNode) != null)
				Do While lastNode IsNot Nothing
					builder.Append(lastNode.ownChar)
						lastNode = lastNode.parentNode
				Loop

'JAVA TO VB CONVERTER TODO TASK: There is no .NET StringBuilder equivalent to the Java 'reverse' method:
				Return builder.reverse().ToString()
			End Function
		End Class
	End Class

End Namespace
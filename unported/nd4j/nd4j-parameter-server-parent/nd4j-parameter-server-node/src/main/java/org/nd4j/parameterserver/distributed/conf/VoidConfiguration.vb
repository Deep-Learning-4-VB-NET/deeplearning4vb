Imports System
Imports System.Collections.Generic
Imports lombok
Imports Slf4j = lombok.extern.slf4j.Slf4j
Imports ND4JIllegalStateException = org.nd4j.linalg.exception.ND4JIllegalStateException
Imports ExecutionMode = org.nd4j.parameterserver.distributed.enums.ExecutionMode
Imports FaultToleranceStrategy = org.nd4j.parameterserver.distributed.enums.FaultToleranceStrategy
Imports NodeRole = org.nd4j.parameterserver.distributed.enums.NodeRole
Imports TransportType = org.nd4j.parameterserver.distributed.enums.TransportType
Imports MeshBuildMode = org.nd4j.parameterserver.distributed.v2.enums.MeshBuildMode
Imports PortSupplier = org.nd4j.parameterserver.distributed.v2.transport.PortSupplier
Imports StaticPortSupplier = org.nd4j.parameterserver.distributed.v2.transport.impl.StaticPortSupplier

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

Namespace org.nd4j.parameterserver.distributed.conf


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @NoArgsConstructor @AllArgsConstructor @Builder @Slf4j @Data public class VoidConfiguration implements java.io.Serializable
	<Serializable>
	Public Class VoidConfiguration
		Public Const DEFAULT_AERON_UDP_PORT As Integer = 49876

		''' <summary>
		''' StreamId is used for Aeron configuration
		''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Builder.@Default private int streamId = -1;
'JAVA TO VB CONVERTER NOTE: The field streamId was renamed since Visual Basic does not allow fields to have the same name as other class members:
		Private streamId_Conflict As Integer = -1

		''' <summary>
		''' This variable defines UDP port that will be used for communication with cluster driver.<br>
		''' NOTE: Use <seealso cref="setPortSupplier(PortSupplier)"/> to set the port to use - the value of the unicastControllerPort
		''' field will automatically be updated to the value set by the PortSupplier provided on the master/controller machine,
		''' before communicating to the worker machines. Setting this field directly will NOT control the port that is
		''' used.
		''' 
		''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Builder.@Default private int unicastControllerPort = 49876;
		Private unicastControllerPort As Integer = 49876

		''' <summary>
		''' This method specifies UDP port for multicast/broadcast transport
		''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Builder.@Default private int multicastPort = 59876;
		Private multicastPort As Integer = 59876

		''' <summary>
		''' This method defines number of shards. Reserved for future use.
		''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Builder.@Default private int numberOfShards = 1;
		Private numberOfShards As Integer = 1

		''' <summary>
		''' Reserved for future use.
		''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Builder.@Default private org.nd4j.parameterserver.distributed.enums.FaultToleranceStrategy faultToleranceStrategy = org.nd4j.parameterserver.distributed.enums.FaultToleranceStrategy.NONE;
		Private faultToleranceStrategy As FaultToleranceStrategy = FaultToleranceStrategy.NONE

		''' <summary>
		''' Reserved for future use.
		''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Builder.@Default private org.nd4j.parameterserver.distributed.enums.ExecutionMode executionMode = org.nd4j.parameterserver.distributed.enums.ExecutionMode.SHARDED;
'JAVA TO VB CONVERTER NOTE: The field executionMode was renamed since Visual Basic does not allow fields to have the same name as other class members:
		Private executionMode_Conflict As ExecutionMode = ExecutionMode.SHARDED

		''' <summary>
		''' Reserved for future use.
		''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Builder.@Default private java.util.List<String> shardAddresses = new java.util.ArrayList<>();
'JAVA TO VB CONVERTER NOTE: The field shardAddresses was renamed since Visual Basic does not allow fields to have the same name as other class members:
		Private shardAddresses_Conflict As IList(Of String) = New List(Of String)()

		''' <summary>
		''' Reserved for future use.
		''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Builder.@Default private java.util.List<String> backupAddresses = new java.util.ArrayList<>();
'JAVA TO VB CONVERTER NOTE: The field backupAddresses was renamed since Visual Basic does not allow fields to have the same name as other class members:
		Private backupAddresses_Conflict As IList(Of String) = New List(Of String)()

		''' <summary>
		''' This variable defines network transport to be used for comms
		''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Builder.@Default private org.nd4j.parameterserver.distributed.enums.TransportType transportType = org.nd4j.parameterserver.distributed.enums.TransportType.ROUTED_UDP;
		Private transportType As TransportType = TransportType.ROUTED_UDP

		''' <summary>
		''' This variable defines how cluster nodes are organized
		''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Builder.@Default private org.nd4j.parameterserver.distributed.v2.enums.MeshBuildMode meshBuildMode = org.nd4j.parameterserver.distributed.v2.enums.MeshBuildMode.PLAIN;
		Private meshBuildMode As MeshBuildMode = MeshBuildMode.PLAIN

		''' <summary>
		''' This variable acts as hint for ParameterServer about IP address to be used for comms.
		''' Used only if SPARK_PUBLIC_DNS is undefined (i.e. as in YARN environment)
		''' </summary>
'JAVA TO VB CONVERTER NOTE: The field networkMask was renamed since Visual Basic does not allow fields to have the same name as other class members:
		Private networkMask_Conflict As String

		''' <summary>
		''' This value is optional, and has effect only for UDP MulticastTransport
		''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Builder.@Default private String multicastNetwork = "224.0.1.1";
		Private multicastNetwork As String = "224.0.1.1"

		''' <summary>
		''' This value is optional, and has effect only for UDP MulticastTransport
		''' </summary>
		Private multicastInterface As String

		''' <summary>
		''' This value is optional, and has effect only for UDP MulticastTransport
		''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Builder.@Default private int ttl = 4;
		Private ttl As Integer = 4

		''' <summary>
		''' This option is for debugging mostly. Do not use it, unless you have to.
		''' </summary>
		Protected Friend forcedRole As NodeRole

		' FIXME: probably worth moving somewhere else
		''' <summary>
		''' This value is optional, and has effect only for UDP MulticastTransport
		''' </summary>
		<Obsolete>
		Private useHS As Boolean = True
		''' <summary>
		''' This value is optional, and has effect only for UDP MulticastTransport
		''' </summary>
		<Obsolete>
		Private useNS As Boolean = False

		''' <summary>
		''' This variable defines, how long transport should wait before resending message in case of network issues.
		''' Measured in milliseconds.
		''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Builder.@Default private long retransmitTimeout = 1000;
		Private retransmitTimeout As Long = 1000

		''' <summary>
		''' This variable defines, how long transport should wait for response on specific messages.
		''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Builder.@Default private long responseTimeframe = 500;
		Private responseTimeframe As Long = 500

		''' <summary>
		''' This variable defines, how long transport should wait for answer on specific messages.
		''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Builder.@Default private long responseTimeout = 30000;
		Private responseTimeout As Long = 30000

		''' <summary>
		''' This variable defines amount of memory used of
		''' Default value: 1GB
		''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Builder.@Default private long chunksBufferSize = 1073741824;
		Private chunksBufferSize As Long = 1073741824

		''' <summary>
		''' This variable defines max chunk size for INDArray splits
		''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Builder.@Default private int maxChunkSize = 65536;
		Private maxChunkSize As Integer = 65536

		''' <summary>
		''' This variable defines max number of allowed reconnects per node
		''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Builder.@Default private int maxFailuresPerNode = 3;
		Private maxFailuresPerNode As Integer = 3

		''' <summary>
		''' This optional variable defines IP address of the box which acts as master for gradients training.
		''' Leave it null, and Spark Master node will be used as Master for parameter server as well.
		''' </summary>
		Private controllerAddress As String

		''' <summary>
		''' The port supplier used to provide the networking port to use for communication to/from the driver (UDP via Aeron).
		''' This setting can be used to control how ports get assigned (including port setting different ports on different machines).<br>
		''' See <seealso cref="PortSupplier"/> for further details.<br>
		''' Default: static port (i.e., <seealso cref="StaticPortSupplier"/>) with value <seealso cref="DEFAULT_AERON_UDP_PORT"/>
		''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Builder.@Default private org.nd4j.parameterserver.distributed.v2.transport.PortSupplier portSupplier = new org.nd4j.parameterserver.distributed.v2.transport.impl.StaticPortSupplier(49876);
		Private portSupplier As PortSupplier = New StaticPortSupplier(49876)

		Public Overridable WriteOnly Property StreamId As Integer
			Set(ByVal streamId As Integer)
				If streamId < 1 Then
					Throw New ND4JIllegalStateException("You can't use streamId 0, please specify other one")
				End If
    
				Me.streamId_Conflict = streamId
			End Set
		End Property

		Protected Friend Overridable Sub validateNetmask()
			If networkMask_Conflict Is Nothing Then
				Return
			End If

			' micro-validaiton here
			Dim chunks() As String = networkMask_Conflict.Split("\.", True)
			If chunks.Length = 1 OrElse networkMask_Conflict.Length = 0 Then
				Throw New ND4JIllegalStateException("Provided netmask doesn't look like a legit one. Proper format is: 192.168.1.0/24 or 10.0.0.0/8")
			End If


			' TODO: add support for IPv6 eventually here
			If chunks.Length <> 4 Then
				Throw New ND4JIllegalStateException("4 octets expected here for network mask")
			End If

			For i As Integer = 0 To 2
				Dim curr As String = chunks(i)
				Try
					Dim conv As Integer = Convert.ToInt32(curr)
					If conv < 0 OrElse conv > 255 Then
						Throw New ND4JIllegalStateException()
					End If
				Catch e As Exception
					Throw New ND4JIllegalStateException("All IP address octets should be in range of 0...255")
				End Try
			Next i

			If Convert.ToInt32(chunks(0)) = 0 Then
				Throw New ND4JIllegalStateException("First network mask octet should be non-zero. I.e. 10.0.0.0/8")
			End If

			' we enforce last octet to be 0/24 always
			If Not networkMask_Conflict.Contains("/") OrElse Not chunks(3).StartsWith("0", StringComparison.Ordinal) Then
				chunks(3) = "0/24"
			End If

			Me.networkMask_Conflict = chunks(0) & "." & chunks(1) & "." & chunks(2) & "." & chunks(3)
		End Sub

		''' <summary>
		''' This option is very important: in shared network environment and yarn (like on EC2 etc),
		''' please set this to the network, which will be available on all boxes. I.e. 10.1.1.0/24 or 192.168.0.0/16
		''' </summary>
		''' <param name="netmask"> netmask to be used for IP address selection </param>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public void setNetworkMask(@NonNull String netmask)
		Public Overridable Property NetworkMask As String
			Set(ByVal netmask As String)
				Me.networkMask_Conflict = netmask
				validateNetmask()
			End Set
			Get
				validateNetmask()
				Return Me.networkMask_Conflict
			End Get
		End Property


		Public Overridable WriteOnly Property ShardAddresses As IList(Of String)
			Set(ByVal addresses As IList(Of String))
				Me.shardAddresses_Conflict = addresses
			End Set
		End Property

		Public Overridable WriteOnly Property ShardAddresses As String()
			Set(ByVal ips() As String)
				If shardAddresses_Conflict Is Nothing Then
					shardAddresses_Conflict = New List(Of String)()
				End If
    
				For Each ip As String In ips
					If ip IsNot Nothing Then
						shardAddresses_Conflict.Add(ip)
					End If
				Next ip
			End Set
		End Property

		Public Overridable WriteOnly Property BackupAddresses As IList(Of String)
			Set(ByVal addresses As IList(Of String))
				Me.backupAddresses_Conflict = addresses
			End Set
		End Property

		Public Overridable WriteOnly Property BackupAddresses As String()
			Set(ByVal ips() As String)
				If backupAddresses_Conflict Is Nothing Then
					backupAddresses_Conflict = New List(Of String)()
				End If
    
				For Each ip As String In ips
					If ip IsNot Nothing Then
						backupAddresses_Conflict.Add(ip)
					End If
				Next ip
			End Set
		End Property

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public void setExecutionMode(@NonNull ExecutionMode executionMode)
		Public Overridable WriteOnly Property ExecutionMode As ExecutionMode
			Set(ByVal executionMode As ExecutionMode)
				Me.executionMode_Conflict = executionMode
			End Set
		End Property

		'Partial implementation to hideh/exclude unicastControllerPort from builder - users should use portSupplier method instead
		' in the builder - otherwise, users might think they can set it via this method (instead, it gets overriden by whatever
		' is provided by the port supplier)
		'Also hide from builder some unsupported methods
		Public Class VoidConfigurationBuilder

			''' <summary>
			''' Equivalent to calling <seealso cref="portSupplier(PortSupplier)"/> with a <seealso cref="StaticPortSupplier"/> </summary>
			''' <param name="unicastPort"> Port to use for </param>
			''' <seealso cref= #portSupplier(PortSupplier) </seealso>
'JAVA TO VB CONVERTER NOTE: The parameter unicastPort was renamed since Visual Basic will not allow parameters with the same name as their enclosing function or property:
			Public Overridable Function unicastPort(ByVal unicastPort_Conflict As Integer) As VoidConfigurationBuilder
				Return portSupplier(New StaticPortSupplier(unicastPort_Conflict))
			End Function

'JAVA TO VB CONVERTER NOTE: The parameter unicastControllerPort was renamed since Visual Basic will not allow parameters with the same name as their enclosing function or property:
			Friend Overridable Function unicastControllerPort(ByVal unicastControllerPort_Conflict As Integer) As VoidConfigurationBuilder
				Throw New System.NotSupportedException("Not supported. Use portSupplier method instead")
			End Function

'JAVA TO VB CONVERTER NOTE: The parameter faultToleranceStrategy was renamed since Visual Basic will not allow parameters with the same name as their enclosing function or property:
			Friend Overridable Function faultToleranceStrategy(ByVal faultToleranceStrategy_Conflict As FaultToleranceStrategy) As VoidConfigurationBuilder
				Throw New System.NotSupportedException("Reserved for future use")
			End Function

'JAVA TO VB CONVERTER NOTE: The parameter backupAddresses was renamed since Visual Basic will not allow parameters with the same name as their enclosing function or property:
			Public Overridable Function backupAddresses(ByVal backupAddresses_Conflict As IList(Of String)) As VoidConfigurationBuilder
				Throw New System.NotSupportedException("Reserved for future use")
			End Function

		End Class
	End Class

End Namespace